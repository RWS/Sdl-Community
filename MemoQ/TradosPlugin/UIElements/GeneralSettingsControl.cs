using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TradosPlugin
{
    public partial class GeneralSettingsControl : UserControl
    {
        public bool ConcordanceNumericEquiv
        {
            get { return this.chkConcNumericEquiv.Checked; }
        }

        public bool ConcordanceCaseSensitive
        {
            get { return this.chkConcCaseSens.Checked; }
        }

        public delegate void HeaderClickedHandler();
        public event HeaderClickedHandler HeaderClicked;

        /// <summary>
        /// For the designer. Use the other constructor to initialize checkboxes.
        /// </summary>
        public GeneralSettingsControl()
        {
            InitializeComponent();
            chkConcCaseSens.Text = PluginResources.chkConcCaseSens;
            chkConcNumericEquiv.Text = PluginResources.chkConcNumericEquiv;
            lblGeneralSettings.Text = PluginResources.grpGeneralSettings + "  ▼"; // ▼ U+25BC BLACK DOWN-POINTING TRIANGLE
            // ▾ - U+25BE SMALL BLACK DOWN-POINTING TRIANGLE
        }

        public GeneralSettingsControl(bool numEquiv, bool caseSens)
            : this()
        {
            this.chkConcCaseSens.Checked = caseSens;
            this.chkConcNumericEquiv.Checked = numEquiv;
        }

        private void gradientPanel4_Click(object sender, EventArgs e)
        {
            if (HeaderClicked != null) HeaderClicked();
        }

        private void lblGeneralSettings_Click(object sender, EventArgs e)
        {
            if (HeaderClicked != null) HeaderClicked();
        }


    }
}
