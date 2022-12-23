using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	public class HelpAction : AbstractTellMeAction
	{
		public HelpAction()
		{
			Name = $"{Constants.GoogleNaming_FullName} - Wiki";
		}

		public override bool IsAvailable => true;

		public override Icon Icon => PluginResources.Question;

		public override string Category => Constants.GoogleNaming_FullName;

		public override void Execute()
		{
			Process.Start(Constants.TellMe_HelpUrl);
		}
	}
}