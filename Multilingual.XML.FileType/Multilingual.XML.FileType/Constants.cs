using System.Reflection;

namespace Multilingual.XML.FileType
{
	public class Constants
	{
		public static readonly string FileTypeDefinitionId = "Multilingual XML FileType v " + Assembly.GetExecutingAssembly().GetName().Version;
		public const string FileTypeName = "Multilingual XML";
		public const string FileTypeDocumentName = "Multilingual XML Document";
		public const string FileTypeDocumentsName = "Multilingual XML Documents";
		public const string Description = "Multilingual XML documents.";
		public const string FileTypeFrameworkVersion = "1.0.0.1";
		public const string FileDialogWildcardExpression = "*.xml;*.xliff";
		public const string DefaultFileExtension = "xml";

        public const string LanguageMappingId = "MultilingualXMLFileType_LanguageMapping_Id";
		public const string EmbeddedContentId = "MultilingualXMLFileType_EmbeddedContent_Id";
		public const string PlaceholdersId = "MultilingualXMLFileType_Placeholders_Id";
		public const string CommentMappingId = "MultilingualXMLFileType_CommentMapping_Id";
		public const string WriterId = "MultilingualXMLFileType_Writer_Id";
		public const string EntitiesId = "MultilingualXMLFileType_Entities_Id";
		public const string WriterSettingsId = "MultilingualXMLFileType_WriterSettings_Id";
		public const string ComponentBuilderId = "MultilingualXMLFileType_ComponentBuilder_Id";
		public const string ImportBatchTaskId = "MultilingualXMLFileType_ImportBatchTask_Id";
		public const string ExportBatchTaskId = "MultilingualXMLFileType_ExportBatchTask_Id";

		public const string DefaultNamespace = "ns";
		public const string StructureParagraphUnit = "StructureParagraphUnit";
		public const string MultilingualParagraphUnit = "MultilingualParagraphUnit";
		public const string MultilingualParagraphUnitIndex = "MultilingualParagraphUnitIndex";
		public const string MultilingualSegment = "MultilingualSegment";
		public const string IsCDATA = "IsCDATA";
		public const string ElementPropertyParagraphUnit = "ElementProperty";
	}
}
