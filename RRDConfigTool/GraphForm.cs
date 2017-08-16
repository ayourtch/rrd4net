using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using rrd4n.Common;
using rrd4n.Graph;
using RRDConfigTool.Controllers;
using RRDConfigTool.Data;

namespace RRDConfigTool
{
   public partial class GraphForm :  WeifenLuo.WinFormsUI.Docking.DockContent
   {
      public string DataPath { get; set; }
      public DateTime StartTime { get; set; }
      public DateTime EndTime;

      private GraphController controller;

      public GraphForm()
      {
         InitializeComponent();
         DataPath = string.Empty;
         StartTime = DateTime.MinValue;
         EndTime = DateTime.MaxValue;
      }
#region Controller interface
		      
      public void RegisterController(GraphController graphController)
      {
         controller = graphController;
      }

      public void SetImge(MemoryStream imageStream)
      {
         graphBox.Image = Bitmap.FromStream(imageStream);
      }
#endregion

      

      private void loadButton_Click(object sender, EventArgs e)
      {
         try
         {
            // "- --start \"" + startDateTime.ToString() + "\" --end \"" + endDateTime.ToString() + "\" --imgformat PNG DEF:myspeed=" + rrdPath + ":speed:AVERAGE LINE2:myspeed#FF0000"
            GraphParser parser = new GraphParser(graphCommandTextBox.Text);
            RrdGraphDef graphDef = parser.CreateGraphDef();

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            string startTimeString;
            string endTimeString;
            if (DateTime.TryParse(startTimeTextBox.Text, out startDate))
               startTimeString = string.Format("{0}:{1} {2}{3}{4}", startDate.Hour, startDate.Minute, startDate.Year,
                                               startDate.Month, startDate.Day);
            else
               startTimeString = startTimeTextBox.Text;

            if (DateTime.TryParse(endTimeTextBox.Text, out endDate))
               endTimeString = string.Format("{0}:{1} {2}{3}{4}", endDate.Hour, endDate.Minute, endDate.Year,
                                               endDate.Month, endDate.Day);
            else
               endTimeString = endTimeTextBox.Text;

            long[] timeStamps = Util.getTimestamps(startTimeString, endTimeString);
            graphDef.setStartTime(timeStamps[0]);
            graphDef.setEndTime(timeStamps[1]);
            controller.ShowGraph(graphDef);
         }
         catch (FormatException ex)
         {
            MessageBox.Show("Wrong format. " + ex.Message);
         }
         catch (ArgumentException ex)
         {
            MessageBox.Show("Wrong graph definition. " + ex.Message);
         }
         catch (FileNotFoundException ex)
         {
            MessageBox.Show("Fail to load data from file. " + ex.Message);
         }
      }

      private void GraphForm_Load(object sender, EventArgs e)
      {
         startTimeTextBox.Text = "end-1d";
         endTimeTextBox.Text = "now";
      }


      private void saveButton_Click(object sender, EventArgs e)
      {
         saveFileDialog1.AddExtension = true;
         saveFileDialog1.OverwritePrompt = true;
         saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                            @"\RRDConfig\";
         saveFileDialog1.Filter = "Graph(*.grph)|*.grph|All files (*.*)|*.*";
         if (saveFileDialog1.ShowDialog(this) == DialogResult.Cancel)
            return;
         controller.SaveGraph(saveFileDialog1.FileName, graphCommandTextBox.Text);
      }

      public void SetGraphDefinition(string graphDefinition)
      {
         graphCommandTextBox.Text = graphDefinition;
         graphBox.Image = null;
      }
      public void SetTitle(string title)
      {
         Text = title;
      }

      private void graphCommandTextBox_DragEnter(object sender, DragEventArgs e)
      {
         if ((e.AllowedEffect & DragDropEffects.Link) != 0
            && e.Data.GetDataPresent(typeof(DatabaseData)))
            e.Effect = DragDropEffects.Link;
      }

      private void graphCommandTextBox_DragDrop(object sender, DragEventArgs e)
      {
         DatabaseData rrdDef = (DatabaseData)e.Data.GetData(typeof(DatabaseData));
         StringBuilder sb = new StringBuilder();
         sb.AppendLine("-");
         sb.AppendLine("-w 790");
         sb.AppendLine("-h 260");
         sb.AppendLine("--imgformat PNG");
         foreach (var ds in rrdDef.Definition.getDsDefs())
         {
            sb.AppendFormat(string.Format("DEF:{0}_v={1}:{2}:AVERAGE", ds.getDsName(), Path.GetFileName(rrdDef.Definition.Path), ds.getDsName()));
            sb.AppendLine();
            sb.AppendFormat("LINE2:{0}_v#FF0000", ds.getDsName());
         }

         graphCommandTextBox.Text = sb.ToString();
      }
   }
}
