using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Sdl.Community.TmAnonymizer.Helpers
{
    public class RichTextBoxFormatter: ITextFormatter
    {
	    public string GetText(FlowDocument document)
	    {
			return new TextRange(document.ContentStart, document.ContentEnd).Text;
		}

	    public void SetText(FlowDocument document, string text)
	    {
		    var textRange = new TextRange(document.ContentStart, document.ContentEnd)
		    {
			    Text = text
		    };
		    var start = document.ContentStart.GetPositionAtOffset(0, LogicalDirection.Forward);
		   var endPos = document.ContentStart.GetPositionAtOffset(6 + 2, LogicalDirection.Forward);
			textRange.Select(start, endPos);
		    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));

			//new TextRange(document.ContentStart, document.ContentEnd).Text = text;
		   
	    }
    }
}
