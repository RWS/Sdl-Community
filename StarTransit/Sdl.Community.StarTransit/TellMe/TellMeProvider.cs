﻿using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "StarTransit Tell Me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new CommunitySupportAction
			{
				Keywords = new []{ "startransit", "star transit", "startransit community", "startransit support"}
			},
			new HelpAction
			{
				Keywords = new []{ "startransit", "star transit", "startransit help", "startransit guide" }
			},
			new AppStoreDownloadAction
			{
				Keywords = new[] { "startransit", "star transit", "store", "download", "appstore" }
			}
	};
	}
}