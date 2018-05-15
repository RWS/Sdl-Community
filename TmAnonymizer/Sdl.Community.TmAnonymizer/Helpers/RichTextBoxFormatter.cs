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
		    var parent = (RichTextBox)document.Parent;
		    var tag = string.Empty;
		    if (parent != null)
		    {
			     tag = (string) parent.Tag;
		    }
		    if (tag.Equals("SourceBox"))
		    {
				foreach (var matchPosition in dataContex.MatchResult.Positions)
				{
					var initialPointer = document.ContentStart;
					var start = GetPoint(initialPointer, matchPosition.Index);
					var endPos = GetPoint(initialPointer, matchPosition.Index + matchPosition.Length);

					if (start == null || endPos == null) continue;
					textRange.Select(start, endPos);
					var color = (SolidColorBrush)new BrushConverter().ConvertFrom("#EAC684");
					if (color != null)
					{
						textRange.ApplyPropertyValue(TextElement.BackgroundProperty, color);
					}
				}
			}
		    if (tag.Equals("TargetBox"))
		    {
				foreach (var matchPosition in dataContex.TargetMatchResult.Positions)
				{
					var initialPointer = document.ContentStart;
					var start = GetPoint(initialPointer, matchPosition.Index);
					var endPos = GetPoint(initialPointer, matchPosition.Index + matchPosition.Length);

					if (start == null || endPos == null) continue;
					textRange.Select(start, endPos);
					var color = (SolidColorBrush)new BrushConverter().ConvertFrom("#EAC684");
					if (color != null)
					{
						textRange.ApplyPropertyValue(TextElement.BackgroundProperty, color);
					}
				}
			}
		 
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
