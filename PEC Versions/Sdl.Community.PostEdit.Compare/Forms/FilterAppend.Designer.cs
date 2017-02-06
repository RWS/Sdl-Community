namespace PostEdit.Compare.Forms
{
    partial class FilterAppend
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterAppend));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_regularExpression = new System.Windows.Forms.CheckBox();
            this.comboBox_names_extension_list_type = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_names_extension_list = new System.Windows.Forms.TextBox();
            this.groupBox_file_attributes = new System.Windows.Forms.GroupBox();
            this.comboBox_attributes_readOnly = new System.Windows.Forms.ComboBox();
            this.checkBox_attributes_readOnly = new System.Windows.Forms.CheckBox();
            this.comboBox_attributes_hidden = new System.Windows.Forms.ComboBox();
            this.checkBox_attributes_hidden = new System.Windows.Forms.CheckBox();
            this.comboBox_attributes_system = new System.Windows.Forms.ComboBox();
            this.checkBox_attributes_system = new System.Windows.Forms.CheckBox();
            this.comboBox_attributes_archive = new System.Windows.Forms.ComboBox();
            this.checkBox_attributes_archive = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboBox_filterDate_type = new System.Windows.Forms.ComboBox();
            this.dateTimePicker_filterDate = new System.Windows.Forms.DateTimePicker();
            this.checkBox_filterDate = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox_file_attributes.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox_header_image);
            this.panel6.Location = new System.Drawing.Point(12, 7);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(371, 56);
            this.panel6.TabIndex = 60;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(63, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(251, 25);
            this.textBox_header_title.TabIndex = 3;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = "Filter Settings";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(4, 4);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(56, 45);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(228, 486);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 59;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(309, 486);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 58;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_regularExpression);
            this.groupBox1.Controls.Add(this.comboBox_names_extension_list_type);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_names_extension_list);
            this.groupBox1.Location = new System.Drawing.Point(13, 122);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 135);
            this.groupBox1.TabIndex = 61;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "     File Names (regular expression)";
            // 
            // checkBox_regularExpression
            // 
            this.checkBox_regularExpression.AutoSize = true;
            this.checkBox_regularExpression.Location = new System.Drawing.Point(236, 32);
            this.checkBox_regularExpression.Name = "checkBox_regularExpression";
            this.checkBox_regularExpression.Size = new System.Drawing.Size(117, 17);
            this.checkBox_regularExpression.TabIndex = 79;
            this.checkBox_regularExpression.Text = "Regular Expression";
            this.checkBox_regularExpression.UseVisualStyleBackColor = true;
            this.checkBox_regularExpression.Visible = false;
            // 
            // comboBox_names_extension_list_type
            // 
            this.comboBox_names_extension_list_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_names_extension_list_type.FormattingEnabled = true;
            this.comboBox_names_extension_list_type.Items.AddRange(new object[] {
            "Include",
            "Exclude"});
            this.comboBox_names_extension_list_type.Location = new System.Drawing.Point(57, 29);
            this.comboBox_names_extension_list_type.Name = "comboBox_names_extension_list_type";
            this.comboBox_names_extension_list_type.Size = new System.Drawing.Size(121, 21);
            this.comboBox_names_extension_list_type.TabIndex = 78;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 77;
            this.label2.Text = "Type:";
            // 
            // textBox_names_extension_list
            // 
            this.textBox_names_extension_list.Location = new System.Drawing.Point(17, 53);
            this.textBox_names_extension_list.Multiline = true;
            this.textBox_names_extension_list.Name = "textBox_names_extension_list";
            this.textBox_names_extension_list.Size = new System.Drawing.Size(336, 67);
            this.textBox_names_extension_list.TabIndex = 76;
            this.textBox_names_extension_list.TextChanged += new System.EventHandler(this.textBox_names_include_TextChanged);
            // 
            // groupBox_file_attributes
            // 
            this.groupBox_file_attributes.Controls.Add(this.comboBox_attributes_readOnly);
            this.groupBox_file_attributes.Controls.Add(this.checkBox_attributes_readOnly);
            this.groupBox_file_attributes.Controls.Add(this.comboBox_attributes_hidden);
            this.groupBox_file_attributes.Controls.Add(this.checkBox_attributes_hidden);
            this.groupBox_file_attributes.Controls.Add(this.comboBox_attributes_system);
            this.groupBox_file_attributes.Controls.Add(this.checkBox_attributes_system);
            this.groupBox_file_attributes.Controls.Add(this.comboBox_attributes_archive);
            this.groupBox_file_attributes.Controls.Add(this.checkBox_attributes_archive);
            this.groupBox_file_attributes.Location = new System.Drawing.Point(17, 357);
            this.groupBox_file_attributes.Name = "groupBox_file_attributes";
            this.groupBox_file_attributes.Size = new System.Drawing.Size(366, 125);
            this.groupBox_file_attributes.TabIndex = 61;
            this.groupBox_file_attributes.TabStop = false;
            this.groupBox_file_attributes.Text = "     File Attributes";
            // 
            // comboBox_attributes_readOnly
            // 
            this.comboBox_attributes_readOnly.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_attributes_readOnly.FormattingEnabled = true;
            this.comboBox_attributes_readOnly.Items.AddRange(new object[] {
            "Included",
            "Excluded"});
            this.comboBox_attributes_readOnly.Location = new System.Drawing.Point(107, 91);
            this.comboBox_attributes_readOnly.Name = "comboBox_attributes_readOnly";
            this.comboBox_attributes_readOnly.Size = new System.Drawing.Size(83, 21);
            this.comboBox_attributes_readOnly.TabIndex = 2;
            // 
            // checkBox_attributes_readOnly
            // 
            this.checkBox_attributes_readOnly.AutoSize = true;
            this.checkBox_attributes_readOnly.Location = new System.Drawing.Point(27, 94);
            this.checkBox_attributes_readOnly.Name = "checkBox_attributes_readOnly";
            this.checkBox_attributes_readOnly.Size = new System.Drawing.Size(77, 17);
            this.checkBox_attributes_readOnly.TabIndex = 0;
            this.checkBox_attributes_readOnly.Text = "Read-only:";
            this.checkBox_attributes_readOnly.UseVisualStyleBackColor = true;
            this.checkBox_attributes_readOnly.CheckedChanged += new System.EventHandler(this.checkBox_attributes_readOnly_CheckedChanged);
            // 
            // comboBox_attributes_hidden
            // 
            this.comboBox_attributes_hidden.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_attributes_hidden.FormattingEnabled = true;
            this.comboBox_attributes_hidden.Items.AddRange(new object[] {
            "Included",
            "Excluded"});
            this.comboBox_attributes_hidden.Location = new System.Drawing.Point(107, 69);
            this.comboBox_attributes_hidden.Name = "comboBox_attributes_hidden";
            this.comboBox_attributes_hidden.Size = new System.Drawing.Size(83, 21);
            this.comboBox_attributes_hidden.TabIndex = 2;
            // 
            // checkBox_attributes_hidden
            // 
            this.checkBox_attributes_hidden.AutoSize = true;
            this.checkBox_attributes_hidden.Location = new System.Drawing.Point(27, 72);
            this.checkBox_attributes_hidden.Name = "checkBox_attributes_hidden";
            this.checkBox_attributes_hidden.Size = new System.Drawing.Size(63, 17);
            this.checkBox_attributes_hidden.TabIndex = 0;
            this.checkBox_attributes_hidden.Text = "Hidden:";
            this.checkBox_attributes_hidden.UseVisualStyleBackColor = true;
            this.checkBox_attributes_hidden.CheckedChanged += new System.EventHandler(this.checkBox_attributes_hidden_CheckedChanged);
            // 
            // comboBox_attributes_system
            // 
            this.comboBox_attributes_system.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_attributes_system.FormattingEnabled = true;
            this.comboBox_attributes_system.Items.AddRange(new object[] {
            "Included",
            "Excluded"});
            this.comboBox_attributes_system.Location = new System.Drawing.Point(107, 47);
            this.comboBox_attributes_system.Name = "comboBox_attributes_system";
            this.comboBox_attributes_system.Size = new System.Drawing.Size(83, 21);
            this.comboBox_attributes_system.TabIndex = 2;
            // 
            // checkBox_attributes_system
            // 
            this.checkBox_attributes_system.AutoSize = true;
            this.checkBox_attributes_system.Location = new System.Drawing.Point(27, 50);
            this.checkBox_attributes_system.Name = "checkBox_attributes_system";
            this.checkBox_attributes_system.Size = new System.Drawing.Size(63, 17);
            this.checkBox_attributes_system.TabIndex = 0;
            this.checkBox_attributes_system.Text = "System:";
            this.checkBox_attributes_system.UseVisualStyleBackColor = true;
            this.checkBox_attributes_system.CheckedChanged += new System.EventHandler(this.checkBox_attributes_system_CheckedChanged);
            // 
            // comboBox_attributes_archive
            // 
            this.comboBox_attributes_archive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_attributes_archive.FormattingEnabled = true;
            this.comboBox_attributes_archive.Items.AddRange(new object[] {
            "Included",
            "Excluded"});
            this.comboBox_attributes_archive.Location = new System.Drawing.Point(107, 25);
            this.comboBox_attributes_archive.Name = "comboBox_attributes_archive";
            this.comboBox_attributes_archive.Size = new System.Drawing.Size(83, 21);
            this.comboBox_attributes_archive.TabIndex = 2;
            // 
            // checkBox_attributes_archive
            // 
            this.checkBox_attributes_archive.AutoSize = true;
            this.checkBox_attributes_archive.Location = new System.Drawing.Point(27, 28);
            this.checkBox_attributes_archive.Name = "checkBox_attributes_archive";
            this.checkBox_attributes_archive.Size = new System.Drawing.Size(65, 17);
            this.checkBox_attributes_archive.TabIndex = 0;
            this.checkBox_attributes_archive.Text = "Archive:";
            this.checkBox_attributes_archive.UseVisualStyleBackColor = true;
            this.checkBox_attributes_archive.CheckedChanged += new System.EventHandler(this.checkBox_attributes_archive_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.comboBox_filterDate_type);
            this.groupBox3.Controls.Add(this.dateTimePicker_filterDate);
            this.groupBox3.Controls.Add(this.checkBox_filterDate);
            this.groupBox3.Location = new System.Drawing.Point(17, 269);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(366, 75);
            this.groupBox3.TabIndex = 61;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "     File Date";
            // 
            // comboBox_filterDate_type
            // 
            this.comboBox_filterDate_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_filterDate_type.FormattingEnabled = true;
            this.comboBox_filterDate_type.Items.AddRange(new object[] {
            "is after",
            "is before"});
            this.comboBox_filterDate_type.Location = new System.Drawing.Point(102, 30);
            this.comboBox_filterDate_type.Name = "comboBox_filterDate_type";
            this.comboBox_filterDate_type.Size = new System.Drawing.Size(99, 21);
            this.comboBox_filterDate_type.TabIndex = 2;
            this.comboBox_filterDate_type.SelectedIndexChanged += new System.EventHandler(this.comboBox_filterDate_type_SelectedIndexChanged);
            // 
            // dateTimePicker_filterDate
            // 
            this.dateTimePicker_filterDate.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.dateTimePicker_filterDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_filterDate.Location = new System.Drawing.Point(207, 30);
            this.dateTimePicker_filterDate.Name = "dateTimePicker_filterDate";
            this.dateTimePicker_filterDate.Size = new System.Drawing.Size(142, 20);
            this.dateTimePicker_filterDate.TabIndex = 1;
            this.dateTimePicker_filterDate.ValueChanged += new System.EventHandler(this.dateTimePicker_filterDate_ValueChanged);
            // 
            // checkBox_filterDate
            // 
            this.checkBox_filterDate.AutoSize = true;
            this.checkBox_filterDate.Location = new System.Drawing.Point(27, 33);
            this.checkBox_filterDate.Name = "checkBox_filterDate";
            this.checkBox_filterDate.Size = new System.Drawing.Size(69, 17);
            this.checkBox_filterDate.TabIndex = 0;
            this.checkBox_filterDate.Text = "Modified:";
            this.checkBox_filterDate.UseVisualStyleBackColor = true;
            this.checkBox_filterDate.CheckedChanged += new System.EventHandler(this.checkBox_filterDate_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.ErrorImage = null;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(9, 115);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(24, 26);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 62;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox3.ErrorImage")));
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(14, 351);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(24, 26);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 62;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox4.ErrorImage")));
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(14, 263);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(24, 26);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 62;
            this.pictureBox4.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 63;
            this.label1.Text = "Filter Name:";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(84, 77);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(282, 20);
            this.textBox_name.TabIndex = 64;
            this.textBox_name.TextChanged += new System.EventHandler(this.textBox_name_TextChanged);
            // 
            // FilterAppend
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 516);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox_file_attributes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterAppend";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Filter";
            this.Load += new System.EventHandler(this.FilterAppend_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_file_attributes.ResumeLayout(false);
            this.groupBox_file_attributes.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox_header_title;
        private System.Windows.Forms.PictureBox pictureBox_header_image;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox_file_attributes;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox_names_extension_list;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.CheckBox checkBox_filterDate;
        private System.Windows.Forms.DateTimePicker dateTimePicker_filterDate;
        private System.Windows.Forms.ComboBox comboBox_filterDate_type;
        private System.Windows.Forms.ComboBox comboBox_attributes_archive;
        private System.Windows.Forms.CheckBox checkBox_attributes_archive;
        private System.Windows.Forms.ComboBox comboBox_attributes_readOnly;
        private System.Windows.Forms.CheckBox checkBox_attributes_readOnly;
        private System.Windows.Forms.ComboBox comboBox_attributes_hidden;
        private System.Windows.Forms.CheckBox checkBox_attributes_hidden;
        private System.Windows.Forms.ComboBox comboBox_attributes_system;
        private System.Windows.Forms.CheckBox checkBox_attributes_system;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.ComboBox comboBox_names_extension_list_type;
        internal System.Windows.Forms.CheckBox checkBox_regularExpression;
    }
}