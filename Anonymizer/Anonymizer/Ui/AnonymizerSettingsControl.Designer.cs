namespace Sdl.Community.projectAnonymizer.Ui
{
	partial class AnonymizerSettingsControl
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
			this.mainPanel = new System.Windows.Forms.TableLayoutPanel();
			this.expressionsGrid = new System.Windows.Forms.DataGridView();
			this.importBtn = new System.Windows.Forms.Button();
			this.exportBtn = new System.Windows.Forms.Button();
			this.encryptionLbl = new System.Windows.Forms.Label();
			this.encryptionBox = new System.Windows.Forms.TextBox();
			this.descriptionPanel = new System.Windows.Forms.TableLayoutPanel();
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.selectAll = new System.Windows.Forms.CheckBox();
			this.encryptedPanel = new System.Windows.Forms.TableLayoutPanel();
			this.lockPictureBox = new System.Windows.Forms.PictureBox();
			this.decryptionPanel = new System.Windows.Forms.Panel();
			this.keyTextBox = new System.Windows.Forms.TextBox();
			this.decryptButton = new System.Windows.Forms.Button();
			this.encryptedMessage = new System.Windows.Forms.Label();
			this.errorLabel = new System.Windows.Forms.Label();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsGrid)).BeginInit();
			this.descriptionPanel.SuspendLayout();
			this.encryptedPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lockPictureBox)).BeginInit();
			this.decryptionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.ColumnCount = 2;
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.90133F));
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.09867F));
			this.mainPanel.Controls.Add(this.expressionsGrid, 0, 1);
			this.mainPanel.Controls.Add(this.importBtn, 0, 2);
			this.mainPanel.Controls.Add(this.exportBtn, 1, 2);
			this.mainPanel.Controls.Add(this.encryptionLbl, 0, 3);
			this.mainPanel.Controls.Add(this.encryptionBox, 1, 3);
			this.mainPanel.Controls.Add(this.descriptionPanel, 0, 0);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.RowCount = 4;
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
			this.mainPanel.Size = new System.Drawing.Size(829, 428);
			this.mainPanel.TabIndex = 0;
			// 
			// expressionsGrid
			// 
			this.expressionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.mainPanel.SetColumnSpan(this.expressionsGrid, 2);
			this.expressionsGrid.Cursor = System.Windows.Forms.Cursors.Hand;
			this.expressionsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionsGrid.Location = new System.Drawing.Point(3, 131);
			this.expressionsGrid.Name = "expressionsGrid";
			this.expressionsGrid.Size = new System.Drawing.Size(823, 186);
			this.expressionsGrid.TabIndex = 1;
			this.expressionsGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.expressionsGrid_CellValueChanged);
			this.expressionsGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.expressionsGrid_KeyDown);
			// 
			// importBtn
			// 
			this.importBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.importBtn.Location = new System.Drawing.Point(3, 329);
			this.importBtn.Name = "importBtn";
			this.importBtn.Size = new System.Drawing.Size(109, 23);
			this.importBtn.TabIndex = 2;
			this.importBtn.Text = "Import rules(s)";
			this.importBtn.UseVisualStyleBackColor = true;
			this.importBtn.Click += new System.EventHandler(this.importBtn_Click);
			// 
			// exportBtn
			// 
			this.exportBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.exportBtn.Location = new System.Drawing.Point(250, 329);
			this.exportBtn.Name = "exportBtn";
			this.exportBtn.Size = new System.Drawing.Size(108, 23);
			this.exportBtn.TabIndex = 3;
			this.exportBtn.Text = "Export rules(s)";
			this.exportBtn.UseVisualStyleBackColor = true;
			this.exportBtn.Click += new System.EventHandler(this.exportBtn_Click);
			// 
			// encryptionLbl
			// 
			this.encryptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.encryptionLbl.AutoSize = true;
			this.encryptionLbl.Location = new System.Drawing.Point(3, 388);
			this.encryptionLbl.Name = "encryptionLbl";
			this.encryptionLbl.Size = new System.Drawing.Size(0, 13);
			this.encryptionLbl.TabIndex = 4;
			// 
			// encryptionBox
			// 
			this.encryptionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.encryptionBox.Location = new System.Drawing.Point(250, 385);
			this.encryptionBox.Name = "encryptionBox";
			this.encryptionBox.PasswordChar = '*';
			this.encryptionBox.Size = new System.Drawing.Size(214, 20);
			this.encryptionBox.TabIndex = 5;
			// 
			// descriptionPanel
			// 
			this.descriptionPanel.ColumnCount = 1;
			this.mainPanel.SetColumnSpan(this.descriptionPanel, 2);
			this.descriptionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.descriptionPanel.Controls.Add(this.descriptionLbl, 0, 0);
			this.descriptionPanel.Controls.Add(this.selectAll, 0, 1);
			this.descriptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.descriptionPanel.Location = new System.Drawing.Point(3, 3);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.RowCount = 2;
			this.descriptionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.descriptionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.descriptionPanel.Size = new System.Drawing.Size(823, 122);
			this.descriptionPanel.TabIndex = 6;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descriptionLbl.AutoSize = true;
			this.descriptionLbl.Location = new System.Drawing.Point(3, 42);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(35, 13);
			this.descriptionLbl.TabIndex = 0;
			this.descriptionLbl.Text = "label1";
			// 
			// selectAll
			// 
			this.selectAll.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.selectAll.AutoSize = true;
			this.selectAll.Location = new System.Drawing.Point(3, 101);
			this.selectAll.Name = "selectAll";
			this.selectAll.Size = new System.Drawing.Size(69, 17);
			this.selectAll.TabIndex = 1;
			this.selectAll.Text = "Select all";
			this.selectAll.UseVisualStyleBackColor = true;
			this.selectAll.CheckedChanged += new System.EventHandler(this.selectAll_CheckedChanged);
			// 
			// encryptedPanel
			// 
			this.encryptedPanel.ColumnCount = 3;
			this.encryptedPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.encryptedPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.encryptedPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.encryptedPanel.Controls.Add(this.lockPictureBox, 1, 1);
			this.encryptedPanel.Controls.Add(this.decryptionPanel, 1, 5);
			this.encryptedPanel.Controls.Add(this.encryptedMessage, 1, 3);
			this.encryptedPanel.Controls.Add(this.errorLabel, 1, 6);
			this.encryptedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.encryptedPanel.Location = new System.Drawing.Point(0, 0);
			this.encryptedPanel.Name = "encryptedPanel";
			this.encryptedPanel.RowCount = 8;
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.encryptedPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.encryptedPanel.Size = new System.Drawing.Size(829, 428);
			this.encryptedPanel.TabIndex = 10;
			this.encryptedPanel.Visible = false;
			// 
			// lockPictureBox
			// 
			this.lockPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.lockPictureBox.Image = global::Sdl.Community.projectAnonymizer.PluginResources.lockxxx;
			this.lockPictureBox.Location = new System.Drawing.Point(350, 17);
			this.lockPictureBox.Name = "lockPictureBox";
			this.lockPictureBox.Size = new System.Drawing.Size(129, 132);
			this.lockPictureBox.TabIndex = 8;
			this.lockPictureBox.TabStop = false;
			// 
			// decryptionPanel
			// 
			this.decryptionPanel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.decryptionPanel.Controls.Add(this.keyTextBox);
			this.decryptionPanel.Controls.Add(this.decryptButton);
			this.decryptionPanel.Location = new System.Drawing.Point(243, 208);
			this.decryptionPanel.Name = "decryptionPanel";
			this.decryptionPanel.Size = new System.Drawing.Size(342, 32);
			this.decryptionPanel.TabIndex = 9;
			// 
			// keyTextBox
			// 
			this.keyTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.keyTextBox.CausesValidation = false;
			this.keyTextBox.Location = new System.Drawing.Point(5, 5);
			this.keyTextBox.Multiline = true;
			this.keyTextBox.Name = "keyTextBox";
			this.keyTextBox.PasswordChar = '*';
			this.keyTextBox.Size = new System.Drawing.Size(231, 21);
			this.keyTextBox.TabIndex = 5;
			// 
			// decryptButton
			// 
			this.decryptButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.decryptButton.Location = new System.Drawing.Point(242, 4);
			this.decryptButton.Name = "decryptButton";
			this.decryptButton.Size = new System.Drawing.Size(97, 23);
			this.decryptButton.TabIndex = 6;
			this.decryptButton.Text = "Decrypt";
			this.decryptButton.UseVisualStyleBackColor = true;
			this.decryptButton.Click += new System.EventHandler(this.decryptButton_Click);
			// 
			// encryptedMessage
			// 
			this.encryptedMessage.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.encryptedMessage.AutoEllipsis = true;
			this.encryptedMessage.AutoSize = true;
			this.encryptedMessage.CausesValidation = false;
			this.encryptedMessage.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.encryptedMessage.Location = new System.Drawing.Point(177, 166);
			this.encryptedMessage.Name = "encryptedMessage";
			this.encryptedMessage.Size = new System.Drawing.Size(475, 25);
			this.encryptedMessage.TabIndex = 7;
			this.encryptedMessage.Text = "Settings are encrypted. Provide the key to edit patterns:";
			this.encryptedMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// errorLabel
			// 
			this.errorLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.errorLabel.AutoEllipsis = true;
			this.errorLabel.AutoSize = true;
			this.errorLabel.CausesValidation = false;
			this.errorLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.errorLabel.ForeColor = System.Drawing.Color.Red;
			this.errorLabel.Location = new System.Drawing.Point(268, 243);
			this.errorLabel.Name = "errorLabel";
			this.errorLabel.Size = new System.Drawing.Size(293, 17);
			this.errorLabel.TabIndex = 9;
			this.errorLabel.Text = "Decryption key doesn\'t match the encryption key.";
			this.errorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.errorLabel.Visible = false;
			// 
			// AnonymizerSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.encryptedPanel);
			this.Controls.Add(this.mainPanel);
			this.Name = "AnonymizerSettingsControl";
			this.Size = new System.Drawing.Size(829, 428);
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsGrid)).EndInit();
			this.descriptionPanel.ResumeLayout(false);
			this.descriptionPanel.PerformLayout();
			this.encryptedPanel.ResumeLayout(false);
			this.encryptedPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lockPictureBox)).EndInit();
			this.decryptionPanel.ResumeLayout(false);
			this.decryptionPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainPanel;
		private System.Windows.Forms.DataGridView expressionsGrid;
		private System.Windows.Forms.Button importBtn;
		private System.Windows.Forms.Button exportBtn;
		private System.Windows.Forms.Label encryptionLbl;
		private System.Windows.Forms.TextBox encryptionBox;
		private System.Windows.Forms.TableLayoutPanel descriptionPanel;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.CheckBox selectAll;
		private System.Windows.Forms.TextBox keyTextBox;
		private System.Windows.Forms.Button decryptButton;
		private System.Windows.Forms.Label encryptedMessage;
		private System.Windows.Forms.PictureBox lockPictureBox;
		private System.Windows.Forms.Label errorLabel;
		private System.Windows.Forms.TableLayoutPanel encryptedPanel;
		private System.Windows.Forms.Panel decryptionPanel;
	}
}
