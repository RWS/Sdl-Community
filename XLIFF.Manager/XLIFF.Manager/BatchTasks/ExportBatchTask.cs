using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Community.XLIFF.Manager.Service;
using Sdl.Community.XLIFF.Manager.Wizard.ViewModel.Export;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using ProjectFile = Sdl.ProjectAutomation.Core.ProjectFile;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	[AutomaticTask("XLIFF.Manager.BatchTasks.Export",
		"Export to XLIFF",
		"Export to XLIFF",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(ExportSettings), typeof(ExportSettingsPage))]
	public class ExportBatchTask: AbstractFileContentProcessingAutomaticTask
	{
		private ExportSettings _exportSettings;

		protected override void OnInitializeTask()
		{		
			_exportSettings = GetSetting<ExportSettings>();			
			RemoveSettingsFromProject(_exportSettings.Id);
			base.OnInitializeTask();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			
		}

		private void RemoveSettingsFromProject(string id)
		{
			var projectSettings = Project.GetSettings();
			projectSettings.RemoveSettingsGroup(id);
			Project.UpdateSettings(projectSettings);
		}


		//public string GetDefaultTransactionPath(string localProjectFolder)
		//{
		//	var rootPath = Path.Combine(localProjectFolder, "XLIFF.Manager");
		//	var path = Path.Combine(rootPath, Enumerators.Action.Export.ToString());

		//	if (!Directory.Exists(rootPath))
		//	{
		//		Directory.CreateDirectory(rootPath);
		//	}

		//	if (!Directory.Exists(path))
		//	{
		//		Directory.CreateDirectory(path);
		//	}

		//	return path;
		//}


	}
}
