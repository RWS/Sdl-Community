namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	partial class BatchAnonymizerSettingsControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.descriptonLbl = new System.Windows.Forms.Label();
			this.completeBtn = new System.Windows.Forms.RadioButton();
			this.tmMatchBtn = new System.Windows.Forms.RadioButton();
			this.scoreLbl = new System.Windows.Forms.Label();
			this.tmNameLbl = new System.Windows.Forms.Label();
			this.tmNameBox = new System.Windows.Forms.TextBox();
			this.scoreBox = new System.Windows.Forms.NumericUpDown();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scoreBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.descriptonLbl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.completeBtn, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tmMatchBtn, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.scoreLbl, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.tmNameLbl, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.tmNameBox, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.scoreBox, 1, 3);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 5;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(707, 521);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// descriptonLbl
			// 
			this.descriptonLbl.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.descriptonLbl, 2);
			this.descriptonLbl.Dock = System.Windows.Forms.DockStyle.Top;
			this.descriptonLbl.Location = new System.Drawing.Point(3, 0);
			this.descriptonLbl.Name = "descriptonLbl";
			this.descriptonLbl.Size = new System.Drawing.Size(701, 13);
			this.descriptonLbl.TabIndex = 0;
			this.descriptonLbl.Text = "Please select anonymization option";
			// 
			// completeBtn
			// 
			this.completeBtn.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.completeBtn, 2);
			this.completeBtn.Dock = System.Windows.Forms.DockStyle.Top;
			this.completeBtn.Location = new System.Drawing.Point(3, 23);
			this.completeBtn.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
			this.completeBtn.Name = "completeBtn";
			this.completeBtn.Size = new System.Drawing.Size(701, 17);
			this.completeBtn.TabIndex = 1;
			this.completeBtn.TabStop = true;
			this.completeBtn.Text = "Anonymize complete";
			this.completeBtn.UseVisualStyleBackColor = true;
			// 
			// tmMatchBtn
			// 
			this.tmMatchBtn.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this.tmMatchBtn, 2);
			this.tmMatchBtn.Location = new System.Drawing.Point(3, 46);
			this.tmMatchBtn.Name = "tmMatchBtn";
			this.tmMatchBtn.Size = new System.Drawing.Size(204, 17);
			this.tmMatchBtn.TabIndex = 2;
			this.tmMatchBtn.TabStop = true;
			this.tmMatchBtn.Text = "Anonymize Translation Memory Match";
			this.tmMatchBtn.UseVisualStyleBackColor = true;
			// 
			// scoreLbl
			// 
			this.scoreLbl.AutoSize = true;
			this.scoreLbl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scoreLbl.Location = new System.Drawing.Point(3, 66);
			this.scoreLbl.Name = "scoreLbl";
			this.scoreLbl.Size = new System.Drawing.Size(54, 26);
			this.scoreLbl.TabIndex = 4;
			this.scoreLbl.Text = "Score";
			this.scoreLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tmNameLbl
			// 
			this.tmNameLbl.AutoSize = true;
			this.tmNameLbl.Dock = System.Windows.Forms.DockStyle.Top;
			this.tmNameLbl.Location = new System.Drawing.Point(3, 97);
			this.tmNameLbl.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
			this.tmNameLbl.Name = "tmNameLbl";
			this.tmNameLbl.Size = new System.Drawing.Size(54, 13);
			this.tmNameLbl.TabIndex = 5;
			this.tmNameLbl.Text = "TM Name";
			this.tmNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tmNameBox
			// 
			this.tmNameBox.Location = new System.Drawing.Point(63, 95);
			this.tmNameBox.Name = "tmNameBox";
			this.tmNameBox.Size = new System.Drawing.Size(185, 20);
			this.tmNameBox.TabIndex = 6;
			// 
			// scoreBox
			// 
			this.scoreBox.Location = new System.Drawing.Point(63, 69);
			this.scoreBox.Name = "scoreBox";
			this.scoreBox.Size = new System.Drawing.Size(185, 20);
			this.scoreBox.TabIndex = 7;
			// 
			// BatchAnonymizerSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "BatchAnonymizerSettingsControl";
			this.Size = new System.Drawing.Size(707, 521);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.scoreBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label descriptonLbl;
		private System.Windows.Forms.RadioButton completeBtn;
		private System.Windows.Forms.RadioButton tmMatchBtn;
		private System.Windows.Forms.Label scoreLbl;
		private System.Windows.Forms.Label tmNameLbl;
		private System.Windows.Forms.TextBox tmNameBox;
		private System.Windows.Forms.NumericUpDown scoreBox;
	}
}
