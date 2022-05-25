﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.Provider;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeTranslationProviderTests
{
	[TestClass]
    public class PluginConfigurationTests
    {
        /// <summary>
        /// Tests the default values of the plugin configuration are null
        /// </summary>
        [TestMethod]
        public void PluginConfiguration_GetCurrentInstance_NewConfiguration()
        {
			if (File.Exists(Path.Combine(PluginConfiguration.CurrentInstance.Directory, "config.json")))
			{
				File.Delete(Path.Combine(PluginConfiguration.CurrentInstance.Directory, "config.json"));
			}
			var newPluginConfiguration = PluginConfiguration.CurrentInstance;
            Assert.IsNull(newPluginConfiguration.DefaultConnection);
            Assert.IsNull(newPluginConfiguration.LogLevel);
        }

        /// <summary>
        /// Tests saving the configuration as well as parsing the configuration
        /// </summary>
        [TestMethod]
        public void PluginConfiguration_SaveToFile_SavesConfiguration()
        {
            var newPluginConfiguration = PluginConfiguration.CurrentInstance;
            newPluginConfiguration.DefaultConnection = new Connection(
                host: "foobar.languageweaver.com",
                port: 8001
            );

            newPluginConfiguration.LogLevel = "DEBUG";
            newPluginConfiguration.SaveToFile();
			var savedConfiguration = PluginConfiguration.ParseConfiguration(newPluginConfiguration.Directory);
            Assert.AreEqual(savedConfiguration.LogLevel, newPluginConfiguration.LogLevel);
            Assert.AreEqual(savedConfiguration.DefaultConnection.Value.Host, newPluginConfiguration.DefaultConnection.Value.Host);
            Assert.AreEqual(savedConfiguration.DefaultConnection.Value.Port, newPluginConfiguration.DefaultConnection.Value.Port);
            File.Delete(Path.Combine(newPluginConfiguration.Directory, "config.json"));
        }

        /// <summary>
        /// Tests that if you pass in a directory that doesn't have config.json in it, it will return a null
        /// plugin configuration.
        /// </summary>
        [TestMethod]
        public void PluginConfiguration_ParseConfiguration_NoSuchFile()
        {
            var currentConfiguration = PluginConfiguration.CurrentInstance;
			if (File.Exists(Path.Combine(currentConfiguration.Directory, "config.json")))
			{
				File.Delete(Path.Combine(currentConfiguration.Directory, "config.json"));
			}
			
			// Above has not been saved to a directory yet.
            Assert.IsNull(PluginConfiguration.ParseConfiguration(currentConfiguration.Directory));
        }
    }
}