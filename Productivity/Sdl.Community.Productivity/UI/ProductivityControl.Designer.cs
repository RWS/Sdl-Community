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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductivityControl));
            this.listView = new BrightIdeasSoftware.ObjectListView();
            this.segmentTextColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.insertedCharactersColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.efficiencyColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.keyStrokesSavedColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlbottom = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pbInfo1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tblScore = new System.Windows.Forms.TableLayoutPanel();
            this.pbTweetAccountImage = new System.Windows.Forms.PictureBox();
            this.lblScore = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.btnTweet = new Sdl.Community.Productivity.UI.RoundedButton();
            this.btnLeaderboard = new Sdl.Community.Productivity.UI.RoundedButton();
            this.btnScore = new Sdl.Community.Productivity.UI.RoundedButton();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlbottom.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).BeginInit();
            this.tblScore.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTweetAccountImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.AllColumns.Add(this.segmentTextColumn);
            this.listView.AllColumns.Add(this.insertedCharactersColumn);
            this.listView.AllColumns.Add(this.efficiencyColumn);
            this.listView.AllColumns.Add(this.keyStrokesSavedColumn);
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.segmentTextColumn,
            this.insertedCharactersColumn,
            this.efficiencyColumn,
            this.keyStrokesSavedColumn});
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.EmptyListMsg = "No information available";
            this.listView.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.SelectColumnsOnRightClick = false;
            this.listView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.listView.Size = new System.Drawing.Size(992, 368);
            this.listView.SortGroupItemsByPrimaryColumn = false;
            this.listView.SpaceBetweenGroups = 10;
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // segmentTextColumn
            // 
            this.segmentTextColumn.AspectName = "FileName";
            this.segmentTextColumn.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.segmentTextColumn.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(108)))), ((int)(((byte)(122)))));
            this.segmentTextColumn.Hideable = false;
            this.segmentTextColumn.Hyperlink = true;
            this.segmentTextColumn.IsEditable = false;
            this.segmentTextColumn.Text = "Project";
            this.segmentTextColumn.Width = 367;
            // 
            // insertedCharactersColumn
            // 
            this.insertedCharactersColumn.AspectName = "InsertedCharacters";
            this.insertedCharactersColumn.AspectToStringFormat = "{0:n0}";
            this.insertedCharactersColumn.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.insertedCharactersColumn.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(108)))), ((int)(((byte)(122)))));
            this.insertedCharactersColumn.Text = "Inserted Characters";
            this.insertedCharactersColumn.Width = 217;
            // 
            // efficiencyColumn
            // 
            this.efficiencyColumn.AspectName = "Efficiency";
            this.efficiencyColumn.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.efficiencyColumn.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(108)))), ((int)(((byte)(122)))));
            this.efficiencyColumn.MinimumWidth = 109;
            this.efficiencyColumn.Text = "Efficiency";
            this.efficiencyColumn.Width = 170;
            // 
            // keyStrokesSavedColumn
            // 
            this.keyStrokesSavedColumn.AspectName = "KeystrokesSaved";
            this.keyStrokesSavedColumn.AspectToStringFormat = "{0:n0}";
            this.keyStrokesSavedColumn.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.keyStrokesSavedColumn.HeaderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(108)))), ((int)(((byte)(122)))));
            this.keyStrokesSavedColumn.Text = "Keystrokes saved";
            this.keyStrokesSavedColumn.Width = 224;
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(220)))), ((int)(((byte)(222)))));
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(992, 448);
            this.pnlMain.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnlbottom);
            this.panel2.Controls.Add(this.listView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 80);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(992, 368);
            this.panel2.TabIndex = 2;
            // 
            // pnlbottom
            // 
            this.pnlbottom.Controls.Add(this.btnTweet);
            this.pnlbottom.Controls.Add(this.btnLeaderboard);
            this.pnlbottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlbottom.Location = new System.Drawing.Point(0, 324);
            this.pnlbottom.Name = "pnlbottom";
            this.pnlbottom.Size = new System.Drawing.Size(992, 44);
            this.pnlbottom.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Controls.Add(this.tblScore);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(992, 80);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 81.15942F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18.84058F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Controls.Add(this.btnScore, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pbInfo1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(546, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(446, 80);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // pbInfo1
            // 
            this.pbInfo1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbInfo1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbInfo1.Image = global::Sdl.Community.Productivity.PluginResources.info;
            this.pbInfo1.Location = new System.Drawing.Point(419, 53);
            this.pbInfo1.Name = "pbInfo1";
            this.pbInfo1.Size = new System.Drawing.Size(24, 24);
            this.pbInfo1.TabIndex = 8;
            this.pbInfo1.TabStop = false;
            this.toolTip1.SetToolTip(this.pbInfo1, resources.GetString("pbInfo1.ToolTip"));
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(139)))), ((int)(((byte)(141)))));
            this.label1.Location = new System.Drawing.Point(29, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(302, 40);
            this.label1.TabIndex = 3;
            this.label1.Text = "total translation efficiency ";
            // 
            // tblScore
            // 
            this.tblScore.ColumnCount = 3;
            this.tblScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.51313F));
            this.tblScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84.48687F));
            this.tblScore.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblScore.Controls.Add(this.pbTweetAccountImage, 0, 0);
            this.tblScore.Controls.Add(this.lblScore, 1, 0);
            this.tblScore.Controls.Add(this.pictureBox1, 2, 0);
            this.tblScore.Dock = System.Windows.Forms.DockStyle.Left;
            this.tblScore.Location = new System.Drawing.Point(0, 0);
            this.tblScore.Name = "tblScore";
            this.tblScore.RowCount = 1;
            this.tblScore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblScore.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tblScore.Size = new System.Drawing.Size(450, 80);
            this.tblScore.TabIndex = 10;
            // 
            // pbTweetAccountImage
            // 
            this.pbTweetAccountImage.Cursor = System.Windows.Forms.Cursors.Default;
            this.pbTweetAccountImage.Image = global::Sdl.Community.Productivity.PluginResources.cup_48;
            this.pbTweetAccountImage.Location = new System.Drawing.Point(3, 3);
            this.pbTweetAccountImage.Name = "pbTweetAccountImage";
            this.pbTweetAccountImage.Size = new System.Drawing.Size(48, 48);
            this.pbTweetAccountImage.TabIndex = 2;
            this.pbTweetAccountImage.TabStop = false;
            // 
            // lblScore
            // 
            this.lblScore.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblScore.AutoSize = true;
            this.lblScore.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(139)))), ((int)(((byte)(141)))));
            this.lblScore.Location = new System.Drawing.Point(68, 11);
            this.lblScore.Name = "lblScore";
            this.lblScore.Size = new System.Drawing.Size(337, 58);
            this.lblScore.TabIndex = 1;
            this.lblScore.Text = "Romulus Crisan, your score is:\r\n222,222,222 points!";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::Sdl.Community.Productivity.PluginResources.info;
            this.pictureBox1.Location = new System.Drawing.Point(422, 53);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(23, 24);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.toolTip2.SetToolTip(this.pictureBox1, "Leaderboard score:\r\nCompete against translators from around the world for prizes " +
        "and prestige!  Earn points for keeping a high efficiency percentage in all of yo" +
        "ur completed projects.");
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 32767;
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(220)))), ((int)(((byte)(222)))));
            this.toolTip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(139)))), ((int)(((byte)(141)))));
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // toolTip2
            // 
            this.toolTip2.AutoPopDelay = 32767;
            this.toolTip2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(220)))), ((int)(((byte)(222)))));
            this.toolTip2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(139)))), ((int)(((byte)(141)))));
            this.toolTip2.InitialDelay = 500;
            this.toolTip2.IsBalloon = true;
            this.toolTip2.ReshowDelay = 100;
            this.toolTip2.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // btnTweet
            // 
            this.btnTweet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTweet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(121)))), ((int)(((byte)(197)))));
            this.btnTweet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTweet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTweet.ForeColor = System.Drawing.Color.White;
            this.btnTweet.Location = new System.Drawing.Point(666, 3);
            this.btnTweet.Name = "btnTweet";
            this.btnTweet.Radius = 1;
            this.btnTweet.Size = new System.Drawing.Size(157, 36);
            this.btnTweet.TabIndex = 4;
            this.btnTweet.Text = "Tweet";
            this.btnTweet.UseVisualStyleBackColor = false;
            this.btnTweet.Click += new System.EventHandler(this.btnTweet_Click);
            // 
            // btnLeaderboard
            // 
            this.btnLeaderboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeaderboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(121)))), ((int)(((byte)(197)))));
            this.btnLeaderboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLeaderboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLeaderboard.ForeColor = System.Drawing.Color.White;
            this.btnLeaderboard.Location = new System.Drawing.Point(832, 3);
            this.btnLeaderboard.Name = "btnLeaderboard";
            this.btnLeaderboard.Radius = 1;
            this.btnLeaderboard.Size = new System.Drawing.Size(157, 36);
            this.btnLeaderboard.TabIndex = 5;
            this.btnLeaderboard.Text = "Leaderboard";
            this.btnLeaderboard.UseVisualStyleBackColor = false;
            this.btnLeaderboard.Click += new System.EventHandler(this.btnLeaderboard_Click);
            // 
            // btnScore
            // 
            this.btnScore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(121)))), ((int)(((byte)(197)))));
            this.btnScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScore.ForeColor = System.Drawing.Color.White;
            this.btnScore.Location = new System.Drawing.Point(338, 3);
            this.btnScore.Name = "btnScore";
            this.btnScore.Radius = 70;
            this.btnScore.Size = new System.Drawing.Size(70, 70);
            this.btnScore.TabIndex = 7;
            this.btnScore.Text = "100%";
            this.btnScore.UseVisualStyleBackColor = false;
            // 
            // ProductivityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Name = "ProductivityControl";
            this.Size = new System.Drawing.Size(992, 448);
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlbottom.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo1)).EndInit();
            this.tblScore.ResumeLayout(false);
            this.tblScore.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTweetAccountImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.ObjectListView listView;
        private BrightIdeasSoftware.OLVColumn segmentTextColumn;
        private BrightIdeasSoftware.OLVColumn insertedCharactersColumn;
        private BrightIdeasSoftware.OLVColumn efficiencyColumn;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel1;
        private RoundedButton btnTweet;
        private RoundedButton btnLeaderboard;
        private System.Windows.Forms.Label label1;
        private RoundedButton btnScore;
        private BrightIdeasSoftware.OLVColumn keyStrokesSavedColumn;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.PictureBox pbInfo1;
        private System.Windows.Forms.Panel pnlbottom;
        private System.Windows.Forms.TableLayoutPanel tblScore;
        private System.Windows.Forms.PictureBox pbTweetAccountImage;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;


    }
}
