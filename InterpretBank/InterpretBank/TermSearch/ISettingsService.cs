using System.Collections.Generic;
using InterpretBank.GlossaryService.DAL;

namespace InterpretBank.TermSearch
{
	public interface ISettingsService
	{
		List<int> LanguageIndices { get; set; }
		List<string> GlossaryNames { get; set; }
		List<DbTag> Tags { get; set; }
	}
}