using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Sdl.Community.TmAnonymizer.Model;
using Sdl.Community.TmAnonymizer.ViewModel;

namespace Sdl.Community.TmAnonymizer.Ui
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
			var docStart = _textBox.Document.ContentStart;
			var start = _textBox.Selection.Start;
			var end = _textBox.Selection.End;

			var tr = new TextRange(start, end);
			tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.LightSalmon);

			var dataContext = _textBox.DataContext as SourceSearchResult;
			var position = new Position
			{
				Index = docStart.GetOffsetToPosition(start),
				Length = docStart.GetOffsetToPosition(end)
			};
			dataContext?.SelectedWordsIndex.Add(position.Index);
		}

		private void UnselectWord(object sender, RoutedEventArgs e)
		{
			var docStart = _textBox.Document.ContentStart;
			var start = _textBox.Selection.Start;
			var end = _textBox.Selection.End;

			var tr = new TextRange(start, end);
			tr.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.White);
		}
	}
}
