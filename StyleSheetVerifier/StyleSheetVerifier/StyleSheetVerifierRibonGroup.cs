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

	[Action("StyleSheetVerifier", Name = "StyleSheet Verifier", Icon = "icon",Description = "Launches a preview tool for XML/XSLT")]
	[ActionLayout(typeof(StyleSheetVerifierRibonGroup), 11, DisplayType.Large)]
	public class StyleSheetVerifierAction : AbstractAction
	{
		protected override void Execute()
		{
			var verifier = new StyleSheetVerifierForm();
			verifier.ShowDialog();
		}
	}
	[Action("StyleSheetVerifierHelpAction", Icon = "Question", Name = "StyleSheet Verifier Help", Description = "A Wiki documentation will be opened in the browser")]
	[ActionLayout(typeof(StyleSheetVerifierRibonGroup), 10, DisplayType.Large)]
	public class StyleSheetVerifierActionHelp : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3170/stylesheet-verifier");
		}
	}
}