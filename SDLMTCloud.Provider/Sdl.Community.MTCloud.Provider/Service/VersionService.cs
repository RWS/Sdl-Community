﻿using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Linq;
using Sdl.Versioning;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class VersionService
	{
		/// <summary>
		/// Get plugin version from the pluginpackage.manifest.xml file
		/// </summary>
		/// <returns>plugin version</returns>
		public string GetPluginVersion()
		{
			try
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
			}
			catch
			{
				// catch all; ignore
			}

			return string.Empty;
		}

		private StudioVersion StudioVersion { get; } = new StudioVersionService().GetStudioVersion();

		/// <summary>
		/// Get current Studio version 
		/// </summary>
		/// <returns>studio version</returns>
		public string GetStudioVersion()
		{
			try
			{
				if (StudioVersion != null)
				{
					return $"{StudioVersion.PublicVersion}-{StudioVersion.ExecutableVersion}";
				}
			}
			catch
			{
				// catch all; ignore
			}

			return string.Empty;
		}

		public string GetAppDataStudioFolder()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL",
				"SDL Trados Studio", StudioVersion.Version);
		}
	}
}