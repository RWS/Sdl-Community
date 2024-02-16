using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradosPlugin
{
    public partial class GetCredentialsForm : Form
    {
        public GetCredentialsForm()
        {
            InitializeComponent();
            label1.Text = PluginResources.lblUsername;
            label2.Text = PluginResources.lblPassword;
        }

        public string UserName
        {
            get
            {
                return txtUserName.Text;
            }
            set
            {
                txtUserName.Text = value;
            }
        }

        public string Password
        {
            get
            {
                return txtPassword.Text;
            }
            set
            {
                txtPassword.Text = value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
