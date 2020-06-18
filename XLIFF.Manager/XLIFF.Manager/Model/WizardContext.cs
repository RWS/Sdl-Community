using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Core.Globalization;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WizardContext : BaseModel
	{
		public WizardContext()
		{
			Action = Enumerators.Action.None;
			ProjectFiles = new List<ProjectFile>();
			DateTimeStamp = DateTime.UtcNow;

			ExportSupport = Enumerators.XLIFFSupport.xliff12polyglot;
			ExportIncludeTranslations = false;
			ExportCopySourceToTarget = false;

			ImportBackupFiles = true;
			ImportOverwriteTranslations = true;
			ImportOriginSystem = string.Empty;
			ImportConfirmationStatus = ConfirmationLevel.Draft;

			ExcludeFilterItems = new List<FilterItem>();
		}

		public List<FilterItem> ExcludeFilterItems { get; set; }

		public Enumerators.Action Action { get; set; }

		public bool Completed { get; set; }

		public string Message { get; set; }

		public Project Project { get; set; }

		public List<ProjectFile> ProjectFiles { get; set; }

		public string TransactionFolder { get; set; }

		public string WorkingFolder => Path.Combine(TransactionFolder, DateTimeStampToString);

		public string LocalProjectFolder { get; set; }

		public DateTime DateTimeStamp { get; set; }

		public string DateTimeStampToString
		{
			get
			{
				var value = DateTimeStamp.Year
							+ "" + DateTimeStamp.Month.ToString().PadLeft(2, '0')
							+ "" + DateTimeStamp.Day.ToString().PadLeft(2, '0')
							+ "" + DateTimeStamp.Hour.ToString().PadLeft(2, '0')
							+ "" + DateTimeStamp.Minute.ToString().PadLeft(2, '0')
							+ "" + DateTimeStamp.Second.ToString().PadLeft(2, '0');

				return value;
			}
		}

		public Enumerators.XLIFFSupport ExportSupport { get; set; }

		public bool ExportIncludeTranslations { get; set; }

		public bool ExportCopySourceToTarget { get; set; }

		public bool ImportBackupFiles { get; set; }

		public bool ImportOverwriteTranslations { get; set; }

		public string ImportOriginSystem { get; set; }

		public ConfirmationLevel ImportConfirmationStatus { get; set; }

		public string GetDefaultTransactionPath()
		{
			var rootPath = Path.Combine(LocalProjectFolder, "XLIFF.Manager");
			var path = Path.Combine(rootPath, Action.ToString());

			if (!Directory.Exists(rootPath))
			{
				Directory.CreateDirectory(rootPath);
			}

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		public string GetLanguageFolder(CultureInfo cultureInfo)
		{
			var languageFolder = Path.Combine(WorkingFolder, cultureInfo.Name);
			return languageFolder;
		}
	}
}
