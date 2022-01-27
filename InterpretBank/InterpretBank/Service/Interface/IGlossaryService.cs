using System.Collections.Generic;
using InterpretBank.Model;

namespace InterpretBank.Service.Interface
{
	internal interface IGlossaryService
	{
		List<Term> GetTerms();

		List<LanguageEquivalent> GetTermsFromGlossaries(List<string> names);

		List<LanguageEquivalent> GetTermsFromTaggedLocations(List<string> tags);
	}
}