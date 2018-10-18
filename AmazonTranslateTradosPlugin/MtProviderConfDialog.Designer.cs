namespace Sdl.Community.AmazonTranslateTradosPlugin
{
    partial class MtProviderConfDialog
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
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.openFile = new System.Windows.Forms.OpenFileDialog();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboAuthType = new System.Windows.Forms.ComboBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBoxAuth = new System.Windows.Forms.GroupBox();
			this.lblRegion = new System.Windows.Forms.Label();
			this.txtRegion = new System.Windows.Forms.TextBox();
			this.lblAWSProfileName = new System.Windows.Forms.Label();
			this.txtAWSProfileName = new System.Windows.Forms.TextBox();
			this.lblSecretKey = new System.Windows.Forms.Label();
			this.txtSecretKey = new System.Windows.Forms.TextBox();
			this.btnDeleteSavedAWSKeys = new System.Windows.Forms.Button();
			this.lblAccessKey = new System.Windows.Forms.Label();
			this.txtAccessKey = new System.Windows.Forms.TextBox();
			this.chkSaveCred = new System.Windows.Forms.CheckBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.groupBoxPostedit = new System.Windows.Forms.GroupBox();
			this.txtPostEditFileName = new System.Windows.Forms.TextBox();
			this.btnBrowsePostEditFile = new System.Windows.Forms.Button();
			this.groupBoxPreedit = new System.Windows.Forms.GroupBox();
			this.txtPreEditFileName = new System.Windows.Forms.TextBox();
			this.btnBrowsePreEdit = new System.Windows.Forms.Button();
			this.chkResendDrafts = new System.Windows.Forms.CheckBox();
			this.chkUsePostEdit = new System.Windows.Forms.CheckBox();
			this.chkUsePreEdit = new System.Windows.Forms.CheckBox();
			this.chkPlainTextOnly = new System.Windows.Forms.CheckBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.groupBox3.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBoxAuth.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBoxPostedit.SuspendLayout();
			this.groupBoxPreedit.SuspendLayout();
			this.SuspendLayout();
			// 
			// btn_OK
			// 
			this.btn_OK.Location = new System.Drawing.Point(226, 403);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(75, 23);
			this.btn_OK.TabIndex = 3;
			this.btn_OK.Text = "&OK";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
			// 
			// btn_Cancel
			// 
			this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btn_Cancel.Location = new System.Drawing.Point(322, 403);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
			this.btn_Cancel.TabIndex = 4;
			this.btn_Cancel.Text = "&Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 10000;
			this.toolTip1.InitialDelay = 500;
			this.toolTip1.IsBalloon = true;
			this.toolTip1.ReshowDelay = 100;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboAuthType);
			this.groupBox3.Location = new System.Drawing.Point(2, 4);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 1, 2, 1);
			this.groupBox3.Size = new System.Drawing.Size(366, 55);
			this.groupBox3.TabIndex = 15;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Choose AWS auth type";
			// 
			// comboAuthType
			// 
			this.comboAuthType.BackColor = System.Drawing.SystemColors.Window;
			this.comboAuthType.FormattingEnabled = true;
			this.comboAuthType.Items.AddRange(new object[] {
            "Local AWS profile",
            "Access key / Secret access key"});
			this.comboAuthType.Location = new System.Drawing.Point(5, 16);
			this.comboAuthType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.comboAuthType.Name = "comboAuthType";
			this.comboAuthType.Size = new System.Drawing.Size(324, 21);
			this.comboAuthType.TabIndex = 0;
			this.comboAuthType.SelectedIndexChanged += new System.EventHandler(this.comboProvider_SelectedIndexChanged);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(9, 10);
			this.tabControl1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(392, 387);
			this.tabControl1.TabIndex = 16;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPage1.Controls.Add(this.groupBoxAuth);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.tabPage1.Size = new System.Drawing.Size(384, 361);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Authentication";
			// 
			// groupBoxAuth
			// 
			this.groupBoxAuth.Controls.Add(this.linkLabel1);
			this.groupBoxAuth.Controls.Add(this.lblRegion);
			this.groupBoxAuth.Controls.Add(this.txtRegion);
			this.groupBoxAuth.Controls.Add(this.lblAWSProfileName);
			this.groupBoxAuth.Controls.Add(this.txtAWSProfileName);
			this.groupBoxAuth.Controls.Add(this.lblSecretKey);
			this.groupBoxAuth.Controls.Add(this.txtSecretKey);
			this.groupBoxAuth.Controls.Add(this.btnDeleteSavedAWSKeys);
			this.groupBoxAuth.Controls.Add(this.lblAccessKey);
			this.groupBoxAuth.Controls.Add(this.txtAccessKey);
			this.groupBoxAuth.Controls.Add(this.chkSaveCred);
			this.groupBoxAuth.Location = new System.Drawing.Point(3, 64);
			this.groupBoxAuth.Name = "groupBoxAuth";
			this.groupBoxAuth.Size = new System.Drawing.Size(365, 291);
			this.groupBoxAuth.TabIndex = 5;
			this.groupBoxAuth.TabStop = false;
			this.groupBoxAuth.Text = "AWS auth";
			// 
			// lblRegion
			// 
			this.lblRegion.AutoSize = true;
			this.lblRegion.Location = new System.Drawing.Point(5, 70);
			this.lblRegion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblRegion.Name = "lblRegion";
			this.lblRegion.Size = new System.Drawing.Size(93, 13);
			this.lblRegion.TabIndex = 22;
			this.lblRegion.Text = "AWS region name";
			// 
			// txtRegion
			// 
			this.txtRegion.Enabled = false;
			this.txtRegion.Location = new System.Drawing.Point(8, 86);
			this.txtRegion.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtRegion.Name = "txtRegion";
			this.txtRegion.Size = new System.Drawing.Size(321, 20);
			this.txtRegion.TabIndex = 21;
			// 
			// lblAWSProfileName
			// 
			this.lblAWSProfileName.AutoSize = true;
			this.lblAWSProfileName.Enabled = false;
			this.lblAWSProfileName.Location = new System.Drawing.Point(5, 19);
			this.lblAWSProfileName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblAWSProfileName.Name = "lblAWSProfileName";
			this.lblAWSProfileName.Size = new System.Drawing.Size(152, 13);
			this.lblAWSProfileName.TabIndex = 20;
			this.lblAWSProfileName.Text = "Locally configured AWS profile";
			// 
			// txtAWSProfileName
			// 
			this.txtAWSProfileName.Enabled = false;
			this.txtAWSProfileName.Location = new System.Drawing.Point(8, 35);
			this.txtAWSProfileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtAWSProfileName.Name = "txtAWSProfileName";
			this.txtAWSProfileName.Size = new System.Drawing.Size(321, 20);
			this.txtAWSProfileName.TabIndex = 19;
			// 
			// lblSecretKey
			// 
			this.lblSecretKey.AutoSize = true;
			this.lblSecretKey.Enabled = false;
			this.lblSecretKey.Location = new System.Drawing.Point(5, 152);
			this.lblSecretKey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblSecretKey.Name = "lblSecretKey";
			this.lblSecretKey.Size = new System.Drawing.Size(95, 13);
			this.lblSecretKey.TabIndex = 18;
			this.lblSecretKey.Text = "Secret access key";
			// 
			// txtSecretKey
			// 
			this.txtSecretKey.Enabled = false;
			this.txtSecretKey.Location = new System.Drawing.Point(8, 168);
			this.txtSecretKey.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtSecretKey.Name = "txtSecretKey";
			this.txtSecretKey.Size = new System.Drawing.Size(321, 20);
			this.txtSecretKey.TabIndex = 17;
			// 
			// btnDeleteSavedAWSKeys
			// 
			this.btnDeleteSavedAWSKeys.AutoSize = true;
			this.btnDeleteSavedAWSKeys.Enabled = false;
			this.btnDeleteSavedAWSKeys.Location = new System.Drawing.Point(5, 260);
			this.btnDeleteSavedAWSKeys.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.btnDeleteSavedAWSKeys.Name = "btnDeleteSavedAWSKeys";
			this.btnDeleteSavedAWSKeys.Size = new System.Drawing.Size(145, 25);
			this.btnDeleteSavedAWSKeys.TabIndex = 16;
			this.btnDeleteSavedAWSKeys.Text = "Delete saved keys";
			this.btnDeleteSavedAWSKeys.UseVisualStyleBackColor = true;
			this.btnDeleteSavedAWSKeys.Click += new System.EventHandler(this.btnDeleteSavedAWSKeys_Click);
			// 
			// lblAccessKey
			// 
			this.lblAccessKey.AutoSize = true;
			this.lblAccessKey.Location = new System.Drawing.Point(5, 113);
			this.lblAccessKey.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.lblAccessKey.Name = "lblAccessKey";
			this.lblAccessKey.Size = new System.Drawing.Size(62, 13);
			this.lblAccessKey.TabIndex = 15;
			this.lblAccessKey.Text = "Access key";
			// 
			// txtAccessKey
			// 
			this.txtAccessKey.Enabled = false;
			this.txtAccessKey.Location = new System.Drawing.Point(8, 129);
			this.txtAccessKey.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtAccessKey.Name = "txtAccessKey";
			this.txtAccessKey.Size = new System.Drawing.Size(321, 20);
			this.txtAccessKey.TabIndex = 3;
			// 
			// chkSaveCred
			// 
			this.chkSaveCred.AutoSize = true;
			this.chkSaveCred.Enabled = false;
			this.chkSaveCred.Location = new System.Drawing.Point(8, 222);
			this.chkSaveCred.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.chkSaveCred.Name = "chkSaveCred";
			this.chkSaveCred.Size = new System.Drawing.Size(164, 17);
			this.chkSaveCred.TabIndex = 6;
			this.chkSaveCred.Text = "Save keys for future sessions";
			this.chkSaveCred.UseVisualStyleBackColor = true;
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.tabPage3.Controls.Add(this.groupBox4);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(384, 361);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Additional options";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.groupBoxPostedit);
			this.groupBox4.Controls.Add(this.groupBoxPreedit);
			this.groupBox4.Controls.Add(this.chkResendDrafts);
			this.groupBox4.Controls.Add(this.chkUsePostEdit);
			this.groupBox4.Controls.Add(this.chkUsePreEdit);
			this.groupBox4.Controls.Add(this.chkPlainTextOnly);
			this.groupBox4.Location = new System.Drawing.Point(2, 14);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBox4.Size = new System.Drawing.Size(368, 332);
			this.groupBox4.TabIndex = 6;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Provider Options";
			// 
			// groupBoxPostedit
			// 
			this.groupBoxPostedit.Controls.Add(this.txtPostEditFileName);
			this.groupBoxPostedit.Controls.Add(this.btnBrowsePostEditFile);
			this.groupBoxPostedit.Location = new System.Drawing.Point(0, 166);
			this.groupBoxPostedit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBoxPostedit.Name = "groupBoxPostedit";
			this.groupBoxPostedit.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBoxPostedit.Size = new System.Drawing.Size(351, 45);
			this.groupBoxPostedit.TabIndex = 22;
			this.groupBoxPostedit.TabStop = false;
			this.groupBoxPostedit.Text = "Post-lookup find/replace filename:";
			// 
			// txtPostEditFileName
			// 
			this.txtPostEditFileName.Enabled = false;
			this.txtPostEditFileName.Location = new System.Drawing.Point(2, 17);
			this.txtPostEditFileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtPostEditFileName.MaxLength = 30;
			this.txtPostEditFileName.Name = "txtPostEditFileName";
			this.txtPostEditFileName.ReadOnly = true;
			this.txtPostEditFileName.Size = new System.Drawing.Size(284, 20);
			this.txtPostEditFileName.TabIndex = 8;
			// 
			// btnBrowsePostEditFile
			// 
			this.btnBrowsePostEditFile.Location = new System.Drawing.Point(292, 14);
			this.btnBrowsePostEditFile.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.btnBrowsePostEditFile.Name = "btnBrowsePostEditFile";
			this.btnBrowsePostEditFile.Size = new System.Drawing.Size(55, 23);
			this.btnBrowsePostEditFile.TabIndex = 10;
			this.btnBrowsePostEditFile.Text = "Browse...";
			this.btnBrowsePostEditFile.UseVisualStyleBackColor = true;
			this.btnBrowsePostEditFile.Click += new System.EventHandler(this.btnBrowsePostEditFile_Click);
			// 
			// groupBoxPreedit
			// 
			this.groupBoxPreedit.Controls.Add(this.txtPreEditFileName);
			this.groupBoxPreedit.Controls.Add(this.btnBrowsePreEdit);
			this.groupBoxPreedit.Location = new System.Drawing.Point(2, 87);
			this.groupBoxPreedit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBoxPreedit.Name = "groupBoxPreedit";
			this.groupBoxPreedit.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.groupBoxPreedit.Size = new System.Drawing.Size(349, 52);
			this.groupBoxPreedit.TabIndex = 21;
			this.groupBoxPreedit.TabStop = false;
			this.groupBoxPreedit.Text = "Pre-lookup find/replace filename:";
			// 
			// txtPreEditFileName
			// 
			this.txtPreEditFileName.Enabled = false;
			this.txtPreEditFileName.Location = new System.Drawing.Point(5, 19);
			this.txtPreEditFileName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.txtPreEditFileName.MaxLength = 30;
			this.txtPreEditFileName.Name = "txtPreEditFileName";
			this.txtPreEditFileName.ReadOnly = true;
			this.txtPreEditFileName.Size = new System.Drawing.Size(279, 20);
			this.txtPreEditFileName.TabIndex = 20;
			this.txtPreEditFileName.TabStop = false;
			// 
			// btnBrowsePreEdit
			// 
			this.btnBrowsePreEdit.Location = new System.Drawing.Point(290, 17);
			this.btnBrowsePreEdit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.btnBrowsePreEdit.Name = "btnBrowsePreEdit";
			this.btnBrowsePreEdit.Size = new System.Drawing.Size(55, 23);
			this.btnBrowsePreEdit.TabIndex = 14;
			this.btnBrowsePreEdit.Text = "Browse...";
			this.btnBrowsePreEdit.UseVisualStyleBackColor = true;
			this.btnBrowsePreEdit.Click += new System.EventHandler(this.btnBrowsePreEdit_Click);
			// 
			// chkResendDrafts
			// 
			this.chkResendDrafts.AutoSize = true;
			this.chkResendDrafts.Location = new System.Drawing.Point(5, 21);
			this.chkResendDrafts.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.chkResendDrafts.Name = "chkResendDrafts";
			this.chkResendDrafts.Size = new System.Drawing.Size(208, 17);
			this.chkResendDrafts.TabIndex = 11;
			this.chkResendDrafts.Text = "Re-send draft and translated segments";
			this.chkResendDrafts.UseVisualStyleBackColor = true;
			// 
			// chkUsePostEdit
			// 
			this.chkUsePostEdit.AutoSize = true;
			this.chkUsePostEdit.Location = new System.Drawing.Point(2, 144);
			this.chkUsePostEdit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.chkUsePostEdit.Name = "chkUsePostEdit";
			this.chkUsePostEdit.Size = new System.Drawing.Size(158, 17);
			this.chkUsePostEdit.TabIndex = 7;
			this.chkUsePostEdit.Text = "Do post-lookup find/replace";
			this.chkUsePostEdit.UseVisualStyleBackColor = true;
			this.chkUsePostEdit.CheckedChanged += new System.EventHandler(this.chkUsePostEdit_CheckedChanged);
			// 
			// chkUsePreEdit
			// 
			this.chkUsePreEdit.AutoSize = true;
			this.chkUsePreEdit.Location = new System.Drawing.Point(5, 65);
			this.chkUsePreEdit.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.chkUsePreEdit.Name = "chkUsePreEdit";
			this.chkUsePreEdit.Size = new System.Drawing.Size(153, 17);
			this.chkUsePreEdit.TabIndex = 13;
			this.chkUsePreEdit.Text = "Do pre-lookup find/replace";
			this.chkUsePreEdit.UseVisualStyleBackColor = true;
			this.chkUsePreEdit.CheckedChanged += new System.EventHandler(this.chkUsePreEdit_CheckedChanged);
			// 
			// chkPlainTextOnly
			// 
			this.chkPlainTextOnly.AutoSize = true;
			this.chkPlainTextOnly.Location = new System.Drawing.Point(5, 43);
			this.chkPlainTextOnly.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.chkPlainTextOnly.Name = "chkPlainTextOnly";
			this.chkPlainTextOnly.Size = new System.Drawing.Size(162, 17);
			this.chkPlainTextOnly.TabIndex = 12;
			this.chkPlainTextOnly.Text = "Send plain text only (no tags)";
			this.chkPlainTextOnly.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(8, 195);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(29, 13);
			this.linkLabel1.TabIndex = 23;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Help";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// MtProviderConfDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(412, 438);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btn_Cancel);
			this.Controls.Add(this.btn_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MtProviderConfDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Settings";
			this.groupBox3.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBoxAuth.ResumeLayout(false);
			this.groupBoxAuth.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBoxPostedit.ResumeLayout(false);
			this.groupBoxPostedit.PerformLayout();
			this.groupBoxPreedit.ResumeLayout(false);
			this.groupBoxPreedit.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.OpenFileDialog openFile;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtPreEditFileName;
        private System.Windows.Forms.Button btnBrowsePreEdit;
        private System.Windows.Forms.CheckBox chkResendDrafts;
        private System.Windows.Forms.Button btnBrowsePostEditFile;
        private System.Windows.Forms.TextBox txtPostEditFileName;
        private System.Windows.Forms.CheckBox chkUsePostEdit;
        private System.Windows.Forms.CheckBox chkUsePreEdit;
        private System.Windows.Forms.CheckBox chkPlainTextOnly;
        private System.Windows.Forms.ComboBox comboAuthType;
        private System.Windows.Forms.GroupBox groupBoxPreedit;
        private System.Windows.Forms.GroupBox groupBoxPostedit;
        private System.Windows.Forms.GroupBox groupBoxAuth;
        private System.Windows.Forms.Button btnDeleteSavedAWSKeys;
        private System.Windows.Forms.Label lblAccessKey;
        private System.Windows.Forms.TextBox txtAccessKey;
        private System.Windows.Forms.CheckBox chkSaveCred;
        private System.Windows.Forms.Label lblSecretKey;
        private System.Windows.Forms.TextBox txtSecretKey;
        private System.Windows.Forms.Label lblAWSProfileName;
        private System.Windows.Forms.TextBox txtAWSProfileName;
        private System.Windows.Forms.Label lblRegion;
        private System.Windows.Forms.TextBox txtRegion;
		private System.Windows.Forms.LinkLabel linkLabel1;
	}
}