using System;
using LanguageWeaverProvider.View;
using LanguageWeaverProvider.ViewModel;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace LanguageWeaverProvider.Studio.Actions
{
	[Action("Create new dictionary term",
	Name = "Create new dictionary term",
	Icon = "lwLogo_Main_Icon",
	Description = "Create new dictionary term")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class CreateDictionaryTerm : AbstractAction
	{
		protected override void Execute()
		{
			var viewModel = new CreateDictionaryTermViewModel();
			var view = new CreateDictionaryTermView() { DataContext = viewModel };
			viewModel.CloseEventRaised += view.Close;
			view.ShowDialog();
		}
	}
}