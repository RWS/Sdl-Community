using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleTranslatorProvider.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = $"{Constants.GooglePluginName} - Wiki";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Question;

		public override string Category => Constants.GooglePluginName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_HelpUrl);
		}
	}
}