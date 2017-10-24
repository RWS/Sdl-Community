using System.ComponentModel;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Sdl.Community.Qualitivity.Custom;

namespace Sdl.Community.Qualitivity.Dialogs
{
    partial class TrackProjectActivity
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackProjectActivity));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox_client = new System.Windows.Forms.ComboBox();
            this.comboBox_projects = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel5 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame = new System.Windows.Forms.LinkLabel();
            this.imageList_links = new System.Windows.Forms.ImageList(this.components);
            this.label20 = new System.Windows.Forms.Label();
            this.label_total_elapsed = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comboBox_billable = new System.Windows.Forms.ComboBox();
            this.comboBox_status = new System.Windows.Forms.ComboBox();
            this.dateTimePicker_end_date = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePicker_end_hours = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_start_hours = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_start_date = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel6 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.objectListView_documents = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_source = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_target = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_type = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_changes = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_elapsed = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_opened = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_closed = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.imageList_flags = new System.Windows.Forms.ImageList(this.components);
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel12 = new System.Windows.Forms.Panel();
            this.label_document_count = new System.Windows.Forms.Label();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.panel16 = new System.Windows.Forms.Panel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.panel_activity_rates_parent = new System.Windows.Forms.Panel();
            this.panel_language_rate = new System.Windows.Forms.Panel();
            this.textBox_language_rate_description = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label_rate_currency = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.linkLabel_pem_rate_view_details = new System.Windows.Forms.LinkLabel();
            this.numericUpDown_pem_rate_total = new UpDownControl();
            this.checkBox_language_rate = new System.Windows.Forms.CheckBox();
            this.comboBox_pem_rates = new System.Windows.Forms.ComboBox();
            this.label_pem_rate_toal = new System.Windows.Forms.Label();
            this.label_pem_rate = new System.Windows.Forms.Label();
            this.panel_custom_rate = new System.Windows.Forms.Panel();
            this.textBox_custom_rate_description = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.checkBox_custom_rate = new System.Windows.Forms.CheckBox();
            this.numericUpDown_custom_rate_total = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label_custom_rate_currency = new System.Windows.Forms.Label();
            this.panel_hourly_rate = new System.Windows.Forms.Panel();
            this.textBox_hourly_rate_description = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.numericUpDown_hourly_rate_total = new UpDownControl();
            this.checkBox_hourly_rate = new System.Windows.Forms.CheckBox();
            this.linkLabel_get_default_hourly_rate = new System.Windows.Forms.LinkLabel();
            this.linkLabel_get_total_hours_elapsed_from_documents = new System.Windows.Forms.LinkLabel();
            this.label_hourly_rate_rate = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.label_hourly_rate_curency = new System.Windows.Forms.Label();
            this.label_hourly_rate_total = new System.Windows.Forms.Label();
            this.numericUpDown_hourly_rate_rate = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_hourly_rate_hours = new System.Windows.Forms.NumericUpDown();
            this.label_hourly_rate_hours = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label_activity_rates_total = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label_rate_total_currency = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel10 = new System.Windows.Forms.Panel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.panel13 = new System.Windows.Forms.Panel();
            this.comboBox_comparisonType = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.checkBox_group_changes = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.checkBox_includeTagsInComparison = new System.Windows.Forms.CheckBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.panel14 = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.panel15 = new System.Windows.Forms.Panel();
            this.textBox_quality_metric_name = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.numericUpDown_company_profile_maximum_value_in_words = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown_company_profile_maximum_value = new System.Windows.Forms.NumericUpDown();
            this.label41 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.imageList_tab = new System.Windows.Forms.ImageList(this.components);
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView_documents)).BeginInit();
            this.panel8.SuspendLayout();
            this.panel12.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.panel16.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel_activity_rates_parent.SuspendLayout();
            this.panel_language_rate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_pem_rate_total)).BeginInit();
            this.panel_custom_rate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_custom_rate_total)).BeginInit();
            this.panel_hourly_rate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_total)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_rate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_hours)).BeginInit();
            this.panel9.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel10.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel13.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.panel14.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel15.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value_in_words)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value)).BeginInit();
            this.SuspendLayout();
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.textBox_header);
            this.panel4.Controls.Add(this.pictureBox_header);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(948, 50);
            this.panel4.TabIndex = 38;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(72, 13);
            this.textBox_header.Name = "textBox_header";
            this.textBox_header.ReadOnly = true;
            this.textBox_header.Size = new System.Drawing.Size(288, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "Project Activity";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(19, 5);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(38, 38);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 606);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(948, 33);
            this.panel1.TabIndex = 39;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_save);
            this.panel2.Controls.Add(this.button_cancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(768, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(180, 33);
            this.panel2.TabIndex = 0;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(15, 3);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 1;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(96, 3);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 0;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.comboBox_client);
            this.groupBox2.Controls.Add(this.comboBox_projects);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(914, 88);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Project Details";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(49, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Project:";
            // 
            // comboBox_client
            // 
            this.comboBox_client.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_client.FormattingEnabled = true;
            this.comboBox_client.Location = new System.Drawing.Point(96, 23);
            this.comboBox_client.Name = "comboBox_client";
            this.comboBox_client.Size = new System.Drawing.Size(779, 21);
            this.comboBox_client.TabIndex = 4;
            // 
            // comboBox_projects
            // 
            this.comboBox_projects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_projects.FormattingEnabled = true;
            this.comboBox_projects.Location = new System.Drawing.Point(96, 50);
            this.comboBox_projects.Name = "comboBox_projects";
            this.comboBox_projects.Size = new System.Drawing.Size(779, 21);
            this.comboBox_projects.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(56, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "Client:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tabControl1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 50);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(948, 556);
            this.panel3.TabIndex = 42;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList_tab;
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(938, 546);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel5);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(930, 503);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Details ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.groupBox1);
            this.panel5.Controls.Add(this.groupBox2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(5);
            this.panel5.Size = new System.Drawing.Size(924, 497);
            this.panel5.TabIndex = 42;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label_total_elapsed);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.comboBox_billable);
            this.groupBox1.Controls.Add(this.comboBox_status);
            this.groupBox1.Controls.Add(this.dateTimePicker_end_date);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dateTimePicker_end_hours);
            this.groupBox1.Controls.Add(this.dateTimePicker_start_hours);
            this.groupBox1.Controls.Add(this.dateTimePicker_start_date);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_description);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(5, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(914, 399);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project Activity Details";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(26, 255);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(22, 22);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 31;
            this.pictureBox3.TabStop = false;
            // 
            // linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame
            // 
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.ImageIndex = 0;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.ImageList = this.imageList_links;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.Location = new System.Drawing.Point(92, 356);
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.Name = "linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame";
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.Size = new System.Drawing.Size(387, 17);
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.TabIndex = 15;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.TabStop = true;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.Text = "Adjust time frame to the earliest && latest dates associated with the documents";
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame_LinkClicked);
            // 
            // imageList_links
            // 
            this.imageList_links.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_links.ImageStream")));
            this.imageList_links.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_links.Images.SetKeyName(0, "Time-Clock-32.png");
            this.imageList_links.Images.SetKeyName(1, "Hourglass-32.png");
            this.imageList_links.Images.SetKeyName(2, "cost.png");
            this.imageList_links.Images.SetKeyName(3, "cost(2).png");
            this.imageList_links.Images.SetKeyName(4, "cost(1).png");
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("MS Reference Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(453, 284);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(28, 58);
            this.label20.TabIndex = 16;
            this.label20.Text = "}";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_total_elapsed
            // 
            this.label_total_elapsed.AutoSize = true;
            this.label_total_elapsed.Location = new System.Drawing.Point(486, 309);
            this.label_total_elapsed.Name = "label_total_elapsed";
            this.label_total_elapsed.Size = new System.Drawing.Size(170, 13);
            this.label_total_elapsed.TabIndex = 17;
            this.label_total_elapsed.Text = "0 days, 0 hours, 0 mins, 0 seconds";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(95, 267);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(776, 4);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // comboBox_billable
            // 
            this.comboBox_billable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_billable.FormattingEnabled = true;
            this.comboBox_billable.Items.AddRange(new object[] {
            "Yes",
            "No"});
            this.comboBox_billable.Location = new System.Drawing.Point(96, 179);
            this.comboBox_billable.Name = "comboBox_billable";
            this.comboBox_billable.Size = new System.Drawing.Size(119, 21);
            this.comboBox_billable.TabIndex = 7;
            this.comboBox_billable.SelectedIndexChanged += new System.EventHandler(this.comboBox_billable_SelectedIndexChanged);
            // 
            // comboBox_status
            // 
            this.comboBox_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_status.FormattingEnabled = true;
            this.comboBox_status.Items.AddRange(new object[] {
            "New",
            "Confirmed"});
            this.comboBox_status.Location = new System.Drawing.Point(96, 152);
            this.comboBox_status.Name = "comboBox_status";
            this.comboBox_status.Size = new System.Drawing.Size(119, 21);
            this.comboBox_status.TabIndex = 5;
            this.comboBox_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_status_SelectedIndexChanged);
            // 
            // dateTimePicker_end_date
            // 
            this.dateTimePicker_end_date.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dateTimePicker_end_date.Location = new System.Drawing.Point(150, 320);
            this.dateTimePicker_end_date.Name = "dateTimePicker_end_date";
            this.dateTimePicker_end_date.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker_end_date.TabIndex = 13;
            this.dateTimePicker_end_date.ValueChanged += new System.EventHandler(this.dateTimePicker_end_date_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(92, 324);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "End Time:";
            // 
            // dateTimePicker_end_hours
            // 
            this.dateTimePicker_end_hours.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_end_hours.Location = new System.Drawing.Point(345, 320);
            this.dateTimePicker_end_hours.Name = "dateTimePicker_end_hours";
            this.dateTimePicker_end_hours.ShowUpDown = true;
            this.dateTimePicker_end_hours.Size = new System.Drawing.Size(87, 20);
            this.dateTimePicker_end_hours.TabIndex = 14;
            this.dateTimePicker_end_hours.ValueChanged += new System.EventHandler(this.dateTimePicker_end_hours_ValueChanged);
            // 
            // dateTimePicker_start_hours
            // 
            this.dateTimePicker_start_hours.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_start_hours.Location = new System.Drawing.Point(345, 294);
            this.dateTimePicker_start_hours.Name = "dateTimePicker_start_hours";
            this.dateTimePicker_start_hours.ShowUpDown = true;
            this.dateTimePicker_start_hours.Size = new System.Drawing.Size(87, 20);
            this.dateTimePicker_start_hours.TabIndex = 11;
            this.dateTimePicker_start_hours.ValueChanged += new System.EventHandler(this.dateTimePicker_start_hours_ValueChanged);
            // 
            // dateTimePicker_start_date
            // 
            this.dateTimePicker_start_date.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dateTimePicker_start_date.Location = new System.Drawing.Point(150, 294);
            this.dateTimePicker_start_date.Name = "dateTimePicker_start_date";
            this.dateTimePicker_start_date.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker_start_date.TabIndex = 10;
            this.dateTimePicker_start_date.ValueChanged += new System.EventHandler(this.dateTimePicker_start_date_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 298);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Start Time:";
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(96, 55);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(779, 91);
            this.textBox_description.TabIndex = 3;
            this.textBox_description.TextChanged += new System.EventHandler(this.textBox_description_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description:";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(96, 29);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(779, 20);
            this.textBox_name.TabIndex = 1;
            this.textBox_name.TextChanged += new System.EventHandler(this.textBox_name_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(49, 183);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Billable:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 260);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Dates:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(52, 156);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Status:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel6);
            this.tabPage2.ImageIndex = 1;
            this.tabPage2.Location = new System.Drawing.Point(4, 39);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(930, 503);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Documents ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.groupBox3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(924, 497);
            this.panel6.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel7);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(914, 487);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Document Activities";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.objectListView_documents);
            this.panel7.Controls.Add(this.panel8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(3, 16);
            this.panel7.Name = "panel7";
            this.panel7.Padding = new System.Windows.Forms.Padding(5);
            this.panel7.Size = new System.Drawing.Size(908, 468);
            this.panel7.TabIndex = 0;
            // 
            // objectListView_documents
            // 
            this.objectListView_documents.AllColumns.Add(this.olvColumn_name);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_source);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_target);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_type);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_changes);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_elapsed);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_opened);
            this.objectListView_documents.AllColumns.Add(this.olvColumn_closed);
            this.objectListView_documents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn_name,
            this.olvColumn_source,
            this.olvColumn_target,
            this.olvColumn_type,
            this.olvColumn_changes,
            this.olvColumn_elapsed,
            this.olvColumn_opened,
            this.olvColumn_closed});
            this.objectListView_documents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView_documents.FullRowSelect = true;
            this.objectListView_documents.HeaderUsesThemes = false;
            this.objectListView_documents.HeaderWordWrap = true;
            this.objectListView_documents.HideSelection = false;
            this.objectListView_documents.IncludeColumnHeadersInCopy = true;
            this.objectListView_documents.Location = new System.Drawing.Point(5, 5);
            this.objectListView_documents.Name = "objectListView_documents";
            this.objectListView_documents.OwnerDraw = true;
            this.objectListView_documents.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.objectListView_documents.ShowCommandMenuOnRightClick = true;
            this.objectListView_documents.ShowHeaderInAllViews = false;
            this.objectListView_documents.ShowItemCountOnGroups = true;
            this.objectListView_documents.ShowItemToolTips = true;
            this.objectListView_documents.Size = new System.Drawing.Size(898, 437);
            this.objectListView_documents.SmallImageList = this.imageList_flags;
            this.objectListView_documents.SpaceBetweenGroups = 20;
            this.objectListView_documents.TabIndex = 1;
            this.objectListView_documents.UseAlternatingBackColors = true;
            this.objectListView_documents.UseCellFormatEvents = true;
            this.objectListView_documents.UseCompatibleStateImageBehavior = false;
            this.objectListView_documents.UseFilterIndicator = true;
            this.objectListView_documents.UseFiltering = true;
            this.objectListView_documents.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn_name
            // 
            this.olvColumn_name.AspectName = "documentName";
            this.olvColumn_name.CellPadding = null;
            this.olvColumn_name.Text = "Name";
            this.olvColumn_name.Width = 220;
            // 
            // olvColumn_source
            // 
            this.olvColumn_source.AspectName = "sourceLang";
            this.olvColumn_source.CellPadding = null;
            this.olvColumn_source.Text = "Source";
            // 
            // olvColumn_target
            // 
            this.olvColumn_target.AspectName = "targetLang";
            this.olvColumn_target.CellPadding = null;
            this.olvColumn_target.Text = "Target";
            // 
            // olvColumn_type
            // 
            this.olvColumn_type.AspectName = "DocumentActivityType";
            this.olvColumn_type.CellPadding = null;
            this.olvColumn_type.Text = "Type";
            this.olvColumn_type.Width = 80;
            // 
            // olvColumn_changes
            // 
            this.olvColumn_changes.CellPadding = null;
            this.olvColumn_changes.Text = "Segments Modified";
            this.olvColumn_changes.Width = 110;
            // 
            // olvColumn_elapsed
            // 
            this.olvColumn_elapsed.AspectName = "ticksActivity";
            this.olvColumn_elapsed.CellPadding = null;
            this.olvColumn_elapsed.Text = "Elapsed";
            this.olvColumn_elapsed.Width = 90;
            // 
            // olvColumn_opened
            // 
            this.olvColumn_opened.AspectName = "Started";
            this.olvColumn_opened.CellPadding = null;
            this.olvColumn_opened.Text = "Opened";
            this.olvColumn_opened.Width = 120;
            // 
            // olvColumn_closed
            // 
            this.olvColumn_closed.AspectName = "Stopped";
            this.olvColumn_closed.CellPadding = null;
            this.olvColumn_closed.Text = "Closed";
            this.olvColumn_closed.Width = 120;
            // 
            // imageList_flags
            // 
            this.imageList_flags.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_flags.ImageStream")));
            this.imageList_flags.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_flags.Images.SetKeyName(0, "af-ZA.gif");
            this.imageList_flags.Images.SetKeyName(1, "ar.gif");
            this.imageList_flags.Images.SetKeyName(2, "ar-AE.gif");
            this.imageList_flags.Images.SetKeyName(3, "ar-BH.gif");
            this.imageList_flags.Images.SetKeyName(4, "ar-DZ.gif");
            this.imageList_flags.Images.SetKeyName(5, "ar-EG.gif");
            this.imageList_flags.Images.SetKeyName(6, "ar-IQ.gif");
            this.imageList_flags.Images.SetKeyName(7, "ar-JO.gif");
            this.imageList_flags.Images.SetKeyName(8, "ar-KW.gif");
            this.imageList_flags.Images.SetKeyName(9, "ar-LB.gif");
            this.imageList_flags.Images.SetKeyName(10, "ar-LY.gif");
            this.imageList_flags.Images.SetKeyName(11, "ar-MA.gif");
            this.imageList_flags.Images.SetKeyName(12, "ar-OM.gif");
            this.imageList_flags.Images.SetKeyName(13, "ar-QA.gif");
            this.imageList_flags.Images.SetKeyName(14, "ar-SA.gif");
            this.imageList_flags.Images.SetKeyName(15, "ar-SY.gif");
            this.imageList_flags.Images.SetKeyName(16, "ar-TN.gif");
            this.imageList_flags.Images.SetKeyName(17, "ar-YE.gif");
            this.imageList_flags.Images.SetKeyName(18, "be.gif");
            this.imageList_flags.Images.SetKeyName(19, "be-BY.gif");
            this.imageList_flags.Images.SetKeyName(20, "bg.gif");
            this.imageList_flags.Images.SetKeyName(21, "bg-BG.gif");
            this.imageList_flags.Images.SetKeyName(22, "bp.gif");
            this.imageList_flags.Images.SetKeyName(23, "bs.gif");
            this.imageList_flags.Images.SetKeyName(24, "bs-Cyrl-BA.gif");
            this.imageList_flags.Images.SetKeyName(25, "bs-Latn-BA.gif");
            this.imageList_flags.Images.SetKeyName(26, "ca.gif");
            this.imageList_flags.Images.SetKeyName(27, "ca-ES.gif");
            this.imageList_flags.Images.SetKeyName(28, "cf.gif");
            this.imageList_flags.Images.SetKeyName(29, "ch.gif");
            this.imageList_flags.Images.SetKeyName(30, "cs.gif");
            this.imageList_flags.Images.SetKeyName(31, "cs-CZ.gif");
            this.imageList_flags.Images.SetKeyName(32, "da.gif");
            this.imageList_flags.Images.SetKeyName(33, "da-DK.gif");
            this.imageList_flags.Images.SetKeyName(34, "de.gif");
            this.imageList_flags.Images.SetKeyName(35, "de-AT.gif");
            this.imageList_flags.Images.SetKeyName(36, "de-CH.gif");
            this.imageList_flags.Images.SetKeyName(37, "de-DE.gif");
            this.imageList_flags.Images.SetKeyName(38, "de-LI.gif");
            this.imageList_flags.Images.SetKeyName(39, "de-LU.gif");
            this.imageList_flags.Images.SetKeyName(40, "el.gif");
            this.imageList_flags.Images.SetKeyName(41, "el-GR.gif");
            this.imageList_flags.Images.SetKeyName(42, "empty.png");
            this.imageList_flags.Images.SetKeyName(43, "en.gif");
            this.imageList_flags.Images.SetKeyName(44, "en-AU.gif");
            this.imageList_flags.Images.SetKeyName(45, "en-BZ.gif");
            this.imageList_flags.Images.SetKeyName(46, "en-CA.gif");
            this.imageList_flags.Images.SetKeyName(47, "en-GB.gif");
            this.imageList_flags.Images.SetKeyName(48, "en-IE.gif");
            this.imageList_flags.Images.SetKeyName(49, "en-JM.gif");
            this.imageList_flags.Images.SetKeyName(50, "en-NZ.gif");
            this.imageList_flags.Images.SetKeyName(51, "en-PH.gif");
            this.imageList_flags.Images.SetKeyName(52, "en-TT.gif");
            this.imageList_flags.Images.SetKeyName(53, "en-US.gif");
            this.imageList_flags.Images.SetKeyName(54, "en-ZW.gif");
            this.imageList_flags.Images.SetKeyName(55, "es.gif");
            this.imageList_flags.Images.SetKeyName(56, "es-AR.gif");
            this.imageList_flags.Images.SetKeyName(57, "es-BO.gif");
            this.imageList_flags.Images.SetKeyName(58, "es-CL.gif");
            this.imageList_flags.Images.SetKeyName(59, "es-CO.gif");
            this.imageList_flags.Images.SetKeyName(60, "es-CR.gif");
            this.imageList_flags.Images.SetKeyName(61, "es-DO.gif");
            this.imageList_flags.Images.SetKeyName(62, "es-EC.gif");
            this.imageList_flags.Images.SetKeyName(63, "es-ES.gif");
            this.imageList_flags.Images.SetKeyName(64, "es-GT.gif");
            this.imageList_flags.Images.SetKeyName(65, "es-HN.gif");
            this.imageList_flags.Images.SetKeyName(66, "es-MX.gif");
            this.imageList_flags.Images.SetKeyName(67, "es-NI.gif");
            this.imageList_flags.Images.SetKeyName(68, "es-PA.gif");
            this.imageList_flags.Images.SetKeyName(69, "es-PE.gif");
            this.imageList_flags.Images.SetKeyName(70, "es-PR.gif");
            this.imageList_flags.Images.SetKeyName(71, "es-PY.gif");
            this.imageList_flags.Images.SetKeyName(72, "es-SV.gif");
            this.imageList_flags.Images.SetKeyName(73, "es-UY.gif");
            this.imageList_flags.Images.SetKeyName(74, "es-VE.gif");
            this.imageList_flags.Images.SetKeyName(75, "et.gif");
            this.imageList_flags.Images.SetKeyName(76, "et-EE.gif");
            this.imageList_flags.Images.SetKeyName(77, "eu.gif");
            this.imageList_flags.Images.SetKeyName(78, "eu-ES.gif");
            this.imageList_flags.Images.SetKeyName(79, "fa.gif");
            this.imageList_flags.Images.SetKeyName(80, "fa-IR.gif");
            this.imageList_flags.Images.SetKeyName(81, "fi.gif");
            this.imageList_flags.Images.SetKeyName(82, "fi-FI.gif");
            this.imageList_flags.Images.SetKeyName(83, "fl.gif");
            this.imageList_flags.Images.SetKeyName(84, "fr.gif");
            this.imageList_flags.Images.SetKeyName(85, "fr-BE.gif");
            this.imageList_flags.Images.SetKeyName(86, "fr-CA.gif");
            this.imageList_flags.Images.SetKeyName(87, "fr-CH.gif");
            this.imageList_flags.Images.SetKeyName(88, "fr-FR.gif");
            this.imageList_flags.Images.SetKeyName(89, "fr-LU.gif");
            this.imageList_flags.Images.SetKeyName(90, "ga.gif");
            this.imageList_flags.Images.SetKeyName(91, "gb.gif");
            this.imageList_flags.Images.SetKeyName(92, "he.gif");
            this.imageList_flags.Images.SetKeyName(93, "he-IL.gif");
            this.imageList_flags.Images.SetKeyName(94, "hi.gif");
            this.imageList_flags.Images.SetKeyName(95, "hi-IN.gif");
            this.imageList_flags.Images.SetKeyName(96, "hr.gif");
            this.imageList_flags.Images.SetKeyName(97, "hr-HR.gif");
            this.imageList_flags.Images.SetKeyName(98, "hu.gif");
            this.imageList_flags.Images.SetKeyName(99, "hu-HU.gif");
            this.imageList_flags.Images.SetKeyName(100, "id-ID.gif");
            this.imageList_flags.Images.SetKeyName(101, "in.gif");
            this.imageList_flags.Images.SetKeyName(102, "is.gif");
            this.imageList_flags.Images.SetKeyName(103, "is-IS.gif");
            this.imageList_flags.Images.SetKeyName(104, "it.gif");
            this.imageList_flags.Images.SetKeyName(105, "it-CH.gif");
            this.imageList_flags.Images.SetKeyName(106, "it-IT.gif");
            this.imageList_flags.Images.SetKeyName(107, "ja.gif");
            this.imageList_flags.Images.SetKeyName(108, "ja-JP.gif");
            this.imageList_flags.Images.SetKeyName(109, "kk.gif");
            this.imageList_flags.Images.SetKeyName(110, "kk-KZ.gif");
            this.imageList_flags.Images.SetKeyName(111, "ko.gif");
            this.imageList_flags.Images.SetKeyName(112, "ko-KR.gif");
            this.imageList_flags.Images.SetKeyName(113, "sourceLang.gif");
            this.imageList_flags.Images.SetKeyName(114, "targetLang.gif");
            this.imageList_flags.Images.SetKeyName(115, "targetLang-LT.gif");
            this.imageList_flags.Images.SetKeyName(116, "lv.gif");
            this.imageList_flags.Images.SetKeyName(117, "lv-LV.gif");
            this.imageList_flags.Images.SetKeyName(118, "lx.gif");
            this.imageList_flags.Images.SetKeyName(119, "mk.gif");
            this.imageList_flags.Images.SetKeyName(120, "mk-MK.gif");
            this.imageList_flags.Images.SetKeyName(121, "ms.gif");
            this.imageList_flags.Images.SetKeyName(122, "ms-MY.gif");
            this.imageList_flags.Images.SetKeyName(123, "nb-NO.gif");
            this.imageList_flags.Images.SetKeyName(124, "nl.gif");
            this.imageList_flags.Images.SetKeyName(125, "nl-BE.gif");
            this.imageList_flags.Images.SetKeyName(126, "nl-NL.gif");
            this.imageList_flags.Images.SetKeyName(127, "nn-NO.gif");
            this.imageList_flags.Images.SetKeyName(128, "no.gif");
            this.imageList_flags.Images.SetKeyName(129, "pl.gif");
            this.imageList_flags.Images.SetKeyName(130, "pl-PL.gif");
            this.imageList_flags.Images.SetKeyName(131, "pt.gif");
            this.imageList_flags.Images.SetKeyName(132, "pt-BR.gif");
            this.imageList_flags.Images.SetKeyName(133, "pt-PT.gif");
            this.imageList_flags.Images.SetKeyName(134, "ro.gif");
            this.imageList_flags.Images.SetKeyName(135, "ro-RO.gif");
            this.imageList_flags.Images.SetKeyName(136, "ru.gif");
            this.imageList_flags.Images.SetKeyName(137, "ru-RU.gif");
            this.imageList_flags.Images.SetKeyName(138, "sd.gif");
            this.imageList_flags.Images.SetKeyName(139, "sf.gif");
            this.imageList_flags.Images.SetKeyName(140, "si.gif");
            this.imageList_flags.Images.SetKeyName(141, "sk.gif");
            this.imageList_flags.Images.SetKeyName(142, "sk-SK.gif");
            this.imageList_flags.Images.SetKeyName(143, "sl.gif");
            this.imageList_flags.Images.SetKeyName(144, "sl-SI.gif");
            this.imageList_flags.Images.SetKeyName(145, "sp.gif");
            this.imageList_flags.Images.SetKeyName(146, "sq.gif");
            this.imageList_flags.Images.SetKeyName(147, "sq-AL.gif");
            this.imageList_flags.Images.SetKeyName(148, "sr.gif");
            this.imageList_flags.Images.SetKeyName(149, "sr-Cyrl-CS.gif");
            this.imageList_flags.Images.SetKeyName(150, "sr-Latn-CS.gif");
            this.imageList_flags.Images.SetKeyName(151, "sv.gif");
            this.imageList_flags.Images.SetKeyName(152, "sv-FI.gif");
            this.imageList_flags.Images.SetKeyName(153, "sv-SE.gif");
            this.imageList_flags.Images.SetKeyName(154, "syr-SY.gif");
            this.imageList_flags.Images.SetKeyName(155, "th.gif");
            this.imageList_flags.Images.SetKeyName(156, "th-TH.gif");
            this.imageList_flags.Images.SetKeyName(157, "tr.gif");
            this.imageList_flags.Images.SetKeyName(158, "tr-TR.gif");
            this.imageList_flags.Images.SetKeyName(159, "uk.gif");
            this.imageList_flags.Images.SetKeyName(160, "uk-UA.gif");
            this.imageList_flags.Images.SetKeyName(161, "ur-PK.gif");
            this.imageList_flags.Images.SetKeyName(162, "uz-Cyrl-UZ.gif");
            this.imageList_flags.Images.SetKeyName(163, "uz-Latn-UZ.gif");
            this.imageList_flags.Images.SetKeyName(164, "vi.gif");
            this.imageList_flags.Images.SetKeyName(165, "vi-VN.gif");
            this.imageList_flags.Images.SetKeyName(166, "xh-ZA.gif");
            this.imageList_flags.Images.SetKeyName(167, "zh.gif");
            this.imageList_flags.Images.SetKeyName(168, "zh-CN.gif");
            this.imageList_flags.Images.SetKeyName(169, "zh-HK.gif");
            this.imageList_flags.Images.SetKeyName(170, "zh-MO.gif");
            this.imageList_flags.Images.SetKeyName(171, "zh-SG.gif");
            this.imageList_flags.Images.SetKeyName(172, "zh-TW.gif");
            this.imageList_flags.Images.SetKeyName(173, "zt.gif");
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel12);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel8.Location = new System.Drawing.Point(5, 442);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(898, 21);
            this.panel8.TabIndex = 0;
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.label_document_count);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel12.Location = new System.Drawing.Point(698, 0);
            this.panel12.Name = "panel12";
            this.panel12.Padding = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.panel12.Size = new System.Drawing.Size(200, 21);
            this.panel12.TabIndex = 0;
            // 
            // label_document_count
            // 
            this.label_document_count.AutoSize = true;
            this.label_document_count.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_document_count.Location = new System.Drawing.Point(84, 3);
            this.label_document_count.Name = "label_document_count";
            this.label_document_count.Size = new System.Drawing.Size(113, 13);
            this.label_document_count.TabIndex = 0;
            this.label_document_count.Text = "Document Activities: 0";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.panel16);
            this.tabPage6.ImageIndex = 4;
            this.tabPage6.Location = new System.Drawing.Point(4, 39);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(930, 503);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Activity Rates ";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.groupBox7);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel16.Location = new System.Drawing.Point(0, 0);
            this.panel16.Name = "panel16";
            this.panel16.Padding = new System.Windows.Forms.Padding(5);
            this.panel16.Size = new System.Drawing.Size(930, 503);
            this.panel16.TabIndex = 3;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.panel_activity_rates_parent);
            this.groupBox7.Controls.Add(this.panel9);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(5, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(920, 493);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Activity Rates";
            // 
            // panel_activity_rates_parent
            // 
            this.panel_activity_rates_parent.Controls.Add(this.panel_language_rate);
            this.panel_activity_rates_parent.Controls.Add(this.panel_custom_rate);
            this.panel_activity_rates_parent.Controls.Add(this.panel_hourly_rate);
            this.panel_activity_rates_parent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_activity_rates_parent.Location = new System.Drawing.Point(3, 16);
            this.panel_activity_rates_parent.Name = "panel_activity_rates_parent";
            this.panel_activity_rates_parent.Padding = new System.Windows.Forms.Padding(5, 10, 5, 5);
            this.panel_activity_rates_parent.Size = new System.Drawing.Size(914, 419);
            this.panel_activity_rates_parent.TabIndex = 0;
            // 
            // panel_language_rate
            // 
            this.panel_language_rate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_language_rate.Controls.Add(this.textBox_language_rate_description);
            this.panel_language_rate.Controls.Add(this.label19);
            this.panel_language_rate.Controls.Add(this.label_rate_currency);
            this.panel_language_rate.Controls.Add(this.pictureBox6);
            this.panel_language_rate.Controls.Add(this.linkLabel_pem_rate_view_details);
            this.panel_language_rate.Controls.Add(this.numericUpDown_pem_rate_total);
            this.panel_language_rate.Controls.Add(this.checkBox_language_rate);
            this.panel_language_rate.Controls.Add(this.comboBox_pem_rates);
            this.panel_language_rate.Controls.Add(this.label_pem_rate_toal);
            this.panel_language_rate.Controls.Add(this.label_pem_rate);
            this.panel_language_rate.Location = new System.Drawing.Point(40, 56);
            this.panel_language_rate.Name = "panel_language_rate";
            this.panel_language_rate.Padding = new System.Windows.Forms.Padding(5);
            this.panel_language_rate.Size = new System.Drawing.Size(164, 178);
            this.panel_language_rate.TabIndex = 0;
            // 
            // textBox_language_rate_description
            // 
            this.textBox_language_rate_description.Location = new System.Drawing.Point(136, 44);
            this.textBox_language_rate_description.Name = "textBox_language_rate_description";
            this.textBox_language_rate_description.Size = new System.Drawing.Size(667, 20);
            this.textBox_language_rate_description.TabIndex = 3;
            this.textBox_language_rate_description.TextChanged += new System.EventHandler(this.textBox_language_rate_name_TextChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(73, 48);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(63, 13);
            this.label19.TabIndex = 2;
            this.label19.Text = "Description:";
            // 
            // label_rate_currency
            // 
            this.label_rate_currency.AutoSize = true;
            this.label_rate_currency.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_rate_currency.Location = new System.Drawing.Point(240, 101);
            this.label_rate_currency.Name = "label_rate_currency";
            this.label_rate_currency.Size = new System.Drawing.Size(16, 13);
            this.label_rate_currency.TabIndex = 9;
            this.label_rate_currency.Text = "...";
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(46, 12);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(22, 22);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox6.TabIndex = 110;
            this.pictureBox6.TabStop = false;
            // 
            // linkLabel_pem_rate_view_details
            // 
            this.linkLabel_pem_rate_view_details.AutoSize = true;
            this.linkLabel_pem_rate_view_details.Location = new System.Drawing.Point(515, 75);
            this.linkLabel_pem_rate_view_details.Name = "linkLabel_pem_rate_view_details";
            this.linkLabel_pem_rate_view_details.Size = new System.Drawing.Size(65, 13);
            this.linkLabel_pem_rate_view_details.TabIndex = 6;
            this.linkLabel_pem_rate_view_details.TabStop = true;
            this.linkLabel_pem_rate_view_details.Text = "View Details";
            this.linkLabel_pem_rate_view_details.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_pem_rate_view_details_LinkClicked);
            // 
            // numericUpDown_pem_rate_total
            // 
            this.numericUpDown_pem_rate_total.DecimalPlaces = 2;
            this.numericUpDown_pem_rate_total.Location = new System.Drawing.Point(136, 97);
            this.numericUpDown_pem_rate_total.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_pem_rate_total.Name = "numericUpDown_pem_rate_total";
            this.numericUpDown_pem_rate_total.ReadOnly = true;
            this.numericUpDown_pem_rate_total.Size = new System.Drawing.Size(98, 20);
            this.numericUpDown_pem_rate_total.TabIndex = 8;
            this.numericUpDown_pem_rate_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_language_rate
            // 
            this.checkBox_language_rate.AutoSize = true;
            this.checkBox_language_rate.Location = new System.Drawing.Point(76, 15);
            this.checkBox_language_rate.Name = "checkBox_language_rate";
            this.checkBox_language_rate.Size = new System.Drawing.Size(100, 17);
            this.checkBox_language_rate.TabIndex = 1;
            this.checkBox_language_rate.Text = "Language Rate";
            this.checkBox_language_rate.UseVisualStyleBackColor = true;
            this.checkBox_language_rate.CheckedChanged += new System.EventHandler(this.checkBox_pem_rate_CheckedChanged);
            // 
            // comboBox_pem_rates
            // 
            this.comboBox_pem_rates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_pem_rates.FormattingEnabled = true;
            this.comboBox_pem_rates.Location = new System.Drawing.Point(136, 70);
            this.comboBox_pem_rates.Name = "comboBox_pem_rates";
            this.comboBox_pem_rates.Size = new System.Drawing.Size(375, 21);
            this.comboBox_pem_rates.TabIndex = 5;
            this.comboBox_pem_rates.SelectedIndexChanged += new System.EventHandler(this.comboBox_pem_rates_SelectedIndexChanged);
            // 
            // label_pem_rate_toal
            // 
            this.label_pem_rate_toal.AutoSize = true;
            this.label_pem_rate_toal.Location = new System.Drawing.Point(102, 101);
            this.label_pem_rate_toal.Name = "label_pem_rate_toal";
            this.label_pem_rate_toal.Size = new System.Drawing.Size(34, 13);
            this.label_pem_rate_toal.TabIndex = 7;
            this.label_pem_rate_toal.Text = "Total:";
            // 
            // label_pem_rate
            // 
            this.label_pem_rate.AutoSize = true;
            this.label_pem_rate.Location = new System.Drawing.Point(103, 74);
            this.label_pem_rate.Name = "label_pem_rate";
            this.label_pem_rate.Size = new System.Drawing.Size(33, 13);
            this.label_pem_rate.TabIndex = 4;
            this.label_pem_rate.Text = "Rate:";
            // 
            // panel_custom_rate
            // 
            this.panel_custom_rate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_custom_rate.Controls.Add(this.textBox_custom_rate_description);
            this.panel_custom_rate.Controls.Add(this.pictureBox2);
            this.panel_custom_rate.Controls.Add(this.checkBox_custom_rate);
            this.panel_custom_rate.Controls.Add(this.numericUpDown_custom_rate_total);
            this.panel_custom_rate.Controls.Add(this.label15);
            this.panel_custom_rate.Controls.Add(this.label16);
            this.panel_custom_rate.Controls.Add(this.label_custom_rate_currency);
            this.panel_custom_rate.Location = new System.Drawing.Point(568, 70);
            this.panel_custom_rate.Name = "panel_custom_rate";
            this.panel_custom_rate.Padding = new System.Windows.Forms.Padding(5);
            this.panel_custom_rate.Size = new System.Drawing.Size(276, 164);
            this.panel_custom_rate.TabIndex = 0;
            // 
            // textBox_custom_rate_description
            // 
            this.textBox_custom_rate_description.Location = new System.Drawing.Point(123, 44);
            this.textBox_custom_rate_description.Name = "textBox_custom_rate_description";
            this.textBox_custom_rate_description.Size = new System.Drawing.Size(667, 20);
            this.textBox_custom_rate_description.TabIndex = 24;
            this.textBox_custom_rate_description.TextChanged += new System.EventHandler(this.textBox_custom_rate_name_TextChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(27, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(22, 22);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 115;
            this.pictureBox2.TabStop = false;
            // 
            // checkBox_custom_rate
            // 
            this.checkBox_custom_rate.AutoSize = true;
            this.checkBox_custom_rate.Location = new System.Drawing.Point(59, 16);
            this.checkBox_custom_rate.Name = "checkBox_custom_rate";
            this.checkBox_custom_rate.Size = new System.Drawing.Size(87, 17);
            this.checkBox_custom_rate.TabIndex = 22;
            this.checkBox_custom_rate.Text = "Custom Rate";
            this.checkBox_custom_rate.UseVisualStyleBackColor = true;
            this.checkBox_custom_rate.CheckedChanged += new System.EventHandler(this.checkBox_custome_rate_CheckedChanged);
            // 
            // numericUpDown_custom_rate_total
            // 
            this.numericUpDown_custom_rate_total.DecimalPlaces = 2;
            this.numericUpDown_custom_rate_total.Location = new System.Drawing.Point(123, 70);
            this.numericUpDown_custom_rate_total.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_custom_rate_total.Name = "numericUpDown_custom_rate_total";
            this.numericUpDown_custom_rate_total.Size = new System.Drawing.Size(96, 20);
            this.numericUpDown_custom_rate_total.TabIndex = 26;
            this.numericUpDown_custom_rate_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_custom_rate_total.ValueChanged += new System.EventHandler(this.numericUpDown_custom_rate_total_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(61, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(63, 13);
            this.label15.TabIndex = 23;
            this.label15.Text = "Description:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(90, 74);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(34, 13);
            this.label16.TabIndex = 25;
            this.label16.Text = "Total:";
            // 
            // label_custom_rate_currency
            // 
            this.label_custom_rate_currency.AutoSize = true;
            this.label_custom_rate_currency.Location = new System.Drawing.Point(225, 74);
            this.label_custom_rate_currency.Name = "label_custom_rate_currency";
            this.label_custom_rate_currency.Size = new System.Drawing.Size(16, 13);
            this.label_custom_rate_currency.TabIndex = 110;
            this.label_custom_rate_currency.Text = "...";
            // 
            // panel_hourly_rate
            // 
            this.panel_hourly_rate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_hourly_rate.Controls.Add(this.textBox_hourly_rate_description);
            this.panel_hourly_rate.Controls.Add(this.label18);
            this.panel_hourly_rate.Controls.Add(this.pictureBox7);
            this.panel_hourly_rate.Controls.Add(this.numericUpDown_hourly_rate_total);
            this.panel_hourly_rate.Controls.Add(this.checkBox_hourly_rate);
            this.panel_hourly_rate.Controls.Add(this.linkLabel_get_default_hourly_rate);
            this.panel_hourly_rate.Controls.Add(this.linkLabel_get_total_hours_elapsed_from_documents);
            this.panel_hourly_rate.Controls.Add(this.label_hourly_rate_rate);
            this.panel_hourly_rate.Controls.Add(this.pictureBox5);
            this.panel_hourly_rate.Controls.Add(this.label_hourly_rate_curency);
            this.panel_hourly_rate.Controls.Add(this.label_hourly_rate_total);
            this.panel_hourly_rate.Controls.Add(this.numericUpDown_hourly_rate_rate);
            this.panel_hourly_rate.Controls.Add(this.numericUpDown_hourly_rate_hours);
            this.panel_hourly_rate.Controls.Add(this.label_hourly_rate_hours);
            this.panel_hourly_rate.Location = new System.Drawing.Point(223, 56);
            this.panel_hourly_rate.Name = "panel_hourly_rate";
            this.panel_hourly_rate.Padding = new System.Windows.Forms.Padding(5);
            this.panel_hourly_rate.Size = new System.Drawing.Size(301, 178);
            this.panel_hourly_rate.TabIndex = 0;
            // 
            // textBox_hourly_rate_description
            // 
            this.textBox_hourly_rate_description.Location = new System.Drawing.Point(136, 44);
            this.textBox_hourly_rate_description.Name = "textBox_hourly_rate_description";
            this.textBox_hourly_rate_description.Size = new System.Drawing.Size(667, 20);
            this.textBox_hourly_rate_description.TabIndex = 12;
            this.textBox_hourly_rate_description.TextChanged += new System.EventHandler(this.textBox_hourly_rate_name_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(73, 48);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(63, 13);
            this.label18.TabIndex = 11;
            this.label18.Text = "Description:";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.Location = new System.Drawing.Point(40, 14);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(22, 18);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox7.TabIndex = 114;
            this.pictureBox7.TabStop = false;
            // 
            // numericUpDown_hourly_rate_total
            // 
            this.numericUpDown_hourly_rate_total.DecimalPlaces = 2;
            this.numericUpDown_hourly_rate_total.Location = new System.Drawing.Point(136, 131);
            this.numericUpDown_hourly_rate_total.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_hourly_rate_total.Name = "numericUpDown_hourly_rate_total";
            this.numericUpDown_hourly_rate_total.ReadOnly = true;
            this.numericUpDown_hourly_rate_total.Size = new System.Drawing.Size(98, 20);
            this.numericUpDown_hourly_rate_total.TabIndex = 20;
            this.numericUpDown_hourly_rate_total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // checkBox_hourly_rate
            // 
            this.checkBox_hourly_rate.AutoSize = true;
            this.checkBox_hourly_rate.Location = new System.Drawing.Point(72, 15);
            this.checkBox_hourly_rate.Name = "checkBox_hourly_rate";
            this.checkBox_hourly_rate.Size = new System.Drawing.Size(82, 17);
            this.checkBox_hourly_rate.TabIndex = 10;
            this.checkBox_hourly_rate.Text = "Hourly Rate";
            this.checkBox_hourly_rate.UseVisualStyleBackColor = true;
            this.checkBox_hourly_rate.CheckedChanged += new System.EventHandler(this.checkBox_hourly_rate_CheckedChanged);
            // 
            // linkLabel_get_default_hourly_rate
            // 
            this.linkLabel_get_default_hourly_rate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_get_default_hourly_rate.ImageIndex = 4;
            this.linkLabel_get_default_hourly_rate.ImageList = this.imageList_links;
            this.linkLabel_get_default_hourly_rate.Location = new System.Drawing.Point(239, 98);
            this.linkLabel_get_default_hourly_rate.Name = "linkLabel_get_default_hourly_rate";
            this.linkLabel_get_default_hourly_rate.Size = new System.Drawing.Size(130, 18);
            this.linkLabel_get_default_hourly_rate.TabIndex = 18;
            this.linkLabel_get_default_hourly_rate.TabStop = true;
            this.linkLabel_get_default_hourly_rate.Text = "Get default hourly rate";
            this.linkLabel_get_default_hourly_rate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_get_default_hourly_rate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_get_default_hourly_rate_LinkClicked);
            // 
            // linkLabel_get_total_hours_elapsed_from_documents
            // 
            this.linkLabel_get_total_hours_elapsed_from_documents.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_get_total_hours_elapsed_from_documents.ImageIndex = 1;
            this.linkLabel_get_total_hours_elapsed_from_documents.ImageList = this.imageList_links;
            this.linkLabel_get_total_hours_elapsed_from_documents.Location = new System.Drawing.Point(239, 69);
            this.linkLabel_get_total_hours_elapsed_from_documents.Name = "linkLabel_get_total_hours_elapsed_from_documents";
            this.linkLabel_get_total_hours_elapsed_from_documents.Size = new System.Drawing.Size(213, 24);
            this.linkLabel_get_total_hours_elapsed_from_documents.TabIndex = 15;
            this.linkLabel_get_total_hours_elapsed_from_documents.TabStop = true;
            this.linkLabel_get_total_hours_elapsed_from_documents.Text = "Get total hours elasped from documents";
            this.linkLabel_get_total_hours_elapsed_from_documents.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_get_total_hours_elapsed_from_documents.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_get_total_hours_elapsed_from_documents_LinkClicked);
            // 
            // label_hourly_rate_rate
            // 
            this.label_hourly_rate_rate.AutoSize = true;
            this.label_hourly_rate_rate.Location = new System.Drawing.Point(103, 101);
            this.label_hourly_rate_rate.Name = "label_hourly_rate_rate";
            this.label_hourly_rate_rate.Size = new System.Drawing.Size(33, 13);
            this.label_hourly_rate_rate.TabIndex = 16;
            this.label_hourly_rate_rate.Text = "Rate:";
            // 
            // pictureBox5
            // 
            this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox5.Location = new System.Drawing.Point(98, 123);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(194, 4);
            this.pictureBox5.TabIndex = 113;
            this.pictureBox5.TabStop = false;
            // 
            // label_hourly_rate_curency
            // 
            this.label_hourly_rate_curency.AutoSize = true;
            this.label_hourly_rate_curency.Location = new System.Drawing.Point(238, 134);
            this.label_hourly_rate_curency.Name = "label_hourly_rate_curency";
            this.label_hourly_rate_curency.Size = new System.Drawing.Size(30, 13);
            this.label_hourly_rate_curency.TabIndex = 21;
            this.label_hourly_rate_curency.Text = "EUR";
            this.label_hourly_rate_curency.TextChanged += new System.EventHandler(this.label_hourly_rate_curency_TextChanged);
            // 
            // label_hourly_rate_total
            // 
            this.label_hourly_rate_total.AutoSize = true;
            this.label_hourly_rate_total.Location = new System.Drawing.Point(102, 135);
            this.label_hourly_rate_total.Name = "label_hourly_rate_total";
            this.label_hourly_rate_total.Size = new System.Drawing.Size(34, 13);
            this.label_hourly_rate_total.TabIndex = 19;
            this.label_hourly_rate_total.Text = "Total:";
            // 
            // numericUpDown_hourly_rate_rate
            // 
            this.numericUpDown_hourly_rate_rate.DecimalPlaces = 2;
            this.numericUpDown_hourly_rate_rate.Location = new System.Drawing.Point(136, 98);
            this.numericUpDown_hourly_rate_rate.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_hourly_rate_rate.Name = "numericUpDown_hourly_rate_rate";
            this.numericUpDown_hourly_rate_rate.Size = new System.Drawing.Size(96, 20);
            this.numericUpDown_hourly_rate_rate.TabIndex = 17;
            this.numericUpDown_hourly_rate_rate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_hourly_rate_rate.ValueChanged += new System.EventHandler(this.numericUpDown_hourly_rate_rate_ValueChanged);
            // 
            // numericUpDown_hourly_rate_hours
            // 
            this.numericUpDown_hourly_rate_hours.DecimalPlaces = 3;
            this.numericUpDown_hourly_rate_hours.Location = new System.Drawing.Point(136, 70);
            this.numericUpDown_hourly_rate_hours.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_hourly_rate_hours.Name = "numericUpDown_hourly_rate_hours";
            this.numericUpDown_hourly_rate_hours.Size = new System.Drawing.Size(96, 20);
            this.numericUpDown_hourly_rate_hours.TabIndex = 14;
            this.numericUpDown_hourly_rate_hours.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_hourly_rate_hours.ValueChanged += new System.EventHandler(this.numericUpDown_hourly_rate_hours_ValueChanged);
            // 
            // label_hourly_rate_hours
            // 
            this.label_hourly_rate_hours.AutoSize = true;
            this.label_hourly_rate_hours.Location = new System.Drawing.Point(98, 74);
            this.label_hourly_rate_hours.Name = "label_hourly_rate_hours";
            this.label_hourly_rate_hours.Size = new System.Drawing.Size(38, 13);
            this.label_hourly_rate_hours.TabIndex = 13;
            this.label_hourly_rate_hours.Text = "Hours:";
            // 
            // panel9
            // 
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Controls.Add(this.label_activity_rates_total);
            this.panel9.Controls.Add(this.label21);
            this.panel9.Controls.Add(this.label_rate_total_currency);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel9.Location = new System.Drawing.Point(3, 435);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(914, 55);
            this.panel9.TabIndex = 1;
            // 
            // label_activity_rates_total
            // 
            this.label_activity_rates_total.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_activity_rates_total.Location = new System.Drawing.Point(115, 20);
            this.label_activity_rates_total.Name = "label_activity_rates_total";
            this.label_activity_rates_total.Size = new System.Drawing.Size(72, 13);
            this.label_activity_rates_total.TabIndex = 0;
            this.label_activity_rates_total.Text = "0000.00";
            this.label_activity_rates_total.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(16, 20);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(102, 13);
            this.label21.TabIndex = 0;
            this.label21.Text = "Activity Rates Total:";
            // 
            // label_rate_total_currency
            // 
            this.label_rate_total_currency.AutoSize = true;
            this.label_rate_total_currency.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_rate_total_currency.Location = new System.Drawing.Point(187, 20);
            this.label_rate_total_currency.Name = "label_rate_total_currency";
            this.label_rate_total_currency.Size = new System.Drawing.Size(16, 13);
            this.label_rate_total_currency.TabIndex = 9;
            this.label_rate_total_currency.Text = "...";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel10);
            this.tabPage4.ImageIndex = 8;
            this.tabPage4.Location = new System.Drawing.Point(4, 39);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(930, 503);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Comparison Settings ";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.groupBox5);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel10.Location = new System.Drawing.Point(0, 0);
            this.panel10.Name = "panel10";
            this.panel10.Padding = new System.Windows.Forms.Padding(5);
            this.panel10.Size = new System.Drawing.Size(930, 503);
            this.panel10.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.panel13);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(5, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(920, 493);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Comparison Settings";
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.comboBox_comparisonType);
            this.panel13.Controls.Add(this.label33);
            this.panel13.Controls.Add(this.checkBox_group_changes);
            this.panel13.Controls.Add(this.label8);
            this.panel13.Controls.Add(this.label4);
            this.panel13.Controls.Add(this.label10);
            this.panel13.Controls.Add(this.label31);
            this.panel13.Controls.Add(this.checkBox_includeTagsInComparison);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel13.Location = new System.Drawing.Point(3, 16);
            this.panel13.Name = "panel13";
            this.panel13.Padding = new System.Windows.Forms.Padding(5);
            this.panel13.Size = new System.Drawing.Size(914, 474);
            this.panel13.TabIndex = 0;
            // 
            // comboBox_comparisonType
            // 
            this.comboBox_comparisonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_comparisonType.Enabled = false;
            this.comboBox_comparisonType.FormattingEnabled = true;
            this.comboBox_comparisonType.Items.AddRange(new object[] {
            "words",
            "characters"});
            this.comboBox_comparisonType.Location = new System.Drawing.Point(123, 25);
            this.comboBox_comparisonType.Name = "comboBox_comparisonType";
            this.comboBox_comparisonType.Size = new System.Drawing.Size(121, 21);
            this.comboBox_comparisonType.TabIndex = 121;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(25, 29);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(92, 13);
            this.label33.TabIndex = 120;
            this.label33.Text = "Comparison Type:";
            // 
            // checkBox_group_changes
            // 
            this.checkBox_group_changes.AutoSize = true;
            this.checkBox_group_changes.Location = new System.Drawing.Point(28, 63);
            this.checkBox_group_changes.Name = "checkBox_group_changes";
            this.checkBox_group_changes.Size = new System.Drawing.Size(193, 17);
            this.checkBox_group_changes.TabIndex = 119;
            this.checkBox_group_changes.Text = "Consolidate comparison differences";
            this.checkBox_group_changes.UseVisualStyleBackColor = true;
            this.checkBox_group_changes.CheckedChanged += new System.EventHandler(this.checkBox_group_changes_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Gray;
            this.label8.Location = new System.Drawing.Point(43, 170);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 118;
            this.label8.Text = "Note:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(80, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(539, 13);
            this.label4.TabIndex = 117;
            this.label4.Text = "The properties related to the \"Differences Formatting\" can be found in the main s" +
    "ettings are under \'Client Profiles\',";
            // 
            // label10
            // 
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(44, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(776, 30);
            this.label10.TabIndex = 116;
            this.label10.Text = "Consolidate the comparison differences in groups of similar types; this improves " +
    "readability of the comparison differences from the report";
            // 
            // label31
            // 
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label31.Location = new System.Drawing.Point(44, 139);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(776, 31);
            this.label31.TabIndex = 116;
            this.label31.Text = resources.GetString("label31.Text");
            // 
            // checkBox_includeTagsInComparison
            // 
            this.checkBox_includeTagsInComparison.AutoSize = true;
            this.checkBox_includeTagsInComparison.Location = new System.Drawing.Point(28, 118);
            this.checkBox_includeTagsInComparison.Name = "checkBox_includeTagsInComparison";
            this.checkBox_includeTagsInComparison.Size = new System.Drawing.Size(241, 17);
            this.checkBox_includeTagsInComparison.TabIndex = 103;
            this.checkBox_includeTagsInComparison.Text = "Include <tag> formatting in comparison results";
            this.checkBox_includeTagsInComparison.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.panel14);
            this.tabPage5.ImageIndex = 9;
            this.tabPage5.Location = new System.Drawing.Point(4, 39);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(930, 503);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Quality Assessment Settings";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.groupBox6);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel14.Location = new System.Drawing.Point(0, 0);
            this.panel14.Name = "panel14";
            this.panel14.Padding = new System.Windows.Forms.Padding(5);
            this.panel14.Size = new System.Drawing.Size(930, 503);
            this.panel14.TabIndex = 2;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.panel15);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(5, 5);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(920, 493);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Quality Assessment Settings";
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.textBox_quality_metric_name);
            this.panel15.Controls.Add(this.label13);
            this.panel15.Controls.Add(this.richTextBox1);
            this.panel15.Controls.Add(this.numericUpDown_company_profile_maximum_value_in_words);
            this.panel15.Controls.Add(this.numericUpDown_company_profile_maximum_value);
            this.panel15.Controls.Add(this.label41);
            this.panel15.Controls.Add(this.label40);
            this.panel15.Controls.Add(this.label14);
            this.panel15.Controls.Add(this.label39);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel15.Location = new System.Drawing.Point(3, 16);
            this.panel15.Name = "panel15";
            this.panel15.Padding = new System.Windows.Forms.Padding(5);
            this.panel15.Size = new System.Drawing.Size(914, 474);
            this.panel15.TabIndex = 0;
            // 
            // textBox_quality_metric_name
            // 
            this.textBox_quality_metric_name.Location = new System.Drawing.Point(103, 29);
            this.textBox_quality_metric_name.Name = "textBox_quality_metric_name";
            this.textBox_quality_metric_name.ReadOnly = true;
            this.textBox_quality_metric_name.Size = new System.Drawing.Size(445, 20);
            this.textBox_quality_metric_name.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Location = new System.Drawing.Point(20, 77);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(535, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Define the maximum number of penalty points allowed in the number of words before" +
    " the quality assessment fails.";
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.Location = new System.Drawing.Point(20, 143);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(797, 165);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // numericUpDown_company_profile_maximum_value_in_words
            // 
            this.numericUpDown_company_profile_maximum_value_in_words.Location = new System.Drawing.Point(238, 103);
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
            this.numericUpDown_company_profile_maximum_value_in_words.TabIndex = 6;
            this.numericUpDown_company_profile_maximum_value_in_words.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value_in_words.ValueChanged += new System.EventHandler(this.numericUpDown_company_profile_maximum_value_in_words_ValueChanged);
            // 
            // numericUpDown_company_profile_maximum_value
            // 
            this.numericUpDown_company_profile_maximum_value.Location = new System.Drawing.Point(139, 103);
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
            this.numericUpDown_company_profile_maximum_value.TabIndex = 4;
            this.numericUpDown_company_profile_maximum_value.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_company_profile_maximum_value.ValueChanged += new System.EventHandler(this.numericUpDown_company_profile_maximum_value_ValueChanged);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(305, 107);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(35, 13);
            this.label41.TabIndex = 7;
            this.label41.Text = "words";
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(189, 107);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(47, 13);
            this.label40.TabIndex = 5;
            this.label40.Text = "in every:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(23, 32);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(74, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Quality Metric:";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(20, 107);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(112, 13);
            this.label39.TabIndex = 3;
            this.label39.Text = "Assessment threshold:";
            // 
            // imageList_tab
            // 
            this.imageList_tab.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_tab.ImageStream")));
            this.imageList_tab.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_tab.Images.SetKeyName(0, "evolution_tasks.ico");
            this.imageList_tab.Images.SetKeyName(1, "briefcase_files.ico");
            this.imageList_tab.Images.SetKeyName(2, "cost(1).ico");
            this.imageList_tab.Images.SetKeyName(3, "cost(2).ico");
            this.imageList_tab.Images.SetKeyName(4, "cost.ico");
            this.imageList_tab.Images.SetKeyName(5, "default_document.ico");
            this.imageList_tab.Images.SetKeyName(6, "wallet_louis_vuitton_money.ico");
            this.imageList_tab.Images.SetKeyName(7, "1998_low_cost_clock.ico");
            this.imageList_tab.Images.SetKeyName(8, "Funny-Search-32.png");
            this.imageList_tab.Images.SetKeyName(9, "Edit-Yes-32.png");
            // 
            // TrackProjectActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 639);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TrackProjectActivity";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Activity";
            this.Load += new System.EventHandler(this.TrackProjectActivity_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView_documents)).EndInit();
            this.panel8.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.tabPage6.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.panel_activity_rates_parent.ResumeLayout(false);
            this.panel_language_rate.ResumeLayout(false);
            this.panel_language_rate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_pem_rate_total)).EndInit();
            this.panel_custom_rate.ResumeLayout(false);
            this.panel_custom_rate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_custom_rate_total)).EndInit();
            this.panel_hourly_rate.ResumeLayout(false);
            this.panel_hourly_rate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_total)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_rate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_hours)).EndInit();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.panel14.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value_in_words)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_profile_maximum_value)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel4;
        private TextBox textBox_header;
        private PictureBox pictureBox_header;
        private Panel panel1;
        private Panel panel2;
        private Button button_save;
        private Button button_cancel;
        private GroupBox groupBox2;
        private Label label11;
        private Label label9;
        private Panel panel3;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private ImageList imageList_tab;
        private Panel panel5;
        private GroupBox groupBox1;
        private ComboBox comboBox_status;
        private DateTimePicker dateTimePicker_end_date;
        private Label label7;
        private DateTimePicker dateTimePicker_end_hours;
        private DateTimePicker dateTimePicker_start_hours;
        private DateTimePicker dateTimePicker_start_date;
        private Label label5;
        private TextBox textBox_description;
        private Label label2;
        private TextBox textBox_name;
        private Label label12;
        private Label label1;
        private Panel panel6;
        private GroupBox groupBox3;
        private Panel panel7;
        private Panel panel8;
        internal ObjectListView objectListView_documents;
        private Panel panel12;
        private Label label_document_count;
        private OLVColumn olvColumn_name;
        private OLVColumn olvColumn_source;
        private OLVColumn olvColumn_target;
        private OLVColumn olvColumn_type;
        private OLVColumn olvColumn_elapsed;
        private OLVColumn olvColumn_opened;
        private OLVColumn olvColumn_closed;
        private OLVColumn olvColumn_changes;
        private ImageList imageList_flags;
        private PictureBox pictureBox1;
        private Label label3;
        private ComboBox comboBox_projects;
        private ComboBox comboBox_billable;
        private Label label6;
        private Label label20;
        private Label label_total_elapsed;
        private LinkLabel linkLabel_adjust_time_frame_to_document_earliest_lastest_time_frame;
        private ImageList imageList_links;
        private PictureBox pictureBox3;
        private TabPage tabPage4;
        private Panel panel10;
        private GroupBox groupBox5;
        private Panel panel13;
        public CheckBox checkBox_includeTagsInComparison;
        private Label label31;
        private Label label8;
        private Label label4;
        private CheckBox checkBox_group_changes;
        private Label label10;
        private ComboBox comboBox_comparisonType;
        private Label label33;
        private TabPage tabPage5;
        private Panel panel14;
        private GroupBox groupBox6;
        private Panel panel15;
        private RichTextBox richTextBox1;
        private NumericUpDown numericUpDown_company_profile_maximum_value_in_words;
        private NumericUpDown numericUpDown_company_profile_maximum_value;
        private Label label41;
        private Label label40;
        private Label label39;
        private Label label13;
        private TextBox textBox_quality_metric_name;
        private Label label14;
        private ComboBox comboBox_client;
        private TabPage tabPage6;
        private Panel panel16;
        private GroupBox groupBox7;
        private Panel panel_activity_rates_parent;
        private Panel panel_custom_rate;
        private Panel panel_language_rate;
        private Panel panel_hourly_rate;
        private PictureBox pictureBox6;
        private LinkLabel linkLabel_pem_rate_view_details;
        private UpDownControl numericUpDown_pem_rate_total;
        private CheckBox checkBox_language_rate;
        private ComboBox comboBox_pem_rates;
        private Label label_rate_currency;
        private Label label_pem_rate_toal;
        private Label label_pem_rate;
        private PictureBox pictureBox7;
        private UpDownControl numericUpDown_hourly_rate_total;
        private CheckBox checkBox_hourly_rate;
        private LinkLabel linkLabel_get_default_hourly_rate;
        private LinkLabel linkLabel_get_total_hours_elapsed_from_documents;
        private Label label_hourly_rate_rate;
        private PictureBox pictureBox5;
        private Label label_hourly_rate_curency;
        private Label label_hourly_rate_total;
        private NumericUpDown numericUpDown_hourly_rate_rate;
        private NumericUpDown numericUpDown_hourly_rate_hours;
        private Label label_hourly_rate_hours;
        private PictureBox pictureBox2;
        private CheckBox checkBox_custom_rate;
        private NumericUpDown numericUpDown_custom_rate_total;
        private Label label15;
        private Label label16;
        private Label label_custom_rate_currency;
        private TextBox textBox_custom_rate_description;
        private TextBox textBox_hourly_rate_description;
        private Label label18;
        private TextBox textBox_language_rate_description;
        private Label label19;
        private Panel panel9;
        private Label label_activity_rates_total;
        private Label label21;
        private Label label_rate_total_currency;
    }
}