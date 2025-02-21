using Sdl.TellMe.ProviderApi;

namespace SDLTM.Import.TellMe
{
	[TellMeProvider]
	public class SdlTmImportTellMeProvider : ITellMeProvider
	{
		public string Name => "SDLTM Import Plus Tell Me";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
            new SdlTmImportDocumentationAction
            {
                Keywords = new[] {"sdltm import plus", "sdltmimport", "documentation" }
            },
            new SdlTmImportCommunitySupportAction
			{
				Keywords = new[] {"sdltm import plus", "sdltmimport plus", "sdltm import plus community", "sdltmimport plus support", "forum" }
			},
			new SdlTmImportSourceCodeAction
			{
				Keywords = new[] {"sdltm import plus", "sdltmimport", "source", "code" }
			},
            new SdlTmImportSettingsAction
            {
                Keywords = new[] {"sdltm import plus", "sdltmimport", "settings" }
            }
        };
	}
}
