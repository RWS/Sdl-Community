using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class Settings
	{
		public Settings()
		{
			ExportOptions = new ExportOptions();
			ExportOptions.XliffSupport = Enumerators.XLIFFSupport.xliff12sdl;
			ExportOptions.IncludeTranslations = true;
			ExportOptions.CopySourceToTarget = true;
			ExportOptions.ExcludeFilterIds = new List<string>();

			ImportOptions = new ImportOptions();			
			ImportOptions.BackupFiles = true;
			ImportOptions.OverwriteTranslations = true;
			ImportOptions.OriginSystem = string.Empty;
			ImportOptions.StatusTranslationUpdatedId = ConfirmationLevel.Translated.ToString();
			ImportOptions.StatusTranslationNotUpdatedId = string.Empty;
			ImportOptions.StatusSegmentNotImportedId = string.Empty;
			ImportOptions.ExcludeFilterIds = new List<string>();
		}

		public ExportOptions ExportOptions { get; set; }

		public ImportOptions ImportOptions { get; set; }
	}
}
