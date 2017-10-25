using System.ComponentModel;

namespace Sdl.Community.TMLifting
{
    partial class TMLiftingForm
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
            this.components = new System.ComponentModel.Container();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gridServerBasedTMs = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.locationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FuzzyIndexes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TmId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reindexBtn = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.serverNameTxtBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.helpBtnServerBased = new System.Windows.Forms.Button();
            this.authentication_box = new System.Windows.Forms.GroupBox();
            this.passwordTxtBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.userNameTxtBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOkServerBased = new System.Windows.Forms.Button();
            this.cancelBtnServerBased = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cleanBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.upLiftCheckBox = new System.Windows.Forms.CheckBox();
            this.reIndexCheckBox = new System.Windows.Forms.CheckBox();
            this.chkLoadStudioTMs = new System.Windows.Forms.CheckBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnReindex = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rtbStatus = new System.Windows.Forms.RichTextBox();
            this.lstTms = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.itemsBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridServerBasedTMs)).BeginInit();
            this.panel3.SuspendLayout();
            this.authentication_box.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.itemsBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.reindexBtn);
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1038, 564);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gridServerBasedTMs);
            this.groupBox1.Location = new System.Drawing.Point(6, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(600, 400);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Translation Memories";
            // 
            // gridServerBasedTMs
            // 
            this.gridServerBasedTMs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridServerBasedTMs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.locationDataGridViewTextBoxColumn,
            this.FuzzyIndexes,
            this.TmId});
            this.gridServerBasedTMs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridServerBasedTMs.Location = new System.Drawing.Point(3, 16);
            this.gridServerBasedTMs.Name = "gridServerBasedTMs";
            this.gridServerBasedTMs.Size = new System.Drawing.Size(594, 381);
            this.gridServerBasedTMs.TabIndex = 6;
            this.gridServerBasedTMs.Visible = false;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.Width = 150;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.Width = 150;
            // 
            // locationDataGridViewTextBoxColumn
            // 
            this.locationDataGridViewTextBoxColumn.DataPropertyName = "Location";
            this.locationDataGridViewTextBoxColumn.HeaderText = "Location";
            this.locationDataGridViewTextBoxColumn.Name = "locationDataGridViewTextBoxColumn";
            // 
            // FuzzyIndexes
            // 
            this.FuzzyIndexes.HeaderText = "FuzzyIndexes";
            this.FuzzyIndexes.Name = "FuzzyIndexes";
            // 
            // TmId
            // 
            this.TmId.HeaderText = "TmId";
            this.TmId.Name = "TmId";
            this.TmId.Visible = false;
            // 
            // reindexBtn
            // 
            this.reindexBtn.Location = new System.Drawing.Point(792, 435);
            this.reindexBtn.Name = "reindexBtn";
            this.reindexBtn.Size = new System.Drawing.Size(75, 23);
            this.reindexBtn.TabIndex = 8;
            this.reindexBtn.Text = "Reindex";
            this.reindexBtn.UseVisualStyleBackColor = true;
            this.reindexBtn.Click += new System.EventHandler(this.reindexBtn_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBox1);
            this.panel3.Controls.Add(this.textBox2);
            this.panel3.Controls.Add(this.serverNameTxtBox);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.helpBtnServerBased);
            this.panel3.Controls.Add(this.authentication_box);
            this.panel3.Controls.Add(this.btnOkServerBased);
            this.panel3.Controls.Add(this.cancelBtnServerBased);
            this.panel3.Location = new System.Drawing.Point(582, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(450, 325);
            this.panel3.TabIndex = 7;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(170, 61);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(217, 17);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "The server requires a secure connection";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(170, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(52, 20);
            this.textBox2.TabIndex = 15;
            // 
            // serverNameTxtBox
            // 
            this.serverNameTxtBox.Location = new System.Drawing.Point(170, 9);
            this.serverNameTxtBox.Name = "serverNameTxtBox";
            this.serverNameTxtBox.Size = new System.Drawing.Size(249, 20);
            this.serverNameTxtBox.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(47, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(47, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Server Address:";
            // 
            // helpBtnServerBased
            // 
            this.helpBtnServerBased.Location = new System.Drawing.Point(372, 288);
            this.helpBtnServerBased.Name = "helpBtnServerBased";
            this.helpBtnServerBased.Size = new System.Drawing.Size(75, 23);
            this.helpBtnServerBased.TabIndex = 11;
            this.helpBtnServerBased.Text = "Help";
            this.helpBtnServerBased.UseVisualStyleBackColor = true;
            // 
            // authentication_box
            // 
            this.authentication_box.Controls.Add(this.passwordTxtBox);
            this.authentication_box.Controls.Add(this.label3);
            this.authentication_box.Controls.Add(this.userNameTxtBox);
            this.authentication_box.Controls.Add(this.label2);
            this.authentication_box.Location = new System.Drawing.Point(50, 80);
            this.authentication_box.Name = "authentication_box";
            this.authentication_box.Size = new System.Drawing.Size(397, 202);
            this.authentication_box.TabIndex = 8;
            this.authentication_box.TabStop = false;
            this.authentication_box.Text = "Authentication";
            // 
            // passwordTxtBox
            // 
            this.passwordTxtBox.Location = new System.Drawing.Point(120, 73);
            this.passwordTxtBox.Name = "passwordTxtBox";
            this.passwordTxtBox.PasswordChar = '●';
            this.passwordTxtBox.Size = new System.Drawing.Size(249, 20);
            this.passwordTxtBox.TabIndex = 1;
            this.passwordTxtBox.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Password:";
            // 
            // userNameTxtBox
            // 
            this.userNameTxtBox.Location = new System.Drawing.Point(120, 36);
            this.userNameTxtBox.Name = "userNameTxtBox";
            this.userNameTxtBox.Size = new System.Drawing.Size(249, 20);
            this.userNameTxtBox.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "User name:";
            // 
            // btnOkServerBased
            // 
            this.btnOkServerBased.Location = new System.Drawing.Point(210, 288);
            this.btnOkServerBased.Name = "btnOkServerBased";
            this.btnOkServerBased.Size = new System.Drawing.Size(75, 23);
            this.btnOkServerBased.TabIndex = 3;
            this.btnOkServerBased.Text = "Ok";
            this.btnOkServerBased.UseVisualStyleBackColor = true;
            this.btnOkServerBased.Click += new System.EventHandler(this.btnOkServerBased_Click);
            // 
            // cancelBtnServerBased
            // 
            this.cancelBtnServerBased.Location = new System.Drawing.Point(291, 288);
            this.cancelBtnServerBased.Name = "cancelBtnServerBased";
            this.cancelBtnServerBased.Size = new System.Drawing.Size(75, 23);
            this.cancelBtnServerBased.TabIndex = 10;
            this.cancelBtnServerBased.Text = "Cancel";
            this.cancelBtnServerBased.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.panel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1038, 564);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cleanBtn);
            this.panel1.Controls.Add(this.cancelBtn);
            this.panel1.Controls.Add(this.upLiftCheckBox);
            this.panel1.Controls.Add(this.reIndexCheckBox);
            this.panel1.Controls.Add(this.chkLoadStudioTMs);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.btnReindex);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 525);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1032, 36);
            this.panel1.TabIndex = 2;
            // 
            // cleanBtn
            // 
            this.cleanBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanBtn.Location = new System.Drawing.Point(850, 6);
            this.cleanBtn.Margin = new System.Windows.Forms.Padding(2);
            this.cleanBtn.Name = "cleanBtn";
            this.cleanBtn.Size = new System.Drawing.Size(50, 23);
            this.cleanBtn.TabIndex = 6;
            this.cleanBtn.Text = "Clean";
            this.cleanBtn.UseVisualStyleBackColor = true;
            this.cleanBtn.Click += new System.EventHandler(this.cleanBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(904, 6);
            this.cancelBtn.Margin = new System.Windows.Forms.Padding(2);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // upLiftCheckBox
            // 
            this.upLiftCheckBox.AutoSize = true;
            this.upLiftCheckBox.Location = new System.Drawing.Point(299, 10);
            this.upLiftCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.upLiftCheckBox.Name = "upLiftCheckBox";
            this.upLiftCheckBox.Size = new System.Drawing.Size(50, 17);
            this.upLiftCheckBox.TabIndex = 4;
            this.upLiftCheckBox.Text = "Uplift";
            this.upLiftCheckBox.UseVisualStyleBackColor = true;
            // 
            // reIndexCheckBox
            // 
            this.reIndexCheckBox.AutoSize = true;
            this.reIndexCheckBox.Location = new System.Drawing.Point(213, 10);
            this.reIndexCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.reIndexCheckBox.Name = "reIndexCheckBox";
            this.reIndexCheckBox.Size = new System.Drawing.Size(69, 17);
            this.reIndexCheckBox.TabIndex = 3;
            this.reIndexCheckBox.Text = "Re-Index";
            this.reIndexCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkLoadStudioTMs
            // 
            this.chkLoadStudioTMs.AutoSize = true;
            this.chkLoadStudioTMs.Location = new System.Drawing.Point(94, 11);
            this.chkLoadStudioTMs.Name = "chkLoadStudioTMs";
            this.chkLoadStudioTMs.Size = new System.Drawing.Size(107, 17);
            this.chkLoadStudioTMs.TabIndex = 2;
            this.chkLoadStudioTMs.Text = "Load Studio TMs";
            this.chkLoadStudioTMs.UseVisualStyleBackColor = true;
            this.chkLoadStudioTMs.CheckedChanged += new System.EventHandler(this.chkLoadStudioTMs_CheckedChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(12, 6);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse ...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnReindex
            // 
            this.btnReindex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReindex.Location = new System.Drawing.Point(959, 6);
            this.btnReindex.Name = "btnReindex";
            this.btnReindex.Size = new System.Drawing.Size(56, 23);
            this.btnReindex.TabIndex = 0;
            this.btnReindex.Text = "Process";
            this.btnReindex.UseVisualStyleBackColor = true;
            this.btnReindex.Click += new System.EventHandler(this.btnReindex_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rtbStatus);
            this.panel2.Controls.Add(this.lstTms);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1032, 558);
            this.panel2.TabIndex = 3;
            // 
            // rtbStatus
            // 
            this.rtbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbStatus.Location = new System.Drawing.Point(0, 186);
            this.rtbStatus.Name = "rtbStatus";
            this.rtbStatus.Size = new System.Drawing.Size(1032, 372);
            this.rtbStatus.TabIndex = 1;
            this.rtbStatus.Text = "";
            // 
            // lstTms
            // 
            this.lstTms.AllowDrop = true;
            this.lstTms.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstTms.FormattingEnabled = true;
            this.lstTms.Location = new System.Drawing.Point(0, 0);
            this.lstTms.Name = "lstTms";
            this.lstTms.Size = new System.Drawing.Size(1032, 186);
            this.lstTms.TabIndex = 0;
            this.lstTms.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstTms_DragDrop);
            this.lstTms.DragOver += new System.Windows.Forms.DragEventHandler(this.lstTms_DragOver);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1046, 590);
            this.tabControl1.TabIndex = 0;
            // 
            // itemsBindingSource1
            // 
            this.itemsBindingSource1.DataMember = "Items";
            // 
            // TMLiftingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "TMLiftingForm";
            this.Size = new System.Drawing.Size(1140, 596);
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridServerBasedTMs)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.authentication_box.ResumeLayout(false);
            this.authentication_box.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.itemsBindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button cleanBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.CheckBox upLiftCheckBox;
		private System.Windows.Forms.CheckBox reIndexCheckBox;
		private System.Windows.Forms.CheckBox chkLoadStudioTMs;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnReindex;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.RichTextBox rtbStatus;
		private System.Windows.Forms.ListBox lstTms;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button btnOkServerBased;
		private System.Windows.Forms.DataGridView gridServerBasedTMs;
		private System.Windows.Forms.BindingSource itemsBindingSource1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Button helpBtnServerBased;
		private System.Windows.Forms.Button cancelBtnServerBased;
		private System.Windows.Forms.GroupBox authentication_box;
		private System.Windows.Forms.TextBox passwordTxtBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox userNameTxtBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox serverNameTxtBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button reindexBtn;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn FuzzyIndexes;
		private System.Windows.Forms.DataGridViewTextBoxColumn TmId;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

