using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public interface IRegistryHelper
	{
		Task BackupKeys(List<LocationDetails> location);

		void DeleteKeys(List<LocationDetails> locations, bool tradosKeys);

		Task RestoreKeys(List<LocationDetails> pathsToKeys);
	}
}