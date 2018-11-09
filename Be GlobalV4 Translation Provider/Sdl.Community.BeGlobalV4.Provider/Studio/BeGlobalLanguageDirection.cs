using System;
using System.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalLanguageDirection	: ITranslationProviderLanguageDirection
	{
		private readonly BeGlobalTranslationProvider _beGlobalTranslationProvider;
		private readonly BeGlobalTranslationOptions _options;
		private readonly LanguagePair _languageDirection;

		public ITranslationProvider TranslationProvider => _beGlobalTranslationProvider;
		public CultureInfo SourceLanguage { get; }
		public CultureInfo TargetLanguage { get; }
		public bool CanReverseLanguageDirection { get; }

		public BeGlobalLanguageDirection(BeGlobalTranslationProvider beGlobalTranslationProvider,LanguagePair languageDirection)
		{
			_beGlobalTranslationProvider = beGlobalTranslationProvider;
			_languageDirection = languageDirection;
			_options = beGlobalTranslationProvider.Options;
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

		public SearchResults SearchTranslationUnit(SearchSettings settings,
			TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnits(SearchSettings settings,
			TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

		public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings,
			TranslationUnit[] translationUnits, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult AddTranslationUnit(TranslationUnit translationUnit,
			ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits,
			ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
		{
			throw new NotImplementedException();
		}

		public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
		{
			throw new NotImplementedException();
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			throw new NotImplementedException();
		}

	}
}
