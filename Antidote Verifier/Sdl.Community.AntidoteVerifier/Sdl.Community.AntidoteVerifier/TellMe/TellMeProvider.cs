using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AntidoteVerifier.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new AppStoreForumAction
			{
				Keywords = new[] { "antidote", "verifier", "support", "forum" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "antidote", "verifier", "store", "download", "appstore" }

			},new SourceCodeAction
			{
				Keywords = new[] { "antidote", "verifier", "community", "source code", "github" }

			},new DocumentationAction
			{
				Keywords = new[] { "antidote", "verifier", "help", "documentation"}
			}
		};
	}
}