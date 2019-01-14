using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace ETSTranslationProvider.ETSTellMe
{
	public class ETSTranslationServerAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "ETS results";
		public override Icon Icon => PluginResources.ForumIcon;

		public ETSTranslationServerAction()
		{
			Name = "SDL Enterprise translation server";
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/software-and-services/translation-software/machine-translation/enterprise-translation-server.html");
		}
	}
}