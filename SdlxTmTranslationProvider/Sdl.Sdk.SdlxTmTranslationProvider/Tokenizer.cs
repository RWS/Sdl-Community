//-----------------------------------------------------------------------
// <copyright file="Tokenizer.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.Core.Tokenization;

    /// <summary>
    /// A class to tokenize a segment
    /// </summary>
    public class Tokenizer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        public Tokenizer()
        {
        }

        /// <summary>
        /// Tokenizes the specified segment.
        /// </summary>
        /// <param name="segment">The segment to tokenize.</param>
        /// <returns>A list of tokens found in the segment</returns>
        public List<Token> Tokenize(Segment segment)
        {
            List<Token> tokens = new List<Token>();

            int run = -1;

            // We'll just do some simple tokenization
            foreach (SegmentElement segmentElement in segment.Elements)
            {
                ++run;

                if (segmentElement == null)
                {
                    continue;
                }

                Text txtR = segmentElement as Text;
                Token tokR = segmentElement as Token;
                Tag tagR = segmentElement as Tag;

                if (tagR != null)
                {
                    Token t = new TagToken(tagR);
                    t.Span = new SegmentRange(run, 0, 0);
                    tokens.Add(t);
                }
                else if (tokR != null)
                {
                    tokR.Span = new SegmentRange(run, 0, 0);
                    tokens.Add(tokR);
                }
                else if (txtR != null)
                {
                    List<Token> tokenized = this.TokenizeInternal(txtR.Value, run, true, false);
                    if (tokenized != null && tokenized.Count > 0)
                    {
                        tokens.AddRange(tokenized);
                    }
                }
            }

            return tokens;
        }

        /// <summary>
        /// Tokenizes a string.
        /// </summary>
        /// <param name="textString">The string to tokenize.</param>
        /// <param name="currentRun">The current run.</param>
        /// <param name="createWhitespaceTokens">if set to <c>true</c> then the tokenizer will create whitespace tokens.</param>
        /// <param name="allowTokenBundles">if set to <c>true</c> then the tokenizer will allow token bundles.</param>
        /// <returns>A list of tokens</returns>
        private List<Token> TokenizeInternal(string textString, int currentRun, bool createWhitespaceTokens, bool allowTokenBundles)
        {
            List<Token> tokens = new List<Token>();

            int position = 0;
            int stringLength = textString.Length;
            while (position < stringLength)
            {
                int start = position;

                while (position < stringLength && char.IsWhiteSpace(textString, position))
                {
                    ++position;
                }

                if (position > start)
                {
                    if (createWhitespaceTokens)
                    {
                        Token t = new SimpleToken(textString.Substring(start, position - start), TokenType.Whitespace);
                        t.Span = new SegmentRange(currentRun, start, position - 1);
                        tokens.Add(t);
                    }

                    start = position;
                }

                if (position >= stringLength)
                {
                    break;
                }

                int category = this.GetCharacterType(textString, start);
                while (position < stringLength && this.GetCharacterType(textString, position) == category)
                {
                    ++position;
                }

                int winningLength = position - start;
                Token winningToken = new SimpleToken(textString.Substring(start, position - start), TokenType.Word);
                winningToken.Span = new SegmentRange(currentRun, start, position - 1);

                tokens.Add(winningToken);
            }

            return tokens;
        }

        /// <summary>
        /// Gets the type of the character.
        /// </summary>
        /// <param name="textString">The text string.</param>
        /// <param name="position">The position.</param>
        /// <returns>An integer to represent the character type</returns>
        private int GetCharacterType(string textString, int position)
        {
            int returnValue = 0;

            switch (textString[position])
            {
                case '\x0009':
                case '\x000a':
                case '\x0020':
                case '\x0021':
                case '\x0022':
                case '\x0028':
                case '\x0029':
                case '\x002c':
                case '\x002e':
                case '\x002f':
                case '\x003a':
                case '\x003b':
                case '\x003c':
                case '\x003e':
                case '\x003f':
                case '\x005b':
                case '\x005c':
                case '\x005d':
                case '\x007b':
                case '\x007c':
                case '\x007d':
                case '\x00a0':
                case '\x201c':
                case '\x201d':
                case '\x2028':
                    returnValue = 0;
                    break;

                default:
                    System.Globalization.UnicodeCategory category = char.GetUnicodeCategory(textString, position);
                    switch (category)
                    {
                        case System.Globalization.UnicodeCategory.LowercaseLetter:
                        case System.Globalization.UnicodeCategory.UppercaseLetter:
                        case System.Globalization.UnicodeCategory.TitlecaseLetter:
                        case System.Globalization.UnicodeCategory.DecimalDigitNumber:
                        case System.Globalization.UnicodeCategory.LetterNumber:
                            returnValue = 1;
                            break;
                        case System.Globalization.UnicodeCategory.ClosePunctuation:
                        case System.Globalization.UnicodeCategory.ConnectorPunctuation:
                        case System.Globalization.UnicodeCategory.CurrencySymbol:
                        case System.Globalization.UnicodeCategory.DashPunctuation:
                        case System.Globalization.UnicodeCategory.EnclosingMark:
                        case System.Globalization.UnicodeCategory.FinalQuotePunctuation:
                        case System.Globalization.UnicodeCategory.Format:
                        case System.Globalization.UnicodeCategory.InitialQuotePunctuation:
                        case System.Globalization.UnicodeCategory.NonSpacingMark:
                        case System.Globalization.UnicodeCategory.OpenPunctuation:
                            returnValue = 2;
                            break;
                        default:
                            returnValue = 3;
                            break;
                    }

                    break;
            }

            return returnValue;
        }
    }
}
