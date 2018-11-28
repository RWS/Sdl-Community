using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLContactAction : AbstractTellMeAction
	{
		public override void Execute()
		{
			Process.Start("https://www.deepl.com/pro.html");
		}

		public DeepLContactAction()
		{
			Name = "Contact DeepL";
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";
		public override Icon Icon => PluginResources.deepLIcon;
	}
}
