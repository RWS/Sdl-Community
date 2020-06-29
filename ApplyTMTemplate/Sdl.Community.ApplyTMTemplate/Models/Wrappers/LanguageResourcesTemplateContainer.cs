using System;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models.Wrappers
{
	public class LanguageResourcesTemplateContainer : ILanguageResourcesContainer
	{
		private readonly FileBasedLanguageResourcesTemplate _languageResourcesTemplate;

		public LanguageResourcesTemplateContainer(string filePath)
		{
			_languageResourcesTemplate = new FileBasedLanguageResourcesTemplate(filePath);
		}

		public LanguageResourceBundleCollection LanguageResourceBundles => _languageResourcesTemplate.LanguageResourceBundles;

		public BuiltinRecognizers? Recognizers
		{
			get => _languageResourcesTemplate.Recognizers;
			set
			{
				_languageResourcesTemplate.Recognizers = value;
			}

		}
		public WordCountFlags? WordCountFlags
		{
			get => _languageResourcesTemplate.WordCountFlags;
			set
			{
				_languageResourcesTemplate.WordCountFlags = value;
			}
		}

		public void Save()
		{
			_languageResourcesTemplate.Save();
		}

		public bool ValidateTemplate(bool isImport)
		{
			return isImport ? LanguageResourceBundles != null : LanguageResourceBundles?.Count != 0;
		}
	}
}