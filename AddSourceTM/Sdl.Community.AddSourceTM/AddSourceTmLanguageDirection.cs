using System.Runtime.Remoting;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AddSourceTM
{
    public class AddSourceTmLanguageDirection : ITranslationProviderLanguageDirection
    {
        private readonly ITranslationProviderLanguageDirection _fileBasedTranslationProviderLanguageDirection;

        public AddSourceTmLanguageDirection(ITranslationProviderLanguageDirection languageDirection)
        {
            _fileBasedTranslationProviderLanguageDirection = languageDirection;
        }

        #region ITranslationProviderLanguageDirection Members

        public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(translationUnits,
                previousTranslationHashes, settings);
            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (result == null) continue;
                var translationUnit = translationUnits[i];

                if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
                {
                    tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
                }
            }

            return results;
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            var results =
                _fileBasedTranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(translationUnits,
                    previousTranslationHashes, settings, mask);

            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (result == null) continue;
                var translationUnit = translationUnits[i];

                if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
                {
                    tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
                }
            }

            return results;
        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            var result =
                _fileBasedTranslationProviderLanguageDirection.AddTranslationUnit(translationUnit,
                    settings);
            if (result == null) return null;
            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);



            if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
            {
                tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
            }

            return result;
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            var results =
              _fileBasedTranslationProviderLanguageDirection.AddTranslationUnits(translationUnits,
                   settings);

            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (result == null) continue;
                var translationUnit = translationUnits[i];

                if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
                {
                    tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
                }
            }

            return results;
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            var results =
                _fileBasedTranslationProviderLanguageDirection.AddTranslationUnitsMasked(translationUnits,
                    settings, mask);

            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (result == null) continue;
                var translationUnit = translationUnits[i];

                if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
                {
                    tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
                }
            }
            return results;
        }

        public bool CanReverseLanguageDirection
        {
            get { return _fileBasedTranslationProviderLanguageDirection.CanReverseLanguageDirection; }
        }

        public SearchResults SearchSegment(SearchSettings settings, Segment segment)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchSegment(settings, segment);
        }

        public SearchResults[] SearchSegments(SearchSettings settings, Segment[] segments)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchSegments(settings, segments);

        }

        public SearchResults[] SearchSegmentsMasked(SearchSettings settings, Segment[] segments, bool[] mask)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchSegmentsMasked(settings, segments, mask);

        }

        public SearchResults SearchText(SearchSettings settings, string segment)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchText(settings, segment);

        }

        public SearchResults SearchTranslationUnit(SearchSettings settings, TranslationUnit translationUnit)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchTranslationUnit(settings, translationUnit);
        }

        public SearchResults[] SearchTranslationUnits(SearchSettings settings, TranslationUnit[] translationUnits)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchTranslationUnits(settings, translationUnits);
        }

        public SearchResults[] SearchTranslationUnitsMasked(SearchSettings settings, TranslationUnit[] translationUnits, bool[] mask)
        {
            return _fileBasedTranslationProviderLanguageDirection.SearchTranslationUnitsMasked(settings,
                translationUnits, mask);

        }

        public System.Globalization.CultureInfo SourceLanguage
        {
            get { return _fileBasedTranslationProviderLanguageDirection.SourceLanguage; }
        }

        public System.Globalization.CultureInfo TargetLanguage
        {
            get { return _fileBasedTranslationProviderLanguageDirection.TargetLanguage; }
        }

        public ITranslationProvider TranslationProvider
        {
            get { return _fileBasedTranslationProviderLanguageDirection.TranslationProvider; }
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            var result =
                _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnit(translationUnit);
            if (result == null) return null;
            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);



            if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
            {
                tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
            }
            return result;
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            var results =
              _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnits(translationUnits);

            var tmPath = this.TranslationProvider.Uri.AbsolutePath;
            var tmDataAccess = TmDataAccess.OpenConnection(tmPath);

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (result == null) continue;
                var translationUnit = translationUnits[i];

                if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
                {
                    tmDataAccess.AddOrUpdateSourceFile(result.TuId.Id, translationUnit.DocumentProperties.LastOpenedAsPath);
                }
            }
            return results;
        }

        #endregion
    }
}
