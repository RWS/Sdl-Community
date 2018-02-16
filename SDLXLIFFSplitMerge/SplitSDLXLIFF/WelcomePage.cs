using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sdl.Utilities.SplitSDLXLIFF.Wizard;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class WelcomePage : InternalWizardPage
    {

        public WelcomePage()
        {
            InitializeComponent();

            this.Banner.Title = Properties.Resources.msgWelcomeTitle;
            this.Banner.Subtitle = Properties.Resources.msgWelcomeSubtitle;

            rbSplit.Checked = true;
        }

        private void WelcomePage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next);
            EnableCancelButton(true);
        }

        private void WelcomePage_WizardNext(object sender, WizardPageEventArgs e)
        {
            for (int i = GetWizard().Pages.Count; i > 2; i--)
                GetWizard().Pages.RemoveAt(GetWizard().Pages.Count - 1);

            if (rbSplit.Checked)
            {
                AppOptions.isMerge = false;
                GetWizard().Pages.Add(new OptionsPage());
                GetWizard().Pages.Add(new PerformSplit());
            }
            else
            {
                AppOptions.isMerge = true;
                GetWizard().Pages.Add(new PerformMerge());
            }
        }

    }
}
