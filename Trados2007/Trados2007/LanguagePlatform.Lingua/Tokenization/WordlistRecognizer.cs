using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
    internal class WordlistRecognizer : Recognizer
    {
        private Wordlist _List;
        private SearchOption _Options = SearchOption.None;

		public WordlistRecognizer(Core.Tokenization.TokenType t, int priority, string tokenClassName, string recognizerName, Wordlist w)
            : base(t, priority, tokenClassName, recognizerName)
        {
            if (w == null)
                throw new ArgumentNullException("w");
            _List = w;
        }

        public SearchOption Options
        {
            get { return _Options; }
            set { _Options = value; }
        }

		public override Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength)
        {
			throw new NotImplementedException();

			/*
            List<int> prefixes = _List.ContainedPrefixes(s, from, _Options);
            if (prefixes == null || prefixes.Count == 0)
                return null;

            for (int i = prefixes.Count - 1; i >= 0; --i)
            {
                int prefixLength = prefixes[i];

                if (VerifyContextConstraints(s, from + prefixLength))
                {
                    // TODO ultimately we may want to record which recognizer found the item, and 
                    //  maybe even reflect that in the UI?
                    consumedLength = prefixLength;
                    return new SimpleToken(s.Substring(from, prefixLength), _Type);
                }
            }

            return null;
			 * */
        }
    }
}
