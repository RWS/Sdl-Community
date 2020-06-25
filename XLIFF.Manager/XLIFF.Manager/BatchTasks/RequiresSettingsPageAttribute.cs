using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.PluginFramework;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace Sdl.Community.XLIFF.Manager.BatchTasks
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class RequiresSettingsPageAttribute : RequiresSettingsAttribute
	{				
		public RequiresSettingsPageAttribute(Type settingsType, Type settingsPageType) 
			: base(settingsType, settingsPageType)
		{		
		}
	}
}
