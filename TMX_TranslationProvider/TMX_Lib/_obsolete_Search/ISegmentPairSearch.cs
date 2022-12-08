﻿using System.Globalization;
using TMX_Lib._obsolete_Search.Result;
using TMX_Lib._obsolete_Search.SearchSegment;

namespace TMX_Lib._obsolete_Search
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