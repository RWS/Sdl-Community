using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MicrosoftTranslatorProvider.Helpers
{
	public static class AssemblyResolver
	{
		public static List<string> AssemblyFolders = new List<string>();

		public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			AssemblyFolders = AssemblyFolders.Distinct().ToList();
			var binBath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.Substring(6);
			var binFolder = new DirectoryInfo($"{binBath}");
			if (!AssemblyFolders.Contains(binFolder.FullName))
			{
				AssemblyFolders.Add(binFolder.FullName);
			}

			var requestedAssembly = new AssemblyName(args.Name);
			var files = new List<FileInfo>();
			foreach (var assemblyFolder in AssemblyFolders)
			{
				files.AddRange(new DirectoryInfo(assemblyFolder).GetFiles("*.*", SearchOption.AllDirectories).Where(V => V.Name.Contains(requestedAssembly.Name)));
			}

			var asemblies = new List<Assembly>();
			foreach (var file in files)
			{
				if (!file.FullName.ToLower().EndsWith(".dll")
					&& !file.FullName.ToLower().EndsWith(".exe"))
				{
					continue;
				}

				var foundAsm = AssemblyName.GetAssemblyName(file.FullName);
				if (foundAsm.Version > requestedAssembly.Version)
				{
					requestedAssembly.Version = foundAsm.Version;
				}
				else if (foundAsm.Version < requestedAssembly.Version)
				{
					continue;
				}

				asemblies.Add(Assembly.LoadFrom(file.FullName));
				break;
			}

			foreach (var assembly in asemblies)
			{
				if (assembly.FullName.Split(',').ElementAt(0) == args.Name.Split(',').ElementAt(0))
				{
					return assembly;
				}
			}

			return null;
		}
	}
}