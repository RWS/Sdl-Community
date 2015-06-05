namespace Sdl.Community.Productivity.UI
{
    partial class TweetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TweetForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtTweetMessage = new System.Windows.Forms.TextBox();
            this.shareTweet = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shareTweet)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.txtTweetMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.shareTweet, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 173);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtTweetMessage
            // 
            this.txtTweetMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTweetMessage.Location = new System.Drawing.Point(3, 3);
            this.txtTweetMessage.Multiline = true;
            this.txtTweetMessage.Name = "txtTweetMessage";
            this.txtTweetMessage.Size = new System.Drawing.Size(594, 80);
            this.txtTweetMessage.TabIndex = 0;
            this.txtTweetMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // shareTweet
            // 
            this.shareTweet.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.shareTweet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.shareTweet.Image = global::Sdl.Community.Productivity.PluginResources.twitter_48;
            this.shareTweet.InitialImage = global::Sdl.Community.Productivity.PluginResources.twitter_48;
            this.shareTweet.Location = new System.Drawing.Point(272, 120);
            this.shareTweet.Name = "shareTweet";
            this.shareTweet.Size = new System.Drawing.Size(55, 50);
            this.shareTweet.TabIndex = 1;
            this.shareTweet.TabStop = false;
            this.shareTweet.Click += new System.EventHandler(this.shareTweet_Click);
            // 
            // TweetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 173);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TweetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Share on twitter";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shareTweet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtTweetMessage;
        private System.Windows.Forms.PictureBox shareTweet;
    }
}