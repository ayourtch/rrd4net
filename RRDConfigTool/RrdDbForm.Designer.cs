namespace RRDConfigTool
{
   partial class RrdDbForm
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
         this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
         this.dataSourceListView = new System.Windows.Forms.ListView();
         this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
         this.DataListView = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.propertiesGroupBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // propertiesGroupBox
         // 
         this.propertiesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.propertiesGroupBox.Controls.Add(this.dataSourceListView);
         this.propertiesGroupBox.Location = new System.Drawing.Point(12, 12);
         this.propertiesGroupBox.Name = "propertiesGroupBox";
         this.propertiesGroupBox.Size = new System.Drawing.Size(420, 185);
         this.propertiesGroupBox.TabIndex = 3;
         this.propertiesGroupBox.TabStop = false;
         this.propertiesGroupBox.Text = "Properties";
         // 
         // dataSourceListView
         // 
         this.dataSourceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
         this.dataSourceListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataSourceListView.Location = new System.Drawing.Point(3, 16);
         this.dataSourceListView.Name = "dataSourceListView";
         this.dataSourceListView.Size = new System.Drawing.Size(414, 166);
         this.dataSourceListView.TabIndex = 0;
         this.dataSourceListView.UseCompatibleStateImageBehavior = false;
         this.dataSourceListView.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader3
         // 
         this.columnHeader3.Text = "Name";
         this.columnHeader3.Width = 200;
         // 
         // columnHeader4
         // 
         this.columnHeader4.Text = "Value";
         this.columnHeader4.Width = 200;
         // 
         // DataListView
         // 
         this.DataListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this.DataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
         this.DataListView.Location = new System.Drawing.Point(15, 214);
         this.DataListView.Name = "DataListView";
         this.DataListView.Size = new System.Drawing.Size(325, 179);
         this.DataListView.TabIndex = 4;
         this.DataListView.UseCompatibleStateImageBehavior = false;
         this.DataListView.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Time";
         this.columnHeader1.Width = 150;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "Value";
         this.columnHeader2.Width = 150;
         // 
         // RrdDbForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoScroll = true;
         this.ClientSize = new System.Drawing.Size(911, 433);
         this.CloseButtonVisible = false;
         this.Controls.Add(this.propertiesGroupBox);
         this.Controls.Add(this.DataListView);
         this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.Name = "RrdDbForm";
         this.Text = "Database properties";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RrdDbForm_FormClosing);
         this.propertiesGroupBox.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox propertiesGroupBox;
      private System.Windows.Forms.ListView dataSourceListView;
      private System.Windows.Forms.ColumnHeader columnHeader3;
      private System.Windows.Forms.ColumnHeader columnHeader4;
      private System.Windows.Forms.ListView DataListView;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
   }
}