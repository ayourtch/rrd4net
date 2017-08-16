using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using rrd4n.Core;
using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   public partial class NewArchiveForm : Form
   {
      public ArchiveController Controller { get; set; }
      public ArcDef ArchiveDef { get; set; }

      private const string timespanPattern = @"(\d+\.\d{1,2}:\d{1,2}:\d{1,2})|(\dx?)";

      public NewArchiveForm()
      {
         InitializeComponent();
         archiveTypeComboBox.Items.AddRange(new string[] { "AVERAGE", "MIN", "MAX", "LAST", "FIRST", "TOTAL" });
      }


      private void ShowDef()
      {
         defLabel.Text = Controller.CreateDef(archiveTypeComboBox.SelectedItem.ToString(), xxfTextBox.Text, steppsTextBox.Text, rowsTextBox.Text);
      }

      private void NewArchiveForm_Load(object sender, EventArgs e)
      {
         if (ArchiveDef != null)
         {
            archiveTypeComboBox.SelectedIndex = archiveTypeComboBox.FindString(ArchiveDef.getConsolFun().Name);
            xxfTextBox.Text = ArchiveDef.getXff().ToString();
            steppsTextBox.Text = ArchiveDef.getSteps().ToString();
            rowsTextBox.Text = ArchiveDef.getRows().ToString();
            xxfTextBox.Enabled = false;
            archiveTypeComboBox.Enabled = false;
            steppsTextBox.Enabled = false;
            addButton.Text = "Update";
         }
         else
         {
            archiveTypeComboBox.SelectedIndex = 0;
            xxfTextBox.Text = "0,5";
            steppsTextBox.Text = "1";
            rowsTextBox.Text = "2";
         }
         dbStepsTextBox.Text = Controller.DatabaseSteps.ToString();
         ShowDef();
      }

      private void addButton_Click(object sender, EventArgs e)
      {
         int stepps;
         if (!int.TryParse(steppsTextBox.Text, out stepps)
            || stepps < 1)
         {
            errorProvider1.SetIconAlignment(steppsTextBox, ErrorIconAlignment.MiddleRight);
            errorProvider1.SetError(steppsTextBox, "Invalid step value");
            steppsTextBox.SelectAll();
            steppsTextBox.Focus();
            return;
         }
         int rows;
         if (!int.TryParse(rowsTextBox.Text, out rows)
            || rows < 1)
         {
            errorProvider1.SetIconAlignment(rowsTextBox, ErrorIconAlignment.MiddleRight);
            errorProvider1.SetError(rowsTextBox, "Invalid step value");
            rowsTextBox.SelectAll();
            rowsTextBox.Focus();
            return;
         }

         try
         {
            //// RRA:cfun:xff:steps:rows
            string command = string.Format("RRA:{0}:{1}:{2}:{3}",
                                           archiveTypeComboBox.SelectedItem.ToString(),
                                           xxfTextBox.Text,
                                           steppsTextBox.Text,
                                           rowsTextBox.Text
               );
            if (ArchiveDef == null)
               Controller.AddArchive(command);
            else
               Controller.UpdateArchive(command);
         }
         catch (ArgumentException ex)
         {
            MessageBox.Show(ex.Message, "Create error", MessageBoxButtons.OK);
            return;
         }

         DialogResult = DialogResult.OK;
         Close();
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
      }

      private void xxfTextBox_TextChanged(object sender, EventArgs e)
      {
         double xxf;
         if (!double.TryParse(xxfTextBox.Text, out xxf)
            || xxf < 0d || xxf > 1d)
         {
            errorProvider1.SetIconAlignment(xxfTextBox, ErrorIconAlignment.MiddleRight);
            errorProvider1.SetError(xxfTextBox, "Invalid step value");
            xxfTextBox.SelectAll();
            xxfTextBox.Focus();
            return;
         }
      }

      private void timeSpanTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
      {
         Regex validateHeartbeatRegex = new Regex(timespanPattern);
         if (validateHeartbeatRegex.IsMatch(((Control)sender).Text))
            errorProvider1.SetError((Control)sender, "");
         else
         {
            errorProvider1.SetError((Control)sender, "Invalid timespan format. Use d.hh.mm.ss");
            e.Cancel = true;
         }
      }

      private void TextBox_Validated(object sender, EventArgs e)
      {
         ShowDef();
      }
   }
}
