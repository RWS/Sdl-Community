using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Stemming
{
	public class CachingStemmer : IStemmer
	{
		private IStemmer _WrappedStemmer;
		private Dictionary<string, string> _Cache;

		/// <summary>
		/// Constructs a new cache around the specified stemmer.
		/// </summary>
		public CachingStemmer(IStemmer wrapped)
		{
			if (wrapped == null)
				throw new ArgumentNullException("wrapped");

			_WrappedStemmer = wrapped;
			_Cache = new Dictionary<string, string>();
		}

		/// <summary>
		/// See <see cref="IStemmer.Stem"/>
		/// </summary>
		public string Stem(string word)
		{
			string stem;
			if (!_Cache.TryGetValue(word, out stem))
			{
				stem = _WrappedStemmer.Stem(word);
				_Cache.Add(word, stem);
			}
			return stem;
		}
	}
}
