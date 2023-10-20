using System.Collections.Generic;

namespace InterpretBank.Model
{
    public class GlossaryImport
    {
        public GlossaryImport(string[][] import)
        {
            for (var i = 0; i < import[0].Length - 1; i += 3) Languages.Add(import[0][i]);
        }

        private List<string> Languages { get; } = new();
    }
}