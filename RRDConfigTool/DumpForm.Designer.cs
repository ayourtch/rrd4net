namespace RRDConfigTool
{
   partial class DumpForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.dumpTextBox = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // dumpTextBox
         // 
         this.dumpTextBox.AcceptsReturn = true;
         this.dumpTextBox.AcceptsTab = true;
         this.dumpTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.dumpTextBox.Location = new System.Drawing.Point(12, 26);
         this.dumpTextBox.Multiline = true;
         this.dumpTextBox.Name = "dumpTextBox";
         this.dumpTextBox.Size = new System.Drawing.Size(690, 425);
         this.dumpTextBox.TabIndex = 0;
         // 
         // DumpForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(714, 463);
         this.Controls.Add(this.dumpTextBox);
         this.Name = "DumpForm";
         this.Text = "DumpForm";
         this.Load += new System.EventHandler(this.DumpForm_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox dumpTextBox;
   }
}