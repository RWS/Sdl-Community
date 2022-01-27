using System;
using InterpretBank.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace InterpretBank
{
	[Action("Test",
		Name = "Test",
		Description = "Test")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class TestAction : AbstractAction
	{
		//TODO: Delete this at dev end if not needed; was created for test purposes only
		protected override void Execute()
		{
			var x = new GlossaryService(@"C:\Users\ealbu\Desktop\Interpret Bank\test.db");
		}
	}
}