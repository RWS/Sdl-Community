using System.Windows.Forms;

namespace Sdl.Community.MTEdge.Provider.Dialogs
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
            this.ConnectionBox = new System.Windows.Forms.CheckBox();
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
            this.CancelBtn.Location = new System.Drawing.Point(746, 486);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(112, 35);
            this.CancelBtn.TabIndex = 12;
            this.CancelBtn.Text = "&Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // UsernameField
            // 
            this.UsernameField.Location = new System.Drawing.Point(154, 66);
            this.UsernameField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UsernameField.Name = "UsernameField";
            this.UsernameField.Size = new System.Drawing.Size(649, 26);
            this.UsernameField.TabIndex = 6;
            this.UsernameField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.UsernameField_KeyUp);
            // 
            // PasswordField
            // 
            this.PasswordField.Location = new System.Drawing.Point(154, 106);
            this.PasswordField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PasswordField.Name = "PasswordField";
            this.PasswordField.PasswordChar = '*';
            this.PasswordField.Size = new System.Drawing.Size(649, 26);
            this.PasswordField.TabIndex = 7;
            this.PasswordField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PasswordField_KeyUp);
            // 
            // UsernameLabel
            // 
            this.UsernameLabel.AccessibleName = "UserName";
            this.UsernameLabel.AutoSize = true;
            this.UsernameLabel.Location = new System.Drawing.Point(51, 71);
            this.UsernameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.UsernameLabel.Name = "UsernameLabel";
            this.UsernameLabel.Size = new System.Drawing.Size(93, 20);
            this.UsernameLabel.TabIndex = 7;
            this.UsernameLabel.Text = "User Name:";
            // 
            // PasswordLabel
            // 
            this.PasswordLabel.AutoSize = true;
            this.PasswordLabel.Location = new System.Drawing.Point(51, 111);
            this.PasswordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PasswordLabel.Name = "PasswordLabel";
            this.PasswordLabel.Size = new System.Drawing.Size(82, 20);
            this.PasswordLabel.TabIndex = 8;
            this.PasswordLabel.Text = "Password:";
            // 
            // SaveCredentialsOption
            // 
            this.SaveCredentialsOption.AutoSize = true;
            this.SaveCredentialsOption.Location = new System.Drawing.Point(9, 242);
            this.SaveCredentialsOption.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SaveCredentialsOption.Name = "SaveCredentialsOption";
            this.SaveCredentialsOption.Size = new System.Drawing.Size(155, 24);
            this.SaveCredentialsOption.TabIndex = 10;
            this.SaveCredentialsOption.Text = "Save Credentials";
            this.SaveCredentialsOption.UseVisualStyleBackColor = true;
            this.SaveCredentialsOption.CheckedChanged += new System.EventHandler(this.PersistCredentialsOptionChanged);
            // 
            // HostNameLabel
            // 
            this.HostNameLabel.AutoSize = true;
            this.HostNameLabel.Location = new System.Drawing.Point(9, 18);
            this.HostNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.HostNameLabel.Name = "HostNameLabel";
            this.HostNameLabel.Size = new System.Drawing.Size(47, 20);
            this.HostNameLabel.TabIndex = 11;
            this.HostNameLabel.Text = "Host:";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(9, 60);
            this.PortLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(42, 20);
            this.PortLabel.TabIndex = 12;
            this.PortLabel.Text = "Port:";
            // 
            // HostNameField
            // 
            this.HostNameField.Location = new System.Drawing.Point(104, 9);
            this.HostNameField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.HostNameField.Name = "HostNameField";
            this.HostNameField.Size = new System.Drawing.Size(709, 26);
            this.HostNameField.TabIndex = 0;
            this.HostNameField.TextChanged += new System.EventHandler(this.HostNameChanged);
            // 
            // PortField
            // 
            this.PortField.Location = new System.Drawing.Point(104, 51);
            this.PortField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PortField.Name = "PortField";
            this.PortField.Size = new System.Drawing.Size(709, 26);
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
            this.AuthenticationGroup.Location = new System.Drawing.Point(9, 135);
            this.AuthenticationGroup.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AuthenticationGroup.Name = "AuthenticationGroup";
            this.AuthenticationGroup.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AuthenticationGroup.Size = new System.Drawing.Size(814, 277);
            this.AuthenticationGroup.TabIndex = 2;
            this.AuthenticationGroup.TabStop = false;
            this.AuthenticationGroup.Text = "Authentication";
            // 
            // APIKeyLabel
            // 
            this.APIKeyLabel.AutoSize = true;
            this.APIKeyLabel.Location = new System.Drawing.Point(51, 206);
            this.APIKeyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.APIKeyLabel.Name = "APIKeyLabel";
            this.APIKeyLabel.Size = new System.Drawing.Size(65, 20);
            this.APIKeyLabel.TabIndex = 11;
            this.APIKeyLabel.Text = "API Key";
            // 
            // APIKeyField
            // 
            this.APIKeyField.Location = new System.Drawing.Point(154, 202);
            this.APIKeyField.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.APIKeyField.Name = "APIKeyField";
            this.APIKeyField.PasswordChar = '*';
            this.APIKeyField.Size = new System.Drawing.Size(649, 26);
            this.APIKeyField.TabIndex = 9;
            this.APIKeyField.TextChanged += new System.EventHandler(this.APIKeyChanged);
            // 
            // APIKeyOption
            // 
            this.APIKeyOption.AutoSize = true;
            this.APIKeyOption.Location = new System.Drawing.Point(26, 163);
            this.APIKeyOption.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.APIKeyOption.Name = "APIKeyOption";
            this.APIKeyOption.Size = new System.Drawing.Size(123, 24);
            this.APIKeyOption.TabIndex = 8;
            this.APIKeyOption.TabStop = true;
            this.APIKeyOption.Text = "Use API Key";
            this.APIKeyOption.UseVisualStyleBackColor = true;
            this.APIKeyOption.CheckedChanged += new System.EventHandler(this.APIKeyOptionChanged);
            // 
            // BasicAuthenticationOption
            // 
            this.BasicAuthenticationOption.AutoSize = true;
            this.BasicAuthenticationOption.Location = new System.Drawing.Point(26, 29);
            this.BasicAuthenticationOption.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BasicAuthenticationOption.Name = "BasicAuthenticationOption";
            this.BasicAuthenticationOption.Size = new System.Drawing.Size(189, 24);
            this.BasicAuthenticationOption.TabIndex = 5;
            this.BasicAuthenticationOption.TabStop = true;
            this.BasicAuthenticationOption.Text = "Use RWS Credentials";
            this.BasicAuthenticationOption.UseVisualStyleBackColor = true;
            this.BasicAuthenticationOption.CheckedChanged += new System.EventHandler(this.BasicAuthenticationOptionChanged);
            // 
            // FinishButton
            // 
            this.FinishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.FinishButton.Enabled = false;
            this.FinishButton.Location = new System.Drawing.Point(620, 486);
            this.FinishButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.FinishButton.Name = "FinishButton";
            this.FinishButton.Size = new System.Drawing.Size(112, 35);
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
            this.TradosLPs.RowHeadersWidth = 62;
            this.TradosLPs.Size = new System.Drawing.Size(584, 238);
            this.TradosLPs.TabIndex = 13;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.CredentialTab);
            this.tabControl.Location = new System.Drawing.Point(20, 15);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(844, 462);
            this.tabControl.TabIndex = 13;
            // 
            // CredentialTab
            // 
            this.CredentialTab.BackColor = System.Drawing.SystemColors.Control;
            this.CredentialTab.Controls.Add(this.ConnectionBox);
            this.CredentialTab.Controls.Add(this.setDefaultTM);
            this.CredentialTab.Controls.Add(this.PortField);
            this.CredentialTab.Controls.Add(this.HostNameField);
            this.CredentialTab.Controls.Add(this.PortLabel);
            this.CredentialTab.Controls.Add(this.HostNameLabel);
            this.CredentialTab.Controls.Add(this.AuthenticationGroup);
            this.CredentialTab.Location = new System.Drawing.Point(4, 29);
            this.CredentialTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CredentialTab.Name = "CredentialTab";
            this.CredentialTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.CredentialTab.Size = new System.Drawing.Size(836, 429);
            this.CredentialTab.TabIndex = 0;
            this.CredentialTab.Text = "General";
            // 
            // ConnectionBox
            // 
            this.ConnectionBox.AutoSize = true;
            this.ConnectionBox.Location = new System.Drawing.Point(398, 91);
            this.ConnectionBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ConnectionBox.Name = "ConnectionBox";
            this.ConnectionBox.Size = new System.Drawing.Size(212, 24);
            this.ConnectionBox.TabIndex = 14;
            this.ConnectionBox.Text = "Requires secure protocol";
            this.ConnectionBox.UseVisualStyleBackColor = true;
            this.ConnectionBox.CheckedChanged += new System.EventHandler(this.connectionBox_CheckedChanged);
            // 
            // setDefaultTM
            // 
            this.setDefaultTM.AutoSize = true;
            this.setDefaultTM.Location = new System.Drawing.Point(104, 91);
            this.setDefaultTM.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.setDefaultTM.Name = "setDefaultTM";
            this.setDefaultTM.Size = new System.Drawing.Size(239, 24);
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
            this.LPTab.Size = new System.Drawing.Size(590, 291);
            this.LPTab.TabIndex = 1;
            this.LPTab.Text = "Language Pair Mappings";
            // 
            // ProviderConfDialog
            // 
            this.AcceptButton = this.FinishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(870, 542);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.FinishButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ProviderConfDialog";
            this.Padding = new System.Windows.Forms.Padding(15);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Language Weaver Edge";
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
		private CheckBox ConnectionBox;
	}
}
