namespace Sdl.Utilities.SplitSDLXLIFF
{
    partial class PerformMerge
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
			this.tbLog = new System.Windows.Forms.TextBox();
			this.pbProgress = new System.Windows.Forms.ProgressBar();
			this.btnOpenFolder = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Banner
			// 
			this.Banner.Size = new System.Drawing.Size(610, 64);
			// 
			// tbLog
			// 
			this.tbLog.BackColor = System.Drawing.Color.White;
			this.tbLog.Location = new System.Drawing.Point(6, 95);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbLog.Size = new System.Drawing.Size(594, 251);
			this.tbLog.TabIndex = 1;
			// 
			// pbProgress
			// 
			this.pbProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
			this.pbProgress.Location = new System.Drawing.Point(6, 71);
			this.pbProgress.Name = "pbProgress";
			this.pbProgress.Size = new System.Drawing.Size(594, 17);
			this.pbProgress.TabIndex = 7;
			// 
			// btnOpenFolder
			// 
			this.btnOpenFolder.Location = new System.Drawing.Point(472, 352);
			this.btnOpenFolder.Name = "btnOpenFolder";
			this.btnOpenFolder.Size = new System.Drawing.Size(128, 23);
			this.btnOpenFolder.TabIndex = 2;
			this.btnOpenFolder.Text = "&Open Target Folder";
			this.btnOpenFolder.UseVisualStyleBackColor = true;
			this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);
			// 
			// PerformMerge
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbLog);
			this.Controls.Add(this.pbProgress);
			this.Controls.Add(this.btnOpenFolder);
			this.Name = "PerformMerge";
			this.Size = new System.Drawing.Size(610, 385);
			this.SetActive += new System.ComponentModel.CancelEventHandler(this.PerformMerge_SetActive);
			this.PostSetActive += new System.ComponentModel.CancelEventHandler(this.PerformMerge_PostSetActive);
			this.WizardNewStart += new Sdl.Utilities.SplitSDLXLIFF.Wizard.WizardPageEventHandler(this.PerformMerge_WizardNewStart);
			this.Controls.SetChildIndex(this.Banner, 0);
			this.Controls.SetChildIndex(this.btnOpenFolder, 0);
			this.Controls.SetChildIndex(this.pbProgress, 0);
			this.Controls.SetChildIndex(this.tbLog, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Button btnOpenFolder;

    }
}
