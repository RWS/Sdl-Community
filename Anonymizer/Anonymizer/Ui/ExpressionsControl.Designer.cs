namespace Sdl.Community.Anonymizer.Ui
{
	partial class ExpressionsControl
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
			this.expressionListGb = new System.Windows.Forms.GroupBox();
			this.expressionListLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.descriptionLbl = new System.Windows.Forms.Label();
			this.expressionsDataGrid = new System.Windows.Forms.DataGridView();
			this.buttonsLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.importBtn = new System.Windows.Forms.Button();
			this.exporBtn = new System.Windows.Forms.Button();
			this.expressionListGb.SuspendLayout();
			this.expressionListLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsDataGrid)).BeginInit();
			this.buttonsLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// expressionListGb
			// 
			this.expressionListGb.Controls.Add(this.expressionListLayoutPanel);
			this.expressionListGb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionListGb.Location = new System.Drawing.Point(0, 0);
			this.expressionListGb.Name = "expressionListGb";
			this.expressionListGb.Size = new System.Drawing.Size(662, 414);
			this.expressionListGb.TabIndex = 0;
			this.expressionListGb.TabStop = false;
			this.expressionListGb.Text = "Expression List";
			// 
			// expressionListLayoutPanel
			// 
			this.expressionListLayoutPanel.ColumnCount = 1;
			this.expressionListLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.expressionListLayoutPanel.Controls.Add(this.descriptionLbl, 0, 0);
			this.expressionListLayoutPanel.Controls.Add(this.expressionsDataGrid, 0, 1);
			this.expressionListLayoutPanel.Controls.Add(this.buttonsLayoutPanel, 0, 2);
			this.expressionListLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionListLayoutPanel.Location = new System.Drawing.Point(3, 16);
			this.expressionListLayoutPanel.Name = "expressionListLayoutPanel";
			this.expressionListLayoutPanel.RowCount = 3;
			this.expressionListLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.expressionListLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
			this.expressionListLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.expressionListLayoutPanel.Size = new System.Drawing.Size(656, 395);
			this.expressionListLayoutPanel.TabIndex = 0;
			// 
			// descriptionLbl
			// 
			this.descriptionLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.descriptionLbl.AutoSize = true;
			this.descriptionLbl.Location = new System.Drawing.Point(3, 13);
			this.descriptionLbl.Name = "descriptionLbl";
			this.descriptionLbl.Size = new System.Drawing.Size(650, 13);
			this.descriptionLbl.TabIndex = 0;
			this.descriptionLbl.Text = "Available regular expressions are shown bellow. New expressions can be added or i" +
    "mported";
			// 
			// expressionsDataGrid
			// 
			this.expressionsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.expressionsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expressionsDataGrid.Location = new System.Drawing.Point(3, 42);
			this.expressionsDataGrid.Name = "expressionsDataGrid";
			this.expressionsDataGrid.Size = new System.Drawing.Size(650, 270);
			this.expressionsDataGrid.TabIndex = 1;
			// 
			// buttonsLayoutPanel
			// 
			this.buttonsLayoutPanel.ColumnCount = 2;
			this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
			this.buttonsLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
			this.buttonsLayoutPanel.Controls.Add(this.importBtn, 0, 0);
			this.buttonsLayoutPanel.Controls.Add(this.exporBtn, 1, 0);
			this.buttonsLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonsLayoutPanel.Location = new System.Drawing.Point(3, 318);
			this.buttonsLayoutPanel.Name = "buttonsLayoutPanel";
			this.buttonsLayoutPanel.RowCount = 1;
			this.buttonsLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.buttonsLayoutPanel.Size = new System.Drawing.Size(650, 74);
			this.buttonsLayoutPanel.TabIndex = 2;
			// 
			// importBtn
			// 
			this.importBtn.Location = new System.Drawing.Point(3, 3);
			this.importBtn.Name = "importBtn";
			this.importBtn.Size = new System.Drawing.Size(102, 23);
			this.importBtn.TabIndex = 0;
			this.importBtn.Text = "Import pattern(s)";
			this.importBtn.UseVisualStyleBackColor = true;
			// 
			// exporBtn
			// 
			this.exporBtn.Location = new System.Drawing.Point(133, 3);
			this.exporBtn.Name = "exporBtn";
			this.exporBtn.Size = new System.Drawing.Size(103, 23);
			this.exporBtn.TabIndex = 1;
			this.exporBtn.Text = "Export pattern(s)";
			this.exporBtn.UseVisualStyleBackColor = true;
			// 
			// ExpressionsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.expressionListGb);
			this.Name = "ExpressionsControl";
			this.Size = new System.Drawing.Size(662, 414);
			this.expressionListGb.ResumeLayout(false);
			this.expressionListLayoutPanel.ResumeLayout(false);
			this.expressionListLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.expressionsDataGrid)).EndInit();
			this.buttonsLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox expressionListGb;
		private System.Windows.Forms.TableLayoutPanel expressionListLayoutPanel;
		private System.Windows.Forms.Label descriptionLbl;
		private System.Windows.Forms.DataGridView expressionsDataGrid;
		private System.Windows.Forms.TableLayoutPanel buttonsLayoutPanel;
		private System.Windows.Forms.Button importBtn;
		private System.Windows.Forms.Button exporBtn;
	}
}
