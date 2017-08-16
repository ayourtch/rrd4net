namespace RRDConfigTool
{
   partial class GraphLeftForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphLeftForm));
         this.toolStrip1 = new System.Windows.Forms.ToolStrip();
         this.newToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
         this.cutToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.copyToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.pasteToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
         this.graphListView = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.toolStrip1.SuspendLayout();
         this.SuspendLayout();
         // 
         // toolStrip1
         // 
         this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripButton,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.printToolStripButton,
            this.toolStripSeparator,
            this.cutToolStripButton,
            this.copyToolStripButton,
            this.pasteToolStripButton,
            this.toolStripSeparator1,
            this.helpToolStripButton});
         this.toolStrip1.Location = new System.Drawing.Point(0, 0);
         this.toolStrip1.Name = "toolStrip1";
         this.toolStrip1.Size = new System.Drawing.Size(292, 25);
         this.toolStrip1.TabIndex = 0;
         this.toolStrip1.Text = "toolStrip1";
         // 
         // newToolStripButton
         // 
         this.newToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.newToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripButton.Image")));
         this.newToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.newToolStripButton.Name = "newToolStripButton";
         this.newToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.newToolStripButton.Text = "&New";
         // 
         // openToolStripButton
         // 
         this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.openToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripButton.Image")));
         this.openToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.openToolStripButton.Name = "openToolStripButton";
         this.openToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.openToolStripButton.Text = "&Open";
         // 
         // saveToolStripButton
         // 
         this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.saveToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripButton.Image")));
         this.saveToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.saveToolStripButton.Name = "saveToolStripButton";
         this.saveToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.saveToolStripButton.Text = "&Save";
         // 
         // printToolStripButton
         // 
         this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.printToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripButton.Image")));
         this.printToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.printToolStripButton.Name = "printToolStripButton";
         this.printToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.printToolStripButton.Text = "&Print";
         // 
         // toolStripSeparator
         // 
         this.toolStripSeparator.Name = "toolStripSeparator";
         this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
         // 
         // cutToolStripButton
         // 
         this.cutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.cutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripButton.Image")));
         this.cutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.cutToolStripButton.Name = "cutToolStripButton";
         this.cutToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.cutToolStripButton.Text = "C&ut";
         // 
         // copyToolStripButton
         // 
         this.copyToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.copyToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripButton.Image")));
         this.copyToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.copyToolStripButton.Name = "copyToolStripButton";
         this.copyToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.copyToolStripButton.Text = "&Copy";
         // 
         // pasteToolStripButton
         // 
         this.pasteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.pasteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripButton.Image")));
         this.pasteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.pasteToolStripButton.Name = "pasteToolStripButton";
         this.pasteToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.pasteToolStripButton.Text = "&Paste";
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
         // 
         // helpToolStripButton
         // 
         this.helpToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
         this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
         this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.helpToolStripButton.Name = "helpToolStripButton";
         this.helpToolStripButton.Size = new System.Drawing.Size(23, 22);
         this.helpToolStripButton.Text = "He&lp";
         // 
         // graphListView
         // 
         this.graphListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
         this.graphListView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.graphListView.Location = new System.Drawing.Point(0, 25);
         this.graphListView.MultiSelect = false;
         this.graphListView.Name = "graphListView";
         this.graphListView.Size = new System.Drawing.Size(292, 241);
         this.graphListView.TabIndex = 1;
         this.graphListView.UseCompatibleStateImageBehavior = false;
         this.graphListView.View = System.Windows.Forms.View.Details;
         this.graphListView.ItemActivate += new System.EventHandler(this.graphListView_ItemActivate);
         this.graphListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.graphListView_MouseClick);
         this.graphListView.SelectedIndexChanged += new System.EventHandler(this.graphListView_SelectedIndexChanged);
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "Name";
         this.columnHeader1.Width = 246;
         // 
         // GraphLeftForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(292, 266);
         this.Controls.Add(this.graphListView);
         this.Controls.Add(this.toolStrip1);
         this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.Name = "GraphLeftForm";
         this.Text = "Graph definitions";
         this.Load += new System.EventHandler(this.GraphLeftForm_Load);
         this.toolStrip1.ResumeLayout(false);
         this.toolStrip1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ToolStrip toolStrip1;
      private System.Windows.Forms.ToolStripButton newToolStripButton;
      private System.Windows.Forms.ToolStripButton openToolStripButton;
      private System.Windows.Forms.ToolStripButton saveToolStripButton;
      private System.Windows.Forms.ToolStripButton printToolStripButton;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
      private System.Windows.Forms.ToolStripButton cutToolStripButton;
      private System.Windows.Forms.ToolStripButton copyToolStripButton;
      private System.Windows.Forms.ToolStripButton pasteToolStripButton;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripButton helpToolStripButton;
      private System.Windows.Forms.ListView graphListView;
      private System.Windows.Forms.ColumnHeader columnHeader1;
   }
}