using System;
using System.Collections.Generic;

namespace InterpretBank.GlossaryService.Interface
{
    public interface IGlossaryService : IDisposable
    {
        void Create(IGlossaryEntry termEntry);

        void CreateDb(string filePath);

        public void DeleteGlossary(int glossaryName);

        void DeleteTerm(string termId);

        List<IGlossaryEntry> GetGlossaries();

        List<IGlossaryEntry> GetTerms(string searchString = null, List<int> languages = null, List<string> glossaryNames = null, List<string> tags = null);

        void LoadDb(string filePath);

        void MergeGlossaries(string firstGlossary, string secondGlossary, string subGlossary = null);

        void UpdateContent(IGlossaryEntry entry);
    }
}