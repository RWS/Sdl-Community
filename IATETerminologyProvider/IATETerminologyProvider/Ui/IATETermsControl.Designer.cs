namespace IATETerminologyProvider.Ui
{
	partial class IATETermsControl
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
            this.lblLanguageText = new System.Windows.Forms.Label();
            this.lblDefinition = new System.Windows.Forms.Label();
            this.lblDefinitionText = new System.Windows.Forms.Label();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lblSubDomain = new System.Windows.Forms.Label();
            this.lblSubdomainText = new System.Windows.Forms.Label();
            this.lblDomainText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTerm = new System.Windows.Forms.Label();
            this.lblTermText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLanguageText
            // 
            this.lblLanguageText.AutoSize = true;
            this.lblLanguageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblLanguageText.Location = new System.Drawing.Point(38, 7);
            this.lblLanguageText.Name = "lblLanguageText";
            this.lblLanguageText.Size = new System.Drawing.Size(0, 16);
            this.lblLanguageText.TabIndex = 0;
            // 
            // lblDefinition
            // 
            this.lblDefinition.AutoSize = true;
            this.lblDefinition.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDefinition.Location = new System.Drawing.Point(20, 64);
            this.lblDefinition.Name = "lblDefinition";
            this.lblDefinition.Size = new System.Drawing.Size(54, 13);
            this.lblDefinition.TabIndex = 1;
            this.lblDefinition.Text = "Definition:";
            // 
            // lblDefinitionText
            // 
            this.lblDefinitionText.AutoSize = true;
            this.lblDefinitionText.Location = new System.Drawing.Point(80, 64);
            this.lblDefinitionText.Name = "lblDefinitionText";
            this.lblDefinitionText.Size = new System.Drawing.Size(0, 13);
            this.lblDefinitionText.TabIndex = 2;
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDomain.Location = new System.Drawing.Point(20, 86);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(46, 13);
            this.lblDomain.TabIndex = 3;
            this.lblDomain.Text = "Domain:";
            // 
            // lblSubDomain
            // 
            this.lblSubDomain.AutoSize = true;
            this.lblSubDomain.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblSubDomain.Location = new System.Drawing.Point(20, 107);
            this.lblSubDomain.Name = "lblSubDomain";
            this.lblSubDomain.Size = new System.Drawing.Size(63, 13);
            this.lblSubDomain.TabIndex = 4;
            this.lblSubDomain.Text = "Subdomain:";
            // 
            // lblSubdomainText
            // 
            this.lblSubdomainText.AutoSize = true;
            this.lblSubdomainText.Location = new System.Drawing.Point(89, 107);
            this.lblSubdomainText.Name = "lblSubdomainText";
            this.lblSubdomainText.Size = new System.Drawing.Size(0, 13);
            this.lblSubdomainText.TabIndex = 5;
            // 
            // lblDomainText
            // 
            this.lblDomainText.AutoSize = true;
            this.lblDomainText.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDomainText.Location = new System.Drawing.Point(83, 86);
            this.lblDomainText.Name = "lblDomainText";
            this.lblDomainText.Size = new System.Drawing.Size(0, 13);
            this.lblDomainText.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(10, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 23);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // lblTerm
            // 
            this.lblTerm.AutoSize = true;
            this.lblTerm.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblTerm.Location = new System.Drawing.Point(20, 42);
            this.lblTerm.Name = "lblTerm";
            this.lblTerm.Size = new System.Drawing.Size(34, 13);
            this.lblTerm.TabIndex = 8;
            this.lblTerm.Text = "Term:";
            // 
            // lblTermText
            // 
            this.lblTermText.AutoSize = true;
            this.lblTermText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblTermText.Location = new System.Drawing.Point(80, 42);
            this.lblTermText.Name = "lblTermText";
            this.lblTermText.Size = new System.Drawing.Size(0, 13);
            this.lblTermText.TabIndex = 9;
            // 
            // IATETermsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.lblTermText);
            this.Controls.Add(this.lblTerm);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblDomainText);
            this.Controls.Add(this.lblSubdomainText);
            this.Controls.Add(this.lblSubDomain);
            this.Controls.Add(this.lblDomain);
            this.Controls.Add(this.lblDefinitionText);
            this.Controls.Add(this.lblDefinition);
            this.Controls.Add(this.lblLanguageText);
            this.Name = "IATETermsControl";
            this.Size = new System.Drawing.Size(508, 506);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblLanguageText;
		private System.Windows.Forms.Label lblDefinition;
		private System.Windows.Forms.Label lblDefinitionText;
		private System.Windows.Forms.Label lblDomain;
		private System.Windows.Forms.Label lblSubDomain;
		private System.Windows.Forms.Label lblSubdomainText;
		private System.Windows.Forms.Label lblDomainText;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label lblTerm;
		private System.Windows.Forms.Label lblTermText;
	}
}
