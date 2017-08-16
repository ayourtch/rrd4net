namespace RRDConfigTool
{
   partial class EditDatabaseForm
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
         this.databaseNameLabel = new System.Windows.Forms.Label();
         this.label1 = new System.Windows.Forms.Label();
         this.startTimeTextBox = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.stepTextBox = new System.Windows.Forms.TextBox();
         this.OKButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // databaseNameLabel
         // 
         this.databaseNameLabel.AutoSize = true;
         this.databaseNameLabel.Location = new System.Drawing.Point(13, 23);
         this.databaseNameLabel.Name = "databaseNameLabel";
         this.databaseNameLabel.Size = new System.Drawing.Size(82, 13);
         this.databaseNameLabel.TabIndex = 0;
         this.databaseNameLabel.Text = "Database name";
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(13, 61);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(51, 13);
         this.label1.TabIndex = 1;
         this.label1.Text = "Start time";
         // 
         // startTimeTextBox
         // 
         this.startTimeTextBox.Location = new System.Drawing.Point(16, 77);
         this.startTimeTextBox.Name = "startTimeTextBox";
         this.startTimeTextBox.Size = new System.Drawing.Size(164, 20);
         this.startTimeTextBox.TabIndex = 2;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(13, 101);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(29, 13);
         this.label2.TabIndex = 3;
         this.label2.Text = "Step";
         // 
         // stepTextBox
         // 
         this.stepTextBox.Location = new System.Drawing.Point(16, 117);
         this.stepTextBox.Name = "stepTextBox";
         this.stepTextBox.Size = new System.Drawing.Size(161, 20);
         this.stepTextBox.TabIndex = 4;
         // 
         // OKButton
         // 
         this.OKButton.Location = new System.Drawing.Point(16, 154);
         this.OKButton.Name = "OKButton";
         this.OKButton.Size = new System.Drawing.Size(75, 23);
         this.OKButton.TabIndex = 5;
         this.OKButton.Text = "Ok";
         this.OKButton.UseVisualStyleBackColor = true;
         this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.Location = new System.Drawing.Point(125, 153);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 6;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // EditDatabaseForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(227, 204);
         this.Controls.Add(this.cancelButton);
         this.Controls.Add(this.OKButton);
         this.Controls.Add(this.stepTextBox);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.startTimeTextBox);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.databaseNameLabel);
         this.Name = "EditDatabaseForm";
         this.Text = "EditDatabaseForm";
         this.Load += new System.EventHandler(this.EditDatabaseForm_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label databaseNameLabel;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox startTimeTextBox;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox stepTextBox;
      private System.Windows.Forms.Button OKButton;
      private System.Windows.Forms.Button cancelButton;
   }
}