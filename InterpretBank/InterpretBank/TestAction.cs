using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using InterpretBank.Service;
using InterpretBank.Service.Interface;
using InterpretBank.Service.Model;
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
			var filePath = @"..\..\..\InterpretBankTests\Resources\InterpretBankDatabaseV6.db";
			IGlossaryService x = new GlossaryService(new DatabaseConnection(filePath), new SqlBuilder(), new ConditionBuilder());

			var all = x.GetTerms();
		}
	}
}