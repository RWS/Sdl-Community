namespace PostEdit.Compare.Forms
{
    partial class DeleteFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteFiles));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_scope_message = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_selected_right_side = new System.Windows.Forms.CheckBox();
            this.checkBox_selected_left_side = new System.Windows.Forms.CheckBox();
            this.label_size_right = new System.Windows.Forms.Label();
            this.pictureBox_selection_image = new System.Windows.Forms.PictureBox();
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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_selection_image)).BeginInit();
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
            this.textBox_header_title.Text = "Delete Files";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(20, 4);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(54, 46);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(315, 300);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 63;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(396, 300);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 211);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(458, 83);
            this.groupBox1.TabIndex = 64;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Details";
            // 
            // label_scope_message
            // 
            this.label_scope_message.Location = new System.Drawing.Point(24, 20);
            this.label_scope_message.Name = "label_scope_message";
            this.label_scope_message.Size = new System.Drawing.Size(405, 47);
            this.label_scope_message.TabIndex = 0;
            this.label_scope_message.Text = "[message]";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_selected_right_side);
            this.groupBox2.Controls.Add(this.checkBox_selected_left_side);
            this.groupBox2.Controls.Add(this.label_size_right);
            this.groupBox2.Controls.Add(this.pictureBox_selection_image);
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
            this.groupBox2.Location = new System.Drawing.Point(14, 74);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(456, 132);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selection";
            // 
            // checkBox_selected_right_side
            // 
            this.checkBox_selected_right_side.AutoSize = true;
            this.checkBox_selected_right_side.Location = new System.Drawing.Point(276, 29);
            this.checkBox_selected_right_side.Name = "checkBox_selected_right_side";
            this.checkBox_selected_right_side.Size = new System.Drawing.Size(75, 17);
            this.checkBox_selected_right_side.TabIndex = 2;
            this.checkBox_selected_right_side.Text = "Right Side";
            this.checkBox_selected_right_side.UseVisualStyleBackColor = true;
            this.checkBox_selected_right_side.CheckedChanged += new System.EventHandler(this.checkBox_selected_right_side_CheckedChanged);
            // 
            // checkBox_selected_left_side
            // 
            this.checkBox_selected_left_side.AutoSize = true;
            this.checkBox_selected_left_side.Location = new System.Drawing.Point(38, 29);
            this.checkBox_selected_left_side.Name = "checkBox_selected_left_side";
            this.checkBox_selected_left_side.Size = new System.Drawing.Size(68, 17);
            this.checkBox_selected_left_side.TabIndex = 2;
            this.checkBox_selected_left_side.Text = "Left Side";
            this.checkBox_selected_left_side.UseVisualStyleBackColor = true;
            this.checkBox_selected_left_side.CheckedChanged += new System.EventHandler(this.checkBox_selected_left_side_CheckedChanged);
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
            // pictureBox_selection_image
            // 
            this.pictureBox_selection_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_selection_image.Image")));
            this.pictureBox_selection_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_selection_image.InitialImage")));
            this.pictureBox_selection_image.Location = new System.Drawing.Point(164, 46);
            this.pictureBox_selection_image.Name = "pictureBox_selection_image";
            this.pictureBox_selection_image.Size = new System.Drawing.Size(50, 50);
            this.pictureBox_selection_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_selection_image.TabIndex = 0;
            this.pictureBox_selection_image.TabStop = false;
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
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Move-Right");
            this.imageList1.Images.SetKeyName(1, "Move-Left");
            this.imageList1.Images.SetKeyName(2, "Copy-Right");
            this.imageList1.Images.SetKeyName(3, "Copy-Left");
            // 
            // DeleteFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 332);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteFiles";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Delete Files";
            this.Load += new System.EventHandler(this.ManipulateFiles_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_selection_image)).EndInit();
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
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBox_selection_image;
        public System.Windows.Forms.CheckBox checkBox_selected_right_side;
        public System.Windows.Forms.CheckBox checkBox_selected_left_side;
        public System.Windows.Forms.Label label_scope_message;
    }
}