using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryBrowserService : IGlossaryBrowserService
    {
        public GlossaryBrowserService(IOpenFileDialog openFileDialog)
        {
            OpenFileDialog = openFileDialog;
        }

        private IOpenFileDialog OpenFileDialog { get; }

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
    }
}