namespace Sdl.Community.PostEdit.Versions
{    
    partial class PostEditCompareViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostEditCompareViewControl));
			this.panel1 = new System.Windows.Forms.Panel();
			this.label_SELECTED_PROJECT_NAME = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label7 = new System.Windows.Forms.Label();
			this.label_TOTAL_PROJECT_VERSIONS = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label_PROJECT_VERSIONS_MESSAGE = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.listView_postEditCompareProjectVersions = new System.Windows.Forms.ListView();
			this.columnHeader_id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_projectVersionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_projectVersionDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_dateCreatedAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_sourceLanguage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_projectTargetLanguages = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_totalFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_shallowCopy = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_translatableFiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_location = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.saveNewProjectVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editSelectedProjectVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeSelectedProjectVersionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.restoreProjectVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.compareWithPostEditCompareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.viewProjectVersionInWindowsExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel3.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label_SELECTED_PROJECT_NAME);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(3, 5, 5, 0);
			this.panel1.Size = new System.Drawing.Size(1190, 27);
			this.panel1.TabIndex = 0;
			// 
			// label_SELECTED_PROJECT_NAME
			// 
			this.label_SELECTED_PROJECT_NAME.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_SELECTED_PROJECT_NAME.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_SELECTED_PROJECT_NAME.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label_SELECTED_PROJECT_NAME.Location = new System.Drawing.Point(3, 5);
			this.label_SELECTED_PROJECT_NAME.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.label_SELECTED_PROJECT_NAME.Name = "label_SELECTED_PROJECT_NAME";
			this.label_SELECTED_PROJECT_NAME.Size = new System.Drawing.Size(1182, 22);
			this.label_SELECTED_PROJECT_NAME.TabIndex = 4;
			this.label_SELECTED_PROJECT_NAME.Text = "[SELECTED_PROJECT_NAME]";
			this.label_SELECTED_PROJECT_NAME.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.label7);
			this.panel2.Controls.Add(this.label_TOTAL_PROJECT_VERSIONS);
			this.panel2.Controls.Add(this.label5);
			this.panel2.Controls.Add(this.label_PROJECT_VERSIONS_MESSAGE);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.label_TOTAL_PROJECT_VERSIONS_SELECTED);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 374);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
			this.panel2.Size = new System.Drawing.Size(1190, 21);
			this.panel2.TabIndex = 1;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Dock = System.Windows.Forms.DockStyle.Right;
			this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label7.Location = new System.Drawing.Point(668, 4);
			this.label7.Margin = new System.Windows.Forms.Padding(0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(77, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "Total Versions:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label_TOTAL_PROJECT_VERSIONS
			// 
			this.label_TOTAL_PROJECT_VERSIONS.AutoSize = true;
			this.label_TOTAL_PROJECT_VERSIONS.Dock = System.Windows.Forms.DockStyle.Right;
			this.label_TOTAL_PROJECT_VERSIONS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label_TOTAL_PROJECT_VERSIONS.Location = new System.Drawing.Point(745, 4);
			this.label_TOTAL_PROJECT_VERSIONS.Margin = new System.Windows.Forms.Padding(0);
			this.label_TOTAL_PROJECT_VERSIONS.Name = "label_TOTAL_PROJECT_VERSIONS";
			this.label_TOTAL_PROJECT_VERSIONS.Size = new System.Drawing.Size(164, 13);
			this.label_TOTAL_PROJECT_VERSIONS.TabIndex = 5;
			this.label_TOTAL_PROJECT_VERSIONS.Text = "[TOTAL_PROJECT_VERSIONS]";
			this.label_TOTAL_PROJECT_VERSIONS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Dock = System.Windows.Forms.DockStyle.Right;
			this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label5.Location = new System.Drawing.Point(909, 4);
			this.label5.Margin = new System.Windows.Forms.Padding(0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(52, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Selected:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label_PROJECT_VERSIONS_MESSAGE
			// 
			this.label_PROJECT_VERSIONS_MESSAGE.AutoSize = true;
			this.label_PROJECT_VERSIONS_MESSAGE.Dock = System.Windows.Forms.DockStyle.Left;
			this.label_PROJECT_VERSIONS_MESSAGE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_PROJECT_VERSIONS_MESSAGE.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label_PROJECT_VERSIONS_MESSAGE.Location = new System.Drawing.Point(21, 4);
			this.label_PROJECT_VERSIONS_MESSAGE.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.label_PROJECT_VERSIONS_MESSAGE.Name = "label_PROJECT_VERSIONS_MESSAGE";
			this.label_PROJECT_VERSIONS_MESSAGE.Size = new System.Drawing.Size(181, 13);
			this.label_PROJECT_VERSIONS_MESSAGE.TabIndex = 3;
			this.label_PROJECT_VERSIONS_MESSAGE.Text = "[PROJECT_VERSIONS_MESSAGE]";
			this.label_PROJECT_VERSIONS_MESSAGE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(3, 4);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(18, 14);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// label_TOTAL_PROJECT_VERSIONS_SELECTED
			// 
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.AutoSize = true;
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Dock = System.Windows.Forms.DockStyle.Right;
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Location = new System.Drawing.Point(961, 4);
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Margin = new System.Windows.Forms.Padding(0);
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Name = "label_TOTAL_PROJECT_VERSIONS_SELECTED";
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Size = new System.Drawing.Size(226, 13);
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.TabIndex = 1;
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.Text = "[TOTAL_PROJECT_VERSIONS_SELECTED]";
			this.label_TOTAL_PROJECT_VERSIONS_SELECTED.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.listView_postEditCompareProjectVersions);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Location = new System.Drawing.Point(0, 27);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(1190, 347);
			this.panel3.TabIndex = 2;
			// 
			// listView_postEditCompareProjectVersions
			// 
			this.listView_postEditCompareProjectVersions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_id,
            this.columnHeader_projectVersionName,
            this.columnHeader_projectVersionDescription,
            this.columnHeader_dateCreatedAt,
            this.columnHeader_sourceLanguage,
            this.columnHeader_projectTargetLanguages,
            this.columnHeader_totalFiles,
            this.columnHeader_shallowCopy,
            this.columnHeader_translatableFiles,
            this.columnHeader_location});
			this.listView_postEditCompareProjectVersions.ContextMenuStrip = this.contextMenuStrip1;
			this.listView_postEditCompareProjectVersions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView_postEditCompareProjectVersions.FullRowSelect = true;
			this.listView_postEditCompareProjectVersions.HideSelection = false;
			this.listView_postEditCompareProjectVersions.Location = new System.Drawing.Point(0, 0);
			this.listView_postEditCompareProjectVersions.Name = "listView_postEditCompareProjectVersions";
			this.listView_postEditCompareProjectVersions.Size = new System.Drawing.Size(1190, 347);
			this.listView_postEditCompareProjectVersions.SmallImageList = this.imageList1;
			this.listView_postEditCompareProjectVersions.TabIndex = 0;
			this.listView_postEditCompareProjectVersions.UseCompatibleStateImageBehavior = false;
			this.listView_postEditCompareProjectVersions.View = System.Windows.Forms.View.Details;
			this.listView_postEditCompareProjectVersions.SelectedIndexChanged += new System.EventHandler(this.listView_postEditCompareProjectVersions_SelectedIndexChanged);
			// 
			// columnHeader_id
			// 
			this.columnHeader_id.Text = "ID";
			// 
			// columnHeader_projectVersionName
			// 
			this.columnHeader_projectVersionName.Text = "Project Version Name";
			this.columnHeader_projectVersionName.Width = 180;
			// 
			// columnHeader_projectVersionDescription
			// 
			this.columnHeader_projectVersionDescription.Text = "Description";
			this.columnHeader_projectVersionDescription.Width = 160;
			// 
			// columnHeader_dateCreatedAt
			// 
			this.columnHeader_dateCreatedAt.Text = "Version Created At";
			this.columnHeader_dateCreatedAt.Width = 120;
			// 
			// columnHeader_sourceLanguage
			// 
			this.columnHeader_sourceLanguage.Text = "Source Language";
			this.columnHeader_sourceLanguage.Width = 100;
			// 
			// columnHeader_projectTargetLanguages
			// 
			this.columnHeader_projectTargetLanguages.Text = "Target Languages";
			this.columnHeader_projectTargetLanguages.Width = 120;
			// 
			// columnHeader_totalFiles
			// 
			this.columnHeader_totalFiles.Text = "Total Files";
			this.columnHeader_totalFiles.Width = 70;
			// 
			// columnHeader_shallowCopy
			// 
			this.columnHeader_shallowCopy.Text = "Shallow Copy";
			// 
			// columnHeader_translatableFiles
			// 
			this.columnHeader_translatableFiles.Text = "Soruce Files";
			this.columnHeader_translatableFiles.Width = 50;
			// 
			// columnHeader_location
			// 
			this.columnHeader_location.Text = "Location";
			this.columnHeader_location.Width = 300;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveNewProjectVersionToolStripMenuItem,
            this.editSelectedProjectVersionToolStripMenuItem,
            this.removeSelectedProjectVersionsToolStripMenuItem,
            this.restoreProjectVersionToolStripMenuItem,
            this.toolStripSeparator1,
            this.compareWithPostEditCompareToolStripMenuItem,
            this.toolStripSeparator2,
            this.viewProjectVersionInWindowsExplorerToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(291, 148);
			// 
			// saveNewProjectVersionToolStripMenuItem
			// 
			this.saveNewProjectVersionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveNewProjectVersionToolStripMenuItem.Image")));
			this.saveNewProjectVersionToolStripMenuItem.Name = "saveNewProjectVersionToolStripMenuItem";
			this.saveNewProjectVersionToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.saveNewProjectVersionToolStripMenuItem.Text = "New Project Version";
			// 
			// editSelectedProjectVersionToolStripMenuItem
			// 
			this.editSelectedProjectVersionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editSelectedProjectVersionToolStripMenuItem.Image")));
			this.editSelectedProjectVersionToolStripMenuItem.Name = "editSelectedProjectVersionToolStripMenuItem";
			this.editSelectedProjectVersionToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.editSelectedProjectVersionToolStripMenuItem.Text = "Edit Project Version";
			// 
			// removeSelectedProjectVersionsToolStripMenuItem
			// 
			this.removeSelectedProjectVersionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeSelectedProjectVersionsToolStripMenuItem.Image")));
			this.removeSelectedProjectVersionsToolStripMenuItem.Name = "removeSelectedProjectVersionsToolStripMenuItem";
			this.removeSelectedProjectVersionsToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.removeSelectedProjectVersionsToolStripMenuItem.Text = "Remove Project Version";
			// 
			// restoreProjectVersionToolStripMenuItem
			// 
			this.restoreProjectVersionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restoreProjectVersionToolStripMenuItem.Image")));
			this.restoreProjectVersionToolStripMenuItem.Name = "restoreProjectVersionToolStripMenuItem";
			this.restoreProjectVersionToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.restoreProjectVersionToolStripMenuItem.Text = "Restore Project Version";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(287, 6);
			// 
			// compareWithPostEditCompareToolStripMenuItem
			// 
			this.compareWithPostEditCompareToolStripMenuItem.Enabled = false;
			this.compareWithPostEditCompareToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("compareWithPostEditCompareToolStripMenuItem.Image")));
			this.compareWithPostEditCompareToolStripMenuItem.Name = "compareWithPostEditCompareToolStripMenuItem";
			this.compareWithPostEditCompareToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.compareWithPostEditCompareToolStripMenuItem.Text = "Compare with \'Post-Edit Compare\'";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(287, 6);
			// 
			// viewProjectVersionInWindowsExplorerToolStripMenuItem
			// 
			this.viewProjectVersionInWindowsExplorerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewProjectVersionInWindowsExplorerToolStripMenuItem.Image")));
			this.viewProjectVersionInWindowsExplorerToolStripMenuItem.Name = "viewProjectVersionInWindowsExplorerToolStripMenuItem";
			this.viewProjectVersionInWindowsExplorerToolStripMenuItem.Size = new System.Drawing.Size(290, 22);
			this.viewProjectVersionInWindowsExplorerToolStripMenuItem.Text = "View Project Version in Windows Explorer";
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Warning");
			this.imageList1.Images.SetKeyName(1, "Blue");
			this.imageList1.Images.SetKeyName(2, "Green");
			this.imageList1.Images.SetKeyName(3, "Red");
			this.imageList1.Images.SetKeyName(4, "Yellow");
			// 
			// PostEditCompareViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "PostEditCompareViewControl";
			this.Size = new System.Drawing.Size(1190, 395);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel3.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ColumnHeader columnHeader_projectVersionName;
        private System.Windows.Forms.ColumnHeader columnHeader_dateCreatedAt;
        private System.Windows.Forms.ColumnHeader columnHeader_location;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ColumnHeader columnHeader_sourceLanguage;
        private System.Windows.Forms.ColumnHeader columnHeader_projectTargetLanguages;
        private System.Windows.Forms.ColumnHeader columnHeader_translatableFiles;
        private System.Windows.Forms.ColumnHeader columnHeader_projectVersionDescription;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.ListView listView_postEditCompareProjectVersions;
        internal System.Windows.Forms.Label label_TOTAL_PROJECT_VERSIONS_SELECTED;
        internal System.Windows.Forms.Label label_PROJECT_VERSIONS_MESSAGE;
        internal System.Windows.Forms.Label label_TOTAL_PROJECT_VERSIONS;
        private System.Windows.Forms.ColumnHeader columnHeader_shallowCopy;
        internal System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ColumnHeader columnHeader_id;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem saveNewProjectVersionToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem editSelectedProjectVersionToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem removeSelectedProjectVersionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem viewProjectVersionInWindowsExplorerToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem compareWithPostEditCompareToolStripMenuItem;
        internal System.Windows.Forms.Label label_SELECTED_PROJECT_NAME;
        private System.Windows.Forms.ColumnHeader columnHeader_totalFiles;
        internal System.Windows.Forms.ToolStripMenuItem restoreProjectVersionToolStripMenuItem;


    }
}