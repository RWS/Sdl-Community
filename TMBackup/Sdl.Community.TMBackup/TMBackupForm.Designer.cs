namespace Sdl.Community.TMBackup
{
	partial class TMBackupForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMBackupForm));
            this.txt_BackupFrom = new System.Windows.Forms.TextBox();
            this.btn_BackupFrom = new System.Windows.Forms.Button();
            this.btn_BackupTo = new System.Windows.Forms.Button();
            this.txt_BackupTo = new System.Windows.Forms.TextBox();
            this.lbl_BackupFrom = new System.Windows.Forms.Label();
            this.lbl_BackupTo = new System.Windows.Forms.Label();
            this.txt_Description = new System.Windows.Forms.TextBox();
            this.lbl_Description = new System.Windows.Forms.Label();
            this.btn_SaveSettings = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Change = new System.Windows.Forms.Button();
            this.btn_Details = new System.Windows.Forms.Button();
            this.txt_BackupDetails = new System.Windows.Forms.TextBox();
            this.txt_BackupTime = new System.Windows.Forms.TextBox();
            this.lbl_BackupTime = new System.Windows.Forms.Label();
            this.lbl_BackupFileDesc = new System.Windows.Forms.Label();
            this.lbl_Line2 = new System.Windows.Forms.Label();
            this.lbl_Line1 = new System.Windows.Forms.Label();
            this.txt_BackupName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_BackupNameError = new System.Windows.Forms.TextBox();
            this.txt_BackupFromError = new System.Windows.Forms.TextBox();
            this.txt_BackupToError = new System.Windows.Forms.TextBox();
            this.txt_TaskNameError = new System.Windows.Forms.TextBox();
            this.txt_WhenToBackupError = new System.Windows.Forms.TextBox();
            this.browseBtn_ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // txt_BackupFrom
            // 
            this.txt_BackupFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_BackupFrom.Location = new System.Drawing.Point(133, 53);
            this.txt_BackupFrom.Multiline = true;
            this.txt_BackupFrom.Name = "txt_BackupFrom";
            this.txt_BackupFrom.ReadOnly = true;
            this.txt_BackupFrom.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_BackupFrom.Size = new System.Drawing.Size(398, 87);
            this.txt_BackupFrom.TabIndex = 2;
            // 
            // btn_BackupFrom
            // 
            this.btn_BackupFrom.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_BackupFrom.Location = new System.Drawing.Point(537, 83);
            this.btn_BackupFrom.Name = "btn_BackupFrom";
            this.btn_BackupFrom.Size = new System.Drawing.Size(104, 28);
            this.btn_BackupFrom.TabIndex = 3;
            this.btn_BackupFrom.Text = "Browse";
            this.browseBtn_ToolTip.SetToolTip(this.btn_BackupFrom, "Browse location from where to backup files");
            this.btn_BackupFrom.UseVisualStyleBackColor = true;
            this.btn_BackupFrom.Click += new System.EventHandler(this.btn_BackupFrom_Click);
            // 
            // btn_BackupTo
            // 
            this.btn_BackupTo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_BackupTo.Location = new System.Drawing.Point(537, 150);
            this.btn_BackupTo.Name = "btn_BackupTo";
            this.btn_BackupTo.Size = new System.Drawing.Size(104, 27);
            this.btn_BackupTo.TabIndex = 4;
            this.btn_BackupTo.Text = "Browse";
            this.browseBtn_ToolTip.SetToolTip(this.btn_BackupTo, "Browse destination location for the backed up files");
            this.btn_BackupTo.UseVisualStyleBackColor = true;
            this.btn_BackupTo.Click += new System.EventHandler(this.btn_BackupTo_Click);
            // 
            // txt_BackupTo
            // 
            this.txt_BackupTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_BackupTo.Location = new System.Drawing.Point(133, 146);
            this.txt_BackupTo.Multiline = true;
            this.txt_BackupTo.Name = "txt_BackupTo";
            this.txt_BackupTo.ReadOnly = true;
            this.txt_BackupTo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_BackupTo.Size = new System.Drawing.Size(398, 38);
            this.txt_BackupTo.TabIndex = 5;
            // 
            // lbl_BackupFrom
            // 
            this.lbl_BackupFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_BackupFrom.AutoSize = true;
            this.lbl_BackupFrom.Location = new System.Drawing.Point(17, 83);
            this.lbl_BackupFrom.Name = "lbl_BackupFrom";
            this.lbl_BackupFrom.Size = new System.Drawing.Size(67, 13);
            this.lbl_BackupFrom.TabIndex = 11;
            this.lbl_BackupFrom.Text = "Backup from";
            // 
            // lbl_BackupTo
            // 
            this.lbl_BackupTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_BackupTo.AutoSize = true;
            this.lbl_BackupTo.Location = new System.Drawing.Point(17, 157);
            this.lbl_BackupTo.Name = "lbl_BackupTo";
            this.lbl_BackupTo.Size = new System.Drawing.Size(56, 13);
            this.lbl_BackupTo.TabIndex = 12;
            this.lbl_BackupTo.Text = "Backup to";
            // 
            // txt_Description
            // 
            this.txt_Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_Description.Location = new System.Drawing.Point(133, 197);
            this.txt_Description.Multiline = true;
            this.txt_Description.Name = "txt_Description";
            this.txt_Description.Size = new System.Drawing.Size(398, 27);
            this.txt_Description.TabIndex = 14;
            // 
            // lbl_Description
            // 
            this.lbl_Description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Description.AutoSize = true;
            this.lbl_Description.Location = new System.Drawing.Point(17, 200);
            this.lbl_Description.Name = "lbl_Description";
            this.lbl_Description.Size = new System.Drawing.Size(60, 13);
            this.lbl_Description.TabIndex = 13;
            this.lbl_Description.Text = "Description";
            // 
            // btn_SaveSettings
            // 
            this.btn_SaveSettings.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_SaveSettings.Location = new System.Drawing.Point(436, 345);
            this.btn_SaveSettings.Name = "btn_SaveSettings";
            this.btn_SaveSettings.Size = new System.Drawing.Size(104, 27);
            this.btn_SaveSettings.TabIndex = 18;
            this.btn_SaveSettings.Text = "Save Settings";
            this.btn_SaveSettings.UseVisualStyleBackColor = true;
            this.btn_SaveSettings.Click += new System.EventHandler(this.btn_SaveSettings_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Cancel.Location = new System.Drawing.Point(546, 345);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(104, 27);
            this.btn_Cancel.TabIndex = 19;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Change
            // 
            this.btn_Change.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Change.Location = new System.Drawing.Point(537, 283);
            this.btn_Change.Name = "btn_Change";
            this.btn_Change.Size = new System.Drawing.Size(104, 27);
            this.btn_Change.TabIndex = 25;
            this.btn_Change.Text = "Change ↓";
            this.browseBtn_ToolTip.SetToolTip(this.btn_Change, "Select period of backing up");
            this.btn_Change.UseVisualStyleBackColor = true;
            this.btn_Change.Click += new System.EventHandler(this.btn_Change_Click);
            // 
            // btn_Details
            // 
            this.btn_Details.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Details.Location = new System.Drawing.Point(537, 250);
            this.btn_Details.Name = "btn_Details";
            this.btn_Details.Size = new System.Drawing.Size(104, 27);
            this.btn_Details.TabIndex = 24;
            this.btn_Details.Text = "Details";
            this.browseBtn_ToolTip.SetToolTip(this.btn_Details, "Select what kind of files should be backed up");
            this.btn_Details.UseVisualStyleBackColor = true;
            this.btn_Details.Click += new System.EventHandler(this.btn_Details_Click);
            // 
            // txt_BackupDetails
            // 
            this.txt_BackupDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_BackupDetails.Location = new System.Drawing.Point(133, 250);
            this.txt_BackupDetails.Multiline = true;
            this.txt_BackupDetails.Name = "txt_BackupDetails";
            this.txt_BackupDetails.ReadOnly = true;
            this.txt_BackupDetails.Size = new System.Drawing.Size(398, 27);
            this.txt_BackupDetails.TabIndex = 23;
            // 
            // txt_BackupTime
            // 
            this.txt_BackupTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_BackupTime.Location = new System.Drawing.Point(133, 283);
            this.txt_BackupTime.Multiline = true;
            this.txt_BackupTime.Name = "txt_BackupTime";
            this.txt_BackupTime.ReadOnly = true;
            this.txt_BackupTime.Size = new System.Drawing.Size(398, 27);
            this.txt_BackupTime.TabIndex = 22;
            // 
            // lbl_BackupTime
            // 
            this.lbl_BackupTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_BackupTime.AutoSize = true;
            this.lbl_BackupTime.Location = new System.Drawing.Point(17, 286);
            this.lbl_BackupTime.Name = "lbl_BackupTime";
            this.lbl_BackupTime.Size = new System.Drawing.Size(87, 13);
            this.lbl_BackupTime.TabIndex = 21;
            this.lbl_BackupTime.Text = "When to backup";
            // 
            // lbl_BackupFileDesc
            // 
            this.lbl_BackupFileDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_BackupFileDesc.AutoSize = true;
            this.lbl_BackupFileDesc.Location = new System.Drawing.Point(17, 253);
            this.lbl_BackupFileDesc.Name = "lbl_BackupFileDesc";
            this.lbl_BackupFileDesc.Size = new System.Drawing.Size(84, 13);
            this.lbl_BackupFileDesc.TabIndex = 20;
            this.lbl_BackupFileDesc.Text = "What to backup";
            // 
            // lbl_Line2
            // 
            this.lbl_Line2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Line2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line2.Location = new System.Drawing.Point(-11, 335);
            this.lbl_Line2.Name = "lbl_Line2";
            this.lbl_Line2.Size = new System.Drawing.Size(683, 2);
            this.lbl_Line2.TabIndex = 26;
            // 
            // lbl_Line1
            // 
            this.lbl_Line1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbl_Line1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line1.Location = new System.Drawing.Point(-12, 234);
            this.lbl_Line1.Name = "lbl_Line1";
            this.lbl_Line1.Size = new System.Drawing.Size(684, 2);
            this.lbl_Line1.TabIndex = 27;
            // 
            // txt_BackupName
            // 
            this.txt_BackupName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_BackupName.Location = new System.Drawing.Point(133, 12);
            this.txt_BackupName.Multiline = true;
            this.txt_BackupName.Name = "txt_BackupName";
            this.txt_BackupName.Size = new System.Drawing.Size(398, 27);
            this.txt_BackupName.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "Backup name";
            // 
            // txt_BackupNameError
            // 
            this.txt_BackupNameError.BackColor = System.Drawing.SystemColors.Control;
            this.txt_BackupNameError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_BackupNameError.ForeColor = System.Drawing.Color.Red;
            this.txt_BackupNameError.Location = new System.Drawing.Point(538, 12);
            this.txt_BackupNameError.Multiline = true;
            this.txt_BackupNameError.Name = "txt_BackupNameError";
            this.txt_BackupNameError.Size = new System.Drawing.Size(100, 30);
            this.txt_BackupNameError.TabIndex = 30;
            this.txt_BackupNameError.Text = "Backup name is mandatory!";
            this.txt_BackupNameError.Visible = false;
            // 
            // txt_BackupFromError
            // 
            this.txt_BackupFromError.BackColor = System.Drawing.SystemColors.Control;
            this.txt_BackupFromError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_BackupFromError.ForeColor = System.Drawing.Color.Red;
            this.txt_BackupFromError.Location = new System.Drawing.Point(20, 99);
            this.txt_BackupFromError.Multiline = true;
            this.txt_BackupFromError.Name = "txt_BackupFromError";
            this.txt_BackupFromError.Size = new System.Drawing.Size(100, 30);
            this.txt_BackupFromError.TabIndex = 31;
            this.txt_BackupFromError.Text = "\'Backup from\' cannot be empty!";
            this.txt_BackupFromError.Visible = false;
            // 
            // txt_BackupToError
            // 
            this.txt_BackupToError.BackColor = System.Drawing.SystemColors.Control;
            this.txt_BackupToError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_BackupToError.ForeColor = System.Drawing.Color.Red;
            this.txt_BackupToError.Location = new System.Drawing.Point(21, 173);
            this.txt_BackupToError.Multiline = true;
            this.txt_BackupToError.Name = "txt_BackupToError";
            this.txt_BackupToError.Size = new System.Drawing.Size(100, 30);
            this.txt_BackupToError.TabIndex = 32;
            this.txt_BackupToError.Text = "\'Backup to\' cannot be empty!";
            this.txt_BackupToError.Visible = false;
            // 
            // txt_TaskNameError
            // 
            this.txt_TaskNameError.BackColor = System.Drawing.SystemColors.Control;
            this.txt_TaskNameError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_TaskNameError.ForeColor = System.Drawing.Color.Red;
            this.txt_TaskNameError.Location = new System.Drawing.Point(20, 34);
            this.txt_TaskNameError.Multiline = true;
            this.txt_TaskNameError.Name = "txt_TaskNameError";
            this.txt_TaskNameError.Size = new System.Drawing.Size(100, 30);
            this.txt_TaskNameError.TabIndex = 33;
            this.txt_TaskNameError.Text = "Task already exist with this  name!";
            this.txt_TaskNameError.Visible = false;
            // 
            // txt_WhenToBackupError
            // 
            this.txt_WhenToBackupError.BackColor = System.Drawing.SystemColors.Control;
            this.txt_WhenToBackupError.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_WhenToBackupError.ForeColor = System.Drawing.Color.Red;
            this.txt_WhenToBackupError.Location = new System.Drawing.Point(21, 302);
            this.txt_WhenToBackupError.Multiline = true;
            this.txt_WhenToBackupError.Name = "txt_WhenToBackupError";
            this.txt_WhenToBackupError.Size = new System.Drawing.Size(100, 30);
            this.txt_WhenToBackupError.TabIndex = 34;
            this.txt_WhenToBackupError.Text = "\'When to backup\' cannot be empty!";
            this.txt_WhenToBackupError.Visible = false;
            // 
            // TMBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 384);
            this.Controls.Add(this.txt_WhenToBackupError);
            this.Controls.Add(this.txt_TaskNameError);
            this.Controls.Add(this.txt_BackupToError);
            this.Controls.Add(this.txt_BackupFromError);
            this.Controls.Add(this.txt_BackupNameError);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_BackupName);
            this.Controls.Add(this.lbl_Line1);
            this.Controls.Add(this.lbl_Line2);
            this.Controls.Add(this.txt_Description);
            this.Controls.Add(this.btn_Change);
            this.Controls.Add(this.lbl_Description);
            this.Controls.Add(this.btn_Details);
            this.Controls.Add(this.lbl_BackupFrom);
            this.Controls.Add(this.txt_BackupDetails);
            this.Controls.Add(this.lbl_BackupTo);
            this.Controls.Add(this.txt_BackupTime);
            this.Controls.Add(this.txt_BackupFrom);
            this.Controls.Add(this.btn_BackupFrom);
            this.Controls.Add(this.lbl_BackupTime);
            this.Controls.Add(this.btn_BackupTo);
            this.Controls.Add(this.lbl_BackupFileDesc);
            this.Controls.Add(this.txt_BackupTo);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_SaveSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(3000, 440);
            this.Name = "TMBackupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TM backup";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txt_BackupFrom;
		private System.Windows.Forms.Button btn_BackupFrom;
		private System.Windows.Forms.Button btn_BackupTo;
		private System.Windows.Forms.TextBox txt_BackupTo;
		private System.Windows.Forms.Label lbl_BackupFrom;
		private System.Windows.Forms.Label lbl_BackupTo;
		private System.Windows.Forms.Label lbl_Description;
		private System.Windows.Forms.TextBox txt_Description;
		private System.Windows.Forms.Button btn_SaveSettings;
		private System.Windows.Forms.Button btn_Cancel;
		private System.Windows.Forms.Button btn_Change;
		private System.Windows.Forms.Button btn_Details;
		private System.Windows.Forms.TextBox txt_BackupDetails;
		private System.Windows.Forms.TextBox txt_BackupTime;
		private System.Windows.Forms.Label lbl_BackupTime;
		private System.Windows.Forms.Label lbl_BackupFileDesc;
		private System.Windows.Forms.Label lbl_Line2;
		private System.Windows.Forms.Label lbl_Line1;
		private System.Windows.Forms.TextBox txt_BackupName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txt_BackupNameError;
		private System.Windows.Forms.TextBox txt_BackupFromError;
		private System.Windows.Forms.TextBox txt_BackupToError;
		private System.Windows.Forms.TextBox txt_TaskNameError;
		private System.Windows.Forms.TextBox txt_WhenToBackupError;
		private System.Windows.Forms.ToolTip browseBtn_ToolTip;
	}
}