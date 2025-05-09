namespace GroupshareExcelAddIn.Controls
{
    partial class ProgressBarForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressBarForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this._barsGroupBox = new System.Windows.Forms.GroupBox();
            this._thirdLabel = new System.Windows.Forms.Label();
            this._thirdProgressBar = new System.Windows.Forms.ProgressBar();
            this._secondLabel = new System.Windows.Forms.Label();
            this._firstLabel = new System.Windows.Forms.Label();
            this._secondProgressBar = new System.Windows.Forms.ProgressBar();
            this._firstProgressBar = new System.Windows.Forms.ProgressBar();
            this._barsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Adding data to the Excel sheet...";
            this.label1.UseWaitCursor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(228, 197);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(87, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.UseWaitCursor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // _barsGroupBox
            // 
            this._barsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._barsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._barsGroupBox.Controls.Add(this._thirdLabel);
            this._barsGroupBox.Controls.Add(this._thirdProgressBar);
            this._barsGroupBox.Controls.Add(this._secondLabel);
            this._barsGroupBox.Controls.Add(this._firstLabel);
            this._barsGroupBox.Controls.Add(this._secondProgressBar);
            this._barsGroupBox.Controls.Add(this._firstProgressBar);
            this._barsGroupBox.Location = new System.Drawing.Point(12, 41);
            this._barsGroupBox.Name = "_barsGroupBox";
            this._barsGroupBox.Size = new System.Drawing.Size(303, 138);
            this._barsGroupBox.TabIndex = 8;
            this._barsGroupBox.TabStop = false;
            this._barsGroupBox.UseWaitCursor = true;
            // 
            // _thirdLabel
            // 
            this._thirdLabel.AutoSize = true;
            this._thirdLabel.Location = new System.Drawing.Point(6, 90);
            this._thirdLabel.Name = "_thirdLabel";
            this._thirdLabel.Size = new System.Drawing.Size(28, 13);
            this._thirdLabel.TabIndex = 12;
            this._thirdLabel.Text = "Files";
            this._thirdLabel.UseWaitCursor = true;
            // 
            // _thirdProgressBar
            // 
            this._thirdProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._thirdProgressBar.Location = new System.Drawing.Point(9, 109);
            this._thirdProgressBar.Name = "_thirdProgressBar";
            this._thirdProgressBar.Size = new System.Drawing.Size(288, 10);
            this._thirdProgressBar.TabIndex = 11;
            this._thirdProgressBar.UseWaitCursor = true;
            // 
            // _secondLabel
            // 
            this._secondLabel.AutoSize = true;
            this._secondLabel.Location = new System.Drawing.Point(6, 53);
            this._secondLabel.Name = "_secondLabel";
            this._secondLabel.Size = new System.Drawing.Size(28, 13);
            this._secondLabel.TabIndex = 10;
            this._secondLabel.Text = "Files";
            this._secondLabel.UseWaitCursor = true;
            // 
            // _firstLabel
            // 
            this._firstLabel.AutoSize = true;
            this._firstLabel.Location = new System.Drawing.Point(6, 16);
            this._firstLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this._firstLabel.Name = "_firstLabel";
            this._firstLabel.Size = new System.Drawing.Size(45, 13);
            this._firstLabel.TabIndex = 9;
            this._firstLabel.Text = "Projects";
            this._firstLabel.UseWaitCursor = true;
            // 
            // _secondProgressBar
            // 
            this._secondProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._secondProgressBar.Location = new System.Drawing.Point(9, 72);
            this._secondProgressBar.Name = "_secondProgressBar";
            this._secondProgressBar.Size = new System.Drawing.Size(288, 10);
            this._secondProgressBar.TabIndex = 8;
            this._secondProgressBar.UseWaitCursor = true;
            // 
            // _firstProgressBar
            // 
            this._firstProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._firstProgressBar.Location = new System.Drawing.Point(9, 35);
            this._firstProgressBar.Name = "_firstProgressBar";
            this._firstProgressBar.Size = new System.Drawing.Size(288, 10);
            this._firstProgressBar.TabIndex = 7;
            this._firstProgressBar.UseWaitCursor = true;
            // 
            // ProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(334, 232);
            this.Controls.Add(this._barsGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ProgressBarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data retrieval in progress...";
            this.TopMost = true;
            this.UseWaitCursor = true;
            this._barsGroupBox.ResumeLayout(false);
            this._barsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox _barsGroupBox;
        private System.Windows.Forms.Label _secondLabel;
        private System.Windows.Forms.Label _firstLabel;
        private System.Windows.Forms.ProgressBar _secondProgressBar;
        private System.Windows.Forms.ProgressBar _firstProgressBar;
        private System.Windows.Forms.Label _thirdLabel;
        private System.Windows.Forms.ProgressBar _thirdProgressBar;
    }
}