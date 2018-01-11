using Sdl.Community.Utilities.TMTool.Task;
using Sdl.Desktop.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Runtime.CompilerServices;

namespace Sdl.Community.TranslationMemoryProvider
{
	[RibbonGroup("TM Provider", Name = "TM Provider", Description = "TM Provider", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	public class TranslationMemoryProviderRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.TranslationMemoryProvider", Name = "TM Provider", Icon = "TranslationMemory", Description = "TM Provider")]
		[ActionLayout(typeof(TranslationMemoryProviderRibbon), 20, DisplayType.Large)]
		public class TMProviderAction : AbstractAction
		{
			protected override void Execute()
			{
				List<ITask> TMTasks = GetTasksToPerform();
				if (TMTasks.Count == 0)
				{
					MessageBox.Show(
						string.Format(PluginResources.errTasksNotFound,
									  PluginResources.Title,
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Error));
				}
				else
				{
					Application.Run(new TMToolForm(TMTasks));
				}
			}

			private List<ITask> GetTasksToPerform()
			{
				var appAssembly = LoadAssemblies();
				
				List<ITask> tasks = new List<ITask>();
				foreach (KeyValuePair<string, Assembly> assembly in appAssembly)
				{
					try
					{
						Type[] pathTypes = Assembly.Load(assembly.Value.FullName).GetTypes();

						IList<ITask> newTasks = pathTypes.Where(t => t.GetInterfaces().Where(iface => iface == typeof(ITask))
													 .Any())
													 .Select(t => (ITask)Activator.CreateInstance(t))
													 .ToList();

						if (newTasks.Count > 0)
						{
							if (pathTypes.Where(t => t.GetInterfaces().Where(iface => iface == typeof(IControl)).Any()).Count() > 0
								&& pathTypes.Where(t => t.GetInterfaces().Where(iface => iface == typeof(ISettings)).Any()).Count() > 0)
							{
								tasks.AddRange(newTasks);

							}
							else
							{
								// error: ISettings or(and) IControl not implemented
								MessageBox.Show(string.Format(
									PluginResources.wrnTaskTypesMissing,
									assembly),
									PluginResources.AssemblyProblemTitle,
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning);
							}
						}
					}
					catch (BadImageFormatException ex)
					{
						// error: framework > 3.5
						MessageBox.Show(string.Format(
									PluginResources.wrnTaskAssVersion,
									assembly,
									ex.Message).Replace("\\r\\n", "\r\n"),
									PluginResources.AssemblyProblemTitle,
									MessageBoxButtons.OK,
									MessageBoxIcon.Warning);
					}
				}
				return tasks;
			}


			[MethodImpl(MethodImplOptions.NoInlining)]
			private static Dictionary<string, Assembly> LoadAssemblies()
			{
				Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();
				Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
				{
					var shortName = new AssemblyName(args.Name).Name;
					if (_assemblies.TryGetValue(shortName, out var assembly))
					{
						return assembly;
					}
					return null;
				}
				var appAssembly = typeof(TranslationMemoryProviderRibbon).Assembly;
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

				return _assemblies;
			}
		}
	}
}