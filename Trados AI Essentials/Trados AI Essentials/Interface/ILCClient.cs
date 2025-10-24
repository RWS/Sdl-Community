using System.Collections.Generic;
using System.Threading.Tasks;
using Trados_AI_Essentials.Model;

namespace Trados_AI_Essentials.Interface
{
    public interface ILCClient
    {
        void Authenticate();

        Task<GenerativeTranslationResult> TranslateAsync(TranslationRequest request);
        Task<List<TranslationEngineItem>> GetLLMTranslationEngines();
    }
}