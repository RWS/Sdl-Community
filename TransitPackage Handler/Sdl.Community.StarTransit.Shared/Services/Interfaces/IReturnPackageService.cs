using System.Collections.Generic;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Shared.Services.Interfaces
{
	public interface IReturnPackageService
	{
		(IReturnPackage, string) GetPackage();
		bool IsTransitProject(List<ProjectFile> filesPath);
		bool ExportFiles(IReturnPackage package, int encodingCode);
	}
}
