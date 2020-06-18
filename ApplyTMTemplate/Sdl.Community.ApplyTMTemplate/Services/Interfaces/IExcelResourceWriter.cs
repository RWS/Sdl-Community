using System.Collections.Generic;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Services.Interfaces
{
	public interface IExcelResourceWriter
	{
		void ExportResourcesToExcel(FileBasedLanguageResourcesTemplate template, string filePathTo, Settings settings);
		List<LanguageResourceBundle> GetResourceBundlesFromExcel(string filePathFrom);
	}
}