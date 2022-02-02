using System.Collections.Generic;

namespace InterpretBank.Model
{
	public class TermEntry
	{
		public string CommentAll { get; set; }
		public string Id { get; set; }
		public List<LanguageEquivalent> LanguageEquivalents { get; set; } = new();
		public string Tag1 { get; set; }
		public string Tag2 { get; set; }

		public void Add(LanguageEquivalent languageEquivalent)
		{
			LanguageEquivalents.Add(languageEquivalent);
		}
	}
}