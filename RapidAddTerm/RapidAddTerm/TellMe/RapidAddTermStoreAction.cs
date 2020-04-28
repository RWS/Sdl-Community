using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RapidAddTerm.TellMe
{
	public class RapidAddTermStoreAction : AbstractTellMeAction
	{
		public RapidAddTermStoreAction()
		{
			Name = "Download Rapid Add Term from AppStore";
		}
		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/rapid-add-term/1050/");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
