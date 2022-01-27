using System.Collections.Generic;

namespace InterpretBank.Model
{
	public class Term
	{
		public string CommentAll { get; set; }
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }
		public List<LanguageEquivalent> LanguageEquivalents { get; set; }
	}
}