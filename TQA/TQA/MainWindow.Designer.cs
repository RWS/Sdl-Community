namespace TQA
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
			this.ProjectNameLabel = new System.Windows.Forms.Label();
			this.LanguageSelector = new System.Windows.Forms.ComboBox();
			this.LanguageInfoLabel = new System.Windows.Forms.Label();
			this.StartButton = new System.Windows.Forms.Button();
			this.outputSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// ProjectNameLabel
			// 
			this.ProjectNameLabel.AutoSize = true;
			this.ProjectNameLabel.Location = new System.Drawing.Point(12, 9);
			this.ProjectNameLabel.Name = "ProjectNameLabel";
			this.ProjectNameLabel.Size = new System.Drawing.Size(35, 13);
			this.ProjectNameLabel.TabIndex = 0;
			this.ProjectNameLabel.Text = "label1";
			// 
			// LanguageSelector
			// 
			this.LanguageSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.LanguageSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LanguageSelector.FormattingEnabled = true;
			this.LanguageSelector.Location = new System.Drawing.Point(15, 42);
			this.LanguageSelector.MaxDropDownItems = 16;
			this.LanguageSelector.Name = "LanguageSelector";
			this.LanguageSelector.Size = new System.Drawing.Size(351, 21);
			this.LanguageSelector.TabIndex = 1;
			// 
			// LanguageInfoLabel
			// 
			this.LanguageInfoLabel.AutoSize = true;
			this.LanguageInfoLabel.Location = new System.Drawing.Point(12, 26);
			this.LanguageInfoLabel.Name = "LanguageInfoLabel";
			this.LanguageInfoLabel.Size = new System.Drawing.Size(252, 13);
			this.LanguageInfoLabel.TabIndex = 2;
			this.LanguageInfoLabel.Text = "Please select language which should be processed:";
			// 
			// StartButton
			// 
			this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.StartButton.Location = new System.Drawing.Point(372, 41);
			this.StartButton.Name = "StartButton";
			this.StartButton.Size = new System.Drawing.Size(75, 23);
			this.StartButton.TabIndex = 3;
			this.StartButton.Text = "Run TQA";
			this.StartButton.UseVisualStyleBackColor = true;
			this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
			// 
			// outputSaveDialog
			// 
			this.outputSaveDialog.DefaultExt = "*.xlsm";
			this.outputSaveDialog.FileName = "output";
			this.outputSaveDialog.Filter = "Excel Macro-Enabled Workbook|*.xlsm";
			// 
			// MainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(456, 74);
			this.Controls.Add(this.StartButton);
			this.Controls.Add(this.LanguageInfoLabel);
			this.Controls.Add(this.LanguageSelector);
			this.Controls.Add(this.ProjectNameLabel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Run TQA";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProjectNameLabel;
        private System.Windows.Forms.ComboBox LanguageSelector;
        private System.Windows.Forms.Label LanguageInfoLabel;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.SaveFileDialog outputSaveDialog;
    }
}