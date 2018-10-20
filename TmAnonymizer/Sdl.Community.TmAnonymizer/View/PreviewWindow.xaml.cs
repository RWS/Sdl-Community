using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.ViewModel;

namespace Sdl.Community.SdlTmAnonymizer.View
{
	/// <summary>
	/// Interaction logic for PreviewWindow.xaml
	/// </summary>
	public partial class PreviewWindow
	{
		private RichTextBox _textBox;		

		public PreviewWindow()
		{
			InitializeComponent();			
		}

		private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var rtb = sender as RichTextBox;
			_textBox = rtb;
		}

		private void MenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				var docStart = _textBox.Document.ContentStart;
				var start = _textBox.Selection.Start;
				var end = _textBox.Selection.End;

				var parent = (RichTextBox)_textBox.Document.Parent;
				var tag = string.Empty;
				if (parent != null)
				{
					tag = (string)parent.Tag;
				}

				var tr = new TextRange(start, end);
				tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightSalmon);

				var dataContext = _textBox.DataContext as ContentSearchResult;
				var startRange = new TextRange(docStart, start);
				var indexStartAbs = startRange.Text.Length;
				var text = new TextRange(docStart, _textBox.Document.ContentEnd).Text.TrimEnd();
				var wordDetails = new WordDetails
				{
					Position = indexStartAbs,
					Length = indexStartAbs + _textBox.Selection.Text.TrimEnd().Length,
					Text = _textBox.Selection.Text.TrimEnd()

				};
				var nextWord = GetNextWord(wordDetails, text);
				wordDetails.NextWord = nextWord;
				if (tag.Equals("SourceBox"))
				{
					dataContext?.SelectedWordsDetails.Add(wordDetails);
				}
				else
				{
					dataContext?.TargetSelectedWordsDetails.Add(wordDetails);
				}
			}
			catch (Exception exception)
			{
				// ignored
			}
		}

		private string GetNextWord(WordDetails wordDetails, string text)
		{
			var splitedWord = string.Empty;
			if (wordDetails.Length.Equals(text.Length))
			{
				splitedWord = text.Substring(wordDetails.Length);
			}
			else
			{
				splitedWord = text.Substring(wordDetails.Length + 1);
			}
			if (!string.IsNullOrEmpty(splitedWord))
			{
				return splitedWord.Substring(0, splitedWord.IndexOf(" ", StringComparison.Ordinal));
			}
			//that means selected word is the last one
			return string.Empty;
		}

		private void UnselectWord(object sender, RoutedEventArgs e)
		{
			var docStart = _textBox.Document.ContentStart;
			var start = _textBox.Selection.Start;
			var end = _textBox.Selection.End;

			var parent = (RichTextBox)_textBox.Document.Parent;
			var tag = string.Empty;
			if (parent != null)
			{
				tag = (string)parent.Tag;
			}
			var tr = new TextRange(start, end);
			tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
			var dataContext = _textBox.DataContext as ContentSearchResult;

			var startRange = new TextRange(docStart, start);
			var indexStartAbs = startRange.Text.Length;
			var text = new TextRange(docStart, _textBox.Document.ContentEnd).Text.TrimEnd();

			var wordDetails = new WordDetails
			{
				Position = indexStartAbs,
				Length = indexStartAbs + _textBox.Selection.Text.TrimEnd().Length,
				Text = _textBox.Selection.Text.TrimEnd()
			};
			var prevWord = GetPreviosWord(wordDetails, text);
			wordDetails.PreviousWord = prevWord;
			if (tag.Equals("SourceBox"))
			{
				dataContext?.DeSelectedWordsDetails.Add(wordDetails);
			}
			else
			{
				dataContext?.TargetDeSelectedWordsDetails.Add(wordDetails);
			}
		}

		private string GetPreviosWord(WordDetails wordDetails, string text)
		{
			//if is first word we don't have prev word
			if (wordDetails.Position.Equals(0))
			{
				return string.Empty;
			}
			var firstPartOfString = text.Substring(0, wordDetails.Position).TrimEnd();
			var lastIndexOfSpace = firstPartOfString.LastIndexOf(" ", StringComparison.Ordinal);
			if (lastIndexOfSpace != -1)
			{
				return firstPartOfString.Substring(lastIndexOfSpace);
			}
			//if user deselected only a part from the word
			return string.Empty;
		}

		private void AnonymizeAction(object sender, RoutedEventArgs e)
		{
			((PreviewWindowViewModel)DataContext).ApplyChanges();
		}
	}
}

