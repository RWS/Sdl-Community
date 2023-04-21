using System.Linq;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.Settings.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.MTCloud.Provider.Studio.ContextMenuAction
{
	[Action("Add Dictionary Term",
		Name = "Add Term to Language Weaver Dictionary",
		Icon = "add_dictionary",
		Description = "Add a term to the current dictionary")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AddDictionaryTermAction : AbstractAction
	{
		public AddDictionaryTermAction()
		{
			Enabled = false;
			//Because this object gets constructed before the EditorController has been initialized, we need to make sure that the subscription to the event happens only after the EditorController has had a chance to be initialized
			//We cannot do this in the MTAppInitializer because this object gets constructed even before MtAppInit.Execute() is called
			MtCloudApplicationInitializer.Subscribe<StudioWindowCreatedNotificationEvent>(
				_ => MtCloudApplicationInitializer.EditorController.ActiveDocumentChanged +=
					(_, _) => EnableAction());

			MtCloudApplicationInitializer.Subscribe<TranslationProviderAdded>(_ => EnableAction(new TranslationProviderStatusChanged(null, true)));

			MtCloudApplicationInitializer.Subscribe<TranslationProviderStatusChanged>(EnableAction);
		}

		protected override async void Execute()
		{
			var selection = MtCloudApplicationInitializer.EditorController.ActiveDocument.Selection;
			var term = new Term
			{
				Source = selection.Source.ToString(),
				Target = selection.Target.ToString()
			};

			await MtCloudApplicationInitializer.TranslationService.AddTermToDictionary(term);
		}

		private void EnableAction(TranslationProviderStatusChanged tpStatus = null)
		{
			var currentProject = MtCloudApplicationInitializer.GetProjectInProcessing();
			if (currentProject == null || currentProject.GetProjectInfo().Id !=
				MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project.GetProjectInfo().Id)
				return;

			bool? hasSdlMtAdded;
			if (tpStatus == null)
			{
				hasSdlMtAdded = MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project
					.GetTranslationProviderConfiguration().
					Entries?.FirstOrDefault(
						entry =>
							entry.MainTranslationProvider
								.Uri
								.ToString().Contains(PluginResources.SDLMTCloudUri))?.MainTranslationProvider.Enabled;
			}
			else
			{
				if (!tpStatus.TpUri?.ToString().Contains(PluginResources.SDLMTCloudUri) ?? false)
					return;
				hasSdlMtAdded = tpStatus.NewStatus;
			}

			Enabled = hasSdlMtAdded ?? false;
		}
	}
}