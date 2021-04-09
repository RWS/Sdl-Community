using System.Collections.Generic;

namespace MultiTermSearchExample.Model
{
	public class TermEntry
	{
		public string Term { get; set; }
		
		public string Language { get; set; }
		
		public List<EntryField> IndexFields { get; set; }
		
		public List<EntryField> TermFields { get; set; }
		
		public bool ReadOnly { get; set; }
	}
}
