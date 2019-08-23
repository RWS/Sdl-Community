using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider.Studio
{
	class DtSearch4StudioLanguageDirection : ITranslationProviderLanguageDirection
	{
		public DtSearch4StudioLanguageDirection(DtSearch4StudioProvider dtSearch4StudioProvider, LanguagePair languageDirection)
		{

		}
		#region ITranslationProviderLanguageDirection Members
		// To be implemented all the methods /properties bellow
		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			return null;
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
		{
			return null;
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
		{
			return null;
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			return null;
		}

		public bool CanReverseLanguageDirection
		{
			get => false;
		}

		public SearchResults SearchSegment(SearchSettings settings, Segment segment)
		{
			return null;
		}

		public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
		{
			return null;
		}

		public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
		{
			return null;
		}

		public SearchResults SearchText(SearchSettings settings, string segment)
		{
			return null;
		}

		public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
		{
			return null;
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
		{
			return null;
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
		{
			return null;
		}

		public System.Globalization.CultureInfo SourceLanguage
		{
			get => new System.Globalization.CultureInfo(string.Empty);
		}

		public System.Globalization.CultureInfo TargetLanguage
		{
			get => new System.Globalization.CultureInfo(string.Empty);
		}

		public ITranslationProvider TranslationProvider
		{
			get => null;
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			return null;
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			return null;
		}
		#endregion
	}
}
