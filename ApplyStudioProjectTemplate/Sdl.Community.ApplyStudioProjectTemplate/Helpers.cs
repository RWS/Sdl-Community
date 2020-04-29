using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ApplyStudioProjectTemplate
{
	public static class Helpers
	{
		private static TemplateLanguageDetails GetTemplateLanguageDirection(string templatePath)
		{
			var document = new XmlDocument();
			document.Load(templatePath);
			var node = document.DocumentElement?.SelectSingleNode("LanguageDirections");
			var templateLanguage = new TemplateLanguageDetails
			{
				TagrgetLanguagesCodeList =  new List<string>()
			};
			if (node?.HasChildNodes == true)
			{
				var sourceLanguageCode = node.ChildNodes[0].Attributes?["SourceLanguageCode"].InnerText;
				if (!string.IsNullOrEmpty(sourceLanguageCode))
				{
					templateLanguage.SourceLanguageCode = sourceLanguageCode;
				}
				foreach (XmlNode childNode in node.ChildNodes)
				{
					var targetLanguageCode = childNode.Attributes?["TargetLanguageCode"].InnerText;
					if (!string.IsNullOrEmpty(targetLanguageCode))
					{
						templateLanguage.TagrgetLanguagesCodeList.Add(targetLanguageCode);
					}
				}
			}
			return templateLanguage;
		}

		public static bool Matches(List<FileBasedProject> selectedProjects, ApplyTemplate selectedTemplate)
		{
			foreach (var selectedProject in selectedProjects)
			{
				var projectInfo = selectedProject.GetProjectInfo();
				if (projectInfo == null) continue;
				var templatePath = selectedTemplate.Uri != null ? selectedTemplate.Uri.LocalPath : selectedTemplate.FileLocation;
				var projectTemplateLanguages = GetTemplateLanguageDirection(templatePath);
				if (projectInfo.SourceLanguage != null)
				{
					var sourceLanguage = projectInfo.SourceLanguage.CultureInfo.Name;
					if (projectInfo.TargetLanguages != null)
					{
						var targetLanguages = projectInfo.TargetLanguages.ToList();
						var matchesLanguages =
							ProjectLanguageMatchesTemplate(projectTemplateLanguages, sourceLanguage, targetLanguages);
						if (!matchesLanguages)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private  static bool ProjectLanguageMatchesTemplate(TemplateLanguageDetails projectTemplateLanguages, string sourceLanguage, List<Language> targetLanguages)
		{
			if (projectTemplateLanguages.SourceLanguageCode == null)
				return true;

			var targetLanguagesCode = new List<string>();
			foreach (var language in targetLanguages)
			{
				targetLanguagesCode.Add(language.CultureInfo.Name);
			}

			if (!projectTemplateLanguages.SourceLanguageCode.Equals(sourceLanguage, System.StringComparison.CurrentCultureIgnoreCase))
			{
				return false;
			}
			foreach (var templateTargetLanguage in projectTemplateLanguages.TagrgetLanguagesCodeList)
			{
				var sourceProjectContaisLanguage = targetLanguagesCode.Any(l=>l.Equals(templateTargetLanguage, System.StringComparison.CurrentCultureIgnoreCase));
				if (!sourceProjectContaisLanguage)
				{
					return false;
				}
			}
			return true;
		}
	}
}
