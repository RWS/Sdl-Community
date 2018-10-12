using System.Windows.Documents;
using System.Windows.Media;
using Sdl.Community.SdlTmAnonymizer.Model;
using Xceed.Wpf.Toolkit;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
    public class RichTextBoxFormatter: ITextFormatter
    {
	    public string GetText(FlowDocument document)
	    {
			return new TextRange(document.ContentStart, document.ContentEnd).Text;
		}

		/// <summary>
		/// Set background color for matching expressions in preview grid
		/// </summary>
		/// <param name="document"></param>
		/// <param name="text"></param>
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
			    if (dataContex.MatchResult != null)
			    {
				    foreach (var matchPosition in dataContex.MatchResult.Positions)
				    {
					    var initialPointer = document.ContentStart;
					    var start = CustomTextBox.GetPoint(initialPointer, matchPosition.Index);
					    var endPos = CustomTextBox.GetPoint(initialPointer, matchPosition.Index + matchPosition.Length);

					    if (start == null || endPos == null) continue;
					    textRange.Select(start, endPos);
					    var color = (SolidColorBrush) new BrushConverter().ConvertFrom("#EAC684");
					    if (color != null)
					    {
						    textRange.ApplyPropertyValue(TextElement.BackgroundProperty, color);
					    }
				    }
			    }
		    }
		    if (tag.Equals("TargetBox"))
		    {
			    if (dataContex.TargetMatchResult != null)
			    {
					foreach (var matchPosition in dataContex.TargetMatchResult.Positions)
					{
						var initialPointer = document.ContentStart;
						var start = CustomTextBox.GetPoint(initialPointer, matchPosition.Index);
						var endPos = CustomTextBox.GetPoint(initialPointer, matchPosition.Index + matchPosition.Length);

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
		}
	}
}
