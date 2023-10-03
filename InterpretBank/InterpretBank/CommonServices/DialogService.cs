using InterpretBank.Interface;
using InterpretBank.TermbaseViewer.UI;
using System.Collections.Generic;

namespace InterpretBank.CommonServices
{
    public class DialogService : IDialogService
    {
        public string GetGlossaryNameFromUser(List<string> glossaries)
        {
            var chooseGlossaryWindow = new ChooseGlossaryWindow(glossaries);
            return chooseGlossaryWindow.ShowDialog() ?? false ? chooseGlossaryWindow.SelectedGlossary : null;
        }
    }
}