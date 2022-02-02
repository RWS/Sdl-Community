using System.Collections.Generic;
using InterpretBank.Model;

namespace InterpretBank.Service.Interface
{
	internal interface IGlossaryService
	{
		List<TermEntry> GetTerms(string searchString = null, List<int> languages = null, List<string> glossaryNames = null, List<string> tags = null);

		void AddTerm(TermEntry termEntry);

		void UpdateTerm(TermEntry termEntry);
	}
}