using System.ComponentModel;

namespace Sdl.Community.TMLifting
{
    partial class TMLiftingForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.upLiftCheckBox = new System.Windows.Forms.CheckBox();
            this.reIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.chkLoadStudioTMs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnReindex = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbStatus = new System.Windows.Forms.RichTextBox();
            this.lstTms = new System.Windows.Forms.ListBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.cleanBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cleanBtn);
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.upLiftCheckBox);
            this.panel1.Controls.Add(this.reIndexCheckBox);
            this.panel1.Controls.Add(this.chkLoadStudioTMs);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.btnReindex);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 534);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(786, 55);
            this.panel1.TabIndex = 0;
            // 
            // upLiftCheckBox
            // 
            this.upLiftCheckBox.AutoSize = true;
            this.upLiftCheckBox.Location = new System.Drawing.Point(449, 15);
            this.upLiftCheckBox.Name = "upLiftCheckBox";
            this.upLiftCheckBox.Size = new System.Drawing.Size(65, 24);
            this.upLiftCheckBox.TabIndex = 4;
            this.upLiftCheckBox.Text = "Uplift";
            this.upLiftCheckBox.UseVisualStyleBackColor = true;
            // 
            // reIndexCheckBox
            // 
            this.reIndexCheckBox.AutoSize = true;
            this.reIndexCheckBox.Location = new System.Drawing.Point(320, 15);
            this.reIndexCheckBox.Name = "reIndexCheckBox";
            this.reIndexCheckBox.Size = new System.Drawing.Size(93, 24);
            this.reIndexCheckBox.TabIndex = 3;
            this.reIndexCheckBox.Text = "Re-Index";
            this.reIndexCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkLoadStudioTMs
            // 
            this.chkLoadStudioTMs.AutoSize = true;
            this.chkLoadStudioTMs.Location = new System.Drawing.Point(141, 17);
            this.chkLoadStudioTMs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkLoadStudioTMs.Name = "chkLoadStudioTMs";
            this.chkLoadStudioTMs.Size = new System.Drawing.Size(148, 24);
            this.chkLoadStudioTMs.TabIndex = 2;
            this.chkLoadStudioTMs.Text = "Load Studio TMs";
            this.chkLoadStudioTMs.UseVisualStyleBackColor = true;
            this.chkLoadStudioTMs.CheckedChanged += new System.EventHandler(this.chkLoadStudioTMs_CheckedChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(18, 9);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(112, 35);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse ...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnReindex
            // 
            this.btnReindex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReindex.Location = new System.Drawing.Point(684, 9);
            this.btnReindex.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnReindex.Name = "btnReindex";
            this.btnReindex.Size = new System.Drawing.Size(84, 35);
            this.btnReindex.TabIndex = 0;
            this.btnReindex.Text = "Process";
            this.btnReindex.UseVisualStyleBackColor = true;
            this.btnReindex.Click += new System.EventHandler(this.btnReindex_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtbStatus);
            this.panel2.Controls.Add(this.lstTms);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(786, 534);
            this.panel2.TabIndex = 1;
            // 
            // rtbStatus
            // 
            this.rtbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbStatus.Location = new System.Drawing.Point(0, 284);
            this.rtbStatus.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbStatus.Name = "rtbStatus";
            this.rtbStatus.Size = new System.Drawing.Size(786, 250);
            this.rtbStatus.TabIndex = 1;
            this.rtbStatus.Text = "";
            // 
            // lstTms
            // 
            this.lstTms.AllowDrop = true;
            this.lstTms.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstTms.FormattingEnabled = true;
            this.lstTms.ItemHeight = 20;
            this.lstTms.Location = new System.Drawing.Point(0, 0);
            this.lstTms.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstTms.Name = "lstTms";
            this.lstTms.Size = new System.Drawing.Size(786, 284);
            this.lstTms.TabIndex = 0;
            this.lstTms.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstTms_DragDrop);
            this.lstTms.DragOver += new System.Windows.Forms.DragEventHandler(this.lstTms_DragOver);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(602, 9);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 35);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // cleanBtn
            // 
            this.cleanBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanBtn.Location = new System.Drawing.Point(520, 9);
            this.cleanBtn.Name = "cleanBtn";
            this.cleanBtn.Size = new System.Drawing.Size(75, 35);
            this.cleanBtn.TabIndex = 6;
            this.cleanBtn.Text = "Clean";
            this.cleanBtn.UseVisualStyleBackColor = true;
            this.cleanBtn.Click += new System.EventHandler(this.cleanBtn_Click);
            // 
            // ReindexForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ReindexForm";
            this.Size = new System.Drawing.Size(786, 589);
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
        private System.Windows.Forms.CheckBox upLiftCheckBox;
        private System.Windows.Forms.CheckBox reIndexCheckBox;
        private System.Windows.Forms.Button cleanBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}

