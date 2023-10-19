using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossarySniffer
    {
        (string source, string target, char delimiter) GetGlossaryFileMetadata(string filename, List<string> supportedLanguages);
    }
}