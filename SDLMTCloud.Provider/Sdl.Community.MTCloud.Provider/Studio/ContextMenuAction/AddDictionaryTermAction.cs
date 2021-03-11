using System;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.ProjectAutomation.Settings.Events;
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
		public AddDictionaryTermAction()
		{
			Enabled = false;
			var eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();

			//TODO: move this functionality to its own class, sharing it with other features
			//Because this object gets constructed before the EditorController has been initialized, we need to make sure that the subscription to the event happens only after the EditorController has had a chance to be initialized
			//We cannot do this in the MTAppInitializer because this object gets constructed even before MtAppInit.Execute() is called
			eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(
				_ => MtCloudApplicationInitializer.EditorController.ActiveDocumentChanged +=
					(_, _) => EnableAction());

			eventAggregator.GetEvent<TranslationProviderAdded>().Subscribe(_ => EnableAction(new TranslationProviderStatusChanged(null, true)));

			eventAggregator.GetEvent<TranslationProviderStatusChanged>().Subscribe(EnableAction);
		}

		private void EnableAction(TranslationProviderStatusChanged tpStatus = null)
		{
			if (MtCloudApplicationInitializer.GetProjectInProcessing().GetProjectInfo().Id !=
			    MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project.GetProjectInfo().Id) return;

			bool? hasSdlMtAdded;
			if (tpStatus == null)
			{
				hasSdlMtAdded = MtCloudApplicationInitializer.EditorController.ActiveDocument?.Project
					.GetTranslationProviderConfiguration().
					Entries?.Any(
						entry =>
							entry.MainTranslationProvider
								.Uri
								.ToString().Contains("sdlmtcloud"));
			}
			else
			{
				hasSdlMtAdded = tpStatus.NewStatus;
			}

			Enabled = hasSdlMtAdded ?? false;
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
	}
}