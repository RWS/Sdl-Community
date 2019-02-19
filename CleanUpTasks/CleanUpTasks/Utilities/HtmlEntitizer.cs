using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public class HtmlEntitizer 
	{	 
        public string Entitize(string input, string searchText)
        {	
	        var splitedList = GetOriginalTextSplited(input, searchText);
			 var builder = new StringBuilder();
	        foreach (var text in splitedList)
	        {
		        if (!IsTag(text))
		        {
			        var updatedString = text.Replace("<", "&lt;");
			        builder.Append(updatedString);
		        }
		        else
		        {
					builder.Append(text);	 
				}
			}	  
	        return builder.ToString();
        }

	    private bool IsTag(string text)
	    {
		    return text.Contains('<') && text.Contains('>');
	    }

	    public List<string> GetOriginalTextSplited(string input, string searchText)
	    {
			var matchesIndexs = new List<int>();
		    var matchCollection = Regex.Matches(input, searchText);
		    foreach (Match match in matchCollection)
		    {
			    if (match.Index != 0)
			    {
				    matchesIndexs.Add(match.Index);
			    }
			    matchesIndexs.Add(match.Index + match.Length);
		    }
		     return input.SplitAt(matchesIndexs.ToArray()).ToList();
		}

		public string DeEntitize(string input)
        {
	        return input.Replace("&lt;", "<");
        } 
    }
}