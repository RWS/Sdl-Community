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
    public partial class MessageBoxWithCheckboxForm : Form
    {
        public bool DontShowAgain
        {
            get { return chkDontShow.Checked; }
        }

        private string linkURL = "";

        public MessageBoxWithCheckboxForm()
        {
            InitializeComponent();
            btnOK.Text = PluginResources.btnOK;
            chkDontShow.Text = PluginResources.chkDontShow;
        }

        public MessageBoxWithCheckboxForm(string title, string message)
            : this()
        {
            this.Text = title;
            lblMessage.Text = message;
            lnkLink.Visible = false;
            // sizing
            chkDontShow.Top = lblMessage.Bottom + 4;
            btnOK.Top = chkDontShow.Bottom + 4;
            sizeControls();
        }

        public MessageBoxWithCheckboxForm(string title, string message, string linkText, string linkURL, bool showCheckbox)
            : this()
        {
            this.Text = title;
            lblMessage.Text = message;
            lnkLink.Text = linkText;
            this.linkURL = linkURL;
            if (!showCheckbox)
            {
                chkDontShow.Visible = false;
                lnkLink.Top = lblMessage.Bottom + 4;
                btnOK.Top = lnkLink.Bottom + 4;
            }
            else
            {
                lnkLink.Top = lblMessage.Bottom + 4;
                chkDontShow.Top = lnkLink.Bottom + 4;
                btnOK.Top = chkDontShow.Bottom + 4;
            }
            // sizing
            sizeControls();
        }

        private void lnkLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(this.linkURL);
        }

        private void sizeControls()
        {
            this.Height = btnOK.Bottom + 4;
            lblMessage.Left = this.Width / 2 - lblMessage.Width / 2;
            btnOK.Left = this.Width / 2 - btnOK.Width / 2;
        }
    }
}
