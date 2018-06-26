using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using Sdl.Community.TmAnonymizer.Helpers;
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.ViewModel;

namespace Sdl.Community.TmAnonymizer.Ui
{
	/// <summary>
	/// Interaction logic for PreviewWindow.xaml
	/// </summary>
	public partial class PreviewWindow
	{
		public  IDialogCoordinator DialogCoordinatorWindow;
		private RichTextBox _textBox;
		public PreviewWindow()
		{
			InitializeComponent();
			DialogCoordinatorWindow =DialogCoordinator.Instance;
		}

		private void FrameworkElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			var rtb = sender as RichTextBox;
			_textBox = rtb;
		}

		private void MenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			var docStart = _textBox.Document.ContentStart;
			var start = _textBox.Selection.Start;
			var end = _textBox.Selection.End;

			var tr = new TextRange(start, end);
			tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightSalmon);

			var dataContext = _textBox.DataContext as SourceSearchResult;
			var startRange = new TextRange(docStart, start);
			var indexStartAbs = startRange.Text.Length;
			var text = new TextRange(docStart, _textBox.Document.ContentEnd).Text.TrimEnd();
			var wordDetails = new WordDetails
			{
				Position = indexStartAbs,
				Length = indexStartAbs+_textBox.Selection.Text.TrimEnd().Length, 
				Text = _textBox.Selection.Text.TrimEnd()
				
			};
			var nextWord = GetNextWord(wordDetails, text);
			wordDetails.NextWord = nextWord;
			dataContext?.SelectedWordsDetails.Add(wordDetails);
		}
			
		private string GetNextWord(WordDetails wordDetails,string text)
		{
			var splitedWord = text.Substring(wordDetails.Length+1);
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

			var tr = new TextRange(start, end);
			tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
			var dataContext = _textBox.DataContext as SourceSearchResult;

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
			dataContext?.DeSelectedWordsDetails.Add(wordDetails);
		}

		private string GetPreviosWord(WordDetails wordDetails, string text)
		{
			//if is first word we don't have prev word
			if (wordDetails.Position.Equals(0))
			{
				return string.Empty;
			}
			var firstPartOfString = text.Substring(0, wordDetails.Position).TrimEnd();
			return firstPartOfString.Substring(firstPartOfString.LastIndexOf(" ", StringComparison.Ordinal));
		}
	}
}
