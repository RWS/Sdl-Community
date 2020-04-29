namespace Sdl.Community.CleanUpTasks
{
    partial class CleanUpTargetSettingsControl
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
            this.components = new System.ComponentModel.Container();
            this.saveFoldertextBox = new System.Windows.Forms.TextBox();
            this.saveFolderLabel = new System.Windows.Forms.Label();
            this.saveFolderButton = new System.Windows.Forms.Button();
            this.backupFolderLabel = new System.Windows.Forms.Label();
            this.backupTextBox = new System.Windows.Forms.TextBox();
            this.backupCheckBox = new System.Windows.Forms.CheckBox();
            this.preservePlaceHoldersCheckBox = new System.Windows.Forms.CheckBox();
            this.backupButton = new System.Windows.Forms.Button();
            this.conversionsSettingsControl = new ConversionsSettingsControl();
            this.generateTargetCheckBox = new System.Windows.Forms.CheckBox();
            this.saveFolderToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.backUpFolderToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // saveFoldertextBox
            // 
            this.saveFoldertextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.saveFoldertextBox.Location = new System.Drawing.Point(100, 13);
            this.saveFoldertextBox.Name = "saveFoldertextBox";
            this.saveFoldertextBox.ReadOnly = true;
            this.saveFoldertextBox.Size = new System.Drawing.Size(315, 19);
            this.saveFoldertextBox.TabIndex = 0;
            // 
            // saveFolderLabel
            // 
            this.saveFolderLabel.AutoSize = true;
            this.saveFolderLabel.Location = new System.Drawing.Point(15, 16);
            this.saveFolderLabel.Name = "saveFolderLabel";
            this.saveFolderLabel.Size = new System.Drawing.Size(68, 12);
            this.saveFolderLabel.TabIndex = 1;
            this.saveFolderLabel.Text = "Save Folder:";
            this.saveFolderToolTip.SetToolTip(this.saveFolderLabel, "Generates target file and saves to specified folder");
            // 
            // saveFolderButton
            // 
            this.saveFolderButton.Location = new System.Drawing.Point(100, 38);
            this.saveFolderButton.Name = "saveFolderButton";
            this.saveFolderButton.Size = new System.Drawing.Size(75, 23);
            this.saveFolderButton.TabIndex = 2;
            this.saveFolderButton.Text = "Set Folder";
            this.saveFolderButton.UseVisualStyleBackColor = true;
            // 
            // backupFolderLabel
            // 
            this.backupFolderLabel.AutoSize = true;
            this.backupFolderLabel.Location = new System.Drawing.Point(15, 90);
            this.backupFolderLabel.Name = "backupFolderLabel";
            this.backupFolderLabel.Size = new System.Drawing.Size(81, 12);
            this.backupFolderLabel.TabIndex = 3;
            this.backupFolderLabel.Text = "Backup Folder:";
            this.backUpFolderToolTip.SetToolTip(this.backupFolderLabel, "Saves a backup of the selected sdlxliff files");
            // 
            // backupTextBox
            // 
            this.backupTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.backupTextBox.Location = new System.Drawing.Point(102, 87);
            this.backupTextBox.Name = "backupTextBox";
            this.backupTextBox.ReadOnly = true;
            this.backupTextBox.Size = new System.Drawing.Size(313, 19);
            this.backupTextBox.TabIndex = 4;
            // 
            // backupCheckBox
            // 
            this.backupCheckBox.AutoSize = true;
            this.backupCheckBox.Checked = true;
            this.backupCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.backupCheckBox.Location = new System.Drawing.Point(100, 141);
            this.backupCheckBox.Name = "backupCheckBox";
            this.backupCheckBox.Size = new System.Drawing.Size(99, 16);
            this.backupCheckBox.TabIndex = 5;
            this.backupCheckBox.Text = "Make Backups";
            this.backupCheckBox.UseVisualStyleBackColor = true;
            // 
            // preservePlaceHoldersCheckBox
            // 
            this.preservePlaceHoldersCheckBox.AutoSize = true;
            this.preservePlaceHoldersCheckBox.Location = new System.Drawing.Point(205, 141);
            this.preservePlaceHoldersCheckBox.Name = "preservePlaceHoldersCheckBox";
            this.preservePlaceHoldersCheckBox.Size = new System.Drawing.Size(161, 16);
            this.preservePlaceHoldersCheckBox.TabIndex = 6;
            this.preservePlaceHoldersCheckBox.Text = "Preserve Placeholder Tags";
            this.preservePlaceHoldersCheckBox.UseVisualStyleBackColor = true;
            // 
            // backupButton
            // 
            this.backupButton.Location = new System.Drawing.Point(100, 112);
            this.backupButton.Name = "backupButton";
            this.backupButton.Size = new System.Drawing.Size(75, 23);
            this.backupButton.TabIndex = 7;
            this.backupButton.Text = "Set Folder";
            this.backupButton.UseVisualStyleBackColor = true;
            // 
            // conversionsSettingsControl
            // 
            this.conversionsSettingsControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.conversionsSettingsControl.Location = new System.Drawing.Point(0, 163);
            this.conversionsSettingsControl.Name = "conversionsSettingsControl";
            this.conversionsSettingsControl.Size = new System.Drawing.Size(430, 212);
            this.conversionsSettingsControl.TabIndex = 8;
            // 
            // generateTargetCheckBox
            // 
            this.generateTargetCheckBox.AutoSize = true;
            this.generateTargetCheckBox.Location = new System.Drawing.Point(102, 65);
            this.generateTargetCheckBox.Name = "generateTargetCheckBox";
            this.generateTargetCheckBox.Size = new System.Drawing.Size(107, 16);
            this.generateTargetCheckBox.TabIndex = 9;
            this.generateTargetCheckBox.Text = "Generate Target";
            this.generateTargetCheckBox.UseVisualStyleBackColor = true;
            // 
            // CleanUpTargetSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.generateTargetCheckBox);
            this.Controls.Add(this.conversionsSettingsControl);
            this.Controls.Add(this.backupButton);
            this.Controls.Add(this.preservePlaceHoldersCheckBox);
            this.Controls.Add(this.backupCheckBox);
            this.Controls.Add(this.backupTextBox);
            this.Controls.Add(this.backupFolderLabel);
            this.Controls.Add(this.saveFolderButton);
            this.Controls.Add(this.saveFolderLabel);
            this.Controls.Add(this.saveFoldertextBox);
            this.MinimumSize = new System.Drawing.Size(430, 375);
            this.Name = "CleanUpTargetSettingsControl";
            this.Size = new System.Drawing.Size(430, 375);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox saveFoldertextBox;
        private System.Windows.Forms.Label saveFolderLabel;
        private System.Windows.Forms.Button saveFolderButton;
        private System.Windows.Forms.Label backupFolderLabel;
        private System.Windows.Forms.TextBox backupTextBox;
        private System.Windows.Forms.CheckBox backupCheckBox;
        private System.Windows.Forms.CheckBox preservePlaceHoldersCheckBox;
        private System.Windows.Forms.Button backupButton;
        private ConversionsSettingsControl conversionsSettingsControl;
        private System.Windows.Forms.CheckBox generateTargetCheckBox;
        private System.Windows.Forms.ToolTip saveFolderToolTip;
        private System.Windows.Forms.ToolTip backUpFolderToolTip;
    }
}
