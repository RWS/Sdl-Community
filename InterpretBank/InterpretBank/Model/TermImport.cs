using System.Collections.Generic;

namespace InterpretBank.Model
{
    public class TermImport
    {
        public TermImport(List<string> languages)
        {
            Languages = languages;

        }



        public List<string> Languages { get; }
    }
}