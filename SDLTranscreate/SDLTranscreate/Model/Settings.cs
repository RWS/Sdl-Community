using System.Collections.Generic;
using Sdl.Community.Transcreate.Common;
using Sdl.Core.Globalization;

namespace Sdl.Community.Transcreate.Model
{
	public class Settings
	{
		public Settings()
		{
			ConvertOptions = new ConvertOptions();
			ConvertOptions.MaxAlternativeTranslations = 3;

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

		public ConvertOptions ConvertOptions { get; set; }

		public ExportOptions ExportOptions { get; set; }

		public ImportOptions ImportOptions { get; set; }
	}
}
