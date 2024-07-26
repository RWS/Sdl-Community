using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace MicrosoftTranslatorProvider.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = $"{Constants.MicrosoftNaming_FullName} - Wiki";
		}

		public override bool IsAvailable => true;

		public override string Category => $"{Constants.MicrosoftNaming_FullName}";

		public override Icon Icon => PluginResources.Question;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_HelpUrl);
		}
	}
}