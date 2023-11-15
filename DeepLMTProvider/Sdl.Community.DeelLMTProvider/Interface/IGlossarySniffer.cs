using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IGlossarySniffer
    {
        public (string Source, string Target, string Delimiter) GetGlossaryFileMetadata(string filename, List<string> supportedLanguages = null);
    }
}