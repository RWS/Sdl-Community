using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace ETSTranslationProvider.ETSTellMe
{
	public class ETSLanguagePairsAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "SDL MT Edge results";
		public override Icon Icon => PluginResources.LanguagePairsIcon;

		public ETSLanguagePairsAction()
		{
			Name = "SDL MT Edge Language Pairs";
		}

		public override void Execute()
		{
			Process.Start("https://www.sdl.com/software-and-services/translation-software/machine-translation/language-pairs.html");
		}
	}
}
