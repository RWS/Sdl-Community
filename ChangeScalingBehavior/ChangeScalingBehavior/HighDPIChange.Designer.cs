namespace ChangeScalingBehavior
{
    partial class HighDPIChange
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HighDPIChange));
            this.objectListView = new BrightIdeasSoftware.ObjectListView();
            this.olvApplicationVersions = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.olvColumnAction = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).BeginInit();
            this.SuspendLayout();
            // 
            // objectListView
            // 
            this.objectListView.AllColumns.Add(this.olvApplicationVersions);
            this.objectListView.AllColumns.Add(this.olvColumnAction);
            this.objectListView.CellEditUseWholeCell = false;
            this.objectListView.CellPadding = new System.Drawing.Rectangle(4, 2, 4, 2);
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvApplicationVersions,
            this.olvColumnAction});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView.HasCollapsibleGroups = false;
            this.objectListView.Location = new System.Drawing.Point(0, 0);
            this.objectListView.Name = "objectListView";
            this.objectListView.RowHeight = 40;
            this.objectListView.Size = new System.Drawing.Size(384, 375);
            this.objectListView.TabIndex = 10;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            // 
            // olvApplicationVersions
            // 
            this.olvApplicationVersions.AspectName = "PublicVersion";
            this.olvApplicationVersions.Groupable = false;
            this.olvApplicationVersions.Text = "Application Versions";
            this.olvApplicationVersions.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvApplicationVersions.Width = 155;
            // 
            // olvColumnAction
            // 
            this.olvColumnAction.AspectName = "";
            this.olvColumnAction.ButtonSizing = BrightIdeasSoftware.OLVColumn.ButtonSizingMode.CellBounds;
            this.olvColumnAction.Groupable = false;
            this.olvColumnAction.IsButton = true;
            this.olvColumnAction.Text = "Action";
            this.olvColumnAction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.olvColumnAction.Width = 192;
            // 
            // HighDPIChange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 375);
            this.Controls.Add(this.objectListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "HighDPIChange";
            this.Text = "Change Scaling Behavior";
            this.Load += new System.EventHandler(this.HighDPIChange_Load);
            ((System.ComponentModel.ISupportInitialize)(this.objectListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private BrightIdeasSoftware.ObjectListView objectListView;
        private BrightIdeasSoftware.OLVColumn olvApplicationVersions;
        private BrightIdeasSoftware.OLVColumn olvColumnAction;
    }
}

