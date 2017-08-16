using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using RRDConfigTool.Controllers;
using RRDConfigTool.Data;
using rrd4n.Core;
using rrd4n.DataAccess.Data;
namespace RRDConfigTool
{
   public partial class RrdDbForm : DockContent
   {
      const string dataSourceNodesName = "Datasources";
      const string archiveNodesName = "Archives";

      private ViewController controller = null;
      public void RegisterController(ViewController viewController)
      {
         this.controller = viewController;
      }

      public RrdDbForm()
      {
         InitializeComponent();
      }

      private void LoadDatabaseView(DatabaseData databaseData)
      {
         dataSourceListView.Columns.Clear();
         dataSourceListView.Columns.Add("Name");
         dataSourceListView.Columns.Add("Value", 200);

         dataSourceListView.Items.Clear();
         ListViewItem lvi = dataSourceListView.Items.Add("Tick");
         lvi.SubItems.Add(databaseData.Definition.getStepTimeSpan().ToString());
         lvi = dataSourceListView.Items.Add(dataSourceNodesName);
         lvi.SubItems.Add(databaseData.Definition.getDsCount().ToString());
         lvi = dataSourceListView.Items.Add(archiveNodesName);
         lvi.SubItems.Add(databaseData.Definition.getArcCount().ToString());
         lvi = dataSourceListView.Items.Add("Estimated size");
         lvi.SubItems.Add(databaseData.Definition.getEstimatedSize().ToString());
         if (databaseData.LastUpdated != DateTime.MinValue)
         {
            lvi = dataSourceListView.Items.Add("Last update time");
            lvi.SubItems.Add(databaseData.LastUpdated.ToString());
         }
         lvi = dataSourceListView.Items.Add("Last Value");
         lvi.SubItems.Add(databaseData.LastValue.ToString());
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
      }
      public void SetDatabaseDefinition(DatabaseData databaseData)
      {
         LoadDatabaseView(databaseData);

//         Text = string.Format("RrdDb Configuration [{0}{1}]", Path.GetFileNameWithoutExtension(rrdDef.Path), dirty ? "*" : "");
      }
      public void SetDocumentName(string documentName)
      {
         Text = documentName;
      }
      
      public void SetDatabaseData(RrdDef rrdDef, DateTime lastUpdated, double lastValue)
      {
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

      private void RrdDbForm_FormClosing(object sender, FormClosingEventArgs e)
      {
         controller.CloseApp();
      }

   }
}
