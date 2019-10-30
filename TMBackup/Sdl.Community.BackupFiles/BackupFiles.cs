using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sdl.Community.BackupFiles
{
	public class BackupFiles
	{
		static void Main(string[] args)
		{
			LoadAssemblies();

			if (args.Any())
			{
				var service = new BackupService();
				service.BackupFilesRecursive(args[0]);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadAssemblies()
		{
			var _assemblies = new Dictionary<string, Assembly>();
			Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
			{
				var shortName = new AssemblyName(args.Name).Name;
				if (_assemblies.TryGetValue(shortName, out var assembly))
				{
					return assembly;
				}
				return null;
			}
			var appAssembly = typeof(BackupFiles).Assembly;
			foreach (var resourceName in appAssembly.GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				{
					using (var stream = appAssembly.GetManifestResourceStream(resourceName))
					{
						var assemblyData = new byte[(int)stream.Length];
						stream.Read(assemblyData, 0, assemblyData.Length);
						var assembly = Assembly.Load(assemblyData);
						_assemblies.Add(assembly.GetName().Name, assembly);
					}
				}
			}
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}
	}
}