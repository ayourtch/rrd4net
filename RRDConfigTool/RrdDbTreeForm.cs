using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Castle.MicroKernel;
using WeifenLuo.WinFormsUI.Docking;
using RRDConfigTool.Controllers;
using RRDConfigTool.Data;
using rrd4n.Core;
using rrd4n.DataAccess.Data;

namespace RRDConfigTool
{
   public partial class RrdDbTreeForm : DockContent
   {

      private readonly IKernel kernel;

      private ViewController controller = null;
      const string dataSourceNodesName = "Datasources";
      const string archiveNodesName = "Archives";

      private enum SelectedNodeType
      {
         unknown,
         database,
         archives,
         archive,
         datasources,
         datasource,
         datasourcearchive
      }

      public RrdDbTreeForm()
      {
         InitializeComponent();
      }

      public RrdDbTreeForm(IKernel kernel)
         : this()
      {
         this.kernel = kernel;
      }

      public void RegisterController(ViewController viewController)
      {
         this.controller = viewController;
      }

      private void LoadTree(DatabaseData databaseData)
      {
         RrdDef databaseDefinition = databaseData.Definition;
         string databaseName = Path.GetFileNameWithoutExtension(databaseDefinition.Path);
         TreeNode databaseNode;
         TreeNode[] databaseNodes = rrdDbTreeView.Nodes.Find(databaseDefinition.Path, true);
         if (databaseNodes.Length == 0)
         {
            databaseNode = rrdDbTreeView.Nodes.Add(databaseDefinition.Path, databaseName);
            if (!databaseData.Saved)
               databaseNode.Text += "*";
         }
         else
         {
            databaseNode = databaseNodes[0];
            if (!databaseData.Saved
               && !databaseNode.Text.Contains("*"))
               databaseNode.Text += "*";
         }


         databaseNode.Tag = databaseData;

         databaseNode.Nodes.Clear();
         var datasources = databaseNode.Nodes.Add(dataSourceNodesName, dataSourceNodesName);
         foreach (var datasource in databaseDefinition.getDsDefs())
         {
            TreeNode datasourceNode = datasources.Nodes.Add(datasource.DsName);
            datasourceNode.Tag = datasource;
            foreach (var arcDef in databaseDefinition.getArcDefs())
            {
               string nodeText = string.Format("RRA:{0}:{1}:{2}:{3}", arcDef.getConsolFun().Name,
                                                arcDef.Xff, arcDef.Steps, arcDef.Rows);
               var archiveNode = datasourceNode.Nodes.Add(nodeText);
               archiveNode.Tag = arcDef;
            }
         }
         databaseNode.Nodes.Add(archiveNodesName, archiveNodesName);
      }
      private void rrdDbTreeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         //importDataToolStripMenuItem1.Enabled = false;
         //editToolStripMenuItem1.Enabled = false;
         object selectedObject;
         SelectedNodeType selectedNodeType = GetSelectedNodeObject(e.Node, out selectedObject);

         switch (selectedNodeType)
         {
            case SelectedNodeType.database:
               controller.DatabaseSelected((DatabaseData)selectedObject);
               return;
            case SelectedNodeType.datasources:
               controller.DatasourcesSelected((DatabaseData)selectedObject);
               return;
            case SelectedNodeType.archives:
               selectedObject = e.Node.Parent.Tag;
               controller.ArchivesSelected(selectedObject as DatabaseData);
               return;
            case SelectedNodeType.datasource:
               return;
            case SelectedNodeType.datasourcearchive:
               Application.UseWaitCursor = true;
               DatabaseData databaseData = e.Node.Parent.Parent.Parent.Tag as DatabaseData;
               controller.DatasourceArchiveSelected(databaseData, e.Node.Parent.Tag, e.Node.Tag);
               Application.UseWaitCursor = false;
               return;
         }
      }
      private SelectedNodeType GetSelectedNodeObject(out object selectedObject)
      {
         return GetSelectedNodeObject(rrdDbTreeView.SelectedNode, out selectedObject);
      }

