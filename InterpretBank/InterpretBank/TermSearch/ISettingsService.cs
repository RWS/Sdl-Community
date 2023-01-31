using System.Collections.Generic;

namespace InterpretBank.TermSearch
{
	public interface ISettingsService
	{
		List<int> LanguageIndices { get; set; }
		List<string> GlossaryNames { get; set; }
		List<string> Tags { get; set; }
	}
}