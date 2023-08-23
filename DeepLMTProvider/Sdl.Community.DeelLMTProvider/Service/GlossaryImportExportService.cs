using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryImportExportService : IGlossaryBrowserService
    {
        public GlossaryImportExportService(IBrowseDialog browseDialog)
        {
            BrowseDialog = browseDialog;
        }

        private IBrowseDialog BrowseDialog { get; }

        public bool OpenImportDialog(List<string> supportedLanguages, out List<GlossaryItem> glossaries)
        {
            glossaries = default;

            if (BrowseDialog.ShowDialog() != true) return false;

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, BrowseDialog);
            browseGlossaryWindow.AddGlossaries(BrowseDialog.FileNames);
            if (!(browseGlossaryWindow.ShowDialog() ?? false))
                return false;

            glossaries = new List<GlossaryItem>(browseGlossaryWindow.Glossaries);
            return true;
        }

        public (bool Success, string Path) OpenExportDialog()
        {
            var result = BrowseDialog.ShowSaveDialog();
            return result == null ? (false, null) : (true, result);
        }
    }
}