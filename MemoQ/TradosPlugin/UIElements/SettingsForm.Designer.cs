namespace TradosPlugin
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTMs = new System.Windows.Forms.Panel();
            this.lblNoTMs = new System.Windows.Forms.Label();
            this.cmbTargetLang = new System.Windows.Forms.ComboBox();
            this.lblTargetLang = new System.Windows.Forms.Label();
            this.lblFilter = new System.Windows.Forms.Label();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.dgrTMs = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPermissions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSDLLookup = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colSDLUpdate = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lblTMs = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.pnlLogin = new System.Windows.Forms.Panel();
            this.btnChangeAccount = new System.Windows.Forms.Button();
            this.gplAccessDetails = new TradosPlugin.GradientPanel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblServerAccessDetails = new System.Windows.Forms.Label();
            this.pnlLoginFields = new System.Windows.Forms.Panel();
            this.btnLogin = new System.Windows.Forms.Button();
            this.pnlLoginType = new System.Windows.Forms.Panel();
            this.rbSSO = new System.Windows.Forms.RadioButton();
            this.rbWindows = new System.Windows.Forms.RadioButton();
            this.rbLanguageTerminal = new System.Windows.Forms.RadioButton();
            this.rbMemoQServer = new System.Windows.Forms.RadioButton();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.pnlServer = new System.Windows.Forms.Panel();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.lblServerName = new System.Windows.Forms.Label();
            this.lblLTAccountData = new System.Windows.Forms.Label();
            this.pnlLeftSide = new System.Windows.Forms.Panel();
            this.gplGeneralSettings = new TradosPlugin.GradientPanel();
            this.lblGeneralSettings = new System.Windows.Forms.Label();
            this.lnkAddServer = new System.Windows.Forms.LinkLabel();
            this.pbAddMemoQServer = new System.Windows.Forms.PictureBox();
            this.lblServers = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlVerticalLine = new System.Windows.Forms.Panel();
            this.btnOpenLTAccount = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnlTMs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrTMs)).BeginInit();
            this.pnlLogin.SuspendLayout();
            this.gplAccessDetails.SuspendLayout();
            this.pnlLoginFields.SuspendLayout();
            this.pnlLoginType.SuspendLayout();
            this.pnlServer.SuspendLayout();
            this.pnlLeftSide.SuspendLayout();
            this.gplGeneralSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbAddMemoQServer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlTMs
            // 
            this.pnlTMs.Controls.Add(this.lblNoTMs);
            this.pnlTMs.Controls.Add(this.cmbTargetLang);
            this.pnlTMs.Controls.Add(this.lblTargetLang);
            this.pnlTMs.Controls.Add(this.lblFilter);
            this.pnlTMs.Controls.Add(this.txtFilter);
            this.pnlTMs.Controls.Add(this.dgrTMs);
            resources.ApplyResources(this.pnlTMs, "pnlTMs");
            this.pnlTMs.Name = "pnlTMs";
            // 
            // lblNoTMs
            // 
            this.lblNoTMs.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.lblNoTMs, "lblNoTMs");
            this.lblNoTMs.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblNoTMs.Name = "lblNoTMs";
            // 
            // cmbTargetLang
            // 
            this.cmbTargetLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTargetLang.FormattingEnabled = true;
            resources.ApplyResources(this.cmbTargetLang, "cmbTargetLang");
            this.cmbTargetLang.Name = "cmbTargetLang";
            this.cmbTargetLang.Sorted = true;
            this.cmbTargetLang.SelectedIndexChanged += new System.EventHandler(this.cmbTargetLang_SelectedIndexChanged);
            // 
            // lblTargetLang
            // 
            resources.ApplyResources(this.lblTargetLang, "lblTargetLang");
            this.lblTargetLang.Name = "lblTargetLang";
            // 
            // lblFilter
            // 
            resources.ApplyResources(this.lblFilter, "lblFilter");
            this.lblFilter.Name = "lblFilter";
            // 
            // txtFilter
            // 
            resources.ApplyResources(this.txtFilter, "txtFilter");
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // dgrTMs
            // 
            this.dgrTMs.AllowUserToAddRows = false;
            this.dgrTMs.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dgrTMs, "dgrTMs");
            this.dgrTMs.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgrTMs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrTMs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgrTMs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgrTMs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colOwner,
            this.colSource,
            this.colTarget,
            this.colPermissions,
            this.colSDLLookup,
            this.colSDLUpdate});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgrTMs.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgrTMs.GridColor = System.Drawing.Color.Lavender;
            this.dgrTMs.Name = "dgrTMs";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgrTMs.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgrTMs.RowHeadersVisible = false;
            this.dgrTMs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgrTMs.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrTMs_CellMouseEnter);
            this.dgrTMs.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgrTMs_CellMouseLeave);
            this.dgrTMs.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgrTMs_ColumnHeaderMouseClick);
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "TMName";
            resources.ApplyResources(this.colName, "colName");
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colOwner
            // 
            this.colOwner.DataPropertyName = "Owner";
            resources.ApplyResources(this.colOwner, "colOwner");
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            // 
            // colSource
            // 
            this.colSource.DataPropertyName = "SourceLang";
            resources.ApplyResources(this.colSource, "colSource");
            this.colSource.Name = "colSource";
            this.colSource.ReadOnly = true;
            // 
            // colTarget
            // 
            this.colTarget.DataPropertyName = "TargetLang";
            resources.ApplyResources(this.colTarget, "colTarget");
            this.colTarget.Name = "colTarget";
            this.colTarget.ReadOnly = true;
            // 
            // colPermissions
            // 
            this.colPermissions.DataPropertyName = "LTPermission";
            resources.ApplyResources(this.colPermissions, "colPermissions");
            this.colPermissions.Name = "colPermissions";
            this.colPermissions.ReadOnly = true;
            // 
            // colSDLLookup
            // 
            this.colSDLLookup.DataPropertyName = "SDLLookup";
            resources.ApplyResources(this.colSDLLookup, "colSDLLookup");
            this.colSDLLookup.Name = "colSDLLookup";
            this.colSDLLookup.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSDLLookup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colSDLUpdate
            // 
            this.colSDLUpdate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colSDLUpdate.DataPropertyName = "SDLUpdate";
            resources.ApplyResources(this.colSDLUpdate, "colSDLUpdate");
            this.colSDLUpdate.Name = "colSDLUpdate";
            this.colSDLUpdate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSDLUpdate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // lblTMs
            // 
            resources.ApplyResources(this.lblTMs, "lblTMs");
            this.lblTMs.BackColor = System.Drawing.Color.Transparent;
            this.lblTMs.ForeColor = System.Drawing.Color.White;
            this.lblTMs.Name = "lblTMs";
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.UseVisualStyleBackColor = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pnlLogin
            // 
            this.pnlLogin.Controls.Add(this.btnChangeAccount);
            this.pnlLogin.Controls.Add(this.gplAccessDetails);
            this.pnlLogin.Controls.Add(this.pnlLoginFields);
            this.pnlLogin.Controls.Add(this.pnlServer);
            this.pnlLogin.Controls.Add(this.lblLTAccountData);
            resources.ApplyResources(this.pnlLogin, "pnlLogin");
            this.pnlLogin.Name = "pnlLogin";
            // 
            // btnChangeAccount
            // 
            resources.ApplyResources(this.btnChangeAccount, "btnChangeAccount");
            this.btnChangeAccount.Name = "btnChangeAccount";
            this.btnChangeAccount.UseVisualStyleBackColor = false;
            this.btnChangeAccount.Click += new System.EventHandler(this.btnChangeAccount_Click);
            // 
            // gplAccessDetails
            // 
            this.gplAccessDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplAccessDetails.BackEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplAccessDetails.BackStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplAccessDetails.Controls.Add(this.lblWarning);
            this.gplAccessDetails.Controls.Add(this.lblServerAccessDetails);
            this.gplAccessDetails.GradientMode = TradosPlugin.GradientPanel.GradientModes.TwoPoint;
            this.gplAccessDetails.GradientOrientation = System.Windows.Forms.Orientation.Horizontal;
            resources.ApplyResources(this.gplAccessDetails, "gplAccessDetails");
            this.gplAccessDetails.Name = "gplAccessDetails";
            // 
            // lblWarning
            // 
            resources.ApplyResources(this.lblWarning, "lblWarning");
            this.lblWarning.BackColor = System.Drawing.Color.Transparent;
            this.lblWarning.ForeColor = System.Drawing.Color.White;
            this.lblWarning.Name = "lblWarning";
            // 
            // lblServerAccessDetails
            // 
            resources.ApplyResources(this.lblServerAccessDetails, "lblServerAccessDetails");
            this.lblServerAccessDetails.BackColor = System.Drawing.Color.Transparent;
            this.lblServerAccessDetails.ForeColor = System.Drawing.Color.White;
            this.lblServerAccessDetails.Name = "lblServerAccessDetails";
            // 
            // pnlLoginFields
            // 
            this.pnlLoginFields.Controls.Add(this.btnLogin);
            this.pnlLoginFields.Controls.Add(this.pnlLoginType);
            this.pnlLoginFields.Controls.Add(this.txtPassword);
            this.pnlLoginFields.Controls.Add(this.txtUsername);
            this.pnlLoginFields.Controls.Add(this.lblUserName);
            this.pnlLoginFields.Controls.Add(this.lblPassword);
            resources.ApplyResources(this.pnlLoginFields, "pnlLoginFields");
            this.pnlLoginFields.Name = "pnlLoginFields";
            // 
            // btnLogin
            // 
            resources.ApplyResources(this.btnLogin, "btnLogin");
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // pnlLoginType
            // 
            this.pnlLoginType.Controls.Add(this.rbSSO);
            this.pnlLoginType.Controls.Add(this.rbWindows);
            this.pnlLoginType.Controls.Add(this.rbLanguageTerminal);
            this.pnlLoginType.Controls.Add(this.rbMemoQServer);
            resources.ApplyResources(this.pnlLoginType, "pnlLoginType");
            this.pnlLoginType.Name = "pnlLoginType";
            // 
            // rbSSO
            // 
            resources.ApplyResources(this.rbSSO, "rbSSO");
            this.rbSSO.Name = "rbSSO";
            this.rbSSO.UseVisualStyleBackColor = true;
            // 
            // rbWindows
            // 
            resources.ApplyResources(this.rbWindows, "rbWindows");
            this.rbWindows.Name = "rbWindows";
            this.rbWindows.UseVisualStyleBackColor = true;
            this.rbWindows.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbLanguageTerminal
            // 
            resources.ApplyResources(this.rbLanguageTerminal, "rbLanguageTerminal");
            this.rbLanguageTerminal.Name = "rbLanguageTerminal";
            this.rbLanguageTerminal.UseVisualStyleBackColor = true;
            this.rbLanguageTerminal.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbMemoQServer
            // 
            this.rbMemoQServer.Checked = true;
            resources.ApplyResources(this.rbMemoQServer, "rbMemoQServer");
            this.rbMemoQServer.Name = "rbMemoQServer";
            this.rbMemoQServer.TabStop = true;
            this.rbMemoQServer.UseVisualStyleBackColor = true;
            this.rbMemoQServer.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtUsername
            // 
            resources.ApplyResources(this.txtUsername, "txtUsername");
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
            // 
            // lblUserName
            // 
            resources.ApplyResources(this.lblUserName, "lblUserName");
            this.lblUserName.Name = "lblUserName";
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            // 
            // pnlServer
            // 
            this.pnlServer.Controls.Add(this.txtServerName);
            this.pnlServer.Controls.Add(this.lblServerName);
            resources.ApplyResources(this.pnlServer, "pnlServer");
            this.pnlServer.Name = "pnlServer";
            // 
            // txtServerName
            // 
            resources.ApplyResources(this.txtServerName, "txtServerName");
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txtServerName.Leave += new System.EventHandler(this.txtServerName_Leave);
            // 
            // lblServerName
            // 
            resources.ApplyResources(this.lblServerName, "lblServerName");
            this.lblServerName.Name = "lblServerName";
            // 
            // lblLTAccountData
            // 
            resources.ApplyResources(this.lblLTAccountData, "lblLTAccountData");
            this.lblLTAccountData.Name = "lblLTAccountData";
            // 
            // pnlLeftSide
            // 
            this.pnlLeftSide.BackColor = System.Drawing.Color.White;
            this.pnlLeftSide.Controls.Add(this.gplGeneralSettings);
            this.pnlLeftSide.Controls.Add(this.lnkAddServer);
            this.pnlLeftSide.Controls.Add(this.pbAddMemoQServer);
            resources.ApplyResources(this.pnlLeftSide, "pnlLeftSide");
            this.pnlLeftSide.Name = "pnlLeftSide";
            // 
            // gplGeneralSettings
            // 
            resources.ApplyResources(this.gplGeneralSettings, "gplGeneralSettings");
            this.gplGeneralSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplGeneralSettings.BackEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplGeneralSettings.BackStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.gplGeneralSettings.Controls.Add(this.lblGeneralSettings);
            this.gplGeneralSettings.GradientMode = TradosPlugin.GradientPanel.GradientModes.TwoPoint;
            this.gplGeneralSettings.GradientOrientation = System.Windows.Forms.Orientation.Horizontal;
            this.gplGeneralSettings.Name = "gplGeneralSettings";
            this.gplGeneralSettings.Click += new System.EventHandler(this.gplGeneralSettings_Click);
            // 
            // lblGeneralSettings
            // 
            resources.ApplyResources(this.lblGeneralSettings, "lblGeneralSettings");
            this.lblGeneralSettings.BackColor = System.Drawing.Color.Transparent;
            this.lblGeneralSettings.ForeColor = System.Drawing.Color.White;
            this.lblGeneralSettings.Name = "lblGeneralSettings";
            this.lblGeneralSettings.Click += new System.EventHandler(this.lblGeneralSettings_Click);
            // 
            // lnkAddServer
            // 
            this.lnkAddServer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lnkAddServer, "lnkAddServer");
            this.lnkAddServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.lnkAddServer.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkAddServer.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.lnkAddServer.Name = "lnkAddServer";
            this.lnkAddServer.TabStop = true;
            this.lnkAddServer.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(99)))), ((int)(((byte)(130)))));
            this.lnkAddServer.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddServer_LinkClicked);
            // 
            // pbAddMemoQServer
            // 
            resources.ApplyResources(this.pbAddMemoQServer, "pbAddMemoQServer");
            this.pbAddMemoQServer.Name = "pbAddMemoQServer";
            this.pbAddMemoQServer.TabStop = false;
            // 
            // lblServers
            // 
            resources.ApplyResources(this.lblServers, "lblServers");
            this.lblServers.BackColor = System.Drawing.Color.Transparent;
            this.lblServers.ForeColor = System.Drawing.Color.White;
            this.lblServers.Name = "lblServers";
            // 
            // pnlVerticalLine
            // 
            this.pnlVerticalLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.pnlVerticalLine, "pnlVerticalLine");
            this.pnlVerticalLine.Name = "pnlVerticalLine";
            // 
            // btnOpenLTAccount
            // 
            resources.ApplyResources(this.btnOpenLTAccount, "btnOpenLTAccount");
            this.btnOpenLTAccount.Name = "btnOpenLTAccount";
            this.btnOpenLTAccount.UseVisualStyleBackColor = false;
            this.btnOpenLTAccount.Click += new System.EventHandler(this.btnOpenLTAccount_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // SettingsForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.btnOpenLTAccount);
            this.Controls.Add(this.pnlLeftSide);
            this.Controls.Add(this.pnlVerticalLine);
            this.Controls.Add(this.pnlLogin);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pnlTMs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.pnlTMs.ResumeLayout(false);
            this.pnlTMs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgrTMs)).EndInit();
            this.pnlLogin.ResumeLayout(false);
            this.gplAccessDetails.ResumeLayout(false);
            this.pnlLoginFields.ResumeLayout(false);
            this.pnlLoginFields.PerformLayout();
            this.pnlLoginType.ResumeLayout(false);
            this.pnlServer.ResumeLayout(false);
            this.pnlServer.PerformLayout();
            this.pnlLeftSide.ResumeLayout(false);
            this.pnlLeftSide.PerformLayout();
            this.gplGeneralSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbAddMemoQServer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTMs;
        private System.Windows.Forms.DataGridView dgrTMs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTarget;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPermissions;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSDLLookup;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSDLUpdate;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtFilter;
        private GradientPanel gplTMs;
        private System.Windows.Forms.Label lblTMs;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Panel pnlLogin;
        private System.Windows.Forms.Panel pnlLoginFields;
        private System.Windows.Forms.Panel pnlLoginType;
        private System.Windows.Forms.Panel pnlLeftSide;
        private System.Windows.Forms.RadioButton rbWindows;
        private System.Windows.Forms.RadioButton rbLanguageTerminal;
        private System.Windows.Forms.RadioButton rbMemoQServer;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Panel pnlServer;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.Label lblServerName;
        private System.Windows.Forms.Label lblLTAccountData;
        private GradientPanel gplAccessDetails;
        private System.Windows.Forms.Label lblServerAccessDetails;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.RadioButton rbSSO;
        private System.Windows.Forms.Panel pnlVerticalLine;
        private System.Windows.Forms.ComboBox cmbTargetLang;
        private System.Windows.Forms.Label lblTargetLang;
        private System.Windows.Forms.LinkLabel lnkAddServer;
        private System.Windows.Forms.PictureBox pbAddMemoQServer;
        private GradientPanel gplTMServers;
        private System.Windows.Forms.Label lblServers;
        private System.Windows.Forms.Label lblNoTMs;
        private GradientPanel gplGeneralSettings;
        private System.Windows.Forms.Label lblGeneralSettings;
        private System.Windows.Forms.Button btnChangeAccount;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnOpenLTAccount;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Label lblWarning;
    }
}