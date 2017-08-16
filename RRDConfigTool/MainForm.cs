using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Castle.MicroKernel;
using rrd4n;
using rrd4n.Core;
using RRDConfigTool.Controllers;
using RRDConfigTool.Data;
using rrd4n.DataAccess.Data;

namespace RRDConfigTool
{
   public partial class MainForm : Form
   {
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
      const string dataSourceNodesName = "Datasources";
      const string archiveNodesName = "Archives";

      private ViewController controller = null;
      private readonly IKernel kernel;

      public string[] Arguments { get; set; }
      
      public MainForm()
      {
         InitializeComponent();
      }

      public MainForm(IKernel kernel)
         : this()
      {
         this.kernel = kernel;
      }

      private void openToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //controller.CloseDatabase();
         //openFileDialog1.DefaultExt = "rra";
         //openFileDialog1.AddExtension = true;
         //openFileDialog1.CheckFileExists = true;
         //openFileDialog1.FileName = "";
         //openFileDialog1.ReadOnlyChecked = true;
         //openFileDialog1.ShowReadOnly = true;
         //openFileDialog1.FileName = "*.rra";
         //if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
         //   return;
         //if (!controller.OpenDatabase(openFileDialog1.FileName, openFileDialog1.ReadOnlyChecked))
         //   return;
         //saveToolStripMenuItem.Enabled = false;
      }


      private SelectedNodeType GetSelectedNodeObject(out object selectedObject)
      {
         return GetSelectedNodeObject(rrdDbTreeView.SelectedNode, out selectedObject);
      }

