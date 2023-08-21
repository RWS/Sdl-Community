using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossaryReaderWriterService : IGlossaryReaderWriterService
    {
        public enum Format
        {
            TSV,
            CSV,
            XLSX
        }
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

        public ActionResult<Glossary> WriteGlossary(Glossary selectedGlossary, Format format, string filePath)
        {
            var (success, glossaryReaderWriter, message) = GlossaryReaderWriterFactory.CreateGlossaryWriter(format);
            return !success ? new(false, null, message) : glossaryReaderWriter.WriteGlossary(selectedGlossary, filePath);
        }
    }
}