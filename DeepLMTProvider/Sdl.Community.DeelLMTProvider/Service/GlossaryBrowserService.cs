using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryBrowserService : IGlossaryBrowserService
    {
        public GlossaryBrowserService(IBrowseDialog openFileDialog)
        {
            OpenFileDialog = openFileDialog;
        }

        private IBrowseDialog OpenFileDialog { get; }

        public bool Browse(List<string> supportedLanguages, out List<GlossaryItem> glossaries)
        {
            glossaries = default;

            if (OpenFileDialog.ShowDialog() != true) return false;

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, OpenFileDialog);
            browseGlossaryWindow.AddGlossaries(OpenFileDialog.FileNames);
            if (!(browseGlossaryWindow.ShowDialog() ?? false))
                return false;

            glossaries = new List<GlossaryItem>(browseGlossaryWindow.Glossaries);
            return true;
        }

        public (bool Success, string Path) OpenExportDialog()
        {
            var result = OpenFileDialog.ShowSaveDialog();
            return result == null ? (false, null): (true, result);
        }
    }
}