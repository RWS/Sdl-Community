namespace PostEdit.Compare.Forms
{
    partial class Settings
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Filters", 1, 1);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Folder Viewer", 3, 3);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Report Viewer", 4, 4);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Viewers", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Reports", 12, 12);
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Comparison Projects", 5, 5);
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Rate Groups", 6, 6);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Comparer", 7, 7);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Events Log", 8, 8);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.treeView_main = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel_ErrorHandling = new System.Windows.Forms.Panel();
            this.panel_report_viewer = new System.Windows.Forms.Panel();
            this.panel_options = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox_viewFilesWithNoTranslationDifferences = new System.Windows.Forms.CheckBox();
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows = new System.Windows.Forms.CheckBox();
            this.checkBox_showGoogleChartsInReport = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.javaWebsiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.button_javaExecutablePath = new System.Windows.Forms.Button();
            this.textBox_javaExecutablePath = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.checkBox_showSegmentComments = new System.Windows.Forms.CheckBox();
            this.checkBox_showTargetSegmentComparison = new System.Windows.Forms.CheckBox();
            this.checkBox_showUpdatedRevisionMarkerTargetSegment = new System.Windows.Forms.CheckBox();
            this.checkBox_showUpdatedTargetSegment = new System.Windows.Forms.CheckBox();
            this.checkBox_showOriginalRevisionMarkerTargetSegment = new System.Windows.Forms.CheckBox();
            this.checkBox_showOriginalTargetSegment = new System.Windows.Forms.CheckBox();
            this.checkBox_showSegmentPEM = new System.Windows.Forms.CheckBox();
            this.checkBox_showSegmentTERPAnalysis = new System.Windows.Forms.CheckBox();
            this.checkBox_showSegmentMatch = new System.Windows.Forms.CheckBox();
            this.checkBox_showSegmentStatus = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox_showLockedSegments = new System.Windows.Forms.CheckBox();
            this.checkBox_showOriginalSourceSegment = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.comboBox_segments_match_value_updated = new System.Windows.Forms.ComboBox();
            this.comboBox_segments_match_value_original = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.checkBox_viewLockedSegments = new System.Windows.Forms.CheckBox();
            this.checkBox_viewSegmentsWithComments = new System.Windows.Forms.CheckBox();
            this.checkBox_viewSegmentsWithStatusChanges = new System.Windows.Forms.CheckBox();
            this.checkBox_viewSegmentsWithTranslationChanges = new System.Windows.Forms.CheckBox();
            this.checkBox_viewSegmentsWithNoChanges = new System.Windows.Forms.CheckBox();
            this.imageList5 = new System.Windows.Forms.ImageList(this.components);
            this.panel_reports = new System.Windows.Forms.Panel();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.linkLabel_reports_viewFolder = new System.Windows.Forms.LinkLabel();
            this.checkBox_reportsAutoSave = new System.Windows.Forms.CheckBox();
            this.checkBox_reportsCreateMonthlySubFolders = new System.Windows.Forms.CheckBox();
            this.button_reportsAutoSaveFullPath = new System.Windows.Forms.Button();
            this.textBox_reportsAutoSaveFullPath = new System.Windows.Forms.TextBox();
            this.panel_filters = new System.Windows.Forms.Panel();
            this.listView_filters = new System.Windows.Forms.ListView();
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_filters = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.moveDownFilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList4 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_filters_add = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_filters_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_filers_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_filers_export = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_filters_import = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_filters_move_down = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_filters_moveUp = new System.Windows.Forms.ToolStripButton();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.panel_comparer = new System.Windows.Forms.Panel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.linkLabel_resetToDefaults_comparer = new System.Windows.Forms.LinkLabel();
            this.button_changeFormattingTagRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTagNew = new System.Windows.Forms.Button();
            this.button_changeFormattingTextRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTextNew = new System.Windows.Forms.Button();
            this.richTextBox_formatting_Tag_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Tag_New = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_New = new System.Windows.Forms.RichTextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_includeTagsInComparison = new System.Windows.Forms.CheckBox();
            this.comboBox_comparisonType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel_comparision_projects = new System.Windows.Forms.Panel();
            this.listView_comparison_projects = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_comparison_project_listview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newCompareListItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editCompareListItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeCompareListItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.fileAlignmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_comparison_project_item_new = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_comparison_project_item_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_comparison_project_item_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_comparison_project_file_alignment = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_comparison_project_item_moveDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_comparison_project_item_moveUp = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel_events_log = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBox_eventsLogTrackFilters = new System.Windows.Forms.CheckBox();
            this.checkBox_eventsLogTrackFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_eventsLogTrackProjects = new System.Windows.Forms.CheckBox();
            this.checkBox_eventsLogTrackReports = new System.Windows.Forms.CheckBox();
            this.checkBox_eventsLogTrackCompare = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.linkLabel_clear_comparison_log = new System.Windows.Forms.LinkLabel();
            this.numericUpDown_maximum_comparisonLogEntries = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureBox_clear_comparison_log = new System.Windows.Forms.PictureBox();
            this.panel_folder_viewer = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel_clear_folder_comparison_histroy = new System.Windows.Forms.LinkLabel();
            this.numericUpDown_maximum_folderComparisonEntries = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox_clear_folder_comparison_histroy = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.checkBox_folder_viewer_show_empty_folders = new System.Windows.Forms.CheckBox();
            this.checkBox_folder_viewer_show_mismatches_right = new System.Windows.Forms.CheckBox();
            this.checkBox_folder_viewer_show_orphan_files_right = new System.Windows.Forms.CheckBox();
            this.checkBox_folder_viewer_show_mismatches_left = new System.Windows.Forms.CheckBox();
            this.checkBox_folder_viewer_show_orphan_files_left = new System.Windows.Forms.CheckBox();
            this.checkBox_folderViewer_columns_modified = new System.Windows.Forms.CheckBox();
            this.checkBox_folderViewer_columns_size = new System.Windows.Forms.CheckBox();
            this.checkBox_folderViewer_columns_type = new System.Windows.Forms.CheckBox();
            this.checkBox_folderViewer_columns_name = new System.Windows.Forms.CheckBox();
            this.checkBox_folder_viewer_show_equal_files = new System.Windows.Forms.CheckBox();
            this.panel_general = new System.Windows.Forms.Panel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox_automaticallyExpandComparisonFolders = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel_price_groups = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listView_price_groups = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_price_groups_listview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.analysisBandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel4 = new System.Windows.Forms.Panel();
            this.treeView_price_groups = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_price_groups_treeview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsDefaultPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolStrip_ProjectsBlocks = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_priceGroup_new = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_priceGroup_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_priceGroup_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_groupPrice_add = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripButton_groupPrice_addMultiple = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_groupPrice_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_groupPrice_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_analysisBandPercentage = new System.Windows.Forms.ToolStripButton();
            this.panel_viewers = new System.Windows.Forms.Panel();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.linkLabel_reportViewer = new System.Windows.Forms.LinkLabel();
            this.linkLabel_fileViewer = new System.Windows.Forms.LinkLabel();
            this.linkLabel_folderViewer = new System.Windows.Forms.LinkLabel();
            this.panel_fonts = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tagVisualizationComboBox = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.panel_ErrorHandling.SuspendLayout();
            this.panel_report_viewer.SuspendLayout();
            this.panel_options.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel_reports.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.panel_filters.SuspendLayout();
            this.contextMenuStrip_filters.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.panel_comparer.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.panel_comparision_projects.SuspendLayout();
            this.contextMenuStrip_comparison_project_listview.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel_events_log.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maximum_comparisonLogEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clear_comparison_log)).BeginInit();
            this.panel_folder_viewer.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maximum_folderComparisonEntries)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clear_folder_comparison_histroy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel_general.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel_price_groups.SuspendLayout();
            this.panel3.SuspendLayout();
            this.contextMenuStrip_price_groups_listview.SuspendLayout();
            this.panel4.SuspendLayout();
            this.contextMenuStrip_price_groups_treeview.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.toolStrip_ProjectsBlocks.SuspendLayout();
            this.panel_viewers.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.panel_fonts.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView_main
            // 
            this.treeView_main.BackColor = System.Drawing.Color.White;
            this.treeView_main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView_main.FullRowSelect = true;
            this.treeView_main.HideSelection = false;
            this.treeView_main.ImageIndex = 0;
            this.treeView_main.ImageList = this.imageList1;
            this.treeView_main.ItemHeight = 42;
            this.treeView_main.Location = new System.Drawing.Point(7, 8);
            this.treeView_main.Name = "treeView_main";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node_general";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "General";
            treeNode2.ImageIndex = 1;
            treeNode2.Name = "Node_filters";
            treeNode2.SelectedImageIndex = 1;
            treeNode2.Text = "Filters";
            treeNode3.ImageIndex = 3;
            treeNode3.Name = "Node_folder_viewer";
            treeNode3.SelectedImageIndex = 3;
            treeNode3.Text = "Folder Viewer";
            treeNode4.ImageIndex = 4;
            treeNode4.Name = "Node_report_viewer";
            treeNode4.SelectedImageIndex = 4;
            treeNode4.Text = "Report Viewer";
            treeNode5.ImageIndex = 2;
            treeNode5.Name = "Node_viewers";
            treeNode5.SelectedImageIndex = 2;
            treeNode5.Text = "Viewers";
            treeNode6.ImageIndex = 12;
            treeNode6.Name = "Node_reports";
            treeNode6.SelectedImageIndex = 12;
            treeNode6.Text = "Reports";
            treeNode7.ImageIndex = 5;
            treeNode7.Name = "Node_comparision_projects";
            treeNode7.SelectedImageIndex = 5;
            treeNode7.Text = "Comparison Projects";
            treeNode8.ImageIndex = 6;
            treeNode8.Name = "Node_price_groups";
            treeNode8.SelectedImageIndex = 6;
            treeNode8.Text = "Rate Groups";
            treeNode9.ImageIndex = 7;
            treeNode9.Name = "Node_comparer";
            treeNode9.SelectedImageIndex = 7;
            treeNode9.Text = "Comparer";
            treeNode10.ImageIndex = 8;
            treeNode10.Name = "Node_events_log";
            treeNode10.SelectedImageIndex = 8;
            treeNode10.Text = "Events Log";
            this.treeView_main.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10});
            this.treeView_main.SelectedImageIndex = 0;
            this.treeView_main.Size = new System.Drawing.Size(234, 511);
            this.treeView_main.TabIndex = 20;
            this.treeView_main.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_main_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Setting-64.png");
            this.imageList1.Images.SetKeyName(1, "Filter Feather_64.png");
            this.imageList1.Images.SetKeyName(2, "Forms-64.png");
            this.imageList1.Images.SetKeyName(3, "My Documents-64.png");
            this.imageList1.Images.SetKeyName(4, "Bar Graph _64x64.png");
            this.imageList1.Images.SetKeyName(5, "Document_Folder_blue_64.png");
            this.imageList1.Images.SetKeyName(6, "Coins-64x64.png");
            this.imageList1.Images.SetKeyName(7, "Funny-Search-64.png");
            this.imageList1.Images.SetKeyName(8, "Folder-Gear_64.png");
            this.imageList1.Images.SetKeyName(9, "Document-64.png");
            this.imageList1.Images.SetKeyName(10, "Documents-48.png");
            this.imageList1.Images.SetKeyName(11, "My-Documents_64.png");
            this.imageList1.Images.SetKeyName(12, "My-Documents-Director-64.png");
            this.imageList1.Images.SetKeyName(13, "Google-Docs-colorfull-64.png");
            this.imageList1.Images.SetKeyName(14, "Document_Folder_blue_48.png");
            this.imageList1.Images.SetKeyName(15, "My Documents-48.png");
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Save.Location = new System.Drawing.Point(962, 525);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 19;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_Close
            // 
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Close.Location = new System.Drawing.Point(1039, 525);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 18;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox_header);
            this.panel2.Controls.Add(this.pictureBox_header);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(868, 56);
            this.panel2.TabIndex = 36;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(72, 16);
            this.textBox_header.Name = "textBox_header";
            this.textBox_header.ReadOnly = true;
            this.textBox_header.Size = new System.Drawing.Size(333, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "General Settings";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(19, 7);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(42, 42);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // panel_ErrorHandling
            // 
            this.panel_ErrorHandling.Controls.Add(this.panel_report_viewer);
            this.panel_ErrorHandling.Controls.Add(this.panel_reports);
            this.panel_ErrorHandling.Controls.Add(this.panel_filters);
            this.panel_ErrorHandling.Controls.Add(this.panel_comparer);
            this.panel_ErrorHandling.Controls.Add(this.panel_comparision_projects);
            this.panel_ErrorHandling.Controls.Add(this.panel_events_log);
            this.panel_ErrorHandling.Controls.Add(this.panel_folder_viewer);
            this.panel_ErrorHandling.Controls.Add(this.panel_general);
            this.panel_ErrorHandling.Controls.Add(this.panel_price_groups);
            this.panel_ErrorHandling.Controls.Add(this.panel_viewers);
            this.panel_ErrorHandling.Controls.Add(this.panel_fonts);
            this.panel_ErrorHandling.Controls.Add(this.panel2);
            this.panel_ErrorHandling.Location = new System.Drawing.Point(245, 8);
            this.panel_ErrorHandling.Name = "panel_ErrorHandling";
            this.panel_ErrorHandling.Size = new System.Drawing.Size(868, 511);
            this.panel_ErrorHandling.TabIndex = 23;
            // 
            // panel_report_viewer
            // 
            this.panel_report_viewer.Controls.Add(this.panel_options);
            this.panel_report_viewer.Location = new System.Drawing.Point(30, 74);
            this.panel_report_viewer.Name = "panel_report_viewer";
            this.panel_report_viewer.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_report_viewer.Size = new System.Drawing.Size(762, 419);
            this.panel_report_viewer.TabIndex = 37;
            // 
            // panel_options
            // 
            this.panel_options.Controls.Add(this.tabControl1);
            this.panel_options.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_options.Location = new System.Drawing.Point(0, 15);
            this.panel_options.Name = "panel_options";
            this.panel_options.Size = new System.Drawing.Size(762, 404);
            this.panel_options.TabIndex = 71;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList5;
            this.tabControl1.ItemSize = new System.Drawing.Size(74, 26);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(762, 404);
            this.tabControl1.TabIndex = 70;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(5);
            this.tabPage1.Size = new System.Drawing.Size(754, 370);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "File Summary";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.checkBox_viewFilesWithNoTranslationDifferences);
            this.groupBox5.Controls.Add(this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows);
            this.groupBox5.Controls.Add(this.checkBox_showGoogleChartsInReport);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(5, 5);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(744, 360);
            this.groupBox5.TabIndex = 66;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "File Summary";
            // 
            // label6
            // 
            this.label6.ForeColor = System.Drawing.Color.Gray;
            this.label6.Location = new System.Drawing.Point(35, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(639, 125);
            this.label6.TabIndex = 91;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // checkBox_viewFilesWithNoTranslationDifferences
            // 
            this.checkBox_viewFilesWithNoTranslationDifferences.AutoSize = true;
            this.checkBox_viewFilesWithNoTranslationDifferences.Location = new System.Drawing.Point(19, 39);
            this.checkBox_viewFilesWithNoTranslationDifferences.Name = "checkBox_viewFilesWithNoTranslationDifferences";
            this.checkBox_viewFilesWithNoTranslationDifferences.Size = new System.Drawing.Size(217, 17);
            this.checkBox_viewFilesWithNoTranslationDifferences.TabIndex = 90;
            this.checkBox_viewFilesWithNoTranslationDifferences.Text = "Show files with no translation differences";
            this.checkBox_viewFilesWithNoTranslationDifferences.UseVisualStyleBackColor = true;
            // 
            // checkBox_calculateSummaryAnalysisBasedOnFilteredRows
            // 
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Location = new System.Drawing.Point(19, 80);
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Name = "checkBox_calculateSummaryAnalysisBasedOnFilteredRows";
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Size = new System.Drawing.Size(670, 19);
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.TabIndex = 88;
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.Text = "Bind summary totals to filtered segments";
            this.checkBox_calculateSummaryAnalysisBasedOnFilteredRows.UseVisualStyleBackColor = true;
            // 
            // checkBox_showGoogleChartsInReport
            // 
            this.checkBox_showGoogleChartsInReport.AutoSize = true;
            this.checkBox_showGoogleChartsInReport.Checked = true;
            this.checkBox_showGoogleChartsInReport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showGoogleChartsInReport.Location = new System.Drawing.Point(19, 61);
            this.checkBox_showGoogleChartsInReport.Name = "checkBox_showGoogleChartsInReport";
            this.checkBox_showGoogleChartsInReport.Size = new System.Drawing.Size(233, 17);
            this.checkBox_showGoogleChartsInReport.TabIndex = 89;
            this.checkBox_showGoogleChartsInReport.Text = "Show google charts in the file summary area";
            this.checkBox_showGoogleChartsInReport.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.ImageIndex = 2;
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(5);
            this.tabPage2.Size = new System.Drawing.Size(754, 370);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Segment Columns";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.javaWebsiteLinkLabel);
            this.groupBox3.Controls.Add(this.button_javaExecutablePath);
            this.groupBox3.Controls.Add(this.textBox_javaExecutablePath);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.checkBox_showSegmentComments);
            this.groupBox3.Controls.Add(this.checkBox_showTargetSegmentComparison);
            this.groupBox3.Controls.Add(this.checkBox_showUpdatedRevisionMarkerTargetSegment);
            this.groupBox3.Controls.Add(this.checkBox_showUpdatedTargetSegment);
            this.groupBox3.Controls.Add(this.checkBox_showOriginalRevisionMarkerTargetSegment);
            this.groupBox3.Controls.Add(this.checkBox_showOriginalTargetSegment);
            this.groupBox3.Controls.Add(this.checkBox_showSegmentPEM);
            this.groupBox3.Controls.Add(this.checkBox_showSegmentTERPAnalysis);
            this.groupBox3.Controls.Add(this.checkBox_showSegmentMatch);
            this.groupBox3.Controls.Add(this.checkBox_showSegmentStatus);
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.checkBox_showLockedSegments);
            this.groupBox3.Controls.Add(this.checkBox_showOriginalSourceSegment);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(5, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(744, 360);
            this.groupBox3.TabIndex = 64;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Segment Columns";
            // 
            // javaWebsiteLinkLabel
            // 
            this.javaWebsiteLinkLabel.AutoSize = true;
            this.javaWebsiteLinkLabel.Location = new System.Drawing.Point(650, 283);
            this.javaWebsiteLinkLabel.Name = "javaWebsiteLinkLabel";
            this.javaWebsiteLinkLabel.Size = new System.Drawing.Size(69, 13);
            this.javaWebsiteLinkLabel.TabIndex = 109;
            this.javaWebsiteLinkLabel.TabStop = true;
            this.javaWebsiteLinkLabel.Text = "Java website";
            this.javaWebsiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.javaWebsiteLinkLabel_LinkClicked);
            // 
            // button_javaExecutablePath
            // 
            this.button_javaExecutablePath.Location = new System.Drawing.Point(611, 277);
            this.button_javaExecutablePath.Name = "button_javaExecutablePath";
            this.button_javaExecutablePath.Size = new System.Drawing.Size(26, 23);
            this.button_javaExecutablePath.TabIndex = 91;
            this.button_javaExecutablePath.Text = "...";
            this.button_javaExecutablePath.UseVisualStyleBackColor = true;
            this.button_javaExecutablePath.Click += new System.EventHandler(this.button_javaExecutablePath_Click);
            // 
            // textBox_javaExecutablePath
            // 
            this.textBox_javaExecutablePath.Location = new System.Drawing.Point(153, 279);
            this.textBox_javaExecutablePath.Name = "textBox_javaExecutablePath";
            this.textBox_javaExecutablePath.Size = new System.Drawing.Size(451, 20);
            this.textBox_javaExecutablePath.TabIndex = 90;
            this.textBox_javaExecutablePath.TextChanged += new System.EventHandler(this.textBox_javaExecutablePath_TextChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(38, 283);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(112, 13);
            this.label18.TabIndex = 89;
            this.label18.Text = "Java executable path:";
            // 
            // checkBox_showSegmentComments
            // 
            this.checkBox_showSegmentComments.AutoSize = true;
            this.checkBox_showSegmentComments.Location = new System.Drawing.Point(22, 327);
            this.checkBox_showSegmentComments.Name = "checkBox_showSegmentComments";
            this.checkBox_showSegmentComments.Size = new System.Drawing.Size(202, 17);
            this.checkBox_showSegmentComments.TabIndex = 93;
            this.checkBox_showSegmentComments.Text = "Show the segment comments column";
            this.checkBox_showSegmentComments.UseVisualStyleBackColor = true;
            // 
            // checkBox_showTargetSegmentComparison
            // 
            this.checkBox_showTargetSegmentComparison.AutoSize = true;
            this.checkBox_showTargetSegmentComparison.Location = new System.Drawing.Point(22, 235);
            this.checkBox_showTargetSegmentComparison.Name = "checkBox_showTargetSegmentComparison";
            this.checkBox_showTargetSegmentComparison.Size = new System.Drawing.Size(238, 17);
            this.checkBox_showTargetSegmentComparison.TabIndex = 91;
            this.checkBox_showTargetSegmentComparison.Text = "Show the target segment comparison column";
            this.checkBox_showTargetSegmentComparison.UseVisualStyleBackColor = true;
            // 
            // checkBox_showUpdatedRevisionMarkerTargetSegment
            // 
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.AutoSize = true;
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.Location = new System.Drawing.Point(22, 213);
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.Name = "checkBox_showUpdatedRevisionMarkerTargetSegment";
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.Size = new System.Drawing.Size(305, 17);
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.TabIndex = 89;
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.Text = "Show the updated target segment (Track Changes) column";
            this.checkBox_showUpdatedRevisionMarkerTargetSegment.UseVisualStyleBackColor = true;
            // 
            // checkBox_showUpdatedTargetSegment
            // 
            this.checkBox_showUpdatedTargetSegment.AutoSize = true;
            this.checkBox_showUpdatedTargetSegment.Location = new System.Drawing.Point(22, 192);
            this.checkBox_showUpdatedTargetSegment.Name = "checkBox_showUpdatedTargetSegment";
            this.checkBox_showUpdatedTargetSegment.Size = new System.Drawing.Size(223, 17);
            this.checkBox_showUpdatedTargetSegment.TabIndex = 89;
            this.checkBox_showUpdatedTargetSegment.Text = "Show the updated target segment column";
            this.checkBox_showUpdatedTargetSegment.UseVisualStyleBackColor = true;
            // 
            // checkBox_showOriginalRevisionMarkerTargetSegment
            // 
            this.checkBox_showOriginalRevisionMarkerTargetSegment.AutoSize = true;
            this.checkBox_showOriginalRevisionMarkerTargetSegment.Enabled = false;
            this.checkBox_showOriginalRevisionMarkerTargetSegment.Location = new System.Drawing.Point(22, 171);
            this.checkBox_showOriginalRevisionMarkerTargetSegment.Name = "checkBox_showOriginalRevisionMarkerTargetSegment";
            this.checkBox_showOriginalRevisionMarkerTargetSegment.Size = new System.Drawing.Size(299, 17);
            this.checkBox_showOriginalRevisionMarkerTargetSegment.TabIndex = 81;
            this.checkBox_showOriginalRevisionMarkerTargetSegment.Text = "Show the original target segment (Track Changes) column";
            this.checkBox_showOriginalRevisionMarkerTargetSegment.UseVisualStyleBackColor = true;
            // 
            // checkBox_showOriginalTargetSegment
            // 
            this.checkBox_showOriginalTargetSegment.AutoSize = true;
            this.checkBox_showOriginalTargetSegment.Location = new System.Drawing.Point(22, 149);
            this.checkBox_showOriginalTargetSegment.Name = "checkBox_showOriginalTargetSegment";
            this.checkBox_showOriginalTargetSegment.Size = new System.Drawing.Size(217, 17);
            this.checkBox_showOriginalTargetSegment.TabIndex = 81;
            this.checkBox_showOriginalTargetSegment.Text = "Show the original target segment column";
            this.checkBox_showOriginalTargetSegment.UseVisualStyleBackColor = true;
            // 
            // checkBox_showSegmentPEM
            // 
            this.checkBox_showSegmentPEM.AutoSize = true;
            this.checkBox_showSegmentPEM.Checked = true;
            this.checkBox_showSegmentPEM.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showSegmentPEM.Location = new System.Drawing.Point(22, 304);
            this.checkBox_showSegmentPEM.Name = "checkBox_showSegmentPEM";
            this.checkBox_showSegmentPEM.Size = new System.Drawing.Size(304, 17);
            this.checkBox_showSegmentPEM.TabIndex = 92;
            this.checkBox_showSegmentPEM.Text = "Show the segment PEM % (Post-Edit Modifications) column";
            this.checkBox_showSegmentPEM.UseVisualStyleBackColor = true;
            // 
            // checkBox_showSegmentTERPAnalysis
            // 
            this.checkBox_showSegmentTERPAnalysis.AutoSize = true;
            this.checkBox_showSegmentTERPAnalysis.Checked = true;
            this.checkBox_showSegmentTERPAnalysis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showSegmentTERPAnalysis.Location = new System.Drawing.Point(22, 258);
            this.checkBox_showSegmentTERPAnalysis.Name = "checkBox_showSegmentTERPAnalysis";
            this.checkBox_showSegmentTERPAnalysis.Size = new System.Drawing.Size(222, 17);
            this.checkBox_showSegmentTERPAnalysis.TabIndex = 88;
            this.checkBox_showSegmentTERPAnalysis.Text = "Show the segment TERp analysis column";
            this.checkBox_showSegmentTERPAnalysis.UseVisualStyleBackColor = true;
            // 
            // checkBox_showSegmentMatch
            // 
            this.checkBox_showSegmentMatch.AutoSize = true;
            this.checkBox_showSegmentMatch.Checked = true;
            this.checkBox_showSegmentMatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showSegmentMatch.Location = new System.Drawing.Point(22, 127);
            this.checkBox_showSegmentMatch.Name = "checkBox_showSegmentMatch";
            this.checkBox_showSegmentMatch.Size = new System.Drawing.Size(251, 17);
            this.checkBox_showSegmentMatch.TabIndex = 83;
            this.checkBox_showSegmentMatch.Text = "Show the segment match % comparison column";
            this.checkBox_showSegmentMatch.UseVisualStyleBackColor = true;
            // 
            // checkBox_showSegmentStatus
            // 
            this.checkBox_showSegmentStatus.AutoSize = true;
            this.checkBox_showSegmentStatus.Checked = true;
            this.checkBox_showSegmentStatus.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showSegmentStatus.Location = new System.Drawing.Point(22, 104);
            this.checkBox_showSegmentStatus.Name = "checkBox_showSegmentStatus";
            this.checkBox_showSegmentStatus.Size = new System.Drawing.Size(239, 17);
            this.checkBox_showSegmentStatus.TabIndex = 84;
            this.checkBox_showSegmentStatus.Text = "Show the segment status comparison column";
            this.checkBox_showSegmentStatus.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(22, 37);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(165, 17);
            this.checkBox1.TabIndex = 86;
            this.checkBox1.Text = "Show the segment ID column";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox_showLockedSegments
            // 
            this.checkBox_showLockedSegments.AutoSize = true;
            this.checkBox_showLockedSegments.Location = new System.Drawing.Point(22, 60);
            this.checkBox_showLockedSegments.Name = "checkBox_showLockedSegments";
            this.checkBox_showLockedSegments.Size = new System.Drawing.Size(186, 17);
            this.checkBox_showLockedSegments.TabIndex = 85;
            this.checkBox_showLockedSegments.Text = "Show the locked segment column";
            this.checkBox_showLockedSegments.UseVisualStyleBackColor = true;
            // 
            // checkBox_showOriginalSourceSegment
            // 
            this.checkBox_showOriginalSourceSegment.AutoSize = true;
            this.checkBox_showOriginalSourceSegment.Location = new System.Drawing.Point(22, 82);
            this.checkBox_showOriginalSourceSegment.Name = "checkBox_showOriginalSourceSegment";
            this.checkBox_showOriginalSourceSegment.Size = new System.Drawing.Size(222, 17);
            this.checkBox_showOriginalSourceSegment.TabIndex = 82;
            this.checkBox_showOriginalSourceSegment.Text = "Show the original source segment column";
            this.checkBox_showOriginalSourceSegment.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.ImageIndex = 3;
            this.tabPage3.Location = new System.Drawing.Point(4, 30);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(5);
            this.tabPage3.Size = new System.Drawing.Size(754, 370);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Segment Rows";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.tagVisualizationComboBox);
            this.groupBox7.Controls.Add(this.label16);
            this.groupBox7.Controls.Add(this.comboBox_segments_match_value_updated);
            this.groupBox7.Controls.Add(this.comboBox_segments_match_value_original);
            this.groupBox7.Controls.Add(this.label11);
            this.groupBox7.Controls.Add(this.label15);
            this.groupBox7.Controls.Add(this.checkBox_viewLockedSegments);
            this.groupBox7.Controls.Add(this.checkBox_viewSegmentsWithComments);
            this.groupBox7.Controls.Add(this.checkBox_viewSegmentsWithStatusChanges);
            this.groupBox7.Controls.Add(this.checkBox_viewSegmentsWithTranslationChanges);
            this.groupBox7.Controls.Add(this.checkBox_viewSegmentsWithNoChanges);
            this.groupBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox7.Location = new System.Drawing.Point(5, 5);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(744, 360);
            this.groupBox7.TabIndex = 64;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Segment Rows (filter)";
            // 
            // comboBox_segments_match_value_updated
            // 
            this.comboBox_segments_match_value_updated.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_segments_match_value_updated.FormattingEnabled = true;
            this.comboBox_segments_match_value_updated.Items.AddRange(new object[] {
            "{All}",
            "PM {Perfect Match}",
            "CM {Context Match}",
            "AT {Automated Translation}",
            "Exact Match",
            "Fuzzy Match",
            "No Match"});
            this.comboBox_segments_match_value_updated.Location = new System.Drawing.Point(260, 209);
            this.comboBox_segments_match_value_updated.Name = "comboBox_segments_match_value_updated";
            this.comboBox_segments_match_value_updated.Size = new System.Drawing.Size(177, 21);
            this.comboBox_segments_match_value_updated.TabIndex = 84;
            // 
            // comboBox_segments_match_value_original
            // 
            this.comboBox_segments_match_value_original.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_segments_match_value_original.FormattingEnabled = true;
            this.comboBox_segments_match_value_original.Items.AddRange(new object[] {
            "{All}",
            "PM {Perfect Match}",
            "CM {Context Match}",
            "AT {Automated Translation}",
            "Exact Match",
            "Fuzzy Match",
            "No Match"});
            this.comboBox_segments_match_value_original.Location = new System.Drawing.Point(260, 185);
            this.comboBox_segments_match_value_original.Name = "comboBox_segments_match_value_original";
            this.comboBox_segments_match_value_original.Size = new System.Drawing.Size(177, 21);
            this.comboBox_segments_match_value_original.TabIndex = 82;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 213);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(239, 13);
            this.label11.TabIndex = 83;
            this.label11.Text = "Show segments with translation origin (Updated): ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(19, 189);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(233, 13);
            this.label15.TabIndex = 81;
            this.label15.Text = "Show segments with translation origin (Original): ";
            // 
            // checkBox_viewLockedSegments
            // 
            this.checkBox_viewLockedSegments.AutoSize = true;
            this.checkBox_viewLockedSegments.Location = new System.Drawing.Point(19, 128);
            this.checkBox_viewLockedSegments.Name = "checkBox_viewLockedSegments";
            this.checkBox_viewLockedSegments.Size = new System.Drawing.Size(175, 17);
            this.checkBox_viewLockedSegments.TabIndex = 76;
            this.checkBox_viewLockedSegments.Text = "Show segments that are locked";
            this.checkBox_viewLockedSegments.UseVisualStyleBackColor = true;
            // 
            // checkBox_viewSegmentsWithComments
            // 
            this.checkBox_viewSegmentsWithComments.AutoSize = true;
            this.checkBox_viewSegmentsWithComments.Location = new System.Drawing.Point(19, 106);
            this.checkBox_viewSegmentsWithComments.Name = "checkBox_viewSegmentsWithComments";
            this.checkBox_viewSegmentsWithComments.Size = new System.Drawing.Size(174, 17);
            this.checkBox_viewSegmentsWithComments.TabIndex = 72;
            this.checkBox_viewSegmentsWithComments.Text = "Show segments with comments";
            this.checkBox_viewSegmentsWithComments.UseVisualStyleBackColor = true;
            // 
            // checkBox_viewSegmentsWithStatusChanges
            // 
            this.checkBox_viewSegmentsWithStatusChanges.AutoSize = true;
            this.checkBox_viewSegmentsWithStatusChanges.Location = new System.Drawing.Point(19, 84);
            this.checkBox_viewSegmentsWithStatusChanges.Name = "checkBox_viewSegmentsWithStatusChanges";
            this.checkBox_viewSegmentsWithStatusChanges.Size = new System.Drawing.Size(218, 17);
            this.checkBox_viewSegmentsWithStatusChanges.TabIndex = 73;
            this.checkBox_viewSegmentsWithStatusChanges.Text = "Show segments with status modifications";
            this.checkBox_viewSegmentsWithStatusChanges.UseVisualStyleBackColor = true;
            // 
            // checkBox_viewSegmentsWithTranslationChanges
            // 
            this.checkBox_viewSegmentsWithTranslationChanges.AutoSize = true;
            this.checkBox_viewSegmentsWithTranslationChanges.Location = new System.Drawing.Point(19, 63);
            this.checkBox_viewSegmentsWithTranslationChanges.Name = "checkBox_viewSegmentsWithTranslationChanges";
            this.checkBox_viewSegmentsWithTranslationChanges.Size = new System.Drawing.Size(238, 17);
            this.checkBox_viewSegmentsWithTranslationChanges.TabIndex = 74;
            this.checkBox_viewSegmentsWithTranslationChanges.Text = "Show segments with translation modifications";
            this.checkBox_viewSegmentsWithTranslationChanges.UseVisualStyleBackColor = true;
            // 
            // checkBox_viewSegmentsWithNoChanges
            // 
            this.checkBox_viewSegmentsWithNoChanges.AutoSize = true;
            this.checkBox_viewSegmentsWithNoChanges.Location = new System.Drawing.Point(19, 41);
            this.checkBox_viewSegmentsWithNoChanges.Name = "checkBox_viewSegmentsWithNoChanges";
            this.checkBox_viewSegmentsWithNoChanges.Size = new System.Drawing.Size(253, 17);
            this.checkBox_viewSegmentsWithNoChanges.TabIndex = 75;
            this.checkBox_viewSegmentsWithNoChanges.Text = "Show segments with no translation modifications";
            this.checkBox_viewSegmentsWithNoChanges.UseVisualStyleBackColor = true;
            // 
            // imageList5
            // 
            this.imageList5.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList5.ImageStream")));
            this.imageList5.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList5.Images.SetKeyName(0, "Accordion-32.png");
            this.imageList5.Images.SetKeyName(1, "Horizontal-List-32(1).png");
            this.imageList5.Images.SetKeyName(2, "Table-Select-Column-32.png");
            this.imageList5.Images.SetKeyName(3, "Table-Select-Row-32.png");
            // 
            // panel_reports
            // 
            this.panel_reports.Controls.Add(this.groupBox10);
            this.panel_reports.Location = new System.Drawing.Point(3, 316);
            this.panel_reports.Name = "panel_reports";
            this.panel_reports.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_reports.Size = new System.Drawing.Size(129, 128);
            this.panel_reports.TabIndex = 40;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.linkLabel_reports_viewFolder);
            this.groupBox10.Controls.Add(this.checkBox_reportsAutoSave);
            this.groupBox10.Controls.Add(this.checkBox_reportsCreateMonthlySubFolders);
            this.groupBox10.Controls.Add(this.button_reportsAutoSaveFullPath);
            this.groupBox10.Controls.Add(this.textBox_reportsAutoSaveFullPath);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Location = new System.Drawing.Point(0, 15);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(129, 113);
            this.groupBox10.TabIndex = 1;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Reports Properties";
            // 
            // linkLabel_reports_viewFolder
            // 
            this.linkLabel_reports_viewFolder.AutoSize = true;
            this.linkLabel_reports_viewFolder.Location = new System.Drawing.Point(761, 64);
            this.linkLabel_reports_viewFolder.Name = "linkLabel_reports_viewFolder";
            this.linkLabel_reports_viewFolder.Size = new System.Drawing.Size(62, 13);
            this.linkLabel_reports_viewFolder.TabIndex = 4;
            this.linkLabel_reports_viewFolder.TabStop = true;
            this.linkLabel_reports_viewFolder.Text = "View Folder";
            this.linkLabel_reports_viewFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_reports_viewFolder_LinkClicked);
            // 
            // checkBox_reportsAutoSave
            // 
            this.checkBox_reportsAutoSave.AutoSize = true;
            this.checkBox_reportsAutoSave.Location = new System.Drawing.Point(27, 63);
            this.checkBox_reportsAutoSave.Name = "checkBox_reportsAutoSave";
            this.checkBox_reportsAutoSave.Size = new System.Drawing.Size(176, 17);
            this.checkBox_reportsAutoSave.TabIndex = 3;
            this.checkBox_reportsAutoSave.Text = "Save a copy of reports in folder:";
            this.checkBox_reportsAutoSave.UseVisualStyleBackColor = true;
            this.checkBox_reportsAutoSave.CheckedChanged += new System.EventHandler(this.checkBox_reportsAutoSave_CheckedChanged);
            // 
            // checkBox_reportsCreateMonthlySubFolders
            // 
            this.checkBox_reportsCreateMonthlySubFolders.AutoSize = true;
            this.checkBox_reportsCreateMonthlySubFolders.Location = new System.Drawing.Point(27, 95);
            this.checkBox_reportsCreateMonthlySubFolders.Name = "checkBox_reportsCreateMonthlySubFolders";
            this.checkBox_reportsCreateMonthlySubFolders.Size = new System.Drawing.Size(139, 17);
            this.checkBox_reportsCreateMonthlySubFolders.TabIndex = 3;
            this.checkBox_reportsCreateMonthlySubFolders.Text = "Create montly sub-folder";
            this.checkBox_reportsCreateMonthlySubFolders.UseVisualStyleBackColor = true;
            // 
            // button_reportsAutoSaveFullPath
            // 
            this.button_reportsAutoSaveFullPath.Location = new System.Drawing.Point(720, 58);
            this.button_reportsAutoSaveFullPath.Name = "button_reportsAutoSaveFullPath";
            this.button_reportsAutoSaveFullPath.Size = new System.Drawing.Size(26, 23);
            this.button_reportsAutoSaveFullPath.TabIndex = 2;
            this.button_reportsAutoSaveFullPath.Text = "...";
            this.button_reportsAutoSaveFullPath.UseVisualStyleBackColor = true;
            this.button_reportsAutoSaveFullPath.Click += new System.EventHandler(this.button_reportsAutoSaveFullPath_Click);
            // 
            // textBox_reportsAutoSaveFullPath
            // 
            this.textBox_reportsAutoSaveFullPath.Location = new System.Drawing.Point(206, 61);
            this.textBox_reportsAutoSaveFullPath.Name = "textBox_reportsAutoSaveFullPath";
            this.textBox_reportsAutoSaveFullPath.Size = new System.Drawing.Size(507, 20);
            this.textBox_reportsAutoSaveFullPath.TabIndex = 1;
            // 
            // panel_filters
            // 
            this.panel_filters.Controls.Add(this.listView_filters);
            this.panel_filters.Controls.Add(this.toolStrip2);
            this.panel_filters.Controls.Add(this.statusStrip2);
            this.panel_filters.Location = new System.Drawing.Point(684, 222);
            this.panel_filters.Name = "panel_filters";
            this.panel_filters.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_filters.Size = new System.Drawing.Size(159, 113);
            this.panel_filters.TabIndex = 37;
            // 
            // listView_filters
            // 
            this.listView_filters.AllowDrop = true;
            this.listView_filters.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader21,
            this.columnHeader22});
            this.listView_filters.ContextMenuStrip = this.contextMenuStrip_filters;
            this.listView_filters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_filters.FullRowSelect = true;
            this.listView_filters.GridLines = true;
            this.listView_filters.HideSelection = false;
            this.listView_filters.Location = new System.Drawing.Point(0, 44);
            this.listView_filters.Name = "listView_filters";
            this.listView_filters.ShowItemToolTips = true;
            this.listView_filters.Size = new System.Drawing.Size(159, 69);
            this.listView_filters.SmallImageList = this.imageList4;
            this.listView_filters.TabIndex = 10;
            this.listView_filters.UseCompatibleStateImageBehavior = false;
            this.listView_filters.View = System.Windows.Forms.View.Details;
            this.listView_filters.SelectedIndexChanged += new System.EventHandler(this.listView_filters_SelectedIndexChanged);
            this.listView_filters.DoubleClick += new System.EventHandler(this.listView_filters_DoubleClick);
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Name";
            this.columnHeader18.Width = 160;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "File Name/Extension";
            this.columnHeader19.Width = 210;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "File Modified Date";
            this.columnHeader21.Width = 150;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "File Attributes";
            this.columnHeader22.Width = 220;
            // 
            // contextMenuStrip_filters
            // 
            this.contextMenuStrip_filters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addFilterToolStripMenuItem,
            this.editFilterToolStripMenuItem,
            this.removeFilterToolStripMenuItem,
            this.toolStripSeparator6,
            this.moveDownFilerToolStripMenuItem,
            this.moveUpFilterToolStripMenuItem});
            this.contextMenuStrip_filters.Name = "contextMenuStrip_filters";
            this.contextMenuStrip_filters.Size = new System.Drawing.Size(145, 120);
            // 
            // addFilterToolStripMenuItem
            // 
            this.addFilterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addFilterToolStripMenuItem.Image")));
            this.addFilterToolStripMenuItem.Name = "addFilterToolStripMenuItem";
            this.addFilterToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.addFilterToolStripMenuItem.Text = "Add filter";
            this.addFilterToolStripMenuItem.Click += new System.EventHandler(this.addFilterToolStripMenuItem_Click);
            // 
            // editFilterToolStripMenuItem
            // 
            this.editFilterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editFilterToolStripMenuItem.Image")));
            this.editFilterToolStripMenuItem.Name = "editFilterToolStripMenuItem";
            this.editFilterToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.editFilterToolStripMenuItem.Text = "Edit filter";
            this.editFilterToolStripMenuItem.Click += new System.EventHandler(this.editFilterToolStripMenuItem_Click);
            // 
            // removeFilterToolStripMenuItem
            // 
            this.removeFilterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeFilterToolStripMenuItem.Image")));
            this.removeFilterToolStripMenuItem.Name = "removeFilterToolStripMenuItem";
            this.removeFilterToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.removeFilterToolStripMenuItem.Text = "Remove filter";
            this.removeFilterToolStripMenuItem.Click += new System.EventHandler(this.removeFilterToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(141, 6);
            // 
            // moveDownFilerToolStripMenuItem
            // 
            this.moveDownFilerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveDownFilerToolStripMenuItem.Image")));
            this.moveDownFilerToolStripMenuItem.Name = "moveDownFilerToolStripMenuItem";
            this.moveDownFilerToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.moveDownFilerToolStripMenuItem.Text = "Move down";
            this.moveDownFilerToolStripMenuItem.Click += new System.EventHandler(this.moveDownFilerToolStripMenuItem_Click);
            // 
            // moveUpFilterToolStripMenuItem
            // 
            this.moveUpFilterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveUpFilterToolStripMenuItem.Image")));
            this.moveUpFilterToolStripMenuItem.Name = "moveUpFilterToolStripMenuItem";
            this.moveUpFilterToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.moveUpFilterToolStripMenuItem.Text = "Move up";
            this.moveUpFilterToolStripMenuItem.Click += new System.EventHandler(this.moveUpFilterToolStripMenuItem_Click);
            // 
            // imageList4
            // 
            this.imageList4.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList4.ImageStream")));
            this.imageList4.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList4.Images.SetKeyName(0, "Filter Feather_32(1).png");
            // 
            // toolStrip2
            // 
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_filters_add,
            this.toolStripButton_filters_edit,
            this.toolStripButton_filers_remove,
            this.toolStripSeparator5,
            this.toolStripButton_filers_export,
            this.toolStripButton_filters_import,
            this.toolStripSeparator7,
            this.toolStripButton_filters_move_down,
            this.toolStripButton_filters_moveUp});
            this.toolStrip2.Location = new System.Drawing.Point(0, 15);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip2.Size = new System.Drawing.Size(159, 29);
            this.toolStrip2.TabIndex = 11;
            // 
            // toolStripButton_filters_add
            // 
            this.toolStripButton_filters_add.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filters_add.Image")));
            this.toolStripButton_filters_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filters_add.Name = "toolStripButton_filters_add";
            this.toolStripButton_filters_add.Size = new System.Drawing.Size(55, 26);
            this.toolStripButton_filters_add.Text = "Add";
            this.toolStripButton_filters_add.ToolTipText = "New filter";
            this.toolStripButton_filters_add.Click += new System.EventHandler(this.toolStripButton_filters_add_Click);
            // 
            // toolStripButton_filters_edit
            // 
            this.toolStripButton_filters_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filters_edit.Image")));
            this.toolStripButton_filters_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filters_edit.Name = "toolStripButton_filters_edit";
            this.toolStripButton_filters_edit.Size = new System.Drawing.Size(53, 26);
            this.toolStripButton_filters_edit.Text = "Edit";
            this.toolStripButton_filters_edit.ToolTipText = "Edit filter";
            this.toolStripButton_filters_edit.Click += new System.EventHandler(this.toolStripButton_filters_edit_Click);
            // 
            // toolStripButton_filers_remove
            // 
            this.toolStripButton_filers_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filers_remove.Image")));
            this.toolStripButton_filers_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filers_remove.Name = "toolStripButton_filers_remove";
            this.toolStripButton_filers_remove.Size = new System.Drawing.Size(76, 26);
            this.toolStripButton_filers_remove.Text = "Remove";
            this.toolStripButton_filers_remove.ToolTipText = "Remove filter";
            this.toolStripButton_filers_remove.Click += new System.EventHandler(this.toolStripButton_filers_remove_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripButton_filers_export
            // 
            this.toolStripButton_filers_export.Enabled = false;
            this.toolStripButton_filers_export.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filers_export.Image")));
            this.toolStripButton_filers_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filers_export.Name = "toolStripButton_filers_export";
            this.toolStripButton_filers_export.Size = new System.Drawing.Size(66, 26);
            this.toolStripButton_filers_export.Text = "Export";
            this.toolStripButton_filers_export.ToolTipText = "Export filter setttings";
            this.toolStripButton_filers_export.Click += new System.EventHandler(this.toolStripButton_filers_export_Click);
            // 
            // toolStripButton_filters_import
            // 
            this.toolStripButton_filters_import.Enabled = false;
            this.toolStripButton_filters_import.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filters_import.Image")));
            this.toolStripButton_filters_import.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filters_import.Name = "toolStripButton_filters_import";
            this.toolStripButton_filters_import.Size = new System.Drawing.Size(69, 26);
            this.toolStripButton_filters_import.Text = "Import";
            this.toolStripButton_filters_import.ToolTipText = "Import filter settings";
            this.toolStripButton_filters_import.Click += new System.EventHandler(this.toolStripButton_filters_import_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripButton_filters_move_down
            // 
            this.toolStripButton_filters_move_down.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filters_move_down.Image")));
            this.toolStripButton_filters_move_down.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filters_move_down.Name = "toolStripButton_filters_move_down";
            this.toolStripButton_filters_move_down.Size = new System.Drawing.Size(97, 26);
            this.toolStripButton_filters_move_down.Text = "Move Down";
            this.toolStripButton_filters_move_down.Click += new System.EventHandler(this.toolStripButton_filters_move_down_Click);
            // 
            // toolStripButton_filters_moveUp
            // 
            this.toolStripButton_filters_moveUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_filters_moveUp.Image")));
            this.toolStripButton_filters_moveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_filters_moveUp.Name = "toolStripButton_filters_moveUp";
            this.toolStripButton_filters_moveUp.Size = new System.Drawing.Size(81, 26);
            this.toolStripButton_filters_moveUp.Text = "Move Up";
            this.toolStripButton_filters_moveUp.Click += new System.EventHandler(this.toolStripButton_filters_moveUp_Click);
            // 
            // statusStrip2
            // 
            this.statusStrip2.Location = new System.Drawing.Point(0, 126);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(384, 22);
            this.statusStrip2.SizingGrip = false;
            this.statusStrip2.TabIndex = 9;
            this.statusStrip2.Text = "statusStrip2";
            this.statusStrip2.Visible = false;
            // 
            // panel_comparer
            // 
            this.panel_comparer.Controls.Add(this.groupBox8);
            this.panel_comparer.Location = new System.Drawing.Point(684, 121);
            this.panel_comparer.Name = "panel_comparer";
            this.panel_comparer.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_comparer.Size = new System.Drawing.Size(123, 66);
            this.panel_comparer.TabIndex = 38;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.linkLabel_resetToDefaults_comparer);
            this.groupBox8.Controls.Add(this.button_changeFormattingTagRemoved);
            this.groupBox8.Controls.Add(this.button_changeFormattingTagNew);
            this.groupBox8.Controls.Add(this.button_changeFormattingTextRemoved);
            this.groupBox8.Controls.Add(this.button_changeFormattingTextNew);
            this.groupBox8.Controls.Add(this.richTextBox_formatting_Tag_Removed);
            this.groupBox8.Controls.Add(this.richTextBox_formatting_Tag_New);
            this.groupBox8.Controls.Add(this.richTextBox_formatting_Text_Removed);
            this.groupBox8.Controls.Add(this.richTextBox_formatting_Text_New);
            this.groupBox8.Controls.Add(this.panel6);
            this.groupBox8.Controls.Add(this.label4);
            this.groupBox8.Controls.Add(this.checkBox_includeTagsInComparison);
            this.groupBox8.Controls.Add(this.comboBox_comparisonType);
            this.groupBox8.Controls.Add(this.label2);
            this.groupBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox8.Location = new System.Drawing.Point(0, 15);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(123, 51);
            this.groupBox8.TabIndex = 1;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Comparer Settings";
            // 
            // linkLabel_resetToDefaults_comparer
            // 
            this.linkLabel_resetToDefaults_comparer.AutoSize = true;
            this.linkLabel_resetToDefaults_comparer.Location = new System.Drawing.Point(47, 392);
            this.linkLabel_resetToDefaults_comparer.Name = "linkLabel_resetToDefaults_comparer";
            this.linkLabel_resetToDefaults_comparer.Size = new System.Drawing.Size(89, 13);
            this.linkLabel_resetToDefaults_comparer.TabIndex = 105;
            this.linkLabel_resetToDefaults_comparer.TabStop = true;
            this.linkLabel_resetToDefaults_comparer.Text = "Reset to Defaults";
            this.linkLabel_resetToDefaults_comparer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_resetToDefaults_comparer_LinkClicked);
            // 
            // button_changeFormattingTagRemoved
            // 
            this.button_changeFormattingTagRemoved.Location = new System.Drawing.Point(414, 342);
            this.button_changeFormattingTagRemoved.Name = "button_changeFormattingTagRemoved";
            this.button_changeFormattingTagRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagRemoved.TabIndex = 102;
            this.button_changeFormattingTagRemoved.Text = "Change...";
            this.button_changeFormattingTagRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagRemoved.Click += new System.EventHandler(this.button_changeFormattingTagRemoved_Click);
            // 
            // button_changeFormattingTagNew
            // 
            this.button_changeFormattingTagNew.Location = new System.Drawing.Point(414, 304);
            this.button_changeFormattingTagNew.Name = "button_changeFormattingTagNew";
            this.button_changeFormattingTagNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagNew.TabIndex = 101;
            this.button_changeFormattingTagNew.Text = "Change...";
            this.button_changeFormattingTagNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagNew.Click += new System.EventHandler(this.button_changeFormattingTagNew_Click);
            // 
            // button_changeFormattingTextRemoved
            // 
            this.button_changeFormattingTextRemoved.Location = new System.Drawing.Point(414, 266);
            this.button_changeFormattingTextRemoved.Name = "button_changeFormattingTextRemoved";
            this.button_changeFormattingTextRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextRemoved.TabIndex = 104;
            this.button_changeFormattingTextRemoved.Text = "Change...";
            this.button_changeFormattingTextRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextRemoved.Click += new System.EventHandler(this.button_changeFormattingTextRemoved_Click);
            // 
            // button_changeFormattingTextNew
            // 
            this.button_changeFormattingTextNew.Location = new System.Drawing.Point(414, 228);
            this.button_changeFormattingTextNew.Name = "button_changeFormattingTextNew";
            this.button_changeFormattingTextNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextNew.TabIndex = 103;
            this.button_changeFormattingTextNew.Text = "Change...";
            this.button_changeFormattingTextNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextNew.Click += new System.EventHandler(this.button_changeFormattingTextNew_Click);
            // 
            // richTextBox_formatting_Tag_Removed
            // 
            this.richTextBox_formatting_Tag_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_Removed.Location = new System.Drawing.Point(47, 342);
            this.richTextBox_formatting_Tag_Removed.Name = "richTextBox_formatting_Tag_Removed";
            this.richTextBox_formatting_Tag_Removed.ReadOnly = true;
            this.richTextBox_formatting_Tag_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_Removed.TabIndex = 100;
            this.richTextBox_formatting_Tag_Removed.Text = "";
            // 
            // richTextBox_formatting_Tag_New
            // 
            this.richTextBox_formatting_Tag_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_New.Location = new System.Drawing.Point(47, 304);
            this.richTextBox_formatting_Tag_New.Name = "richTextBox_formatting_Tag_New";
            this.richTextBox_formatting_Tag_New.ReadOnly = true;
            this.richTextBox_formatting_Tag_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_New.TabIndex = 97;
            this.richTextBox_formatting_Tag_New.Text = "";
            // 
            // richTextBox_formatting_Text_Removed
            // 
            this.richTextBox_formatting_Text_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_Removed.Location = new System.Drawing.Point(47, 266);
            this.richTextBox_formatting_Text_Removed.Name = "richTextBox_formatting_Text_Removed";
            this.richTextBox_formatting_Text_Removed.ReadOnly = true;
            this.richTextBox_formatting_Text_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_Removed.TabIndex = 98;
            this.richTextBox_formatting_Text_Removed.Text = "";
            // 
            // richTextBox_formatting_Text_New
            // 
            this.richTextBox_formatting_Text_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_New.Location = new System.Drawing.Point(47, 228);
            this.richTextBox_formatting_Text_New.Name = "richTextBox_formatting_Text_New";
            this.richTextBox_formatting_Text_New.ReadOnly = true;
            this.richTextBox_formatting_Text_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_New.TabIndex = 99;
            this.richTextBox_formatting_Text_New.Text = "";
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel6.Location = new System.Drawing.Point(162, 195);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(325, 4);
            this.panel6.TabIndex = 96;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label4.Location = new System.Drawing.Point(26, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 95;
            this.label4.Text = "Differences formatting";
            // 
            // checkBox_includeTagsInComparison
            // 
            this.checkBox_includeTagsInComparison.AutoSize = true;
            this.checkBox_includeTagsInComparison.Location = new System.Drawing.Point(47, 74);
            this.checkBox_includeTagsInComparison.Name = "checkBox_includeTagsInComparison";
            this.checkBox_includeTagsInComparison.Size = new System.Drawing.Size(241, 17);
            this.checkBox_includeTagsInComparison.TabIndex = 5;
            this.checkBox_includeTagsInComparison.Text = "Include <tag> formatting in comparison results";
            this.checkBox_includeTagsInComparison.UseVisualStyleBackColor = true;
            this.checkBox_includeTagsInComparison.CheckedChanged += new System.EventHandler(this.checkBox_includeTagsInComparison_CheckedChanged);
            // 
            // comboBox_comparisonType
            // 
            this.comboBox_comparisonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_comparisonType.Enabled = false;
            this.comboBox_comparisonType.FormattingEnabled = true;
            this.comboBox_comparisonType.Items.AddRange(new object[] {
            "words",
            "characters"});
            this.comboBox_comparisonType.Location = new System.Drawing.Point(138, 35);
            this.comboBox_comparisonType.Name = "comboBox_comparisonType";
            this.comboBox_comparisonType.Size = new System.Drawing.Size(121, 21);
            this.comboBox_comparisonType.TabIndex = 4;
            this.comboBox_comparisonType.SelectedIndexChanged += new System.EventHandler(this.comboBox_comparisonType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Comparison type:";
            // 
            // panel_comparision_projects
            // 
            this.panel_comparision_projects.Controls.Add(this.listView_comparison_projects);
            this.panel_comparision_projects.Controls.Add(this.toolStrip1);
            this.panel_comparision_projects.Controls.Add(this.statusStrip1);
            this.panel_comparision_projects.Location = new System.Drawing.Point(6, 56);
            this.panel_comparision_projects.Name = "panel_comparision_projects";
            this.panel_comparision_projects.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_comparision_projects.Size = new System.Drawing.Size(200, 93);
            this.panel_comparision_projects.TabIndex = 37;
            // 
            // listView_comparison_projects
            // 
            this.listView_comparison_projects.AllowDrop = true;
            this.listView_comparison_projects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17});
            this.listView_comparison_projects.ContextMenuStrip = this.contextMenuStrip_comparison_project_listview;
            this.listView_comparison_projects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_comparison_projects.FullRowSelect = true;
            this.listView_comparison_projects.GridLines = true;
            this.listView_comparison_projects.HideSelection = false;
            this.listView_comparison_projects.Location = new System.Drawing.Point(0, 46);
            this.listView_comparison_projects.Name = "listView_comparison_projects";
            this.listView_comparison_projects.ShowItemToolTips = true;
            this.listView_comparison_projects.Size = new System.Drawing.Size(200, 47);
            this.listView_comparison_projects.SmallImageList = this.imageList3;
            this.listView_comparison_projects.TabIndex = 11;
            this.listView_comparison_projects.UseCompatibleStateImageBehavior = false;
            this.listView_comparison_projects.View = System.Windows.Forms.View.Details;
            this.listView_comparison_projects.SelectedIndexChanged += new System.EventHandler(this.listView_comparison_projects_SelectedIndexChanged);
            this.listView_comparison_projects.DoubleClick += new System.EventHandler(this.listView_comparison_project_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Name";
            this.columnHeader3.Width = 160;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Created";
            this.columnHeader15.Width = 120;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Path (left)";
            this.columnHeader16.Width = 280;
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Path (right)";
            this.columnHeader17.Width = 280;
            // 
            // contextMenuStrip_comparison_project_listview
            // 
            this.contextMenuStrip_comparison_project_listview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCompareListItemToolStripMenuItem,
            this.editCompareListItemToolStripMenuItem,
            this.removeCompareListItemToolStripMenuItem,
            this.toolStripSeparator9,
            this.fileAlignmentToolStripMenuItem,
            this.toolStripSeparator10,
            this.moveDownToolStripMenuItem,
            this.moveUpToolStripMenuItem});
            this.contextMenuStrip_comparison_project_listview.Name = "contextMenuStrip_comparison_project_listview";
            this.contextMenuStrip_comparison_project_listview.Size = new System.Drawing.Size(224, 148);
            // 
            // newCompareListItemToolStripMenuItem
            // 
            this.newCompareListItemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newCompareListItemToolStripMenuItem.Image")));
            this.newCompareListItemToolStripMenuItem.Name = "newCompareListItemToolStripMenuItem";
            this.newCompareListItemToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.newCompareListItemToolStripMenuItem.Text = "New comparison project";
            this.newCompareListItemToolStripMenuItem.Click += new System.EventHandler(this.newCompareListItemToolStripMenuItem_Click);
            // 
            // editCompareListItemToolStripMenuItem
            // 
            this.editCompareListItemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editCompareListItemToolStripMenuItem.Image")));
            this.editCompareListItemToolStripMenuItem.Name = "editCompareListItemToolStripMenuItem";
            this.editCompareListItemToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.editCompareListItemToolStripMenuItem.Text = "Edit comparison project";
            this.editCompareListItemToolStripMenuItem.Click += new System.EventHandler(this.editCompareListItemToolStripMenuItem_Click);
            // 
            // removeCompareListItemToolStripMenuItem
            // 
            this.removeCompareListItemToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeCompareListItemToolStripMenuItem.Image")));
            this.removeCompareListItemToolStripMenuItem.Name = "removeCompareListItemToolStripMenuItem";
            this.removeCompareListItemToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.removeCompareListItemToolStripMenuItem.Text = "Remove comparison project";
            this.removeCompareListItemToolStripMenuItem.Click += new System.EventHandler(this.removeCompareListItemToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(220, 6);
            // 
            // fileAlignmentToolStripMenuItem
            // 
            this.fileAlignmentToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fileAlignmentToolStripMenuItem.Image")));
            this.fileAlignmentToolStripMenuItem.Name = "fileAlignmentToolStripMenuItem";
            this.fileAlignmentToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.fileAlignmentToolStripMenuItem.Text = "View file name alignment";
            this.fileAlignmentToolStripMenuItem.Click += new System.EventHandler(this.fileAlignmentToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(220, 6);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveDownToolStripMenuItem.Image")));
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.moveDownToolStripMenuItem.Text = "Move down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("moveUpToolStripMenuItem.Image")));
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.moveUpToolStripMenuItem.Text = "Move up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "Coin-Single-Gold");
            this.imageList3.Images.SetKeyName(1, "Exclamation");
            this.imageList3.Images.SetKeyName(2, "Exclamation-Circle-blue");
            this.imageList3.Images.SetKeyName(3, "Exclamation-Circle-green");
            this.imageList3.Images.SetKeyName(4, "Exclamation-Circle-red");
            this.imageList3.Images.SetKeyName(5, "Exclamation-Circle-yellow");
            this.imageList3.Images.SetKeyName(6, "No");
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_comparison_project_item_new,
            this.toolStripButton_comparison_project_item_edit,
            this.toolStripButton_comparison_project_item_remove,
            this.toolStripSeparator4,
            this.toolStripButton_comparison_project_file_alignment,
            this.toolStripSeparator8,
            this.toolStripButton_comparison_project_item_moveDown,
            this.toolStripButton_comparison_project_item_moveUp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 15);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(200, 31);
            this.toolStrip1.TabIndex = 10;
            // 
            // toolStripButton_comparison_project_item_new
            // 
            this.toolStripButton_comparison_project_item_new.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_item_new.Image")));
            this.toolStripButton_comparison_project_item_new.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_item_new.Name = "toolStripButton_comparison_project_item_new";
            this.toolStripButton_comparison_project_item_new.Size = new System.Drawing.Size(59, 28);
            this.toolStripButton_comparison_project_item_new.Text = "New";
            this.toolStripButton_comparison_project_item_new.ToolTipText = "New comparison project";
            this.toolStripButton_comparison_project_item_new.Click += new System.EventHandler(this.toolStripButton_comparison_project_item_new_Click);
            // 
            // toolStripButton_comparison_project_item_edit
            // 
            this.toolStripButton_comparison_project_item_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_item_edit.Image")));
            this.toolStripButton_comparison_project_item_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_item_edit.Name = "toolStripButton_comparison_project_item_edit";
            this.toolStripButton_comparison_project_item_edit.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_comparison_project_item_edit.Text = "Edit";
            this.toolStripButton_comparison_project_item_edit.ToolTipText = "Edit comparison project";
            this.toolStripButton_comparison_project_item_edit.Click += new System.EventHandler(this.toolStripButton_comparison_project_item_edit_Click);
            // 
            // toolStripButton_comparison_project_item_remove
            // 
            this.toolStripButton_comparison_project_item_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_item_remove.Image")));
            this.toolStripButton_comparison_project_item_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_item_remove.Name = "toolStripButton_comparison_project_item_remove";
            this.toolStripButton_comparison_project_item_remove.Size = new System.Drawing.Size(78, 28);
            this.toolStripButton_comparison_project_item_remove.Text = "Remove";
            this.toolStripButton_comparison_project_item_remove.ToolTipText = "Remove comparison project";
            this.toolStripButton_comparison_project_item_remove.Click += new System.EventHandler(this.toolStripButton_comparison_project_item_remove_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_comparison_project_file_alignment
            // 
            this.toolStripButton_comparison_project_file_alignment.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_file_alignment.Image")));
            this.toolStripButton_comparison_project_file_alignment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_file_alignment.Name = "toolStripButton_comparison_project_file_alignment";
            this.toolStripButton_comparison_project_file_alignment.Size = new System.Drawing.Size(143, 28);
            this.toolStripButton_comparison_project_file_alignment.Text = "File name alignment";
            this.toolStripButton_comparison_project_file_alignment.Click += new System.EventHandler(this.toolStripButton_comparison_project_file_alignment_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_comparison_project_item_moveDown
            // 
            this.toolStripButton_comparison_project_item_moveDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_item_moveDown.Image")));
            this.toolStripButton_comparison_project_item_moveDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_item_moveDown.Name = "toolStripButton_comparison_project_item_moveDown";
            this.toolStripButton_comparison_project_item_moveDown.Size = new System.Drawing.Size(99, 28);
            this.toolStripButton_comparison_project_item_moveDown.Text = "Move Down";
            this.toolStripButton_comparison_project_item_moveDown.Click += new System.EventHandler(this.toolStripButton_comparison_project_item_moveDown_Click);
            // 
            // toolStripButton_comparison_project_item_moveUp
            // 
            this.toolStripButton_comparison_project_item_moveUp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_comparison_project_item_moveUp.Image")));
            this.toolStripButton_comparison_project_item_moveUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_comparison_project_item_moveUp.Name = "toolStripButton_comparison_project_item_moveUp";
            this.toolStripButton_comparison_project_item_moveUp.Size = new System.Drawing.Size(83, 28);
            this.toolStripButton_comparison_project_item_moveUp.Text = "Move Up";
            this.toolStripButton_comparison_project_item_moveUp.Click += new System.EventHandler(this.toolStripButton_comparison_project_item_moveUp_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 89);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(109, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            this.statusStrip1.Visible = false;
            // 
            // panel_events_log
            // 
            this.panel_events_log.Controls.Add(this.groupBox4);
            this.panel_events_log.Location = new System.Drawing.Point(678, 356);
            this.panel_events_log.Name = "panel_events_log";
            this.panel_events_log.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_events_log.Size = new System.Drawing.Size(96, 108);
            this.panel_events_log.TabIndex = 37;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBox_eventsLogTrackFilters);
            this.groupBox4.Controls.Add(this.checkBox_eventsLogTrackFiles);
            this.groupBox4.Controls.Add(this.checkBox_eventsLogTrackProjects);
            this.groupBox4.Controls.Add(this.checkBox_eventsLogTrackReports);
            this.groupBox4.Controls.Add(this.checkBox_eventsLogTrackCompare);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.linkLabel_clear_comparison_log);
            this.groupBox4.Controls.Add(this.numericUpDown_maximum_comparisonLogEntries);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.pictureBox_clear_comparison_log);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(0, 15);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(96, 93);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Events Log Properties";
            // 
            // checkBox_eventsLogTrackFilters
            // 
            this.checkBox_eventsLogTrackFilters.AutoSize = true;
            this.checkBox_eventsLogTrackFilters.Enabled = false;
            this.checkBox_eventsLogTrackFilters.Location = new System.Drawing.Point(53, 158);
            this.checkBox_eventsLogTrackFilters.Name = "checkBox_eventsLogTrackFilters";
            this.checkBox_eventsLogTrackFilters.Size = new System.Drawing.Size(140, 17);
            this.checkBox_eventsLogTrackFilters.TabIndex = 88;
            this.checkBox_eventsLogTrackFilters.Text = "Filters (Add/Edit/Delete)";
            this.checkBox_eventsLogTrackFilters.UseVisualStyleBackColor = true;
            // 
            // checkBox_eventsLogTrackFiles
            // 
            this.checkBox_eventsLogTrackFiles.AutoSize = true;
            this.checkBox_eventsLogTrackFiles.Location = new System.Drawing.Point(53, 135);
            this.checkBox_eventsLogTrackFiles.Name = "checkBox_eventsLogTrackFiles";
            this.checkBox_eventsLogTrackFiles.Size = new System.Drawing.Size(148, 17);
            this.checkBox_eventsLogTrackFiles.TabIndex = 88;
            this.checkBox_eventsLogTrackFiles.Text = "Files (Move/Copy/Delete)";
            this.checkBox_eventsLogTrackFiles.UseVisualStyleBackColor = true;
            // 
            // checkBox_eventsLogTrackProjects
            // 
            this.checkBox_eventsLogTrackProjects.AutoSize = true;
            this.checkBox_eventsLogTrackProjects.Enabled = false;
            this.checkBox_eventsLogTrackProjects.Location = new System.Drawing.Point(53, 111);
            this.checkBox_eventsLogTrackProjects.Name = "checkBox_eventsLogTrackProjects";
            this.checkBox_eventsLogTrackProjects.Size = new System.Drawing.Size(209, 17);
            this.checkBox_eventsLogTrackProjects.TabIndex = 88;
            this.checkBox_eventsLogTrackProjects.Text = "Comparison Projects (Add/Edit/Delete)";
            this.checkBox_eventsLogTrackProjects.UseVisualStyleBackColor = true;
            // 
            // checkBox_eventsLogTrackReports
            // 
            this.checkBox_eventsLogTrackReports.AutoSize = true;
            this.checkBox_eventsLogTrackReports.Location = new System.Drawing.Point(53, 88);
            this.checkBox_eventsLogTrackReports.Name = "checkBox_eventsLogTrackReports";
            this.checkBox_eventsLogTrackReports.Size = new System.Drawing.Size(154, 17);
            this.checkBox_eventsLogTrackReports.TabIndex = 88;
            this.checkBox_eventsLogTrackReports.Text = "Comparision Reports (New)";
            this.checkBox_eventsLogTrackReports.UseVisualStyleBackColor = true;
            // 
            // checkBox_eventsLogTrackCompare
            // 
            this.checkBox_eventsLogTrackCompare.AutoSize = true;
            this.checkBox_eventsLogTrackCompare.Location = new System.Drawing.Point(53, 65);
            this.checkBox_eventsLogTrackCompare.Name = "checkBox_eventsLogTrackCompare";
            this.checkBox_eventsLogTrackCompare.Size = new System.Drawing.Size(148, 17);
            this.checkBox_eventsLogTrackCompare.TabIndex = 88;
            this.checkBox_eventsLogTrackCompare.Text = "Compare Projects/Folders";
            this.checkBox_eventsLogTrackCompare.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label5.Location = new System.Drawing.Point(21, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 87;
            this.label5.Text = "Events Log History";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(21, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 87;
            this.label3.Text = "Track Events";
            // 
            // linkLabel_clear_comparison_log
            // 
            this.linkLabel_clear_comparison_log.AutoSize = true;
            this.linkLabel_clear_comparison_log.Location = new System.Drawing.Point(31, 272);
            this.linkLabel_clear_comparison_log.Name = "linkLabel_clear_comparison_log";
            this.linkLabel_clear_comparison_log.Size = new System.Drawing.Size(111, 13);
            this.linkLabel_clear_comparison_log.TabIndex = 86;
            this.linkLabel_clear_comparison_log.TabStop = true;
            this.linkLabel_clear_comparison_log.Text = "Clear event log history";
            this.linkLabel_clear_comparison_log.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_clear_comparison_log_LinkClicked);
            // 
            // numericUpDown_maximum_comparisonLogEntries
            // 
            this.numericUpDown_maximum_comparisonLogEntries.Location = new System.Drawing.Point(248, 244);
            this.numericUpDown_maximum_comparisonLogEntries.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_maximum_comparisonLogEntries.Name = "numericUpDown_maximum_comparisonLogEntries";
            this.numericUpDown_maximum_comparisonLogEntries.Size = new System.Drawing.Size(65, 20);
            this.numericUpDown_maximum_comparisonLogEntries.TabIndex = 85;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(31, 247);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(214, 13);
            this.label12.TabIndex = 84;
            this.label12.Text = "Maximum number of entries in the event log:";
            // 
            // pictureBox_clear_comparison_log
            // 
            this.pictureBox_clear_comparison_log.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_clear_comparison_log.Image")));
            this.pictureBox_clear_comparison_log.Location = new System.Drawing.Point(148, 272);
            this.pictureBox_clear_comparison_log.Name = "pictureBox_clear_comparison_log";
            this.pictureBox_clear_comparison_log.Size = new System.Drawing.Size(22, 17);
            this.pictureBox_clear_comparison_log.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_clear_comparison_log.TabIndex = 83;
            this.pictureBox_clear_comparison_log.TabStop = false;
            // 
            // panel_folder_viewer
            // 
            this.panel_folder_viewer.Controls.Add(this.groupBox1);
            this.panel_folder_viewer.Location = new System.Drawing.Point(445, 392);
            this.panel_folder_viewer.Name = "panel_folder_viewer";
            this.panel_folder_viewer.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_folder_viewer.Size = new System.Drawing.Size(181, 78);
            this.panel_folder_viewer.TabIndex = 37;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabel_clear_folder_comparison_histroy);
            this.groupBox1.Controls.Add(this.numericUpDown_maximum_folderComparisonEntries);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.pictureBox6);
            this.groupBox1.Controls.Add(this.pictureBox5);
            this.groupBox1.Controls.Add(this.pictureBox4);
            this.groupBox1.Controls.Add(this.pictureBox_clear_folder_comparison_histroy);
            this.groupBox1.Controls.Add(this.pictureBox3);
            this.groupBox1.Controls.Add(this.pictureBox2);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_empty_folders);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_mismatches_right);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_orphan_files_right);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_mismatches_left);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_orphan_files_left);
            this.groupBox1.Controls.Add(this.checkBox_folderViewer_columns_modified);
            this.groupBox1.Controls.Add(this.checkBox_folderViewer_columns_size);
            this.groupBox1.Controls.Add(this.checkBox_folderViewer_columns_type);
            this.groupBox1.Controls.Add(this.checkBox_folderViewer_columns_name);
            this.groupBox1.Controls.Add(this.checkBox_folder_viewer_show_equal_files);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Folder Viewer Properties";
            // 
            // linkLabel_clear_folder_comparison_histroy
            // 
            this.linkLabel_clear_folder_comparison_histroy.AutoSize = true;
            this.linkLabel_clear_folder_comparison_histroy.Location = new System.Drawing.Point(58, 285);
            this.linkLabel_clear_folder_comparison_histroy.Name = "linkLabel_clear_folder_comparison_histroy";
            this.linkLabel_clear_folder_comparison_histroy.Size = new System.Drawing.Size(150, 13);
            this.linkLabel_clear_folder_comparison_histroy.TabIndex = 77;
            this.linkLabel_clear_folder_comparison_histroy.TabStop = true;
            this.linkLabel_clear_folder_comparison_histroy.Text = "Clear folder comparison history";
            this.linkLabel_clear_folder_comparison_histroy.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_clear_folder_comparison_histroy_LinkClicked);
            // 
            // numericUpDown_maximum_folderComparisonEntries
            // 
            this.numericUpDown_maximum_folderComparisonEntries.Location = new System.Drawing.Point(351, 256);
            this.numericUpDown_maximum_folderComparisonEntries.Name = "numericUpDown_maximum_folderComparisonEntries";
            this.numericUpDown_maximum_folderComparisonEntries.Size = new System.Drawing.Size(47, 20);
            this.numericUpDown_maximum_folderComparisonEntries.TabIndex = 76;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(58, 258);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(286, 13);
            this.label8.TabIndex = 75;
            this.label8.Text = "Maximum number of entries in the folder comparison history:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label9.Location = new System.Drawing.Point(32, 233);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 13);
            this.label9.TabIndex = 74;
            this.label9.Text = "Folder comparison history";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label10.Location = new System.Drawing.Point(483, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 74;
            this.label10.Text = "View columns";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label7.Location = new System.Drawing.Point(32, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 13);
            this.label7.TabIndex = 74;
            this.label7.Text = "Folder/File comparision";
            // 
            // pictureBox6
            // 
            this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
            this.pictureBox6.Location = new System.Drawing.Point(61, 184);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(22, 17);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox6.TabIndex = 1;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
            this.pictureBox5.Location = new System.Drawing.Point(61, 161);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(22, 17);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 1;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(61, 139);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(22, 17);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 1;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox_clear_folder_comparison_histroy
            // 
            this.pictureBox_clear_folder_comparison_histroy.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_clear_folder_comparison_histroy.Image")));
            this.pictureBox_clear_folder_comparison_histroy.Location = new System.Drawing.Point(214, 283);
            this.pictureBox_clear_folder_comparison_histroy.Name = "pictureBox_clear_folder_comparison_histroy";
            this.pictureBox_clear_folder_comparison_histroy.Size = new System.Drawing.Size(22, 17);
            this.pictureBox_clear_folder_comparison_histroy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_clear_folder_comparison_histroy.TabIndex = 1;
            this.pictureBox_clear_folder_comparison_histroy.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(61, 115);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(22, 17);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(61, 92);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(22, 17);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(61, 68);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(22, 17);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // checkBox_folder_viewer_show_empty_folders
            // 
            this.checkBox_folder_viewer_show_empty_folders.AutoSize = true;
            this.checkBox_folder_viewer_show_empty_folders.Location = new System.Drawing.Point(89, 184);
            this.checkBox_folder_viewer_show_empty_folders.Name = "checkBox_folder_viewer_show_empty_folders";
            this.checkBox_folder_viewer_show_empty_folders.Size = new System.Drawing.Size(118, 17);
            this.checkBox_folder_viewer_show_empty_folders.TabIndex = 0;
            this.checkBox_folder_viewer_show_empty_folders.Text = "Show empty folders";
            this.checkBox_folder_viewer_show_empty_folders.UseVisualStyleBackColor = true;
            // 
            // checkBox_folder_viewer_show_mismatches_right
            // 
            this.checkBox_folder_viewer_show_mismatches_right.AutoSize = true;
            this.checkBox_folder_viewer_show_mismatches_right.Location = new System.Drawing.Point(89, 161);
            this.checkBox_folder_viewer_show_mismatches_right.Name = "checkBox_folder_viewer_show_mismatches_right";
            this.checkBox_folder_viewer_show_mismatches_right.Size = new System.Drawing.Size(194, 17);
            this.checkBox_folder_viewer_show_mismatches_right.TabIndex = 0;
            this.checkBox_folder_viewer_show_mismatches_right.Text = "Show mismatches - right side newer";
            this.checkBox_folder_viewer_show_mismatches_right.UseVisualStyleBackColor = true;
            // 
            // checkBox_folder_viewer_show_orphan_files_right
            // 
            this.checkBox_folder_viewer_show_orphan_files_right.AutoSize = true;
            this.checkBox_folder_viewer_show_orphan_files_right.Location = new System.Drawing.Point(89, 115);
            this.checkBox_folder_viewer_show_orphan_files_right.Name = "checkBox_folder_viewer_show_orphan_files_right";
            this.checkBox_folder_viewer_show_orphan_files_right.Size = new System.Drawing.Size(188, 17);
            this.checkBox_folder_viewer_show_orphan_files_right.TabIndex = 0;
            this.checkBox_folder_viewer_show_orphan_files_right.Text = "Show orphan files on the right side";
            this.checkBox_folder_viewer_show_orphan_files_right.UseVisualStyleBackColor = true;
            // 
            // checkBox_folder_viewer_show_mismatches_left
            // 
            this.checkBox_folder_viewer_show_mismatches_left.AutoSize = true;
            this.checkBox_folder_viewer_show_mismatches_left.Location = new System.Drawing.Point(89, 139);
            this.checkBox_folder_viewer_show_mismatches_left.Name = "checkBox_folder_viewer_show_mismatches_left";
            this.checkBox_folder_viewer_show_mismatches_left.Size = new System.Drawing.Size(188, 17);
            this.checkBox_folder_viewer_show_mismatches_left.TabIndex = 0;
            this.checkBox_folder_viewer_show_mismatches_left.Text = "Show mismatches - left side newer";
            this.checkBox_folder_viewer_show_mismatches_left.UseVisualStyleBackColor = true;
            // 
            // checkBox_folder_viewer_show_orphan_files_left
            // 
            this.checkBox_folder_viewer_show_orphan_files_left.AutoSize = true;
            this.checkBox_folder_viewer_show_orphan_files_left.Location = new System.Drawing.Point(89, 92);
            this.checkBox_folder_viewer_show_orphan_files_left.Name = "checkBox_folder_viewer_show_orphan_files_left";
            this.checkBox_folder_viewer_show_orphan_files_left.Size = new System.Drawing.Size(182, 17);
            this.checkBox_folder_viewer_show_orphan_files_left.TabIndex = 0;
            this.checkBox_folder_viewer_show_orphan_files_left.Text = "Show orphan files on the left side";
            this.checkBox_folder_viewer_show_orphan_files_left.UseVisualStyleBackColor = true;
            // 
            // checkBox_folderViewer_columns_modified
            // 
            this.checkBox_folderViewer_columns_modified.AutoSize = true;
            this.checkBox_folderViewer_columns_modified.Checked = true;
            this.checkBox_folderViewer_columns_modified.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_folderViewer_columns_modified.Enabled = false;
            this.checkBox_folderViewer_columns_modified.Location = new System.Drawing.Point(523, 136);
            this.checkBox_folderViewer_columns_modified.Name = "checkBox_folderViewer_columns_modified";
            this.checkBox_folderViewer_columns_modified.Size = new System.Drawing.Size(168, 17);
            this.checkBox_folderViewer_columns_modified.TabIndex = 0;
            this.checkBox_folderViewer_columns_modified.Text = "View file modified date column";
            this.checkBox_folderViewer_columns_modified.UseVisualStyleBackColor = true;
            // 
            // checkBox_folderViewer_columns_size
            // 
            this.checkBox_folderViewer_columns_size.AutoSize = true;
            this.checkBox_folderViewer_columns_size.Location = new System.Drawing.Point(523, 113);
            this.checkBox_folderViewer_columns_size.Name = "checkBox_folderViewer_columns_size";
            this.checkBox_folderViewer_columns_size.Size = new System.Drawing.Size(123, 17);
            this.checkBox_folderViewer_columns_size.TabIndex = 0;
            this.checkBox_folderViewer_columns_size.Text = "View file size column";
            this.checkBox_folderViewer_columns_size.UseVisualStyleBackColor = true;
            // 
            // checkBox_folderViewer_columns_type
            // 
            this.checkBox_folderViewer_columns_type.AutoSize = true;
            this.checkBox_folderViewer_columns_type.Location = new System.Drawing.Point(523, 90);
            this.checkBox_folderViewer_columns_type.Name = "checkBox_folderViewer_columns_type";
            this.checkBox_folderViewer_columns_type.Size = new System.Drawing.Size(125, 17);
            this.checkBox_folderViewer_columns_type.TabIndex = 0;
            this.checkBox_folderViewer_columns_type.Text = "View file type column";
            this.checkBox_folderViewer_columns_type.UseVisualStyleBackColor = true;
            // 
            // checkBox_folderViewer_columns_name
            // 
            this.checkBox_folderViewer_columns_name.AutoSize = true;
            this.checkBox_folderViewer_columns_name.Checked = true;
            this.checkBox_folderViewer_columns_name.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_folderViewer_columns_name.Enabled = false;
            this.checkBox_folderViewer_columns_name.Location = new System.Drawing.Point(523, 68);
            this.checkBox_folderViewer_columns_name.Name = "checkBox_folderViewer_columns_name";
            this.checkBox_folderViewer_columns_name.Size = new System.Drawing.Size(162, 17);
            this.checkBox_folderViewer_columns_name.TabIndex = 0;
            this.checkBox_folderViewer_columns_name.Text = "View folder/file name column";
            this.checkBox_folderViewer_columns_name.UseVisualStyleBackColor = true;
            // 
            // checkBox_folder_viewer_show_equal_files
            // 
            this.checkBox_folder_viewer_show_equal_files.AutoSize = true;
            this.checkBox_folder_viewer_show_equal_files.Location = new System.Drawing.Point(89, 68);
            this.checkBox_folder_viewer_show_equal_files.Name = "checkBox_folder_viewer_show_equal_files";
            this.checkBox_folder_viewer_show_equal_files.Size = new System.Drawing.Size(142, 17);
            this.checkBox_folder_viewer_show_equal_files.TabIndex = 0;
            this.checkBox_folder_viewer_show_equal_files.Text = "Show files that are equal";
            this.checkBox_folder_viewer_show_equal_files.UseVisualStyleBackColor = true;
            // 
            // panel_general
            // 
            this.panel_general.Controls.Add(this.groupBox6);
            this.panel_general.Location = new System.Drawing.Point(319, 391);
            this.panel_general.Name = "panel_general";
            this.panel_general.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_general.Size = new System.Drawing.Size(75, 89);
            this.panel_general.TabIndex = 37;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.checkBox10);
            this.groupBox6.Controls.Add(this.checkBox9);
            this.groupBox6.Controls.Add(this.checkBox8);
            this.groupBox6.Controls.Add(this.checkBox_automaticallyExpandComparisonFolders);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(0, 15);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(75, 74);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "General Properties";
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Checked = true;
            this.checkBox10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox10.Enabled = false;
            this.checkBox10.Location = new System.Drawing.Point(51, 188);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(181, 17);
            this.checkBox10.TabIndex = 87;
            this.checkBox10.Text = "Confirm create comparison report";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Checked = true;
            this.checkBox9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox9.Enabled = false;
            this.checkBox9.Location = new System.Drawing.Point(51, 165);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(106, 17);
            this.checkBox9.TabIndex = 87;
            this.checkBox9.Text = "Confirm file move";
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Checked = true;
            this.checkBox8.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox8.Enabled = false;
            this.checkBox8.Location = new System.Drawing.Point(51, 141);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(103, 17);
            this.checkBox8.TabIndex = 87;
            this.checkBox8.Text = "Confirm file copy";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox_automaticallyExpandComparisonFolders
            // 
            this.checkBox_automaticallyExpandComparisonFolders.AutoSize = true;
            this.checkBox_automaticallyExpandComparisonFolders.Location = new System.Drawing.Point(51, 60);
            this.checkBox_automaticallyExpandComparisonFolders.Name = "checkBox_automaticallyExpandComparisonFolders";
            this.checkBox_automaticallyExpandComparisonFolders.Size = new System.Drawing.Size(280, 17);
            this.checkBox_automaticallyExpandComparisonFolders.TabIndex = 86;
            this.checkBox_automaticallyExpandComparisonFolders.Text = "Automatically expand the folder trees when comparing";
            this.checkBox_automaticallyExpandComparisonFolders.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label14.Location = new System.Drawing.Point(23, 118);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(109, 13);
            this.label14.TabIndex = 85;
            this.label14.Text = "Confirmation windows";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label13.Location = new System.Drawing.Point(23, 39);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 13);
            this.label13.TabIndex = 85;
            this.label13.Text = "Comparison window";
            // 
            // panel_price_groups
            // 
            this.panel_price_groups.Controls.Add(this.panel3);
            this.panel_price_groups.Controls.Add(this.panel1);
            this.panel_price_groups.Location = new System.Drawing.Point(255, 71);
            this.panel_price_groups.Name = "panel_price_groups";
            this.panel_price_groups.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_price_groups.Size = new System.Drawing.Size(458, 209);
            this.panel_price_groups.TabIndex = 37;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listView_price_groups);
            this.panel3.Controls.Add(this.splitter1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 57);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(458, 152);
            this.panel3.TabIndex = 1;
            // 
            // listView_price_groups
            // 
            this.listView_price_groups.AllowDrop = true;
            this.listView_price_groups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14});
            this.listView_price_groups.ContextMenuStrip = this.contextMenuStrip_price_groups_listview;
            this.listView_price_groups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_price_groups.FullRowSelect = true;
            this.listView_price_groups.GridLines = true;
            this.listView_price_groups.HideSelection = false;
            this.listView_price_groups.Location = new System.Drawing.Point(154, 0);
            this.listView_price_groups.Name = "listView_price_groups";
            this.listView_price_groups.ShowItemToolTips = true;
            this.listView_price_groups.Size = new System.Drawing.Size(304, 152);
            this.listView_price_groups.SmallImageList = this.imageList3;
            this.listView_price_groups.TabIndex = 2;
            this.listView_price_groups.UseCompatibleStateImageBehavior = false;
            this.listView_price_groups.View = System.Windows.Forms.View.Details;
            this.listView_price_groups.SelectedIndexChanged += new System.EventHandler(this.listView_price_groups_SelectedIndexChanged);
            this.listView_price_groups.DoubleClick += new System.EventHandler(this.listView_price_groups_DoubleClick);
            this.listView_price_groups.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_price_groups_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Source";
            this.columnHeader1.Width = 55;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Target";
            this.columnHeader2.Width = 55;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Rate";
            this.columnHeader4.Width = 65;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Round";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "PM";
            this.columnHeader6.Width = 50;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "CM";
            this.columnHeader7.Width = 50;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Rep.";
            this.columnHeader8.Width = 50;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "100%";
            this.columnHeader9.Width = 50;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "99-94%";
            this.columnHeader10.Width = 50;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "94-85%";
            this.columnHeader11.Width = 50;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "84-75%";
            this.columnHeader12.Width = 50;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "75-50%";
            this.columnHeader13.Width = 50;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "New";
            this.columnHeader14.Width = 50;
            // 
            // contextMenuStrip_price_groups_listview
            // 
            this.contextMenuStrip_price_groups_listview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPriceToolStripMenuItem,
            this.editPriceToolStripMenuItem,
            this.removePriceToolStripMenuItem,
            this.toolStripSeparator3,
            this.analysisBandToolStripMenuItem});
            this.contextMenuStrip_price_groups_listview.Name = "contextMenuStrip1";
            this.contextMenuStrip_price_groups_listview.Size = new System.Drawing.Size(161, 98);
            // 
            // addPriceToolStripMenuItem
            // 
            this.addPriceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addPriceToolStripMenuItem.Image")));
            this.addPriceToolStripMenuItem.Name = "addPriceToolStripMenuItem";
            this.addPriceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addPriceToolStripMenuItem.Text = "Add Rate";
            this.addPriceToolStripMenuItem.Click += new System.EventHandler(this.addPriceToolStripMenuItem_Click);
            // 
            // editPriceToolStripMenuItem
            // 
            this.editPriceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editPriceToolStripMenuItem.Image")));
            this.editPriceToolStripMenuItem.Name = "editPriceToolStripMenuItem";
            this.editPriceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.editPriceToolStripMenuItem.Text = "Edit Rate";
            this.editPriceToolStripMenuItem.Click += new System.EventHandler(this.editPriceToolStripMenuItem_Click);
            // 
            // removePriceToolStripMenuItem
            // 
            this.removePriceToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removePriceToolStripMenuItem.Image")));
            this.removePriceToolStripMenuItem.Name = "removePriceToolStripMenuItem";
            this.removePriceToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.removePriceToolStripMenuItem.Text = "Remove Rate";
            this.removePriceToolStripMenuItem.Click += new System.EventHandler(this.removePriceToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(157, 6);
            // 
            // analysisBandToolStripMenuItem
            // 
            this.analysisBandToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("analysisBandToolStripMenuItem.Image")));
            this.analysisBandToolStripMenuItem.Name = "analysisBandToolStripMenuItem";
            this.analysisBandToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.analysisBandToolStripMenuItem.Text = "Analysis Band %";
            this.analysisBandToolStripMenuItem.Click += new System.EventHandler(this.analysisBandToolStripMenuItem_Click);
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter1.Location = new System.Drawing.Point(151, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 152);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.treeView_price_groups);
            this.panel4.Controls.Add(this.panel5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(151, 152);
            this.panel4.TabIndex = 0;
            // 
            // treeView_price_groups
            // 
            this.treeView_price_groups.BackColor = System.Drawing.Color.White;
            this.treeView_price_groups.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView_price_groups.ContextMenuStrip = this.contextMenuStrip_price_groups_treeview;
            this.treeView_price_groups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_price_groups.FullRowSelect = true;
            this.treeView_price_groups.HideSelection = false;
            this.treeView_price_groups.ImageIndex = 0;
            this.treeView_price_groups.ImageList = this.imageList2;
            this.treeView_price_groups.ItemHeight = 32;
            this.treeView_price_groups.Location = new System.Drawing.Point(0, 26);
            this.treeView_price_groups.Name = "treeView_price_groups";
            this.treeView_price_groups.SelectedImageIndex = 0;
            this.treeView_price_groups.ShowNodeToolTips = true;
            this.treeView_price_groups.ShowRootLines = false;
            this.treeView_price_groups.Size = new System.Drawing.Size(151, 126);
            this.treeView_price_groups.TabIndex = 22;
            this.treeView_price_groups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_price_groups_AfterSelect);
            this.treeView_price_groups.DoubleClick += new System.EventHandler(this.treeView_price_groups_DoubleClick);
            this.treeView_price_groups.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeView_price_groups_KeyUp);
            // 
            // contextMenuStrip_price_groups_treeview
            // 
            this.contextMenuStrip_price_groups_treeview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newPriceGroupToolStripMenuItem,
            this.editPriceGroupToolStripMenuItem,
            this.removePriceGroupToolStripMenuItem,
            this.toolStripSeparator11,
            this.setAsDefaultPriceGroupToolStripMenuItem});
            this.contextMenuStrip_price_groups_treeview.Name = "contextMenuStrip2";
            this.contextMenuStrip_price_groups_treeview.Size = new System.Drawing.Size(207, 98);
            // 
            // newPriceGroupToolStripMenuItem
            // 
            this.newPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newPriceGroupToolStripMenuItem.Image")));
            this.newPriceGroupToolStripMenuItem.Name = "newPriceGroupToolStripMenuItem";
            this.newPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.newPriceGroupToolStripMenuItem.Text = "New Rate Group";
            this.newPriceGroupToolStripMenuItem.Click += new System.EventHandler(this.newPriceGroupToolStripMenuItem_Click);
            // 
            // editPriceGroupToolStripMenuItem
            // 
            this.editPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editPriceGroupToolStripMenuItem.Image")));
            this.editPriceGroupToolStripMenuItem.Name = "editPriceGroupToolStripMenuItem";
            this.editPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.editPriceGroupToolStripMenuItem.Text = "Edit Rate Group";
            this.editPriceGroupToolStripMenuItem.Click += new System.EventHandler(this.editPriceGroupToolStripMenuItem_Click);
            // 
            // removePriceGroupToolStripMenuItem
            // 
            this.removePriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removePriceGroupToolStripMenuItem.Image")));
            this.removePriceGroupToolStripMenuItem.Name = "removePriceGroupToolStripMenuItem";
            this.removePriceGroupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.removePriceGroupToolStripMenuItem.Text = "Remove Rate Group";
            this.removePriceGroupToolStripMenuItem.Click += new System.EventHandler(this.removePriceGroupToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(203, 6);
            // 
            // setAsDefaultPriceGroupToolStripMenuItem
            // 
            this.setAsDefaultPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("setAsDefaultPriceGroupToolStripMenuItem.Image")));
            this.setAsDefaultPriceGroupToolStripMenuItem.Name = "setAsDefaultPriceGroupToolStripMenuItem";
            this.setAsDefaultPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.setAsDefaultPriceGroupToolStripMenuItem.Text = "Set as default Rate Group";
            this.setAsDefaultPriceGroupToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultPriceGroupToolStripMenuItem_Click);
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "Coins-32x32.png");
            this.imageList2.Images.SetKeyName(1, "Coins-32.png");
            this.imageList2.Images.SetKeyName(2, "Price-32.png");
            this.imageList2.Images.SetKeyName(3, "Coins-32(1).png");
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.label1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(151, 26);
            this.panel5.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rate Groups";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.toolStrip_ProjectsBlocks);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(458, 42);
            this.panel1.TabIndex = 0;
            // 
            // toolStrip_ProjectsBlocks
            // 
            this.toolStrip_ProjectsBlocks.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip_ProjectsBlocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_priceGroup_new,
            this.toolStripButton_priceGroup_edit,
            this.toolStripButton_priceGroup_remove,
            this.toolStripSeparator2,
            this.toolStripButton_groupPrice_add,
            this.toolStripButton_groupPrice_edit,
            this.toolStripButton_groupPrice_remove,
            this.toolStripSeparator1,
            this.toolStripButton_analysisBandPercentage});
            this.toolStrip_ProjectsBlocks.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_ProjectsBlocks.Name = "toolStrip_ProjectsBlocks";
            this.toolStrip_ProjectsBlocks.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip_ProjectsBlocks.Size = new System.Drawing.Size(458, 31);
            this.toolStrip_ProjectsBlocks.TabIndex = 8;
            // 
            // toolStripButton_priceGroup_new
            // 
            this.toolStripButton_priceGroup_new.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_priceGroup_new.Image")));
            this.toolStripButton_priceGroup_new.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_priceGroup_new.Name = "toolStripButton_priceGroup_new";
            this.toolStripButton_priceGroup_new.Size = new System.Drawing.Size(59, 28);
            this.toolStripButton_priceGroup_new.Text = "New";
            this.toolStripButton_priceGroup_new.ToolTipText = "New Rate Group";
            this.toolStripButton_priceGroup_new.Click += new System.EventHandler(this.toolStripButton_priceGroup_new_Click);
            // 
            // toolStripButton_priceGroup_edit
            // 
            this.toolStripButton_priceGroup_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_priceGroup_edit.Image")));
            this.toolStripButton_priceGroup_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_priceGroup_edit.Name = "toolStripButton_priceGroup_edit";
            this.toolStripButton_priceGroup_edit.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_priceGroup_edit.Text = "Edit";
            this.toolStripButton_priceGroup_edit.ToolTipText = "Edit Rate Group";
            this.toolStripButton_priceGroup_edit.Click += new System.EventHandler(this.toolStripButton_priceGroup_edit_Click);
            // 
            // toolStripButton_priceGroup_remove
            // 
            this.toolStripButton_priceGroup_remove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_priceGroup_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_priceGroup_remove.Image")));
            this.toolStripButton_priceGroup_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_priceGroup_remove.Name = "toolStripButton_priceGroup_remove";
            this.toolStripButton_priceGroup_remove.Size = new System.Drawing.Size(28, 28);
            this.toolStripButton_priceGroup_remove.Text = "Remove";
            this.toolStripButton_priceGroup_remove.ToolTipText = "Remove Rate Group";
            this.toolStripButton_priceGroup_remove.Click += new System.EventHandler(this.toolStripButton_priceGroup_remove_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_groupPrice_add
            // 
            this.toolStripButton_groupPrice_add.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_groupPrice_addMultiple});
            this.toolStripButton_groupPrice_add.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_groupPrice_add.Image")));
            this.toolStripButton_groupPrice_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_groupPrice_add.Name = "toolStripButton_groupPrice_add";
            this.toolStripButton_groupPrice_add.Size = new System.Drawing.Size(95, 28);
            this.toolStripButton_groupPrice_add.Text = "Add Rate";
            this.toolStripButton_groupPrice_add.ButtonClick += new System.EventHandler(this.toolStripButton_groupPrice_add_Click);
            // 
            // toolStripButton_groupPrice_addMultiple
            // 
            this.toolStripButton_groupPrice_addMultiple.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_groupPrice_addMultiple.Image")));
            this.toolStripButton_groupPrice_addMultiple.Name = "toolStripButton_groupPrice_addMultiple";
            this.toolStripButton_groupPrice_addMultiple.Size = new System.Drawing.Size(174, 22);
            this.toolStripButton_groupPrice_addMultiple.Text = "Add Multiple Rates";
            this.toolStripButton_groupPrice_addMultiple.Click += new System.EventHandler(this.toolStripButton_groupPrice_addMultiple_Click);
            // 
            // toolStripButton_groupPrice_edit
            // 
            this.toolStripButton_groupPrice_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_groupPrice_edit.Image")));
            this.toolStripButton_groupPrice_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_groupPrice_edit.Name = "toolStripButton_groupPrice_edit";
            this.toolStripButton_groupPrice_edit.Size = new System.Drawing.Size(81, 28);
            this.toolStripButton_groupPrice_edit.Text = "Edit Rate";
            this.toolStripButton_groupPrice_edit.Click += new System.EventHandler(this.toolStripButton_groupPrice_edit_Click);
            // 
            // toolStripButton_groupPrice_remove
            // 
            this.toolStripButton_groupPrice_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_groupPrice_remove.Image")));
            this.toolStripButton_groupPrice_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_groupPrice_remove.Name = "toolStripButton_groupPrice_remove";
            this.toolStripButton_groupPrice_remove.Size = new System.Drawing.Size(104, 28);
            this.toolStripButton_groupPrice_remove.Text = "Remove Rate";
            this.toolStripButton_groupPrice_remove.Click += new System.EventHandler(this.toolStripButton_groupPrice_remove_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_analysisBandPercentage
            // 
            this.toolStripButton_analysisBandPercentage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_analysisBandPercentage.Image")));
            this.toolStripButton_analysisBandPercentage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_analysisBandPercentage.Name = "toolStripButton_analysisBandPercentage";
            this.toolStripButton_analysisBandPercentage.Size = new System.Drawing.Size(121, 28);
            this.toolStripButton_analysisBandPercentage.Text = "Analysis Band %";
            this.toolStripButton_analysisBandPercentage.Click += new System.EventHandler(this.toolStripButton_analysisBandPercentage_Click);
            // 
            // panel_viewers
            // 
            this.panel_viewers.Controls.Add(this.groupBox9);
            this.panel_viewers.Location = new System.Drawing.Point(490, 92);
            this.panel_viewers.Name = "panel_viewers";
            this.panel_viewers.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_viewers.Size = new System.Drawing.Size(170, 127);
            this.panel_viewers.TabIndex = 39;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.linkLabel_reportViewer);
            this.groupBox9.Controls.Add(this.linkLabel_fileViewer);
            this.groupBox9.Controls.Add(this.linkLabel_folderViewer);
            this.groupBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox9.Location = new System.Drawing.Point(0, 15);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(170, 112);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Viewers";
            // 
            // linkLabel_reportViewer
            // 
            this.linkLabel_reportViewer.AutoSize = true;
            this.linkLabel_reportViewer.Location = new System.Drawing.Point(42, 103);
            this.linkLabel_reportViewer.Name = "linkLabel_reportViewer";
            this.linkLabel_reportViewer.Size = new System.Drawing.Size(74, 13);
            this.linkLabel_reportViewer.TabIndex = 0;
            this.linkLabel_reportViewer.TabStop = true;
            this.linkLabel_reportViewer.Text = "Report Viewer";
            this.linkLabel_reportViewer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_reportViewer_LinkClicked);
            // 
            // linkLabel_fileViewer
            // 
            this.linkLabel_fileViewer.AutoSize = true;
            this.linkLabel_fileViewer.Enabled = false;
            this.linkLabel_fileViewer.Location = new System.Drawing.Point(42, 76);
            this.linkLabel_fileViewer.Name = "linkLabel_fileViewer";
            this.linkLabel_fileViewer.Size = new System.Drawing.Size(58, 13);
            this.linkLabel_fileViewer.TabIndex = 0;
            this.linkLabel_fileViewer.TabStop = true;
            this.linkLabel_fileViewer.Text = "File Viewer";
            // 
            // linkLabel_folderViewer
            // 
            this.linkLabel_folderViewer.AutoSize = true;
            this.linkLabel_folderViewer.Location = new System.Drawing.Point(42, 49);
            this.linkLabel_folderViewer.Name = "linkLabel_folderViewer";
            this.linkLabel_folderViewer.Size = new System.Drawing.Size(71, 13);
            this.linkLabel_folderViewer.TabIndex = 0;
            this.linkLabel_folderViewer.TabStop = true;
            this.linkLabel_folderViewer.Text = "Folder Viewer";
            this.linkLabel_folderViewer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_folderViewer_LinkClicked);
            // 
            // panel_fonts
            // 
            this.panel_fonts.Controls.Add(this.groupBox2);
            this.panel_fonts.Location = new System.Drawing.Point(445, 278);
            this.panel_fonts.Name = "panel_fonts";
            this.panel_fonts.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_fonts.Size = new System.Drawing.Size(101, 77);
            this.panel_fonts.TabIndex = 37;
            // 
            // groupBox2
            // 
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(101, 62);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Font Properties";
            // 
            // tagVisualizationComboBox
            // 
            this.tagVisualizationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagVisualizationComboBox.FormattingEnabled = true;
            this.tagVisualizationComboBox.Items.AddRange(new object[] {
            "Empty",
            "Partial",
            "Full"});
            this.tagVisualizationComboBox.Location = new System.Drawing.Point(113, 155);
            this.tagVisualizationComboBox.Name = "tagVisualizationComboBox";
            this.tagVisualizationComboBox.Size = new System.Drawing.Size(170, 21);
            this.tagVisualizationComboBox.TabIndex = 86;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(19, 158);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 13);
            this.label16.TabIndex = 85;
            this.label16.Text = "Tag visualization:";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1124, 553);
            this.Controls.Add(this.treeView_main);
            this.Controls.Add(this.panel_ErrorHandling);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_Save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.panel_ErrorHandling.ResumeLayout(false);
            this.panel_report_viewer.ResumeLayout(false);
            this.panel_options.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.panel_reports.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.panel_filters.ResumeLayout(false);
            this.panel_filters.PerformLayout();
            this.contextMenuStrip_filters.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.panel_comparer.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.panel_comparision_projects.ResumeLayout(false);
            this.panel_comparision_projects.PerformLayout();
            this.contextMenuStrip_comparison_project_listview.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_events_log.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maximum_comparisonLogEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clear_comparison_log)).EndInit();
            this.panel_folder_viewer.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maximum_folderComparisonEntries)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_clear_folder_comparison_histroy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel_general.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panel_price_groups.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.contextMenuStrip_price_groups_listview.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.contextMenuStrip_price_groups_treeview.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.toolStrip_ProjectsBlocks.ResumeLayout(false);
            this.toolStrip_ProjectsBlocks.PerformLayout();
            this.panel_viewers.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.panel_fonts.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_header;
        private System.Windows.Forms.PictureBox pictureBox_header;
        private System.Windows.Forms.Panel panel_ErrorHandling;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel panel_report_viewer;
        private System.Windows.Forms.Panel panel_events_log;
        private System.Windows.Forms.Panel panel_folder_viewer;
        private System.Windows.Forms.Panel panel_fonts;
        private System.Windows.Forms.Panel panel_general;
        private System.Windows.Forms.Panel panel_filters;
        private System.Windows.Forms.Panel panel_price_groups;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListView listView_price_groups;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStrip toolStrip_ProjectsBlocks;
        private System.Windows.Forms.ToolStripButton toolStripButton_priceGroup_edit;
        private System.Windows.Forms.ToolStripButton toolStripButton_priceGroup_remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton_groupPrice_edit;
        private System.Windows.Forms.ToolStripButton toolStripButton_groupPrice_remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_analysisBandPercentage;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ToolStripButton toolStripButton_priceGroup_new;
        private System.Windows.Forms.TreeView treeView_price_groups;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSplitButton toolStripButton_groupPrice_add;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton_groupPrice_addMultiple;
        private System.Windows.Forms.ImageList imageList3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_price_groups_listview;
        private System.Windows.Forms.ToolStripMenuItem addPriceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPriceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePriceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem analysisBandToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_price_groups_treeview;
        private System.Windows.Forms.ToolStripMenuItem newPriceGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editPriceGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePriceGroupToolStripMenuItem;
        private System.Windows.Forms.Panel panel_comparision_projects;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_comparison_project_listview;
        private System.Windows.Forms.ToolStripMenuItem newCompareListItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editCompareListItemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeCompareListItemToolStripMenuItem;
        public System.Windows.Forms.TreeView treeView_main;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox6;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_orphan_files_left;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_equal_files;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_orphan_files_right;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_mismatches_left;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_empty_folders;
        internal System.Windows.Forms.CheckBox checkBox_folder_viewer_show_mismatches_right;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.NumericUpDown numericUpDown_maximum_folderComparisonEntries;
        private System.Windows.Forms.Label label9;
        internal System.Windows.Forms.LinkLabel linkLabel_clear_folder_comparison_histroy;
        private System.Windows.Forms.PictureBox pictureBox_clear_folder_comparison_histroy;
        private System.Windows.Forms.ListView listView_filters;
        private System.Windows.Forms.ColumnHeader columnHeader18;
        private System.Windows.Forms.ColumnHeader columnHeader19;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton toolStripButton_filters_add;
        private System.Windows.Forms.ToolStripButton toolStripButton_filters_edit;
        private System.Windows.Forms.ToolStripButton toolStripButton_filers_remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButton_filters_move_down;
        private System.Windows.Forms.ToolStripButton toolStripButton_filters_moveUp;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_filters;
        private System.Windows.Forms.ToolStripMenuItem addFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem moveDownFilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpFilterToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ListView listView_comparison_projects;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader17;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_item_new;
        private System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_item_edit;
        private System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_item_remove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_item_moveDown;
        private System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_item_moveUp;
        private System.Windows.Forms.ToolStripButton toolStripButton_filers_export;
        private System.Windows.Forms.ToolStripButton toolStripButton_filters_import;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.Label label10;
        internal System.Windows.Forms.CheckBox checkBox_folderViewer_columns_modified;
        internal System.Windows.Forms.CheckBox checkBox_folderViewer_columns_size;
        internal System.Windows.Forms.CheckBox checkBox_folderViewer_columns_type;
        internal System.Windows.Forms.CheckBox checkBox_folderViewer_columns_name;
        private System.Windows.Forms.ImageList imageList4;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.CheckBox checkBox_automaticallyExpandComparisonFolders;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        public System.Windows.Forms.ToolStripButton toolStripButton_comparison_project_file_alignment;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem fileAlignmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem setAsDefaultPriceGroupToolStripMenuItem;
        private System.Windows.Forms.Panel panel_options;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox checkBox_viewFilesWithNoTranslationDifferences;
        public System.Windows.Forms.CheckBox checkBox_calculateSummaryAnalysisBasedOnFilteredRows;
        public System.Windows.Forms.CheckBox checkBox_showGoogleChartsInReport;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.CheckBox checkBox_showSegmentComments;
        public System.Windows.Forms.CheckBox checkBox_showTargetSegmentComparison;
        public System.Windows.Forms.CheckBox checkBox_showUpdatedTargetSegment;
        public System.Windows.Forms.CheckBox checkBox_showOriginalTargetSegment;
        public System.Windows.Forms.CheckBox checkBox_showSegmentPEM;
        public System.Windows.Forms.CheckBox checkBox_showSegmentTERPAnalysis;
        public System.Windows.Forms.CheckBox checkBox_showSegmentMatch;
        public System.Windows.Forms.CheckBox checkBox_showSegmentStatus;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.CheckBox checkBox_showLockedSegments;
        public System.Windows.Forms.CheckBox checkBox_showOriginalSourceSegment;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox7;
        public System.Windows.Forms.CheckBox checkBox_viewLockedSegments;
        public System.Windows.Forms.CheckBox checkBox_viewSegmentsWithComments;
        public System.Windows.Forms.CheckBox checkBox_viewSegmentsWithStatusChanges;
        public System.Windows.Forms.CheckBox checkBox_viewSegmentsWithTranslationChanges;
        public System.Windows.Forms.CheckBox checkBox_viewSegmentsWithNoChanges;
        private System.Windows.Forms.ImageList imageList5;
        private System.Windows.Forms.Panel panel_comparer;
        private System.Windows.Forms.GroupBox groupBox8;
        public System.Windows.Forms.CheckBox checkBox_includeTagsInComparison;
        public System.Windows.Forms.ComboBox comboBox_comparisonType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_changeFormattingTagRemoved;
        private System.Windows.Forms.Button button_changeFormattingTagNew;
        private System.Windows.Forms.Button button_changeFormattingTextRemoved;
        private System.Windows.Forms.Button button_changeFormattingTextNew;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Tag_Removed;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Tag_New;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Text_Removed;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Text_New;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel_resetToDefaults_comparer;
        public System.Windows.Forms.CheckBox checkBox_showUpdatedRevisionMarkerTargetSegment;
        public System.Windows.Forms.CheckBox checkBox_showOriginalRevisionMarkerTargetSegment;
        private System.Windows.Forms.Panel panel_viewers;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.LinkLabel linkLabel_reportViewer;
        private System.Windows.Forms.LinkLabel linkLabel_fileViewer;
        private System.Windows.Forms.LinkLabel linkLabel_folderViewer;
        internal System.Windows.Forms.LinkLabel linkLabel_clear_comparison_log;
        internal System.Windows.Forms.NumericUpDown numericUpDown_maximum_comparisonLogEntries;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureBox_clear_comparison_log;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox checkBox_eventsLogTrackCompare;
        public System.Windows.Forms.CheckBox checkBox_eventsLogTrackReports;
        public System.Windows.Forms.CheckBox checkBox_eventsLogTrackProjects;
        public System.Windows.Forms.CheckBox checkBox_eventsLogTrackFiles;
        public System.Windows.Forms.CheckBox checkBox_eventsLogTrackFilters;
        private System.Windows.Forms.Panel panel_reports;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Button button_reportsAutoSaveFullPath;
        internal System.Windows.Forms.CheckBox checkBox_reportsAutoSave;
        internal System.Windows.Forms.CheckBox checkBox_reportsCreateMonthlySubFolders;
        internal System.Windows.Forms.TextBox textBox_reportsAutoSaveFullPath;
        private System.Windows.Forms.LinkLabel linkLabel_reports_viewFolder;
        internal System.Windows.Forms.ComboBox comboBox_segments_match_value_updated;
        internal System.Windows.Forms.ComboBox comboBox_segments_match_value_original;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button_javaExecutablePath;
        internal System.Windows.Forms.TextBox textBox_javaExecutablePath;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.LinkLabel javaWebsiteLinkLabel;
        internal System.Windows.Forms.ComboBox tagVisualizationComboBox;
        private System.Windows.Forms.Label label16;

    }
}