using Sdl.Community.Qualitivity.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.Qualitivity.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction
			{
				Keywords = new[] { "qualitivity", "community", "documentation" }
			},
			new AppStoreForumAction
			{
				Keywords = new[] { "qualitivity", "support", "forum" }
			},
			new SourceCodeAction
			{
				Keywords = new[] { "qualitivity", "source code" }
			},
			new SettingsAction
			{
				Keywords = new[] { "qualitivity", "settings" }
			},
			new NewQualitivityProjectAction
			{
				Keywords = new[] { "qualitivity", "new qualitivity project" }
			},
			new QualitivityViewAction
			{
				Keywords = new[] { "qualitivity", "qualitivity view" }
			},
			new NewProjectActivityAction
			{
				Keywords = new[] { "qualitivity", "new qualitivity project activity" }
			},
			new StopTrackingAction
			{
				Keywords = new[] { "qualitivity", "stop tracking" }
			}
		};
	}
}
