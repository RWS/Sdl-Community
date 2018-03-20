using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	// TODO should this inherit from Recognizer?

	public class FSTRecognizer
	{
		protected LanguagePlatform.Lingua.FST.FST _FST;
		protected List<int> _First;
		protected System.Globalization.CultureInfo _Culture;

		public FSTRecognizer(LanguagePlatform.Lingua.FST.FST fst,
			System.Globalization.CultureInfo culture)
		{
			if (fst == null)
				throw new ArgumentNullException("fst");
			if (culture == null)
				culture = System.Globalization.CultureInfo.InvariantCulture;

			_FST = fst;
			_Culture = culture;
			_First = _FST.GetFirstSet(false);
		}

		public LanguagePlatform.Lingua.FST.FST FST
		{
			get { return _FST; }
		}

		public List<int> First
		{
			get { return _First; }
		}

		public System.Globalization.CultureInfo Culture
		{
			get { return _Culture; }
		}

		/// <summary>
		/// Computes the matches for the current FST, given the input.
		/// </summary>
		/// <param name="s">The input string</param>
		/// <param name="startOffset">The offset in the input string where to start the matching process</param>
		/// <param name="ignoreCase">If <c>true</c>, the matching process will ignore case differences. If <c>false</c>, 
		/// symbols must match literally.</param>
		/// <param name="cap">If <c>&gt;0</c>, the number of matches will
		/// be limited to that number. Otherwise, all matches will be returned.</param>
		/// <param name="keepLongestMatchesOnly">If <c>true</c>, only the (group of) longest matches
		/// is kept in the result set, even if the number in that group is smaller than <paramref name="cap"/>. If <c>false</c>, 
		/// matches of all lengths are kept in the result set.</param>
		/// <returns>The matches, sorted by decreasing coverage length, or <c>null</c> if no matches
		/// were found.</returns>
		public List<FSTMatch> ComputeMatches(string s, int startOffset, bool ignoreCase, 
			int cap, bool keepLongestMatchesOnly)
		{
			if (_FST == null)
				return null;

			if (startOffset < s.Length && _First != null)
			{
				// TODO pass culture to the matcher, to enable culture-specific case mapping?

				char first = s[startOffset];
				if (!LanguagePlatform.Lingua.FST.Label.Matches(first, _First, ignoreCase))
					return null;
			}

			LanguagePlatform.Lingua.FST.Matcher matcher = new Sdl.LanguagePlatform.Lingua.FST.Matcher(_FST);
			List<FST.MatchState> matches = new List<Sdl.LanguagePlatform.Lingua.FST.MatchState>();
			matcher.Match(s, false, Sdl.LanguagePlatform.Lingua.FST.Matcher.MatchMode.Analyse, startOffset,
				ignoreCase,
				delegate(FST.MatchState ms)
				{
					matches.Add(ms);
					return true;
				},
				null);

			if (matches.Count == 0)
				return null;

			// sort by decreasing input coverage
			matches.Sort((a, b) => b.ConsumedSymbols - a.ConsumedSymbols);
			int longest = matches[0].ConsumedSymbols;

			List<FSTMatch> result = new List<FSTMatch>();
			foreach (FST.MatchState ms in matches)
			{
				if (keepLongestMatchesOnly && ms.ConsumedSymbols < longest)
					break;

				result.Add(new FSTMatch(startOffset, ms.ConsumedSymbols, ms.GetOutputAsString()));
				if (cap > 0 && result.Count >= cap)
					break;
			}
			return result;
		}

	}
}
