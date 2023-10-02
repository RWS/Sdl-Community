using System.Collections.Generic;

namespace InterpretBank.Interface
{
    public interface IDialogService
    {
        public string GetGlossaryNameFromUser(List<string> glossaries);
    }
}