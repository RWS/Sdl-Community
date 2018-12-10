using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace IATETerminologyProvider.IATEProviderTellMe
{
	public class IATEStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "IATE results";
		public override Icon Icon => PluginResources.Download;

		public IATEStoreAction()
		{
			Name = "Download IATE Terminology Provider from AppStore";
		}

		public override void Execute()
		{
			// To do: add the link to the SDL app store after plugin will be published
			//Process.Start("https://appstore.sdl.com/language/app/deepl-translation-provider/847/");
		}
	}
}