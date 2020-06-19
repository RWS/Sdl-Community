using System;
using System.IO;
using System.Xml;
using Sdl.Versioning;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class TemplateLoader
	{
		private readonly string _path;

		public TemplateLoader()
		{
			_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				$@"SDL\{Versions.ProductDescription}\{Versions.MajorVersion}.0.0.0\UserSettings.xml");

			DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), Versions.StudioDocumentsFolderName);
		}

		public string DefaultPath { get; set; }

		public string GetTmFolderPath()
		{
			XmlNodeList data;
			try
			{
				data = LoadDataFromFile(_path, "Setting");
			}
			catch
			{
				return DefaultPath;
			}

			foreach (XmlNode setting in data)
			{
				var id = setting?.Attributes?["Id"];

				if (id.Value == "RecentTranslationMemoryFolder")
				{
					return setting?.InnerText;
				}
			}

			return DefaultPath;
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