namespace Sdl.Community.Utilities.TMTool.Tasks.RevertIndex
{
	partial class RevertIndexControl
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
            this.gbTaskOptions = new System.Windows.Forms.GroupBox();
            this.chPreservePsw = new System.Windows.Forms.CheckBox();
            this.chOverwriteTUs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.tbTargetFile = new System.Windows.Forms.TextBox();
            this.lblTargetFile = new System.Windows.Forms.Label();
            this.gbScenario = new System.Windows.Forms.GroupBox();
            this.rbMixedSc = new System.Windows.Forms.RadioButton();
            this.rbPresegmentedSc = new System.Windows.Forms.RadioButton();
            this.rbDefaultSc = new System.Windows.Forms.RadioButton();
            this.gbTaskOptions.SuspendLayout();
            this.gbScenario.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTaskOptions
            // 
            this.gbTaskOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTaskOptions.Controls.Add(this.chPreservePsw);
            this.gbTaskOptions.Controls.Add(this.chOverwriteTUs);
            this.gbTaskOptions.Controls.Add(this.btnBrowse);
            this.gbTaskOptions.Controls.Add(this.tbTargetFile);
            this.gbTaskOptions.Controls.Add(this.lblTargetFile);
            this.gbTaskOptions.Location = new System.Drawing.Point(0, 0);
            this.gbTaskOptions.Name = "gbTaskOptions";
            this.gbTaskOptions.Size = new System.Drawing.Size(588, 111);
            this.gbTaskOptions.TabIndex = 7;
            this.gbTaskOptions.TabStop = false;
            this.gbTaskOptions.Text = "&Reverse Index Task Options";
            // 
            // chPreservePsw
            // 
            this.chPreservePsw.AutoSize = true;
            this.chPreservePsw.Location = new System.Drawing.Point(9, 81);
            this.chPreservePsw.Name = "chPreservePsw";
            this.chPreservePsw.Size = new System.Drawing.Size(240, 17);
            this.chPreservePsw.TabIndex = 5;
            this.chPreservePsw.Text = "Preserve &administrator password (if protected)";
            this.chPreservePsw.UseVisualStyleBackColor = true;
            // 
            // chOverwriteTUs
            // 
            this.chOverwriteTUs.AutoSize = true;
            this.chOverwriteTUs.Location = new System.Drawing.Point(9, 58);
            this.chOverwriteTUs.Name = "chOverwriteTUs";
            this.chOverwriteTUs.Size = new System.Drawing.Size(185, 17);
            this.chOverwriteTUs.TabIndex = 4;
            this.chOverwriteTUs.Text = "&Overwrite existing translation units";
            this.chOverwriteTUs.UseVisualStyleBackColor = true;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(507, 21);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "&Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // tbTargetFile
            // 
            this.tbTargetFile.AllowDrop = true;
            this.tbTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTargetFile.Location = new System.Drawing.Point(114, 23);
            this.tbTargetFile.Name = "tbTargetFile";
            this.tbTargetFile.Size = new System.Drawing.Size(388, 20);
            this.tbTargetFile.TabIndex = 2;
            this.tbTargetFile.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbTargetFile_DragDrop);
            this.tbTargetFile.DragEnter += new System.Windows.Forms.DragEventHandler(this.tbTargetFile_DragEnter);
            // 
            // lblTargetFile
            // 
            this.lblTargetFile.AutoSize = true;
            this.lblTargetFile.Location = new System.Drawing.Point(6, 26);
            this.lblTargetFile.Name = "lblTargetFile";
            this.lblTargetFile.Size = new System.Drawing.Size(107, 13);
            this.lblTargetFile.TabIndex = 1;
            this.lblTargetFile.Text = "Indicate target &folder:";
            // 
            // gbScenario
            // 
            this.gbScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScenario.Controls.Add(this.rbMixedSc);
            this.gbScenario.Controls.Add(this.rbPresegmentedSc);
            this.gbScenario.Controls.Add(this.rbDefaultSc);
            this.gbScenario.Location = new System.Drawing.Point(0, 117);
            this.gbScenario.Name = "gbScenario";
            this.gbScenario.Size = new System.Drawing.Size(588, 93);
            this.gbScenario.TabIndex = 8;
            this.gbScenario.TabStop = false;
            this.gbScenario.Text = "&Import Scenario";
            // 
            // rbMixedSc
            // 
            this.rbMixedSc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbMixedSc.Location = new System.Drawing.Point(9, 65);
            this.rbMixedSc.Name = "rbMixedSc";
            this.rbMixedSc.Size = new System.Drawing.Size(567, 17);
            this.rbMixedSc.TabIndex = 9;
            this.rbMixedSc.TabStop = true;
            this.rbMixedSc.Text = "The imported data will be primarily used in &mixed scenarios";
            this.rbMixedSc.UseVisualStyleBackColor = true;
            // 
            // rbPresegmentedSc
            // 
            this.rbPresegmentedSc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbPresegmentedSc.Location = new System.Drawing.Point(9, 42);
            this.rbPresegmentedSc.Name = "rbPresegmentedSc";
            this.rbPresegmentedSc.Size = new System.Drawing.Size(567, 17);
            this.rbPresegmentedSc.TabIndex = 8;
            this.rbPresegmentedSc.TabStop = true;
            this.rbPresegmentedSc.Text = "The imported data will be primarily used with presegmented le&gacy SDL Trados ITD" +
    " or TTX files";
            this.rbPresegmentedSc.UseVisualStyleBackColor = true;
            // 
            // rbDefaultSc
            // 
            this.rbDefaultSc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rbDefaultSc.Location = new System.Drawing.Point(9, 19);
            this.rbDefaultSc.Name = "rbDefaultSc";
            this.rbDefaultSc.Size = new System.Drawing.Size(567, 17);
            this.rbDefaultSc.TabIndex = 7;
            this.rbDefaultSc.TabStop = true;
            this.rbDefaultSc.Text = "The imported data will be primarily used with &new, native source files or files " +
    "processed only with SDL Trados Studio";
            this.rbDefaultSc.UseVisualStyleBackColor = true;
            // 
            // RevertIndexControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbScenario);
            this.Controls.Add(this.gbTaskOptions);
            this.MinimumSize = new System.Drawing.Size(588, 210);
            this.Name = "RevertIndexControl";
            this.Size = new System.Drawing.Size(588, 210);
            this.gbTaskOptions.ResumeLayout(false);
            this.gbTaskOptions.PerformLayout();
            this.gbScenario.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbTaskOptions;
		private System.Windows.Forms.CheckBox chPreservePsw;
		private System.Windows.Forms.CheckBox chOverwriteTUs;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox tbTargetFile;
		private System.Windows.Forms.Label lblTargetFile;
		private System.Windows.Forms.GroupBox gbScenario;
		private System.Windows.Forms.RadioButton rbMixedSc;
		private System.Windows.Forms.RadioButton rbPresegmentedSc;
		private System.Windows.Forms.RadioButton rbDefaultSc;
	}
}
