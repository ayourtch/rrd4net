using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using rrd4n.Core;
using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   public partial class NewDatasourceForm : Form
   {
      public DataSourceController Controller { get; set; }
      public DsDef DataSourceDef { get; set; }

      private const string heartbeatPattern = @"(\d+\.\d{1,2}:\d{1,2}:\d{1,2})|(\dx?)";

      private bool dirty;
      private bool changed;
      public NewDatasourceForm()
      {
         InitializeComponent();
         dataSourceTypeComboBox.Items.AddRange(new string[] { "GAUGE", "COUNTER", "DERIVE", "ABSOLUTE" });
      }

      private void ShowDef()
      {
         try
         {
            defLabel.Text = Controller.CreateDef(minValueTextBox.Text, maxValueTextBox.Text, heartbeatTextBox.Text,
               dataSourceNameTextBox.Text, dataSourceTypeComboBox.SelectedItem.ToString());
         }
         catch (FormatException ex)
         {
            defLabel.Text = ex.Message;
         }
      }

      private void NewDatasourceForm_Load(object sender, EventArgs e)
      {
         if (DataSourceDef != null)
         {
            int index = dataSourceTypeComboBox.FindString(DataSourceDef.getDsType().Name);
            dataSourceTypeComboBox.SelectedIndex = index;
            minValueTextBox.Text = DataSourceDef.getMinValue().ToString();
            maxValueTextBox.Text = DataSourceDef.getMaxValue().ToString();
            heartbeatTextBox.Text = DataSourceDef.getHeartbeat().ToString();
            dataSourceNameTextBox.Text = DataSourceDef.getDsName();
            createButton.Enabled = false;
         }
         else
         {
            dataSourceTypeComboBox.SelectedIndex = 0;
            minValueTextBox.Text = double.NaN.ToString();
            maxValueTextBox.Text = double.NaN.ToString();
            heartbeatTextBox.Text = string.Empty;
            dataSourceNameTextBox.Text = "ds1";
            heartbeatTextBox.Text = Controller.DatabaseStep.ToString();
            createButton.Enabled = true;
         }
         dirty = false;
         changed = false;
         databaseStepTextBox.Text = Controller.DatabaseStep.ToString();
      }

      private void createButton_Click(object sender, EventArgs e)
      {
         CreateDataSource();
      }

      private bool CreateDataSource()
      {

         // DEF:name:type:heratbeat:min:max
         string command = Controller.CreateDef(minValueTextBox.Text, maxValueTextBox.Text, heartbeatTextBox.Text,
            dataSourceNameTextBox.Text, dataSourceTypeComboBox.SelectedItem.ToString());

         try
         {
            if (DataSourceDef == null)
               Controller.AddDataSource(command);
            else
               Controller.UpdateDataSource(command, DataSourceDef);

            dirty = false;
            changed = true;
            return true;
         }
         catch (ArgumentException ex)
         {
            MessageBox.Show("Failed to create data source:" + ex.Message);
         }
         catch (ApplicationException  ex)
         {
            MessageBox.Show("Failed to update data source: " + ex.Message);
         }
         return false;

      }

      private void dataSourceNameTextBox_TextChanged(object sender, EventArgs e)
      {
         dirty = true;
      }

      private void heartbeatTextBox_TextChanged(object sender, EventArgs e)
      {
         dirty = true;
      }

      private void minValueTextBox_TextChanged(object sender, EventArgs e)
      {
         dirty = true;
      }

      private void maxValueTextBox_TextChanged(object sender, EventArgs e)
      {
         dirty = true;
      }

      private void finishButton_Click(object sender, EventArgs e)
      {
         if (dirty && !CreateDataSource())
            return;

         DialogResult = changed ? DialogResult.OK : DialogResult.Cancel;
         Close();
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
      }

      private void dataSourceTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         dirty = true;
      }

      private void heartbeatTextBox_Validating(object sender, CancelEventArgs e)
      {
         Regex validateHeartbeatRegex = new Regex(heartbeatPattern);
         if (validateHeartbeatRegex.IsMatch(heartbeatTextBox.Text))
            errorProvider1.SetError((Control)sender, "");
         else
         {
            errorProvider1.SetError((Control)sender, "Invalid timespan format. Use d.hh.mm.ss");
            e.Cancel = true;
         }
      }

      private void heartbeatTextBox_Validated(object sender, EventArgs e)
      {
         ShowDef();
      }

      private void double_Validating(object sender, CancelEventArgs e)
      {
         double value;
         if (!double.TryParse(((TextBox)sender).Text, out value))
         {
            errorProvider1.SetError((Control)sender, "Invalid number format.");
            e.Cancel = true;
            return;
         }
         errorProvider1.SetError((Control)sender, "");
      }

      private void double_Validated(object sender, EventArgs e)
      {
         ShowDef();
      }

      private void dataSourceNameTextBox_Validating(object sender, CancelEventArgs e)
      {
         if (string.IsNullOrEmpty(dataSourceNameTextBox.Text))
         {
            errorProvider1.SetError((Control)sender, "Datasource name can't be empty");
            e.Cancel = true;
            return;
         }
         errorProvider1.SetError((Control)sender, "");
      }

      private void dataSourceTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
      {
         ShowDef();
      }
   }
}
