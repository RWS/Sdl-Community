using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.Community.ApplyTMTemplate.Models.Wrappers;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TranslationMemoryContainer = Sdl.Community.ApplyTMTemplate.Models.Wrappers.TranslationMemoryContainer;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class LanguageResourcesAdapter : ILanguageResourcesAdapter
	{
		private ILanguageResourcesContainer _languageResourceContainer;

		public void Load(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
			{
				throw new ArgumentException();
			}

			try
			{
				_languageResourceContainer = new LanguageResourcesTemplateContainer(filePath);
			}
			catch (FileNotFoundException)
			{
				_languageResourceContainer = new TranslationMemoryContainer(filePath);
			}
		}

		public List<LanguageResourceBundle> LanguageResourceBundles
		{
			get => _languageResourceContainer.LanguageResourceBundles.ToList();
			set
			{
				foreach (var bundle in value)
				{
					_languageResourceContainer.LanguageResourceBundles.Clear();
					_languageResourceContainer.LanguageResourceBundles.Insert(0, bundle);
				}
			}
		}

		public BuiltinRecognizers? Recognizers
		{
			get => _languageResourceContainer.Recognizers;
			set => _languageResourceContainer.Recognizers = value;
		}

		public WordCountFlags? WordCountFlags
		{
			get => _languageResourceContainer.WordCountFlags;
			set => _languageResourceContainer.WordCountFlags = value;
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