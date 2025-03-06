using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.FileType.Settings.Entities;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Providers.OpenXml;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;
using Multilingual.Excel.FileType.Services.Entities;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class BilingualWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware, ISettingsAware, ISubContentAware
	{
		private IPersistentFileConversionProperties _originalFileProperties;

		private INativeOutputFileProperties _nativeFileProperties;

		private LanguageMapping _sourceLanguageMapping;

		private LanguageMapping _targetLanguageMapping;

		private SegmentVisitor _segmentVisitor;

		private IDocumentProperties _documentInfo;

		private UniqueEntitySettings _entitySettings;

		private readonly bool _isPreview;

		private readonly bool _isSource;

		private readonly SegmentBuilder _segmentBuilder;

		private readonly Queue<IParagraphUnit> _embeddedContentPositionMarkerParagraph;

		private readonly EntityContext _entityContext;

		private readonly EntityService _entityService;

		private readonly ExcelReader _excelReader;

		private readonly ExcelWriter _excelWriter;

		private List<ExcelSheet> _excelSheets;

		private int _excelSheetIndex;
		private string _excelSheetName;
		private uint _excelRowIndex;
		private bool _isCDATA;

		public BilingualWriter(SegmentBuilder segmentBuilder, EntityContext entityContext, EntityService entityService, ExcelReader excelReader, ExcelWriter excelWriter,
			bool isPreview = false, bool isSource = false)
		{
			_segmentBuilder = segmentBuilder;
			_entityContext = entityContext;
			_entityService = entityService;
			_excelReader = excelReader;
			_excelWriter = excelWriter;

			_isPreview = isPreview;
			_isSource = isSource;

			_embeddedContentPositionMarkerParagraph = new Queue<IParagraphUnit>();
		}

		public LanguageMappingSettings LanguageMappingSettings { get; private set; }

		public EmbeddedContentSettings EmbeddedContentSettings { get; private set; }

		public PlaceholderPatternSettings PlaceholderPatternSettings { get; private set; }

		public CommentMappingSettings CommentMappingSettings { get; private set; }

		private void LoadEntityMappings()
		{
			_entityContext.EnableEntityMappingsSetting = _entitySettings.ConvertEntities;
			_entityContext.ConvertNumericEntitiesToPlaceholder = _entitySettings.ConvertNumericEntityToPlaceholder;
			_entityContext.SkipConversionInsideLockedContent = _entitySettings.SkipConversionInsideLockedContent;

			foreach (var entitySet in _entitySettings.EntitySets.Values)
			{
				foreach (var entityMapping in entitySet.Mappings)
				{
					_entityContext.AddSettingsEntity(entityMapping);
				}
			}
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_documentInfo = documentInfo;
			_segmentVisitor = new SegmentVisitor(_entityService, false);
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			_originalFileProperties = fileProperties;
		}

		private string GetProjectRoot(string languageFilePath, string languageId)
		{
			var languageParam = "\\" + languageId + "\\";
			var projectRootPath = languageFilePath.Substring(0, languageFilePath.LastIndexOf(languageParam, StringComparison.InvariantCultureIgnoreCase));
			return projectRootPath;
		}

		private string GetOriginalFilePath(IPersistentFileConversionProperties fileInfo)
		{
			var filePath = fileInfo.OriginalFilePath;
			if (!File.Exists(filePath))
			{
				var languageParam = "\\" + fileInfo.SourceLanguage.IsoAbbreviation + "\\";
				var relativeFilePath =
					filePath.Substring(filePath.LastIndexOf(languageParam,
						StringComparison.InvariantCultureIgnoreCase));

				var projectRootPath = GetProjectRoot(_documentInfo.LastOpenedAsPath, _documentInfo.TargetLanguage.IsoAbbreviation);
				filePath = Path.Combine(projectRootPath, relativeFilePath.TrimStart('\\'));
			}

			return filePath;
		}

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			LanguageMappingSettings = new LanguageMappingSettings();
			LanguageMappingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			EmbeddedContentSettings = new EmbeddedContentSettings();
			EmbeddedContentSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			PlaceholderPatternSettings = new PlaceholderPatternSettings();
			PlaceholderPatternSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			CommentMappingSettings = new CommentMappingSettings();
			CommentMappingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			_entitySettings = new UniqueEntitySettings();
			_entitySettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			LoadEntityMappings();

			var sourceLanguageId = _documentInfo.SourceLanguage?.CultureInfo?.Name;
			var targetLanguageId = _documentInfo.TargetLanguage?.CultureInfo?.Name;

			_sourceLanguageMapping = LanguageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, sourceLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);
			_targetLanguageMapping = LanguageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, targetLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (_sourceLanguageMapping == null)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, sourceLanguageId));
			}

			if (_targetLanguageMapping == null)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, targetLanguageId));
			}

			var excelOptions = new ExcelOptions
			{
				ReadAllWorkSheets = LanguageMappingSettings.LanguageMappingReadAllWorkSheets,
				FirstRowIndex = LanguageMappingSettings.LanguageMappingFirstRowIndex,
				FirstRowIndexIsHeading = LanguageMappingSettings.LanguageMappingFirstRowIsHeading,
				ReadHyperlinks = LanguageMappingSettings.LanguageMappingReadHyperlinks,
			};
			_excelSheets = new List<ExcelSheet>();


			var originalFilePath = GetOriginalFilePath(_originalFileProperties);
			var excelColumns = GetExcelColumns();
			_excelSheets = _excelReader.GetExcelSheets(originalFilePath, excelOptions, excelColumns);
		}


		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_nativeFileProperties = properties;
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_originalFileProperties = fileInfo.FileConversionProperties;
			_originalFileProperties.OriginalEncoding = new Codepage(Encoding.UTF8);
			_originalFileProperties.OriginalFilePath = GetOriginalFilePath(_originalFileProperties);
		}

		private List<ExcelColumn> GetExcelColumns()
		{
			var excelColumns = new List<ExcelColumn>();
			if (!string.IsNullOrEmpty(_sourceLanguageMapping.ContentColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _sourceLanguageMapping.ContentColumn
				});
			}

			if (!string.IsNullOrEmpty(_targetLanguageMapping.ContentColumn))
			{
				excelColumns.Add(new ExcelColumn
				{
					Name = _targetLanguageMapping.ContentColumn
				});
			}

			return excelColumns;
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var multilingualParagraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a =>
					a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
			if (multilingualParagraphUnitContext != null)
			{
				_excelSheetIndex = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelSheetIndex));
				_excelSheetName = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelSheetName);
				_excelRowIndex = Convert.ToUInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelRowIndex));
				_isCDATA = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.IsCDATA));
			}

			if (!paragraphUnit.IsStructure)
			{
				CreateParagraphUnit(paragraphUnit);
			}
			else
			{
				_embeddedContentPositionMarkerParagraph.Enqueue(paragraphUnit);
			}
		}

		public void AddSubContent(Stream subContentStream)
		{
			if (_embeddedContentPositionMarkerParagraph.Count == 0)
			{
				return;
			}

			var structureParagraph = _embeddedContentPositionMarkerParagraph.Dequeue();
			var targetSubContent = GetTargetSubContent(subContentStream);

			var excelSheet = _excelSheets.FirstOrDefault(a => a.Index == _excelSheetIndex);
			var excelRow = excelSheet?.Rows.FirstOrDefault(a => a.Index == _excelRowIndex);
			if (excelRow != null)
			{
				//var sourceContent = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.ContentColumn);
				var targetContent = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _targetLanguageMapping.ContentColumn);

				//var escapedInnerXml = _entityService.ConvertKnownCharactersToEntities(targetContent.Value);

				targetContent.Value = _isCDATA ? string.Format(XmlConstants.CdataFormat, targetSubContent) : targetSubContent;
			}
		}

		private XmlNode GetXmlNodeFromContent(string content)
		{
			if (string.IsNullOrEmpty(content))
			{
				return null;
			}

			var cellContent = _entityService.MarkupKnownEntities(content);
			cellContent = _entityService.EscapeXmlCharacters(cellContent);

			try
			{
				var xmlNode = GetXmlDocumentRootNode(cellContent);
				if (xmlNode != null)
				{
					return xmlNode;
				}

				// attempt to markup html self closing tags (e.g. br) + reprocess
				cellContent = _entityService.MarkupSelfClosingHtmlTags(cellContent);

				return GetXmlDocumentRootNode(cellContent);
			}
			catch (Exception ex)
			{
				// catch all; ignore
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}

			return null;
		}

		private static XmlNode GetXmlDocumentRootNode(string content)
		{
			try
			{
				var xmlContent = new XmlDocument();
				xmlContent.LoadXml("<ExcelRoot>" + content + "</ExcelRoot>");

				return xmlContent.FirstChild;
			}
			catch (Exception ex)
			{
				// catch all; ignore
				System.Diagnostics.Trace.WriteLine(ex.Message);
			}

			return null;
		}

		private ISegment GetSegment(string content, ISegmentPairProperties segmentPairProperties)
		{
			var xmlNode = GetXmlNodeFromContent(content);
			var segment = xmlNode == null
				? _segmentBuilder.CreateSegment(content, segmentPairProperties)
				: _segmentBuilder.CreateSegment(xmlNode, segmentPairProperties, SegmentationHint.MayExclude);

			return segment;
		}

		private string GetTargetSubContent(Stream subContentStream)
		{
			string subContent;
			using (var reader = new StreamReader(subContentStream, Encoding.UTF8))
			{
				subContent = reader.ReadToEnd();
			}

			var segmentProperties = _segmentBuilder.CreateSegmentPairProperties();
			var segment = GetSegment(subContent, segmentProperties);
			_segmentVisitor.VisitSegment(segment);

			var content = _entityService.EntityMarkerConversionService.BackwardEntityMarkersConversion(_segmentVisitor.Text);

			return content;
		}

		private void CreateParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			// prepare the target segment for output format.
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				_segmentVisitor.VisitSegment(segmentPair.Target);
				segmentPair.Target.Clear();
				segmentPair.Target.Add(_segmentBuilder.Text(_segmentVisitor.Text));
			}


			var excelSheet = _excelSheets.FirstOrDefault(a => a.Index == _excelSheetIndex);
			var excelRow = excelSheet?.Rows.FirstOrDefault(a => a.Index == _excelRowIndex);
			if (excelRow != null)
			{
				var multilingualParagraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts;
				var multilingualParagraphUnitStructureInfo = paragraphUnit.Properties.Contexts?.StructureInfo?.ContextInfo;

				var hyperlink = multilingualParagraphUnitContext?.FirstOrDefault(a => a.ContextType == "sdl:hyperlink");
				var hyperlinkDataType = hyperlink?.GetMetaData("HyperlinkDataType");
				var hyperlinkId = hyperlink?.GetMetaData("HyperlinkId");
				var hyperlinkLocation = hyperlink?.GetMetaData("HyperlinkLocation");
				var hyperlinkReference = _targetLanguageMapping.ContentColumn + excelRow.Index;
				var hyperlinkIsExternal = hyperlink?.GetMetaData("HyperlinkIsExternal");
				var hyperlinkDisplay = hyperlink?.GetMetaData("HyperlinkDisplay");

				var sourceContent = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _sourceLanguageMapping.ContentColumn);
				var targetContent = excelRow.Cells.FirstOrDefault(a => a.Column.Name == _targetLanguageMapping.ContentColumn);

				if (targetContent == null)
				{
					return;
				}

				if (hyperlink != null && !string.IsNullOrEmpty(hyperlinkDataType))
				{
					if (targetContent.Hyperlink == null )
					{
						targetContent.Hyperlink = sourceContent?.Hyperlink.Clone() as Hyperlink ?? new Hyperlink();
					}

					targetContent.Hyperlink.Id = hyperlinkId;
					targetContent.Hyperlink.Location = hyperlinkLocation;
					targetContent.Hyperlink.Reference = hyperlinkReference;
					targetContent.Hyperlink.IsExternal = !string.IsNullOrEmpty(hyperlinkIsExternal) && Convert.ToBoolean(hyperlinkIsExternal);
					targetContent.Hyperlink.Display = hyperlinkDisplay;

					switch (hyperlinkDataType)
					{
						case nameof(targetContent.Hyperlink.Url):
							targetContent.Hyperlink.Url = paragraphUnit.Target.ToString();
							break;
						case nameof(targetContent.Hyperlink.Tooltip):
							targetContent.Hyperlink.Tooltip = paragraphUnit.Target.ToString();
							break;
						case nameof(targetContent.Hyperlink.Email):
							targetContent.Hyperlink.Url = targetContent.Hyperlink.Url.Replace(targetContent.Hyperlink.Email,
								paragraphUnit.Target.ToString());
							targetContent.Hyperlink.Email = paragraphUnit.Target.ToString();
							break;
						case nameof(targetContent.Hyperlink.Subject):
							targetContent.Hyperlink.Url = targetContent.Hyperlink.Url.Replace(targetContent.Hyperlink.Subject,
								paragraphUnit.Target.ToString());
							targetContent.Hyperlink.Subject = paragraphUnit.Target.ToString();
							break;
					}
				}
				else
				{
					targetContent.Value = paragraphUnit.Target.ToString();
				}
			}
		}

		private string GetMarkupText(IParagraphUnit paragraphUnit, ISegmentPair segmentPair, IEnumerable<Element> elements)
		{
			var content = string.Empty;
			foreach (var element in elements)
			{
				if (element is ElementText text)
				{
					var markup = string.Format(XmlConstants.CdataFormat, "<" + FiletypeConstants.MultilingualSegment + " "
								 + "pid=\"" + paragraphUnit.Properties.ParagraphUnitId.Id + "\" sid=\"" + segmentPair.Properties.Id.Id + "\">");
					markup += text.Text;
					markup += string.Format(XmlConstants.CdataFormat, "</" + FiletypeConstants.MultilingualSegment + ">");

					content += markup;
				}

				if (element is ElementTagPair tagPair)
				{
					content += tagPair.TagContent;
				}

				if (element is ElementPlaceholder placeholder)
				{
					content += placeholder.TagContent;
				}
			}

			return content;
		}

		public void FileComplete()
		{
			var filePath = _nativeFileProperties.OutputFilePath;
			var originalFilePath = _originalFileProperties.OriginalFilePath;
			File.Copy(originalFilePath, filePath);

			var excelOptions = new ExcelOptions
			{
				ReadAllWorkSheets = LanguageMappingSettings.LanguageMappingReadAllWorkSheets,
				FirstRowIndex = LanguageMappingSettings.LanguageMappingFirstRowIndex,
				FirstRowIndexIsHeading = LanguageMappingSettings.LanguageMappingFirstRowIsHeading,
				ReadHyperlinks = LanguageMappingSettings.LanguageMappingReadHyperlinks,
			};

			var excelColumns = GetExcelColumns();
			_excelWriter.UpdateExcelSheets(filePath, _excelSheets, excelOptions, excelColumns);
		}

		public void Complete()
		{
		}

		public void Dispose()
		{
		}
	}
}
