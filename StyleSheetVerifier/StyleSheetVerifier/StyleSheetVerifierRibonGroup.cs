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
			var verifier = new Form1();
			verifier.ShowDialog();
		}
	}
	[Action("StyleSheetVerifierHelpAction", Icon = "question", Name = "StyleSheet Verifier Help", Description = "An wikie page will be opened in browser uith documentation")]
	[ActionLayout(typeof(StyleSheetVerifierRibonGroup), 10, DisplayType.Large)]
	public class StyleSheetVerifierActionHelp : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3170.stylesheet-preview");
		}
	}
}
