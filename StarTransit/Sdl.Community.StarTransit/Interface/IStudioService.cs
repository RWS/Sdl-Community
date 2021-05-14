using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Interface
{
	public interface IStudioService
	{
		List<ProjectTemplateInfo> GetProjectTemplates();
		Task<List<Customer>> GetCustomers();

		PackageModel GetModelBasedOnStudioTemplate(string templatePath, CultureInfo sourceCultureInfo,
			Language[] targetLanguages);

		(bool, Language) IsTmCreatedFromPlugin(string tmName, CultureInfo sourceCultureInfo, Language[] targetLanguages);
		bool GetTranslationMemoryLanguage(string tmUri);
	}
}
