namespace Multilingual.Excel.FileType.Constants
{
	public class FiletypeConstants
	{
		// IMPORTANT: This identifier is persisted in user project files (.sdlproj, .sdlxliff)
		// when a file is added to a Trados Studio project. It MUST remain stable across
		// plugin releases, otherwise Studio will not recognize files created with previous
		// versions of the plugin as belonging to this file type.
		// Historically this string was built at runtime from Assembly.GetName().Version
		// (e.g. "Multilingual Excel FileType v 3.0.0.0"). Legacy versioned IDs that exist
		// in already-deployed projects are rewritten to this stable form on Studio startup
		// and whenever a project is added (see FileTypeIdMigrationService).
		// Do NOT change this string when bumping the assembly version.
		public const string FileTypeDefinitionId = "Multilingual Excel FileType";
		public const string FileTypeName = "Multilingual Excel";
		public const string FileTypeDocumentName = "Multilingual Excel Document";
		public const string FileTypeDocumentsName = "Multilingual Excel Documents";
		public const string Description = "Multilingual Excel documents.";
		public const string FileTypeFrameworkVersion = "1.0.0.1";
		public const string FileDialogWildcardExpression = "*.xlsx";
		public const string DefaultFileExtension = "xlsx";

		public const string ExcelContextInformationDisplayCode = "XC";
		public const string ExcelContextInformationDisplayName = "Excel Context";

		public const string LanguageMappingId = "MultilingualExcelFileType_LanguageMapping_Id";
		public const string EmbeddedContentId = "MultilingualExcelFileType_EmbeddedContent_Id";
		public const string PlaceholdersId = "MultilingualExcelFileType_Placeholders_Id";
		public const string CommentMappingId = "MultilingualExcelFileType_CommentMapping_Id";
		public const string EntitiesId = "MultilingualExcelFileType_Entities_Id";
		public const string WriterSettingsId = "MultilingualExcelFileType_WriterSettings_Id";
		public const string ComponentBuilderId = "MultilingualExcelFileType_ComponentBuilder_Id";
		public const string ImportBatchTaskId = "MultilingualExcelFileType_ImportBatchTask_Id";
		public const string ExportBatchTaskId = "MultilingualExcelFileType_ExportBatchTask_Id";

		public const string DefaultNamespace = "ns";
		public const string StructureParagraphUnit = "StructureParagraphUnit";
		public const string MultilingualParagraphUnit = "MultilingualParagraphUnit";
		public const string MultilingualParagraphUnitIndex = "MultilingualParagraphUnitIndex";
		public const string MultilingualExcelSheetIndex = "MultilingualExcelSheetIndex";
		public const string MultilingualExcelSheetName = "MultilingualExcelSheetName";
		public const string MultilingualExcelRowIndex = "MultilingualExcelRowIndex";
		public const string MultilingualSegment = "MultilingualSegment";
		public const string MultilingualExcelContextInformation = "MultilingualExcelContextInformation";

		public const string MultilingualExcelFilterBackgroundColorSource = "MultilingualExcelFilterBackgroundColorSource";
		public const string MultilingualExcelFilterBackgroundColorTarget = "MultilingualExcelFilterBackgroundColorTarget";
		public const string MultilingualExcelFilterLockSegmentsSource = "MultilingualExcelFilterLockSegmentsSource";
		public const string MultilingualExcelFilterLockSegmentsTarget = "MultilingualExcelFilterLockSegmentsTarget";

		public const string MultilingualExcelCharacterLimitationSource = "MultilingualExcelCharacterLimitationSource";
		public const string MultilingualExcelLineLimitationSource = "MultilingualExcelLineLimitationSource";
		public const string MultilingualExcelPixelLimitationSource = "MultilingualExcelPixelLimitationSource";
		public const string MultilingualExcelPixelFontNameSource = "MultilingualExcelPixelFontNameSource";
		public const string MultilingualExcelPixelFontSizeSource = "MultilingualExcelPixelFontSizeSource";

		public const string MultilingualExcelCharacterLimitationTarget = "MultilingualExcelCharacterLimitationTarget";
		public const string MultilingualExcelLineLimitationTarget = "MultilingualExcelLineLimitationTarget";
		public const string MultilingualExcelPixelLimitationTarget = "MultilingualExcelPixelLimitationTarget";
		public const string MultilingualExcelPixelFontNameTarget = "MultilingualExcelPixelFontNameTarget";
		public const string MultilingualExcelPixelFontSizeTarget = "MultilingualExcelPixelFontSizeTarget";
		public const string IsCDATA = "IsCDATA";
	}
}
