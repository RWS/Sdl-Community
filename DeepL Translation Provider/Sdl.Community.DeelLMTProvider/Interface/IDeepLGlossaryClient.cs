using Sdl.Community.DeepLMTProvider.Extensions;
using Sdl.Community.DeepLMTProvider.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IDeepLGlossaryClient
    {
        string ApiVersion { get; set; }

        Task<ActionResult<List<GlossaryInfo>>> GetGlossaries(string apiKey, bool continueOnCapturedContext = true);
    }
}