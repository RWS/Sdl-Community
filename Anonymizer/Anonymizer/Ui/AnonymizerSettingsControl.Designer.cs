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
			this.mainPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsGrid)).BeginInit();
			this.descriptionPanel.SuspendLayout();
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
			this.mainPanel.Size = new System.Drawing.Size(556, 428);
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
			this.expressionsGrid.Size = new System.Drawing.Size(550, 186);
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
			this.exportBtn.Location = new System.Drawing.Point(169, 329);
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
			this.encryptionBox.Location = new System.Drawing.Point(169, 385);
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
			this.descriptionPanel.Size = new System.Drawing.Size(550, 122);
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
			this.descriptionPanel.ResumeLayout(false);
			this.descriptionPanel.PerformLayout();
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
	}
}
