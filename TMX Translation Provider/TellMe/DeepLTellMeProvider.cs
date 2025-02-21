using Sdl.TellMe.ProviderApi;

namespace TMX_TranslationProvider.Tellme
{
	[TellMeProvider]
    public class TmxProviderTellMeProvider : ITellMeProvider
    {
        public string Name => "TmxProvider Tell Me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new TmxProviderStoreAction
            {
                Keywords = new[] {"TmxProvider", "TmxProvider store", "TmxProvider download"}
            },
            new TmxProviderContactAction
            {
                Keywords = new[] {"TmxProvider", "TmxProvider contact", "TmxProvider trial"}
            },
            new TmxProviderHelpAction
            {
                Keywords = new[] {"TmxProvider", "TmxProvider help", "TmxProvider guide"}
            },
            new TmxProviderCommunitySupportAction
            {
                Keywords = new[] {"TmxProvider", "TmxProvider community", "TmxProvider support"}
            },
            new TmxProviderSettingsAction
            {
                Keywords = new[] {"TmxProvider", "TmxProvider settings", "settings"}
            }
        };
    }
}