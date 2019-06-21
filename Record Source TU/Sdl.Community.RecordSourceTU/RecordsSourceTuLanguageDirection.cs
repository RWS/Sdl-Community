using System.IO;
using System.Linq;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.RecordSourceTU
{
	public class RecordsSourceTuLanguageDirection : ITranslationMemoryLanguageDirection
	{
		#region Private fields
		private readonly ITranslationProviderLanguageDirection _fileBasedTranslationProviderLanguageDirection;
		private readonly ITranslationMemoryLanguageDirection _tmlanguageDirection;
		private EditorController _editorController = null;
		private string _activeFileName = string.Empty;
		#endregion

		#region Constructors
		public RecordsSourceTuLanguageDirection(ITranslationProviderLanguageDirection languageDirection, ITranslationMemoryLanguageDirection tmlanguageDirection)
		{
			_fileBasedTranslationProviderLanguageDirection = languageDirection;
			_tmlanguageDirection = tmlanguageDirection;
			_editorController = GetEditorController();
			_editorController.Opened += _editorController_Opened;

		}
		#endregion

		#region Events
		private void _editorController_Opened(object sender, DocumentEventArgs e)
		{
			var activeDoc = _editorController.ActiveDocument;
			if (activeDoc != null)
			{
				activeDoc.ActiveSegmentChanged += ActiveDoc_ActiveSegmentChanged;
			}
		}

		private void ActiveDoc_ActiveSegmentChanged(object sender, System.EventArgs e)
		{
			var document = sender as Document;

			// used to get the info for each active file when files are merged during project creation
			if (document?.Files.Count() == 1)
			{
				_activeFileName = Path.GetFileName(document?.ActiveFileProperties?.FileConversionProperties?.OriginalFilePath);
			}
			else
			{
				// used to get the info for each active file when files are merged after creating project 
				_activeFileName = Path.GetFileName(document?.ActiveFile?.LocalFilePath);
			}

		}
		#endregion

		#region ITranslationProviderLanguageDirection Members
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

			var translationUnits = new TranslationUnit[] { translationUnit };
			var results = new ImportResult[] { result };

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
			var result = _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnit(translationUnit);
			if (result == null) return null;

			var translationUnits = new TranslationUnit[] { translationUnit };
			var results = new ImportResult[] { result };

			ProcessTranslationUnits(translationUnits, results);

			return result;
		}

		public ImportResult[] UpdateTranslationUnits(TranslationUnit[] translationUnits)
		{
			var results = _fileBasedTranslationProviderLanguageDirection.UpdateTranslationUnits(translationUnits);

			ProcessTranslationUnits(translationUnits, results);

			return results;
		}

		#endregion

		public bool ApplyFieldsToTranslationUnit(FieldValues values, bool overwrite, PersistentObjectToken translationUnitId)
		{
			return _tmlanguageDirection.ApplyFieldsToTranslationUnit(values, overwrite, translationUnitId);
		}

		public int ApplyFieldsToTranslationUnits(FieldValues values, bool overwrite, PersistentObjectToken[] translationUnitIds)
		{
			return _tmlanguageDirection.ApplyFieldsToTranslationUnits(values, overwrite, translationUnitIds);
		}

		public int DeleteAllTranslationUnits()
		{
			return _tmlanguageDirection.DeleteAllTranslationUnits();
		}

		public bool DeleteTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return _tmlanguageDirection.DeleteTranslationUnit(translationUnitId);
		}

		public int DeleteTranslationUnits(PersistentObjectToken[] translationUnitIds)
		{
			return _tmlanguageDirection.DeleteTranslationUnits(translationUnitIds);
		}

		public int DeleteTranslationUnitsWithIterator(ref RegularIterator iterator)
		{
			return _tmlanguageDirection.DeleteTranslationUnitsWithIterator(ref iterator);
		}

		public int EditTranslationUnits(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode updateMode, PersistentObjectToken[] translationUnitIds)
		{
			return _tmlanguageDirection.EditTranslationUnits(editScript, updateMode, translationUnitIds);
		}

		public int EditTranslationUnitsWithIterator(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, LanguagePlatform.TranslationMemory.EditScripts.EditUpdateMode updateMode, ref RegularIterator iterator)
		{
			return _tmlanguageDirection.EditTranslationUnitsWithIterator(editScript, updateMode, ref iterator);
		}

		public TranslationUnit[] GetDuplicateTranslationUnits(ref DuplicateIterator iterator)
		{
			return _tmlanguageDirection.GetDuplicateTranslationUnits(ref iterator);
		}

		public TranslationUnit GetTranslationUnit(PersistentObjectToken translationUnitId)
		{
			return _tmlanguageDirection.GetTranslationUnit(translationUnitId);
		}

		public int GetTranslationUnitCount()
		{
			return _tmlanguageDirection.GetTranslationUnitCount();
		}

		public TranslationUnit[] GetTranslationUnits(ref RegularIterator iterator)
		{
			return _tmlanguageDirection.GetTranslationUnits(ref iterator);
		}

		public TranslationUnit[] PreviewEditTranslationUnitsWithIterator(LanguagePlatform.TranslationMemory.EditScripts.EditScript editScript, ref RegularIterator iterator)
		{
			return _tmlanguageDirection.PreviewEditTranslationUnitsWithIterator(editScript, ref iterator);
		}

		public bool ReindexTranslationUnits(ref RegularIterator iterator)
		{
			return _tmlanguageDirection.ReindexTranslationUnits(ref iterator);
		}

		ITranslationMemory ITranslationMemoryLanguageDirection.TranslationProvider
		{
			get { return _tmlanguageDirection.TranslationProvider; }
		}

		public ImportResult[] UpdateTranslationUnitsMasked(TranslationUnit[] translationUnits, bool[] mask)
		{
			return _tmlanguageDirection.UpdateTranslationUnitsMasked(translationUnits, mask);
		}

		#region Private Methods
		private EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		private void ProcessTranslationUnits(TranslationUnit[] translationUnits, ImportResult[] results)
		{
			var tmDataAccess = TmDataAccess.OpenConnection(TranslationProvider.Uri);

			for (int i = 0; i < results.Length; i++)
			{
				var result = results[i];
				if (result == null) continue;
				var translationUnit = translationUnits[i];

				if (result.Action == Action.Add || result.Action == Action.Merge || result.Action == Action.Overwrite)
				{
					var customFieldsValue = new CustomFieldValues
					{
						FileNameFullPath = _activeFileName,
						ProjectName = translationUnit.GetProjectName()
					};
					tmDataAccess.AddOrUpdateCustomFields(result.TuId.Id, customFieldsValue);
				}
			}
		}
		#endregion
	}
}