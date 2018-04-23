namespace Sdl.Community.Anonymizer.Ui
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
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.expressionsGrid = new System.Windows.Forms.DataGridView();
			this.importBtn = new System.Windows.Forms.Button();
			this.exportBtn = new System.Windows.Forms.Button();
			this.encryptionLbl = new System.Windows.Forms.Label();
			this.encryptionBox = new System.Windows.Forms.TextBox();
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// mainPanel
			// 
			this.mainPanel.ColumnCount = 2;
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.61871F));
			this.mainPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.38129F));
			this.mainPanel.Controls.Add(this.descriptionLbl, 0, 0);
			this.mainPanel.Controls.Add(this.expressionsGrid, 0, 1);
			this.mainPanel.Controls.Add(this.importBtn, 0, 2);
			this.mainPanel.Controls.Add(this.exportBtn, 1, 2);
			this.mainPanel.Controls.Add(this.encryptionLbl, 0, 3);
			this.mainPanel.Controls.Add(this.encryptionBox, 1, 3);
			this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainPanel.Location = new System.Drawing.Point(0, 0);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.RowCount = 4;
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.mainPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.mainPanel.Size = new System.Drawing.Size(556, 428);
			this.mainPanel.TabIndex = 0;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.descriptionLbl.AutoSize = true;
			this.mainPanel.SetColumnSpan(this.descriptionLbl, 2);
			this.descriptionLbl.Location = new System.Drawing.Point(3, 14);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(0, 13);
			this.descriptionLbl.TabIndex = 0;
			// 
			// expressionsGrid
			// 
			this.expressionsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.mainPanel.SetColumnSpan(this.expressionsGrid, 2);
			this.expressionsGrid.Cursor = System.Windows.Forms.Cursors.Hand;
			this.expressionsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionsGrid.Location = new System.Drawing.Point(3, 45);
			this.expressionsGrid.Name = "expressionsGrid";
			this.expressionsGrid.Size = new System.Drawing.Size(550, 208);
			this.expressionsGrid.TabIndex = 1;
			this.expressionsGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.expressionsGrid_CellValueChanged);
			// 
			// importBtn
			// 
			this.importBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.importBtn.Location = new System.Drawing.Point(3, 287);
			this.importBtn.Name = "importBtn";
			this.importBtn.Size = new System.Drawing.Size(109, 23);
			this.importBtn.TabIndex = 2;
			this.importBtn.Text = "Import pattern(s)";
			this.importBtn.UseVisualStyleBackColor = true;
			// 
			// exportBtn
			// 
			this.exportBtn.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.exportBtn.Location = new System.Drawing.Point(151, 287);
			this.exportBtn.Name = "exportBtn";
			this.exportBtn.Size = new System.Drawing.Size(108, 23);
			this.exportBtn.TabIndex = 3;
			this.exportBtn.Text = "Export pattern(s)";
			this.exportBtn.UseVisualStyleBackColor = true;
			// 
			// encryptionLbl
			// 
			this.encryptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.encryptionLbl.AutoSize = true;
			this.encryptionLbl.Location = new System.Drawing.Point(3, 378);
			this.encryptionLbl.Name = "encryptionLbl";
			this.encryptionLbl.Size = new System.Drawing.Size(132, 13);
			this.encryptionLbl.TabIndex = 4;
			this.encryptionLbl.Text = "Please add encryption key";
			// 
			// encryptionBox
			// 
			this.encryptionBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.encryptionBox.Location = new System.Drawing.Point(151, 374);
			this.encryptionBox.Name = "encryptionBox";
			this.encryptionBox.PasswordChar = '*';
			this.encryptionBox.Size = new System.Drawing.Size(214, 20);
			this.encryptionBox.TabIndex = 5;
			// 
			// AnonymizerSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.mainPanel);
			this.Name = "AnonymizerSettingsControl";
			this.Size = new System.Drawing.Size(556, 428);
			this.mainPanel.ResumeLayout(false);
			this.mainPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel mainPanel;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.DataGridView expressionsGrid;
		private System.Windows.Forms.Button importBtn;
		private System.Windows.Forms.Button exportBtn;
		private System.Windows.Forms.Label encryptionLbl;
		private System.Windows.Forms.TextBox encryptionBox;
	}
}
