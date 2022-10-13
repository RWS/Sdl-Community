namespace Multilingual.XML.FileType
{
	public static class XmlConstants
	{
		public const string XmlNamespace = "xmlns";
		public const string PrefixSeparator = ":";
		public const char PrefixSeparatorChar = ':';
		public const string StartTagMarker = "<";
		public const string EndTagMarker = "</";
		public const string CloseTagMarker = ">";
		public const string EmptyTagMarker = "/>";
		public const string CommentStartMark = "<!--";
		public const string CommentEndMark = "-->";
		public const string CommentPlaceholderTagContentFormat = CommentStartMark + "{0}" + CommentEndMark;
		public const string AttributeReferenceCharacter = "@";
		public const string AttributeSelector = "attribute::";
		public const string ConditionStartCharacter = "[";
		public const string ConditionEndCharacter = "]";
		public const string CdataFormat = "<![CDATA[{0}]]>";
		public const string EscapedCdataFormat = "&lt;![CDATA[{0}]]&gt;";
		public const string CommentFormat = "<!--{0}-->";

		public const string StartNodeIdMetaKey = "StartNodeId";
		public const string EndNodeIdMetaKey = "EndNodeId";
		public const string RawTagContentMetaKey = "RawTagContent";
		public const string NamespaceMappingForInlineElementMetaKey = "NamespaceMappingForInlineElement";
		public const char PrefixSeparatorInMetadata = '~';
		public const char NamespaceMappingSeparatorInMetadata = '|';
		public const string XmlLanguage = "xml:lang";
		public const string TagTypeMetaKey = "TagType";
		public const string PartialTagTextForCommentPlaceholder = "Comment";
		public const string StructureParagraphContext = "StructureParagraphUnit";

		public const string SdlDefaultNamespacePrefix = "sdl_default_namespace_prefix";
	}
}
