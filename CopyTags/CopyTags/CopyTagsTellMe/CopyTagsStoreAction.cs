using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace SDLCopyTags.CopyTagsTellMe
{
	public class CopyTagsStoreAction : AbstractTellMeAction
	{
		public CopyTagsStoreAction()
		{
			Name = "Download SDLCopyTags from AppStore";
		}

		public override void Execute()
		{
			Process.Start("");
		}

		public override bool IsAvailable => true;
		public override string Category => "SDLCopyTags results";
		public override Icon Icon => PluginResources.Download;
	}
}