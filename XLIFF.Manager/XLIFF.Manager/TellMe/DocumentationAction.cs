using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.XLIFF.Manager.TellMe
{
	public class DocumentationAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.TellmeDocumentation;

		public DocumentationAction()
		{
			Name = "XLIFF Manager Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5583/xliff-manager-for-sdl-trados-studio");
		}
	}
}
