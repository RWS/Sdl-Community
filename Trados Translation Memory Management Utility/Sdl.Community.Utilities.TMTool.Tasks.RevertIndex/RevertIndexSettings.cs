using Sdl.Community.Utilities.TMTool.Task;
using Sdl.LanguagePlatform.TranslationMemory;
using System.IO;

namespace Sdl.Community.Utilities.TMTool.Tasks.RevertIndex
{
	public class RevertIndexSettings : ISettings
	{
		/// <summary>
		/// if protected TM, preserve admin level psw
		/// </summary>
		public bool IsPreservePsw
		{ get; set; }
		/// <summary>
		/// overwrite duplicate TUs
		/// </summary>
		public bool IsOverwriteTUs
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

			if (TargetFolder.Length < 1)
			{
				errMsg = Properties.Resources.errTargetFolderLength;
				return false;
			}
			if (!Directory.Exists(Path.GetPathRoot(TargetFolder)))
			{
				errMsg = Properties.Resources.errTargetFolderNotLegal;
				return false;
			}

			return true;
		}

		/// <summary>
		/// resets settings to defaults
		/// </summary>
		public void ResetToDefaults()
		{
			IsPreservePsw = true;
			IsOverwriteTUs = false;
			Scenario = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
			TargetFolder = string.Empty;
		}
	}
}