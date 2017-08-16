using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RRDConfigTool.Controllers;

namespace RRDConfigTool
{
   public partial class CreateDatabaseForm : Form
   {
      public CreateDatabaseController Controller { get; set; }

      public CreateDatabaseForm()
      {
         InitializeComponent();
      }

      private void parseButton_Click(object sender, EventArgs e)
      {
         try
         {
            Controller.SetDatabaseDefinition(configTextBox.Text);
            DialogResult = DialogResult.OK;
            Close();
         }
         catch (Exception ex)
         {
            MessageBox.Show(this, "Fail to create database!\n" + ex.Message, "Error");
         }
      }

      private void cancelButton_Click(object sender, EventArgs e)
      {
         DialogResult = DialogResult.Cancel;
         Close();
      }
   }
}
