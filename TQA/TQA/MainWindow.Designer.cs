namespace Sdl.Community.TQA
{
    partial class MainWindow
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
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.outputSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtStandardUsed = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.Label();
            this.gbTQAReportingOptions = new System.Windows.Forms.GroupBox();
            this.QualityCombo = new System.Windows.Forms.ComboBox();
            this.QualityLevelLbl = new System.Windows.Forms.Label();
            this.LanguageInfoLabel = new System.Windows.Forms.Label();
            this.LanguageSelector = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label_progress = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.gbTQAReportingOptions.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputSaveDialog
            // 
            this.outputSaveDialog.DefaultExt = "*.xlsm";
            this.outputSaveDialog.Filter = "Excel Macro-Enabled Workbook|*.xlsm";
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.txtStandardUsed);
            this.panel1.Controls.Add(this.txtProjectName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4, 23, 4, 9);
            this.panel1.Size = new System.Drawing.Size(604, 89);
            this.panel1.TabIndex = 3;
            // 
            // txtStandardUsed
            // 
            this.txtStandardUsed.AutoEllipsis = true;
            this.txtStandardUsed.AutoSize = true;
            this.txtStandardUsed.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtStandardUsed.Location = new System.Drawing.Point(4, 48);
            this.txtStandardUsed.Margin = new System.Windows.Forms.Padding(0);
            this.txtStandardUsed.MaximumSize = new System.Drawing.Size(400, 0);
            this.txtStandardUsed.Name = "txtStandardUsed";
            this.txtStandardUsed.Padding = new System.Windows.Forms.Padding(8, 15, 0, 5);
            this.txtStandardUsed.Size = new System.Drawing.Size(142, 40);
            this.txtStandardUsed.TabIndex = 11;
            this.txtStandardUsed.Text = "TQStandardUsed";
            // 
            // txtProjectName
            // 
            this.txtProjectName.AutoEllipsis = true;
            this.txtProjectName.AutoSize = true;
            this.txtProjectName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtProjectName.Location = new System.Drawing.Point(4, 23);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(0, 5, 0, 9);
            this.txtProjectName.MaximumSize = new System.Drawing.Size(400, 0);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Padding = new System.Windows.Forms.Padding(8, 0, 0, 5);
            this.txtProjectName.Size = new System.Drawing.Size(108, 25);
            this.txtProjectName.TabIndex = 1;
            this.txtProjectName.Text = "ProjectName";
            // 
            // gbTQAReportingOptions
            // 
            this.gbTQAReportingOptions.Controls.Add(this.QualityCombo);
            this.gbTQAReportingOptions.Controls.Add(this.QualityLevelLbl);
            this.gbTQAReportingOptions.Controls.Add(this.LanguageInfoLabel);
            this.gbTQAReportingOptions.Controls.Add(this.LanguageSelector);
            this.gbTQAReportingOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbTQAReportingOptions.Location = new System.Drawing.Point(15, 15);
            this.gbTQAReportingOptions.Name = "gbTQAReportingOptions";
            this.gbTQAReportingOptions.Size = new System.Drawing.Size(574, 192);
            this.gbTQAReportingOptions.TabIndex = 3;
            this.gbTQAReportingOptions.TabStop = false;
            this.gbTQAReportingOptions.Text = "Options";
            // 
            // QualityCombo
            // 
            this.QualityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QualityCombo.FormattingEnabled = true;
            this.QualityCombo.Location = new System.Drawing.Point(22, 125);
            this.QualityCombo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.QualityCombo.Name = "QualityCombo";
            this.QualityCombo.Size = new System.Drawing.Size(312, 28);
            this.QualityCombo.TabIndex = 1;
            // 
            // QualityLevelLbl
            // 
            this.QualityLevelLbl.AutoSize = true;
            this.QualityLevelLbl.Location = new System.Drawing.Point(10, 98);
            this.QualityLevelLbl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.QualityLevelLbl.Name = "QualityLevelLbl";
            this.QualityLevelLbl.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.QualityLevelLbl.Size = new System.Drawing.Size(104, 20);
            this.QualityLevelLbl.TabIndex = 8;
            this.QualityLevelLbl.Text = "Quality level:";
            // 
            // LanguageInfoLabel
            // 
            this.LanguageInfoLabel.AutoSize = true;
            this.LanguageInfoLabel.Location = new System.Drawing.Point(10, 32);
            this.LanguageInfoLabel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LanguageInfoLabel.Name = "LanguageInfoLabel";
            this.LanguageInfoLabel.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.LanguageInfoLabel.Size = new System.Drawing.Size(143, 20);
            this.LanguageInfoLabel.TabIndex = 7;
            this.LanguageInfoLabel.Text = "Target Language:";
            // 
            // LanguageSelector
            // 
            this.LanguageSelector.DisplayMember = "DisplayName";
            this.LanguageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageSelector.FormattingEnabled = true;
            this.LanguageSelector.Location = new System.Drawing.Point(22, 57);
            this.LanguageSelector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LanguageSelector.MaxDropDownItems = 16;
            this.LanguageSelector.Name = "LanguageSelector";
            this.LanguageSelector.Size = new System.Drawing.Size(312, 28);
            this.LanguageSelector.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.label_progress);
            this.panel3.Controls.Add(this.StartButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 311);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.panel3.Size = new System.Drawing.Size(604, 49);
            this.panel3.TabIndex = 5;
            // 
            // label_progress
            // 
            this.label_progress.AutoSize = true;
            this.label_progress.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label_progress.Location = new System.Drawing.Point(9, 11);
            this.label_progress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_progress.Name = "label_progress";
            this.label_progress.Size = new System.Drawing.Size(55, 20);
            this.label_progress.TabIndex = 3;
            this.label_progress.Text = "Ready";
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(398, 2);
            this.StartButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(194, 35);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Run TQA Report";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.RunTQAReport_ButtonClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gbTQAReportingOptions);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 89);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(15, 15, 15, 15);
            this.panel2.Size = new System.Drawing.Size(604, 222);
            this.panel2.TabIndex = 4;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(604, 360);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Run TQA Reporting";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbTQAReportingOptions.ResumeLayout(false);
            this.gbTQAReportingOptions.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog outputSaveDialog;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label txtProjectName;
		private System.Windows.Forms.Label txtStandardUsed;
		private System.Windows.Forms.GroupBox gbTQAReportingOptions;
		private System.Windows.Forms.ComboBox QualityCombo;
		private System.Windows.Forms.Label QualityLevelLbl;
		private System.Windows.Forms.Label LanguageInfoLabel;
		private System.Windows.Forms.ComboBox LanguageSelector;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button StartButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label_progress;
	}
}