using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.TmAnonymizer.Studio
{
	public class SegmentElementVisitor : ISegmentElementVisitor
	{
		private List<string> _patterns = new List<string>
		{
			@"\b(?:\d[ -]*?){13,16}\b",//PCI (Payment Card Industry)
			@"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",//Email addresses
			@"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b",//UK National Insurance Number
			@"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b", //"Social Security Numbers"
		};
		public List<object> SegmentColection { get; set; }

		public void VisitText(Text text)
		{
			var segmentCollection = new List<object>();
			var containsPi = ContainsPi(text.Value);
			if (containsPi)
			{
				var personalData = GetPersonalData(text.Value);
				GetSubsegmentPi(text.Value, personalData, segmentCollection);
			}
			SegmentColection = segmentCollection;
		}

		private void GetSubsegmentPi(string segmentText, List<int> personalData, List<object> segmentCollection)
		{
			var elementsColection = segmentText.SplitAt(personalData.ToArray());
			foreach (var element in elementsColection)
			{
				if (!string.IsNullOrEmpty(element))
				{
					if (ContainsPi(element))
					{
						//create new tag
						var tag = new Tag(TagType.TextPlaceholder, string.Empty, 1);
						segmentCollection.Add(tag);
					}
					else
					{
						//create text
						var text = new Text(element);
						segmentCollection.Add(text);
					}
				}
			}
		}

		private List<int> GetPersonalData(string text)
		{
			var personalDataIndex = new List<int>();
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
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
		private bool ContainsPi(string text)
		{
			foreach (var pattern in _patterns)
			{
				var regex = new Regex(pattern, RegexOptions.IgnoreCase);
				var match = regex.Match(text);
				if (match.Success)
				{
					return true;
				}
			}
			return false;
		}

		public void VisitTag(Tag tag)
		{

		}

		public void VisitDateTimeToken(DateTimeToken token)
		{

		}

		public void VisitNumberToken(NumberToken token)
		{

		}

		public void VisitMeasureToken(MeasureToken token)
		{

		}

		public void VisitSimpleToken(SimpleToken token)
		{

		}

		public void VisitTagToken(TagToken token)
		{

		}

	}
}
