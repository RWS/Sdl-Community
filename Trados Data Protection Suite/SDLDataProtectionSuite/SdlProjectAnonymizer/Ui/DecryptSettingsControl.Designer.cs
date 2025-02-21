﻿namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui
{
	partial class DecryptSettingsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.TableLayoutPanel decryptPanel;

		private System.Windows.Forms.PictureBox lockPictureBox;

		private System.Windows.Forms.Label encryptedMessage;

		private System.Windows.Forms.Panel decryptionPanel;

		private System.Windows.Forms.TextBox decryptionBox;

		private System.Windows.Forms.Label messageLbl;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecryptSettingsControl));
            this.decryptPanel = new System.Windows.Forms.TableLayoutPanel();
            this.messageLbl = new System.Windows.Forms.Label();
            this.decryptionPanel = new System.Windows.Forms.Panel();
            this.decryptionBox = new System.Windows.Forms.TextBox();
            this.encryptedMessage = new System.Windows.Forms.Label();
            this.lockPictureBox = new System.Windows.Forms.PictureBox();
            this.decryptPanel.SuspendLayout();
            this.decryptionPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // decryptPanel
            // 
            this.decryptPanel.AutoSize = true;
            this.decryptPanel.ColumnCount = 3;
            this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.decryptPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.decryptPanel.Controls.Add(this.messageLbl, 1, 7);
            this.decryptPanel.Controls.Add(this.decryptionPanel, 1, 5);
            this.decryptPanel.Controls.Add(this.encryptedMessage, 1, 3);
            this.decryptPanel.Controls.Add(this.lockPictureBox, 1, 1);
            this.decryptPanel.Location = new System.Drawing.Point(0, 0);
            this.decryptPanel.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.decryptPanel.Name = "decryptPanel";
            this.decryptPanel.RowCount = 10;
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.decryptPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.decryptPanel.Size = new System.Drawing.Size(1913, 1035);
            this.decryptPanel.TabIndex = 0;
            // 
            // messageLbl
            // 
            this.messageLbl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.messageLbl.AutoEllipsis = true;
            this.messageLbl.AutoSize = true;
            this.messageLbl.CausesValidation = false;
            this.messageLbl.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.messageLbl.Location = new System.Drawing.Point(956, 624);
            this.messageLbl.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.messageLbl.Name = "messageLbl";
            this.messageLbl.Size = new System.Drawing.Size(0, 65);
            this.messageLbl.TabIndex = 12;
            this.messageLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.messageLbl.Visible = false;
            // 
            // decryptionPanel
            // 
            this.decryptionPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.decryptionPanel.Controls.Add(this.decryptionBox);
            this.decryptionPanel.Location = new System.Drawing.Point(500, 506);
            this.decryptionPanel.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.decryptionPanel.Name = "decryptionPanel";
            this.decryptionPanel.Size = new System.Drawing.Size(912, 76);
            this.decryptionPanel.TabIndex = 11;
            // 
            // decryptionBox
            // 
            this.decryptionBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.decryptionBox.Location = new System.Drawing.Point(0, 0);
            this.decryptionBox.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.decryptionBox.Name = "decryptionBox";
            this.decryptionBox.PasswordChar = '*';
            this.decryptionBox.Size = new System.Drawing.Size(905, 61);
            this.decryptionBox.TabIndex = 0;
            // 
            // encryptedMessage
            // 
            this.encryptedMessage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.encryptedMessage.AutoEllipsis = true;
            this.encryptedMessage.AutoSize = true;
            this.encryptedMessage.CausesValidation = false;
            this.encryptedMessage.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.encryptedMessage.Location = new System.Drawing.Point(686, 399);
            this.encryptedMessage.Margin = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.encryptedMessage.Name = "encryptedMessage";
            this.encryptedMessage.Size = new System.Drawing.Size(541, 65);
            this.encryptedMessage.TabIndex = 10;
            this.encryptedMessage.Text = "Enter the encryption key";
            this.encryptedMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lockPictureBox
            // 
            this.lockPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lockPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("lockPictureBox.Image")));
            this.lockPictureBox.Location = new System.Drawing.Point(784, 42);
            this.lockPictureBox.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.lockPictureBox.Name = "lockPictureBox";
            this.lockPictureBox.Size = new System.Drawing.Size(345, 315);
            this.lockPictureBox.TabIndex = 9;
            this.lockPictureBox.TabStop = false;
            // 
            // DecryptSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.decryptPanel);
            this.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.Name = "DecryptSettingsControl";
            this.Size = new System.Drawing.Size(1913, 1035);
            this.decryptPanel.ResumeLayout(false);
            this.decryptPanel.PerformLayout();
            this.decryptionPanel.ResumeLayout(false);
            this.decryptionPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lockPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
	}
}
