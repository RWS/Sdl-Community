using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.QualityMetrics
{
    partial class QualityMetricGroup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(QualityMetricGroup));
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDown_company_profile_maximum_value_in_words = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_company_profile_maximum_value = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_severity_item_count = new System.Windows.Forms.Label();
            this.listView_severities = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_delete_severity = new System.Windows.Forms.Button();
            this.button_edit_severity = new System.Windows.Forms.Button();
            this.button_add_severity = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value_in_words)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 515);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(616, 40);
            this.panel3.TabIndex = 2;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.button_Save);
            this.panel7.Controls.Add(this.button_Close);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel7.Location = new System.Drawing.Point(416, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(200, 40);
            this.panel7.TabIndex = 0;
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Save.Location = new System.Drawing.Point(33, 9);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 16;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Close
            // 
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Close.Location = new System.Drawing.Point(110, 9);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 15;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(616, 63);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox_header);
            this.panel2.Controls.Add(this.pictureBox_header);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(610, 56);
            this.panel2.TabIndex = 38;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(73, 16);
            this.textBox_header.Name = "textBox_header";
            this.textBox_header.ReadOnly = true;
            this.textBox_header.Size = new System.Drawing.Size(333, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "Quality Metric Group";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(18, 5);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(46, 46);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.textBox_description);
            this.panel4.Controls.Add(this.textBox_name);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Controls.Add(this.groupBox3);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 63);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5);
            this.panel4.Size = new System.Drawing.Size(616, 452);
            this.panel4.TabIndex = 4;
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(17, 72);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(584, 91);
            this.textBox_description.TabIndex = 3;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(17, 30);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(584, 20);
            this.textBox_name.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.numericUpDown_company_profile_maximum_value_in_words);
            this.groupBox2.Controls.Add(this.numericUpDown_company_profile_maximum_value);
            this.groupBox2.Controls.Add(this.label41);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.label39);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox2.Location = new System.Drawing.Point(17, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(584, 104);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Quality Assessment Settings";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Location = new System.Drawing.Point(18, 30);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(555, 30);
            this.label13.TabIndex = 19;
            this.label13.Text = "Define the maximum number of penalty points allowed in the number of words before" +
    " the quality assessment fails.";
            // 
            // numericUpDown_company_profile_maximum_value_in_words
            // 
            this.numericUpDown_company_profile_maximum_value_in_words.Location = new System.Drawing.Point(237, 63);
            this.numericUpDown_company_profile_maximum_value_in_words.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value_in_words.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value_in_words.Name = "numericUpDown_company_profile_maximum_value_in_words";
            this.numericUpDown_company_profile_maximum_value_in_words.Size = new System.Drawing.Size(65, 20);
            this.numericUpDown_company_profile_maximum_value_in_words.TabIndex = 8;
            this.numericUpDown_company_profile_maximum_value_in_words.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // numericUpDown_company_profile_maximum_value
            // 
            this.numericUpDown_company_profile_maximum_value.Location = new System.Drawing.Point(138, 63);
            this.numericUpDown_company_profile_maximum_value.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value.Name = "numericUpDown_company_profile_maximum_value";
            this.numericUpDown_company_profile_maximum_value.Size = new System.Drawing.Size(46, 20);
            this.numericUpDown_company_profile_maximum_value.TabIndex = 7;
            this.numericUpDown_company_profile_maximum_value.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(304, 67);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(35, 13);
            this.label41.TabIndex = 14;
            this.label41.Text = "words";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(188, 67);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(47, 13);
            this.label40.TabIndex = 7;
            this.label40.Text = "in every:";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(19, 67);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(112, 13);
            this.label39.TabIndex = 5;
            this.label39.Text = "Assessment threshold:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel5);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox3.Location = new System.Drawing.Point(19, 279);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(582, 167);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Severities";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label_severity_item_count);
            this.panel5.Controls.Add(this.listView_severities);
            this.panel5.Controls.Add(this.button_delete_severity);
            this.panel5.Controls.Add(this.button_edit_severity);
            this.panel5.Controls.Add(this.button_add_severity);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 16);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(10);
            this.panel5.Size = new System.Drawing.Size(576, 148);
            this.panel5.TabIndex = 0;
            // 
            // label_severity_item_count
            // 
            this.label_severity_item_count.AutoSize = true;
            this.label_severity_item_count.Location = new System.Drawing.Point(516, 121);
            this.label_severity_item_count.Name = "label_severity_item_count";
            this.label_severity_item_count.Size = new System.Drawing.Size(47, 13);
            this.label_severity_item_count.TabIndex = 4;
            this.label_severity_item_count.Text = "Items: 0 ";
            this.label_severity_item_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listView_severities
            // 
            this.listView_severities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView_severities.FullRowSelect = true;
            this.listView_severities.Location = new System.Drawing.Point(61, 13);
            this.listView_severities.Name = "listView_severities";
            this.listView_severities.Size = new System.Drawing.Size(502, 105);
            this.listView_severities.TabIndex = 14;
            this.listView_severities.UseCompatibleStateImageBehavior = false;
            this.listView_severities.View = System.Windows.Forms.View.Details;
            this.listView_severities.SelectedIndexChanged += new System.EventHandler(this.listView_severities_SelectedIndexChanged);
            this.listView_severities.DoubleClick += new System.EventHandler(this.listView_severities_DoubleClick);
            this.listView_severities.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_severities_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Weight";
            this.columnHeader2.Width = 80;
            // 
            // button_delete_severity
            // 
            this.button_delete_severity.Location = new System.Drawing.Point(6, 61);
            this.button_delete_severity.Name = "button_delete_severity";
            this.button_delete_severity.Size = new System.Drawing.Size(49, 23);
            this.button_delete_severity.TabIndex = 12;
            this.button_delete_severity.Text = "Delete";
            this.button_delete_severity.UseVisualStyleBackColor = true;
            this.button_delete_severity.Click += new System.EventHandler(this.button_delete_severity_Click);
            // 
            // button_edit_severity
            // 
            this.button_edit_severity.Location = new System.Drawing.Point(6, 37);
            this.button_edit_severity.Name = "button_edit_severity";
            this.button_edit_severity.Size = new System.Drawing.Size(49, 23);
            this.button_edit_severity.TabIndex = 11;
            this.button_edit_severity.Text = "Edit";
            this.button_edit_severity.UseVisualStyleBackColor = true;
            this.button_edit_severity.Click += new System.EventHandler(this.button_edit_severity_Click);
            // 
            // button_add_severity
            // 
            this.button_add_severity.Location = new System.Drawing.Point(6, 13);
            this.button_add_severity.Name = "button_add_severity";
            this.button_add_severity.Size = new System.Drawing.Size(49, 23);
            this.button_add_severity.TabIndex = 10;
            this.button_add_severity.Text = "Add";
            this.button_add_severity.UseVisualStyleBackColor = true;
            this.button_add_severity.Click += new System.EventHandler(this.button_add_severity_Click);
            // 
            // QualityMetricGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 555);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QualityMetricGroup";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quality Metric Group";
            this.Load += new System.EventHandler(this.QualityMetricGroup_Load);
            this.panel3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value_in_words)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel3;
        private Panel panel7;
        private Button button_Save;
        private Button button_Close;
        private Panel panel1;
        private Panel panel2;
        private TextBox textBox_header;
        private PictureBox pictureBox_header;
        private Panel panel4;
        private TextBox textBox_description;
        private TextBox textBox_name;
        private Label label2;
        private Label label1;
        private GroupBox groupBox3;
        private Panel panel5;
        private Label label_severity_item_count;
        private ListView listView_severities;
        private Button button_delete_severity;
        private Button button_edit_severity;
        private Button button_add_severity;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private GroupBox groupBox2;
        private Label label13;
        private NumericUpDown numericUpDown_company_profile_maximum_value_in_words;
        private NumericUpDown numericUpDown_company_profile_maximum_value;
        private Label label41;
        private Label label40;
        private Label label39;
    }
}