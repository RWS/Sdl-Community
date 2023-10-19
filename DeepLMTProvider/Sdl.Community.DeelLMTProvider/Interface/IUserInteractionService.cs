using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IUserInteractionService
    {
        bool OpenImportDialog(List<string> supportedLanguages, out List<GlossaryItem> glossaries);
        (bool Success, string Path) OpenExportDialog();
        bool OpenNewGlossaryDialog(List<string> existingGlossaryNames, List<string> supportedLanguages, out GlossaryItem glossary);
        bool OpenImportEntriesDialog(out List<string> fileNames);
        //char GetDelimiterFromUser();
    }
}