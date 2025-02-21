using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.StarTransit.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "StarTransit Tell Me provider";
		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new OfficialWebSiteAction
			{
				Keywords = new[] { "startransit","package","handler", "Star", "Transit", "star", "transit", "star transit", "web site", "web", "site" }
			},
			new DocumentationAction
			{
				Keywords = new[] { "startransit","package","handler", "Star", "Transit", "star", "transit", "star transit", "documentation" }
			},
			new CommunitySupportAction
			{
				Keywords = new []{ "startransit", "package", "handler", "Star", "Transit", "star","transit" ,"star transit", "startransit community", "startransit support"}
			},
			new SourceCodeAction
			{
				Keywords = new []{ "startransit", "package", "handler", "Star", "Transit", "star", "transit", "star transit", "startransit source code", "source", "code" }
			},
			new SettingsAction
			{
				Keywords = new []{ "startransit", "package", "handler", "Star", "Transit", "star", "transit", "star transit", "settings","star trasnit settings" }
			}

	};
	}
}