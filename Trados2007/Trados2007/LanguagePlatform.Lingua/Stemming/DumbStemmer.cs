using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
    /// <summary>
    /// A dumb stemmer doesn't know anything about languages and simply uses some heuristics
    /// to pseudo-stem its input, such as lowercasing.
    /// </summary>
    public class DumbStemmer : IStemmer
    {
		/// <summary>
		/// Returns the stem form of a word
		/// </summary>
		/// <param name="word">The word.</param>
		/// <returns>The stem form of word.</returns>
        public string Stem(string word)
        {
            return Core.CharacterProperties.ToBase(word).ToLowerInvariant();
        }
    }
}
