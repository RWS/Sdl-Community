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
			private ResourceManager _rm = new ResourceManager("Sdl.Community.TranslationMemoryProvider.PluginResource", Assembly.GetExecutingAssembly());

			protected override void Execute()
			{
				string tasksDir = AppDomain.CurrentDomain.BaseDirectory + @"TMTaskDlls\";

				List<ITask> TMTasks = GetTasksToPerform(tasksDir);
				if (TMTasks.Count == 0)
				{
					MessageBox.Show(
						string.Format(_rm.GetString("errTasksNotFound", CultureInfo.CurrentCulture), tasksDir),
									  _rm.GetString("Title", CultureInfo.CurrentCulture),
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Error);
				}
				else
				{
					Application.Run(new TMToolForm(TMTasks));
				}
			}

			private List<ITask> GetTasksToPerform(string dir)
			{
				List<ITask> tasks = new List<ITask>();
				foreach (string path in Directory.GetFiles(dir))
				{
					if (Path.GetExtension(path).ToLower() == ".dll")
					{
						try
						{
							Type[] pathTypes = Assembly.LoadFile(path).GetTypes();
							IList<ITask> newTasks = pathTypes.Where(t => t.GetInterfaces()
																		  .Where(iface => iface == typeof(ITask))
																		  .Any()
																	)
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
									MessageBox.Show(string.Format(_rm.GetString("wrnTaskTypesMissing", CultureInfo.CurrentCulture),
										path),
										_rm.GetString("AssemblyProblemTitle", CultureInfo.CurrentCulture),
										MessageBoxButtons.OK,
										MessageBoxIcon.Warning);
								}
							}
						}
						catch (BadImageFormatException ex)
						{                         
							// error: framework > 3.5
							MessageBox.Show(string.Format(_rm.GetString("wrnTaskAssVersion", CultureInfo.CurrentCulture),
										path,
										ex.Message).Replace("\\r\\n", "\r\n"),
										_rm.GetString("AssemblyProblemTitle", CultureInfo.CurrentCulture),
										MessageBoxButtons.OK,
										MessageBoxIcon.Warning);
						}
					}
				}
				return tasks;
			}
		}
	}
}