using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
    public  class PersonalInformation
    {
	    private List<Rule> _patterns;
		public PersonalInformation(List<Rule> patterns)
		{
			_patterns = patterns;
		}
		
		public bool ContainsPi(string text)
	    {
		    foreach (var pattern in _patterns)
		    {
			    var regex = new Regex(pattern.Name, RegexOptions.IgnoreCase);
			    var match = regex.Match(text);
			    if (match.Success)
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public  List<Position> GetPersonalDataPositions(string text)
	    {
		    var personalDataIndex = new List<Position>();
		    foreach (var pattern in _patterns)
		    {
			    var regex = new Regex(pattern.Name, RegexOptions.IgnoreCase);
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
