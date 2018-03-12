using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sdl.Community.Trados2007.UI
{
    public partial class AddEditServerDialog : Form
    {
        public AddEditServerDialog(Trados2007ServerAccount account = null)
        {
            InitializeComponent();

            this.Text = account == null ? 
                PluginResources.Trados2007_AddEditServerDialog_Add : 
                PluginResources.Trados2007_AddEditServerDialog_Edit;

            if (account != null)
            {
                this.addressTextBox.Text = account.TranslationServer;

                if (account.IsWindowsAuthentication)
                {
                    this.windowsAuthRadioButton.Checked = true;
                }
                else
                {
                    this.serverAuthRadioButton.Checked = true;
                    
                    this.loginTextBox.Text = account.Login;
                }
            }
            else
            {
                this.serverAuthRadioButton.Checked = true;
            }

            InitEnabled();
        }

        //public AddEditServerDialog(string path)
        //{
        //    InitializeComponent();

        //    this.Text = PluginResources.Trados2007_AddEditServerDialog_Login;

        //    this.addressTextBox.Text = path;

        //    InitEnabled();
        //}

        private void InitEnabled()
        {
            this.addressTextBox.TextChanged += OnUpdateOkEnabled;
            this.loginTextBox.TextChanged += OnUpdateOkEnabled;
            this.passwordTextBox.TextChanged += OnUpdateOkEnabled;

            this.serverAuthRadioButton.CheckedChanged += OnAuthTypeChanged;
            this.windowsAuthRadioButton.CheckedChanged += OnAuthTypeChanged;

            UpdateOkEnabled();
            UpdateAuthType();
        }

        void OnAuthTypeChanged(object sender, EventArgs e)
        {
            UpdateAuthType();
        }

        private void UpdateAuthType()
        {
            bool enabled = this.serverAuthRadioButton.Checked;

            this.loginTextBox.Enabled = enabled;
            this.passwordTextBox.Enabled = enabled;

            UpdateOkEnabled();
        }

        private void OnOkButtonClick(object sender, EventArgs e)
        {
            ServerAccount = this.windowsAuthRadioButton.Checked ? 
                new Trados2007ServerAccount(this.addressTextBox.Text) :
                new Trados2007ServerAccount(
                        this.addressTextBox.Text,
                        this.loginTextBox.Text,
                        this.passwordTextBox.Text);

            if (!ServerAccount.Valid)
            {
                MessagingHelpers.ShowError(PluginResources.Trados2007_AddEditServerDialog_WrongCredentials);
                return;
            }
                
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public Trados2007ServerAccount ServerAccount { get; private set; }

        private void OnUpdateOkEnabled(object sender, EventArgs e)
        {
            UpdateOkEnabled();
        }

        private void UpdateOkEnabled()
        {
            okButton.Enabled = this.windowsAuthRadioButton.Checked || 
                this.addressTextBox.Text.Length != 0 && this.loginTextBox.Text.Length != 0 && this.passwordTextBox.Text.Length != 0;
        }
    }
}
