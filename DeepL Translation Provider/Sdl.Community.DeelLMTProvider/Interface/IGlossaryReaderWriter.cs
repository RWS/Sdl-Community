using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryReaderWriter
    {
        public ActionResult<Glossary> ReadGlossary(string filePath);

        public ActionResult<Glossary> WriteGlossary(Glossary glossary, string filePath);
    }
}