using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Service
{
	public class TermSearchCriteriaService
	{
		private readonly Dictionary<string, List<string>> _stopWords = new Dictionary<string, List<string>>
		{
			{
				"en", new List<string>
				{
					"a",
					"an",
					"the"
				}
			},
			{
				"it", new List<string>
				{
					"il",
					"la",
					"le",
					"lo",
					"i",
					"gli",
					"un",
					"una",
					"uno"
				}
			}
		};

		public IEnumerable<string> GetSearchCriteria(string text, ILanguage language)
		{
			var sourceLanguageId = language.Locale.TwoLetterISOLanguageName;
			var searchCriteria = new List<string>();
			foreach (var wordGroups in GetNgrams(text).OrderByDescending(a => a.Key))
			{
				if (wordGroups.Key == 1)
				{
					foreach (var wordGroup in wordGroups.Value)
					{
						if (wordGroup.Length <= 2)
						{
							continue;
						}

						if (sourceLanguageId.Contains(sourceLanguageId))
						{
							var stopWords = _stopWords[sourceLanguageId];
							if (stopWords.Contains(wordGroup.ToLower()))
							{
								continue;
							}
						}

						searchCriteria.Add(wordGroup);
					}
				}
				else if (wordGroups.Key < 4)
				{
					searchCriteria.AddRange(wordGroups.Value);
				}
			}

			return searchCriteria;
		}

		public List<string> GetWordGroups(string text, int maxWords, int prefixBuffer = 3)
		{
			var regexSplit = new Regex(@"[\s\t]", RegexOptions.Singleline);
			var words = regexSplit.Split(text).Where(word => !string.IsNullOrEmpty(word)).ToList();

			var wordGroups = new List<string>();

			var wordGroup = string.Empty;
			for (var i = 0; i < words.Count; i++)
			{
				wordGroup += (!string.IsNullOrEmpty(wordGroup) ? " " : string.Empty) + words[i];

				if (i > 0 && i % maxWords == 0)
				{
					wordGroups.Add(wordGroup);

					//fixed prefixBuffer = 3
					wordGroup = words[i - 2] + " " + words[i - 1] + " " + words[i];
				}
			}

			if (!string.IsNullOrEmpty(wordGroup))
			{
				wordGroups.Add(wordGroup);
			}

			return wordGroups;
		}

		private static Dictionary<int, List<string>> GetNgrams(string text)
		{
			var results = new Dictionary<int, List<string>>();

			var regexSplit = new Regex(@"[\s\t]", RegexOptions.Singleline);
			var words = regexSplit.Split(text).Where(word => !string.IsNullOrEmpty(word)).ToList();

			for (var i = 0; i < words.Count; i++)
			{
				var wordGroup = string.Empty;
				var wordCount = 0;
				for (var j = i; j >= 0; j--)
				{
					wordCount++;
					wordGroup = words[j] + (!string.IsNullOrEmpty(wordGroup) ? " " : string.Empty) + wordGroup;

					if (results.ContainsKey(wordCount))
					{
						var groups = results[wordCount];
						if (!groups.Contains(wordGroup))
						{
							groups.Add(wordGroup);
						}
					}
					else
					{
						results.Add(wordCount, new List<string> { wordGroup });
					}
				}
			}

			return results;
		}
	}
}
