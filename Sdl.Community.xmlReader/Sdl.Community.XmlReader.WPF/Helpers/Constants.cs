using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.XmlReader.WPF.Helpers
{
	public static class Constants
	{
		public static  string StudioLocation => ExecutingStudioLocation();
		public static string ProjectApiDll = "Sdl.ProjectApi.Reporting.dll";
		public static string AnalysidDll = "Sdl.ProjectApi.AutomaticTasks.Analysis.dll";

		private static string ExecutingStudioLocation()
		{
			var entryAssembly = Assembly.GetEntryAssembly().Location;
			var location = entryAssembly.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

			return location;
		}
	}
}
