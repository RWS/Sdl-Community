using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	public class SegmentElementVisitor : ISegmentElementVisitor
	{
		private readonly List<WordDetails> _deSelectedWordsDetails;
		private readonly List<Rule> _rules;
		private readonly List<int> _anchorIds;

		public SegmentElementVisitor(List<WordDetails> deSelectedWords, List<int> anchorIds, List<Rule> rules)
		{
			_deSelectedWordsDetails = deSelectedWords;
			_anchorIds = anchorIds;
			_rules = rules;
			SegmentColection = new List<object>();
		}

		/// <summary>
		/// All subsegments in current translation unit
		/// </summary>
		public List<object> SegmentColection { get; set; }

		public void VisitText(Text text)
		{			
			var personalData = GetPersonalData(text.Value);
			if (personalData.Count > 0)
			{
				SegmentColection.AddRange(GetSubsegmentPi(text.Value, personalData));
			}
		}

		private int GetUniqueAnchorId()
		{
			for (var i = 1; i < 1000; i++)
			{
				if (!_anchorIds.Contains(i))
				{
					_anchorIds.Add(i);
					return i;
				}
			}

			return 0;
		}

		private IEnumerable<object> GetSubsegmentPi(string segmentText, List<int> personalData)
		{
			var segmentCollection = new List<object>();
			var elementsColection = segmentText.SplitAt(personalData.ToArray());

			for (var i = 0; i < elementsColection.Length; i++)
			{
				if (!string.IsNullOrEmpty(elementsColection[i]))
				{
					var shouldAnonymize = i != 0
						? ShouldAnonymize(elementsColection[i], elementsColection[i - 1])
						: ShouldAnonymize(elementsColection[i], string.Empty);

					//refactor the code put in method
					if (shouldAnonymize)
					{
						//create new tag
						var anchorId = GetUniqueAnchorId();
						var tag = new Tag(TagType.TextPlaceholder, string.Empty, anchorId);

						segmentCollection.Add(tag);
					}
					else
					{
						//create text
						var text = new Text(elementsColection[i]);
						segmentCollection.Add(text);
					}
				}
			}

			return segmentCollection;
		}

		/// <summary>
		/// Gets a list with the index of PI in segment
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private List<int> GetPersonalData(string text)
		{
			var personalDataIndex = new List<int>();

			foreach (var rule in _rules.Where(a => a.IsSelected))
			{
				var regex = new Regex(rule.Name, RegexOptions.IgnoreCase);
				var matches = regex.Matches(text);

				foreach (System.Text.RegularExpressions.Match match in matches)
				{
					if (match.Index.Equals(0))
					{
						personalDataIndex.Add(match.Length);
					}
					else
					{
						//check if there is any text after PI
						var remainingText = text.Substring(match.Index + match.Length);
						if (!string.IsNullOrEmpty(remainingText))
						{
							//get the position where PI starts to split before
							personalDataIndex.Add(match.Index);

							//split after PI
							personalDataIndex.Add(match.Index + match.Length);
						}
						else
						{
							personalDataIndex.Add(match.Index);
						}
					}
				}
			}
			return personalDataIndex;
		}

		private bool ShouldAnonymize(string currentText, string prevText)
		{
			foreach (var rule in _rules.Where(a => a.IsSelected))
			{
				var regex = new Regex(rule.Name, RegexOptions.IgnoreCase);
				var matches = regex.Matches(currentText);

				foreach (System.Text.RegularExpressions.Match match in matches)
				{
					var matchesDeselected = _deSelectedWordsDetails.Where(n => n.Text.Equals(match.Value.TrimEnd())).ToList();
					if (matchesDeselected.Any())
					{
						//check the previous word to see if is the same word which user deselected
						if (!string.IsNullOrEmpty(prevText))
						{
							var combinedText = prevText + currentText;

							var firstPartOfString = combinedText.Substring(0, combinedText.IndexOf(match.Value, StringComparison.Ordinal)).TrimEnd();
							var prevWord = firstPartOfString.Substring(firstPartOfString.LastIndexOf(" ", StringComparison.Ordinal));

							var matchToBeDeselected = matchesDeselected.FirstOrDefault(w => w.PreviousWord.Equals(prevWord));
							if (matchToBeDeselected != null)
							{
								return false;
							}
						}
						else
						{
							//this means the unselected text is checked second time we need to skip it because we already created a text element
							return false;
						}
					}
					else
					{
						return true;
					}
				}
			}
			return false;
		}

		public void VisitTag(Tag tag)
		{
			// not required with this implementation
		}

		public void VisitDateTimeToken(DateTimeToken token)
		{
			// not required with this implementation
		}

		public void VisitNumberToken(NumberToken token)
		{
			// not required with this implementation
		}

		public void VisitMeasureToken(MeasureToken token)
		{
			// not required with this implementation
		}

		public void VisitSimpleToken(SimpleToken token)
		{
			// not required with this implementation
		}

		public void VisitTagToken(TagToken token)
		{
			// not required with this implementation
		}
	}
}
