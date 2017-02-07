namespace Sdl.Community.PostEdit.Versions
{
    partial class PostEditCompareNavigationControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostEditCompareNavigationControl));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel_top = new System.Windows.Forms.Panel();
            this.textBox_view = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.linkLabel_removeProjectVersions = new System.Windows.Forms.LinkLabel();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.label_NAVIGATION_MESSAGE = new System.Windows.Forms.Label();
            this.panel_middle = new System.Windows.Forms.Panel();
            this.treeView_navigation = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_navControl = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createNewProjectVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewProjectInWindowsExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_top.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel_bottom.SuspendLayout();
            this.panel_middle.SuspendLayout();
            this.contextMenuStrip_navControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Project.png");
            this.imageList1.Images.SetKeyName(1, "No-32.png");
            // 
            // panel_top
            // 
            this.panel_top.Controls.Add(this.textBox_view);
            this.panel_top.Controls.Add(this.panel2);
            this.panel_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_top.Location = new System.Drawing.Point(5, 5);
            this.panel_top.Name = "panel_top";
            this.panel_top.Padding = new System.Windows.Forms.Padding(0, 3, 5, 0);
            this.panel_top.Size = new System.Drawing.Size(327, 34);
            this.panel_top.TabIndex = 0;
            // 
            // textBox_view
            // 
            this.textBox_view.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_view.Location = new System.Drawing.Point(36, 3);
            this.textBox_view.Name = "textBox_view";
            this.textBox_view.Size = new System.Drawing.Size(286, 20);
            this.textBox_view.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 3);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.panel2.Size = new System.Drawing.Size(36, 31);
            this.panel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Right;
            this.label1.Location = new System.Drawing.Point(0, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "View: ";
            // 
            // panel_bottom
            // 
            this.panel_bottom.Controls.Add(this.linkLabel_removeProjectVersions);
            this.panel_bottom.Controls.Add(this.label_NAVIGATION_MESSAGE);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.Location = new System.Drawing.Point(5, 197);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Padding = new System.Windows.Forms.Padding(3);
            this.panel_bottom.Size = new System.Drawing.Size(327, 27);
            this.panel_bottom.TabIndex = 1;
            // 
            // linkLabel_removeProjectVersions
            // 
            this.linkLabel_removeProjectVersions.Dock = System.Windows.Forms.DockStyle.Right;
            this.linkLabel_removeProjectVersions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_removeProjectVersions.ImageIndex = 0;
            this.linkLabel_removeProjectVersions.ImageList = this.imageList2;
            this.linkLabel_removeProjectVersions.Location = new System.Drawing.Point(259, 3);
            this.linkLabel_removeProjectVersions.Name = "linkLabel_removeProjectVersions";
            this.linkLabel_removeProjectVersions.Size = new System.Drawing.Size(65, 21);
            this.linkLabel_removeProjectVersions.TabIndex = 1;
            this.linkLabel_removeProjectVersions.TabStop = true;
            this.linkLabel_removeProjectVersions.Text = "Remove";
            this.linkLabel_removeProjectVersions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Red X.ico");
            // 
            // label_NAVIGATION_MESSAGE
            // 
            this.label_NAVIGATION_MESSAGE.AutoSize = true;
            this.label_NAVIGATION_MESSAGE.Location = new System.Drawing.Point(4, 8);
            this.label_NAVIGATION_MESSAGE.Name = "label_NAVIGATION_MESSAGE";
            this.label_NAVIGATION_MESSAGE.Size = new System.Drawing.Size(137, 13);
            this.label_NAVIGATION_MESSAGE.TabIndex = 0;
            this.label_NAVIGATION_MESSAGE.Text = "[NAVIGATION_MESSAGE]";
            this.label_NAVIGATION_MESSAGE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_middle
            // 
            this.panel_middle.Controls.Add(this.treeView_navigation);
            this.panel_middle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_middle.Location = new System.Drawing.Point(5, 39);
            this.panel_middle.Name = "panel_middle";
            this.panel_middle.Padding = new System.Windows.Forms.Padding(3);
            this.panel_middle.Size = new System.Drawing.Size(327, 158);
            this.panel_middle.TabIndex = 2;
            // 
            // treeView_navigation
            // 
            this.treeView_navigation.ContextMenuStrip = this.contextMenuStrip_navControl;
            this.treeView_navigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_navigation.FullRowSelect = true;
            this.treeView_navigation.HideSelection = false;
            this.treeView_navigation.ImageIndex = 0;
            this.treeView_navigation.ImageList = this.imageList1;
            this.treeView_navigation.Location = new System.Drawing.Point(3, 3);
            this.treeView_navigation.Name = "treeView_navigation";
            this.treeView_navigation.SelectedImageIndex = 0;
            this.treeView_navigation.ShowPlusMinus = false;
            this.treeView_navigation.ShowRootLines = false;
            this.treeView_navigation.Size = new System.Drawing.Size(321, 152);
            this.treeView_navigation.TabIndex = 0;
            // 
            // contextMenuStrip_navControl
            // 
            this.contextMenuStrip_navControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewProjectVersionToolStripMenuItem,
            this.toolStripSeparator2,
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.viewProjectInWindowsExplorerToolStripMenuItem});
            this.contextMenuStrip_navControl.Name = "contextMenuStrip1";
            this.contextMenuStrip_navControl.Size = new System.Drawing.Size(250, 82);
            // 
            // createNewProjectVersionToolStripMenuItem
            // 
            this.createNewProjectVersionToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createNewProjectVersionToolStripMenuItem.Image")));
            this.createNewProjectVersionToolStripMenuItem.Name = "createNewProjectVersionToolStripMenuItem";
            this.createNewProjectVersionToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.createNewProjectVersionToolStripMenuItem.Text = "New Project Version";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(246, 6);
            // 
            // removeAllVersionsForTheSelectedProjectToolStripMenuItem
            // 
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeAllVersionsForTheSelectedProjectToolStripMenuItem.Image")));
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Name = "removeAllVersionsForTheSelectedProjectToolStripMenuItem";
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.removeAllVersionsForTheSelectedProjectToolStripMenuItem.Text = "Remove Project Versions";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // viewProjectInWindowsExplorerToolStripMenuItem
            // 
            this.viewProjectInWindowsExplorerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewProjectInWindowsExplorerToolStripMenuItem.Image")));
            this.viewProjectInWindowsExplorerToolStripMenuItem.Name = "viewProjectInWindowsExplorerToolStripMenuItem";
            this.viewProjectInWindowsExplorerToolStripMenuItem.Size = new System.Drawing.Size(249, 22);
            this.viewProjectInWindowsExplorerToolStripMenuItem.Text = "View Project in Windows Explorer";
            // 
            // PostEditCompareNavigationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel_middle);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.panel_top);
            this.Name = "PostEditCompareNavigationControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(337, 229);
            this.panel_top.ResumeLayout(false);
            this.panel_top.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel_bottom.ResumeLayout(false);
            this.panel_bottom.PerformLayout();
            this.panel_middle.ResumeLayout(false);
            this.contextMenuStrip_navControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel_top;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_bottom;
        private System.Windows.Forms.Panel panel_middle;
        internal System.Windows.Forms.TreeView treeView_navigation;
        internal System.Windows.Forms.Label label_NAVIGATION_MESSAGE;
        internal System.Windows.Forms.ToolStripMenuItem removeAllVersionsForTheSelectedProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem viewProjectInWindowsExplorerToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList2;
        internal System.Windows.Forms.LinkLabel linkLabel_removeProjectVersions;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem createNewProjectVersionToolStripMenuItem;
        internal System.Windows.Forms.ContextMenuStrip contextMenuStrip_navControl;
        internal System.Windows.Forms.TextBox textBox_view;


    }
}