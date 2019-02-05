using System.Diagnostics;
using System.Drawing;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.FileTypeSupport.MXLIFF.TellMe
{
	public class MXLIFFStoreAction : AbstractTellMeAction
	{
		public override bool IsAvailable => true;
		public override string Category => "MXLIFF results";
		public override Icon Icon => PluginResources.Download;

		public MXLIFFStoreAction()
		{
			Name = "Download MXLIFF File Type Support plugin from AppStore";
		}

		public override void Execute()
		{
			Process.Start("https://appstore.sdl.com/language/app/mxliff-file-type/962/");
		}
	}
}