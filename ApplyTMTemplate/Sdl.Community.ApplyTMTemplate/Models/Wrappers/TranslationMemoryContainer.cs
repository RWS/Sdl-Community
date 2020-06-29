using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models.Wrappers
{
	public class TranslationMemoryContainer : ILanguageResourcesContainer
	{
		private readonly FileBasedTranslationMemory _translationMemory;

		public TranslationMemoryContainer(string filePath)
		{
			_translationMemory = new FileBasedTranslationMemory(filePath);
		}

		public LanguageResourceBundleCollection LanguageResourceBundles => _translationMemory.LanguageResourceBundles;

		public BuiltinRecognizers? Recognizers
		{
			get => _translationMemory.Recognizers;
			set
			{
				if (value != null)
				{
					_translationMemory.Recognizers = (BuiltinRecognizers)value;
				}
			}
		}

		public WordCountFlags? WordCountFlags
		{
			get => _translationMemory.WordCountFlags;
			set
			{
				if (value != null)
				{
					_translationMemory.WordCountFlags = (WordCountFlags)value;
				}
			}
		}

		public void Save()
		{
			_translationMemory.Save();
		}

		public bool ValidateTemplate(bool isImport)
		{
			return isImport ? LanguageResourceBundles != null : LanguageResourceBundles?.Count != 0;
		}
	}
}