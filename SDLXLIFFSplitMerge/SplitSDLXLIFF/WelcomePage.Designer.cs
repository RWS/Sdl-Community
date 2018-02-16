using Sdl.Utilities.SplitSDLXLIFF.Wizard;
namespace Sdl.Utilities.SplitSDLXLIFF
{
    partial class WelcomePage
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
            this.gbSplitMerge = new System.Windows.Forms.GroupBox();
            this.rbMerge = new System.Windows.Forms.RadioButton();
            this.rbSplit = new System.Windows.Forms.RadioButton();
            this.gbSplitMerge.SuspendLayout();
            this.SuspendLayout();
            // 
            // Banner
            // 
            this.Banner.MaximumSize = new System.Drawing.Size(1000, 64);
            this.Banner.MinimumSize = new System.Drawing.Size(30, 64);
            this.Banner.Size = new System.Drawing.Size(610, 64);
            // 
            // gbSplitMerge
            // 
            this.gbSplitMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSplitMerge.BackColor = System.Drawing.Color.Transparent;
            this.gbSplitMerge.Controls.Add(this.rbMerge);
            this.gbSplitMerge.Controls.Add(this.rbSplit);
            this.gbSplitMerge.Location = new System.Drawing.Point(15, 71);
            this.gbSplitMerge.Name = "gbSplitMerge";
            this.gbSplitMerge.Size = new System.Drawing.Size(580, 300);
            this.gbSplitMerge.TabIndex = 1;
            this.gbSplitMerge.TabStop = false;
            this.gbSplitMerge.Text = "Task";
            // 
            // rbMerge
            // 
            this.rbMerge.AutoSize = true;
            this.rbMerge.Location = new System.Drawing.Point(10, 40);
            this.rbMerge.Name = "rbMerge";
            this.rbMerge.Size = new System.Drawing.Size(131, 17);
            this.rbMerge.TabIndex = 1;
            this.rbMerge.TabStop = true;
            this.rbMerge.Text = "&Merge SDLXLIFF files";
            this.rbMerge.UseVisualStyleBackColor = true;
            // 
            // rbSplit
            // 
            this.rbSplit.AutoSize = true;
            this.rbSplit.Location = new System.Drawing.Point(10, 17);
            this.rbSplit.Name = "rbSplit";
            this.rbSplit.Size = new System.Drawing.Size(121, 17);
            this.rbSplit.TabIndex = 0;
            this.rbSplit.TabStop = true;
            this.rbSplit.Text = "&Split SDLXLIFF files";
            this.rbSplit.UseVisualStyleBackColor = true;
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbSplitMerge);
            this.Name = "WelcomePage";
            this.Size = new System.Drawing.Size(610, 385);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.WelcomePage_SetActive);
            this.WizardNext += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.WelcomePage_WizardNext);
            this.Controls.SetChildIndex(this.gbSplitMerge, 0);
            this.Controls.SetChildIndex(this.Banner, 0);
            this.gbSplitMerge.ResumeLayout(false);
            this.gbSplitMerge.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSplitMerge;
        private System.Windows.Forms.RadioButton rbMerge;
        private System.Windows.Forms.RadioButton rbSplit;
    }
}
