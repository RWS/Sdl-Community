// ---------------------------------
// <copyright file="SegmentParser.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-08</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.Core.Tokenization;
    using Sdl.LanguagePlatform.TranslationMemory;

    using Sdl.LanguagePlatform.Lingua.Tokenization;

    /// <summary>
    /// Static container which incapsulates logic to work with <see cref="T:Sdl.LanguagePlatform.Core.Segment"/>
    /// </summary>
    public class SegmentParser
    {
        #region Fields

        /// <summary>
        /// Stores tokenizer for the specific culture
        /// </summary>
        private static Tokenizer tokenizer;

        /// <summary>
        /// Stores xml regex
        /// </summary>
        private static readonly Regex XmlRegex = new Regex(@"<(.|\n)*?>");

        /// <summary>
        /// Stores &lt; regex
        /// </summary>
        private static readonly Regex LessRegex = new Regex(@"&lt;");

        /// <summary>
        /// Stores &gt; regex
        /// </summary>
        private static readonly Regex GreaterRegex = new Regex(@"&gt;");

        /// <summary>
        /// Stores &amp; regex
        /// </summary>
        private static readonly Regex AmpRegex = new Regex(@"&amp;");

        /// <summary>
        /// Stores &quot; regex
        /// </summary>
        private static readonly Regex QuotRegex = new Regex(@"&quot;");

        #endregion // Fields

        /// <summary>
        /// Extracts the segment text.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>Plain text representation of the segment.</returns>
        public static string ExtractSegmentText(Segment segment)
        {
            IList<Tag> list = new List<Tag>();
            return SegmentParser.ExtractSegmentText(segment, out list);
        }

        /// <summary>
        /// Extracts the segment text.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <param name="extractedTags">The extracted tags.</param>
        /// <returns>Plain text representation of the segment.</returns>
        public static string ExtractSegmentText(Segment segment, out IList<Tag> extractedTags)
        {
            var builder = new StringBuilder();
            extractedTags = new List<Tag>();
            int tagId = 1;

            // while this is not required for search, we still need the lookup segment to be tokenized in the
            // search result. This is taking care of it:
            TokenizeSegment(ref segment);

            Text txt;
            Tag tg;
            Token tk;

            foreach (SegmentElement se in segment.Elements)
            {
                if ((txt = se as Text) != null)
                {
                    builder.Append(txt.Value);
                }
                else if ((tg = se as Tag) != null)
                {
                    extractedTags.Add(tg);

                    string tagText = GetTagAsText(tg, tagId++);
                    builder.Append(tagText);
                }
                else if ((tk = se as Token) != null)
                {
                    // we shouldn't really see tokens in the segment, so this won't happen:
                    if (tk.Text != null)
                    {
                        builder.Append(tk.Text);
                    }
                }
                else
                {
                    throw new Exception("Unexpected segment element");
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets the tag as text.
        /// </summary>
        /// <param name="tag">The SDL tag.</param>
        /// <param name="tagId">The tag id.</param>
        /// <returns>Plain text.</returns>
        public static string GetTagAsText(Tag tag, int tagId)
        {
            string result = string.Empty;

            switch (tag.Type)
            {
                case TagType.Start:
                case TagType.End:
                case TagType.LockedContent:
                case TagType.Standalone:
                case TagType.TextPlaceholder:
                    {
                        if (tag.TagID != null)
                        {
                            result = tagId.ToString();
                        }

                        break;
                    }

                case TagType.Undefined:
                case TagType.UnmatchedEnd:
                case TagType.UnmatchedStart:
                    {
                        return string.Empty;
                    }
            }

            return string.Format("{{{0}}}", result);
        }

        /// <summary>
        /// Tokenizes the segment.
        /// </summary>
        /// <param name="segment">The segment.</param>
        public static void TokenizeSegment(ref Segment segment)
        {
            if (segment.Tokens == null)
            {
                if ((tokenizer == null) || (tokenizer.Parameters.Culture != segment.Culture))
                {
                    TokenizerSetup setup = TokenizerSetupFactory.Create(segment.Culture);
                     
                    setup.CreateWhitespaceTokens = true;

                    setup.BuiltinRecognizers |= BuiltinRecognizers.RecognizeNumbers
                         | BuiltinRecognizers.RecognizeDates
                         | BuiltinRecognizers.RecognizeTimes
                         | BuiltinRecognizers.RecognizeMeasurements; 

                    tokenizer = new Tokenizer(setup);
                }

                segment.Tokens = tokenizer.Tokenize(segment);
            }
        }

        /// <summary>
        /// Creates the segment.
        /// </summary>
        /// <param name="scoringResult">The scoring result.</param>
        /// <param name="searchSettings">The search settings.</param>
        /// <param name="matchResult">The match result.</param>
        /// <param name="extractedTags">The extracted tags.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// New segment
        /// </returns>
        public static Segment CreateSegment(
            ScoringResult scoringResult,
            SearchSettings searchSettings,
            string matchResult,
            CultureInfo culture)
        {
            var segment = new Segment(culture);

            // matches all xml tags with attributes inside
            MatchCollection matches = SegmentParser.XmlRegex.Matches(matchResult);

            if (matches.Count > 0)
            {
                int prev_ind = 0;
                int matchesCount = matches.Count;

                for (int i = 0; i < matchesCount; i++)
                {
                    var match = matches[i];

                    var tagElement = SegmentParser.GetTag(match.Value);
                    string adjacent = matchResult.Substring(prev_ind, match.Index - prev_ind);
                    adjacent = SegmentParser.UnescapeLiterals(adjacent);

                    // move index
                    prev_ind += adjacent.Length;
                    prev_ind += match.Length;

                    // add elements
                    segment.Add(adjacent);
                    segment.Add(tagElement);
                }

                // text after last match
                var lastMatch = matches[matchesCount - 1];
                var lastPosition = lastMatch.Index + lastMatch.Length;
                string text = matchResult.Substring(lastPosition, matchResult.Length - lastPosition);
                segment.Add(SegmentParser.UnescapeLiterals(text));
            }
            else
            {
                // no tags, add plain text
                segment.Add(SegmentParser.UnescapeLiterals(matchResult));
            }

            return segment;
        }

        /// <summary>
        /// Gets the tag. Converts all Trados 2007 tags into Standalone.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// Segment Element
        /// </returns>
        public static SegmentElement GetTag(string tag)
        {
            string value = tag;
            var element = new Tag();

            // get tag name
            var nameRegex = new Regex(@"<[\S]*");
            var nameMatch = nameRegex.Match(value);
            string name = value.Substring(1, nameMatch.Length - 1);

            // temp  logic for now
            element.Type = TagType.Standalone;
            element.Anchor = new Random().Next();
            element.TagID = name;

            return element;
        }

        /// <summary>
        /// Unescapes the following literals:
        /// <list>
        ///  <value>&lt;</value>
        ///  <value>&gt;</value>
        ///  <value>&amp;</value>
        ///  <value>&quot;</value>
        /// </list>
        /// </summary>
        /// <param name="escapedString">The escaped string.</param>
        /// <returns>Unescaped string</returns>
        public static string UnescapeLiterals(string escapedString)
        {
            // unescape &lt, ...
            var unescapedString = SegmentParser.LessRegex.Replace(escapedString, "<");
            unescapedString = SegmentParser.GreaterRegex.Replace(unescapedString, ">");
            unescapedString = SegmentParser.AmpRegex.Replace(unescapedString, "&");
            unescapedString = SegmentParser.QuotRegex.Replace(unescapedString, "\"");

            return unescapedString;
        }
    }
}
