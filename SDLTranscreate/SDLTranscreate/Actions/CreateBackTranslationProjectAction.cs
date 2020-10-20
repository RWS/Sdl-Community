using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using Sdl.Community.Transcreate.Common;
using Sdl.Community.Transcreate.CustomEventArgs;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;
using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Writers;
using Sdl.Community.Transcreate.LanguageMapping;
using Sdl.Community.Transcreate.LanguageMapping.Interfaces;
using Sdl.Community.Transcreate.Model;
using Sdl.Community.Transcreate.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using File = System.IO.File;
using ProjectFile = Sdl.Community.Transcreate.Model.ProjectFile;

namespace Sdl.Community.Transcreate.Actions
{
	[Action("TranscreateManager_CreateBackTranslationProject_Action",
		Name = "TranscreateManager_CreateBackTranslationProject_Name",
		Description = "TranscreateManager_CreateBackTranslationProject_Description",
		ContextByType = typeof(TranscreateViewController),
		Icon = "Icon"
		)]
	[ActionLayout(typeof(TranscreateManagerActionsGroup), 3, DisplayType.Large)]
	public class CreateBackTranslationProjectAction : AbstractViewControllerAction<TranscreateViewController>
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

			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var analysisBands = _projectAutomationService.GetAnalysisBands(studioProject);

			var sdlxliffReader = new SdlxliffReader(_segmentBuilder, exportOptions, analysisBands);
			var sdlxliffWriter = new SdlxliffWriter(fileTypeManager, _segmentBuilder, importOptions, analysisBands);
			var xliffWriter = new XliffWriter(Enumerators.XLIFFSupport.xliff12sdl);

			var proceed = true;

			// Read the SDLXLIFF data
			var fileDataList = GetFileDataList(project, studioProjectInfo, sdlxliffReader);
			var filesWithEmptyTranslations = fileDataList.Count(a => a.HasEmptyTranslations);
			if (filesWithEmptyTranslations > 0)
			{
				var message01 = string.Format("Found empty translations in {0} files", filesWithEmptyTranslations)
								+ Environment.NewLine + Environment.NewLine;
				var message02 = "Do you want to proceed and copy source to target for empty translations";

				var response = MessageBox.Show(message01 + message02, PluginResources.Plugin_Name,
					MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (response != DialogResult.Yes)
				{
					proceed = false;
				}
			}

			if (proceed)
			{
				foreach (var targetLanguage in project.TargetLanguages)
				{

					var sourceFiles = new List<string>();
					var languageFolder = GetPath(workingFolder, targetLanguage.CultureInfo.Name);

					var targetFiles = project.ProjectFiles.Where(a =>
						string.Compare(a.TargetLanguage, targetLanguage.CultureInfo.Name,
							StringComparison.CurrentCultureIgnoreCase) == 0);

					foreach (var projectFile in targetFiles)
					{
						var fileData = fileDataList.FirstOrDefault(a => a.Data.DocInfo.DocumentId == projectFile.FileId);
						if (fileData == null)
						{
							continue;
						}

						var sourceLanguage = fileData.Data.DocInfo.SourceLanguage;
						fileData.Data.DocInfo.SourceLanguage = fileData.Data.DocInfo.TargetLanguage;
						fileData.Data.DocInfo.TargetLanguage = sourceLanguage;

						var languagePathId = "\\" + fileData.Data.DocInfo.SourceLanguage + "\\";
						var languagePathLocation = fileData.Data.DocInfo.Source.LastIndexOf(languagePathId, StringComparison.CurrentCultureIgnoreCase);
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

									segmentPair.Source.Elements = target.Elements;
									segmentPair.Target.Elements = new List<Element>();


									// TODO: target elements should be inherited from the back-translations automatically

									//List<Token> backTranslationTokens = null;
									//if (segmentPair.TranslationOrigin != null &&
									//    segmentPair.TranslationOrigin.MetaDataContainsKey("back-translation"))
									//{
									//	//TODO check if we need to create TranslationOrigin??

									//	var backTranslation =
									//		segmentPair.TranslationOrigin.GetMetaData("back-translation");

									//	backTranslationTokens = JsonConvert.DeserializeObject<List<Token>>(backTranslation);
									//}
								}
							}
						}

						var xliffFolder = GetPath(languageFolder, projectFile.Path);
						var xliffFilePath = Path.Combine(xliffFolder,
							projectFile.Name.Substring(0, projectFile.Name.Length - ".sdlxliff".Length));

						// Write the XLIFF file
						var success = xliffWriter.WriteFile(fileData.Data, xliffFilePath, true);

						if (!success)
						{
							throw new Exception("TODO: error message: file not converted!");
						}

						sourceFiles.Add(xliffFilePath);


						//var sdlXliffFilePath = Path.Combine(languageFolder, projectFile.Path);
						//var tempFileName = Path.GetTempFileName();
						//var tmpInputFile = tempFileName + ".tmp.sdlxliff";
						//var s1 = sdlxliffWriter.UpdateFile(fileData.Data, sdlXliffFilePath, tmpInputFile);

						//File.Copy(tmpInputFile, sdlXliffFilePath + ".tmp.sdlxliff");
						//File.Delete(tempFileName);
						//File.Delete(tmpInputFile);
					}


					var iconPath = GetBackTranslationIconPath();
					var newStudioProject = _projectAutomationService.CreateBackTranslationProject(studioProject, iconPath, sourceFiles, "BT");
					var newStudioProjectInfo = newStudioProject.GetProjectInfo();


					//var backTranslateProject = _projectAutomationService.GetProject(newStudioProject, null);
					//backTranslateProject.ProjectFiles.RemoveAll(a => a.TargetLanguage == backTranslateProject.SourceLanguage.CultureInfo.Name);


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
					taskContext.ProjectFiles = workingProject.ProjectFiles;


					//TODO inject the back-translations


					//TODO create backproject
					//project.BackTranslationProjects.Add(backTranslateProject as BackTranslationProject);

					taskContext.Completed = true;

					_controllers.TranscreateController.UpdateBackTranslationProjectData(project as Project, taskContext);
				}



				


			}
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

			_controllers.TranscreateController.ProjectFileSelectionChanged += ProjectsController_SelectedProjectsChanged;

			SetEnabled();
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

		private void ProjectsController_SelectedProjectsChanged(object sender, ProjectFileSelectionChangedEventArgs e)
		{
			SetEnabled();
		}

		private void SetEnabled()
		{
			var projects = _controllers.TranscreateController.GetSelectedProjects();
			if (projects == null)
			{
				Enabled = false;
				return;
			}

			Enabled = projects.Count == 1 && !(projects[0] is BackTranslationProject);
		}
	}
}
