using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GoogleCloudTranslationProvider.Helpers
{
	public static class AssemblyResolver
	{
		private static List<string> assemblyFolders = new();

		public static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs e)
		{
			assemblyFolders = assemblyFolders.Distinct().ToList();
			var binBath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase)?.Substring(6);
			var binFolder = new DirectoryInfo($"{binBath}");
			if (!assemblyFolders.Contains(binFolder.FullName))
			{
				assemblyFolders.Add(binFolder.FullName);
			}

			var files = new List<FileInfo>();
			var requestedAssembly = new AssemblyName(e.Name);
			foreach (var assemblyFolder in assemblyFolders)
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

				var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
				if (assemblyName.Version > requestedAssembly.Version)
				{
					requestedAssembly.Version = assemblyName.Version;
				}
				else if (assemblyName.Version < requestedAssembly.Version)
				{
					continue;
				}

				asemblies.Add(Assembly.LoadFrom(file.FullName));
			}

			foreach (var assembly in asemblies)
			{
				if (assembly.FullName.Split(',').ElementAt(0) == e.Name.Split(',').ElementAt(0))
				{
					return assembly;
				}
			}

			return null;
		}
	}
}