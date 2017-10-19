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
            this.txt_TMSelection = new System.Windows.Forms.TextBox();
            this.btn_TMSelection = new System.Windows.Forms.Button();
            this.btn_TMBackupDestination = new System.Windows.Forms.Button();
            this.txt_TMBackupDestination = new System.Windows.Forms.TextBox();
            this.lbl_ScheduleBackup = new System.Windows.Forms.Label();
            this.radioBtn_Daily = new System.Windows.Forms.RadioButton();
            this.radioBtn_Weekly = new System.Windows.Forms.RadioButton();
            this.radioBtn_Montly = new System.Windows.Forms.RadioButton();
            this.radioBtn_Customized = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txt_TMSelection
            // 
            this.txt_TMSelection.Location = new System.Drawing.Point(27, 84);
            this.txt_TMSelection.Multiline = true;
            this.txt_TMSelection.Name = "txt_TMSelection";
            this.txt_TMSelection.Size = new System.Drawing.Size(181, 46);
            this.txt_TMSelection.TabIndex = 2;
            // 
            // btn_TMSelection
            // 
            this.btn_TMSelection.Location = new System.Drawing.Point(27, 46);
            this.btn_TMSelection.Name = "btn_TMSelection";
            this.btn_TMSelection.Size = new System.Drawing.Size(181, 27);
            this.btn_TMSelection.TabIndex = 3;
            this.btn_TMSelection.Text = "Choose TM";
            this.btn_TMSelection.UseVisualStyleBackColor = true;
            // 
            // btn_TMBackupDestination
            // 
            this.btn_TMBackupDestination.Location = new System.Drawing.Point(339, 46);
            this.btn_TMBackupDestination.Name = "btn_TMBackupDestination";
            this.btn_TMBackupDestination.Size = new System.Drawing.Size(181, 27);
            this.btn_TMBackupDestination.TabIndex = 4;
            this.btn_TMBackupDestination.Text = "Select TM backup destination";
            this.btn_TMBackupDestination.UseVisualStyleBackColor = true;
            // 
            // txt_TMBackupDestination
            // 
            this.txt_TMBackupDestination.Location = new System.Drawing.Point(339, 84);
            this.txt_TMBackupDestination.Multiline = true;
            this.txt_TMBackupDestination.Name = "txt_TMBackupDestination";
            this.txt_TMBackupDestination.Size = new System.Drawing.Size(181, 46);
            this.txt_TMBackupDestination.TabIndex = 5;
            // 
            // lbl_ScheduleBackup
            // 
            this.lbl_ScheduleBackup.AutoSize = true;
            this.lbl_ScheduleBackup.Location = new System.Drawing.Point(24, 157);
            this.lbl_ScheduleBackup.Name = "lbl_ScheduleBackup";
            this.lbl_ScheduleBackup.Size = new System.Drawing.Size(91, 13);
            this.lbl_ScheduleBackup.TabIndex = 6;
            this.lbl_ScheduleBackup.Text = "Schedule backup";
            // 
            // radioBtn_Daily
            // 
            this.radioBtn_Daily.AutoSize = true;
            this.radioBtn_Daily.Location = new System.Drawing.Point(27, 186);
            this.radioBtn_Daily.Name = "radioBtn_Daily";
            this.radioBtn_Daily.Size = new System.Drawing.Size(48, 17);
            this.radioBtn_Daily.TabIndex = 7;
            this.radioBtn_Daily.TabStop = true;
            this.radioBtn_Daily.Text = "Daily";
            this.radioBtn_Daily.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Weekly
            // 
            this.radioBtn_Weekly.AutoSize = true;
            this.radioBtn_Weekly.Location = new System.Drawing.Point(27, 209);
            this.radioBtn_Weekly.Name = "radioBtn_Weekly";
            this.radioBtn_Weekly.Size = new System.Drawing.Size(61, 17);
            this.radioBtn_Weekly.TabIndex = 8;
            this.radioBtn_Weekly.TabStop = true;
            this.radioBtn_Weekly.Text = "Weekly";
            this.radioBtn_Weekly.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Montly
            // 
            this.radioBtn_Montly.AutoSize = true;
            this.radioBtn_Montly.Location = new System.Drawing.Point(27, 232);
            this.radioBtn_Montly.Name = "radioBtn_Montly";
            this.radioBtn_Montly.Size = new System.Drawing.Size(56, 17);
            this.radioBtn_Montly.TabIndex = 9;
            this.radioBtn_Montly.TabStop = true;
            this.radioBtn_Montly.Text = "Montly";
            this.radioBtn_Montly.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Customized
            // 
            this.radioBtn_Customized.AutoSize = true;
            this.radioBtn_Customized.Location = new System.Drawing.Point(27, 255);
            this.radioBtn_Customized.Name = "radioBtn_Customized";
            this.radioBtn_Customized.Size = new System.Drawing.Size(67, 17);
            this.radioBtn_Customized.TabIndex = 10;
            this.radioBtn_Customized.TabStop = true;
            this.radioBtn_Customized.Text = "Between";
            this.radioBtn_Customized.UseVisualStyleBackColor = true;
            // 
            // TMBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 322);
            this.Controls.Add(this.radioBtn_Customized);
            this.Controls.Add(this.radioBtn_Montly);
            this.Controls.Add(this.radioBtn_Weekly);
            this.Controls.Add(this.radioBtn_Daily);
            this.Controls.Add(this.lbl_ScheduleBackup);
            this.Controls.Add(this.txt_TMBackupDestination);
            this.Controls.Add(this.btn_TMBackupDestination);
            this.Controls.Add(this.btn_TMSelection);
            this.Controls.Add(this.txt_TMSelection);
            this.Name = "TMBackupForm";
            this.Text = "TMBackupForm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txt_TMSelection;
		private System.Windows.Forms.Button btn_TMSelection;
		private System.Windows.Forms.Button btn_TMBackupDestination;
		private System.Windows.Forms.TextBox txt_TMBackupDestination;
		private System.Windows.Forms.Label lbl_ScheduleBackup;
		private System.Windows.Forms.RadioButton radioBtn_Daily;
		private System.Windows.Forms.RadioButton radioBtn_Weekly;
		private System.Windows.Forms.RadioButton radioBtn_Montly;
		private System.Windows.Forms.RadioButton radioBtn_Customized;
	}
}