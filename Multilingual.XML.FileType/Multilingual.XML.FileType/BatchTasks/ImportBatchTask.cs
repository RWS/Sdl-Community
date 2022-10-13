using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using Multilingual.XML.FileType.BatchTasks.Pages;
using Multilingual.XML.FileType.BatchTasks.Settings;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;

namespace Multilingual.XML.FileType.BatchTasks
{
	[AutomaticTask(Id = Constants.ImportBatchTaskId,
		Name = "MultilingualXMLFileType_ImportBatchTask_Name",
		Description = "MultilingualXMLFileType_ImportBatchTask_Description",
		GeneratedFileType = AutomaticTaskFileType.BilingualTarget, AllowMultiple = true)]
	[AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
	[RequiresSettings(typeof(MultilingualXmlImportSettings), typeof(ImportSettingsPage))]
	public class ImportBatchTask : AbstractFileContentProcessingAutomaticTask
	{
		private MultilingualXmlImportSettings _settings;
		private LanguageMappingSettings _languageMappingSettings;
		private XmlDocument _document;
		private IPropertiesFactory _propertiesFactory;
		private IDocumentItemFactory _itemFactory;
		private ProjectFile _projectFile;
		private SdlxliffReader _sdlxliffReader;
		private SdlxliffImporter _sdlxliffImporter;
		private SegmentBuilder _segmentBuilder;
		private IFileTypeManager _filetypeManager;
		private SegmentVisitor _segmentVisitor;

		protected override void OnInitializeTask()
		{
			var settings = Project.GetSettings();

			var analysisBands = GetAnalysisBands(Project as FileBasedProject);
			var filterItemService = new FilterItemService(analysisBands);

			var documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			var propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			var entityContext = new EntityContext();
			var sdlFrameworkService = new SdlFrameworkService(documentItemFactory, propertiesFactory);
			var entityMarkerConversionService = new EntityMarkerConversionService();
			var entityService = new EntityService(entityContext, sdlFrameworkService, entityMarkerConversionService);

			_segmentVisitor = new SegmentVisitor(entityService, true);
			_segmentBuilder = new SegmentBuilder(entityService);

			_sdlxliffReader = new SdlxliffReader();
			_sdlxliffImporter = new SdlxliffImporter(filterItemService, _segmentBuilder, _segmentVisitor);

			_languageMappingSettings = new LanguageMappingSettings();
			_languageMappingSettings.PopulateFromSettingsBundle(settings, Constants.FileTypeDefinitionId);

			_settings = GetSetting<MultilingualXmlImportSettings>();

			_filetypeManager = DefaultFileTypeManager.CreateInstance(true);
			_filetypeManager.SettingsBundle = Project.GetSettings();

			base.OnInitializeTask();
		}

		public override bool ShouldProcessFile(ProjectFile projectFile)
		{
			if (_languageMappingSettings.LanguageMappingMonoLanguage)
			{
				var message = string.Format(PluginResources.ExceptionMessage_Unable_To_Import_From_Monolingual_File);
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

		protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
		{
			_projectFile = projectFile;
			_itemFactory = multiFileConverter.ItemFactory;
			_propertiesFactory = multiFileConverter.PropertiesFactory;

			var originalFilePath = Path.Combine(Path.GetDirectoryName(projectFile.SourceFile.LocalFilePath) ?? string.Empty,
				projectFile.SourceFile.OriginalName);
			if (!File.Exists(originalFilePath))
			{
				throw new Exception(string.Format("Unable to locate the source file {0}", originalFilePath));
			}

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


			EnsureDocumentSegmented(projectFile);


			var dummyFile = Path.GetTempFileName();
			var dummySdlxliffFile = dummyFile + ".sdlxliff";
			File.Move(dummyFile, dummySdlxliffFile);

			var originalFilePathXml = originalFilePath + ".ImportFile.xml";
			File.Copy(originalFilePath, originalFilePathXml, true);

			var converter = _filetypeManager.GetConverterToDefaultBilingual(originalFilePathXml, dummySdlxliffFile, null);
			var extractor = converter.Extractors.FirstOrDefault();
			var bilingualParser = (BilingualParser)extractor?.BilingualParser;
			bilingualParser.DefaultSourceLanguage = projectFile.Language;
			converter.Parse();

			var sourceSegmentPairInfos = _sdlxliffReader.GetParagraphUnitInfos(projectFile.LocalFilePath);
			var sourceMultilingualParagraphUnits = GetMultilingualParagraphUnits(sourceSegmentPairInfos);

			var targetSegmentPairInfos = _sdlxliffReader.GetParagraphUnitInfos(dummySdlxliffFile);
			var targetMultilingualParagraphUnits = GetMultilingualParagraphUnits(targetSegmentPairInfos);

			if (File.Exists(originalFilePathXml))
			{
				File.Delete(originalFilePathXml);
			}

			if (File.Exists(dummySdlxliffFile))
			{
				File.Delete(dummySdlxliffFile);
			}

			var documentTagsSource = GetDocumentTags(sourceMultilingualParagraphUnits, true);
			var documentTagsTarget = GetDocumentTags(sourceMultilingualParagraphUnits, false);

			var segmentTagAligner = new SegmentTagAligner(documentTagsSource, documentTagsTarget);

			var updatedParagraphUnits = AlignParagraphUnits(sourceMultilingualParagraphUnits, targetMultilingualParagraphUnits);
			NormalizeTagIds(updatedParagraphUnits, segmentTagAligner);

			var updatedFilePath = Path.GetTempFileName();
			var importResult = _sdlxliffImporter.UpdateFile(updatedParagraphUnits, _settings, projectFile.LocalFilePath, updatedFilePath);

			File.Delete(importResult.FilePath);
			File.Move(importResult.UpdatedFilePath, importResult.FilePath);
		}


		/// <summary>
		/// The purpose of this procedure is to prepare the paragraph units before importing the
		/// translations from the multilingual structure; the paragraph units need to be segmented
		/// prior to the import procedures.
		/// 1. Check if segmentation marks exist
		/// 2. If no segmentation marks are found,
		///    2 a. Run an automated task to perform a pre-translation.
		///    Note: this is needed to ensure we recover the segmentation rules defined by the user
		///	   from the language resource context of the project.  If no language resources are found,
		///    then the default rules are used.    
		///    2 b. After pre-translation task, remove the reports that were generated for the process
		///    2 c. Remove any translations that were applied and reset the segment status to undefined. 
		/// </summary>
		/// <param name="projectFile"></param>
		private void EnsureDocumentSegmented(ProjectFile projectFile)
		{
			// 1. check if the file has segmentation markers
			var segmentPairInfos = _sdlxliffReader.GetParagraphUnitInfos(projectFile.LocalFilePath);
			var segmentationCount = segmentPairInfos.Sum(a => a.SegmentPairs.Count);

			if (segmentationCount == 0)
			{
				// 2 a. if the file is not segmented then run an automated translation action
				// Important: this is needed to ensure we pull in the translation resources that are
				// defined by the user; otherwise the default rules will be applied for that language.
				var task = Project.RunAutomaticTask(new Guid[] { projectFile.Id },
					AutomaticTaskTemplateIds.PreTranslateFiles);

				// 2 b. Disregard the reports that were automatically created and added to the
				// project by the automation task manager
				var projectReports = new ProjectReportsOperations(Project as FileBasedProject);
				projectReports.RemoveReports(task.Reports.Select(a => a.Id).ToList());

				// 2 c. Remove the translations and reset the segment status to not defined.
				var sdlxliffUpdater = new SdlxliffUpdater(new ClearTranslationsContentProcessor());
				UpdateFile(projectFile, sdlxliffUpdater);
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

		private List<IParagraphUnit> AlignParagraphUnits(IReadOnlyList<MultilingualParagraphUnit> originalMultilingualParagraphUnitInfos, IReadOnlyList<MultilingualParagraphUnit> updatedMultilingualParagraphUnitInfos)
		{
			var paragraphUnits = new List<IParagraphUnit>();

			if (originalMultilingualParagraphUnitInfos.Count != updatedMultilingualParagraphUnitInfos.Count)
			{
				throw new Exception("The source and target paragraph units are not aligned!");
			}

			for (var index = 0; index < originalMultilingualParagraphUnitInfos.Count; index++)
			{
				var originalMultilingualParagraphUnit = originalMultilingualParagraphUnitInfos[index];
				var targetMultilingualParagraphUnit = updatedMultilingualParagraphUnitInfos[index];

				var i = 0;
				foreach (var paragraphUnitInfo in originalMultilingualParagraphUnit.ParagraphUnitInfos)
				{
					var paragraphUnitProperties = paragraphUnitInfo.ParagraphUnit.Properties;
					var newParagraphUnit = _segmentBuilder.CreateParagraphUnit(paragraphUnitProperties?.Clone() as IParagraphUnitProperties);

					var updatedParagraphUnitInfo = new ParagraphUnitInfo();
					if ((i + 1) <= targetMultilingualParagraphUnit?.ParagraphUnitInfos?.Count)
					{
						updatedParagraphUnitInfo = targetMultilingualParagraphUnit.ParagraphUnitInfos[i];
					}
					else
					{
						Console.WriteLine("Empty target paragraph unit {0}", paragraphUnitInfo.ParagraphUnitId);
					}
					i++;


					if (paragraphUnitInfo.SegmentPairs.Count == 0 && updatedParagraphUnitInfo?.SegmentPairs?.Count == 0)
					{
						// Both source and target are NOT segmented!
						newParagraphUnit.Source = paragraphUnitInfo.ParagraphUnit.Source.Clone() as IParagraph;
						newParagraphUnit.Target = updatedParagraphUnitInfo.ParagraphUnit.Source.Clone() as IParagraph;

						paragraphUnits.Add(newParagraphUnit);

						continue;
					}

					if (paragraphUnitInfo.SegmentPairs.Count > 0 && updatedParagraphUnitInfo?.SegmentPairs?.Count == 0)
					{
						// Source is segmented, Target is NOT segmented
						// Create a segment for the target and populate it into the first segment pair container.  
						// Target segmentation is unknown at this point.
						// TODO: investigate if we can run pretranslation task reverting the language direction
						for (var m = 0; m < paragraphUnitInfo.SegmentPairs.Count; m++)
						{
							var originalSegmentPairInfo = paragraphUnitInfo.SegmentPairs[m];
							var segmentPairProperties = originalSegmentPairInfo.SegmentPair.Properties.Clone() as ISegmentPairProperties;
							var sourceSegment = _segmentBuilder.CreateSegment(segmentPairProperties);
							var targetSegment = sourceSegment.Clone() as ISegment;

							foreach (var item in originalSegmentPairInfo.SegmentPair.Source)
							{
								sourceSegment.Add(item.Clone() as IAbstractMarkupData);
							}

							if (m == 0)
							{
								foreach (var item in updatedParagraphUnitInfo.ParagraphUnit.Source)
								{
									targetSegment.Add(item.Clone() as IAbstractMarkupData);
								}
							}

							newParagraphUnit.Source.Add(sourceSegment);
							newParagraphUnit.Target.Add(targetSegment);
						}

						paragraphUnits.Add(newParagraphUnit);

						continue;
					}


					var j = 0;
					foreach (var originalSegmentPair in paragraphUnitInfo.SegmentPairs)
					{
						var updatedSegmentPairInfo = new SegmentPairInfo();
						if ((j + 1) <= updatedParagraphUnitInfo?.SegmentPairs?.Count)
						{
							updatedSegmentPairInfo = updatedParagraphUnitInfo.SegmentPairs[j];
						}
						else
						{
							Console.WriteLine("Empty target segment {0} for paragraph unit {1}",
								originalSegmentPair.SegmentId, paragraphUnitInfo.ParagraphUnitId);
						}

						j++;

						var segmentPairProperties = originalSegmentPair.SegmentPair.Properties.Clone() as ISegmentPairProperties;
						var sourceSegment = _segmentBuilder.CreateSegment(segmentPairProperties);
						var targetSegment = sourceSegment.Clone() as ISegment;

						foreach (var item in originalSegmentPair.SegmentPair.Source)
						{
							sourceSegment.Add(item.Clone() as IAbstractMarkupData);
						}

						if (updatedSegmentPairInfo.SegmentPair?.Source != null)
						{
							foreach (var item in updatedSegmentPairInfo.SegmentPair.Source)
							{
								targetSegment?.Add(item.Clone() as IAbstractMarkupData);
							}
						}

						newParagraphUnit.Source.Add(sourceSegment);
						newParagraphUnit.Target.Add(targetSegment);
					}

					if (updatedParagraphUnitInfo?.SegmentPairs?.Count > j)
					{
						// if the target paragraph has more segments, then append to the last segment.
						if (newParagraphUnit.Target.LastOrDefault() is ISegment targetSegment)
						{
							for (var y = j; y < updatedParagraphUnitInfo?.SegmentPairs?.Count; y++)
							{
								var updatedSegmentPairInfo = updatedParagraphUnitInfo.SegmentPairs[y];

								if (updatedSegmentPairInfo.SegmentPair?.Source == null)
								{
									continue;
								}

								foreach (var item in updatedSegmentPairInfo.SegmentPair.Source)
								{
									targetSegment.Add(item.Clone() as IAbstractMarkupData);
								}
							}
						}
					}

					paragraphUnits.Add(newParagraphUnit);
				}

				if (targetMultilingualParagraphUnit.ParagraphUnitInfos?.Count > i
					&& paragraphUnits.LastOrDefault() is IParagraphUnit paragraphUnit)
				{
					// if the target paragraph has more segments, then append to the last segment of the last paragraph
					if (!(paragraphUnit.Target?.LastOrDefault() is ISegment targetSegment))
					{
						continue;
					}

					for (var y = i; y < targetMultilingualParagraphUnit.ParagraphUnitInfos.Count; y++)
					{
						var updatedParagraphUnitInfo = targetMultilingualParagraphUnit.ParagraphUnitInfos[y];

						foreach (var updatedSegmentPairInfo in updatedParagraphUnitInfo.SegmentPairs)
						{
							if (updatedSegmentPairInfo.SegmentPair?.Source == null)
							{
								continue;
							}

							foreach (var item in updatedSegmentPairInfo.SegmentPair.Source)
							{
								targetSegment.Add(item.Clone() as IAbstractMarkupData);
							}
						}
					}
				}
			}


			return paragraphUnits;
		}

		private void UpdateSegments(IAbstractMarkupDataContainer container, IEnumerable<ISegmentPair> segmentPairs, bool isSource)
		{
			for (var index = 0; index < container.Count; index++)
			{
				var markupData = container[index];
				if (markupData is ISegment segment)
				{

					segment.Clear();
					var updatedSegmentPair = segmentPairs.FirstOrDefault(a => a.Properties.Id.Id == segment.Properties.Id.Id);

					var items = isSource ? updatedSegmentPair.Source : updatedSegmentPair.Target;
					foreach (var item in items)
					{
						segment.Add(item.Clone() as IAbstractMarkupData);
					}
				}
				else if (markupData is IAbstractMarkupDataContainer subContainer)
				{
					UpdateSegments(subContainer, segmentPairs, isSource);
				}
			}
		}

		private void NormalizeTagIds(IReadOnlyCollection<IParagraphUnit> paragraphUnits, SegmentTagAligner segmentTagAligner)
		{
			foreach (var paragraphUnit in paragraphUnits)
			{
				if (paragraphUnit.SegmentPairs.Any())
				{
					foreach (var segmentPair in paragraphUnit.SegmentPairs)
					{
						_segmentVisitor.VisitSegment(segmentPair.Source);

						var sourceTagPairElements = _segmentVisitor.TagPairElements;
						var sourcePlaceholderElements = _segmentVisitor.PlaceholderElements;
						var sourceSegmentTags = new ElementTags
						{
							TagPairElements = sourceTagPairElements,
							PlaceholderElements = sourcePlaceholderElements
						};

						segmentTagAligner.NormalizeTags(segmentPair.Target, sourceSegmentTags);
					}
				}
				else
				{
					segmentTagAligner.NormalizeTags(paragraphUnit.Target);
				}
			}
		}
		
		private List<IParagraphUnit> GetParagraphUnits(IEnumerable<ParagraphUnitInfo> paragraphUnitInfos)
		{
			var paragraphUnits = new List<IParagraphUnit>();

			foreach (var paragraphUnitInfo in paragraphUnitInfos)
			{
				var paragraphUnit = paragraphUnitInfo.ParagraphUnit;
				if (paragraphUnit != null)
				{
					paragraphUnits.Add(paragraphUnit);
				}
			}

			return paragraphUnits;
		}

		private ElementTags GetDocumentTags(IEnumerable<MultilingualParagraphUnit> multilingualParagraphUnits, bool isSource)
		{
			var documentTags = new ElementTags();
			if (multilingualParagraphUnits == null)
			{
				return documentTags;
			}


			foreach (var multilingualParagraphUnit in multilingualParagraphUnits)
			{
				foreach (var paragraphUnitInfo in multilingualParagraphUnit.ParagraphUnitInfos)
				{
					AddParentDocumentTags(isSource, paragraphUnitInfo, documentTags);

					if (paragraphUnitInfo.SegmentPairs.Any())
					{
						AddSegmentPairDocumentTags(isSource, paragraphUnitInfo, documentTags);
					}
					else
					{
						AddParagraphDocumentTags(isSource, paragraphUnitInfo, documentTags);
					}
				}
			}

			return documentTags;
		}

		private void AddSegmentPairDocumentTags(bool isSource, ParagraphUnitInfo paragraphUnitInfo, ElementTags documentTags)
		{
			foreach (var segmentPairInfo in paragraphUnitInfo.SegmentPairs)
			{
				_segmentVisitor.VisitSegment(isSource
					? segmentPairInfo.SegmentPair.Source
					: segmentPairInfo.SegmentPair.Target);
				UpdateDocumentTags(documentTags, _segmentVisitor.TagPairElements, _segmentVisitor.PlaceholderElements);
			}
		}

		private void AddParagraphDocumentTags(bool isSource, ParagraphUnitInfo paragraphUnitInfo, ElementTags documentTags)
		{
			// placeholders & tag pairs outside the scope of ISegmentPair, such as CDATA
			_segmentVisitor.InitializeComponents();
			var paragraph = isSource ? paragraphUnitInfo.ParagraphUnit.Source : paragraphUnitInfo.ParagraphUnit.Target;
			if (paragraph == null)
			{
				return;
			}

			foreach (var abstractMarkupData in paragraph.AllSubItems)
			{
				switch (abstractMarkupData)
				{
					case IPlaceholderTag placeholderTag:
						_segmentVisitor.VisitPlaceholderTag(placeholderTag);
						break;
					case ITagPair tagPair:
						_segmentVisitor.VisitTagPair(tagPair);
						break;
				}
			}

			UpdateDocumentTags(documentTags, _segmentVisitor.TagPairElements, _segmentVisitor.PlaceholderElements);
		}

		private void AddParentDocumentTags(bool isSource, ParagraphUnitInfo paragraphUnitInfo, ElementTags documentTags)
		{
			var tagPairElements = new List<ElementTagPair>();
			var placeholderElements = new List<ElementPlaceholder>();

			var paragraph = isSource ? paragraphUnitInfo.ParagraphUnit.Source : paragraphUnitInfo.ParagraphUnit.Target;
			if (paragraph == null)
			{
				return;
			}

			foreach (var abstractMarkupData in paragraph.AllSubItems)
			{
				switch (abstractMarkupData)
				{
					case IPlaceholderTag placeholderTag:
						{
							var placeholderElement = new ElementPlaceholder
							{
								TagId = placeholderTag.TagProperties.TagId.Id,
								TagContent = placeholderTag.TagProperties.TagContent,
								DisplayText = placeholderTag.TagProperties.DisplayText,
								TextEquivalent = placeholderTag.Properties.TextEquivalent
							};

							if (!placeholderElements.Exists(a => a.TagId == placeholderElement.TagId))
							{
								placeholderElements.Add(placeholderElement);
							}

							break;
						}

					case ITagPair tagPair:
						{
							var tagPairElement = new ElementTagPair
							{
								Type = Element.TagType.TagOpen,
								TagId = tagPair.TagProperties.TagId.Id,
								TagContent = tagPair.TagProperties.TagContent,
								DisplayText = tagPair.TagProperties.DisplayText
							};


							if (!tagPairElements.Exists(a => a.TagId == tagPairElement.TagId))
							{
								tagPairElements.Add(tagPairElement);
							}

							break;
						}
				}
			}

			UpdateDocumentTags(documentTags, tagPairElements, placeholderElements);
		}

		private void UpdateDocumentTags(ElementTags documentTags, List<ElementTagPair> tagPairElements, List<ElementPlaceholder> placeholderElements)
		{
			foreach (var element in tagPairElements.Where(element =>
				!documentTags.TagPairElements.Exists(a => a.TagId == element.TagId)))
			{
				documentTags.TagPairElements.Add(element.Clone() as ElementTagPair);
			}

			foreach (var element in placeholderElements.Where(element =>
				!documentTags.PlaceholderElements.Exists(a => a.TagId == element.TagId)))
			{
				documentTags.PlaceholderElements.Add(element.Clone() as ElementPlaceholder);
			}
		}


		private List<MultilingualParagraphUnit> GetMultilingualParagraphUnits(IEnumerable<ParagraphUnitInfo> paragraphUnitInfos)
		{
			var multilingualParagraphUnits = new List<MultilingualParagraphUnit>();

			foreach (var paragraphUnitInfo in paragraphUnitInfos)
			{
				var paragraphUnitIndex = paragraphUnitInfo?.ParagraphUnitIndex;

				var multilingualParagraphUnit = multilingualParagraphUnits.FirstOrDefault(a =>
					a.ParagraphUnitIndex == paragraphUnitIndex);

				if (multilingualParagraphUnit == null)
				{
					multilingualParagraphUnit = new MultilingualParagraphUnit
					{
						ParagraphUnitIndex = paragraphUnitIndex ?? -1,
						ParagraphUnitInfos = new List<ParagraphUnitInfo> { paragraphUnitInfo }
					};

					multilingualParagraphUnits.Add(multilingualParagraphUnit);
				}
				else
				{
					multilingualParagraphUnit.ParagraphUnitInfos.Add(paragraphUnitInfo);
				}
			}

			return multilingualParagraphUnits;
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
