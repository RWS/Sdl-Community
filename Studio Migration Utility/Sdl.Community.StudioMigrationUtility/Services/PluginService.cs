using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Versioning;

namespace Sdl.Community.StudioMigrationUtility.Services
{
	public class PluginService
    {
        private readonly List<string> _pluginFolderLocations = new List<string>
        {
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        };

		public List<string> GetInstalledPlugins(StudioVersion studioVersion)
        {
            var installedPlugins = new List<string>();

			var pluginPath = _pluginFolderLocations.Select(
				pluginFolderLocation => Path.Combine(pluginFolderLocation, studioVersion.PluginPackagePath));

            foreach (var plugin in pluginPath.Select(path => Directory.GetFiles(path, "*.sdlplugin"))
                .Where(plugin => plugin.Length != 0))
            {
                installedPlugins.AddRange(plugin);
            }

            return installedPlugins;
        }

    }
}
