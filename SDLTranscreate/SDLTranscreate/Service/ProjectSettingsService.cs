using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sdl.Community.Transcreate.Model;

namespace Sdl.Community.Transcreate.Service
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

		private static string GetSettingsBundleAttributeValue(XElement settingsBundle, string attributeId)
		{
			var settingsBundleGuid = string.Empty;
			foreach (var attribute in settingsBundle.Attributes())
			{
				if (string.Compare(attribute.Name.LocalName, attributeId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					settingsBundleGuid = attribute.Value;
				}
			}

			return settingsBundleGuid;
		}
	}
}
