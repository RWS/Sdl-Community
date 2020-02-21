using System.Windows.Forms;

namespace ETSTranslationProvider
{
    partial class ProviderConfDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProviderConfDialog));
            this.CancelBtn = new System.Windows.Forms.Button();
            this.UsernameField = new System.Windows.Forms.TextBox();
            this.PasswordField = new System.Windows.Forms.TextBox();
            this.UsernameLabel = new System.Windows.Forms.Label();
            this.PasswordLabel = new System.Windows.Forms.Label();
            this.SaveCredentialsOption = new System.Windows.Forms.CheckBox();
            this.HostNameLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.HostNameField = new System.Windows.Forms.TextBox();
            this.PortField = new System.Windows.Forms.TextBox();
            this.AuthenticationGroup = new System.Windows.Forms.GroupBox();
            this.APIKeyLabel = new System.Windows.Forms.Label();
            this.APIKeyField = new System.Windows.Forms.TextBox();
            this.APIKeyOption = new System.Windows.Forms.RadioButton();
            this.BasicAuthenticationOption = new System.Windows.Forms.RadioButton();
            this.FinishButton = new System.Windows.Forms.Button();
            this.TradosLPs = new System.Windows.Forms.DataGridView();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.CredentialTab = new System.Windows.Forms.TabPage();
            this.setDefaultTM = new System.Windows.Forms.CheckBox();
            this.LPTab = new System.Windows.Forms.TabPage();
            this.AuthenticationGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TradosLPs)).BeginInit();
            this.tabControl.SuspendLayout();
            this.CredentialTab.SuspendLayout();
            this.LPTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // CancelBtn
            // 
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(462, 316);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 12;
            this.CancelBtn.Text = "&Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // UsernameField
            // 
            this.UsernameField.Location = new System.Drawing.Point(103, 43);
            this.UsernameField.Name = "UsernameField";
            this.UsernameField.Size = new System.Drawing.Size(395, 20);
            this.UsernameField.TabIndex = 6;
            this.UsernameField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UsernameField_KeyUp);
            // 
            // PasswordField
            // 
            this.PasswordField.Location = new System.Drawing.Point(103, 69);
            this.PasswordField.Name = "PasswordField";
            this.PasswordField.PasswordChar = '*';
            this.PasswordField.Size = new System.Drawing.Size(395, 20);
            this.PasswordField.TabIndex = 7;
            this.PasswordField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PasswordField_KeyUp);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AccessibleName = "UserName";
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(34, 46);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(63, 13);
            this.UsernameLabel.TabIndex = 7;
            this.UsernameLabel.Text = "User Name:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(34, 72);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
            this.PasswordLabel.TabIndex = 8;
            this.PasswordLabel.Text = "Password:";
            // 
            // SaveCredentialsOption
            // 
            this.SaveCredentialsOption.AutoSize = true;
            this.SaveCredentialsOption.Location = new System.Drawing.Point(6, 157);
            this.SaveCredentialsOption.Name = "SaveCredentialsOption";
            this.SaveCredentialsOption.Size = new System.Drawing.Size(106, 17);
            this.SaveCredentialsOption.TabIndex = 10;
            this.SaveCredentialsOption.Text = "Save Credentials";
            this.SaveCredentialsOption.UseVisualStyleBackColor = true;
            this.SaveCredentialsOption.CheckedChanged += new System.EventHandler(this.PersistCredentialsOptionChanged);
            // 
            // HostNameLabel
            // 
            this.HostNameLabel.AutoSize = true;
            this.HostNameLabel.Location = new System.Drawing.Point(6, 12);
            this.HostNameLabel.Name = "HostNameLabel";
            this.HostNameLabel.Size = new System.Drawing.Size(32, 13);
            this.HostNameLabel.TabIndex = 11;
            this.HostNameLabel.Text = "Host:";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(6, 39);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(29, 13);
            this.PortLabel.TabIndex = 12;
            this.PortLabel.Text = "Port:";
            // 
            // HostNameField
            // 
            this.HostNameField.Location = new System.Drawing.Point(69, 6);
            this.HostNameField.Name = "HostNameField";
            this.HostNameField.Size = new System.Drawing.Size(441, 20);
            this.HostNameField.TabIndex = 0;
            this.HostNameField.TextChanged += new System.EventHandler(this.HostNameChanged);
            // 
            // PortField
            // 
            this.PortField.Location = new System.Drawing.Point(69, 33);
            this.PortField.Name = "PortField";
            this.PortField.Size = new System.Drawing.Size(441, 20);
            this.PortField.TabIndex = 1;
            this.PortField.TextChanged += new System.EventHandler(this.PortChanged);
            this.PortField.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PortKeyPressed);
            // 
            // AuthenticationGroup
            // 
            this.AuthenticationGroup.Controls.Add(this.APIKeyLabel);
            this.AuthenticationGroup.Controls.Add(this.APIKeyField);
            this.AuthenticationGroup.Controls.Add(this.APIKeyOption);
            this.AuthenticationGroup.Controls.Add(this.BasicAuthenticationOption);
            this.AuthenticationGroup.Controls.Add(this.UsernameField);
            this.AuthenticationGroup.Controls.Add(this.UsernameLabel);
            this.AuthenticationGroup.Controls.Add(this.PasswordField);
            this.AuthenticationGroup.Controls.Add(this.SaveCredentialsOption);
            this.AuthenticationGroup.Controls.Add(this.PasswordLabel);
            this.AuthenticationGroup.Location = new System.Drawing.Point(6, 88);
            this.AuthenticationGroup.Name = "AuthenticationGroup";
            this.AuthenticationGroup.Size = new System.Drawing.Size(504, 180);
            this.AuthenticationGroup.TabIndex = 2;
            this.AuthenticationGroup.TabStop = false;
            this.AuthenticationGroup.Text = "Authentication";
            // 
            // APIKeyLabel
            // 
            this.APIKeyLabel.AutoSize = true;
            this.APIKeyLabel.Location = new System.Drawing.Point(34, 134);
            this.APIKeyLabel.Name = "APIKeyLabel";
            this.APIKeyLabel.Size = new System.Drawing.Size(45, 13);
            this.APIKeyLabel.TabIndex = 11;
            this.APIKeyLabel.Text = "API Key";
            // 
            // APIKeyField
            // 
            this.APIKeyField.Location = new System.Drawing.Point(103, 131);
            this.APIKeyField.Name = "APIKeyField";
            this.APIKeyField.PasswordChar = '*';
            this.APIKeyField.Size = new System.Drawing.Size(395, 20);
            this.APIKeyField.TabIndex = 9;
            this.APIKeyField.TextChanged += new System.EventHandler(this.APIKeyChanged);
            // 
            // APIKeyOption
            // 
            this.APIKeyOption.AutoSize = true;
            this.APIKeyOption.Location = new System.Drawing.Point(17, 106);
            this.APIKeyOption.Name = "APIKeyOption";
            this.APIKeyOption.Size = new System.Drawing.Size(85, 17);
            this.APIKeyOption.TabIndex = 8;
            this.APIKeyOption.TabStop = true;
            this.APIKeyOption.Text = "Use API Key";
            this.APIKeyOption.UseVisualStyleBackColor = true;
            this.APIKeyOption.CheckedChanged += new System.EventHandler(this.APIKeyOptionChanged);
            // 
            // BasicAuthenticationOption
            // 
            this.BasicAuthenticationOption.AutoSize = true;
            this.BasicAuthenticationOption.Location = new System.Drawing.Point(17, 19);
            this.BasicAuthenticationOption.Name = "BasicAuthenticationOption";
            this.BasicAuthenticationOption.Size = new System.Drawing.Size(123, 17);
            this.BasicAuthenticationOption.TabIndex = 5;
            this.BasicAuthenticationOption.TabStop = true;
            this.BasicAuthenticationOption.Text = "Use SDL Credentials";
            this.BasicAuthenticationOption.UseVisualStyleBackColor = true;
            this.BasicAuthenticationOption.CheckedChanged += new System.EventHandler(this.BasicAuthenticationOptionChanged);
            // 
            // FinishButton
            // 
            this.FinishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.FinishButton.Enabled = false;
            this.FinishButton.Location = new System.Drawing.Point(377, 316);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(75, 23);
            this.FinishButton.TabIndex = 11;
            this.FinishButton.Text = "&OK";
            this.FinishButton.UseVisualStyleBackColor = true;
            this.FinishButton.Click += new System.EventHandler(this.OKClicked);
            // 
            // TradosLPs
            // 
            this.TradosLPs.AllowUserToOrderColumns = true;
            this.TradosLPs.AllowUserToResizeRows = false;
            this.TradosLPs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.TradosLPs.BackgroundColor = System.Drawing.SystemColors.Control;
            this.TradosLPs.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TradosLPs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TradosLPs.Dock = System.Windows.Forms.DockStyle.Top;
            this.TradosLPs.GridColor = System.Drawing.SystemColors.Control;
            this.TradosLPs.Location = new System.Drawing.Point(3, 3);
            this.TradosLPs.Name = "TradosLPs";
            this.TradosLPs.RowHeadersVisible = false;
            this.TradosLPs.Size = new System.Drawing.Size(509, 238);
            this.TradosLPs.TabIndex = 13;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.CredentialTab);
            this.tabControl.Location = new System.Drawing.Point(13, 10);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(524, 300);
            this.tabControl.TabIndex = 13;
            // 
            // CredentialTab
            // 
            this.CredentialTab.BackColor = System.Drawing.SystemColors.Control;
            this.CredentialTab.Controls.Add(this.setDefaultTM);
            this.CredentialTab.Controls.Add(this.PortField);
            this.CredentialTab.Controls.Add(this.HostNameField);
            this.CredentialTab.Controls.Add(this.PortLabel);
            this.CredentialTab.Controls.Add(this.HostNameLabel);
            this.CredentialTab.Controls.Add(this.AuthenticationGroup);
            this.CredentialTab.Location = new System.Drawing.Point(4, 22);
            this.CredentialTab.Name = "CredentialTab";
            this.CredentialTab.Padding = new System.Windows.Forms.Padding(3);
            this.CredentialTab.Size = new System.Drawing.Size(516, 274);
            this.CredentialTab.TabIndex = 0;
            this.CredentialTab.Text = "General";
            // 
            // setDefaultTM
            // 
            this.setDefaultTM.AutoSize = true;
            this.setDefaultTM.Location = new System.Drawing.Point(69, 59);
            this.setDefaultTM.Name = "setDefaultTM";
            this.setDefaultTM.Size = new System.Drawing.Size(161, 17);
            this.setDefaultTM.TabIndex = 13;
            this.setDefaultTM.Text = "Set as Default Host and Port";
            this.setDefaultTM.UseVisualStyleBackColor = true;
            // 
            // LPTab
            // 
            this.LPTab.BackColor = System.Drawing.SystemColors.Control;
            this.LPTab.Controls.Add(this.TradosLPs);
            this.LPTab.Location = new System.Drawing.Point(4, 22);
            this.LPTab.Name = "LPTab";
            this.LPTab.Padding = new System.Windows.Forms.Padding(3);
            this.LPTab.Size = new System.Drawing.Size(515, 291);
            this.LPTab.TabIndex = 1;
            this.LPTab.Text = "Language Pair Mappings";
            // 
            // ProviderConfDialog
            // 
            this.AcceptButton = this.FinishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(550, 352);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.FinishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProviderConfDialog";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SDL MT Edge Options";
            this.AuthenticationGroup.ResumeLayout(false);
            this.AuthenticationGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TradosLPs)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.CredentialTab.ResumeLayout(false);
            this.CredentialTab.PerformLayout();
            this.LPTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion
        private Button CancelBtn;
        private TextBox UsernameField;
        private TextBox PasswordField;
        private Label UsernameLabel;
        private Label PasswordLabel;
        private CheckBox SaveCredentialsOption;
        private Label HostNameLabel;
        private Label PortLabel;
        private TextBox HostNameField;
        private TextBox PortField;
        private GroupBox AuthenticationGroup;
        private RadioButton BasicAuthenticationOption;
        private Label APIKeyLabel;
        private TextBox APIKeyField;
        private RadioButton APIKeyOption;
        private Button FinishButton;
        private DataGridView TradosLPs;
        private TabControl tabControl;
        private TabPage CredentialTab;
        private TabPage LPTab;
        private CheckBox setDefaultTM;
    }
}
