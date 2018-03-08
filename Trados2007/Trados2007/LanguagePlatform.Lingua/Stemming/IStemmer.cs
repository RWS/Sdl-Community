using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
	/// <summary>
	/// Basic interface for stemming.
	/// </summary>
    public interface IStemmer
    {
		/// <summary>
		/// Returns the stem form of the specified word.
		/// </summary>
		/// <param name="word">The word.</param>
		/// <returns>The stem form of word.</returns>
        string Stem(string word);

        // TODO a more elaborate stemmer may need the sentence since the stem depends on context
        // TODO an even more elaborate stemmer may even need to modify the token structure?
    }
}
