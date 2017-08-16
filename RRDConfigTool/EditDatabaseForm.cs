using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using rrd4n.Core;
namespace RRDConfigTool
{
   public partial class EditDatabaseForm : Form
   {
      public RrdDef DatabaseDefinition { get; set; }
      public EditDatabaseForm()
      {
         InitializeComponent();
      }

      private void EditDatabaseForm_Load(object sender, EventArgs e)
      {
         if (DatabaseDefinition == null)
            return;
         databaseNameLabel.Text = DatabaseDefinition.getPath();
         stepTextBox.Text = DatabaseDefinition.getStepTimeSpan().ToString();
         startTimeTextBox.Text = DatabaseDefinition.getStartDateTime().ToString();
      }

      private void OKButton_Click(object sender, EventArgs e)
      {
         DatabaseDefinition.setStartTime(DateTime.Parse(startTimeTextBox.Text));
         DatabaseDefinition.setStepTimeSpan(TimeSpan.Parse(stepTextBox.Text));
         DialogResult = DialogResult.OK;
         Hide();
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Hide();
      }
   }
}
