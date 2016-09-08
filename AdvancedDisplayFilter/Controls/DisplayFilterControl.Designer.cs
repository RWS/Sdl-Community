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
            this.textBox_target = new System.Windows.Forms.TextBox();
            this.checkBox_regularExpression = new System.Windows.Forms.CheckBox();
            this.label_tab_content_filter_message = new System.Windows.Forms.Label();
            this.tabPage_filters = new System.Windows.Forms.TabPage();
            this.panel8 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label_tab_filter_message = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.button_removeAll = new System.Windows.Forms.Button();
            this.button_remove = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.panel10 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label_filters_selected = new System.Windows.Forms.Label();
            this.panel11 = new System.Windows.Forms.Panel();
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
            this.tabPage_contextInfo = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label_documentStructure_message = new System.Windows.Forms.Label();
            this.listView_contextInfo = new System.Windows.Forms.ListView();
            this.columnHeader_code = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.linkLabel_contextInfoClearSelection = new System.Windows.Forms.LinkLabel();
            this.label_contextInfoSelected = new System.Windows.Forms.Label();
            this.imageList_filter = new System.Windows.Forms.ImageList(this.components);
            this.splitter_filterExpression = new System.Windows.Forms.Splitter();
            this.panel_buffer = new System.Windows.Forms.Panel();
            this.toolStrip_filter = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_applyFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_clearFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_saveFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_loadFilter = new System.Windows.Forms.ToolStripButton();
            this.panel_filterExpression = new System.Windows.Forms.Panel();
            this.panel_filterStatus = new System.Windows.Forms.Panel();
            this.panel_filterStatusBar = new System.Windows.Forms.Panel();
            this.panel_filterStatusBarMessage = new System.Windows.Forms.Panel();
            this.label_filterStatusBarMessage = new System.Windows.Forms.Label();
            this.panel_filterStatusBarImage = new System.Windows.Forms.Panel();
            this.pictureBox_filterStatusBarImage = new System.Windows.Forms.PictureBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_selected = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
            this.columnHeader_filtersSelected_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listView_available = new Sdl.Community.AdvancedDisplayFilter.Controls.ListViewExtended();
            this.columnHeader_filtersAvailable_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.filterExpressionControl = new Sdl.Community.AdvancedDisplayFilter.Controls.FilterExpression();
            this.panel_body.SuspendLayout();
            this.tabControl_filter.SuspendLayout();
            this.tabPage_content.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.checkBox_caseSensitive, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label_target, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_target, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_regularExpression, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label_tab_content_filter_message, 0, 0);
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
            this.textBox_source.TextChanged += new System.EventHandler(this.textBox_source_TextChanged);
            this.textBox_source.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_source_KeyUp);
            // 
            // checkBox_caseSensitive
            // 
            resources.ApplyResources(this.checkBox_caseSensitive, "checkBox_caseSensitive");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBox_caseSensitive, 2);
            this.checkBox_caseSensitive.Name = "checkBox_caseSensitive";
            this.checkBox_caseSensitive.UseVisualStyleBackColor = true;
            this.checkBox_caseSensitive.CheckedChanged += new System.EventHandler(this.checkBox_caseSensitive_CheckedChanged);
            // 
            // label_target
            // 
            resources.ApplyResources(this.label_target, "label_target");
            this.label_target.Name = "label_target";
            // 
            // textBox_target
            // 
            resources.ApplyResources(this.textBox_target, "textBox_target");
            this.textBox_target.Name = "textBox_target";
            this.textBox_target.TextChanged += new System.EventHandler(this.textBox_target_TextChanged);
            this.textBox_target.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_target_KeyUp);
            // 
            // checkBox_regularExpression
            // 
            resources.ApplyResources(this.checkBox_regularExpression, "checkBox_regularExpression");
            this.tableLayoutPanel1.SetColumnSpan(this.checkBox_regularExpression, 2);
            this.checkBox_regularExpression.Name = "checkBox_regularExpression";
            this.checkBox_regularExpression.UseVisualStyleBackColor = true;
            this.checkBox_regularExpression.CheckedChanged += new System.EventHandler(this.checkBox_regularExpression_CheckedChanged);
            // 
            // label_tab_content_filter_message
            // 
            resources.ApplyResources(this.label_tab_content_filter_message, "label_tab_content_filter_message");
            this.tableLayoutPanel1.SetColumnSpan(this.label_tab_content_filter_message, 2);
            this.label_tab_content_filter_message.Name = "label_tab_content_filter_message";
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
            this.button_removeAll.Click += new System.EventHandler(this.button_removeAll_Click);
            // 
            // button_remove
            // 
            resources.ApplyResources(this.button_remove, "button_remove");
            this.button_remove.Name = "button_remove";
            this.button_remove.UseVisualStyleBackColor = true;
            this.button_remove.Click += new System.EventHandler(this.button_remove_Click);
            // 
            // button_add
            // 
            resources.ApplyResources(this.button_add, "button_add");
            this.button_add.Name = "button_add";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
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
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // comboBox_commentSeverity
            // 
            resources.ApplyResources(this.comboBox_commentSeverity, "comboBox_commentSeverity");
            this.comboBox_commentSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_commentSeverity.FormattingEnabled = true;
            this.comboBox_commentSeverity.Name = "comboBox_commentSeverity";
            this.comboBox_commentSeverity.SelectedIndexChanged += new System.EventHandler(this.comboBox_commentSeverity_SelectedIndexChanged);
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
            this.textBox_commentText.TextChanged += new System.EventHandler(this.textBox_commentText_TextChanged);
            this.textBox_commentText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_commentText_KeyUp);
            // 
            // textBox_commentAuthor
            // 
            resources.ApplyResources(this.textBox_commentAuthor, "textBox_commentAuthor");
            this.textBox_commentAuthor.Name = "textBox_commentAuthor";
            this.textBox_commentAuthor.TextChanged += new System.EventHandler(this.textBox_commentAuthor_TextChanged);
            this.textBox_commentAuthor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_commentAuthor_KeyUp);
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
            // linkLabel_contextInfoClearSelection
            // 
            resources.ApplyResources(this.linkLabel_contextInfoClearSelection, "linkLabel_contextInfoClearSelection");
            this.linkLabel_contextInfoClearSelection.Name = "linkLabel_contextInfoClearSelection";
            this.linkLabel_contextInfoClearSelection.TabStop = true;
            this.linkLabel_contextInfoClearSelection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_contextInfoClearSelection_LinkClicked);
            // 
            // label_contextInfoSelected
            // 
            resources.ApplyResources(this.label_contextInfoSelected, "label_contextInfoSelected");
            this.label_contextInfoSelected.Name = "label_contextInfoSelected";
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
            this.toolStripButton_clearFilter,
            this.toolStripSeparator1,
            this.toolStripButton_saveFilter,
            this.toolStripButton_loadFilter});
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
            this.listView_available.Resize += new System.EventHandler(this.listView_available_Resize);
            // 
            // columnHeader_filtersAvailable_name
            // 
            resources.ApplyResources(this.columnHeader_filtersAvailable_name, "columnHeader_filtersAvailable_name");
            // 
            // filterExpressionControl
            // 
            this.filterExpressionControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.filterExpressionControl, "filterExpressionControl");
            this.filterExpressionControl.Name = "filterExpressionControl";
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
        private System.Windows.Forms.TabControl tabControl_filter;
        private System.Windows.Forms.TabPage tabPage_content;
        private System.Windows.Forms.CheckBox checkBox_regularExpression;
        public System.Windows.Forms.TextBox textBox_target;
        private System.Windows.Forms.Label label_source;
        private System.Windows.Forms.Label label_target;
        public System.Windows.Forms.TextBox textBox_source;
        private System.Windows.Forms.TabPage tabPage_filters;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label_filterStatusBarMessage;
        private System.Windows.Forms.TabPage tabPage_comments;
        private System.Windows.Forms.Label label_commentText;
        public System.Windows.Forms.TextBox textBox_commentText;
        private System.Windows.Forms.Label label_commentSeverity;
        private System.Windows.Forms.Label label_commentAuthor;
        public System.Windows.Forms.TextBox textBox_commentAuthor;
        private System.Windows.Forms.ComboBox comboBox_commentSeverity;
        private System.Windows.Forms.ToolStripButton toolStripButton_applyFilter;
        private System.Windows.Forms.ToolStripButton toolStripButton_clearFilter;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label_filters_selected;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label_fitlers_available;
        private System.Windows.Forms.Button button_remove;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox checkBox_caseSensitive;
        private System.Windows.Forms.Panel panel_filterStatusBar;
        private System.Windows.Forms.Panel panel_filterStatusBarImage;
        private System.Windows.Forms.PictureBox pictureBox_filterStatusBarImage;
        private System.Windows.Forms.ImageList imageList_filter;
        private System.Windows.Forms.Label label_tab_content_filter_message;
        private System.Windows.Forms.Label label_tab_comments_filter_message;
        private System.Windows.Forms.Button button_removeAll;
        private System.Windows.Forms.Panel panel_buffer;
        private System.Windows.Forms.Label label_tab_filter_message;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton_saveFilter;
        private System.Windows.Forms.ToolStripButton toolStripButton_loadFilter;
        private System.Windows.Forms.Panel panel_filterExpression;
        private System.Windows.Forms.Splitter splitter_filterExpression;
        private System.Windows.Forms.Panel panel_filterStatusBarMessage;
        private System.Windows.Forms.TabPage tabPage_contextInfo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label_documentStructure_message;
        private System.Windows.Forms.ListView listView_contextInfo;
        private System.Windows.Forms.ColumnHeader columnHeader_code;
        private System.Windows.Forms.ColumnHeader columnHeader_name;
        private System.Windows.Forms.ColumnHeader columnHeader_description;
        private ListViewExtended listView_selected;
        private ListViewExtended listView_available;
        private ColumnHeader columnHeader_filtersSelected_name;
        private ColumnHeader columnHeader_filtersAvailable_name;
        private FilterExpression filterExpressionControl;
        private LinkLabel linkLabel_contextInfoClearSelection;
        private Label label_contextInfoSelected;

    }
}
