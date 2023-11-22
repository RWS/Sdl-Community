using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Multilingual.Excel.FileType.BatchTasks.Pages;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Processors;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Services;
using Multilingual.Excel.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using MessageLevel = Sdl.ProjectAutomation.Core.MessageLevel;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Multilingual.Excel.FileType.BatchTasks
{
	[AutomaticTask(Id = FiletypeConstants.ExportBatchTaskId,
		Name = "MultilingualExcelFileType_ExportBatchTask_Name",
		Description = "MultilingualExcelFileType_ExportBatchTask_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(MultilingualExcelExportSettings), typeof(ExportSettingsPage))]
	public class ExportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private MultilingualExcelExportSettings _settings;
		private ExcelReader _excelReader;
		private ExcelWriter _excelWriter;
		private LanguageMappingSettings _languageMappingSettings;
		private List<MultilingualFileInfo> _multilingualFileInfos;
		private SegmentBuilder _segmentBuilder;

		protected override void OnInitializeTask()
		{
			var settings = Project.GetSettings();

			var colorService = new ColorService();
			_excelReader = new ExcelReader(colorService);
			_excelWriter = new ExcelWriter();

			_languageMappingSettings = new LanguageMappingSettings();
			_languageMappingSettings.PopulateFromSettingsBundle(settings, FiletypeConstants.FileTypeDefinitionId);

			_settings = GetSetting<MultilingualExcelExportSettings>();
			_multilingualFileInfos = new List<MultilingualFileInfo>();

			var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			var entityContext = new EntityContext();
			var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
			var entityMarkerConversionService = new EntityMarkerConversionService();
			var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

			_segmentBuilder = new SegmentBuilder(entityService);

			base.OnInitializeTask();
		}

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var fileIds = new List<Guid> { projectFile.Id };

			try
			{
				var sdlxliffUpdaterProtectEmptyTarget = new SdlxliffUpdater(new EmptyTargetProcessor(_segmentBuilder, true));
				UpdateFile(projectFile, sdlxliffUpdaterProtectEmptyTarget);

				var task = Project.RunAutomaticTask(fileIds.ToArray(), AutomaticTaskTemplateIds.GenerateTargetTranslations);

				if (task != null && (task.Status != TaskStatus.Completed || task.Messages.ToList().Exists(a => a.Level == MessageLevel.Error)))
				{
					var exception = task.Messages.FirstOrDefault(a => a.Level == MessageLevel.Error)?.Exception;
					if (exception != null)
					{
						throw exception;
					}
				}
			}
			finally
			{
				var sdlxliffUpdaterUnProtectEmptyTarget = new SdlxliffUpdater(new EmptyTargetProcessor(_segmentBuilder, false));
				UpdateFile(projectFile, sdlxliffUpdaterUnProtectEmptyTarget);
			}
		}

		private static void UpdateFile(ProjectFile projectFile, SdlxliffUpdater sdlxliffUpdater)
		{
			var updatedFilePath = Path.GetTempFileName();
			sdlxliffUpdater.UpdateFile(projectFile.LocalFilePath, updatedFilePath);
			File.Copy(updatedFilePath, projectFile.LocalFilePath, true);
			if (File.Exists(updatedFilePath))
			{
				File.Delete(updatedFilePath);
			}
		}

		public override bool ShouldProcessFile(ProjectFile projectFile)
		{
			var valid = projectFile.FileTypeId == FiletypeConstants.FileTypeDefinitionId;
			if (!valid)
			{
				var message = string.Format(PluginResources.ExceptionMessage_Incorrect_File_Type,
					projectFile.FileTypeId, FiletypeConstants.FileTypeDefinitionId);
				throw new Exception(message);
			}

			return true;
		}

		public override bool OnFileComplete(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			var originalFilePath = Path.Combine(Path.GetDirectoryName(projectFile.SourceFile.LocalFilePath) ?? string.Empty,
				projectFile.SourceFile.OriginalName);
			if (!File.Exists(originalFilePath))
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheSourceFile_, originalFilePath));
			}

			var outputFilePath = Path.Combine(Path.GetDirectoryName(projectFile.LocalFilePath) ?? string.Empty,
				projectFile.OriginalName);

			var sourceLanguageId = projectFile.SourceFile?.Language?.CultureInfo?.Name;
			var targetLanguageId = projectFile.Language?.CultureInfo?.Name;

			var sourceLanguage = _languageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, sourceLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);
			var targetLanguage = _languageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, targetLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (sourceLanguage == null)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, sourceLanguageId));
			}

			if (targetLanguage == null)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, targetLanguageId));
			}

			var extractor = multiFileConverter.Extractors.FirstOrDefault();
			var sniffInfo = extractor?.FileConversionProperties.FileSnifferInfo;
			var encoding = sniffInfo?.DetectedEncoding.First.Encoding ?? Encoding.UTF8;

			var multilingualFilePath = GetMultiLingualFilePath(originalFilePath, sourceLanguage);

			_multilingualFileInfos.Add(new MultilingualFileInfo
			{
				OriginalFilePath = originalFilePath,
				OutputFilePath = outputFilePath,
				MultilingualFilePath = multilingualFilePath,
				SourceLanguage = sourceLanguage,
				TargetLanguage = targetLanguage,
				FileEncoding = encoding
			});

			return base.OnFileComplete(projectFile, multiFileConverter);
		}

		public override void TaskComplete()
		{
			foreach (var multilingualFileInfo in _multilingualFileInfos)
			{
				var settings = Project.GetSettings();
				var languageMappingSettings = new LanguageMappingSettings();
				languageMappingSettings.PopulateFromSettingsBundle(settings, FiletypeConstants.FileTypeDefinitionId);
				var excelOptions = new ExcelOptions
				{
					ReadAllWorkSheets = languageMappingSettings.LanguageMappingReadAllWorkSheets,
					FirstRowIndex = languageMappingSettings.LanguageMappingFirstRowIndex,
					FirstRowIndexIsHeading = languageMappingSettings.LanguageMappingFirstRowIsHeading
				};

				var excelColumns = GetExcelColumns(multilingualFileInfo);
				var excelSheets = _excelReader.GetExcelSheets(multilingualFileInfo.OutputFilePath, excelOptions, excelColumns);

				_excelWriter.UpdateExcelSheets(multilingualFileInfo.MultilingualFilePath, excelSheets, excelOptions, excelColumns);
			}
		}

		private List<ExcelColumn> GetExcelColumns(MultilingualFileInfo multilingualFileInfo)
		{
			var excelColumns = new List<ExcelColumn>
			{
				new ExcelColumn
				{
					Name = multilingualFileInfo.TargetLanguage.ContentColumn
				}
			};

			return excelColumns;
		}

		private string GetMultiLingualFilePath(string originalFilePath, LanguageMapping sourceLanguage)
		{
			var projectPath = GetProjectRootPath(
				originalFilePath, sourceLanguage.LanguageId, out var fileLocalPath);

			var multilingualDirectory = Path.Combine(projectPath, "Multilingual");
			var multilingualFileDirectory = Path.Combine(multilingualDirectory, fileLocalPath);
			if (!Directory.Exists(multilingualFileDirectory))
			{
				Directory.CreateDirectory(multilingualFileDirectory);
			}

			var multilingualFilePath = Path.Combine(multilingualFileDirectory, Path.GetFileName(originalFilePath));

			if (!File.Exists(multilingualFilePath))
			{
				File.Copy(originalFilePath, multilingualFilePath);
			}

			return multilingualFilePath;
		}

		private string GetProjectRootPath(string languageFileFullPath, string languageId, out string fileLocalPath)
		{
			var languageFilePath = Path.GetDirectoryName(languageFileFullPath) + "\\";

			var projectPath = languageFilePath.Substring(0,
				languageFilePath.LastIndexOf("\\" + languageId + "\\", StringComparison.InvariantCultureIgnoreCase));

			var projectLanguagePath = Path.Combine(projectPath, languageId) + "\\";
			fileLocalPath = languageFilePath.Substring(projectLanguagePath.Length);

			return projectPath;
		}

		private List<Models.AnalysisBand> GetAnalysisBands(FileBasedProject project)
		{
			var regex = new Regex(@"(?<min>[\d]*)([^\d]*)(?<max>[\d]*)", RegexOptions.IgnoreCase);

			var analysisBands = new List<Models.AnalysisBand>();
			var type = project.GetType();
			var internalProjectField = type.GetField("_project", BindingFlags.NonPublic | BindingFlags.Instance);
			if (internalProjectField != null)
			{
				dynamic internalDynamicProject = internalProjectField.GetValue(project);
				foreach (var analysisBand in internalDynamicProject.AnalysisBands)
				{
					Match match = regex.Match(analysisBand.ToString());
					if (match.Success)
					{
						var min = match.Groups["min"].Value;
						var max = match.Groups["max"].Value;
						analysisBands.Add(new Models.AnalysisBand
						{
							MinimumMatchValue = Convert.ToInt32(min),
							MaximumMatchValue = Convert.ToInt32(max)
						});
					}
				}
			}

			return analysisBands;
		}
	}
}
