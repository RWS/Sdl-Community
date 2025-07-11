using Sdl.Core.PluginFramework;
using System.Collections.Generic;
using System.Linq;

namespace QATracker.Components.SettingsProvider.Components
{
    public static class PluginManagerWrapper
    {
        private static Dictionary<string, string> KnownThirdPartyVerifiers { get; } = new()
        {
            [Constants.NumberVerifierName] = Constants.NumberVerifierSettings,
        };

        public static List<string> GetInstalledThirdPartyVerifiers()
        {
            var plReg = PluginManager.DefaultPluginRegistry;
            var plDescs = plReg.Plugins.Select(p => p.Descriptor);

            var installedPlugins = plDescs.Where(p => p is not FileBasedPluginDescriptor).ToList();
            var names = installedPlugins.Cast<dynamic>().Select(p => p.PlugInName);

            var knownInstalledVerfiers = new List<string>();
            foreach (var name in names)
                if (KnownThirdPartyVerifiers.TryGetValue(name, out string settingId))
                    knownInstalledVerfiers.Add(settingId);

            return knownInstalledVerfiers;
        }
    }
}