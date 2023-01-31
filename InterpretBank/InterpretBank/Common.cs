using System.Collections.Generic;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SQLBuilder;
using InterpretBank.Studio;
using InterpretBank.TermSearch;
using NSubstitute;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace InterpretBank
{
	public static class Common
	{
		private static ProjectsController _projectsController;

		public static ProjectsController ProjectsController =>
			_projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static InterpretBankProvider GetInterpretBankProvider()
		{
			//var sqlGlossaryService = new SqlGlossaryService(new DatabaseConnection("file"), new SqlBuilder());
			//var settingsService = new SettingsService();

			//var termSearchService = new TermSearchService(sqlGlossaryService, settingsService);
			var termSearchService = Substitute.For<ITermSearchService>();
			termSearchService
				.GetFuzzyTerms(default, default, default)
				.ReturnsForAnyArgs(new List<string> { "firstTerm", "secondTerm" });

			return new InterpretBankProvider(termSearchService);
		}
	}
}