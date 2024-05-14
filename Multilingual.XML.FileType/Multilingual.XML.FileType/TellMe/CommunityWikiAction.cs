using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.XML.FileType
{
	public class CommunityWikiAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.TellmeDocumentation;

		public CommunityWikiAction()
		{
			Name = string.Format("{0} Documentation", PluginResources.Plugin_Name);
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/13?tab=documentation");
		}
	}
}
