using Sdl.Community.WordCloud.Controls;

namespace Sdl.Community.WordCloud.Plugin
{
    partial class WordCloudViewPartControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._cloudControl = new CloudControl();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._generateButton = new System.Windows.Forms.Button();
            this._generatingLabel = new System.Windows.Forms.Label();
            this._tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanel
            // 
            this._tableLayoutPanel.ColumnCount = 3;
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Controls.Add(this._cloudControl, 0, 1);
            this._tableLayoutPanel.Controls.Add(this._progressBar, 2, 0);
            this._tableLayoutPanel.Controls.Add(this._generateButton, 0, 0);
            this._tableLayoutPanel.Controls.Add(this._generatingLabel, 1, 0);
            this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanel.Name = "_tableLayoutPanel";
            this._tableLayoutPanel.RowCount = 2;
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanel.Size = new System.Drawing.Size(510, 313);
            this._tableLayoutPanel.TabIndex = 0;
            // 
            // _cloudControl
            // 
            this._cloudControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._cloudControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._tableLayoutPanel.SetColumnSpan(this._cloudControl, 3);
            this._cloudControl.LayoutType = LayoutType.Spiral;
            this._cloudControl.Location = new System.Drawing.Point(3, 32);
            this._cloudControl.MaxFontSize = 68;
            this._cloudControl.MinFontSize = 6;
            this._cloudControl.Name = "_cloudControl";
            this._cloudControl.Palette = new System.Drawing.Color[] {
        System.Drawing.Color.DarkRed,
        System.Drawing.Color.DarkBlue,
        System.Drawing.Color.DarkGreen,
        System.Drawing.Color.Navy,
        System.Drawing.Color.DarkCyan,
        System.Drawing.Color.DarkOrange,
        System.Drawing.Color.DarkGoldenrod,
        System.Drawing.Color.DarkKhaki,
        System.Drawing.Color.Blue,
        System.Drawing.Color.Red,
        System.Drawing.Color.Green};
            this._cloudControl.Size = new System.Drawing.Size(504, 278);
            this._cloudControl.TabIndex = 2;
            this._cloudControl.WeightedWords = null;
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.Location = new System.Drawing.Point(254, 3);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(253, 23);
            this._progressBar.TabIndex = 3;
            // 
            // _generateButton
            // 
            this._generateButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._generateButton.AutoSize = true;
            this._generateButton.Location = new System.Drawing.Point(3, 3);
            this._generateButton.Name = "_generateButton";
            this._generateButton.Size = new System.Drawing.Size(129, 23);
            this._generateButton.TabIndex = 1;
            this._generateButton.Text = "Create Word Cloud...";
            this._generateButton.UseVisualStyleBackColor = true;
            this._generateButton.Click += new System.EventHandler(this._generateButton_Click);
            // 
            // _generatingLabel
            // 
            this._generatingLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._generatingLabel.AutoSize = true;
            this._generatingLabel.Location = new System.Drawing.Point(138, 8);
            this._generatingLabel.Name = "_generatingLabel";
            this._generatingLabel.Size = new System.Drawing.Size(110, 13);
            this._generatingLabel.TabIndex = 4;
            this._generatingLabel.Text = "Creating word cloud...";
            // 
            // WordCloudViewPartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayoutPanel);
            this.Name = "WordCloudViewPartControl";
            this.Size = new System.Drawing.Size(510, 313);
            this._tableLayoutPanel.ResumeLayout(false);
            this._tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.Button _generateButton;
        private CloudControl _cloudControl;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Label _generatingLabel;
    }
}
