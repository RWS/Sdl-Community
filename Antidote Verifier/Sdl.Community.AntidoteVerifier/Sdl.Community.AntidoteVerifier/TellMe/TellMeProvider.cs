using Sdl.Community.AntidoteVerifier.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AntidoteVerifier.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions =>
        [
            new AppStoreForumAction
			{
				Keywords = ["antidote", "verifier", "support", "forum"]
            },new SourceCodeAction
			{
				Keywords = ["antidote", "verifier", "community", "source code", "github"]

            },new DocumentationAction
			{
				Keywords = ["antidote", "verifier", "help", "documentation"]
            },new CorrectorAction
            {
				Keywords = ["antidote", "verifier", "corrector"]
            },new DictionaryAction
            {
				Keywords = ["antidote", "verifier", "dictionary"]
            },new GuidesAction
            {
				Keywords = ["antidote", "verifier", "guides"]
            }
        ];
	}
}