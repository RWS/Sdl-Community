using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.ExcelTerminology.ExcelTerminologyTellMe
{
	public class ExcelTermStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "Excel Terminology results";
		public override Icon Icon => PluginResources.Download;

		public ExcelTermStoreAction()
		{
			Name = "Download Excel Terminology / Term Excelerator from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/termexcelerator/534/");
		}
	}
}