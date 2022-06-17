using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.RecordSourceTU
{
    partial class SourceTmConfiguration
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourceTmConfiguration));
			this.MaximumSize = new System.Drawing.Size(Screen.PrimaryScreen.WorkingArea.Width, 306);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbFilenameFields = new System.Windows.Forms.ComboBox();
            this.txtFilenameField = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbCompletePathField = new System.Windows.Forms.ComboBox();
            this.txtCompletePathField = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbProjectNameField = new System.Windows.Forms.ComboBox();
            this.txtProjectNameField = new System.Windows.Forms.TextBox();
            this.chkFileName = new System.Windows.Forms.CheckBox();
            this.chkFullPath = new System.Windows.Forms.CheckBox();
            this.chkProjectName = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cmbFilenameFields, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtFilenameField, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.cmbCompletePathField, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtCompletePathField, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.cmbProjectNameField, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.txtProjectNameField, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.chkFileName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkFullPath, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.chkProjectName, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnOk, 1, 9);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(525, 340);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(212, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Select existing custom field";
            // 
            // cmbFilenameFields
            // 
            this.cmbFilenameFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbFilenameFields.FormattingEnabled = true;
            this.cmbFilenameFields.Location = new System.Drawing.Point(215, 26);
            this.cmbFilenameFields.MaximumSize = new System.Drawing.Size(1920, 0);
            this.cmbFilenameFields.Name = "cmbFilenameFields";
            this.cmbFilenameFields.Size = new System.Drawing.Size(297, 21);
            this.cmbFilenameFields.TabIndex = 12;
            // 
            // txtFilenameField
            // 
            this.txtFilenameField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFilenameField.Location = new System.Drawing.Point(215, 53);
            this.txtFilenameField.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtFilenameField.Name = "txtFilenameField";
            this.txtFilenameField.Size = new System.Drawing.Size(297, 20);
            this.txtFilenameField.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(212, 83);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Select existing custom field";
            // 
            // cmbCompletePathField
            // 
            this.cmbCompletePathField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCompletePathField.FormattingEnabled = true;
            this.cmbCompletePathField.Location = new System.Drawing.Point(215, 99);
            this.cmbCompletePathField.Name = "cmbCompletePathField";
            this.cmbCompletePathField.Size = new System.Drawing.Size(297, 21);
            this.cmbCompletePathField.TabIndex = 15;
            // 
            // txtCompletePathField
            // 
            this.txtCompletePathField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCompletePathField.Location = new System.Drawing.Point(215, 126);
            this.txtCompletePathField.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtCompletePathField.Name = "txtCompletePathField";
            this.txtCompletePathField.Size = new System.Drawing.Size(297, 20);
            this.txtCompletePathField.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(212, 156);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(134, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Select existing custom field";
            // 
            // cmbProjectNameField
            // 
            this.cmbProjectNameField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbProjectNameField.FormattingEnabled = true;
            this.cmbProjectNameField.Location = new System.Drawing.Point(215, 172);
            this.cmbProjectNameField.Name = "cmbProjectNameField";
            this.cmbProjectNameField.Size = new System.Drawing.Size(297, 21);
            this.cmbProjectNameField.TabIndex = 19;
            // 
            // txtProjectNameField
            // 
            this.txtProjectNameField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProjectNameField.Location = new System.Drawing.Point(215, 199);
            this.txtProjectNameField.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.txtProjectNameField.Name = "txtProjectNameField";
            this.txtProjectNameField.Size = new System.Drawing.Size(297, 20);
            this.txtProjectNameField.TabIndex = 21;
            // 
            // chkFileName
            // 
            this.chkFileName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkFileName.AutoSize = true;
            this.chkFileName.Location = new System.Drawing.Point(13, 38);
            this.chkFileName.Name = "chkFileName";
            this.tableLayoutPanel1.SetRowSpan(this.chkFileName, 3);
            this.chkFileName.Size = new System.Drawing.Size(138, 17);
            this.chkFileName.TabIndex = 5;
            this.chkFileName.Text = "Record source filename";
            this.chkFileName.UseVisualStyleBackColor = true;
            // 
            // chkFullPath
            // 
            this.chkFullPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkFullPath.AutoSize = true;
            this.chkFullPath.Location = new System.Drawing.Point(13, 111);
            this.chkFullPath.Name = "chkFullPath";
            this.tableLayoutPanel1.SetRowSpan(this.chkFullPath, 3);
            this.chkFullPath.Size = new System.Drawing.Size(182, 17);
            this.chkFullPath.TabIndex = 3;
            this.chkFullPath.Text = "Record source file complete path";
            this.chkFullPath.UseVisualStyleBackColor = true;
            // 
            // chkProjectName
            // 
            this.chkProjectName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkProjectName.AutoSize = true;
            this.chkProjectName.Location = new System.Drawing.Point(13, 184);
            this.chkProjectName.Name = "chkProjectName";
            this.tableLayoutPanel1.SetRowSpan(this.chkProjectName, 3);
            this.chkProjectName.Size = new System.Drawing.Size(160, 17);
            this.chkProjectName.TabIndex = 9;
            this.chkProjectName.Text = "Record source project name";
            this.chkProjectName.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(442, 232);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(70, 23);
            this.btnOk.TabIndex = 22;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // SourceTmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(525, 340);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(16, 39);
            this.Name = "SourceTmConfiguration";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Record Source TU provider configuration";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox chkProjectName;
		private System.Windows.Forms.CheckBox chkFullPath;
		private System.Windows.Forms.CheckBox chkFileName;
		private System.Windows.Forms.ComboBox cmbFilenameFields;
		private System.Windows.Forms.TextBox txtFilenameField;
		private System.Windows.Forms.ComboBox cmbCompletePathField;
		private System.Windows.Forms.TextBox txtCompletePathField;
		private System.Windows.Forms.ComboBox cmbProjectNameField;
		private System.Windows.Forms.TextBox txtProjectNameField;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label5;
		private Button btnOk;
	}
}