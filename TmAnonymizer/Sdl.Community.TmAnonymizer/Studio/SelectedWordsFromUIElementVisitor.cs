using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.TmAnonymizer.Studio
{
	public class SelectedWordsFromUiElementVisitor : ISegmentElementVisitor
	{
		private List<WordDetails> _selectedWordsDetails;
		/// <summary>
		/// All subsegments in current translation unit
		/// </summary>
		public List<object> SegmentColection { get; set; }
		public SelectedWordsFromUiElementVisitor(List<WordDetails>selectedWordsDetails)
		{
			_selectedWordsDetails = selectedWordsDetails;
		}
		
		public void VisitText(Text text)
		{
			var segmentCollection = new List<object>();
			GetSubsegmentSelectedData(text.Value,segmentCollection);
			SegmentColection = segmentCollection;
		}

		private void GetSubsegmentSelectedData(string segmentText, List<object> segmentCollection)
		{
			var wordsIndexes = new List<int>();
			foreach (var selectedWord in _selectedWordsDetails)
			{
				wordsIndexes.Add(selectedWord.Position);
				wordsIndexes.Add(selectedWord.Length);
				wordsIndexes.Add(selectedWord.Length+selectedWord.NextWord.Length+1);
			}

			var positionOfSelectedText = new List<int>();

			var elementsCollection = segmentText.SplitAt(wordsIndexes.ToArray());
			foreach (var selectedWord in _selectedWordsDetails)
			{
				for (var i = 0; i < elementsCollection.Length; i++)
				{
					if (selectedWord.Text.Equals(elementsCollection[i]))
					{
						//check next word
						if (elementsCollection[i + 1].TrimStart().Equals(selectedWord.NextWord))
						{
							positionOfSelectedText.Add(i);
						}
						break;
					}
				}
			}
			_selectedWordsDetails.Clear();
			CreateSegmentCollection(elementsCollection, positionOfSelectedText, segmentCollection);
		}

		private void CreateSegmentCollection(string[] elementsCollection,List<int> positionOfSelectedText, List<object> segmentCollection)
		{
			for (int i = 0; i < elementsCollection.Length; i++)
			{
				var isTagAtPosition = positionOfSelectedText.Contains(i);
				if (!isTagAtPosition)
				{
					var text = new Text(elementsCollection[i]);
					segmentCollection.Add(text);
				}
				else
				{
					var tag = new Tag(TagType.TextPlaceholder, string.Empty, 1);
					segmentCollection.Add(tag);
				}

			}
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
