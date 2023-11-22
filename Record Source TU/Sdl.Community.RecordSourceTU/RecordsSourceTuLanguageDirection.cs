using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.RecordSourceTU
{
	public class RecordsSourceTuLanguageDirection : ITranslationMemoryLanguageDirection
    {
		private readonly ITranslationProviderLanguageDirection _fileBasedTranslationProviderLanguageDirection;
        private readonly ITranslationMemoryLanguageDirection _tmLanguageDirection;

		public RecordsSourceTuLanguageDirection(ITranslationProviderLanguageDirection languageDirection, ITranslationMemoryLanguageDirection tmlanguageDirection)
        {
            _fileBasedTranslationProviderLanguageDirection = languageDirection;
            _tmLanguageDirection = tmlanguageDirection;
        }

		public ImportResult[] AddOrUpdateTranslationUnits(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.AddOrUpdateTranslationUnits(
				translationUnits,
                previousTranslationHashes,
				settings);

			ProcessTranslationUnits(translationUnits, results);

            return results;
        }

        public ImportResult[] AddOrUpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, int[] previousTranslationHashes, ImportSettings settings, bool[] mask)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.AddOrUpdateTranslationUnitsMasked(
				translationUnits,
                previousTranslationHashes,
				settings,
				mask);

			ProcessTranslationUnits(translationUnits, results);

            return results;
        }

        public ImportResult AddTranslationUnit(TranslationUnit translationUnit, ImportSettings settings)
        {
            var result = _fileBasedTranslationProviderLanguageDirection.AddTranslationUnit(translationUnit, settings);
            if (result == null) return null;

			var translationUnits = new[] { translationUnit };
			var results = new[] { result };

			ProcessTranslationUnits(translationUnits, results);
			return result;
        }

        public ImportResult[] AddTranslationUnits(TranslationUnit[] translationUnits, ImportSettings settings)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.AddTranslationUnits(
				translationUnits,
                settings);

			ProcessTranslationUnits(translationUnits, results);

            return results;
        }

        public ImportResult[] AddTranslationUnitsMasked(TranslationUnit[] translationUnits, ImportSettings settings, bool[] mask)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.AddTranslationUnitsMasked(
				translationUnits,
                settings,
				mask);

			ProcessTranslationUnits(translationUnits, results);
			
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
            return _fileBasedTranslationProviderLanguageDirection.SearchTranslationUnitsMasked(
				settings,
                translationUnits,
				mask);
        }		
		
		public CultureCode SourceLanguage => _fileBasedTranslationProviderLanguageDirection.SourceLanguage;
		public CultureCode TargetLanguage => _fileBasedTranslationProviderLanguageDirection.TargetLanguage;

		public ITranslationProvider TranslationProvider
        {
            get { return _fileBasedTranslationProviderLanguageDirection.TranslationProvider; }
        }

        public ImportResult UpdateTranslationUnit(TranslationUnit translationUnit)
        {
            var result =  _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnit(translationUnit);
            if (result == null) return null;

			var translationUnits = new[] { translationUnit };
			var results = new[] { result };

			ProcessTranslationUnits(translationUnits, results);

            return result;
        }

        public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
        {
            var results = _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnits(translationUnits);			

			ProcessTranslationUnits(translationUnits, results);

			return results;
        }

        public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
        {
            return _tmLanguageDirection.ApplyFieldsToTranslationUnit(values, overwrite, translationUnitId);
        }

        public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
        {
            return _tmLanguageDirection.ApplyFieldsToTranslationUnits(values, overwrite, translationUnitIds);
        }

        public int DeleteAllTranslationUnits()
        {
           return _tmLanguageDirection.DeleteAllTranslationUnits();
        }

        public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
        {
           return _tmLanguageDirection.DeleteTranslationUnit(translationUnitId);
        }

        public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
        {
           return _tmLanguageDirection.DeleteTranslationUnits(translationUnitIds);
        }

        public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
        {
         return   _tmLanguageDirection.DeleteTranslationUnitsWithIterator(ref iterator);
        }

        public int EditTranslationUnits(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
        {
          return  _tmLanguageDirection.EditTranslationUnits(editScript, updateMode, translationUnitIds);
        }

        public int EditTranslationUnitsWithIterator(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode updateMode, ref RegularIterator iterator)
        {
           return _tmLanguageDirection.EditTranslationUnitsWithIterator(editScript, updateMode, ref iterator);
        }

        public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
        {
          return  _tmLanguageDirection.GetDuplicateTranslationUnits(ref iterator);
        }

        public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
        {
           return _tmLanguageDirection.GetTranslationUnit(translationUnitId);
        }

        public int GetTranslationUnitCount()
        {
         return   _tmLanguageDirection.GetTranslationUnitCount();
        }

        public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
        {
            return _tmLanguageDirection.GetTranslationUnits(ref iterator);
        }

        public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, ref RegularIterator iterator)
        {
            return _tmLanguageDirection.PreviewEditTranslationUnitsWithIterator(editScript, ref iterator);
        }

        public bool ReindexTranslationUnits(ref RegularIterator iterator)
        {
            return _tmLanguageDirection.ReindexTranslationUnits(ref iterator);
        }

        ITranslationMemory ITranslationMemoryLanguageDirection.TranslationProvider
        {
            get { return _tmLanguageDirection.TranslationProvider; }
        }
			
		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
        {
            return _tmLanguageDirection.UpdateTranslationUnitsMasked(translationUnits, mask);
        }

        private void ProcessTranslationUnits(TranslationUnit[] translationUnits, ImportResult[] results)
		{
			var tmDataAccess = TmDataAccess.OpenConnection(TranslationProvider.Uri);

			for (var i = 0; i < results.Length; i++)
			{
				var result = results[i];
				if (result == null) continue;
				var translationUnit = translationUnits[i];

				if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
				{
					var customFieldsValue = new CustomFieldValues
					{
						FileNameFullPath = translationUnit.GetFilePath(),
						ProjectName = translationUnit.GetProjectName()
					};
					tmDataAccess.AddOrUpdateCustomFields(result.TuId.Id, customFieldsValue);
				}
			}
		}

	}
}