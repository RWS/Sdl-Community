namespace PostEdit.Compare.Forms
{
    partial class ComparisonProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComparisonProject));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.linkLabel_fileAlignment = new System.Windows.Forms.LinkLabel();
            this.button_browse_path_right = new System.Windows.Forms.Button();
            this.button_browse_path_left = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_path_right = new System.Windows.Forms.TextBox();
            this.textBox_path_left = new System.Windows.Forms.TextBox();
            this.textBox_created = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox_header_image);
            this.panel6.Location = new System.Drawing.Point(12, 5);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(539, 56);
            this.panel6.TabIndex = 58;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(63, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(334, 25);
            this.textBox_header_title.TabIndex = 3;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = " Comparison Project";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(4, 6);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(56, 40);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(395, 350);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 60;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(476, 350);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 59;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.linkLabel_fileAlignment);
            this.groupBox1.Controls.Add(this.button_browse_path_right);
            this.groupBox1.Controls.Add(this.button_browse_path_left);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_path_right);
            this.groupBox1.Controls.Add(this.textBox_path_left);
            this.groupBox1.Controls.Add(this.textBox_created);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 277);
            this.groupBox1.TabIndex = 61;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(49, 230);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // linkLabel_fileAlignment
            // 
            this.linkLabel_fileAlignment.AutoSize = true;
            this.linkLabel_fileAlignment.Location = new System.Drawing.Point(83, 240);
            this.linkLabel_fileAlignment.Name = "linkLabel_fileAlignment";
            this.linkLabel_fileAlignment.Size = new System.Drawing.Size(123, 13);
            this.linkLabel_fileAlignment.TabIndex = 4;
            this.linkLabel_fileAlignment.TabStop = true;
            this.linkLabel_fileAlignment.Text = "View file name alignment";
            this.linkLabel_fileAlignment.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_fileAlignment_LinkClicked);
            // 
            // button_browse_path_right
            // 
            this.button_browse_path_right.Location = new System.Drawing.Point(479, 155);
            this.button_browse_path_right.Name = "button_browse_path_right";
            this.button_browse_path_right.Size = new System.Drawing.Size(26, 23);
            this.button_browse_path_right.TabIndex = 3;
            this.button_browse_path_right.Text = "...";
            this.button_browse_path_right.UseVisualStyleBackColor = true;
            this.button_browse_path_right.Click += new System.EventHandler(this.button_browse_path_right_Click);
            // 
            // button_browse_path_left
            // 
            this.button_browse_path_left.Location = new System.Drawing.Point(479, 86);
            this.button_browse_path_left.Name = "button_browse_path_left";
            this.button_browse_path_left.Size = new System.Drawing.Size(26, 23);
            this.button_browse_path_left.TabIndex = 3;
            this.button_browse_path_left.Text = "...";
            this.button_browse_path_left.UseVisualStyleBackColor = true;
            this.button_browse_path_left.Click += new System.EventHandler(this.button_browse_path_left_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 36);
            this.label4.TabIndex = 2;
            this.label4.Text = "Base Folder (right):";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(9, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 31);
            this.label3.TabIndex = 2;
            this.label3.Text = "Base Folder (left):";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Created:";
            // 
            // textBox_path_right
            // 
            this.textBox_path_right.AllowDrop = true;
            this.textBox_path_right.Location = new System.Drawing.Point(82, 154);
            this.textBox_path_right.Multiline = true;
            this.textBox_path_right.Name = "textBox_path_right";
            this.textBox_path_right.Size = new System.Drawing.Size(391, 61);
            this.textBox_path_right.TabIndex = 1;
            this.textBox_path_right.TextChanged += new System.EventHandler(this.textBox_path_right_TextChanged);
            this.textBox_path_right.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_path_right_DragDrop);
            this.textBox_path_right.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_path_right_DragOver);
            // 
            // textBox_path_left
            // 
            this.textBox_path_left.AllowDrop = true;
            this.textBox_path_left.Location = new System.Drawing.Point(82, 85);
            this.textBox_path_left.Multiline = true;
            this.textBox_path_left.Name = "textBox_path_left";
            this.textBox_path_left.Size = new System.Drawing.Size(391, 61);
            this.textBox_path_left.TabIndex = 1;
            this.textBox_path_left.TextChanged += new System.EventHandler(this.textBox_path_left_TextChanged);
            this.textBox_path_left.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_path_left_DragDrop);
            this.textBox_path_left.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_path_left_DragOver);
            // 
            // textBox_created
            // 
            this.textBox_created.Location = new System.Drawing.Point(82, 58);
            this.textBox_created.Name = "textBox_created";
            this.textBox_created.ReadOnly = true;
            this.textBox_created.Size = new System.Drawing.Size(391, 20);
            this.textBox_created.TabIndex = 1;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(82, 33);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(391, 20);
            this.textBox_name.TabIndex = 1;
            this.textBox_name.TextChanged += new System.EventHandler(this.textBox_name_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // ComparisonProject
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 380);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ComparisonProject";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Comparison Project";
            this.Load += new System.EventHandler(this.ComparisonListPair_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ComparisonListPair_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ComparisonListPair_DragOver);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox_header_title;
        private System.Windows.Forms.PictureBox pictureBox_header_image;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_browse_path_right;
        private System.Windows.Forms.Button button_browse_path_left;
        internal System.Windows.Forms.TextBox textBox_name;
        internal System.Windows.Forms.TextBox textBox_created;
        internal System.Windows.Forms.TextBox textBox_path_left;
        internal System.Windows.Forms.TextBox textBox_path_right;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.LinkLabel linkLabel_fileAlignment;
    }
}