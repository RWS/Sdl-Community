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
            System.Windows.Forms.Button btn_RealTimeDetails;
            this.radioBtn_RealTimeChange = new System.Windows.Forms.RadioButton();
            this.radioBtn_TimeChange = new System.Windows.Forms.RadioButton();
            this.radioBtn_Manually = new System.Windows.Forms.RadioButton();
            this.btn_TimeDetails = new System.Windows.Forms.Button();
            btn_RealTimeDetails = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // radioBtn_RealTimeChange
            // 
            this.radioBtn_RealTimeChange.AutoSize = true;
            this.radioBtn_RealTimeChange.Location = new System.Drawing.Point(13, 13);
            this.radioBtn_RealTimeChange.Name = "radioBtn_RealTimeChange";
            this.radioBtn_RealTimeChange.Size = new System.Drawing.Size(232, 17);
            this.radioBtn_RealTimeChange.TabIndex = 0;
            this.radioBtn_RealTimeChange.TabStop = true;
            this.radioBtn_RealTimeChange.Text = "When files or folder are modified, in real-time";
            this.radioBtn_RealTimeChange.UseVisualStyleBackColor = true;
            // 
            // radioBtn_TimeChange
            // 
            this.radioBtn_TimeChange.AutoSize = true;
            this.radioBtn_TimeChange.Location = new System.Drawing.Point(12, 47);
            this.radioBtn_TimeChange.Name = "radioBtn_TimeChange";
            this.radioBtn_TimeChange.Size = new System.Drawing.Size(90, 17);
            this.radioBtn_TimeChange.TabIndex = 1;
            this.radioBtn_TimeChange.TabStop = true;
            this.radioBtn_TimeChange.Text = "Every 6 hours";
            this.radioBtn_TimeChange.UseVisualStyleBackColor = true;
            // 
            // radioBtn_Manually
            // 
            this.radioBtn_Manually.AutoSize = true;
            this.radioBtn_Manually.Location = new System.Drawing.Point(12, 81);
            this.radioBtn_Manually.Name = "radioBtn_Manually";
            this.radioBtn_Manually.Size = new System.Drawing.Size(133, 17);
            this.radioBtn_Manually.TabIndex = 2;
            this.radioBtn_Manually.TabStop = true;
            this.radioBtn_Manually.Text = "When manually started";
            this.radioBtn_Manually.UseVisualStyleBackColor = true;
            // 
            // btn_RealTimeDetails
            // 
            btn_RealTimeDetails.Location = new System.Drawing.Point(363, 8);
            btn_RealTimeDetails.Name = "btn_RealTimeDetails";
            btn_RealTimeDetails.Size = new System.Drawing.Size(99, 26);
            btn_RealTimeDetails.TabIndex = 3;
            btn_RealTimeDetails.Text = "Edit details...";
            btn_RealTimeDetails.UseVisualStyleBackColor = true;
            btn_RealTimeDetails.Click += new System.EventHandler(this.btn_RealTimeDetails_Click);
            // 
            // btn_TimeDetails
            // 
            this.btn_TimeDetails.Location = new System.Drawing.Point(363, 42);
            this.btn_TimeDetails.Name = "btn_TimeDetails";
            this.btn_TimeDetails.Size = new System.Drawing.Size(99, 26);
            this.btn_TimeDetails.TabIndex = 4;
            this.btn_TimeDetails.Text = "Edit details...";
            this.btn_TimeDetails.UseVisualStyleBackColor = true;
            this.btn_TimeDetails.Click += new System.EventHandler(this.btn_TimeDetails_Click);
            // 
            // TMBackupChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 118);
            this.Controls.Add(this.btn_TimeDetails);
            this.Controls.Add(btn_RealTimeDetails);
            this.Controls.Add(this.radioBtn_Manually);
            this.Controls.Add(this.radioBtn_TimeChange);
            this.Controls.Add(this.radioBtn_RealTimeChange);
            this.Name = "TMBackupChangeForm";
            this.Text = "Change settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioBtn_RealTimeChange;
		private System.Windows.Forms.RadioButton radioBtn_TimeChange;
		private System.Windows.Forms.RadioButton radioBtn_Manually;
		private System.Windows.Forms.Button btn_TimeDetails;
	}
}