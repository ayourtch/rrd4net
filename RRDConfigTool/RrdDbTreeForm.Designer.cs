namespace RRDConfigTool
{
   partial class RrdDbTreeForm
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
         this.components = new System.ComponentModel.Container();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RrdDbTreeForm));
         this.rrdDbTreeView = new System.Windows.Forms.TreeView();
         this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.copyDatabasedefToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.addDataSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.addArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.modifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.removeFromProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.importDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.createNewToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.removeToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
         this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
         this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.contextMenuStrip1.SuspendLayout();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // rrdDbTreeView
         // 
         this.rrdDbTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.rrdDbTreeView.ContextMenuStrip = this.contextMenuStrip1;
         this.rrdDbTreeView.Location = new System.Drawing.Point(0, 28);
         this.rrdDbTreeView.Name = "rrdDbTreeView";
         this.rrdDbTreeView.Size = new System.Drawing.Size(264, 230);
         this.rrdDbTreeView.TabIndex = 1;
         this.rrdDbTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rrdDbTreeView_AfterSelect);
         this.rrdDbTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.rrdDbTreeView_ItemDrag);
         // 
         // contextMenuStrip1
         // 
         this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyDatabasedefToClipboardToolStripMenuItem,
            this.addDataSourceToolStripMenuItem,
            this.addArchiveToolStripMenuItem,
            this.modifyToolStripMenuItem,
            this.removeFromProjectToolStripMenuItem,
            this.importDataToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem});
         this.contextMenuStrip1.Name = "contextMenuStrip1";
         this.contextMenuStrip1.Size = new System.Drawing.Size(234, 202);
         this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
         // 
         // copyDatabasedefToClipboardToolStripMenuItem
         // 
         this.copyDatabasedefToClipboardToolStripMenuItem.Name = "copyDatabasedefToClipboardToolStripMenuItem";
         this.copyDatabasedefToClipboardToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.copyDatabasedefToClipboardToolStripMenuItem.Text = "Copy databasedef to clipboard";
         this.copyDatabasedefToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyDatabasedefToClipboardToolStripMenuItem_Click);
         // 
         // addDataSourceToolStripMenuItem
         // 
         this.addDataSourceToolStripMenuItem.Name = "addDataSourceToolStripMenuItem";
         this.addDataSourceToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.addDataSourceToolStripMenuItem.Text = "Add Data Source";
         this.addDataSourceToolStripMenuItem.Click += new System.EventHandler(this.addDataSourceToolStripMenuItem_Click);
         // 
         // addArchiveToolStripMenuItem
         // 
         this.addArchiveToolStripMenuItem.Name = "addArchiveToolStripMenuItem";
         this.addArchiveToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.addArchiveToolStripMenuItem.Text = "Add Archive";
         this.addArchiveToolStripMenuItem.Click += new System.EventHandler(this.addArchiveToolStripMenuItem_Click);
         // 
         // modifyToolStripMenuItem
         // 
         this.modifyToolStripMenuItem.Name = "modifyToolStripMenuItem";
         this.modifyToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.modifyToolStripMenuItem.Text = "Modify";
         this.modifyToolStripMenuItem.Click += new System.EventHandler(this.modifyToolStripMenuItem_Click);
         // 
         // removeFromProjectToolStripMenuItem
         // 
         this.removeFromProjectToolStripMenuItem.Name = "removeFromProjectToolStripMenuItem";
         this.removeFromProjectToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.removeFromProjectToolStripMenuItem.Text = "Remove from project";
         this.removeFromProjectToolStripMenuItem.Click += new System.EventHandler(this.removeFromProjectToolStripMenuItem_Click);
         // 
         // importDataToolStripMenuItem
         // 
         this.importDataToolStripMenuItem.Name = "importDataToolStripMenuItem";
         this.importDataToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.importDataToolStripMenuItem.Text = "Import data";
         this.importDataToolStripMenuItem.Click += new System.EventHandler(this.importDataToolStripMenuItem_Click);
         // 
         // toolStrip1
         // 
         this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewToolStripButton,
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.removeToolStripButton});
         this.toolStrip1.Location = new System.Drawing.Point(0, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(264, 25);
         this.toolStrip1.TabIndex = 2;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // createNewToolStripButton
         // 
         this.createNewToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.createNewToolStripButton.Image = global::RRDConfigTool.Properties.Resources.CreateNew;
         this.createNewToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.createNewToolStripButton.Name = "createNewToolStripButton";
         this.createNewToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.createNewToolStripButton.Text = "toolStripButton1";
         this.createNewToolStripButton.Click += new System.EventHandler(this.createNewToolStripButton_Click);
         // 
         // newToolStripButton
         // 
         this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
         this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.newToolStripButton.Name = "newToolStripButton";
         this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.newToolStripButton.Text = "&New";
         this.newToolStripButton.Click += new System.EventHandler(this.newToolStripButton_Click);
         // 
         // openToolStripButton
         // 
         this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
         this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.openToolStripButton.Name = "openToolStripButton";
         this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.openToolStripButton.Text = "&Open";
         this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
         // 
         // saveToolStripButton
         // 
         this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
         this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.saveToolStripButton.Name = "saveToolStripButton";
         this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.saveToolStripButton.Text = "&Save";
         this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
         // 
         // removeToolStripButton
         // 
         this.removeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.removeToolStripButton.Image = global::RRDConfigTool.Properties.Resources.Remove;
         this.removeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.removeToolStripButton.Name = "removeToolStripButton";
         this.removeToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.removeToolStripButton.Text = "toolStripButton1";
         this.removeToolStripButton.Visible = false;
         this.removeToolStripButton.Click += new System.EventHandler(this.removeToolStripButton_Click);
         // 
         // openFileDialog1
         // 
         this.openFileDialog1.FileName = "openFileDialog1";
         // 
         // exportToolStripMenuItem
         // 
         this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
         this.exportToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.exportToolStripMenuItem.Text = "Export";
         this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
         // 
         // importToolStripMenuItem
         // 
         this.importToolStripMenuItem.Name = "importToolStripMenuItem";
         this.importToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
         this.importToolStripMenuItem.Text = "Import";
         this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
         // 
         // RrdDbTreeForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(264, 256);
         this.Controls.Add(this.toolStrip1);
         this.Controls.Add(this.rrdDbTreeView);
         this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.Name = "RrdDbTreeForm";
         this.Text = "Databases";
         this.contextMenuStrip1.ResumeLayout(false);
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TreeView rrdDbTreeView;
      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.OpenFileDialog openFileDialog1;
      private System.Windows.Forms.SaveFileDialog saveFileDialog1;
      private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
      private System.Windows.Forms.ToolStripMenuItem copyDatabasedefToClipboardToolStripMenuItem;
      private System.Windows.Forms.ToolStripButton newToolStripButton;
      private System.Windows.Forms.ToolStripButton openToolStripButton;
      private System.Windows.Forms.ToolStripButton saveToolStripButton;
      private System.Windows.Forms.ToolStripButton createNewToolStripButton;
      private System.Windows.Forms.ToolStripButton removeToolStripButton;
      private System.Windows.Forms.ToolStripMenuItem addDataSourceToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem addArchiveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem modifyToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem removeFromProjectToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem importDataToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
   }
}