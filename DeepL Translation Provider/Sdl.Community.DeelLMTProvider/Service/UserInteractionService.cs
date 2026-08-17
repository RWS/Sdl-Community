using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;
using System.IO;
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

        public bool OpenImportEntriesDialog(out List<GlossaryDelimiterItem> glossaryDelimiterItems)
        {
            glossaryDelimiterItems = default;

            if (BrowseDialog.ShowDialog() != true) return false;

            var fileNames = BrowseDialog.FileNames.ToList();

            var glossarySniffer = new GlossarySniffer();
            var glossaryDelimiters = fileNames.Select(fn =>
            {
                var metadata = glossarySniffer.GetGlossaryFileMetadata(fn);
                return new GlossaryDelimiterItem(fn, metadata.Delimiter);
            }).ToList();

            var importEntriesWindow = new ImportEntriesWindow(glossaryDelimiters);
            glossaryDelimiterItems = importEntriesWindow.Glossaries;

            return importEntriesWindow.ShowDialog() ?? false;
        }

        public bool OpenNewGlossaryDialog(List<string> existingGlossaryNames, List<string> supportedLanguages, out GlossaryItem glossary)
        {
            glossary = default;

            var browseGlossaryWindow = new BrowseGlossariesWindow(supportedLanguages, new GlossarySniffer());
            browseGlossaryWindow.AddGlossaries(new[] { FindAvailableNewGlossaryName(existingGlossaryNames) });

            if (!(browseGlossaryWindow.ShowDialog() ?? false)) return false;
            glossary = browseGlossaryWindow.Glossaries[0];

            return true;
        }

        private string FindAvailableNewGlossaryName(List<string> existingGlossaryNames)
        {
            var i = 0;
            while (true)
            {
                var newName = $"New Glossary {i}";
                if (existingGlossaryNames.All(gi => gi != newName))
                    return newName;

                ++i;
            }
        }
    }
}