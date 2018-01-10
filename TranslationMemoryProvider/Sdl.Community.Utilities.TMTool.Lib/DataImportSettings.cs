using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.Utilities.TMTool.Lib
{
	public class DataImportSettings
	{
		/// <summary>
		/// preserve admin-level password
		/// </summary>
		public bool PreservePsw
		{ get; set; }

		/// <summary>
		/// remove duplicate TUs
		/// </summary>
		public bool OverwriteExistingTUs
		{ get; set; }

		/// <summary>
		/// Import TUs processing mode
		/// </summary>
		public ImportSettings.ImportTUProcessingMode Scenario
		{ get; set; }

		/// <summary>
		/// create original file backup copy
		/// </summary>
		public bool CreateBackupFile
		{ get; set; }

		public DataImportSettings()
		{
			Scenario = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
		}

		public DataImportSettings(ImportSettings.ImportTUProcessingMode scenario)
		{
			Scenario = scenario;
		}
	}
}