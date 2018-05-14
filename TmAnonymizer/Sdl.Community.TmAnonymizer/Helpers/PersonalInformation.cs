using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
    public  static class PersonalInformation
    {
	    private static List<string> _patterns = new List<string>
	    {
		    @"\b(?:\d[ -]*?){13,16}\b",//PCI (Payment Card Industry)
		    @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b",//Email addresses
		    @"\b[A-Z]{2}\s\d{2}\s\d{2}\s\d{2}\s[A-Z]\b",//UK National Insurance Number
		    @"\b(?!000)(?!666)[0-8][0-9]{2}[- ](?!00)[0-9]{2}[- ](?!0000)[0-9]{4}\b", //"Social Security Numbers"
	    };

		public static bool ContainsPi(string text)
	    {
		    foreach (var pattern in _patterns)
		    {
			    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
			    var match = regex.Match(text);
			    if (match.Success)
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public static List<Position> GetPersonalDataPositions(string text)
	    {
		    var personalDataIndex = new List<Position>();
		    foreach (var pattern in _patterns)
		    {
			    var regex = new Regex(pattern, RegexOptions.IgnoreCase);
			    var matches = regex.Matches(text);

			    foreach (Match match in matches)
			    {
				    var position = new Position
				    {
					    Length = match.Length,
					    Index = match.Index
				    };
				    personalDataIndex.Add(position);
			    }
		    }
		    return personalDataIndex;
	    }
	}
}
