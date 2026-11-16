using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sdl.Community.Qualitivity.Tracking;
using Xunit;

namespace Sdl.Community.Qualitivity.Tests
{
	public class PluginReferencesTests
	{
		// Studio refuses to load a plugin that references an Sdl assembly missing from <PublicAssemblies>
		[Fact]
		public void PluginAssembly_ReferencesOnlyPublicStudioAssemblies()
		{
			var pluginConfig = XDocument.Load(Path.Combine(StudioAssemblyResolver.TradosFolder, "pluginconfig.xml"));
			var publicAssemblies = pluginConfig.Descendants(pluginConfig.Root.Name.Namespace + "PublicAssembly")
				.Select(a => (string)a.Attribute("name"));

			var notPublic = typeof(SegmentWordCounter).Assembly.GetReferencedAssemblies()
				.Select(a => a.Name)
				.Where(name => name.StartsWith("Sdl.", StringComparison.Ordinal)
					&& File.Exists(Path.Combine(StudioAssemblyResolver.TradosFolder, name + ".dll")))
				.Except(publicAssemblies)
				.ToList();

			Assert.Empty(notPublic);
		}
	}
}
