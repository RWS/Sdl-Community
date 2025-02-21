using System;
using System.IO;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class LanguageResourcesAdapter : ILanguageResourcesAdapter
	{
		private ILanguageResourcesContainer _languageResourceContainer;

		public LanguageResourceBundleCollection LanguageResourceBundles => _languageResourceContainer.LanguageResourceBundles;

		public BuiltinRecognizers Recognizers
		{
			get => _languageResourceContainer.Recognizers;
			set => _languageResourceContainer.Recognizers = value;
		}

		public WordCountFlags WordCountFlags
		{
			get => _languageResourceContainer.WordCountFlags;
			set => _languageResourceContainer.WordCountFlags = value;
		}

		public void Load(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException();
			}

			try
			{
				_languageResourceContainer = new LanguageResourcesTemplate(filePath);
			}
			catch (FileNotFoundException)
			{
				_languageResourceContainer = new TranslationMemory(filePath);
			}
		}

		public void Save()
		{
			_languageResourceContainer.Save();
		}

		public bool ValidateTemplate(bool isImport)
		{
			return isImport ? LanguageResourceBundles != null : LanguageResourceBundles?.Count != 0;
		}
	}
}