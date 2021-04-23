using System;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
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
			Enabled = false;
			//TODO: move this functionality to its own class, sharing it with other features
			//Because this object gets constructed before the EditorController has been initialized, we need to make sure that the subscription to the event happens only after the EditorController has had a chance to be initialized
			//We cannot do this in the MTAppInitializer because this object gets constructed even before MtAppInit.Execute() is called
			MtCloudApplicationInitializer.Subscribe<StudioWindowCreatedNotificationEvent>(
				_ => MtCloudApplicationInitializer.EditorController.ActiveDocumentChanged +=
					(_, _) => EnableAction());

			MtCloudApplicationInitializer.Subscribe<TranslationProviderAdded>(_ => EnableAction(new TranslationProviderStatusChanged(null, true)));

			MtCloudApplicationInitializer.Subscribe<TranslationProviderStatusChanged>(EnableAction);
		}

		private void EnableAction(TranslationProviderStatusChanged tpStatus = null)
		{
			if (MtCloudApplicationInitializer.GetProjectInProcessing().GetProjectInfo().Id !=
			    MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project.GetProjectInfo().Id) return;
			var selection = MtCloudApplicationInitializer.EditorController?.ActiveDocument?.Selection;

			var term = new Term
			{
				Source = selection?.Source.ToString(),
				Target = selection?.Target.ToString()
			};

			if (MtCloudApplicationInitializer.TranslationService == null)
			{
				MtCloudApplicationInitializer.MessageService.ShowWarningMessage(PluginResources.SDL_MT_Cloud_Provider_is_not_added_to_the_current_project, PluginResources.Operation_failed);
				return;
			}
			await MtCloudApplicationInitializer.TranslationService.AddTermToDictionary(term);
		}
	}
}