using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Interface
{
	public interface IWizardModel
	{
		string TransitFilePathLocation { get; set; }
		string PathToTempFolder { get; set; }
		AsyncTaskWatcherService<PackageModel> PackageModel { get; set; }
	}
}
