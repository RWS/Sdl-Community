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
			this.tabControl_filter = new System.Windows.Forms.TabControl();
			this.tabPage_content = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.label_source = new System.Windows.Forms.Label();
			this.textBox_source = new System.Windows.Forms.TextBox();
			this.checkBox_caseSensitive = new System.Windows.Forms.CheckBox();
			this.label_target = new System.Windows.Forms.Label();
			this.target_textbox = new System.Windows.Forms.TextBox();
			this.checkBox_regularExpression = new System.Windows.Forms.CheckBox();
			this.label_tab_content_filter_message = new System.Windows.Forms.Label();
			this.label_dsiLocation = new System.Windows.Forms.Label();
			this.dsiLocation_textbox = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkBox_TagContent = new System.Windows.Forms.CheckBox();
			this.content_groupBox = new System.Windows.Forms.GroupBox();
			this.alsoTags_radioButton = new System.Windows.Forms.RadioButton();
			this.onlyTags_radioButton = new System.Windows.Forms.RadioButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel6 = new System.Windows.Forms.Panel();
			this.comboBox_SourceTargetFilterLogicalOperator = new System.Windows.Forms.ComboBox();
			this.panel3 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.checkBox_useBackReferences = new System.Windows.Forms.CheckBox();
			this.tabPage_filters = new System.Windows.Forms.TabPage();
			this.panel8 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label_tab_filter_message = new System.Windows.Forms.Label();
			this.panel9 = new System.Windows.Forms.Panel();
			this.button_removeAll = new System.Windows.Forms.Button();
			this.button_remove = new System.Windows.Forms.Button();
			this.button_add = new System.Windows.Forms.Button();
			this.panel10 = new System.Windows.Forms.Panel();
			this.listView_selected = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
			this.columnHeader_filtersSelected_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel5 = new System.Windows.Forms.Panel();
			this.label_filters_selected = new System.Windows.Forms.Label();
			this.panel11 = new System.Windows.Forms.Panel();
			this.listView_available = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
			this.columnHeader_filtersAvailable_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel4 = new System.Windows.Forms.Panel();
			this.label_fitlers_available = new System.Windows.Forms.Label();
			this.tabPage_comments = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.comboBox_commentSeverity = new System.Windows.Forms.ComboBox();
			this.label_commentText = new System.Windows.Forms.Label();
			this.label_commentSeverity = new System.Windows.Forms.Label();
			this.textBox_commentText = new System.Windows.Forms.TextBox();
			this.textBox_commentAuthor = new System.Windows.Forms.TextBox();
			this.label_commentAuthor = new System.Windows.Forms.Label();
			this.label_tab_comments_filter_message = new System.Windows.Forms.Label();
			this.commentRegexBox = new System.Windows.Forms.CheckBox();
			this.tabPage_contextInfo = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.label_documentStructure_message = new System.Windows.Forms.Label();
			this.listView_contextInfo = new System.Windows.Forms.ListView();
			this.columnHeader_code = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader_description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.linkLabel_contextInfoClearSelection = new System.Windows.Forms.LinkLabel();
			this.label_contextInfoSelected = new System.Windows.Forms.Label();
			this.tabPage_segmentNumbers = new System.Windows.Forms.TabPage();
			this.sampleOptionsPanel = new System.Windows.Forms.Panel();
			this.samplingOptions = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
			this.radioButton_randomlySelect = new System.Windows.Forms.RadioButton();
			this.radioButton_selectOneInEvery = new System.Windows.Forms.RadioButton();
			this.checkBox_segmentSelection = new System.Windows.Forms.CheckBox();
			this.numericUpDown_randomSelect = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.checkBox_minMaxCharsPerSegment = new System.Windows.Forms.CheckBox();
			this.numericUpDown_maxCharsPerSegment = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown_selectOneInEvery = new System.Windows.Forms.NumericUpDown();
			this.numericUpDown_minCharsPerSegment = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.panel_spacer_00 = new System.Windows.Forms.Panel();
			this.fuzzyValuesBetweenGroupBox = new System.Windows.Forms.GroupBox();
			this.fuzzyMin = new System.Windows.Forms.TextBox();
			this.fuzzyMax = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.panel_spacer_01 = new System.Windows.Forms.Panel();
			this.userBoxGroupBox = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.createdByBox = new System.Windows.Forms.TextBox();
			this.modifiedByBox = new System.Windows.Forms.TextBox();
			this.createdByCheck = new System.Windows.Forms.CheckBox();
			this.userLabel = new System.Windows.Forms.Label();
			this.modifiedByCheck = new System.Windows.Forms.CheckBox();
			this.panel_spacer_02 = new System.Windows.Forms.Panel();
			this.segmentsGroupBox = new System.Windows.Forms.GroupBox();
			this.containsTagsCheckBox = new System.Windows.Forms.CheckBox();
			this.mergedAcross = new System.Windows.Forms.CheckBox();
			this.mergedCheckbox = new System.Windows.Forms.CheckBox();
			this.splitCheckBox = new System.Windows.Forms.CheckBox();
			this.sourceSameBox = new System.Windows.Forms.CheckBox();
			this.equalsCaseSensitive = new System.Windows.Forms.CheckBox();
			this.panel_spacer_03 = new System.Windows.Forms.Panel();
			this.numberOptionsGroupBox = new System.Windows.Forms.GroupBox();
			this.panel7 = new System.Windows.Forms.Panel();
			this.segmentsBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.noneBtn = new System.Windows.Forms.RadioButton();
			this.groupedBtn = new System.Windows.Forms.RadioButton();
			this.oddBtn = new System.Windows.Forms.RadioButton();
			this.evenBtn = new System.Windows.Forms.RadioButton();
			this.panel_spacer_05 = new System.Windows.Forms.Panel();
			this.tabPage_colors = new System.Windows.Forms.TabPage();
			this.colorLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.label6 = new System.Windows.Forms.Label();
			this.colorsListView = new System.Windows.Forms.ListView();
			this.imageList_filter = new System.Windows.Forms.ImageList(this.components);
			this.splitter_filterExpression = new System.Windows.Forms.Splitter();
			this.panel_buffer = new System.Windows.Forms.Panel();
			this.toolStrip_filter = new System.Windows.Forms.ToolStrip();
			this.toolStripButton_applyFilter = new System.Windows.Forms.ToolStripButton();
			this.reverseBtn = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_clearFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.highlightColorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearHighlightingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.generateXliff = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_saveFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_loadFilter = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.helpButton = new System.Windows.Forms.ToolStripButton();
			this.panel_filterExpression = new System.Windows.Forms.Panel();
			this.filterExpressionControl = new Sdl.Community.AdvancedDisplayFilter.Controls.FilterExpression();
			this.panel_filterStatus = new System.Windows.Forms.Panel();
			this.panel_filterStatusBar = new System.Windows.Forms.Panel();
			this.panel_filterStatusBarMessage = new System.Windows.Forms.Panel();
			this.label_filterStatusBarMessage = new System.Windows.Forms.Label();
			this.panel_filterStatusBarImage = new System.Windows.Forms.Panel();
			this.pictureBox_filterStatusBarImage = new System.Windows.Forms.PictureBox();
			this.toolStripMenuItem_yellow = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_brightGreen = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_turquoise = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_pink = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_blue = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_red = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_darkBlue = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_teal = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_green = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_violet = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_darkRed = new System.Windows.Forms.ToolStripMenuItem();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
			this.tags_tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.content_toolTips = new System.Windows.Forms.ToolTip(this.components);
			this.panel_body.SuspendLayout();
			this.tabControl_filter.SuspendLayout();
			this.tabPage_content.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.content_groupBox.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabPage_filters.SuspendLayout();
			this.panel8.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel9.SuspendLayout();
			this.panel10.SuspendLayout();
			this.panel5.SuspendLayout();
			this.panel11.SuspendLayout();
			this.panel4.SuspendLayout();
			this.tabPage_comments.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tabPage_contextInfo.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tabPage_segmentNumbers.SuspendLayout();
			this.sampleOptionsPanel.SuspendLayout();
			this.samplingOptions.SuspendLayout();
			this.tableLayoutPanel6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_randomSelect)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maxCharsPerSegment)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_selectOneInEvery)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_minCharsPerSegment)).BeginInit();
			this.fuzzyValuesBetweenGroupBox.SuspendLayout();
			this.userBoxGroupBox.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.segmentsGroupBox.SuspendLayout();
			this.numberOptionsGroupBox.SuspendLayout();
			this.panel7.SuspendLayout();
			this.tabPage_colors.SuspendLayout();
			this.colorLayoutPanel.SuspendLayout();
			this.toolStrip_filter.SuspendLayout();
			this.panel_filterExpression.SuspendLayout();
			this.panel_filterStatus.SuspendLayout();
			this.panel_filterStatusBar.SuspendLayout();
			this.panel_filterStatusBarMessage.SuspendLayout();
			this.panel_filterStatusBarImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_filterStatusBarImage)).BeginInit();
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
			// tabControl_filter
			// 
			this.tabControl_filter.Controls.Add(this.tabPage_content);
			this.tabControl_filter.Controls.Add(this.tabPage_filters);
			this.tabControl_filter.Controls.Add(this.tabPage_comments);
			this.tabControl_filter.Controls.Add(this.tabPage_contextInfo);
			this.tabControl_filter.Controls.Add(this.tabPage_segmentNumbers);
			this.tabControl_filter.Controls.Add(this.tabPage_colors);
			resources.ApplyResources(this.tabControl_filter, "tabControl_filter");
			this.tabControl_filter.ImageList = this.imageList_filter;
			this.tabControl_filter.Name = "tabControl_filter";
			this.tabControl_filter.SelectedIndex = 0;
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
			this.tableLayoutPanel1.Controls.Add(this.dsiLocation_textbox, 1, 3);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 7);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 1);
			this.tableLayoutPanel1.Controls.Add(this.checkBox_useBackReferences, 0, 5);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// label_source
			// 
			resources.ApplyResources(this.label_source, "label_source");
			this.label_source.Name = "label_source";
			// 
			// textBox_source
			// 
			resources.ApplyResources(this.textBox_source, "textBox_source");
			this.textBox_source.Name = "textBox_source";
			this.textBox_source.TextChanged += new System.EventHandler(this.TextBox_source_TextChanged);
			this.textBox_source.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_source_KeyUp);
			// 
			// checkBox_caseSensitive
			// 
			resources.ApplyResources(this.checkBox_caseSensitive, "checkBox_caseSensitive");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_caseSensitive, 3);
			this.checkBox_caseSensitive.Name = "checkBox_caseSensitive";
			this.checkBox_caseSensitive.UseVisualStyleBackColor = true;
			this.checkBox_caseSensitive.CheckedChanged += new System.EventHandler(this.CheckBox_caseSensitive_CheckedChanged);
			// 
			// label_target
			// 
			resources.ApplyResources(this.label_target, "label_target");
			this.label_target.Name = "label_target";
			// 
			// target_textbox
			// 
			resources.ApplyResources(this.target_textbox, "target_textbox");
			this.target_textbox.Name = "target_textbox";
			this.target_textbox.TextChanged += new System.EventHandler(this.TextBox_target_TextChanged);
			this.target_textbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_target_KeyUp);
			// 
			// checkBox_regularExpression
			// 
			resources.ApplyResources(this.checkBox_regularExpression, "checkBox_regularExpression");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_regularExpression, 3);
			this.checkBox_regularExpression.Name = "checkBox_regularExpression";
			this.checkBox_regularExpression.UseVisualStyleBackColor = true;
			this.checkBox_regularExpression.CheckedChanged += new System.EventHandler(this.CheckBox_regularExpression_CheckedChanged);
			// 
			// label_tab_content_filter_message
			// 
			resources.ApplyResources(this.label_tab_content_filter_message, "label_tab_content_filter_message");
			this.tableLayoutPanel1.SetColumnSpan(this.label_tab_content_filter_message, 3);
			this.label_tab_content_filter_message.Name = "label_tab_content_filter_message";
			// 
			// label_dsiLocation
			// 
			resources.ApplyResources(this.label_dsiLocation, "label_dsiLocation");
			this.label_dsiLocation.Name = "label_dsiLocation";
			// 
			// dsiLocation_textbox
			// 
			resources.ApplyResources(this.dsiLocation_textbox, "dsiLocation_textbox");
			this.tableLayoutPanel1.SetColumnSpan(this.dsiLocation_textbox, 2);
			this.dsiLocation_textbox.Name = "dsiLocation_textbox";
			this.dsiLocation_textbox.TextChanged += new System.EventHandler(this.DsiLocation_textbox_TextChanged);
			// 
			// panel1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
			this.panel1.Controls.Add(this.checkBox_TagContent);
			this.panel1.Controls.Add(this.content_groupBox);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// checkBox_TagContent
			// 
			resources.ApplyResources(this.checkBox_TagContent, "checkBox_TagContent");
			this.checkBox_TagContent.Name = "checkBox_TagContent";
			this.checkBox_TagContent.UseVisualStyleBackColor = true;
			this.checkBox_TagContent.CheckedChanged += new System.EventHandler(this.CheckBox_TagContent_CheckedChanged);
			// 
			// content_groupBox
			// 
			this.content_groupBox.Controls.Add(this.alsoTags_radioButton);
			this.content_groupBox.Controls.Add(this.onlyTags_radioButton);
			resources.ApplyResources(this.content_groupBox, "content_groupBox");
			this.content_groupBox.Name = "content_groupBox";
			this.content_groupBox.TabStop = false;
			// 
			// alsoTags_radioButton
			// 
			resources.ApplyResources(this.alsoTags_radioButton, "alsoTags_radioButton");
			this.alsoTags_radioButton.Checked = true;
			this.alsoTags_radioButton.Name = "alsoTags_radioButton";
			this.alsoTags_radioButton.TabStop = true;
			this.tags_tooltip.SetToolTip(this.alsoTags_radioButton, global::Sdl.Community.AdvancedDisplayFilter.StringResources.DisplayFilter_IncludeTags_Tooltip);
			this.alsoTags_radioButton.UseVisualStyleBackColor = true;
			this.alsoTags_radioButton.CheckedChanged += new System.EventHandler(this.AlsoTags_radioButton_CheckedChanged);
			// 
			// onlyTags_radioButton
			// 
			resources.ApplyResources(this.onlyTags_radioButton, "onlyTags_radioButton");
			this.onlyTags_radioButton.Name = "onlyTags_radioButton";
			this.tags_tooltip.SetToolTip(this.onlyTags_radioButton, global::Sdl.Community.AdvancedDisplayFilter.StringResources.DisplayFilter_OnlyTags_Tooltip);
			this.onlyTags_radioButton.UseVisualStyleBackColor = true;
			this.onlyTags_radioButton.CheckedChanged += new System.EventHandler(this.OnlyTags_radioButton_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.panel6);
			this.panel2.Controls.Add(this.panel3);
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Name = "panel2";
			this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
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
			this.comboBox_SourceTargetFilterLogicalOperator.SelectedIndexChanged += new System.EventHandler(this.ComboBox_SourceTargetFilterLogicalOperator_SelectedIndexChanged);
			this.comboBox_SourceTargetFilterLogicalOperator.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ComboBox_SourceTargetFilterLogicalOperator_KeyPress);
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
			// checkBox_useBackReferences
			// 
			resources.ApplyResources(this.checkBox_useBackReferences, "checkBox_useBackReferences");
			this.tableLayoutPanel1.SetColumnSpan(this.checkBox_useBackReferences, 3);
			this.checkBox_useBackReferences.Name = "checkBox_useBackReferences";
			this.checkBox_useBackReferences.UseVisualStyleBackColor = true;
			this.checkBox_useBackReferences.CheckedChanged += new System.EventHandler(this.CheckBox_useBackReferences_CheckedChanged);
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
			// label_tab_filter_message
			// 
			resources.ApplyResources(this.label_tab_filter_message, "label_tab_filter_message");
			this.tableLayoutPanel2.SetColumnSpan(this.label_tab_filter_message, 3);
			this.label_tab_filter_message.Name = "label_tab_filter_message";
			// 
			// panel9
			// 
			this.panel9.Controls.Add(this.button_removeAll);
			this.panel9.Controls.Add(this.button_remove);
			this.panel9.Controls.Add(this.button_add);
			resources.ApplyResources(this.panel9, "panel9");
			this.panel9.Name = "panel9";
			// 
			// button_removeAll
			// 
			resources.ApplyResources(this.button_removeAll, "button_removeAll");
			this.button_removeAll.Name = "button_removeAll";
			this.button_removeAll.UseVisualStyleBackColor = true;
			this.button_removeAll.Click += new System.EventHandler(this.Button_removeAll_Click);
			// 
			// button_remove
			// 
			resources.ApplyResources(this.button_remove, "button_remove");
			this.button_remove.Name = "button_remove";
			this.button_remove.UseVisualStyleBackColor = true;
			this.button_remove.Click += new System.EventHandler(this.Button_remove_Click);
			// 
			// button_add
			// 
			resources.ApplyResources(this.button_add, "button_add");
			this.button_add.Name = "button_add";
			this.button_add.UseVisualStyleBackColor = true;
			this.button_add.Click += new System.EventHandler(this.Button_add_Click);
			// 
			// panel10
			// 
			this.panel10.Controls.Add(this.listView_selected);
			this.panel10.Controls.Add(this.panel5);
			resources.ApplyResources(this.panel10, "panel10");
			this.panel10.Name = "panel10";
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
			this.listView_selected.SelectedIndexChanged += new System.EventHandler(this.ListView_selected_SelectedIndexChanged);
			this.listView_selected.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView_selected_MouseDoubleClick);
			this.listView_selected.Resize += new System.EventHandler(this.ListView_selected_Resize);
			// 
			// columnHeader_filtersSelected_name
			// 
			resources.ApplyResources(this.columnHeader_filtersSelected_name, "columnHeader_filtersSelected_name");
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
			// panel11
			// 
			this.panel11.Controls.Add(this.listView_available);
			this.panel11.Controls.Add(this.panel4);
			resources.ApplyResources(this.panel11, "panel11");
			this.panel11.Name = "panel11";
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
			this.listView_available.SelectedIndexChanged += new System.EventHandler(this.ListView_available_SelectedIndexChanged);
			this.listView_available.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListView_available_MouseDoubleClick);
			this.listView_available.Resize += new System.EventHandler(this.ListView_available_Resize);
			// 
			// columnHeader_filtersAvailable_name
			// 
			resources.ApplyResources(this.columnHeader_filtersAvailable_name, "columnHeader_filtersAvailable_name");
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
			// comboBox_commentSeverity
			// 
			resources.ApplyResources(this.comboBox_commentSeverity, "comboBox_commentSeverity");
			this.comboBox_commentSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_commentSeverity.FormattingEnabled = true;
			this.comboBox_commentSeverity.Name = "comboBox_commentSeverity";
			this.comboBox_commentSeverity.SelectedIndexChanged += new System.EventHandler(this.ComboBox_commentSeverity_SelectedIndexChanged);
			// 
			// label_commentText
			// 
			resources.ApplyResources(this.label_commentText, "label_commentText");
			this.label_commentText.Name = "label_commentText";
			// 
			// label_commentSeverity
			// 
			resources.ApplyResources(this.label_commentSeverity, "label_commentSeverity");
			this.label_commentSeverity.Name = "label_commentSeverity";
			// 
			// textBox_commentText
			// 
			resources.ApplyResources(this.textBox_commentText, "textBox_commentText");
			this.textBox_commentText.Name = "textBox_commentText";
			this.textBox_commentText.TextChanged += new System.EventHandler(this.TextBox_commentText_TextChanged);
			this.textBox_commentText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_commentText_KeyUp);
			// 
			// textBox_commentAuthor
			// 
			resources.ApplyResources(this.textBox_commentAuthor, "textBox_commentAuthor");
			this.textBox_commentAuthor.Name = "textBox_commentAuthor";
			this.textBox_commentAuthor.TextChanged += new System.EventHandler(this.TextBox_commentAuthor_TextChanged);
			this.textBox_commentAuthor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_commentAuthor_KeyUp);
			// 
			// label_commentAuthor
			// 
			resources.ApplyResources(this.label_commentAuthor, "label_commentAuthor");
			this.label_commentAuthor.Name = "label_commentAuthor";
			// 
			// label_tab_comments_filter_message
			// 
			resources.ApplyResources(this.label_tab_comments_filter_message, "label_tab_comments_filter_message");
			this.tableLayoutPanel3.SetColumnSpan(this.label_tab_comments_filter_message, 2);
			this.label_tab_comments_filter_message.Name = "label_tab_comments_filter_message";
			// 
			// commentRegexBox
			// 
			resources.ApplyResources(this.commentRegexBox, "commentRegexBox");
			this.tableLayoutPanel3.SetColumnSpan(this.commentRegexBox, 2);
			this.commentRegexBox.Name = "commentRegexBox";
			this.commentRegexBox.UseVisualStyleBackColor = true;
			this.commentRegexBox.CheckedChanged += new System.EventHandler(this.CommentRegexBox_CheckedChanged);
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
			// label_documentStructure_message
			// 
			resources.ApplyResources(this.label_documentStructure_message, "label_documentStructure_message");
			this.tableLayoutPanel4.SetColumnSpan(this.label_documentStructure_message, 2);
			this.label_documentStructure_message.Name = "label_documentStructure_message";
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
			this.listView_contextInfo.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.ListView_contextInfo_ItemSelectionChanged);
			this.listView_contextInfo.Resize += new System.EventHandler(this.ListView_contextInfo_Resize);
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
			// linkLabel_contextInfoClearSelection
			// 
			resources.ApplyResources(this.linkLabel_contextInfoClearSelection, "linkLabel_contextInfoClearSelection");
			this.linkLabel_contextInfoClearSelection.Name = "linkLabel_contextInfoClearSelection";
			this.linkLabel_contextInfoClearSelection.TabStop = true;
			this.linkLabel_contextInfoClearSelection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_contextInfoClearSelection_LinkClicked);
			// 
			// label_contextInfoSelected
			// 
			resources.ApplyResources(this.label_contextInfoSelected, "label_contextInfoSelected");
			this.label_contextInfoSelected.Name = "label_contextInfoSelected";
			// 
			// tabPage_segmentNumbers
			// 
			resources.ApplyResources(this.tabPage_segmentNumbers, "tabPage_segmentNumbers");
			this.tabPage_segmentNumbers.Controls.Add(this.sampleOptionsPanel);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_00);
			this.tabPage_segmentNumbers.Controls.Add(this.fuzzyValuesBetweenGroupBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_01);
			this.tabPage_segmentNumbers.Controls.Add(this.userBoxGroupBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_02);
			this.tabPage_segmentNumbers.Controls.Add(this.segmentsGroupBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_03);
			this.tabPage_segmentNumbers.Controls.Add(this.numberOptionsGroupBox);
			this.tabPage_segmentNumbers.Controls.Add(this.panel_spacer_05);
			this.tabPage_segmentNumbers.Name = "tabPage_segmentNumbers";
			this.tabPage_segmentNumbers.UseVisualStyleBackColor = true;
			// 
			// sampleOptionsPanel
			// 
			this.sampleOptionsPanel.Controls.Add(this.samplingOptions);
			resources.ApplyResources(this.sampleOptionsPanel, "sampleOptionsPanel");
			this.sampleOptionsPanel.Name = "sampleOptionsPanel";
			// 
			// samplingOptions
			// 
			this.samplingOptions.Controls.Add(this.tableLayoutPanel6);
			resources.ApplyResources(this.samplingOptions, "samplingOptions");
			this.samplingOptions.Name = "samplingOptions";
			this.samplingOptions.TabStop = false;
			// 
			// tableLayoutPanel6
			// 
			resources.ApplyResources(this.tableLayoutPanel6, "tableLayoutPanel6");
			this.tableLayoutPanel6.Controls.Add(this.radioButton_randomlySelect, 1, 1);
			this.tableLayoutPanel6.Controls.Add(this.radioButton_selectOneInEvery, 1, 2);
			this.tableLayoutPanel6.Controls.Add(this.checkBox_segmentSelection, 0, 0);
			this.tableLayoutPanel6.Controls.Add(this.numericUpDown_randomSelect, 2, 1);
			this.tableLayoutPanel6.Controls.Add(this.label2, 3, 1);
			this.tableLayoutPanel6.Controls.Add(this.checkBox_minMaxCharsPerSegment, 0, 3);
			this.tableLayoutPanel6.Controls.Add(this.numericUpDown_maxCharsPerSegment, 2, 5);
			this.tableLayoutPanel6.Controls.Add(this.numericUpDown_selectOneInEvery, 2, 2);
			this.tableLayoutPanel6.Controls.Add(this.numericUpDown_minCharsPerSegment, 2, 4);
			this.tableLayoutPanel6.Controls.Add(this.label3, 1, 4);
			this.tableLayoutPanel6.Controls.Add(this.label4, 1, 5);
			this.tableLayoutPanel6.Name = "tableLayoutPanel6";
			// 
			// radioButton_randomlySelect
			// 
			resources.ApplyResources(this.radioButton_randomlySelect, "radioButton_randomlySelect");
			this.radioButton_randomlySelect.Checked = true;
			this.radioButton_randomlySelect.Name = "radioButton_randomlySelect";
			this.radioButton_randomlySelect.TabStop = true;
			this.radioButton_randomlySelect.UseVisualStyleBackColor = true;
			this.radioButton_randomlySelect.CheckedChanged += new System.EventHandler(this.RadioButton_randomlySelect_CheckedChanged);
			// 
			// radioButton_selectOneInEvery
			// 
			resources.ApplyResources(this.radioButton_selectOneInEvery, "radioButton_selectOneInEvery");
			this.radioButton_selectOneInEvery.Name = "radioButton_selectOneInEvery";
			this.radioButton_selectOneInEvery.UseVisualStyleBackColor = true;
			this.radioButton_selectOneInEvery.CheckedChanged += new System.EventHandler(this.RadioButton_selectOneInEvery_CheckedChanged);
			// 
			// checkBox_segmentSelection
			// 
			resources.ApplyResources(this.checkBox_segmentSelection, "checkBox_segmentSelection");
			this.tableLayoutPanel6.SetColumnSpan(this.checkBox_segmentSelection, 4);
			this.checkBox_segmentSelection.Name = "checkBox_segmentSelection";
			this.checkBox_segmentSelection.UseVisualStyleBackColor = true;
			this.checkBox_segmentSelection.CheckedChanged += new System.EventHandler(this.CheckBox_segmentSelection_CheckedChanged);
			// 
			// numericUpDown_randomSelect
			// 
			resources.ApplyResources(this.numericUpDown_randomSelect, "numericUpDown_randomSelect");
			this.numericUpDown_randomSelect.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_randomSelect.Name = "numericUpDown_randomSelect";
			this.numericUpDown_randomSelect.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericUpDown_randomSelect.ValueChanged += new System.EventHandler(this.NumericUpDown_randomSelect_ValueChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// checkBox_minMaxCharsPerSegment
			// 
			resources.ApplyResources(this.checkBox_minMaxCharsPerSegment, "checkBox_minMaxCharsPerSegment");
			this.tableLayoutPanel6.SetColumnSpan(this.checkBox_minMaxCharsPerSegment, 4);
			this.checkBox_minMaxCharsPerSegment.Name = "checkBox_minMaxCharsPerSegment";
			this.checkBox_minMaxCharsPerSegment.UseVisualStyleBackColor = true;
			this.checkBox_minMaxCharsPerSegment.CheckedChanged += new System.EventHandler(this.CheckBox_minMaxCharsPerSegment_CheckedChanged);
			// 
			// numericUpDown_maxCharsPerSegment
			// 
			resources.ApplyResources(this.numericUpDown_maxCharsPerSegment, "numericUpDown_maxCharsPerSegment");
			this.numericUpDown_maxCharsPerSegment.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDown_maxCharsPerSegment.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_maxCharsPerSegment.Name = "numericUpDown_maxCharsPerSegment";
			this.numericUpDown_maxCharsPerSegment.Value = new decimal(new int[] {
            75,
            0,
            0,
            0});
			this.numericUpDown_maxCharsPerSegment.ValueChanged += new System.EventHandler(this.NumericUpDown_maxCharsPerSegment_ValueChanged);
			// 
			// numericUpDown_selectOneInEvery
			// 
			resources.ApplyResources(this.numericUpDown_selectOneInEvery, "numericUpDown_selectOneInEvery");
			this.numericUpDown_selectOneInEvery.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_selectOneInEvery.Name = "numericUpDown_selectOneInEvery";
			this.numericUpDown_selectOneInEvery.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numericUpDown_selectOneInEvery.ValueChanged += new System.EventHandler(this.NumericUpDown_selectOneInEvery_ValueChanged);
			// 
			// numericUpDown_minCharsPerSegment
			// 
			resources.ApplyResources(this.numericUpDown_minCharsPerSegment, "numericUpDown_minCharsPerSegment");
			this.numericUpDown_minCharsPerSegment.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
			this.numericUpDown_minCharsPerSegment.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown_minCharsPerSegment.Name = "numericUpDown_minCharsPerSegment";
			this.numericUpDown_minCharsPerSegment.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.numericUpDown_minCharsPerSegment.ValueChanged += new System.EventHandler(this.NumericUpDown_minCharsPerSegment_ValueChanged);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// panel_spacer_00
			// 
			resources.ApplyResources(this.panel_spacer_00, "panel_spacer_00");
			this.panel_spacer_00.Name = "panel_spacer_00";
			// 
			// fuzzyValuesBetweenGroupBox
			// 
			this.fuzzyValuesBetweenGroupBox.Controls.Add(this.fuzzyMin);
			this.fuzzyValuesBetweenGroupBox.Controls.Add(this.fuzzyMax);
			this.fuzzyValuesBetweenGroupBox.Controls.Add(this.label5);
			resources.ApplyResources(this.fuzzyValuesBetweenGroupBox, "fuzzyValuesBetweenGroupBox");
			this.fuzzyValuesBetweenGroupBox.Name = "fuzzyValuesBetweenGroupBox";
			this.fuzzyValuesBetweenGroupBox.TabStop = false;
			// 
			// fuzzyMin
			// 
			resources.ApplyResources(this.fuzzyMin, "fuzzyMin");
			this.fuzzyMin.Name = "fuzzyMin";
			this.fuzzyMin.TextChanged += new System.EventHandler(this.FuzzyMin_TextChanged);
			// 
			// fuzzyMax
			// 
			resources.ApplyResources(this.fuzzyMax, "fuzzyMax");
			this.fuzzyMax.Name = "fuzzyMax";
			this.fuzzyMax.TextChanged += new System.EventHandler(this.FuzzyMax_TextChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// panel_spacer_01
			// 
			resources.ApplyResources(this.panel_spacer_01, "panel_spacer_01");
			this.panel_spacer_01.Name = "panel_spacer_01";
			// 
			// userBoxGroupBox
			// 
			this.userBoxGroupBox.Controls.Add(this.tableLayoutPanel5);
			resources.ApplyResources(this.userBoxGroupBox, "userBoxGroupBox");
			this.userBoxGroupBox.Name = "userBoxGroupBox";
			this.userBoxGroupBox.TabStop = false;
			// 
			// tableLayoutPanel5
			// 
			resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
			this.tableLayoutPanel5.Controls.Add(this.createdByBox, 1, 2);
			this.tableLayoutPanel5.Controls.Add(this.modifiedByBox, 1, 1);
			this.tableLayoutPanel5.Controls.Add(this.createdByCheck, 0, 2);
			this.tableLayoutPanel5.Controls.Add(this.userLabel, 0, 0);
			this.tableLayoutPanel5.Controls.Add(this.modifiedByCheck, 0, 1);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			// 
			// createdByBox
			// 
			resources.ApplyResources(this.createdByBox, "createdByBox");
			this.createdByBox.Name = "createdByBox";
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
			this.createdByCheck.CheckedChanged += new System.EventHandler(this.CreatedByCheck_CheckedChanged);
			// 
			// userLabel
			// 
			resources.ApplyResources(this.userLabel, "userLabel");
			this.tableLayoutPanel5.SetColumnSpan(this.userLabel, 2);
			this.userLabel.Name = "userLabel";
			// 
			// modifiedByCheck
			// 
			resources.ApplyResources(this.modifiedByCheck, "modifiedByCheck");
			this.modifiedByCheck.Name = "modifiedByCheck";
			this.modifiedByCheck.UseVisualStyleBackColor = true;
			this.modifiedByCheck.CheckedChanged += new System.EventHandler(this.ModifiedByCheck_CheckedChanged);
			// 
			// panel_spacer_02
			// 
			resources.ApplyResources(this.panel_spacer_02, "panel_spacer_02");
			this.panel_spacer_02.Name = "panel_spacer_02";
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
			// containsTagsCheckBox
			// 
			resources.ApplyResources(this.containsTagsCheckBox, "containsTagsCheckBox");
			this.containsTagsCheckBox.Name = "containsTagsCheckBox";
			this.containsTagsCheckBox.UseVisualStyleBackColor = true;
			this.containsTagsCheckBox.CheckedChanged += new System.EventHandler(this.ContainsTagsCheckBox_CheckedChanged);
			// 
			// mergedAcross
			// 
			resources.ApplyResources(this.mergedAcross, "mergedAcross");
			this.mergedAcross.Name = "mergedAcross";
			this.mergedAcross.UseVisualStyleBackColor = true;
			this.mergedAcross.CheckedChanged += new System.EventHandler(this.MergedAcross_CheckedChanged);
			// 
			// mergedCheckbox
			// 
			resources.ApplyResources(this.mergedCheckbox, "mergedCheckbox");
			this.mergedCheckbox.Name = "mergedCheckbox";
			this.mergedCheckbox.UseVisualStyleBackColor = true;
			this.mergedCheckbox.CheckedChanged += new System.EventHandler(this.MergedCheckbox_CheckedChanged);
			// 
			// splitCheckBox
			// 
			resources.ApplyResources(this.splitCheckBox, "splitCheckBox");
			this.splitCheckBox.Name = "splitCheckBox";
			this.splitCheckBox.UseVisualStyleBackColor = true;
			this.splitCheckBox.CheckedChanged += new System.EventHandler(this.SplitCheckBox_CheckedChanged);
			// 
			// sourceSameBox
			// 
			resources.ApplyResources(this.sourceSameBox, "sourceSameBox");
			this.sourceSameBox.Name = "sourceSameBox";
			this.sourceSameBox.UseVisualStyleBackColor = true;
			this.sourceSameBox.CheckedChanged += new System.EventHandler(this.SourceSameBox_CheckedChanged);
			// 
			// equalsCaseSensitive
			// 
			resources.ApplyResources(this.equalsCaseSensitive, "equalsCaseSensitive");
			this.equalsCaseSensitive.Name = "equalsCaseSensitive";
			this.equalsCaseSensitive.UseVisualStyleBackColor = true;
			this.equalsCaseSensitive.CheckedChanged += new System.EventHandler(this.EqualsCaseSensitive_CheckedChanged);
			// 
			// panel_spacer_03
			// 
			resources.ApplyResources(this.panel_spacer_03, "panel_spacer_03");
			this.panel_spacer_03.Name = "panel_spacer_03";
			// 
			// numberOptionsGroupBox
			// 
			this.numberOptionsGroupBox.Controls.Add(this.panel7);
			this.numberOptionsGroupBox.Controls.Add(this.noneBtn);
			this.numberOptionsGroupBox.Controls.Add(this.groupedBtn);
			this.numberOptionsGroupBox.Controls.Add(this.oddBtn);
			this.numberOptionsGroupBox.Controls.Add(this.evenBtn);
			resources.ApplyResources(this.numberOptionsGroupBox, "numberOptionsGroupBox");
			this.numberOptionsGroupBox.Name = "numberOptionsGroupBox";
			this.numberOptionsGroupBox.TabStop = false;
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.segmentsBox);
			this.panel7.Controls.Add(this.label1);
			resources.ApplyResources(this.panel7, "panel7");
			this.panel7.Name = "panel7";
			// 
			// segmentsBox
			// 
			resources.ApplyResources(this.segmentsBox, "segmentsBox");
			this.segmentsBox.Name = "segmentsBox";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Name = "label1";
			// 
			// noneBtn
			// 
			resources.ApplyResources(this.noneBtn, "noneBtn");
			this.noneBtn.Checked = true;
			this.noneBtn.Name = "noneBtn";
			this.noneBtn.TabStop = true;
			this.noneBtn.UseVisualStyleBackColor = true;
			this.noneBtn.CheckedChanged += new System.EventHandler(this.NoneBtn_CheckedChanged);
			// 
			// groupedBtn
			// 
			resources.ApplyResources(this.groupedBtn, "groupedBtn");
			this.groupedBtn.Name = "groupedBtn";
			this.groupedBtn.UseVisualStyleBackColor = true;
			this.groupedBtn.CheckedChanged += new System.EventHandler(this.GroupedBtn_CheckedChanged);
			// 
			// oddBtn
			// 
			resources.ApplyResources(this.oddBtn, "oddBtn");
			this.oddBtn.Name = "oddBtn";
			this.oddBtn.UseVisualStyleBackColor = true;
			this.oddBtn.CheckedChanged += new System.EventHandler(this.OddBtn_CheckedChanged);
			// 
			// evenBtn
			// 
			resources.ApplyResources(this.evenBtn, "evenBtn");
			this.evenBtn.Name = "evenBtn";
			this.evenBtn.UseVisualStyleBackColor = true;
			this.evenBtn.CheckedChanged += new System.EventHandler(this.EvenBtn_CheckedChanged);
			// 
			// panel_spacer_05
			// 
			resources.ApplyResources(this.panel_spacer_05, "panel_spacer_05");
			this.panel_spacer_05.Name = "panel_spacer_05";
			// 
			// tabPage_colors
			// 
			this.tabPage_colors.Controls.Add(this.colorLayoutPanel);
			resources.ApplyResources(this.tabPage_colors, "tabPage_colors");
			this.tabPage_colors.Name = "tabPage_colors";
			this.tabPage_colors.UseVisualStyleBackColor = true;
			// 
			// colorLayoutPanel
			// 
			resources.ApplyResources(this.colorLayoutPanel, "colorLayoutPanel");
			this.colorLayoutPanel.Controls.Add(this.label6, 0, 0);
			this.colorLayoutPanel.Controls.Add(this.colorsListView, 0, 1);
			this.colorLayoutPanel.Name = "colorLayoutPanel";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// colorsListView
			// 
			this.colorsListView.CheckBoxes = true;
			resources.ApplyResources(this.colorsListView, "colorsListView");
			this.colorsListView.Name = "colorsListView";
			this.colorsListView.UseCompatibleStateImageBehavior = false;
			this.colorsListView.SelectedIndexChanged += new System.EventHandler(this.ColorsListView_SelectedIndexChanged);
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
			this.toolStripButton_applyFilter.Click += new System.EventHandler(this.ToolStripButton_applyFilter_Click);
			// 
			// reverseBtn
			// 
			resources.ApplyResources(this.reverseBtn, "reverseBtn");
			this.reverseBtn.Name = "reverseBtn";
			this.reverseBtn.Click += new System.EventHandler(this.ReverseBtn_Click);
			// 
			// toolStripButton_clearFilter
			// 
			resources.ApplyResources(this.toolStripButton_clearFilter, "toolStripButton_clearFilter");
			this.toolStripButton_clearFilter.Name = "toolStripButton_clearFilter";
			this.toolStripButton_clearFilter.Click += new System.EventHandler(this.ToolStripButton_clearFilter_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.highlightColorsToolStripMenuItem,
            this.clearHighlightingToolStripMenuItem});
			resources.ApplyResources(this.toolStripDropDownButton1, "toolStripDropDownButton1");
			this.toolStripDropDownButton1.Margin = new System.Windows.Forms.Padding(0, 1, 2, 2);
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			// 
			// highlightColorsToolStripMenuItem
			// 
			resources.ApplyResources(this.highlightColorsToolStripMenuItem, "highlightColorsToolStripMenuItem");
			this.highlightColorsToolStripMenuItem.Name = "highlightColorsToolStripMenuItem";
			// 
			// clearHighlightingToolStripMenuItem
			// 
			resources.ApplyResources(this.clearHighlightingToolStripMenuItem, "clearHighlightingToolStripMenuItem");
			this.clearHighlightingToolStripMenuItem.Name = "clearHighlightingToolStripMenuItem";
			this.clearHighlightingToolStripMenuItem.Click += new System.EventHandler(this.ClearHighlightingToolStripMenuItem_Click);
			// 
			// generateXliff
			// 
			resources.ApplyResources(this.generateXliff, "generateXliff");
			this.generateXliff.Name = "generateXliff";
			this.generateXliff.Click += new System.EventHandler(this.GenerateXliff_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// toolStripButton_saveFilter
			// 
			resources.ApplyResources(this.toolStripButton_saveFilter, "toolStripButton_saveFilter");
			this.toolStripButton_saveFilter.Name = "toolStripButton_saveFilter";
			this.toolStripButton_saveFilter.Click += new System.EventHandler(this.ToolStripButton_saveFilter_Click);
			// 
			// toolStripButton_loadFilter
			// 
			resources.ApplyResources(this.toolStripButton_loadFilter, "toolStripButton_loadFilter");
			this.toolStripButton_loadFilter.Name = "toolStripButton_loadFilter";
			this.toolStripButton_loadFilter.Click += new System.EventHandler(this.ToolStripButton_loadFilter_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
			// 
			// helpButton
			// 
			resources.ApplyResources(this.helpButton, "helpButton");
			this.helpButton.Name = "helpButton";
			this.helpButton.Click += new System.EventHandler(this.HelpButton_Click);
			// 
			// panel_filterExpression
			// 
			this.panel_filterExpression.Controls.Add(this.filterExpressionControl);
			resources.ApplyResources(this.panel_filterExpression, "panel_filterExpression");
			this.panel_filterExpression.Name = "panel_filterExpression";
			// 
			// filterExpressionControl
			// 
			this.filterExpressionControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.filterExpressionControl, "filterExpressionControl");
			this.filterExpressionControl.Name = "filterExpressionControl";
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
			// toolStripMenuItem_yellow
			// 
			this.toolStripMenuItem_yellow.Name = "toolStripMenuItem_yellow";
			resources.ApplyResources(this.toolStripMenuItem_yellow, "toolStripMenuItem_yellow");
			// 
			// toolStripMenuItem_brightGreen
			// 
			this.toolStripMenuItem_brightGreen.Name = "toolStripMenuItem_brightGreen";
			resources.ApplyResources(this.toolStripMenuItem_brightGreen, "toolStripMenuItem_brightGreen");
			// 
			// toolStripMenuItem_turquoise
			// 
			this.toolStripMenuItem_turquoise.Name = "toolStripMenuItem_turquoise";
			resources.ApplyResources(this.toolStripMenuItem_turquoise, "toolStripMenuItem_turquoise");
			// 
			// toolStripMenuItem_pink
			// 
			this.toolStripMenuItem_pink.Name = "toolStripMenuItem_pink";
			resources.ApplyResources(this.toolStripMenuItem_pink, "toolStripMenuItem_pink");
			// 
			// toolStripMenuItem_blue
			// 
			this.toolStripMenuItem_blue.Name = "toolStripMenuItem_blue";
			resources.ApplyResources(this.toolStripMenuItem_blue, "toolStripMenuItem_blue");
			// 
			// toolStripMenuItem_red
			// 
			this.toolStripMenuItem_red.Name = "toolStripMenuItem_red";
			resources.ApplyResources(this.toolStripMenuItem_red, "toolStripMenuItem_red");
			// 
			// toolStripMenuItem_darkBlue
			// 
			this.toolStripMenuItem_darkBlue.Name = "toolStripMenuItem_darkBlue";
			resources.ApplyResources(this.toolStripMenuItem_darkBlue, "toolStripMenuItem_darkBlue");
			// 
			// toolStripMenuItem_teal
			// 
			this.toolStripMenuItem_teal.Name = "toolStripMenuItem_teal";
			resources.ApplyResources(this.toolStripMenuItem_teal, "toolStripMenuItem_teal");
			// 
			// toolStripMenuItem_green
			// 
			this.toolStripMenuItem_green.Name = "toolStripMenuItem_green";
			resources.ApplyResources(this.toolStripMenuItem_green, "toolStripMenuItem_green");
			// 
			// toolStripMenuItem_violet
			// 
			this.toolStripMenuItem_violet.Name = "toolStripMenuItem_violet";
			resources.ApplyResources(this.toolStripMenuItem_violet, "toolStripMenuItem_violet");
			// 
			// toolStripMenuItem_darkRed
			// 
			this.toolStripMenuItem_darkRed.Name = "toolStripMenuItem_darkRed";
			resources.ApplyResources(this.toolStripMenuItem_darkRed, "toolStripMenuItem_darkRed");
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
			// DisplayFilterControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel_body);
			this.Name = "DisplayFilterControl";
			this.panel_body.ResumeLayout(false);
			this.panel_body.PerformLayout();
			this.tabControl_filter.ResumeLayout(false);
			this.tabPage_content.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.content_groupBox.ResumeLayout(false);
			this.content_groupBox.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabPage_filters.ResumeLayout(false);
			this.panel8.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.panel9.ResumeLayout(false);
			this.panel10.ResumeLayout(false);
			this.panel5.ResumeLayout(false);
			this.panel5.PerformLayout();
			this.panel11.ResumeLayout(false);
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.tabPage_comments.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tabPage_contextInfo.ResumeLayout(false);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tabPage_segmentNumbers.ResumeLayout(false);
			this.sampleOptionsPanel.ResumeLayout(false);
			this.samplingOptions.ResumeLayout(false);
			this.tableLayoutPanel6.ResumeLayout(false);
			this.tableLayoutPanel6.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_randomSelect)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_maxCharsPerSegment)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_selectOneInEvery)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown_minCharsPerSegment)).EndInit();
			this.fuzzyValuesBetweenGroupBox.ResumeLayout(false);
			this.fuzzyValuesBetweenGroupBox.PerformLayout();
			this.userBoxGroupBox.ResumeLayout(false);
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.segmentsGroupBox.ResumeLayout(false);
			this.segmentsGroupBox.PerformLayout();
			this.numberOptionsGroupBox.ResumeLayout(false);
			this.numberOptionsGroupBox.PerformLayout();
			this.panel7.ResumeLayout(false);
			this.panel7.PerformLayout();
			this.tabPage_colors.ResumeLayout(false);
			this.colorLayoutPanel.ResumeLayout(false);
			this.toolStrip_filter.ResumeLayout(false);
			this.toolStrip_filter.PerformLayout();
			this.panel_filterExpression.ResumeLayout(false);
			this.panel_filterStatus.ResumeLayout(false);
			this.panel_filterStatusBar.ResumeLayout(false);
			this.panel_filterStatusBarMessage.ResumeLayout(false);
			this.panel_filterStatusBarImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_filterStatusBarImage)).EndInit();
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
		private TextBox dsiLocation_textbox;
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
		private GroupBox fuzzyValuesBetweenGroupBox;
		private TextBox fuzzyMin;
		private TextBox fuzzyMax;
		private Label label5;
		private GroupBox userBoxGroupBox;
		private Panel panel_spacer_02;
		private GroupBox segmentsGroupBox;
		private CheckBox containsTagsCheckBox;
		private CheckBox mergedAcross;
		private CheckBox mergedCheckbox;
		private CheckBox splitCheckBox;
		private CheckBox sourceSameBox;
		private CheckBox equalsCaseSensitive;
		private Panel panel_spacer_03;
		private GroupBox numberOptionsGroupBox;
		private Panel panel7;
		private TextBox segmentsBox;
		private Label label1;
		private RadioButton noneBtn;
		private RadioButton groupedBtn;
		private RadioButton oddBtn;
		private RadioButton evenBtn;
		private Panel panel_spacer_05;
		private TabPage tabPage_colors;
		private TableLayoutPanel colorLayoutPanel;
		private Label label6;
		private ListView colorsListView;
		private ToolStripMenuItem highlightColorsToolStripMenuItem;
		private ToolStripMenuItem toolStripMenuItem_yellow;
		private ToolStripMenuItem toolStripMenuItem_brightGreen;
		private ToolStripMenuItem toolStripMenuItem_turquoise;
		private ToolStripMenuItem toolStripMenuItem_pink;
		private ToolStripMenuItem toolStripMenuItem_blue;
		private ToolStripMenuItem toolStripMenuItem_red;
		private ToolStripMenuItem toolStripMenuItem_darkBlue;
		private ToolStripMenuItem toolStripMenuItem_teal;
		private ToolStripMenuItem toolStripMenuItem_green;
		private ToolStripMenuItem toolStripMenuItem_violet;
		private ToolStripMenuItem toolStripMenuItem_darkRed;
		private Panel panel_spacer_00;
		private Panel panel_spacer_01;
		private TableLayoutPanel tableLayoutPanel5;
		private TextBox createdByBox;
		private TextBox modifiedByBox;
		private CheckBox createdByCheck;
		private Label userLabel;
		private CheckBox modifiedByCheck;
		private Panel sampleOptionsPanel;
		private GroupBox samplingOptions;
		private RadioButton radioButton_selectOneInEvery;
		private RadioButton radioButton_randomlySelect;
		private TableLayoutPanel tableLayoutPanel6;
		private CheckBox checkBox_segmentSelection;
		private NumericUpDown numericUpDown_selectOneInEvery;
		private NumericUpDown numericUpDown_randomSelect;
		private CheckBox checkBox_minMaxCharsPerSegment;
		private Label label2;
		private NumericUpDown numericUpDown_minCharsPerSegment;
		private NumericUpDown numericUpDown_maxCharsPerSegment;
		private Label label3;
		private Label label4;
	}
}
