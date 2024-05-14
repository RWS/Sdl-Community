using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.TargetWordCount.TellMe
{
	[TellMeProvider]
	public class TargetWordCountTellMeProvider : ITellMeProvider
	{
		public string Name => PluginResources.Plugin_Name;

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new TargetWordCountDocumentationAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "target word count wiki" }
			},
			new TargetWordCountCommunityForumAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "target word count forum" }
			},new SettingsAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "settings" }
			},new SourceCodeAction
			{
				Keywords = new[] { "target word count", "target word count community", "target word count support", "source code" }
			}
		};
	}
}