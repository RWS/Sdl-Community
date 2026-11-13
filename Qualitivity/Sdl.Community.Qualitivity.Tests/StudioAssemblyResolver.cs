using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sdl.Community.Qualitivity.Tests
{
	// Outside Studio there is no SDLTradosStudio.exe.config to redirect versions, so load what is missing from the Studio folder
	internal static class StudioAssemblyResolver
	{
		public static readonly string TradosFolder = typeof(StudioAssemblyResolver).Assembly
			.GetCustomAttributes<AssemblyMetadataAttribute>().Single(a => a.Key == "TradosFolder").Value;

		private static bool _installed;

		public static void Install()
		{
			if (_installed)
			{
				return;
			}

			_installed = true;
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				var path = Path.Combine(TradosFolder, new AssemblyName(args.Name).Name + ".dll");
				return File.Exists(path) ? Assembly.LoadFrom(path) : null;
			};
		}
	}
}
