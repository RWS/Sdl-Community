namespace Sdl.Community.Utilities.TMTool.Tasks.RemapTMX
{
	partial class RemapTMXControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.outputFolderBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.saveTargetFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "TMX output folder:";
            // 
            // outputFolderBox
            // 
            this.outputFolderBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFolderBox.Location = new System.Drawing.Point(104, 10);
            this.outputFolderBox.Name = "outputFolderBox";
            this.outputFolderBox.Size = new System.Drawing.Size(449, 20);
            this.outputFolderBox.TabIndex = 4;
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(559, 7);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 5;
            this.browseButton.Text = "&Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // saveTargetFolderCheckBox
            // 
            this.saveTargetFolderCheckBox.AutoSize = true;
            this.saveTargetFolderCheckBox.Location = new System.Drawing.Point(104, 49);
            this.saveTargetFolderCheckBox.Name = "saveTargetFolderCheckBox";
            this.saveTargetFolderCheckBox.Size = new System.Drawing.Size(144, 17);
            this.saveTargetFolderCheckBox.TabIndex = 6;
            this.saveTargetFolderCheckBox.Text = "Save in taget TM\'s folder";
            this.saveTargetFolderCheckBox.UseVisualStyleBackColor = true;
            this.saveTargetFolderCheckBox.CheckedChanged += new System.EventHandler(this.saveTargetFolderCheckBox_CheckedChanged);
            // 
            // RemapTMXControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.saveTargetFolderCheckBox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.outputFolderBox);
            this.Controls.Add(this.label1);
            this.Name = "RemapTMXControl";
            this.Size = new System.Drawing.Size(647, 304);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox outputFolderBox;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.CheckBox saveTargetFolderCheckBox;
	}
}
