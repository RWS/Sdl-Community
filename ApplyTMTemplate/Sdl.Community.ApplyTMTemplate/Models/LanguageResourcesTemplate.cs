using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class LanguageResourcesTemplate : FileBasedLanguageResourcesTemplate, ILanguageResourcesContainer
	{
		public LanguageResourcesTemplate(string filePath) : base(filePath) {}

		public new BuiltinRecognizers Recognizers
		{
			get => base.Recognizers ?? BuiltinRecognizers.RecognizeAll;
			set => base.Recognizers = value;
		}

		public new WordCountFlags WordCountFlags
		{
			get => base.WordCountFlags ?? WordCountFlags.DefaultFlags;
			set => base.WordCountFlags = value;
		}
	}
}