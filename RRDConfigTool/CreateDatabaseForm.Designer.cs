namespace RRDConfigTool
{
   partial class CreateDatabaseForm
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
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.configTextBox = new System.Windows.Forms.TextBox();
         this.parseButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.groupBox2.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.configTextBox);
         this.groupBox2.Location = new System.Drawing.Point(12, 12);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(443, 178);
         this.groupBox2.TabIndex = 16;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Configuration";
         // 
         // configTextBox
         // 
         this.configTextBox.Location = new System.Drawing.Point(6, 19);
         this.configTextBox.Multiline = true;
         this.configTextBox.Name = "configTextBox";
         this.configTextBox.Size = new System.Drawing.Size(431, 153);
         this.configTextBox.TabIndex = 14;
         // 
         // parseButton
         // 
         this.parseButton.Location = new System.Drawing.Point(162, 208);
         this.parseButton.Name = "parseButton";
         this.parseButton.Size = new System.Drawing.Size(75, 23);
         this.parseButton.TabIndex = 13;
         this.parseButton.Text = "Create";
         this.parseButton.UseVisualStyleBackColor = true;
         this.parseButton.Click += new System.EventHandler(this.parseButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cancelButton.Location = new System.Drawing.Point(254, 208);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 15;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // CreateDatabaseForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(466, 241);
         this.Controls.Add(this.groupBox2);
         this.Controls.Add(this.parseButton);
         this.Controls.Add(this.cancelButton);
         this.Name = "CreateDatabaseForm";
         this.Text = "Database configuration";
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.TextBox configTextBox;
      private System.Windows.Forms.Button parseButton;
      private System.Windows.Forms.Button cancelButton;
   }
}