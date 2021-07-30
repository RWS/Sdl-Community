using Sdl.TellMe.ProviderApi;

namespace Trados.TargetRenamer.TellMe
{
	[TellMeProvider]
    public class TellMeProvider : ITellMeProvider
    {
        public string Name => $"{PluginResources.TargetRenamer_Name} tell me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
                new HelpAction
                {
                    Keywords = new []{ $"{PluginResources.TargetRenamer_Name.ToLower()}", "help", "guide" }
                },
        };
    }
}