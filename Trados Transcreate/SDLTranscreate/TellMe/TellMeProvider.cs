using Sdl.TellMe.ProviderApi;
using Trados.Transcreate.TellMe.Actions;

namespace Trados.Transcreate.TellMe
{
	[TellMeProvider]
	public class TellMeProvider : ITellMeProvider
	{
		public string Name => $"{PluginResources.Plugin_Name} Tell Me provider";

		public AbstractTellMeAction[] ProviderActions =>
		[
			new DocumentationAction { Keywords = ["trans", "create", "transcreate", "community", "support", "wiki"] },
			new CommunityAppStoreForumAction { Keywords = ["trans", "create", "transcreate", "support", "forum"] },
			new ConvertToTranscreateProjectAction { Keywords = ["trans", "create", "transcreate", "convert project"] },
			new ShowTranscreateViewAction { Keywords = ["trans", "create", "transcreate", "view"] },
			new SettingsAction { Keywords = ["trans", "create", "transcreate", "settings"] },
			new ImportAction { Keywords = ["trans", "create", "transcreate", "import"] },
			new ExportAction { Keywords = ["trans", "create", "transcreate", "export"] },
			new CreateBackTranslationAction { Keywords = ["trans", "create", "transcreate", "create back translation back-translation backtranslation"] }
		];
	}
}
