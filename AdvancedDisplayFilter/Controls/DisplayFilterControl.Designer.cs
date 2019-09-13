using System.Windows.Forms;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
    partial class DisplayFilterControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayFilterControl));
			this.panel_body = new System.Windows.Forms.Panel();
			this.imageList_filter = new System.Windows.Forms.ImageList(this.components);
			this.splitter_filterExpression = new System.Windows.Forms.Splitter();
			this.panel_buffer = new System.Windows.Forms.Panel();
			this.toolStrip_filter = new System.Windows.Forms.ToolStrip();
			this.toolStripButton_applyFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_clearFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_saveFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_loadFilter = new System.Windows.Forms.ToolStripButton();
			this.reverseBtn = new System.Windows.Forms.ToolStripButton();
			this.generateXliff = new System.Windows.Forms.ToolStripButton();
			this.helpButton = new System.Windows.Forms.ToolStripButton();
			this.panel_filterExpression = new System.Windows.Forms.Panel();
			this.panel_filterStatus = new System.Windows.Forms.Panel();
			this.panel_filterStatusBar = new System.Windows.Forms.Panel();
			this.panel_filterStatusBarMessage = new System.Windows.Forms.Panel();
			this.label_filterStatusBarMessage = new System.Windows.Forms.Label();
			this.panel_filterStatusBarImage = new System.Windows.Forms.Panel();
			this.pictureBox_filterStatusBarImage = new System.Windows.Forms.PictureBox();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
			this.tags_tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.content_toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.clearHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.tabPage_Colors = new System.Windows.Forms.TabPage();
			this.colorLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.colorsListView = new System.Windows.Forms.ListView();
			this.label6 = new System.Windows.Forms.Label();
			this.tabPage_segmentNumbers = new System.Windows.Forms.TabPage();
			this.panel_spacer_05 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.evenBtn = new System.Windows.Forms.RadioButton();
			this.oddBtn = new System.Windows.Forms.RadioButton();
			this.groupedBtn = new System.Windows.Forms.RadioButton();
			this.noneBtn = new System.Windows.Forms.RadioButton();
			this.panel7 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.segmentsBox = new System.Windows.Forms.TextBox();
			this.panel_spacer_03 = new System.Windows.Forms.Panel();
			this.segmentsGroupBox = new System.Windows.Forms.GroupBox();
			this.equalsCaseSensitive = new System.Windows.Forms.CheckBox();
			this.sourceSameBox = new System.Windows.Forms.CheckBox();
			this.splitCheckBox = new System.Windows.Forms.CheckBox();
			this.mergedCheckbox = new System.Windows.Forms.CheckBox();
			this.mergedAcross = new System.Windows.Forms.CheckBox();
			this.containsTagsCheckBox = new System.Windows.Forms.CheckBox();
			this.panel_spacer_02 = new System.Windows.Forms.Panel();
			this.userBox = new System.Windows.Forms.GroupBox();
			this.userLabel = new System.Windows.Forms.Label();
			this.modifiedByCheck = new System.Windows.Forms.CheckBox();
			this.modifiedByBox = new System.Windows.Forms.TextBox();
			this.createdByCheck = new System.Windows.Forms.CheckBox();
			this.createdByBox = new System.Windows.Forms.TextBox();
			this.panel_spacer_01 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.fuzzyMax = new System.Windows.Forms.TextBox();
			this.fuzzyMin = new System.Windows.Forms.TextBox();
			this.tabPage_contextInfo = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.label_contextInfoSelected = new System.Windows.Forms.Label();
			this.linkLabel_contextInfoClearSelection = new System.Windows.Forms.LinkLabel();
			this.listView_contextInfo = new System.Windows.Forms.ListView();
			this.columnHeader_code = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label_documentStructure_message = new System.Windows.Forms.Label();
			this.tabPage_comments = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.commentRegexBox = new System.Windows.Forms.CheckBox();
			this.label_tab_comments_filter_message = new System.Windows.Forms.Label();
			this.label_commentAuthor = new System.Windows.Forms.Label();
			this.textBox_commentAuthor = new System.Windows.Forms.TextBox();
			this.textBox_commentText = new System.Windows.Forms.TextBox();
			this.label_commentSeverity = new System.Windows.Forms.Label();
			this.label_commentText = new System.Windows.Forms.Label();
			this.comboBox_commentSeverity = new System.Windows.Forms.ComboBox();
			this.tabPage_filters = new System.Windows.Forms.TabPage();
			this.panel8 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.panel11 = new System.Windows.Forms.Panel();
			this.panel4 = new System.Windows.Forms.Panel();
			this.label_fitlers_available = new System.Windows.Forms.Label();
			this.panel10 = new System.Windows.Forms.Panel();
			this.panel5 = new System.Windows.Forms.Panel();
			this.label_filters_selected = new System.Windows.Forms.Label();
			this.panel9 = new System.Windows.Forms.Panel();
			this.button_add = new System.Windows.Forms.Button();
			this.button_remove = new System.Windows.Forms.Button();
			this.button_removeAll = new System.Windows.Forms.Button();
			this.label_tab_filter_message = new System.Windows.Forms.Label();
			this.tabPage_content = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.checkBox_useBackReferences = new System.Windows.Forms.CheckBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel3 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel6 = new System.Windows.Forms.Panel();
			this.comboBox_SourceTargetFilterLogicalOperator = new System.Windows.Forms.ComboBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.content_groupBox = new System.Windows.Forms.GroupBox();
			this.onlyTags_radioButton = new System.Windows.Forms.RadioButton();
			this.alsoTags_radioButton = new System.Windows.Forms.RadioButton();
			this.checkBox_TagContent = new System.Windows.Forms.CheckBox();
			this.stringId_textbox = new System.Windows.Forms.TextBox();
			this.label_dsiLocation = new System.Windows.Forms.Label();
			this.label_tab_content_filter_message = new System.Windows.Forms.Label();
			this.checkBox_regularExpression = new System.Windows.Forms.CheckBox();
			this.target_textbox = new System.Windows.Forms.TextBox();
			this.label_target = new System.Windows.Forms.Label();
			this.checkBox_caseSensitive = new System.Windows.Forms.CheckBox();
			this.textBox_source = new System.Windows.Forms.TextBox();
			this.label_source = new System.Windows.Forms.Label();
			this.tabControl_filter = new System.Windows.Forms.TabControl();
			this.applyHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.filterExpressionControl = new Sdl.Community.AdvancedDisplayFilter.Controls.FilterExpression();
			this.listView_available = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
			this.columnHeader_filtersAvailable_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listView_selected = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
			this.columnHeader_filtersSelected_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel_body.SuspendLayout();
			this.toolStrip_filter.SuspendLayout();
			this.panel_filterExpression.SuspendLayout();
			this.panel_filterStatus.SuspendLayout();
			this.panel_filterStatusBar.SuspendLayout();
			this.panel_filterStatusBarMessage.SuspendLayout();
			this.panel_filterStatusBarImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_filterStatusBarImage)).BeginInit();
			this.tabPage_Colors.SuspendLayout();
			this.colorLayoutPanel.SuspendLayout();
			this.tabPage_segmentNumbers.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel7.SuspendLayout();
			this.segmentsGroupBox.SuspendLayout();
			this.userBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tabPage_contextInfo.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tabPage_comments.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tabPage_filters.SuspendLayout();
			this.panel8.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel11.SuspendLayout();
			this.panel4.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel9.SuspendLayout();
			this.tabPage_content.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel6.SuspendLayout();
			this.panel1.SuspendLayout();
			this.content_groupBox.SuspendLayout();
			this.tabControl_filter.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel_body
			// 
			this.panel_body.Controls.Add(this.tabControl_filter);
			this.panel_body.Controls.Add(this.splitter_filterExpression);
			this.panel_body.Controls.Add(this.panel_buffer);
			this.panel_body.Controls.Add(this.toolStrip_filter);
			this.panel_body.Controls.Add(this.panel_filterExpression);
			this.panel_body.Controls.Add(this.panel_filterStatus);
			resources.ApplyResources(this.panel_body, "panel_body");
			this.panel_body.Name = "panel_body";
			// 
			// imageList_filter
			// 
			this.imageList_filter.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_filter.ImageStream")));
			this.imageList_filter.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList_filter.Images.SetKeyName(0, "ok.32x32.png");
			this.imageList_filter.Images.SetKeyName(1, "pencil.32x32.png");
			// 
			// splitter_filterExpression
			// 
			resources.ApplyResources(this.splitter_filterExpression, "splitter_filterExpression");
			this.splitter_filterExpression.Name = "splitter_filterExpression";
			this.splitter_filterExpression.TabStop = false;
			// 
			// panel_buffer
			// 
			resources.ApplyResources(this.panel_buffer, "panel_buffer");
			this.panel_buffer.Name = "panel_buffer";
			// 
			// toolStrip_filter
			// 
			this.toolStrip_filter.BackColor = System.Drawing.Color.Transparent;
			this.toolStrip_filter.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
			this.toolStrip_filter.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip_filter.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip_filter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_applyFilter,
            this.reverseBtn,
            this.toolStripButton_clearFilter,
            this.toolStripSeparator1,
            this.toolStripDropDownButton1,
            this.generateXliff,
            this.toolStripSeparator3,
            this.toolStripButton_saveFilter,
            this.toolStripButton_loadFilter,
            this.toolStripSeparator2,
            this.helpButton});
			resources.ApplyResources(this.toolStrip_filter, "toolStrip_filter");
			this.toolStrip_filter.Name = "toolStrip_filter";
			// 
			// toolStripButton_applyFilter
			// 
			resources.ApplyResources(this.toolStripButton_applyFilter, "toolStripButton_applyFilter");
			this.toolStripButton_applyFilter.Name = "toolStripButton_applyFilter";
			this.toolStripButton_applyFilter.Click += new System.EventHandler(this.toolStripButton_applyFilter_Click);
			// 
			// toolStripButton_clearFilter
			// 
			resources.ApplyResources(this.toolStripButton_clearFilter, "toolStripButton_clearFilter");
			this.toolStripButton_clearFilter.Name = "toolStripButton_clearFilter";
			this.toolStripButton_clearFilter.Click += new System.EventHandler(this.toolStripButton_clearFilter_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// toolStripButton_saveFilter
			// 
			resources.ApplyResources(this.toolStripButton_saveFilter, "toolStripButton_saveFilter");
			this.toolStripButton_saveFilter.Name = "toolStripButton_saveFilter";
			this.toolStripButton_saveFilter.Click += new System.EventHandler(this.toolStripButton_saveFilter_Click);
			// 
			// toolStripButton_loadFilter
			// 
			resources.ApplyResources(this.toolStripButton_loadFilter, "toolStripButton_loadFilter");
			this.toolStripButton_loadFilter.Name = "toolStripButton_loadFilter";
			this.toolStripButton_loadFilter.Click += new System.EventHandler(this.toolStripButton_loadFilter_Click);
			// 
			// reverseBtn
			// 
			resources.ApplyResources(this.reverseBtn, "reverseBtn");
			this.reverseBtn.Name = "reverseBtn";
			this.reverseBtn.Click += new System.EventHandler(this.reverseBtn_Click);
			// 
			// generateXliff
			// 
			resources.ApplyResources(this.generateXliff, "generateXliff");
			this.generateXliff.Name = "generateXliff";
			this.generateXliff.Click += new System.EventHandler(this.generateXliff_Click);
			// 
			// helpButton
			// 
			resources.ApplyResources(this.helpButton, "helpButton");
			this.helpButton.Name = "helpButton";
			this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
			// 
			// panel_filterExpression
			// 
			this.panel_filterExpression.Controls.Add(this.filterExpressionControl);
			resources.ApplyResources(this.panel_filterExpression, "panel_filterExpression");
			this.panel_filterExpression.Name = "panel_filterExpression";
			// 
			// panel_filterStatus
			// 
			this.panel_filterStatus.BackColor = System.Drawing.Color.Transparent;
			this.panel_filterStatus.Controls.Add(this.panel_filterStatusBar);
			resources.ApplyResources(this.panel_filterStatus, "panel_filterStatus");
			this.panel_filterStatus.Name = "panel_filterStatus";
			// 
			// panel_filterStatusBar
			// 
			this.panel_filterStatusBar.BackColor = System.Drawing.Color.Transparent;
			this.panel_filterStatusBar.Controls.Add(this.panel_filterStatusBarMessage);
			this.panel_filterStatusBar.Controls.Add(this.panel_filterStatusBarImage);
			resources.ApplyResources(this.panel_filterStatusBar, "panel_filterStatusBar");
			this.panel_filterStatusBar.Name = "panel_filterStatusBar";
			// 
			// panel_filterStatusBarMessage
			// 
			this.panel_filterStatusBarMessage.Controls.Add(this.label_filterStatusBarMessage);
			resources.ApplyResources(this.panel_filterStatusBarMessage, "panel_filterStatusBarMessage");
			this.panel_filterStatusBarMessage.Name = "panel_filterStatusBarMessage";
			// 
			// label_filterStatusBarMessage
			// 
			this.label_filterStatusBarMessage.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.label_filterStatusBarMessage, "label_filterStatusBarMessage");
			this.label_filterStatusBarMessage.Name = "label_filterStatusBarMessage";
			// 
			// panel_filterStatusBarImage
			// 
			this.panel_filterStatusBarImage.BackColor = System.Drawing.Color.Transparent;
			this.panel_filterStatusBarImage.Controls.Add(this.pictureBox_filterStatusBarImage);
			resources.ApplyResources(this.panel_filterStatusBarImage, "panel_filterStatusBarImage");
			this.panel_filterStatusBarImage.Name = "panel_filterStatusBarImage";
			// 
			// pictureBox_filterStatusBarImage
			// 
			this.pictureBox_filterStatusBarImage.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.pictureBox_filterStatusBarImage, "pictureBox_filterStatusBarImage");
			this.pictureBox_filterStatusBarImage.Name = "pictureBox_filterStatusBarImage";
			this.pictureBox_filterStatusBarImage.TabStop = false;
			// 
			// columnHeader1
			// 
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			// 
			// columnHeader2
			// 
			resources.ApplyResources(this.columnHeader2, "columnHeader2");
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyHighlightingToolStripMenuItem,
            this.clearHighlightingToolStripMenuItem});
			resources.ApplyResources(this.toolStripDropDownButton1, "toolStripDropDownButton1");
			this.toolStripDropDownButton1.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			// 
			// clearHighlightingToolStripMenuItem
			// 
			resources.ApplyResources(this.clearHighlightingToolStripMenuItem, "clearHighlightingToolStripMenuItem");
			this.clearHighlightingToolStripMenuItem.Name = "clearHighlightingToolStripMenuItem";
			this.clearHighlightingToolStripMenuItem.Click += new System.EventHandler(this.clearHighlightingToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// tabPage_Colors
			// 
			this.tabPage_Colors.Controls.Add(this.colorLayoutPanel);
			resources.ApplyResources(this.tabPage_Colors, "tabPage_Colors");
			this.tabPage_Colors.Name = "tabPage_Colors";
			this.tabPage_Colors.UseVisualStyleBackColor = true;
			// 
			// colorLayoutPanel
			// 
			resources.ApplyResources(this.colorLayoutPanel, "colorLayoutPanel");
			this.colorLayoutPanel.Controls.Add(this.label6, 0, 0);
			this.colorLayoutPanel.Controls.Add(this.colorsListView, 0, 1);
			this.colorLayoutPanel.Name = "colorLayoutPanel";
			// 
			// colorsListView
			// 
			this.colorsListView.CheckBoxes = true;
			resources.ApplyResources(this.colorsListView, "colorsListView");
			this.colorsListView.Name = "colorsListView";
			this.colorsListView.UseCompatibleStateImageBehavior = false;
			this.colorsListView.SelectedIndexChanged += new System.EventHandler(this.colorsListView_SelectedIndexChanged);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// tabPage_segmentNumbers
			// 
			resources.ApplyResources(this.tabPage_segmentNumbers, "tabPage_segmentNumbers");
			this.tabPage_segmentNumbers.Controls.Add(this.groupBox2);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_01);
			this.tabPage_segmentNumbers.Controls.Add(this.userBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_02);
			this.tabPage_segmentNumbers.Controls.Add(this.segmentsGroupBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_03);
			this.tabPage_segmentNumbers.Controls.Add(this.groupBox1);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_05);
			this.tabPage_segmentNumbers.Name = "tabPage_segmentNumbers";
			this.tabPage_segmentNumbers.UseVisualStyleBackColor = true;
			// 
			// panel_spacer_05
			// 
			resources.ApplyResources(this.panel_spacer_05, "panel_spacer_05");
			this.panel_spacer_05.Name = "panel_spacer_05";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.panel7);
			this.groupBox1.Controls.Add(this.noneBtn);
			this.groupBox1.Controls.Add(this.groupedBtn);
			this.groupBox1.Controls.Add(this.oddBtn);
			this.groupBox1.Controls.Add(this.evenBtn);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// evenBtn
			// 
			resources.ApplyResources(this.evenBtn, "evenBtn");
			this.evenBtn.Name = "evenBtn";
			this.evenBtn.TabStop = true;
			this.evenBtn.UseVisualStyleBackColor = true;
			this.evenBtn.CheckedChanged += new System.EventHandler(this.evenBtn_CheckedChanged);
			// 
			// oddBtn
			// 
			resources.ApplyResources(this.oddBtn, "oddBtn");
			this.oddBtn.Name = "oddBtn";
			this.oddBtn.TabStop = true;
			this.oddBtn.UseVisualStyleBackColor = true;
			this.oddBtn.CheckedChanged += new System.EventHandler(this.oddBtn_CheckedChanged);
			// 
			// groupedBtn
			// 
			resources.ApplyResources(this.groupedBtn, "groupedBtn");
			this.groupedBtn.Name = "groupedBtn";
			this.groupedBtn.TabStop = true;
			this.groupedBtn.UseVisualStyleBackColor = true;
			this.groupedBtn.CheckedChanged += new System.EventHandler(this.groupedBtn_CheckedChanged);
			// 
			// noneBtn
			// 
			resources.ApplyResources(this.noneBtn, "noneBtn");
			this.noneBtn.Name = "noneBtn";
			this.noneBtn.TabStop = true;
			this.noneBtn.UseVisualStyleBackColor = true;
			this.noneBtn.CheckedChanged += new System.EventHandler(this.noneBtn_CheckedChanged);
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.segmentsBox);
			this.panel7.Controls.Add(this.label1);
			resources.ApplyResources(this.panel7, "panel7");
			this.panel7.Name = "panel7";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Name = "label1";
			// 
			// segmentsBox
			// 
			resources.ApplyResources(this.segmentsBox, "segmentsBox");
			this.segmentsBox.Name = "segmentsBox";
			// 
			// panel_spacer_03
			// 
			resources.ApplyResources(this.panel_spacer_03, "panel_spacer_03");
			this.panel_spacer_03.Name = "panel_spacer_03";
			// 
			// segmentsGroupBox
			// 
			this.segmentsGroupBox.Controls.Add(this.containsTagsCheckBox);
			this.segmentsGroupBox.Controls.Add(this.mergedAcross);
			this.segmentsGroupBox.Controls.Add(this.mergedCheckbox);
			this.segmentsGroupBox.Controls.Add(this.splitCheckBox);
			this.segmentsGroupBox.Controls.Add(this.sourceSameBox);
			this.segmentsGroupBox.Controls.Add(this.equalsCaseSensitive);
			resources.ApplyResources(this.segmentsGroupBox, "segmentsGroupBox");
			this.segmentsGroupBox.Name = "segmentsGroupBox";
			this.segmentsGroupBox.TabStop = false;
			// 
			// equalsCaseSensitive
			// 
			resources.ApplyResources(this.equalsCaseSensitive, "equalsCaseSensitive");
			this.equalsCaseSensitive.Name = "equalsCaseSensitive";
			this.equalsCaseSensitive.UseVisualStyleBackColor = true;
			this.equalsCaseSensitive.CheckedChanged += new System.EventHandler(this.equalsCaseSensitive_CheckedChanged);
			// 
			// sourceSameBox
			// 
			resources.ApplyResources(this.sourceSameBox, "sourceSameBox");
			this.sourceSameBox.Name = "sourceSameBox";
			this.sourceSameBox.UseVisualStyleBackColor = true;
			this.sourceSameBox.CheckedChanged += new System.EventHandler(this.sourceSameBox_CheckedChanged);
			// 
			// splitCheckBox
			// 
			resources.ApplyResources(this.splitCheckBox, "splitCheckBox");
			this.splitCheckBox.Name = "splitCheckBox";
			this.splitCheckBox.UseVisualStyleBackColor = true;
			this.splitCheckBox.CheckedChanged += new System.EventHandler(this.splitCheckBox_CheckedChanged);
			// 
			// mergedCheckbox
			// 
			resources.ApplyResources(this.mergedCheckbox, "mergedCheckbox");
			this.mergedCheckbox.Name = "mergedCheckbox";
			this.mergedCheckbox.UseVisualStyleBackColor = true;
			this.mergedCheckbox.CheckedChanged += new System.EventHandler(this.mergedCheckbox_CheckedChanged);
			// 
			// mergedAcross
			// 
			resources.ApplyResources(this.mergedAcross, "mergedAcross");
			this.mergedAcross.Name = "mergedAcross";
			this.mergedAcross.UseVisualStyleBackColor = true;
			this.mergedAcross.CheckedChanged += new System.EventHandler(this.mergedAcross_CheckedChanged);
			// 
			// containsTagsCheckBox
			// 
			resources.ApplyResources(this.containsTagsCheckBox, "containsTagsCheckBox");
			this.containsTagsCheckBox.Name = "containsTagsCheckBox";
			this.containsTagsCheckBox.UseVisualStyleBackColor = true;
			this.containsTagsCheckBox.CheckedChanged += new System.EventHandler(this.containsTagsCheckBox_CheckedChanged);
			// 
			// panel_spacer_02
			// 
			resources.ApplyResources(this.panel_spacer_02, "panel_spacer_02");
			this.panel_spacer_02.Name = "panel_spacer_02";
			// 
			// userBox
			// 
			this.userBox.Controls.Add(this.createdByBox);
			this.userBox.Controls.Add(this.createdByCheck);
			this.userBox.Controls.Add(this.modifiedByBox);
			this.userBox.Controls.Add(this.modifiedByCheck);
			this.userBox.Controls.Add(this.userLabel);
			resources.ApplyResources(this.userBox, "userBox");
			this.userBox.Name = "userBox";
			this.userBox.TabStop = false;
			// 
			// userLabel
			// 
			resources.ApplyResources(this.userLabel, "userLabel");
			this.userLabel.Name = "userLabel";
			// 
			// modifiedByCheck
			// 
			resources.ApplyResources(this.modifiedByCheck, "modifiedByCheck");
			this.modifiedByCheck.Name = "modifiedByCheck";
			this.modifiedByCheck.UseVisualStyleBackColor = true;
			this.modifiedByCheck.CheckedChanged += new System.EventHandler(this.modifiedByCheck_CheckedChanged);
			// 
			// modifiedByBox
			// 
			resources.ApplyResources(this.modifiedByBox, "modifiedByBox");
			this.modifiedByBox.Name = "modifiedByBox";
			// 
			// createdByCheck
			// 
			resources.ApplyResources(this.createdByCheck, "createdByCheck");
			this.createdByCheck.Name = "createdByCheck";
			this.createdByCheck.UseVisualStyleBackColor = true;
			this.createdByCheck.CheckedChanged += new System.EventHandler(this.createdByCheck_CheckedChanged);
			// 
			// createdByBox
			// 
			resources.ApplyResources(this.createdByBox, "createdByBox");
			this.createdByBox.Name = "createdByBox";
			// 
			// panel_spacer_01
			// 
			resources.ApplyResources(this.panel_spacer_01, "panel_spacer_01");
			this.panel_spacer_01.Name = "panel_spacer_01";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.fuzzyMin);
			this.groupBox2.Controls.Add(this.fuzzyMax);
			this.groupBox2.Controls.Add(this.label5);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// fuzzyMax
			// 
			resources.ApplyResources(this.fuzzyMax, "fuzzyMax");
			this.fuzzyMax.Name = "fuzzyMax";
			this.fuzzyMax.TextChanged += new System.EventHandler(this.fuzzyMax_TextChanged);
			// 
			// fuzzyMin
			// 
			resources.ApplyResources(this.fuzzyMin, "fuzzyMin");
			this.fuzzyMin.Name = "fuzzyMin";
			this.fuzzyMin.TextChanged += new System.EventHandler(this.fuzzyMin_TextChanged);
			// 
			// tabPage_contextInfo
			// 
			this.tabPage_contextInfo.Controls.Add(this.tableLayoutPanel4);
			resources.ApplyResources(this.tabPage_contextInfo, "tabPage_contextInfo");
			this.tabPage_contextInfo.Name = "tabPage_contextInfo";
			this.tabPage_contextInfo.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel4
			// 
			resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
			this.tableLayoutPanel4.Controls.Add(this.label_documentStructure_message, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.listView_contextInfo, 0, 1);
			this.tableLayoutPanel4.Controls.Add(this.linkLabel_contextInfoClearSelection, 0, 2);
			this.tableLayoutPanel4.Controls.Add(this.label_contextInfoSelected, 1, 2);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			// 
			// label_contextInfoSelected
			// 
			resources.ApplyResources(this.label_contextInfoSelected, "label_contextInfoSelected");
			this.label_contextInfoSelected.Name = "label_contextInfoSelected";
			// 
			// linkLabel_contextInfoClearSelection
			// 
			resources.ApplyResources(this.linkLabel_contextInfoClearSelection, "linkLabel_contextInfoClearSelection");
			this.linkLabel_contextInfoClearSelection.Name = "linkLabel_contextInfoClearSelection";
			this.linkLabel_contextInfoClearSelection.TabStop = true;
			this.linkLabel_contextInfoClearSelection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_contextInfoClearSelection_LinkClicked);
			// 
			// listView_contextInfo
			// 
			resources.ApplyResources(this.listView_contextInfo, "listView_contextInfo");
			this.listView_contextInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_code,
            this.columnHeader_name,
            this.columnHeader_description});
			this.tableLayoutPanel4.SetColumnSpan(this.listView_contextInfo, 2);
			this.listView_contextInfo.FullRowSelect = true;
			this.listView_contextInfo.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView_contextInfo.HideSelection = false;
			this.listView_contextInfo.Name = "listView_contextInfo";
			this.listView_contextInfo.UseCompatibleStateImageBehavior = false;
			this.listView_contextInfo.View = System.Windows.Forms.View.Details;
			this.listView_contextInfo.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_contextInfo_ItemSelectionChanged);
			this.listView_contextInfo.Resize += new System.EventHandler(this.listView_contextInfo_Resize);
			// 
			// columnHeader_code
			// 
			resources.ApplyResources(this.columnHeader_code, "columnHeader_code");
			// 
			// columnHeader_name
			// 
			resources.ApplyResources(this.columnHeader_name, "columnHeader_name");
			// 
			// columnHeader_description
			// 
			resources.ApplyResources(this.columnHeader_description, "columnHeader_description");
			// 
			// label_documentStructure_message
			// 
			resources.ApplyResources(this.label_documentStructure_message, "label_documentStructure_message");
			this.tableLayoutPanel4.SetColumnSpan(this.label_documentStructure_message, 2);
			this.label_documentStructure_message.Name = "label_documentStructure_message";
			// 
			// tabPage_comments
			// 
			this.tabPage_comments.Controls.Add(this.tableLayoutPanel3);
			resources.ApplyResources(this.tabPage_comments, "tabPage_comments");
			this.tabPage_comments.Name = "tabPage_comments";
			this.tabPage_comments.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel3
			// 
			resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
			this.tableLayoutPanel3.Controls.Add(this.comboBox_commentSeverity, 1, 3);
			this.tableLayoutPanel3.Controls.Add(this.label_commentText, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.label_commentSeverity, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.textBox_commentText, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.textBox_commentAuthor, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this.label_commentAuthor, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.label_tab_comments_filter_message, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.commentRegexBox, 0, 4);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			// 
			// commentRegexBox
			// 
			resources.ApplyResources(this.commentRegexBox, "commentRegexBox");
			this.tableLayoutPanel3.SetColumnSpan(this.commentRegexBox, 2);
			this.commentRegexBox.Name = "commentRegexBox";
			this.commentRegexBox.UseVisualStyleBackColor = true;
			this.commentRegexBox.CheckedChanged += new System.EventHandler(this.commentRegexBox_CheckedChanged);
			// 
			// label_tab_comments_filter_message
			// 
			resources.ApplyResources(this.label_tab_comments_filter_message, "label_tab_comments_filter_message");
			this.tableLayoutPanel3.SetColumnSpan(this.label_tab_comments_filter_message, 2);
			this.label_tab_comments_filter_message.Name = "label_tab_comments_filter_message";
			// 
			// label_commentAuthor
			// 
			resources.ApplyResources(this.label_commentAuthor, "label_commentAuthor");
			this.label_commentAuthor.Name = "label_commentAuthor";
			// 
			// textBox_commentAuthor
			// 
			resources.ApplyResources(this.textBox_commentAuthor, "textBox_commentAuthor");
			this.textBox_commentAuthor.Name = "textBox_commentAuthor";
			this.textBox_commentAuthor.TextChanged += new System.EventHandler(this.textBox_commentAuthor_TextChanged);
			this.textBox_commentAuthor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_commentAuthor_KeyUp);
			// 
			// textBox_commentText
			// 
			resources.ApplyResources(this.textBox_commentText, "textBox_commentText");
			this.textBox_commentText.Name = "textBox_commentText";
			this.textBox_commentText.TextChanged += new System.EventHandler(this.textBox_commentText_TextChanged);
			this.textBox_commentText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_commentText_KeyUp);
			// 
			// label_commentSeverity
			// 
			resources.ApplyResources(this.label_commentSeverity, "label_commentSeverity");
			this.label_commentSeverity.Name = "label_commentSeverity";
			// 
			// label_commentText
			// 
			resources.ApplyResources(this.label_commentText, "label_commentText");
			this.label_commentText.Name = "label_commentText";
			// 
			// comboBox_commentSeverity
			// 
			resources.ApplyResources(this.comboBox_commentSeverity, "comboBox_commentSeverity");
			this.comboBox_commentSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_commentSeverity.FormattingEnabled = true;
			this.comboBox_commentSeverity.Name = "comboBox_commentSeverity";
			this.comboBox_commentSeverity.SelectedIndexChanged += new System.EventHandler(this.comboBox_commentSeverity_SelectedIndexChanged);
			// 
			// tabPage_filters
			// 
			this.tabPage_filters.Controls.Add(this.panel8);
			resources.ApplyResources(this.tabPage_filters, "tabPage_filters");
			this.tabPage_filters.Name = "tabPage_filters";
			this.tabPage_filters.UseVisualStyleBackColor = true;
			// 
			// panel8
			// 
			this.panel8.Controls.Add(this.tableLayoutPanel2);
			resources.ApplyResources(this.panel8, "panel8");
			this.panel8.Name = "panel8";
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this.label_tab_filter_message, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.panel9, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.panel10, 2, 1);
			this.tableLayoutPanel2.Controls.Add(this.panel11, 0, 1);
			this.tableLayoutPanel2.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// panel11
			// 
			this.panel11.Controls.Add(this.listView_available);
			this.panel11.Controls.Add(this.panel4);
			resources.ApplyResources(this.panel11, "panel11");
			this.panel11.Name = "panel11";
			// 
			// panel4
			// 
			this.panel4.Controls.Add(this.label_fitlers_available);
			resources.ApplyResources(this.panel4, "panel4");
			this.panel4.Name = "panel4";
			// 
			// label_fitlers_available
			// 
			resources.ApplyResources(this.label_fitlers_available, "label_fitlers_available");
			this.label_fitlers_available.Name = "label_fitlers_available";
			// 
			// panel10
			// 
			this.panel10.Controls.Add(this.listView_selected);
			this.panel10.Controls.Add(this.panel5);
			resources.ApplyResources(this.panel10, "panel10");
			this.panel10.Name = "panel10";
			// 
			// panel5
			// 
			this.panel5.Controls.Add(this.label_filters_selected);
			resources.ApplyResources(this.panel5, "panel5");
			this.panel5.Name = "panel5";
			// 
			// label_filters_selected
			// 
			resources.ApplyResources(this.label_filters_selected, "label_filters_selected");
			this.label_filters_selected.Name = "label_filters_selected";
			// 
			// panel9
			// 
			this.panel9.Controls.Add(this.button_removeAll);
			this.panel9.Controls.Add(this.button_remove);
			this.panel9.Controls.Add(this.button_add);
			resources.ApplyResources(this.panel9, "panel9");
			this.panel9.Name = "panel9";
			// 
			// button_add
			// 
			resources.ApplyResources(this.button_add, "button_add");
			this.button_add.Name = "button_add";
			this.button_add.UseVisualStyleBackColor = true;
			this.button_add.Click += new System.EventHandler(this.button_add_Click);
			// 
			// button_remove
			// 
			resources.ApplyResources(this.button_remove, "button_remove");
			this.button_remove.Name = "button_remove";
			this.button_remove.UseVisualStyleBackColor = true;
			this.button_remove.Click += new System.EventHandler(this.button_remove_Click);
			// 
			// button_removeAll
			// 
			resources.ApplyResources(this.button_removeAll, "button_removeAll");
			this.button_removeAll.Name = "button_removeAll";
			this.button_removeAll.UseVisualStyleBackColor = true;
			this.button_removeAll.Click += new System.EventHandler(this.button_removeAll_Click);
			// 
			// label_tab_filter_message
			// 
			resources.ApplyResources(this.label_tab_filter_message, "label_tab_filter_message");
			this.tableLayoutPanel2.SetColumnSpan(this.label_tab_filter_message, 3);
			this.label_tab_filter_message.Name = "label_tab_filter_message";
			// 
			// tabPage_content
			// 
			this.tabPage_content.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this.tabPage_content, "tabPage_content");
			this.tabPage_content.Name = "tabPage_content";
			this.tabPage_content.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.label_source, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.textBox_source, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.checkBox_caseSensitive, 0, 6);
			this.tableLayoutPanel1.Controls.Add(this.label_target, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.target_textbox, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.checkBox_regularExpression, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.label_tab_content_filter_message, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.label_dsiLocation, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.stringId_textbox, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.checkBox_useBackReferences, 0, 5);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// checkBox_useBackReferences
			// 
			resources.ApplyResources(this.checkBox_useBackReferences, "checkBox_useBackReferences");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_useBackReferences, 3);
			this.checkBox_useBackReferences.Name = "checkBox_useBackReferences";
			this.checkBox_useBackReferences.UseVisualStyleBackColor = true;
			this.checkBox_useBackReferences.CheckedChanged += new System.EventHandler(this.checkBox_useBackReferences_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.panel6);
			this.panel2.Controls.Add(this.panel3);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.pictureBox1);
			resources.ApplyResources(this.panel3, "panel3");
			this.panel3.Name = "panel3";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.comboBox_SourceTargetFilterLogicalOperator);
			resources.ApplyResources(this.panel6, "panel6");
			this.panel6.Name = "panel6";
			// 
			// comboBox_SourceTargetFilterLogicalOperator
			// 
			this.comboBox_SourceTargetFilterLogicalOperator.BackColor = System.Drawing.SystemColors.Window;
			resources.ApplyResources(this.comboBox_SourceTargetFilterLogicalOperator, "comboBox_SourceTargetFilterLogicalOperator");
			this.comboBox_SourceTargetFilterLogicalOperator.FormattingEnabled = true;
			this.comboBox_SourceTargetFilterLogicalOperator.Items.AddRange(new object[] {
            resources.GetString("comboBox_SourceTargetFilterLogicalOperator.Items"),
            resources.GetString("comboBox_SourceTargetFilterLogicalOperator.Items1")});
			this.comboBox_SourceTargetFilterLogicalOperator.Name = "comboBox_SourceTargetFilterLogicalOperator";
			this.comboBox_SourceTargetFilterLogicalOperator.SelectedIndexChanged += new System.EventHandler(this.comboBox_SourceTargetFilterLogicalOperator_SelectedIndexChanged);
			this.comboBox_SourceTargetFilterLogicalOperator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_SourceTargetFilterLogicalOperator_KeyPress);
			// 
			// panel1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
			this.panel1.Controls.Add(this.checkBox_TagContent);
			this.panel1.Controls.Add(this.content_groupBox);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// content_groupBox
			// 
			this.content_groupBox.Controls.Add(this.alsoTags_radioButton);
			this.content_groupBox.Controls.Add(this.onlyTags_radioButton);
			resources.ApplyResources(this.content_groupBox, "content_groupBox");
			this.content_groupBox.Name = "content_groupBox";
			this.content_groupBox.TabStop = false;
			// 
			// onlyTags_radioButton
			// 
			resources.ApplyResources(this.onlyTags_radioButton, "onlyTags_radioButton");
			this.onlyTags_radioButton.Name = "onlyTags_radioButton";
			this.tags_tooltip.SetToolTip(this.onlyTags_radioButton, global::Sdl.Community.AdvancedDisplayFilter.StringResources.DisplayFilter_OnlyTags_Tooltip);
			this.onlyTags_radioButton.UseVisualStyleBackColor = true;
			this.onlyTags_radioButton.CheckedChanged += new System.EventHandler(this.onlyTags_radioButton_CheckedChanged);
			// 
			// alsoTags_radioButton
			// 
			resources.ApplyResources(this.alsoTags_radioButton, "alsoTags_radioButton");
			this.alsoTags_radioButton.Checked = true;
			this.alsoTags_radioButton.Name = "alsoTags_radioButton";
			this.alsoTags_radioButton.TabStop = true;
			this.tags_tooltip.SetToolTip(this.alsoTags_radioButton, global::Sdl.Community.AdvancedDisplayFilter.StringResources.DisplayFilter_IncludeTags_Tooltip);
			this.alsoTags_radioButton.UseVisualStyleBackColor = true;
			this.alsoTags_radioButton.CheckedChanged += new System.EventHandler(this.alsoTags_radioButton_CheckedChanged);
			// 
			// checkBox_TagContent
			// 
			resources.ApplyResources(this.checkBox_TagContent, "checkBox_TagContent");
			this.checkBox_TagContent.Name = "checkBox_TagContent";
			this.checkBox_TagContent.UseVisualStyleBackColor = true;
			this.checkBox_TagContent.CheckedChanged += new System.EventHandler(this.CheckBox_TagContent_CheckedChanged);
			// 
			// stringId_textbox
			// 
			resources.ApplyResources(this.stringId_textbox, "stringId_textbox");
			this.tableLayoutPanel1.SetColumnSpan(this.stringId_textbox, 2);
			this.stringId_textbox.Name = "stringId_textbox";
			this.stringId_textbox.TextChanged += new System.EventHandler(this.stringId_textbox_TextChanged);
			// 
			// label_dsiLocation
			// 
			resources.ApplyResources(this.label_dsiLocation, "label_dsiLocation");
			this.label_dsiLocation.Name = "label_dsiLocation";
			// 
			// label_tab_content_filter_message
			// 
			resources.ApplyResources(this.label_tab_content_filter_message, "label_tab_content_filter_message");
			this.tableLayoutPanel1.SetColumnSpan(this.label_tab_content_filter_message, 3);
			this.label_tab_content_filter_message.Name = "label_tab_content_filter_message";
			// 
			// checkBox_regularExpression
			// 
			resources.ApplyResources(this.checkBox_regularExpression, "checkBox_regularExpression");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_regularExpression, 3);
			this.checkBox_regularExpression.Name = "checkBox_regularExpression";
			this.checkBox_regularExpression.UseVisualStyleBackColor = true;
			this.checkBox_regularExpression.CheckedChanged += new System.EventHandler(this.checkBox_regularExpression_CheckedChanged);
			// 
			// target_textbox
			// 
			resources.ApplyResources(this.target_textbox, "target_textbox");
			this.target_textbox.Name = "target_textbox";
			this.target_textbox.TextChanged += new System.EventHandler(this.textBox_target_TextChanged);
			this.target_textbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_target_KeyUp);
			// 
			// label_target
			// 
			resources.ApplyResources(this.label_target, "label_target");
			this.label_target.Name = "label_target";
			// 
			// checkBox_caseSensitive
			// 
			resources.ApplyResources(this.checkBox_caseSensitive, "checkBox_caseSensitive");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_caseSensitive, 3);
			this.checkBox_caseSensitive.Name = "checkBox_caseSensitive";
			this.checkBox_caseSensitive.UseVisualStyleBackColor = true;
			this.checkBox_caseSensitive.CheckedChanged += new System.EventHandler(this.checkBox_caseSensitive_CheckedChanged);
			// 
			// textBox_source
			// 
			resources.ApplyResources(this.textBox_source, "textBox_source");
			this.textBox_source.Name = "textBox_source";
			this.textBox_source.TextChanged += new System.EventHandler(this.textBox_source_TextChanged);
			this.textBox_source.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_source_KeyUp);
			// 
			// label_source
			// 
			resources.ApplyResources(this.label_source, "label_source");
			this.label_source.Name = "label_source";
			// 
			// tabControl_filter
			// 
			this.tabControl_filter.Controls.Add(this.tabPage_content);
			this.tabControl_filter.Controls.Add(this.tabPage_filters);
			this.tabControl_filter.Controls.Add(this.tabPage_comments);
			this.tabControl_filter.Controls.Add(this.tabPage_contextInfo);
			this.tabControl_filter.Controls.Add(this.tabPage_segmentNumbers);
			this.tabControl_filter.Controls.Add(this.tabPage_Colors);
			resources.ApplyResources(this.tabControl_filter, "tabControl_filter");
			this.tabControl_filter.ImageList = this.imageList_filter;
			this.tabControl_filter.Name = "tabControl_filter";
			this.tabControl_filter.SelectedIndex = 0;
			// 
			// applyHighlightingToolStripMenuItem
			// 
			resources.ApplyResources(this.applyHighlightingToolStripMenuItem, "applyHighlightingToolStripMenuItem");
			this.applyHighlightingToolStripMenuItem.Name = "applyHighlightingToolStripMenuItem";
			this.applyHighlightingToolStripMenuItem.Click += new System.EventHandler(this.applyHighlightingToolStripMenuItem_Click);
			// 
			// filterExpressionControl
			// 
			this.filterExpressionControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.filterExpressionControl, "filterExpressionControl");
			this.filterExpressionControl.Name = "filterExpressionControl";
			// 
			// listView_available
			// 
			this.listView_available.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_filtersAvailable_name});
			resources.ApplyResources(this.listView_available, "listView_available");
			this.listView_available.FullRowSelect = true;
			this.listView_available.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView_available.HideSelection = false;
			this.listView_available.Name = "listView_available";
			this.listView_available.UseCompatibleStateImageBehavior = false;
			this.listView_available.View = System.Windows.Forms.View.Details;
			this.listView_available.SelectedIndexChanged += new System.EventHandler(this.listView_available_SelectedIndexChanged);
			this.listView_available.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_available_MouseDoubleClick);
			this.listView_available.Resize += new System.EventHandler(this.listView_available_Resize);
			// 
			// columnHeader_filtersAvailable_name
			// 
			resources.ApplyResources(this.columnHeader_filtersAvailable_name, "columnHeader_filtersAvailable_name");
			// 
			// listView_selected
			// 
			this.listView_selected.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_filtersSelected_name});
			resources.ApplyResources(this.listView_selected, "listView_selected");
			this.listView_selected.FullRowSelect = true;
			this.listView_selected.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listView_selected.HideSelection = false;
			this.listView_selected.Name = "listView_selected";
			this.listView_selected.UseCompatibleStateImageBehavior = false;
			this.listView_selected.View = System.Windows.Forms.View.Details;
			this.listView_selected.SelectedIndexChanged += new System.EventHandler(this.listView_selected_SelectedIndexChanged);
			this.listView_selected.Resize += new System.EventHandler(this.listView_selected_Resize);
			// 
			// columnHeader_filtersSelected_name
			// 
			resources.ApplyResources(this.columnHeader_filtersSelected_name, "columnHeader_filtersSelected_name");
			// 
			// DisplayFilterControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel_body);
			this.Name = "DisplayFilterControl";
			this.panel_body.ResumeLayout(false);
			this.panel_body.PerformLayout();
			this.toolStrip_filter.ResumeLayout(false);
			this.toolStrip_filter.PerformLayout();
			this.panel_filterExpression.ResumeLayout(false);
			this.panel_filterStatus.ResumeLayout(false);
			this.panel_filterStatusBar.ResumeLayout(false);
			this.panel_filterStatusBarMessage.ResumeLayout(false);
			this.panel_filterStatusBarImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_filterStatusBarImage)).EndInit();
			this.tabPage_Colors.ResumeLayout(false);
			this.colorLayoutPanel.ResumeLayout(false);
			this.tabPage_segmentNumbers.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel7.ResumeLayout(false);
			this.panel7.PerformLayout();
			this.segmentsGroupBox.ResumeLayout(false);
			this.segmentsGroupBox.PerformLayout();
			this.userBox.ResumeLayout(false);
			this.userBox.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tabPage_contextInfo.ResumeLayout(false);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tabPage_comments.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tabPage_filters.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel11.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.panel10.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel9.ResumeLayout(false);
			this.tabPage_content.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel6.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.content_groupBox.ResumeLayout(false);
			this.content_groupBox.PerformLayout();
			this.tabControl_filter.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_body;
        private System.Windows.Forms.Panel panel_filterStatus;
        private System.Windows.Forms.ToolStrip toolStrip_filter;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label_filterStatusBarMessage;
        private System.Windows.Forms.ToolStripButton toolStripButton_applyFilter;
        private System.Windows.Forms.ToolStripButton toolStripButton_clearFilter;
        private System.Windows.Forms.Panel panel_filterStatusBar;
        private System.Windows.Forms.Panel panel_filterStatusBarImage;
        private System.Windows.Forms.PictureBox pictureBox_filterStatusBarImage;
        private System.Windows.Forms.ImageList imageList_filter;
        private System.Windows.Forms.Panel panel_buffer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_saveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButton_loadFilter;
        private System.Windows.Forms.Panel panel_filterExpression;
        private System.Windows.Forms.Splitter splitter_filterExpression;
        private System.Windows.Forms.Panel panel_filterStatusBarMessage;
        private FilterExpression filterExpressionControl;
		private ContextMenuStrip contextMenuStrip1;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private ToolStripButton reverseBtn;
		private ToolStripButton helpButton;
		private ToolStripButton generateXliff;
		private System.ComponentModel.BackgroundWorker backgroundWorker2;
		private ToolTip tags_tooltip;
		private ToolTip content_toolTips;
		private ToolStripDropDownButton toolStripDropDownButton1;
		private ToolStripMenuItem clearHighlightingToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripSeparator toolStripSeparator2;
		private TabControl tabControl_filter;
		private TabPage tabPage_content;
		private TableLayoutPanel tableLayoutPanel1;
		private Label label_source;
		public TextBox textBox_source;
		private CheckBox checkBox_caseSensitive;
		private Label label_target;
		public TextBox target_textbox;
		private CheckBox checkBox_regularExpression;
		private Label label_tab_content_filter_message;
		private Label label_dsiLocation;
		private TextBox stringId_textbox;
		private Panel panel1;
		private CheckBox checkBox_TagContent;
		private GroupBox content_groupBox;
		private RadioButton alsoTags_radioButton;
		private RadioButton onlyTags_radioButton;
		private Panel panel2;
		private Panel panel6;
		private ComboBox comboBox_SourceTargetFilterLogicalOperator;
		private Panel panel3;
		private PictureBox pictureBox1;
		private CheckBox checkBox_useBackReferences;
		private TabPage tabPage_filters;
		private Panel panel8;
		private TableLayoutPanel tableLayoutPanel2;
		private Label label_tab_filter_message;
		private Panel panel9;
		private Button button_removeAll;
		private Button button_remove;
		private Button button_add;
		private Panel panel10;
		private ListViewExtended listView_selected;
		private ColumnHeader columnHeader_filtersSelected_name;
		private Panel panel5;
		private Label label_filters_selected;
		private Panel panel11;
		private ListViewExtended listView_available;
		private ColumnHeader columnHeader_filtersAvailable_name;
		private Panel panel4;
		private Label label_fitlers_available;
		private TabPage tabPage_comments;
		private TableLayoutPanel tableLayoutPanel3;
		private ComboBox comboBox_commentSeverity;
		private Label label_commentText;
		private Label label_commentSeverity;
		public TextBox textBox_commentText;
		public TextBox textBox_commentAuthor;
		private Label label_commentAuthor;
		private Label label_tab_comments_filter_message;
		private CheckBox commentRegexBox;
		private TabPage tabPage_contextInfo;
		private TableLayoutPanel tableLayoutPanel4;
		private Label label_documentStructure_message;
		private ListView listView_contextInfo;
		private ColumnHeader columnHeader_code;
		private ColumnHeader columnHeader_name;
		private ColumnHeader columnHeader_description;
		private LinkLabel linkLabel_contextInfoClearSelection;
		private Label label_contextInfoSelected;
		private TabPage tabPage_segmentNumbers;
		private GroupBox groupBox2;
		private TextBox fuzzyMin;
		private TextBox fuzzyMax;
		private Label label5;
		private Panel panel_spacer_01;
		private GroupBox userBox;
		private TextBox createdByBox;
		private CheckBox createdByCheck;
		private TextBox modifiedByBox;
		private CheckBox modifiedByCheck;
		private Label userLabel;
		private Panel panel_spacer_02;
		private GroupBox segmentsGroupBox;
		private CheckBox containsTagsCheckBox;
		private CheckBox mergedAcross;
		private CheckBox mergedCheckbox;
		private CheckBox splitCheckBox;
		private CheckBox sourceSameBox;
		private CheckBox equalsCaseSensitive;
		private Panel panel_spacer_03;
		private GroupBox groupBox1;
		private Panel panel7;
		private TextBox segmentsBox;
		private Label label1;
		private RadioButton noneBtn;
		private RadioButton groupedBtn;
		private RadioButton oddBtn;
		private RadioButton evenBtn;
		private Panel panel_spacer_05;
		private TabPage tabPage_Colors;
		private TableLayoutPanel colorLayoutPanel;
		private Label label6;
		private ListView colorsListView;
		private ToolStripMenuItem applyHighlightingToolStripMenuItem;
	}
}
