using System.Collections.Generic;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;

namespace Sdl.Community.ApplyTMTemplate.Services.Interfaces
{
	public interface IResourceManager
	{
		void ApplyTemplateToTms(ILanguageResourcesAdapter languageResourcesContainer, List<TranslationMemory> translationMemories, Settings settings);

		void ExportResourcesToExcel(ILanguageResourcesAdapter languageResourcesContainer, string filePathTo,
			Settings settings);

		void ImportResourcesFromExcel(string excelFilePath, ILanguageResourcesAdapter languageResourcesContainer, Settings settings);

		void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories, ILanguageResourcesAdapter languageResourcesContainer, Settings settings);

		bool ValidateTemplate(ILanguageResourcesAdapter languageResourcesContainer, bool isImport);
	}
}