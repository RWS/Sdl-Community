namespace Sdl.Community.InvoiceAndQuotes
{
    partial class StudioInQuote
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StudioInQuote));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.pageCreateQuote = new System.Windows.Forms.TabPage();
			this.btnGenerate = new System.Windows.Forms.Button();
			this.groupReportType = new System.Windows.Forms.GroupBox();
			this.grpTemplateType = new System.Windows.Forms.GroupBox();
			this.rbGroupedAnalysis = new System.Windows.Forms.RadioButton();
			this.rbStandardLines = new System.Windows.Forms.RadioButton();
			this.rbSimpleWordAnalysis = new System.Windows.Forms.RadioButton();
			this.ckClipboard = new System.Windows.Forms.RadioButton();
			this.ckExcel = new System.Windows.Forms.RadioButton();
			this.ckWord = new System.Windows.Forms.RadioButton();
			this.lblLanguagePair = new System.Windows.Forms.Label();
			this.lblSummaryReportType = new System.Windows.Forms.Label();
			this.cmbLanguagePair = new System.Windows.Forms.ComboBox();
			this.cmbReportType = new System.Windows.Forms.ComboBox();
			this.ddlExcel = new System.Windows.Forms.ComboBox();
			this.ddlWord = new System.Windows.Forms.ComboBox();
			this.groupCustomer = new System.Windows.Forms.GroupBox();
			this.btnCustomer = new System.Windows.Forms.Button();
			this.groupProjects = new System.Windows.Forms.GroupBox();
			this.btnBrowseProjectsXML = new System.Windows.Forms.Button();
			this.txtProjectsXML = new System.Windows.Forms.TextBox();
			this.lblProjectsXML = new System.Windows.Forms.Label();
			this.projectsList = new System.Windows.Forms.ListBox();
			this.pageTemplates = new System.Windows.Forms.TabPage();
			this.groupTemplateTemplates = new System.Windows.Forms.GroupBox();
			this.rbTemplateGroupedAnalysis = new System.Windows.Forms.RadioButton();
			this.rbTemplateStandardLines = new System.Windows.Forms.RadioButton();
			this.rbTemplateSimpleWordAnalysis = new System.Windows.Forms.RadioButton();
			this.groupStudioAnalysisBands = new System.Windows.Forms.GroupBox();
			this.pageUserDetails = new System.Windows.Forms.TabPage();
			this.groupUserDetails = new System.Windows.Forms.GroupBox();
			this.ddlLanguages = new System.Windows.Forms.ComboBox();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.tabControl1.SuspendLayout();
			this.pageCreateQuote.SuspendLayout();
			this.groupReportType.SuspendLayout();
			this.grpTemplateType.SuspendLayout();
			this.groupCustomer.SuspendLayout();
			this.groupProjects.SuspendLayout();
			this.pageTemplates.SuspendLayout();
			this.groupTemplateTemplates.SuspendLayout();
			this.pageUserDetails.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.pageCreateQuote);
			this.tabControl1.Controls.Add(this.pageTemplates);
			this.tabControl1.Controls.Add(this.pageUserDetails);
			this.tabControl1.Location = new System.Drawing.Point(18, 18);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(1126, 685);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// pageCreateQuote
			// 
			this.pageCreateQuote.Controls.Add(this.btnGenerate);
			this.pageCreateQuote.Controls.Add(this.groupReportType);
			this.pageCreateQuote.Controls.Add(this.groupCustomer);
			this.pageCreateQuote.Controls.Add(this.groupProjects);
			this.pageCreateQuote.Location = new System.Drawing.Point(4, 29);
			this.pageCreateQuote.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pageCreateQuote.Name = "pageCreateQuote";
			this.pageCreateQuote.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pageCreateQuote.Size = new System.Drawing.Size(1118, 652);
			this.pageCreateQuote.TabIndex = 0;
			this.pageCreateQuote.Text = "Create Quote";
			this.pageCreateQuote.UseVisualStyleBackColor = true;
			// 
			// btnGenerate
			// 
			this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnGenerate.Location = new System.Drawing.Point(628, 543);
			this.btnGenerate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnGenerate.Name = "btnGenerate";
			this.btnGenerate.Size = new System.Drawing.Size(471, 92);
			this.btnGenerate.TabIndex = 2;
			this.btnGenerate.Text = "Generate Quote";
			this.btnGenerate.UseVisualStyleBackColor = true;
			this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
			// 
			// groupReportType
			// 
			this.groupReportType.Controls.Add(this.grpTemplateType);
			this.groupReportType.Controls.Add(this.ckClipboard);
			this.groupReportType.Controls.Add(this.ckExcel);
			this.groupReportType.Controls.Add(this.ckWord);
			this.groupReportType.Controls.Add(this.lblLanguagePair);
			this.groupReportType.Controls.Add(this.lblSummaryReportType);
			this.groupReportType.Controls.Add(this.cmbLanguagePair);
			this.groupReportType.Controls.Add(this.cmbReportType);
			this.groupReportType.Controls.Add(this.ddlExcel);
			this.groupReportType.Controls.Add(this.ddlWord);
			this.groupReportType.Location = new System.Drawing.Point(10, 338);
			this.groupReportType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupReportType.Name = "groupReportType";
			this.groupReportType.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupReportType.Size = new System.Drawing.Size(609, 297);
			this.groupReportType.TabIndex = 1;
			this.groupReportType.TabStop = false;
			this.groupReportType.Text = "3. Report Type";
			// 
			// grpTemplateType
			// 
			this.grpTemplateType.Controls.Add(this.rbGroupedAnalysis);
			this.grpTemplateType.Controls.Add(this.rbStandardLines);
			this.grpTemplateType.Controls.Add(this.rbSimpleWordAnalysis);
			this.grpTemplateType.Location = new System.Drawing.Point(390, 18);
			this.grpTemplateType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.grpTemplateType.Name = "grpTemplateType";
			this.grpTemplateType.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.grpTemplateType.Size = new System.Drawing.Size(210, 145);
			this.grpTemplateType.TabIndex = 3;
			this.grpTemplateType.TabStop = false;
			this.grpTemplateType.Text = "Templates";
			// 
			// rbGroupedAnalysis
			// 
			this.rbGroupedAnalysis.AutoSize = true;
			this.rbGroupedAnalysis.Location = new System.Drawing.Point(12, 95);
			this.rbGroupedAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbGroupedAnalysis.Name = "rbGroupedAnalysis";
			this.rbGroupedAnalysis.Size = new System.Drawing.Size(159, 24);
			this.rbGroupedAnalysis.TabIndex = 17;
			this.rbGroupedAnalysis.Text = "Grouped Analysis";
			this.rbGroupedAnalysis.UseVisualStyleBackColor = true;
			this.rbGroupedAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rbStandardLines
			// 
			this.rbStandardLines.AutoSize = true;
			this.rbStandardLines.Location = new System.Drawing.Point(12, 60);
			this.rbStandardLines.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbStandardLines.Name = "rbStandardLines";
			this.rbStandardLines.Size = new System.Drawing.Size(142, 24);
			this.rbStandardLines.TabIndex = 16;
			this.rbStandardLines.Text = "Standard Lines";
			this.rbStandardLines.UseVisualStyleBackColor = true;
			this.rbStandardLines.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rbSimpleWordAnalysis
			// 
			this.rbSimpleWordAnalysis.AutoSize = true;
			this.rbSimpleWordAnalysis.Location = new System.Drawing.Point(12, 25);
			this.rbSimpleWordAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbSimpleWordAnalysis.Name = "rbSimpleWordAnalysis";
			this.rbSimpleWordAnalysis.Size = new System.Drawing.Size(186, 24);
			this.rbSimpleWordAnalysis.TabIndex = 15;
			this.rbSimpleWordAnalysis.TabStop = true;
			this.rbSimpleWordAnalysis.Text = "Simple Word Analysis";
			this.rbSimpleWordAnalysis.UseVisualStyleBackColor = true;
			this.rbSimpleWordAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// ckClipboard
			// 
			this.ckClipboard.AutoSize = true;
			this.ckClipboard.Location = new System.Drawing.Point(244, 106);
			this.ckClipboard.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ckClipboard.Name = "ckClipboard";
			this.ckClipboard.Size = new System.Drawing.Size(101, 24);
			this.ckClipboard.TabIndex = 11;
			this.ckClipboard.TabStop = true;
			this.ckClipboard.Text = "Clipboard";
			this.ckClipboard.UseVisualStyleBackColor = true;
			this.ckClipboard.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
			// 
			// ckExcel
			// 
			this.ckExcel.AutoSize = true;
			this.ckExcel.Location = new System.Drawing.Point(244, 68);
			this.ckExcel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ckExcel.Name = "ckExcel";
			this.ckExcel.Size = new System.Drawing.Size(100, 24);
			this.ckExcel.TabIndex = 10;
			this.ckExcel.TabStop = true;
			this.ckExcel.Text = "MS Excel";
			this.ckExcel.UseVisualStyleBackColor = true;
			this.ckExcel.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
			// 
			// ckWord
			// 
			this.ckWord.AutoSize = true;
			this.ckWord.Location = new System.Drawing.Point(244, 29);
			this.ckWord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ckWord.Name = "ckWord";
			this.ckWord.Size = new System.Drawing.Size(100, 24);
			this.ckWord.TabIndex = 9;
			this.ckWord.TabStop = true;
			this.ckWord.Text = "MS Word";
			this.ckWord.UseVisualStyleBackColor = true;
			this.ckWord.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
			// 
			// lblLanguagePair
			// 
			this.lblLanguagePair.AutoSize = true;
			this.lblLanguagePair.Location = new System.Drawing.Point(4, 225);
			this.lblLanguagePair.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblLanguagePair.Name = "lblLanguagePair";
			this.lblLanguagePair.Size = new System.Drawing.Size(203, 20);
			this.lblLanguagePair.TabIndex = 8;
			this.lblLanguagePair.Text = "Analyse file / Language pair";
			// 
			// lblSummaryReportType
			// 
			this.lblSummaryReportType.AutoSize = true;
			this.lblSummaryReportType.Location = new System.Drawing.Point(3, 163);
			this.lblSummaryReportType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblSummaryReportType.Name = "lblSummaryReportType";
			this.lblSummaryReportType.Size = new System.Drawing.Size(92, 20);
			this.lblSummaryReportType.TabIndex = 7;
			this.lblSummaryReportType.Text = "Report type";
			// 
			// cmbLanguagePair
			// 
			this.cmbLanguagePair.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbLanguagePair.FormattingEnabled = true;
			this.cmbLanguagePair.Location = new System.Drawing.Point(9, 249);
			this.cmbLanguagePair.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.cmbLanguagePair.Name = "cmbLanguagePair";
			this.cmbLanguagePair.Size = new System.Drawing.Size(368, 28);
			this.cmbLanguagePair.TabIndex = 6;
			// 
			// cmbReportType
			// 
			this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbReportType.FormattingEnabled = true;
			this.cmbReportType.Items.AddRange(new object[] {
            "Breakdown by file and  summary",
            "Summary"});
			this.cmbReportType.Location = new System.Drawing.Point(9, 188);
			this.cmbReportType.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.cmbReportType.Name = "cmbReportType";
			this.cmbReportType.Size = new System.Drawing.Size(368, 28);
			this.cmbReportType.TabIndex = 5;
			// 
			// ddlExcel
			// 
			this.ddlExcel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlExcel.FormattingEnabled = true;
			this.ddlExcel.Location = new System.Drawing.Point(9, 66);
			this.ddlExcel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ddlExcel.Name = "ddlExcel";
			this.ddlExcel.Size = new System.Drawing.Size(224, 28);
			this.ddlExcel.TabIndex = 4;
			this.ddlExcel.Visible = false;
			// 
			// ddlWord
			// 
			this.ddlWord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlWord.FormattingEnabled = true;
			this.ddlWord.Location = new System.Drawing.Point(9, 28);
			this.ddlWord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ddlWord.Name = "ddlWord";
			this.ddlWord.Size = new System.Drawing.Size(224, 28);
			this.ddlWord.TabIndex = 3;
			this.ddlWord.Visible = false;
			// 
			// groupCustomer
			// 
			this.groupCustomer.Controls.Add(this.btnCustomer);
			this.groupCustomer.Location = new System.Drawing.Point(628, 11);
			this.groupCustomer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupCustomer.Name = "groupCustomer";
			this.groupCustomer.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupCustomer.Size = new System.Drawing.Size(471, 469);
			this.groupCustomer.TabIndex = 1;
			this.groupCustomer.TabStop = false;
			this.groupCustomer.Text = "2. Select Customer";
			// 
			// btnCustomer
			// 
			this.btnCustomer.Location = new System.Drawing.Point(9, 29);
			this.btnCustomer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnCustomer.Name = "btnCustomer";
			this.btnCustomer.Size = new System.Drawing.Size(225, 35);
			this.btnCustomer.TabIndex = 1;
			this.btnCustomer.Text = "Customer";
			this.btnCustomer.UseVisualStyleBackColor = true;
			this.btnCustomer.Click += new System.EventHandler(this.btnCustomer_Click);
			// 
			// groupProjects
			// 
			this.groupProjects.Controls.Add(this.btnBrowseProjectsXML);
			this.groupProjects.Controls.Add(this.txtProjectsXML);
			this.groupProjects.Controls.Add(this.lblProjectsXML);
			this.groupProjects.Controls.Add(this.projectsList);
			this.groupProjects.Location = new System.Drawing.Point(10, 11);
			this.groupProjects.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupProjects.Name = "groupProjects";
			this.groupProjects.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupProjects.Size = new System.Drawing.Size(609, 322);
			this.groupProjects.TabIndex = 0;
			this.groupProjects.TabStop = false;
			this.groupProjects.Text = "1. Select Project";
			// 
			// btnBrowseProjectsXML
			// 
			this.btnBrowseProjectsXML.Location = new System.Drawing.Point(561, 29);
			this.btnBrowseProjectsXML.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.btnBrowseProjectsXML.Name = "btnBrowseProjectsXML";
			this.btnBrowseProjectsXML.Size = new System.Drawing.Size(39, 35);
			this.btnBrowseProjectsXML.TabIndex = 3;
			this.btnBrowseProjectsXML.Text = "...";
			this.btnBrowseProjectsXML.UseVisualStyleBackColor = true;
			this.btnBrowseProjectsXML.Click += new System.EventHandler(this.btnBrowseProjectsXML_Click);
			// 
			// txtProjectsXML
			// 
			this.txtProjectsXML.Location = new System.Drawing.Point(138, 32);
			this.txtProjectsXML.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.txtProjectsXML.Name = "txtProjectsXML";
			this.txtProjectsXML.ReadOnly = true;
			this.txtProjectsXML.Size = new System.Drawing.Size(412, 26);
			this.txtProjectsXML.TabIndex = 2;
			// 
			// lblProjectsXML
			// 
			this.lblProjectsXML.AutoSize = true;
			this.lblProjectsXML.Location = new System.Drawing.Point(9, 37);
			this.lblProjectsXML.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lblProjectsXML.Name = "lblProjectsXML";
			this.lblProjectsXML.Size = new System.Drawing.Size(124, 20);
			this.lblProjectsXML.TabIndex = 1;
			this.lblProjectsXML.Text = "Path to projects:";
			// 
			// projectsList
			// 
			this.projectsList.FormattingEnabled = true;
			this.projectsList.HorizontalScrollbar = true;
			this.projectsList.ItemHeight = 20;
			this.projectsList.Location = new System.Drawing.Point(9, 69);
			this.projectsList.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.projectsList.Name = "projectsList";
			this.projectsList.Size = new System.Drawing.Size(589, 244);
			this.projectsList.TabIndex = 0;
			// 
			// pageTemplates
			// 
			this.pageTemplates.Controls.Add(this.groupTemplateTemplates);
			this.pageTemplates.Controls.Add(this.groupStudioAnalysisBands);
			this.pageTemplates.Location = new System.Drawing.Point(4, 29);
			this.pageTemplates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pageTemplates.Name = "pageTemplates";
			this.pageTemplates.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pageTemplates.Size = new System.Drawing.Size(1118, 652);
			this.pageTemplates.TabIndex = 1;
			this.pageTemplates.Text = "Templates";
			this.pageTemplates.UseVisualStyleBackColor = true;
			// 
			// groupTemplateTemplates
			// 
			this.groupTemplateTemplates.Controls.Add(this.rbTemplateGroupedAnalysis);
			this.groupTemplateTemplates.Controls.Add(this.rbTemplateStandardLines);
			this.groupTemplateTemplates.Controls.Add(this.rbTemplateSimpleWordAnalysis);
			this.groupTemplateTemplates.Location = new System.Drawing.Point(15, 9);
			this.groupTemplateTemplates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupTemplateTemplates.Name = "groupTemplateTemplates";
			this.groupTemplateTemplates.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupTemplateTemplates.Size = new System.Drawing.Size(210, 145);
			this.groupTemplateTemplates.TabIndex = 4;
			this.groupTemplateTemplates.TabStop = false;
			this.groupTemplateTemplates.Text = "Templates";
			// 
			// rbTemplateGroupedAnalysis
			// 
			this.rbTemplateGroupedAnalysis.AutoSize = true;
			this.rbTemplateGroupedAnalysis.Location = new System.Drawing.Point(12, 95);
			this.rbTemplateGroupedAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbTemplateGroupedAnalysis.Name = "rbTemplateGroupedAnalysis";
			this.rbTemplateGroupedAnalysis.Size = new System.Drawing.Size(159, 24);
			this.rbTemplateGroupedAnalysis.TabIndex = 17;
			this.rbTemplateGroupedAnalysis.Text = "Grouped Analysis";
			this.rbTemplateGroupedAnalysis.UseVisualStyleBackColor = true;
			this.rbTemplateGroupedAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rbTemplateStandardLines
			// 
			this.rbTemplateStandardLines.AutoSize = true;
			this.rbTemplateStandardLines.Location = new System.Drawing.Point(12, 60);
			this.rbTemplateStandardLines.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbTemplateStandardLines.Name = "rbTemplateStandardLines";
			this.rbTemplateStandardLines.Size = new System.Drawing.Size(142, 24);
			this.rbTemplateStandardLines.TabIndex = 16;
			this.rbTemplateStandardLines.Text = "Standard Lines";
			this.rbTemplateStandardLines.UseVisualStyleBackColor = true;
			this.rbTemplateStandardLines.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// rbTemplateSimpleWordAnalysis
			// 
			this.rbTemplateSimpleWordAnalysis.AutoSize = true;
			this.rbTemplateSimpleWordAnalysis.Location = new System.Drawing.Point(12, 25);
			this.rbTemplateSimpleWordAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.rbTemplateSimpleWordAnalysis.Name = "rbTemplateSimpleWordAnalysis";
			this.rbTemplateSimpleWordAnalysis.Size = new System.Drawing.Size(186, 24);
			this.rbTemplateSimpleWordAnalysis.TabIndex = 15;
			this.rbTemplateSimpleWordAnalysis.TabStop = true;
			this.rbTemplateSimpleWordAnalysis.Text = "Simple Word Analysis";
			this.rbTemplateSimpleWordAnalysis.UseVisualStyleBackColor = true;
			this.rbTemplateSimpleWordAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
			// 
			// groupStudioAnalysisBands
			// 
			this.groupStudioAnalysisBands.Location = new System.Drawing.Point(234, 9);
			this.groupStudioAnalysisBands.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupStudioAnalysisBands.Name = "groupStudioAnalysisBands";
			this.groupStudioAnalysisBands.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupStudioAnalysisBands.Size = new System.Drawing.Size(852, 626);
			this.groupStudioAnalysisBands.TabIndex = 1;
			this.groupStudioAnalysisBands.TabStop = false;
			this.groupStudioAnalysisBands.Text = "Studio Analysis Bands";
			// 
			// pageUserDetails
			// 
			this.pageUserDetails.Controls.Add(this.groupUserDetails);
			this.pageUserDetails.Location = new System.Drawing.Point(4, 29);
			this.pageUserDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.pageUserDetails.Name = "pageUserDetails";
			this.pageUserDetails.Size = new System.Drawing.Size(1118, 652);
			this.pageUserDetails.TabIndex = 2;
			this.pageUserDetails.Text = "User Details";
			this.pageUserDetails.UseVisualStyleBackColor = true;
			// 
			// groupUserDetails
			// 
			this.groupUserDetails.Location = new System.Drawing.Point(56, 71);
			this.groupUserDetails.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupUserDetails.Name = "groupUserDetails";
			this.groupUserDetails.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.groupUserDetails.Size = new System.Drawing.Size(1005, 451);
			this.groupUserDetails.TabIndex = 0;
			this.groupUserDetails.TabStop = false;
			this.groupUserDetails.Text = "User Details";
			// 
			// ddlLanguages
			// 
			this.ddlLanguages.FormattingEnabled = true;
			this.ddlLanguages.Location = new System.Drawing.Point(1002, 11);
			this.ddlLanguages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ddlLanguages.Name = "ddlLanguages";
			this.ddlLanguages.Size = new System.Drawing.Size(120, 28);
			this.ddlLanguages.TabIndex = 3;
			this.ddlLanguages.SelectedIndexChanged += new System.EventHandler(this.ddlLanguages_SelectedIndexChanged);
			// 
			// StudioInQuote
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1155, 715);
			this.Controls.Add(this.ddlLanguages);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StudioInQuote";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = " Trados Studio InQuote";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.StudioInQuote_FormClosed);
			this.Load += new System.EventHandler(this.StudioInQuote_Load);
			this.tabControl1.ResumeLayout(false);
			this.pageCreateQuote.ResumeLayout(false);
			this.groupReportType.ResumeLayout(false);
			this.groupReportType.PerformLayout();
			this.grpTemplateType.ResumeLayout(false);
			this.grpTemplateType.PerformLayout();
			this.groupCustomer.ResumeLayout(false);
			this.groupProjects.ResumeLayout(false);
			this.groupProjects.PerformLayout();
			this.pageTemplates.ResumeLayout(false);
			this.groupTemplateTemplates.ResumeLayout(false);
			this.groupTemplateTemplates.PerformLayout();
			this.pageUserDetails.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageCreateQuote;
        private System.Windows.Forms.TabPage pageTemplates;
        private System.Windows.Forms.TabPage pageUserDetails;
        private System.Windows.Forms.GroupBox groupCustomer;
        private System.Windows.Forms.GroupBox groupProjects;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.GroupBox groupReportType;
        private System.Windows.Forms.ListBox projectsList;
        private System.Windows.Forms.Button btnCustomer;
        private System.Windows.Forms.GroupBox groupStudioAnalysisBands;
        private System.Windows.Forms.GroupBox groupUserDetails;
        private System.Windows.Forms.ComboBox ddlExcel;
        private System.Windows.Forms.ComboBox ddlWord;
        private System.Windows.Forms.ComboBox ddlLanguages;
        private System.Windows.Forms.ComboBox cmbLanguagePair;
        private System.Windows.Forms.ComboBox cmbReportType;
        private System.Windows.Forms.Label lblLanguagePair;
        private System.Windows.Forms.Label lblSummaryReportType;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.RadioButton ckClipboard;
        private System.Windows.Forms.RadioButton ckExcel;
        private System.Windows.Forms.RadioButton ckWord;
        private System.Windows.Forms.GroupBox grpTemplateType;
        private System.Windows.Forms.RadioButton rbGroupedAnalysis;
        private System.Windows.Forms.RadioButton rbStandardLines;
        private System.Windows.Forms.RadioButton rbSimpleWordAnalysis;
        private System.Windows.Forms.GroupBox groupTemplateTemplates;
        private System.Windows.Forms.RadioButton rbTemplateGroupedAnalysis;
        private System.Windows.Forms.RadioButton rbTemplateStandardLines;
        private System.Windows.Forms.RadioButton rbTemplateSimpleWordAnalysis;
        private System.Windows.Forms.Button btnBrowseProjectsXML;
        private System.Windows.Forms.TextBox txtProjectsXML;
        private System.Windows.Forms.Label lblProjectsXML;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

