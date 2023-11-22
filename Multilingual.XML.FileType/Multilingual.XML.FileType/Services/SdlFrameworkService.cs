using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Services
{
    public class SdlFrameworkService 
    {
	    public SdlFrameworkService(IDocumentItemFactory itemsFactory, IPropertiesFactory propertiesFactory)
        {
	        DocumentItemFactory = itemsFactory;
	        PropertiesFactory = propertiesFactory;
        }

        public IDocumentItemFactory DocumentItemFactory { get; }

        public IPropertiesFactory PropertiesFactory { get; }

        public IParagraphUnit CreateParagraphUnit()
        {
            var paragraphUnit = DocumentItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            var contextProperties = PropertiesFactory.CreateContextProperties();
            contextProperties.Contexts.Add(PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph));
            paragraphUnit.Properties.Contexts = contextProperties;

            return paragraphUnit;
        }

        public IParagraphUnit CreateStructureParagraphUnit()
        {
            var paragraphUnit = DocumentItemFactory.CreateParagraphUnit(LockTypeFlags.Structure);
            var contextProperties = DocumentItemFactory.PropertiesFactory.CreateContextProperties();

            var contextInfo = CreateContextInfo("StructureParagraphUnit");

            contextProperties.Contexts.Add(contextInfo);
            paragraphUnit.Properties.Contexts = contextProperties;

            return paragraphUnit;
        }

        public IParagraphUnit CreateParagraphUnit(IContextProperties contextProperties)
        {
            var paragraphUnit = DocumentItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            paragraphUnit.Properties.Contexts = contextProperties;

            return paragraphUnit;
        }

        public IContextProperties CreateContextProperties()
        {
            return PropertiesFactory.CreateContextProperties();
        }

        public IContextInfo CreateContextInfo(string contextType)
        {
            return PropertiesFactory.CreateContextInfo(contextType);
        }

        public IStructureInfo CreateStructureInfo(IContextInfo contextInfo, bool mustUseDisplayName)
        {
            return PropertiesFactory.CreateStructureInfo(contextInfo, mustUseDisplayName, null);
        }

        public IText CreateText(string inputText)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(inputText);
            var text = DocumentItemFactory.CreateText(textProperties);

            return text;
        }


        public IStartTagProperties CreateStartTagProperties(string startTagContent = "")
        {
            return PropertiesFactory.CreateStartTagProperties(startTagContent);
        }

        public ITagPair CreateTagPair(string startTagContent = "", string endTagContent = "")
        {
            var startTagProperties = CreateStartTagProperties(startTagContent);
            var endTagProperties = PropertiesFactory.CreateEndTagProperties(endTagContent);

            return DocumentItemFactory.CreateTagPair(startTagProperties, endTagProperties);
        }

        public IPlaceholderTag CreatePlaceholderTag(string tagContent = "")
        {
            var tagProps = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
            return DocumentItemFactory.CreatePlaceholderTag(tagProps);
        }

        public ISubSegmentProperties CreateSubSegmentProperties(int attributeValueOffset, int valueLength)
        {
            var subSegmentProperties = PropertiesFactory.CreateSubSegmentProperties(attributeValueOffset, valueLength);
            return subSegmentProperties;
        }

        public ISubSegmentReference CreateSubSegmentReference(
            ISubSegmentProperties subSegmentProperties,
            ParagraphUnitId paragraphUnitId)
        {
            var subSegmentReference = DocumentItemFactory.CreateSubSegmentReference(subSegmentProperties,
                paragraphUnitId);

            return subSegmentReference;
        }

        public virtual ILockedContent CreateLockedContent()
        {
            var lockedContentProperties = PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Structure);
            var lockedContent = DocumentItemFactory.CreateLockedContent(lockedContentProperties);
            return lockedContent;
        }

        public ICommentMarker CreateCommentMarker(string localizationNote, string author)
        {
            var comment = PropertiesFactory.CreateComment(localizationNote, author, Severity.Low);
            var properties = PropertiesFactory.CreateCommentProperties();
            properties.Add(comment);
            var commentMarker = DocumentItemFactory.CreateCommentMarker(properties);

            return commentMarker;
        }
    }
}
