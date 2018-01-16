namespace Sdl.Community.Utilities.TMTool.Tasks.RemoveDuplicates
{
	partial class RemoveDupControl
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
            this.chBackup = new System.Windows.Forms.CheckBox();
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
            this.gbTaskOptions.Controls.Add(this.chBackup);
            this.gbTaskOptions.Location = new System.Drawing.Point(3, 3);
            this.gbTaskOptions.Name = "gbTaskOptions";
            this.gbTaskOptions.Size = new System.Drawing.Size(582, 80);
            this.gbTaskOptions.TabIndex = 4;
            this.gbTaskOptions.TabStop = false;
            this.gbTaskOptions.Text = "Remove &Duplicates Task Options";
            // 
            // chPreservePsw
            // 
            this.chPreservePsw.AutoSize = true;
            this.chPreservePsw.Location = new System.Drawing.Point(9, 51);
            this.chPreservePsw.Name = "chPreservePsw";
            this.chPreservePsw.Size = new System.Drawing.Size(240, 17);
            this.chPreservePsw.TabIndex = 2;
            this.chPreservePsw.Text = "Preserve &administrator password (if protected)";
            this.chPreservePsw.UseVisualStyleBackColor = true;
            // 
            // chBackup
            // 
            this.chBackup.AutoSize = true;
            this.chBackup.Location = new System.Drawing.Point(9, 28);
            this.chBackup.Name = "chBackup";
            this.chBackup.Size = new System.Drawing.Size(190, 17);
            this.chBackup.TabIndex = 1;
            this.chBackup.Text = "Create &backups from changed files";
            this.chBackup.UseVisualStyleBackColor = true;
            // 
            // gbScenario
            // 
            this.gbScenario.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbScenario.Controls.Add(this.rbMixedSc);
            this.gbScenario.Controls.Add(this.rbPresegmentedSc);
            this.gbScenario.Controls.Add(this.rbDefaultSc);
            this.gbScenario.Location = new System.Drawing.Point(3, 89);
            this.gbScenario.Name = "gbScenario";
            this.gbScenario.Size = new System.Drawing.Size(582, 91);
            this.gbScenario.TabIndex = 4;
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
            this.rbMixedSc.TabIndex = 6;
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
            this.rbPresegmentedSc.Size = new System.Drawing.Size(561, 17);
            this.rbPresegmentedSc.TabIndex = 5;
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
            this.rbDefaultSc.Size = new System.Drawing.Size(561, 17);
            this.rbDefaultSc.TabIndex = 4;
            this.rbDefaultSc.TabStop = true;
            this.rbDefaultSc.Text = "The imported data will be primarily used with new, &native source files or files " +
    "processed only with SDL Trados Studio";
            this.rbDefaultSc.UseVisualStyleBackColor = true;
            // 
            // RemoveDupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbScenario);
            this.Controls.Add(this.gbTaskOptions);
            this.MaximumSize = new System.Drawing.Size(588, 180);
            this.Name = "RemoveDupControl";
            this.Size = new System.Drawing.Size(588, 180);
            this.gbTaskOptions.ResumeLayout(false);
            this.gbTaskOptions.PerformLayout();
            this.gbScenario.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbTaskOptions;
		private System.Windows.Forms.CheckBox chPreservePsw;
		private System.Windows.Forms.CheckBox chBackup;
		private System.Windows.Forms.GroupBox gbScenario;
		private System.Windows.Forms.RadioButton rbMixedSc;
		private System.Windows.Forms.RadioButton rbPresegmentedSc;
		private System.Windows.Forms.RadioButton rbDefaultSc;
	}
}
