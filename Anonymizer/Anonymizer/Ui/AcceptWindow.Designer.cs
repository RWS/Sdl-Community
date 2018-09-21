namespace Sdl.Community.projectAnonymizer.Ui
{
	partial class AcceptWindow
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.TableLayoutPanel mainPanel;

		private System.Windows.Forms.Label descriptionLabel;

		private System.Windows.Forms.CheckBox acceptBox;

		private System.Windows.Forms.Button acceptBtn;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AcceptWindow));
			this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
			this.descriptionLabel = new System.Windows.Forms.Label();
			this.acceptBox = new System.Windows.Forms.CheckBox();
			this.acceptBtn = new System.Windows.Forms.Button();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.ColumnCount = 2;
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.mainPanel.Controls.Add(this.descriptionLabel, 0, 0);
			this.mainPanel.Controls.Add(this.acceptBox, 0, 1);
			this.mainPanel.Controls.Add(this.acceptBtn, 1, 2);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.RowCount = 3;
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.mainPanel.Size = new System.Drawing.Size(419, 205);
			this.mainPanel.TabIndex = 0;
			// 
			// descriptionLabel
			// 
			this.descriptionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descriptionLabel.AutoSize = true;
			this.mainPanel.SetColumnSpan(this.descriptionLabel, 2);
			this.descriptionLabel.Location = new System.Drawing.Point(3, 44);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Size = new System.Drawing.Size(35, 13);
			this.descriptionLabel.TabIndex = 0;
			this.descriptionLabel.Text = "label1";
			// 
			// acceptBox
			// 
			this.acceptBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.acceptBox.AutoSize = true;
			this.acceptBox.Location = new System.Drawing.Point(3, 119);
			this.acceptBox.Name = "acceptBox";
			this.acceptBox.Size = new System.Drawing.Size(59, 17);
			this.acceptBox.TabIndex = 1;
			this.acceptBox.Text = "I agree";
			this.acceptBox.UseVisualStyleBackColor = true;
			this.acceptBox.CheckedChanged += new System.EventHandler(this.acceptBox_CheckedChanged);
			// 
			// acceptBtn
			// 
			this.acceptBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.acceptBtn.Location = new System.Drawing.Point(286, 167);
			this.acceptBtn.Margin = new System.Windows.Forms.Padding(50, 3, 3, 3);
			this.acceptBtn.Name = "acceptBtn";
			this.acceptBtn.Size = new System.Drawing.Size(103, 23);
			this.acceptBtn.TabIndex = 2;
			this.acceptBtn.Text = "Ok";
			this.acceptBtn.UseVisualStyleBackColor = true;
			this.acceptBtn.Click += new System.EventHandler(this.acceptBtn_Click);
			// 
			// AcceptWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(419, 205);
			this.Controls.Add(this.mainPanel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AcceptWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Project Anonymizer";
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
	}
}