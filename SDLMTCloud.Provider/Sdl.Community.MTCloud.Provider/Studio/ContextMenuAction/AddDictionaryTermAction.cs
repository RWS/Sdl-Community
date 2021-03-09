using System;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.MTCloud.Provider.Studio.ContextMenuAction
{
	[Action("Add Dictionary Term",
		Name = "Add Dictionary Term",
		Icon = "add_dictionary",
		Description = "Add a term to the current dictionary")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AddDictionaryTermAction : AbstractAction
	{
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