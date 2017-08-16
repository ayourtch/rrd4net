namespace RRDConfigTool
{
   partial class NewDatasourceForm
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
         this.createButton = new System.Windows.Forms.Button();
         this.groupBox2 = new System.Windows.Forms.GroupBox();
         this.databaseStepTextBox = new System.Windows.Forms.TextBox();
         this.label1 = new System.Windows.Forms.Label();
         this.heartbeatTextBox = new System.Windows.Forms.TextBox();
         this.dataSourceNameTextBox = new System.Windows.Forms.TextBox();
         this.maxValueTextBox = new System.Windows.Forms.TextBox();
         this.minValueTextBox = new System.Windows.Forms.TextBox();
         this.label8 = new System.Windows.Forms.Label();
         this.label7 = new System.Windows.Forms.Label();
         this.label6 = new System.Windows.Forms.Label();
         this.dataSourceTypeComboBox = new System.Windows.Forms.ComboBox();
         this.label4 = new System.Windows.Forms.Label();
         this.label5 = new System.Windows.Forms.Label();
         this.FinishButton = new System.Windows.Forms.Button();
         this.cancelButton = new System.Windows.Forms.Button();
         this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
         this.defLabel = new System.Windows.Forms.Label();
         this.groupBox2.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
         this.SuspendLayout();
         // 
         // createButton
         // 
         this.createButton.Location = new System.Drawing.Point(10, 279);
         this.createButton.Name = "createButton";
         this.createButton.Size = new System.Drawing.Size(62, 23);
         this.createButton.TabIndex = 9;
         this.createButton.Text = "Create";
         this.createButton.UseVisualStyleBackColor = true;
         this.createButton.Click += new System.EventHandler(this.createButton_Click);
         // 
         // groupBox2
         // 
         this.groupBox2.Controls.Add(this.databaseStepTextBox);
         this.groupBox2.Controls.Add(this.label1);
         this.groupBox2.Controls.Add(this.heartbeatTextBox);
         this.groupBox2.Controls.Add(this.dataSourceNameTextBox);
         this.groupBox2.Controls.Add(this.maxValueTextBox);
         this.groupBox2.Controls.Add(this.minValueTextBox);
         this.groupBox2.Controls.Add(this.label8);
         this.groupBox2.Controls.Add(this.label7);
         this.groupBox2.Controls.Add(this.label6);
         this.groupBox2.Controls.Add(this.dataSourceTypeComboBox);
         this.groupBox2.Controls.Add(this.label4);
         this.groupBox2.Controls.Add(this.label5);
         this.groupBox2.Location = new System.Drawing.Point(12, 12);
         this.groupBox2.Name = "groupBox2";
         this.groupBox2.Size = new System.Drawing.Size(204, 220);
         this.groupBox2.TabIndex = 8;
         this.groupBox2.TabStop = false;
         this.groupBox2.Text = "Datasource";
         // 
         // databaseStepTextBox
         // 
         this.databaseStepTextBox.Enabled = false;
         this.databaseStepTextBox.Location = new System.Drawing.Point(129, 117);
         this.databaseStepTextBox.Name = "databaseStepTextBox";
         this.databaseStepTextBox.Size = new System.Drawing.Size(56, 20);
         this.databaseStepTextBox.TabIndex = 14;
         this.databaseStepTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(126, 101);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(29, 13);
         this.label1.TabIndex = 13;
         this.label1.Text = "Step";
         // 
         // heartbeatTextBox
         // 
         this.heartbeatTextBox.Location = new System.Drawing.Point(10, 117);
         this.heartbeatTextBox.Name = "heartbeatTextBox";
         this.heartbeatTextBox.Size = new System.Drawing.Size(100, 20);
         this.heartbeatTextBox.TabIndex = 12;
         this.heartbeatTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.heartbeatTextBox.TextChanged += new System.EventHandler(this.heartbeatTextBox_TextChanged);
         this.heartbeatTextBox.Validated += new System.EventHandler(this.heartbeatTextBox_Validated);
         this.heartbeatTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.heartbeatTextBox_Validating);
         // 
         // dataSourceNameTextBox
         // 
         this.dataSourceNameTextBox.Location = new System.Drawing.Point(10, 36);
         this.dataSourceNameTextBox.Name = "dataSourceNameTextBox";
         this.dataSourceNameTextBox.Size = new System.Drawing.Size(121, 20);
         this.dataSourceNameTextBox.TabIndex = 11;
         this.dataSourceNameTextBox.TextChanged += new System.EventHandler(this.dataSourceNameTextBox_TextChanged);
         this.dataSourceNameTextBox.Validated += new System.EventHandler(this.double_Validated);
         this.dataSourceNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.dataSourceNameTextBox_Validating);
         // 
         // maxValueTextBox
         // 
         this.maxValueTextBox.Location = new System.Drawing.Point(9, 195);
         this.maxValueTextBox.Name = "maxValueTextBox";
         this.maxValueTextBox.Size = new System.Drawing.Size(100, 20);
         this.maxValueTextBox.TabIndex = 10;
         this.maxValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.maxValueTextBox.TextChanged += new System.EventHandler(this.maxValueTextBox_TextChanged);
         this.maxValueTextBox.Validated += new System.EventHandler(this.double_Validated);
         this.maxValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.double_Validating);
         // 
         // minValueTextBox
         // 
         this.minValueTextBox.Location = new System.Drawing.Point(10, 156);
         this.minValueTextBox.Name = "minValueTextBox";
         this.minValueTextBox.Size = new System.Drawing.Size(100, 20);
         this.minValueTextBox.TabIndex = 9;
         this.minValueTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         this.minValueTextBox.TextChanged += new System.EventHandler(this.minValueTextBox_TextChanged);
         this.minValueTextBox.Validated += new System.EventHandler(this.double_Validated);
         this.minValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.double_Validating);
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(11, 179);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(53, 13);
         this.label8.TabIndex = 8;
         this.label8.Text = "Maxvalue";
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(11, 140);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(50, 13);
         this.label7.TabIndex = 7;
         this.label7.Text = "Minvalue";
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(7, 101);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(54, 13);
         this.label6.TabIndex = 6;
         this.label6.Text = "Heartbeat";
         // 
         // dataSourceTypeComboBox
         // 
         this.dataSourceTypeComboBox.FormattingEnabled = true;
         this.dataSourceTypeComboBox.Location = new System.Drawing.Point(10, 75);
         this.dataSourceTypeComboBox.Name = "dataSourceTypeComboBox";
         this.dataSourceTypeComboBox.Size = new System.Drawing.Size(121, 21);
         this.dataSourceTypeComboBox.TabIndex = 5;
         this.dataSourceTypeComboBox.SelectionChangeCommitted += new System.EventHandler(this.dataSourceTypeComboBox_SelectionChangeCommitted);
         this.dataSourceTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.dataSourceTypeComboBox_SelectedIndexChanged);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(6, 59);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(31, 13);
         this.label4.TabIndex = 4;
         this.label4.Text = "Type";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(7, 20);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(35, 13);
         this.label5.TabIndex = 0;
         this.label5.Text = "Name";
         // 
         // FinishButton
         // 
         this.FinishButton.Location = new System.Drawing.Point(91, 280);
         this.FinishButton.Name = "FinishButton";
         this.FinishButton.Size = new System.Drawing.Size(62, 23);
         this.FinishButton.TabIndex = 10;
         this.FinishButton.Text = "Finish";
         this.FinishButton.UseVisualStyleBackColor = true;
         this.FinishButton.Click += new System.EventHandler(this.finishButton_Click);
         // 
         // cancelButton
         // 
         this.cancelButton.Location = new System.Drawing.Point(173, 280);
         this.cancelButton.Name = "cancelButton";
         this.cancelButton.Size = new System.Drawing.Size(62, 23);
         this.cancelButton.TabIndex = 11;
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
         this.defLabel.Location = new System.Drawing.Point(13, 239);
         this.defLabel.Name = "defLabel";
         this.defLabel.Size = new System.Drawing.Size(35, 13);
         this.defLabel.TabIndex = 12;
         this.defLabel.Text = "label2";
         // 
         // NewDatasourceForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(262, 315);
         this.Controls.Add(this.defLabel);
         this.Controls.Add(this.cancelButton);
         this.Controls.Add(this.FinishButton);
         this.Controls.Add(this.createButton);
         this.Controls.Add(this.groupBox2);
         this.Name = "NewDatasourceForm";
         this.Text = "Add Datasource";
         this.Load += new System.EventHandler(this.NewDatasourceForm_Load);
         this.groupBox2.ResumeLayout(false);
         this.groupBox2.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button createButton;
      private System.Windows.Forms.GroupBox groupBox2;
      private System.Windows.Forms.TextBox heartbeatTextBox;
      private System.Windows.Forms.TextBox dataSourceNameTextBox;
      private System.Windows.Forms.TextBox maxValueTextBox;
      private System.Windows.Forms.TextBox minValueTextBox;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.ComboBox dataSourceTypeComboBox;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Button FinishButton;
      private System.Windows.Forms.Button cancelButton;
      private System.Windows.Forms.ErrorProvider errorProvider1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox databaseStepTextBox;
      private System.Windows.Forms.Label defLabel;
   }
}