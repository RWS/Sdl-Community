using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.PluginFramework;

namespace ProjectAutomation
{
	public class PluginFilter: IPluginFilter
	{
		public bool ShouldLoadPlugin(string pluginName)
		{
			return true;
		}

		public bool ShouldLoadExtension(IPlugin plugin, string extensionId)
		{
			return true;
		}
	}
}
