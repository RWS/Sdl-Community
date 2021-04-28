using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IPackageService
	{
		Task<PackageModel> OpenPackage(string packagePath, string pathToTempFolder);
		PackageModel GetPackageModel();
		List<string> GetFilesNamesFromPrjFile(List<KeyValuePair<string, string>> fileNames);
		List<string> GetFilesPath(string pathToExtractedProject, CultureInfo cultureInfo, List<string> fileNames);
		bool PackageContainsTms(PackageModel packageModel);
	}
}
