namespace Sdl.LanguagePlatform.MTConnectors.Google.UI
{
	partial class DlgBrowse
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgBrowse));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._comboBoxGoogleUser = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lineFrame1 = new Sdl.LanguagePlatform.MTConnectors.Google.UI.LineFrame();
            this.btnRemove = new System.Windows.Forms.Button();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.lblPubSvrLicensedLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTranslationModel = new System.Windows.Forms.Label();
            this.rbNeuralMT = new System.Windows.Forms.RadioButton();
            this.rbPhraseBasedMT = new System.Windows.Forms.RadioButton();
            this.wizardTitleControl1 = new Sdl.LanguagePlatform.MTConnectors.Google.UI.WizardTitleControl();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // _comboBoxGoogleUser
            // 
            this._comboBoxGoogleUser.ForeColor = System.Drawing.SystemColors.GrayText;
            this._comboBoxGoogleUser.FormattingEnabled = true;
            resources.ApplyResources(this._comboBoxGoogleUser, "_comboBoxGoogleUser");
            this._comboBoxGoogleUser.Name = "_comboBoxGoogleUser";
            this._comboBoxGoogleUser.SelectedIndexChanged += new System.EventHandler(this._comboBoxGoogleUser_SelectedIndexChanged);
            this._comboBoxGoogleUser.TextUpdate += new System.EventHandler(this._comboBoxGoogleUser_TextUpdate);
            this._comboBoxGoogleUser.Click += new System.EventHandler(this._comboBoxGoogleUser_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lineFrame1);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnCancel);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // lineFrame1
            // 
            this.lineFrame1.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lineFrame1, "lineFrame1");
            this.lineFrame1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.lineFrame1.Name = "lineFrame1";
            // 
            // btnRemove
            // 
            resources.ApplyResources(this.btnRemove, "btnRemove");
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // linkLabel2
            // 
            resources.ApplyResources(this.linkLabel2, "linkLabel2");
            this.linkLabel2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabel2.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(214)))));
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.TabStop = true;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // txtApiKey
            // 
            resources.ApplyResources(this.txtApiKey, "txtApiKey");
            this.txtApiKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.UseSystemPasswordChar = true;
            this.txtApiKey.TextChanged += new System.EventHandler(this.txtApiKey_TextChanged);
            // 
            // lblPubSvrLicensedLbl
            // 
            resources.ApplyResources(this.lblPubSvrLicensedLbl, "lblPubSvrLicensedLbl");
            this.lblPubSvrLicensedLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.lblPubSvrLicensedLbl.Name = "lblPubSvrLicensedLbl";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.label1.Name = "label1";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.txtApiKey, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPubSvrLicensedLbl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel2, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.label4.Name = "label4";
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Controls.Add(this.lblTranslationModel);
            this.flowLayoutPanel1.Controls.Add(this.rbNeuralMT);
            this.flowLayoutPanel1.Controls.Add(this.rbPhraseBasedMT);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // lblTranslationModel
            // 
            resources.ApplyResources(this.lblTranslationModel, "lblTranslationModel");
            this.lblTranslationModel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(30)))), ((int)(((byte)(44)))));
            this.lblTranslationModel.Name = "lblTranslationModel";
            // 
            // rbNeuralMT
            // 
            resources.ApplyResources(this.rbNeuralMT, "rbNeuralMT");
            this.rbNeuralMT.Checked = true;
            this.rbNeuralMT.Name = "rbNeuralMT";
            this.rbNeuralMT.TabStop = true;
            this.rbNeuralMT.UseVisualStyleBackColor = true;
            // 
            // rbPhraseBasedMT
            // 
            resources.ApplyResources(this.rbPhraseBasedMT, "rbPhraseBasedMT");
            this.rbPhraseBasedMT.Name = "rbPhraseBasedMT";
            this.rbPhraseBasedMT.TabStop = true;
            this.rbPhraseBasedMT.UseVisualStyleBackColor = true;
            // 
            // wizardTitleControl1
            // 
            resources.ApplyResources(this.wizardTitleControl1, "wizardTitleControl1");
            this.wizardTitleControl1.Name = "wizardTitleControl1";
            this.wizardTitleControl1.ShowLine = true;
            this.wizardTitleControl1.SubTitleText = "Subtitle";
            this.wizardTitleControl1.TitleBitmap = null;
            this.wizardTitleControl1.TitleText = "Title";
            // 
            // DlgBrowse
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._comboBoxGoogleUser);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.wizardTitleControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "DlgBrowse";
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private WizardTitleControl wizardTitleControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _comboBoxGoogleUser;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label lblPubSvrLicensedLbl;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private LineFrame lineFrame1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTranslationModel;
        private System.Windows.Forms.RadioButton rbNeuralMT;
        private System.Windows.Forms.RadioButton rbPhraseBasedMT;
	}
}