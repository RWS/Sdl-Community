﻿namespace Sdl.Community.TMBackup
{
	partial class PeriodicBackupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeriodicBackupForm));
            this.txt_Schedule = new System.Windows.Forms.TextBox();
            this.lbl_BackupInterval = new System.Windows.Forms.Label();
            this.lbl_FirstBackup = new System.Windows.Forms.Label();
            this.txtBox_TimeInterval = new System.Windows.Forms.TextBox();
            this.cmbBox_Interval = new System.Windows.Forms.ComboBox();
            this.dateTimePicker_FirstBackup = new System.Windows.Forms.DateTimePicker();
            this.lbl_At = new System.Windows.Forms.Label();
            this.timePicker_At = new System.Windows.Forms.DateTimePicker();
            this.btn_Now = new System.Windows.Forms.Button();
            this.lbl_Line = new System.Windows.Forms.Label();
            this.btn_Set = new System.Windows.Forms.Button();
            this.btn_Close = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_Schedule
            // 
            this.txt_Schedule.BackColor = System.Drawing.Color.LightGray;
            this.txt_Schedule.Location = new System.Drawing.Point(12, 12);
            this.txt_Schedule.Name = "txt_Schedule";
            this.txt_Schedule.ReadOnly = true;
            this.txt_Schedule.Size = new System.Drawing.Size(537, 20);
            this.txt_Schedule.TabIndex = 0;
            this.txt_Schedule.Text = "Schedule";
            // 
            // lbl_BackupInterval
            // 
            this.lbl_BackupInterval.AutoSize = true;
            this.lbl_BackupInterval.Location = new System.Drawing.Point(12, 55);
            this.lbl_BackupInterval.Name = "lbl_BackupInterval";
            this.lbl_BackupInterval.Size = new System.Drawing.Size(81, 13);
            this.lbl_BackupInterval.TabIndex = 2;
            this.lbl_BackupInterval.Text = "Backup interval";
            // 
            // lbl_FirstBackup
            // 
            this.lbl_FirstBackup.AutoSize = true;
            this.lbl_FirstBackup.Location = new System.Drawing.Point(12, 92);
            this.lbl_FirstBackup.Name = "lbl_FirstBackup";
            this.lbl_FirstBackup.Size = new System.Drawing.Size(80, 13);
            this.lbl_FirstBackup.TabIndex = 3;
            this.lbl_FirstBackup.Text = "First backup on";
            // 
            // txtBox_TimeInterval
            // 
            this.txtBox_TimeInterval.Location = new System.Drawing.Point(121, 52);
            this.txtBox_TimeInterval.Name = "txtBox_TimeInterval";
            this.txtBox_TimeInterval.Size = new System.Drawing.Size(49, 20);
            this.txtBox_TimeInterval.TabIndex = 4;
            this.txtBox_TimeInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBox_TimeInterval_KeyPress);
            // 
            // cmbBox_Interval
            // 
            this.cmbBox_Interval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBox_Interval.FormattingEnabled = true;
            this.cmbBox_Interval.Location = new System.Drawing.Point(176, 51);
            this.cmbBox_Interval.Name = "cmbBox_Interval";
            this.cmbBox_Interval.Size = new System.Drawing.Size(134, 21);
            this.cmbBox_Interval.TabIndex = 5;
            // 
            // dateTimePicker_FirstBackup
            // 
            this.dateTimePicker_FirstBackup.Location = new System.Drawing.Point(121, 86);
            this.dateTimePicker_FirstBackup.Name = "dateTimePicker_FirstBackup";
            this.dateTimePicker_FirstBackup.Size = new System.Drawing.Size(189, 20);
            this.dateTimePicker_FirstBackup.TabIndex = 6;
            // 
            // lbl_At
            // 
            this.lbl_At.AutoSize = true;
            this.lbl_At.Location = new System.Drawing.Point(328, 90);
            this.lbl_At.Name = "lbl_At";
            this.lbl_At.Size = new System.Drawing.Size(16, 13);
            this.lbl_At.TabIndex = 7;
            this.lbl_At.Text = "at";
            // 
            // timePicker_At
            // 
            this.timePicker_At.Location = new System.Drawing.Point(350, 86);
            this.timePicker_At.Name = "timePicker_At";
            this.timePicker_At.Size = new System.Drawing.Size(103, 20);
            this.timePicker_At.TabIndex = 8;
            // 
            // btn_Now
            // 
            this.btn_Now.Location = new System.Drawing.Point(474, 85);
            this.btn_Now.Name = "btn_Now";
            this.btn_Now.Size = new System.Drawing.Size(75, 23);
            this.btn_Now.TabIndex = 9;
            this.btn_Now.Text = "Now";
            this.btn_Now.UseVisualStyleBackColor = true;
            this.btn_Now.Click += new System.EventHandler(this.btn_Now_Click);
            // 
            // lbl_Line
            // 
            this.lbl_Line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbl_Line.Location = new System.Drawing.Point(-3, 149);
            this.lbl_Line.Name = "lbl_Line";
            this.lbl_Line.Size = new System.Drawing.Size(588, 2);
            this.lbl_Line.TabIndex = 13;
            // 
            // btn_Set
            // 
            this.btn_Set.Location = new System.Drawing.Point(378, 171);
            this.btn_Set.Name = "btn_Set";
            this.btn_Set.Size = new System.Drawing.Size(75, 23);
            this.btn_Set.TabIndex = 14;
            this.btn_Set.Text = "Set";
            this.btn_Set.UseVisualStyleBackColor = true;
            this.btn_Set.Click += new System.EventHandler(this.btn_Set_Click);
            // 
            // btn_Close
            // 
            this.btn_Close.Location = new System.Drawing.Point(474, 171);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(75, 23);
            this.btn_Close.TabIndex = 15;
            this.btn_Close.Text = "Close";
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // PeriodicBackupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(557, 213);
            this.Controls.Add(this.btn_Close);
            this.Controls.Add(this.btn_Set);
            this.Controls.Add(this.lbl_Line);
            this.Controls.Add(this.btn_Now);
            this.Controls.Add(this.timePicker_At);
            this.Controls.Add(this.lbl_At);
            this.Controls.Add(this.dateTimePicker_FirstBackup);
            this.Controls.Add(this.cmbBox_Interval);
            this.Controls.Add(this.txtBox_TimeInterval);
            this.Controls.Add(this.lbl_FirstBackup);
            this.Controls.Add(this.lbl_BackupInterval);
            this.Controls.Add(this.txt_Schedule);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(573, 340);
            this.Name = "PeriodicBackupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Periodic backup";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt_Schedule;
		private System.Windows.Forms.Label lbl_BackupInterval;
		private System.Windows.Forms.Label lbl_FirstBackup;
		private System.Windows.Forms.TextBox txtBox_TimeInterval;
		private System.Windows.Forms.ComboBox cmbBox_Interval;
		private System.Windows.Forms.DateTimePicker dateTimePicker_FirstBackup;
		private System.Windows.Forms.Label lbl_At;
		private System.Windows.Forms.DateTimePicker timePicker_At;
		private System.Windows.Forms.Button btn_Now;
		private System.Windows.Forms.Label lbl_Line;
		private System.Windows.Forms.Button btn_Set;
		private System.Windows.Forms.Button btn_Close;
	}
}