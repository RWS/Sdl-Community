using System.Threading.Tasks;
using Sdl.LC.AddonBlueprint.Models;

namespace Sdl.LC.AddonBlueprint.Interfaces
{
	public interface ITranslationService
	{
		Task<TranslationResponse> Translate(TranslationRequest translationRequest, string apiKey,string folmality);
	}
}
