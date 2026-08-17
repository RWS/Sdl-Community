using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Service;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossaryReaderWriterFactory
    {
        ActionResult<IGlossaryReaderWriter> CreateFileReader(string filePath, string delimiter = default);
        ActionResult<IGlossaryReaderWriter> CreateGlossaryWriter(GlossaryReaderWriterService.Format format);
    }
}