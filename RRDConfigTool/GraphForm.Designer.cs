namespace RRDConfigTool
{
   partial class GraphForm
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
         this.graphBox = new System.Windows.Forms.PictureBox();
         this.loadButton = new System.Windows.Forms.Button();
         this.graphCommandTextBox = new System.Windows.Forms.TextBox();
         this.startTimelabel = new System.Windows.Forms.Label();
         this.startTimeTextBox = new System.Windows.Forms.TextBox();
         this.endTimeTextBox = new System.Windows.Forms.TextBox();
         this.endTimeLabel = new System.Windows.Forms.Label();
         this.saveButton = new System.Windows.Forms.Button();
         this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
         this.label3 = new System.Windows.Forms.Label();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
         ((System.ComponentModel.ISupportInitialize)(this.graphBox)).BeginInit();
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
         this.SuspendLayout();
         // 
         // graphBox
         // 
         this.graphBox.Location = new System.Drawing.Point(12, 25);
         this.graphBox.Name = "graphBox";
         this.graphBox.Size = new System.Drawing.Size(925, 300);
         this.graphBox.TabIndex = 0;
         this.graphBox.TabStop = false;
         // 
         // loadButton
         // 
         this.loadButton.Location = new System.Drawing.Point(6, 107);
         this.loadButton.Name = "loadButton";
         this.loadButton.Size = new System.Drawing.Size(75, 23);
         this.loadButton.TabIndex = 1;
         this.loadButton.Text = "Show graph";
         this.loadButton.UseVisualStyleBackColor = true;
         this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
         // 
         // graphCommandTextBox
         // 
         this.graphCommandTextBox.AllowDrop = true;
         this.graphCommandTextBox.Location = new System.Drawing.Point(12, 370);
         this.graphCommandTextBox.Multiline = true;
         this.graphCommandTextBox.Name = "graphCommandTextBox";
         this.graphCommandTextBox.Size = new System.Drawing.Size(629, 255);
         this.graphCommandTextBox.TabIndex = 2;
         this.graphCommandTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.graphCommandTextBox_DragDrop);
         this.graphCommandTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.graphCommandTextBox_DragEnter);
         // 
         // startTimelabel
         // 
         this.startTimelabel.AutoSize = true;
         this.startTimelabel.Location = new System.Drawing.Point(6, 25);
         this.startTimelabel.Name = "startTimelabel";
         this.startTimelabel.Size = new System.Drawing.Size(51, 13);
         this.startTimelabel.TabIndex = 3;
         this.startTimelabel.Text = "Start time";
         // 
         // startTimeTextBox
         // 
         this.startTimeTextBox.Location = new System.Drawing.Point(9, 41);
         this.startTimeTextBox.Name = "startTimeTextBox";
         this.startTimeTextBox.Size = new System.Drawing.Size(124, 20);
         this.startTimeTextBox.TabIndex = 4;
         // 
         // endTimeTextBox
         // 
         this.endTimeTextBox.Location = new System.Drawing.Point(9, 81);
         this.endTimeTextBox.Name = "endTimeTextBox";
         this.endTimeTextBox.Size = new System.Drawing.Size(124, 20);
         this.endTimeTextBox.TabIndex = 6;
         // 
         // endTimeLabel
         // 
         this.endTimeLabel.AutoSize = true;
         this.endTimeLabel.Location = new System.Drawing.Point(6, 64);
         this.endTimeLabel.Name = "endTimeLabel";
         this.endTimeLabel.Size = new System.Drawing.Size(48, 13);
         this.endTimeLabel.TabIndex = 5;
         this.endTimeLabel.Text = "End time";
         // 
         // saveButton
         // 
         this.saveButton.Location = new System.Drawing.Point(647, 592);
         this.saveButton.Name = "saveButton";
         this.saveButton.Size = new System.Drawing.Size(47, 23);
         this.saveButton.TabIndex = 10;
         this.saveButton.Text = "Save";
         this.saveButton.UseVisualStyleBackColor = true;
         this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(12, 354);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(51, 13);
         this.label3.TabIndex = 12;
         this.label3.Text = "Definition";
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.startTimeTextBox);
         this.groupBox1.Controls.Add(this.startTimelabel);
         this.groupBox1.Controls.Add(this.endTimeLabel);
         this.groupBox1.Controls.Add(this.loadButton);
         this.groupBox1.Controls.Add(this.endTimeTextBox);
         this.groupBox1.Location = new System.Drawing.Point(737, 370);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(167, 141);
         this.groupBox1.TabIndex = 13;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Time settings";
         // 
         // errorProvider1
         // 
         this.errorProvider1.ContainerControl = this;
         // 
         // GraphForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.AutoScroll = true;
         this.ClientSize = new System.Drawing.Size(957, 638);
         this.Controls.Add(this.groupBox1);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.saveButton);
         this.Controls.Add(this.graphCommandTextBox);
         this.Controls.Add(this.graphBox);
         this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.Name = "GraphForm";
         this.Text = "Graph";
         this.Load += new System.EventHandler(this.GraphForm_Load);
         ((System.ComponentModel.ISupportInitialize)(this.graphBox)).EndInit();
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox graphBox;
      private System.Windows.Forms.Button loadButton;
      private System.Windows.Forms.TextBox graphCommandTextBox;
      private System.Windows.Forms.Label startTimelabel;
      private System.Windows.Forms.TextBox startTimeTextBox;
      private System.Windows.Forms.TextBox endTimeTextBox;
      private System.Windows.Forms.Label endTimeLabel;
      private System.Windows.Forms.Button saveButton;
      private System.Windows.Forms.SaveFileDialog saveFileDialog1;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.ErrorProvider errorProvider1;
   }
}