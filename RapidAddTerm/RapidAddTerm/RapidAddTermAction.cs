using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.RapidAddTerm
{
	[Action("Rapid Add Term", Name = "Rapid Add Term", Icon = "RAT1")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
	public class RapidAddTermAction : AbstractAction
	{
		private readonly TermbaseService _termbaseService = new TermbaseService();
		protected override void Execute()
		{
			_termbaseService.AddNewTerm();
		}
	}
}
