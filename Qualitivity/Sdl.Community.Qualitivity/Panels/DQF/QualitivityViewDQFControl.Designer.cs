using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Panels.DQF
{
    partial class QualitivityViewDqfControl
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(QualitivityViewDqfControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.viewProjectTaskInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.panel6 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_DQF_PROJECT_NAME = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.treeView_dqf_projects = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newDQFProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDQFProjectFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveDQFProjectSettingsToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDQFProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewDQFProjectInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewDQFProjectReportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel8 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.linkLabel_view_project = new System.Windows.Forms.LinkLabel();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.panel7 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_dqf_project_new = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_dqf_import_project_settings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_dqf_export_project_settings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_dqf_remove_project = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.splitter1);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(1001, 487);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(252, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(746, 481);
            this.panel2.TabIndex = 5;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView1.ContextMenuStrip = this.contextMenuStrip2;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 28);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(746, 425);
            this.listView1.SmallImageList = this.imageList2;
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Activity Name";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Task ID";
            this.columnHeader2.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Document Name";
            this.columnHeader3.Width = 200;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Source Language";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Target Language";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Uploaded";
            this.columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Segments";
            this.columnHeader6.Width = 80;
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewProjectTaskInfoToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(191, 26);
            // 
            // viewProjectTaskInfoToolStripMenuItem
            // 
            this.viewProjectTaskInfoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewProjectTaskInfoToolStripMenuItem.Image")));
            this.viewProjectTaskInfoToolStripMenuItem.Name = "viewProjectTaskInfoToolStripMenuItem";
            this.viewProjectTaskInfoToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.viewProjectTaskInfoToolStripMenuItem.Text = "View Project Task Info";
            this.viewProjectTaskInfoToolStripMenuItem.Click += new System.EventHandler(this.viewProjectTaskInfoToolStripMenuItem_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Bullet-Green-32.png");
            this.imageList2.Images.SetKeyName(1, "Bullet-Red-32.png");
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.comboBox1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 453);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(746, 28);
            this.panel6.TabIndex = 1;
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "View All"});
            this.comboBox1.Location = new System.Drawing.Point(6, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(168, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.Text = "View All";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label_DQF_PROJECT_NAME);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(746, 28);
            this.panel5.TabIndex = 0;
            // 
            // label_DQF_PROJECT_NAME
            // 
            this.label_DQF_PROJECT_NAME.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_DQF_PROJECT_NAME.Location = new System.Drawing.Point(0, 0);
            this.label_DQF_PROJECT_NAME.Name = "label_DQF_PROJECT_NAME";
            this.label_DQF_PROJECT_NAME.Size = new System.Drawing.Size(746, 28);
            this.label_DQF_PROJECT_NAME.TabIndex = 0;
            this.label_DQF_PROJECT_NAME.Text = "[DQF_PROJECT_NAME]";
            this.label_DQF_PROJECT_NAME.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(249, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 481);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.treeView_dqf_projects);
            this.panel3.Controls.Add(this.panel8);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Controls.Add(this.panel7);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.MinimumSize = new System.Drawing.Size(232, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(246, 481);
            this.panel3.TabIndex = 3;
            // 
            // treeView_dqf_projects
            // 
            this.treeView_dqf_projects.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView_dqf_projects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_dqf_projects.FullRowSelect = true;
            this.treeView_dqf_projects.HideSelection = false;
            this.treeView_dqf_projects.ImageIndex = 0;
            this.treeView_dqf_projects.ImageList = this.imageList1;
            this.treeView_dqf_projects.Indent = 5;
            this.treeView_dqf_projects.Location = new System.Drawing.Point(0, 50);
            this.treeView_dqf_projects.Name = "treeView_dqf_projects";
            this.treeView_dqf_projects.SelectedImageIndex = 0;
            this.treeView_dqf_projects.ShowLines = false;
            this.treeView_dqf_projects.ShowPlusMinus = false;
            this.treeView_dqf_projects.ShowRootLines = false;
            this.treeView_dqf_projects.Size = new System.Drawing.Size(246, 403);
            this.treeView_dqf_projects.TabIndex = 2;
            this.treeView_dqf_projects.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_dqf_projects_AfterSelect);
            this.treeView_dqf_projects.DoubleClick += new System.EventHandler(this.treeView_dqf_projects_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDQFProjectToolStripMenuItem,
            this.importDQFProjectFromFileToolStripMenuItem,
            this.saveDQFProjectSettingsToFileToolStripMenuItem,
            this.removeDQFProjectToolStripMenuItem,
            this.toolStripSeparator1,
            this.viewDQFProjectInfoToolStripMenuItem,
            this.toolStripSeparator2,
            this.viewDQFProjectReportsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(242, 170);
            // 
            // newDQFProjectToolStripMenuItem
            // 
            this.newDQFProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newDQFProjectToolStripMenuItem.Image")));
            this.newDQFProjectToolStripMenuItem.Name = "newDQFProjectToolStripMenuItem";
            this.newDQFProjectToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.newDQFProjectToolStripMenuItem.Text = "New DQF Project";
            this.newDQFProjectToolStripMenuItem.Click += new System.EventHandler(this.newDQFProjectToolStripMenuItem_Click);
            // 
            // importDQFProjectFromFileToolStripMenuItem
            // 
            this.importDQFProjectFromFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("importDQFProjectFromFileToolStripMenuItem.Image")));
            this.importDQFProjectFromFileToolStripMenuItem.Name = "importDQFProjectFromFileToolStripMenuItem";
            this.importDQFProjectFromFileToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.importDQFProjectFromFileToolStripMenuItem.Text = "Import DQF Project from file";
            this.importDQFProjectFromFileToolStripMenuItem.Click += new System.EventHandler(this.importDQFProjectFromFileToolStripMenuItem_Click);
            // 
            // saveDQFProjectSettingsToFileToolStripMenuItem
            // 
            this.saveDQFProjectSettingsToFileToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveDQFProjectSettingsToFileToolStripMenuItem.Image")));
            this.saveDQFProjectSettingsToFileToolStripMenuItem.Name = "saveDQFProjectSettingsToFileToolStripMenuItem";
            this.saveDQFProjectSettingsToFileToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.saveDQFProjectSettingsToFileToolStripMenuItem.Text = "Save DQF Project settings to file";
            this.saveDQFProjectSettingsToFileToolStripMenuItem.Click += new System.EventHandler(this.saveDQFProjectSettingsToFileToolStripMenuItem_Click);
            // 
            // removeDQFProjectToolStripMenuItem
            // 
            this.removeDQFProjectToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeDQFProjectToolStripMenuItem.Image")));
            this.removeDQFProjectToolStripMenuItem.Name = "removeDQFProjectToolStripMenuItem";
            this.removeDQFProjectToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.removeDQFProjectToolStripMenuItem.Text = "Remove DQF Project";
            this.removeDQFProjectToolStripMenuItem.Click += new System.EventHandler(this.removeDQFProjectToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(238, 6);
            // 
            // viewDQFProjectInfoToolStripMenuItem
            // 
            this.viewDQFProjectInfoToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewDQFProjectInfoToolStripMenuItem.Image")));
            this.viewDQFProjectInfoToolStripMenuItem.Name = "viewDQFProjectInfoToolStripMenuItem";
            this.viewDQFProjectInfoToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.viewDQFProjectInfoToolStripMenuItem.Text = "View DQF Project Info";
            this.viewDQFProjectInfoToolStripMenuItem.Click += new System.EventHandler(this.viewDQFProjectInfoToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(238, 6);
            // 
            // viewDQFProjectReportsToolStripMenuItem
            // 
            this.viewDQFProjectReportsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("viewDQFProjectReportsToolStripMenuItem.Image")));
            this.viewDQFProjectReportsToolStripMenuItem.Name = "viewDQFProjectReportsToolStripMenuItem";
            this.viewDQFProjectReportsToolStripMenuItem.Size = new System.Drawing.Size(241, 22);
            this.viewDQFProjectReportsToolStripMenuItem.Text = "View DQF Project Reports";
            this.viewDQFProjectReportsToolStripMenuItem.Click += new System.EventHandler(this.viewDQFProjectReportsToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Light-Blue-Circle-32.png");
            this.imageList1.Images.SetKeyName(1, "Orange-Circle-32.png");
            // 
            // panel8
            // 
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Controls.Add(this.label1);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 28);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(246, 22);
            this.panel8.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(244, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "DQF Projects";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.linkLabel_view_project);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 453);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(246, 28);
            this.panel4.TabIndex = 1;
            // 
            // linkLabel_view_project
            // 
            this.linkLabel_view_project.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_view_project.ImageIndex = 2;
            this.linkLabel_view_project.ImageList = this.imageList3;
            this.linkLabel_view_project.Location = new System.Drawing.Point(9, 3);
            this.linkLabel_view_project.Name = "linkLabel_view_project";
            this.linkLabel_view_project.Size = new System.Drawing.Size(155, 23);
            this.linkLabel_view_project.TabIndex = 0;
            this.linkLabel_view_project.TabStop = true;
            this.linkLabel_view_project.Text = "View DQF Project Reports";
            this.linkLabel_view_project.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_view_project.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_view_project_LinkClicked);
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "tausdata1.png");
            this.imageList3.Images.SetKeyName(1, "Chart-Bar-blue-32.png");
            this.imageList3.Images.SetKeyName(2, "Chart-simple-32.png");
            this.imageList3.Images.SetKeyName(3, "Chart-32.png");
            this.imageList3.Images.SetKeyName(4, "Chart-Bar-32.png");
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.toolStrip1);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(246, 28);
            this.panel7.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.Transparent;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_dqf_project_new,
            this.toolStripButton_dqf_import_project_settings,
            this.toolStripButton_dqf_export_project_settings,
            this.toolStripButton_dqf_remove_project});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(246, 29);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_dqf_project_new
            // 
            this.toolStripButton_dqf_project_new.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_dqf_project_new.Image")));
            this.toolStripButton_dqf_project_new.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_dqf_project_new.Name = "toolStripButton_dqf_project_new";
            this.toolStripButton_dqf_project_new.Size = new System.Drawing.Size(57, 26);
            this.toolStripButton_dqf_project_new.Text = "New";
            this.toolStripButton_dqf_project_new.ToolTipText = "Create a new DQF Project";
            this.toolStripButton_dqf_project_new.Click += new System.EventHandler(this.toolStripButton_dqf_project_new_Click);
            // 
            // toolStripButton_dqf_import_project_settings
            // 
            this.toolStripButton_dqf_import_project_settings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_dqf_import_project_settings.Image")));
            this.toolStripButton_dqf_import_project_settings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_dqf_import_project_settings.Name = "toolStripButton_dqf_import_project_settings";
            this.toolStripButton_dqf_import_project_settings.Size = new System.Drawing.Size(69, 26);
            this.toolStripButton_dqf_import_project_settings.Text = "Import";
            this.toolStripButton_dqf_import_project_settings.ToolTipText = "Import DQF Project settings from file";
            this.toolStripButton_dqf_import_project_settings.Click += new System.EventHandler(this.toolStripButton_dqf_import_project_settings_Click);
            // 
            // toolStripButton_dqf_export_project_settings
            // 
            this.toolStripButton_dqf_export_project_settings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_dqf_export_project_settings.Image")));
            this.toolStripButton_dqf_export_project_settings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_dqf_export_project_settings.Name = "toolStripButton_dqf_export_project_settings";
            this.toolStripButton_dqf_export_project_settings.Size = new System.Drawing.Size(57, 26);
            this.toolStripButton_dqf_export_project_settings.Text = "Save";
            this.toolStripButton_dqf_export_project_settings.ToolTipText = "Save DQF Project settings to file";
            this.toolStripButton_dqf_export_project_settings.Click += new System.EventHandler(this.toolStripButton_dqf_export_project_settings_Click);
            // 
            // toolStripButton_dqf_remove_project
            // 
            this.toolStripButton_dqf_remove_project.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_dqf_remove_project.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_dqf_remove_project.Image")));
            this.toolStripButton_dqf_remove_project.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_dqf_remove_project.Name = "toolStripButton_dqf_remove_project";
            this.toolStripButton_dqf_remove_project.Size = new System.Drawing.Size(26, 26);
            this.toolStripButton_dqf_remove_project.Text = "Remove";
            this.toolStripButton_dqf_remove_project.ToolTipText = "Remove DQF Project from the list";
            this.toolStripButton_dqf_remove_project.Click += new System.EventHandler(this.toolStripButton_dqf_remove_project_Click);
            // 
            // StudioTimeTrackerViewDQFControl
            // 
            this.Controls.Add(this.panel1);
            this.Name = "StudioTimeTrackerViewDQFControl";
            this.Size = new System.Drawing.Size(1001, 487);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel6;
        private Panel panel5;
        private Splitter splitter1;
        private Panel panel3;
        private Panel panel4;
        private Panel panel7;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton_dqf_project_new;
        private ToolStripButton toolStripButton_dqf_import_project_settings;
        private ToolStripButton toolStripButton_dqf_export_project_settings;
        private ImageList imageList1;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private Label label_DQF_PROJECT_NAME;
        private ToolStripButton toolStripButton_dqf_remove_project;
        public TreeView treeView_dqf_projects;
        private ImageList imageList2;
        private LinkLabel linkLabel_view_project;
        private ImageList imageList3;
        private ComboBox comboBox1;
        private ColumnHeader columnHeader7;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem newDQFProjectToolStripMenuItem;
        private ToolStripMenuItem importDQFProjectFromFileToolStripMenuItem;
        private ToolStripMenuItem saveDQFProjectSettingsToFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem viewDQFProjectInfoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem viewDQFProjectReportsToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem viewProjectTaskInfoToolStripMenuItem;
        private ToolStripMenuItem removeDQFProjectToolStripMenuItem;
        private Panel panel8;
        private Label label1;



    }
}