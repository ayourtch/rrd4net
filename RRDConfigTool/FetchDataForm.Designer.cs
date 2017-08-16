namespace RRDConfigTool
{
   partial class FetchDataForm
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
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.dataListView = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.archiveTypeComboBox = new System.Windows.Forms.ComboBox();
         this.fetchButton = new System.Windows.Forms.Button();
         this.fromDateTextBox = new System.Windows.Forms.TextBox();
         this.toDateTextBox = new System.Windows.Forms.TextBox();
         this.resolutionTextBox = new System.Windows.Forms.TextBox();
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // groupBox1
         // 
         this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this.groupBox1.Controls.Add(this.dataListView);
         this.groupBox1.Location = new System.Drawing.Point(13, 69);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(267, 345);
         this.groupBox1.TabIndex = 1;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "groupBox1";
         // 
         // dataListView
         // 
         this.dataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
         this.dataListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataListView.Location = new System.Drawing.Point(3, 16);
         this.dataListView.Name = "dataListView";
         this.dataListView.Size = new System.Drawing.Size(261, 326);
         this.dataListView.TabIndex = 1;
         this.dataListView.UseCompatibleStateImageBehavior = false;
         this.dataListView.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Timestamp";
         this.columnHeader1.Width = 100;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "value";
         this.columnHeader2.Width = 100;
         // 
         // archiveTypeComboBox
         // 
         this.archiveTypeComboBox.FormattingEnabled = true;
         this.archiveTypeComboBox.Location = new System.Drawing.Point(16, 12);
         this.archiveTypeComboBox.Name = "archiveTypeComboBox";
         this.archiveTypeComboBox.Size = new System.Drawing.Size(121, 21);
         this.archiveTypeComboBox.TabIndex = 3;
         // 
         // fetchButton
         // 
         this.fetchButton.Location = new System.Drawing.Point(244, 37);
         this.fetchButton.Name = "fetchButton";
         this.fetchButton.Size = new System.Drawing.Size(48, 23);
         this.fetchButton.TabIndex = 4;
         this.fetchButton.Text = "Fetch";
         this.fetchButton.UseVisualStyleBackColor = true;
         this.fetchButton.Click += new System.EventHandler(this.fetchButton_Click);
         // 
         // fromDateTextBox
         // 
         this.fromDateTextBox.Location = new System.Drawing.Point(16, 40);
         this.fromDateTextBox.Name = "fromDateTextBox";
         this.fromDateTextBox.Size = new System.Drawing.Size(100, 20);
         this.fromDateTextBox.TabIndex = 5;
         // 
         // toDateTextBox
         // 
         this.toDateTextBox.Location = new System.Drawing.Point(138, 40);
         this.toDateTextBox.Name = "toDateTextBox";
         this.toDateTextBox.Size = new System.Drawing.Size(100, 20);
         this.toDateTextBox.TabIndex = 6;
         // 
         // resolutionTextBox
         // 
         this.resolutionTextBox.Location = new System.Drawing.Point(157, 14);
         this.resolutionTextBox.Name = "resolutionTextBox";
         this.resolutionTextBox.Size = new System.Drawing.Size(100, 20);
         this.resolutionTextBox.TabIndex = 7;
         // 
         // FetchDataForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(292, 426);
         this.Controls.Add(this.resolutionTextBox);
         this.Controls.Add(this.toDateTextBox);
         this.Controls.Add(this.fromDateTextBox);
         this.Controls.Add(this.fetchButton);
         this.Controls.Add(this.archiveTypeComboBox);
         this.Controls.Add(this.groupBox1);
         this.Name = "FetchDataForm";
         this.Text = "FetchDataForm";
         this.Load += new System.EventHandler(this.FetchDataForm_Load);
         this.groupBox1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.ListView dataListView;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.ComboBox archiveTypeComboBox;
      private System.Windows.Forms.Button fetchButton;
      private System.Windows.Forms.TextBox fromDateTextBox;
      private System.Windows.Forms.TextBox toDateTextBox;
      private System.Windows.Forms.TextBox resolutionTextBox;

   }
}