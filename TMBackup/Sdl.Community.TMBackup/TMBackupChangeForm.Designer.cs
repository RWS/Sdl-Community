namespace Sdl.Community.TMBackup
{
	partial class TMBackupChangeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TMBackupChangeForm));
            this.radioBtn_TimeChange = new System.Windows.Forms.RadioButton();
            this.radioBtn_Manually = new System.Windows.Forms.RadioButton();
            this.btn_TimeDetails = new System.Windows.Forms.Button();
            this.txtBox_timeInterval = new System.Windows.Forms.TextBox();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioBtn_TimeChange
            // 
            this.radioBtn_TimeChange.AutoSize = true;
            this.radioBtn_TimeChange.Location = new System.Drawing.Point(12, 26);
            this.radioBtn_TimeChange.Name = "radioBtn_TimeChange";
            this.radioBtn_TimeChange.Size = new System.Drawing.Size(52, 17);
            this.radioBtn_TimeChange.TabIndex = 1;
            this.radioBtn_TimeChange.TabStop = true;
            this.radioBtn_TimeChange.Text = "Every";
            this.radioBtn_TimeChange.UseVisualStyleBackColor = true;
            this.radioBtn_TimeChange.CheckedChanged += new System.EventHandler(this.radioBtn_TimeChange_CheckedChanged);
            // 
            // radioBtn_Manually
            // 
            this.radioBtn_Manually.AutoSize = true;
            this.radioBtn_Manually.Location = new System.Drawing.Point(12, 68);
            this.radioBtn_Manually.Name = "radioBtn_Manually";
            this.radioBtn_Manually.Size = new System.Drawing.Size(133, 17);
            this.radioBtn_Manually.TabIndex = 2;
            this.radioBtn_Manually.TabStop = true;
            this.radioBtn_Manually.Text = "When manually started";
            this.radioBtn_Manually.UseVisualStyleBackColor = true;
            this.radioBtn_Manually.CheckedChanged += new System.EventHandler(this.radioBtn_Manually_CheckedChanged);
            // 
            // btn_TimeDetails
            // 
            this.btn_TimeDetails.Location = new System.Drawing.Point(363, 26);
            this.btn_TimeDetails.Name = "btn_TimeDetails";
            this.btn_TimeDetails.Size = new System.Drawing.Size(100, 23);
            this.btn_TimeDetails.TabIndex = 4;
            this.btn_TimeDetails.Text = "Edit details...";
            this.btn_TimeDetails.UseVisualStyleBackColor = true;
            this.btn_TimeDetails.Click += new System.EventHandler(this.btn_TimeDetails_Click);
            // 
            // txtBox_timeInterval
            // 
            this.txtBox_timeInterval.BackColor = System.Drawing.SystemColors.MenuBar;
            this.txtBox_timeInterval.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBox_timeInterval.Location = new System.Drawing.Point(70, 49);
            this.txtBox_timeInterval.Name = "txtBox_timeInterval";
            this.txtBox_timeInterval.Size = new System.Drawing.Size(100, 13);
            this.txtBox_timeInterval.TabIndex = 5;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(363, 102);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 23);
            this.btn_Ok.TabIndex = 7;
            this.btn_Ok.Text = "Ok";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // TMBackupChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 137);
            this.Controls.Add(this.btn_Ok);
            this.Controls.Add(this.txtBox_timeInterval);
            this.Controls.Add(this.btn_TimeDetails);
            this.Controls.Add(this.radioBtn_Manually);
            this.Controls.Add(this.radioBtn_TimeChange);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(493, 180);
            this.Name = "TMBackupChangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.RadioButton radioBtn_TimeChange;
		private System.Windows.Forms.RadioButton radioBtn_Manually;
		private System.Windows.Forms.Button btn_TimeDetails;
		private System.Windows.Forms.TextBox txtBox_timeInterval;
		private System.Windows.Forms.Button btn_Ok;
	}
}