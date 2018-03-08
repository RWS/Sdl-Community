namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button cancelButton;
        private GradientPanel roundedStatusPanel;
        private System.Windows.Forms.LinkLabel helpLinkLabel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.cancelButton = new System.Windows.Forms.Button();
			this.helpLinkLabel = new System.Windows.Forms.LinkLabel();
			this.roundedStatusPanel = new Sdl.TranslationStudio.Plugins.Trados2007.UI.GradientPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.fileBasedButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.roundedStatusPanel.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(451, 164);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);
			// 
			// helpLinkLabel
			// 
			this.helpLinkLabel.AutoSize = true;
			this.helpLinkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.helpLinkLabel.Location = new System.Drawing.Point(397, 167);
			this.helpLinkLabel.Name = "helpLinkLabel";
			this.helpLinkLabel.Size = new System.Drawing.Size(33, 15);
			this.helpLinkLabel.TabIndex = 7;
			this.helpLinkLabel.TabStop = true;
			this.helpLinkLabel.Text = "Help";
			this.helpLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnHelpLinkLabelLinkClicked);
			// 
			// roundedStatusPanel
			// 
			this.roundedStatusPanel.BorderColor = System.Drawing.Color.Gray;
			this.roundedStatusPanel.Controls.Add(this.tableLayoutPanel1);
			this.roundedStatusPanel.GradientEndColor = System.Drawing.SystemColors.Control;
			this.roundedStatusPanel.GradientStartColor = System.Drawing.Color.White;
			this.roundedStatusPanel.Image = null;
			this.roundedStatusPanel.ImageLocation = new System.Drawing.Point(4, 4);
			this.roundedStatusPanel.Location = new System.Drawing.Point(12, 12);
			this.roundedStatusPanel.Name = "roundedStatusPanel";
			this.roundedStatusPanel.RoundCornerRadius = 20;
			this.roundedStatusPanel.Size = new System.Drawing.Size(514, 125);
			this.roundedStatusPanel.TabIndex = 6;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.HighlightText;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.fileBasedButton, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 2, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 18);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 8F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 89.33334F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.66667F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 83);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// fileBasedButton
			// 
			this.fileBasedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fileBasedButton.Image = global::Sdl.TranslationStudio.Plugins.Trados2007.PluginResources.Trados2007_FileBased_Image;
			this.fileBasedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.fileBasedButton.Location = new System.Drawing.Point(24, 18);
			this.fileBasedButton.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
			this.fileBasedButton.Name = "fileBasedButton";
			this.fileBasedButton.Size = new System.Drawing.Size(222, 47);
			this.fileBasedButton.TabIndex = 0;
			this.fileBasedButton.Text = "&File-based Translation Memory...";
			this.fileBasedButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.fileBasedButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.fileBasedButton.UseVisualStyleBackColor = true;
			this.fileBasedButton.Click += new System.EventHandler(this.OnFileBasedButtonClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(283, 33);
			this.label1.Margin = new System.Windows.Forms.Padding(33, 25, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(164, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Select a local translation memory.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(546, 208);
			this.Controls.Add(this.helpLinkLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.roundedStatusPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add SDL Trados 2007 Translation Memory";
			this.roundedStatusPanel.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Button fileBasedButton;
		private System.Windows.Forms.Label label1;
	}
}