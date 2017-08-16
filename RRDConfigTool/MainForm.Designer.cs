namespace RRDConfigTool
{
   partial class MainForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.printPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
         this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.importDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.fetchDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dumpDatabaseDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.graphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.statusStrip1 = new System.Windows.Forms.StatusStrip();
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.rrdDbTreeView = new System.Windows.Forms.TreeView();
         this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.addArchiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.addDatasourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.resizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dumpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.importDataToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
         this.DataListView = new System.Windows.Forms.ListView();
         this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
         this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
         this.dataSourceListView = new System.Windows.Forms.ListView();
         this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
         this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
         this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
         this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
         this.menuStrip1.SuspendLayout();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.contextMenuStrip1.SuspendLayout();
         this.propertiesGroupBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(760, 24);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.createToolStripMenuItem,
            this.toolStripSeparator,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.printToolStripMenuItem,
            this.printPreviewToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
         this.fileToolStripMenuItem.Text = "&File";
         // 
         // newToolStripMenuItem
         // 
         this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
         this.newToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.newToolStripMenuItem.Name = "newToolStripMenuItem";
         this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
         this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.newToolStripMenuItem.Text = "&New";
         this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
         this.openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
         this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.openToolStripMenuItem.Text = "&Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // createToolStripMenuItem
         // 
         this.createToolStripMenuItem.Name = "createToolStripMenuItem";
         this.createToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.createToolStripMenuItem.Text = "Create";
         this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
         // 
         // toolStripSeparator
         // 
         this.toolStripSeparator.Name = "toolStripSeparator";
         this.toolStripSeparator.Size = new System.Drawing.Size(149, 6);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
         this.saveToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.saveToolStripMenuItem.Text = "&Save";
         this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
         // 
         // saveAsToolStripMenuItem
         // 
         this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
         this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.saveAsToolStripMenuItem.Text = "Save &As";
         this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(149, 6);
         // 
         // printToolStripMenuItem
         // 
         this.printToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printToolStripMenuItem.Image")));
         this.printToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.printToolStripMenuItem.Name = "printToolStripMenuItem";
         this.printToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
         this.printToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.printToolStripMenuItem.Text = "&Print";
         // 
         // printPreviewToolStripMenuItem
         // 
         this.printPreviewToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("printPreviewToolStripMenuItem.Image")));
         this.printPreviewToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.printPreviewToolStripMenuItem.Name = "printPreviewToolStripMenuItem";
         this.printPreviewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.printPreviewToolStripMenuItem.Text = "Print Pre&view";
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
         // 
         // exitToolStripMenuItem
         // 
         this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
         this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
         this.exitToolStripMenuItem.Text = "E&xit";
         this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
         // 
         // editToolStripMenuItem
         // 
         this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator3,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator4,
            this.selectAllToolStripMenuItem});
         this.editToolStripMenuItem.Name = "editToolStripMenuItem";
         this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
         this.editToolStripMenuItem.Text = "&Edit";
         // 
         // undoToolStripMenuItem
         // 
         this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
         this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
         this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.undoToolStripMenuItem.Text = "&Undo";
         // 
         // redoToolStripMenuItem
         // 
         this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
         this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
         this.redoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.redoToolStripMenuItem.Text = "&Redo";
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
         // 
         // cutToolStripMenuItem
         // 
         this.cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cutToolStripMenuItem.Image")));
         this.cutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
         this.cutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
         this.cutToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.cutToolStripMenuItem.Text = "Cu&t";
         // 
         // copyToolStripMenuItem
         // 
         this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
         this.copyToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
         this.copyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
         this.copyToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.copyToolStripMenuItem.Text = "&Copy";
         // 
         // pasteToolStripMenuItem
         // 
         this.pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("pasteToolStripMenuItem.Image")));
         this.pasteToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
         this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
         this.pasteToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.pasteToolStripMenuItem.Text = "&Paste";
         // 
         // toolStripSeparator4
         // 
         this.toolStripSeparator4.Name = "toolStripSeparator4";
         this.toolStripSeparator4.Size = new System.Drawing.Size(141, 6);
         // 
         // selectAllToolStripMenuItem
         // 
         this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
         this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
         this.selectAllToolStripMenuItem.Text = "Select &All";
         // 
         // toolsToolStripMenuItem
         // 
         this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.importDataToolStripMenuItem,
            this.fetchDataToolStripMenuItem,
            this.dumpDatabaseDefinitionToolStripMenuItem,
            this.graphToolStripMenuItem});
         this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
         this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
         this.toolsToolStripMenuItem.Text = "&Tools";
         // 
         // customizeToolStripMenuItem
         // 
         this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
         this.customizeToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.customizeToolStripMenuItem.Text = "&Customize";
         // 
         // optionsToolStripMenuItem
         // 
         this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
         this.optionsToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.optionsToolStripMenuItem.Text = "&Options";
         // 
         // importDataToolStripMenuItem
         // 
         this.importDataToolStripMenuItem.Name = "importDataToolStripMenuItem";
         this.importDataToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.importDataToolStripMenuItem.Text = "Import data";
         this.importDataToolStripMenuItem.Click += new System.EventHandler(this.importDataToolStripMenuItem_Click);
         // 
         // fetchDataToolStripMenuItem
         // 
         this.fetchDataToolStripMenuItem.Name = "fetchDataToolStripMenuItem";
         this.fetchDataToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.fetchDataToolStripMenuItem.Text = "Fetch data";
         this.fetchDataToolStripMenuItem.Click += new System.EventHandler(this.fetchDataToolStripMenuItem_Click);
         // 
         // dumpDatabaseDefinitionToolStripMenuItem
         // 
         this.dumpDatabaseDefinitionToolStripMenuItem.Name = "dumpDatabaseDefinitionToolStripMenuItem";
         this.dumpDatabaseDefinitionToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.dumpDatabaseDefinitionToolStripMenuItem.Text = "Dump database definition";
         this.dumpDatabaseDefinitionToolStripMenuItem.Click += new System.EventHandler(this.dumpDatabaseDefinitionToolStripMenuItem_Click);
         // 
         // graphToolStripMenuItem
         // 
         this.graphToolStripMenuItem.Name = "graphToolStripMenuItem";
         this.graphToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
         this.graphToolStripMenuItem.Text = "Graph";
         this.graphToolStripMenuItem.Click += new System.EventHandler(this.graphToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
         this.helpToolStripMenuItem.Text = "&Help";
         // 
         // contentsToolStripMenuItem
         // 
         this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
         this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
         this.contentsToolStripMenuItem.Text = "&Contents";
         // 
         // indexToolStripMenuItem
         // 
         this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
         this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
         this.indexToolStripMenuItem.Text = "&Index";
         // 
         // searchToolStripMenuItem
         // 
         this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
         this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
         this.searchToolStripMenuItem.Text = "&Search";
         // 
         // toolStripSeparator5
         // 
         this.toolStripSeparator5.Name = "toolStripSeparator5";
         this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
         this.aboutToolStripMenuItem.Text = "&About...";
         this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
         // 
         // statusStrip1
         // 
         this.statusStrip1.Location = new System.Drawing.Point(0, 422);
         this.statusStrip1.Name = "statusStrip1";
         this.statusStrip1.Size = new System.Drawing.Size(760, 22);
         this.statusStrip1.TabIndex = 1;
         this.statusStrip1.Text = "statusStrip1";
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(0, 24);
         this.splitContainer1.Name = "splitContainer1";
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.rrdDbTreeView);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.DataListView);
         this.splitContainer1.Panel2.Controls.Add(this.propertiesGroupBox);
         this.splitContainer1.Size = new System.Drawing.Size(760, 398);
         this.splitContainer1.SplitterDistance = 253;
         this.splitContainer1.TabIndex = 2;
         // 
         // rrdDbTreeView
         // 
         this.rrdDbTreeView.ContextMenuStrip = this.contextMenuStrip1;
         this.rrdDbTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
         this.rrdDbTreeView.Location = new System.Drawing.Point(0, 0);
         this.rrdDbTreeView.Name = "rrdDbTreeView";
         this.rrdDbTreeView.Size = new System.Drawing.Size(253, 398);
         this.rrdDbTreeView.TabIndex = 0;
         this.rrdDbTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rrdDbTreeView_AfterSelect);
         // 
         // contextMenuStrip1
         // 
         this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addArchiveToolStripMenuItem,
            this.addDatasourceToolStripMenuItem,
            this.editToolStripMenuItem1,
            this.resizeToolStripMenuItem,
            this.dumpToolStripMenuItem,
            this.importDataToolStripMenuItem1});
         this.contextMenuStrip1.Name = "contextMenuStrip1";
         this.contextMenuStrip1.Size = new System.Drawing.Size(158, 136);
         this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
         // 
         // addArchiveToolStripMenuItem
         // 
         this.addArchiveToolStripMenuItem.Name = "addArchiveToolStripMenuItem";
         this.addArchiveToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         this.addArchiveToolStripMenuItem.Text = "Add archive";
         this.addArchiveToolStripMenuItem.Click += new System.EventHandler(this.addArchiveToolStripMenuItem_Click);
         // 
         // addDatasourceToolStripMenuItem
         // 
         this.addDatasourceToolStripMenuItem.Name = "addDatasourceToolStripMenuItem";
         this.addDatasourceToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         this.addDatasourceToolStripMenuItem.Text = "Add datasource";
         this.addDatasourceToolStripMenuItem.Click += new System.EventHandler(this.addDatasourceToolStripMenuItem_Click);
         // 
         // editToolStripMenuItem1
         // 
         this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
         this.editToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
         this.editToolStripMenuItem1.Text = "Edit";
         this.editToolStripMenuItem1.Click += new System.EventHandler(this.editToolStripMenuItem1_Click);
         // 
         // resizeToolStripMenuItem
         // 
         this.resizeToolStripMenuItem.Name = "resizeToolStripMenuItem";
         this.resizeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         this.resizeToolStripMenuItem.Text = "Resize";
         this.resizeToolStripMenuItem.Click += new System.EventHandler(this.resizeToolStripMenuItem_Click);
         // 
         // dumpToolStripMenuItem
         // 
         this.dumpToolStripMenuItem.Name = "dumpToolStripMenuItem";
         this.dumpToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
         this.dumpToolStripMenuItem.Text = "Dump";
         this.dumpToolStripMenuItem.Click += new System.EventHandler(this.dumpToolStripMenuItem_Click);
         // 
         // importDataToolStripMenuItem1
         // 
         this.importDataToolStripMenuItem1.Name = "importDataToolStripMenuItem1";
         this.importDataToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
         this.importDataToolStripMenuItem1.Text = "Import data";
         this.importDataToolStripMenuItem1.Click += new System.EventHandler(this.importDataToolStripMenuItem1_Click);
         // 
         // DataListView
         // 
         this.DataListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this.DataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
         this.DataListView.Location = new System.Drawing.Point(17, 216);
         this.DataListView.Name = "DataListView";
         this.DataListView.Size = new System.Drawing.Size(324, 179);
         this.DataListView.TabIndex = 2;
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
         // propertiesGroupBox
         // 
         this.propertiesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.propertiesGroupBox.Controls.Add(this.dataSourceListView);
         this.propertiesGroupBox.Location = new System.Drawing.Point(14, 13);
         this.propertiesGroupBox.Name = "propertiesGroupBox";
         this.propertiesGroupBox.Size = new System.Drawing.Size(477, 185);
         this.propertiesGroupBox.TabIndex = 1;
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
         this.dataSourceListView.Size = new System.Drawing.Size(471, 166);
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
         // openFileDialog1
         // 
         this.openFileDialog1.FileName = "openFileDialog1";
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(760, 444);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.statusStrip1);
         this.Controls.Add(this.menuStrip1);
         this.Name = "MainForm";
         this.Text = "RrdDb Configuration";
         this.Load += new System.EventHandler(this.MainForm_Load);
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.ResumeLayout(false);
         this.contextMenuStrip1.ResumeLayout(false);
         this.propertiesGroupBox.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem printPreviewToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
      private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
      private System.Windows.Forms.StatusStrip statusStrip1;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.TreeView rrdDbTreeView;
      private System.Windows.Forms.OpenFileDialog openFileDialog1;
      private System.Windows.Forms.GroupBox propertiesGroupBox;
      private System.Windows.Forms.ListView dataSourceListView;
      private System.Windows.Forms.ColumnHeader columnHeader3;
      private System.Windows.Forms.ColumnHeader columnHeader4;
      private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
      private System.Windows.Forms.ToolStripMenuItem addArchiveToolStripMenuItem;
      private System.Windows.Forms.SaveFileDialog saveFileDialog1;
      private System.Windows.Forms.ToolStripMenuItem importDataToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem fetchDataToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem addDatasourceToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem resizeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem dumpToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem importDataToolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem dumpDatabaseDefinitionToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem graphToolStripMenuItem;
      private System.Windows.Forms.ListView DataListView;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;

   }
}

