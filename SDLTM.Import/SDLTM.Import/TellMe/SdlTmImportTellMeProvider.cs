using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	[TellMeProvider]
	public class SdlTmImportTellMeProvider : ITellMeProvider
	{
		public string Name => "SDLTM Import Plus Tell Me";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new SdlTmImportCommunitySupportAction
			{
				Keywords = new[] {"sdltm import plus", "sdltmimport plus", "sdltm import plus community", "sdltmimport plus support" }
			},
			new SdlTmImportHelpAction
			{
				Keywords = new[] {"sdltm import plus", "sdltmimport help", "sdltm import plus wiki", "sdltmimport plus wiki" }
			},
			new SdlTmImportStoreAction
			{
				Keywords = new[] {"sdltm import plus", "sdltmimport download", "sdltm import plus download", "sdltmimport plus download" }
			}
		};
	}
}
