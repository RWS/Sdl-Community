using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TemplateLoader
	{
		public string GetTmTemplateFolderPath()
		{
			var data =
				LoadDataFromFile(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						@"SDL\SDL Trados Studio\15.0.0.0\UserSettings.xml"), "Setting");

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentLanguageResourceGroupFolder")
				{
					return setting.InnerText;
				}
			}

			return null;
		}

		public string GetTmFolderPath()
		{
			var data =
				LoadDataFromFile(
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
						@"SDL\SDL Trados Studio\15.0.0.0\UserSettings.xml"), "Setting");

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentTranslationMemoryFolder")
				{
					return setting.InnerText;
				}
			}

			return null;
		}

		public List<LanguageResourceBundle> GetLanguageResourceBundles()
		{
			return null;
		}

		public XmlNodeList LoadDataFromFile(string filePath, string element)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);
			var data = doc.GetElementsByTagName(element);

			return data;
		}
	}
}