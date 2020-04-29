using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Model
{
	public class TermResultGroup
	{
		public int Id { get; set; }
		public List<ISearchResult> Results { get; set; }
	}
}
