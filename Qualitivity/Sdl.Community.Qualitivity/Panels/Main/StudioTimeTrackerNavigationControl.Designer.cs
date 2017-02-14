using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Panels.Main
{
    partial class QualitivityNavigationControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityNavigationControl));
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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.newDQFProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.panel14 = new System.Windows.Forms.Panel();
            this.textBox_filter_name = new System.Windows.Forms.TextBox();
            this.button_project_search = new System.Windows.Forms.Button();
            this.panel9 = new System.Windows.Forms.Panel();
            this.comboBox_filter_name = new System.Windows.Forms.ComboBox();
            this.panel12 = new System.Windows.Forms.Panel();
            this.comboBox_activity_status = new System.Windows.Forms.ComboBox();
            this.panel13 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.checkBox_include_unlisted_projects = new System.Windows.Forms.CheckBox();
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
            this.panel14.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel13.SuspendLayout();
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
            this.panel_middle.Location = new System.Drawing.Point(5, 119);
            this.panel_middle.Name = "panel_middle";
            this.panel_middle.Padding = new System.Windows.Forms.Padding(3);
            this.panel_middle.Size = new System.Drawing.Size(344, 323);
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
            this.treeView_navigation.Size = new System.Drawing.Size(338, 317);
            this.treeView_navigation.TabIndex = 0;
            // 
            // contextMenuStrip_navigation_control
            // 
            this.contextMenuStrip_navigation_control.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTimeTrackerProjectToolStripMenuItem,
            this.editTimeTrackerProjectToolStripMenuItem,
            this.removeTimeTrackerProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.newProjectActivityToolStripMenuItem,
            this.toolStripSeparator2,
            this.newDQFProjectToolStripMenuItem});
            this.contextMenuStrip_navigation_control.Name = "contextMenuStrip1";
            this.contextMenuStrip_navigation_control.Size = new System.Drawing.Size(215, 126);
            // 
            // newTimeTrackerProjectToolStripMenuItem
            // 
            this.newTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newTimeTrackerProjectToolStripMenuItem.Image")));
            this.newTimeTrackerProjectToolStripMenuItem.Name = "newTimeTrackerProjectToolStripMenuItem";
            this.newTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.newTimeTrackerProjectToolStripMenuItem.Text = "New Qualitivity Project";
            // 
            // editTimeTrackerProjectToolStripMenuItem
            // 
            this.editTimeTrackerProjectToolStripMenuItem.Enabled = false;
            this.editTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editTimeTrackerProjectToolStripMenuItem.Image")));
            this.editTimeTrackerProjectToolStripMenuItem.Name = "editTimeTrackerProjectToolStripMenuItem";
            this.editTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.editTimeTrackerProjectToolStripMenuItem.Text = "Edit Qualitivity Project";
            // 
            // removeTimeTrackerProjectToolStripMenuItem
            // 
            this.removeTimeTrackerProjectToolStripMenuItem.Enabled = false;
            this.removeTimeTrackerProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeTimeTrackerProjectToolStripMenuItem.Image")));
            this.removeTimeTrackerProjectToolStripMenuItem.Name = "removeTimeTrackerProjectToolStripMenuItem";
            this.removeTimeTrackerProjectToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.removeTimeTrackerProjectToolStripMenuItem.Text = "Remove Qualitivity Project";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(211, 6);
            // 
            // newProjectActivityToolStripMenuItem
            // 
            this.newProjectActivityToolStripMenuItem.Enabled = false;
            this.newProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectActivityToolStripMenuItem.Image")));
            this.newProjectActivityToolStripMenuItem.Name = "newProjectActivityToolStripMenuItem";
            this.newProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.newProjectActivityToolStripMenuItem.Text = "New Project Activity";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(211, 6);
            // 
            // newDQFProjectToolStripMenuItem
            // 
            this.newDQFProjectToolStripMenuItem.Enabled = false;
            this.newDQFProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newDQFProjectToolStripMenuItem.Image")));
            this.newDQFProjectToolStripMenuItem.Name = "newDQFProjectToolStripMenuItem";
            this.newDQFProjectToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.newDQFProjectToolStripMenuItem.Text = "New DQF Project";
            // 
            // panel_bottom
            // 
            this.panel_bottom.Controls.Add(this.panel3);
            this.panel_bottom.Controls.Add(this.panel1);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.Location = new System.Drawing.Point(5, 442);
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
            this.panel_top.Size = new System.Drawing.Size(344, 114);
            this.panel_top.TabIndex = 0;
            // 
            // groupBox_filter
            // 
            this.groupBox_filter.Controls.Add(this.panel2);
            this.groupBox_filter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_filter.Location = new System.Drawing.Point(4, 3);
            this.groupBox_filter.Name = "groupBox_filter";
            this.groupBox_filter.Size = new System.Drawing.Size(337, 111);
            this.groupBox_filter.TabIndex = 0;
            this.groupBox_filter.TabStop = false;
            this.groupBox_filter.Text = "Filter";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel12);
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 16);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(331, 92);
            this.panel2.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.panel14);
            this.panel5.Controls.Add(this.button_project_search);
            this.panel5.Controls.Add(this.panel9);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 63);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(331, 22);
            this.panel5.TabIndex = 3;
            // 
            // panel14
            // 
            this.panel14.Controls.Add(this.textBox_filter_name);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel14.Location = new System.Drawing.Point(61, 0);
            this.panel14.Name = "panel14";
            this.panel14.Padding = new System.Windows.Forms.Padding(1, 0, 2, 0);
            this.panel14.Size = new System.Drawing.Size(244, 22);
            this.panel14.TabIndex = 3;
            // 
            // textBox_filter_name
            // 
            this.textBox_filter_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_filter_name.Location = new System.Drawing.Point(1, 0);
            this.textBox_filter_name.Name = "textBox_filter_name";
            this.textBox_filter_name.Size = new System.Drawing.Size(241, 20);
            this.textBox_filter_name.TabIndex = 4;
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
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.comboBox_filter_name);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel9.Location = new System.Drawing.Point(0, 0);
            this.panel9.Name = "panel9";
            this.panel9.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.panel9.Size = new System.Drawing.Size(61, 22);
            this.panel9.TabIndex = 1;
            // 
            // comboBox_filter_name
            // 
            this.comboBox_filter_name.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_filter_name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_filter_name.FormattingEnabled = true;
            this.comboBox_filter_name.Items.AddRange(new object[] {
            "Project",
            "Activity"});
            this.comboBox_filter_name.Location = new System.Drawing.Point(2, 0);
            this.comboBox_filter_name.Name = "comboBox_filter_name";
            this.comboBox_filter_name.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBox_filter_name.Size = new System.Drawing.Size(59, 21);
            this.comboBox_filter_name.TabIndex = 0;
            this.comboBox_filter_name.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_filter_name_KeyPress);
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.comboBox_activity_status);
            this.panel12.Controls.Add(this.panel13);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(0, 40);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(331, 23);
            this.panel12.TabIndex = 2;
            // 
            // comboBox_activity_status
            // 
            this.comboBox_activity_status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox_activity_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_activity_status.FormattingEnabled = true;
            this.comboBox_activity_status.Items.AddRange(new object[] {
            "Show all activities",
            "New",
            "Confirmed"});
            this.comboBox_activity_status.Location = new System.Drawing.Point(61, 0);
            this.comboBox_activity_status.Name = "comboBox_activity_status";
            this.comboBox_activity_status.Size = new System.Drawing.Size(270, 21);
            this.comboBox_activity_status.TabIndex = 3;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.label4);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Padding = new System.Windows.Forms.Padding(2, 4, 0, 0);
            this.panel13.Size = new System.Drawing.Size(61, 23);
            this.panel13.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Left;
            this.label4.Location = new System.Drawing.Point(2, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Activity:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.checkBox_include_unlisted_projects);
            this.panel4.Controls.Add(this.comboBox_project_status);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(331, 40);
            this.panel4.TabIndex = 0;
            // 
            // checkBox_include_unlisted_projects
            // 
            this.checkBox_include_unlisted_projects.AutoSize = true;
            this.checkBox_include_unlisted_projects.Location = new System.Drawing.Point(60, 23);
            this.checkBox_include_unlisted_projects.Name = "checkBox_include_unlisted_projects";
            this.checkBox_include_unlisted_projects.Size = new System.Drawing.Size(100, 17);
            this.checkBox_include_unlisted_projects.TabIndex = 4;
            this.checkBox_include_unlisted_projects.Text = "Include unlisted";
            this.checkBox_include_unlisted_projects.UseVisualStyleBackColor = true;
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
            this.comboBox_project_status.Location = new System.Drawing.Point(61, 0);
            this.comboBox_project_status.Name = "comboBox_project_status";
            this.comboBox_project_status.Size = new System.Drawing.Size(270, 21);
            this.comboBox_project_status.TabIndex = 3;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.label2);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(2, 4, 0, 0);
            this.panel6.Size = new System.Drawing.Size(61, 40);
            this.panel6.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(2, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Project:";
            // 
            // QualitivityNavigationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel_middle);
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.panel_top);
            this.Name = "QualitivityNavigationControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(354, 475);
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
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.panel13.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal ImageList imageList1;
        private Panel panel_middle;
        internal TreeView treeView_navigation;
        private ImageList imageList2;
        private Panel panel_bottom;
        private Panel panel1;
        private Panel panel8;
        private Panel panel7;
        private Label label1;
        private Panel panel3;
        private Panel panel_top;
        private GroupBox groupBox_filter;
        private Panel panel2;
        private Panel panel4;
        private Panel panel6;
        private Label label2;
        internal ComboBox comboBox_project_status;
        private Panel panel11;
        internal ComboBox comboBox_groupBy;
        private Panel panel10;
        internal Button button_auto_expand_treeview;
        internal ContextMenuStrip contextMenuStrip_navigation_control;
        internal ToolStripMenuItem newTimeTrackerProjectToolStripMenuItem;
        internal ToolStripMenuItem editTimeTrackerProjectToolStripMenuItem;
        internal ToolStripMenuItem removeTimeTrackerProjectToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        internal ToolStripMenuItem newProjectActivityToolStripMenuItem;
        private Panel panel5;
        private Panel panel14;
        internal TextBox textBox_filter_name;
        internal Button button_project_search;
        private Panel panel9;
        private Panel panel12;
        internal ComboBox comboBox_activity_status;
        private Panel panel13;
        private Label label4;
        internal ComboBox comboBox_filter_name;
        private ToolStripSeparator toolStripSeparator2;
        internal ToolStripMenuItem newDQFProjectToolStripMenuItem;
        public CheckBox checkBox_include_unlisted_projects;


    }
}