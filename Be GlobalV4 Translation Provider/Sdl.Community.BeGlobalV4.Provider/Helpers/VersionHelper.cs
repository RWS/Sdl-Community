using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class VersionHelper
	{
		/// <summary>
		/// Get plugin version from the pluginpackage.manifest.xml file
		/// </summary>
		/// <returns>plugin version</returns>
		public static string GetPluginVersion()
		{
			var executingAsseblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (executingAsseblyPath != null)
			{
				executingAsseblyPath = Path.Combine(executingAsseblyPath, "pluginpackage.manifest.xml");
				var doc = new XmlDocument();
				doc.Load(executingAsseblyPath);

				if (doc.DocumentElement?.ChildNodes != null)
				{
					foreach (XmlNode n in doc.DocumentElement?.ChildNodes)
					{
						if (n.Name.Equals("Version"))
						{
							return n.InnerText;
						}
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Get installed version for Studio 2015
		/// </summary>
		/// <returns>studio version</returns>
		public static string GetStudioVersion()
		{
			var studioVersion = new Toolkit.Core.Studio().GetInstalledStudioVersion()?.FirstOrDefault(v => v.Version.Equals("Studio4"));
			if (studioVersion != null)
			{
				return $"{studioVersion?.PublicVersion}-{studioVersion?.ExecutableVersion.ToString()}";
			}
			return string.Empty;
		}
	}
}