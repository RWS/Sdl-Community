namespace Sdl.Community.XliffCompare
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel_Progress_Percentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_Message = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.help1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Run = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Options = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.panel_report_output_settings = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBox_reportFormat = new ComboBoxEx();
            this.imageList_Combobox = new System.Windows.Forms.ImageList(this.components);
            this.checkBox_viewReportWhenProcessingFinished = new System.Windows.Forms.CheckBox();
            this.textBox_reportFileName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_browse_reportDirectory = new System.Windows.Forms.Button();
            this.textBox_reportDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel_filePath_settings = new System.Windows.Forms.Panel();
            this.tabControl_comparison_type = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox_singleFilePathSettings = new System.Windows.Forms.GroupBox();
            this.panel_compareFiles = new System.Windows.Forms.Panel();
            this.label_dragNDropMessage_CompareFiles = new System.Windows.Forms.Label();
            this.button_Browse_UpdatedFile = new System.Windows.Forms.Button();
            this.button_Browse_OriginalFile = new System.Windows.Forms.Button();
            this.textBox_UpdatedFile = new System.Windows.Forms.TextBox();
            this.label_UpdatedFile = new System.Windows.Forms.Label();
            this.textBox_OriginalFile = new System.Windows.Forms.TextBox();
            this.label_OriginalFile = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel_compareDirectories = new System.Windows.Forms.Panel();
            this.label_dragNDropMessage_CompareDirectories = new System.Windows.Forms.Label();
            this.button_Browse_UpdatedDirectory = new System.Windows.Forms.Button();
            this.button_Browse_OriginalDirectory = new System.Windows.Forms.Button();
            this.checkBox_processSubFolders = new System.Windows.Forms.CheckBox();
            this.textBox_UpdatedDirectory = new System.Windows.Forms.TextBox();
            this.label_UpdatedDirectory = new System.Windows.Forms.Label();
            this.textBox_OriginalDirectory = new System.Windows.Forms.TextBox();
            this.label_OriginalDirectory = new System.Windows.Forms.Label();
            this.imageList_TabControl = new System.Windows.Forms.ImageList(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel_report_output_settings.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel_filePath_settings.SuspendLayout();
            this.tabControl_comparison_type.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox_singleFilePathSettings.SuspendLayout();
            this.panel_compareFiles.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel_compareDirectories.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_Status,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel_Progress_Percentage,
            this.toolStripStatusLabel_Message});
            this.statusStrip1.Location = new System.Drawing.Point(0, 444);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(688, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_Status
            // 
            this.toolStripStatusLabel_Status.AutoSize = false;
            this.toolStripStatusLabel_Status.Name = "toolStripStatusLabel_Status";
            this.toolStripStatusLabel_Status.Size = new System.Drawing.Size(100, 17);
            this.toolStripStatusLabel_Status.Text = "Ready";
            this.toolStripStatusLabel_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.AutoSize = false;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Step = 1;
            // 
            // toolStripStatusLabel_Progress_Percentage
            // 
            this.toolStripStatusLabel_Progress_Percentage.AutoSize = false;
            this.toolStripStatusLabel_Progress_Percentage.Name = "toolStripStatusLabel_Progress_Percentage";
            this.toolStripStatusLabel_Progress_Percentage.Size = new System.Drawing.Size(40, 17);
            this.toolStripStatusLabel_Progress_Percentage.Text = "0% ";
            // 
            // toolStripStatusLabel_Message
            // 
            this.toolStripStatusLabel_Message.Name = "toolStripStatusLabel_Message";
            this.toolStripStatusLabel_Message.Size = new System.Drawing.Size(431, 17);
            this.toolStripStatusLabel_Message.Spring = true;
            this.toolStripStatusLabel_Message.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(688, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
            this.closeToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(161, 38);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.toolStripSeparator1,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runToolStripMenuItem.Image")));
            this.runToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.runToolStripMenuItem.Size = new System.Drawing.Size(220, 38);
            this.runToolStripMenuItem.Text = "&Run Comparison";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(217, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
            this.optionsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(220, 38);
            this.optionsToolStripMenuItem.Text = "&Settings";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1,
            this.help1ToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem1.Image")));
            this.aboutToolStripMenuItem1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(157, 38);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.helpToolStripMenuItem1_Click);
            // 
            // help1ToolStripMenuItem
            // 
            this.help1ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("help1ToolStripMenuItem.Image")));
            this.help1ToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.help1ToolStripMenuItem.Name = "help1ToolStripMenuItem";
            this.help1ToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F1)));
            this.help1ToolStripMenuItem.Size = new System.Drawing.Size(157, 38);
            this.help1ToolStripMenuItem.Text = "&Help";
            this.help1ToolStripMenuItem.Click += new System.EventHandler(this.help1ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Run,
            this.toolStripSeparator2,
            this.toolStripButton_Options,
            this.toolStripSeparator3});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(688, 39);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Run
            // 
            this.toolStripButton_Run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Run.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Run.Image")));
            this.toolStripButton_Run.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Run.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Run.Name = "toolStripButton_Run";
            this.toolStripButton_Run.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_Run.Text = "Run Comparison";
            this.toolStripButton_Run.Click += new System.EventHandler(this.toolStripButton_Run_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButton_Options
            // 
            this.toolStripButton_Options.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Options.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Options.Image")));
            this.toolStripButton_Options.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Options.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Options.Name = "toolStripButton_Options";
            this.toolStripButton_Options.Size = new System.Drawing.Size(36, 36);
            this.toolStripButton_Options.Text = "Settings";
            this.toolStripButton_Options.Click += new System.EventHandler(this.toolStripButton_Options_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // panel_report_output_settings
            // 
            this.panel_report_output_settings.Controls.Add(this.groupBox2);
            this.panel_report_output_settings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_report_output_settings.Location = new System.Drawing.Point(0, 299);
            this.panel_report_output_settings.Name = "panel_report_output_settings";
            this.panel_report_output_settings.Padding = new System.Windows.Forms.Padding(8);
            this.panel_report_output_settings.Size = new System.Drawing.Size(688, 145);
            this.panel_report_output_settings.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.comboBox_reportFormat);
            this.groupBox2.Controls.Add(this.checkBox_viewReportWhenProcessingFinished);
            this.groupBox2.Controls.Add(this.textBox_reportFileName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button_browse_reportDirectory);
            this.groupBox2.Controls.Add(this.textBox_reportDirectory);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(672, 129);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report (output)";
            // 
            // comboBox_reportFormat
            // 
            this.comboBox_reportFormat.BackColor = System.Drawing.SystemColors.Control;
            this.comboBox_reportFormat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox_reportFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_reportFormat.FormattingEnabled = true;
            this.comboBox_reportFormat.ImageList = this.imageList_Combobox;
            this.comboBox_reportFormat.ItemHeight = 18;
            this.comboBox_reportFormat.Location = new System.Drawing.Point(528, 52);
            this.comboBox_reportFormat.Name = "comboBox_reportFormat";
            this.comboBox_reportFormat.Size = new System.Drawing.Size(71, 24);
            this.comboBox_reportFormat.TabIndex = 17;
            // 
            // imageList_Combobox
            // 
            this.imageList_Combobox.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_Combobox.ImageStream")));
            this.imageList_Combobox.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_Combobox.Images.SetKeyName(0, "HTMLPage.png");
            this.imageList_Combobox.Images.SetKeyName(1, "XMLFileHS.png");
            // 
            // checkBox_viewReportWhenProcessingFinished
            // 
            this.checkBox_viewReportWhenProcessingFinished.AutoSize = true;
            this.checkBox_viewReportWhenProcessingFinished.Checked = true;
            this.checkBox_viewReportWhenProcessingFinished.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_viewReportWhenProcessingFinished.Location = new System.Drawing.Point(120, 78);
            this.checkBox_viewReportWhenProcessingFinished.Name = "checkBox_viewReportWhenProcessingFinished";
            this.checkBox_viewReportWhenProcessingFinished.Size = new System.Drawing.Size(221, 17);
            this.checkBox_viewReportWhenProcessingFinished.TabIndex = 16;
            this.checkBox_viewReportWhenProcessingFinished.Text = "View report when processing has finished";
            this.checkBox_viewReportWhenProcessingFinished.UseVisualStyleBackColor = true;
            // 
            // textBox_reportFileName
            // 
            this.textBox_reportFileName.Location = new System.Drawing.Point(120, 53);
            this.textBox_reportFileName.Name = "textBox_reportFileName";
            this.textBox_reportFileName.Size = new System.Drawing.Size(295, 20);
            this.textBox_reportFileName.TabIndex = 15;
            this.textBox_reportFileName.TextChanged += new System.EventHandler(this.textBox_reportFileName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label3.Location = new System.Drawing.Point(448, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Report format:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(29, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Report file name:";
            // 
            // button_browse_reportDirectory
            // 
            this.button_browse_reportDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_browse_reportDirectory.Location = new System.Drawing.Point(605, 26);
            this.button_browse_reportDirectory.Name = "button_browse_reportDirectory";
            this.button_browse_reportDirectory.Size = new System.Drawing.Size(28, 23);
            this.button_browse_reportDirectory.TabIndex = 13;
            this.button_browse_reportDirectory.Text = "...";
            this.button_browse_reportDirectory.UseVisualStyleBackColor = true;
            this.button_browse_reportDirectory.Click += new System.EventHandler(this.button_browse_reportDirectory_Click);
            // 
            // textBox_reportDirectory
            // 
            this.textBox_reportDirectory.Location = new System.Drawing.Point(120, 28);
            this.textBox_reportDirectory.Name = "textBox_reportDirectory";
            this.textBox_reportDirectory.Size = new System.Drawing.Size(479, 20);
            this.textBox_reportDirectory.TabIndex = 12;
            this.textBox_reportDirectory.TextChanged += new System.EventHandler(this.textBox_reportDirectory_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(31, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Report directory:";
            // 
            // panel_filePath_settings
            // 
            this.panel_filePath_settings.Controls.Add(this.tabControl_comparison_type);
            this.panel_filePath_settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_filePath_settings.Location = new System.Drawing.Point(0, 63);
            this.panel_filePath_settings.Name = "panel_filePath_settings";
            this.panel_filePath_settings.Padding = new System.Windows.Forms.Padding(8);
            this.panel_filePath_settings.Size = new System.Drawing.Size(688, 236);
            this.panel_filePath_settings.TabIndex = 0;
            // 
            // tabControl_comparison_type
            // 
            this.tabControl_comparison_type.AllowDrop = true;
            this.tabControl_comparison_type.Controls.Add(this.tabPage1);
            this.tabControl_comparison_type.Controls.Add(this.tabPage2);
            this.tabControl_comparison_type.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_comparison_type.ImageList = this.imageList_TabControl;
            this.tabControl_comparison_type.ItemSize = new System.Drawing.Size(138, 36);
            this.tabControl_comparison_type.Location = new System.Drawing.Point(8, 8);
            this.tabControl_comparison_type.Multiline = true;
            this.tabControl_comparison_type.Name = "tabControl_comparison_type";
            this.tabControl_comparison_type.SelectedIndex = 0;
            this.tabControl_comparison_type.Size = new System.Drawing.Size(672, 220);
            this.tabControl_comparison_type.TabIndex = 0;
            this.tabControl_comparison_type.SelectedIndexChanged += new System.EventHandler(this.tabControl_comparison_type_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox_singleFilePathSettings);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 40);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(8, 10, 8, 8);
            this.tabPage1.Size = new System.Drawing.Size(664, 176);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Compare single file";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox_singleFilePathSettings
            // 
            this.groupBox_singleFilePathSettings.Controls.Add(this.panel_compareFiles);
            this.groupBox_singleFilePathSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_singleFilePathSettings.Location = new System.Drawing.Point(8, 10);
            this.groupBox_singleFilePathSettings.Name = "groupBox_singleFilePathSettings";
            this.groupBox_singleFilePathSettings.Size = new System.Drawing.Size(648, 158);
            this.groupBox_singleFilePathSettings.TabIndex = 0;
            this.groupBox_singleFilePathSettings.TabStop = false;
            this.groupBox_singleFilePathSettings.Text = "Settings (compare a single file)";
            // 
            // panel_compareFiles
            // 
            this.panel_compareFiles.AllowDrop = true;
            this.panel_compareFiles.Controls.Add(this.label_dragNDropMessage_CompareFiles);
            this.panel_compareFiles.Controls.Add(this.button_Browse_UpdatedFile);
            this.panel_compareFiles.Controls.Add(this.button_Browse_OriginalFile);
            this.panel_compareFiles.Controls.Add(this.textBox_UpdatedFile);
            this.panel_compareFiles.Controls.Add(this.label_UpdatedFile);
            this.panel_compareFiles.Controls.Add(this.textBox_OriginalFile);
            this.panel_compareFiles.Controls.Add(this.label_OriginalFile);
            this.panel_compareFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_compareFiles.Location = new System.Drawing.Point(3, 16);
            this.panel_compareFiles.Name = "panel_compareFiles";
            this.panel_compareFiles.Size = new System.Drawing.Size(642, 139);
            this.panel_compareFiles.TabIndex = 6;
            this.panel_compareFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_compareFiles_DragDrop);
            this.panel_compareFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.panel_compareFiles_DragOver);
            // 
            // label_dragNDropMessage_CompareFiles
            // 
            this.label_dragNDropMessage_CompareFiles.AllowDrop = true;
            this.label_dragNDropMessage_CompareFiles.AutoSize = true;
            this.label_dragNDropMessage_CompareFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_dragNDropMessage_CompareFiles.ForeColor = System.Drawing.Color.Tan;
            this.label_dragNDropMessage_CompareFiles.Location = new System.Drawing.Point(380, 100);
            this.label_dragNDropMessage_CompareFiles.Name = "label_dragNDropMessage_CompareFiles";
            this.label_dragNDropMessage_CompareFiles.Size = new System.Drawing.Size(244, 13);
            this.label_dragNDropMessage_CompareFiles.TabIndex = 12;
            this.label_dragNDropMessage_CompareFiles.Text = "Drag \'n Drop a pair of files to compare in this panel";
            this.label_dragNDropMessage_CompareFiles.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_compareFiles_DragDrop);
            this.label_dragNDropMessage_CompareFiles.DragOver += new System.Windows.Forms.DragEventHandler(this.panel_compareFiles_DragOver);
            // 
            // button_Browse_UpdatedFile
            // 
            this.button_Browse_UpdatedFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Browse_UpdatedFile.Location = new System.Drawing.Point(596, 58);
            this.button_Browse_UpdatedFile.Name = "button_Browse_UpdatedFile";
            this.button_Browse_UpdatedFile.Size = new System.Drawing.Size(28, 23);
            this.button_Browse_UpdatedFile.TabIndex = 11;
            this.button_Browse_UpdatedFile.Text = "...";
            this.button_Browse_UpdatedFile.UseVisualStyleBackColor = true;
            this.button_Browse_UpdatedFile.Click += new System.EventHandler(this.button_Browse_UpdatedFile_Click);
            // 
            // button_Browse_OriginalFile
            // 
            this.button_Browse_OriginalFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Browse_OriginalFile.Location = new System.Drawing.Point(596, 29);
            this.button_Browse_OriginalFile.Name = "button_Browse_OriginalFile";
            this.button_Browse_OriginalFile.Size = new System.Drawing.Size(28, 23);
            this.button_Browse_OriginalFile.TabIndex = 8;
            this.button_Browse_OriginalFile.Text = "...";
            this.button_Browse_OriginalFile.UseVisualStyleBackColor = true;
            this.button_Browse_OriginalFile.Click += new System.EventHandler(this.button_Browse_OriginalFile_Click);
            // 
            // textBox_UpdatedFile
            // 
            this.textBox_UpdatedFile.AllowDrop = true;
            this.textBox_UpdatedFile.Location = new System.Drawing.Point(111, 59);
            this.textBox_UpdatedFile.Name = "textBox_UpdatedFile";
            this.textBox_UpdatedFile.Size = new System.Drawing.Size(479, 20);
            this.textBox_UpdatedFile.TabIndex = 10;
            this.textBox_UpdatedFile.TextChanged += new System.EventHandler(this.textBox_UpdatedFile_TextChanged);
            this.textBox_UpdatedFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_UpdatedFile_DragDrop);
            this.textBox_UpdatedFile.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_UpdatedFile_DragOver);
            // 
            // label_UpdatedFile
            // 
            this.label_UpdatedFile.AutoSize = true;
            this.label_UpdatedFile.Location = new System.Drawing.Point(40, 63);
            this.label_UpdatedFile.Name = "label_UpdatedFile";
            this.label_UpdatedFile.Size = new System.Drawing.Size(67, 13);
            this.label_UpdatedFile.TabIndex = 9;
            this.label_UpdatedFile.Text = "Updated file:";
            // 
            // textBox_OriginalFile
            // 
            this.textBox_OriginalFile.AllowDrop = true;
            this.textBox_OriginalFile.Location = new System.Drawing.Point(111, 30);
            this.textBox_OriginalFile.Name = "textBox_OriginalFile";
            this.textBox_OriginalFile.Size = new System.Drawing.Size(479, 20);
            this.textBox_OriginalFile.TabIndex = 7;
            this.textBox_OriginalFile.TextChanged += new System.EventHandler(this.textBox_OriginalFile_TextChanged);
            this.textBox_OriginalFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_OriginalFile_DragDrop);
            this.textBox_OriginalFile.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_OriginalFile_DragOver);
            // 
            // label_OriginalFile
            // 
            this.label_OriginalFile.AutoSize = true;
            this.label_OriginalFile.Location = new System.Drawing.Point(46, 34);
            this.label_OriginalFile.Name = "label_OriginalFile";
            this.label_OriginalFile.Size = new System.Drawing.Size(61, 13);
            this.label_OriginalFile.TabIndex = 6;
            this.label_OriginalFile.Text = "Original file:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.ImageIndex = 1;
            this.tabPage2.Location = new System.Drawing.Point(4, 40);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(8, 10, 8, 8);
            this.tabPage2.Size = new System.Drawing.Size(664, 160);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Compare directory of files";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel_compareDirectories);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(8, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(648, 142);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings (compare a directory of files)";
            // 
            // panel_compareDirectories
            // 
            this.panel_compareDirectories.AllowDrop = true;
            this.panel_compareDirectories.Controls.Add(this.label_dragNDropMessage_CompareDirectories);
            this.panel_compareDirectories.Controls.Add(this.button_Browse_UpdatedDirectory);
            this.panel_compareDirectories.Controls.Add(this.button_Browse_OriginalDirectory);
            this.panel_compareDirectories.Controls.Add(this.checkBox_processSubFolders);
            this.panel_compareDirectories.Controls.Add(this.textBox_UpdatedDirectory);
            this.panel_compareDirectories.Controls.Add(this.label_UpdatedDirectory);
            this.panel_compareDirectories.Controls.Add(this.textBox_OriginalDirectory);
            this.panel_compareDirectories.Controls.Add(this.label_OriginalDirectory);
            this.panel_compareDirectories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_compareDirectories.Location = new System.Drawing.Point(3, 16);
            this.panel_compareDirectories.Name = "panel_compareDirectories";
            this.panel_compareDirectories.Size = new System.Drawing.Size(642, 123);
            this.panel_compareDirectories.TabIndex = 0;
            this.panel_compareDirectories.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_compareDirectories_DragDrop);
            this.panel_compareDirectories.DragOver += new System.Windows.Forms.DragEventHandler(this.panel_compareDirectories_DragOver);
            // 
            // label_dragNDropMessage_CompareDirectories
            // 
            this.label_dragNDropMessage_CompareDirectories.AllowDrop = true;
            this.label_dragNDropMessage_CompareDirectories.AutoSize = true;
            this.label_dragNDropMessage_CompareDirectories.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_dragNDropMessage_CompareDirectories.ForeColor = System.Drawing.Color.Tan;
            this.label_dragNDropMessage_CompareDirectories.Location = new System.Drawing.Point(350, 100);
            this.label_dragNDropMessage_CompareDirectories.Name = "label_dragNDropMessage_CompareDirectories";
            this.label_dragNDropMessage_CompareDirectories.Size = new System.Drawing.Size(274, 13);
            this.label_dragNDropMessage_CompareDirectories.TabIndex = 27;
            this.label_dragNDropMessage_CompareDirectories.Text = "Drag \'n Drop a pair of directories to compare in this panel";
            this.label_dragNDropMessage_CompareDirectories.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_compareDirectories_DragDrop);
            this.label_dragNDropMessage_CompareDirectories.DragOver += new System.Windows.Forms.DragEventHandler(this.panel_compareDirectories_DragOver);
            // 
            // button_Browse_UpdatedDirectory
            // 
            this.button_Browse_UpdatedDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Browse_UpdatedDirectory.Location = new System.Drawing.Point(596, 58);
            this.button_Browse_UpdatedDirectory.Name = "button_Browse_UpdatedDirectory";
            this.button_Browse_UpdatedDirectory.Size = new System.Drawing.Size(28, 23);
            this.button_Browse_UpdatedDirectory.TabIndex = 26;
            this.button_Browse_UpdatedDirectory.Text = "...";
            this.button_Browse_UpdatedDirectory.UseVisualStyleBackColor = true;
            this.button_Browse_UpdatedDirectory.Click += new System.EventHandler(this.button_Browse_UpdatedDirectory_Click);
            // 
            // button_Browse_OriginalDirectory
            // 
            this.button_Browse_OriginalDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Browse_OriginalDirectory.Location = new System.Drawing.Point(596, 29);
            this.button_Browse_OriginalDirectory.Name = "button_Browse_OriginalDirectory";
            this.button_Browse_OriginalDirectory.Size = new System.Drawing.Size(28, 23);
            this.button_Browse_OriginalDirectory.TabIndex = 25;
            this.button_Browse_OriginalDirectory.Text = "...";
            this.button_Browse_OriginalDirectory.UseVisualStyleBackColor = true;
            this.button_Browse_OriginalDirectory.Click += new System.EventHandler(this.button_Browse_OriginalDirectory_Click);
            // 
            // checkBox_processSubFolders
            // 
            this.checkBox_processSubFolders.AutoSize = true;
            this.checkBox_processSubFolders.Location = new System.Drawing.Point(111, 87);
            this.checkBox_processSubFolders.Name = "checkBox_processSubFolders";
            this.checkBox_processSubFolders.Size = new System.Drawing.Size(118, 17);
            this.checkBox_processSubFolders.TabIndex = 24;
            this.checkBox_processSubFolders.Text = "Process sub folders";
            this.checkBox_processSubFolders.UseVisualStyleBackColor = true;
            // 
            // textBox_UpdatedDirectory
            // 
            this.textBox_UpdatedDirectory.AllowDrop = true;
            this.textBox_UpdatedDirectory.Location = new System.Drawing.Point(111, 59);
            this.textBox_UpdatedDirectory.Name = "textBox_UpdatedDirectory";
            this.textBox_UpdatedDirectory.Size = new System.Drawing.Size(479, 20);
            this.textBox_UpdatedDirectory.TabIndex = 23;
            this.textBox_UpdatedDirectory.TextChanged += new System.EventHandler(this.textBox_UpdatedDirectory_TextChanged);
            this.textBox_UpdatedDirectory.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_UpdatedDirectory_DragDrop);
            this.textBox_UpdatedDirectory.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_UpdatedDirectory_DragOver);
            // 
            // label_UpdatedDirectory
            // 
            this.label_UpdatedDirectory.AutoSize = true;
            this.label_UpdatedDirectory.Location = new System.Drawing.Point(13, 63);
            this.label_UpdatedDirectory.Name = "label_UpdatedDirectory";
            this.label_UpdatedDirectory.Size = new System.Drawing.Size(94, 13);
            this.label_UpdatedDirectory.TabIndex = 22;
            this.label_UpdatedDirectory.Text = "Updated directory:";
            // 
            // textBox_OriginalDirectory
            // 
            this.textBox_OriginalDirectory.AllowDrop = true;
            this.textBox_OriginalDirectory.Location = new System.Drawing.Point(111, 30);
            this.textBox_OriginalDirectory.Name = "textBox_OriginalDirectory";
            this.textBox_OriginalDirectory.Size = new System.Drawing.Size(479, 20);
            this.textBox_OriginalDirectory.TabIndex = 21;
            this.textBox_OriginalDirectory.TextChanged += new System.EventHandler(this.textBox_OriginalDirectory_TextChanged);
            this.textBox_OriginalDirectory.DragDrop += new System.Windows.Forms.DragEventHandler(this.textBox_OriginalDirectory_DragDrop);
            this.textBox_OriginalDirectory.DragOver += new System.Windows.Forms.DragEventHandler(this.textBox_OriginalDirectory_DragOver);
            // 
            // label_OriginalDirectory
            // 
            this.label_OriginalDirectory.AutoSize = true;
            this.label_OriginalDirectory.Location = new System.Drawing.Point(19, 34);
            this.label_OriginalDirectory.Name = "label_OriginalDirectory";
            this.label_OriginalDirectory.Size = new System.Drawing.Size(88, 13);
            this.label_OriginalDirectory.TabIndex = 20;
            this.label_OriginalDirectory.Text = "Original directory:";
            // 
            // imageList_TabControl
            // 
            this.imageList_TabControl.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_TabControl.ImageStream")));
            this.imageList_TabControl.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_TabControl.Images.SetKeyName(0, "file-32.png");
            this.imageList_TabControl.Images.SetKeyName(1, "Folder-files-32.png");
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = "SDLXLIFFCompare.chm";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 466);
            this.Controls.Add(this.panel_filePath_settings);
            this.Controls.Add(this.panel_report_output_settings);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel_report_output_settings.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel_filePath_settings.ResumeLayout(false);
            this.tabControl_comparison_type.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox_singleFilePathSettings.ResumeLayout(false);
            this.panel_compareFiles.ResumeLayout(false);
            this.panel_compareFiles.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.panel_compareDirectories.ResumeLayout(false);
            this.panel_compareDirectories.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Status;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Progress_Percentage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Message;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem help1ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Run;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton_Options;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Panel panel_report_output_settings;
        private System.Windows.Forms.Panel panel_filePath_settings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox_reportFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_browse_reportDirectory;
        private System.Windows.Forms.TextBox textBox_reportDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl_comparison_type;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox_singleFilePathSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_viewReportWhenProcessingFinished;
        private System.Windows.Forms.ImageList imageList_TabControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Panel panel_compareFiles;
        private System.Windows.Forms.Panel panel_compareDirectories;
        private System.Windows.Forms.Button button_Browse_UpdatedDirectory;
        private System.Windows.Forms.Button button_Browse_OriginalDirectory;
        private System.Windows.Forms.CheckBox checkBox_processSubFolders;
        private System.Windows.Forms.TextBox textBox_UpdatedDirectory;
        private System.Windows.Forms.Label label_UpdatedDirectory;
        private System.Windows.Forms.TextBox textBox_OriginalDirectory;
        private System.Windows.Forms.Label label_OriginalDirectory;
        private System.Windows.Forms.Label label_dragNDropMessage_CompareFiles;
        private System.Windows.Forms.Label label_dragNDropMessage_CompareDirectories;
        private ComboBoxEx comboBox_reportFormat;
        private System.Windows.Forms.ImageList imageList_Combobox;
        private System.Windows.Forms.Button button_Browse_UpdatedFile;
        private System.Windows.Forms.Button button_Browse_OriginalFile;
        private System.Windows.Forms.TextBox textBox_UpdatedFile;
        private System.Windows.Forms.Label label_UpdatedFile;
        private System.Windows.Forms.TextBox textBox_OriginalFile;
        private System.Windows.Forms.Label label_OriginalFile;
    }
}

