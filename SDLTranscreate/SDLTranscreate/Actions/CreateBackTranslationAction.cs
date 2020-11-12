using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Model.ProjectSettings;
using Sdl.Community.Transcreate.Service;
using Sdl.Community.Transcreate.Service.ProgressDialog;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using File = System.IO.File;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_CreateBackTranslationProject_Action",
		Name = "TranscreateManager_CreateBackTranslationProject_Name",
		Description = "TranscreateManager_CreateBackTranslationProject_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "sdl_transcreate_back"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 3, DisplayType.Large)]
	public class CreateBackTranslationAction : AbstractViewControllerAction<TranscreateViewController>
	{
		private Settings _settings;
		private CustomerProvider _customerProvider;
		private PathInfo _pathInfo;
		private ImageService _imageService;
		private SegmentBuilder _segmentBuilder;
		private ProjectAutomationService _projectAutomationService;
		private Controllers _controllers;

		protected override void Execute()
		{
			var projects = _controllers.TranscreateController.GetSelectedProjects();
			if (projects?.Count != 1)
			{
				Enabled = false;
				return;
			}

			var project = projects[0];
			if (project is BackTranslationProject)
			{
				return;
			}

			var studioProject = _controllers.ProjectsController.GetProjects()
				.FirstOrDefault(a => a.GetProjectInfo().Id.ToString() == project.Id);
			if (studioProject == null)
			{
				return;
			}

			var studioProjectInfo = studioProject.GetProjectInfo();


			var backTranslationsFolder = Path.Combine(studioProjectInfo.LocalProjectFolder, "BackProjects");
			if (Directory.Exists(backTranslationsFolder))
			{
				var message01 = "The Back-Translations folder is not empty."
								+ Environment.NewLine + Environment.NewLine
								+ "'" + backTranslationsFolder + "'"
								+ Environment.NewLine + Environment.NewLine;
				var message02 = "Do you want to proceed and delete this folder?";

				var response = MessageBox.Show(message01 + message02, PluginResources.Plugin_Name,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (response != DialogResult.Yes)
				{
					return;
				}

				TryDeleteDirectory(backTranslationsFolder);
			}


			var taskContexts = new List<TaskContext>();

			var progressSettings = new ProgressDialogSettings(ApplicationInstance.GetActiveForm(), true, true, true);
			var result = ProgressDialog.Execute("Create Back-Translation Projects", () =>
			{
				ProgressDialog.Current.Report(0, "Reading language files...");

				var dateTimeStamp = DateTime.UtcNow;
				var dataTimeStampToString = DateTimeStampToString(dateTimeStamp);
				var workFlowPath = GetPath(studioProjectInfo.LocalProjectFolder, "WorkFlow");
				var workingActionPath = GetPath(workFlowPath, "Convert");
				var workingFolder = GetPath(workingActionPath, dataTimeStampToString);

				var exportOptions = new ExportOptions();
				exportOptions.IncludeBackTranslations = true;
				exportOptions.IncludeTranslations = true;
				exportOptions.CopySourceToTarget = false;

				var importOptions = new ImportOptions();
				importOptions.OverwriteTranslations = true;
				importOptions.OriginSystem = "Transcreate Automation";
				importOptions.StatusTranslationUpdatedId = string.Empty;

				var analysisBands = _projectAutomationService.GetAnalysisBands(studioProject);

				var sdlxliffReader = new SdlxliffReader(_segmentBuilder, exportOptions, analysisBands);
				var sdlxliffWriter = new SdlxliffWriter(_segmentBuilder, importOptions, analysisBands);
				var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

				// Read the SDLXLIFF data
				var fileDataList = GetFileDataList(project, studioProjectInfo, sdlxliffReader);
				var filesWithEmptyTranslations = fileDataList.Count(a => a.HasEmptyTranslations);
				if (filesWithEmptyTranslations > 0)
				{
					var message01 = string.Format(PluginResources.Found_empty_translations_in_0_files,
										filesWithEmptyTranslations)
									+ Environment.NewLine + Environment.NewLine;
					var message02 = PluginResources.Proceed_and_copy_source_to_target_for_empty_translations;

					var response = MessageBox.Show(message01 + message02, PluginResources.Plugin_Name,
						MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (response != DialogResult.Yes)
					{
						return;
					}
				}

				ProgressDialog.Current.ProgressBarIsIndeterminate = false;
				decimal maximum = project.TargetLanguages.Count;
				decimal current = 0;

				foreach (var targetLanguage in project.TargetLanguages)
				{
					if (ProgressDialog.Current.CheckCancellationPending())
					{
						ProgressDialog.Current.ThrowIfCancellationPending();
					}

					current++;
					var progress = current / maximum * 100;
					ProgressDialog.Current.Report((int)progress, "Language: " + targetLanguage.CultureInfo.DisplayName);


					var sourceFiles = new List<string>();
					var languageFolder = GetPath(workingFolder, targetLanguage.CultureInfo.Name);

					var targetFiles = project.ProjectFiles.Where(a =>
						string.Compare(a.TargetLanguage, targetLanguage.CultureInfo.Name,
							StringComparison.CurrentCultureIgnoreCase) == 0);

					var languageFileData = new List<FileData>();
					foreach (var projectFile in targetFiles)
					{
						var fileData =
							fileDataList.FirstOrDefault(a => a.Data.DocInfo.DocumentId == projectFile.FileId);
						if (fileData == null)
						{
							continue;
						}

						SwitchSourceWithTargetSegments(fileData);

						var xliffFolder = GetPath(languageFolder, projectFile.Path);
						var xliffFilePath = Path.Combine(xliffFolder,
							projectFile.Name.Substring(0, projectFile.Name.Length - ".sdlxliff".Length));

						// Write the XLIFF file
						var success = xliffWriter.WriteFile(fileData.Data, xliffFilePath, true);
						if (!success)
						{
							throw new Exception(string.Format(
								PluginResources.Unexpected_error_while_converting_the_file, xliffFilePath));
						}

						sourceFiles.Add(xliffFilePath);
						languageFileData.Add(fileData);
					}

					var iconPath = GetBackTranslationIconPath();

					var newStudioProject = _projectAutomationService.CreateBackTranslationProject(
						studioProject, targetLanguage.CultureInfo.Name, iconPath, sourceFiles, "BT");

					_projectAutomationService.RunPretranslationWithoutTm(newStudioProject);

					var taskContext = CreateBackTranslationTaskContext(newStudioProject, languageFileData,
						studioProjectInfo.LocalProjectFolder, sdlxliffReader, sdlxliffWriter, xliffWriter);

					taskContext.Completed = true;
					taskContexts.Add(taskContext);
				}

			}, progressSettings);

			if (result.Cancelled || result.OperationFailed)
			{
				TryDeleteDirectory(backTranslationsFolder);

				var message = result.Cancelled ? "Process cancelled by user." : result.Error?.Message;
				MessageBox.Show(message, PluginResources.Plugin_Name);
				return;
			}

			foreach (var taskContext in taskContexts)
			{
				CleanupProjectSettings(taskContext.FileBasedProject);

				ActivateProject(taskContext.FileBasedProject);
				_projectAutomationService.RemoveLastReportOfType("Translate");

				var reports = _controllers.TranscreateController.CreateHtmlReports(taskContext, taskContext.FileBasedProject, taskContext.Project);
				_controllers.TranscreateController.ReportsController.AddReports(_controllers.TranscreateController.ClientId, reports);

				_controllers.TranscreateController.UpdateBackTranslationProjectData(project, taskContext);
			}

			_controllers.TranscreateController.InvalidateProjectsContainer();

			Enabled = false;
		}

		private static void TryDeleteDirectory(string backTranslationsFolder)
		{
			if (Directory.Exists(backTranslationsFolder))
			{
				try
				{
					Directory.Delete(backTranslationsFolder, true);
				}
				catch
				{
					// ignore catch all;
				}
			}
		}

		private void ActivateProject(FileBasedProject project)
		{
			_controllers.ProjectsController.Close(project);
			_controllers.ProjectsController.Add(project.FilePath);
		}

		private TaskContext CreateBackTranslationTaskContext(FileBasedProject newStudioProject,
			IReadOnlyCollection<FileData> languageFileData, string localProjectFolder,
			SdlxliffReader sdlxliffReader, SdlxliffWriter sdlxliffWriter, XliffWriter xliffWriter)
		{

			var newStudioProjectInfo = newStudioProject.GetProjectInfo();

			var action = Enumerators.Action.CreateBackTranslation;
			var workFlow = Enumerators.WorkFlow.Internal;
			var setttings = GetSettings();

			var taskContext = new TaskContext(action, workFlow, setttings);
			taskContext.AnalysisBands = _projectAutomationService.GetAnalysisBands(newStudioProject);
			taskContext.ExportOptions.IncludeBackTranslations = true;
			taskContext.ExportOptions.IncludeTranslations = true;
			taskContext.ExportOptions.CopySourceToTarget = false;

			
			taskContext.LocalProjectFolder = newStudioProjectInfo.LocalProjectFolder;
			taskContext.WorkflowFolder = taskContext.GetWorkflowPath();

			var workingProject = _projectAutomationService.GetProject(newStudioProject, null);
			workingProject.ProjectFiles.RemoveAll(a => a.TargetLanguage == workingProject.SourceLanguage.CultureInfo.Name);
			taskContext.Project = workingProject;
			taskContext.FileBasedProject = newStudioProject;
			taskContext.ProjectFiles = workingProject.ProjectFiles;

			foreach (var projectFile in taskContext.ProjectFiles)
			{
				projectFile.Selected = true;
				var fileData = GetFileData(languageFileData, localProjectFolder, projectFile);

				var tmpInputFile = Path.GetTempFileName();
				File.Move(tmpInputFile, tmpInputFile + ".sdlxliff");
				tmpInputFile = tmpInputFile + ".sdlxliff";

				var paragraphMap = GetParagraphMap(sdlxliffReader, projectFile.ProjectId, projectFile.FileId,
					projectFile.Location, projectFile.TargetLanguage);
				AlignParagraphIds(fileData.Data, paragraphMap.Keys.ToList());

				var filePath = Path.Combine(taskContext.WorkingFolder, projectFile.Path.Trim('\\'));

				var externalFilePath = Path.Combine(filePath, projectFile.Name + ".xliff");
				if (!Directory.Exists(filePath))
				{
					Directory.CreateDirectory(filePath);
				}

				xliffWriter.WriteFile(fileData.Data, externalFilePath, true);

				var success = sdlxliffWriter.UpdateFile(fileData.Data, projectFile.Location, tmpInputFile, true);
				if (success)
				{
					projectFile.Date = taskContext.DateTimeStamp;
					projectFile.Action = action;
					projectFile.WorkFlow = workFlow;
					projectFile.Status = Enumerators.Status.Success;
					projectFile.Report = string.Empty;
					projectFile.ExternalFilePath = externalFilePath;
					projectFile.ConfirmationStatistics = sdlxliffWriter.ConfirmationStatistics;
					projectFile.TranslationOriginStatistics = sdlxliffWriter.TranslationOriginStatistics;
				}

				var activityFile = new ProjectFileActivity
				{
					ProjectFileId = projectFile.FileId,
					ActivityId = Guid.NewGuid().ToString(),
					Action = action,
					WorkFlow = workFlow,
					Status = success ? Enumerators.Status.Success : Enumerators.Status.Error,
					Date = projectFile.Date,
					Name = Path.GetFileName(projectFile.ExternalFilePath),
					Path = Path.GetDirectoryName(projectFile.ExternalFilePath),
					Report = string.Empty,
					ProjectFile = projectFile,
					ConfirmationStatistics = projectFile.ConfirmationStatistics,
					TranslationOriginStatistics = projectFile.TranslationOriginStatistics
				};

				projectFile.ProjectFileActivities.Add(activityFile);

				File.Copy(projectFile.Location, Path.Combine(filePath, projectFile.Name));
				File.Delete(projectFile.Location);

				File.Copy(tmpInputFile, projectFile.Location, true);
				File.Delete(tmpInputFile);
			}

			return taskContext;
		}

		private void SwitchSourceWithTargetSegments(FileData fileData)
		{
			var sourceLanguage = fileData.Data.DocInfo.SourceLanguage;
			fileData.Data.DocInfo.SourceLanguage = fileData.Data.DocInfo.TargetLanguage;
			fileData.Data.DocInfo.TargetLanguage = sourceLanguage;

			var languagePathId = "\\" + fileData.Data.DocInfo.SourceLanguage + "\\";
			var languagePathLocation =
				fileData.Data.DocInfo.Source.LastIndexOf(languagePathId, StringComparison.CurrentCultureIgnoreCase);
			if (languagePathLocation > -1)
			{
				var prefix = fileData.Data.DocInfo.Source.Substring(0, languagePathLocation);
				var suffix = fileData.Data.DocInfo.Source.Substring(languagePathLocation + languagePathId.Length);
				fileData.Data.DocInfo.Source = prefix + "\\" + fileData.Data.DocInfo.TargetLanguage + "\\" + suffix;
			}

			foreach (var file in fileData.Data.Files)
			{
				file.SourceLanguage = fileData.Data.DocInfo.SourceLanguage;
				file.TargetLanguage = fileData.Data.DocInfo.TargetLanguage;

				foreach (var transUnit in file.Body.TransUnits)
				{
					foreach (var segmentPair in transUnit.SegmentPairs)
					{
						var source = segmentPair.Source.Clone() as Source;
						var target = segmentPair.Target.Clone() as Target;

						var sourceText = GetSegmentText(segmentPair.Source.Elements, true);
						var targetText = GetSegmentText(segmentPair.Target.Elements, true);

						if (string.IsNullOrWhiteSpace(sourceText) && string.IsNullOrWhiteSpace(targetText))
						{
							continue;
						}

						if (string.IsNullOrWhiteSpace(targetText))
						{
							target.Elements = (source.Clone() as Source).Elements;
						}

						segmentPair.Source.Elements = new List<Element>();
						segmentPair.Target.Elements = new List<Element>();

						// Remove comment tags
						// TODO: consider adding them as comments on source segment!
						foreach (var targetElement in target.Elements)
						{
							if (targetElement is ElementComment)
							{
								continue;
							}

							segmentPair.Source.Elements.Add(targetElement);
						}

						var backTranslation = segmentPair.TranslationOrigin?.GetMetaData("back-translation");
						if (backTranslation == null)
						{
							segmentPair.TranslationOrigin = null;
							segmentPair.ConfirmationLevel = ConfirmationLevel.Unspecified;
						}
						else
						{
							segmentPair.ConfirmationLevel = ConfirmationLevel.Translated;
						}
					}
				}
			}
		}

		private void CleanupProjectSettings(FileBasedProject newStudioProject)
		{
			try
			{
				_controllers.ProjectsController.Close(newStudioProject);

				string content;
				using (var sr = new StreamReader(newStudioProject.FilePath, Encoding.UTF8))
				{
					content = sr.ReadToEnd();
					sr.Close();
				}

				var regex1 = new Regex(@"<SettingsGroup\s+Id\=""ReportsViewerSettings"">(.*?|)</SettingsGroup>",
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
				var match1 = regex1.Match(content);
				if (match1.Success)
				{
					var prefix = content.Substring(0, match1.Index).TrimEnd();
					var suffix = content.Substring(match1.Index + match1.Length).TrimStart();
					content = prefix + suffix;
				}

				var regex2 = new Regex(@"<SettingsGroup\s+Id\=""SDLTranscreateProject"">(.*?|)</SettingsGroup>",
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
				var match2 = regex2.Match(content);
				if (match2.Success)
				{
					var prefix = content.Substring(0, match2.Index).TrimEnd();
					var suffix = content.Substring(match2.Index + match2.Length).TrimStart();
					content = prefix + suffix;
				}

				var regex3 = new Regex(@"<SettingsGroup\s+Id\=""SDLTranscreateBackProjects"">(.*?|)</SettingsGroup>",
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
				var match3 = regex3.Match(content);
				if (match3.Success)
				{
					var prefix = content.Substring(0, match3.Index).TrimEnd();
					var suffix = content.Substring(match3.Index + match3.Length).TrimStart();
					content = prefix + suffix;
				}


				using (var writer = new StreamWriter(newStudioProject.FilePath, false, Encoding.UTF8))
				{
					writer.Write(content);
					writer.Flush();
					writer.Close();
				}
			}
			finally
			{
				_controllers.ProjectsController.Add(newStudioProject.FilePath);
			}
		}

		public void Run()
		{
			Execute();
		}

		private void UpdateProjectSettingsBundle(FileBasedProject project)
		{
			var settingsBundle = project.GetSettings();
			var sdlTranscreateProject = settingsBundle.GetSettingsGroup<SDLTranscreateProject>();

			var projectFiles = new List<SDLTranscreateProjectFile>();
			sdlTranscreateProject.ProjectFilesJson.Value = JsonConvert.SerializeObject(projectFiles);

			project.UpdateSettings(sdlTranscreateProject.SettingsBundle);
			project.Save();


			var sdlBackTranslateProjects = settingsBundle.GetSettingsGroup<SDLTranscreateBackProjects>();
			var backProjects = new List<SDLTranscreateBackProject>();
			sdlBackTranslateProjects.BackProjectsJson.Value = JsonConvert.SerializeObject(backProjects);

			project.UpdateSettings(sdlTranscreateProject.SettingsBundle);
			project.Save();
		}

		private Dictionary<string, List<string>> GetParagraphMap(Xliff xliffData)
		{
			var paragraphMap = new Dictionary<string, List<string>>();

			foreach (var file in xliffData.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					if (paragraphMap.ContainsKey(transUnit.Id))
					{
						continue;
					}

					var segmentIds = new List<string>();
					foreach (var segmentPair in transUnit.SegmentPairs)
					{
						segmentIds.Add(segmentPair.Id);
					}

					paragraphMap.Add(transUnit.Id, segmentIds);
				}
			}

			return paragraphMap;
		}

		private Dictionary<string, List<string>> GetParagraphMap(SdlxliffReader sdlxliffReader, string projectId, string fileId, string path, string targetLanguage)
		{
			var xliffData = sdlxliffReader.ReadFile(projectId, fileId, path, targetLanguage);
			return GetParagraphMap(xliffData);
		}

		private void AlignParagraphIds(Xliff xliffData, IReadOnlyList<string> paragraphIds)
		{
			var i = 0;
			foreach (var file in xliffData.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					transUnit.Id = paragraphIds[i++];
				}
			}
		}

		private FileData GetFileData(IEnumerable<FileData> languageFileData, string localProjectFolder, ProjectFile projectFile)
		{
			foreach (var fileData in languageFileData)
			{
				var fileDataFullPath = GetRelativePath(localProjectFolder, fileData.Data.DocInfo.Source);
				var projectFilePth = Path.Combine(projectFile.TargetLanguage, projectFile.Path.Trim('\\'));
				var projectFileFullPath = Path.Combine(projectFilePth, projectFile.Name);

				if (string.Compare(fileDataFullPath, projectFileFullPath, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return fileData;
				}
			}

			return null;
		}

		private string GetRelativePath(string projectPath, string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}

			return path.Replace(projectPath.Trim('\\') + '\\', string.Empty);
		}

		private string GetBackTranslationIconPath()
		{
			var iconPath = Path.Combine(_pathInfo.ApplicationIconsFolderPath, "BackTranslation.ico");
			if (!File.Exists(iconPath))
			{
				using (var fs = new FileStream(iconPath, FileMode.Create))
				{
					PluginResources.sdl_transcreate_back.Save(fs);
				}
			}

			return iconPath;
		}

		public string DateTimeStampToString(DateTime dt)
		{

			var value = dt.Year
						+ "" + dt.Month.ToString().PadLeft(2, '0')
						+ "" + dt.Day.ToString().PadLeft(2, '0')
						+ "" + dt.Hour.ToString().PadLeft(2, '0')
						+ "" + dt.Minute.ToString().PadLeft(2, '0')
						+ "" + dt.Second.ToString().PadLeft(2, '0');

			return value;
		}

		public string GetPath(string path1, string path2)
		{
			var path = Path.Combine(path1, path2.TrimStart('\\'));

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			return path;
		}

		private List<FileData> GetFileDataList(Interfaces.IProject project, ProjectInfo studioProjectInfo, SdlxliffReader sdlxliffReader)
		{
			var fileDataList = new List<FileData>();
			foreach (var targetLanguage in project.TargetLanguages)
			{
				var targetFiles = project.ProjectFiles.Where(a =>
					string.Compare(a.TargetLanguage, targetLanguage.CultureInfo.Name,
						StringComparison.CurrentCultureIgnoreCase) == 0);

				foreach (var projectFile in targetFiles)
				{
					var inputPath = Path.Combine(studioProjectInfo.LocalProjectFolder, projectFile.Location);
					var data = sdlxliffReader.ReadFile(project.Id, projectFile.FileId, inputPath, projectFile.TargetLanguage);
					var hasEmptyTranslations = ContainsEmptyTranslations(data);
					fileDataList.Add(new FileData
					{
						Data = data,
						HasEmptyTranslations = hasEmptyTranslations
					});
				}
			}

			return fileDataList;
		}

		private bool ContainsEmptyTranslations(Xliff data)
		{
			foreach (var file in data.Files)
			{
				foreach (var transUnit in file.Body.TransUnits)
				{
					if ((from segmentPair in transUnit.SegmentPairs
						 let sourceText = GetSegmentText(segmentPair.Source.Elements, true)
						 let targetText = GetSegmentText(segmentPair.Target.Elements, true)
						 where !string.IsNullOrWhiteSpace(sourceText) || !string.IsNullOrWhiteSpace(targetText)
						 select targetText).Any(string.IsNullOrWhiteSpace))
					{
						return true;
					}
				}
			}

			return false;
		}

		private string GetSegmentText(IEnumerable<Element> elements, bool includeTags)
		{
			var content = string.Empty;
			foreach (var element in elements)
			{
				if (element is ElementText text)
				{
					content += text.Text;
				}

				if (!includeTags)
				{
					continue;
				}

				if (element is ElementTagPair tag)
				{
					switch (tag.Type)
					{
						case Element.TagType.TagOpen:
							content += "<bpt ";
							content += "id=\"" + tag.TagId + "\">";
							content += tag.TagContent;
							content += "</bpt>";
							break;
						case Element.TagType.TagClose:
							content += "<ept ";
							content += "id=\"" + tag.TagId + "\">";
							content += tag.TagContent;
							content += "</ept>";
							break;
					}
				}

				if (element is ElementPlaceholder placeholder)
				{
					content += "<ph ";
					content += "id=\"" + placeholder.TagId + "\">";
					content += placeholder.TagContent;
					content += "</ph>";
				}

				if (element is ElementLocked locked)
				{
					switch (locked.Type)
					{
						case Element.TagType.TagOpen:
							content += "<mrk ";
							content += "mtype=\"protected\">";
							break;
						case Element.TagType.TagClose:
							content += "</mrk>";
							break;
					}
				}

				if (element is ElementComment comment)
				{
					switch (comment.Type)
					{
						case Element.TagType.TagOpen:
							content += "<mrk ";
							content += "mtype=\"x-sdl-comment\" ";
							content += "cid=\"" + comment.Id + "\">";

							break;
						case Element.TagType.TagClose:
							content += "</mrk>";
							break;
					}
				}
			}

			return content;
		}

		public override void Initialize()
		{
			Enabled = false;

			_customerProvider = new CustomerProvider();
			_pathInfo = new PathInfo();
			_imageService = new ImageService();
			_settings = GetSettings();
			_segmentBuilder = new SegmentBuilder();
			_controllers = new Controllers();
			_projectAutomationService = new ProjectAutomationService(_imageService, _controllers.TranscreateController, _customerProvider);

			_controllers.TranscreateController.ProjectSelectionChanged += ProjectsController_SelectedProjectsChanged;

			var projects = _controllers?.TranscreateController?.GetSelectedProjects();
			SetEnabled(projects?[0]);
		}

		private Settings GetSettings()
		{
			if (File.Exists(_pathInfo.SettingsFilePath))
			{
				var json = File.ReadAllText(_pathInfo.SettingsFilePath);
				return JsonConvert.DeserializeObject<Settings>(json);
			}

			return new Settings();
		}

		private void ProjectsController_SelectedProjectsChanged(object sender, ProjectSelectionChangedEventArgs e)
		{
			SetEnabled(e.SelectedProject);
		}

		private void SetEnabled(Interfaces.IProject selectedProject)
		{
			if (selectedProject == null ||
				selectedProject is BackTranslationProject ||
				selectedProject.BackTranslationProjects?.Count > 0)
			{
				Enabled = false;
			}
			else
			{
				Enabled = true;
			}
		}
	}
}
