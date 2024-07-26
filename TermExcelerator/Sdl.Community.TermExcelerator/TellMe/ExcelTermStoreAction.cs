using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class ExcelTermStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Term Excelerator results";
		public override Icon Icon => PluginResources.Download;

		public ExcelTermStoreAction()
		{
			Name = "Term Excelerator from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.rws.com/Plugin/59");
		}
	}
}