using System;
using System.IO;
using System.Xml.Linq;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.Settings;

namespace Sdl.Community.FailSafeTask
{
	[AutomaticTask("Sdl.Community.FailSafeTask",
		"Failsafe Task",
		"Verifies generating target translation is possible",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(FailSafeTaskSettings), typeof(FailSafeTaskSettingsPage))]
	public class FailSafeTask : AbstractFileContentProcessingAutomaticTask
	{
		private const string backupFolder = "FailSafeTaskBackups";
		private readonly XElement root = new XElement("FailSafe");
		private string backupPath = null;
		private FailSafeTaskSettings settings = null;
		private readonly ErrorMessageReporter errorMsgReporter = new ErrorMessageReporter();

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			return true;
		}

		public override void TaskComplete()
		{
			if (!settings.CopyToTarget)
			{
				Project.RunAutomaticTask(TaskFiles.GetIds(), AutomaticTaskTemplateIds.PseudoTranslateFiles);
			}

			foreach (var file in TaskFiles)
			{
				TestFiles(file);
			}

			RestoreFiles();

			CreateReport("Fail Safe Task Report", "Determines whether native file generation is possible", root.ToString());

			// Report any errors to prevent subsequent tasks from running
			errorMsgReporter.ReportErrors();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(errorMsgReporter));
			PrepareForGenerateTarget(projectFile, multiFileConverter);
		}

		protected override void OnInitializeTask()
		{
			settings = GetSetting<FailSafeTaskSettings>();

			BackupFiles();
		}

		private void BackupFiles()
		{
			var projectInfo = Project.GetProjectInfo();
			if (projectInfo != null)
			{
				backupPath = Path.Combine(projectInfo.LocalProjectFolder, backupFolder);

				if (!Directory.Exists(backupPath))
				{
					Directory.CreateDirectory(backupPath);

					foreach (var file in TaskFiles)
					{
						var savePath = Path.Combine(backupPath, file.Name);

						File.Copy(file.LocalFilePath, savePath, true);
					}
				}
			}
		}

		private string FormatMessage(ExecutionMessage[] messages)
		{
			var msg = "";

			foreach (var message in messages)
			{
				msg += message.Message == null ? "" : message.Message + Environment.NewLine;
				msg += message.Exception == null ? "" : message.Exception.ToString() + Environment.NewLine;
			}

			return msg;
		}

		private void PrepareForGenerateTarget(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new ClearTarget()));

			if (settings.CopyToTarget)
			{
				multiFileConverter.AddBilingualProcessor(new BilingualContentHandlerAdapter(new CopySourceToTarget()));
			}
		}

		private void RestoreFiles()
		{
			foreach (var file in TaskFiles)
			{
				var savePath = Path.Combine(backupPath, file.Name);

				File.Copy(savePath, file.LocalFilePath, true);
			}

			Directory.Delete(backupPath, true);
		}

		private void TestFiles(ProjectFile projectFile)
		{
			var failed = false;

			var settings = GetSetting<ExportFilesSettings>();

			// Store current settings so they can be restored later
			var exportLocation = settings.GetSetting<string>("ExportLocation");
			var fileVersion = settings.GetSetting<ExportFileVersion>("ExportFileVersion");

			settings.BeginEdit();
			settings.GetSetting<string>("ExportLocation").Value = backupPath;
			settings.GetSetting<ExportFileVersion>("ExportFileVersion").Value = ExportFileVersion.Native;
			settings.EndEdit();

			AutomaticTask task = null;

			try
			{
				task = Project.RunAutomaticTask(new[] { projectFile.Id }, AutomaticTaskTemplateIds.ExportFiles);
			}
			catch (Exception e)
			{
				failed = true;
				root.Add(new XElement("Result",
							new XAttribute("Name", projectFile.Name),
							new XAttribute("Failed", true),
							e.ToString()));
			}
			finally
			{
				// Restore
				settings.BeginEdit();
				settings.GetSetting<string>("ExportLocation").Value = exportLocation;
				settings.GetSetting<ExportFileVersion>("ExportFileVersion").Value = fileVersion;
				settings.EndEdit();
			}

			if (!failed && (task.Status != TaskStatus.Failed))
			{
				root.Add(new XElement("Result",
							new XAttribute("Name", projectFile.Name),
							new XAttribute("Failed", false)));
			}
			else
			{
				root.Add(new XElement("Result",
						new XAttribute("Name", projectFile.Name),
						new XAttribute("Failed", true),
						FormatMessage(task.Messages)));

				errorMsgReporter.StoreMessage(projectFile.Name, FormatMessage(task.Messages));
			}
		}
	}
}