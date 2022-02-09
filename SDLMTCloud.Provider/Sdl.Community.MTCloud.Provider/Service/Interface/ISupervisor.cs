using Sdl.Community.MTCloud.Provider.Interfaces;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface ISupervisor
	{
		void StartSupervising(ITranslationService translationService);
	}
}