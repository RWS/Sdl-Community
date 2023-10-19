using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class UserInteractionService : IUserInteractionService
    {
        public UserInteractionService(IBrowseDialog browseDialog)
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

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, BrowseDialog, new GlossarySniffer());
            browseGlossaryWindow.AddGlossaries(BrowseDialog.FileNames);
            if (!(browseGlossaryWindow.ShowDialog() ?? false))
                return false;

            glossaries = new List<GlossaryItem>(browseGlossaryWindow.Glossaries);
            return true;
        }

        public bool OpenImportEntriesDialog(out List<string> fileNames)
        {
            fileNames = new List<string>();
            if (BrowseDialog.ShowDialog() != true) return false;

            fileNames = BrowseDialog.FileNames.ToList();
            return true;
        }

        public bool OpenNewGlossaryDialog(List<string> existingGlossaryNames, List<string> supportedLanguages, out GlossaryItem glossary)
        {
            glossary = default;

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, null, new GlossarySniffer(), true);
            browseGlossaryWindow.AddGlossaries(new[] { FindAvailableNewGlossaryName(existingGlossaryNames) });

            if (!(browseGlossaryWindow.ShowDialog() ?? false)) return false;
            glossary = browseGlossaryWindow.Glossaries[0];

            return true;
        }

        private string FindAvailableNewGlossaryName(List<string> existingGlossaryNames, int i = 0)
        {
            return existingGlossaryNames.Any(gi => gi == $"New Glossary {i}")
                ? FindAvailableNewGlossaryName(existingGlossaryNames, ++i)
                : $"New Glossary {i}";
        }
    }
}