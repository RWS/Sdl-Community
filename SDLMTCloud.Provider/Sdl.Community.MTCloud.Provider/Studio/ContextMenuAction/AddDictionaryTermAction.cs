using System;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.MTCloud.Provider.Studio.ContextMenuAction
{
	[Action("Add Dictionary Term",
		Name = "Add Term to MT Cloud Dictionary",
		Icon = "add_dictionary",
		Description = "Add a term to the current dictionary")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AddDictionaryTermAction : AbstractAction
	{
		protected override async void Execute()
		{
			var selection = MtCloudApplicationInitializer.EditorController?.ActiveDocument?.Selection;

			var term = new Term
			{
				Source = selection?.Source.ToString(),
				Target = selection?.Target.ToString()
			};

			if (MtCloudApplicationInitializer.TranslationService == null || !IsSdlMtAddedToCurrentProject())
			{
				MtCloudApplicationInitializer.MessageService.ShowWarningMessage(PluginResources.SDL_MT_Cloud_Provider_is_not_added_to_the_current_project, PluginResources.Operation_failed);
				return;
			}
			await MtCloudApplicationInitializer.TranslationService.AddTermToDictionary(term);
		}

		private bool IsSdlMtAddedToCurrentProject()
		{
			return MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project
				.GetTranslationProviderConfiguration().
				Entries?.Any(
					entry =>
						entry.MainTranslationProvider
							.Uri
							.ToString().Contains("sdlmtcloud")) ?? false;
		}
	}
}