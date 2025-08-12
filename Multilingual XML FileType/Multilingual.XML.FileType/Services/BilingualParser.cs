using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Multilingual.XML.FileType.EmbeddedContent;
using Multilingual.XML.FileType.Extensions;
using Multilingual.XML.FileType.FileType.Settings;
using Multilingual.XML.FileType.FileType.Settings.Entities;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Services.Entities;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
    public class BilingualParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware, IPublishSubContent
    {
        private IFileProperties _fileProperties;

        private XmlDocument _document;

        private LanguageMapping _sourceLanguage;

        private XmlNamespaceManager _nsmgr;

        private readonly SegmentBuilder _segmentBuilder;

        private readonly DefaultNamespaceHelper _defaultNamespaceHelper;

        private readonly XmlReaderFactory _xmlReaderFactory;

        private readonly EntityContext _entityContext;

        private readonly EntityService _entityService;

        private readonly AlternativeInputFileGenerator _alternativeInputFileGenerator;

        private readonly FileSystemService _fileSystemService;

        private UniqueEntitySettings _entitySettings;

        private FileStream _fileStream;

        private XmlReader _xmlReader;

        public Language DefaultSourceLanguage { get; set; }

        //public List<IComment> ParagraphComments { get; set; }

        //public List<IComment> SegmentComments { get; set; }

        public BilingualParser(SegmentBuilder segmentBuilder, DefaultNamespaceHelper defaultNamespaceHelper,
            XmlReaderFactory xmlReaderFactory, EntityContext entityContext, EntityService entityService,
            AlternativeInputFileGenerator alternativeInputFileGenerator, FileSystemService fileSystemService)
        {
            _segmentBuilder = segmentBuilder;
            _defaultNamespaceHelper = defaultNamespaceHelper;
            _xmlReaderFactory = xmlReaderFactory;
            _entityContext = entityContext;
            _entityService = entityService;
            _alternativeInputFileGenerator = alternativeInputFileGenerator;
            _fileSystemService = fileSystemService;
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

        public event EventHandler<ProgressEventArgs> Progress;

        public IDocumentProperties DocumentProperties { get; set; }

        public IBilingualContentHandler Output { get; set; }

        public LanguageMappingSettings LanguageMappingSettings { get; private set; }

        public EmbeddedContentSettings EmbeddedContentSettings { get; private set; }

        public PlaceholderPatternSettings PlaceholderPatternSettings { get; private set; }

        public CommentMappingSettings CommentMappingSettings { get; private set; }

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
        }

        public void SetFileProperties(IFileProperties properties)
        {
            _fileProperties = properties;

            _fileProperties.FileConversionProperties.SourceLanguage = _fileProperties.FileConversionProperties.SourceLanguage.CultureInfo != null
                ? _fileProperties.FileConversionProperties.SourceLanguage
                : DefaultSourceLanguage;

            if (_fileProperties.FileConversionProperties?.SourceLanguage?.CultureInfo == null)
            {
                throw new Exception("Source language cannot be null!");
            }
            _sourceLanguage = LanguageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
            string.Compare(a.LanguageId, _fileProperties.FileConversionProperties.SourceLanguage.CultureInfo.Name, StringComparison.InvariantCultureIgnoreCase) == 0);

            var fileProperties = ItemFactory.CreateFileProperties();
            fileProperties.FileConversionProperties = _fileProperties.FileConversionProperties;
        }

        public void StartOfInput()
        {
            OnProgress(0);

            Output.Initialize(DocumentProperties);
            Output.SetFileProperties(_fileProperties);
        }

        public void EndOfInput()
        {
            Output.FileComplete();
            Output.Complete();

            _xmlReader?.Close();
            _xmlReader?.Dispose();

            _fileStream?.Close();
            _fileStream?.Dispose();

            Output.FileComplete();
            Output.Complete();

            OnProgress(100);

            _document = null;
        }

        public bool ParseNext()
        {

            var filePath = _fileProperties.FileConversionProperties.InputFilePath;
            var encoding = _fileProperties.FileConversionProperties.OriginalEncoding.Encoding;

            filePath = _alternativeInputFileGenerator.GenerateTempFileWithHiddenEntities(filePath, encoding);

            _document = new XmlDocument();

            _fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _xmlReader = _xmlReaderFactory.CreateReader(_fileStream, encoding, true);

            _document.Load(_xmlReader);
            _nsmgr = new XmlNamespaceManager(_document.NameTable);
            _document.AddAllNamespaces(_nsmgr);

            UpdateLanguageMappingSettings(_nsmgr);

            var nodes = _document.SelectNodes(LanguageMappingSettings.LanguageMappingLanguagesXPath, _nsmgr);
            if (nodes == null)
            {
                return false;
            }

            var xmlNodes = nodes.Cast<XmlNode>().ToList();

            for (var index = 0; index < xmlNodes.Count; index++)
            {
                //ParagraphComments = new List<IComment>();
                //SegmentComments = new List<IComment>();

                var xmlNode = nodes[index];

                var sourceXmlNode = xmlNode.SafeSelectSingleNode(_sourceLanguage.XPath, _nsmgr);
                var childNode = sourceXmlNode?.FirstChild;
                var isCdata = childNode?.NodeType == XmlNodeType.CDATA;

                //var sourceSegmentCommentXmlNodes = sourceXmlNode?.SelectNodes(".//" + CommentMappingSettings.CommentElementName, _nsmgr);
                //var paragraphCommentXmlNodes = GetParagraphLevelCommentNodes(xmlNode);

                //SegmentComments = GetComments(sourceSegmentCommentXmlNodes);
                //ParagraphComments = GetComments(paragraphCommentXmlNodes);

                if (EmbeddedContentSettings.EmbeddedContentProcess &&
                    //ContainsTags(sourceXmlNode) &&
                    (EmbeddedContentSettings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.All ||
                     (isCdata && EmbeddedContentSettings.EmbeddedContentFoundIn == EmbeddedContentSettings.FoundIn.CDATA)))
                {
                    var paragraphUnit = AddStructureParagraph(sourceXmlNode, index, isCdata);
                    Output.ProcessParagraphUnit(paragraphUnit);

                    PublishEmbeddedSubContent(EmbeddedContentSettings.EmbeddedContentProcessorId,
                        (isCdata ? sourceXmlNode.InnerText : sourceXmlNode?.InnerXml));
                }
                else
                {
                    var paragraphUnit = CreateParagraphUnit(sourceXmlNode, index);
                    Output.ProcessParagraphUnit(paragraphUnit);
                }

                OnProgress(Convert.ToByte(Math.Round(100m * ((index + 1m) / xmlNodes.Count), 0)));
            }

            return false;
        }



        private List<XmlNode> GetParagraphLevelCommentNodes(XmlNode xmlNode)
        {
            var paragraphCommentXmlNodes = new List<XmlNode>();

            var allSegmentCommentXmlNodes = new List<XmlNode>();
            foreach (var languageMappingLanguage in LanguageMappingSettings.LanguageMappingLanguages)
            {
                var languageXmlNode = xmlNode.SafeSelectSingleNode(languageMappingLanguage.XPath, _nsmgr);
                var languageComments = languageXmlNode?.SelectNodes(".//" + CommentMappingSettings.CommentElementName, _nsmgr);
                if (languageComments != null)
                {
                    foreach (XmlNode node in languageComments)
                    {
                        allSegmentCommentXmlNodes.Add(node);
                    }
                }
            }

            var allParagraphCommentXmlNodes = xmlNode.SelectNodes(".//" + CommentMappingSettings.CommentElementName, _nsmgr);
            if (allParagraphCommentXmlNodes != null)
            {
                foreach (XmlNode node in allParagraphCommentXmlNodes)
                {
                    if (!allSegmentCommentXmlNodes.Exists(a => a == node))
                    {
                        paragraphCommentXmlNodes.Add(node);
                    }
                }
            }

            return paragraphCommentXmlNodes;
        }

        private List<IComment> GetComments(XmlNodeList xmlNodeList)
        {
            var comments = new List<IComment>();
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                var comment = GetComment(xmlNode);
                comments.Add(comment);
            }

            return comments;
        }

        private List<IComment> GetComments(List<XmlNode> xmlNodes)
        {
            var comments = new List<IComment>();

            foreach (var xmlNode in xmlNodes)
            {
                var comment = GetComment(xmlNode);
                comments.Add(comment);
            }

            return comments;
        }

        private IComment GetComment(XmlNode xmlNode)
        {
            var text = xmlNode.InnerText;
            var author = string.Empty;
            var version = "1.0";
            var date = DateTime.MinValue;
            var severity = Severity.Low;

            if (xmlNode.Attributes != null && CommentMappingSettings.CommentMappings.Count > 0)
            {
                foreach (XmlAttribute attribute in xmlNode.Attributes)
                {
                    var name = attribute.Name;
                    var value = attribute.Value;

                    var commentMapping = CommentMappingSettings.CommentMappings.FirstOrDefault(a =>
                        string.Compare(a.PropertyName, name,
                            StringComparison.CurrentCultureIgnoreCase) == 0);

                    if (commentMapping != null)
                    {
                        if (string.Compare(commentMapping.StudioPropertyName, "Author",
                            StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            author = value;
                        }
                        else if (string.Compare(commentMapping.StudioPropertyName, "Date",
                            StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            var success = DateTime.TryParse(value, out var dateTimeValue);
                            date = success ? dateTimeValue : DateTime.MinValue;
                        }
                        else if (string.Compare(commentMapping.StudioPropertyName, "Version",
                            StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            version = value;
                        }
                        else if (string.Compare(commentMapping.StudioPropertyName, "Severity",
                            StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            var success = int.TryParse(value, out int intValue);
                            severity = success ? (Severity)intValue : Severity.Low;
                        }
                    }
                }
            }

            var newComment = _segmentBuilder.CreateComment(text, author, severity, date, version);
            return newComment;
        }

        private void UpdateLanguageMappingSettings(XmlNamespaceManager nsmgr)
        {
            if (_fileProperties.FileConversionProperties.FileSnifferInfo.MetaDataContainsKey(Constants.DefaultNamespace))
            {
                var namespaceUri = _fileProperties.FileConversionProperties.FileSnifferInfo.GetMetaData(Constants.DefaultNamespace);
                if (!string.IsNullOrEmpty(namespaceUri))
                {
                    var defaultNameSpace = new XmlNameSpace { Name = Constants.DefaultNamespace, Value = namespaceUri };
                    _defaultNamespaceHelper.AddXmlNameSpacesFromDocument(nsmgr, _document, defaultNameSpace);

                    LanguageMappingSettings.LanguageMappingLanguagesXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
                        LanguageMappingSettings.LanguageMappingLanguagesXPath, Constants.DefaultNamespace);

                    foreach (var mappingLanguage in LanguageMappingSettings.LanguageMappingLanguages)
                    {
                        mappingLanguage.XPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
                            mappingLanguage.XPath, Constants.DefaultNamespace);

                        mappingLanguage.CommentXPath = _defaultNamespaceHelper.UpdateXPathWithNamespace(
                            mappingLanguage.CommentXPath, Constants.DefaultNamespace);
                    }

                    CommentMappingSettings.CommentElementName = Constants.DefaultNamespace + ":" + CommentMappingSettings.CommentElementName;
                }
            }
        }

        private IParagraphUnit AddStructureParagraph(XmlNode sourceXmlNode, int index, bool isCdata)
        {
            var structureParagraphUnit = GetStructureParagraphUnit(sourceXmlNode, LockTypeFlags.Structure, index, isCdata);
            return structureParagraphUnit;
        }

        private IParagraphUnit CreateParagraphUnit(XmlNode sourceXmlNode, int index)
        {
            var structureParagraphUnit = GetStructureParagraphUnit(sourceXmlNode, LockTypeFlags.Unlocked, index, false);
            var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
            if (_sourceLanguage == null)
            {
                return structureParagraphUnit;
            }

            var segment = _segmentBuilder.CreateSegment(sourceXmlNode, segmentPairProperties, SegmentationHint.MayExclude);

            IAbstractMarkupDataContainer currentContainer = structureParagraphUnit.Source;
            IAbstractMarkupDataContainer previousContainer = null;
            foreach (var abstractMarkupData in segment)
            {
                if (abstractMarkupData is IAbstractMarkupDataContainer container)
                {
                    currentContainer = container;
                }

                if (previousContainer != null)
                {
                    if (previousContainer == abstractMarkupData.Parent)
                    {
                        previousContainer = currentContainer;
                        continue;
                    }
                }

                if (!currentContainer.Contains(abstractMarkupData))
                {
                    structureParagraphUnit.Source.Add(abstractMarkupData.Clone() as IAbstractMarkupData);
                }

                previousContainer = currentContainer;
            }

            return structureParagraphUnit;
        }

        private IParagraphUnit GetStructureParagraphUnit(XmlNode sourceXmlNode, LockTypeFlags lockType, int index, bool isCdata)
        {
            var structureParagraphUnit = ItemFactory.CreateParagraphUnit(lockType);
            structureParagraphUnit.Properties.Comments = PropertiesFactory.CreateCommentProperties();

            var contextProperties = ItemFactory.PropertiesFactory.CreateContextProperties();

            var paragraphContextInfo = ItemFactory.PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextProperties.Contexts.Add(paragraphContextInfo);

            var multilingualParagraphContextInfo = _segmentBuilder.CreateMultilingualParagraphContextInfo();
            contextProperties.Contexts.Add(multilingualParagraphContextInfo);

            structureParagraphUnit.Properties.Contexts = contextProperties;

            multilingualParagraphContextInfo.SetMetaData(Constants.MultilingualParagraphUnitIndex, index.ToString());
            multilingualParagraphContextInfo.SetMetaData(Constants.IsCDATA, isCdata.ToString());

            if (sourceXmlNode?.Attributes != null)
            {
                foreach (XmlAttribute attribute in sourceXmlNode.Attributes)
                {
                    var propertyParagraphContextInfo = _segmentBuilder.CreateElementPropertyParagraphContextInfo(attribute.Name, attribute.Value);
                    contextProperties.Contexts.Add(propertyParagraphContextInfo);
                }
            }

            return structureParagraphUnit;
        }

        //private bool ContainsTags(XmlNode xmlNode)
        //{
        //	if (xmlNode == null)
        //	{
        //		return false;
        //	}

        //	return xmlNode.ChildNodes.Cast<XmlNode>().Any(
        //		childNode => childNode.NodeType == XmlNodeType.Element
        //					 || childNode.NodeType == XmlNodeType.EndElement
        //					 || childNode.NodeType == XmlNodeType.CDATA);
        //}

        public void Dispose()
        {
            _document = null;
        }

        protected virtual void OnProgress(byte percent)
        {
            Progress?.Invoke(this, new ProgressEventArgs(percent));
        }

        private void PublishEmbeddedSubContent(string embeddedProcessorId, string text)
        {
            using (var memoryStream = new MemoryStream())
            {
                var content = new StringBuilder();
                //var fragments = _entityService.ConvertKnownEntitiesInMarkupData(text, EntityRule.Parser);


                var textValue = ProtextSoftReturns(text);

                textValue = textValue?.Replace(EntityConstants.BeginSdlEntityRefEscape, EntityConstants.BeginEntityRef)
                    ?.Replace(EntityConstants.EndSdlEntityRefEscape, EntityConstants.EndEntityRef);

                content.Append(textValue);


                //foreach (var fragment in fragments)
                //{
                //	if (fragment is IText textFragment)
                //	{
                //		if (textFragment.Properties?.Text != null)
                //		{
                //			textFragment.Properties.Text = ProtextSoftReturns(textFragment.Properties.Text);
                //			content.Append(textFragment.Properties.Text);
                //		}
                //	}
                //	else
                //	{
                //		content.Append(fragment);
                //	}
                //}

                memoryStream.WriteString(content.ToString());

                PublishSubContent(embeddedProcessorId, memoryStream);
            }
        }

        private static string ProtextSoftReturns(string text)
        {
            text = text?.Replace("\n", EntityConstants.SoftReturnEntityRefEscape);
            return text;
        }

        private void PublishSubContent(string embeddedProcessorId, Stream subContentStream)
        {
            if (string.IsNullOrEmpty(embeddedProcessorId))
            {
                return;
            }

            var subContentEventArgs = new ProcessSubContentEventArgs(embeddedProcessorId, subContentStream);

            PublishContent(subContentEventArgs);
        }

        public event EventHandler<ProcessSubContentEventArgs> ProcessSubContent;

        public void PublishContent(ProcessSubContentEventArgs eventArgs)
        {
            ProcessSubContent?.Invoke(this, eventArgs);
        }
    }
}
