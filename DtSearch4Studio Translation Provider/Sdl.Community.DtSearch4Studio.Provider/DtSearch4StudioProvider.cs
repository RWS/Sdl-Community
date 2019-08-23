using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DtSearch4Studio.Provider
{
	class DtSearch4StudioProvider : ITranslationProvider
	{
		#region ITranslationProvider Members

		// To be implemented all the methods /properties bellow. 
		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return null;
		}

		public bool IsReadOnly
		{
			get => true;
		}

		public void LoadState(string translationProviderState)
		{
		}

		public string Name
		{
			get => string.Empty;
		}

		public void RefreshStatusInfo()
		{
			
		}

		public string SerializeState()
		{
			throw new NotImplementedException();
		}

		public ProviderStatusInfo StatusInfo
		{
			get => null;
		}

		public bool SupportsConcordanceSearch
		{
			get => true;
		}

		public bool SupportsDocumentSearches
		{
			get => true;
		}

		public bool SupportsFilters
		{
			get => true;
		}

		public bool SupportsFuzzySearch
		{
			get => true;
		}

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			return true;
		}

		public bool SupportsMultipleResults
		{
			get => true;
		}

		public bool SupportsPenalties
		{
			get => true;
		}

		public bool SupportsPlaceables
		{
			get => true;
		}

		public bool SupportsScoring
		{
			get => true;
		}

		public bool SupportsSearchForTranslationUnits
		{
			get => true;
		}

		public bool SupportsSourceConcordanceSearch
		{
			get => true;
		}

		public bool SupportsStructureContext
		{
			get => true;
		}

		public bool SupportsTaggedInput
		{
			get => true;
		}

		public bool SupportsTargetConcordanceSearch
		{
			get => true;
		}

		public bool SupportsTranslation
		{
			get => true;
		}

		public bool SupportsUpdate
		{
			get => true;
		}

		public bool SupportsWordCounts
		{
			get => true;
		}

		public TranslationMethod TranslationMethod
		{
			get => TranslationMethod.Mixed;
		}

		public Uri Uri
		{
			get => new Uri(string.Empty);
		}

		#endregion
	}
}

