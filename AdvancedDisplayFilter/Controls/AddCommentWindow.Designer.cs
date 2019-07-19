namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Controls
{
	partial class AddCommentWindow
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddCommentWindow));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.okBtn = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.commentTextBox = new System.Windows.Forms.TextBox();
			this.severityBox = new System.Windows.Forms.ComboBox();
			this.severityLbl = new System.Windows.Forms.Label();
			this.mainPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.okBtn);
			this.mainPanel.Controls.Add(this.groupBox1);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(484, 261);
			this.mainPanel.TabIndex = 0;
			// 
			// okBtn
			// 
			this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okBtn.Location = new System.Drawing.Point(397, 226);
			this.okBtn.Name = "okBtn";
			this.okBtn.Size = new System.Drawing.Size(75, 23);
			this.okBtn.TabIndex = 1;
			this.okBtn.Text = "OK";
			this.okBtn.UseVisualStyleBackColor = true;
			this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.commentTextBox);
			this.groupBox1.Controls.Add(this.severityBox);
			this.groupBox1.Controls.Add(this.severityLbl);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(460, 195);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Comment information";
			// 
			// commentTextBox
			// 
			this.commentTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.commentTextBox.Location = new System.Drawing.Point(9, 54);
			this.commentTextBox.Multiline = true;
			this.commentTextBox.Name = "commentTextBox";
			this.commentTextBox.Size = new System.Drawing.Size(445, 135);
			this.commentTextBox.TabIndex = 2;
			// 
			// severityBox
			// 
			this.severityBox.FormattingEnabled = true;
			this.severityBox.Location = new System.Drawing.Point(100, 20);
			this.severityBox.Name = "severityBox";
			this.severityBox.Size = new System.Drawing.Size(354, 21);
			this.severityBox.TabIndex = 1;
			// 
			// severityLbl
			// 
			this.severityLbl.AutoSize = true;
			this.severityLbl.Location = new System.Drawing.Point(6, 23);
			this.severityLbl.Name = "severityLbl";
			this.severityLbl.Size = new System.Drawing.Size(70, 13);
			this.severityLbl.TabIndex = 0;
			this.severityLbl.Text = "Severity level";
			// 
			// AddCommentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 261);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddCommentWindow";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Add Comment";
			this.mainPanel.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Button okBtn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox severityBox;
		private System.Windows.Forms.Label severityLbl;
		private System.Windows.Forms.TextBox commentTextBox;
	}
}