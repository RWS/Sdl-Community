using System.ComponentModel;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
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
            var treeNode1 = new System.Windows.Forms.TreeNode("General", 0, 0);
            var treeNode2 = new System.Windows.Forms.TreeNode("Activity Types", 3, 3);
            var treeNode3 = new System.Windows.Forms.TreeNode("Client Rates", 1, 1);
            var treeNode4 = new System.Windows.Forms.TreeNode("My Details", 8, 8);
            var treeNode5 = new System.Windows.Forms.TreeNode("Backup", 10, 10);
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.button_Close = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_help = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeView_main = new System.Windows.Forms.TreeView();
            this.imageList_settings_navigation = new System.Windows.Forms.ImageList(this.components);
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel_backup = new System.Windows.Forms.Panel();
            this.linkLabel_viewFoldersInWindowsExplorer = new System.Windows.Forms.LinkLabel();
            this.comboBox_backup_every_type = new System.Windows.Forms.ComboBox();
            this.numericUpDown_backup_every = new System.Windows.Forms.NumericUpDown();
            this.button_browse_backup_folder = new System.Windows.Forms.Button();
            this.textBox_backup_folder = new System.Windows.Forms.TextBox();
            this.label_backup_last = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panel_general = new System.Windows.Forms.Panel();
            this.checkBox_trackerConfirmActivities = new System.Windows.Forms.CheckBox();
            this.comboBox_default_project_group_by = new System.Windows.Forms.ComboBox();
            this.comboBox_default_project_status = new System.Windows.Forms.ComboBox();
            this.comboBox_default_currency = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel_client_rates = new System.Windows.Forms.Panel();
            this.listView_clientActivityRates = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_client_rates = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editActivityRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList_activity = new System.Windows.Forms.ImageList(this.components);
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel_clients_navigation_1 = new System.Windows.Forms.Panel();
            this.treeView_clients = new System.Windows.Forms.TreeView();
            this.contextMenuStrip_clients = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList_clients = new System.Windows.Forms.ImageList(this.components);
            this.panel12 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.linkLabel_client_resetDefaultRates = new System.Windows.Forms.LinkLabel();
            this.linkLabel_client_rates_activities_uncheckall = new System.Windows.Forms.LinkLabel();
            this.linkLabel_client_rates_activities_checkall = new System.Windows.Forms.LinkLabel();
            this.panel_client_rates_bottom = new System.Windows.Forms.Panel();
            this.label_clientCount = new System.Windows.Forms.Label();
            this.toolStrip_clients = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_addClient = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_editClient = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_deleteClient = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_editActivityRate = new System.Windows.Forms.ToolStripButton();
            this.panel_activities = new System.Windows.Forms.Panel();
            this.listView_activities = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip_activity = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeActivityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.label_activities_count = new System.Windows.Forms.Label();
            this.toolStrip_activity = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_addActivity = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_eidtActivity = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_deleteActivity = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
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
            this.textBox_userCompanyName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox_header = new System.Windows.Forms.TextBox();
            this.pictureBox_header = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel8.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel_backup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backup_every)).BeginInit();
            this.panel_general.SuspendLayout();
            this.panel_client_rates.SuspendLayout();
            this.contextMenuStrip_client_rates.SuspendLayout();
            this.panel_clients_navigation_1.SuspendLayout();
            this.contextMenuStrip_clients.SuspendLayout();
            this.panel12.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel10.SuspendLayout();
            this.panel_client_rates_bottom.SuspendLayout();
            this.toolStrip_clients.SuspendLayout();
            this.panel_activities.SuspendLayout();
            this.contextMenuStrip_activity.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel9.SuspendLayout();
            this.toolStrip_activity.SuspendLayout();
            this.panel_my_info.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.button_help);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 484);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1154, 38);
            this.panel1.TabIndex = 0;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.button_Close);
            this.panel8.Controls.Add(this.button_Save);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(954, 0);
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
            this.panel2.Size = new System.Drawing.Size(231, 484);
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
            this.treeView_main.ItemHeight = 42;
            this.treeView_main.Location = new System.Drawing.Point(5, 5);
            this.treeView_main.Name = "treeView_main";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node_general";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "General";
            treeNode2.ImageIndex = 3;
            treeNode2.Name = "Node_activities";
            treeNode2.SelectedImageIndex = 3;
            treeNode2.Text = "Activity Types";
            treeNode3.ImageIndex = 1;
            treeNode3.Name = "Node_client_rates";
            treeNode3.SelectedImageIndex = 1;
            treeNode3.Text = "Client Rates";
            treeNode4.ImageIndex = 8;
            treeNode4.Name = "Node_my_info";
            treeNode4.SelectedImageIndex = 8;
            treeNode4.Text = "My Details";
            treeNode5.ImageIndex = 10;
            treeNode5.Name = "Node_backup";
            treeNode5.SelectedImageIndex = 10;
            treeNode5.Text = "Backup";
            this.treeView_main.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5});
            this.treeView_main.SelectedImageIndex = 0;
            this.treeView_main.Size = new System.Drawing.Size(221, 474);
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
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel_backup);
            this.panel3.Controls.Add(this.panel_general);
            this.panel3.Controls.Add(this.panel_client_rates);
            this.panel3.Controls.Add(this.panel_activities);
            this.panel3.Controls.Add(this.panel_my_info);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(231, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(5);
            this.panel3.Size = new System.Drawing.Size(923, 484);
            this.panel3.TabIndex = 2;
            // 
            // panel_backup
            // 
            this.panel_backup.Controls.Add(this.linkLabel_viewFoldersInWindowsExplorer);
            this.panel_backup.Controls.Add(this.comboBox_backup_every_type);
            this.panel_backup.Controls.Add(this.numericUpDown_backup_every);
            this.panel_backup.Controls.Add(this.button_browse_backup_folder);
            this.panel_backup.Controls.Add(this.textBox_backup_folder);
            this.panel_backup.Controls.Add(this.label_backup_last);
            this.panel_backup.Controls.Add(this.label15);
            this.panel_backup.Controls.Add(this.label14);
            this.panel_backup.Controls.Add(this.label13);
            this.panel_backup.Location = new System.Drawing.Point(106, 233);
            this.panel_backup.Name = "panel_backup";
            this.panel_backup.Size = new System.Drawing.Size(700, 188);
            this.panel_backup.TabIndex = 40;
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
            // panel_general
            // 
            this.panel_general.Controls.Add(this.checkBox_trackerConfirmActivities);
            this.panel_general.Controls.Add(this.comboBox_default_project_group_by);
            this.panel_general.Controls.Add(this.comboBox_default_project_status);
            this.panel_general.Controls.Add(this.comboBox_default_currency);
            this.panel_general.Controls.Add(this.label12);
            this.panel_general.Controls.Add(this.label11);
            this.panel_general.Controls.Add(this.label1);
            this.panel_general.Controls.Add(this.label10);
            this.panel_general.Controls.Add(this.label16);
            this.panel_general.Controls.Add(this.label3);
            this.panel_general.Location = new System.Drawing.Point(106, 90);
            this.panel_general.Name = "panel_general";
            this.panel_general.Size = new System.Drawing.Size(249, 106);
            this.panel_general.TabIndex = 38;
            // 
            // checkBox_trackerConfirmActivities
            // 
            this.checkBox_trackerConfirmActivities.AutoSize = true;
            this.checkBox_trackerConfirmActivities.Location = new System.Drawing.Point(90, 249);
            this.checkBox_trackerConfirmActivities.Name = "checkBox_trackerConfirmActivities";
            this.checkBox_trackerConfirmActivities.Size = new System.Drawing.Size(280, 17);
            this.checkBox_trackerConfirmActivities.TabIndex = 94;
            this.checkBox_trackerConfirmActivities.Text = "Always confirm before adding an activity to the project";
            this.checkBox_trackerConfirmActivities.UseVisualStyleBackColor = true;
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
            this.comboBox_default_project_group_by.Location = new System.Drawing.Point(172, 83);
            this.comboBox_default_project_group_by.Name = "comboBox_default_project_group_by";
            this.comboBox_default_project_group_by.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_project_group_by.TabIndex = 93;
            // 
            // comboBox_default_project_status
            // 
            this.comboBox_default_project_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_project_status.FormattingEnabled = true;
            this.comboBox_default_project_status.Items.AddRange(new object[] {
            "Show all projects",
            "In progress",
            "Completed"});
            this.comboBox_default_project_status.Location = new System.Drawing.Point(172, 58);
            this.comboBox_default_project_status.Name = "comboBox_default_project_status";
            this.comboBox_default_project_status.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_project_status.TabIndex = 92;
            // 
            // comboBox_default_currency
            // 
            this.comboBox_default_currency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_default_currency.FormattingEnabled = true;
            this.comboBox_default_currency.Location = new System.Drawing.Point(172, 164);
            this.comboBox_default_currency.Name = "comboBox_default_currency";
            this.comboBox_default_currency.Size = new System.Drawing.Size(187, 21);
            this.comboBox_default_currency.Sorted = true;
            this.comboBox_default_currency.TabIndex = 91;
            this.comboBox_default_currency.SelectedIndexChanged += new System.EventHandler(this.comboBox_default_currency_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(34, 86);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(135, 13);
            this.label12.TabIndex = 2;
            this.label12.Text = "Default project grouped by:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(59, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(110, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Default project status:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(81, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 89;
            this.label1.Text = "Default currency:";
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
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label16.Location = new System.Drawing.Point(35, 219);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(89, 13);
            this.label16.TabIndex = 88;
            this.label16.Text = "Activities Tracker";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(31, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "Currency";
            // 
            // panel_client_rates
            // 
            this.panel_client_rates.Controls.Add(this.listView_clientActivityRates);
            this.panel_client_rates.Controls.Add(this.splitter1);
            this.panel_client_rates.Controls.Add(this.panel_clients_navigation_1);
            this.panel_client_rates.Controls.Add(this.panel5);
            this.panel_client_rates.Controls.Add(this.toolStrip_clients);
            this.panel_client_rates.Location = new System.Drawing.Point(479, 239);
            this.panel_client_rates.Name = "panel_client_rates";
            this.panel_client_rates.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_client_rates.Size = new System.Drawing.Size(171, 205);
            this.panel_client_rates.TabIndex = 38;
            // 
            // listView_clientActivityRates
            // 
            this.listView_clientActivityRates.CheckBoxes = true;
            this.listView_clientActivityRates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader9,
            this.columnHeader3,
            this.columnHeader7,
            this.columnHeader8});
            this.listView_clientActivityRates.ContextMenuStrip = this.contextMenuStrip_client_rates;
            this.listView_clientActivityRates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_clientActivityRates.FullRowSelect = true;
            this.listView_clientActivityRates.HideSelection = false;
            this.listView_clientActivityRates.Location = new System.Drawing.Point(241, 46);
            this.listView_clientActivityRates.Name = "listView_clientActivityRates";
            this.listView_clientActivityRates.Size = new System.Drawing.Size(0, 132);
            this.listView_clientActivityRates.SmallImageList = this.imageList_activity;
            this.listView_clientActivityRates.TabIndex = 4;
            this.listView_clientActivityRates.UseCompatibleStateImageBehavior = false;
            this.listView_clientActivityRates.View = System.Windows.Forms.View.Details;
            this.listView_clientActivityRates.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_clientActivityRates_ItemCheck);
            this.listView_clientActivityRates.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_clientActivityRates_ItemSelectionChanged);
            this.listView_clientActivityRates.DoubleClick += new System.EventHandler(this.listView_clientActivityRates_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Activity Name";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Description";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Billable";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Hourly Rate";
            this.columnHeader3.Width = 80;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Adjustment";
            this.columnHeader7.Width = 100;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Currency";
            // 
            // contextMenuStrip_client_rates
            // 
            this.contextMenuStrip_client_rates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editActivityRateToolStripMenuItem});
            this.contextMenuStrip_client_rates.Name = "contextMenuStrip1";
            this.contextMenuStrip_client_rates.Size = new System.Drawing.Size(164, 26);
            // 
            // editActivityRateToolStripMenuItem
            // 
            this.editActivityRateToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editActivityRateToolStripMenuItem.Image")));
            this.editActivityRateToolStripMenuItem.Name = "editActivityRateToolStripMenuItem";
            this.editActivityRateToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.editActivityRateToolStripMenuItem.Text = "Edit Activity Rate";
            this.editActivityRateToolStripMenuItem.Click += new System.EventHandler(this.editActivityRateToolStripMenuItem_Click);
            // 
            // imageList_activity
            // 
            this.imageList_activity.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_activity.ImageStream")));
            this.imageList_activity.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_activity.Images.SetKeyName(0, "Gnome-Task-Due-32.png");
            this.imageList_activity.Images.SetKeyName(1, "Bullet-Green-32.png");
            this.imageList_activity.Images.SetKeyName(2, "Bullet-Red-32.png");
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(238, 46);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 132);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
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
            this.panel_clients_navigation_1.Size = new System.Drawing.Size(238, 132);
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
            this.treeView_clients.ShowPlusMinus = false;
            this.treeView_clients.ShowRootLines = false;
            this.treeView_clients.Size = new System.Drawing.Size(238, 108);
            this.treeView_clients.TabIndex = 3;
            this.treeView_clients.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_clients_AfterSelect);
            this.treeView_clients.DoubleClick += new System.EventHandler(this.treeView_clients_DoubleClick);
            // 
            // contextMenuStrip_clients
            // 
            this.contextMenuStrip_clients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addClientToolStripMenuItem,
            this.editClientToolStripMenuItem,
            this.removeClientToolStripMenuItem});
            this.contextMenuStrip_clients.Name = "contextMenuStrip_clients";
            this.contextMenuStrip_clients.Size = new System.Drawing.Size(152, 70);
            // 
            // addClientToolStripMenuItem
            // 
            this.addClientToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addClientToolStripMenuItem.Image")));
            this.addClientToolStripMenuItem.Name = "addClientToolStripMenuItem";
            this.addClientToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.addClientToolStripMenuItem.Text = "Add Client";
            this.addClientToolStripMenuItem.Click += new System.EventHandler(this.addClientToolStripMenuItem_Click);
            // 
            // editClientToolStripMenuItem
            // 
            this.editClientToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editClientToolStripMenuItem.Image")));
            this.editClientToolStripMenuItem.Name = "editClientToolStripMenuItem";
            this.editClientToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.editClientToolStripMenuItem.Text = "Edit Client";
            this.editClientToolStripMenuItem.Click += new System.EventHandler(this.editClientToolStripMenuItem_Click);
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
            // panel5
            // 
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.panel_client_rates_bottom);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 178);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(171, 27);
            this.panel5.TabIndex = 1;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.panel10);
            this.panel7.Controls.Add(this.linkLabel_client_rates_activities_uncheckall);
            this.panel7.Controls.Add(this.linkLabel_client_rates_activities_checkall);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(241, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(0, 27);
            this.panel7.TabIndex = 1;
            // 
            // panel10
            // 
            this.panel10.Controls.Add(this.linkLabel_client_resetDefaultRates);
            this.panel10.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel10.Location = new System.Drawing.Point(-142, 0);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(142, 27);
            this.panel10.TabIndex = 1;
            // 
            // linkLabel_client_resetDefaultRates
            // 
            this.linkLabel_client_resetDefaultRates.AutoSize = true;
            this.linkLabel_client_resetDefaultRates.Location = new System.Drawing.Point(41, 6);
            this.linkLabel_client_resetDefaultRates.Name = "linkLabel_client_resetDefaultRates";
            this.linkLabel_client_resetDefaultRates.Size = new System.Drawing.Size(96, 13);
            this.linkLabel_client_resetDefaultRates.TabIndex = 23;
            this.linkLabel_client_resetDefaultRates.TabStop = true;
            this.linkLabel_client_resetDefaultRates.Text = "Reset default rates";
            this.linkLabel_client_resetDefaultRates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_client_resetDefaultRates_LinkClicked);
            // 
            // linkLabel_client_rates_activities_uncheckall
            // 
            this.linkLabel_client_rates_activities_uncheckall.AutoSize = true;
            this.linkLabel_client_rates_activities_uncheckall.Location = new System.Drawing.Point(74, 6);
            this.linkLabel_client_rates_activities_uncheckall.Name = "linkLabel_client_rates_activities_uncheckall";
            this.linkLabel_client_rates_activities_uncheckall.Size = new System.Drawing.Size(79, 13);
            this.linkLabel_client_rates_activities_uncheckall.TabIndex = 0;
            this.linkLabel_client_rates_activities_uncheckall.TabStop = true;
            this.linkLabel_client_rates_activities_uncheckall.Text = "Deactivated All";
            this.linkLabel_client_rates_activities_uncheckall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_client_rates_activities_uncheckall_LinkClicked);
            // 
            // linkLabel_client_rates_activities_checkall
            // 
            this.linkLabel_client_rates_activities_checkall.AutoSize = true;
            this.linkLabel_client_rates_activities_checkall.Location = new System.Drawing.Point(6, 6);
            this.linkLabel_client_rates_activities_checkall.Name = "linkLabel_client_rates_activities_checkall";
            this.linkLabel_client_rates_activities_checkall.Size = new System.Drawing.Size(60, 13);
            this.linkLabel_client_rates_activities_checkall.TabIndex = 0;
            this.linkLabel_client_rates_activities_checkall.TabStop = true;
            this.linkLabel_client_rates_activities_checkall.Text = "Activate All";
            this.linkLabel_client_rates_activities_checkall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_client_rates_activities_checkall_LinkClicked);
            // 
            // panel_client_rates_bottom
            // 
            this.panel_client_rates_bottom.Controls.Add(this.label_clientCount);
            this.panel_client_rates_bottom.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_client_rates_bottom.Location = new System.Drawing.Point(0, 0);
            this.panel_client_rates_bottom.Name = "panel_client_rates_bottom";
            this.panel_client_rates_bottom.Size = new System.Drawing.Size(241, 27);
            this.panel_client_rates_bottom.TabIndex = 0;
            // 
            // label_clientCount
            // 
            this.label_clientCount.AutoSize = true;
            this.label_clientCount.Location = new System.Drawing.Point(5, 6);
            this.label_clientCount.Name = "label_clientCount";
            this.label_clientCount.Size = new System.Drawing.Size(50, 13);
            this.label_clientCount.TabIndex = 0;
            this.label_clientCount.Text = "Clients: 0";
            this.label_clientCount.Visible = false;
            // 
            // toolStrip_clients
            // 
            this.toolStrip_clients.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip_clients.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_addClient,
            this.toolStripButton_editClient,
            this.toolStripButton_deleteClient,
            this.toolStripSeparator1,
            this.toolStripButton_editActivityRate});
            this.toolStrip_clients.Location = new System.Drawing.Point(0, 15);
            this.toolStrip_clients.Name = "toolStrip_clients";
            this.toolStrip_clients.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip_clients.Size = new System.Drawing.Size(171, 31);
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
            // toolStripButton_editClient
            // 
            this.toolStripButton_editClient.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_editClient.Image")));
            this.toolStripButton_editClient.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_editClient.Name = "toolStripButton_editClient";
            this.toolStripButton_editClient.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_editClient.Text = "Edit";
            this.toolStripButton_editClient.ToolTipText = "Edit Client";
            this.toolStripButton_editClient.Click += new System.EventHandler(this.toolStripButton_editClient_Click);
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
            this.toolStripButton_editActivityRate.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_editActivityRate.Image")));
            this.toolStripButton_editActivityRate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_editActivityRate.Name = "toolStripButton_editActivityRate";
            this.toolStripButton_editActivityRate.Size = new System.Drawing.Size(124, 28);
            this.toolStripButton_editActivityRate.Text = "Edit Activity Rate";
            this.toolStripButton_editActivityRate.Click += new System.EventHandler(this.toolStripButton_editActivityRate_Click);
            // 
            // panel_activities
            // 
            this.panel_activities.Controls.Add(this.listView_activities);
            this.panel_activities.Controls.Add(this.panel6);
            this.panel_activities.Controls.Add(this.toolStrip_activity);
            this.panel_activities.Location = new System.Drawing.Point(534, 83);
            this.panel_activities.Name = "panel_activities";
            this.panel_activities.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.panel_activities.Size = new System.Drawing.Size(298, 113);
            this.panel_activities.TabIndex = 39;
            // 
            // listView_activities
            // 
            this.listView_activities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13});
            this.listView_activities.ContextMenuStrip = this.contextMenuStrip_activity;
            this.listView_activities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_activities.FullRowSelect = true;
            this.listView_activities.GridLines = true;
            this.listView_activities.HideSelection = false;
            this.listView_activities.Location = new System.Drawing.Point(0, 46);
            this.listView_activities.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.listView_activities.Name = "listView_activities";
            this.listView_activities.Size = new System.Drawing.Size(298, 42);
            this.listView_activities.SmallImageList = this.imageList_activity;
            this.listView_activities.TabIndex = 5;
            this.listView_activities.UseCompatibleStateImageBehavior = false;
            this.listView_activities.View = System.Windows.Forms.View.Details;
            this.listView_activities.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_activities_ItemSelectionChanged);
            this.listView_activities.DoubleClick += new System.EventHandler(this.listView_activities_DoubleClick);
            this.listView_activities.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_activities_KeyUp);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Activity Type";
            this.columnHeader5.Width = 180;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Description";
            this.columnHeader6.Width = 150;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Billable";
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Hourly Rate (default)";
            this.columnHeader12.Width = 110;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Currency";
            this.columnHeader13.Width = 200;
            // 
            // contextMenuStrip_activity
            // 
            this.contextMenuStrip_activity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addActivityToolStripMenuItem,
            this.editActivityToolStripMenuItem,
            this.removeActivityToolStripMenuItem});
            this.contextMenuStrip_activity.Name = "contextMenuStrip_activity";
            this.contextMenuStrip_activity.Size = new System.Drawing.Size(161, 70);
            // 
            // addActivityToolStripMenuItem
            // 
            this.addActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("addActivityToolStripMenuItem.Image")));
            this.addActivityToolStripMenuItem.Name = "addActivityToolStripMenuItem";
            this.addActivityToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.addActivityToolStripMenuItem.Text = "Add Activity";
            this.addActivityToolStripMenuItem.Click += new System.EventHandler(this.addActivityToolStripMenuItem_Click);
            // 
            // editActivityToolStripMenuItem
            // 
            this.editActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("editActivityToolStripMenuItem.Image")));
            this.editActivityToolStripMenuItem.Name = "editActivityToolStripMenuItem";
            this.editActivityToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.editActivityToolStripMenuItem.Text = "Edit Activity";
            this.editActivityToolStripMenuItem.Click += new System.EventHandler(this.editActivityToolStripMenuItem_Click);
            // 
            // removeActivityToolStripMenuItem
            // 
            this.removeActivityToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("removeActivityToolStripMenuItem.Image")));
            this.removeActivityToolStripMenuItem.Name = "removeActivityToolStripMenuItem";
            this.removeActivityToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.removeActivityToolStripMenuItem.Text = "Remove Activity";
            this.removeActivityToolStripMenuItem.Click += new System.EventHandler(this.removeActivityToolStripMenuItem_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.panel9);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 88);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(298, 25);
            this.panel6.TabIndex = 4;
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.label_activities_count);
            this.panel9.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel9.Location = new System.Drawing.Point(98, 0);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(200, 25);
            this.panel9.TabIndex = 0;
            // 
            // label_activities_count
            // 
            this.label_activities_count.AutoSize = true;
            this.label_activities_count.Location = new System.Drawing.Point(134, 5);
            this.label_activities_count.Name = "label_activities_count";
            this.label_activities_count.Size = new System.Drawing.Size(61, 13);
            this.label_activities_count.TabIndex = 1;
            this.label_activities_count.Text = "Activities: 0";
            this.label_activities_count.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStrip_activity
            // 
            this.toolStrip_activity.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip_activity.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_addActivity,
            this.toolStripButton_eidtActivity,
            this.toolStripButton_deleteActivity,
            this.toolStripSeparator2});
            this.toolStrip_activity.Location = new System.Drawing.Point(0, 15);
            this.toolStrip_activity.Name = "toolStrip_activity";
            this.toolStrip_activity.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip_activity.Size = new System.Drawing.Size(298, 31);
            this.toolStrip_activity.TabIndex = 3;
            this.toolStrip_activity.Text = "Activities";
            // 
            // toolStripButton_addActivity
            // 
            this.toolStripButton_addActivity.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_addActivity.Image")));
            this.toolStripButton_addActivity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_addActivity.Name = "toolStripButton_addActivity";
            this.toolStripButton_addActivity.Size = new System.Drawing.Size(129, 28);
            this.toolStripButton_addActivity.Text = "Add Activity Type";
            this.toolStripButton_addActivity.Click += new System.EventHandler(this.toolStripButton_addActivity_Click);
            // 
            // toolStripButton_eidtActivity
            // 
            this.toolStripButton_eidtActivity.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_eidtActivity.Image")));
            this.toolStripButton_eidtActivity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_eidtActivity.Name = "toolStripButton_eidtActivity";
            this.toolStripButton_eidtActivity.Size = new System.Drawing.Size(55, 28);
            this.toolStripButton_eidtActivity.Text = "Edit";
            this.toolStripButton_eidtActivity.ToolTipText = "Edit Activity Type";
            this.toolStripButton_eidtActivity.Click += new System.EventHandler(this.toolStripButton_eidtActivity_Click);
            // 
            // toolStripButton_deleteActivity
            // 
            this.toolStripButton_deleteActivity.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_deleteActivity.Image")));
            this.toolStripButton_deleteActivity.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_deleteActivity.Name = "toolStripButton_deleteActivity";
            this.toolStripButton_deleteActivity.Size = new System.Drawing.Size(78, 28);
            this.toolStripButton_deleteActivity.Text = "Remove";
            this.toolStripButton_deleteActivity.ToolTipText = "Remove Activity Type";
            this.toolStripButton_deleteActivity.Click += new System.EventHandler(this.toolStripButton_deleteActivity_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
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
            this.panel_my_info.Controls.Add(this.textBox_userCompanyName);
            this.panel_my_info.Controls.Add(this.label4);
            this.panel_my_info.Location = new System.Drawing.Point(140, 261);
            this.panel_my_info.Name = "panel_my_info";
            this.panel_my_info.Size = new System.Drawing.Size(226, 127);
            this.panel_my_info.TabIndex = 38;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "E-Mail:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(48, 316);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "Fax Number:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(34, 287);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(81, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Phone Number:";
            // 
            // label_webPageAddress
            // 
            this.label_webPageAddress.AutoSize = true;
            this.label_webPageAddress.Location = new System.Drawing.Point(55, 261);
            this.label_webPageAddress.Name = "label_webPageAddress";
            this.label_webPageAddress.Size = new System.Drawing.Size(60, 13);
            this.label_webPageAddress.TabIndex = 10;
            this.label_webPageAddress.Text = "Web page:";
            // 
            // textBox_userFaxNumber
            // 
            this.textBox_userFaxNumber.Location = new System.Drawing.Point(126, 312);
            this.textBox_userFaxNumber.Name = "textBox_userFaxNumber";
            this.textBox_userFaxNumber.Size = new System.Drawing.Size(244, 20);
            this.textBox_userFaxNumber.TabIndex = 15;
            // 
            // textBox_userPhoneNumber
            // 
            this.textBox_userPhoneNumber.Location = new System.Drawing.Point(126, 283);
            this.textBox_userPhoneNumber.Name = "textBox_userPhoneNumber";
            this.textBox_userPhoneNumber.Size = new System.Drawing.Size(244, 20);
            this.textBox_userPhoneNumber.TabIndex = 13;
            // 
            // textBox_userInternetWebPageAddress
            // 
            this.textBox_userInternetWebPageAddress.Location = new System.Drawing.Point(126, 257);
            this.textBox_userInternetWebPageAddress.Name = "textBox_userInternetWebPageAddress";
            this.textBox_userInternetWebPageAddress.Size = new System.Drawing.Size(244, 20);
            this.textBox_userInternetWebPageAddress.TabIndex = 11;
            // 
            // textBox_userAddress
            // 
            this.textBox_userAddress.Location = new System.Drawing.Point(127, 60);
            this.textBox_userAddress.Multiline = true;
            this.textBox_userAddress.Name = "textBox_userAddress";
            this.textBox_userAddress.ReadOnly = true;
            this.textBox_userAddress.Size = new System.Drawing.Size(244, 84);
            this.textBox_userAddress.TabIndex = 3;
            // 
            // button_userAddress
            // 
            this.button_userAddress.Location = new System.Drawing.Point(36, 61);
            this.button_userAddress.Name = "button_userAddress";
            this.button_userAddress.Size = new System.Drawing.Size(83, 23);
            this.button_userAddress.TabIndex = 2;
            this.button_userAddress.Text = "Address...";
            this.button_userAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button_userAddress.UseVisualStyleBackColor = true;
            this.button_userAddress.Click += new System.EventHandler(this.button_userAddress_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 186);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "VAT Code:";
            // 
            // textBox_userInternetEmail
            // 
            this.textBox_userInternetEmail.Location = new System.Drawing.Point(126, 230);
            this.textBox_userInternetEmail.Name = "textBox_userInternetEmail";
            this.textBox_userInternetEmail.Size = new System.Drawing.Size(244, 20);
            this.textBox_userInternetEmail.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "TAX Code:";
            // 
            // textBox_userVatCode
            // 
            this.textBox_userVatCode.Location = new System.Drawing.Point(127, 182);
            this.textBox_userVatCode.Name = "textBox_userVatCode";
            this.textBox_userVatCode.Size = new System.Drawing.Size(244, 20);
            this.textBox_userVatCode.TabIndex = 7;
            // 
            // textBox_userTaxCode
            // 
            this.textBox_userTaxCode.Location = new System.Drawing.Point(127, 159);
            this.textBox_userTaxCode.Name = "textBox_userTaxCode";
            this.textBox_userTaxCode.Size = new System.Drawing.Size(244, 20);
            this.textBox_userTaxCode.TabIndex = 5;
            // 
            // textBox_userCompanyName
            // 
            this.textBox_userCompanyName.Location = new System.Drawing.Point(126, 28);
            this.textBox_userCompanyName.Name = "textBox_userCompanyName";
            this.textBox_userCompanyName.Size = new System.Drawing.Size(244, 20);
            this.textBox_userCompanyName.TabIndex = 1;
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
            this.panel4.Size = new System.Drawing.Size(913, 56);
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
            this.pictureBox_header.Location = new System.Drawing.Point(19, 7);
            this.pictureBox_header.Name = "pictureBox_header";
            this.pictureBox_header.Size = new System.Drawing.Size(42, 42);
            this.pictureBox_header.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox_header.TabIndex = 0;
            this.pictureBox_header.TabStop = false;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 522);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1170, 560);
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.panel1.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel_backup.ResumeLayout(false);
            this.panel_backup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_backup_every)).EndInit();
            this.panel_general.ResumeLayout(false);
            this.panel_general.PerformLayout();
            this.panel_client_rates.ResumeLayout(false);
            this.panel_client_rates.PerformLayout();
            this.contextMenuStrip_client_rates.ResumeLayout(false);
            this.panel_clients_navigation_1.ResumeLayout(false);
            this.contextMenuStrip_clients.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.panel12.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel10.ResumeLayout(false);
            this.panel10.PerformLayout();
            this.panel_client_rates_bottom.ResumeLayout(false);
            this.panel_client_rates_bottom.PerformLayout();
            this.toolStrip_clients.ResumeLayout(false);
            this.toolStrip_clients.PerformLayout();
            this.panel_activities.ResumeLayout(false);
            this.panel_activities.PerformLayout();
            this.contextMenuStrip_activity.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.toolStrip_activity.ResumeLayout(false);
            this.toolStrip_activity.PerformLayout();
            this.panel_my_info.ResumeLayout(false);
            this.panel_my_info.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_header)).EndInit();
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
        private Panel panel_client_rates;
        private Panel panel_activities;
        private Label label3;
        private ToolStrip toolStrip_clients;
        private ToolStripButton toolStripButton_addClient;
        private ToolStripButton toolStripButton_editClient;
        private ToolStripButton toolStripButton_deleteClient;
        private ToolStripSeparator toolStripSeparator1;
        private ListView listView_activities;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private Panel panel6;
        private ToolStrip toolStrip_activity;
        private ToolStripButton toolStripButton_addActivity;
        private ToolStripButton toolStripButton_eidtActivity;
        private ToolStripButton toolStripButton_deleteActivity;
        private ToolStripSeparator toolStripSeparator2;
        private ListView listView_clientActivityRates;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private ToolStripButton toolStripButton_editActivityRate;
        private Panel panel5;
        private Panel panel7;
        private LinkLabel linkLabel_client_rates_activities_checkall;
        private Panel panel8;
        private Button button_Close;
        private Button button_Save;
        private LinkLabel linkLabel_client_rates_activities_uncheckall;
        private Panel panel9;
        private Label label_activities_count;
        private ColumnHeader columnHeader11;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader13;
        private Panel panel10;
        private LinkLabel linkLabel_client_resetDefaultRates;
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
        private TreeView treeView_clients;
        private Label label2;
        private Panel panel_client_rates_bottom;
        private Label label_clientCount;
        private Button button_help;
        private ComboBox comboBox_default_currency;
        private Label label1;
        private ContextMenuStrip contextMenuStrip_client_rates;
        private ToolStripMenuItem editActivityRateToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip_clients;
        private ToolStripMenuItem addClientToolStripMenuItem;
        private ToolStripMenuItem editClientToolStripMenuItem;
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
        private CheckBox checkBox_trackerConfirmActivities;
        private Label label16;
    }
}