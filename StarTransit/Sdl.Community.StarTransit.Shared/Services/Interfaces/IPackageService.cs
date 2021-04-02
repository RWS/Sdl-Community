using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IPackageService
	{
		Task<PackageModel> OpenPackage(string packagePath, string pathToTempFolder);
		PackageModel GetPackageModel();
	}
}
