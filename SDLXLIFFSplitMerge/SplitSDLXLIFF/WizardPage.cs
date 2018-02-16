using Sdl.Utilities.SplitSDLXLIFF.Wizard;
using System.Collections.Generic;
using Sdl.Utilities.SplitSDLXLIFF.Helpers;
using Sdl.Utilities.SplitSDLXLIFF.Lib;

namespace Sdl.Utilities.SplitSDLXLIFF
{
    public partial class WizardPage : WizardSheet
    {
        public WizardPage()
        {
            InitializeComponent();

            this.Pages.Add(new WelcomePage());
            this.Pages.Add(new FileOptionsPage());
            this.Text = Constants.SDLXLIFFName;

			AppOptions.splitOpts.SplitNonCountStatus = new List<SegStatus>();
			AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedSignOff);
			AppOptions.splitOpts.SplitNonCountStatus.Add(SegStatus.ApprovedTranslation);

		}
    }
}
