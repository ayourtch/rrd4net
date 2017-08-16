using System;
using System.Collections.Generic;
using System.Windows.Forms;
using rrd4n.Common;
using rrd4n.Core;
using RRDConfigTool.Controllers;
using RRDConfigTool.Data;

namespace RRDConfigTool
{
   public partial class FetchDataForm : Form
   {
      public FetchDataController Controller { get; set; }

      public RrdDb Database { get; set; }

      public FetchDataForm()
      {
         InitializeComponent();
         string[] archiveTypes = new string[] { "AVERAGE", "MIN", "MAX", "LAST", "FIRST", "TOTAL" };
         archiveTypeComboBox.Items.AddRange(archiveTypes);
      }

      private void FetchDataForm_Load(object sender, EventArgs e)
      {
         archiveTypeComboBox.SelectedIndex = 0;

         fromDateTextBox.Text = DateTime.Now.AddHours(-1).ToString();// "2000-05-05 00:00:00";
         toDateTextBox.Text = DateTime.Now.ToString();// "2006-01-01 00:00:00";
         resolutionTextBox.Text = "1.00:00:00";
      }

      private void LoadData()
      {
         dataListView.Items.Clear();
         DateTime fromDate = DateTime.Parse(fromDateTextBox.Text);
         DateTime toDate = DateTime.Parse(toDateTextBox.Text);
         long res;
         TimeSpan resolution = TimeSpan.MinValue;
         if (long.TryParse(resolutionTextBox.Text, out res))
         {
            resolution = new TimeSpan(res * 10000000L);
         }
         else
         {
            if (!TimeSpan.TryParse(resolutionTextBox.Text, out resolution))
            {
               MessageBox.Show(this, "Wrong resolution format", "Error", MessageBoxButtons.OK);
               return;
            }
         }

         try
         {
            Controller.LoadData(archiveTypeComboBox.SelectedItem.ToString(), fromDate, toDate, resolution);

         }
         catch (ApplicationException ex)
         {
            MessageBox.Show(this, "Fetch failed:" + ex.Message);
         }
      }

      private void fetchButton_Click(object sender, EventArgs e)
      {
         LoadData();
      }

      //public void SetData(List<FetchedData> fetchedData)
      //{
      //   dataListView.Items.Clear();
      //   foreach(var data in fetchedData)
      //   {
      //      ListViewItem lvi = dataListView.Items.Add(data.TimeStamp.ToString());
      //      lvi.SubItems.Add(data.Value.ToString());
      //   }
      //}
   }
}
