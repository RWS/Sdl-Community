using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Services
{
	public class ContentParsingService
	{
		/// <summary>
		/// Gets a list with the positions and lengths of PI in text 
		/// </summary>
		/// <param name="text">Segment plain text</param>
		/// <param name="patterns"></param>
		/// <returns>List of PI positions</returns>
		public List<Position> GetMatchPositions(string text, List<Rule> patterns)
		{
			return (from pattern in patterns.Where(a => a.IsSelected)
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
