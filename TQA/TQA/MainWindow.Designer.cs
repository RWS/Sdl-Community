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
            this.StartButton = new System.Windows.Forms.Button();
            this.outputSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.QualityCombo = new System.Windows.Forms.ComboBox();
            this.QualityLevelLbl = new System.Windows.Forms.Label();
            this.LanguageInfoLabel = new System.Windows.Forms.Label();
            this.LanguageSelector = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtStandardUsed = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(500, 174);
            this.StartButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(112, 35);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Run TQA";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // outputSaveDialog
            // 
            this.outputSaveDialog.DefaultExt = "*.xlsm";
            this.outputSaveDialog.Filter = "Excel Macro-Enabled Workbook|*.xlsm";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.QualityCombo);
            this.groupBox1.Controls.Add(this.QualityLevelLbl);
            this.groupBox1.Controls.Add(this.LanguageInfoLabel);
            this.groupBox1.Controls.Add(this.LanguageSelector);
            this.groupBox1.Location = new System.Drawing.Point(22, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(422, 155);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // QualityCombo
            // 
            this.QualityCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QualityCombo.FormattingEnabled = true;
            this.QualityCombo.Location = new System.Drawing.Point(32, 115);
            this.QualityCombo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.QualityCombo.Name = "QualityCombo";
            this.QualityCombo.Size = new System.Drawing.Size(358, 28);
            this.QualityCombo.TabIndex = 1;
            // 
            // QualityLevelLbl
            // 
            this.QualityLevelLbl.AutoSize = true;
            this.QualityLevelLbl.Location = new System.Drawing.Point(28, 90);
            this.QualityLevelLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.QualityLevelLbl.Name = "QualityLevelLbl";
            this.QualityLevelLbl.Size = new System.Drawing.Size(191, 20);
            this.QualityLevelLbl.TabIndex = 8;
            this.QualityLevelLbl.Text = "Please select quality level:";
            // 
            // LanguageInfoLabel
            // 
            this.LanguageInfoLabel.AutoSize = true;
            this.LanguageInfoLabel.Location = new System.Drawing.Point(28, 22);
            this.LanguageInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LanguageInfoLabel.Name = "LanguageInfoLabel";
            this.LanguageInfoLabel.Size = new System.Drawing.Size(372, 20);
            this.LanguageInfoLabel.TabIndex = 7;
            this.LanguageInfoLabel.Text = "Please select language which should be processed:";
            // 
            // LanguageSelector
            // 
            this.LanguageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LanguageSelector.FormattingEnabled = true;
            this.LanguageSelector.Location = new System.Drawing.Point(32, 47);
            this.LanguageSelector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LanguageSelector.MaxDropDownItems = 16;
            this.LanguageSelector.Name = "LanguageSelector";
            this.LanguageSelector.Size = new System.Drawing.Size(358, 28);
            this.LanguageSelector.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtStandardUsed);
            this.panel1.Controls.Add(this.txtProjectName);
            this.panel1.Location = new System.Drawing.Point(22, 15);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(611, 33);
            this.panel1.TabIndex = 4;
            // 
            // txtStandardUsed
            // 
            this.txtStandardUsed.AutoSize = true;
            this.txtStandardUsed.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtStandardUsed.Location = new System.Drawing.Point(115, 5);
            this.txtStandardUsed.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.txtStandardUsed.Name = "txtStandardUsed";
            this.txtStandardUsed.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.txtStandardUsed.Size = new System.Drawing.Size(139, 20);
            this.txtStandardUsed.TabIndex = 3;
            this.txtStandardUsed.Text = "TQStandardUsed";
            // 
            // txtProjectName
            // 
            this.txtProjectName.AutoSize = true;
            this.txtProjectName.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtProjectName.Location = new System.Drawing.Point(5, 5);
            this.txtProjectName.Margin = new System.Windows.Forms.Padding(0);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.txtProjectName.Size = new System.Drawing.Size(110, 20);
            this.txtProjectName.TabIndex = 1;
            this.txtProjectName.Text = "ProjectName";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 227);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.StartButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Run TQA Reporting";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.SaveFileDialog outputSaveDialog;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox QualityCombo;
		private System.Windows.Forms.Label QualityLevelLbl;
		private System.Windows.Forms.Label LanguageInfoLabel;
		private System.Windows.Forms.ComboBox LanguageSelector;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label txtStandardUsed;
		private System.Windows.Forms.Label txtProjectName;
	}
}