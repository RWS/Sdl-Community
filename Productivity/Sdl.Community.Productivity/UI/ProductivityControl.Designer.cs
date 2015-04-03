namespace Sdl.Community.Productivity.UI
{
    partial class ProductivityControl
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
            this.listView = new BrightIdeasSoftware.ObjectListView();
            this.segmentTextColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.isTranslatedColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.segmentProductivityScoreColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlScore = new System.Windows.Forms.Panel();
            this.pbScore = new System.Windows.Forms.PictureBox();
            this.lblScore = new System.Windows.Forms.Label();
            this.lblProductivityScore = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlScore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbScore)).BeginInit();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.AllColumns.Add(this.segmentTextColumn);
            this.listView.AllColumns.Add(this.isTranslatedColumn);
            this.listView.AllColumns.Add(this.segmentProductivityScoreColumn);
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.segmentTextColumn,
            this.isTranslatedColumn,
            this.segmentProductivityScoreColumn});
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.EmptyListMsg = "No information available";
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.SelectColumnsOnRightClick = false;
            this.listView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.listView.Size = new System.Drawing.Size(992, 284);
            this.listView.SortGroupItemsByPrimaryColumn = false;
            this.listView.SpaceBetweenGroups = 10;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // segmentTextColumn
            // 
            this.segmentTextColumn.AspectName = "Text";
            this.segmentTextColumn.FillsFreeSpace = true;
            this.segmentTextColumn.Hideable = false;
            this.segmentTextColumn.Hyperlink = true;
            this.segmentTextColumn.IsEditable = false;
            this.segmentTextColumn.Text = "Text";
            // 
            // isTranslatedColumn
            // 
            this.isTranslatedColumn.AspectName = "Translated";
            this.isTranslatedColumn.Groupable = false;
            this.isTranslatedColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.isTranslatedColumn.Text = "Translated";
            this.isTranslatedColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.isTranslatedColumn.Width = 130;
            // 
            // segmentProductivityScoreColumn
            // 
            this.segmentProductivityScoreColumn.AspectName = "SegmentProductivityScore";
            this.segmentProductivityScoreColumn.Groupable = false;
            this.segmentProductivityScoreColumn.HeaderTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.segmentProductivityScoreColumn.Text = "Productivity Score";
            this.segmentProductivityScoreColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.segmentProductivityScoreColumn.Width = 109;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.SystemColors.Window;
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(992, 352);
            this.pnlMain.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 68);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(992, 284);
            this.panel2.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlScore);
            this.panel1.Controls.Add(this.lblProductivityScore);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(992, 68);
            this.panel1.TabIndex = 1;
            // 
            // pnlScore
            // 
            this.pnlScore.Controls.Add(this.pbScore);
            this.pnlScore.Controls.Add(this.lblScore);
            this.pnlScore.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlScore.Location = new System.Drawing.Point(3, 14);
            this.pnlScore.Name = "pnlScore";
            this.pnlScore.Size = new System.Drawing.Size(233, 48);
            this.pnlScore.TabIndex = 3;
            // 
            // pbScore
            // 
            this.pbScore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbScore.Image = global::Sdl.Community.Productivity.PluginResources.cup_48;
            this.pbScore.Location = new System.Drawing.Point(3, 0);
            this.pbScore.Name = "pbScore";
            this.pbScore.Size = new System.Drawing.Size(48, 48);
            this.pbScore.TabIndex = 2;
            this.pbScore.TabStop = false;
            this.pbScore.Click += new System.EventHandler(this.pbScore_Click);
            // 
            // lblScore
            // 
            this.lblScore.AutoSize = true;
            this.lblScore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblScore.Location = new System.Drawing.Point(57, 9);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(97, 29);
            this.lblScore.TabIndex = 1;
            this.lblScore.Text = "240987";
            this.lblScore.Click += new System.EventHandler(this.lblScore_Click);
            // 
            // lblProductivityScore
            // 
            this.lblProductivityScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProductivityScore.AutoSize = true;
            this.lblProductivityScore.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblProductivityScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProductivityScore.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblProductivityScore.Location = new System.Drawing.Point(707, 23);
            this.lblProductivityScore.Name = "lblProductivityScore";
            this.lblProductivityScore.Size = new System.Drawing.Size(282, 29);
            this.lblProductivityScore.TabIndex = 0;
            this.lblProductivityScore.Text = "Productivity Score 87%";
            this.lblProductivityScore.Click += new System.EventHandler(this.lblProductivityScore_Click);
            // 
            // ProductivityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Name = "ProductivityControl";
            this.Size = new System.Drawing.Size(992, 352);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlScore.ResumeLayout(false);
            this.pnlScore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbScore)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView listView;
        private BrightIdeasSoftware.OLVColumn segmentTextColumn;
        private BrightIdeasSoftware.OLVColumn isTranslatedColumn;
        private BrightIdeasSoftware.OLVColumn segmentProductivityScoreColumn;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblProductivityScore;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.PictureBox pbScore;
        private System.Windows.Forms.Panel pnlScore;


    }
}
