namespace Sdl.Community.projectAnonymizer.Ui
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
			this.ignoreEncrypted = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.messageLbl = new System.Windows.Forms.Label();
			this.decryptPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// decryptPanel
			// 
			this.decryptPanel.ColumnCount = 2;
			this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
			this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
			this.decryptPanel.Controls.Add(this.descriptionLbl, 0, 0);
			this.decryptPanel.Controls.Add(this.encryptionBox, 1, 0);
			this.decryptPanel.Controls.Add(this.ignoreEncrypted, 0, 1);
			this.decryptPanel.Controls.Add(this.label1, 1, 1);
			this.decryptPanel.Controls.Add(this.messageLbl, 0, 2);
			this.decryptPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.decryptPanel.Location = new System.Drawing.Point(0, 0);
			this.decryptPanel.Name = "decryptPanel";
			this.decryptPanel.RowCount = 3;
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.decryptPanel.Size = new System.Drawing.Size(717, 434);
			this.decryptPanel.TabIndex = 0;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descriptionLbl.AutoSize = true;
			this.descriptionLbl.Location = new System.Drawing.Point(3, 14);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(205, 13);
			this.descriptionLbl.TabIndex = 0;
			this.descriptionLbl.Text = "Enter the key you used to encrypt the files";
			// 
			// encryptionBox
			// 
			this.encryptionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.encryptionBox.Location = new System.Drawing.Point(289, 10);
			this.encryptionBox.Name = "encryptionBox";
			this.encryptionBox.PasswordChar = '*';
			this.encryptionBox.Size = new System.Drawing.Size(119, 20);
			this.encryptionBox.TabIndex = 1;
			// 
			// ignoreEncrypted
			// 
			this.ignoreEncrypted.AutoSize = true;
			this.ignoreEncrypted.Location = new System.Drawing.Point(3, 44);
			this.ignoreEncrypted.Name = "ignoreEncrypted";
			this.ignoreEncrypted.Size = new System.Drawing.Size(130, 17);
			this.ignoreEncrypted.TabIndex = 2;
			this.ignoreEncrypted.Text = "Ignore encrypted data";
			this.ignoreEncrypted.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(289, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(332, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "If this option is enabled only the tagged data will be converted in text.";
			// 
			// messageLbl
			// 
			this.messageLbl.AutoSize = true;
			this.decryptPanel.SetColumnSpan(this.messageLbl, 2);
			this.messageLbl.Location = new System.Drawing.Point(3, 413);
			this.messageLbl.Name = "messageLbl";
			this.messageLbl.Size = new System.Drawing.Size(35, 13);
			this.messageLbl.TabIndex = 4;
			this.messageLbl.Text = "label2";
			this.messageLbl.Visible = false;
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
		private System.Windows.Forms.CheckBox ignoreEncrypted;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label messageLbl;
	}
}
