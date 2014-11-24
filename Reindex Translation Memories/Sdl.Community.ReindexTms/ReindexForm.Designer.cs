namespace Sdl.Community.ReindexTms
{
    partial class ReindexForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReindexForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkLoadStudioTMs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnReindex = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbStatus = new System.Windows.Forms.RichTextBox();
            this.lstTms = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkLoadStudioTMs);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.btnReindex);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 347);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(524, 36);
            this.panel1.TabIndex = 0;
            // 
            // chkLoadStudioTMs
            // 
            this.chkLoadStudioTMs.AutoSize = true;
            this.chkLoadStudioTMs.Location = new System.Drawing.Point(94, 11);
            this.chkLoadStudioTMs.Name = "chkLoadStudioTMs";
            this.chkLoadStudioTMs.Size = new System.Drawing.Size(107, 17);
            this.chkLoadStudioTMs.TabIndex = 2;
            this.chkLoadStudioTMs.Text = "Load Studio TMs";
            this.chkLoadStudioTMs.UseVisualStyleBackColor = true;
            this.chkLoadStudioTMs.CheckedChanged += new System.EventHandler(this.chkLoadStudioTMs_CheckedChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse ...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnReindex
            // 
            this.btnReindex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReindex.Location = new System.Drawing.Point(437, 6);
            this.btnReindex.Name = "btnReindex";
            this.btnReindex.Size = new System.Drawing.Size(75, 23);
            this.btnReindex.TabIndex = 0;
            this.btnReindex.Text = "Re-index";
            this.btnReindex.UseVisualStyleBackColor = true;
            this.btnReindex.Click += new System.EventHandler(this.btnReindex_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtbStatus);
            this.panel2.Controls.Add(this.lstTms);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(524, 347);
            this.panel2.TabIndex = 1;
            // 
            // rtbStatus
            // 
            this.rtbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbStatus.Location = new System.Drawing.Point(0, 186);
            this.rtbStatus.Name = "rtbStatus";
            this.rtbStatus.Size = new System.Drawing.Size(524, 161);
            this.rtbStatus.TabIndex = 1;
            this.rtbStatus.Text = "";
            // 
            // lstTms
            // 
            this.lstTms.AllowDrop = true;
            this.lstTms.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstTms.FormattingEnabled = true;
            this.lstTms.Location = new System.Drawing.Point(0, 0);
            this.lstTms.Name = "lstTms";
            this.lstTms.Size = new System.Drawing.Size(524, 186);
            this.lstTms.TabIndex = 0;
            this.lstTms.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstTms_DragDrop);
            this.lstTms.DragOver += new System.Windows.Forms.DragEventHandler(this.lstTms_DragOver);
            // 
            // ReindexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 383);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReindexForm";
            this.Text = "Available Translation Memories";
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnReindex;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox lstTms;
        private System.Windows.Forms.CheckBox chkLoadStudioTMs;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.RichTextBox rtbStatus;
    }
}

