using System.IO;
using System.Reflection;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Utils
{
	public class Helpers
	{
		public ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}

		// Write the icon to the Plugins\Unpacked folder and use the icon's path from that folder in order to set the project's IconPath
		public string GetIconPath()
		{
			var assemblyPath = Assembly.GetExecutingAssembly().Location;
			var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			var targetPath = assemblyPath.Remove(assemblyPath.Length - assemblyName.Length - 4);
			targetPath = targetPath + "transit.ico";

			using (var fs = new FileStream(targetPath, FileMode.Create))
			{
				Resources.transit.Save(fs);
			}
			return targetPath;
		}
	}
}