using System.Collections.Generic;

namespace InterpretBank.TermSearch
{
	public class SettingsService : ISettingsService
	{
		public List<string> GlossaryNames { get; set; }
		public List<int> LanguageIndices { get; set; }
		public List<string> Tags { get; set; }
	}
}