      private SelectedNodeType GetSelectedNodeObject(TreeNode selectedNode, out object selectedObject)
      {
         selectedObject = selectedNode.Tag;
         if (selectedNode.Parent == null)
            return SelectedNodeType.database;
         if (selectedNode.Text.CompareTo(dataSourceNodesName) == 0)
            return SelectedNodeType.datasources;
         if (selectedNode.Text.CompareTo(archiveNodesName) == 0)
            return SelectedNodeType.archives;
         if (selectedNode.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return SelectedNodeType.datasource;
         if (selectedNode.Parent.Parent.Text.CompareTo(dataSourceNodesName) == 0)
            return SelectedNodeType.datasourcearchive;
         return SelectedNodeType.unknown;
      }

      private void LoadTree(RrdDef databaseDefinition)
      {
         rrdDbTreeView.Nodes.Clear();

         var databaseNode = rrdDbTreeView.Nodes.Add("databasenode",Path.GetFileNameWithoutExtension(databaseDefinition.Path));
         var datasources = databaseNode.Nodes.Add(dataSourceNodesName, dataSourceNodesName);
         foreach (var datasource in databaseDefinition.getDsDefs())
         {
            var datasourceNode = datasources.Nodes.Add(datasource.DsName);
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

      private void LoadDatabaseView(RrdDef rrDef, DateTime lastUpdated, double lastValue)
      {
         dataSourceListView.Columns.Clear();
         dataSourceListView.Columns.Add("Name");
         dataSourceListView.Columns.Add("Value", 200);

         dataSourceListView.Items.Clear();
         if (rrDef == null)
            return;
         ListViewItem lvi = dataSourceListView.Items.Add("Tick");
         lvi.SubItems.Add(rrDef.getStepTimeSpan().ToString());
         lvi = dataSourceListView.Items.Add(dataSourceNodesName);
         lvi.SubItems.Add(rrDef.getDsCount().ToString());
         lvi = dataSourceListView.Items.Add(archiveNodesName);
         lvi.SubItems.Add(rrDef.getArcCount().ToString());
         lvi = dataSourceListView.Items.Add("Estimated size");
         lvi.SubItems.Add(rrDef.getEstimatedSize().ToString());
         if (lastUpdated != DateTime.MinValue)
         {
            lvi = dataSourceListView.Items.Add("Last update time");
            lvi.SubItems.Add(lastUpdated.ToString());
         }
         lvi = dataSourceListView.Items.Add("Last Value");
         lvi.SubItems.Add(lastValue.ToString());
      }


      private void LoadDataSourceView(IEnumerable<DsDef> dataSources)
      {
         dataSourceListView.Items.Clear();
         dataSourceListView.Columns.Clear();
         dataSourceListView.Columns.Add("Name");
         dataSourceListView.Columns.Add("Type");
         dataSourceListView.Columns.Add("Heartbeat");
         dataSourceListView.Columns.Add("Min");
         dataSourceListView.Columns.Add("Max");
         if (dataSources == null)
            return;
         foreach (var dataSource in dataSources)
         {
            ListViewItem lvi = dataSourceListView.Items.Add(dataSource.getDsName());
            lvi.SubItems.Add(dataSource.getDsType().Name);
            lvi.SubItems.Add(dataSource.getHeartbeat().ToString());
            lvi.SubItems.Add(dataSource.getMinValue().ToString());
            lvi.SubItems.Add(dataSource.getMaxValue().ToString());
            lvi.Tag = dataSource;
         }
      }

      private void LoadDataSourceView(ArchiveDisplayData[] archives)
      {
         dataSourceListView.Items.Clear();
         dataSourceListView.Columns.Clear();
         dataSourceListView.Columns.Add("CF type");
         dataSourceListView.Columns.Add("Rows");
         dataSourceListView.Columns.Add("Steps");
         dataSourceListView.Columns.Add("xff");
         if (archives == null)
            return;

         if (archives.Length == 0)
            return;
         if (archives[0].StartTime != DateTime.MinValue)
         {
            dataSourceListView.Columns.Add("Start");
            dataSourceListView.Columns.Add("End");
         }

         foreach (var archive in archives)
         {
            ListViewItem lvi = dataSourceListView.Items.Add(archive.ConsolFunctionName);
            lvi.SubItems.Add(archive.RowCount.ToString());
            lvi.SubItems.Add(archive.Steps.ToString());
            lvi.SubItems.Add(archive.Xff.ToString());
            lvi.Tag = archive;
            if (archive.StartTime != DateTime.MinValue)
            {
               lvi.SubItems.Add(archive.StartTime.ToString());
               lvi.SubItems.Add(archive.EndTime.ToString());
            }
         }
      }

      public void ClearViews()
      {
         dataSourceListView.Items.Clear();
         rrdDbTreeView.Nodes.Clear();
      }

      private void MainForm_Load(object sender, EventArgs e)
      {
         Arguments = Environment.GetCommandLineArgs();

         if (Arguments != null && Arguments.Length > 1)
            ParseCommands(Arguments);
      }

      private void rrdDbTreeView_AfterSelect(object sender, TreeViewEventArgs e)
      {
         importDataToolStripMenuItem1.Enabled = false;
         editToolStripMenuItem1.Enabled = false;
         object selectedObject;
         SelectedNodeType selectedNodeType = GetSelectedNodeObject(e.Node, out selectedObject);

         switch(selectedNodeType)
         {
            case SelectedNodeType.database:
               propertiesGroupBox.Text = "Database";
               //controller.DatabaseSelected();
               return;
            case SelectedNodeType.datasources:
               propertiesGroupBox.Text = "Datasources";
               //controller.DatasourcesSelected();
               return;
            case SelectedNodeType.archives:
               propertiesGroupBox.Text = "Archives";
               controller.ArchivesSelected(selectedObject as DatabaseData);
               return;
            case SelectedNodeType.datasource:
               importDataToolStripMenuItem1.Enabled = true;
               editToolStripMenuItem1.Enabled = true;
               return;
            case SelectedNodeType.datasourcearchive:
               editToolStripMenuItem1.Enabled = true;
               Application.UseWaitCursor = true;
               //controller.DatasourceArchiveSelected(e.Node.Parent.Tag, e.Node.Tag);
               Application.UseWaitCursor = false;
               return;
         }
      }

      private void newToolStripMenuItem_Click(object sender, EventArgs e)
      {
         NewRrdDbForm frm = new NewRrdDbForm();
         frm.Controller = controller;
         if (frm.ShowDialog(this) == DialogResult.Cancel)
            return;
         saveToolStripMenuItem.Enabled = true;
         // Can't import data until saved
         importDataToolStripMenuItem.Enabled = false;
         importDataToolStripMenuItem.ToolTipText = "Can't import until database saved";
      }

      private void addArchiveToolStripMenuItem_Click(object sender, EventArgs e)
      {
         ArchiveController arController = kernel[typeof(ArchiveController)] as ArchiveController;
         arController.Run();
         controller.DatabaseUpdated();
         //controller.ArchivesSelected();
      }

      private void exitToolStripMenuItem_Click(object sender, EventArgs e)
      {
         Close();
      }

      private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         controller.CloseApp();
      }

      private void importDataToolStripMenuItem_Click(object sender, EventArgs e)
      {
         openFileDialog1.DefaultExt = "csv";
         openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
         if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            return;
      }

      private void saveToolStripMenuItem_Click(object sender, EventArgs e)
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
            return;
         }
         //Database created. Can not save again
         saveToolStripMenuItem.Enabled = false;
         importDataToolStripMenuItem.Enabled = true;
         importDataToolStripMenuItem.ToolTipText = "Import data from a csv file";
      }

