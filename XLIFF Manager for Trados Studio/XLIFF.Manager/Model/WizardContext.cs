using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class WizardContext : BaseModel
	{		
		public WizardContext(Enumerators.Action action, Settings settings)
		{
			Action = action;
			ProjectFiles = new List<ProjectFile>();
			DateTimeStamp = DateTime.UtcNow;
			
			ExportOptions = settings.ExportOptions;
			ImportOptions = settings.ImportOptions;
			
			Owner = Enumerators.Controller.None;
		}
	
		public Enumerators.Action Action { get; set; }

		public List<AnalysisBand> AnalysisBands { get; set; }

		public bool Completed { get; set; }

		public string Message { get; set; }

		public Enumerators.Controller Owner { get; set; }

		public Project Project { get; set; }

		public List<ProjectFile> ProjectFiles { get; set; }

		public string TransactionFolder { get; set; }

		public string WorkingFolder => Path.Combine(TransactionFolder, DateTimeStampToString);

		public string RelativeWorkingFolder => WorkingFolder.Replace(Project.Path + '\\', string.Empty).Trim('\\');

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

		public ExportOptions ExportOptions { get; set; }

		public ImportOptions ImportOptions { get; set; }

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

		public string GetLanguageFolder(string name)
		{
			var languageFolder = Path.Combine(WorkingFolder, name);
			return languageFolder;
		}		
	}
}
