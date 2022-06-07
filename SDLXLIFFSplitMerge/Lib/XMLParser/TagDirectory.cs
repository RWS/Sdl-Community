// <copyright file="TagDirectory.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>TagDirectory</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Enumeration of tags
    /// </summary>
    public enum Tags : int
    {
        /// <summary>
        /// Represents missed tag
        /// </summary>
        None,

        /// <summary>
        /// Represents unknown tag
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents starting of the xliff tag
        /// </summary>
        XliffStart,

        /// <summary>
        /// Represents ending of the xliff tag
        /// </summary>
        XliffEnd,

        /// <summary>
        /// Represents starting of the DocInfo tag
        /// </summary>
        DocInfoStart,

        /// <summary>
        /// Represents ending of the DocInfo tag
        /// </summary>
        DocInfoEnd,

        /// <summary>
        /// Represents starting of the header tag
        /// </summary>
        HeaderStart,

        /// <summary>
        /// Represents ending of the header tag
        /// </summary>
        HeaderEnd,

        /// <summary>
        /// Represents starting of the reference tag
        /// </summary>
        ReferenceStart,

        /// <summary>
        /// Represents ending of the reference tag
        /// </summary>
        ReferenceEnd,

        /// <summary>
        /// Represents starting of the body tag
        /// </summary>
        BodyStart,

        /// <summary>
        /// Represents ending of the body tag
        /// </summary>
        BodyEnd,

        /// <summary>
        /// Represents starting of the file tag
        /// </summary>
        FileStart,

        /// <summary>
        /// Represents ending of the file tag
        /// </summary>
        FileEnd,

        /// <summary>
        /// Represents starting of the group tag
        /// </summary>
        GroupStart,

        /// <summary>
        /// Represents ending of the group tag
        /// </summary>
        GroupEnd,

        /// <summary>
        /// Represents starting of the transunit tag
        /// </summary>
        TransUnitStart,

        /// <summary>
        /// Represents ending of the transunit tag
        /// </summary>
        TransUnitEnd,

        /// <summary>
        /// Represents starting of the transunits tag
        /// </summary>
        TransUnitsStart,

        /// <summary>
        /// Represents ending of the transunits tag
        /// </summary>
        TransUnitsEnd,

        /// <summary>
        /// Represents starting of the mrk tag
        /// </summary>
        MrkStart,

        /// <summary>
        /// Represents ending of the mrk tag
        /// </summary>
        MrkEnd,

        /// <summary>
        /// Represents starting of the segment tag
        /// </summary>
        SegStart,

        /// <summary>
        /// Represents ending of the segment tag
        /// </summary>
        SegEnd,

        /// <summary>
        /// Represents starting of the segdefs tag
        /// </summary>
        SegDefsStart,

        /// <summary>
        /// Represents ending of the segdefs tag
        /// </summary>
        SegDefsEnd,

        /// <summary>
        /// Represents starting of the seg-source tag
        /// </summary>
        SegSourceStart,

        /// <summary>
        /// Represents ending of the seg-source tag
        /// </summary>
        SegSourceEnd,

        /// <summary>
        /// Represents starting of the comment tag
        /// </summary>
        CmtDefStart,

        /// <summary>
        /// Represents ending of the comment tag
        /// </summary>
        CmtDefEnd,

        /// <summary>
        /// Represents starting of the comment tag
        /// </summary>
        CmtsStart,

        /// <summary>
        /// Represents ending of the comment tag
        /// </summary>
        CmtsEnd,

        /// <summary>
        /// Represents starting of the comment tag
        /// </summary>
        FileCmtStart,

        /// <summary>
        /// Represents ending of the comment tag
        /// </summary>
        FileCmtEnd,

        /// <summary>
        /// Represents starting of the tag defs tag
        /// </summary>
        TagDefsStart,

        /// <summary>
        /// Represents ending of the tag defs tag
        /// </summary>
        TagDefsEnd,

        /// <summary>
        /// Represents starting of the tag tag
        /// </summary>
        TagStart,

        /// <summary>
        /// Represents ending of the tag tag
        /// </summary>
        TagEnd,

        /// <summary>
        /// Represents starting of the x (reference to tag) tag
        /// </summary>
        TagSubStart,

        /// <summary>
        /// Represents ending of the x (reference to tag) tag
        /// </summary>
        TagSubEnd,

        /// <summary>
        /// Represents starting of the x (reference to tag) tag
        /// </summary>
        XStart,

        /// <summary>
        /// Represents ending of the x (reference to tag) tag
        /// </summary>
        XEnd,

        /// <summary>
        /// Represents starting of the fmt-defs tag
        /// </summary>
        FmtDefsStart,

        /// <summary>
        /// Represents ending of the fmt-defs tag
        /// </summary>
        FmtDefsEnd,

        /// <summary>
        /// Represents starting of the fmt-def tag
        /// </summary>
        FmtDefStart,

        /// <summary>
        /// Represents ending of the fmt-def tag
        /// </summary>
        FmtDefEnd,

        /// <summary>
        /// Represents starting of the fmt tag
        /// </summary>
        FmtStart,

        /// <summary>
        /// Represents ending of the fmt tag
        /// </summary>
        FmtEnd,
        
        /// <summary>
        /// Represents starting of the fmt tag
        /// </summary>
        WordsStart,

        /// <summary>
        /// Represents ending of the fmt tag
        /// </summary>
        WordsEnd
}

    /// <summary>
    /// Any new tags should be described in this class to be proccessed by parser
    /// </summary>
    public static class TagDirectory
    {
        #region Const format strings

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string None = "none";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string Unknown = "unknown";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string XliffStart = "<xliff";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string XliffEnd = "</xliff>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string DocInfoStart = "<doc-info";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string DocInfoEnd = "</doc-info>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string HeaderStart = "<header";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string HeaderEnd = "</header>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string ReferenceStart = "<reference";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string ReferenceEnd = "</reference>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string BodyStart = "<body";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string BodyEnd = "</body>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FileStart = "<file";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FileEnd = "</file>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string GroupStart = "<group";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string GroupEnd = "</group>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TransUnitStart = "<trans-unit";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TransUnitEnd = "</trans-unit>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TransUnitsStart = "<trans-units";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TransUnitsEnd = "</trans-units>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string MrkStart = "<mrk";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string MrkEnd = "</mrk>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegStart = "<sdl:seg";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegEnd = "</sdl:seg>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegDefsStart = "<sdl:seg-defs";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegDefsEnd = "</sdl:seg-defs>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegSourceStart = "<seg-source";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string SegSourceEnd = "</seg-source>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CommentStart = "<Comment";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CommentEnd = "</Comment>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CommentsStart = "<Comments";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CommentsEnd = "</Comments>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CmtDefStart = "<cmt-def";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CmtDefEnd = "</cmt-def>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CmtsStart = "<cmt-defs";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string CmtsEnd = "</cmt-defs>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FileCmtStart = "<sdl:cmt";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FileCmtEnd = "</sdl:cmt>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagDefsStart = "<tag-defs";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagDefsEnd = "</tag-defs>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagStart = "<tag";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagEnd = "</tag>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagSubStart = "<sub";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string TagSubEnd = "</sub>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string XStart = "<x";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string XEnd = "</x>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtDefsStart = "<fmt-defs";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtDefsEnd = "</fmt-defs>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtDefStart = "<fmt-def";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtDefEnd = "</fmt-def>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtStart = "<fmt";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string FmtEnd = "</fmt>";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string WordsStart = "<words";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string WordsEnd = "</words>";

        #region My Attributes
        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrID = "id";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrXID = "xid";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSourceLang = "source-language";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegConf = "conf";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegPerc= "percent";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegMID = "mid";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrCommMID = "sdl:cid";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegMType = "mtype";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegMTypeValue = "seg";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegMTypeCommValue = "x-sdl-comment";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegLocked = "locked";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrSegLockedValue = "true";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrUnitTranslate = "translate";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrUnitTranslateValue = "no";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrFileName = "name";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrUser = "user";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrDate = "date";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrXmlNs = "xmlns";

        /// <summary>
        /// Odt constant
        /// </summary>
        public const string AttrXmlNsValue = "http://sdl.com/FileTypes/SdlXliff/1.0";
        #endregion



        #endregion

        /// <summary>
        /// Full Dictionary
        /// </summary>
        public static readonly Dictionary<string, Tags> FullDictionary;

        /// <summary>
        /// HyperLink Dictionary
        /// </summary>
        public static readonly Dictionary<string, Tags> HyperlinkDictionary;

        /// <summary>
        /// Initializes static members of the TagDirectory class
        /// </summary>
        static TagDirectory()
        {
            InitilizeTagProperties();

            int i = 0;
            FullDictionary = TagProperties.ToDictionary(e => e.Name, e => (Tags)i++);
        }

        /// <summary>
        /// Gets or sets properties of the tag
        /// </summary>
        internal static TagProperties[] TagProperties { get; set; }

        #region Tags properties

        /// <summary>
        /// Returns name of the tag
        /// </summary>
        /// <param name="tag">Tag argument</param>
        /// <returns>Name of the tag</returns>
        public static string Tag(Tags tag)
        {
            return TagProperties[(int)tag].Name;
        }

        /// <summary>
        /// Returns tag type
        /// </summary>
        /// <param name="tag">Tag argument</param>
        /// <returns>Type of the tag</returns>
        public static TagType Type(Tags tag)
        {
            return TagProperties[(int)tag].TagType;
        }

        /// <summary>
        /// Initializing tag properties
        /// </summary>
        private static void InitilizeTagProperties()
        {
            TagProperties = new TagProperties[Enum.GetNames(typeof(Tags)).Length];

            TagProperties[(int)Tags.None] = new TagProperties(None, TagType.StructureTag);
            TagProperties[(int)Tags.Unknown] = new TagProperties(Unknown, TagType.StructureTag);
            TagProperties[(int)Tags.XliffStart] = new TagProperties(XliffStart, TagType.StructureTag);
            TagProperties[(int)Tags.XliffEnd] = new TagProperties(XliffEnd, TagType.StructureTag);
            TagProperties[(int)Tags.DocInfoStart] = new TagProperties(DocInfoStart, TagType.StructureTag);
            TagProperties[(int)Tags.DocInfoEnd] = new TagProperties(DocInfoEnd, TagType.StructureTag);
            TagProperties[(int)Tags.HeaderStart] = new TagProperties(HeaderStart, TagType.StructureTag);
            TagProperties[(int)Tags.HeaderEnd] = new TagProperties(HeaderEnd, TagType.StructureTag);
            TagProperties[(int)Tags.ReferenceStart] = new TagProperties(ReferenceStart, TagType.StructureTag);
            TagProperties[(int)Tags.ReferenceEnd] = new TagProperties(ReferenceEnd, TagType.StructureTag);
            TagProperties[(int)Tags.BodyStart] = new TagProperties(BodyStart, TagType.StructureTag);
            TagProperties[(int)Tags.BodyEnd] = new TagProperties(BodyEnd, TagType.StructureTag);
            TagProperties[(int)Tags.FileStart] = new TagProperties(FileStart, TagType.StructureTag);
            TagProperties[(int)Tags.FileEnd] = new TagProperties(FileEnd, TagType.StructureTag);

            TagProperties[(int)Tags.GroupStart] = new TagProperties(GroupStart, TagType.StructureTag);
            TagProperties[(int)Tags.GroupEnd] = new TagProperties(GroupEnd, TagType.StructureTag);
            TagProperties[(int)Tags.TransUnitStart] = new TagProperties(TransUnitStart, TagType.StructureTag);
            TagProperties[(int)Tags.TransUnitEnd] = new TagProperties(TransUnitEnd, TagType.StructureTag);
            TagProperties[(int)Tags.TransUnitsStart] = new TagProperties(TransUnitsStart, TagType.StructureTag);
            TagProperties[(int)Tags.TransUnitsEnd] = new TagProperties(TransUnitsEnd, TagType.StructureTag);
            TagProperties[(int)Tags.MrkStart] = new TagProperties(MrkStart, TagType.StructureTag);
            TagProperties[(int)Tags.MrkEnd] = new TagProperties(MrkEnd, TagType.StructureTag);
           
            TagProperties[(int)Tags.SegStart] = new TagProperties(SegStart, TagType.StructureTag);
            TagProperties[(int)Tags.SegEnd] = new TagProperties(SegEnd, TagType.StructureTag);
            TagProperties[(int)Tags.SegDefsStart] = new TagProperties(SegDefsStart, TagType.StructureTag);
            TagProperties[(int)Tags.SegDefsEnd] = new TagProperties(SegDefsEnd, TagType.StructureTag);
            TagProperties[(int)Tags.SegSourceStart] = new TagProperties(SegSourceStart, TagType.StructureTag);
            TagProperties[(int)Tags.SegSourceEnd] = new TagProperties(SegSourceEnd, TagType.StructureTag);
          
            TagProperties[(int)Tags.CmtDefStart] = new TagProperties(CmtDefStart, TagType.StructureTag);
            TagProperties[(int)Tags.CmtDefEnd] = new TagProperties(CmtDefEnd, TagType.StructureTag);
            TagProperties[(int)Tags.CmtsStart] = new TagProperties(CmtsStart, TagType.StructureTag);
            TagProperties[(int)Tags.CmtsEnd] = new TagProperties(CmtsEnd, TagType.StructureTag);
            TagProperties[(int)Tags.FileCmtStart] = new TagProperties(FileCmtStart, TagType.StructureTag);
            TagProperties[(int)Tags.FileCmtEnd] = new TagProperties(FileCmtEnd, TagType.StructureTag);

            TagProperties[(int)Tags.TagDefsStart] = new TagProperties(TagDefsStart, TagType.StructureTag);
            TagProperties[(int)Tags.TagDefsEnd] = new TagProperties(TagDefsEnd, TagType.StructureTag);
            TagProperties[(int)Tags.TagStart] = new TagProperties(TagStart, TagType.StructureTag);
            TagProperties[(int)Tags.TagEnd] = new TagProperties(TagEnd, TagType.StructureTag);
            TagProperties[(int)Tags.TagSubStart] = new TagProperties(TagSubStart, TagType.StructureTag);
            TagProperties[(int)Tags.TagSubEnd] = new TagProperties(TagSubEnd, TagType.StructureTag);
            TagProperties[(int)Tags.XStart] = new TagProperties(XStart, TagType.StructureTag);
            TagProperties[(int)Tags.XEnd] = new TagProperties(XEnd, TagType.StructureTag);

            TagProperties[(int)Tags.FmtDefsStart] = new TagProperties(FmtDefsStart, TagType.StructureTag);
            TagProperties[(int)Tags.FmtDefsEnd] = new TagProperties(FmtDefsEnd, TagType.StructureTag);
            TagProperties[(int)Tags.FmtDefStart] = new TagProperties(FmtDefStart, TagType.StructureTag);
            TagProperties[(int)Tags.FmtDefEnd] = new TagProperties(FmtDefEnd, TagType.StructureTag);
            TagProperties[(int)Tags.FmtStart] = new TagProperties(FmtStart, TagType.StructureTag);
            TagProperties[(int)Tags.FmtEnd] = new TagProperties(FmtEnd, TagType.StructureTag);

            TagProperties[(int)Tags.WordsStart] = new TagProperties(WordsStart, TagType.StructureTag);
            TagProperties[(int)Tags.WordsEnd] = new TagProperties(WordsEnd, TagType.StructureTag);
        }
        #endregion
    }
}
