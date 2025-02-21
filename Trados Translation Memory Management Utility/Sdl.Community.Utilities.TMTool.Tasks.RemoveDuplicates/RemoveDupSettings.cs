using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.Community.Utilities.TMTool.Task;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemoveDuplicates
{
	public class RemoveDupSettings : ISettings
	{
		/// <summary>
		/// if protected TM, preserve admin level psw
		/// </summary>
		public bool IsPreservePsw
		{ get; set; }
		/// <summary>
		/// create file backup copy
		/// </summary>
		public bool IsBackupFiles
		{ get; set; }
		/// <summary>
		/// ImportTUProcessingMode scenario
		/// </summary>
		public ImportSettings.ImportTUProcessingMode Scenario
		{ get; set; }
		/// <summary>
		/// folder to create output files in
		/// </summary>
		public string TargetFolder
		{ get; set; }

		/// <summary>
		/// performs settings validation
		/// </summary>
		/// <param name="errMsg">error message, empty if validation succeeded</param>
		/// <returns>false - if validation failed</returns>
		public bool ValidateSettings(out string errMsg)
		{
			errMsg = string.Empty;
			return true;
		}

		/// <summary>
		/// resets settings to defaults
		/// </summary>
		public void ResetToDefaults()
		{
			IsPreservePsw = true;
			IsBackupFiles = false;
			Scenario = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
		}
	}
}