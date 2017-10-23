using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Toolkit.Core.Services;

namespace Sdl.Community.ReportExporter.Helpers
{
	public static class Help
	{
		public static int GetInstalledStudioMajorVersion()
		{
			var studioService = new StudioVersionService();
			return studioService.GetStudioVersion().ExecutableVersion.Major;
		}

		public static string GetStudioProjectsPath()
		{
			var installedStudioVersion = GetInstalledStudioMajorVersion();
			var projectsPath = string.Empty;

			if (installedStudioVersion.Equals(14))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2017\Projects\projects.xml");
			}
			if (installedStudioVersion.Equals(12))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2015\Projects\projects.xml");
			}

			return projectsPath;
		}
	}
}
