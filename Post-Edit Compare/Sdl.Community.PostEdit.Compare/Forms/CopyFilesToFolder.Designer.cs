namespace PostEdit.Compare.Forms
{
    partial class CopyFilesToFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyFilesToFolder));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_scope_message = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_right_side = new System.Windows.Forms.RadioButton();
            this.radioButton_left_side = new System.Windows.Forms.RadioButton();
            this.label_size_right = new System.Windows.Forms.Label();
            this.label_size_left = new System.Windows.Forms.Label();
            this.label_folderCount_right = new System.Windows.Forms.Label();
            this.label_folderCount_left = new System.Windows.Forms.Label();
            this.label_fileCount_right = new System.Windows.Forms.Label();
            this.label_fileCount_left = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox_selection_image_left = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_overwirte_existing_files = new System.Windows.Forms.CheckBox();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_selection_image_left)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox_header_image);
            this.panel6.Location = new System.Drawing.Point(12, 12);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(458, 56);
            this.panel6.TabIndex = 60;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(85, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(357, 25);
            this.textBox_header_title.TabIndex = 3;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = "Copy Selected Files to...";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(22, 9);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(48, 34);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Enabled = false;
            this.button_save.Location = new System.Drawing.Point(314, 400);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 63;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(395, 400);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 62;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_scope_message);
            this.groupBox1.Location = new System.Drawing.Point(12, 314);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 83);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // label_scope_message
            // 
            this.label_scope_message.Location = new System.Drawing.Point(18, 19);
            this.label_scope_message.Name = "label_scope_message";
            this.label_scope_message.Size = new System.Drawing.Size(424, 47);
            this.label_scope_message.TabIndex = 0;
            this.label_scope_message.Text = "[message]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_right_side);
            this.groupBox2.Controls.Add(this.radioButton_left_side);
            this.groupBox2.Controls.Add(this.label_size_right);
            this.groupBox2.Controls.Add(this.label_size_left);
            this.groupBox2.Controls.Add(this.label_folderCount_right);
            this.groupBox2.Controls.Add(this.label_folderCount_left);
            this.groupBox2.Controls.Add(this.label_fileCount_right);
            this.groupBox2.Controls.Add(this.label_fileCount_left);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pictureBox_selection_image_left);
            this.groupBox2.Location = new System.Drawing.Point(14, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 132);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection";
            // 
            // radioButton_right_side
            // 
            this.radioButton_right_side.AutoSize = true;
            this.radioButton_right_side.Location = new System.Drawing.Point(270, 30);
            this.radioButton_right_side.Name = "radioButton_right_side";
            this.radioButton_right_side.Size = new System.Drawing.Size(74, 17);
            this.radioButton_right_side.TabIndex = 2;
            this.radioButton_right_side.Text = "Right Side";
            this.radioButton_right_side.UseVisualStyleBackColor = true;
            this.radioButton_right_side.CheckedChanged += new System.EventHandler(this.radioButton_right_side_CheckedChanged);
            // 
            // radioButton_left_side
            // 
            this.radioButton_left_side.AutoSize = true;
            this.radioButton_left_side.Checked = true;
            this.radioButton_left_side.Location = new System.Drawing.Point(35, 30);
            this.radioButton_left_side.Name = "radioButton_left_side";
            this.radioButton_left_side.Size = new System.Drawing.Size(67, 17);
            this.radioButton_left_side.TabIndex = 2;
            this.radioButton_left_side.TabStop = true;
            this.radioButton_left_side.Text = "Left Side";
            this.radioButton_left_side.UseVisualStyleBackColor = true;
            this.radioButton_left_side.CheckedChanged += new System.EventHandler(this.radioButton_left_side_CheckedChanged);
            // 
            // label_size_right
            // 
            this.label_size_right.AutoSize = true;
            this.label_size_right.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_size_right.Location = new System.Drawing.Point(338, 94);
            this.label_size_right.Name = "label_size_right";
            this.label_size_right.Size = new System.Drawing.Size(13, 13);
            this.label_size_right.TabIndex = 1;
            this.label_size_right.Text = "0";
            // 
            // label_size_left
            // 
            this.label_size_left.AutoSize = true;
            this.label_size_left.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_size_left.Location = new System.Drawing.Point(103, 93);
            this.label_size_left.Name = "label_size_left";
            this.label_size_left.Size = new System.Drawing.Size(13, 13);
            this.label_size_left.TabIndex = 1;
            this.label_size_left.Text = "0";
            // 
            // label_folderCount_right
            // 
            this.label_folderCount_right.AutoSize = true;
            this.label_folderCount_right.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_folderCount_right.Location = new System.Drawing.Point(338, 75);
            this.label_folderCount_right.Name = "label_folderCount_right";
            this.label_folderCount_right.Size = new System.Drawing.Size(13, 13);
            this.label_folderCount_right.TabIndex = 1;
            this.label_folderCount_right.Text = "0";
            // 
            // label_folderCount_left
            // 
            this.label_folderCount_left.AutoSize = true;
            this.label_folderCount_left.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_folderCount_left.Location = new System.Drawing.Point(103, 74);
            this.label_folderCount_left.Name = "label_folderCount_left";
            this.label_folderCount_left.Size = new System.Drawing.Size(13, 13);
            this.label_folderCount_left.TabIndex = 1;
            this.label_folderCount_left.Text = "0";
            // 
            // label_fileCount_right
            // 
            this.label_fileCount_right.AutoSize = true;
            this.label_fileCount_right.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_fileCount_right.Location = new System.Drawing.Point(338, 56);
            this.label_fileCount_right.Name = "label_fileCount_right";
            this.label_fileCount_right.Size = new System.Drawing.Size(13, 13);
            this.label_fileCount_right.TabIndex = 1;
            this.label_fileCount_right.Text = "0";
            // 
            // label_fileCount_left
            // 
            this.label_fileCount_left.AutoSize = true;
            this.label_fileCount_left.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_fileCount_left.Location = new System.Drawing.Point(103, 55);
            this.label_fileCount_left.Name = "label_fileCount_left";
            this.label_fileCount_left.Size = new System.Drawing.Size(13, 13);
            this.label_fileCount_left.TabIndex = 1;
            this.label_fileCount_left.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Location = new System.Drawing.Point(281, 94);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(36, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "- Size:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(46, 93);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "- Size:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(281, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "- Folders:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(46, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "- Folders:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(281, 56);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "- Files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(46, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "- Files:";
            // 
            // pictureBox_selection_image_left
            // 
            this.pictureBox_selection_image_left.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_selection_image_left.Image")));
            this.pictureBox_selection_image_left.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_selection_image_left.InitialImage")));
            this.pictureBox_selection_image_left.Location = new System.Drawing.Point(165, 48);
            this.pictureBox_selection_image_left.Name = "pictureBox_selection_image_left";
            this.pictureBox_selection_image_left.Size = new System.Drawing.Size(50, 50);
            this.pictureBox_selection_image_left.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_selection_image_left.TabIndex = 3;
            this.pictureBox_selection_image_left.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.checkBox_overwirte_existing_files);
            this.groupBox3.Location = new System.Drawing.Point(12, 212);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(458, 96);
            this.groupBox3.TabIndex = 66;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Action";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(418, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(25, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(21, 42);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(390, 20);
            this.textBox1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Copy selected files to folder:";
            // 
            // checkBox_overwirte_existing_files
            // 
            this.checkBox_overwirte_existing_files.AutoSize = true;
            this.checkBox_overwirte_existing_files.Location = new System.Drawing.Point(23, 69);
            this.checkBox_overwirte_existing_files.Name = "checkBox_overwirte_existing_files";
            this.checkBox_overwirte_existing_files.Size = new System.Drawing.Size(130, 17);
            this.checkBox_overwirte_existing_files.TabIndex = 0;
            this.checkBox_overwirte_existing_files.Text = "Overwrite existing files";
            this.checkBox_overwirte_existing_files.UseVisualStyleBackColor = true;
            this.checkBox_overwirte_existing_files.CheckedChanged += new System.EventHandler(this.checkBox_overwirte_existing_files_CheckedChanged);
            // 
            // CopyFilesToFolder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 427);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CopyFilesToFolder";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Move Files";
            this.Load += new System.EventHandler(this.ManipulateFiles_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_selection_image_left)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox_header_title;
        private System.Windows.Forms.PictureBox pictureBox_header_image;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label_size_left;
        public System.Windows.Forms.Label label_folderCount_left;
        public System.Windows.Forms.Label label_fileCount_left;
        public System.Windows.Forms.Label label_size_right;
        public System.Windows.Forms.Label label_folderCount_right;
        public System.Windows.Forms.Label label_fileCount_right;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox checkBox_overwirte_existing_files;
        public System.Windows.Forms.PictureBox pictureBox_selection_image_left;
        public System.Windows.Forms.RadioButton radioButton_right_side;
        public System.Windows.Forms.RadioButton radioButton_left_side;
        public System.Windows.Forms.TextBox textBox1;
        public System.Windows.Forms.Label label_scope_message;
    }
}