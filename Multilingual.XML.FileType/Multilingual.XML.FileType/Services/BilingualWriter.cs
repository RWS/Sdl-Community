using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.Settings.Entities;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Multilingual.XML.FileType.Services
{
	public class BilingualWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware, ISettingsAware, ISubContentAware
	{
		private IPersistentFileConversionProperties _originalFileProperties;

		private INativeOutputFileProperties _nativeFileProperties;

		private XmlDocument _targetDocument;

		private SegmentVisitor _segmentVisitor;

		private LanguageMappingSettings _languageMappingSettings;

		//private WriterSettings _writerSettings;

		private LanguageMapping _sourceLanguageMapping;

		private LanguageMapping _targetLanguageMapping;

		private IDocumentProperties _documentInfo;

		private XmlNodeList _xmlNodes;

		private bool _isCDATA;

		private int _paragraphIndex;

		private XmlNamespaceManager _nsmgr;

		private XmlNodeService _xmlNodeService;

		private readonly DefaultNamespaceHelper _defaultNamespaceHelper;

		private readonly XmlReaderFactory _xmlReaderFactory;

		private UniqueEntitySettings _entitySettings;

		private readonly bool _isPreview;

		private readonly bool _isSource;

		private readonly SegmentBuilder _segmentBuilder;

		private readonly Queue<IParagraphUnit> _embeddedContentPositionMarkerParagraph;

		private readonly EntityContext _entityContext;

		private readonly EntityService _entityService;

		private readonly AlternativeInputFileGenerator _alternativeInputFileGenerator;

		private readonly FileSystemService _fileSystemService;

		private FileStream _fileStream;

		private XmlReader _xmlReader;

		public BilingualWriter(SegmentBuilder segmentBuilder, DefaultNamespaceHelper defaultNamespaceHelper, XmlReaderFactory xmlReaderFactory, 
			EntityContext entityContext, EntityService entityService,
			AlternativeInputFileGenerator alternativeInputFileGenerator, FileSystemService fileSystemService,
			bool isPreview = false, bool isSource = false)
		{
			_segmentBuilder = segmentBuilder;
			_defaultNamespaceHelper = defaultNamespaceHelper;
			_xmlReaderFactory = xmlReaderFactory;
			_entityContext = entityContext;
			_entityService = entityService;
			_alternativeInputFileGenerator = alternativeInputFileGenerator;
			_fileSystemService = fileSystemService;

			_isPreview = isPreview;
			_isSource = isSource;

			_embeddedContentPositionMarkerParagraph = new Queue<IParagraphUnit>();
		}

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

		public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
		{
			_languageMappingSettings = new LanguageMappingSettings();
			_languageMappingSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			//_writerSettings = new WriterSettings();
			//_writerSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);

			_entitySettings = new UniqueEntitySettings();
			_entitySettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
			LoadEntityMappings();
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			_nativeFileProperties = properties;
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_originalFileProperties = fileInfo.FileConversionProperties;

			var sourceLanguageId = _documentInfo.SourceLanguage?.CultureInfo?.Name;
			var targetLanguageId = _documentInfo.TargetLanguage?.CultureInfo?.Name;

			_sourceLanguageMapping = _languageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, sourceLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);
			_targetLanguageMapping = _languageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				string.Compare(a.LanguageId, targetLanguageId, StringComparison.InvariantCultureIgnoreCase) == 0);

			if (_sourceLanguageMapping == null)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, sourceLanguageId));
			}

			if (_targetLanguageMapping == null && !_languageMappingSettings.LanguageMappingMonoLanguage)
			{
				throw new Exception(string.Format(PluginResources.ExceptionMessage_UnableToLocateTheMappedLanguage_, targetLanguageId));
			}

			_targetDocument = new XmlDocument();
			var encoding = _originalFileProperties.FileSnifferInfo?.DetectedEncoding?.First?.Encoding ?? Encoding.UTF8;

			var originalFilePath = GetProjectRootPath(_originalFileProperties.OriginalFilePath);
			
			_fileStream = File.Open(originalFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
			_xmlReader = _xmlReaderFactory.CreateReader(_fileStream, encoding, true);

			_targetDocument.Load(_xmlReader);

			_nsmgr = new XmlNamespaceManager(_targetDocument.NameTable);

			if (_originalFileProperties.FileSnifferInfo != null && _originalFileProperties.FileSnifferInfo.MetaDataContainsKey(Constants.DefaultNamespace))
			{
				var namespaceUri = _originalFileProperties.FileSnifferInfo.GetMetaData(Constants.DefaultNamespace);
				if (!string.IsNullOrEmpty(namespaceUri))
				{
					var defaultNameSpace = new XmlNameSpace { Name = Constants.DefaultNamespace, Value = namespaceUri };
					_defaultNamespaceHelper.AddXmlNameSpacesFromDocument(_nsmgr, _targetDocument, defaultNameSpace);

					_languageMappingSettings.LanguageMappingLanguagesXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
						_languageMappingSettings.LanguageMappingLanguagesXPath, Constants.DefaultNamespace);

					foreach (var mappingLanguage in _languageMappingSettings.LanguageMappingLanguages)
					{
						mappingLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
							mappingLanguage.XPath, Constants.DefaultNamespace);

						mappingLanguage.CommentXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
							mappingLanguage.CommentXPath, Constants.DefaultNamespace);
					}
				}
			}

			_xmlNodeService = new XmlNodeService(_targetDocument, _nsmgr);
			_xmlNodes = _targetDocument.SelectNodes(_languageMappingSettings.LanguageMappingLanguagesXPath, _nsmgr);
		}

		private string GetProjectRootPath(string originalFilePath)
		{
			if (File.Exists(originalFilePath))
			{
				return originalFilePath;
			}

			if (string.IsNullOrEmpty(_documentInfo.LastOpenedAsPath))
			{
				return originalFilePath;
			}

			var languageFileName = Path.GetFileName(originalFilePath);

			var projectPath = _documentInfo.LastOpenedAsPath.Substring(0,
				_documentInfo.LastOpenedAsPath.LastIndexOf("\\" + _targetLanguageMapping.LanguageId + "\\", StringComparison.InvariantCultureIgnoreCase));

			var sourceLanguagePath = Path.Combine(projectPath, _sourceLanguageMapping.LanguageId) + "\\";
			var sourceFileLocalPath = Path.Combine(sourceLanguagePath, languageFileName);

			return sourceFileLocalPath;
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (_xmlNodes == null)
			{
				return;
			}

			var multilingualParagraphUnit = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a =>
					a.ContextType == Constants.MultilingualParagraphUnit);
			if (multilingualParagraphUnit != null)
			{
				_paragraphIndex = Convert.ToInt32(multilingualParagraphUnit.GetMetaData(Constants.MultilingualParagraphUnitIndex));
				_isCDATA = Convert.ToBoolean(multilingualParagraphUnit.GetMetaData(Constants.IsCDATA));
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

			var xmlNode = GetXmlNode(false);
			if (xmlNode != null)
			{
				//var escapedInnerXml = _entityService.ConvertKnownCharactersToEntities(xmlNode.InnerXml);

				if (_isPreview)
				{
					if (_isSource)
					{
						//xmlNode.InnerXml = _isCDATA ? $"<![CDATA[{escapedInnerXml}]]>" : escapedInnerXml;
						xmlNode = GetXmlNode(true);

						var xmlContent = "<![CDATA[<" + Constants.MultilingualParagraphUnit + " " + "pid=\"" + structureParagraph.Properties.ParagraphUnitId.Id + "\">]]>";
						xmlContent += targetSubContent;
						xmlContent += "<![CDATA[</" + Constants.MultilingualParagraphUnit + ">]]>";

						xmlNode.InnerXml = xmlContent;
					}
					else
					{
						var xmlContent = "<![CDATA[<" + Constants.MultilingualParagraphUnit + " " + "pid=\"" + structureParagraph.Properties.ParagraphUnitId.Id + "\">]]>";
						xmlContent += _isCDATA ? $"<![CDATA[{targetSubContent}]]>" : targetSubContent;
						xmlContent += "<![CDATA[</" + Constants.MultilingualParagraphUnit + ">]]>";

						xmlNode.InnerXml = xmlContent;
					}
				}
				else
				{
					xmlNode.InnerXml = _isCDATA ? $"<![CDATA[{targetSubContent}]]>" : targetSubContent;
				}
			}
		}

		private void CreateParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			var xmlSourceContent = string.Empty;
			var xmlTargetContent = string.Empty;
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				if (_isSource)
				{
					_segmentVisitor.VisitSegment(segmentPair.Source);
					xmlSourceContent += _isPreview ? "<![CDATA[<" + Constants.MultilingualSegment + " " + "pid=\"" + paragraphUnit.Properties.ParagraphUnitId.Id + "\" sid=\"" + segmentPair.Properties.Id.Id + "\">]]>" : string.Empty;
					xmlSourceContent += _segmentVisitor.Text;
					xmlSourceContent += _isPreview ? "<![CDATA[</" + Constants.MultilingualSegment + ">]]>" : string.Empty;
				}

				_segmentVisitor.VisitSegment(segmentPair.Target);
				if (_isSource)
				{
					xmlTargetContent += _segmentVisitor.Text;
				}
				else
				{
					xmlTargetContent += _isPreview ? "<![CDATA[<" + Constants.MultilingualSegment + " " + "pid=\"" + paragraphUnit.Properties.ParagraphUnitId.Id + "\" sid=\"" + segmentPair.Properties.Id.Id + "\">]]>" : string.Empty;
					xmlTargetContent += _segmentVisitor.Text;
					xmlTargetContent += _isPreview ? "<![CDATA[</" + Constants.MultilingualSegment + ">]]>" : string.Empty;
				}
			}

			var xmlNode = GetXmlNode(false);
			if (xmlNode != null)
			{
				if (_isPreview)
				{
					if (_isSource)
					{
						xmlNode.InnerXml = xmlTargetContent;
						xmlNode = GetXmlNode(true);
						xmlNode.InnerXml = xmlSourceContent;
					}
					else
					{
						xmlNode.InnerXml = xmlTargetContent;
					}
				}
				else
				{
					xmlNode.InnerXml = xmlTargetContent;
				}
			}
		}

		private XmlNode GetXmlNodeFromContent(string content)
		{
			try
			{
				if (string.IsNullOrEmpty(content))
				{
					return null;
				}

				content = _entityService.MarkupKnownEntities(content);
				content = _entityService.EscapeXmlCharacters(content);

				var xmlContent = new XmlDocument();
				xmlContent.LoadXml("<ExcelRoot>" + content + "</ExcelRoot>");

				return xmlContent.FirstChild;
			}
			catch
			{
				// catch all; ignore
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
			using (var reader = new StreamReader(subContentStream, _nativeFileProperties.Encoding.Encoding))
			{
				subContent = reader.ReadToEnd();
			}

			var segmentProperties = _segmentBuilder.CreateSegmentPairProperties();
			var segment = GetSegment(subContent, segmentProperties);
			_segmentVisitor.VisitSegment(segment);

			var content = _entityService.EntityMarkerConversionService.BackwardEntityMarkersConversion(_segmentVisitor.Text);

			return content;
		}

		private string GetMarkupText(IParagraphUnit paragraphUnit, ISegmentPair segmentPair, IEnumerable<Element> elements)
		{
			var content = string.Empty;
			foreach (var element in elements)
			{
				if (element is ElementText text)
				{
					var markup = "<![CDATA[<" + Constants.MultilingualSegment + " " + "pid=\"" +
								 paragraphUnit.Properties.ParagraphUnitId.Id + "\" sid=\"" + segmentPair.Properties.Id.Id +
								 "\">]]>";
					markup += text.Text;
					markup += "<![CDATA[</" + Constants.MultilingualSegment + ">]]>";

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

		private XmlNode GetXmlNode(bool isSource)
		{
			var xmlUnit = _xmlNodes[_paragraphIndex];

			if (isSource || _languageMappingSettings.LanguageMappingMonoLanguage)
			{
				return GetXmlUnitNode(xmlUnit, _sourceLanguageMapping.XPath);
			}

			return GetXmlUnitNode(xmlUnit, _targetLanguageMapping.XPath);
		}

		private XmlNode GetXmlUnitNode(XmlNode xmlUnit, string xPath)
		{
			var xmlNode = xmlUnit.SelectSingleNode(xPath, _nsmgr);
			if (xmlNode == null)
			{
				xmlNode = _xmlNodeService.AddXmlNode(xmlUnit, xPath);
			}

			return xmlNode;
		}

		public void FileComplete()
		{
			_targetDocument.Save(_nativeFileProperties.OutputFilePath);
			
			_xmlReader?.Close();
			_xmlReader?.Dispose();

			_fileStream?.Close();
			_fileStream?.Dispose();

			_targetDocument = null;
		}

		public void Complete()
		{
		}

		public void Dispose()
		{
		}
	}
}
