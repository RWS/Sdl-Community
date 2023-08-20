using Sdl.Community.DeepLMTProvider.Extensions;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryReaderWriterFactory
    {
        ActionResult<IGlossaryReaderWriter> CreateFileReader(string filePath);
    }
}