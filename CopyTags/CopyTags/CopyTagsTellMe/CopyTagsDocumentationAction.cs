using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsDocumentationAction : AbstractTellMeAction
	{
		public CopyTagsDocumentationAction()
		{
			Name = "Trados Copy Tags Documentation";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/26?tab=documentation");
		}

		public override bool IsAvailable => true;
		public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.TellMe_Documentation;
	}
}
