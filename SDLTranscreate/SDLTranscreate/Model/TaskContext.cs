using System;
using System.Collections.Generic;
using System.IO;
using Sdl.ProjectAutomation.FileBased;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.Model
{
	public class TaskContext : BaseModel
	{		
		public TaskContext(Enumerators.Action action, Enumerators.WorkFlow workFlow, Settings settings)
		{
			Action = action;
			WorkFlow = workFlow;
			ProjectFiles = new List<ProjectFile>();
			DateTimeStamp = DateTime.UtcNow;
			
			ExportOptions = settings.ExportOptions;
			ImportOptions = settings.ImportOptions;
			ConvertOptions = settings.ConvertOptions;
			BackTranslationOptions = settings.BackTranslationOptions;

			Owner = Enumerators.Controller.None;
		}
	
		public Enumerators.Action Action { get; set; }

		public Enumerators.WorkFlow WorkFlow { get; set; }

		public List<AnalysisBand> AnalysisBands { get; set; }

		public bool Completed { get; set; }

		public string Message { get; set; }

		public Enumerators.Controller Owner { get; set; }

		public Interfaces.IProject Project { get; set; }

		public FileBasedProject FileBasedProject { get; set; }

		public List<ProjectFile> ProjectFiles { get; set; }

		public string WorkflowFolder { get; set; }

		public string WorkingFolder => Path.Combine(WorkflowFolder, DateTimeStampToString);

		public string ProjectBackupFolder => Path.Combine(WorkflowFolder, "Original");

		public string ProjectBackupPath => Path.Combine(ProjectBackupFolder, Project.Name + ".zip");

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

		public ConvertOptions ConvertOptions { get; set; }
		
		public BackTranslationOptions BackTranslationOptions { get; set; }

		public string GetWorkflowPath()
		{
			var rootPath = Path.Combine(LocalProjectFolder, "WorkFlow");
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
