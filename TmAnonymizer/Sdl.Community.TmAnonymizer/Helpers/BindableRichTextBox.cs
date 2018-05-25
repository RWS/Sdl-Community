using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
    public class BindableRichTextBox: RichTextBox
	{
		public static readonly DependencyProperty DocumentProperty =
			DependencyProperty.Register("Document", typeof(FlowDocument),
				typeof(BindableRichTextBox), new FrameworkPropertyMetadata
					(null, new PropertyChangedCallback(OnDocumentChanged)));

		public new FlowDocument Document
		{
			get
			{
				return (FlowDocument)this.GetValue(DocumentProperty);
			}

			set
			{
				this.SetValue(DocumentProperty, value);
			}
		}

		public static void OnDocumentChanged(DependencyObject obj,
			DependencyPropertyChangedEventArgs args)
		{
			RichTextBox rtb = (RichTextBox)obj;
			var dataContext = (SourceSearchResult)rtb.DataContext;

			rtb.Document = (FlowDocument)args.NewValue;
			rtb.SelectAll();
			var textRange = rtb.Selection;
			var start = rtb.Document.ContentStart;
			var startPos = start.GetPositionAtOffset(0,LogicalDirection.Forward);
			//daca lungimea e 6, se selecteaza doar 4
			var endPos = start.GetPositionAtOffset(6+2,LogicalDirection.Forward);
			textRange.Select(startPos, endPos);
			textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));

			//textRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
		}
	}
}
