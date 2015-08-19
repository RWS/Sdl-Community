namespace Sdl.Community.YourProductivity.UI
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
            this.btnTweet = new Sdl.Community.YourProductivity.UI.RoundedButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtTweetMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnTweet, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 136);
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
            // btnTweet
            // 
            this.btnTweet.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnTweet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(121)))), ((int)(((byte)(197)))));
            this.btnTweet.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTweet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTweet.ForeColor = System.Drawing.Color.White;
            this.btnTweet.Location = new System.Drawing.Point(221, 91);
            this.btnTweet.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnTweet.Name = "btnTweet";
            this.btnTweet.Radius = 1;
            this.btnTweet.Size = new System.Drawing.Size(157, 36);
            this.btnTweet.TabIndex = 5;
            this.btnTweet.Text = "Tweet";
            this.btnTweet.UseVisualStyleBackColor = false;
            this.btnTweet.Click += new System.EventHandler(this.btnTweet_Click);
            // 
            // TweetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(220)))), ((int)(((byte)(222)))));
            this.ClientSize = new System.Drawing.Size(600, 136);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TweetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Share #YourProductivity score";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox txtTweetMessage;
        private RoundedButton btnTweet;
    }
}