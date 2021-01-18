using System.Threading.Tasks;
using Sdl.Community.DeeplAddon.Models;

namespace Sdl.Community.DeeplAddon.Interfaces
{
	public interface ITranslationService
	{
		Task<TranslationResponse> Translate(TranslationRequest translationRequest, string apiKey,string folmality);
	}
}
