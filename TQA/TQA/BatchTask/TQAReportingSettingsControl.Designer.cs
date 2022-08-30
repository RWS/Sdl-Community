
namespace Sdl.Community.TQA.BatchTask
{
	partial class TQAReportingSettingsControl
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.TQAQualityLevelComboBox = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.QualityLevelLbl = new System.Windows.Forms.Label();
			this.TQAProfileName = new System.Windows.Forms.Label();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.tableLayoutPanel1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(2);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(5, 10, 5, 5);
			this.panel1.Size = new System.Drawing.Size(551, 63);
			this.panel1.TabIndex = 4;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.TQAQualityLevelComboBox, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.QualityLevelLbl, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.TQAProfileName, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 10);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(541, 48);
			this.tableLayoutPanel1.TabIndex = 12;
			// 
			// TQAQualityLevelComboBox
			// 
			this.TQAQualityLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TQAQualityLevelComboBox.FormattingEnabled = true;
			this.TQAQualityLevelComboBox.Location = new System.Drawing.Point(81, 27);
			this.TQAQualityLevelComboBox.Name = "TQAQualityLevelComboBox";
			this.TQAQualityLevelComboBox.Size = new System.Drawing.Size(209, 21);
			this.TQAQualityLevelComboBox.TabIndex = 1;
			this.TQAQualityLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.TQAQualityLevelComboBox_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(3, 3);
			this.label1.Margin = new System.Windows.Forms.Padding(3);
			this.label1.Name = "label1";
			this.label1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.label1.Size = new System.Drawing.Size(72, 18);
			this.label1.TabIndex = 12;
			this.label1.Text = "TQA Profile:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// QualityLevelLbl
			// 
			this.QualityLevelLbl.AutoSize = true;
			this.QualityLevelLbl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.QualityLevelLbl.Location = new System.Drawing.Point(3, 27);
			this.QualityLevelLbl.Margin = new System.Windows.Forms.Padding(3);
			this.QualityLevelLbl.Name = "QualityLevelLbl";
			this.QualityLevelLbl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
			this.QualityLevelLbl.Size = new System.Drawing.Size(72, 18);
			this.QualityLevelLbl.TabIndex = 8;
			this.QualityLevelLbl.Text = "Quality level:";
			this.QualityLevelLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TQAProfileName
			// 
			this.TQAProfileName.AutoEllipsis = true;
			this.TQAProfileName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TQAProfileName.Location = new System.Drawing.Point(78, 0);
			this.TQAProfileName.Margin = new System.Windows.Forms.Padding(0);
			this.TQAProfileName.Name = "TQAProfileName";
			this.TQAProfileName.Padding = new System.Windows.Forms.Padding(3);
			this.TQAProfileName.Size = new System.Drawing.Size(463, 24);
			this.TQAProfileName.TabIndex = 11;
			this.TQAProfileName.Text = "TQAProfileName";
			this.TQAProfileName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TQAReportingSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Name = "TQAReportingSettingsControl";
			this.Size = new System.Drawing.Size(551, 278);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label TQAProfileName;
		private System.Windows.Forms.ComboBox TQAQualityLevelComboBox;
		private System.Windows.Forms.Label QualityLevelLbl;
		private System.Windows.Forms.Label label1;
	}
}