      private SelectedNodeType GetSelectedNodeObject(TreeNode selectedNode, out object selectedObject)
      {
         selectedObject = null;
         if (selectedNode == null)
            return SelectedNodeType.unknown;

         selectedObject = selectedNode.Tag;
         if (selectedNode.Parent == null)
            return SelectedNodeType.database;
         if (selectedNode.Text.CompareTo(dataSourceNodesName) == 0)
         {
            selectedObject = selectedNode.Parent.Tag;
            return SelectedNodeType.datasources;
         }
         if (selectedNode.Text.CompareTo(archiveNodesName) == 0)
            return SelectedNodeType.archives;
         if (selectedNode.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return SelectedNodeType.datasource;
         if (selectedNode.Parent.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return SelectedNodeType.datasourcearchive;
         return SelectedNodeType.unknown;
      }

      public DatabaseData GetSrcDatabase()
      {
         TreeNode selectedNode = rrdDbTreeView.SelectedNode;
         if (selectedNode.Parent == null)
            return null;
         if (selectedNode.Text.CompareTo(dataSourceNodesName) == 0)
         {
            return selectedNode.Parent.Tag as DatabaseData;
         }
         if (selectedNode.Text.CompareTo(archiveNodesName) == 0)
            return selectedNode.Parent.Tag as DatabaseData;
         if (selectedNode.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return selectedNode.Parent.Parent.Tag as DatabaseData;
         if (selectedNode.Parent.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return selectedNode.Parent.Parent.Tag as DatabaseData;
         return null;
      }


      public void SetDatabaseDefinition(DatabaseData databaseData)
      {
         LoadTree(databaseData);
      }

      public void RemoveDatabaseDefinition(string databasePath)
      {
         rrdDbTreeView.Nodes.RemoveByKey(databasePath);
      }

      public void SetEditMode(bool editing)
      {
         saveToolStripButton.Enabled = editing;
         //createNewToolStripButton.Enabled = !editing;
         //removeToolStripButton.Enabled = !editing;
         //editToolStripMenuItem.Enabled = !editing;
         //addDataSourceToolStripMenuItem.Enabled = editing;
         //addArchiveToolStripMenuItem.Enabled = editing;
      }

      public void ClearViews()
      {
         rrdDbTreeView.Nodes.Clear();
      }

      public string SaveDataBase(string databasePath)
      {
         if (MessageBox.Show(this, "Database not saved! \r Save changes?", "RRD Config tool", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return string.Empty;
         saveFileDialog1.CheckFileExists = false;
         saveFileDialog1.AddExtension = true;
         saveFileDialog1.DefaultExt = "rra";
         saveFileDialog1.FileName = databasePath;
         if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            return string.Empty;
         return saveFileDialog1.FileName;
      }

      private void openToolStripButton_Click(object sender, EventArgs e)
      {
         openFileDialog1.DefaultExt = "rra";
         openFileDialog1.AddExtension = true;
         openFileDialog1.CheckFileExists = true;
         openFileDialog1.FileName = "";
         openFileDialog1.ReadOnlyChecked = true;
         openFileDialog1.ShowReadOnly = true;
         openFileDialog1.FileName = "*.rra";
         if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            return;
         if (!controller.OpenDatabase(openFileDialog1.FileName))
            return;
      }

      private void createNewToolStripButton_Click(object sender, EventArgs e)
      {
         if (!CheckIfUnsaved())
            return;

         var createDatabaseController = kernel[typeof(CreateDatabaseController)] as CreateDatabaseController;
         if (createDatabaseController.Run())
            controller.DatabaseCreated();
      }

      private bool NeedToSaveData()
      {
         return controller.DatabaseUnsaved();
      }

      private bool CheckIfUnsaved()
      {
         if (controller.DatabaseUnsaved())
         {
            DialogResult res = MessageBox.Show(this, "Database not saved!\nSave before continue?\n", "Unsaved data", MessageBoxButtons.YesNoCancel);
            if (res == System.Windows.Forms.DialogResult.Cancel)
               return false;

            if (res == DialogResult.No)

               if (res == DialogResult.Yes)
               {
                  saveFileDialog1.CheckFileExists = false;
                  if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                     return false;
                  try
                  {
                     controller.CreateDatabase(saveFileDialog1.FileName);
                     return true;
                  }
                  catch (ArgumentException ex)
                  {
                     MessageBox.Show(ex.Message, "Fail to save database");
                     return false;
                  }
               }
               else
                  return false;
         }
         return true;
      }

      private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
      {
         object selectedObject = new object();
         SelectedNodeType nodeType = GetSelectedNodeObject(rrdDbTreeView.SelectedNode, out selectedObject);
         if (nodeType == SelectedNodeType.database)
            copyDatabasedefToClipboardToolStripMenuItem.Enabled = true;
         else
            copyDatabasedefToClipboardToolStripMenuItem.Enabled = false;
      }

      private void copyDatabasedefToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object selectedObject;
         SelectedNodeType nodeType = GetSelectedNodeObject(rrdDbTreeView.SelectedNode, out selectedObject);

         Clipboard.SetText(controller.DumpDatabaseDefinition(selectedObject as DatabaseData));
      }

      private void saveToolStripButton_Click(object sender, EventArgs e)
      {
         saveFileDialog1.CheckFileExists = false;
         saveFileDialog1.FileName = controller.GetDatabaseDefinitionPath();
         saveFileDialog1.DefaultExt = "rra";
         saveFileDialog1.Filter = "(*.rra)|*.rra|(All files)|*.*";
         saveFileDialog1.AddExtension = true;
         if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            return;
         try
         {
            controller.CreateDatabase(saveFileDialog1.FileName);
         }
         catch (ArgumentException ex)
         {
            MessageBox.Show(ex.Message, "Fail to save database");
            return;
         }

      }

      private void removeToolStripButton_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, "Do you really want to remove database definition?", "remove definition", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            return;
         controller.DeleteDatabaseDefinition();
      }


      private void addDataSourceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DataSourceController dsController = kernel[typeof(DataSourceController)] as DataSourceController;
         dsController.DataSourceDef = null;
         if (dsController.Run())
         {
            controller.DatabaseUpdated();
            return;
         }

      }

      private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object nodeData;

         SelectedNodeType nodeType = GetSelectedNodeObject(out nodeData);
         if (nodeType == SelectedNodeType.datasource)
         {
            DataSourceController dsController = kernel[typeof(DataSourceController)] as DataSourceController;
            dsController.DataSourceDef = nodeData as DsDef;
            DatabaseData srcDatabaseData = GetSrcDatabase();
            controller.StartEditDatabase(srcDatabaseData);
            dsController.SrcDatabase = srcDatabaseData;
            if (dsController.Run())
               controller.DatabaseUpdated(srcDatabaseData);
            return;

         }
         if (nodeType == SelectedNodeType.datasourcearchive)
         {
            ArchiveController archController = kernel[typeof(ArchiveController)] as ArchiveController;
            archController.ArchiveDef = nodeData as ArcDef;
            if (archController.Run())
               controller.DatabaseUpdated(GetSrcDatabase());
            return;
         }

         if (nodeType == SelectedNodeType.database)
         {
            if (!CheckIfUnsaved())
               return;
            controller.StartEditDatabase(nodeData as DatabaseData);
         }
      }

      private void addArchiveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ArchiveController arController = kernel[typeof(ArchiveController)] as ArchiveController;
         arController.ArchiveDef = null;
         arController.Run();
         controller.DatabaseUpdated();
         //controller.ArchivesSelected();
      }

      private void removeFromProjectToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object nodeData;
         SelectedNodeType nodeType = GetSelectedNodeObject(out nodeData);
         if (nodeType == SelectedNodeType.database)
         {
            if (controller.DatabaseUnsaved(nodeData as DatabaseData))
            {
               DialogResult res = MessageBox.Show(this, "Database not saved!\nSave before continue?\n", "Unsaved data", MessageBoxButtons.YesNoCancel);
               if (res == System.Windows.Forms.DialogResult.Cancel)
                  return;

               if (res == DialogResult.Yes)
               {
                  saveFileDialog1.CheckFileExists = false;
                  if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                     return;
                  try
                  {
                     controller.CreateDatabase(saveFileDialog1.FileName);
                  }
                  catch (ArgumentException ex)
                  {
                     MessageBox.Show(ex.Message, "Fail to save database");
                  }
               }
            }
            controller.RemoveDatabase(nodeData as DatabaseData);
         }
      }

