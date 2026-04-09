using Sdl.Community.DeepLMTProvider.Model;
using Sdl.LanguagePlatform.Core;
using System.Threading.Tasks;

namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface ILanguageValidationService
    {
        Task<LanguagePairValidationResult> ValidateAsync(
            LanguagePair languagePair,
            string apiKey);
    }
}
