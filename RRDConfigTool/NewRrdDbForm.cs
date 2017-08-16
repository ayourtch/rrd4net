using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using rrd4n;
using rrd4n.Core;
using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   public partial class NewRrdDbForm : Form
   {
      public ViewController Controller { private get; set; }

      public NewRrdDbForm()
      {
         InitializeComponent();
      }

      public RrdDef RrdDef { get; private set; }
 
      private void NewRrdDbForm_Load(object sender, EventArgs e)
      {
         startTimeTextBox.Text = DateTime.Now.AddSeconds(-10).ToString();// new DateTime(2000, 05, 04).ToString();// 
         stepTextBox.Text = "0.0:5:0";// 5 minutes ie 300 sec
      }


      private void cancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }

      private void createRrDefButton_Click(object sender, EventArgs e)
      {
         string databaseName = databaseNameTextBox.Text;
         DateTime startTime;
         long step;
                  
         if (string.IsNullOrEmpty(databaseName))
         {
            databaseNameTextBox.Focus();
            errorProvider1.SetIconAlignment(databaseNameTextBox, ErrorIconAlignment.MiddleRight);
            errorProvider1.SetError(databaseNameTextBox, "Bad date format");
            return;
         }

         if (!DateTime.TryParse(startTimeTextBox.Text, out startTime))
         {
            errorProvider1.SetIconAlignment(startTimeTextBox, ErrorIconAlignment.MiddleRight);
            errorProvider1.SetError(startTimeTextBox, "Bad date format");
            startTimeTextBox.Focus();
            return;
         }
         TimeSpan ts;
         if (TimeSpan.TryParse(stepTextBox.Text, out ts))
         {
            step = (long) ts.TotalSeconds;
         }
         else
         {
            if (!long.TryParse(stepTextBox.Text, out step))
            {
               errorProvider1.SetIconAlignment(stepTextBox, ErrorIconAlignment.MiddleRight);
               errorProvider1.SetError(stepTextBox, "Bad date format");
               stepTextBox.Focus();
               stepTextBox.SelectAll();
               return;
            }
         }

         Controller.NewDatabase(databaseName, startTime, step);
         DialogResult = System.Windows.Forms.DialogResult.OK;
         Close();
      }
   }
}
