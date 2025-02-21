namespace Sdl.Community.Legit
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.BrowseForFolderButton = new System.Windows.Forms.Button();
			this.HelpTooltip = new System.Windows.Forms.ToolTip(this.components);
			this.ShowSettingsMangerButton = new System.Windows.Forms.Button();
			this.ConvertButton = new System.Windows.Forms.Button();
			this.FilterManagerButton = new System.Windows.Forms.Button();
			this.SourceLangCombo = new System.Windows.Forms.ComboBox();
			this.ClearFilesListboxLink = new System.Windows.Forms.LinkLabel();
			this.ClearLogLink = new System.Windows.Forms.LinkLabel();
			this.SaveLogToTextFile = new System.Windows.Forms.LinkLabel();
			this.btnTM = new System.Windows.Forms.Button();
			this.TargetLangCombo = new System.Windows.Forms.ComboBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.BackgroundConverter = new System.ComponentModel.BackgroundWorker();
			this.ConversionProgressBar = new System.Windows.Forms.ProgressBar();
			this.BrowseForFilesDialog = new System.Windows.Forms.OpenFileDialog();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.txtTargetLanguage = new System.Windows.Forms.TextBox();
			this.txtSourceLanguage = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SourceFilesList = new System.Windows.Forms.TextBox();
			this.SourceLangLabel = new System.Windows.Forms.Label();
			this.LogTextbox = new System.Windows.Forms.TextBox();
			this.SaveToFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.OpenResultingFolderLink = new System.Windows.Forms.LinkLabel();
			this.ckTTX = new System.Windows.Forms.CheckBox();
			this.ckBilingualDoc = new System.Windows.Forms.CheckBox();
			this.lableTM = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// BrowseForFolderButton
			// 
			resources.ApplyResources(this.BrowseForFolderButton, "BrowseForFolderButton");
			this.BrowseForFolderButton.Name = "BrowseForFolderButton";
			this.BrowseForFolderButton.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.BrowseForFolderButton, resources.GetString("BrowseForFolderButton.ToolTip"));
			this.BrowseForFolderButton.UseVisualStyleBackColor = true;
			this.BrowseForFolderButton.Click += new System.EventHandler(this.BrowseForFolderButton_Click);
			// 
			// HelpTooltip
			// 
			this.HelpTooltip.IsBalloon = true;
			this.HelpTooltip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			// 
			// ShowSettingsMangerButton
			// 
			resources.ApplyResources(this.ShowSettingsMangerButton, "ShowSettingsMangerButton");
			this.ShowSettingsMangerButton.Name = "ShowSettingsMangerButton";
			this.ShowSettingsMangerButton.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.ShowSettingsMangerButton, resources.GetString("ShowSettingsMangerButton.ToolTip"));
			this.ShowSettingsMangerButton.UseVisualStyleBackColor = true;
			this.ShowSettingsMangerButton.Click += new System.EventHandler(this.ShowSettingsMangerButton_Click);
			// 
			// ConvertButton
			// 
			resources.ApplyResources(this.ConvertButton, "ConvertButton");
			this.ConvertButton.Name = "ConvertButton";
			this.ConvertButton.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.ConvertButton, resources.GetString("ConvertButton.ToolTip"));
			this.ConvertButton.UseVisualStyleBackColor = true;
			this.ConvertButton.Click += new System.EventHandler(this.ConvertButton_Click);
			// 
			// FilterManagerButton
			// 
			resources.ApplyResources(this.FilterManagerButton, "FilterManagerButton");
			this.FilterManagerButton.Name = "FilterManagerButton";
			this.FilterManagerButton.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.FilterManagerButton, resources.GetString("FilterManagerButton.ToolTip"));
			this.FilterManagerButton.UseVisualStyleBackColor = true;
			this.FilterManagerButton.Click += new System.EventHandler(this.FilterManagerButton_Click);
			// 
			// SourceLangCombo
			// 
			resources.ApplyResources(this.SourceLangCombo, "SourceLangCombo");
			this.SourceLangCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.SourceLangCombo.FormattingEnabled = true;
			this.SourceLangCombo.Name = "SourceLangCombo";
			this.SourceLangCombo.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.SourceLangCombo, resources.GetString("SourceLangCombo.ToolTip"));
			// 
			// ClearFilesListboxLink
			// 
			resources.ApplyResources(this.ClearFilesListboxLink, "ClearFilesListboxLink");
			this.ClearFilesListboxLink.Name = "ClearFilesListboxLink";
			this.ClearFilesListboxLink.TabStop = true;
			this.ClearFilesListboxLink.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.ClearFilesListboxLink, resources.GetString("ClearFilesListboxLink.ToolTip"));
			this.ClearFilesListboxLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ClearFilesListboxLink_LinkClicked);
			// 
			// ClearLogLink
			// 
			resources.ApplyResources(this.ClearLogLink, "ClearLogLink");
			this.ClearLogLink.Name = "ClearLogLink";
			this.ClearLogLink.TabStop = true;
			this.ClearLogLink.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.ClearLogLink, resources.GetString("ClearLogLink.ToolTip"));
			this.ClearLogLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ClearLogLink_LinkClicked);
			// 
			// SaveLogToTextFile
			// 
			resources.ApplyResources(this.SaveLogToTextFile, "SaveLogToTextFile");
			this.SaveLogToTextFile.Name = "SaveLogToTextFile";
			this.SaveLogToTextFile.TabStop = true;
			this.SaveLogToTextFile.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.SaveLogToTextFile, resources.GetString("SaveLogToTextFile.ToolTip"));
			this.SaveLogToTextFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SaveLogToTextFile_LinkClicked);
			// 
			// btnTM
			// 
			resources.ApplyResources(this.btnTM, "btnTM");
			this.btnTM.Name = "btnTM";
			this.btnTM.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.btnTM, resources.GetString("btnTM.ToolTip"));
			this.btnTM.UseVisualStyleBackColor = true;
			this.btnTM.Click += new System.EventHandler(this.btnTM_Click);
			// 
			// TargetLangCombo
			// 
			resources.ApplyResources(this.TargetLangCombo, "TargetLangCombo");
			this.TargetLangCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TargetLangCombo.FormattingEnabled = true;
			this.TargetLangCombo.Name = "TargetLangCombo";
			this.TargetLangCombo.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.TargetLangCombo, resources.GetString("TargetLangCombo.ToolTip"));
			// 
			// linkLabel1
			// 
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Tag = "toggle";
			this.HelpTooltip.SetToolTip(this.linkLabel1, resources.GetString("linkLabel1.ToolTip"));
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// ConversionProgressBar
			// 
			resources.ApplyResources(this.ConversionProgressBar, "ConversionProgressBar");
			this.ConversionProgressBar.Name = "ConversionProgressBar";
			// 
			// BrowseForFilesDialog
			// 
			this.BrowseForFilesDialog.Multiselect = true;
			resources.ApplyResources(this.BrowseForFilesDialog, "BrowseForFilesDialog");
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.txtTargetLanguage);
			this.splitContainer1.Panel1.Controls.Add(this.ConvertButton);
			this.splitContainer1.Panel1.Controls.Add(this.txtSourceLanguage);
			this.splitContainer1.Panel1.Controls.Add(this.TargetLangCombo);
			this.splitContainer1.Panel1.Controls.Add(this.label1);
			this.splitContainer1.Panel1.Controls.Add(this.SourceFilesList);
			this.splitContainer1.Panel1.Controls.Add(this.BrowseForFolderButton);
			this.splitContainer1.Panel1.Controls.Add(this.FilterManagerButton);
			this.splitContainer1.Panel1.Controls.Add(this.ShowSettingsMangerButton);
			this.splitContainer1.Panel1.Controls.Add(this.SourceLangLabel);
			this.splitContainer1.Panel1.Controls.Add(this.SourceLangCombo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.LogTextbox);
			// 
			// txtTargetLanguage
			// 
			resources.ApplyResources(this.txtTargetLanguage, "txtTargetLanguage");
			this.txtTargetLanguage.Name = "txtTargetLanguage";
			this.txtTargetLanguage.ReadOnly = true;
			// 
			// txtSourceLanguage
			// 
			resources.ApplyResources(this.txtSourceLanguage, "txtSourceLanguage");
			this.txtSourceLanguage.Name = "txtSourceLanguage";
			this.txtSourceLanguage.ReadOnly = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// SourceFilesList
			// 
			resources.ApplyResources(this.SourceFilesList, "SourceFilesList");
			this.SourceFilesList.Name = "SourceFilesList";
			this.SourceFilesList.ReadOnly = true;
			// 
			// SourceLangLabel
			// 
			resources.ApplyResources(this.SourceLangLabel, "SourceLangLabel");
			this.SourceLangLabel.Name = "SourceLangLabel";
			// 
			// LogTextbox
			// 
			resources.ApplyResources(this.LogTextbox, "LogTextbox");
			this.LogTextbox.Name = "LogTextbox";
			this.LogTextbox.ReadOnly = true;
			// 
			// OpenResultingFolderLink
			// 
			resources.ApplyResources(this.OpenResultingFolderLink, "OpenResultingFolderLink");
			this.OpenResultingFolderLink.Name = "OpenResultingFolderLink";
			this.OpenResultingFolderLink.TabStop = true;
			this.OpenResultingFolderLink.Tag = "toggle";
			this.OpenResultingFolderLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenResultingFolderLink_LinkClicked);
			// 
			// ckTTX
			// 
			resources.ApplyResources(this.ckTTX, "ckTTX");
			this.ckTTX.Name = "ckTTX";
			this.ckTTX.UseVisualStyleBackColor = true;
			// 
			// ckBilingualDoc
			// 
			resources.ApplyResources(this.ckBilingualDoc, "ckBilingualDoc");
			this.ckBilingualDoc.Name = "ckBilingualDoc";
			this.ckBilingualDoc.UseVisualStyleBackColor = true;
			// 
			// lableTM
			// 
			resources.ApplyResources(this.lableTM, "lableTM");
			this.lableTM.Name = "lableTM";
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.lableTM);
			this.Controls.Add(this.btnTM);
			this.Controls.Add(this.ckBilingualDoc);
			this.Controls.Add(this.ckTTX);
			this.Controls.Add(this.OpenResultingFolderLink);
			this.Controls.Add(this.SaveLogToTextFile);
			this.Controls.Add(this.ClearLogLink);
			this.Controls.Add(this.ClearFilesListboxLink);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.ConversionProgressBar);
			this.Name = "MainForm";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BrowseForFolderButton;
        private System.Windows.Forms.ToolTip HelpTooltip;
        private System.Windows.Forms.Button ShowSettingsMangerButton;
        private System.Windows.Forms.Button ConvertButton;
        private System.Windows.Forms.Button FilterManagerButton;
        private System.ComponentModel.BackgroundWorker BackgroundConverter;
        private System.Windows.Forms.ProgressBar ConversionProgressBar;
        private System.Windows.Forms.OpenFileDialog BrowseForFilesDialog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox LogTextbox;
        private System.Windows.Forms.TextBox SourceFilesList;
        private System.Windows.Forms.ComboBox SourceLangCombo;
        private System.Windows.Forms.LinkLabel ClearFilesListboxLink;
        private System.Windows.Forms.LinkLabel ClearLogLink;
        private System.Windows.Forms.LinkLabel SaveLogToTextFile;
        private System.Windows.Forms.SaveFileDialog SaveToFileDialog;
        private System.Windows.Forms.Label SourceLangLabel;
        private System.Windows.Forms.LinkLabel OpenResultingFolderLink;
        private System.Windows.Forms.CheckBox ckTTX;
        private System.Windows.Forms.CheckBox ckBilingualDoc;
        private System.Windows.Forms.Button btnTM;
        private System.Windows.Forms.Label lableTM;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox TargetLangCombo;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TextBox txtTargetLanguage;
        private System.Windows.Forms.TextBox txtSourceLanguage;

    }
}

