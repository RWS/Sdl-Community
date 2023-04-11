using Sdl.TellMe.ProviderApi;

namespace Trados.TargetRenamer.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => $"{PluginResources.TargetRenamer_Name} Tell Me provider";

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
                new HelpAction
                {
                    Keywords = new []{ $"{PluginResources.TargetRenamer_Name.ToLower()}", "help", "guide" }
                },
				new AppStoreForumAction
			    {
				    Keywords = new[] { "target", "renamer", "targetrenamer", "support", "forum" }
			    },
			    new AppStoreDownloadAction
			    {
				    Keywords = new[] { "target", "renamer", "targetrenamer", "store", "download", "appstore" }
				}
		};
    }
}