﻿using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.DsiViewer.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "DSI Viewer Tell Me provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "dsi", "viewer", "metadata", "community", "support" }
			},
			new HelpAction
			{
				Keywords = new []{ "dsi", "viewer", "metadata", "community", "support", "help" }
			},
			new SourceCodeAction()
			{
				Keywords = new []{ "dsi", "viewer", "metadata", "community", "source code", "github" }
			},
			new SettingsAction()
			{
				Keywords = new []{ "dsi", "viewer", "metadata", "community", "settings" }
			}
		};
	}
}