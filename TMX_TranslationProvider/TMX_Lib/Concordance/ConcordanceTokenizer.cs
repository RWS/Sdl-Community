using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;

namespace TMX_Lib.Concordance
{
	internal class ConcordanceTokenizer
	{
		private SimpleResult _simpleResult;
		private TmxSearchSettings _settings;
		private CultureInfo _sourceLanguage, _targetLanguage;
		private string _searchText;
		private int _score;

		private SearchResult _result;

		public ConcordanceTokenizer(SimpleResult simpleResult, TmxSearchSettings settings, CultureInfo sourceLanguage, CultureInfo targetLanguage, string searchText)
		{
			_simpleResult = simpleResult;
			_settings = settings;
			_searchText = searchText;
			_sourceLanguage = sourceLanguage;
			_targetLanguage = targetLanguage;
			_score = simpleResult.Score;
			ComputeSearchResult();
		}

		public SearchResult Result => _result;

		private void ComputeSearchResult()
		{
			var isSourceConcordance = _settings.Mode == SearchMode.ConcordanceSearch;
			var searchResult = _simpleResult.ToSearchResult(_searchText, _settings, _sourceLanguage, _targetLanguage);
			var tu = searchResult.TranslationProposal;
			if (isSourceConcordance)
			{
				tu.SourceSegment.Tokens = TokenizeSegment(tu.SourceSegment);
				searchResult.ScoringResult.MatchingConcordanceRanges = CollectConcordanceMatchRanges(tu.SourceSegment, _searchText);
			}
			else
			{
				tu.TargetSegment.Tokens = TokenizeSegment(tu.TargetSegment);
				searchResult.ScoringResult.MatchingConcordanceRanges = CollectConcordanceMatchRanges(tu.TargetSegment, _searchText);
			}

			_result = searchResult;
		}


		// a naive tokenizer implementation to treat a single text run as one token.
		private static List<Sdl.LanguagePlatform.Core.Tokenization.Token> TokenizeSegment(Segment segment)
		{
			var tokens = new List<Sdl.LanguagePlatform.Core.Tokenization.Token>();
			var run = 0;
			foreach (var element in segment.Elements)
			{
				var text = element as Text;
				if (text == null || string.IsNullOrEmpty(text.Value)) continue;
				var token = new Sdl.LanguagePlatform.Core.Tokenization.SimpleToken(text.Value)
				{
					Span = new SegmentRange(run, 0, text.Value.Length - 1)
				};
				tokens.Add(token);
				run++;
			}
			return tokens;
		}

		private List<string> GetListOfSimpleWords(string text)
		{

			var tmpWords = new List<string>();
			var words = text.Split(' ', '(', ')', '[', ']', ',', '\"');

			foreach (var word in words)
			{

				var wordTmp = string.Empty;

				foreach (var _char in word.ToCharArray())
				{
					if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2)
					{
						if (wordTmp != string.Empty)
							tmpWords.Add(wordTmp);
						wordTmp = string.Empty;

						tmpWords.Add(_char.ToString());
					}
					else
						wordTmp += _char.ToString();
				}
				if (wordTmp != string.Empty)
				{
					tmpWords.Add(wordTmp);
				}
			}

			// get rid of the last punctuation mark at beginning & end  |
			var punctuations = new [] { "!", ":", ".", ";", "?" };
			// note we don't want to get rid of all punctuation marks
			if (tmpWords.Count > 0)
			{
				var firstWord = tmpWords[0];
				var lastWord = tmpWords[tmpWords.Count - 1];

				//spanish question marks
				if (firstWord.Trim().StartsWith("ż") && lastWord.Trim().EndsWith("?"))
				{
					tmpWords[0] = tmpWords[0].Substring(tmpWords[0].IndexOf("ż", StringComparison.Ordinal) + 1);
				}


				if (lastWord.Trim().EndsWith("..."))
				{
					tmpWords[tmpWords.Count - 1]
						= tmpWords[tmpWords.Count - 1].Substring(0, tmpWords[tmpWords.Count - 1].LastIndexOf("...", StringComparison.Ordinal));
				}
				else
				{
					var punctuation = lastWord.Trim().Substring(lastWord.Trim().Length - 1);

					if (punctuations.Contains(punctuation))
					{
						tmpWords[tmpWords.Count - 1]
							= tmpWords[tmpWords.Count - 1].Substring(0, tmpWords[tmpWords.Count - 1].LastIndexOf(punctuation, StringComparison.Ordinal));
					}
				}
			}

			var wordList = new List<string>();
			foreach (var word in tmpWords)
			{
				var foundWord = wordList.Any(s => string.Compare(s, word, StringComparison.OrdinalIgnoreCase) == 0);
				if (!foundWord)
					wordList.Add(word);
			}

			return wordList;
		}

		private List<SegmentRange> CollectConcordanceMatchRanges(Segment segment, string searchString)
		{
			var words = GetListOfSimpleWords(searchString);

			var concordanceMatchRanges = new List<SegmentRange>();


			foreach (var word in words)
			{
				var run = 0;
				var searchLength = word.Length;
				var wordBuilder = string.Empty;
				foreach (var element in segment.Elements)
				{
					var text = element as Text;
					if (text == null || string.IsNullOrEmpty(text.Value))
						continue;

					var index = text.Value.IndexOf(word, StringComparison.OrdinalIgnoreCase);
					while (index >= 0 && index < text.Value.Length)
					{
						var segmentRange = new SegmentRange(run, index, index + searchLength - 1);
						var prefixBoundary = true;
						//test that the beginning is a boundary character
						if (index > 0)
						{
							var c = Convert.ToChar(text.Value.Substring(index - 1, 1));
							if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
							{
								prefixBoundary = false;
							}
						}
						else if (wordBuilder != string.Empty)
						{
							var c = Convert.ToChar(wordBuilder.Substring(wordBuilder.Length - 1));
							if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
							{
								prefixBoundary = false;
							}
						}

						var suffixBountry = true;
						if (index + searchLength + 1 < text.Value.Length)
						{
							var c = Convert.ToChar(text.Value.Substring(index + searchLength, 1));
							if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
							{
								suffixBountry = false;
							}
						}

						if (prefixBoundary && suffixBountry)
						{
							concordanceMatchRanges.Add(segmentRange);
						}

						index += searchLength;
						if (index < text.Value.Length)
							index = text.Value.IndexOf(word, index, StringComparison.OrdinalIgnoreCase);
					}

					run++;
					wordBuilder += text.Value;
				}
			}
			return concordanceMatchRanges;
		}

	}
}
