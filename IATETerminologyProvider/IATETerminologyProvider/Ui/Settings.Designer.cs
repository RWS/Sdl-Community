namespace IATETerminologyProvider.Ui
{
	partial class Settings
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
            this.lbl_Limit = new System.Windows.Forms.Label();
            this.txtBox_Limit = new System.Windows.Forms.TextBox();
            this.btn_SetSettings = new System.Windows.Forms.Button();
            this.lbl_Offset = new System.Windows.Forms.Label();
            this.txtBox_Offset = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbl_Limit
            // 
            this.lbl_Limit.AutoSize = true;
            this.lbl_Limit.Location = new System.Drawing.Point(12, 19);
            this.lbl_Limit.Name = "lbl_Limit";
            this.lbl_Limit.Size = new System.Drawing.Size(28, 13);
            this.lbl_Limit.TabIndex = 0;
            this.lbl_Limit.Text = "Limit";
            // 
            // txtBox_Limit
            // 
            this.txtBox_Limit.Location = new System.Drawing.Point(208, 16);
            this.txtBox_Limit.Name = "txtBox_Limit";
            this.txtBox_Limit.Size = new System.Drawing.Size(118, 20);
            this.txtBox_Limit.TabIndex = 1;
            // 
            // btn_SetSettings
            // 
            this.btn_SetSettings.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btn_SetSettings.Location = new System.Drawing.Point(253, 73);
            this.btn_SetSettings.Name = "btn_SetSettings";
            this.btn_SetSettings.Size = new System.Drawing.Size(75, 23);
            this.btn_SetSettings.TabIndex = 2;
            this.btn_SetSettings.Text = "Submit";
            this.btn_SetSettings.UseVisualStyleBackColor = true;
            // 
            // lbl_Offset
            // 
            this.lbl_Offset.AutoSize = true;
            this.lbl_Offset.Location = new System.Drawing.Point(12, 44);
            this.lbl_Offset.Name = "lbl_Offset";
            this.lbl_Offset.Size = new System.Drawing.Size(35, 13);
            this.lbl_Offset.TabIndex = 3;
            this.lbl_Offset.Text = "Offset";
            // 
            // txtBox_Offset
            // 
            this.txtBox_Offset.Location = new System.Drawing.Point(208, 41);
            this.txtBox_Offset.Name = "txtBox_Offset";
            this.txtBox_Offset.Size = new System.Drawing.Size(118, 20);
            this.txtBox_Offset.TabIndex = 4;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(340, 108);
            this.Controls.Add(this.txtBox_Offset);
            this.Controls.Add(this.lbl_Offset);
            this.Controls.Add(this.btn_SetSettings);
            this.Controls.Add(this.txtBox_Limit);
            this.Controls.Add(this.lbl_Limit);
            this.Name = "Settings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lbl_Limit;
		private System.Windows.Forms.TextBox txtBox_Limit;
		private System.Windows.Forms.Button btn_SetSettings;
		private System.Windows.Forms.Label lbl_Offset;
		private System.Windows.Forms.TextBox txtBox_Offset;
	}
}