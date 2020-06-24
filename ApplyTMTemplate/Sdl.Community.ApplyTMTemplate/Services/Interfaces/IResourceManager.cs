using System.Collections.Generic;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services.Interfaces
{
	public interface IExcelResourceManager
	{
		void ExportResourcesToExcel(ILanguageResourcesContainer template, string filePathTo, Settings settings);
		List<LanguageResourceBundle> GetResourceBundlesFromExcel(string filePathFrom, Settings settings);
	}
}