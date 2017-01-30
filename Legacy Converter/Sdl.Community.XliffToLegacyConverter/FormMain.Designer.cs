namespace Sdl.Community.XliffToLegacyConverter
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel_Progress_Percentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel_Progress_Files = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.toolStripButton_Help = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox_viewReportWhenProcessingFinished = new System.Windows.Forms.CheckBox();
            this.textBox_reportFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_browse_reportDirectory = new System.Windows.Forms.Button();
            this.textBox_reportDirectory = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBox_includeLegacyStructure = new System.Windows.Forms.CheckBox();
            this.comboBox_OuputFormat = new ComboBoxEx();
            this.imageListComboBox = new System.Windows.Forms.ImageList(this.components);
            this.button_loadProjectExport = new System.Windows.Forms.Button();
            this.checkBox_reverseLanguageDirection = new System.Windows.Forms.CheckBox();
            this.checkBox_excludeTags = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label_itemCount_export = new System.Windows.Forms.Label();
            this.button_removeAll_Export = new System.Windows.Forms.Button();
            this.button_removeSDLXLIFF = new System.Windows.Forms.Button();
            this.button_addSDLXLIFF = new System.Windows.Forms.Button();
            this.listView_export = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolderContainingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button_loadProjectImport = new System.Windows.Forms.Button();
            this.button_removeAll_Import = new System.Windows.Forms.Button();
            this.checkBox_createBAKfile = new System.Windows.Forms.CheckBox();
            this.label_itemCount_import = new System.Windows.Forms.Label();
            this.button_DeleteRTF = new System.Windows.Forms.Button();
            this.button_addRTF = new System.Windows.Forms.Button();
            this.listView_import = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageListMainf = new System.Windows.Forms.ImageList(this.components);
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_Status,
            this.toolStripProgressBar1,
            this.toolStripStatusLabel_Progress_Percentage,
            this.toolStripStatusLabel_Progress_Files,
            this.toolStripStatusLabel_Message});
            this.statusStrip1.Location = new System.Drawing.Point(0, 563);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(837, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
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
            // toolStripStatusLabel_Progress_Files
            // 
            this.toolStripStatusLabel_Progress_Files.AutoSize = false;
            this.toolStripStatusLabel_Progress_Files.Name = "toolStripStatusLabel_Progress_Files";
            this.toolStripStatusLabel_Progress_Files.Size = new System.Drawing.Size(120, 17);
            // 
            // toolStripStatusLabel_Message
            // 
            this.toolStripStatusLabel_Message.Name = "toolStripStatusLabel_Message";
            this.toolStripStatusLabel_Message.Size = new System.Drawing.Size(460, 17);
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
            this.menuStrip1.Size = new System.Drawing.Size(837, 24);
            this.menuStrip1.TabIndex = 10;
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
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("runToolStripMenuItem.Image")));
            this.runToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.runToolStripMenuItem.Size = new System.Drawing.Size(215, 38);
            this.runToolStripMenuItem.Text = "&Start Processing";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(212, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("optionsToolStripMenuItem.Image")));
            this.optionsToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(215, 38);
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
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
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
            this.toolStripSeparator3,
            this.toolStripButton_Help});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(837, 55);
            this.toolStrip1.TabIndex = 11;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Run
            // 
            this.toolStripButton_Run.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Run.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Run.Image")));
            this.toolStripButton_Run.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Run.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Run.Name = "toolStripButton_Run";
            this.toolStripButton_Run.Size = new System.Drawing.Size(36, 52);
            this.toolStripButton_Run.Text = "Start Processing";
            this.toolStripButton_Run.Click += new System.EventHandler(this.toolStripButton_Run_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 55);
            // 
            // toolStripButton_Options
            // 
            this.toolStripButton_Options.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Options.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Options.Image")));
            this.toolStripButton_Options.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Options.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Options.Name = "toolStripButton_Options";
            this.toolStripButton_Options.Size = new System.Drawing.Size(36, 52);
            this.toolStripButton_Options.Text = "Settings";
            this.toolStripButton_Options.Click += new System.EventHandler(this.toolStripButton_Options_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 55);
            // 
            // toolStripButton_Help
            // 
            this.toolStripButton_Help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Help.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Help.Image")));
            this.toolStripButton_Help.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Help.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Help.Name = "toolStripButton_Help";
            this.toolStripButton_Help.Size = new System.Drawing.Size(52, 52);
            this.toolStripButton_Help.Text = "Help";
            this.toolStripButton_Help.Click += new System.EventHandler(this.toolStripButton_Help_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 79);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(837, 484);
            this.panel1.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(10, 357);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(5);
            this.panel2.Size = new System.Drawing.Size(817, 117);
            this.panel2.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_viewReportWhenProcessingFinished);
            this.groupBox2.Controls.Add(this.textBox_reportFileName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.button_browse_reportDirectory);
            this.groupBox2.Controls.Add(this.textBox_reportDirectory);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox2.Location = new System.Drawing.Point(5, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(807, 107);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Report";
            // 
            // checkBox_viewReportWhenProcessingFinished
            // 
            this.checkBox_viewReportWhenProcessingFinished.AutoSize = true;
            this.checkBox_viewReportWhenProcessingFinished.Checked = true;
            this.checkBox_viewReportWhenProcessingFinished.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_viewReportWhenProcessingFinished.Location = new System.Drawing.Point(120, 78);
            this.checkBox_viewReportWhenProcessingFinished.Name = "checkBox_viewReportWhenProcessingFinished";
            this.checkBox_viewReportWhenProcessingFinished.Size = new System.Drawing.Size(221, 17);
            this.checkBox_viewReportWhenProcessingFinished.TabIndex = 6;
            this.checkBox_viewReportWhenProcessingFinished.Text = "View report when processing has finished";
            this.checkBox_viewReportWhenProcessingFinished.UseVisualStyleBackColor = true;
            // 
            // textBox_reportFileName
            // 
            this.textBox_reportFileName.Location = new System.Drawing.Point(120, 53);
            this.textBox_reportFileName.Name = "textBox_reportFileName";
            this.textBox_reportFileName.Size = new System.Drawing.Size(396, 20);
            this.textBox_reportFileName.TabIndex = 5;
            this.textBox_reportFileName.TextChanged += new System.EventHandler(this.textBox_reportFileName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(29, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Report file name:";
            // 
            // button_browse_reportDirectory
            // 
            this.button_browse_reportDirectory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_browse_reportDirectory.Location = new System.Drawing.Point(750, 25);
            this.button_browse_reportDirectory.Name = "button_browse_reportDirectory";
            this.button_browse_reportDirectory.Size = new System.Drawing.Size(28, 23);
            this.button_browse_reportDirectory.TabIndex = 3;
            this.button_browse_reportDirectory.Text = "...";
            this.button_browse_reportDirectory.UseVisualStyleBackColor = true;
            this.button_browse_reportDirectory.Click += new System.EventHandler(this.button_browse_reportDirectory_Click);
            // 
            // textBox_reportDirectory
            // 
            this.textBox_reportDirectory.Location = new System.Drawing.Point(120, 28);
            this.textBox_reportDirectory.Name = "textBox_reportDirectory";
            this.textBox_reportDirectory.Size = new System.Drawing.Size(614, 20);
            this.textBox_reportDirectory.TabIndex = 2;
            this.textBox_reportDirectory.TextChanged += new System.EventHandler(this.textBox_reportDirectory_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(31, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Report directory:";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.ImageList = this.imageListMainf;
            this.tabControl1.ItemSize = new System.Drawing.Size(64, 26);
            this.tabControl1.Location = new System.Drawing.Point(10, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(817, 347);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.AllowDrop = true;
            this.tabPage1.Controls.Add(this.checkBox_includeLegacyStructure);
            this.tabPage1.Controls.Add(this.comboBox_OuputFormat);
            this.tabPage1.Controls.Add(this.button_loadProjectExport);
            this.tabPage1.Controls.Add(this.checkBox_reverseLanguageDirection);
            this.tabPage1.Controls.Add(this.checkBox_excludeTags);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.label_itemCount_export);
            this.tabPage1.Controls.Add(this.button_removeAll_Export);
            this.tabPage1.Controls.Add(this.button_removeSDLXLIFF);
            this.tabPage1.Controls.Add(this.button_addSDLXLIFF);
            this.tabPage1.Controls.Add(this.listView_export);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 30);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(809, 313);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Export";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBox_includeLegacyStructure
            // 
            this.checkBox_includeLegacyStructure.AutoSize = true;
            this.checkBox_includeLegacyStructure.Checked = true;
            this.checkBox_includeLegacyStructure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_includeLegacyStructure.Location = new System.Drawing.Point(477, 20);
            this.checkBox_includeLegacyStructure.Name = "checkBox_includeLegacyStructure";
            this.checkBox_includeLegacyStructure.Size = new System.Drawing.Size(145, 17);
            this.checkBox_includeLegacyStructure.TabIndex = 11;
            this.checkBox_includeLegacyStructure.Text = "Include Legacy Structure";
            this.checkBox_includeLegacyStructure.UseVisualStyleBackColor = true;
            // 
            // comboBox_OuputFormat
            // 
            this.comboBox_OuputFormat.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox_OuputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_OuputFormat.FormattingEnabled = true;
            this.comboBox_OuputFormat.ImageList = this.imageListComboBox;
            this.comboBox_OuputFormat.ItemHeight = 20;
            this.comboBox_OuputFormat.Location = new System.Drawing.Point(206, 14);
            this.comboBox_OuputFormat.Name = "comboBox_OuputFormat";
            this.comboBox_OuputFormat.Size = new System.Drawing.Size(237, 26);
            this.comboBox_OuputFormat.TabIndex = 2;
            this.comboBox_OuputFormat.SelectedIndexChanged += new System.EventHandler(this.comboBox_OuputFormat_SelectedIndexChanged);
            // 
            // imageListComboBox
            // 
            this.imageListComboBox.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListComboBox.ImageStream")));
            this.imageListComboBox.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListComboBox.Images.SetKeyName(0, "WordLang0.ico");
            this.imageListComboBox.Images.SetKeyName(1, "Page-White-Word.ico");
            this.imageListComboBox.Images.SetKeyName(2, "TagEditor0.ico");
            this.imageListComboBox.Images.SetKeyName(3, "TagEditor1.ico");
            this.imageListComboBox.Images.SetKeyName(4, "Word.ico");
            // 
            // button_loadProjectExport
            // 
            this.button_loadProjectExport.Location = new System.Drawing.Point(18, 114);
            this.button_loadProjectExport.Name = "button_loadProjectExport";
            this.button_loadProjectExport.Size = new System.Drawing.Size(88, 38);
            this.button_loadProjectExport.TabIndex = 6;
            this.button_loadProjectExport.Text = "Load files from project";
            this.button_loadProjectExport.UseVisualStyleBackColor = true;
            this.button_loadProjectExport.Click += new System.EventHandler(this.button_loadProjectExport_Click);
            // 
            // checkBox_reverseLanguageDirection
            // 
            this.checkBox_reverseLanguageDirection.AutoSize = true;
            this.checkBox_reverseLanguageDirection.Location = new System.Drawing.Point(567, 19);
            this.checkBox_reverseLanguageDirection.Name = "checkBox_reverseLanguageDirection";
            this.checkBox_reverseLanguageDirection.Size = new System.Drawing.Size(156, 17);
            this.checkBox_reverseLanguageDirection.TabIndex = 4;
            this.checkBox_reverseLanguageDirection.Text = "Reverse language direction";
            this.checkBox_reverseLanguageDirection.UseVisualStyleBackColor = true;
            // 
            // checkBox_excludeTags
            // 
            this.checkBox_excludeTags.AutoSize = true;
            this.checkBox_excludeTags.Location = new System.Drawing.Point(477, 19);
            this.checkBox_excludeTags.Name = "checkBox_excludeTags";
            this.checkBox_excludeTags.Size = new System.Drawing.Size(87, 17);
            this.checkBox_excludeTags.TabIndex = 3;
            this.checkBox_excludeTags.Text = "Exclude tags";
            this.checkBox_excludeTags.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(109, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Convert to format:";
            // 
            // label_itemCount_export
            // 
            this.label_itemCount_export.Location = new System.Drawing.Point(679, 281);
            this.label_itemCount_export.Name = "label_itemCount_export";
            this.label_itemCount_export.Size = new System.Drawing.Size(100, 23);
            this.label_itemCount_export.TabIndex = 10;
            this.label_itemCount_export.Text = "files 0";
            this.label_itemCount_export.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_removeAll_Export
            // 
            this.button_removeAll_Export.Location = new System.Drawing.Point(18, 185);
            this.button_removeAll_Export.Name = "button_removeAll_Export";
            this.button_removeAll_Export.Size = new System.Drawing.Size(88, 26);
            this.button_removeAll_Export.TabIndex = 8;
            this.button_removeAll_Export.Text = "Remove all";
            this.button_removeAll_Export.UseVisualStyleBackColor = true;
            this.button_removeAll_Export.Click += new System.EventHandler(this.button_removeAll_Export_Click);
            // 
            // button_removeSDLXLIFF
            // 
            this.button_removeSDLXLIFF.Location = new System.Drawing.Point(18, 156);
            this.button_removeSDLXLIFF.Name = "button_removeSDLXLIFF";
            this.button_removeSDLXLIFF.Size = new System.Drawing.Size(88, 25);
            this.button_removeSDLXLIFF.TabIndex = 7;
            this.button_removeSDLXLIFF.Text = "Remove";
            this.button_removeSDLXLIFF.UseVisualStyleBackColor = true;
            this.button_removeSDLXLIFF.Click += new System.EventHandler(this.button_removeSDLXLIFF_Click);
            // 
            // button_addSDLXLIFF
            // 
            this.button_addSDLXLIFF.Location = new System.Drawing.Point(18, 85);
            this.button_addSDLXLIFF.Name = "button_addSDLXLIFF";
            this.button_addSDLXLIFF.Size = new System.Drawing.Size(88, 25);
            this.button_addSDLXLIFF.TabIndex = 5;
            this.button_addSDLXLIFF.Text = "Add";
            this.button_addSDLXLIFF.UseVisualStyleBackColor = true;
            this.button_addSDLXLIFF.Click += new System.EventHandler(this.button_addSDLXLIFF_Click);
            // 
            // listView_export
            // 
            this.listView_export.AllowDrop = true;
            this.listView_export.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView_export.ContextMenuStrip = this.contextMenuStrip1;
            this.listView_export.FullRowSelect = true;
            this.listView_export.GridLines = true;
            this.listView_export.HideSelection = false;
            this.listView_export.Location = new System.Drawing.Point(112, 59);
            this.listView_export.Name = "listView_export";
            this.listView_export.Size = new System.Drawing.Size(667, 219);
            this.listView_export.TabIndex = 9;
            this.listView_export.UseCompatibleStateImageBehavior = false;
            this.listView_export.View = System.Windows.Forms.View.Details;
            this.listView_export.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_export_DragDrop);
            this.listView_export.DragOver += new System.Windows.Forms.DragEventHandler(this.listView_export_DragOver);
            this.listView_export.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_export_KeyUp);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "SDLXLIFF file path";
            this.columnHeader1.Width = 560;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.toolStripSeparator5,
            this.openFolderContainingToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(207, 54);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(203, 6);
            // 
            // openFolderContainingToolStripMenuItem
            // 
            this.openFolderContainingToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openFolderContainingToolStripMenuItem.Image")));
            this.openFolderContainingToolStripMenuItem.Name = "openFolderContainingToolStripMenuItem";
            this.openFolderContainingToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.openFolderContainingToolStripMenuItem.Text = "Open folder containing...";
            this.openFolderContainingToolStripMenuItem.Click += new System.EventHandler(this.openFolderContainingToolStripMenuItem_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button_loadProjectImport);
            this.tabPage2.Controls.Add(this.button_removeAll_Import);
            this.tabPage2.Controls.Add(this.checkBox_createBAKfile);
            this.tabPage2.Controls.Add(this.label_itemCount_import);
            this.tabPage2.Controls.Add(this.button_DeleteRTF);
            this.tabPage2.Controls.Add(this.button_addRTF);
            this.tabPage2.Controls.Add(this.listView_import);
            this.tabPage2.ImageIndex = 1;
            this.tabPage2.Location = new System.Drawing.Point(4, 30);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(809, 313);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Import";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button_loadProjectImport
            // 
            this.button_loadProjectImport.Location = new System.Drawing.Point(17, 91);
            this.button_loadProjectImport.Name = "button_loadProjectImport";
            this.button_loadProjectImport.Size = new System.Drawing.Size(89, 41);
            this.button_loadProjectImport.TabIndex = 2;
            this.button_loadProjectImport.Text = "Load files from project";
            this.button_loadProjectImport.UseVisualStyleBackColor = true;
            this.button_loadProjectImport.Click += new System.EventHandler(this.button_loadProjectImport_Click);
            // 
            // button_removeAll_Import
            // 
            this.button_removeAll_Import.Location = new System.Drawing.Point(17, 163);
            this.button_removeAll_Import.Name = "button_removeAll_Import";
            this.button_removeAll_Import.Size = new System.Drawing.Size(89, 29);
            this.button_removeAll_Import.TabIndex = 4;
            this.button_removeAll_Import.Text = "Remove all";
            this.button_removeAll_Import.UseVisualStyleBackColor = true;
            this.button_removeAll_Import.Click += new System.EventHandler(this.button_removeAll_Import_Click);
            // 
            // checkBox_createBAKfile
            // 
            this.checkBox_createBAKfile.AutoSize = true;
            this.checkBox_createBAKfile.Location = new System.Drawing.Point(112, 287);
            this.checkBox_createBAKfile.Name = "checkBox_createBAKfile";
            this.checkBox_createBAKfile.Size = new System.Drawing.Size(316, 17);
            this.checkBox_createBAKfile.TabIndex = 6;
            this.checkBox_createBAKfile.Text = "Create a backup of the original SDLXLIFF file before updating";
            this.checkBox_createBAKfile.UseVisualStyleBackColor = true;
            // 
            // label_itemCount_import
            // 
            this.label_itemCount_import.Location = new System.Drawing.Point(679, 281);
            this.label_itemCount_import.Name = "label_itemCount_import";
            this.label_itemCount_import.Size = new System.Drawing.Size(100, 23);
            this.label_itemCount_import.TabIndex = 7;
            this.label_itemCount_import.Text = "files 0";
            this.label_itemCount_import.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button_DeleteRTF
            // 
            this.button_DeleteRTF.Location = new System.Drawing.Point(17, 133);
            this.button_DeleteRTF.Name = "button_DeleteRTF";
            this.button_DeleteRTF.Size = new System.Drawing.Size(89, 29);
            this.button_DeleteRTF.TabIndex = 3;
            this.button_DeleteRTF.Text = "Remove";
            this.button_DeleteRTF.UseVisualStyleBackColor = true;
            this.button_DeleteRTF.Click += new System.EventHandler(this.button_RemoveRTF_Click);
            // 
            // button_addRTF
            // 
            this.button_addRTF.Location = new System.Drawing.Point(17, 60);
            this.button_addRTF.Name = "button_addRTF";
            this.button_addRTF.Size = new System.Drawing.Size(89, 29);
            this.button_addRTF.TabIndex = 1;
            this.button_addRTF.Text = "Add";
            this.button_addRTF.UseVisualStyleBackColor = true;
            this.button_addRTF.Click += new System.EventHandler(this.button_addRTF_Click);
            // 
            // listView_import
            // 
            this.listView_import.AllowDrop = true;
            this.listView_import.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listView_import.ContextMenuStrip = this.contextMenuStrip1;
            this.listView_import.FullRowSelect = true;
            this.listView_import.GridLines = true;
            this.listView_import.HideSelection = false;
            this.listView_import.Location = new System.Drawing.Point(112, 31);
            this.listView_import.Name = "listView_import";
            this.listView_import.Size = new System.Drawing.Size(667, 250);
            this.listView_import.TabIndex = 5;
            this.listView_import.UseCompatibleStateImageBehavior = false;
            this.listView_import.View = System.Windows.Forms.View.Details;
            this.listView_import.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView_import_DragDrop);
            this.listView_import.DragOver += new System.Windows.Forms.DragEventHandler(this.listView_import_DragOver);
            this.listView_import.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listView_import_KeyUp);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Legacy file name (*.rtf, *.doc, *.docx, *.ttx)";
            this.columnHeader2.Width = 230;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Original SDLXLIFF file name";
            this.columnHeader3.Width = 250;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Path";
            this.columnHeader4.Width = 350;
            // 
            // imageListMainf
            // 
            this.imageListMainf.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMainf.ImageStream")));
            this.imageListMainf.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMainf.Images.SetKeyName(0, "agt_add-to-autorun-32.png");
            this.imageListMainf.Images.SetKeyName(1, "Import-32.png");
            this.imageListMainf.Images.SetKeyName(2, "agt_add-to-autorun-32.png");
            this.imageListMainf.Images.SetKeyName(3, "Run-32.png");
            this.imageListMainf.Images.SetKeyName(4, "File-Extension-Rtf-32.png");
            this.imageListMainf.Images.SetKeyName(5, "rtf_32.png");
            this.imageListMainf.Images.SetKeyName(6, "help.png");
            this.imageListMainf.Images.SetKeyName(7, "info.png");
            this.imageListMainf.Images.SetKeyName(8, "Export-32.png");
            // 
            // helpProvider1
            // 
            this.helpProvider1.HelpNamespace = "SdlXliffToLegacyConverter.chm";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 585);
            this.Controls.Add(this.panel1);
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
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem help1ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_Run;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButton_Options;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_Help;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView listView_export;
        private System.Windows.Forms.Button button_addSDLXLIFF;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label_itemCount_export;
        private System.Windows.Forms.Button button_removeSDLXLIFF;
        private System.Windows.Forms.Label label_itemCount_import;
        private System.Windows.Forms.Button button_DeleteRTF;
        private System.Windows.Forms.Button button_addRTF;
        private System.Windows.Forms.ListView listView_import;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        public System.Windows.Forms.CheckBox checkBox_createBAKfile;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button button_removeAll_Export;
        private System.Windows.Forms.Button button_removeAll_Import;
        private System.Windows.Forms.Button button_loadProjectImport;
        private System.Windows.Forms.Button button_loadProjectExport;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_viewReportWhenProcessingFinished;
        private System.Windows.Forms.TextBox textBox_reportFileName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_browse_reportDirectory;
        private System.Windows.Forms.TextBox textBox_reportDirectory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_reverseLanguageDirection;
        private System.Windows.Forms.CheckBox checkBox_excludeTags;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem openFolderContainingToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_Progress_Files;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.ImageList imageListMainf;
        private ComboBoxEx comboBox_OuputFormat;
        private System.Windows.Forms.ImageList imageListComboBox;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.CheckBox checkBox_includeLegacyStructure;

    }
}

