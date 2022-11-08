using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_TranslationProvider.Search.Result;
using TMX_TranslationProvider.TmxFormat;

namespace TMX_TranslationProvider.Search
{
	internal class TmxSearch : ISegmentPairSearch
	{
		private TmxParser _parser;

		public TmxSearch(TmxParser parser)
		{
			_parser = parser;
		}

		public bool SupportsSourceLanguage(CultureInfo language)
		{
			return false;
		}

		public bool SupportsTargetLanguage(CultureInfo language)
		{
			return false;
		}

		public int SegmentPairCount()
		{
			return 0;
		}

		public SimpleResult TryTranslate(string sourceText, int segmentPairIndex, CultureInfo sourceLanguage, CultureInfo targetLaguage, SearchMode searchMode)
		{
			return null;
		}
	}
}
