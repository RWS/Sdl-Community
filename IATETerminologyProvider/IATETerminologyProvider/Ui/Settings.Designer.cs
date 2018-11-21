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
            this.btn_SetSettings = new System.Windows.Forms.Button();
            this.lbl_Domain = new System.Windows.Forms.Label();
            this.ckb_AllDomains = new System.Windows.Forms.CheckBox();
            this.ckb_NoDuplicates = new System.Windows.Forms.CheckBox();
            this.ckb_NoDomains = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
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
            // lbl_Domain
            // 
            this.lbl_Domain.AutoSize = true;
            this.lbl_Domain.Location = new System.Drawing.Point(12, 9);
            this.lbl_Domain.Name = "lbl_Domain";
            this.lbl_Domain.Size = new System.Drawing.Size(43, 13);
            this.lbl_Domain.TabIndex = 0;
            this.lbl_Domain.Text = "Domain";
            // 
            // ckb_AllDomains
            // 
            this.ckb_AllDomains.AutoSize = true;
            this.ckb_AllDomains.Location = new System.Drawing.Point(101, 8);
            this.ckb_AllDomains.Name = "ckb_AllDomains";
            this.ckb_AllDomains.Size = new System.Drawing.Size(81, 17);
            this.ckb_AllDomains.TabIndex = 3;
            this.ckb_AllDomains.Text = "All Domains";
            this.ckb_AllDomains.UseVisualStyleBackColor = true;
            // 
            // ckb_NoDuplicates
            // 
            this.ckb_NoDuplicates.AutoSize = true;
            this.ckb_NoDuplicates.Location = new System.Drawing.Point(101, 54);
            this.ckb_NoDuplicates.Name = "ckb_NoDuplicates";
            this.ckb_NoDuplicates.Size = new System.Drawing.Size(119, 17);
            this.ckb_NoDuplicates.TabIndex = 4;
            this.ckb_NoDuplicates.Text = "No terms duplicates";
            this.ckb_NoDuplicates.UseVisualStyleBackColor = true;
            // 
            // ckb_NoDomains
            // 
            this.ckb_NoDomains.AutoSize = true;
            this.ckb_NoDomains.Location = new System.Drawing.Point(101, 31);
            this.ckb_NoDomains.Name = "ckb_NoDomains";
            this.ckb_NoDomains.Size = new System.Drawing.Size(82, 17);
            this.ckb_NoDomains.TabIndex = 5;
            this.ckb_NoDomains.Text = "No domains";
            this.ckb_NoDomains.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(340, 108);
            this.Controls.Add(this.ckb_NoDomains);
            this.Controls.Add(this.ckb_NoDuplicates);
            this.Controls.Add(this.ckb_AllDomains);
            this.Controls.Add(this.btn_SetSettings);
            this.Controls.Add(this.lbl_Domain);
            this.Name = "Settings";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button btn_SetSettings;
		private System.Windows.Forms.Label lbl_Domain;
		private System.Windows.Forms.CheckBox ckb_AllDomains;
		private System.Windows.Forms.CheckBox ckb_NoDuplicates;
		private System.Windows.Forms.CheckBox ckb_NoDomains;
	}
}
