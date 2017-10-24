namespace Sdl.Community.ReportExporter
{
	partial class ReportExporterControl
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
			this.mainPanel = new System.Windows.Forms.Panel();
			this.copyBtn = new System.Windows.Forms.Button();
			this.languagesListBox = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.includeHeaderCheck = new System.Windows.Forms.CheckBox();
			this.projListbox = new System.Windows.Forms.ListBox();
			this.selectProjLbl = new System.Windows.Forms.Label();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.copyBtn);
			this.mainPanel.Controls.Add(this.languagesListBox);
			this.mainPanel.Controls.Add(this.label1);
			this.mainPanel.Controls.Add(this.includeHeaderCheck);
			this.mainPanel.Controls.Add(this.projListbox);
			this.mainPanel.Controls.Add(this.selectProjLbl);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(774, 501);
			this.mainPanel.TabIndex = 0;
			// 
			// copyBtn
			// 
			this.copyBtn.Location = new System.Drawing.Point(432, 64);
			this.copyBtn.Name = "copyBtn";
			this.copyBtn.Size = new System.Drawing.Size(151, 23);
			this.copyBtn.TabIndex = 8;
			this.copyBtn.Text = "Copy to clipboard";
			this.copyBtn.UseVisualStyleBackColor = true;
			this.copyBtn.Click += new System.EventHandler(this.copyBtn_Click);
			// 
			// languagesListBox
			// 
			this.languagesListBox.FormattingEnabled = true;
			this.languagesListBox.Location = new System.Drawing.Point(218, 64);
			this.languagesListBox.Name = "languagesListBox";
			this.languagesListBox.Size = new System.Drawing.Size(208, 349);
			this.languagesListBox.TabIndex = 7;
			this.languagesListBox.SelectedIndexChanged += new System.EventHandler(this.languagesListBox_SelectedIndexChanged_1);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(215, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Select language(s) for export:";
			// 
			// includeHeaderCheck
			// 
			this.includeHeaderCheck.AutoSize = true;
			this.includeHeaderCheck.Location = new System.Drawing.Point(218, 16);
			this.includeHeaderCheck.Name = "includeHeaderCheck";
			this.includeHeaderCheck.Size = new System.Drawing.Size(140, 17);
			this.includeHeaderCheck.TabIndex = 5;
			this.includeHeaderCheck.Text = "Include header in export";
			this.includeHeaderCheck.UseVisualStyleBackColor = true;
			this.includeHeaderCheck.CheckedChanged += new System.EventHandler(this.includeHeaderCheck_CheckedChanged);
			// 
			// projListbox
			// 
			this.projListbox.FormattingEnabled = true;
			this.projListbox.Location = new System.Drawing.Point(19, 16);
			this.projListbox.Name = "projListbox";
			this.projListbox.Size = new System.Drawing.Size(178, 394);
			this.projListbox.TabIndex = 1;
			this.projListbox.SelectedIndexChanged += new System.EventHandler(this.projListbox_SelectedIndexChanged);
			// 
			// selectProjLbl
			// 
			this.selectProjLbl.AutoSize = true;
			this.selectProjLbl.Location = new System.Drawing.Point(16, 0);
			this.selectProjLbl.Name = "selectProjLbl";
			this.selectProjLbl.Size = new System.Drawing.Size(75, 13);
			this.selectProjLbl.TabIndex = 0;
			this.selectProjLbl.Text = "Select project:";
			// 
			// ReportExporterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainPanel);
			this.Name = "ReportExporterControl";
			this.Size = new System.Drawing.Size(774, 501);
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label selectProjLbl;
		private System.Windows.Forms.ListBox projListbox;
		private System.Windows.Forms.CheckBox includeHeaderCheck;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckedListBox languagesListBox;
		private System.Windows.Forms.Button copyBtn;
	}
}
