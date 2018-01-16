using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System.Globalization;
using Sdl.Community.Utilities.TMTool.Lib.TMHelpers;

namespace Sdl.Community.Utilities.TMTool.Lib
{
	public class TMCreator
	{
		public delegate void OnProgressDelegate(double progress, int operationType);
		public event OnProgressDelegate OnProgress;

		/// <summary>
		/// path of new TM file
		/// </summary>
		public string FilePath
		{
			get;
			private set;
		}

		/// <summary>
		/// new TM
		/// </summary>
		public FileBasedTranslationMemory NewTM
		{
			get;
			private set;
		}

		/// <summary>
		/// number of TUs in TM (can be added through ImportData)
		/// </summary>
		public int TUsCount
		{
			get;
			private set;
		}

		/// <summary>
		/// creates new TMCreator
		/// </summary>
		/// <param name="filePath">path for new TM (will be modified if file with same path already exists)</param>
		public TMCreator(string filePath)
		{
			FilePath = filePath;
		}

		#region public
		/// <summary>
		/// creates new TM
		/// </summary>
		/// <param name="desc">TM description</param>
		/// <param name="sourceLang">TM source language (e.g. en-US)</param>
		/// <param name="targetLang">TM target language (e.g. de-DE)</param>
		public void CreateNewTM(string desc, string sourceLang, string targetLang)
		{
			FilePath = FileHelper.ChangeFileName(FilePath, @"{0}\{1}_{2}.sdltm");

			NewTM = new FileBasedTranslationMemory(FilePath,
				(desc == null ? "" : desc),
				CultureInfo.GetCultureInfo(sourceLang),
				CultureInfo.GetCultureInfo(targetLang),
				DefFuzzyIndexes(),
				DefRecognizers(),
				TokenizerFlags.DefaultFlags,
				WordCountFlags.DefaultFlags);
			TUsCount = 0;

			// clear resources
			NewTM.LanguageResourceBundles.Clear();

			NewTM.Save();
		}

		/// <summary>
		/// creates new TM
		/// </summary>
		/// <param name="TM">TM to get settings (desc, source lang, target lang) from</param>
		/// <param name="isIndexRevert">create TM with reverted index</param>
		public void CreateNewTM(FileBasedTranslationMemory TM, bool isIndexRevert)
		{
			FilePath = FileHelper.ChangeFileName(FilePath, @"{0}\{1}_{2}.sdltm");

			if (isIndexRevert)
			{
				NewTM = new FileBasedTranslationMemory(FilePath,
					(TM.Description == null ? "" : TM.Description),
					TM.LanguageDirection.TargetLanguage,
					TM.LanguageDirection.SourceLanguage,
					TM.FuzzyIndexes,
					TM.Recognizers,
					TM.TokenizerFlags,
					TM.WordCountFlags);
			}
			else
			{
				NewTM = new FileBasedTranslationMemory(FilePath,
					   (TM.Description == null ? "" : TM.Description),
					   TM.LanguageDirection.SourceLanguage,
					   TM.LanguageDirection.TargetLanguage,
					   TM.FuzzyIndexes,
					   TM.Recognizers,
					   TM.TokenizerFlags,
					   TM.WordCountFlags);
			}
			TUsCount = 0;

			// manage settings, get settings from old TM
			CopySettings(TM);

			NewTM.Save();
		}

		/// <summary>
		/// import data from TMX
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="tmxFilePath"></param>
		/// <param name="TUsCountExpected">count of TUs in TMX file</param>
		/// <returns></returns>
		public bool ImportData(DataImportSettings settings, string tmxFilePath, int TUsCountExpected)
		{
			if (NewTM != null)
			{
				Importer tmxImp = new Importer(NewTM, settings);
				tmxImp.OnProgress += new Importer.OnProgressDelegate(updateImportProgress);
				tmxImp.Import(tmxFilePath, TUsCountExpected);
				TUsCount += tmxImp.TUsImported;

				return true;
			}
			return false;
		}

		/// <summary>
		/// set TM protection
		/// </summary>
		/// <param name="psw">password to set</param>
		public void SetAdminProtection(string psw)
		{
			if (NewTM != null)
			{
				NewTM.SetAdministratorPassword(psw);

				NewTM.Save();
			}
		}
		#endregion

		#region private
		private void CopySettings(FileBasedTranslationMemory TM)
		{
			if (NewTM != null)
			{
				// field definitions
				NewTM.FieldDefinitions.AddRange(TM.FieldDefinitions);

				// language resources
				NewTM.LanguageResourceBundles.Clear();
				//for (int i = 0; i < TM.LanguageResourceBundles.Count; i++ )
				//    NewTM.LanguageResourceBundles.Add(TM.LanguageResourceBundles[i]);

				// tuning settings
				NewTM.FuzzyIndexTuningSettings = TM.FuzzyIndexTuningSettings;
			}
		}

		private FuzzyIndexes DefFuzzyIndexes()
		{
			return FuzzyIndexes.SourceCharacterBased |
				FuzzyIndexes.SourceWordBased |
				FuzzyIndexes.TargetCharacterBased |
				FuzzyIndexes.TargetWordBased;
		}

		private BuiltinRecognizers DefRecognizers()
		{
			return BuiltinRecognizers.RecognizeAcronyms |
				BuiltinRecognizers.RecognizeDates |
				BuiltinRecognizers.RecognizeDates |
				BuiltinRecognizers.RecognizeNumbers |
				BuiltinRecognizers.RecognizeTimes |
				BuiltinRecognizers.RecognizeVariables |
				BuiltinRecognizers.RecognizeMeasurements;
		}

		private void updateImportProgress(double progress)
		{
			ProgressImport(progress);
		}

		private void ProgressImport(double progress)
		{
			if (this.OnProgress != null)
			{
				this.OnProgress(progress, 1);
			}
		}
		#endregion
	}
}