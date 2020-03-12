using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.SDLBatchAnonymize.TellMe
{
	public class SdlBatchAnonymizeStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL Batch Anonymize results";
		public override Icon Icon => PluginResources.Download;

		public SdlBatchAnonymizeStoreAction()
		{
			Name = "Download SDL Batch Anonymize from AppStore";
		}

		public override void Execute()
		{
			Process.Start("");
		}
	}
}
