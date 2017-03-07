namespace Sdl.Community.PostEdit.Versions.Dialogs
{
    partial class DefaultSettings
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DefaultSettings));
            this.panel6 = new System.Windows.Forms.Panel();
            this.textBox_header_title = new System.Windows.Forms.TextBox();
            this.pictureBox_header_image = new System.Windows.Forms.PictureBox();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox_createShallowCopy = new System.Windows.Forms.CheckBox();
            this.checkBox_createSubFolderProject = new System.Windows.Forms.CheckBox();
            this.linkLabel_viewFoldersInWindowsExplorer = new System.Windows.Forms.LinkLabel();
            this.button_browseProjectVersionsFolder = new System.Windows.Forms.Button();
            this.textBox_projectVersionsFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            this.panel6.Size = new System.Drawing.Size(561, 56);
            this.panel6.TabIndex = 61;
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
            this.textBox_header_title.Text = "Default Settings";
            // 
            // pictureBox_header_image
            // 
            this.pictureBox_header_image.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.Image")));
            this.pictureBox_header_image.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header_image.InitialImage")));
            this.pictureBox_header_image.Location = new System.Drawing.Point(21, 6);
            this.pictureBox_header_image.Name = "pictureBox_header_image";
            this.pictureBox_header_image.Size = new System.Drawing.Size(58, 45);
            this.pictureBox_header_image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header_image.TabIndex = 0;
            this.pictureBox_header_image.TabStop = false;
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(392, 242);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 65;
            this.button_save.Text = "&OK";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(473, 242);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 23);
            this.button_cancel.TabIndex = 64;
            this.button_cancel.Text = "&Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox_createShallowCopy);
            this.groupBox1.Controls.Add(this.checkBox_createSubFolderProject);
            this.groupBox1.Controls.Add(this.linkLabel_viewFoldersInWindowsExplorer);
            this.groupBox1.Controls.Add(this.button_browseProjectVersionsFolder);
            this.groupBox1.Controls.Add(this.textBox_projectVersionsFolder);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 71);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(536, 165);
            this.groupBox1.TabIndex = 66;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Properties";
            // 
            // checkBox_createShallowCopy
            // 
            this.checkBox_createShallowCopy.AutoSize = true;
            this.checkBox_createShallowCopy.Checked = true;
            this.checkBox_createShallowCopy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_createShallowCopy.Location = new System.Drawing.Point(19, 129);
            this.checkBox_createShallowCopy.Name = "checkBox_createShallowCopy";
            this.checkBox_createShallowCopy.Size = new System.Drawing.Size(195, 17);
            this.checkBox_createShallowCopy.TabIndex = 8;
            this.checkBox_createShallowCopy.Text = "Create a shallow copy of the project";
            this.checkBox_createShallowCopy.UseVisualStyleBackColor = true;
            // 
            // checkBox_createSubFolderProject
            // 
            this.checkBox_createSubFolderProject.AutoSize = true;
            this.checkBox_createSubFolderProject.Checked = true;
            this.checkBox_createSubFolderProject.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_createSubFolderProject.Enabled = false;
            this.checkBox_createSubFolderProject.Location = new System.Drawing.Point(19, 106);
            this.checkBox_createSubFolderProject.Name = "checkBox_createSubFolderProject";
            this.checkBox_createSubFolderProject.Size = new System.Drawing.Size(306, 17);
            this.checkBox_createSubFolderProject.TabIndex = 8;
            this.checkBox_createSubFolderProject.Text = "Create a sub-folder to maintain the versions for each project";
            this.checkBox_createSubFolderProject.UseVisualStyleBackColor = true;
            // 
            // linkLabel_viewFoldersInWindowsExplorer
            // 
            this.linkLabel_viewFoldersInWindowsExplorer.AutoSize = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Location = new System.Drawing.Point(16, 73);
            this.linkLabel_viewFoldersInWindowsExplorer.Name = "linkLabel_viewFoldersInWindowsExplorer";
            this.linkLabel_viewFoldersInWindowsExplorer.Size = new System.Drawing.Size(154, 13);
            this.linkLabel_viewFoldersInWindowsExplorer.TabIndex = 7;
            this.linkLabel_viewFoldersInWindowsExplorer.TabStop = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Text = "View folder in windows explorer";
            this.linkLabel_viewFoldersInWindowsExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_viewFoldersInWindowsExplorer_LinkClicked);
            // 
            // button_browseProjectVersionsFolder
            // 
            this.button_browseProjectVersionsFolder.Location = new System.Drawing.Point(493, 48);
            this.button_browseProjectVersionsFolder.Name = "button_browseProjectVersionsFolder";
            this.button_browseProjectVersionsFolder.Size = new System.Drawing.Size(25, 23);
            this.button_browseProjectVersionsFolder.TabIndex = 6;
            this.button_browseProjectVersionsFolder.Text = "...";
            this.button_browseProjectVersionsFolder.UseVisualStyleBackColor = true;
            this.button_browseProjectVersionsFolder.Click += new System.EventHandler(this.button_browseProjectVersionsFolder_Click);
            // 
            // textBox_projectVersionsFolder
            // 
            this.textBox_projectVersionsFolder.Location = new System.Drawing.Point(19, 50);
            this.textBox_projectVersionsFolder.Name = "textBox_projectVersionsFolder";
            this.textBox_projectVersionsFolder.Size = new System.Drawing.Size(468, 20);
            this.textBox_projectVersionsFolder.TabIndex = 5;
            this.textBox_projectVersionsFolder.TextChanged += new System.EventHandler(this.textBox_projectVersionsFolder_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Project versions root folder:";
            // 
            // DefaultSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 278);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.panel6);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DefaultSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Default Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header_image)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox textBox_header_title;
        private System.Windows.Forms.PictureBox pictureBox_header_image;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel linkLabel_viewFoldersInWindowsExplorer;
        private System.Windows.Forms.Button button_browseProjectVersionsFolder;
        public System.Windows.Forms.TextBox textBox_projectVersionsFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_createShallowCopy;
        private System.Windows.Forms.CheckBox checkBox_createSubFolderProject;
    }
}