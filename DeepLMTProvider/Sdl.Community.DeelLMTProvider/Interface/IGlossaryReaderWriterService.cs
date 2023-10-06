using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using Sdl.Community.DeepLMTProvider.Service;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryReaderWriterService
    {
        ActionResult<Glossary> ReadGlossary(string filePath, char delimiter = default);

        public ActionResult<Glossary> WriteGlossary(Glossary glossary, GlossaryReaderWriterService.Format format,
            string filePath);
    }
}