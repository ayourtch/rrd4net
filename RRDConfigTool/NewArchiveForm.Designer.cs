namespace RRDConfigTool
{
   partial class NewArchiveForm
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
         this.groupBox3 = new System.Windows.Forms.GroupBox();
         this.dbStepsTextBox = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.rowsTextBox = new System.Windows.Forms.TextBox();
         this.steppsTextBox = new System.Windows.Forms.TextBox();
         this.xxfTextBox = new System.Windows.Forms.TextBox();
         this.label12 = new System.Windows.Forms.Label();
         this.label11 = new System.Windows.Forms.Label();
         this.label10 = new System.Windows.Forms.Label();
         this.archiveTypeComboBox = new System.Windows.Forms.ComboBox();
         this.label9 = new System.Windows.Forms.Label();
         this.addButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
         this.defLabel = new System.Windows.Forms.Label();
         this.groupBox3.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
         this.SuspendLayout();
         // 
         // groupBox3
         // 
         this.groupBox3.Controls.Add(this.dbStepsTextBox);
         this.groupBox3.Controls.Add(this.label1);
         this.groupBox3.Controls.Add(this.rowsTextBox);
         this.groupBox3.Controls.Add(this.steppsTextBox);
         this.groupBox3.Controls.Add(this.xxfTextBox);
         this.groupBox3.Controls.Add(this.label12);
         this.groupBox3.Controls.Add(this.label11);
         this.groupBox3.Controls.Add(this.label10);
         this.groupBox3.Controls.Add(this.archiveTypeComboBox);
         this.groupBox3.Controls.Add(this.label9);
         this.groupBox3.Location = new System.Drawing.Point(12, 12);
         this.groupBox3.Name = "groupBox3";
         this.groupBox3.Size = new System.Drawing.Size(178, 188);
         this.groupBox3.TabIndex = 7;
         this.groupBox3.TabStop = false;
         this.groupBox3.Text = "Archive";
         // 
         // dbStepsTextBox
         // 
         this.dbStepsTextBox.Enabled = false;
         this.dbStepsTextBox.Location = new System.Drawing.Point(97, 117);
         this.dbStepsTextBox.Name = "dbStepsTextBox";
         this.dbStepsTextBox.Size = new System.Drawing.Size(75, 20);
         this.dbStepsTextBox.TabIndex = 9;
         this.dbStepsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(94, 101);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(49, 13);
         this.label1.TabIndex = 8;
         this.label1.Text = "Db steps";
         // 
         // rowsTextBox
         // 
         this.rowsTextBox.Location = new System.Drawing.Point(10, 156);
         this.rowsTextBox.Name = "rowsTextBox";
         this.rowsTextBox.Size = new System.Drawing.Size(75, 20);
         this.rowsTextBox.TabIndex = 7;
         this.rowsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.rowsTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
         this.rowsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeSpanTextBox_Validating);
         // 
         // steppsTextBox
         // 
         this.steppsTextBox.Location = new System.Drawing.Point(10, 117);
         this.steppsTextBox.Name = "steppsTextBox";
         this.steppsTextBox.Size = new System.Drawing.Size(75, 20);
         this.steppsTextBox.TabIndex = 6;
         this.steppsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.steppsTextBox.Validated += new System.EventHandler(this.TextBox_Validated);
         this.steppsTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.timeSpanTextBox_Validating);
         // 
         // xxfTextBox
         // 
         this.xxfTextBox.Location = new System.Drawing.Point(10, 75);
         this.xxfTextBox.Name = "xxfTextBox";
         this.xxfTextBox.Size = new System.Drawing.Size(75, 20);
         this.xxfTextBox.TabIndex = 5;
         this.xxfTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.xxfTextBox.TextChanged += new System.EventHandler(this.xxfTextBox_TextChanged);
         // 
         // label12
         // 
         this.label12.AutoSize = true;
         this.label12.Location = new System.Drawing.Point(7, 140);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(34, 13);
         this.label12.TabIndex = 4;
         this.label12.Text = "Rows";
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(7, 101);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(34, 13);
         this.label11.TabIndex = 3;
         this.label11.Text = "Steps";
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Location = new System.Drawing.Point(7, 58);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(20, 13);
         this.label10.TabIndex = 2;
         this.label10.Text = "xxf";
         // 
         // archiveTypeComboBox
         // 
         this.archiveTypeComboBox.FormattingEnabled = true;
         this.archiveTypeComboBox.Location = new System.Drawing.Point(10, 34);
         this.archiveTypeComboBox.Name = "archiveTypeComboBox";
         this.archiveTypeComboBox.Size = new System.Drawing.Size(121, 21);
         this.archiveTypeComboBox.TabIndex = 1;
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(7, 20);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(63, 13);
         this.label9.TabIndex = 0;
         this.label9.Text = "Archivetype";
         // 
         // addButton
         // 
         this.addButton.Location = new System.Drawing.Point(16, 244);
         this.addButton.Name = "addButton";
         this.addButton.Size = new System.Drawing.Size(75, 23);
         this.addButton.TabIndex = 8;
         this.addButton.Text = "Add";
         this.addButton.UseVisualStyleBackColor = true;
         this.addButton.Click += new System.EventHandler(this.addButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.Location = new System.Drawing.Point(109, 244);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(75, 23);
         this.cancelButton.TabIndex = 9;
         this.cancelButton.Text = "Cancel";
         this.cancelButton.UseVisualStyleBackColor = true;
         this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
         // 
         // errorProvider1
         // 
         this.errorProvider1.ContainerControl = this;
         // 
         // defLabel
         // 
         this.defLabel.AutoSize = true;
         this.defLabel.Location = new System.Drawing.Point(19, 215);
         this.defLabel.Name = "defLabel";
         this.defLabel.Size = new System.Drawing.Size(33, 13);
         this.defLabel.TabIndex = 10;
         this.defLabel.Text = "RRA:";
         // 
         // NewArchiveForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(205, 290);
         this.Controls.Add(this.defLabel);
         this.Controls.Add(this.cancelButton);
         this.Controls.Add(this.addButton);
         this.Controls.Add(this.groupBox3);
         this.Name = "NewArchiveForm";
         this.Text = "NewArchiveForm";
         this.Load += new System.EventHandler(this.NewArchiveForm_Load);
         this.groupBox3.ResumeLayout(false);
         this.groupBox3.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.GroupBox groupBox3;
      private System.Windows.Forms.TextBox rowsTextBox;
      private System.Windows.Forms.TextBox steppsTextBox;
      private System.Windows.Forms.TextBox xxfTextBox;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.ComboBox archiveTypeComboBox;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Button addButton;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.ErrorProvider errorProvider1;
      private System.Windows.Forms.TextBox dbStepsTextBox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label defLabel;
   }
}