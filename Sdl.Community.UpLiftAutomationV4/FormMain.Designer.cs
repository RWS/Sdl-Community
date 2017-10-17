

namespace Sdl.Community.FragmentAlignmentAutomation
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStartProcessing = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.textBoxMain = new System.Windows.Forms.TextBox();
            this.StatusBarStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusBarProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusBarPercentage = new System.Windows.Forms.ToolStripStatusLabel();
            this.listViewMain = new FragmentAlignmentAutomation.Controls.DoubleBufferedListView();
            this.columnInputTm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnOutputTm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnMessage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(910, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.optionsToolStripMenuItem.Text = "&Tools";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStartProcessing,
            this.toolStripSeparator1,
            this.toolStripButtonSettings,
            this.toolStripSeparator2,
            this.toolStripButtonAbout});
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(910, 39);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // toolStripButtonStartProcessing
            // 
            this.toolStripButtonStartProcessing.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonStartProcessing.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStartProcessing.Image")));
            this.toolStripButtonStartProcessing.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonStartProcessing.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStartProcessing.Name = "toolStripButtonStartProcessing";
            this.toolStripButtonStartProcessing.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonStartProcessing.Text = "Start Processing";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSettings.Enabled = false;
            this.toolStripButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSettings.Image")));
            this.toolStripButtonSettings.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonSettings.Text = "Settings";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbout.Image")));
            this.toolStripButtonAbout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonAbout.Text = "About";
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusBarStatusLabel,
            this.StatusBarProgressBar,
            this.StatusBarPercentage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 449);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(910, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 63);
            this.splitContainerMain.Name = "splitContainerMain";
            this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.listViewMain);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.textBoxMain);
            this.splitContainerMain.Size = new System.Drawing.Size(910, 386);
            this.splitContainerMain.SplitterDistance = 287;
            this.splitContainerMain.TabIndex = 3;
            // 
            // textBoxMain
            // 
            this.textBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxMain.Location = new System.Drawing.Point(0, 0);
            this.textBoxMain.MinimumSize = new System.Drawing.Size(80, 4);
            this.textBoxMain.Multiline = true;
            this.textBoxMain.Name = "textBoxMain";
            this.textBoxMain.ReadOnly = true;
            this.textBoxMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxMain.Size = new System.Drawing.Size(910, 95);
            this.textBoxMain.TabIndex = 0;
            // 
            // StatusBarStatusLabel
            // 
            this.StatusBarStatusLabel.Name = "StatusBarStatusLabel";
            this.StatusBarStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.StatusBarStatusLabel.Text = "Ready";
            // 
            // StatusBarProgressBar
            // 
            this.StatusBarProgressBar.Name = "StatusBarProgressBar";
            this.StatusBarProgressBar.Size = new System.Drawing.Size(200, 16);
            // 
            // StatusBarPercentage
            // 
            this.StatusBarPercentage.Name = "StatusBarPercentage";
            this.StatusBarPercentage.Size = new System.Drawing.Size(23, 17);
            this.StatusBarPercentage.Text = "0%";
            // 
            // listViewMain
            // 
            this.listViewMain.AllowDrop = true;
            this.listViewMain.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnInputTm,
            this.columnOutputTm,
            this.columnProgress,
            this.columnMessage});
            this.listViewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewMain.FullRowSelect = true;
            this.listViewMain.GridLines = true;
            this.listViewMain.Location = new System.Drawing.Point(0, 0);
            this.listViewMain.Name = "listViewMain";
            this.listViewMain.Size = new System.Drawing.Size(910, 287);
            this.listViewMain.TabIndex = 0;
            this.listViewMain.UseCompatibleStateImageBehavior = false;
            this.listViewMain.View = System.Windows.Forms.View.Details;
            // 
            // columnInputTm
            // 
            this.columnInputTm.Text = "TM Path (Input)";
            this.columnInputTm.Width = 300;
            // 
            // columnOutputTm
            // 
            this.columnOutputTm.Text = "TM Path (Output)";
            this.columnOutputTm.Width = 300;
            // 
            // columnProgress
            // 
            this.columnProgress.Text = "Progress";
            // 
            // columnMessage
            // 
            this.columnMessage.Text = "Message";
            this.columnMessage.Width = 210;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 471);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fragment Alignment Automation";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            this.splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonStartProcessing;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.TextBox textBoxMain;
        private FragmentAlignmentAutomation.Controls.DoubleBufferedListView listViewMain;
        private System.Windows.Forms.ColumnHeader columnInputTm;
        private System.Windows.Forms.ColumnHeader columnProgress;
        private System.Windows.Forms.ColumnHeader columnOutputTm;
        private System.Windows.Forms.ColumnHeader columnMessage;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar StatusBarProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel StatusBarPercentage;
    }
}

