using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Interface;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class GlossarySniffer : IGlossarySniffer
    {
        public (string source, string target, char delimiter) GetGlossaryFileMetadata(string filename, List<string> supportedLanguages)
        {
            var (success, result, _) = ErrorHandler.WrapTryCatch(() =>
            {
                using var reader = new StreamReader(filename, Encoding.Default);
                var line = reader.ReadLine();

                return GetMetadata(line, supportedLanguages);
            });

            return success ? result : default;
        }

        private (string source, string target, char delimiter) GetMetadata(string line, List<string> supportedLanguages)
        {
            var possibleDelimiters = new List<char> { ',', ';', '\t' };
            char delimiter = default;

            foreach (var d in possibleDelimiters)
            {
                var split = line.ToLower().Split(d);

                if (split.Length is not (2 or 4)) continue;

                if (split.Length == 4)
                {
                    if (supportedLanguages.Contains(split[2]) &&
                        supportedLanguages.Contains(split[3]))
                    {
                        return (split[2], split[3], d);
                    }

                    continue;
                }

                delimiter = d;
            }

            return (null, null, delimiter);
        }
    }
}