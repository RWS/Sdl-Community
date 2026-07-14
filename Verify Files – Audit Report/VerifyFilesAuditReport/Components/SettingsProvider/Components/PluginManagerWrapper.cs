using System.Collections.Generic;
using System.Linq;
using Sdl.Core.PluginFramework;

namespace VerifyFilesAuditReport.Components.SettingsProvider.Components
{
    public static class PluginManagerWrapper
    {
        public static HashSet<string> GetInstalledPluginNames()
        {
            var descriptors = PluginManager.DefaultPluginRegistry.Plugins.Select(p => p.Descriptor);
            var installedPlugins = descriptors.Where(d => d is not FileBasedPluginDescriptor);

            return new HashSet<string>(installedPlugins.Cast<dynamic>().Select(d => (string)d.PlugInName));
        }
    }
}
