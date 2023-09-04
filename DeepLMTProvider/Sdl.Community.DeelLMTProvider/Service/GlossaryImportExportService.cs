using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryImportExportService : IGlossaryBrowserService
    {
        public GlossaryImportExportService(IBrowseDialog browseDialog)
        {
            BrowseDialog = browseDialog;
        }

        private IBrowseDialog BrowseDialog { get; }

        public (bool Success, string Path) OpenExportDialog()
        {
            var result = BrowseDialog.ShowSaveDialog();
            return result == null ? (false, null) : (true, result);
        }

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

        public bool OpenNewGlossaryDialog(List<string> existingGlossaryNames, List<string> supportedLanguages, out GlossaryItem glossary)
        {
            glossary = default;

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, null, true);
            browseGlossaryWindow.AddGlossaries(new[] { FindAvailableNewGlossaryName(existingGlossaryNames) });

            if (!(browseGlossaryWindow.ShowDialog() ?? false)) return false;
            glossary = browseGlossaryWindow.Glossaries[0];
            
            return true;
        }

        private string FindAvailableNewGlossaryName(List<string> existingGlossaryNames, int i = 0) =>
            existingGlossaryNames.Any(gi => gi == $"New glossary {i}")
                ? FindAvailableNewGlossaryName(existingGlossaryNames, i++)
                : $"New Glossary {i}";
    }
}