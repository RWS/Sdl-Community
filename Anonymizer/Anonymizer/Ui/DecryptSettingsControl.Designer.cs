namespace Sdl.Community.Anonymizer.Ui
{
	partial class DecryptSettingsControl
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
			this.decryptPanel = new System.Windows.Forms.TableLayoutPanel();
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.encryptionBox = new System.Windows.Forms.TextBox();
			this.decryptPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// decryptPanel
			// 
			this.decryptPanel.ColumnCount = 1;
			this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.decryptPanel.Controls.Add(this.descriptionLbl, 0, 0);
			this.decryptPanel.Controls.Add(this.encryptionBox, 0, 1);
			this.decryptPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.decryptPanel.Location = new System.Drawing.Point(0, 0);
			this.decryptPanel.Name = "decryptPanel";
			this.decryptPanel.RowCount = 2;
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this.decryptPanel.Size = new System.Drawing.Size(717, 434);
			this.decryptPanel.TabIndex = 0;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descriptionLbl.AutoSize = true;
			this.descriptionLbl.Location = new System.Drawing.Point(3, 15);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(224, 13);
			this.descriptionLbl.TabIndex = 0;
			this.descriptionLbl.Text = "Please fill the key you used to encrypt the files";
			// 
			// encryptionBox
			// 
			this.encryptionBox.Location = new System.Drawing.Point(3, 46);
			this.encryptionBox.Name = "encryptionBox";
			this.encryptionBox.PasswordChar = '*';
			this.encryptionBox.Size = new System.Drawing.Size(119, 20);
			this.encryptionBox.TabIndex = 1;
			// 
			// DecryptSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.decryptPanel);
			this.Name = "DecryptSettingsControl";
			this.Size = new System.Drawing.Size(717, 434);
			this.decryptPanel.ResumeLayout(false);
			this.decryptPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel decryptPanel;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.TextBox encryptionBox;
	}
}
