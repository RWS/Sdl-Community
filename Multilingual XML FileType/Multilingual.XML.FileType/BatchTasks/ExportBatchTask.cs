using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Multilingual.XML.FileType.BatchTasks.Pages;
using Multilingual.XML.FileType.BatchTasks.Settings;
using Multilingual.XML.FileType.Extensions;
using Multilingual.XML.FileType.FileType.Processors;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using MessageLevel = Sdl.ProjectAutomation.Core.MessageLevel;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Multilingual.XML.FileType.BatchTasks
{
	[AutomaticTask(Id = Constants.ExportBatchTaskId,
		Name = "MultilingualXMLFileType_ExportBatchTask_Name",
		Description = "MultilingualXMLFileType_ExportBatchTask_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(MultilingualXmlExportSettings), typeof(ExportSettingsPage))]
	public class ExportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private readonly object _lockObject = new object();
		private LanguageMappingSettings _languageMappingSettings;
		//private WriterSettings _writerSettings;
		private List<MultilingualFileInfo> _multilingualFileInfos;
		private DefaultNamespaceHelper _defaultNamespaceHelper;
		private XmlReaderFactory _xmlReaderFactory;
		private AlternativeInputFileGenerator _alternativeInputFileGenerator;
		private EntityMarkerConversionService _entityMarkerConversionService;
		private SegmentBuilder _segmentBuilder;
		private ImageService _imageService;

		protected override void OnInitializeTask()
		{
			var settings = Project.GetSettings();

			_entityMarkerConversionService = new EntityMarkerConversionService();

			_imageService = new ImageService();

			var fileSystemService = new FileSystemService();
			_alternativeInputFileGenerator = new AlternativeInputFileGenerator(fileSystemService);

			_languageMappingSettings = new LanguageMappingSettings();
			_languageMappingSettings.PopulateFromSettingsBundle(settings, Constants.FileTypeDefinitionId);

			//_writerSettings = new WriterSettings();
			//_writerSettings.PopulateFromSettingsBundle(settings, Constants.FileTypeDefinitionId);

			_multilingualFileInfos = new List<MultilingualFileInfo>();

			_xmlReaderFactory = new XmlReaderFactory();
			_defaultNamespaceHelper = new DefaultNamespaceHelper(_xmlReaderFactory);

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

				var task = Project.RunAutomaticTask(fileIds.ToArray(),
					AutomaticTaskTemplateIds.GenerateTargetTranslations);

				if (task != null && (task.Status != TaskStatus.Completed ||
									 task.Messages.ToList().Exists(a => a.Level == MessageLevel.Error)))
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
			if (_languageMappingSettings.LanguageMappingMonoLanguage)
			{
				var message = string.Format(PluginResources.ExceptionMessage_Unable_To_Generate_Multilingual_From_Monolingual_File);
				throw new Exception(message);
			}

			var valid = projectFile.FileTypeId == Constants.FileTypeDefinitionId;
			if (!valid)
			{
				var message = string.Format(PluginResources.ExceptionMessage_Incorrect_File_Type,
					projectFile.FileTypeId, Constants.FileTypeDefinitionId);
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
				throw new Exception(string.Format("Unable to locate the source file {0}", originalFilePath));
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
				if (_languageMappingSettings.LanguageMappingMonoLanguage && !string.IsNullOrEmpty(targetLanguageId))
				{
					targetLanguage = new LanguageMapping
					{
						LanguageId = targetLanguageId,
						XPath = projectFile.Language?.CultureInfo?.Name,
						DisplayName = new CultureInfo(targetLanguageId).DisplayName,
						Image = _imageService.GetImage(targetLanguageId)
					};
					_languageMappingSettings.LanguageMappingLanguages.Add(targetLanguage);
				}
				else
				{
					throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, targetLanguageId));
				}
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
			lock (_lockObject)
			{
				foreach (var multilingualFileInfo in _multilingualFileInfos)
				{
					var settings = Project.GetSettings();
					var languageMappingSettings = new LanguageMappingSettings();
					languageMappingSettings.PopulateFromSettingsBundle(settings, Constants.FileTypeDefinitionId);


					var tempOutputFilePath = _alternativeInputFileGenerator.GenerateTempFileWithHiddenEntities(multilingualFileInfo.OutputFilePath, multilingualFileInfo.FileEncoding);

					var outputDocument = new XmlDocument();
					var outputFileStream = File.Open(tempOutputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
					var outputXmlReader = _xmlReaderFactory.CreateReader(outputFileStream, multilingualFileInfo.FileEncoding, true);
					outputDocument.Load(outputXmlReader);

					var nsmgr = new XmlNamespaceManager(outputDocument.NameTable);
					outputDocument.AddAllNamespaces(nsmgr);

					var namespaceUri = _defaultNamespaceHelper.GetXmlNameSpaceUri(
						multilingualFileInfo.MultilingualFilePath, multilingualFileInfo.FileEncoding);
					if (!string.IsNullOrEmpty(namespaceUri))
					{
						var defaultNameSpace = new XmlNameSpace { Name = Constants.DefaultNamespace, Value = namespaceUri };
						_defaultNamespaceHelper.AddXmlNameSpacesFromDocument(nsmgr, outputDocument, defaultNameSpace);

						languageMappingSettings.LanguageMappingLanguagesXPath =
							_defaultNamespaceHelper.UpdateXPathWithNamespace(
								languageMappingSettings.LanguageMappingLanguagesXPath,
								Constants.DefaultNamespace);

						foreach (var mappingLanguage in languageMappingSettings.LanguageMappingLanguages)
						{
							mappingLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
								mappingLanguage.XPath, Constants.DefaultNamespace);

							mappingLanguage.CommentXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
								mappingLanguage.CommentXPath, Constants.DefaultNamespace);
						}

						multilingualFileInfo.SourceLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(multilingualFileInfo.SourceLanguage.XPath, Constants.DefaultNamespace);
						multilingualFileInfo.TargetLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(multilingualFileInfo.TargetLanguage.XPath, Constants.DefaultNamespace);
					}

					var outputNodes = outputDocument.SelectNodes(languageMappingSettings.LanguageMappingLanguagesXPath, nsmgr);

					var tempMultilingualFilePath = _alternativeInputFileGenerator.GenerateTempFileWithHiddenEntities(multilingualFileInfo.MultilingualFilePath, multilingualFileInfo.FileEncoding);

					var multilingualDocument = new XmlDocument();
					var multilingualFileStream = File.Open(tempMultilingualFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
					var multilingualXmlReader = _xmlReaderFactory.CreateReader(multilingualFileStream, multilingualFileInfo.FileEncoding, true);
					multilingualDocument.Load(multilingualXmlReader);

					var multilingualNodes = multilingualDocument.SelectNodes(languageMappingSettings.LanguageMappingLanguagesXPath, nsmgr);
					var xmlNodeService = new XmlNodeService(multilingualDocument, nsmgr);

					if (outputNodes == null && multilingualNodes == null)
					{
						continue;
					}

					if (outputNodes?.Count != multilingualNodes?.Count)
					{
						throw new Exception(string.Format(
							"The paragraph units are not aligned!" + Environment.NewLine + Environment.NewLine
							+ "Bilingual File: '{0}', Units: {1}" + Environment.NewLine
							+ "Multilingual File: '{0}', Units: {1}" + Environment.NewLine
							+ "" + Environment.NewLine, Path.GetFileName(multilingualFileInfo.OutputFilePath),
							outputNodes?.Count,
							Path.GetFileName(multilingualFileInfo.MultilingualFilePath),
							multilingualNodes?.Count));
					}

					var multilingualNodeList = multilingualNodes.Cast<XmlNode>().ToList();
					var outputNodeList = outputNodes.Cast<XmlNode>().ToList();

					for (var i = 0; i < multilingualNodeList.Count; i++)
					{
						var outputNode = outputNodeList[i];
						var multilingualNode = multilingualNodeList[i];


						var outputlanguagePath = languageMappingSettings.LanguageMappingMonoLanguage
							? multilingualFileInfo.SourceLanguage.XPath
							: multilingualFileInfo.TargetLanguage.XPath;


						var outputXmlNode = outputNode.SelectSingleNode(outputlanguagePath, nsmgr);
						var multilingualXmlNode = multilingualNode.SelectSingleNode(outputlanguagePath, nsmgr);

						if (multilingualXmlNode == null)
						{
							multilingualXmlNode = xmlNodeService.AddXmlNode(multilingualNode, outputlanguagePath);
						}

						if (multilingualXmlNode != null && outputXmlNode != null)
						{
							multilingualXmlNode.InnerXml = outputXmlNode.InnerXml;
						}
					}

					outputXmlReader.Close();
					outputXmlReader.Dispose();

					outputFileStream.Close();
					outputFileStream.Dispose();

					multilingualXmlReader.Close();
					multilingualXmlReader.Dispose();

					multilingualFileStream.Close();
					multilingualFileStream.Dispose();


					multilingualDocument.Save(multilingualFileInfo.MultilingualFilePath);

					var sbOutput = new StringBuilder();
					using (var reader = new StreamReader(multilingualFileInfo.MultilingualFilePath,
						multilingualFileInfo.FileEncoding))
					{
						sbOutput.Append(reader.ReadToEnd());
					}

					sbOutput = new StringBuilder(
						_entityMarkerConversionService.BackwardEntityMarkersConversion(sbOutput.ToString()));

					using (var writer = new StreamWriter(multilingualFileInfo.MultilingualFilePath, false,
						multilingualFileInfo.FileEncoding))
					{
						writer.Write(sbOutput);
						writer.Flush();
					}
				}
			}

			base.TaskComplete();
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
				languageFilePath.IndexOf("\\" + languageId + "\\", StringComparison.InvariantCultureIgnoreCase));

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
