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
			var pexecutingAsseblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			pexecutingAsseblyPath = Path.Combine(pexecutingAsseblyPath, "pluginpackage.manifest.xml");
			var doc = new XmlDocument();
			doc.Load(pexecutingAsseblyPath);

			if (doc.DocumentElement != null)
			{
				foreach (XmlNode n in doc.DocumentElement.ChildNodes)
				{
					if (n.Name == "Version")
					{
						return n.InnerText;
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// Get installed version for Studio 2019
		/// </summary>
		/// <returns>studio version</returns>
		public static string GetStudioVersion()
		{
			var studioVersion = new Toolkit.Core.Studio().GetInstalledStudioVersion().Where(v => v.Version.Equals("Studio15")).FirstOrDefault();
			if (studioVersion != null)
			{
				return $"{studioVersion?.PublicVersion}-{studioVersion?.ExecutableVersion.ToString()}";
			}
			return string.Empty;
		}
	}
}