      private void newToolStripButton_Click(object sender, EventArgs e)
      {
         if (!CheckIfUnsaved())
            return;
         NewRrdDbForm frm = new NewRrdDbForm();
         frm.Controller = controller;
         if (frm.ShowDialog(this) == DialogResult.Cancel)
            return;
      }

      private void rrdDbTreeView_ItemDrag(object sender, ItemDragEventArgs e)
      {
         var treeNode = e.Item as TreeNode;
         if (treeNode == null)
            return;
         if (treeNode.Parent == null)
         {
            var rrdDef = treeNode.Tag as DatabaseData;
            if (rrdDef == null)
               return;
            DoDragDrop(rrdDef, DragDropEffects.Link);
         }
      }

      private void importDataToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object nodeData;

         SelectedNodeType nodeType = GetSelectedNodeObject(out nodeData);
         //if (nodeType == SelectedNodeType.datasource)
         //{
         //   DataSourceController dsController = kernel[typeof(DataSourceController)] as DataSourceController;
         //   dsController.DataSourceDef = nodeData as DsDef;
         //   DatabaseData srcDatabaseData = GetSrcDatabase();
         //   dsController.SrcDatabase = srcDatabaseData;
         //   if (dsController.Run())
         //      controller.DatabaseUpdated(srcDatabaseData);
         //   return;

