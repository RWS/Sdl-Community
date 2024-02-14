using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Globalization;

namespace TradosPlugin
{
    public class Tokenizer
    {
        public static List<Token> TokenizeSegment(Segment segment)
        { 
            // splits up the segment into tokens
            // currently very simple: tokens are text and tags
            List<Token> tokens = new List<Token>();
            int ix = 0;
            foreach (var element in segment.Elements)
            {
                if (element == null) continue;
                if (element is Text)
                {
                    Text text = element as Text;
                    if (text != null && !String.IsNullOrEmpty(text.Value))
                    {
                        Token token = new SimpleToken(text.Value);
                        token.Span = new SegmentRange(ix, 0, text.Value.Length - 1);
                        tokens.Add(token);
                    }
                }
                else if (element is Tag)
                {
                    TagToken token = new TagToken(element as Tag);
                    token.Span = new SegmentRange(ix, 0, 1);
                }
                ix++;
            }
            return tokens;
        }
    }
}
