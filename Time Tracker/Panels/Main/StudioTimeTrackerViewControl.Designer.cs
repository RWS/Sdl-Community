namespace Sdl.Community.Studio.Time.Tracker.Panels.Main
{    
    partial class StudioTimeTrackerViewControl
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(StudioTimeTrackerViewControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_viewer_header = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
            this.olvColumn_client_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_project = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_name = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_description = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_activity_status = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn2 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_billable = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn_invoiced = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn5 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn6 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn7 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn8 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn3 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumn9 = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateTheProjectActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeProjectActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.createAnActivitiesReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportActivitiesToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label_TOTAL_PROJECT_ACTIVITIES = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_TOTAL_PROJECTS = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.linkLabel_unselect_all = new System.Windows.Forms.LinkLabel();
            this.linkLabel_select_all = new System.Windows.Forms.LinkLabel();
            this.linkLabel_collapse_all_groups = new System.Windows.Forms.LinkLabel();
            this.linkLabel_expand_all_groups = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_viewer_header);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3, 2, 5, 2);
            this.panel1.Size = new System.Drawing.Size(1190, 27);
            this.panel1.TabIndex = 0;
            // 
            // label_viewer_header
            // 
            this.label_viewer_header.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_viewer_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.label_viewer_header.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_viewer_header.Location = new System.Drawing.Point(3, 2);
            this.label_viewer_header.Name = "label_viewer_header";
            this.label_viewer_header.Size = new System.Drawing.Size(1182, 23);
            this.label_viewer_header.TabIndex = 0;
            this.label_viewer_header.Text = "label_viewer_header";
            this.label_viewer_header.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.objectListView1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1190, 347);
            this.panel3.TabIndex = 2;
            // 
            // objectListView1
            // 
            this.objectListView1.AllColumns.Add(this.olvColumn_client_name);
            this.objectListView1.AllColumns.Add(this.olvColumn_project);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_name);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_description);
            this.objectListView1.AllColumns.Add(this.olvColumn_activity_status);
            this.objectListView1.AllColumns.Add(this.olvColumn2);
            this.objectListView1.AllColumns.Add(this.olvColumn_billable);
            this.objectListView1.AllColumns.Add(this.olvColumn_invoiced);
            this.objectListView1.AllColumns.Add(this.olvColumn5);
            this.objectListView1.AllColumns.Add(this.olvColumn6);
            this.objectListView1.AllColumns.Add(this.olvColumn7);
            this.objectListView1.AllColumns.Add(this.olvColumn8);
            this.objectListView1.AllColumns.Add(this.olvColumn3);
            this.objectListView1.AllColumns.Add(this.olvColumn9);
            this.objectListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumn_client_name,
            this.olvColumn_project,
            this.olvColumn_activity_name,
            this.olvColumn_activity_description,
            this.olvColumn_activity_status,
            this.olvColumn2,
            this.olvColumn_billable,
            this.olvColumn_invoiced,
            this.olvColumn5,
            this.olvColumn6,
            this.olvColumn7,
            this.olvColumn8,
            this.olvColumn3,
            this.olvColumn9});
            this.objectListView1.ContextMenuStrip = this.contextMenuStrip1;
            this.objectListView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView1.FullRowSelect = true;
            this.objectListView1.HeaderUsesThemes = false;
            this.objectListView1.HeaderWordWrap = true;
            this.objectListView1.HideSelection = false;
            this.objectListView1.IncludeColumnHeadersInCopy = true;
            this.objectListView1.Location = new System.Drawing.Point(0, 0);
            this.objectListView1.Name = "objectListView1";
            this.objectListView1.OwnerDraw = true;
            this.objectListView1.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.objectListView1.ShowCommandMenuOnRightClick = true;
            this.objectListView1.ShowHeaderInAllViews = false;
            this.objectListView1.ShowItemCountOnGroups = true;
            this.objectListView1.ShowItemToolTips = true;
            this.objectListView1.Size = new System.Drawing.Size(1190, 347);
            this.objectListView1.SmallImageList = this.imageList1;
            this.objectListView1.SpaceBetweenGroups = 20;
            this.objectListView1.TabIndex = 0;
            this.objectListView1.UseAlternatingBackColors = true;
            this.objectListView1.UseCellFormatEvents = true;
            this.objectListView1.UseCompatibleStateImageBehavior = false;
            this.objectListView1.UseFilterIndicator = true;
            this.objectListView1.UseFiltering = true;
            this.objectListView1.View = System.Windows.Forms.View.Details;
            // 
            // olvColumn_client_name
            // 
            this.olvColumn_client_name.AspectName = "ClientName";
            this.olvColumn_client_name.CellPadding = null;
            this.olvColumn_client_name.Text = "Client";
            this.olvColumn_client_name.Width = 100;
            // 
            // olvColumn_project
            // 
            this.olvColumn_project.AspectName = "TrackerProjectName";
            this.olvColumn_project.CellPadding = null;
            this.olvColumn_project.Text = "Project";
            this.olvColumn_project.Width = 110;
            // 
            // olvColumn_activity_name
            // 
            this.olvColumn_activity_name.AspectName = "Name";
            this.olvColumn_activity_name.CellPadding = null;
            this.olvColumn_activity_name.HeaderImageKey = "(none)";
            this.olvColumn_activity_name.ImageAspectName = "";
            this.olvColumn_activity_name.Text = "Activity Name";
            this.olvColumn_activity_name.Width = 180;
            // 
            // olvColumn_activity_description
            // 
            this.olvColumn_activity_description.AspectName = "Description";
            this.olvColumn_activity_description.CellPadding = null;
            this.olvColumn_activity_description.Text = "Description";
            this.olvColumn_activity_description.Width = 100;
            // 
            // olvColumn_activity_status
            // 
            this.olvColumn_activity_status.AspectName = "Status";
            this.olvColumn_activity_status.CellPadding = null;
            this.olvColumn_activity_status.Text = "Status";
            this.olvColumn_activity_status.Width = 78;
            // 
            // olvColumn2
            // 
            this.olvColumn2.AspectName = "ActivityTypeName";
            this.olvColumn2.CellPadding = null;
            this.olvColumn2.Text = "Type";
            this.olvColumn2.Width = 70;
            // 
            // olvColumn_billable
            // 
            this.olvColumn_billable.AspectName = "Billable";
            this.olvColumn_billable.CellPadding = null;
            this.olvColumn_billable.Text = "Billable";
            this.olvColumn_billable.Width = 65;
            // 
            // olvColumn_invoiced
            // 
            this.olvColumn_invoiced.AspectName = "Invoiced";
            this.olvColumn_invoiced.CellPadding = null;
            this.olvColumn_invoiced.Text = "Invoiced";
            this.olvColumn_invoiced.Width = 65;
            // 
            // olvColumn5
            // 
            this.olvColumn5.AspectName = "DateStart";
            this.olvColumn5.CellPadding = null;
            this.olvColumn5.HeaderImageKey = "date_start";
            this.olvColumn5.Text = "From";
            this.olvColumn5.Width = 80;
            // 
            // olvColumn6
            // 
            this.olvColumn6.AspectName = "DateEnd";
            this.olvColumn6.CellPadding = null;
            this.olvColumn6.HeaderImageKey = "date_start";
            this.olvColumn6.Text = "To";
            this.olvColumn6.Width = 80;
            // 
            // olvColumn7
            // 
            this.olvColumn7.AspectName = "Quantity";
            this.olvColumn7.CellPadding = null;
            this.olvColumn7.Text = "Hours";
            this.olvColumn7.Width = 50;
            // 
            // olvColumn8
            // 
            this.olvColumn8.AspectName = "HourlyRate";
            this.olvColumn8.CellPadding = null;
            this.olvColumn8.Text = "Rate";
            // 
            // olvColumn3
            // 
            this.olvColumn3.AspectName = "Total";
            this.olvColumn3.CellPadding = null;
            this.olvColumn3.Text = "Total";
            // 
            // olvColumn9
            // 
            this.olvColumn9.AspectName = "Currency";
            this.olvColumn9.CellPadding = null;
            this.olvColumn9.Text = "Currency";
            this.olvColumn9.Width = 55;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectActivityToolStripMenuItem,
            this.editProjectActivityToolStripMenuItem,
            this.removeProjectActivityToolStripMenuItem,
            this.toolStripSeparator1,
            this.duplicateTheProjectActivityToolStripMenuItem,
            this.mergeProjectActivitiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.createAnActivitiesReportToolStripMenuItem,
            this.exportActivitiesToExcelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(214, 170);
            // 
            // newProjectActivityToolStripMenuItem
            // 
            this.newProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newProjectActivityToolStripMenuItem.Image")));
            this.newProjectActivityToolStripMenuItem.Name = "newProjectActivityToolStripMenuItem";
            this.newProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.newProjectActivityToolStripMenuItem.Text = "New Project Activity";
            // 
            // editProjectActivityToolStripMenuItem
            // 
            this.editProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editProjectActivityToolStripMenuItem.Image")));
            this.editProjectActivityToolStripMenuItem.Name = "editProjectActivityToolStripMenuItem";
            this.editProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.editProjectActivityToolStripMenuItem.Text = "Edit Project Activity";
            // 
            // removeProjectActivityToolStripMenuItem
            // 
            this.removeProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeProjectActivityToolStripMenuItem.Image")));
            this.removeProjectActivityToolStripMenuItem.Name = "removeProjectActivityToolStripMenuItem";
            this.removeProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.removeProjectActivityToolStripMenuItem.Text = "Remove Project Activity";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
            // 
            // duplicateTheProjectActivityToolStripMenuItem
            // 
            this.duplicateTheProjectActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("duplicateTheProjectActivityToolStripMenuItem.Image")));
            this.duplicateTheProjectActivityToolStripMenuItem.Name = "duplicateTheProjectActivityToolStripMenuItem";
            this.duplicateTheProjectActivityToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.duplicateTheProjectActivityToolStripMenuItem.Text = "Duplicate Project Activity";
            // 
            // mergeProjectActivitiesToolStripMenuItem
            // 
            this.mergeProjectActivitiesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("mergeProjectActivitiesToolStripMenuItem.Image")));
            this.mergeProjectActivitiesToolStripMenuItem.Name = "mergeProjectActivitiesToolStripMenuItem";
            this.mergeProjectActivitiesToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.mergeProjectActivitiesToolStripMenuItem.Text = "Merge Project Activities";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(210, 6);
            // 
            // createAnActivitiesReportToolStripMenuItem
            // 
            this.createAnActivitiesReportToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("createAnActivitiesReportToolStripMenuItem.Image")));
            this.createAnActivitiesReportToolStripMenuItem.Name = "createAnActivitiesReportToolStripMenuItem";
            this.createAnActivitiesReportToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.createAnActivitiesReportToolStripMenuItem.Text = "Create an Activities Report";
            // 
            // exportActivitiesToExcelToolStripMenuItem
            // 
            this.exportActivitiesToExcelToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportActivitiesToExcelToolStripMenuItem.Image")));
            this.exportActivitiesToExcelToolStripMenuItem.Name = "exportActivitiesToExcelToolStripMenuItem";
            this.exportActivitiesToExcelToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.exportActivitiesToExcelToolStripMenuItem.Text = "Export Activities to Excel";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "calendar");
            this.imageList1.Images.SetKeyName(1, "client");
            this.imageList1.Images.SetKeyName(2, "xno1");
            this.imageList1.Images.SetKeyName(3, "xno2");
            this.imageList1.Images.SetKeyName(4, "iyes");
            this.imageList1.Images.SetKeyName(5, "ino");
            this.imageList1.Images.SetKeyName(6, "vno");
            this.imageList1.Images.SetKeyName(7, "vyes");
            this.imageList1.Images.SetKeyName(8, "tick");
            this.imageList1.Images.SetKeyName(9, "flag_red");
            this.imageList1.Images.SetKeyName(10, "flag_blue");
            this.imageList1.Images.SetKeyName(11, "flag_green");
            this.imageList1.Images.SetKeyName(12, "date_start");
            this.imageList1.Images.SetKeyName(13, "date_end");
            this.imageList1.Images.SetKeyName(14, "question_blue");
            // 
            // label_TOTAL_PROJECT_ACTIVITIES
            // 
            this.label_TOTAL_PROJECT_ACTIVITIES.AutoSize = true;
            this.label_TOTAL_PROJECT_ACTIVITIES.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_TOTAL_PROJECT_ACTIVITIES.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_TOTAL_PROJECT_ACTIVITIES.Location = new System.Drawing.Point(1020, 4);
            this.label_TOTAL_PROJECT_ACTIVITIES.Margin = new System.Windows.Forms.Padding(0);
            this.label_TOTAL_PROJECT_ACTIVITIES.Name = "label_TOTAL_PROJECT_ACTIVITIES";
            this.label_TOTAL_PROJECT_ACTIVITIES.Size = new System.Drawing.Size(167, 13);
            this.label_TOTAL_PROJECT_ACTIVITIES.TabIndex = 1;
            this.label_TOTAL_PROJECT_ACTIVITIES.Text = "[TOTAL_PROJECT_ACTIVITIES]";
            this.label_TOTAL_PROJECT_ACTIVITIES.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Right;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(968, 4);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Activities:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label_TOTAL_PROJECTS
            // 
            this.label_TOTAL_PROJECTS.AutoSize = true;
            this.label_TOTAL_PROJECTS.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_TOTAL_PROJECTS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_TOTAL_PROJECTS.Location = new System.Drawing.Point(858, 4);
            this.label_TOTAL_PROJECTS.Margin = new System.Windows.Forms.Padding(0);
            this.label_TOTAL_PROJECTS.Name = "label_TOTAL_PROJECTS";
            this.label_TOTAL_PROJECTS.Size = new System.Drawing.Size(110, 13);
            this.label_TOTAL_PROJECTS.TabIndex = 5;
            this.label_TOTAL_PROJECTS.Text = "[TOTAL_PROJECTS]";
            this.label_TOTAL_PROJECTS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Right;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(773, 4);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Filtered Projects:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.linkLabel_unselect_all);
            this.panel2.Controls.Add(this.linkLabel_select_all);
            this.panel2.Controls.Add(this.linkLabel_collapse_all_groups);
            this.panel2.Controls.Add(this.linkLabel_expand_all_groups);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label_TOTAL_PROJECTS);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label_TOTAL_PROJECT_ACTIVITIES);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 374);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.panel2.Size = new System.Drawing.Size(1190, 21);
            this.panel2.TabIndex = 1;
            // 
            // linkLabel_unselect_all
            // 
            this.linkLabel_unselect_all.AutoSize = true;
            this.linkLabel_unselect_all.Location = new System.Drawing.Point(229, 3);
            this.linkLabel_unselect_all.Name = "linkLabel_unselect_all";
            this.linkLabel_unselect_all.Size = new System.Drawing.Size(63, 13);
            this.linkLabel_unselect_all.TabIndex = 9;
            this.linkLabel_unselect_all.TabStop = true;
            this.linkLabel_unselect_all.Text = "Unselect All";
            this.linkLabel_unselect_all.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_unselect_all_LinkClicked);
            // 
            // linkLabel_select_all
            // 
            this.linkLabel_select_all.AutoSize = true;
            this.linkLabel_select_all.Location = new System.Drawing.Point(174, 3);
            this.linkLabel_select_all.Name = "linkLabel_select_all";
            this.linkLabel_select_all.Size = new System.Drawing.Size(51, 13);
            this.linkLabel_select_all.TabIndex = 8;
            this.linkLabel_select_all.TabStop = true;
            this.linkLabel_select_all.Text = "Select All";
            this.linkLabel_select_all.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_select_all_LinkClicked);
            // 
            // linkLabel_collapse_all_groups
            // 
            this.linkLabel_collapse_all_groups.AutoSize = true;
            this.linkLabel_collapse_all_groups.Location = new System.Drawing.Point(68, 3);
            this.linkLabel_collapse_all_groups.Name = "linkLabel_collapse_all_groups";
            this.linkLabel_collapse_all_groups.Size = new System.Drawing.Size(61, 13);
            this.linkLabel_collapse_all_groups.TabIndex = 7;
            this.linkLabel_collapse_all_groups.TabStop = true;
            this.linkLabel_collapse_all_groups.Text = "Collapse All";
            this.linkLabel_collapse_all_groups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_collapse_all_groups_LinkClicked);
            // 
            // linkLabel_expand_all_groups
            // 
            this.linkLabel_expand_all_groups.AutoSize = true;
            this.linkLabel_expand_all_groups.Location = new System.Drawing.Point(5, 3);
            this.linkLabel_expand_all_groups.Name = "linkLabel_expand_all_groups";
            this.linkLabel_expand_all_groups.Size = new System.Drawing.Size(57, 13);
            this.linkLabel_expand_all_groups.TabIndex = 7;
            this.linkLabel_expand_all_groups.TabStop = true;
            this.linkLabel_expand_all_groups.Text = "Expand All";
            this.linkLabel_expand_all_groups.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_expand_all_groups_LinkClicked);
            // 
            // StudioTimeTrackerViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "StudioTimeTrackerViewControl";
            this.Size = new System.Drawing.Size(1190, 395);
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        internal System.Windows.Forms.Label label_viewer_header;
        private System.Windows.Forms.ImageList imageList1;
        internal BrightIdeasSoftware.ObjectListView objectListView1;
        private BrightIdeasSoftware.OLVColumn olvColumn2;
        private BrightIdeasSoftware.OLVColumn olvColumn_invoiced;
        private BrightIdeasSoftware.OLVColumn olvColumn_billable;
        private BrightIdeasSoftware.OLVColumn olvColumn5;
        private BrightIdeasSoftware.OLVColumn olvColumn6;
        private BrightIdeasSoftware.OLVColumn olvColumn7;
        private BrightIdeasSoftware.OLVColumn olvColumn8;
        private BrightIdeasSoftware.OLVColumn olvColumn9;
        private BrightIdeasSoftware.OLVColumn olvColumn_project;
        internal BrightIdeasSoftware.OLVColumn olvColumn_client_name;
        internal System.Windows.Forms.Label label_TOTAL_PROJECT_ACTIVITIES;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.Label label_TOTAL_PROJECTS;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.LinkLabel linkLabel_expand_all_groups;
        internal System.Windows.Forms.LinkLabel linkLabel_collapse_all_groups;
        internal BrightIdeasSoftware.OLVColumn olvColumn_activity_name;
        private BrightIdeasSoftware.OLVColumn olvColumn3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem newProjectActivityToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem editProjectActivityToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem removeProjectActivityToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem duplicateTheProjectActivityToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem createAnActivitiesReportToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem exportActivitiesToExcelToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem mergeProjectActivitiesToolStripMenuItem;
        internal System.Windows.Forms.LinkLabel linkLabel_select_all;
        internal System.Windows.Forms.LinkLabel linkLabel_unselect_all;
        private BrightIdeasSoftware.OLVColumn olvColumn_activity_status;
        internal BrightIdeasSoftware.OLVColumn olvColumn_activity_description;


    }
}