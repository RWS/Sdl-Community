namespace Sdl.Community.PostEdit.Compare
{
	partial class SkipSettingsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkipSettingsWindow));
            this.skipWindowPanel = new System.Windows.Forms.TableLayoutPanel();
            this.skipWindowLabel = new System.Windows.Forms.Label();
            this.buttonsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.skippBtn = new System.Windows.Forms.Button();
            this.reportBtn = new System.Windows.Forms.Button();
            this.skipWindowPanel.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // skipWindowPanel
            // 
            this.skipWindowPanel.ColumnCount = 1;
            this.skipWindowPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.skipWindowPanel.Controls.Add(this.skipWindowLabel, 0, 0);
            this.skipWindowPanel.Controls.Add(this.buttonsPanel, 0, 1);
            this.skipWindowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skipWindowPanel.Location = new System.Drawing.Point(0, 0);
            this.skipWindowPanel.Name = "skipWindowPanel";
            this.skipWindowPanel.RowCount = 2;
            this.skipWindowPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.skipWindowPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.skipWindowPanel.Size = new System.Drawing.Size(663, 471);
            this.skipWindowPanel.TabIndex = 0;
            // 
            // skipWindowLabel
            // 
            this.skipWindowLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.skipWindowLabel.AutoSize = true;
            this.skipWindowLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.skipWindowLabel.Location = new System.Drawing.Point(14, 152);
            this.skipWindowLabel.Name = "skipWindowLabel";
            this.skipWindowLabel.Size = new System.Drawing.Size(635, 25);
            this.skipWindowLabel.TabIndex = 0;
            this.skipWindowLabel.Text = "Please select if you want to customize the report or skip the options step.";
            this.skipWindowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.ColumnCount = 2;
            this.buttonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsPanel.Controls.Add(this.skippBtn, 0, 0);
            this.buttonsPanel.Controls.Add(this.reportBtn, 1, 0);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsPanel.Location = new System.Drawing.Point(3, 332);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.RowCount = 1;
            this.buttonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.buttonsPanel.Size = new System.Drawing.Size(657, 136);
            this.buttonsPanel.TabIndex = 1;
            // 
            // skippBtn
            // 
            this.skippBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.skippBtn.Location = new System.Drawing.Point(63, 35);
            this.skippBtn.Name = "skippBtn";
            this.skippBtn.Size = new System.Drawing.Size(201, 65);
            this.skippBtn.TabIndex = 0;
            this.skippBtn.Text = "Skip report settings";
            this.skippBtn.UseVisualStyleBackColor = true;
            // 
            // reportBtn
            // 
            this.reportBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.reportBtn.Location = new System.Drawing.Point(392, 35);
            this.reportBtn.Name = "reportBtn";
            this.reportBtn.Size = new System.Drawing.Size(201, 65);
            this.reportBtn.TabIndex = 1;
            this.reportBtn.Text = "Set report settings";
            this.reportBtn.UseVisualStyleBackColor = true;
            // 
            // SkipSettingsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 471);
            this.Controls.Add(this.skipWindowPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SkipSettingsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.skipWindowPanel.ResumeLayout(false);
            this.skipWindowPanel.PerformLayout();
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel skipWindowPanel;
		private System.Windows.Forms.Label skipWindowLabel;
		private System.Windows.Forms.TableLayoutPanel buttonsPanel;
		private System.Windows.Forms.Button skippBtn;
		private System.Windows.Forms.Button reportBtn;
	}
}