using System.Collections.Generic;
using InterpretBank.Model;
using InterpretBank.Model.Interface;

namespace InterpretBank.Service.Interface
{
	public interface IGlossaryService
	{
		List<IGlossaryEntry> GetTerms(string searchString = null, List<int> languages = null, List<string> glossaryNames = null, List<string> tags = null);

		void AddTerm(TermEntry termEntry);

		void UpdateTermContent(TermEntry termEntry);

		void MergeGlossaries(string firstGlossary, string secondGlossary, string subGlossary = null);

		void DeleteTerm(string termId);

		void CreateGlossary(List<int> languageIndices, List<string> tags, string note);

		List<IGlossaryEntry> GetGlossaries();
	}
}