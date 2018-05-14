using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.TmAnonymizer.Model;
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
		    if (string.IsNullOrEmpty(text)) return;
		    var textRange = new TextRange(document.ContentStart, document.ContentEnd)
		    {
			    Text = text
		    };
		    var dataContex = (SourceSearchResult) document.DataContext;
		    if (dataContex == null) return;
		    foreach (var matchPosition in dataContex.MatchResult.Positions)
		    {
			    var initialPointer = document.ContentStart;
			    var start = GetPoint(initialPointer, matchPosition.Index);
				var endPos = GetPoint(initialPointer, matchPosition.Index + matchPosition.Length);

				//var start = document.ContentStart.GetPositionAtOffset(matchPosition.Index);
				//var endPos = document.ContentStart.GetPositionAtOffset(matchPosition.Index+matchPosition.Length);
				if (start == null || endPos == null) continue;
			    textRange.Select(start, endPos);
			    var color = (SolidColorBrush)new BrushConverter().ConvertFrom("#3D9DAA");
			    if (color != null)
			    {
				    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, color);
			    }
		    }
		    //var start = document.ContentStart.GetPositionAtOffset(0, LogicalDirection.Forward);
		    //var endPos = document.ContentStart.GetPositionAtOffset(6 + 2, LogicalDirection.Forward);
		    //textRange.Select(start, endPos);
		    //textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Red));


		    //new TextRange(document.ContentStart, document.ContentEnd).Text = text;
		   
	    }
	    private static TextPointer GetPoint(TextPointer start, int x)
	    {
		    var ret = start;
		    var i = 0;
		    while (i < x && ret != null)
		    {
			    if (ret.GetPointerContext(LogicalDirection.Backward) ==
			        TextPointerContext.Text ||
			        ret.GetPointerContext(LogicalDirection.Backward) ==
			        TextPointerContext.None)
				    i++;
			    if (ret.GetPositionAtOffset(1,
				        LogicalDirection.Forward) == null)
				    return ret;
			    ret = ret.GetPositionAtOffset(1,
				    LogicalDirection.Forward);
		    }
		    return ret;
	    }
	}
}
