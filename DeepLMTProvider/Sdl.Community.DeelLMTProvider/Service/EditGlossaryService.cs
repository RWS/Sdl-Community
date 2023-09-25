using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.UI;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class EditGlossaryService : IEditGlossaryService
    {
        public List<GlossaryEntry> GlossaryEntries
        {
            get
            {
                var entries = EditGlossaryWindow.GlossaryEntries;
                entries.ForEach(e =>
                {
                    e.SourceTerm = e.SourceTerm.Trim();
                    e.TargetTerm = e.TargetTerm.Trim();
                });

                return new(entries);
            }
        }

        public string GlossaryName => EditGlossaryWindow.GlossaryName.Trim();
        private EditGlossaryWindow EditGlossaryWindow { get; set; }

        public bool EditGlossary(List<GlossaryEntry> glossaryEntries, string glossaryName)
        {
            EditGlossaryWindow = new EditGlossaryWindow(glossaryEntries, glossaryName);
            return EditGlossaryWindow.ShowDialog() ?? false;
        }
    }
}