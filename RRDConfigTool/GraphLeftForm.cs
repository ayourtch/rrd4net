using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   public partial class GraphLeftForm : WeifenLuo.WinFormsUI.Docking.DockContent
   {
      private GraphController controller;
      public GraphLeftForm()
      {
         InitializeComponent();
      }
      public void RegisterController(GraphController graphController)
      {
         controller = graphController;
      }

      private void GraphLeftForm_Load(object sender, EventArgs e)
      {
         string formsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RRDConfig\";
         if (!Directory.Exists(formsPath))
            Directory.CreateDirectory(formsPath);

         DirectoryInfo directoryInfo = new DirectoryInfo(formsPath);
         string[] files = Directory.GetFiles(formsPath, "*.grph", SearchOption.TopDirectoryOnly);
         LoadList(files);
      }

      private void LoadList(string[] filePaths)
      {
         graphListView.Items.Clear();
         foreach (var filePath in filePaths)
         {
            ListViewItem lvi = graphListView.Items.Add(Path.GetFileNameWithoutExtension(filePath));
            lvi.Tag = filePath;
         }
      }

      private void graphListView_SelectedIndexChanged(object sender, EventArgs e)
      {
         ListView.SelectedListViewItemCollection selectedItems = graphListView.SelectedItems;
      }

      public void SetFilepath(string filePath)
      {
         foreach(ListViewItem lvi in graphListView.Items)
         {
            string listFilepath = lvi.Tag as string;
            if (listFilepath == filePath)
               return;
         }
         graphListView.Items.Add(Path.GetFileNameWithoutExtension(filePath)).Tag = filePath;
      }

      private void graphListView_ItemActivate(object sender, EventArgs e)
      {

      }

      private void graphListView_MouseClick(object sender, MouseEventArgs e)
      {
         ListViewHitTestInfo hitInfo = graphListView.HitTest(new Point(e.X, e.Y));
         if (hitInfo.Item != null)
         {
            string filePath = hitInfo.Item.Tag as string;
            controller.LoadGraph(filePath);
         }
      }
   }
}
