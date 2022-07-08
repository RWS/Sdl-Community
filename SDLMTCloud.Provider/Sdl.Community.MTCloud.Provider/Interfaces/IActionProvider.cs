using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface IActionProvider
	{
		List<ISDLMTCloudAction> GetActions();
	}
}