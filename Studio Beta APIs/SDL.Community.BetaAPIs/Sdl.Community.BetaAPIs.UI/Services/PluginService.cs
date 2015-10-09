using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sdl.Community.BetaAPIs.UI.Services
{
    public class PluginService
    {
        private PluginConfig _pluginConfig;
        public PluginService()
        {
        }

        public PluginConfig LoadPluginConfig()
        {
            var serializer = new XmlSerializer(typeof(PluginConfig));
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pluginconfig.xml");
            using (StreamReader reader = new StreamReader(path))
            {
                _pluginConfig = (PluginConfig)serializer.Deserialize(reader);
            }

            return _pluginConfig;
        }

        public void AddPublicAssemblies(List<string> assemblies)
        {
            var publicAssemblies = _pluginConfig.Items.OfType<PluginConfigPublicAssemblies>().FirstOrDefault();
            var listPublicAssemblies = new List<PluginConfigPublicAssembliesPublicAssembly>(publicAssemblies.PublicAssembly);

            foreach (var assembly in assemblies)
            {
                Version version = GetAssemblyVersion(assembly);
                var newPublicAssembly = new PluginConfigPublicAssembliesPublicAssembly
                {
                    name = assembly,
                    version = string.Format("{0}.{1}", version.Major, version.Minor)
                };
                listPublicAssemblies.Add(newPublicAssembly);
                
            }
            publicAssemblies.PublicAssembly = listPublicAssemblies.ToArray();
        }
        

        private Version GetAssemblyVersion(string assemblyName)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
            //var assembly = Assembly.LoadFile(path);
            var versionInfo = FileVersionInfo.GetVersionInfo(path);
            return new Version(versionInfo.FileVersion);
        }

        public void RemovePublicAssemblies(List<string> assemblies)
        {
            var publicAssemblies = _pluginConfig.Items.OfType<PluginConfigPublicAssemblies>().FirstOrDefault();
            var listPublicAssemblies = new List<PluginConfigPublicAssembliesPublicAssembly>(publicAssemblies.PublicAssembly);

            foreach (var assembly in assemblies)
            {
                var publicAssembly = listPublicAssemblies.Find(x => x.name == assembly);

                listPublicAssemblies.Remove(publicAssembly);
            }

            publicAssemblies.PublicAssembly = listPublicAssemblies.ToArray();

        }

        public void SavePluginConfig()
        {
            var serializer = new XmlSerializer(typeof(PluginConfig));
            using (XmlWriter writer = XmlWriter.Create("pluginconfig.xml"))
            {
                serializer.Serialize(writer, _pluginConfig);
            }
        }
    }
}
