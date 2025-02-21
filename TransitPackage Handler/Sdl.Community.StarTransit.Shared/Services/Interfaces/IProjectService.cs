using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IProjectService
	{
		Task<IProject> CreateStudioProject(PackageModel transitPackage);
	}
}
