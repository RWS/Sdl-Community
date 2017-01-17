namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{
    partial class StudioTimeTrackerNavigationControl
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(StudioTimeTrackerNavigationControl));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.panel_middle = new System.Windows.Forms.Panel();
            this.treeView_navigation = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_navigation_control = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newTimeTrackerProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTimeTrackerProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTimeTrackerProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.comboBox_groupBy = new System.Windows.Forms.ComboBox();
            this.panel10 = new System.Windows.Forms.Panel();
            this.button_auto_expand_treeview = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_top = new System.Windows.Forms.Panel();
            this.groupBox_filter = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.textBox_project_name = new System.Windows.Forms.TextBox();
            this.button_project_search = new System.Windows.Forms.Button();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.comboBox_project_status = new System.Windows.Forms.ComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel_middle.SuspendLayout();
            this.contextMenuStrip_navigation_control.SuspendLayout();
            this.panel_bottom.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel11.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel_top.SuspendLayout();
            this.groupBox_filter.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Error");
            this.imageList1.Images.SetKeyName(1, "ClientBlue");
            this.imageList1.Images.SetKeyName(2, "Project");
            this.imageList1.Images.SetKeyName(3, "ProjectInProgress2");
            this.imageList1.Images.SetKeyName(4, "ProjectCompleted2");
            this.imageList1.Images.SetKeyName(5, "ProjectInProgress");
            this.imageList1.Images.SetKeyName(6, "ProjectCompleted");
            this.imageList1.Images.SetKeyName(7, "ClientGreen");
            this.imageList1.Images.SetKeyName(8, "ClientMix");
            this.imageList1.Images.SetKeyName(9, "ClientBlack");
            this.imageList1.Images.SetKeyName(10, "MonthGreen");
            this.imageList1.Images.SetKeyName(11, "MonthBlue");
            this.imageList1.Images.SetKeyName(12, "MonthOrange");
            this.imageList1.Images.SetKeyName(13, "Year");
            this.imageList1.Images.SetKeyName(14, "Client");
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Red X.ico");
            // 
            // panel_middle
            // 
            this.panel_middle.Controls.Add(this.treeView_navigation);
            this.panel_middle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_middle.Location = new System.Drawing.Point(5, 72);
            this.panel_middle.Name = "panel_middle";
            this.panel_middle.Padding = new System.Windows.Forms.Padding(3);
            this.panel_middle.Size = new System.Drawing.Size(344, 140);
            this.panel_middle.TabIndex = 2;
            // 
            // treeView_navigation
            // 
            this.treeView_navigation.ContextMenuStrip = this.contextMenuStrip_navigation_control;
            this.treeView_navigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_navigation.FullRowSelect = true;
            this.treeView_navigation.HideSelection = false;
            this.treeView_navigation.ImageIndex = 0;
            this.treeView_navigation.ImageList = this.imageList1;
            this.treeView_navigation.Indent = 20;
            this.treeView_navigation.ItemHeight = 20;
            this.treeView_navigation.Location = new System.Drawing.Point(3, 3);
            this.treeView_navigation.Name = "treeView_navigation";
            this.treeView_navigation.SelectedImageIndex = 0;
            this.treeView_navigation.Size = new System.Drawing.Size(338, 134);
            this.treeView_navigation.TabIndex = 0;
            // 
            // contextMenuStrip_navigation_control
            // 
            this.contextMenuStrip_navigation_control.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTimeTrackerProjectToolStripMenuItem,
            this.editTimeTrackerProjectToolStripMenuItem,
            this.removeTimeTrackerProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.newProjectActivityToolStripMenuItem});
            this.contextMenuStrip_navigation_control.Name = "contextMenuStrip1";
            this.contextMenuStrip_navigation_control.Size = new System.Drawing.Size(230, 98);
            // 
            // newTimeTrackerProjectToolStripMenuItem
            // 
            this.newTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newTimeTrackerProjectToolStripMenuItem.Image")));
            this.newTimeTrackerProjectToolStripMenuItem.Name = "newTimeTrackerProjectToolStripMenuItem";
            this.newTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.newTimeTrackerProjectToolStripMenuItem.Text = "New Time Tracker Project";
            // 
            // editTimeTrackerProjectToolStripMenuItem
            // 
            this.editTimeTrackerProjectToolStripMenuItem.Enabled = false;
            this.editTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editTimeTrackerProjectToolStripMenuItem.Image")));
            this.editTimeTrackerProjectToolStripMenuItem.Name = "editTimeTrackerProjectToolStripMenuItem";
            this.editTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.editTimeTrackerProjectToolStripMenuItem.Text = "Edit Time Tracker Project";
            // 
            // removeTimeTrackerProjectToolStripMenuItem
            // 
            this.removeTimeTrackerProjectToolStripMenuItem.Enabled = false;
            this.removeTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeTimeTrackerProjectToolStripMenuItem.Image")));
            this.removeTimeTrackerProjectToolStripMenuItem.Name = "removeTimeTrackerProjectToolStripMenuItem";
            this.removeTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.removeTimeTrackerProjectToolStripMenuItem.Text = "Remove Time Tracker Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(226, 6);
            // 
            // newProjectActivityToolStripMenuItem
            // 
            this.newProjectActivityToolStripMenuItem.Enabled = false;
            this.newProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectActivityToolStripMenuItem.Image")));
            this.newProjectActivityToolStripMenuItem.Name = "newProjectActivityToolStripMenuItem";
            this.newProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(229, 22);
            this.newProjectActivityToolStripMenuItem.Text = "New Project Activity";
            // 
            // panel_bottom
            // 
            this.panel_bottom.Controls.Add(this.panel3);
            this.panel_bottom.Controls.Add(this.panel1);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.Location = new System.Drawing.Point(5, 212);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Padding = new System.Windows.Forms.Padding(3);
            this.panel_bottom.Size = new System.Drawing.Size(344, 28);
            this.panel_bottom.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(338, 0);
            this.panel3.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(338, 24);
            this.panel1.TabIndex = 3;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.panel11);
            this.panel8.Controls.Add(this.panel10);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(54, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(284, 24);
            this.panel8.TabIndex = 1;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.comboBox_groupBy);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.panel11.Size = new System.Drawing.Size(255, 24);
            this.panel11.TabIndex = 1;
            // 
            // comboBox_groupBy
            // 
            this.comboBox_groupBy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_groupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_groupBy.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_groupBy.FormattingEnabled = true;
            this.comboBox_groupBy.Items.AddRange(new object[] {
            "Client name",
            "Project name",
            "Project created",
            "Project due"});
            this.comboBox_groupBy.Location = new System.Drawing.Point(0, 1);
            this.comboBox_groupBy.Name = "comboBox_groupBy";
            this.comboBox_groupBy.Size = new System.Drawing.Size(255, 21);
            this.comboBox_groupBy.TabIndex = 6;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.button_auto_expand_treeview);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(255, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(29, 24);
            this.panel10.TabIndex = 0;
            // 
            // button_auto_expand_treeview
            // 
            this.button_auto_expand_treeview.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_auto_expand_treeview.Image = ((System.Drawing.Image)(resources.GetObject("button_auto_expand_treeview.Image")));
            this.button_auto_expand_treeview.Location = new System.Drawing.Point(0, 0);
            this.button_auto_expand_treeview.Name = "button_auto_expand_treeview";
            this.button_auto_expand_treeview.Size = new System.Drawing.Size(29, 24);
            this.button_auto_expand_treeview.TabIndex = 2;
            this.button_auto_expand_treeview.UseVisualStyleBackColor = true;
            this.button_auto_expand_treeview.MouseHover += new System.EventHandler(this.button_auto_expand_treeview_MouseHover);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label1);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(54, 24);
            this.panel7.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group by:";
            // 
            // panel_top
            // 
            this.panel_top.Controls.Add(this.groupBox_filter);
            this.panel_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_top.Location = new System.Drawing.Point(5, 5);
            this.panel_top.Name = "panel_top";
            this.panel_top.Padding = new System.Windows.Forms.Padding(4, 3, 3, 0);
            this.panel_top.Size = new System.Drawing.Size(344, 67);
            this.panel_top.TabIndex = 0;
            // 
            // groupBox_filter
            // 
            this.groupBox_filter.Controls.Add(this.panel2);
            this.groupBox_filter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_filter.Location = new System.Drawing.Point(4, 3);
            this.groupBox_filter.Name = "groupBox_filter";
            this.groupBox_filter.Size = new System.Drawing.Size(337, 64);
            this.groupBox_filter.TabIndex = 0;
            this.groupBox_filter.TabStop = false;
            this.groupBox_filter.Text = "Project Filter";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 16);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(331, 45);
            this.panel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.textBox_project_name);
            this.panel5.Controls.Add(this.button_project_search);
            this.panel5.Controls.Add(this.panel9);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 23);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(331, 22);
            this.panel5.TabIndex = 1;
            // 
            // textBox_project_name
            // 
            this.textBox_project_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_project_name.Location = new System.Drawing.Point(50, 0);
            this.textBox_project_name.Name = "textBox_project_name";
            this.textBox_project_name.Size = new System.Drawing.Size(255, 20);
            this.textBox_project_name.TabIndex = 3;
            // 
            // button_project_search
            // 
            this.button_project_search.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_project_search.Image = ((System.Drawing.Image)(resources.GetObject("button_project_search.Image")));
            this.button_project_search.Location = new System.Drawing.Point(305, 0);
            this.button_project_search.Name = "button_project_search";
            this.button_project_search.Size = new System.Drawing.Size(26, 22);
            this.button_project_search.TabIndex = 2;
            this.button_project_search.UseVisualStyleBackColor = true;
            this.button_project_search.MouseHover += new System.EventHandler(this.button_project_search_MouseHover);
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label3);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.panel9.Size = new System.Drawing.Size(50, 22);
            this.panel9.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Right;
            this.label3.Location = new System.Drawing.Point(12, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Name:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.comboBox_project_status);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(331, 23);
            this.panel4.TabIndex = 0;
            // 
            // comboBox_project_status
            // 
            this.comboBox_project_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_project_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_project_status.FormattingEnabled = true;
            this.comboBox_project_status.Items.AddRange(new object[] {
            "Show all projects",
            "In progress",
            "Completed"});
            this.comboBox_project_status.Location = new System.Drawing.Point(50, 0);
            this.comboBox_project_status.Name = "comboBox_project_status";
            this.comboBox_project_status.Size = new System.Drawing.Size(281, 21);
            this.comboBox_project_status.TabIndex = 3;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.panel6.Size = new System.Drawing.Size(50, 23);
            this.panel6.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Right;
            this.label2.Location = new System.Drawing.Point(10, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Status:";
            // 
            // StudioTimeTrackerNavigationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel_middle);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.panel_top);
            this.Name = "StudioTimeTrackerNavigationControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(354, 245);
            this.panel_middle.ResumeLayout(false);
            this.contextMenuStrip_navigation_control.ResumeLayout(false);
            this.panel_bottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.panel10.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel_top.ResumeLayout(false);
            this.groupBox_filter.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel_middle;
        internal System.Windows.Forms.TreeView treeView_navigation;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Panel panel_bottom;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel_top;
        private System.Windows.Forms.GroupBox groupBox_filter;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.ComboBox comboBox_project_status;
        internal System.Windows.Forms.TextBox textBox_project_name;
        internal System.Windows.Forms.Button button_project_search;
        private System.Windows.Forms.Panel panel11;
        internal System.Windows.Forms.ComboBox comboBox_groupBy;
        private System.Windows.Forms.Panel panel10;
        internal System.Windows.Forms.Button button_auto_expand_treeview;
        internal System.Windows.Forms.ContextMenuStrip contextMenuStrip_navigation_control;
        internal System.Windows.Forms.ToolStripMenuItem newTimeTrackerProjectToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem editTimeTrackerProjectToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem removeTimeTrackerProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem newProjectActivityToolStripMenuItem;


    }
}