using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryBrowserService
    {
        bool OpenImportDialog(List<string> supportedLanguages, out List<GlossaryItem> glossaries);
        (bool Success, string Path) OpenExportDialog();
    }
}