using System.Collections.Generic;
using InterpretBank.Model.Interface;

namespace InterpretBank.Service.Interface
{
	public interface IGlossaryService
	{
		void Create(IGlossaryEntry termEntry);

		void DeleteTerm(string termId);

		List<IGlossaryEntry> GetGlossaries();

		List<IGlossaryEntry> GetTerms(string searchString = null, List<int> languages = null, List<string> glossaryNames = null, List<string> tags = null);

		void MergeGlossaries(string firstGlossary, string secondGlossary, string subGlossary = null);

		void UpdateContent(IGlossaryEntry entry);

		void DeleteGlossary(string glossaryId);
	}
}