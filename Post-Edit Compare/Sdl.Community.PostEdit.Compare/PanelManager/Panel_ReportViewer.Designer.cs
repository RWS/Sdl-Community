﻿namespace PostEdit.Compare
{
    partial class PanelReportViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelReportViewer));
            this.contextMenuStrip_DockContentMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Floating = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Dockable = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_AutoHide = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowserReport = new System.Windows.Forms.WebBrowser();
			this.webBrowserReport.ScriptErrorsSuppressed = true; 
            this.contextMenuStrip_DockContentMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip_DockContentMenu
            // 
            this.contextMenuStrip_DockContentMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Floating,
            this.ToolStripMenuItem_Dockable,
            this.ToolStripMenuItem_AutoHide});
            this.contextMenuStrip_DockContentMenu.Name = "contextMenuStrip1";
            this.contextMenuStrip_DockContentMenu.Size = new System.Drawing.Size(129, 70);
            // 
            // ToolStripMenuItem_Floating
            // 
            this.ToolStripMenuItem_Floating.Name = "ToolStripMenuItem_Floating";
            this.ToolStripMenuItem_Floating.Size = new System.Drawing.Size(128, 22);
            this.ToolStripMenuItem_Floating.Text = "Floating";
            this.ToolStripMenuItem_Floating.Click += new System.EventHandler(this.ToolStripMenuItem_Floating_Click);
            // 
            // ToolStripMenuItem_Dockable
            // 
            this.ToolStripMenuItem_Dockable.Name = "ToolStripMenuItem_Dockable";
            this.ToolStripMenuItem_Dockable.Size = new System.Drawing.Size(128, 22);
            this.ToolStripMenuItem_Dockable.Text = "Dockable";
            this.ToolStripMenuItem_Dockable.Click += new System.EventHandler(this.ToolStripMenuItem_Dockable_Click);
            // 
            // ToolStripMenuItem_AutoHide
            // 
            this.ToolStripMenuItem_AutoHide.Name = "ToolStripMenuItem_AutoHide";
            this.ToolStripMenuItem_AutoHide.Size = new System.Drawing.Size(128, 22);
            this.ToolStripMenuItem_AutoHide.Text = "Auto Hide";
            this.ToolStripMenuItem_AutoHide.Click += new System.EventHandler(this.ToolStripMenuItem_AutoHide_Click);
            // 
            // webBrowser_report
            // 
            this.webBrowserReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserReport.Location = new System.Drawing.Point(0, 0);
            this.webBrowserReport.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserReport.Name = "webBrowserReport";
            this.webBrowserReport.Size = new System.Drawing.Size(486, 468);
            this.webBrowserReport.TabIndex = 1;
            // 
            // Panel_ReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 468);
            this.CloseButton = false;
            this.Controls.Add(this.webBrowserReport);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PanelReportViewer";
            this.TabPageContextMenuStrip = this.contextMenuStrip_DockContentMenu;
            this.Text = "Report";
            this.contextMenuStrip_DockContentMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_DockContentMenu;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Floating;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Dockable;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_AutoHide;
        public System.Windows.Forms.WebBrowser webBrowserReport;
    }
}