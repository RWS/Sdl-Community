using System.Collections.Generic;

namespace InterpretBank.Model
{
    public class GlossaryImport
    {
        public GlossaryImport(string[][] import)
        {
            Import = import;
            Count = import.Length;
            for (var i = 0; i < import[0].Length - 1; i += 3) Languages.Add(import[0][i]);
        }

        public int Count { get; set; }

        public List<string> Languages { get; } = new();
        private string[][] Import { get; set; }

        public string[] this[int entryNumber, string language]
        {
            get
            {
                var termEntry = Import[entryNumber + 1];
                var arrayLanguageIndex = Languages.IndexOf(language) * 3;

                var languageEquivalent = new string[3];
                for (int i = arrayLanguageIndex, j = 0; i < 3; i++, j++)
                    languageEquivalent[j] = termEntry[i];

                return languageEquivalent;
            }
        }

        public string[] this[int entryNumber] => Import[entryNumber + 1];

        public string GetTermComment(int entryNumber) => Import[entryNumber][Languages.Count * 3];
    }
}