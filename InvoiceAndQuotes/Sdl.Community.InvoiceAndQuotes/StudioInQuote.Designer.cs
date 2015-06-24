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
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(751, 445);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // pageCreateQuote
            // 
            this.pageCreateQuote.Controls.Add(this.btnGenerate);
            this.pageCreateQuote.Controls.Add(this.groupReportType);
            this.pageCreateQuote.Controls.Add(this.groupCustomer);
            this.pageCreateQuote.Controls.Add(this.groupProjects);
            this.pageCreateQuote.Location = new System.Drawing.Point(4, 22);
            this.pageCreateQuote.Name = "pageCreateQuote";
            this.pageCreateQuote.Padding = new System.Windows.Forms.Padding(3);
            this.pageCreateQuote.Size = new System.Drawing.Size(743, 419);
            this.pageCreateQuote.TabIndex = 0;
            this.pageCreateQuote.Text = "Create Quote";
            this.pageCreateQuote.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.Location = new System.Drawing.Point(419, 353);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(314, 60);
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
            this.groupReportType.Location = new System.Drawing.Point(7, 220);
            this.groupReportType.Name = "groupReportType";
            this.groupReportType.Size = new System.Drawing.Size(406, 193);
            this.groupReportType.TabIndex = 1;
            this.groupReportType.TabStop = false;
            this.groupReportType.Text = "3. Report Type";
            // 
            // grpTemplateType
            // 
            this.grpTemplateType.Controls.Add(this.rbGroupedAnalysis);
            this.grpTemplateType.Controls.Add(this.rbStandardLines);
            this.grpTemplateType.Controls.Add(this.rbSimpleWordAnalysis);
            this.grpTemplateType.Location = new System.Drawing.Point(260, 12);
            this.grpTemplateType.Name = "grpTemplateType";
            this.grpTemplateType.Size = new System.Drawing.Size(140, 94);
            this.grpTemplateType.TabIndex = 3;
            this.grpTemplateType.TabStop = false;
            this.grpTemplateType.Text = "Templates";
            // 
            // rbGroupedAnalysis
            // 
            this.rbGroupedAnalysis.AutoSize = true;
            this.rbGroupedAnalysis.Location = new System.Drawing.Point(8, 62);
            this.rbGroupedAnalysis.Name = "rbGroupedAnalysis";
            this.rbGroupedAnalysis.Size = new System.Drawing.Size(107, 17);
            this.rbGroupedAnalysis.TabIndex = 17;
            this.rbGroupedAnalysis.Text = "Grouped Analysis";
            this.rbGroupedAnalysis.UseVisualStyleBackColor = true;
            this.rbGroupedAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbStandardLines
            // 
            this.rbStandardLines.AutoSize = true;
            this.rbStandardLines.Location = new System.Drawing.Point(8, 39);
            this.rbStandardLines.Name = "rbStandardLines";
            this.rbStandardLines.Size = new System.Drawing.Size(96, 17);
            this.rbStandardLines.TabIndex = 16;
            this.rbStandardLines.Text = "Standard Lines";
            this.rbStandardLines.UseVisualStyleBackColor = true;
            this.rbStandardLines.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbSimpleWordAnalysis
            // 
            this.rbSimpleWordAnalysis.AutoSize = true;
            this.rbSimpleWordAnalysis.Location = new System.Drawing.Point(8, 16);
            this.rbSimpleWordAnalysis.Name = "rbSimpleWordAnalysis";
            this.rbSimpleWordAnalysis.Size = new System.Drawing.Size(126, 17);
            this.rbSimpleWordAnalysis.TabIndex = 15;
            this.rbSimpleWordAnalysis.TabStop = true;
            this.rbSimpleWordAnalysis.Text = "Simple Word Analysis";
            this.rbSimpleWordAnalysis.UseVisualStyleBackColor = true;
            this.rbSimpleWordAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // ckClipboard
            // 
            this.ckClipboard.AutoSize = true;
            this.ckClipboard.Location = new System.Drawing.Point(163, 69);
            this.ckClipboard.Name = "ckClipboard";
            this.ckClipboard.Size = new System.Drawing.Size(69, 17);
            this.ckClipboard.TabIndex = 11;
            this.ckClipboard.TabStop = true;
            this.ckClipboard.Text = "Clipboard";
            this.ckClipboard.UseVisualStyleBackColor = true;
            this.ckClipboard.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
            // 
            // ckExcel
            // 
            this.ckExcel.AutoSize = true;
            this.ckExcel.Location = new System.Drawing.Point(163, 44);
            this.ckExcel.Name = "ckExcel";
            this.ckExcel.Size = new System.Drawing.Size(70, 17);
            this.ckExcel.TabIndex = 10;
            this.ckExcel.TabStop = true;
            this.ckExcel.Text = "MS Excel";
            this.ckExcel.UseVisualStyleBackColor = true;
            this.ckExcel.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
            // 
            // ckWord
            // 
            this.ckWord.AutoSize = true;
            this.ckWord.Location = new System.Drawing.Point(163, 19);
            this.ckWord.Name = "ckWord";
            this.ckWord.Size = new System.Drawing.Size(70, 17);
            this.ckWord.TabIndex = 9;
            this.ckWord.TabStop = true;
            this.ckWord.Text = "MS Word";
            this.ckWord.UseVisualStyleBackColor = true;
            this.ckWord.CheckedChanged += new System.EventHandler(this.ck_CheckedChanged);
            // 
            // lblLanguagePair
            // 
            this.lblLanguagePair.AutoSize = true;
            this.lblLanguagePair.Location = new System.Drawing.Point(3, 146);
            this.lblLanguagePair.Name = "lblLanguagePair";
            this.lblLanguagePair.Size = new System.Drawing.Size(139, 13);
            this.lblLanguagePair.TabIndex = 8;
            this.lblLanguagePair.Text = "Analyse file / Language pair";
            // 
            // lblSummaryReportType
            // 
            this.lblSummaryReportType.AutoSize = true;
            this.lblSummaryReportType.Location = new System.Drawing.Point(2, 106);
            this.lblSummaryReportType.Name = "lblSummaryReportType";
            this.lblSummaryReportType.Size = new System.Drawing.Size(62, 13);
            this.lblSummaryReportType.TabIndex = 7;
            this.lblSummaryReportType.Text = "Report type";
            // 
            // cmbLanguagePair
            // 
            this.cmbLanguagePair.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguagePair.FormattingEnabled = true;
            this.cmbLanguagePair.Location = new System.Drawing.Point(6, 162);
            this.cmbLanguagePair.Name = "cmbLanguagePair";
            this.cmbLanguagePair.Size = new System.Drawing.Size(247, 21);
            this.cmbLanguagePair.TabIndex = 6;
            // 
            // cmbReportType
            // 
            this.cmbReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbReportType.FormattingEnabled = true;
            this.cmbReportType.Items.AddRange(new object[] {
            "Breakdown by file and  summary",
            "Summary"});
            this.cmbReportType.Location = new System.Drawing.Point(6, 122);
            this.cmbReportType.Name = "cmbReportType";
            this.cmbReportType.Size = new System.Drawing.Size(247, 21);
            this.cmbReportType.TabIndex = 5;
            // 
            // ddlExcel
            // 
            this.ddlExcel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlExcel.FormattingEnabled = true;
            this.ddlExcel.Location = new System.Drawing.Point(6, 43);
            this.ddlExcel.Name = "ddlExcel";
            this.ddlExcel.Size = new System.Drawing.Size(151, 21);
            this.ddlExcel.TabIndex = 4;
            this.ddlExcel.Visible = false;
            // 
            // ddlWord
            // 
            this.ddlWord.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlWord.FormattingEnabled = true;
            this.ddlWord.Location = new System.Drawing.Point(6, 18);
            this.ddlWord.Name = "ddlWord";
            this.ddlWord.Size = new System.Drawing.Size(151, 21);
            this.ddlWord.TabIndex = 3;
            this.ddlWord.Visible = false;
            // 
            // groupCustomer
            // 
            this.groupCustomer.Controls.Add(this.btnCustomer);
            this.groupCustomer.Location = new System.Drawing.Point(419, 7);
            this.groupCustomer.Name = "groupCustomer";
            this.groupCustomer.Size = new System.Drawing.Size(314, 305);
            this.groupCustomer.TabIndex = 1;
            this.groupCustomer.TabStop = false;
            this.groupCustomer.Text = "2. Select Customer";
            // 
            // btnCustomer
            // 
            this.btnCustomer.Location = new System.Drawing.Point(6, 19);
            this.btnCustomer.Name = "btnCustomer";
            this.btnCustomer.Size = new System.Drawing.Size(150, 23);
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
            this.groupProjects.Location = new System.Drawing.Point(7, 7);
            this.groupProjects.Name = "groupProjects";
            this.groupProjects.Size = new System.Drawing.Size(406, 209);
            this.groupProjects.TabIndex = 0;
            this.groupProjects.TabStop = false;
            this.groupProjects.Text = "1. Select Project";
            // 
            // btnBrowseProjectsXML
            // 
            this.btnBrowseProjectsXML.Location = new System.Drawing.Point(374, 19);
            this.btnBrowseProjectsXML.Name = "btnBrowseProjectsXML";
            this.btnBrowseProjectsXML.Size = new System.Drawing.Size(26, 23);
            this.btnBrowseProjectsXML.TabIndex = 3;
            this.btnBrowseProjectsXML.Text = "...";
            this.btnBrowseProjectsXML.UseVisualStyleBackColor = true;
            this.btnBrowseProjectsXML.Click += new System.EventHandler(this.btnBrowseProjectsXML_Click);
            // 
            // txtProjectsXML
            // 
            this.txtProjectsXML.Location = new System.Drawing.Point(92, 21);
            this.txtProjectsXML.Name = "txtProjectsXML";
            this.txtProjectsXML.ReadOnly = true;
            this.txtProjectsXML.Size = new System.Drawing.Size(276, 20);
            this.txtProjectsXML.TabIndex = 2;
            // 
            // lblProjectsXML
            // 
            this.lblProjectsXML.AutoSize = true;
            this.lblProjectsXML.Location = new System.Drawing.Point(6, 24);
            this.lblProjectsXML.Name = "lblProjectsXML";
            this.lblProjectsXML.Size = new System.Drawing.Size(84, 13);
            this.lblProjectsXML.TabIndex = 1;
            this.lblProjectsXML.Text = "Path to projects:";
            // 
            // projectsList
            // 
            this.projectsList.FormattingEnabled = true;
            this.projectsList.HorizontalScrollbar = true;
            this.projectsList.Location = new System.Drawing.Point(6, 45);
            this.projectsList.Name = "projectsList";
            this.projectsList.Size = new System.Drawing.Size(394, 160);
            this.projectsList.TabIndex = 0;
            // 
            // pageTemplates
            // 
            this.pageTemplates.Controls.Add(this.groupTemplateTemplates);
            this.pageTemplates.Controls.Add(this.groupStudioAnalysisBands);
            this.pageTemplates.Location = new System.Drawing.Point(4, 22);
            this.pageTemplates.Name = "pageTemplates";
            this.pageTemplates.Padding = new System.Windows.Forms.Padding(3);
            this.pageTemplates.Size = new System.Drawing.Size(743, 419);
            this.pageTemplates.TabIndex = 1;
            this.pageTemplates.Text = "Templates";
            this.pageTemplates.UseVisualStyleBackColor = true;
            // 
            // groupTemplateTemplates
            // 
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateGroupedAnalysis);
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateStandardLines);
            this.groupTemplateTemplates.Controls.Add(this.rbTemplateSimpleWordAnalysis);
            this.groupTemplateTemplates.Location = new System.Drawing.Point(10, 6);
            this.groupTemplateTemplates.Name = "groupTemplateTemplates";
            this.groupTemplateTemplates.Size = new System.Drawing.Size(140, 94);
            this.groupTemplateTemplates.TabIndex = 4;
            this.groupTemplateTemplates.TabStop = false;
            this.groupTemplateTemplates.Text = "Templates";
            // 
            // rbTemplateGroupedAnalysis
            // 
            this.rbTemplateGroupedAnalysis.AutoSize = true;
            this.rbTemplateGroupedAnalysis.Location = new System.Drawing.Point(8, 62);
            this.rbTemplateGroupedAnalysis.Name = "rbTemplateGroupedAnalysis";
            this.rbTemplateGroupedAnalysis.Size = new System.Drawing.Size(107, 17);
            this.rbTemplateGroupedAnalysis.TabIndex = 17;
            this.rbTemplateGroupedAnalysis.Text = "Grouped Analysis";
            this.rbTemplateGroupedAnalysis.UseVisualStyleBackColor = true;
            this.rbTemplateGroupedAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbTemplateStandardLines
            // 
            this.rbTemplateStandardLines.AutoSize = true;
            this.rbTemplateStandardLines.Location = new System.Drawing.Point(8, 39);
            this.rbTemplateStandardLines.Name = "rbTemplateStandardLines";
            this.rbTemplateStandardLines.Size = new System.Drawing.Size(96, 17);
            this.rbTemplateStandardLines.TabIndex = 16;
            this.rbTemplateStandardLines.Text = "Standard Lines";
            this.rbTemplateStandardLines.UseVisualStyleBackColor = true;
            this.rbTemplateStandardLines.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbTemplateSimpleWordAnalysis
            // 
            this.rbTemplateSimpleWordAnalysis.AutoSize = true;
            this.rbTemplateSimpleWordAnalysis.Location = new System.Drawing.Point(8, 16);
            this.rbTemplateSimpleWordAnalysis.Name = "rbTemplateSimpleWordAnalysis";
            this.rbTemplateSimpleWordAnalysis.Size = new System.Drawing.Size(126, 17);
            this.rbTemplateSimpleWordAnalysis.TabIndex = 15;
            this.rbTemplateSimpleWordAnalysis.TabStop = true;
            this.rbTemplateSimpleWordAnalysis.Text = "Simple Word Analysis";
            this.rbTemplateSimpleWordAnalysis.UseVisualStyleBackColor = true;
            this.rbTemplateSimpleWordAnalysis.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // groupStudioAnalysisBands
            // 
            this.groupStudioAnalysisBands.Location = new System.Drawing.Point(156, 6);
            this.groupStudioAnalysisBands.Name = "groupStudioAnalysisBands";
            this.groupStudioAnalysisBands.Size = new System.Drawing.Size(568, 407);
            this.groupStudioAnalysisBands.TabIndex = 1;
            this.groupStudioAnalysisBands.TabStop = false;
            this.groupStudioAnalysisBands.Text = "Studio Analysis Bands";
            // 
            // pageUserDetails
            // 
            this.pageUserDetails.Controls.Add(this.groupUserDetails);
            this.pageUserDetails.Location = new System.Drawing.Point(4, 22);
            this.pageUserDetails.Name = "pageUserDetails";
            this.pageUserDetails.Size = new System.Drawing.Size(743, 419);
            this.pageUserDetails.TabIndex = 2;
            this.pageUserDetails.Text = "User Details";
            this.pageUserDetails.UseVisualStyleBackColor = true;
            // 
            // groupUserDetails
            // 
            this.groupUserDetails.Location = new System.Drawing.Point(37, 46);
            this.groupUserDetails.Name = "groupUserDetails";
            this.groupUserDetails.Size = new System.Drawing.Size(670, 293);
            this.groupUserDetails.TabIndex = 0;
            this.groupUserDetails.TabStop = false;
            this.groupUserDetails.Text = "User Details";
            // 
            // ddlLanguages
            // 
            this.ddlLanguages.FormattingEnabled = true;
            this.ddlLanguages.Location = new System.Drawing.Point(668, 7);
            this.ddlLanguages.Name = "ddlLanguages";
            this.ddlLanguages.Size = new System.Drawing.Size(81, 21);
            this.ddlLanguages.TabIndex = 3;
            this.ddlLanguages.SelectedIndexChanged += new System.EventHandler(this.ddlLanguages_SelectedIndexChanged);
            // 
            // StudioInQuote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 465);
            this.Controls.Add(this.ddlLanguages);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "StudioInQuote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDL Studio InQuote";
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

