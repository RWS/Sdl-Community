// <copyright file="TagParser.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-09</date>
// <summary>TagParser</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class is parses tags from string. 
    /// String with tags is provided by StringFeeder.
    /// Parsed tags with differed properties are returned.
    /// </summary>
    public sealed class TagParser : IDisposable
    {
        #region Fields

        /// <summary>
        /// Current string that is processed
        /// </summary>
        private string text;

        /// <summary>
        /// Current position in text
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Parsed tag or text as string
        /// </summary>
        private string parsedText;

        /// <summary>
        /// Tag that currently parsed from string.
        /// </summary>
        private TagInfo currentTag;

        /// <summary>
        /// Indicates whether we have more to parse.
        /// </summary>
        private bool canFeed;

        /// <summary>
        /// Provides current class with string to be parsed
        /// </summary>
        private IStringFeeder feeder;

        /// <summary>
        /// Threshold value. 
        /// When lentgh of string for parsing is bigger we will try to reduce length in twice.
        /// </summary>
        private int sizeToShrink = 4096;

        private int bodyDepth;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TagParser class
        /// </summary>
        /// <param name="text">text to parse</param>
        /// <param name="isInContent">Indicates whether we are parsing contnent.xml</param>
        public TagParser(string text)
        {
            this.text = text;

            this.Init();

            this.canFeed = false;
        }

        /// <summary>
        /// Initializes a new instance of the TagParser class
        /// </summary>
        /// <param name="feeder">provides parser with string to parse</param>
        /// <param name="isInContent">Indicates whether we are parsing contnent.xml</param>
        public TagParser(IStringFeeder feeder)
        {
            this.text = String.Empty;

            this.Init();

            this.feeder = feeder;
            this.feeder.Initialize();
            this.FeedString();
        }
        #endregion

        #region Properties

        public bool IsInDocInfo { get; private set; }
        public bool IsInHeader { get; private set; }
        public bool IsInReference { get; private set; }
        public bool IsInBody { get; private set; }
        public bool IsInTransUnit { get; private set; }
        public bool IsInTransUnits { get; private set; }
        public bool IsInWords { get; private set; }
        public bool IsInTransUnitClosed { get; private set; }
        public bool IsInMrkText { get; private set; }
        public bool IsInMrkComm { get; private set; }
        public bool IsInSegSource { get; private set; }
        public bool IsInSeg { get; private set; }
        public bool IsInCmtDef { get; private set; }
        public bool IsInCmts { get; private set; }
        public bool IsInFileCmtClosed { get; private set; }
        public bool IsInTagDefs { get; private set; }
        public bool IsInTag { get; private set; }
        public bool IsInTagSub { get; private set; }
        public bool IsInXClosed { get; private set; }
        public bool IsInFmtDefs { get; private set; }
        public bool IsInFmtDef { get; private set; }
        public bool IsInFmtClosed { get; private set; }

        // public int MrkMIDAttr { get; private set; } // 02.24.2011 - fix
        public string MrkMIDAttr { get; private set; }
        public string MrkCommCIDAttr { get; private set; }
        public string SourceLangAttr { get; private set; }
        public string TransUnitIDAttr { get; private set; }
        public string FileNameAttr { get; private set; }
        public string CmtDefIDAttr { get; private set; }
        public string FileCmtIDAttr { get; private set; }
        public string TagIDAttr { get; private set; }
        public string TagSubXIDAttr { get; private set; }
        public string XTagIDAttr { get; private set; }
        public string XTagXIDAttr { get; private set; }
        public string FmtDefIDAttr { get; private set; }
        public string FmtIDAttr { get; private set; }

        public bool isUnitTranslatable { get; private set; }

        public bool isBodyParent
        {
            get { return this.bodyDepth == 1; }
        }

        /// <summary>
        /// Gets parsedText
        /// </summary>
        public string ParsedText
        {
            get
            {
                return this.parsedText;
            }
        }

        /// <summary>
        /// Gets Id of current tag
        /// </summary>
        public Tags TagID
        {
            get
            {
                return this.currentTag.TagID;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we found tag. Otherwies we have text.
        /// </summary>
        public bool IsTag { get; private set; }

        /// <summary>
        /// Gets or sets a dictionary with know tags
        /// </summary>
        public Dictionary<string, Tags> TagDictionary { get; set; }


        /// <summary>
        /// Gets a value indicating type of parsed information
        /// </summary>
        public TagType ResultType
        {
            get
            {
                if (this.IsTag)
                {
                    return TagDirectory.Type(this.currentTag.TagID);
                }
                else
                {
                    return TagType.Text;
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether text can appears.
        /// </summary>
        private bool IsInContent
        {
            get
            {
                //return this.IsInSegSource && this.IsInMrkText || this.IsInDocInfo || this.IsInHeader;
                return this.IsInBody || this.IsInDocInfo || this.IsInHeader || this.IsInTransUnits || this.IsInWords;
            }
        }


        #endregion

        #region Public methods


        /// <summary>
        /// Parse next tag
        /// </summary>
        /// <returns>a value indicating whether the end of parsed document is reached</returns>
        public bool Next()
        {
            bool success;

            ////do
            ////{
            success = false;

            this.BeforeParsing();

            this.parsedText = String.Empty;
            this.IsTag = false;

            while (!success && this.currentIndex < this.text.Length)
            {
                int prevIndex = this.currentIndex;

                this.currentTag = this.FindTag();

                if (this.IsInContent)
                {
                    if (this.currentTag.IndexStart > prevIndex)
                    {
                        // if it is some text between previous tag and the next
                        // return this text    
                        this.currentIndex = this.currentTag.IndexStart;

                        this.parsedText = this.text.Substring(prevIndex, this.currentIndex - prevIndex);

                        this.currentTag.Text = this.parsedText;

                        this.currentTag.TagID = Tags.None;

                        success = true;
                    }
                    else
                    {
                        this.ConfirmTagFound(ref success);
                    }
                }
                else
                {
                    if (this.currentTag.TagID != Tags.None)
                    {
                        this.ConfirmTagFound(ref success);
                    }
                    else
                    {
                        this.currentIndex = this.text.Length;
                    }
                }

                if (this.currentIndex >= this.text.Length)
                {
                    this.FeedString();
                }
            }

            if (this.text.Length > this.sizeToShrink && this.currentIndex > this.sizeToShrink / 2)
            {
                this.text = this.text.Substring(this.currentIndex);
                this.currentIndex = 0;
            }

            this.AfterParsing();
            ////}
            ////while ( IsInsideDeleted && success); // This is to avoid deleted framgent

            return success;
        }

        /// <summary>
        /// Copies current tag
        /// </summary>
        /// <returns>Returns copy of current tag</returns>
        public TagInfo CopyCurrentTag()
        {
            return this.currentTag.Copy();
        }

        /// <summary>
        /// Progress of the parser
        /// </summary>
        /// <returns>returns progress</returns>
        public double Progress()
        {
            if (this.feeder != null)
            {
                return this.feeder.Progress();
            }
            else
            {
                return 0;
            }
        }

        public void Dispose()
        {
            this.feeder.Dispose();
        }

        #endregion

        #region Private methods
        /// <summary>
        /// Initialize all variables
        /// </summary>
        private void Init()
        {
            this.currentIndex = 0;
            this.currentTag = new TagInfo();

            this.IsTag = false;

            this.IsInMrkText = false;
            this.IsInMrkComm = false;
            this.IsInSegSource = false;
            this.IsInSeg = false;
            this.IsInBody = false;
            this.IsInTransUnit = false;
            this.IsInTransUnits = false;
            this.IsInWords = false;
            this.IsInTransUnitClosed = false;
            this.IsInDocInfo = false;
            this.IsInHeader = false;
            this.IsInReference = false;
            this.IsInCmtDef = false;
            this.IsInCmts = false;
            this.IsInFileCmtClosed = false;
            this.IsInTagDefs = false;
            this.IsInTag = false;
            this.IsInTagSub = false;
            this.IsInXClosed = false;
            this.IsInFmtDefs = false;
            this.IsInFmtDef = false;
            this.IsInFmtClosed = false;
            this.bodyDepth = 0;

            this.MrkMIDAttr = "";
            this.MrkCommCIDAttr = "";
            this.SourceLangAttr = "";
            this.TransUnitIDAttr = "";
            this.FileNameAttr = "";
            this.CmtDefIDAttr = "";
            this.FileCmtIDAttr = "";
            this.TagIDAttr = "";
            this.TagSubXIDAttr = "";
            this.XTagIDAttr = "";
            this.XTagXIDAttr = "";
            this.isUnitTranslatable = true;

            this.TagDictionary = TagDirectory.FullDictionary;
        }

        /// <summary>
        /// Search next tag 
        /// </summary>
        /// <returns>next tag with following spaces</returns>
        private TagInfo FindTag()
        {
            TagInfo tag = this.FindNext(this.currentIndex);


            return tag;
        }

        /// <summary>
        /// Search next tag 
        /// </summary>
        /// <param name="searchIndex">start position for search</param>
        /// <returns>returns next tag</returns>
        private TagInfo FindNext(int searchIndex)
        {
            TagInfo tag = new TagInfo();

            do
            {
                tag.IndexStart = this.text.IndexOf("<", searchIndex);

                if (tag.IndexStart == -1)
                {
                    this.FeedString();
                }
            }
            while (tag.IndexStart == -1 && this.canFeed);

            if (tag.IndexStart != -1)
            {
                do
                {
                    tag.IndexEnd = this.text.IndexOf(">", tag.IndexStart);

                    if (tag.IndexEnd == -1)
                    {
                        this.FeedString();
                    }
                }
                while (tag.IndexEnd == -1 && this.canFeed);

                tag.Text = this.text.Cut(tag.IndexStart, tag.IndexEnd);

                string tagName = tag.Text.NodeName();

                Tags tagCode = Tags.None;

                if (this.TagDictionary.TryGetValue(tagName, out tagCode))
                {
                    tag.TagID = tagCode;
                    tag.Name = tagName;
                }

                if (tag.TagID == Tags.None)
                {
                    tag.TagID = Tags.Unknown;
                    tag.Name = tag.Text; // "unknown";
                }
            }

            return tag;
        }

        /// <summary>
        /// Sets all variable accroding to found tag
        /// </summary>
        /// <param name="success">cofirms that tag is found</param>
        private void ConfirmTagFound(ref bool success)
        {
            success = true;
            this.IsTag = true;
            this.parsedText = this.currentTag.Text;
            this.currentIndex = this.currentTag.IndexEnd + 1;
        }

        /// <summary>
        /// Gets new portion of information from feeder
        /// </summary>
        private void FeedString()
        {
            if (this.feeder != null)
            {
                this.text = string.Format("{0}{1}", this.text, this.feeder.FeedString());
                //this.text = this.text + this.feeder.FeedString();
                this.canFeed = !this.feeder.EOF();

                if (!this.canFeed)
                {
                    this.feeder.Dispose();
                }
            }
            else
            {
                this.canFeed = false;
            }
        }

        /// <summary>
        /// Runned befor parsing of the tag
        /// </summary>
        private void BeforeParsing()
        {
            if (this.IsTag)
            {
                if (this.currentTag.TagID == Tags.MrkEnd)
                {
                    // it is temp solution !!!
                    this.IsInMrkText = false;
                    this.IsInMrkComm = false;
                    this.MrkMIDAttr = "";
                    this.MrkCommCIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.TransUnitEnd)
                    this.isUnitTranslatable = true;
                if (this.currentTag.TagID == Tags.SegSourceEnd)
                    this.IsInSegSource = false;
                if (this.currentTag.TagID == Tags.SegEnd)
                    this.IsInSeg = false;
                if (this.currentTag.TagID == Tags.BodyEnd)
                    this.IsInBody = false;
                if (this.currentTag.TagID == Tags.TransUnitEnd)
                {
                    this.IsInTransUnit = false;
                    this.TransUnitIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.TransUnitsEnd)
                    this.IsInTransUnits = false;
                if (this.currentTag.TagID == Tags.WordsEnd)
                    this.IsInWords = false;
                if (this.currentTag.TagID == Tags.DocInfoEnd)
                    this.IsInDocInfo = false;
                if (this.currentTag.TagID == Tags.HeaderEnd)
                    this.IsInHeader = false;
                if (this.currentTag.TagID == Tags.ReferenceEnd)
                    this.IsInReference = false;
                if (this.currentTag.TagID == Tags.CmtsEnd)
                    this.IsInCmts = false;
                if (this.currentTag.TagID == Tags.TagDefsEnd)
                    this.IsInTagDefs = false;
                if (this.currentTag.TagID == Tags.TagEnd)
                {
                    this.IsInTag = false;
                    this.TagIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.TagSubEnd)
                {
                    this.IsInTagSub = false;
                    this.TagSubXIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.FmtDefsEnd)
                    this.IsInFmtDefs = false;
                if (this.currentTag.TagID == Tags.FmtDefEnd)
                {
                    this.IsInFmtDef = false;
                    this.FmtDefIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.CmtDefEnd)
                {
                    this.IsInCmtDef = false;
                    this.CmtDefIDAttr = "";
                }
                if (this.currentTag.TagID == Tags.FileEnd)
                {
                    this.SourceLangAttr = "";
                    this.FileNameAttr = "";
                }
            }

            if (!(this.currentIndex < this.text.Length))
            {
                this.FeedString();
            }
        }

        /// <summary>
        /// Is runned after new tag is reached
        /// </summary>
        private void AfterParsing()
        {
            if (this.IsTag)
            {
                this.IsInTransUnitClosed = false;
                this.IsInFileCmtClosed = false;
                this.IsInXClosed = false;
                this.IsInFmtClosed = false;

                if (this.currentTag.TagID == Tags.MrkStart && !this.parsedText.IsClosedTag())
                {
                    if (this.ParsedText.AttributeValue(TagDirectory.AttrSegMType) == TagDirectory.AttrSegMTypeValue)
                    {
                        this.IsInMrkText = true;
                        this.MrkMIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrSegMID)) ?
                            "" : this.ParsedText.AttributeValue(TagDirectory.AttrSegMID));
                    }
                    else if (this.ParsedText.AttributeValue(TagDirectory.AttrSegMType) == TagDirectory.AttrSegMTypeCommValue)
                    {
                        this.IsInMrkComm = true;
                        this.MrkCommCIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrCommMID)) ?
                            "" : this.ParsedText.AttributeValue(TagDirectory.AttrCommMID));
                    }
                }
                if (this.currentTag.TagID == Tags.TransUnitStart && !this.parsedText.IsClosedTag())
                    this.isUnitTranslatable = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrUnitTranslate)) ? true : false);
                if (this.currentTag.TagID == Tags.SegSourceStart && !this.parsedText.IsClosedTag())
                    this.IsInSegSource = true;
                if (this.currentTag.TagID == Tags.FileStart && !this.parsedText.IsClosedTag())
                {
                    this.SourceLangAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrSourceLang)) ?
                            "" : this.ParsedText.AttributeValue(TagDirectory.AttrSourceLang));
                    this.FileNameAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrFileName)) ?
                            "" : this.ParsedText.AttributeValue(TagDirectory.AttrFileName));
                }
                if (this.currentTag.TagID == Tags.ReferenceStart && !this.parsedText.IsClosedTag())
                    this.IsInReference = true;
                if (this.currentTag.TagID == Tags.SegStart && !this.parsedText.IsClosedTag())
                    this.IsInSeg = true;
                if (this.currentTag.TagID == Tags.CmtsStart && !this.parsedText.IsClosedTag())
                    this.IsInCmts = true;
                if (this.currentTag.TagID == Tags.CmtDefStart && !this.parsedText.IsClosedTag())
                {
                    this.IsInCmtDef = true;
                    this.CmtDefIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrID)) ?
                        "" : this.ParsedText.AttributeValue(TagDirectory.AttrID));
                }
                if (this.currentTag.TagID == Tags.TagDefsStart && !this.parsedText.IsClosedTag())
                    this.IsInTagDefs = true;
                if (this.currentTag.TagID == Tags.TagStart && !this.parsedText.IsClosedTag())
                {
                    this.IsInTag = true;
                    this.TagIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrID));
                }
                if (this.currentTag.TagID == Tags.TagSubStart && !this.parsedText.IsClosedTag())
                {
                    this.IsInTagSub = true;
                    this.TagSubXIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrXID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrXID));
                }
                if (this.currentTag.TagID == Tags.XStart)
                {
                    this.IsInXClosed = true;
                    this.XTagIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrID));
                    this.XTagXIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrXID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrXID));
                }
                if (this.currentTag.TagID == Tags.FmtDefsStart && !this.parsedText.IsClosedTag())
                    this.IsInFmtDefs = true;
                if (this.currentTag.TagID == Tags.FmtDefStart && !this.parsedText.IsClosedTag())
                {
                    this.IsInFmtDef = true;
                    this.FmtDefIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrID));
                }
                if (this.currentTag.TagID == Tags.FmtStart)
                {
                    this.IsInFmtClosed = true;
                    this.FmtIDAttr = (String.IsNullOrEmpty(this.ParsedText.AttributeValue(TagDirectory.AttrID)) ?
                       "" : this.ParsedText.AttributeValue(TagDirectory.AttrID));
                }


                if (this.currentTag.TagID == Tags.TransUnitsStart && !this.parsedText.IsClosedTag())
                    this.IsInTransUnits = true;
                if (this.currentTag.TagID == Tags.WordsStart && !this.parsedText.IsClosedTag())
                    this.IsInWords = true;
                if (this.currentTag.TagID == Tags.TransUnitStart && !this.parsedText.IsClosedTag())
                {
                    this.IsInTransUnit = true;
                    this.TransUnitIDAttr = this.ParsedText.AttributeValue(TagDirectory.AttrID);
                }
                else if (this.currentTag.TagID == Tags.TransUnitStart)
                {
                    this.IsInTransUnitClosed = true;
                    this.TransUnitIDAttr = this.ParsedText.AttributeValue(TagDirectory.AttrID);
                }

                if (this.currentTag.TagID == Tags.FileCmtStart && this.parsedText.IsClosedTag())
                {
                    this.IsInFileCmtClosed = true;
                    this.FileCmtIDAttr = this.ParsedText.AttributeValue(TagDirectory.AttrID);
                }
                if (this.currentTag.TagID == Tags.BodyStart && !this.parsedText.IsClosedTag())
                    this.IsInBody = true;
                else if (this.currentTag.TagID == Tags.DocInfoStart && !this.parsedText.IsClosedTag())
                    this.IsInDocInfo = true;
                else if (this.currentTag.TagID == Tags.HeaderStart && !this.parsedText.IsClosedTag())
                    this.IsInHeader = true;
                else if (this.IsInBody && !this.parsedText.IsClosedTag())
                {
                    if (this.parsedText.IfCloseTagPart())
                        this.bodyDepth--;
                    else this.bodyDepth++;
                }

            }
        }

        #endregion
    }
}