      private void fetchDataToolStripMenuItem_Click(object sender, EventArgs e)
      {
         FetchDataController fetchController = kernel[typeof(FetchDataController)] as FetchDataController;
         fetchController.Run();
      }

      private void ParseCommands(string[] commandArguments)
      {
         if (commandArguments[1].StartsWith("create"))
         {
            StringBuilder sb = new StringBuilder();
            // Another way to do it is to use the commandlin directly and remove the two first arguments..... 
            for (var i = 2; i < commandArguments.Length; i++)
            {
               if (i > 2)
                  sb.Append(" ");
               sb.Append(commandArguments[i]);
            }
            var createDatabaseController = kernel[typeof(CreateDatabaseController)] as CreateDatabaseController;
            createDatabaseController.SetDatabaseDefinition(sb.ToString());
            controller.DatabaseCreated();
         }
      }

      private void editToolStripMenuItem1_Click(object sender, EventArgs e)
      {
         object selectedObject;
         SelectedNodeType selectedNodeType = GetSelectedNodeObject(out selectedObject);

         switch (selectedNodeType)
         {
            case SelectedNodeType.datasource:
               DataSourceController dsController = kernel[typeof(DataSourceController)] as DataSourceController;
               dsController.DataSourceDef = selectedObject as DsDef;
               dsController.Run();
               return;
            case SelectedNodeType.datasourcearchive:
               ArchiveController archController = kernel[typeof(ArchiveController)] as ArchiveController;
               archController.ArchiveDef = selectedObject as ArcDef;
               archController.Run();
               return;
         }
      }

      private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
      {

      }

      private void addDatasourceToolStripMenuItem_Click(object sender, EventArgs e)
      {
         DataSourceController dsController = kernel[typeof(DataSourceController)] as DataSourceController;
         if (dsController.Run())
         {
            controller.DatabaseUpdated();
            //controller.DatasourcesSelected();
            return;
         }
      }

      public void SetDatabaseDefinition(RrdDef rrdDef,bool dirty)
      {
         LoadTree(rrdDef);
         Text = string.Format("RrdDb Configuration [{0}{1}]", Path.GetFileNameWithoutExtension(rrdDef.Path), dirty ? "*" : "");
      }

      public void SetDatabaseData(RrdDef rrdDef, DateTime lastUpdated, double lastValue)
      {
         LoadDatabaseView(rrdDef, lastUpdated, lastValue);
      }

