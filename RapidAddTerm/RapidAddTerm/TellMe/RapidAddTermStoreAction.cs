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
			Process.Start("https://appstore.rws.com/Plugin/35");
		}
		public override bool IsAvailable => true;
		public override string Category => "Rapid Add Terms results";
		public override Icon Icon => PluginResources.TellMe1;
	}
}
