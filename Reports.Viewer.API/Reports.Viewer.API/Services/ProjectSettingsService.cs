using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Reports.Viewer.API.Services
{
	public class ProjectSettingsService
	{
		public List<LanguageDirectionInfo> GetLanguageDirections(string filePath)
		{
			var languageDirections = new List<LanguageDirectionInfo>();

			if (string.IsNullOrEmpty(filePath))
			{
				return languageDirections;
			}

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "LanguageDirections", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						foreach (var languageDirection in element.Elements())
						{
							var languageDirectionInfo = new LanguageDirectionInfo
							{
								Guid = GetSettingsBundleAttributeValue(languageDirection, "Guid"),
								SettingsBundleGuid = GetSettingsBundleAttributeValue(languageDirection, "SettingsBundleGuid"),
								TargetLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "TargetLanguageCode"),
								SourceLanguageCode = GetSettingsBundleAttributeValue(languageDirection, "SourceLanguageCode")
							};

							languageDirections.Add(languageDirectionInfo);
						}
					}
				}
			}

			return languageDirections;
		}

		public List<Report> GetProjectTaskReports(string filePath)
		{
			var reports = new List<Report>();
			var languageDirections = GetLanguageDirections(filePath);

			if (string.IsNullOrEmpty(filePath))
			{
				return reports;
			}

			var xml = XDocument.Load(filePath);
			var project = xml.Root;
			if (project != null && string.Compare(project.Name.LocalName, "Project", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				foreach (var element in project.Elements())
				{
					if (string.Compare(element.Name.LocalName, "Tasks", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						reports.AddRange(GetTaskReports(element, languageDirections));
						break;
					}
				}
			}

			return reports;
		}

		private static IEnumerable<Report> GetTaskReports(XContainer element, IReadOnlyCollection<LanguageDirectionInfo> languageDirections)
		{
			var reports = new List<Report>();

			foreach (var taskElement in element.Elements())
			{
				if (string.Compare(taskElement.Name.LocalName, "AutomaticTask", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					var status = GetSettingsBundleAttributeValue(taskElement, "Status");
					if (string.Compare(status, "Completed", StringComparison.CurrentCultureIgnoreCase) != 0)
					{
						continue;
					}

					var createdAtValue = GetSettingsBundleAttributeValue(taskElement, "CreatedAt");
					var createdAsHasValue = DateTime.TryParse(createdAtValue, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.AssumeUniversal, out var createdAt);

					foreach (var automaticTaskElement in taskElement.Elements())
					{
						if (string.Compare(automaticTaskElement.Name.LocalName, "Reports", StringComparison.InvariantCultureIgnoreCase) != 0)
						{
							continue;
						}

						foreach (var taskReport in automaticTaskElement.Elements())
						{
							var report = new Report
							{
								Id = GetSettingsBundleAttributeValue(taskReport, "Guid"),
								Name = GetSettingsBundleAttributeValue(taskReport, "Name"),
								Description = GetSettingsBundleAttributeValue(taskReport, "Description"),
								Path = GetSettingsBundleAttributeValue(taskReport, "PhysicalPath"),
								TemplateId = GetSettingsBundleAttributeValue(taskReport, "TaskTemplateId"),
								Group = ResolveGroupName(GetSettingsBundleAttributeValue(taskReport, "TaskTemplateId")),
								Language = GetTargetLanguageCode(languageDirections, GetSettingsBundleAttributeValue(taskReport, "LanguageDirectionGuid")),
								Date = createdAsHasValue ? createdAt : DateTime.Now,
								IsStudioReport = true
							};

							if (report.IsStudioReport)
							{
								reports.Add(report);
							}
						}
					}
				}
			}

			return reports;
		}

		private static string ResolveGroupName(string group)
		{
			return group.Contains(".") 
				? group.Substring(group.LastIndexOf(".", StringComparison.Ordinal) + 1) 
				: group;
		}

		private static string GetTargetLanguageCode(IReadOnlyCollection<LanguageDirectionInfo> languageDirections, string languageDirectionGuid)
		{
			if (languageDirections?.Count > 0 && !string.IsNullOrEmpty(languageDirectionGuid))
			{
				var languageDirection = languageDirections.FirstOrDefault(a => a.Guid == languageDirectionGuid);
				if (languageDirection != null)
				{
					return languageDirection.TargetLanguageCode;
				}
			}

			return string.Empty;
		}

		private static string GetSettingsBundleAttributeValue(XElement settingsBundle, string attributeId)
		{
			var settingsBundleGuid = string.Empty;
			foreach (var attribute in settingsBundle.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, attributeId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					settingsBundleGuid = attribute.Value;
					break;
				}
			}

			return settingsBundleGuid;
		}
	}
}
