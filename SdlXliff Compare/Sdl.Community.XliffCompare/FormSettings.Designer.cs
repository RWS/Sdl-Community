namespace Sdl.Community.XliffCompare
{
    partial class FormSettings
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("General Settings", 0, 0);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Custom style sheet", 4, 4);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Report Settings", 2, 2, new System.Windows.Forms.TreeNode[] {
            treeNode2});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.button_Close = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.treeView_main = new System.Windows.Forms.TreeView();
            this.imageList_TreeView = new System.Windows.Forms.ImageList(this.components);
            this.button_resetToDefaults = new System.Windows.Forms.Button();
            this.panel_General = new System.Windows.Forms.Panel();
            this.groupBox_Comparision = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_includeIndividualFileInformationMergedFiles = new System.Windows.Forms.CheckBox();
            this.checkBox_includeTagsInComparison = new System.Windows.Forms.CheckBox();
            this.comboBox_comparisonType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel14 = new System.Windows.Forms.Panel();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.panel_Report = new System.Windows.Forms.Panel();
            this.groupBox_Report_Settings = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button_changeFormattingTagRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTagNew = new System.Windows.Forms.Button();
            this.button_changeFormattingTextRemoved = new System.Windows.Forms.Button();
            this.button_changeFormattingTextNew = new System.Windows.Forms.Button();
            this.richTextBox_formatting_Tag_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Tag_New = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_Removed = new System.Windows.Forms.RichTextBox();
            this.richTextBox_formatting_Text_New = new System.Windows.Forms.RichTextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_reportFilterSegmentStatusChanged = new System.Windows.Forms.CheckBox();
            this.checkBox_reportFilterFilesWithNoRecordsFiltered = new System.Windows.Forms.CheckBox();
            this.checkBox_reportFilterSegmentsContainingComments = new System.Windows.Forms.CheckBox();
            this.checkBox_reportFilterChangedTargetContent = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel_styleSheet = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_browseCustomXsltFilePath = new System.Windows.Forms.Button();
            this.textBox_Custom_xsltFilePath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox_useCustomStyleSheet = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.panel_General.SuspendLayout();
            this.groupBox_Comparision.SuspendLayout();
            this.panel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.panel_Report.SuspendLayout();
            this.groupBox_Report_Settings.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel_styleSheet.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Close
            // 
            this.button_Close.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Close.Location = new System.Drawing.Point(749, 550);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 3;
            this.button_Close.Text = "&Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // button_Save
            // 
            this.button_Save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_Save.Location = new System.Drawing.Point(668, 550);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(75, 23);
            this.button_Save.TabIndex = 4;
            this.button_Save.Text = "&Save";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.button_Save_Click);
            // 
            // treeView_main
            // 
            this.treeView_main.FullRowSelect = true;
            this.treeView_main.HideSelection = false;
            this.treeView_main.ImageIndex = 0;
            this.treeView_main.ImageList = this.imageList_TreeView;
            this.treeView_main.Location = new System.Drawing.Point(12, 12);
            this.treeView_main.Name = "treeView_main";
            treeNode1.ImageIndex = 0;
            treeNode1.Name = "Node_GeneralSettings";
            treeNode1.SelectedImageIndex = 0;
            treeNode1.Text = "General Settings";
            treeNode2.ImageIndex = 4;
            treeNode2.Name = "Node_CustomStyleSheet";
            treeNode2.SelectedImageIndex = 4;
            treeNode2.Text = "Custom style sheet";
            treeNode3.ImageIndex = 2;
            treeNode3.Name = "Node_ReportSettings";
            treeNode3.SelectedImageIndex = 2;
            treeNode3.Text = "Report Settings";
            this.treeView_main.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode3});
            this.treeView_main.SelectedImageIndex = 0;
            this.treeView_main.Size = new System.Drawing.Size(271, 520);
            this.treeView_main.TabIndex = 5;
            this.treeView_main.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_main_AfterSelect);
            // 
            // imageList_TreeView
            // 
            this.imageList_TreeView.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList_TreeView.ImageStream")));
            this.imageList_TreeView.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList_TreeView.Images.SetKeyName(0, "Settings-48.png");
            this.imageList_TreeView.Images.SetKeyName(1, "Document-Compare-48.png");
            this.imageList_TreeView.Images.SetKeyName(2, "Users files-48.png");
            this.imageList_TreeView.Images.SetKeyName(3, "Document-Network-48.png");
            this.imageList_TreeView.Images.SetKeyName(4, "Font Folder_48.png");
            // 
            // button_resetToDefaults
            // 
            this.button_resetToDefaults.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_resetToDefaults.Location = new System.Drawing.Point(12, 550);
            this.button_resetToDefaults.Name = "button_resetToDefaults";
            this.button_resetToDefaults.Size = new System.Drawing.Size(107, 23);
            this.button_resetToDefaults.TabIndex = 4;
            this.button_resetToDefaults.Text = "&Reset to Defaults";
            this.button_resetToDefaults.UseVisualStyleBackColor = true;
            this.button_resetToDefaults.Click += new System.EventHandler(this.button_resetToDefaults_Click);
            // 
            // panel_General
            // 
            this.panel_General.Controls.Add(this.groupBox_Comparision);
            this.panel_General.Controls.Add(this.panel14);
            this.panel_General.Location = new System.Drawing.Point(300, 12);
            this.panel_General.Name = "panel_General";
            this.panel_General.Size = new System.Drawing.Size(524, 520);
            this.panel_General.TabIndex = 6;
            // 
            // groupBox_Comparision
            // 
            this.groupBox_Comparision.Controls.Add(this.label4);
            this.groupBox_Comparision.Controls.Add(this.checkBox_includeIndividualFileInformationMergedFiles);
            this.groupBox_Comparision.Controls.Add(this.checkBox_includeTagsInComparison);
            this.groupBox_Comparision.Controls.Add(this.comboBox_comparisonType);
            this.groupBox_Comparision.Controls.Add(this.label1);
            this.groupBox_Comparision.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Comparision.Location = new System.Drawing.Point(0, 56);
            this.groupBox_Comparision.Name = "groupBox_Comparision";
            this.groupBox_Comparision.Size = new System.Drawing.Size(524, 464);
            this.groupBox_Comparision.TabIndex = 37;
            this.groupBox_Comparision.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(23, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 71;
            this.label4.Text = "Comparison";
            // 
            // checkBox_includeIndividualFileInformationMergedFiles
            // 
            this.checkBox_includeIndividualFileInformationMergedFiles.AutoSize = true;
            this.checkBox_includeIndividualFileInformationMergedFiles.Location = new System.Drawing.Point(47, 124);
            this.checkBox_includeIndividualFileInformationMergedFiles.Name = "checkBox_includeIndividualFileInformationMergedFiles";
            this.checkBox_includeIndividualFileInformationMergedFiles.Size = new System.Drawing.Size(318, 17);
            this.checkBox_includeIndividualFileInformationMergedFiles.TabIndex = 2;
            this.checkBox_includeIndividualFileInformationMergedFiles.Text = "Include individual file information when comparing merged files";
            this.checkBox_includeIndividualFileInformationMergedFiles.UseVisualStyleBackColor = true;
            // 
            // checkBox_includeTagsInComparison
            // 
            this.checkBox_includeTagsInComparison.AutoSize = true;
            this.checkBox_includeTagsInComparison.Location = new System.Drawing.Point(47, 101);
            this.checkBox_includeTagsInComparison.Name = "checkBox_includeTagsInComparison";
            this.checkBox_includeTagsInComparison.Size = new System.Drawing.Size(206, 17);
            this.checkBox_includeTagsInComparison.TabIndex = 2;
            this.checkBox_includeTagsInComparison.Text = "Include TAG content when comparing";
            this.checkBox_includeTagsInComparison.UseVisualStyleBackColor = true;
            // 
            // comboBox_comparisonType
            // 
            this.comboBox_comparisonType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_comparisonType.FormattingEnabled = true;
            this.comboBox_comparisonType.Items.AddRange(new object[] {
            "words",
            "characters"});
            this.comboBox_comparisonType.Location = new System.Drawing.Point(138, 61);
            this.comboBox_comparisonType.Name = "comboBox_comparisonType";
            this.comboBox_comparisonType.Size = new System.Drawing.Size(121, 21);
            this.comboBox_comparisonType.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(44, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Comparison type:";
            // 
            // panel14
            // 
            this.panel14.BackColor = System.Drawing.SystemColors.Window;
            this.panel14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel14.Controls.Add(this.textBox12);
            this.panel14.Controls.Add(this.pictureBox7);
            this.panel14.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel14.Location = new System.Drawing.Point(0, 0);
            this.panel14.Name = "panel14";
            this.panel14.Size = new System.Drawing.Size(524, 56);
            this.panel14.TabIndex = 36;
            // 
            // textBox12
            // 
            this.textBox12.BackColor = System.Drawing.Color.White;
            this.textBox12.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox12.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox12.Location = new System.Drawing.Point(118, 16);
            this.textBox12.Name = "textBox12";
            this.textBox12.ReadOnly = true;
            this.textBox12.Size = new System.Drawing.Size(333, 25);
            this.textBox12.TabIndex = 3;
            this.textBox12.TabStop = false;
            this.textBox12.Text = "General Settings";
            // 
            // pictureBox7
            // 
            this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
            this.pictureBox7.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox7.InitialImage")));
            this.pictureBox7.Location = new System.Drawing.Point(25, 3);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(61, 48);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox7.TabIndex = 0;
            this.pictureBox7.TabStop = false;
            // 
            // panel_Report
            // 
            this.panel_Report.Controls.Add(this.groupBox_Report_Settings);
            this.panel_Report.Controls.Add(this.panel2);
            this.panel_Report.Location = new System.Drawing.Point(300, 12);
            this.panel_Report.Name = "panel_Report";
            this.panel_Report.Size = new System.Drawing.Size(524, 520);
            this.panel_Report.TabIndex = 7;
            // 
            // groupBox_Report_Settings
            // 
            this.groupBox_Report_Settings.Controls.Add(this.label6);
            this.groupBox_Report_Settings.Controls.Add(this.button_changeFormattingTagRemoved);
            this.groupBox_Report_Settings.Controls.Add(this.button_changeFormattingTagNew);
            this.groupBox_Report_Settings.Controls.Add(this.button_changeFormattingTextRemoved);
            this.groupBox_Report_Settings.Controls.Add(this.button_changeFormattingTextNew);
            this.groupBox_Report_Settings.Controls.Add(this.richTextBox_formatting_Tag_Removed);
            this.groupBox_Report_Settings.Controls.Add(this.richTextBox_formatting_Tag_New);
            this.groupBox_Report_Settings.Controls.Add(this.richTextBox_formatting_Text_Removed);
            this.groupBox_Report_Settings.Controls.Add(this.richTextBox_formatting_Text_New);
            this.groupBox_Report_Settings.Controls.Add(this.panel3);
            this.groupBox_Report_Settings.Controls.Add(this.panel1);
            this.groupBox_Report_Settings.Controls.Add(this.label8);
            this.groupBox_Report_Settings.Controls.Add(this.label3);
            this.groupBox_Report_Settings.Controls.Add(this.label2);
            this.groupBox_Report_Settings.Controls.Add(this.checkBox_reportFilterSegmentStatusChanged);
            this.groupBox_Report_Settings.Controls.Add(this.checkBox_reportFilterFilesWithNoRecordsFiltered);
            this.groupBox_Report_Settings.Controls.Add(this.checkBox_reportFilterSegmentsContainingComments);
            this.groupBox_Report_Settings.Controls.Add(this.checkBox_reportFilterChangedTargetContent);
            this.groupBox_Report_Settings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_Report_Settings.Location = new System.Drawing.Point(0, 56);
            this.groupBox_Report_Settings.Name = "groupBox_Report_Settings";
            this.groupBox_Report_Settings.Size = new System.Drawing.Size(524, 464);
            this.groupBox_Report_Settings.TabIndex = 37;
            this.groupBox_Report_Settings.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(41, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(373, 13);
            this.label6.TabIndex = 95;
            this.label6.Text = "Note: when no filters are selected, then all segments are included in the report";
            // 
            // button_changeFormattingTagRemoved
            // 
            this.button_changeFormattingTagRemoved.Location = new System.Drawing.Point(410, 414);
            this.button_changeFormattingTagRemoved.Name = "button_changeFormattingTagRemoved";
            this.button_changeFormattingTagRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagRemoved.TabIndex = 92;
            this.button_changeFormattingTagRemoved.Text = "Change...";
            this.button_changeFormattingTagRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagRemoved.Click += new System.EventHandler(this.button_changeFormattingTagRemoved_Click);
            // 
            // button_changeFormattingTagNew
            // 
            this.button_changeFormattingTagNew.Location = new System.Drawing.Point(410, 376);
            this.button_changeFormattingTagNew.Name = "button_changeFormattingTagNew";
            this.button_changeFormattingTagNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTagNew.TabIndex = 91;
            this.button_changeFormattingTagNew.Text = "Change...";
            this.button_changeFormattingTagNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTagNew.Click += new System.EventHandler(this.button_changeFormattingTagNew_Click);
            // 
            // button_changeFormattingTextRemoved
            // 
            this.button_changeFormattingTextRemoved.Location = new System.Drawing.Point(410, 338);
            this.button_changeFormattingTextRemoved.Name = "button_changeFormattingTextRemoved";
            this.button_changeFormattingTextRemoved.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextRemoved.TabIndex = 94;
            this.button_changeFormattingTextRemoved.Text = "Change...";
            this.button_changeFormattingTextRemoved.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextRemoved.Click += new System.EventHandler(this.button_changeFormattingTextRemoved_Click);
            // 
            // button_changeFormattingTextNew
            // 
            this.button_changeFormattingTextNew.Location = new System.Drawing.Point(410, 300);
            this.button_changeFormattingTextNew.Name = "button_changeFormattingTextNew";
            this.button_changeFormattingTextNew.Size = new System.Drawing.Size(75, 23);
            this.button_changeFormattingTextNew.TabIndex = 93;
            this.button_changeFormattingTextNew.Text = "Change...";
            this.button_changeFormattingTextNew.UseVisualStyleBackColor = true;
            this.button_changeFormattingTextNew.Click += new System.EventHandler(this.button_changeFormattingTextNew_Click);
            // 
            // richTextBox_formatting_Tag_Removed
            // 
            this.richTextBox_formatting_Tag_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_Removed.Location = new System.Drawing.Point(43, 414);
            this.richTextBox_formatting_Tag_Removed.Name = "richTextBox_formatting_Tag_Removed";
            this.richTextBox_formatting_Tag_Removed.ReadOnly = true;
            this.richTextBox_formatting_Tag_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_Removed.TabIndex = 90;
            this.richTextBox_formatting_Tag_Removed.Text = "";
            // 
            // richTextBox_formatting_Tag_New
            // 
            this.richTextBox_formatting_Tag_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Tag_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Tag_New.Location = new System.Drawing.Point(43, 376);
            this.richTextBox_formatting_Tag_New.Name = "richTextBox_formatting_Tag_New";
            this.richTextBox_formatting_Tag_New.ReadOnly = true;
            this.richTextBox_formatting_Tag_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Tag_New.TabIndex = 87;
            this.richTextBox_formatting_Tag_New.Text = "";
            // 
            // richTextBox_formatting_Text_Removed
            // 
            this.richTextBox_formatting_Text_Removed.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_Removed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_Removed.Location = new System.Drawing.Point(43, 338);
            this.richTextBox_formatting_Text_Removed.Name = "richTextBox_formatting_Text_Removed";
            this.richTextBox_formatting_Text_Removed.ReadOnly = true;
            this.richTextBox_formatting_Text_Removed.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_Removed.TabIndex = 88;
            this.richTextBox_formatting_Text_Removed.Text = "";
            // 
            // richTextBox_formatting_Text_New
            // 
            this.richTextBox_formatting_Text_New.BackColor = System.Drawing.Color.White;
            this.richTextBox_formatting_Text_New.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox_formatting_Text_New.Location = new System.Drawing.Point(43, 300);
            this.richTextBox_formatting_Text_New.Name = "richTextBox_formatting_Text_New";
            this.richTextBox_formatting_Text_New.ReadOnly = true;
            this.richTextBox_formatting_Text_New.Size = new System.Drawing.Size(356, 32);
            this.richTextBox_formatting_Text_New.TabIndex = 89;
            this.richTextBox_formatting_Text_New.Text = "";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Location = new System.Drawing.Point(213, 181);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(270, 4);
            this.panel3.TabIndex = 86;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(213, 266);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(270, 4);
            this.panel1.TabIndex = 86;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Navy;
            this.label8.Location = new System.Drawing.Point(22, 175);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(167, 17);
            this.label8.TabIndex = 85;
            this.label8.Text = "File Summary Information";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(22, 260);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(147, 17);
            this.label3.TabIndex = 85;
            this.label3.Text = "Differences formatting";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(23, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 70;
            this.label2.Text = "Filter Settings";
            // 
            // checkBox_reportFilterSegmentStatusChanged
            // 
            this.checkBox_reportFilterSegmentStatusChanged.AutoSize = true;
            this.checkBox_reportFilterSegmentStatusChanged.Location = new System.Drawing.Point(44, 76);
            this.checkBox_reportFilterSegmentStatusChanged.Name = "checkBox_reportFilterSegmentStatusChanged";
            this.checkBox_reportFilterSegmentStatusChanged.Size = new System.Drawing.Size(276, 17);
            this.checkBox_reportFilterSegmentStatusChanged.TabIndex = 3;
            this.checkBox_reportFilterSegmentStatusChanged.Text = "Segment status changed (i.e. Draft, Translated etc...)";
            this.checkBox_reportFilterSegmentStatusChanged.UseVisualStyleBackColor = true;
            // 
            // checkBox_reportFilterFilesWithNoRecordsFiltered
            // 
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.AutoSize = true;
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.Location = new System.Drawing.Point(46, 209);
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.Name = "checkBox_reportFilterFilesWithNoRecordsFiltered";
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.Size = new System.Drawing.Size(223, 17);
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.TabIndex = 4;
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.Text = "View files that have no segments reported";
            this.checkBox_reportFilterFilesWithNoRecordsFiltered.UseVisualStyleBackColor = true;
            // 
            // checkBox_reportFilterSegmentsContainingComments
            // 
            this.checkBox_reportFilterSegmentsContainingComments.AutoSize = true;
            this.checkBox_reportFilterSegmentsContainingComments.Location = new System.Drawing.Point(44, 98);
            this.checkBox_reportFilterSegmentsContainingComments.Name = "checkBox_reportFilterSegmentsContainingComments";
            this.checkBox_reportFilterSegmentsContainingComments.Size = new System.Drawing.Size(162, 17);
            this.checkBox_reportFilterSegmentsContainingComments.TabIndex = 4;
            this.checkBox_reportFilterSegmentsContainingComments.Text = "Segment contains comments";
            this.checkBox_reportFilterSegmentsContainingComments.UseVisualStyleBackColor = true;
            // 
            // checkBox_reportFilterChangedTargetContent
            // 
            this.checkBox_reportFilterChangedTargetContent.AutoSize = true;
            this.checkBox_reportFilterChangedTargetContent.Location = new System.Drawing.Point(44, 54);
            this.checkBox_reportFilterChangedTargetContent.Name = "checkBox_reportFilterChangedTargetContent";
            this.checkBox_reportFilterChangedTargetContent.Size = new System.Drawing.Size(171, 17);
            this.checkBox_reportFilterChangedTargetContent.TabIndex = 2;
            this.checkBox_reportFilterChangedTargetContent.Text = "Segment translation is modified";
            this.checkBox_reportFilterChangedTargetContent.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(524, 56);
            this.panel2.TabIndex = 36;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(118, 16);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(333, 25);
            this.textBox1.TabIndex = 3;
            this.textBox1.TabStop = false;
            this.textBox1.Text = "Report Settings";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(25, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(61, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel_styleSheet
            // 
            this.panel_styleSheet.Controls.Add(this.groupBox1);
            this.panel_styleSheet.Controls.Add(this.panel4);
            this.panel_styleSheet.Location = new System.Drawing.Point(300, 12);
            this.panel_styleSheet.Name = "panel_styleSheet";
            this.panel_styleSheet.Size = new System.Drawing.Size(524, 520);
            this.panel_styleSheet.TabIndex = 8;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_browseCustomXsltFilePath);
            this.groupBox1.Controls.Add(this.textBox_Custom_xsltFilePath);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.checkBox_useCustomStyleSheet);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(524, 464);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            // 
            // button_browseCustomXsltFilePath
            // 
            this.button_browseCustomXsltFilePath.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.button_browseCustomXsltFilePath.Location = new System.Drawing.Point(458, 82);
            this.button_browseCustomXsltFilePath.Name = "button_browseCustomXsltFilePath";
            this.button_browseCustomXsltFilePath.Size = new System.Drawing.Size(28, 23);
            this.button_browseCustomXsltFilePath.TabIndex = 73;
            this.button_browseCustomXsltFilePath.Text = "...";
            this.button_browseCustomXsltFilePath.UseVisualStyleBackColor = true;
            this.button_browseCustomXsltFilePath.Click += new System.EventHandler(this.button_browseCustomXsltFilePath_Click);
            // 
            // textBox_Custom_xsltFilePath
            // 
            this.textBox_Custom_xsltFilePath.Location = new System.Drawing.Point(138, 85);
            this.textBox_Custom_xsltFilePath.Name = "textBox_Custom_xsltFilePath";
            this.textBox_Custom_xsltFilePath.Size = new System.Drawing.Size(314, 20);
            this.textBox_Custom_xsltFilePath.TabIndex = 72;
            this.textBox_Custom_xsltFilePath.TextChanged += new System.EventHandler(this.textBox_Custom_xsltFilePath_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Location = new System.Drawing.Point(79, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 71;
            this.label5.Text = "XSLT file:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Navy;
            this.label7.Location = new System.Drawing.Point(23, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(146, 17);
            this.label7.TabIndex = 70;
            this.label7.Text = "Extensible Style sheet";
            // 
            // checkBox_useCustomStyleSheet
            // 
            this.checkBox_useCustomStyleSheet.AutoSize = true;
            this.checkBox_useCustomStyleSheet.Location = new System.Drawing.Point(44, 58);
            this.checkBox_useCustomStyleSheet.Name = "checkBox_useCustomStyleSheet";
            this.checkBox_useCustomStyleSheet.Size = new System.Drawing.Size(135, 17);
            this.checkBox_useCustomStyleSheet.TabIndex = 4;
            this.checkBox_useCustomStyleSheet.Text = "Use custom style sheet";
            this.checkBox_useCustomStyleSheet.UseVisualStyleBackColor = true;
            this.checkBox_useCustomStyleSheet.CheckedChanged += new System.EventHandler(this.checkBox_useCustomStyleSheet_CheckedChanged);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Window;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.textBox3);
            this.panel4.Controls.Add(this.pictureBox2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(524, 56);
            this.panel4.TabIndex = 36;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.White;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(118, 16);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(333, 25);
            this.textBox3.TabIndex = 3;
            this.textBox3.TabStop = false;
            this.textBox3.Text = "Custom style sheet";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.InitialImage")));
            this.pictureBox2.Location = new System.Drawing.Point(25, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(61, 48);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 585);
            this.Controls.Add(this.treeView_main);
            this.Controls.Add(this.button_resetToDefaults);
            this.Controls.Add(this.button_Save);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.panel_Report);
            this.Controls.Add(this.panel_General);
            this.Controls.Add(this.panel_styleSheet);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormOptions";
            this.Load += new System.EventHandler(this.FormOptions_Load);
            this.panel_General.ResumeLayout(false);
            this.groupBox_Comparision.ResumeLayout(false);
            this.groupBox_Comparision.PerformLayout();
            this.panel14.ResumeLayout(false);
            this.panel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            this.panel_Report.ResumeLayout(false);
            this.groupBox_Report_Settings.ResumeLayout(false);
            this.groupBox_Report_Settings.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel_styleSheet.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.TreeView treeView_main;
        private System.Windows.Forms.Button button_resetToDefaults;
        private System.Windows.Forms.Panel panel_General;
        private System.Windows.Forms.Panel panel14;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.GroupBox groupBox_Comparision;
        public System.Windows.Forms.CheckBox checkBox_includeTagsInComparison;
        public System.Windows.Forms.ComboBox comboBox_comparisonType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel_Report;
        private System.Windows.Forms.GroupBox groupBox_Report_Settings;
        public System.Windows.Forms.CheckBox checkBox_reportFilterSegmentStatusChanged;
        public System.Windows.Forms.CheckBox checkBox_reportFilterSegmentsContainingComments;
        public System.Windows.Forms.CheckBox checkBox_reportFilterChangedTargetContent;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ImageList imageList_TreeView;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel_styleSheet;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_browseCustomXsltFilePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.CheckBox checkBox_useCustomStyleSheet;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        public System.Windows.Forms.TextBox textBox_Custom_xsltFilePath;
        private System.Windows.Forms.Button button_changeFormattingTagRemoved;
        private System.Windows.Forms.Button button_changeFormattingTagNew;
        private System.Windows.Forms.Button button_changeFormattingTextRemoved;
        private System.Windows.Forms.Button button_changeFormattingTextNew;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Tag_Removed;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Tag_New;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Text_Removed;
        private System.Windows.Forms.RichTextBox richTextBox_formatting_Text_New;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.CheckBox checkBox_includeIndividualFileInformationMergedFiles;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox checkBox_reportFilterFilesWithNoRecordsFiltered;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label8;
    }
}