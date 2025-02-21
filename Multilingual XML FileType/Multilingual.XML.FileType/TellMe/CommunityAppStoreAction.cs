using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Multilingual.XML.FileType
{
	public class CommunityAppStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;

		public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);

		public override Icon Icon => PluginResources.Download;

		public CommunityAppStoreAction()
		{
			Name = string.Format(PluginResources.TellMe_Download_Plugin_From_AppStore, PluginResources.Plugin_Name); 
		}

		public override void Execute()
		{			
			Process.Start("https://appstore.rws.com/Plugin/13");
		}
	}
}
