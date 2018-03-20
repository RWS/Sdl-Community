using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs
{
    partial class Settings
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("General", 0, 0);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Language Rates", 11, 11);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Clients", 1, 1);
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("My Details", 8, 8);
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Activity Tracking", 9, 9);
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Quality Metrics", 12, 12);
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Backup", 10, 10);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.button_Close = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_help = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeView_main = new System.Windows.Forms.TreeView();
            this.imageList_settings_navigation = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel_general = new System.Windows.Forms.Panel();
            this.checkBox_include_unlisted_projects = new System.Windows.Forms.CheckBox();
            this.comboBox_default_project_group_by = new System.Windows.Forms.ComboBox();
            this.comboBox_default_activity_status = new System.Windows.Forms.ComboBox();
            this.comboBox_default_project_status = new System.Windows.Forms.ComboBox();
            this.comboBox_defaultActivityViewGroupsIsOn = new System.Windows.Forms.ComboBox();
            this.comboBox_default_currency = new System.Windows.Forms.ComboBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_quality_metrics = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.listView_quality_metrics = new System.Windows.Forms.ListView();
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_metrics = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeQualityMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel19 = new System.Windows.Forms.Panel();
            this.label_quality_metrics_count = new System.Windows.Forms.Label();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.panel16 = new System.Windows.Forms.Panel();
            this.treeView_metric_group = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_metric_group = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newMetricGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMetricGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeMetricGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.exportMetricGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList4 = new System.Windows.Forms.ImageList(this.components);
            this.panel7 = new System.Windows.Forms.Panel();
            this.linkLabel_set_as_default_metric_group = new System.Windows.Forms.LinkLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel18 = new System.Windows.Forms.Panel();
            this.label42 = new System.Windows.Forms.Label();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel17 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSplitButton_new_metric_group = new System.Windows.Forms.ToolStripSplitButton();
            this.sAEJ2450ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mQMCoreStandardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lISAQAMetricToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_metric_group_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_metric_group_delete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_qualityMetrics_add = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_qualityMetrics_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_qualityMetrics_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_metric_group_import = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_metric_group_export = new System.Windows.Forms.ToolStripButton();
            this.panel_backup = new System.Windows.Forms.Panel();
            this.linkLabel_create_a_backup_now = new System.Windows.Forms.LinkLabel();
            this.linkLabel_viewFoldersInWindowsExplorer = new System.Windows.Forms.LinkLabel();
            this.comboBox_backup_every_type = new System.Windows.Forms.ComboBox();
            this.numericUpDown_backup_every = new System.Windows.Forms.NumericUpDown();
            this.button_browse_backup_folder = new System.Windows.Forms.Button();
            this.textBox_backup_folder = new System.Windows.Forms.TextBox();
            this.label_backup_last = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel_clients = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox_companyFaxNumber = new System.Windows.Forms.TextBox();
            this.textBox_companyPhoneNumber = new System.Windows.Forms.TextBox();
            this.textBox_companyWebPageAddress = new System.Windows.Forms.TextBox();
            this.textBox_companyAddress = new System.Windows.Forms.TextBox();
            this.button_companyAddress = new System.Windows.Forms.Button();
            this.label30 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.textBox_companyEmail = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.textBox_company_contactName = new System.Windows.Forms.TextBox();
            this.textBox_companyVatCode = new System.Windows.Forms.TextBox();
            this.textBox_companyTaxCode = new System.Windows.Forms.TextBox();
            this.textBox_companyName = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox_auto_add_language_rate = new System.Windows.Forms.CheckBox();
            this.checkBox_auto_add_hourly_rate = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label_hourly_rate_message = new System.Windows.Forms.Label();
            this.label_pem_rate_message = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.comboBox_company_hourly_rate_currency = new System.Windows.Forms.ComboBox();
            this.label29 = new System.Windows.Forms.Label();
            this.comboBox_company_pem_rate = new System.Windows.Forms.ComboBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label_company_pem_currency = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.numericUpDown_company_hourly_rate = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.comboBox_comparisonType = new System.Windows.Forms.ComboBox();
            this.label33 = new System.Windows.Forms.Label();
            this.checkBox_group_changes = new System.Windows.Forms.CheckBox();
            this.label32 = new System.Windows.Forms.Label();
            this.linkLabel_reset_differences_formatting = new System.Windows.Forms.LinkLabel();
            this.label31 = new System.Windows.Forms.Label();
            this.button_changeFormattingTagRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTagNew = new System.Windows.Forms.Button();
            this.button_changeFormattingTextRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTextNew = new System.Windows.Forms.Button();
            this.richTextBox_formatting_Tag_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Tag_New = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_New = new System.Windows.Forms.RichTextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label16 = new System.Windows.Forms.Label();
            this.checkBox_includeTagsInComparison = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.comboBox_quality_metric_groups = new System.Windows.Forms.ComboBox();
            this.label38 = new System.Windows.Forms.Label();
            this.imageList_client_tabcontrol = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel_clients_navigation_1 = new System.Windows.Forms.Panel();
            this.treeView_clients = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_clients = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList_clients = new System.Windows.Forms.ImageList(this.components);
            this.panel12 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.toolStrip_clients = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_addClient = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_deleteClient = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_editActivityRate = new System.Windows.Forms.ToolStripButton();
            this.panel_activity_tracking = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.numericUpDown_idleTimeOutMinutes = new System.Windows.Forms.NumericUpDown();
            this.checkBox_idleTimeOutShow = new System.Windows.Forms.CheckBox();
            this.checkBox_idleTimeOut = new System.Windows.Forms.CheckBox();
            this.checkBox_startOnLoad = new System.Windows.Forms.CheckBox();
            this.checkBox_trackerConfirmActivities = new System.Windows.Forms.CheckBox();
            this.checkBox_recordKeyStokes = new System.Windows.Forms.CheckBox();
            this.checkBox_warningMessageActivityTrackingNotRunning = new System.Windows.Forms.CheckBox();
            this.checkBox_autoStartTrackingOnDocumentOpenEvent = new System.Windows.Forms.CheckBox();
            this.checkBox_record_segments_that_are_updated = new System.Windows.Forms.CheckBox();
            this.checkBox_recordNonUpdatedSegments = new System.Windows.Forms.CheckBox();
            this.panel_language_rates = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.listView_price_groups = new System.Windows.Forms.ListView();
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader23 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_price_groups_listview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePriceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.analysisBandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panel13 = new System.Windows.Forms.Panel();
            this.treeView_price_groups = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_price_groups_treeview = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsDefaultPriceGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.panel14 = new System.Windows.Forms.Panel();
            this.label18 = new System.Windows.Forms.Label();
            this.panel15 = new System.Windows.Forms.Panel();
            this.toolStrip_ProjectsBlocks = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_priceGroup_new = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_priceGroup_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_priceGroup_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_groupPrice_add = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripButton_groupPrice_addMultiple = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_groupPrice_edit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_groupPrice_remove = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_analysisBandPercentage = new System.Windows.Forms.ToolStripButton();
            this.panel_my_info = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_webPageAddress = new System.Windows.Forms.Label();
            this.textBox_userFaxNumber = new System.Windows.Forms.TextBox();
            this.textBox_userPhoneNumber = new System.Windows.Forms.TextBox();
            this.textBox_userInternetWebPageAddress = new System.Windows.Forms.TextBox();
            this.textBox_userAddress = new System.Windows.Forms.TextBox();
            this.button_userAddress = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox_userInternetEmail = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_userVatCode = new System.Windows.Forms.TextBox();
            this.textBox_userTaxCode = new System.Windows.Forms.TextBox();
            this.textBox_userName = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.textBox_userCompanyName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip_client_rates = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editActivityRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList_activity = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip_activity = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel_general.SuspendLayout();
            this.panel_quality_metrics.SuspendLayout();
            this.panel9.SuspendLayout();
            this.contextMenuStrip_metrics.SuspendLayout();
            this.panel19.SuspendLayout();
            this.panel16.SuspendLayout();
            this.contextMenuStrip_metric_group.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel18.SuspendLayout();
            this.panel17.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel_backup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backup_every)).BeginInit();
            this.panel_clients.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_hourly_rate)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel_clients_navigation_1.SuspendLayout();
            this.contextMenuStrip_clients.SuspendLayout();
            this.panel12.SuspendLayout();
            this.toolStrip_clients.SuspendLayout();
            this.panel_activity_tracking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_idleTimeOutMinutes)).BeginInit();
            this.panel_language_rates.SuspendLayout();
            this.panel11.SuspendLayout();
            this.contextMenuStrip_price_groups_listview.SuspendLayout();
            this.panel13.SuspendLayout();
            this.contextMenuStrip_price_groups_treeview.SuspendLayout();
            this.panel14.SuspendLayout();
            this.panel15.SuspendLayout();
            this.toolStrip_ProjectsBlocks.SuspendLayout();
            this.panel_my_info.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.contextMenuStrip_client_rates.SuspendLayout();
            this.contextMenuStrip_activity.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.button_help);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 533);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1239, 38);
            this.panel1.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.button_Close);
            this.panel8.Controls.Add(this.button_Save);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(1039, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(200, 38);
            this.panel8.TabIndex = 23;
            // 
            // button_Close
            // 
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Close.Location = new System.Drawing.Point(111, 8);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 22;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Save.Location = new System.Drawing.Point(34, 8);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 23;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // button_help
            // 
            this.button_help.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_help.Location = new System.Drawing.Point(5, 6);
            this.button_help.Name = "button_help";
            this.button_help.Size = new System.Drawing.Size(75, 23);
            this.button_help.TabIndex = 23;
            this.button_help.Text = "&Help";
            this.button_help.UseVisualStyleBackColor = true;
            this.button_help.Click += new System.EventHandler(this.button_help_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.treeView_main);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(231, 533);
            this.panel2.TabIndex = 1;
            // 
            // treeView_main
            // 
            this.treeView_main.BackColor = System.Drawing.Color.White;
            this.treeView_main.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_main.FullRowSelect = true;
            this.treeView_main.HideSelection = false;
            this.treeView_main.ImageIndex = 0;
            this.treeView_main.ImageList = this.imageList_settings_navigation;
            this.treeView_main.ItemHeight = 36;
            this.treeView_main.Location = new System.Drawing.Point(5, 5);
            this.treeView_main.Name = "treeView_main";
            treeNode8.ImageIndex = 0;
            treeNode8.Name = "Node_general";
            treeNode8.SelectedImageIndex = 0;
            treeNode8.Text = "General";
            treeNode9.ImageIndex = 11;
            treeNode9.Name = "Node_language_rates";
            treeNode9.SelectedImageIndex = 11;
            treeNode9.Text = "Language Rates";
            treeNode10.ImageIndex = 1;
            treeNode10.Name = "Node_clients";
            treeNode10.SelectedImageIndex = 1;
            treeNode10.Text = "Clients";
            treeNode11.ImageIndex = 8;
            treeNode11.Name = "Node_my_info";
            treeNode11.SelectedImageIndex = 8;
            treeNode11.Text = "My Details";
            treeNode12.ImageIndex = 9;
            treeNode12.Name = "Node_activity_tracking";
            treeNode12.SelectedImageIndex = 9;
            treeNode12.Text = "Activity Tracking";
            treeNode13.ImageIndex = 12;
            treeNode13.Name = "Node_quality_metrics";
            treeNode13.SelectedImageIndex = 12;
            treeNode13.Text = "Quality Metrics";
            treeNode14.ImageIndex = 10;
            treeNode14.Name = "Node_backup";
            treeNode14.SelectedImageIndex = 10;
            treeNode14.Text = "Backup";
            this.treeView_main.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11,
            treeNode12,
            treeNode13,
            treeNode14});
            this.treeView_main.SelectedImageIndex = 0;
            this.treeView_main.Size = new System.Drawing.Size(221, 523);
            this.treeView_main.TabIndex = 21;
            this.treeView_main.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_main_AfterSelect);
            // 
            // imageList_settings_navigation
            // 
            this.imageList_settings_navigation.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_settings_navigation.ImageStream")));
            this.imageList_settings_navigation.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_settings_navigation.Images.SetKeyName(0, "Setting-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(1, "Users-simple-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(2, "Book-hand-drawn-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(3, "Gnome-Task-Due-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(4, "Emblem-Money-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(5, "Moleskine My Pilipinas-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(6, "my_computer_64.png");
            this.imageList_settings_navigation.Images.SetKeyName(7, "Lego Crash Test Dummy-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(8, "Medical-invoice-information-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(9, "Backup-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(10, "HDD-Green-Backup-64.png");
            this.imageList_settings_navigation.Images.SetKeyName(11, "Coins-64x64.png");
            this.imageList_settings_navigation.Images.SetKeyName(12, "Tasks-48.png");
            this.imageList_settings_navigation.Images.SetKeyName(13, "");
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel_general);
            this.panel3.Controls.Add(this.panel_quality_metrics);
            this.panel3.Controls.Add(this.panel_backup);
            this.panel3.Controls.Add(this.panel_clients);
            this.panel3.Controls.Add(this.panel_activity_tracking);
            this.panel3.Controls.Add(this.panel_language_rates);
            this.panel3.Controls.Add(this.panel_my_info);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(231, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(1008, 533);
            this.panel3.TabIndex = 2;
            // 
            // panel_general
            // 
            this.panel_general.Controls.Add(this.checkBox_include_unlisted_projects);
            this.panel_general.Controls.Add(this.comboBox_default_project_group_by);
            this.panel_general.Controls.Add(this.comboBox_default_activity_status);
            this.panel_general.Controls.Add(this.comboBox_default_project_status);
            this.panel_general.Controls.Add(this.comboBox_defaultActivityViewGroupsIsOn);
            this.panel_general.Controls.Add(this.comboBox_default_currency);
            this.panel_general.Controls.Add(this.label34);
            this.panel_general.Controls.Add(this.label12);
            this.panel_general.Controls.Add(this.label36);
            this.panel_general.Controls.Add(this.label11);
            this.panel_general.Controls.Add(this.label1);
            this.panel_general.Controls.Add(this.label35);
            this.panel_general.Controls.Add(this.label10);
            this.panel_general.Controls.Add(this.label3);
            this.panel_general.Location = new System.Drawing.Point(676, 271);
            this.panel_general.Name = "panel_general";
            this.panel_general.Size = new System.Drawing.Size(205, 153);
            this.panel_general.TabIndex = 38;
            // 
            // checkBox_include_unlisted_projects
            // 
            this.checkBox_include_unlisted_projects.AutoSize = true;
            this.checkBox_include_unlisted_projects.Location = new System.Drawing.Point(190, 85);
            this.checkBox_include_unlisted_projects.Name = "checkBox_include_unlisted_projects";
            this.checkBox_include_unlisted_projects.Size = new System.Drawing.Size(100, 17);
            this.checkBox_include_unlisted_projects.TabIndex = 2;
            this.checkBox_include_unlisted_projects.Text = "Include unlisted";
            this.checkBox_include_unlisted_projects.UseVisualStyleBackColor = true;
            // 
            // comboBox_default_project_group_by
            // 
            this.comboBox_default_project_group_by.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_project_group_by.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox_default_project_group_by.FormattingEnabled = true;
            this.comboBox_default_project_group_by.Items.AddRange(new object[] {
            "Client name",
            "Project name",
            "Project created",
            "Project due"});
            this.comboBox_default_project_group_by.Location = new System.Drawing.Point(190, 130);
            this.comboBox_default_project_group_by.Name = "comboBox_default_project_group_by";
            this.comboBox_default_project_group_by.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_project_group_by.TabIndex = 5;
            this.comboBox_default_project_group_by.SelectedIndexChanged += new System.EventHandler(this.comboBox_default_project_group_by_SelectedIndexChanged);
            // 
            // comboBox_default_activity_status
            // 
            this.comboBox_default_activity_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_activity_status.FormattingEnabled = true;
            this.comboBox_default_activity_status.Items.AddRange(new object[] {
            "Show all activities",
            "New",
            "Confirmed"});
            this.comboBox_default_activity_status.Location = new System.Drawing.Point(190, 105);
            this.comboBox_default_activity_status.Name = "comboBox_default_activity_status";
            this.comboBox_default_activity_status.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_activity_status.TabIndex = 3;
            this.comboBox_default_activity_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_default_activity_status_SelectedIndexChanged);
            // 
            // comboBox_default_project_status
            // 
            this.comboBox_default_project_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_project_status.FormattingEnabled = true;
            this.comboBox_default_project_status.Items.AddRange(new object[] {
            "Show all projects",
            "In progress",
            "Completed"});
            this.comboBox_default_project_status.Location = new System.Drawing.Point(190, 58);
            this.comboBox_default_project_status.Name = "comboBox_default_project_status";
            this.comboBox_default_project_status.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_project_status.TabIndex = 1;
            this.comboBox_default_project_status.SelectedIndexChanged += new System.EventHandler(this.comboBox_default_project_status_SelectedIndexChanged);
            // 
            // comboBox_defaultActivityViewGroupsIsOn
            // 
            this.comboBox_defaultActivityViewGroupsIsOn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_defaultActivityViewGroupsIsOn.FormattingEnabled = true;
            this.comboBox_defaultActivityViewGroupsIsOn.Items.AddRange(new object[] {
            "Turn on grouping",
            "Turn off grouping"});
            this.comboBox_defaultActivityViewGroupsIsOn.Location = new System.Drawing.Point(190, 202);
            this.comboBox_defaultActivityViewGroupsIsOn.Name = "comboBox_defaultActivityViewGroupsIsOn";
            this.comboBox_defaultActivityViewGroupsIsOn.Size = new System.Drawing.Size(187, 21);
            this.comboBox_defaultActivityViewGroupsIsOn.TabIndex = 7;
            this.comboBox_defaultActivityViewGroupsIsOn.SelectedIndexChanged += new System.EventHandler(this.comboBox_defaultActivityViewGroupsIsOn_SelectedIndexChanged);
            // 
            // comboBox_default_currency
            // 
            this.comboBox_default_currency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_currency.FormattingEnabled = true;
            this.comboBox_default_currency.Location = new System.Drawing.Point(190, 280);
            this.comboBox_default_currency.Name = "comboBox_default_currency";
            this.comboBox_default_currency.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_currency.Sorted = true;
            this.comboBox_default_currency.TabIndex = 7;
            this.comboBox_default_currency.SelectedIndexChanged += new System.EventHandler(this.comboBox_default_currency_SelectedIndexChanged);
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(77, 108);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(111, 13);
            this.label34.TabIndex = 3;
            this.label34.Text = "Default activity status:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(52, 133);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(135, 13);
            this.label12.TabIndex = 4;
            this.label12.Text = "Default project grouped by:";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(66, 205);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(115, 13);
            this.label36.TabIndex = 6;
            this.label36.Text = "Default activity groups:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(77, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Default project status:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 283);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Default curreny:";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label35.Location = new System.Drawing.Point(31, 174);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(112, 13);
            this.label35.TabIndex = 88;
            this.label35.Text = "Project Activity Viewer";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label10.Location = new System.Drawing.Point(31, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 13);
            this.label10.TabIndex = 88;
            this.label10.Text = "Navigation Filter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(31, 253);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "Misselanous Settings";
            // 
            // panel_quality_metrics
            // 
            this.panel_quality_metrics.Controls.Add(this.panel9);
            this.panel_quality_metrics.Controls.Add(this.panel17);
            this.panel_quality_metrics.Location = new System.Drawing.Point(480, 271);
            this.panel_quality_metrics.Name = "panel_quality_metrics";
            this.panel_quality_metrics.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_quality_metrics.Size = new System.Drawing.Size(189, 151);
            this.panel_quality_metrics.TabIndex = 43;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.listView_quality_metrics);
            this.panel9.Controls.Add(this.panel19);
            this.panel9.Controls.Add(this.splitter4);
            this.panel9.Controls.Add(this.panel16);
            this.panel9.Controls.Add(this.splitter3);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel9.Location = new System.Drawing.Point(0, 51);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(189, 100);
            this.panel9.TabIndex = 1;
            // 
            // listView_quality_metrics
            // 
            this.listView_quality_metrics.AllowDrop = true;
            this.listView_quality_metrics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader27,
            this.columnHeader28,
            this.columnHeader1});
            this.listView_quality_metrics.ContextMenuStrip = this.contextMenuStrip_metrics;
            this.listView_quality_metrics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_quality_metrics.FullRowSelect = true;
            this.listView_quality_metrics.GridLines = true;
            this.listView_quality_metrics.HideSelection = false;
            this.listView_quality_metrics.Location = new System.Drawing.Point(169, 0);
            this.listView_quality_metrics.Name = "listView_quality_metrics";
            this.listView_quality_metrics.ShowItemToolTips = true;
            this.listView_quality_metrics.Size = new System.Drawing.Size(20, 72);
            this.listView_quality_metrics.TabIndex = 6;
            this.listView_quality_metrics.UseCompatibleStateImageBehavior = false;
            this.listView_quality_metrics.View = System.Windows.Forms.View.Details;
            this.listView_quality_metrics.SelectedIndexChanged += new System.EventHandler(this.listView_quality_metrics_SelectedIndexChanged);
            this.listView_quality_metrics.DoubleClick += new System.EventHandler(this.listView_quality_metrics_DoubleClick);
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "Name";
            this.columnHeader27.Width = 280;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "Default Severity";
            this.columnHeader28.Width = 150;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Description";
            this.columnHeader1.Width = 350;
            // 
            // contextMenuStrip_metrics
            // 
            this.contextMenuStrip_metrics.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addQualityMetricToolStripMenuItem,
            this.editQualityMetricToolStripMenuItem,
            this.removeQualityMetricToolStripMenuItem});
            this.contextMenuStrip_metrics.Name = "contextMenuStrip_metrics";
            this.contextMenuStrip_metrics.Size = new System.Drawing.Size(194, 70);
            // 
            // addQualityMetricToolStripMenuItem
            // 
            this.addQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addQualityMetricToolStripMenuItem.Image")));
            this.addQualityMetricToolStripMenuItem.Name = "addQualityMetricToolStripMenuItem";
            this.addQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.addQualityMetricToolStripMenuItem.Text = "Add quality metric";
            this.addQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.addQualityMetricToolStripMenuItem_Click);
            // 
            // editQualityMetricToolStripMenuItem
            // 
            this.editQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editQualityMetricToolStripMenuItem.Image")));
            this.editQualityMetricToolStripMenuItem.Name = "editQualityMetricToolStripMenuItem";
            this.editQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.editQualityMetricToolStripMenuItem.Text = "Edit quality metric";
            this.editQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.editQualityMetricToolStripMenuItem_Click);
            // 
            // removeQualityMetricToolStripMenuItem
            // 
            this.removeQualityMetricToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeQualityMetricToolStripMenuItem.Image")));
            this.removeQualityMetricToolStripMenuItem.Name = "removeQualityMetricToolStripMenuItem";
            this.removeQualityMetricToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.removeQualityMetricToolStripMenuItem.Text = "Remove quality metric";
            this.removeQualityMetricToolStripMenuItem.Click += new System.EventHandler(this.removeQualityMetricToolStripMenuItem_Click);
            // 
            // panel19
            // 
            this.panel19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel19.Controls.Add(this.label_quality_metrics_count);
            this.panel19.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel19.Location = new System.Drawing.Point(169, 72);
            this.panel19.Name = "panel19";
            this.panel19.Size = new System.Drawing.Size(20, 28);
            this.panel19.TabIndex = 6;
            // 
            // label_quality_metrics_count
            // 
            this.label_quality_metrics_count.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_quality_metrics_count.Location = new System.Drawing.Point(-168, 0);
            this.label_quality_metrics_count.Name = "label_quality_metrics_count";
            this.label_quality_metrics_count.Size = new System.Drawing.Size(186, 26);
            this.label_quality_metrics_count.TabIndex = 0;
            this.label_quality_metrics_count.Text = "Items: 0 ";
            this.label_quality_metrics_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // splitter4
            // 
            this.splitter4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter4.Location = new System.Drawing.Point(166, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 100);
            this.splitter4.TabIndex = 5;
            this.splitter4.TabStop = false;
            // 
            // panel16
            // 
            this.panel16.Controls.Add(this.treeView_metric_group);
            this.panel16.Controls.Add(this.panel7);
            this.panel16.Controls.Add(this.panel18);
            this.panel16.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel16.Location = new System.Drawing.Point(3, 0);
            this.panel16.Name = "panel16";
            this.panel16.Size = new System.Drawing.Size(163, 100);
            this.panel16.TabIndex = 4;
            // 
            // treeView_metric_group
            // 
            this.treeView_metric_group.BackColor = System.Drawing.Color.White;
            this.treeView_metric_group.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView_metric_group.ContextMenuStrip = this.contextMenuStrip_metric_group;
            this.treeView_metric_group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_metric_group.FullRowSelect = true;
            this.treeView_metric_group.HideSelection = false;
            this.treeView_metric_group.ImageIndex = 2;
            this.treeView_metric_group.ImageList = this.imageList4;
            this.treeView_metric_group.ItemHeight = 28;
            this.treeView_metric_group.Location = new System.Drawing.Point(0, 26);
            this.treeView_metric_group.Name = "treeView_metric_group";
            this.treeView_metric_group.SelectedImageIndex = 2;
            this.treeView_metric_group.ShowNodeToolTips = true;
            this.treeView_metric_group.ShowRootLines = false;
            this.treeView_metric_group.Size = new System.Drawing.Size(163, 46);
            this.treeView_metric_group.TabIndex = 23;
            this.treeView_metric_group.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_metric_group_AfterSelect);
            this.treeView_metric_group.DoubleClick += new System.EventHandler(this.treeView_metric_group_DoubleClick);
            // 
            // contextMenuStrip_metric_group
            // 
            this.contextMenuStrip_metric_group.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMetricGroupToolStripMenuItem,
            this.editMetricGroupToolStripMenuItem,
            this.removeMetricGroupToolStripMenuItem,
            this.toolStripSeparator2,
            this.setAsDefaultToolStripMenuItem,
            this.toolStripSeparator8,
            this.exportMetricGroupToolStripMenuItem});
            this.contextMenuStrip_metric_group.Name = "contextMenuStrip_metric_group";
            this.contextMenuStrip_metric_group.Size = new System.Drawing.Size(190, 126);
            // 
            // newMetricGroupToolStripMenuItem
            // 
            this.newMetricGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newMetricGroupToolStripMenuItem.Image")));
            this.newMetricGroupToolStripMenuItem.Name = "newMetricGroupToolStripMenuItem";
            this.newMetricGroupToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.newMetricGroupToolStripMenuItem.Text = "New metric group";
            this.newMetricGroupToolStripMenuItem.Click += new System.EventHandler(this.newMetricGroupToolStripMenuItem_Click);
            // 
            // editMetricGroupToolStripMenuItem
            // 
            this.editMetricGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editMetricGroupToolStripMenuItem.Image")));
            this.editMetricGroupToolStripMenuItem.Name = "editMetricGroupToolStripMenuItem";
            this.editMetricGroupToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.editMetricGroupToolStripMenuItem.Text = "Edit metric group";
            this.editMetricGroupToolStripMenuItem.Click += new System.EventHandler(this.editMetricGroupToolStripMenuItem_Click);
            // 
            // removeMetricGroupToolStripMenuItem
            // 
            this.removeMetricGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeMetricGroupToolStripMenuItem.Image")));
            this.removeMetricGroupToolStripMenuItem.Name = "removeMetricGroupToolStripMenuItem";
            this.removeMetricGroupToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.removeMetricGroupToolStripMenuItem.Text = "Remove metric group";
            this.removeMetricGroupToolStripMenuItem.Click += new System.EventHandler(this.removeMetricGroupToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(186, 6);
            // 
            // setAsDefaultToolStripMenuItem
            // 
            this.setAsDefaultToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("setAsDefaultToolStripMenuItem.Image")));
            this.setAsDefaultToolStripMenuItem.Name = "setAsDefaultToolStripMenuItem";
            this.setAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.setAsDefaultToolStripMenuItem.Text = "Set as default";
            this.setAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.setAsDefaultToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(186, 6);
            // 
            // exportMetricGroupToolStripMenuItem
            // 
            this.exportMetricGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exportMetricGroupToolStripMenuItem.Image")));
            this.exportMetricGroupToolStripMenuItem.Name = "exportMetricGroupToolStripMenuItem";
            this.exportMetricGroupToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.exportMetricGroupToolStripMenuItem.Text = "Export metric group";
            this.exportMetricGroupToolStripMenuItem.Click += new System.EventHandler(this.exportMetricGroupToolStripMenuItem_Click);
            // 
            // imageList4
            // 
            this.imageList4.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList4.ImageStream")));
            this.imageList4.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList4.Images.SetKeyName(0, "Default");
            this.imageList4.Images.SetKeyName(1, "Normal");
            this.imageList4.Images.SetKeyName(2, "Empty");
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.linkLabel_set_as_default_metric_group);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 72);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(163, 28);
            this.panel7.TabIndex = 1;
            // 
            // linkLabel_set_as_default_metric_group
            // 
            this.linkLabel_set_as_default_metric_group.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabel_set_as_default_metric_group.ImageIndex = 0;
            this.linkLabel_set_as_default_metric_group.ImageList = this.imageList1;
            this.linkLabel_set_as_default_metric_group.Location = new System.Drawing.Point(4, 2);
            this.linkLabel_set_as_default_metric_group.Name = "linkLabel_set_as_default_metric_group";
            this.linkLabel_set_as_default_metric_group.Size = new System.Drawing.Size(91, 22);
            this.linkLabel_set_as_default_metric_group.TabIndex = 0;
            this.linkLabel_set_as_default_metric_group.TabStop = true;
            this.linkLabel_set_as_default_metric_group.Text = "Set as default";
            this.linkLabel_set_as_default_metric_group.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel_set_as_default_metric_group.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_set_as_default_metric_group_LinkClicked);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Map-Marker-Chequered-Flag-Right-Pink-32.png");
            // 
            // panel18
            // 
            this.panel18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel18.Controls.Add(this.label42);
            this.panel18.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel18.Location = new System.Drawing.Point(0, 0);
            this.panel18.Name = "panel18";
            this.panel18.Size = new System.Drawing.Size(163, 26);
            this.panel18.TabIndex = 0;
            // 
            // label42
            // 
            this.label42.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label42.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label42.Location = new System.Drawing.Point(0, 0);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(161, 24);
            this.label42.TabIndex = 0;
            this.label42.Text = "Quality Metric Groups";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter3
            // 
            this.splitter3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter3.Location = new System.Drawing.Point(0, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(3, 100);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // panel17
            // 
            this.panel17.Controls.Add(this.toolStrip1);
            this.panel17.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel17.Location = new System.Drawing.Point(0, 15);
            this.panel17.Name = "panel17";
            this.panel17.Size = new System.Drawing.Size(189, 36);
            this.panel17.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSplitButton_new_metric_group,
            this.toolStripButton_metric_group_edit,
            this.toolStripButton_metric_group_delete,
            this.toolStripSeparator6,
            this.toolStripButton_qualityMetrics_add,
            this.toolStripButton_qualityMetrics_edit,
            this.toolStripButton_qualityMetrics_remove,
            this.toolStripSeparator7,
            this.toolStripButton_metric_group_import,
            this.toolStripButton_metric_group_export});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(189, 31);
            this.toolStrip1.TabIndex = 8;
            // 
            // toolStripSplitButton_new_metric_group
            // 
            this.toolStripSplitButton_new_metric_group.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sAEJ2450ToolStripMenuItem,
            this.mQMCoreStandardToolStripMenuItem,
            this.lISAQAMetricToolStripMenuItem});
            this.toolStripSplitButton_new_metric_group.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton_new_metric_group.Image")));
            this.toolStripSplitButton_new_metric_group.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton_new_metric_group.Name = "toolStripSplitButton_new_metric_group";
            this.toolStripSplitButton_new_metric_group.Size = new System.Drawing.Size(71, 28);
            this.toolStripSplitButton_new_metric_group.Text = "New";
            this.toolStripSplitButton_new_metric_group.ButtonClick += new System.EventHandler(this.toolStripSplitButton_new_metric_group_ButtonClick);
            // 
            // sAEJ2450ToolStripMenuItem
            // 
            this.sAEJ2450ToolStripMenuItem.Name = "sAEJ2450ToolStripMenuItem";
            this.sAEJ2450ToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.sAEJ2450ToolStripMenuItem.Text = "SAE J2450";
            this.sAEJ2450ToolStripMenuItem.Click += new System.EventHandler(this.sAEJ2450ToolStripMenuItem_Click);
            // 
            // mQMCoreStandardToolStripMenuItem
            // 
            this.mQMCoreStandardToolStripMenuItem.Name = "mQMCoreStandardToolStripMenuItem";
            this.mQMCoreStandardToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.mQMCoreStandardToolStripMenuItem.Text = "MQM Core";
            this.mQMCoreStandardToolStripMenuItem.Click += new System.EventHandler(this.mQMCoreStandardToolStripMenuItem_Click);
            // 
            // lISAQAMetricToolStripMenuItem
            // 
            this.lISAQAMetricToolStripMenuItem.Name = "lISAQAMetricToolStripMenuItem";
            this.lISAQAMetricToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.lISAQAMetricToolStripMenuItem.Text = "LISA QA Metric";
            this.lISAQAMetricToolStripMenuItem.Click += new System.EventHandler(this.lISAQAMetricToolStripMenuItem_Click);
            // 
            // toolStripButton_metric_group_edit
            // 
            this.toolStripButton_metric_group_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_metric_group_edit.Image")));
            this.toolStripButton_metric_group_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_metric_group_edit.Name = "toolStripButton_metric_group_edit";
            this.toolStripButton_metric_group_edit.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_metric_group_edit.Text = "Edit";
            this.toolStripButton_metric_group_edit.Click += new System.EventHandler(this.toolStripButton_metric_group_edit_Click);
            // 
            // toolStripButton_metric_group_delete
            // 
            this.toolStripButton_metric_group_delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_metric_group_delete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_metric_group_delete.Image")));
            this.toolStripButton_metric_group_delete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_metric_group_delete.Name = "toolStripButton_metric_group_delete";
            this.toolStripButton_metric_group_delete.Size = new System.Drawing.Size(28, 28);
            this.toolStripButton_metric_group_delete.Text = "Delete";
            this.toolStripButton_metric_group_delete.Click += new System.EventHandler(this.toolStripButton_metric_group_delete_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_qualityMetrics_add
            // 
            this.toolStripButton_qualityMetrics_add.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_qualityMetrics_add.Image")));
            this.toolStripButton_qualityMetrics_add.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_qualityMetrics_add.Name = "toolStripButton_qualityMetrics_add";
            this.toolStripButton_qualityMetrics_add.Size = new System.Drawing.Size(57, 28);
            this.toolStripButton_qualityMetrics_add.Text = "Add";
            this.toolStripButton_qualityMetrics_add.Click += new System.EventHandler(this.toolStripButton_qualityMetrics_add_Click);
            // 
            // toolStripButton_qualityMetrics_edit
            // 
            this.toolStripButton_qualityMetrics_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_qualityMetrics_edit.Image")));
            this.toolStripButton_qualityMetrics_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_qualityMetrics_edit.Name = "toolStripButton_qualityMetrics_edit";
            this.toolStripButton_qualityMetrics_edit.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_qualityMetrics_edit.Text = "Edit";
            this.toolStripButton_qualityMetrics_edit.Click += new System.EventHandler(this.toolStripButton_qualityMetrics_edit_Click);
            // 
            // toolStripButton_qualityMetrics_remove
            // 
            this.toolStripButton_qualityMetrics_remove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_qualityMetrics_remove.Image")));
            this.toolStripButton_qualityMetrics_remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_qualityMetrics_remove.Name = "toolStripButton_qualityMetrics_remove";
            this.toolStripButton_qualityMetrics_remove.Size = new System.Drawing.Size(78, 28);
            this.toolStripButton_qualityMetrics_remove.Text = "Remove";
            this.toolStripButton_qualityMetrics_remove.Click += new System.EventHandler(this.toolStripButton_qualityMetrics_remove_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_metric_group_import
            // 
            this.toolStripButton_metric_group_import.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_metric_group_import.Image")));
            this.toolStripButton_metric_group_import.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_metric_group_import.Name = "toolStripButton_metric_group_import";
            this.toolStripButton_metric_group_import.Size = new System.Drawing.Size(71, 28);
            this.toolStripButton_metric_group_import.Text = "Import";
            this.toolStripButton_metric_group_import.ToolTipText = "Import Quality Metric Group";
            this.toolStripButton_metric_group_import.Click += new System.EventHandler(this.toolStripButton_metric_group_import_Click);
            // 
            // toolStripButton_metric_group_export
            // 
            this.toolStripButton_metric_group_export.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_metric_group_export.Image")));
            this.toolStripButton_metric_group_export.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_metric_group_export.Name = "toolStripButton_metric_group_export";
            this.toolStripButton_metric_group_export.Size = new System.Drawing.Size(68, 28);
            this.toolStripButton_metric_group_export.Text = "Export";
            this.toolStripButton_metric_group_export.ToolTipText = "Export Quality Metric Group";
            this.toolStripButton_metric_group_export.Click += new System.EventHandler(this.toolStripButton_metric_group_export_Click);
            // 
            // panel_backup
            // 
            this.panel_backup.Controls.Add(this.linkLabel_create_a_backup_now);
            this.panel_backup.Controls.Add(this.linkLabel_viewFoldersInWindowsExplorer);
            this.panel_backup.Controls.Add(this.comboBox_backup_every_type);
            this.panel_backup.Controls.Add(this.numericUpDown_backup_every);
            this.panel_backup.Controls.Add(this.button_browse_backup_folder);
            this.panel_backup.Controls.Add(this.textBox_backup_folder);
            this.panel_backup.Controls.Add(this.label_backup_last);
            this.panel_backup.Controls.Add(this.label15);
            this.panel_backup.Controls.Add(this.label14);
            this.panel_backup.Controls.Add(this.label13);
            this.panel_backup.Location = new System.Drawing.Point(272, 268);
            this.panel_backup.Name = "panel_backup";
            this.panel_backup.Size = new System.Drawing.Size(198, 154);
            this.panel_backup.TabIndex = 40;
            // 
            // linkLabel_create_a_backup_now
            // 
            this.linkLabel_create_a_backup_now.AutoSize = true;
            this.linkLabel_create_a_backup_now.Location = new System.Drawing.Point(116, 152);
            this.linkLabel_create_a_backup_now.Name = "linkLabel_create_a_backup_now";
            this.linkLabel_create_a_backup_now.Size = new System.Drawing.Size(112, 13);
            this.linkLabel_create_a_backup_now.TabIndex = 9;
            this.linkLabel_create_a_backup_now.TabStop = true;
            this.linkLabel_create_a_backup_now.Text = "Create a backup now!";
            this.linkLabel_create_a_backup_now.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_create_a_backup_now_LinkClicked);
            // 
            // linkLabel_viewFoldersInWindowsExplorer
            // 
            this.linkLabel_viewFoldersInWindowsExplorer.AutoSize = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Location = new System.Drawing.Point(116, 66);
            this.linkLabel_viewFoldersInWindowsExplorer.Name = "linkLabel_viewFoldersInWindowsExplorer";
            this.linkLabel_viewFoldersInWindowsExplorer.Size = new System.Drawing.Size(154, 13);
            this.linkLabel_viewFoldersInWindowsExplorer.TabIndex = 8;
            this.linkLabel_viewFoldersInWindowsExplorer.TabStop = true;
            this.linkLabel_viewFoldersInWindowsExplorer.Text = "View folder in windows explorer";
            this.linkLabel_viewFoldersInWindowsExplorer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_viewFoldersInWindowsExplorer_LinkClicked);
            // 
            // comboBox_backup_every_type
            // 
            this.comboBox_backup_every_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_backup_every_type.FormattingEnabled = true;
            this.comboBox_backup_every_type.Items.AddRange(new object[] {
            "Days",
            "Weeks"});
            this.comboBox_backup_every_type.Location = new System.Drawing.Point(183, 91);
            this.comboBox_backup_every_type.Name = "comboBox_backup_every_type";
            this.comboBox_backup_every_type.Size = new System.Drawing.Size(121, 21);
            this.comboBox_backup_every_type.TabIndex = 4;
            this.comboBox_backup_every_type.SelectedIndexChanged += new System.EventHandler(this.comboBox_backup_every_type_SelectedIndexChanged);
            // 
            // numericUpDown_backup_every
            // 
            this.numericUpDown_backup_every.Location = new System.Drawing.Point(118, 91);
            this.numericUpDown_backup_every.Name = "numericUpDown_backup_every";
            this.numericUpDown_backup_every.Size = new System.Drawing.Size(62, 20);
            this.numericUpDown_backup_every.TabIndex = 3;
            this.numericUpDown_backup_every.ValueChanged += new System.EventHandler(this.numericUpDown_backup_every_ValueChanged);
            // 
            // button_browse_backup_folder
            // 
            this.button_browse_backup_folder.Location = new System.Drawing.Point(598, 39);
            this.button_browse_backup_folder.Name = "button_browse_backup_folder";
            this.button_browse_backup_folder.Size = new System.Drawing.Size(27, 23);
            this.button_browse_backup_folder.TabIndex = 2;
            this.button_browse_backup_folder.Text = "...";
            this.button_browse_backup_folder.UseVisualStyleBackColor = true;
            this.button_browse_backup_folder.Click += new System.EventHandler(this.button_browse_backup_folder_Click);
            // 
            // textBox_backup_folder
            // 
            this.textBox_backup_folder.Location = new System.Drawing.Point(118, 40);
            this.textBox_backup_folder.Name = "textBox_backup_folder";
            this.textBox_backup_folder.Size = new System.Drawing.Size(475, 20);
            this.textBox_backup_folder.TabIndex = 1;
            this.textBox_backup_folder.TextChanged += new System.EventHandler(this.textBox_backup_folder_TextChanged);
            // 
            // label_backup_last
            // 
            this.label_backup_last.AutoSize = true;
            this.label_backup_last.Location = new System.Drawing.Point(116, 121);
            this.label_backup_last.Name = "label_backup_last";
            this.label_backup_last.Size = new System.Drawing.Size(34, 13);
            this.label_backup_last.TabIndex = 0;
            this.label_backup_last.Text = "[date]";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(45, 121);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Last backup:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(38, 95);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Backup every:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(38, 44);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(76, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Backup folder:";
            // 
            // panel_clients
            // 
            this.panel_clients.Controls.Add(this.panel6);
            this.panel_clients.Controls.Add(this.splitter1);
            this.panel_clients.Controls.Add(this.panel_clients_navigation_1);
            this.panel_clients.Controls.Add(this.toolStrip_clients);
            this.panel_clients.Location = new System.Drawing.Point(272, 102);
            this.panel_clients.Name = "panel_clients";
            this.panel_clients.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_clients.Size = new System.Drawing.Size(195, 153);
            this.panel_clients.TabIndex = 38;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Transparent;
            this.panel6.Controls.Add(this.tabControl1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(241, 46);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(0, 107);
            this.panel6.TabIndex = 4;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.ImageList = this.imageList_client_tabcontrol;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(0, 107);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label19);
            this.tabPage1.Controls.Add(this.label20);
            this.tabPage1.Controls.Add(this.label21);
            this.tabPage1.Controls.Add(this.label22);
            this.tabPage1.Controls.Add(this.textBox_companyFaxNumber);
            this.tabPage1.Controls.Add(this.textBox_companyPhoneNumber);
            this.tabPage1.Controls.Add(this.textBox_companyWebPageAddress);
            this.tabPage1.Controls.Add(this.textBox_companyAddress);
            this.tabPage1.Controls.Add(this.button_companyAddress);
            this.tabPage1.Controls.Add(this.label30);
            this.tabPage1.Controls.Add(this.label23);
            this.tabPage1.Controls.Add(this.textBox_companyEmail);
            this.tabPage1.Controls.Add(this.label24);
            this.tabPage1.Controls.Add(this.textBox_company_contactName);
            this.tabPage1.Controls.Add(this.textBox_companyVatCode);
            this.tabPage1.Controls.Add(this.textBox_companyTaxCode);
            this.tabPage1.Controls.Add(this.textBox_companyName);
            this.tabPage1.Controls.Add(this.label25);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(0, 72);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Details";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(65, 236);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(39, 13);
            this.label19.TabIndex = 27;
            this.label19.Text = "E-Mail:";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(37, 318);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(67, 13);
            this.label20.TabIndex = 33;
            this.label20.Text = "Fax Number:";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(23, 289);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(81, 13);
            this.label21.TabIndex = 31;
            this.label21.Text = "Phone Number:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(44, 263);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(60, 13);
            this.label22.TabIndex = 29;
            this.label22.Text = "Web page:";
            // 
            // textBox_companyFaxNumber
            // 
            this.textBox_companyFaxNumber.Location = new System.Drawing.Point(111, 314);
            this.textBox_companyFaxNumber.Name = "textBox_companyFaxNumber";
            this.textBox_companyFaxNumber.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyFaxNumber.TabIndex = 34;
            this.textBox_companyFaxNumber.TextChanged += new System.EventHandler(this.textBox_companyFaxNumber_TextChanged);
            // 
            // textBox_companyPhoneNumber
            // 
            this.textBox_companyPhoneNumber.Location = new System.Drawing.Point(111, 285);
            this.textBox_companyPhoneNumber.Name = "textBox_companyPhoneNumber";
            this.textBox_companyPhoneNumber.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyPhoneNumber.TabIndex = 32;
            this.textBox_companyPhoneNumber.TextChanged += new System.EventHandler(this.textBox_companyPhoneNumber_TextChanged);
            // 
            // textBox_companyWebPageAddress
            // 
            this.textBox_companyWebPageAddress.Location = new System.Drawing.Point(111, 259);
            this.textBox_companyWebPageAddress.Name = "textBox_companyWebPageAddress";
            this.textBox_companyWebPageAddress.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyWebPageAddress.TabIndex = 30;
            this.textBox_companyWebPageAddress.TextChanged += new System.EventHandler(this.textBox_companyWebPageAddress_TextChanged);
            // 
            // textBox_companyAddress
            // 
            this.textBox_companyAddress.Location = new System.Drawing.Point(113, 55);
            this.textBox_companyAddress.Multiline = true;
            this.textBox_companyAddress.Name = "textBox_companyAddress";
            this.textBox_companyAddress.ReadOnly = true;
            this.textBox_companyAddress.Size = new System.Drawing.Size(314, 84);
            this.textBox_companyAddress.TabIndex = 20;
            // 
            // button_companyAddress
            // 
            this.button_companyAddress.Location = new System.Drawing.Point(22, 56);
            this.button_companyAddress.Name = "button_companyAddress";
            this.button_companyAddress.Size = new System.Drawing.Size(83, 23);
            this.button_companyAddress.TabIndex = 19;
            this.button_companyAddress.Text = "Address...";
            this.button_companyAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_companyAddress.UseVisualStyleBackColor = true;
            this.button_companyAddress.Click += new System.EventHandler(this.button_companyAddress_Click);
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(26, 208);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(78, 13);
            this.label30.TabIndex = 25;
            this.label30.Text = "Contact Name:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(46, 183);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(59, 13);
            this.label23.TabIndex = 23;
            this.label23.Text = "VAT Code:";
            // 
            // textBox_companyEmail
            // 
            this.textBox_companyEmail.Location = new System.Drawing.Point(111, 232);
            this.textBox_companyEmail.Name = "textBox_companyEmail";
            this.textBox_companyEmail.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyEmail.TabIndex = 28;
            this.textBox_companyEmail.TextChanged += new System.EventHandler(this.textBox_companyEmail_TextChanged);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(46, 158);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(59, 13);
            this.label24.TabIndex = 21;
            this.label24.Text = "TAX Code:";
            // 
            // textBox_company_contactName
            // 
            this.textBox_company_contactName.Location = new System.Drawing.Point(112, 204);
            this.textBox_company_contactName.Name = "textBox_company_contactName";
            this.textBox_company_contactName.Size = new System.Drawing.Size(314, 20);
            this.textBox_company_contactName.TabIndex = 26;
            this.textBox_company_contactName.TextChanged += new System.EventHandler(this.textBox_company_contactName_TextChanged);
            // 
            // textBox_companyVatCode
            // 
            this.textBox_companyVatCode.Location = new System.Drawing.Point(113, 179);
            this.textBox_companyVatCode.Name = "textBox_companyVatCode";
            this.textBox_companyVatCode.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyVatCode.TabIndex = 24;
            this.textBox_companyVatCode.TextChanged += new System.EventHandler(this.textBox_companyVatCode_TextChanged);
            // 
            // textBox_companyTaxCode
            // 
            this.textBox_companyTaxCode.Location = new System.Drawing.Point(113, 154);
            this.textBox_companyTaxCode.Name = "textBox_companyTaxCode";
            this.textBox_companyTaxCode.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyTaxCode.TabIndex = 22;
            this.textBox_companyTaxCode.TextChanged += new System.EventHandler(this.textBox_companyTaxCode_TextChanged);
            // 
            // textBox_companyName
            // 
            this.textBox_companyName.Location = new System.Drawing.Point(112, 23);
            this.textBox_companyName.Name = "textBox_companyName";
            this.textBox_companyName.Size = new System.Drawing.Size(314, 20);
            this.textBox_companyName.TabIndex = 18;
            this.textBox_companyName.TextChanged += new System.EventHandler(this.textBox_companyName_TextChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(38, 26);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(67, 13);
            this.label25.TabIndex = 17;
            this.label25.Text = "Client Name:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox_auto_add_language_rate);
            this.tabPage2.Controls.Add(this.checkBox_auto_add_hourly_rate);
            this.tabPage2.Controls.Add(this.pictureBox2);
            this.tabPage2.Controls.Add(this.pictureBox3);
            this.tabPage2.Controls.Add(this.label39);
            this.tabPage2.Controls.Add(this.label_hourly_rate_message);
            this.tabPage2.Controls.Add(this.label_pem_rate_message);
            this.tabPage2.Controls.Add(this.pictureBox1);
            this.tabPage2.Controls.Add(this.pictureBox4);
            this.tabPage2.Controls.Add(this.comboBox_company_hourly_rate_currency);
            this.tabPage2.Controls.Add(this.label29);
            this.tabPage2.Controls.Add(this.comboBox_company_pem_rate);
            this.tabPage2.Controls.Add(this.label28);
            this.tabPage2.Controls.Add(this.label_company_pem_currency);
            this.tabPage2.Controls.Add(this.label27);
            this.tabPage2.Controls.Add(this.numericUpDown_company_hourly_rate);
            this.tabPage2.Controls.Add(this.label26);
            this.tabPage2.ImageIndex = 2;
            this.tabPage2.Location = new System.Drawing.Point(4, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(0, 72);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Default Rates";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox_auto_add_language_rate
            // 
            this.checkBox_auto_add_language_rate.AutoSize = true;
            this.checkBox_auto_add_language_rate.Location = new System.Drawing.Point(166, 161);
            this.checkBox_auto_add_language_rate.Name = "checkBox_auto_add_language_rate";
            this.checkBox_auto_add_language_rate.Size = new System.Drawing.Size(225, 17);
            this.checkBox_auto_add_language_rate.TabIndex = 8;
            this.checkBox_auto_add_language_rate.Text = "Automatically add to new Project Activities";
            this.checkBox_auto_add_language_rate.UseVisualStyleBackColor = true;
            this.checkBox_auto_add_language_rate.CheckedChanged += new System.EventHandler(this.checkBox_auto_add_langauge_rate_CheckedChanged);
            // 
            // checkBox_auto_add_hourly_rate
            // 
            this.checkBox_auto_add_hourly_rate.AutoSize = true;
            this.checkBox_auto_add_hourly_rate.Location = new System.Drawing.Point(166, 299);
            this.checkBox_auto_add_hourly_rate.Name = "checkBox_auto_add_hourly_rate";
            this.checkBox_auto_add_hourly_rate.Size = new System.Drawing.Size(225, 17);
            this.checkBox_auto_add_hourly_rate.TabIndex = 14;
            this.checkBox_auto_add_hourly_rate.Text = "Automatically add to new Project Activities";
            this.checkBox_auto_add_hourly_rate.UseVisualStyleBackColor = true;
            this.checkBox_auto_add_hourly_rate.CheckedChanged += new System.EventHandler(this.checkBox_auto_add_hourly_rate_CheckedChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(38, 61);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(22, 22);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 98;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(43, 220);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(22, 22);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 97;
            this.pictureBox3.TabStop = false;
            // 
            // label39
            // 
            this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label39.Location = new System.Drawing.Point(35, 15);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(674, 33);
            this.label39.TabIndex = 0;
            this.label39.Text = "Choose the default rates for this client and whether or not they will be added au" +
    "tomatically to the Project Activities when they are created.\r\n";
            // 
            // label_hourly_rate_message
            // 
            this.label_hourly_rate_message.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_hourly_rate_message.Location = new System.Drawing.Point(125, 252);
            this.label_hourly_rate_message.Name = "label_hourly_rate_message";
            this.label_hourly_rate_message.Size = new System.Drawing.Size(541, 17);
            this.label_hourly_rate_message.TabIndex = 10;
            this.label_hourly_rate_message.Text = "This method allows you to calculate the total by specifying the number of hours w" +
    "orked against a specific rate.\r\n";
            // 
            // label_pem_rate_message
            // 
            this.label_pem_rate_message.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_pem_rate_message.Location = new System.Drawing.Point(125, 89);
            this.label_pem_rate_message.Name = "label_pem_rate_message";
            this.label_pem_rate_message.Size = new System.Drawing.Size(467, 32);
            this.label_pem_rate_message.TabIndex = 4;
            this.label_pem_rate_message.Text = "This method categorizes the modifications in the traditional Trados analysis band" +
    " structure based on the weight of changes applied to each translation.";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Location = new System.Drawing.Point(131, 231);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(497, 4);
            this.pictureBox1.TabIndex = 95;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox4.Location = new System.Drawing.Point(151, 71);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(477, 4);
            this.pictureBox4.TabIndex = 95;
            this.pictureBox4.TabStop = false;
            // 
            // comboBox_company_hourly_rate_currency
            // 
            this.comboBox_company_hourly_rate_currency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_company_hourly_rate_currency.FormattingEnabled = true;
            this.comboBox_company_hourly_rate_currency.Location = new System.Drawing.Point(244, 272);
            this.comboBox_company_hourly_rate_currency.Name = "comboBox_company_hourly_rate_currency";
            this.comboBox_company_hourly_rate_currency.Size = new System.Drawing.Size(200, 21);
            this.comboBox_company_hourly_rate_currency.Sorted = true;
            this.comboBox_company_hourly_rate_currency.TabIndex = 13;
            this.comboBox_company_hourly_rate_currency.SelectedIndexChanged += new System.EventHandler(this.comboBox_company_hourly_rate_currency_SelectedIndexChanged);
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(65, 225);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(66, 13);
            this.label29.TabIndex = 9;
            this.label29.Text = "Hourly Rate:";
            // 
            // comboBox_company_pem_rate
            // 
            this.comboBox_company_pem_rate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_company_pem_rate.FormattingEnabled = true;
            this.comboBox_company_pem_rate.Location = new System.Drawing.Point(166, 127);
            this.comboBox_company_pem_rate.Name = "comboBox_company_pem_rate";
            this.comboBox_company_pem_rate.Size = new System.Drawing.Size(278, 21);
            this.comboBox_company_pem_rate.TabIndex = 6;
            this.comboBox_company_pem_rate.SelectedIndexChanged += new System.EventHandler(this.comboBox_company_price_rate_group_SelectedIndexChanged);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(60, 65);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(84, 13);
            this.label28.TabIndex = 2;
            this.label28.Text = "Language Rate:";
            // 
            // label_company_pem_currency
            // 
            this.label_company_pem_currency.AutoSize = true;
            this.label_company_pem_currency.Location = new System.Drawing.Point(456, 133);
            this.label_company_pem_currency.Name = "label_company_pem_currency";
            this.label_company_pem_currency.Size = new System.Drawing.Size(16, 13);
            this.label_company_pem_currency.TabIndex = 7;
            this.label_company_pem_currency.Text = "...";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(128, 131);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(33, 13);
            this.label27.TabIndex = 5;
            this.label27.Text = "Rate:";
            // 
            // numericUpDown_company_hourly_rate
            // 
            this.numericUpDown_company_hourly_rate.DecimalPlaces = 2;
            this.numericUpDown_company_hourly_rate.Location = new System.Drawing.Point(166, 272);
            this.numericUpDown_company_hourly_rate.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_company_hourly_rate.Name = "numericUpDown_company_hourly_rate";
            this.numericUpDown_company_hourly_rate.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown_company_hourly_rate.TabIndex = 12;
            this.numericUpDown_company_hourly_rate.ValueChanged += new System.EventHandler(this.numericUpDown_company_hourly_rate_ValueChanged);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(128, 275);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(33, 13);
            this.label26.TabIndex = 11;
            this.label26.Text = "Rate:";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.comboBox_comparisonType);
            this.tabPage3.Controls.Add(this.label33);
            this.tabPage3.Controls.Add(this.checkBox_group_changes);
            this.tabPage3.Controls.Add(this.label32);
            this.tabPage3.Controls.Add(this.linkLabel_reset_differences_formatting);
            this.tabPage3.Controls.Add(this.label31);
            this.tabPage3.Controls.Add(this.button_changeFormattingTagRemoved);
            this.tabPage3.Controls.Add(this.button_changeFormattingTagNew);
            this.tabPage3.Controls.Add(this.button_changeFormattingTextRemoved);
            this.tabPage3.Controls.Add(this.button_changeFormattingTextNew);
            this.tabPage3.Controls.Add(this.richTextBox_formatting_Tag_Removed);
            this.tabPage3.Controls.Add(this.richTextBox_formatting_Tag_New);
            this.tabPage3.Controls.Add(this.richTextBox_formatting_Text_Removed);
            this.tabPage3.Controls.Add(this.richTextBox_formatting_Text_New);
            this.tabPage3.Controls.Add(this.panel5);
            this.tabPage3.Controls.Add(this.label16);
            this.tabPage3.Controls.Add(this.checkBox_includeTagsInComparison);
            this.tabPage3.ImageIndex = 3;
            this.tabPage3.Location = new System.Drawing.Point(4, 31);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(0, 72);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Comparison Settings";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // comboBox_comparisonType
            // 
            this.comboBox_comparisonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_comparisonType.Enabled = false;
            this.comboBox_comparisonType.FormattingEnabled = true;
            this.comboBox_comparisonType.Items.AddRange(new object[] {
            "words",
            "characters"});
            this.comboBox_comparisonType.Location = new System.Drawing.Point(125, 21);
            this.comboBox_comparisonType.Name = "comboBox_comparisonType";
            this.comboBox_comparisonType.Size = new System.Drawing.Size(121, 21);
            this.comboBox_comparisonType.TabIndex = 2;
            this.comboBox_comparisonType.SelectedIndexChanged += new System.EventHandler(this.comboBox_comparisonType_SelectedIndexChanged);
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(27, 25);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(92, 13);
            this.label33.TabIndex = 1;
            this.label33.Text = "Comparison Type:";
            // 
            // checkBox_group_changes
            // 
            this.checkBox_group_changes.AutoSize = true;
            this.checkBox_group_changes.Location = new System.Drawing.Point(30, 54);
            this.checkBox_group_changes.Name = "checkBox_group_changes";
            this.checkBox_group_changes.Size = new System.Drawing.Size(182, 17);
            this.checkBox_group_changes.TabIndex = 3;
            this.checkBox_group_changes.Text = "Consolidate comparison changes";
            this.checkBox_group_changes.UseVisualStyleBackColor = true;
            this.checkBox_group_changes.CheckedChanged += new System.EventHandler(this.checkBox_group_changes_CheckedChanged);
            // 
            // label32
            // 
            this.label32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label32.Location = new System.Drawing.Point(47, 74);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(669, 16);
            this.label32.TabIndex = 4;
            this.label32.Text = "Consolidate the comparison differences in groups of similar types; this improves " +
    "readability of the comparison differences from the report";
            // 
            // linkLabel_reset_differences_formatting
            // 
            this.linkLabel_reset_differences_formatting.AutoSize = true;
            this.linkLabel_reset_differences_formatting.Location = new System.Drawing.Point(35, 360);
            this.linkLabel_reset_differences_formatting.Name = "linkLabel_reset_differences_formatting";
            this.linkLabel_reset_differences_formatting.Size = new System.Drawing.Size(139, 13);
            this.linkLabel_reset_differences_formatting.TabIndex = 15;
            this.linkLabel_reset_differences_formatting.TabStop = true;
            this.linkLabel_reset_differences_formatting.Text = "Reset differences formatting";
            this.linkLabel_reset_differences_formatting.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_reset_differences_formatting_LinkClicked);
            // 
            // label31
            // 
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label31.Location = new System.Drawing.Point(46, 115);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(653, 30);
            this.label31.TabIndex = 6;
            this.label31.Text = resources.GetString("label31.Text");
            // 
            // button_changeFormattingTagRemoved
            // 
            this.button_changeFormattingTagRemoved.Location = new System.Drawing.Point(423, 303);
            this.button_changeFormattingTagRemoved.Name = "button_changeFormattingTagRemoved";
            this.button_changeFormattingTagRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagRemoved.TabIndex = 14;
            this.button_changeFormattingTagRemoved.Text = "Change...";
            this.button_changeFormattingTagRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagRemoved.Click += new System.EventHandler(this.button_changeFormattingTagRemoved_Click);
            // 
            // button_changeFormattingTagNew
            // 
            this.button_changeFormattingTagNew.Location = new System.Drawing.Point(423, 265);
            this.button_changeFormattingTagNew.Name = "button_changeFormattingTagNew";
            this.button_changeFormattingTagNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagNew.TabIndex = 12;
            this.button_changeFormattingTagNew.Text = "Change...";
            this.button_changeFormattingTagNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagNew.Click += new System.EventHandler(this.button_changeFormattingTagNew_Click);
            // 
            // button_changeFormattingTextRemoved
            // 
            this.button_changeFormattingTextRemoved.Location = new System.Drawing.Point(423, 227);
            this.button_changeFormattingTextRemoved.Name = "button_changeFormattingTextRemoved";
            this.button_changeFormattingTextRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextRemoved.TabIndex = 10;
            this.button_changeFormattingTextRemoved.Text = "Change...";
            this.button_changeFormattingTextRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextRemoved.Click += new System.EventHandler(this.button_changeFormattingTextRemoved_Click);
            // 
            // button_changeFormattingTextNew
            // 
            this.button_changeFormattingTextNew.Location = new System.Drawing.Point(423, 189);
            this.button_changeFormattingTextNew.Name = "button_changeFormattingTextNew";
            this.button_changeFormattingTextNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextNew.TabIndex = 8;
            this.button_changeFormattingTextNew.Text = "Change...";
            this.button_changeFormattingTextNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextNew.Click += new System.EventHandler(this.button_changeFormattingTextNew_Click);
            // 
            // richTextBox_formatting_Tag_Removed
            // 
            this.richTextBox_formatting_Tag_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_Removed.Location = new System.Drawing.Point(56, 303);
            this.richTextBox_formatting_Tag_Removed.Name = "richTextBox_formatting_Tag_Removed";
            this.richTextBox_formatting_Tag_Removed.ReadOnly = true;
            this.richTextBox_formatting_Tag_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_Removed.TabIndex = 13;
            this.richTextBox_formatting_Tag_Removed.Text = "";
            // 
            // richTextBox_formatting_Tag_New
            // 
            this.richTextBox_formatting_Tag_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_New.Location = new System.Drawing.Point(56, 265);
            this.richTextBox_formatting_Tag_New.Name = "richTextBox_formatting_Tag_New";
            this.richTextBox_formatting_Tag_New.ReadOnly = true;
            this.richTextBox_formatting_Tag_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_New.TabIndex = 11;
            this.richTextBox_formatting_Tag_New.Text = "";
            // 
            // richTextBox_formatting_Text_Removed
            // 
            this.richTextBox_formatting_Text_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_Removed.Location = new System.Drawing.Point(56, 227);
            this.richTextBox_formatting_Text_Removed.Name = "richTextBox_formatting_Text_Removed";
            this.richTextBox_formatting_Text_Removed.ReadOnly = true;
            this.richTextBox_formatting_Text_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_Removed.TabIndex = 9;
            this.richTextBox_formatting_Text_Removed.Text = "";
            // 
            // richTextBox_formatting_Text_New
            // 
            this.richTextBox_formatting_Text_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_New.Location = new System.Drawing.Point(56, 189);
            this.richTextBox_formatting_Text_New.Name = "richTextBox_formatting_Text_New";
            this.richTextBox_formatting_Text_New.ReadOnly = true;
            this.richTextBox_formatting_Text_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_New.TabIndex = 7;
            this.richTextBox_formatting_Text_New.Text = "";
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel5.Location = new System.Drawing.Point(171, 165);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(325, 4);
            this.panel5.TabIndex = 106;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label16.Location = new System.Drawing.Point(35, 158);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(113, 13);
            this.label16.TabIndex = 105;
            this.label16.Text = "Differences Formatting";
            // 
            // checkBox_includeTagsInComparison
            // 
            this.checkBox_includeTagsInComparison.AutoSize = true;
            this.checkBox_includeTagsInComparison.Location = new System.Drawing.Point(30, 94);
            this.checkBox_includeTagsInComparison.Name = "checkBox_includeTagsInComparison";
            this.checkBox_includeTagsInComparison.Size = new System.Drawing.Size(241, 17);
            this.checkBox_includeTagsInComparison.TabIndex = 5;
            this.checkBox_includeTagsInComparison.Text = "Include <tag> formatting in comparison results";
            this.checkBox_includeTagsInComparison.UseVisualStyleBackColor = true;
            this.checkBox_includeTagsInComparison.CheckedChanged += new System.EventHandler(this.checkBox_includeTagsInComparison_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.richTextBox1);
            this.tabPage4.Controls.Add(this.comboBox_quality_metric_groups);
            this.tabPage4.Controls.Add(this.label38);
            this.tabPage4.ImageIndex = 5;
            this.tabPage4.Location = new System.Drawing.Point(4, 31);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(0, 72);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Quality Metrics";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.Location = new System.Drawing.Point(37, 80);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(654, 96);
            this.richTextBox1.TabIndex = 24;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            // 
            // comboBox_quality_metric_groups
            // 
            this.comboBox_quality_metric_groups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_quality_metric_groups.FormattingEnabled = true;
            this.comboBox_quality_metric_groups.Location = new System.Drawing.Point(151, 35);
            this.comboBox_quality_metric_groups.Name = "comboBox_quality_metric_groups";
            this.comboBox_quality_metric_groups.Size = new System.Drawing.Size(337, 21);
            this.comboBox_quality_metric_groups.TabIndex = 23;
            this.comboBox_quality_metric_groups.SelectedIndexChanged += new System.EventHandler(this.comboBox_quality_metric_groups_SelectedIndexChanged);
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label38.Location = new System.Drawing.Point(34, 38);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(111, 13);
            this.label38.TabIndex = 22;
            this.label38.Text = "Quality Metrics Group:";
            // 
            // imageList_client_tabcontrol
            // 
            this.imageList_client_tabcontrol.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_client_tabcontrol.ImageStream")));
            this.imageList_client_tabcontrol.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_client_tabcontrol.Images.SetKeyName(0, "Google-Docs-colorfull-32.png");
            this.imageList_client_tabcontrol.Images.SetKeyName(1, "Coins-64x64.png");
            this.imageList_client_tabcontrol.Images.SetKeyName(2, "cost.png");
            this.imageList_client_tabcontrol.Images.SetKeyName(3, "Funny-Search-32.png");
            this.imageList_client_tabcontrol.Images.SetKeyName(4, "Edit-Yes-32.png");
            this.imageList_client_tabcontrol.Images.SetKeyName(5, "Tasks-32.png");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(238, 46);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 107);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panel_clients_navigation_1
            // 
            this.panel_clients_navigation_1.Controls.Add(this.treeView_clients);
            this.panel_clients_navigation_1.Controls.Add(this.panel12);
            this.panel_clients_navigation_1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_clients_navigation_1.Location = new System.Drawing.Point(0, 46);
            this.panel_clients_navigation_1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panel_clients_navigation_1.MinimumSize = new System.Drawing.Size(238, 4);
            this.panel_clients_navigation_1.Name = "panel_clients_navigation_1";
            this.panel_clients_navigation_1.Size = new System.Drawing.Size(238, 107);
            this.panel_clients_navigation_1.TabIndex = 2;
            // 
            // treeView_clients
            // 
            this.treeView_clients.ContextMenuStrip = this.contextMenuStrip_clients;
            this.treeView_clients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_clients.FullRowSelect = true;
            this.treeView_clients.HideSelection = false;
            this.treeView_clients.ImageIndex = 0;
            this.treeView_clients.ImageList = this.imageList_clients;
            this.treeView_clients.Location = new System.Drawing.Point(0, 24);
            this.treeView_clients.MinimumSize = new System.Drawing.Size(4, 4);
            this.treeView_clients.Name = "treeView_clients";
            this.treeView_clients.SelectedImageIndex = 0;
            this.treeView_clients.ShowNodeToolTips = true;
            this.treeView_clients.ShowPlusMinus = false;
            this.treeView_clients.ShowRootLines = false;
            this.treeView_clients.Size = new System.Drawing.Size(238, 83);
            this.treeView_clients.TabIndex = 3;
            this.treeView_clients.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_clients_AfterSelect);
            // 
            // contextMenuStrip_clients
            // 
            this.contextMenuStrip_clients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addClientToolStripMenuItem,
            this.removeClientToolStripMenuItem});
            this.contextMenuStrip_clients.Name = "contextMenuStrip_clients";
            this.contextMenuStrip_clients.Size = new System.Drawing.Size(152, 48);
            // 
            // addClientToolStripMenuItem
            // 
            this.addClientToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addClientToolStripMenuItem.Image")));
            this.addClientToolStripMenuItem.Name = "addClientToolStripMenuItem";
            this.addClientToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.addClientToolStripMenuItem.Text = "Add Client";
            this.addClientToolStripMenuItem.Click += new System.EventHandler(this.addClientToolStripMenuItem_Click);
            // 
            // removeClientToolStripMenuItem
            // 
            this.removeClientToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeClientToolStripMenuItem.Image")));
            this.removeClientToolStripMenuItem.Name = "removeClientToolStripMenuItem";
            this.removeClientToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.removeClientToolStripMenuItem.Text = "Remove Client";
            this.removeClientToolStripMenuItem.Click += new System.EventHandler(this.removeClientToolStripMenuItem_Click);
            // 
            // imageList_clients
            // 
            this.imageList_clients.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_clients.ImageStream")));
            this.imageList_clients.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_clients.Images.SetKeyName(0, "User-32.png");
            this.imageList_clients.Images.SetKeyName(1, "user_32.png");
            this.imageList_clients.Images.SetKeyName(2, "user_blue");
            this.imageList_clients.Images.SetKeyName(3, "user_red");
            this.imageList_clients.Images.SetKeyName(4, "user_yellow");
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.label2);
            this.panel12.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel12.Location = new System.Drawing.Point(0, 0);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(238, 24);
            this.panel12.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(4, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Clients";
            // 
            // toolStrip_clients
            // 
            this.toolStrip_clients.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip_clients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_addClient,
            this.toolStripButton_deleteClient,
            this.toolStripSeparator1,
            this.toolStripButton_editActivityRate});
            this.toolStrip_clients.Location = new System.Drawing.Point(0, 15);
            this.toolStrip_clients.Name = "toolStrip_clients";
            this.toolStrip_clients.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip_clients.Size = new System.Drawing.Size(195, 31);
            this.toolStrip_clients.TabIndex = 0;
            this.toolStrip_clients.Text = "Clients";
            // 
            // toolStripButton_addClient
            // 
            this.toolStripButton_addClient.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_addClient.Image")));
            this.toolStripButton_addClient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_addClient.Name = "toolStripButton_addClient";
            this.toolStripButton_addClient.Size = new System.Drawing.Size(91, 28);
            this.toolStripButton_addClient.Text = "Add Client";
            this.toolStripButton_addClient.ToolTipText = "Add Client";
            this.toolStripButton_addClient.Click += new System.EventHandler(this.toolStripButton_addClient_Click);
            // 
            // toolStripButton_deleteClient
            // 
            this.toolStripButton_deleteClient.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_deleteClient.Image")));
            this.toolStripButton_deleteClient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_deleteClient.Name = "toolStripButton_deleteClient";
            this.toolStripButton_deleteClient.Size = new System.Drawing.Size(78, 28);
            this.toolStripButton_deleteClient.Text = "Remove";
            this.toolStripButton_deleteClient.ToolTipText = "Remove Client";
            this.toolStripButton_deleteClient.Click += new System.EventHandler(this.toolStripButton_deleteClient_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripButton_editActivityRate
            // 
            this.toolStripButton_editActivityRate.Name = "toolStripButton_editActivityRate";
            this.toolStripButton_editActivityRate.Size = new System.Drawing.Size(23, 4);
            // 
            // panel_activity_tracking
            // 
            this.panel_activity_tracking.Controls.Add(this.label17);
            this.panel_activity_tracking.Controls.Add(this.numericUpDown_idleTimeOutMinutes);
            this.panel_activity_tracking.Controls.Add(this.checkBox_idleTimeOutShow);
            this.panel_activity_tracking.Controls.Add(this.checkBox_idleTimeOut);
            this.panel_activity_tracking.Controls.Add(this.checkBox_startOnLoad);
            this.panel_activity_tracking.Controls.Add(this.checkBox_trackerConfirmActivities);
            this.panel_activity_tracking.Controls.Add(this.checkBox_recordKeyStokes);
            this.panel_activity_tracking.Controls.Add(this.checkBox_warningMessageActivityTrackingNotRunning);
            this.panel_activity_tracking.Controls.Add(this.checkBox_autoStartTrackingOnDocumentOpenEvent);
            this.panel_activity_tracking.Controls.Add(this.checkBox_record_segments_that_are_updated);
            this.panel_activity_tracking.Controls.Add(this.checkBox_recordNonUpdatedSegments);
            this.panel_activity_tracking.Location = new System.Drawing.Point(480, 103);
            this.panel_activity_tracking.Name = "panel_activity_tracking";
            this.panel_activity_tracking.Size = new System.Drawing.Size(189, 152);
            this.panel_activity_tracking.TabIndex = 42;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label17.Location = new System.Drawing.Point(345, 74);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(94, 13);
            this.label17.TabIndex = 5;
            this.label17.Text = "[ 5 ... 60 ] minutes ";
            // 
            // numericUpDown_idleTimeOutMinutes
            // 
            this.numericUpDown_idleTimeOutMinutes.Location = new System.Drawing.Point(290, 70);
            this.numericUpDown_idleTimeOutMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDown_idleTimeOutMinutes.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown_idleTimeOutMinutes.Name = "numericUpDown_idleTimeOutMinutes";
            this.numericUpDown_idleTimeOutMinutes.Size = new System.Drawing.Size(49, 20);
            this.numericUpDown_idleTimeOutMinutes.TabIndex = 4;
            this.numericUpDown_idleTimeOutMinutes.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.numericUpDown_idleTimeOutMinutes.ValueChanged += new System.EventHandler(this.numericUpDown_idleTimeOutMinutes_ValueChanged);
            // 
            // checkBox_idleTimeOutShow
            // 
            this.checkBox_idleTimeOutShow.AutoSize = true;
            this.checkBox_idleTimeOutShow.Location = new System.Drawing.Point(65, 96);
            this.checkBox_idleTimeOutShow.Name = "checkBox_idleTimeOutShow";
            this.checkBox_idleTimeOutShow.Size = new System.Drawing.Size(294, 17);
            this.checkBox_idleTimeOutShow.TabIndex = 6;
            this.checkBox_idleTimeOutShow.Text = "Show Idle timeout counter in the Activity Tracker window";
            this.checkBox_idleTimeOutShow.UseVisualStyleBackColor = true;
            this.checkBox_idleTimeOutShow.CheckedChanged += new System.EventHandler(this.checkBox_idleTimeOutShow_CheckedChanged);
            // 
            // checkBox_idleTimeOut
            // 
            this.checkBox_idleTimeOut.AutoSize = true;
            this.checkBox_idleTimeOut.Location = new System.Drawing.Point(37, 73);
            this.checkBox_idleTimeOut.Name = "checkBox_idleTimeOut";
            this.checkBox_idleTimeOut.Size = new System.Drawing.Size(254, 17);
            this.checkBox_idleTimeOut.TabIndex = 3;
            this.checkBox_idleTimeOut.Text = "Idle timeout before the activity tracker is paused:";
            this.checkBox_idleTimeOut.UseVisualStyleBackColor = true;
            this.checkBox_idleTimeOut.CheckedChanged += new System.EventHandler(this.checkBox_idleTimeOut_CheckedChanged);
            // 
            // checkBox_startOnLoad
            // 
            this.checkBox_startOnLoad.AutoSize = true;
            this.checkBox_startOnLoad.Location = new System.Drawing.Point(37, 50);
            this.checkBox_startOnLoad.Name = "checkBox_startOnLoad";
            this.checkBox_startOnLoad.Size = new System.Drawing.Size(309, 17);
            this.checkBox_startOnLoad.TabIndex = 2;
            this.checkBox_startOnLoad.Text = "Start the activity tracker automatically when Studio is started";
            this.checkBox_startOnLoad.UseVisualStyleBackColor = true;
            this.checkBox_startOnLoad.CheckedChanged += new System.EventHandler(this.checkBox_startOnLoad_CheckedChanged);
            // 
            // checkBox_trackerConfirmActivities
            // 
            this.checkBox_trackerConfirmActivities.AutoSize = true;
            this.checkBox_trackerConfirmActivities.Enabled = false;
            this.checkBox_trackerConfirmActivities.Location = new System.Drawing.Point(37, 27);
            this.checkBox_trackerConfirmActivities.Name = "checkBox_trackerConfirmActivities";
            this.checkBox_trackerConfirmActivities.Size = new System.Drawing.Size(280, 17);
            this.checkBox_trackerConfirmActivities.TabIndex = 1;
            this.checkBox_trackerConfirmActivities.Text = "Always confirm before adding an activity to the project";
            this.checkBox_trackerConfirmActivities.UseVisualStyleBackColor = true;
            this.checkBox_trackerConfirmActivities.CheckedChanged += new System.EventHandler(this.checkBox_trackerConfirmActivities_CheckedChanged);
            // 
            // checkBox_recordKeyStokes
            // 
            this.checkBox_recordKeyStokes.AutoSize = true;
            this.checkBox_recordKeyStokes.Location = new System.Drawing.Point(37, 211);
            this.checkBox_recordKeyStokes.Name = "checkBox_recordKeyStokes";
            this.checkBox_recordKeyStokes.Size = new System.Drawing.Size(134, 17);
            this.checkBox_recordKeyStokes.TabIndex = 11;
            this.checkBox_recordKeyStokes.Text = "Record keystroke data";
            this.checkBox_recordKeyStokes.UseVisualStyleBackColor = true;
            this.checkBox_recordKeyStokes.CheckedChanged += new System.EventHandler(this.checkBox_recordKeyStokes_CheckedChanged);
            // 
            // checkBox_warningMessageActivityTrackingNotRunning
            // 
            this.checkBox_warningMessageActivityTrackingNotRunning.AutoSize = true;
            this.checkBox_warningMessageActivityTrackingNotRunning.Enabled = false;
            this.checkBox_warningMessageActivityTrackingNotRunning.Location = new System.Drawing.Point(62, 142);
            this.checkBox_warningMessageActivityTrackingNotRunning.Name = "checkBox_warningMessageActivityTrackingNotRunning";
            this.checkBox_warningMessageActivityTrackingNotRunning.Size = new System.Drawing.Size(514, 17);
            this.checkBox_warningMessageActivityTrackingNotRunning.TabIndex = 8;
            this.checkBox_warningMessageActivityTrackingNotRunning.Text = "Warning message to user when content changes in the document and the activity tra" +
    "cker is not running";
            this.checkBox_warningMessageActivityTrackingNotRunning.UseVisualStyleBackColor = true;
            this.checkBox_warningMessageActivityTrackingNotRunning.CheckedChanged += new System.EventHandler(this.checkBox_warningMessageActivityTrackingNotRunning_CheckedChanged);
            // 
            // checkBox_autoStartTrackingOnDocumentOpenEvent
            // 
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.AutoSize = true;
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.Location = new System.Drawing.Point(37, 121);
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.Name = "checkBox_autoStartTrackingOnDocumentOpenEvent";
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.Size = new System.Drawing.Size(339, 17);
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.TabIndex = 7;
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.Text = "Start the activity tracker automatically when a document is opened";
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.UseVisualStyleBackColor = true;
            this.checkBox_autoStartTrackingOnDocumentOpenEvent.CheckedChanged += new System.EventHandler(this.checkBox_autoStartTrackingOnDocumentOpenEvent_CheckedChanged);
            // 
            // checkBox_record_segments_that_are_updated
            // 
            this.checkBox_record_segments_that_are_updated.AutoSize = true;
            this.checkBox_record_segments_that_are_updated.Checked = true;
            this.checkBox_record_segments_that_are_updated.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_record_segments_that_are_updated.Enabled = false;
            this.checkBox_record_segments_that_are_updated.Location = new System.Drawing.Point(37, 165);
            this.checkBox_record_segments_that_are_updated.Name = "checkBox_record_segments_that_are_updated";
            this.checkBox_record_segments_that_are_updated.Size = new System.Drawing.Size(190, 17);
            this.checkBox_record_segments_that_are_updated.TabIndex = 9;
            this.checkBox_record_segments_that_are_updated.Text = "Record segments that are updated";
            this.checkBox_record_segments_that_are_updated.UseVisualStyleBackColor = true;
            this.checkBox_record_segments_that_are_updated.CheckedChanged += new System.EventHandler(this.checkBox_record_segments_that_are_updated_CheckedChanged);
            // 
            // checkBox_recordNonUpdatedSegments
            // 
            this.checkBox_recordNonUpdatedSegments.AutoSize = true;
            this.checkBox_recordNonUpdatedSegments.Location = new System.Drawing.Point(37, 188);
            this.checkBox_recordNonUpdatedSegments.Name = "checkBox_recordNonUpdatedSegments";
            this.checkBox_recordNonUpdatedSegments.Size = new System.Drawing.Size(208, 17);
            this.checkBox_recordNonUpdatedSegments.TabIndex = 10;
            this.checkBox_recordNonUpdatedSegments.Text = "Record segments that are not updated";
            this.checkBox_recordNonUpdatedSegments.UseVisualStyleBackColor = true;
            this.checkBox_recordNonUpdatedSegments.CheckedChanged += new System.EventHandler(this.checkBox_recordNonUpdatedSegments_CheckedChanged);
            // 
            // panel_language_rates
            // 
            this.panel_language_rates.Controls.Add(this.panel11);
            this.panel_language_rates.Controls.Add(this.panel15);
            this.panel_language_rates.Location = new System.Drawing.Point(69, 268);
            this.panel_language_rates.Name = "panel_language_rates";
            this.panel_language_rates.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_language_rates.Size = new System.Drawing.Size(197, 156);
            this.panel_language_rates.TabIndex = 41;
            // 
            // panel11
            // 
            this.panel11.Controls.Add(this.listView_price_groups);
            this.panel11.Controls.Add(this.splitter2);
            this.panel11.Controls.Add(this.panel13);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel11.Location = new System.Drawing.Point(0, 51);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(197, 105);
            this.panel11.TabIndex = 1;
            // 
            // listView_price_groups
            // 
            this.listView_price_groups.AllowDrop = true;
            this.listView_price_groups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18,
            this.columnHeader19,
            this.columnHeader20,
            this.columnHeader21,
            this.columnHeader22,
            this.columnHeader23,
            this.columnHeader24,
            this.columnHeader25});
            this.listView_price_groups.ContextMenuStrip = this.contextMenuStrip_price_groups_listview;
            this.listView_price_groups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_price_groups.FullRowSelect = true;
            this.listView_price_groups.GridLines = true;
            this.listView_price_groups.HideSelection = false;
            this.listView_price_groups.Location = new System.Drawing.Point(154, 0);
            this.listView_price_groups.Name = "listView_price_groups";
            this.listView_price_groups.ShowItemToolTips = true;
            this.listView_price_groups.Size = new System.Drawing.Size(43, 105);
            this.listView_price_groups.SmallImageList = this.imageList3;
            this.listView_price_groups.TabIndex = 2;
            this.listView_price_groups.UseCompatibleStateImageBehavior = false;
            this.listView_price_groups.View = System.Windows.Forms.View.Details;
            this.listView_price_groups.SelectedIndexChanged += new System.EventHandler(this.listView_price_groups_SelectedIndexChanged);
            this.listView_price_groups.DoubleClick += new System.EventHandler(this.listView_price_groups_DoubleClick);
            this.listView_price_groups.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_price_groups_KeyUp);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Source";
            this.columnHeader7.Width = 55;
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "Target";
            this.columnHeader14.Width = 55;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Price";
            this.columnHeader15.Width = 65;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Round";
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "PM";
            this.columnHeader17.Width = 50;
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "CM";
            this.columnHeader18.Width = 50;
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Rep.";
            this.columnHeader19.Width = 50;
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "100%";
            this.columnHeader20.Width = 50;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "99-94%";
            this.columnHeader21.Width = 50;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "94-85%";
            this.columnHeader22.Width = 50;
            // 
            // columnHeader23
            // 
            this.columnHeader23.Text = "84-75%";
            this.columnHeader23.Width = 50;
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "75-50%";
            this.columnHeader24.Width = 50;
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "New";
            this.columnHeader25.Width = 50;
            // 
            // contextMenuStrip_price_groups_listview
            // 
            this.contextMenuStrip_price_groups_listview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPriceToolStripMenuItem,
            this.editPriceToolStripMenuItem,
            this.removePriceToolStripMenuItem,
            this.toolStripSeparator5,
            this.analysisBandToolStripMenuItem});
            this.contextMenuStrip_price_groups_listview.Name = "contextMenuStrip1";
            this.contextMenuStrip_price_groups_listview.Size = new System.Drawing.Size(161, 98);
            this.contextMenuStrip_price_groups_listview.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_price_groups_listview_Opening);
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
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(157, 6);
            // 
            // analysisBandToolStripMenuItem
            // 
            this.analysisBandToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("analysisBandToolStripMenuItem.Image")));
            this.analysisBandToolStripMenuItem.Name = "analysisBandToolStripMenuItem";
            this.analysisBandToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.analysisBandToolStripMenuItem.Text = "Analysis Band %";
            this.analysisBandToolStripMenuItem.Click += new System.EventHandler(this.analysisBandToolStripMenuItem_Click);
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
            // splitter2
            // 
            this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitter2.Location = new System.Drawing.Point(151, 0);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(3, 105);
            this.splitter2.TabIndex = 1;
            this.splitter2.TabStop = false;
            // 
            // panel13
            // 
            this.panel13.Controls.Add(this.treeView_price_groups);
            this.panel13.Controls.Add(this.panel14);
            this.panel13.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel13.Location = new System.Drawing.Point(0, 0);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(151, 105);
            this.panel13.TabIndex = 0;
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
            this.treeView_price_groups.Size = new System.Drawing.Size(151, 79);
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
            this.contextMenuStrip_price_groups_treeview.Size = new System.Drawing.Size(262, 98);
            this.contextMenuStrip_price_groups_treeview.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_price_groups_treeview_Opening);
            // 
            // newPriceGroupToolStripMenuItem
            // 
            this.newPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newPriceGroupToolStripMenuItem.Image")));
            this.newPriceGroupToolStripMenuItem.Name = "newPriceGroupToolStripMenuItem";
            this.newPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.newPriceGroupToolStripMenuItem.Text = "New Language Rate Group";
            this.newPriceGroupToolStripMenuItem.Click += new System.EventHandler(this.newPriceGroupToolStripMenuItem_Click);
            // 
            // editPriceGroupToolStripMenuItem
            // 
            this.editPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editPriceGroupToolStripMenuItem.Image")));
            this.editPriceGroupToolStripMenuItem.Name = "editPriceGroupToolStripMenuItem";
            this.editPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.editPriceGroupToolStripMenuItem.Text = "Edit Language Rate Group";
            this.editPriceGroupToolStripMenuItem.Click += new System.EventHandler(this.editPriceGroupToolStripMenuItem_Click);
            // 
            // removePriceGroupToolStripMenuItem
            // 
            this.removePriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removePriceGroupToolStripMenuItem.Image")));
            this.removePriceGroupToolStripMenuItem.Name = "removePriceGroupToolStripMenuItem";
            this.removePriceGroupToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.removePriceGroupToolStripMenuItem.Text = "Remove Language Rate Group";
            this.removePriceGroupToolStripMenuItem.Click += new System.EventHandler(this.removePriceGroupToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(258, 6);
            // 
            // setAsDefaultPriceGroupToolStripMenuItem
            // 
            this.setAsDefaultPriceGroupToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("setAsDefaultPriceGroupToolStripMenuItem.Image")));
            this.setAsDefaultPriceGroupToolStripMenuItem.Name = "setAsDefaultPriceGroupToolStripMenuItem";
            this.setAsDefaultPriceGroupToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.setAsDefaultPriceGroupToolStripMenuItem.Text = "Set as default Language Rate Group";
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
            this.imageList2.Images.SetKeyName(4, "Bullet-Green-32.png");
            this.imageList2.Images.SetKeyName(5, "Default");
            this.imageList2.Images.SetKeyName(6, "Bullet-Red-32.png");
            this.imageList2.Images.SetKeyName(7, "Empty");
            // 
            // panel14
            // 
            this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel14.Controls.Add(this.label18);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel14.Location = new System.Drawing.Point(0, 0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(151, 26);
            this.panel14.TabIndex = 0;
            // 
            // label18
            // 
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label18.Location = new System.Drawing.Point(0, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(149, 24);
            this.label18.TabIndex = 0;
            this.label18.Text = "Language Rate Group";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel15
            // 
            this.panel15.Controls.Add(this.toolStrip_ProjectsBlocks);
            this.panel15.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel15.Location = new System.Drawing.Point(0, 15);
            this.panel15.Name = "panel15";
            this.panel15.Size = new System.Drawing.Size(197, 36);
            this.panel15.TabIndex = 0;
            // 
            // toolStrip_ProjectsBlocks
            // 
            this.toolStrip_ProjectsBlocks.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip_ProjectsBlocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_priceGroup_new,
            this.toolStripButton_priceGroup_edit,
            this.toolStripButton_priceGroup_remove,
            this.toolStripSeparator3,
            this.toolStripButton_groupPrice_add,
            this.toolStripButton_groupPrice_edit,
            this.toolStripButton_groupPrice_remove,
            this.toolStripSeparator4,
            this.toolStripButton_analysisBandPercentage});
            this.toolStrip_ProjectsBlocks.Location = new System.Drawing.Point(0, 0);
            this.toolStrip_ProjectsBlocks.Name = "toolStrip_ProjectsBlocks";
            this.toolStrip_ProjectsBlocks.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip_ProjectsBlocks.Size = new System.Drawing.Size(197, 31);
            this.toolStrip_ProjectsBlocks.TabIndex = 8;
            // 
            // toolStripButton_priceGroup_new
            // 
            this.toolStripButton_priceGroup_new.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_priceGroup_new.Image")));
            this.toolStripButton_priceGroup_new.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_priceGroup_new.Name = "toolStripButton_priceGroup_new";
            this.toolStripButton_priceGroup_new.Size = new System.Drawing.Size(59, 28);
            this.toolStripButton_priceGroup_new.Text = "New";
            this.toolStripButton_priceGroup_new.ToolTipText = "New Language Rate Group";
            this.toolStripButton_priceGroup_new.Click += new System.EventHandler(this.toolStripButton_priceGroup_new_Click);
            // 
            // toolStripButton_priceGroup_edit
            // 
            this.toolStripButton_priceGroup_edit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_priceGroup_edit.Image")));
            this.toolStripButton_priceGroup_edit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_priceGroup_edit.Name = "toolStripButton_priceGroup_edit";
            this.toolStripButton_priceGroup_edit.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_priceGroup_edit.Text = "Edit";
            this.toolStripButton_priceGroup_edit.ToolTipText = "Edit Language Rate Group";
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
            this.toolStripButton_priceGroup_remove.ToolTipText = "Remove Language Rate Group";
            this.toolStripButton_priceGroup_remove.Click += new System.EventHandler(this.toolStripButton_priceGroup_remove_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
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
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
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
            // panel_my_info
            // 
            this.panel_my_info.Controls.Add(this.label6);
            this.panel_my_info.Controls.Add(this.label9);
            this.panel_my_info.Controls.Add(this.label8);
            this.panel_my_info.Controls.Add(this.label_webPageAddress);
            this.panel_my_info.Controls.Add(this.textBox_userFaxNumber);
            this.panel_my_info.Controls.Add(this.textBox_userPhoneNumber);
            this.panel_my_info.Controls.Add(this.textBox_userInternetWebPageAddress);
            this.panel_my_info.Controls.Add(this.textBox_userAddress);
            this.panel_my_info.Controls.Add(this.button_userAddress);
            this.panel_my_info.Controls.Add(this.label7);
            this.panel_my_info.Controls.Add(this.textBox_userInternetEmail);
            this.panel_my_info.Controls.Add(this.label5);
            this.panel_my_info.Controls.Add(this.textBox_userVatCode);
            this.panel_my_info.Controls.Add(this.textBox_userTaxCode);
            this.panel_my_info.Controls.Add(this.textBox_userName);
            this.panel_my_info.Controls.Add(this.label37);
            this.panel_my_info.Controls.Add(this.textBox_userCompanyName);
            this.panel_my_info.Controls.Add(this.label4);
            this.panel_my_info.Location = new System.Drawing.Point(69, 102);
            this.panel_my_info.Name = "panel_my_info";
            this.panel_my_info.Size = new System.Drawing.Size(188, 153);
            this.panel_my_info.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 258);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "E-Mail:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(47, 340);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Fax Number:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(33, 311);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Phone Number:";
            // 
            // label_webPageAddress
            // 
            this.label_webPageAddress.AutoSize = true;
            this.label_webPageAddress.Location = new System.Drawing.Point(54, 285);
            this.label_webPageAddress.Name = "label_webPageAddress";
            this.label_webPageAddress.Size = new System.Drawing.Size(60, 13);
            this.label_webPageAddress.TabIndex = 13;
            this.label_webPageAddress.Text = "Web page:";
            // 
            // textBox_userFaxNumber
            // 
            this.textBox_userFaxNumber.Location = new System.Drawing.Point(125, 336);
            this.textBox_userFaxNumber.Name = "textBox_userFaxNumber";
            this.textBox_userFaxNumber.Size = new System.Drawing.Size(244, 20);
            this.textBox_userFaxNumber.TabIndex = 18;
            this.textBox_userFaxNumber.TextChanged += new System.EventHandler(this.textBox_userFaxNumber_TextChanged);
            // 
            // textBox_userPhoneNumber
            // 
            this.textBox_userPhoneNumber.Location = new System.Drawing.Point(125, 307);
            this.textBox_userPhoneNumber.Name = "textBox_userPhoneNumber";
            this.textBox_userPhoneNumber.Size = new System.Drawing.Size(244, 20);
            this.textBox_userPhoneNumber.TabIndex = 16;
            this.textBox_userPhoneNumber.TextChanged += new System.EventHandler(this.textBox_userPhoneNumber_TextChanged);
            // 
            // textBox_userInternetWebPageAddress
            // 
            this.textBox_userInternetWebPageAddress.Location = new System.Drawing.Point(125, 281);
            this.textBox_userInternetWebPageAddress.Name = "textBox_userInternetWebPageAddress";
            this.textBox_userInternetWebPageAddress.Size = new System.Drawing.Size(244, 20);
            this.textBox_userInternetWebPageAddress.TabIndex = 14;
            this.textBox_userInternetWebPageAddress.TextChanged += new System.EventHandler(this.textBox_userInternetWebPageAddress_TextChanged);
            // 
            // textBox_userAddress
            // 
            this.textBox_userAddress.Location = new System.Drawing.Point(126, 84);
            this.textBox_userAddress.Multiline = true;
            this.textBox_userAddress.Name = "textBox_userAddress";
            this.textBox_userAddress.ReadOnly = true;
            this.textBox_userAddress.Size = new System.Drawing.Size(244, 84);
            this.textBox_userAddress.TabIndex = 6;
            this.textBox_userAddress.TextChanged += new System.EventHandler(this.textBox_userAddress_TextChanged);
            // 
            // button_userAddress
            // 
            this.button_userAddress.Location = new System.Drawing.Point(35, 85);
            this.button_userAddress.Name = "button_userAddress";
            this.button_userAddress.Size = new System.Drawing.Size(83, 23);
            this.button_userAddress.TabIndex = 4;
            this.button_userAddress.Text = "Address...";
            this.button_userAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_userAddress.UseVisualStyleBackColor = true;
            this.button_userAddress.Click += new System.EventHandler(this.button_userAddress_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 210);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "VAT Code:";
            // 
            // textBox_userInternetEmail
            // 
            this.textBox_userInternetEmail.Location = new System.Drawing.Point(125, 254);
            this.textBox_userInternetEmail.Name = "textBox_userInternetEmail";
            this.textBox_userInternetEmail.Size = new System.Drawing.Size(244, 20);
            this.textBox_userInternetEmail.TabIndex = 12;
            this.textBox_userInternetEmail.TextChanged += new System.EventHandler(this.textBox_userInternetEmail_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 187);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "TAX Code:";
            // 
            // textBox_userVatCode
            // 
            this.textBox_userVatCode.Location = new System.Drawing.Point(126, 206);
            this.textBox_userVatCode.Name = "textBox_userVatCode";
            this.textBox_userVatCode.Size = new System.Drawing.Size(244, 20);
            this.textBox_userVatCode.TabIndex = 10;
            this.textBox_userVatCode.TextChanged += new System.EventHandler(this.textBox_userVatCode_TextChanged);
            // 
            // textBox_userTaxCode
            // 
            this.textBox_userTaxCode.Location = new System.Drawing.Point(126, 183);
            this.textBox_userTaxCode.Name = "textBox_userTaxCode";
            this.textBox_userTaxCode.Size = new System.Drawing.Size(244, 20);
            this.textBox_userTaxCode.TabIndex = 8;
            this.textBox_userTaxCode.TextChanged += new System.EventHandler(this.textBox_userTaxCode_TextChanged);
            // 
            // textBox_userName
            // 
            this.textBox_userName.Location = new System.Drawing.Point(126, 54);
            this.textBox_userName.Name = "textBox_userName";
            this.textBox_userName.Size = new System.Drawing.Size(244, 20);
            this.textBox_userName.TabIndex = 3;
            this.textBox_userName.TextChanged += new System.EventHandler(this.textBox_userName_TextChanged);
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(55, 57);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(63, 13);
            this.label37.TabIndex = 2;
            this.label37.Text = "User Name:";
            // 
            // textBox_userCompanyName
            // 
            this.textBox_userCompanyName.Location = new System.Drawing.Point(126, 28);
            this.textBox_userCompanyName.Name = "textBox_userCompanyName";
            this.textBox_userCompanyName.Size = new System.Drawing.Size(244, 20);
            this.textBox_userCompanyName.TabIndex = 1;
            this.textBox_userCompanyName.TextChanged += new System.EventHandler(this.textBox_userCompanyName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Company Name:";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.textBox_header);
            this.panel4.Controls.Add(this.pictureBox_header);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(5, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(998, 56);
            this.panel4.TabIndex = 37;
            // 
            // textBox_header
            // 
            this.textBox_header.BackColor = System.Drawing.Color.White;
            this.textBox_header.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_header.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_header.Location = new System.Drawing.Point(72, 16);
            this.textBox_header.Name = "textBox_header";
            this.textBox_header.ReadOnly = true;
            this.textBox_header.Size = new System.Drawing.Size(288, 25);
            this.textBox_header.TabIndex = 3;
            this.textBox_header.TabStop = false;
            this.textBox_header.Text = "General Settings";
            // 
            // pictureBox_header
            // 
            this.pictureBox_header.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.Image")));
            this.pictureBox_header.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_header.InitialImage")));
            this.pictureBox_header.Location = new System.Drawing.Point(26, 11);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(32, 32);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // contextMenuStrip_client_rates
            // 
            this.contextMenuStrip_client_rates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editActivityRateToolStripMenuItem});
            this.contextMenuStrip_client_rates.Name = "contextMenuStrip1";
            this.contextMenuStrip_client_rates.Size = new System.Drawing.Size(68, 26);
            // 
            // editActivityRateToolStripMenuItem
            // 
            this.editActivityRateToolStripMenuItem.Name = "editActivityRateToolStripMenuItem";
            this.editActivityRateToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // imageList_activity
            // 
            this.imageList_activity.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_activity.ImageStream")));
            this.imageList_activity.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_activity.Images.SetKeyName(0, "Gnome-Task-Due-32.png");
            this.imageList_activity.Images.SetKeyName(1, "Bullet-Green-32.png");
            this.imageList_activity.Images.SetKeyName(2, "Bullet-Red-32.png");
            // 
            // contextMenuStrip_activity
            // 
            this.contextMenuStrip_activity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addActivityToolStripMenuItem,
            this.editActivityToolStripMenuItem,
            this.removeActivityToolStripMenuItem});
            this.contextMenuStrip_activity.Name = "contextMenuStrip_activity";
            this.contextMenuStrip_activity.Size = new System.Drawing.Size(68, 70);
            // 
            // addActivityToolStripMenuItem
            // 
            this.addActivityToolStripMenuItem.Name = "addActivityToolStripMenuItem";
            this.addActivityToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // editActivityToolStripMenuItem
            // 
            this.editActivityToolStripMenuItem.Name = "editActivityToolStripMenuItem";
            this.editActivityToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // removeActivityToolStripMenuItem
            // 
            this.removeActivityToolStripMenuItem.Name = "removeActivityToolStripMenuItem";
            this.removeActivityToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1239, 571);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1255, 587);
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel_general.ResumeLayout(false);
            this.panel_general.PerformLayout();
            this.panel_quality_metrics.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.contextMenuStrip_metrics.ResumeLayout(false);
            this.panel19.ResumeLayout(false);
            this.panel16.ResumeLayout(false);
            this.contextMenuStrip_metric_group.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel18.ResumeLayout(false);
            this.panel17.ResumeLayout(false);
            this.panel17.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_backup.ResumeLayout(false);
            this.panel_backup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backup_every)).EndInit();
            this.panel_clients.ResumeLayout(false);
            this.panel_clients.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_company_hourly_rate)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.panel_clients_navigation_1.ResumeLayout(false);
            this.contextMenuStrip_clients.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.toolStrip_clients.ResumeLayout(false);
            this.toolStrip_clients.PerformLayout();
            this.panel_activity_tracking.ResumeLayout(false);
            this.panel_activity_tracking.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_idleTimeOutMinutes)).EndInit();
            this.panel_language_rates.ResumeLayout(false);
            this.panel11.ResumeLayout(false);
            this.contextMenuStrip_price_groups_listview.ResumeLayout(false);
            this.panel13.ResumeLayout(false);
            this.contextMenuStrip_price_groups_treeview.ResumeLayout(false);
            this.panel14.ResumeLayout(false);
            this.panel15.ResumeLayout(false);
            this.panel15.PerformLayout();
            this.toolStrip_ProjectsBlocks.ResumeLayout(false);
            this.toolStrip_ProjectsBlocks.PerformLayout();
            this.panel_my_info.ResumeLayout(false);
            this.panel_my_info.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
            this.contextMenuStrip_client_rates.ResumeLayout(false);
            this.contextMenuStrip_activity.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        public TreeView treeView_main;
        private Panel panel3;
        private Panel panel_general;
        private Panel panel4;
        private TextBox textBox_header;
        private PictureBox pictureBox_header;
        private ImageList imageList_settings_navigation;
        private Panel panel_clients;
        private Label label3;
        private ToolStrip toolStrip_clients;
        private ToolStripButton toolStripButton_addClient;
        private ToolStripButton toolStripButton_deleteClient;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButton_editActivityRate;
        private Panel panel8;
        private Button button_Close;
        private Button button_Save;
        private Panel panel_my_info;
        private Label label7;
        private Label label5;
        private TextBox textBox_userVatCode;
        private TextBox textBox_userTaxCode;
        private TextBox textBox_userCompanyName;
        private Label label4;
        private TextBox textBox_userAddress;
        private Button button_userAddress;
        private Label label6;
        private Label label_webPageAddress;
        private TextBox textBox_userInternetWebPageAddress;
        private TextBox textBox_userInternetEmail;
        private Label label9;
        private Label label8;
        private TextBox textBox_userFaxNumber;
        private TextBox textBox_userPhoneNumber;
        private ImageList imageList_clients;
        private Splitter splitter1;
        private Panel panel_clients_navigation_1;
        private Panel panel12;
        private Label label2;
        private Button button_help;
        private ComboBox comboBox_default_currency;
        private Label label1;
        private ContextMenuStrip contextMenuStrip_client_rates;
        private ToolStripMenuItem editActivityRateToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip_clients;
        private ToolStripMenuItem addClientToolStripMenuItem;
        private ToolStripMenuItem removeClientToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip_activity;
        private ToolStripMenuItem addActivityToolStripMenuItem;
        private ToolStripMenuItem editActivityToolStripMenuItem;
        private ToolStripMenuItem removeActivityToolStripMenuItem;
        private ImageList imageList_activity;
        private Label label10;
        private Label label12;
        private Label label11;
        internal ComboBox comboBox_default_project_status;
        internal ComboBox comboBox_default_project_group_by;
        private Panel panel_backup;
        private Button button_browse_backup_folder;
        private TextBox textBox_backup_folder;
        private Label label13;
        private ComboBox comboBox_backup_every_type;
        private NumericUpDown numericUpDown_backup_every;
        private Label label14;
        private Label label_backup_last;
        private Label label15;
        private LinkLabel linkLabel_viewFoldersInWindowsExplorer;
        private Panel panel_language_rates;
        private Panel panel11;
        private ListView listView_price_groups;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader14;
        private ColumnHeader columnHeader15;
        private ColumnHeader columnHeader16;
        private ColumnHeader columnHeader17;
        private ColumnHeader columnHeader18;
        private ColumnHeader columnHeader19;
        private ColumnHeader columnHeader20;
        private ColumnHeader columnHeader21;
        private ColumnHeader columnHeader22;
        private ColumnHeader columnHeader23;
        private ColumnHeader columnHeader24;
        private ColumnHeader columnHeader25;
        private Splitter splitter2;
        private Panel panel13;
        private TreeView treeView_price_groups;
        private Panel panel14;
        private Label label18;
        private Panel panel15;
        private ToolStrip toolStrip_ProjectsBlocks;
        private ToolStripButton toolStripButton_priceGroup_new;
        private ToolStripButton toolStripButton_priceGroup_edit;
        private ToolStripButton toolStripButton_priceGroup_remove;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripSplitButton toolStripButton_groupPrice_add;
        private ToolStripMenuItem toolStripButton_groupPrice_addMultiple;
        private ToolStripButton toolStripButton_groupPrice_edit;
        private ToolStripButton toolStripButton_groupPrice_remove;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton toolStripButton_analysisBandPercentage;
        private ContextMenuStrip contextMenuStrip_price_groups_listview;
        private ToolStripMenuItem addPriceToolStripMenuItem;
        private ToolStripMenuItem editPriceToolStripMenuItem;
        private ToolStripMenuItem removePriceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem analysisBandToolStripMenuItem;
        private ImageList imageList3;
        private ContextMenuStrip contextMenuStrip_price_groups_treeview;
        private ToolStripMenuItem newPriceGroupToolStripMenuItem;
        private ToolStripMenuItem editPriceGroupToolStripMenuItem;
        private ToolStripMenuItem removePriceGroupToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem setAsDefaultPriceGroupToolStripMenuItem;
        private ImageList imageList2;
        private Panel panel6;
        private TreeView treeView_clients;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label19;
        private Label label20;
        private Label label21;
        private Label label22;
        private TextBox textBox_companyFaxNumber;
        private TextBox textBox_companyPhoneNumber;
        private TextBox textBox_companyWebPageAddress;
        private TextBox textBox_companyAddress;
        private Button button_companyAddress;
        private Label label23;
        private TextBox textBox_companyEmail;
        private Label label24;
        private TextBox textBox_companyVatCode;
        private TextBox textBox_companyTaxCode;
        private TextBox textBox_companyName;
        private Label label25;
        private ComboBox comboBox_company_pem_rate;
        private Label label27;
        private NumericUpDown numericUpDown_company_hourly_rate;
        private Label label26;
        private ImageList imageList_client_tabcontrol;
        private ComboBox comboBox_company_hourly_rate_currency;
        private Label label_pem_rate_message;
        private PictureBox pictureBox4;
        private PictureBox pictureBox1;
        private Label label_hourly_rate_message;
        private Label label_company_pem_currency;
        private Panel panel_activity_tracking;
        private CheckBox checkBox_recordKeyStokes;
        private CheckBox checkBox_recordNonUpdatedSegments;
        private CheckBox checkBox_record_segments_that_are_updated;
        private Label label17;
        private NumericUpDown numericUpDown_idleTimeOutMinutes;
        private CheckBox checkBox_idleTimeOutShow;
        private CheckBox checkBox_idleTimeOut;
        private CheckBox checkBox_startOnLoad;
        private CheckBox checkBox_trackerConfirmActivities;
        private CheckBox checkBox_autoStartTrackingOnDocumentOpenEvent;
        private CheckBox checkBox_warningMessageActivityTrackingNotRunning;
        private TabPage tabPage3;
        private Label label31;
        private Button button_changeFormattingTagRemoved;
        private Button button_changeFormattingTagNew;
        private Button button_changeFormattingTextRemoved;
        private Button button_changeFormattingTextNew;
        private RichTextBox richTextBox_formatting_Tag_Removed;
        private RichTextBox richTextBox_formatting_Tag_New;
        private RichTextBox richTextBox_formatting_Text_Removed;
        private RichTextBox richTextBox_formatting_Text_New;
        private Panel panel5;
        private Label label16;
        public CheckBox checkBox_includeTagsInComparison;
        private LinkLabel linkLabel_reset_differences_formatting;
        private CheckBox checkBox_group_changes;
        private Label label32;
        private Label label33;
        private ComboBox comboBox_comparisonType;
        internal ComboBox comboBox_default_activity_status;
        private Label label34;
        private Label label35;
        private ComboBox comboBox_defaultActivityViewGroupsIsOn;
        private Label label36;
        private Panel panel_quality_metrics;
        private Panel panel9;
        private Splitter splitter3;
        private Panel panel17;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButton_qualityMetrics_add;
        private ToolStripButton toolStripButton_qualityMetrics_edit;
        private ToolStripButton toolStripButton_qualityMetrics_remove;
        private TextBox textBox_userName;
        private Label label37;
        private TabPage tabPage4;
        private Label label38;
        private Panel panel16;
        private Panel panel18;
        private Label label42;
        private ToolStripButton toolStripButton_metric_group_edit;
        private ToolStripButton toolStripButton_metric_group_delete;
        private ToolStripSeparator toolStripSeparator6;
        private ListView listView_quality_metrics;
        private ColumnHeader columnHeader27;
        private ColumnHeader columnHeader28;
        private ColumnHeader columnHeader1;
        private Splitter splitter4;
        private ToolStripSplitButton toolStripSplitButton_new_metric_group;
        private ToolStripMenuItem sAEJ2450ToolStripMenuItem;
        private TreeView treeView_metric_group;
        private Panel panel7;
        private ContextMenuStrip contextMenuStrip_metric_group;
        private ToolStripMenuItem newMetricGroupToolStripMenuItem;
        private ToolStripMenuItem editMetricGroupToolStripMenuItem;
        private ToolStripMenuItem removeMetricGroupToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem setAsDefaultToolStripMenuItem;
        private LinkLabel linkLabel_set_as_default_metric_group;
        private ImageList imageList1;
        private ImageList imageList4;
        private ContextMenuStrip contextMenuStrip_metrics;
        private ToolStripMenuItem addQualityMetricToolStripMenuItem;
        private ToolStripMenuItem editQualityMetricToolStripMenuItem;
        private ToolStripMenuItem removeQualityMetricToolStripMenuItem;
        private ComboBox comboBox_quality_metric_groups;
        private RichTextBox richTextBox1;
        private ToolStripMenuItem mQMCoreStandardToolStripMenuItem;
        private ToolStripMenuItem lISAQAMetricToolStripMenuItem;
        private Panel panel19;
        private Label label_quality_metrics_count;
        private CheckBox checkBox_auto_add_language_rate;
        private CheckBox checkBox_auto_add_hourly_rate;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Label label39;
        private Label label29;
        private Label label28;
        private Label label30;
        private TextBox textBox_company_contactName;
        private LinkLabel linkLabel_create_a_backup_now;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton toolStripButton_metric_group_import;
        private ToolStripButton toolStripButton_metric_group_export;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem exportMetricGroupToolStripMenuItem;
        public CheckBox checkBox_include_unlisted_projects;
    }
}