using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.SdlTmAnonymizer.Extensions;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.Community.SdlTmAnonymizer.Studio
{
	public class SelectedWordsFromUiElementVisitor : ISegmentElementVisitor
	{
		private readonly List<WordDetails> _selectedWordsDetails;
		/// <summary>
		/// All subsegments in current translation unit
		/// </summary>
		public List<object> SegmentColection { get; set; }
		public SelectedWordsFromUiElementVisitor(List<WordDetails> selectedWordsDetails)
		{
			_selectedWordsDetails = selectedWordsDetails;
		}

		public void VisitText(Text text)
		{
			var segmentCollection = new List<object>();
			foreach (var selectedWord in _selectedWordsDetails.ToList())
			{
				if (text.Value.Contains(selectedWord.Text))
				{
					SubsegmentSelectedData(text.Value, segmentCollection);
				}
			}
			SegmentColection = segmentCollection;
		}

		/// <summary>
		/// Splits segment text into words at selected words indexes
		/// </summary>
		/// <param name="segmentText"></param>
		/// <param name="segmentCollection"></param>
		private void SubsegmentSelectedData(string segmentText, ICollection<object> segmentCollection)
		{
			var wordsIndexes = new List<int>();
			var positionOfSelectedText = new List<int>();
			//for each selected word add start index, the lenght and the position of next word
			foreach (var selectedWord in _selectedWordsDetails)
			{
				wordsIndexes.Add(selectedWord.Position);
				wordsIndexes.Add(selectedWord.Length);
				if (selectedWord.NextWord.Length > 0)
				{
					wordsIndexes.Add(selectedWord.Length + selectedWord.NextWord.Length + 1);
				}
			}
			var segmentTextTrimed = segmentText.TrimStart();

			var indexesBiggerThanTextLenght = wordsIndexes.Count(i => i > segmentTextTrimed.Length);
			//that means selected text is the last word
			if (indexesBiggerThanTextLenght.Equals(wordsIndexes.Count))
			{
				var startIndex = segmentText.IndexOf(segmentTextTrimed, StringComparison.Ordinal);
				positionOfSelectedText.Add(startIndex);
				var elements = segmentText.SplitAt(positionOfSelectedText.ToArray()).ToList();

				//check if the last character is a punctuation sign 
				var lastCharacter = segmentTextTrimed[segmentTextTrimed.Length - 1];
				if (char.IsPunctuation(lastCharacter))
				{
					elements.Add(lastCharacter.ToString());
				}
				CreateSegmentCollection(elements, positionOfSelectedText, segmentCollection);
			}
			else
			{
				//split text to indexes
				var elementsCollection = segmentText.SplitAt(wordsIndexes.ToArray()).ToList();
				//if last character is punctuation sign add it to elements collection
				var lastCharacter = segmentTextTrimed[segmentTextTrimed.Length - 1];
				if (char.IsPunctuation(lastCharacter))
				{
					elementsCollection.Add(lastCharacter.ToString());
				}
				foreach (var selectedWord in _selectedWordsDetails)
				{
					for (var i = 0; i < elementsCollection.Count; i++)
					{
						//Check if is selected word
						if (selectedWord.Text.Equals(elementsCollection[i]))
						{
							//Check next word, trim at start because the word contains space at the begining
							if (!string.IsNullOrEmpty(selectedWord.NextWord))
							{
								if (elementsCollection[i + 1].TrimStart().Equals(selectedWord.NextWord))
								{
									positionOfSelectedText.Add(i);
								}
							}
							positionOfSelectedText.Add(i);
							break;
						}
					}
				}
				CreateSegmentCollection(elementsCollection, positionOfSelectedText, segmentCollection);
			}

			_selectedWordsDetails.Clear();
		}

		/// <summary>
		/// Transforms words list into Text and Tags 
		/// </summary>
		/// <param name="elementsCollection">List of words</param>
		/// <param name="positionOfSelectedText">List with the positions of selected words in Preview Window</param>
		/// <param name="segmentCollection">List with Text and Tags</param>
		private void CreateSegmentCollection(IReadOnlyList<string> elementsCollection, ICollection<int> positionOfSelectedText, ICollection<object> segmentCollection)
		{
			for (var i = 0; i < elementsCollection.Count; i++)
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
