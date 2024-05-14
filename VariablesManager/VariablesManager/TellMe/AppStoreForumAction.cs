using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace VariablesManager.TellMe
{
	public class AppStoreForumAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Question;

		public AppStoreForumAction()
		{
			Name = "RWS Community AppStore Forum";
		}

		public override void Execute()
		{
			Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
		}
	}
}
