namespace Sdl.Community.ExportAnalysisReports
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportExporterControl));
			this.mainPanel = new System.Windows.Forms.Panel();
			this.chkBox_IncludeSingleFileProjects = new System.Windows.Forms.CheckBox();
			this.chkBox_SelectAllLanguages = new System.Windows.Forms.CheckBox();
			this.chkBox_SelectAllProjects = new System.Windows.Forms.CheckBox();
			this.clearBtn = new System.Windows.Forms.Button();
			this.loadBtn = new System.Windows.Forms.Button();
			this.projectStatusComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.browseBtn = new System.Windows.Forms.Button();
			this.reportOutputPath = new System.Windows.Forms.TextBox();
			this.crossRep = new System.Windows.Forms.CheckBox();
			this.contextMatch = new System.Windows.Forms.CheckBox();
			this.perfectMatch = new System.Windows.Forms.CheckBox();
			this.locked = new System.Windows.Forms.CheckBox();
			this.internalFuzzies = new System.Windows.Forms.CheckBox();
			this.adaptiveMT = new System.Windows.Forms.CheckBox();
			this.adaptiveLearnings = new System.Windows.Forms.CheckBox();
			this.optionsLabel = new System.Windows.Forms.Label();
			this.exitBtn = new System.Windows.Forms.Button();
			this.targetBtn = new System.Windows.Forms.Button();
			this.csvBtn = new System.Windows.Forms.Button();
			this.copyBtn = new System.Windows.Forms.Button();
			this.languagesListBox = new System.Windows.Forms.CheckedListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.includeHeaderCheck = new System.Windows.Forms.CheckBox();
			this.projListbox = new System.Windows.Forms.CheckedListBox();
			this.selectProjLbl = new System.Windows.Forms.Label();
			this.mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.Controls.Add(this.chkBox_IncludeSingleFileProjects);
			this.mainPanel.Controls.Add(this.chkBox_SelectAllLanguages);
			this.mainPanel.Controls.Add(this.chkBox_SelectAllProjects);
			this.mainPanel.Controls.Add(this.clearBtn);
			this.mainPanel.Controls.Add(this.loadBtn);
			this.mainPanel.Controls.Add(this.projectStatusComboBox);
			this.mainPanel.Controls.Add(this.label3);
			this.mainPanel.Controls.Add(this.label2);
			this.mainPanel.Controls.Add(this.browseBtn);
			this.mainPanel.Controls.Add(this.reportOutputPath);
			this.mainPanel.Controls.Add(this.crossRep);
			this.mainPanel.Controls.Add(this.contextMatch);
			this.mainPanel.Controls.Add(this.perfectMatch);
			this.mainPanel.Controls.Add(this.locked);
			this.mainPanel.Controls.Add(this.internalFuzzies);
			this.mainPanel.Controls.Add(this.adaptiveMT);
			this.mainPanel.Controls.Add(this.adaptiveLearnings);
			this.mainPanel.Controls.Add(this.optionsLabel);
			this.mainPanel.Controls.Add(this.exitBtn);
			this.mainPanel.Controls.Add(this.targetBtn);
			this.mainPanel.Controls.Add(this.csvBtn);
			this.mainPanel.Controls.Add(this.copyBtn);
			this.mainPanel.Controls.Add(this.languagesListBox);
			this.mainPanel.Controls.Add(this.label1);
			this.mainPanel.Controls.Add(this.includeHeaderCheck);
			this.mainPanel.Controls.Add(this.projListbox);
			this.mainPanel.Controls.Add(this.selectProjLbl);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(811, 455);
			this.mainPanel.TabIndex = 0;
			// 
			// chkBox_IncludeSingleFileProjects
			// 
			this.chkBox_IncludeSingleFileProjects.AutoSize = true;
			this.chkBox_IncludeSingleFileProjects.Location = new System.Drawing.Point(433, 78);
			this.chkBox_IncludeSingleFileProjects.Name = "chkBox_IncludeSingleFileProjects";
			this.chkBox_IncludeSingleFileProjects.Size = new System.Drawing.Size(147, 17);
			this.chkBox_IncludeSingleFileProjects.TabIndex = 30;
			this.chkBox_IncludeSingleFileProjects.Text = "Include single file projects";
			this.chkBox_IncludeSingleFileProjects.UseVisualStyleBackColor = true;
			this.chkBox_IncludeSingleFileProjects.CheckedChanged += new System.EventHandler(this.IncludeSingleFileProjects_CheckedChanged);
			// 
			// chkBox_SelectAllLanguages
			// 
			this.chkBox_SelectAllLanguages.AutoSize = true;
			this.chkBox_SelectAllLanguages.Location = new System.Drawing.Point(249, 29);
			this.chkBox_SelectAllLanguages.Name = "chkBox_SelectAllLanguages";
			this.chkBox_SelectAllLanguages.Size = new System.Drawing.Size(121, 17);
			this.chkBox_SelectAllLanguages.TabIndex = 29;
			this.chkBox_SelectAllLanguages.Text = "Select all languages";
			this.chkBox_SelectAllLanguages.UseVisualStyleBackColor = true;
			this.chkBox_SelectAllLanguages.CheckedChanged += new System.EventHandler(this.SelectAllLanguages_CheckedChanged);
			// 
			// chkBox_SelectAllProjects
			// 
			this.chkBox_SelectAllProjects.AutoSize = true;
			this.chkBox_SelectAllProjects.Location = new System.Drawing.Point(15, 29);
			this.chkBox_SelectAllProjects.Name = "chkBox_SelectAllProjects";
			this.chkBox_SelectAllProjects.Size = new System.Drawing.Size(109, 17);
			this.chkBox_SelectAllProjects.TabIndex = 28;
			this.chkBox_SelectAllProjects.Text = "Select all projects";
			this.chkBox_SelectAllProjects.UseVisualStyleBackColor = true;
			this.chkBox_SelectAllProjects.CheckedChanged += new System.EventHandler(this.SelectAllProjects_CheckedChanged);
			// 
			// clearBtn
			// 
			this.clearBtn.Location = new System.Drawing.Point(690, 253);
			this.clearBtn.Name = "clearBtn";
			this.clearBtn.Size = new System.Drawing.Size(106, 43);
			this.clearBtn.TabIndex = 27;
			this.clearBtn.Text = "Clear external / Refresh projects";
			this.clearBtn.UseVisualStyleBackColor = true;
			this.clearBtn.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// loadBtn
			// 
			this.loadBtn.Location = new System.Drawing.Point(690, 224);
			this.loadBtn.Name = "loadBtn";
			this.loadBtn.Size = new System.Drawing.Size(105, 23);
			this.loadBtn.TabIndex = 26;
			this.loadBtn.Text = "Load external ";
			this.loadBtn.UseVisualStyleBackColor = true;
			this.loadBtn.Click += new System.EventHandler(this.LoadBtn_Click);
			// 
			// projectStatusComboBox
			// 
			this.projectStatusComboBox.FormattingEnabled = true;
			this.projectStatusComboBox.Items.AddRange(new object[] {
            "All",
            "InProgress",
            "Completed"});
			this.projectStatusComboBox.Location = new System.Drawing.Point(432, 133);
			this.projectStatusComboBox.Name = "projectStatusComboBox";
			this.projectStatusComboBox.Size = new System.Drawing.Size(121, 21);
			this.projectStatusComboBox.TabIndex = 25;
			this.projectStatusComboBox.SelectedIndexChanged += new System.EventHandler(this.ProjectStatusComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(432, 117);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 13);
			this.label3.TabIndex = 24;
			this.label3.Text = "Project status";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(430, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 13);
			this.label2.TabIndex = 23;
			this.label2.Text = "Report output path";
			// 
			// browseBtn
			// 
			this.browseBtn.Location = new System.Drawing.Point(690, 55);
			this.browseBtn.Name = "browseBtn";
			this.browseBtn.Size = new System.Drawing.Size(108, 23);
			this.browseBtn.TabIndex = 22;
			this.browseBtn.Text = "Browse";
			this.browseBtn.UseVisualStyleBackColor = true;
			this.browseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
			// 
			// reportOutputPath
			// 
			this.reportOutputPath.Location = new System.Drawing.Point(433, 29);
			this.reportOutputPath.Name = "reportOutputPath";
			this.reportOutputPath.Size = new System.Drawing.Size(366, 20);
			this.reportOutputPath.TabIndex = 21;
			this.reportOutputPath.TextChanged += new System.EventHandler(this.ReportOutputPath_TextChanged);
			this.reportOutputPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ReportOutputPath_KeyUp);
			// 
			// crossRep
			// 
			this.crossRep.AutoSize = true;
			this.crossRep.Location = new System.Drawing.Point(432, 384);
			this.crossRep.Name = "crossRep";
			this.crossRep.Size = new System.Drawing.Size(127, 17);
			this.crossRep.TabIndex = 20;
			this.crossRep.Text = "Cross File Repetitions";
			this.crossRep.UseVisualStyleBackColor = true;
			this.crossRep.CheckedChanged += new System.EventHandler(this.CrossRep_CheckedChanged);
			// 
			// contextMatch
			// 
			this.contextMatch.AutoSize = true;
			this.contextMatch.Location = new System.Drawing.Point(432, 361);
			this.contextMatch.Name = "contextMatch";
			this.contextMatch.Size = new System.Drawing.Size(95, 17);
			this.contextMatch.TabIndex = 19;
			this.contextMatch.Text = "Context Match";
			this.contextMatch.UseVisualStyleBackColor = true;
			this.contextMatch.CheckedChanged += new System.EventHandler(this.ContextMatch_CheckedChanged);
			// 
			// perfectMatch
			// 
			this.perfectMatch.AutoSize = true;
			this.perfectMatch.Location = new System.Drawing.Point(432, 338);
			this.perfectMatch.Name = "perfectMatch";
			this.perfectMatch.Size = new System.Drawing.Size(93, 17);
			this.perfectMatch.TabIndex = 18;
			this.perfectMatch.Text = "Perfect Match";
			this.perfectMatch.UseVisualStyleBackColor = true;
			this.perfectMatch.CheckedChanged += new System.EventHandler(this.PerfectMatch_CheckedChanged);
			// 
			// locked
			// 
			this.locked.AutoSize = true;
			this.locked.Location = new System.Drawing.Point(432, 407);
			this.locked.Name = "locked";
			this.locked.Size = new System.Drawing.Size(62, 17);
			this.locked.TabIndex = 17;
			this.locked.Text = "Locked";
			this.locked.UseVisualStyleBackColor = true;
			this.locked.CheckedChanged += new System.EventHandler(this.Locked_CheckedChanged);
			// 
			// internalFuzzies
			// 
			this.internalFuzzies.AutoSize = true;
			this.internalFuzzies.Location = new System.Drawing.Point(432, 315);
			this.internalFuzzies.Name = "internalFuzzies";
			this.internalFuzzies.Size = new System.Drawing.Size(99, 17);
			this.internalFuzzies.TabIndex = 16;
			this.internalFuzzies.Text = "Internal Fuzzies";
			this.internalFuzzies.UseVisualStyleBackColor = true;
			this.internalFuzzies.CheckedChanged += new System.EventHandler(this.InternalFuzzies_CheckedChanged);
			// 
			// adaptiveMT
			// 
			this.adaptiveMT.AutoSize = true;
			this.adaptiveMT.Location = new System.Drawing.Point(432, 292);
			this.adaptiveMT.Name = "adaptiveMT";
			this.adaptiveMT.Size = new System.Drawing.Size(130, 17);
			this.adaptiveMT.TabIndex = 15;
			this.adaptiveMT.Text = "Adaptive MT Baseline";
			this.adaptiveMT.UseVisualStyleBackColor = true;
			this.adaptiveMT.CheckedChanged += new System.EventHandler(this.AdaptiveMT_CheckedChanged);
			// 
			// adaptiveLearnings
			// 
			this.adaptiveLearnings.AutoSize = true;
			this.adaptiveLearnings.Location = new System.Drawing.Point(432, 269);
			this.adaptiveLearnings.Name = "adaptiveLearnings";
			this.adaptiveLearnings.Size = new System.Drawing.Size(158, 17);
			this.adaptiveLearnings.TabIndex = 14;
			this.adaptiveLearnings.Text = "Adaptive MT with Learnings";
			this.adaptiveLearnings.UseVisualStyleBackColor = true;
			this.adaptiveLearnings.CheckedChanged += new System.EventHandler(this.FragmentMatches_CheckedChanged);
			// 
			// optionsLabel
			// 
			this.optionsLabel.AutoSize = true;
			this.optionsLabel.Location = new System.Drawing.Point(429, 253);
			this.optionsLabel.Name = "optionsLabel";
			this.optionsLabel.Size = new System.Drawing.Size(137, 13);
			this.optionsLabel.TabIndex = 12;
			this.optionsLabel.Text = "Aditional Report Information";
			// 
			// exitBtn
			// 
			this.exitBtn.Location = new System.Drawing.Point(692, 426);
			this.exitBtn.Name = "exitBtn";
			this.exitBtn.Size = new System.Drawing.Size(107, 23);
			this.exitBtn.TabIndex = 11;
			this.exitBtn.Text = "Exit";
			this.exitBtn.UseVisualStyleBackColor = true;
			this.exitBtn.Click += new System.EventHandler(this.ExitBtn_Click);
			// 
			// targetBtn
			// 
			this.targetBtn.Location = new System.Drawing.Point(688, 195);
			this.targetBtn.Name = "targetBtn";
			this.targetBtn.Size = new System.Drawing.Size(107, 23);
			this.targetBtn.TabIndex = 10;
			this.targetBtn.Text = "Open Target Folder";
			this.targetBtn.UseVisualStyleBackColor = true;
			this.targetBtn.Click += new System.EventHandler(this.TargetBtn_Click);
			// 
			// csvBtn
			// 
			this.csvBtn.Location = new System.Drawing.Point(688, 166);
			this.csvBtn.Name = "csvBtn";
			this.csvBtn.Size = new System.Drawing.Size(107, 23);
			this.csvBtn.TabIndex = 9;
			this.csvBtn.Text = "Export to CSV";
			this.csvBtn.UseVisualStyleBackColor = true;
			this.csvBtn.Click += new System.EventHandler(this.CsvBtn_Click);
			// 
			// copyBtn
			// 
			this.copyBtn.Location = new System.Drawing.Point(688, 137);
			this.copyBtn.Name = "copyBtn";
			this.copyBtn.Size = new System.Drawing.Size(108, 23);
			this.copyBtn.TabIndex = 8;
			this.copyBtn.Text = "Copy to clipboard";
			this.copyBtn.UseVisualStyleBackColor = true;
			this.copyBtn.Click += new System.EventHandler(this.CopyBtn_Click);
			// 
			// languagesListBox
			// 
			this.languagesListBox.CheckOnClick = true;
			this.languagesListBox.FormattingEnabled = true;
			this.languagesListBox.Location = new System.Drawing.Point(246, 49);
			this.languagesListBox.Name = "languagesListBox";
			this.languagesListBox.Size = new System.Drawing.Size(177, 394);
			this.languagesListBox.TabIndex = 7;
			this.languagesListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LanguagesListBox_ItemCheck);
			this.languagesListBox.SelectedIndexChanged += new System.EventHandler(this.LanguagesListBox_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(246, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Select language(s) for export:";
			// 
			// includeHeaderCheck
			// 
			this.includeHeaderCheck.AutoSize = true;
			this.includeHeaderCheck.Location = new System.Drawing.Point(433, 55);
			this.includeHeaderCheck.Name = "includeHeaderCheck";
			this.includeHeaderCheck.Size = new System.Drawing.Size(140, 17);
			this.includeHeaderCheck.TabIndex = 5;
			this.includeHeaderCheck.Text = "Include header in export";
			this.includeHeaderCheck.UseVisualStyleBackColor = true;
			this.includeHeaderCheck.CheckedChanged += new System.EventHandler(this.IncludeHeaderCheck_CheckedChanged);
			// 
			// projListbox
			// 
			this.projListbox.CheckOnClick = true;
			this.projListbox.FormattingEnabled = true;
			this.projListbox.Location = new System.Drawing.Point(12, 49);
			this.projListbox.Name = "projListbox";
			this.projListbox.Size = new System.Drawing.Size(221, 394);
			this.projListbox.TabIndex = 1;
			this.projListbox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ProjListbox_ItemCheck);
			this.projListbox.SelectedIndexChanged += new System.EventHandler(this.ProjListbox_SelectedIndexChanged);
			// 
			// selectProjLbl
			// 
			this.selectProjLbl.AutoSize = true;
			this.selectProjLbl.Location = new System.Drawing.Point(12, 9);
			this.selectProjLbl.Name = "selectProjLbl";
			this.selectProjLbl.Size = new System.Drawing.Size(75, 13);
			this.selectProjLbl.TabIndex = 0;
			this.selectProjLbl.Text = "Select project:";
			// 
			// ReportExporterControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(811, 455);
			this.Controls.Add(this.mainPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ReportExporterControl";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Trados Studio – Export Analysis Reports";
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label selectProjLbl;
		private System.Windows.Forms.CheckBox includeHeaderCheck;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckedListBox languagesListBox;
		private System.Windows.Forms.Button copyBtn;
		private System.Windows.Forms.Button exitBtn;
		private System.Windows.Forms.Button targetBtn;
		private System.Windows.Forms.Button csvBtn;
		private System.Windows.Forms.Label optionsLabel;
		private System.Windows.Forms.CheckBox adaptiveMT;
		private System.Windows.Forms.CheckBox adaptiveLearnings;
		private System.Windows.Forms.CheckBox internalFuzzies;
		private System.Windows.Forms.CheckBox crossRep;
		private System.Windows.Forms.CheckBox contextMatch;
		private System.Windows.Forms.CheckBox perfectMatch;
		private System.Windows.Forms.CheckBox locked;
		private System.Windows.Forms.Button browseBtn;
		private System.Windows.Forms.TextBox reportOutputPath;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckedListBox projListbox;
		private System.Windows.Forms.ComboBox projectStatusComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button loadBtn;
		private System.Windows.Forms.Button clearBtn;
		private System.Windows.Forms.CheckBox chkBox_SelectAllProjects;
		private System.Windows.Forms.CheckBox chkBox_SelectAllLanguages;
		private System.Windows.Forms.CheckBox chkBox_IncludeSingleFileProjects;
	}
}