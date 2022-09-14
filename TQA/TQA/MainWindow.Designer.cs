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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
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
			this.tableLayoutPanel1.SuspendLayout();
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
			this.panel1.Controls.Add(this.tableLayoutPanel1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(5, 10, 5, 5);
			this.panel1.Size = new System.Drawing.Size(395, 63);
			this.panel1.TabIndex = 3;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.txtStandardUsed, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.txtProjectName, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 10);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(385, 48);
			this.tableLayoutPanel1.TabIndex = 12;
			// 
			// txtStandardUsed
			// 
			this.txtStandardUsed.AutoEllipsis = true;
			this.txtStandardUsed.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtStandardUsed.Location = new System.Drawing.Point(0, 24);
			this.txtStandardUsed.Margin = new System.Windows.Forms.Padding(0);
			this.txtStandardUsed.Name = "txtStandardUsed";
			this.txtStandardUsed.Padding = new System.Windows.Forms.Padding(3);
			this.txtStandardUsed.Size = new System.Drawing.Size(385, 24);
			this.txtStandardUsed.TabIndex = 11;
			this.txtStandardUsed.Text = "TQAProfileName";
			this.txtStandardUsed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtProjectName
			// 
			this.txtProjectName.AutoEllipsis = true;
			this.txtProjectName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtProjectName.Location = new System.Drawing.Point(0, 0);
			this.txtProjectName.Margin = new System.Windows.Forms.Padding(0);
			this.txtProjectName.Name = "txtProjectName";
			this.txtProjectName.Padding = new System.Windows.Forms.Padding(3);
			this.txtProjectName.Size = new System.Drawing.Size(385, 24);
			this.txtProjectName.TabIndex = 1;
			this.txtProjectName.Text = "ProjectName";
			this.txtProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gbTQAReportingOptions
			// 
			this.gbTQAReportingOptions.Controls.Add(this.QualityCombo);
			this.gbTQAReportingOptions.Controls.Add(this.QualityLevelLbl);
			this.gbTQAReportingOptions.Controls.Add(this.LanguageInfoLabel);
			this.gbTQAReportingOptions.Controls.Add(this.LanguageSelector);
			this.gbTQAReportingOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTQAReportingOptions.Location = new System.Drawing.Point(10, 10);
			this.gbTQAReportingOptions.Margin = new System.Windows.Forms.Padding(2);
			this.gbTQAReportingOptions.Name = "gbTQAReportingOptions";
			this.gbTQAReportingOptions.Padding = new System.Windows.Forms.Padding(2);
			this.gbTQAReportingOptions.Size = new System.Drawing.Size(375, 140);
			this.gbTQAReportingOptions.TabIndex = 3;
			this.gbTQAReportingOptions.TabStop = false;
			this.gbTQAReportingOptions.Text = "Options";
			// 
			// QualityCombo
			// 
			this.QualityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.QualityCombo.FormattingEnabled = true;
			this.QualityCombo.Location = new System.Drawing.Point(15, 81);
			this.QualityCombo.Name = "QualityCombo";
			this.QualityCombo.Size = new System.Drawing.Size(209, 21);
			this.QualityCombo.TabIndex = 1;
			// 
			// QualityLevelLbl
			// 
			this.QualityLevelLbl.AutoSize = true;
			this.QualityLevelLbl.Location = new System.Drawing.Point(7, 64);
			this.QualityLevelLbl.Margin = new System.Windows.Forms.Padding(3);
			this.QualityLevelLbl.Name = "QualityLevelLbl";
			this.QualityLevelLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.QualityLevelLbl.Size = new System.Drawing.Size(72, 13);
			this.QualityLevelLbl.TabIndex = 8;
			this.QualityLevelLbl.Text = "Quality level:";
			// 
			// LanguageInfoLabel
			// 
			this.LanguageInfoLabel.AutoSize = true;
			this.LanguageInfoLabel.Location = new System.Drawing.Point(7, 21);
			this.LanguageInfoLabel.Margin = new System.Windows.Forms.Padding(3);
			this.LanguageInfoLabel.Name = "LanguageInfoLabel";
			this.LanguageInfoLabel.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.LanguageInfoLabel.Size = new System.Drawing.Size(97, 13);
			this.LanguageInfoLabel.TabIndex = 7;
			this.LanguageInfoLabel.Text = "Target Language:";
			// 
			// LanguageSelector
			// 
			this.LanguageSelector.DisplayMember = "DisplayName";
			this.LanguageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LanguageSelector.FormattingEnabled = true;
			this.LanguageSelector.Location = new System.Drawing.Point(15, 37);
			this.LanguageSelector.MaxDropDownItems = 16;
			this.LanguageSelector.Name = "LanguageSelector";
			this.LanguageSelector.Size = new System.Drawing.Size(209, 21);
			this.LanguageSelector.TabIndex = 0;
			this.LanguageSelector.SelectedIndexChanged += new System.EventHandler(this.LanguageSelector_SelectedIndexChanged);
			// 
			// panel3
			// 
			this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel3.Controls.Add(this.label_progress);
			this.panel3.Controls.Add(this.StartButton);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel3.Location = new System.Drawing.Point(0, 223);
			this.panel3.Margin = new System.Windows.Forms.Padding(2, 10, 2, 2);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.panel3.Size = new System.Drawing.Size(395, 32);
			this.panel3.TabIndex = 5;
			// 
			// label_progress
			// 
			this.label_progress.AutoSize = true;
			this.label_progress.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
			this.label_progress.Location = new System.Drawing.Point(6, 7);
			this.label_progress.Name = "label_progress";
			this.label_progress.Size = new System.Drawing.Size(38, 13);
			this.label_progress.TabIndex = 3;
			this.label_progress.Text = "Ready";
			// 
			// StartButton
			// 
			this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.StartButton.Location = new System.Drawing.Point(257, 1);
			this.StartButton.Name = "StartButton";
			this.StartButton.Size = new System.Drawing.Size(129, 23);
			this.StartButton.TabIndex = 2;
			this.StartButton.Text = "Run TQA Report";
			this.StartButton.UseVisualStyleBackColor = true;
			this.StartButton.Click += new System.EventHandler(this.RunTQAReport_ButtonClick);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.gbTQAReportingOptions);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 63);
			this.panel2.Margin = new System.Windows.Forms.Padding(2);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(10);
			this.panel2.Size = new System.Drawing.Size(395, 160);
			this.panel2.TabIndex = 4;
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(395, 255);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainWindow";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Run TQA Reporting";
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
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
		private System.Windows.Forms.GroupBox gbTQAReportingOptions;
		private System.Windows.Forms.ComboBox QualityCombo;
		private System.Windows.Forms.Label QualityLevelLbl;
		private System.Windows.Forms.Label LanguageInfoLabel;
		private System.Windows.Forms.ComboBox LanguageSelector;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button StartButton;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label_progress;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label txtStandardUsed;
		private System.Windows.Forms.Label txtProjectName;
	}
}