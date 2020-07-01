﻿using System.Collections.Generic;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;

namespace Sdl.Community.ApplyTMTemplate.Services.Interfaces
{
	public interface IResourceManager
	{
		void ApplyTemplateToTms(ILanguageResourcesContainer languageResourcesContainer, List<TranslationMemory> translationMemories, Settings settings);

		void ExportResourcesToExcel(ILanguageResourcesContainer languageResourcesContainer, string filePathTo,
			Settings settings);

		void ImportResourcesFromExcel(string excelFilePath, ILanguageResourcesContainer languageResourcesContainer, Settings settings);

		void ImportResourcesFromSdltm(List<TranslationMemory> translationMemories, ILanguageResourcesContainer languageResourcesContainer, Settings settings);

		//bool ValidateTemplate(ILanguageResourcesContainer languageResourcesContainer, bool isImport);
	}
}