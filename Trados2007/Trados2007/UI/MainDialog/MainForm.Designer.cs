namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button fileBasedButton;
        private System.Windows.Forms.Button serverBasedButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
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
            this.serverBasedButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.roundedStatusPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(497, 279);
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
            this.helpLinkLabel.Location = new System.Drawing.Point(370, 284);
            this.helpLinkLabel.Name = "helpLinkLabel";
            this.helpLinkLabel.Size = new System.Drawing.Size(121, 13);
            this.helpLinkLabel.TabIndex = 7;
            this.helpLinkLabel.TabStop = true;
            this.helpLinkLabel.Text = "Help on OpenExchange";
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
            this.roundedStatusPanel.Size = new System.Drawing.Size(560, 254);
            this.roundedStatusPanel.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.fileBasedButton, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.serverBasedButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(15, 18);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 218);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // fileBasedButton
            // 
            this.fileBasedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileBasedButton.Image = global::Sdl.TranslationStudio.Plugins.Trados2007.PluginResources.Trados2007_FileBased_Image;
            this.fileBasedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.fileBasedButton.Location = new System.Drawing.Point(24, 45);
            this.fileBasedButton.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
            this.fileBasedButton.Name = "fileBasedButton";
            this.fileBasedButton.Size = new System.Drawing.Size(235, 54);
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
            this.label1.Location = new System.Drawing.Point(276, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(13, 13, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a local translation memory.";
            // 
            // serverBasedButton
            // 
            this.serverBasedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverBasedButton.Enabled = false;
            this.serverBasedButton.Image = global::Sdl.TranslationStudio.Plugins.Trados2007.PluginResources.Trados2007_ServerBased_Image;
            this.serverBasedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.serverBasedButton.Location = new System.Drawing.Point(24, 119);
            this.serverBasedButton.Margin = new System.Windows.Forms.Padding(4, 10, 4, 10);
            this.serverBasedButton.Name = "serverBasedButton";
            this.serverBasedButton.Size = new System.Drawing.Size(235, 54);
            this.serverBasedButton.TabIndex = 1;
            this.serverBasedButton.Text = "&Server-based Translation Memory...";
            this.serverBasedButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.serverBasedButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.serverBasedButton.UseVisualStyleBackColor = true;
            this.serverBasedButton.Click += new System.EventHandler(this.OnServerBasedButtonClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(276, 122);
            this.label2.Margin = new System.Windows.Forms.Padding(13, 13, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(228, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Connect to a server and select a server-based translation memory. ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(581, 314);
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
    }
}