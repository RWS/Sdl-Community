using Sdl.Community.Utilities.TMTool.Task;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.Community.TranslationMemoryManagementUtility
{
	[RibbonGroup("SDL TM Management", Name = "SDL TM Management")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class TranslationMemoryProviderRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.TranslationMemoryManagementUtility", Name = "SDL TM Management", Icon = "TM_icon", Description = "SDL TM Management")]
		[ActionLayout(typeof(TranslationMemoryProviderRibbon), 20, DisplayType.Large)]
		public class TMProviderAction : AbstractAction
		{
			protected override void Execute()
			{
				var tasksDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				List<ITask> TMTasks = GetTasksToPerform(tasksDir);

				if (TMTasks.Count == 0)
				{
					MessageBox.Show(
						string.Format(PluginResources.errTasksNotFound, tasksDir),
									  PluginResources.Title,
									  MessageBoxButtons.OK,
									  MessageBoxIcon.Error);
				}
				else
				{
					TMToolForm toolForm = new TMToolForm(TMTasks);
					toolForm.Show();
				}
			}

			private static List<ITask> GetTasksToPerform(string dir)
			{
				List<ITask> tasks = new List<ITask>();
				foreach (string path in Directory.GetFiles(dir))
				{
					if (Path.GetExtension(path).ToLower() == ".dll")
					{
						try
						{
							Type[] pathTypes = Assembly.LoadFile(path).GetTypes();

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
									MessageBox.Show(string.Format(PluginResources.wrnTaskTypesMissing,
										  path),
										  PluginResources.AssemblyProblemTitle,
										  MessageBoxButtons.OK,
										  MessageBoxIcon.Warning);
								}
							}
						}
						catch (BadImageFormatException ex)
						{
							MessageBox.Show(string.Format(
										PluginResources.wrnTaskAssVersion,
										path,
										ex.Message).Replace("\\r\\n", "\r\n"),
										PluginResources.AssemblyProblemTitle,
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