using Sdl.Community.RecordSourceTU.TellMe.Actions;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.RecordSourceTU.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => "Record Source TU Tell Me Provider";

		public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
		{
			new DocumentationAction() {Keywords = new[] {"record source tu", "support", "forum"}},
			new HelpAction{Keywords = new []{"record source tu help guide"}},
			new SourceCodeAction{Keywords = new []{"record source tu source code"}},
			new SettingsAction{Keywords = new []{"record source tu settings"}},
		};
	}
}