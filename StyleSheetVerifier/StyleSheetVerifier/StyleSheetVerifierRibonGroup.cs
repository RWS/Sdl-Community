using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.StyleSheetVerifier
{
	[RibbonGroup("StyleSheetVerifier", Name = "StyleSheet Verifier")]
	[RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class StyleSheetVerifierRibonGroup:AbstractRibbonGroup
	{
	}

	[Action("StyleSheetVerifier", Name = "StyleSheet Verifier", Icon = "icon")]
	[ActionLayout(typeof(StyleSheetVerifierRibonGroup), 11, DisplayType.Large)]
	public class StyleSheetVerifierAction : AbstractAction
	{
		protected override void Execute()
		{
			var verifier = new Form1();
			verifier.ShowDialog();
		}
	}
}
