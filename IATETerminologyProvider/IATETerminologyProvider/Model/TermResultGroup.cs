using System.Collections.Generic;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class TermResultGroup
	{
		public int Id { get; set; }
		public List<SearchResultModel> Results { get; set; }
	}
}
