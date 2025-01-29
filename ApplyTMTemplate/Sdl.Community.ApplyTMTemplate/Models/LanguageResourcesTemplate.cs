using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class LanguageResourcesTemplate : FileBasedLanguageResourcesTemplate, ILanguageResourcesContainer
	{
		public LanguageResourcesTemplate(string filePath) : base(filePath)
		{
			var bundles = this.LanguageResourceBundles;

			// Method used to make sure the bundle properties are populated
			// Might remove it when initializing the TM works
			foreach (var bundle in bundles)
			{
				_ = bundle.Abbreviations;
				_ = bundle.CurrencyFormats;
				_ = bundle.LongDateFormats;
				_ = bundle.LongTimeFormats;
				_ = bundle.MeasurementUnits;
				_ = bundle.NumbersSeparators;
				_ = bundle.OrdinalFollowers;
				_ = bundle.SegmentationRules;
				_ = bundle.ShortDateFormats;
				_ = bundle.ShortTimeFormats;
				_ = bundle.Variables;
			}
		}

		public new BuiltinRecognizers Recognizers
		{
			get => base.Recognizers;
			set => base.Recognizers = value;
		}

		public new WordCountFlags WordCountFlags
		{
			get => base.WordCountFlags ?? WordCountFlags.DefaultFlags;
			set => base.WordCountFlags = value;
		}
	}
}