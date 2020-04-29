namespace Sdl.LanguagePlatform.MTConnectors.Google.UI
{
    partial class WizardTitleControl
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
                
                if (_titleBitmap != null)
                {
                    _titleBitmap.Dispose();
                    _titleBitmap = null;
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardTitleControl));
            this._titlePanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this._subtitleLabel = new System.Windows.Forms.Label();
            this._lineFrame = new Sdl.LanguagePlatform.MTConnectors.Google.UI.LineFrame();
            this._titlePanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _titlePanel
            // 
            resources.ApplyResources(this._titlePanel, "_titlePanel");
            this._titlePanel.BackColor = System.Drawing.Color.White;
            this._titlePanel.Controls.Add(this.tableLayoutPanel1);
            this._titlePanel.Controls.Add(this._lineFrame);
            this._titlePanel.Name = "_titlePanel";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._titleLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._pictureBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._subtitleLabel, 0, 1);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _titleLabel
            // 
            resources.ApplyResources(this._titleLabel, "_titleLabel");
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this._titleLabel.Name = "_titleLabel";
            // 
            // _pictureBox
            // 
            resources.ApplyResources(this._pictureBox, "_pictureBox");
            this._pictureBox.BackColor = System.Drawing.Color.White;
            this._pictureBox.Name = "_pictureBox";
            this.tableLayoutPanel1.SetRowSpan(this._pictureBox, 2);
            this._pictureBox.TabStop = false;
            // 
            // _subtitleLabel
            // 
            resources.ApplyResources(this._subtitleLabel, "_subtitleLabel");
            this._subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this._subtitleLabel.Name = "_subtitleLabel";
            // 
            // _lineFrame
            // 
            this._lineFrame.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this._lineFrame, "_lineFrame");
            this._lineFrame.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this._lineFrame.Name = "_lineFrame";
            // 
            // WizardTitleControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._titlePanel);
            this.Name = "WizardTitleControl";
            this._titlePanel.ResumeLayout(false);
            this._titlePanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel _titlePanel;
        private System.Windows.Forms.PictureBox _pictureBox;
        private System.Windows.Forms.Label _subtitleLabel;
        private System.Windows.Forms.Label _titleLabel;
        private Sdl.LanguagePlatform.MTConnectors.Google.UI.LineFrame _lineFrame;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}
