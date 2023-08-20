using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryReaderWriterService : IGlossaryReaderWriterService
    {
        public GlossaryReaderWriterService(IGlossaryReaderWriterFactory glossaryReaderWriterFactory)
        {
            GlossaryReaderWriterFactory = glossaryReaderWriterFactory;
        }

        private IGlossaryReaderWriterFactory GlossaryReaderWriterFactory { get; }

        public ActionResult<Glossary> ReadGlossary(string filePath)
        {
            var (success, glossaryReaderWriter, message) = GlossaryReaderWriterFactory.CreateFileReader(filePath);
            return !success ? new(false, null, message) : glossaryReaderWriter.ReadGlossary(filePath);
        }

        public ActionResult<Glossary> WriteGlossary(GlossaryInfo selectedGlossary)
        {
            throw new System.NotImplementedException();
        }
    }
}