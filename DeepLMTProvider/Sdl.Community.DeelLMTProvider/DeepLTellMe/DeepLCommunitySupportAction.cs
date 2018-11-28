using System.Diagnostics;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLCommunitySupportAction : AbstractTellMeAction
	{
		public DeepLCommunitySupportAction()
		{
			Name = "DeepL Community support";
		}

		public override void Execute()
		{
			Process.Start("http://community.sdl.com/appsupport");
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";
		//public override Icon Icon => PluginResources.Question;
	}
}
