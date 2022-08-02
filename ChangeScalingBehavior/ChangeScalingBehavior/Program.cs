using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

namespace ChangeScalingBehavior
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			LoadAssemblies();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
			{
				Process.GetCurrentProcess().Kill();
			}

			InitializeLog.InitializeLoggingConfiguration();

			Application.Run(new HighDPIChange());
		}


		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadAssemblies()
		{
			var assemblies = new Dictionary<string, Assembly>();
			Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
			{
				var shortName = new AssemblyName(args.Name).Name;
				if (assemblies.TryGetValue(shortName, out var assembly))
				{
					return assembly;
				}

				return null;
			}

			var appAssembly = typeof(HighDPIChange).Assembly;
			foreach (var resourceName in appAssembly.GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				{
					using (var stream = appAssembly.GetManifestResourceStream(resourceName))
					{
						if (stream != null)
						{
							var assemblyData = new byte[(int)stream.Length];
							stream.Read(assemblyData, 0, assemblyData.Length);
							var assembly = Assembly.Load(assemblyData);
							assemblies.Add(assembly.GetName().Name, assembly);
						}
					}
				}
			}

			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}
	}
}
