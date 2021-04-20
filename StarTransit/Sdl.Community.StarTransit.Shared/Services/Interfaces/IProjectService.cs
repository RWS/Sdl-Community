using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IProjectService
	{
		IProject CreateStudioProject(PackageModel transitPackage);
	}
}
