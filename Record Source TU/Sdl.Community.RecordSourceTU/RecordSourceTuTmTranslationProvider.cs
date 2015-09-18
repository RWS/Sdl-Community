using System;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.RecordSourceTU
{
    public class RecordSourceTuTmTranslationProvider : ITranslationMemory
    {
        internal const string ProviderUriScheme = "addSourceTm.";
        

        
        public ITranslationProvider FileBasedTranslationProvider
        {
            get { return _fileBasedTranslationProvider; }
        }

        public FileBasedTranslationMemory FileBasedTranslationMemory
        {
            get { return (FileBasedTranslationMemory)FileBasedTranslationProvider; }
        }

        private readonly ITranslationProvider _fileBasedTranslationProvider;
        public RecordSourceTuTmTranslationProvider(ITranslationProvider provider)
        {
            _fileBasedTranslationProvider = provider;
        }

        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languagePair)
        {
            var languageDirection = _fileBasedTranslationProvider.GetLanguageDirection(languagePair);
            return new RecordsSourceTuLanguageDirection(languageDirection, FileBasedTranslationMemory.LanguageDirection);
        }

        public bool IsReadOnly
        {
            get { return _fileBasedTranslationProvider.IsReadOnly; }
        }

        public void LoadState(string translationProviderState)
        {
            _fileBasedTranslationProvider.LoadState(translationProviderState);
        }

        public string Name
        {
            get { return "Record Source TU: " + _fileBasedTranslationProvider.Name; }
        }

        public void RefreshStatusInfo()
        {
            _fileBasedTranslationProvider.RefreshStatusInfo();
        }

        public string SerializeState()
        {
            return _fileBasedTranslationProvider.SerializeState();
        }

        public ProviderStatusInfo StatusInfo
        {
            get { return _fileBasedTranslationProvider.StatusInfo; }
        }

        public bool SupportsConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsConcordanceSearch; }
        }

        public bool SupportsDocumentSearches
        {
            get { return _fileBasedTranslationProvider.SupportsDocumentSearches; }
        }

        public bool SupportsFilters
        {
            get { return _fileBasedTranslationProvider.SupportsFilters; }
        }

        public bool SupportsFuzzySearch
        {
            get { return _fileBasedTranslationProvider.SupportsFuzzySearch; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
            return _fileBasedTranslationProvider.SupportsLanguageDirection(languageDirection);
        }

        public bool SupportsMultipleResults
        {
            get { return _fileBasedTranslationProvider.SupportsMultipleResults; }
        }

        public bool SupportsPenalties
        {
            get { return _fileBasedTranslationProvider.SupportsPenalties; }
        }

        public bool SupportsPlaceables
        {
            get { return _fileBasedTranslationProvider.SupportsPlaceables; }
        }

        public bool SupportsScoring
        {
            get { return _fileBasedTranslationProvider.SupportsScoring; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return _fileBasedTranslationProvider.SupportsSearchForTranslationUnits; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsSourceConcordanceSearch; }
        }

        public bool SupportsStructureContext
        {
            get { return _fileBasedTranslationProvider.SupportsStructureContext; }
        }

        public bool SupportsTaggedInput
        {
            get { return _fileBasedTranslationProvider.SupportsTaggedInput; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return _fileBasedTranslationProvider.SupportsTargetConcordanceSearch; }
        }

        public bool SupportsTranslation
        {
            get { return _fileBasedTranslationProvider.SupportsTranslation; }
        }

        public bool SupportsUpdate
        {
            get { return _fileBasedTranslationProvider.SupportsUpdate; }
        }

        public bool SupportsWordCounts
        {
            get { return _fileBasedTranslationProvider.SupportsWordCounts; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return _fileBasedTranslationProvider.TranslationMethod; }
        }

        public Uri Uri
        {
            get
            {
                return new Uri(ProviderUriScheme + FileBasedTranslationProvider.Uri.AbsoluteUri);
            }
        }

        #endregion

        #region ITranslationMemory

        public string Copyright
        {
            get { return FileBasedTranslationMemory.Copyright; }
            set { FileBasedTranslationMemory.Copyright = value; }
        }

        public DateTime CreationDate
        {
            get { return FileBasedTranslationMemory.CreationDate; }
        }

        public string CreationUserName
        {
            get { return FileBasedTranslationMemory.CreationUserName; }
        }

        public void Delete()
        {
            FileBasedTranslationMemory.Delete();
        }

        public string Description
        {
            get { return FileBasedTranslationMemory.Description; }
            set { FileBasedTranslationMemory.Description = value; }
        }

        public DateTime? ExpirationDate
        {
            get { return FileBasedTranslationMemory.ExpirationDate; }
            set { FileBasedTranslationMemory.ExpirationDate = value; }
        }

        public FieldDefinitionCollection FieldDefinitions
        {
            get { return FileBasedTranslationMemory.FieldDefinitions; }
        }

        public DateTime? FuzzyIndexStatisticsRecomputedAt
        {
            get { return FileBasedTranslationMemory.FuzzyIndexStatisticsRecomputedAt; }
        }

        public int? FuzzyIndexStatisticsSize
        {
            get { return FileBasedTranslationMemory.FuzzyIndexStatisticsSize; }
        }

        public LanguagePlatform.TranslationMemory.FuzzyIndexTuningSettings FuzzyIndexTuningSettings
        {
            get { return FileBasedTranslationMemory.FuzzyIndexTuningSettings; }
            set { FileBasedTranslationMemory.FuzzyIndexTuningSettings = value; }
        }

        public LanguagePlatform.TranslationMemory.FuzzyIndexes FuzzyIndexes
        {
            get { return FileBasedTranslationMemory.FuzzyIndexes; }
            set { FileBasedTranslationMemory.FuzzyIndexes = value; }
        }

        ITranslationMemoryLanguageDirection ITranslationMemory.GetLanguageDirection(LanguagePair languageDirection)
        {
            return FileBasedTranslationMemory.GetLanguageDirection(languageDirection);
        }

        public int GetTranslationUnitCount()
        {
            return FileBasedTranslationMemory.GetTranslationUnitCount();
        }

        public bool HasPermission(string permission)
        {
            return FileBasedTranslationMemory.HasPermission(permission);
        }

        public LanguageResourceBundleCollection LanguageResourceBundles
        {
            get {  return FileBasedTranslationMemory.LanguageResourceBundles; }
        }

        public LanguagePlatform.Core.Tokenization.BuiltinRecognizers Recognizers
        {
            get { return FileBasedTranslationMemory.Recognizers; }
            set { FileBasedTranslationMemory.Recognizers = value; }
        }

        public void RecomputeFuzzyIndexStatistics()
        {
            FileBasedTranslationMemory.RecomputeFuzzyIndexStatistics();
        }

        public void Save()
        {
             FileBasedTranslationMemory.Save();
        }

        public bool ShouldRecomputeFuzzyIndexStatistics()
        {
            return FileBasedTranslationMemory.ShouldRecomputeFuzzyIndexStatistics();
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<LanguagePair> SupportedLanguageDirections
        {
            get { return FileBasedTranslationMemory.SupportedLanguageDirections; }
        }

        #endregion
    }
}

