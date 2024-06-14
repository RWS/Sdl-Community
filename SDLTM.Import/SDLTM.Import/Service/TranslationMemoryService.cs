using System;
using System.IO;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;
using Action = Sdl.LanguagePlatform.TranslationMemory.Action;

namespace SDLTM.Import.Service
{
	public class TranslationMemoryService:ITranslationMemoryService
	{
		private readonly ImportSummary _importSummary = new ImportSummary();

		private readonly string _backupDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore","SDLTMImportBackup");
		public Settings Settings { get; set; }

		/// <summary>
		/// Import translation unit into tm
		/// </summary>
		/// <param name="tm">Translation Memory where the TUs are imported</param>
		/// <param name="selectedTmDetails">TM Details where we set the import summary</param>
		/// <param name="transaltioTranslationUnits">Array of ContextMatch translation unit and Current Translation unit</param>
		/// <param name="masks">First position has "false" value for context match second position "true" for current translation unit </param>
		public void ImportTranslationUnits(FileBasedTranslationMemory tm, TmDetails selectedTmDetails, TranslationUnit[] transaltioTranslationUnits, bool[] masks)
		{
			var importResult = tm.LanguageDirection.AddTranslationUnitsMasked(transaltioTranslationUnits, GetDefaultImportSettings(), masks);

			if (importResult == null) return;
			var importAction = importResult.Length == 1 ? importResult[0].Action : importResult[1].Action;
			switch (importAction)
			{
				case Action.Add:
				case Action.Merge:
					selectedTmDetails.ImportSummary.AddedTusCount++;
					break;
				case Action.Error:
					selectedTmDetails.ImportSummary.ErrorCount++;
					break;
			}
		}	

		/// <summary>
		/// Imports xliff/tmx file intro TM
		/// </summary>
		public ImportSummary ImportFile(FileBasedTranslationMemory tm, string filePath)
		{
			var tmImporter = new TranslationMemoryImporter(tm.LanguageDirection)
			{
				ImportSettings = GetImportSettings(),
				ChunkSize = 100
			};
			tmImporter.BatchImported += TmImporter_BatchImported;

			tmImporter.Import(filePath);
			return _importSummary;
		}

		public void SetFieldsToTu(FileBasedTranslationMemory tm, TmDetails selectedTmDetails, TranslationUnit tu, string tuNumber,
			string currentFileName)
		{
			var fieldName = selectedTmDetails.GetFileNameField();
			var segmentField = selectedTmDetails.GetSegmentFieldName();

			//user selected a custom field to stamp it with file name
			if (!string.IsNullOrEmpty(fieldName))
			{
				CreateCustomField(tm, fieldName);
				SetCustomFieldValue(tu, fieldName, currentFileName);
			}
			if (string.IsNullOrEmpty(segmentField)) return;
			//Segment id 
			CreateCustomField(tm, segmentField);
			SetCustomFieldValue(tu, segmentField, tuNumber);
		}

		public void BackUpTm(string tmPath)
		{
			if (!Directory.Exists(_backupDirectoryPath))
			{
				Directory.CreateDirectory(_backupDirectoryPath);
			}
			var tmName = Path.GetFileName(tmPath);
			if (!string.IsNullOrEmpty(tmName))
			{
				var backupFilePath = Path.Combine(_backupDirectoryPath, tmName);
				File.Copy(tmPath, backupFilePath, true);
			}
		}

		/// <summary>
		/// Set value for a selected cutom field
		/// </summary>
		private void SetCustomFieldValue(TranslationUnit tu, string fieldName,string fieldValue)
		{
			var fieldDefinition = new FieldDefinition(fieldName, FieldValueType.MultipleString);
			var segmentValue = fieldDefinition.CreateValue();	
			segmentValue.Add(fieldValue);
			tu.FieldValues.Add(segmentValue);
		}

		/// <summary>
		/// Creates custom fileds in TM if does not exists
		/// </summary>
		private void CreateCustomField(FileBasedTranslationMemory tm,string customFieldName)
		{
			var fields = tm.FieldDefinitions;
			var customField = fields.FirstOrDefault(f => f.Name.Equals(customFieldName));
			if (customField != null) return;
			var segmentIdFieldDefinition = new FieldDefinition(customFieldName, FieldValueType.MultipleString);
			tm.FieldDefinitions.Add(segmentIdFieldDefinition);
			tm.Save();
		}

		private void TmImporter_BatchImported(object sender, BatchImportedEventArgs e)
		{
			var importStatistics = e.Statistics;
			if (importStatistics is null) return;

			_importSummary.AddedTusCount = importStatistics.AddedTranslationUnits;
			_importSummary.ErrorCount = importStatistics.Errors;
			_importSummary.ReadTusCount = importStatistics.TotalRead;
		}

		private ImportSettings GetImportSettings()
		{
			ConfirmationLevel[] levels = { ConfirmationLevel.ApprovedTranslation, ConfirmationLevel.Translated, ConfirmationLevel.ApprovedSignOff };
			var defaultSettings = GetDefaultImportSettings();
			defaultSettings.ConfirmationLevels = levels;
			return defaultSettings;
		}

		private ImportSettings GetDefaultImportSettings()
		{
			var settings = new ImportSettings
			{
				CheckMatchingSublanguages = false,
				ExistingFieldsUpdateMode = ImportSettings.FieldUpdateMode.Merge,
				IncrementUsageCount = false,
				PlainText = false,
				NewFields = ImportSettings.NewFieldsOption.AddToSetup,
				ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew,
				AlignmentQuality = 0,
				TUProcessingMode = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly,
				TagCountLimit = 250,
				IsDocumentImport = true, // mandatory in order to have CM instead of 100% 
				UseTmUserIdFromBilingualFile = true,
			};
			if (Settings is null) return settings;
			settings.PlainText = Settings.ImportPlain;
			settings.UseTmUserIdFromBilingualFile = Settings.UseBilingualInfo;
			settings.ExistingTUsUpdateMode = Settings.TuUpdateMode;

			return settings;
		}
}
}
