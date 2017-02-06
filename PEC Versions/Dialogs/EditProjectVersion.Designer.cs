namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    partial class EditProjectVersion
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(EditProjectVersion));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel_viewFoldersInWindowsExplorer = new System.Windows.Forms.LinkLabel();
            this.textBox_location = new System.Windows.Forms.TextBox();
            this.textBox_description = new System.Windows.Forms.TextBox();
            this.textBox_createdAt = new System.Windows.Forms.TextBox();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabel_viewFoldersInWindowsExplorer);
            this.groupBox1.Controls.Add(this.textBox_location);
            this.groupBox1.Controls.Add(this.textBox_description);
            this.groupBox1.Controls.Add(this.textBox_createdAt);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 62);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(651, 228);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // linkLabel_viewFoldersInWindowsExplorer
            // 
            this.linkLabel_viewFoldersInWindowsExplorer.AutoSize = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Location = new System.Drawing.Point(477, 94);
            this.linkLabel_viewFoldersInWindowsExplorer.Name = "linkLabel_viewFoldersInWindowsExplorer";
            this.linkLabel_viewFoldersInWindowsExplorer.Size = new System.Drawing.Size(154, 13);
            this.linkLabel_viewFoldersInWindowsExplorer.TabIndex = 22;
            this.linkLabel_viewFoldersInWindowsExplorer.TabStop = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Text = "View folder in windows explorer";
            this.linkLabel_viewFoldersInWindowsExplorer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_viewFoldersInWindowsExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_viewFoldersInWindowsExplorer_LinkClicked);
            // 
            // textBox_location
            // 
            this.textBox_location.Location = new System.Drawing.Point(74, 71);
            this.textBox_location.Name = "textBox_location";
            this.textBox_location.ReadOnly = true;
            this.textBox_location.Size = new System.Drawing.Size(560, 20);
            this.textBox_location.TabIndex = 19;
            // 
            // textBox_description
            // 
            this.textBox_description.Location = new System.Drawing.Point(19, 136);
            this.textBox_description.Multiline = true;
            this.textBox_description.Name = "textBox_description";
            this.textBox_description.Size = new System.Drawing.Size(615, 74);
            this.textBox_description.TabIndex = 18;
            // 
            // textBox_createdAt
            // 
            this.textBox_createdAt.Location = new System.Drawing.Point(74, 47);
            this.textBox_createdAt.Name = "textBox_createdAt";
            this.textBox_createdAt.ReadOnly = true;
            this.textBox_createdAt.Size = new System.Drawing.Size(127, 20);
            this.textBox_createdAt.TabIndex = 17;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(74, 23);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(560, 20);
            this.textBox_name.TabIndex = 16;
            this.textBox_name.TextChanged += new System.EventHandler(this.textBox_name_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Description:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Location:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(33, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Date:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Name: ";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(507, 296);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 69;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(588, 296);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 68;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.textBox_header_title);
            this.panel6.Controls.Add(this.pictureBox_header_image);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(673, 56);
            this.panel6.TabIndex = 67;
            // 
            // textBox_header_title
            // 
            this.textBox_header_title.BackColor = System.Drawing.Color.White;
            this.textBox_header_title.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header_title.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header_title.Location = new System.Drawing.Point(85, 15);
            this.textBox_header_title.Name = "textBox_header_title";
            this.textBox_header_title.ReadOnly = true;
            this.textBox_header_title.Size = new System.Drawing.Size(462, 25);
            this.textBox_header_title.TabIndex = 3;
            this.textBox_header_title.TabStop = false;
            this.textBox_header_title.Text = "Project Version";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(25, 12);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(50, 34);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // EditProjectVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 329);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditProjectVersion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Version (Edit)";
            this.Load += new System.EventHandler(this.EditProjectVersion_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox_header_title;
        private System.Windows.Forms.PictureBox pictureBox_header_image;
        private System.Windows.Forms.LinkLabel linkLabel_viewFoldersInWindowsExplorer;
        public System.Windows.Forms.TextBox textBox_location;
        private System.Windows.Forms.TextBox textBox_description;
        private System.Windows.Forms.TextBox textBox_createdAt;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
    }
}