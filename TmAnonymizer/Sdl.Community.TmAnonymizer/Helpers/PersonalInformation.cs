using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.Helpers
{
	public class PersonalInformation
	{
		private readonly List<Rule> _patterns;
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

		/// <summary>
		/// Gets a list with the positions and lenghts of PI in text 
		/// </summary>
		/// <param name="text">Segment plain text</param>
		/// <returns>List of PI positions</returns>
		public List<Position> GetPersonalDataPositions(string text)
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
