using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_TranslationProvider.Search.Result;
using TMX_TranslationProvider.Search.SearchSegment;

namespace TMX_TranslationProvider.Search
{
	// if you have a provider that holds <source, target> pairs, implement this
	internal interface ISegmentPairSearch
	{
		bool SupportsSourceLanguage(CultureInfo language);
		bool SupportsTargetLanguage(CultureInfo language);

		int SegmentPairCount();
		SimpleResult TryTranslateExact(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore);
		SimpleResult TryTranslateFuzzy(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, int minScore);
		SimpleResult TryTranslateConcordance(TextSegment sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, bool sourceConcordance, int minScore);
	}
}
