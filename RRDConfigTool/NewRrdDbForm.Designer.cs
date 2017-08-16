namespace RRDConfigTool
{
   partial class NewRrdDbForm
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
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.databaseNameTextBox = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.label13 = new System.Windows.Forms.Label();
         this.stepTextBox = new System.Windows.Forms.TextBox();
         this.startTimeTextBox = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.createRrDefButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
         this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
         this.groupBox1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
         this.SuspendLayout();
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.databaseNameTextBox);
         this.groupBox1.Controls.Add(this.label1);
         this.groupBox1.Controls.Add(this.label13);
         this.groupBox1.Controls.Add(this.stepTextBox);
         this.groupBox1.Controls.Add(this.startTimeTextBox);
         this.groupBox1.Controls.Add(this.label3);
         this.groupBox1.Controls.Add(this.label2);
         this.groupBox1.Location = new System.Drawing.Point(12, 12);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(367, 162);
         this.groupBox1.TabIndex = 4;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Database";
         // 
         // databaseNameTextBox
         // 
         this.databaseNameTextBox.Location = new System.Drawing.Point(12, 34);
         this.databaseNameTextBox.Name = "databaseNameTextBox";
         this.databaseNameTextBox.Size = new System.Drawing.Size(165, 20);
         this.databaseNameTextBox.TabIndex = 1;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(9, 18);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(35, 13);
         this.label1.TabIndex = 14;
         this.label1.Text = "Name";
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(165, 128);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(197, 13);
         this.label13.TabIndex = 10;
         this.label13.Text = "Seconds or days.hours:minutes:seconds";
         // 
         // stepTextBox
         // 
         this.stepTextBox.Location = new System.Drawing.Point(12, 125);
         this.stepTextBox.Name = "stepTextBox";
         this.stepTextBox.Size = new System.Drawing.Size(136, 20);
         this.stepTextBox.TabIndex = 3;
         this.stepTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // startTimeTextBox
         // 
         this.startTimeTextBox.Location = new System.Drawing.Point(12, 81);
         this.startTimeTextBox.Name = "startTimeTextBox";
         this.startTimeTextBox.Size = new System.Drawing.Size(136, 20);
         this.startTimeTextBox.TabIndex = 2;
         this.startTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(9, 109);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(29, 13);
         this.label3.TabIndex = 5;
         this.label3.Text = "Step";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(9, 65);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(51, 13);
         this.label2.TabIndex = 4;
         this.label2.Text = "Start time";
         // 
         // createRrDefButton
         // 
         this.createRrDefButton.Location = new System.Drawing.Point(210, 180);
         this.createRrDefButton.Name = "createRrDefButton";
         this.createRrDefButton.Size = new System.Drawing.Size(75, 23);
         this.createRrDefButton.TabIndex = 4;
         this.createRrDefButton.Text = "Create";
         this.createRrDefButton.UseVisualStyleBackColor = true;
         this.createRrDefButton.Click += new System.EventHandler(this.createRrDefButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.cancelButton.Location = new System.Drawing.Point(300, 180);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 5;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // errorProvider1
         // 
         this.errorProvider1.ContainerControl = this;
         // 
         // NewRrdDbForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.cancelButton;
         this.ClientSize = new System.Drawing.Size(387, 211);
         this.Controls.Add(this.cancelButton);
         this.Controls.Add(this.createRrDefButton);
         this.Controls.Add(this.groupBox1);
         this.Name = "NewRrdDbForm";
         this.Text = "New RrdDb";
         this.Load += new System.EventHandler(this.NewRrdDbForm_Load);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.TextBox stepTextBox;
      private System.Windows.Forms.TextBox startTimeTextBox;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
      private System.Windows.Forms.ErrorProvider errorProvider1;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Button createRrDefButton;
      private System.Windows.Forms.TextBox databaseNameTextBox;
      private System.Windows.Forms.Label label1;

   }
}