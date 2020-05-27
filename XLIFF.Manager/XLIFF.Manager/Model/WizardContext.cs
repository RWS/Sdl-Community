using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WizardContext : BaseModel, ICloneable
	{
		public WizardContext()
		{
			Support = Enumerators.XLIFFSupport.xliff12polyglot;
			ProjectFiles = new List<ProjectFile>();
			IncludeTranslations = false;
			CopySourceToTarget = false;
			DateTimeStamp = DateTime.UtcNow;
			Completed = false;
			Message = string.Empty;
		}

		public bool Completed { get; set; }

		public string Message { get; set; }

		public Project Project { get; set; }

		public List<ProjectFile> ProjectFiles { get; set; }

		public bool IncludeTranslations { get; set; }

		public bool CopySourceToTarget { get; set; }

		public string OutputFolder { get; set; }

		public string WorkingFolder => Path.Combine(OutputFolder, DateTimeStampToString);

		public string ProjectFolder { get; set; }

		public DateTime DateTimeStamp { get; set; }

		public Enumerators.XLIFFSupport Support { get; set; }

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

		public string GetLanguageFolder(CultureInfo cultureInfo)
		{
			var languageFolder = Path.Combine(OutputFolder, DateTimeStampToString, cultureInfo.Name);
			return languageFolder;
		}

		public object Clone()
		{
			var model = new WizardContext
			{
				OutputFolder = OutputFolder,
				DateTimeStamp = DateTimeStamp,
				IncludeTranslations = IncludeTranslations,
				CopySourceToTarget = CopySourceToTarget,
				Support = Support
			};

			foreach (var projectFileModel in ProjectFiles)
			{
				model.ProjectFiles.Add(projectFileModel.Clone() as ProjectFile);
			}

			return model;
		}
	}
}
