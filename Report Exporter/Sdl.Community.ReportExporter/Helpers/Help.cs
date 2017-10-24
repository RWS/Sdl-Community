using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Sdl.Community.ReportExporter.Model;
using Sdl.Community.Toolkit.Core.Services;

namespace Sdl.Community.ReportExporter.Helpers
{
	public static class Help
	{
		public static int GetInstalledStudioMajorVersion()
		{
			var studioService = new StudioVersionService();
			return studioService.GetStudioVersion().ExecutableVersion.Major;
		}

		public static string GetStudioProjectsPath()
		{
			var installedStudioVersion = GetInstalledStudioMajorVersion();
			var projectsPath = string.Empty;

			if (installedStudioVersion.Equals(14))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2017\Projects\projects.xml");
			}
			if (installedStudioVersion.Equals(12))
			{
				projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
					@"Studio 2015\Projects\projects.xml");
			}

			return projectsPath;
		}

		public static Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc)
		{
			var languages = new Dictionary<string, LanguageDirection>();
			var languageeDirectionNode = doc.SelectNodes("/Project/LanguageDirections/LanguageDirection");

			if (languageeDirectionNode == null) return languages;
			foreach (var item in languageeDirectionNode)
			{
				var node = (XmlNode) item;
				if (node.Attributes == null) continue;
				var lang = new LanguageDirection
				{
					Guid = node.Attributes["Guid"].Value,
					TargetLang = CultureInfo.GetCultureInfo(node.Attributes["TargetLanguageCode"].Value)
				};
				languages.Add(lang.Guid, lang);
			}

			return languages;
		}

		public static void LoadReports(XmlDocument doc, string projectPath, Dictionary<string, LanguageDirection> languages)
		{
			var automaticTaskNode = doc.SelectNodes("/Project/Tasks/AutomaticTask");
			if (automaticTaskNode != null)
				foreach (var node in automaticTaskNode)
				{
					var task = (XmlNode) node;
					var reportNodes = task.SelectNodes("Reports/Report");
					if (reportNodes == null) continue;

					foreach (var reportNode in reportNodes)
					{
						var report = (XmlNode) reportNode;
						if (report.Attributes != null && report.Attributes["TaskTemplateId"].Value == "Sdl.ProjectApi.AutomaticTasks.Analysis")
						{
							//always overwrite the previous path, we are only interested into last analyze
							languages[report.Attributes["LanguageDirectionGuid"].Value].PathToReport =
								projectPath + "\\" + report.Attributes["PhysicalPath"].Value;
						}
					}
				}
		}
	}
}
