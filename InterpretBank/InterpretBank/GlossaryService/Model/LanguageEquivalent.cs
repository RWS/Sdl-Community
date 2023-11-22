using System.Collections.Generic;

namespace InterpretBank.GlossaryService.Model
{
	public class LanguageEquivalent
	{
		public string Commenta { get; set; }
		public string Commentb { get; set; }
		public int LanguageIndex { get; set; }
		public string Term { get; set; }

		public static List<string> GetColumns(int languageIndex) => new()
		{
			$"Term{languageIndex}",
			$"Comment{languageIndex}a",
			$"Comment{languageIndex}b"
		};

		public List<string> GetColumns() => new()
		{
			$"TermEntry{LanguageIndex}",
			$"Comment{LanguageIndex}a",
			$"Comment{LanguageIndex}b"
		};

		public List<string> GetValues() =>
			new() { Term, Commenta, Commentb };
	}
}