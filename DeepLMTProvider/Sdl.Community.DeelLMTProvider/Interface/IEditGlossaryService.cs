using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IEditGlossaryService
    {
        List<GlossaryEntry> GlossaryEntries { get; }

        bool EditGlossary(List<GlossaryEntry> glossaryEntries);
    }
}