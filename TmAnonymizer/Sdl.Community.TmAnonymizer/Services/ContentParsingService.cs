using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class ContentParsingService
	{
		//TODO - reorganize and optimize usage; too many separate calls with Regex

		private readonly List<Rule> _patterns;

		public ContentParsingService(List<Rule> patterns)
		{
			_patterns = patterns;
		}

		/// <summary>
		/// Gets a list with the positions and lenghts of PI in text 
		/// </summary>
		/// <param name="text">Segment plain text</param>
		/// <returns>List of PI positions</returns>
		public List<Position> GetMatchPositions(string text)
		{
			return (from pattern in _patterns.Where(a => a.IsSelected)
				select new Regex(pattern.Name, RegexOptions.IgnoreCase)
				into regex
				from Match match in regex.Matches(text)
				select new Position
				{
					Length = match.Length,
					Index = match.Index
				}).ToList();
		}
	}
}
