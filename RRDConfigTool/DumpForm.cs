using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RRDConfigTool
{
   public partial class DumpForm : Form
   {
      public string DumpText { get; set; }
      public DumpForm()
      {
         InitializeComponent();
      }

      private void DumpForm_Load(object sender, EventArgs e)
      {
         string[] lines = DumpText.Split(new char[]{'\n'});
         dumpTextBox.Lines = lines;
      }
   }
}
