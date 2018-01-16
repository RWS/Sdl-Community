using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;

namespace Sdl.Community.Utilities.TMTool.Lib.TMHelpers
{
	public class Importer
	{
		private TranslationMemoryImporter _importer;
		private DataImportSettings _settings;

		private int TUs;

		public delegate void OnProgressDelegate(double progress);
		public event OnProgressDelegate OnProgress;

		/// <summary>
		/// number of translation units read
		/// </summary>
		public int TUsProcessed
		{
			get;
			private set;
		}

		/// <summary>
		/// number of translation units imported
		/// </summary>
		public int TUsImported
		{
			get;
			private set;
		}

		/// <summary>
		/// creates new importer, sets importer settings
		/// </summary>
		/// <param name="tm">TM to import to</param>
		/// <param name="settings">import settings</param>
		public Importer(FileBasedTranslationMemory tm, DataImportSettings settings)
		{
			_settings = settings;

			_importer = new TranslationMemoryImporter(tm.LanguageDirection);
			ChangeSettings();
		}

		/// <summary>
		/// performs TMX to TM import operation
		/// </summary>
		/// <param name="tmxPath">TMX path</param>
		/// <param name="TUsCount">number of TUs in TMX file</param>
		public void Import(string tmxPath, int TUsCount)
		{
			TUs = TUsCount;
			TUsProcessed = 0;
			ProgressImport(0);

			_importer.BatchImported += new EventHandler<BatchImportedEventArgs>(importer_BatchImported);

			// begin import
			_importer.Import(tmxPath);

			ProgressImport(100);
		}

		void importer_BatchImported(object sender, BatchImportedEventArgs e)
		{
			TUsProcessed = e.Statistics.TotalRead;
			TUsImported = e.Statistics.TotalImported;

			if (TUs > 0)
				ProgressImport(TUsProcessed * 100 / TUs);
		}

		private void ChangeSettings()
		{
			_importer.ImportSettings.TUProcessingMode = _settings.Scenario;
			_importer.ImportSettings.OverwriteExistingTUs = _settings.OverwriteExistingTUs;

			//if (_settings.ExportInvalidTUs)
			//    _importer.ImportSettings.InvalidTranslationUnitsExportPath = _settings.ExportInvalidPath;
		}

		private void ProgressImport(double progress)
		{
			if (this.OnProgress != null)
			{
				this.OnProgress(progress);
			}
		}
	}
}