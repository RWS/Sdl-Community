﻿using Sdl.TellMe.ProviderApi;

namespace GoogleCloudTranslationProvider.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public TellMeProvider()
		{
			ProviderActions = GetProviderActions();
		}

		public string Name => PluginResources.Plugin_Name;

		public AbstractTellMeAction[] ProviderActions { get; }

		private AbstractTellMeAction[] GetProviderActions()
		{
			var forumAction = new CommunityForumAction();
			var helpAction = new DocumentationAction();
			var sourceCodeAction = new SourceCodeAction();
			var settingsAction = new SettingsAction();

			var providerActions = new AbstractTellMeAction[] { forumAction, helpAction, sourceCodeAction, settingsAction };

			return providerActions;
		}
	}
}