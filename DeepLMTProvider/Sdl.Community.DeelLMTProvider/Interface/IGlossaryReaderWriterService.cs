using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryReaderWriterService
    {
        ActionResult<Glossary> ReadGlossary(string filePath);
        ActionResult<Glossary> WriteGlossary(GlossaryInfo selectedGlossary);
    }
}