      public void SetDatasourceData(DsDef[] datasources)
      {
         LoadDataSourceView(datasources);
      }

      public void SetArchiveData(ArchiveDisplayData[] archives)
      {
         LoadDataSourceView(archives);
      }

      public void SetArchiveDumpData(FetchData data)
      {
         DataListView.Items.Clear();
         DataListView.BeginUpdate();
         DateTime[] dates = data.getDateTimestamps();
         int count = dates.Length;
         double[] values = data.getValues()[0];
         for (var i = 0; i < count; i++)
         {
            if (double.IsNaN(data.getValues()[0][i]))
               continue;
            ListViewItem lvi = DataListView.Items.Add(dates[i].ToString());
            lvi.SubItems.Add(values[i].ToString());
         }
         DataListView.EndUpdate();
      }
      public void ClearArchiveDumpData()
      {
         DataListView.Items.Clear();
      }


      public string SaveDataBase(string databasePath)
      {
         if (MessageBox.Show(this, "Database not saved! \r Save changes?", "RRD Config tool", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return string.Empty;
         openFileDialog1.CheckFileExists = true;
         openFileDialog1.AddExtension = true;
         openFileDialog1.DefaultExt = "rrd";
         openFileDialog1.FileName = databasePath;
         if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
            return string.Empty;
         return openFileDialog1.FileName;
      }

      public void RegisterController(ViewController viewController)
      {
         this.controller = viewController;
      }

      private void resizeToolStripMenuItem_Click(object sender, EventArgs e)
      {

      }

      private void dumpToolStripMenuItem_Click(object sender, EventArgs e)
      {
         if (rrdDbTreeView.SelectedNode.Parent != null
            && rrdDbTreeView.SelectedNode.Parent.Text == archiveNodesName)
         {
            string dumpText = controller.DumpArchive(rrdDbTreeView.SelectedNode.Tag);
            DumpForm frm = new DumpForm {DumpText = dumpText};
            frm.ShowDialog(this);
         }
      }

      [Obsolete]
      private void importDataToolStripMenuItem1_Click(object sender, EventArgs e)
      {
//         if (rrdDbTreeView.SelectedNode.Parent != null
//            && rrdDbTreeView.SelectedNode.Parent.Text == dataSourceNodesName)
//         {
//            openFileDialog1.DefaultExt = "csv";
//            openFileDialog1.FileName = "*.csv";
////            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//            if (openFileDialog1.ShowDialog(this) == DialogResult.Cancel)
//               return;

//            try
//            {
//               controller.ImportData(openFileDialog1.FileName, rrdDbTreeView.SelectedNode.Text, new TimeSpan(7, 0, 0, 0));

//            }
//            catch (ApplicationException ex)
//            {
//               MessageBox.Show("Fail to import data." + ex.Message);
//            }
//         }
      }

      private void dumpDatabaseDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
      {
         //DumpForm frm = new DumpForm {DumpText = controller.DumpDatabaseDefinition()};
         //frm.ShowDialog(this);
      }

      private void createToolStripMenuItem_Click(object sender, EventArgs e)
      {
         var createDatabaseController = kernel[typeof(CreateDatabaseController)] as CreateDatabaseController;
         if (createDatabaseController.Run())
            controller.DatabaseCreated();
      }

      private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
      {
         AboutBox frm = new AboutBox();
         frm.ShowDialog(this);

      }

      private void graphToolStripMenuItem_Click(object sender, EventArgs e)
      {
         GraphController graphController = kernel[typeof(GraphController)] as GraphController;
         graphController.Run();
      }

      private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
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
            return;
         }
         //Database created. Can not save again
         saveToolStripMenuItem.Enabled = false;
         importDataToolStripMenuItem.Enabled = true;
         importDataToolStripMenuItem.ToolTipText = "Import data from a csv file";
      }
   }
}

