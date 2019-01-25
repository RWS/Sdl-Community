using System;
using System.IO;
using System.Xml;

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

		public XmlNodeList LoadDataFromFile(string filePath, string element)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);
			var data = doc.GetElementsByTagName(element);

			return data;
		}
	}
}