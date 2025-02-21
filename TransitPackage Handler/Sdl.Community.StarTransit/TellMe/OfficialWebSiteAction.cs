using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	public class OfficialWebSiteAction : AbstractTellMeAction
	{
		public OfficialWebSiteAction()
		{
			Name = "Official Star Translation Website";
		}
		public override void Execute()
		{
			Process.Start("https://www.star-ts.com/software/");
		}

		public override bool IsAvailable => true;
		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.starTs_oficialWebSite;
	}
}
