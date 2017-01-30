using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackProjectActivity));
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label_total_elapsed_hours = new System.Windows.Forms.Label();
            this.checkBox_invoiced = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_billable = new System.Windows.Forms.CheckBox();
            this.dateTimePicker_date_invoiced = new System.Windows.Forms.DateTimePicker();
            this.richTextBox_activity_type_note = new System.Windows.Forms.RichTextBox();
            this.label_adjustment = new System.Windows.Forms.Label();
            this.label_total = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_currency_total = new System.Windows.Forms.Label();
            this.label_currency_hourly_rate = new System.Windows.Forms.Label();
            this.numericUpDown_quantity = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown_hourly_rate_client = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_status = new System.Windows.Forms.ComboBox();
            this.comboBox_activity_type = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker_end_date = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePicker_end_hours = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_start_hours = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_start_date = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox_client = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox_projects = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_quantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_client)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.panel4.Size = new System.Drawing.Size(592, 56);
            this.panel4.TabIndex = 38;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(72, 16);
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
            this.pictureBox_header.Location = new System.Drawing.Point(19, 7);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(42, 42);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 548);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(592, 45);
            this.panel1.TabIndex = 39;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button_save);
            this.panel2.Controls.Add(this.button_cancel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(412, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(180, 45);
            this.panel2.TabIndex = 0;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(12, 11);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 25;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(93, 11);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 24;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label_total_elapsed_hours);
            this.groupBox1.Controls.Add(this.checkBox_invoiced);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.checkBox_billable);
            this.groupBox1.Controls.Add(this.dateTimePicker_date_invoiced);
            this.groupBox1.Controls.Add(this.richTextBox_activity_type_note);
            this.groupBox1.Controls.Add(this.label_adjustment);
            this.groupBox1.Controls.Add(this.label_total);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label_currency_total);
            this.groupBox1.Controls.Add(this.label_currency_hourly_rate);
            this.groupBox1.Controls.Add(this.numericUpDown_quantity);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numericUpDown_hourly_rate_client);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.comboBox_status);
            this.groupBox1.Controls.Add(this.comboBox_activity_type);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.dateTimePicker_end_date);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.dateTimePicker_end_hours);
            this.groupBox1.Controls.Add(this.dateTimePicker_start_hours);
            this.groupBox1.Controls.Add(this.dateTimePicker_start_date);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_description);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 156);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(568, 391);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project Activity Details";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("MS Reference Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(392, 118);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 61);
            this.label14.TabIndex = 25;
            this.label14.Text = "}";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_total_elapsed_hours
            // 
            this.label_total_elapsed_hours.AutoSize = true;
            this.label_total_elapsed_hours.Location = new System.Drawing.Point(425, 146);
            this.label_total_elapsed_hours.Name = "label_total_elapsed_hours";
            this.label_total_elapsed_hours.Size = new System.Drawing.Size(83, 13);
            this.label_total_elapsed_hours.TabIndex = 25;
            this.label_total_elapsed_hours.Text = "0.00  total hours";
            // 
            // checkBox_invoiced
            // 
            this.checkBox_invoiced.AutoSize = true;
            this.checkBox_invoiced.Location = new System.Drawing.Point(96, 258);
            this.checkBox_invoiced.Name = "checkBox_invoiced";
            this.checkBox_invoiced.Size = new System.Drawing.Size(70, 17);
            this.checkBox_invoiced.TabIndex = 13;
            this.checkBox_invoiced.Text = "Invoiced:";
            this.checkBox_invoiced.UseVisualStyleBackColor = true;
            this.checkBox_invoiced.CheckedChanged += new System.EventHandler(this.checkBox_invoiced_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(92, 366);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 4);
            this.pictureBox1.TabIndex = 24;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox_billable
            // 
            this.checkBox_billable.AutoSize = true;
            this.checkBox_billable.Location = new System.Drawing.Point(96, 235);
            this.checkBox_billable.Name = "checkBox_billable";
            this.checkBox_billable.Size = new System.Drawing.Size(59, 17);
            this.checkBox_billable.TabIndex = 13;
            this.checkBox_billable.Text = "Billable";
            this.checkBox_billable.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker_date_invoiced
            // 
            this.dateTimePicker_date_invoiced.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dateTimePicker_date_invoiced.Location = new System.Drawing.Point(166, 256);
            this.dateTimePicker_date_invoiced.Name = "dateTimePicker_date_invoiced";
            this.dateTimePicker_date_invoiced.Size = new System.Drawing.Size(202, 20);
            this.dateTimePicker_date_invoiced.TabIndex = 8;
            this.dateTimePicker_date_invoiced.ValueChanged += new System.EventHandler(this.dateTimePicker_end_date_ValueChanged);
            // 
            // richTextBox_activity_type_note
            // 
            this.richTextBox_activity_type_note.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox_activity_type_note.Location = new System.Drawing.Point(96, 213);
            this.richTextBox_activity_type_note.Name = "richTextBox_activity_type_note";
            this.richTextBox_activity_type_note.ReadOnly = true;
            this.richTextBox_activity_type_note.Size = new System.Drawing.Size(356, 17);
            this.richTextBox_activity_type_note.TabIndex = 12;
            this.richTextBox_activity_type_note.TabStop = false;
            this.richTextBox_activity_type_note.Text = "...";
            // 
            // label_adjustment
            // 
            this.label_adjustment.AutoSize = true;
            this.label_adjustment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_adjustment.Location = new System.Drawing.Point(233, 300);
            this.label_adjustment.Name = "label_adjustment";
            this.label_adjustment.Size = new System.Drawing.Size(16, 15);
            this.label_adjustment.TabIndex = 17;
            this.label_adjustment.Text = "...";
            // 
            // label_total
            // 
            this.label_total.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_total.Location = new System.Drawing.Point(98, 350);
            this.label_total.Name = "label_total";
            this.label_total.Size = new System.Drawing.Size(81, 13);
            this.label_total.TabIndex = 22;
            this.label_total.Text = "0,00";
            this.label_total.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(197, 327);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "hours";
            // 
            // label_currency_total
            // 
            this.label_currency_total.AutoSize = true;
            this.label_currency_total.Location = new System.Drawing.Point(195, 350);
            this.label_currency_total.Name = "label_currency_total";
            this.label_currency_total.Size = new System.Drawing.Size(30, 13);
            this.label_currency_total.TabIndex = 23;
            this.label_currency_total.Text = "EUR";
            // 
            // label_currency_hourly_rate
            // 
            this.label_currency_hourly_rate.AutoSize = true;
            this.label_currency_hourly_rate.Location = new System.Drawing.Point(197, 302);
            this.label_currency_hourly_rate.Name = "label_currency_hourly_rate";
            this.label_currency_hourly_rate.Size = new System.Drawing.Size(30, 13);
            this.label_currency_hourly_rate.TabIndex = 16;
            this.label_currency_hourly_rate.Text = "EUR";
            // 
            // numericUpDown_quantity
            // 
            this.numericUpDown_quantity.DecimalPlaces = 3;
            this.numericUpDown_quantity.Location = new System.Drawing.Point(96, 324);
            this.numericUpDown_quantity.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_quantity.Name = "numericUpDown_quantity";
            this.numericUpDown_quantity.Size = new System.Drawing.Size(96, 20);
            this.numericUpDown_quantity.TabIndex = 19;
            this.numericUpDown_quantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_quantity.ValueChanged += new System.EventHandler(this.numericUpDown_quantity_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(58, 350);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Total:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Quantity:";
            // 
            // numericUpDown_hourly_rate_client
            // 
            this.numericUpDown_hourly_rate_client.DecimalPlaces = 2;
            this.numericUpDown_hourly_rate_client.Location = new System.Drawing.Point(96, 298);
            this.numericUpDown_hourly_rate_client.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numericUpDown_hourly_rate_client.Name = "numericUpDown_hourly_rate_client";
            this.numericUpDown_hourly_rate_client.Size = new System.Drawing.Size(96, 20);
            this.numericUpDown_hourly_rate_client.TabIndex = 15;
            this.numericUpDown_hourly_rate_client.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_hourly_rate_client.ValueChanged += new System.EventHandler(this.numericUpDown_hourly_rate_client_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(26, 302);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Hourly Rate:";
            // 
            // comboBox_status
            // 
            this.comboBox_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_status.FormattingEnabled = true;
            this.comboBox_status.Items.AddRange(new object[] {
            "New",
            "Confirmed"});
            this.comboBox_status.Location = new System.Drawing.Point(96, 99);
            this.comboBox_status.Name = "comboBox_status";
            this.comboBox_status.Size = new System.Drawing.Size(119, 21);
            this.comboBox_status.TabIndex = 11;
            this.comboBox_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_activity_type_SelectedIndexChanged);
            // 
            // comboBox_activity_type
            // 
            this.comboBox_activity_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_activity_type.FormattingEnabled = true;
            this.comboBox_activity_type.Location = new System.Drawing.Point(96, 190);
            this.comboBox_activity_type.Name = "comboBox_activity_type";
            this.comboBox_activity_type.Size = new System.Drawing.Size(451, 21);
            this.comboBox_activity_type.TabIndex = 11;
            this.comboBox_activity_type.SelectedIndexChanged += new System.EventHandler(this.comboBox_activity_type_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Activity Type:";
            // 
            // dateTimePicker_end_date
            // 
            this.dateTimePicker_end_date.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dateTimePicker_end_date.Location = new System.Drawing.Point(96, 154);
            this.dateTimePicker_end_date.Name = "dateTimePicker_end_date";
            this.dateTimePicker_end_date.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker_end_date.TabIndex = 8;
            this.dateTimePicker_end_date.ValueChanged += new System.EventHandler(this.dateTimePicker_end_date_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "End Time:";
            // 
            // dateTimePicker_end_hours
            // 
            this.dateTimePicker_end_hours.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_end_hours.Location = new System.Drawing.Point(291, 154);
            this.dateTimePicker_end_hours.Name = "dateTimePicker_end_hours";
            this.dateTimePicker_end_hours.ShowUpDown = true;
            this.dateTimePicker_end_hours.Size = new System.Drawing.Size(95, 20);
            this.dateTimePicker_end_hours.TabIndex = 9;
            this.dateTimePicker_end_hours.ValueChanged += new System.EventHandler(this.dateTimePicker_end_hours_ValueChanged);
            // 
            // dateTimePicker_start_hours
            // 
            this.dateTimePicker_start_hours.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dateTimePicker_start_hours.Location = new System.Drawing.Point(291, 128);
            this.dateTimePicker_start_hours.Name = "dateTimePicker_start_hours";
            this.dateTimePicker_start_hours.ShowUpDown = true;
            this.dateTimePicker_start_hours.Size = new System.Drawing.Size(95, 20);
            this.dateTimePicker_start_hours.TabIndex = 6;
            this.dateTimePicker_start_hours.ValueChanged += new System.EventHandler(this.dateTimePicker_start_hours_ValueChanged);
            // 
            // dateTimePicker_start_date
            // 
            this.dateTimePicker_start_date.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dateTimePicker_start_date.Location = new System.Drawing.Point(96, 128);
            this.dateTimePicker_start_date.Name = "dateTimePicker_start_date";
            this.dateTimePicker_start_date.Size = new System.Drawing.Size(194, 20);
            this.dateTimePicker_start_date.TabIndex = 5;
            this.dateTimePicker_start_date.ValueChanged += new System.EventHandler(this.dateTimePicker_start_date_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Start Time:";
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(96, 55);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(451, 38);
            this.textBox_description.TabIndex = 3;
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
            this.textBox_name.Size = new System.Drawing.Size(451, 20);
            this.textBox_name.TabIndex = 1;
            this.textBox_name.TextChanged += new System.EventHandler(this.textBox_name_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(52, 103);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 0;
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox_client);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.comboBox_projects);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(12, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(568, 88);
            this.groupBox2.TabIndex = 41;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Project Details";
            // 
            // textBox_client
            // 
            this.textBox_client.Location = new System.Drawing.Point(96, 23);
            this.textBox_client.Name = "textBox_client";
            this.textBox_client.ReadOnly = true;
            this.textBox_client.Size = new System.Drawing.Size(451, 20);
            this.textBox_client.TabIndex = 12;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(49, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Project:";
            // 
            // comboBox_projects
            // 
            this.comboBox_projects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_projects.FormattingEnabled = true;
            this.comboBox_projects.Location = new System.Drawing.Point(96, 50);
            this.comboBox_projects.Name = "comboBox_projects";
            this.comboBox_projects.Size = new System.Drawing.Size(451, 21);
            this.comboBox_projects.TabIndex = 11;
            this.comboBox_projects.SelectedIndexChanged += new System.EventHandler(this.comboBox_activity_type_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(56, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "Client:";
            // 
            // TrackProjectActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 593);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_quantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_hourly_rate_client)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private GroupBox groupBox1;
        private TextBox textBox_name;
        private Label label1;
        private TextBox textBox_description;
        private Label label2;
        private DateTimePicker dateTimePicker_start_date;
        private Label label5;
        private Label label7;
        private DateTimePicker dateTimePicker_end_date;
        private Label label6;
        private ComboBox comboBox_activity_type;
        private NumericUpDown numericUpDown_hourly_rate_client;
        private Label label8;
        private Label label_currency_hourly_rate;
        private Label label_total;
        private Label label_currency_total;
        private NumericUpDown numericUpDown_quantity;
        private Label label10;
        private Label label4;
        private Label label_adjustment;
        private DateTimePicker dateTimePicker_end_hours;
        private DateTimePicker dateTimePicker_start_hours;
        private RichTextBox richTextBox_activity_type_note;
        private CheckBox checkBox_billable;
        private PictureBox pictureBox1;
        private Label label3;
        private CheckBox checkBox_invoiced;
        private DateTimePicker dateTimePicker_date_invoiced;
        private GroupBox groupBox2;
        private TextBox textBox_client;
        private Label label11;
        private ComboBox comboBox_projects;
        private Label label9;
        private ComboBox comboBox_status;
        private Label label12;
        private Label label_total_elapsed_hours;
        private Label label14;
    }
}