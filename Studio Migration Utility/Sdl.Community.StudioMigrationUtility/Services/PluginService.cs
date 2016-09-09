using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StudioMigrationUtility.Model;

namespace Sdl.Community.StudioMigrationUtility.Services
{
    public class PluginService
    {
        private readonly List<string> _pluginFolderLocations = new List<string>
        {
           Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
               @"SDL\SDL Trados Studio\{0}\Plugins\Packages"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               @"SDL\SDL Trados Studio\{0}\Plugins\Packages"),
             Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
               @"SDL\SDL Trados Studio\{0}\Plugins\Packages")
        };

        public List<string> GetInstalledPlugins(StudioVersion studioVersion)
        {
            var installedPlugins = new List<string>();


            var pluginPath = _pluginFolderLocations.Select(
                pluginFolderLocation =>
                    string.Format(pluginFolderLocation, studioVersion.ExecutableVersion.Major));


            foreach (var plugin in pluginPath.Select(path => Directory.GetFiles(path, "*.sdlplugin"))
                .Where(plugin => plugin.Length != 0))
            {

                installedPlugins.AddRange(plugin);
            }
            return installedPlugins;
        }

    }
}
