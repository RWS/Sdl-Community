using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	internal class MyTranslationProviderLanguageDirection : ITranslationProviderLanguageDirection
	{
		#region ITranslationProviderLanguageDirection Members

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public bool CanReverseLanguageDirection
		{
			get { throw new NotImplementedException(); }
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			throw new NotImplementedException();
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public System.Globalization.CultureInfo SourceLanguage
		{
			get { throw new NotImplementedException(); }
		}

		public System.Globalization.CultureInfo TargetLanguage
		{
			get { throw new NotImplementedException(); }
		}

		public ITranslationProvider TranslationProvider
		{
			get { throw new NotImplementedException(); }
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
