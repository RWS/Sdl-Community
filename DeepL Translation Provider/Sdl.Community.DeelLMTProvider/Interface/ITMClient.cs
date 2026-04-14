using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface ITMClient
    {
        Task<ActionResult<List<TranslationMemoryInfo>>> GetTranslationMemoriesAsync(string apiKey, bool continueOnCapturedContext = true);
    }
}