         //}
         //if (nodeType == SelectedNodeType.datasourcearchive)
         //{
         //   ArchiveController archController = kernel[typeof(ArchiveController)] as ArchiveController;
         //   archController.ArchiveDef = nodeData as ArcDef;
         //   if (archController.Run())
         //      controller.DatabaseUpdated(GetSrcDatabase());
         //   return;
         //}

         if (nodeType == SelectedNodeType.database)
         {
            openFileDialog1.DefaultExt = "csv";
            openFileDialog1.FileName = "*.csv";
            //            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
               return;

            try
            {
               DatabaseData dd = nodeData as DatabaseData;
               controller.ImportData(openFileDialog1.FileName, dd, new TimeSpan(7, 0, 0, 0));
            }
            catch (ApplicationException ex)
            {
               MessageBox.Show("Fail to import data." + ex.Message);
            }
         }
      }

      private void exportToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object nodeData;
         SelectedNodeType nodeType = GetSelectedNodeObject(out nodeData);
         if (nodeType != SelectedNodeType.database)
            return;

         DatabaseData databaseData = nodeData as DatabaseData;
         saveFileDialog1.CheckFileExists = false;
         saveFileDialog1.DefaultExt = "xml";
         saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(databaseData.Definition.Path) + ".xml";
         //            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            return;
         controller.ExportDatabase(databaseData.Definition.Path, saveFileDialog1.FileName);
      }

      private void importToolStripMenuItem_Click(object sender, EventArgs e)
      {
         object nodeData;
         SelectedNodeType nodeType = GetSelectedNodeObject(out nodeData);
         if (nodeType != SelectedNodeType.database)
            return;

         DatabaseData databaseData = nodeData as DatabaseData;

      }
   }
}
