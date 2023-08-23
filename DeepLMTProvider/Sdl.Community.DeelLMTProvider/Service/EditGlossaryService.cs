using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class EditGlossaryService : IEditGlossaryService
    {
        public List<GlossaryEntry> GlossaryEntries => new(EditGlossaryWindow.GlossaryEntries);
        private EditGlossaryWindow EditGlossaryWindow { get; set; }

        public bool EditGlossary(List<GlossaryEntry> glossaryEntries)
        {
            EditGlossaryWindow = new EditGlossaryWindow(glossaryEntries);
            return EditGlossaryWindow.ShowDialog() ?? false;
        }
    }
}