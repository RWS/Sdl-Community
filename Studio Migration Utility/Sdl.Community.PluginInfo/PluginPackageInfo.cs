using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.PluginFramework.PackageSupport;

namespace Sdl.Community.PluginInfo
{
    public class PluginPackageInfo
    {
        public string PluginName { get; set; }

        public PluginPackageInfo()
        {
            
        }
        public static string GetPluginName(string pluginPackagePath)
        {
            string pluginName;
            using (var pluginPackage = new PluginPackage(pluginPackagePath, FileAccess.Read))
            {
                pluginName = pluginPackage.PackageManifest.PlugInName;
            }

            return pluginName;
        }
    }
}
