using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class EditGlossaryService : IEditGlossaryService
    {
        public List<GlossaryEntry> GlossaryEntries => new(EditGlossaryWindow.GlossaryEntries);

        public string GlossaryName => EditGlossaryWindow.GlossaryName;
        private EditGlossaryWindow EditGlossaryWindow { get; set; }

        public bool EditGlossary(List<GlossaryEntry> glossaryEntries, string glossaryName)
        {
            EditGlossaryWindow = new EditGlossaryWindow(glossaryEntries, glossaryName);
            return EditGlossaryWindow.ShowDialog() ?? false;
        }
    }
}