using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using Sdl.Community.BeGlobalV4.Provider.Model;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class PreTranslateTempFile
	{
		public string CreateXmlFile()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),"temp.xml");
			if (!File.Exists(path))
			{
				var file = File.Create(path);
				file.Close();
				CreatexmlBody(path);
			}
			else
			{
				File.WriteAllText(path, "");
				CreatexmlBody(path);
			}  
			return path; 
		}

		private void CreatexmlBody(string filePath)
		{
			var doc = new XmlDocument();
			var root = doc.CreateElement("root");
			const string ns = "http://sample/namespace";
			var nsAttribute = doc.CreateAttribute("xmlns", "xx","http://www.w3.org/2000/xmlns/");
			nsAttribute.Value = ns;
			root.Attributes.Append(nsAttribute);

			doc.AppendChild(root);
			doc.Save(filePath);
		}

		public void WriteSegments(string filePath,string id, string segmentText)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);	 

			var root = doc.DocumentElement;
			var segment = doc.CreateElement("segment");
			segment.InnerText = segmentText;
			var idAttribute = doc.CreateAttribute("id");
			idAttribute.Value = id;
			segment.Attributes.Append(idAttribute);		 

			root?.AppendChild(segment);
			doc.Save(filePath);
		}

		public List<PreTranslation> GetTranslationFromTemp(string filePath)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);
			var preTranslations = new List<PreTranslation>();
			var root = doc.GetElementsByTagName("root")[0];
			foreach (XmlNode node in root.ChildNodes)
			{
				var preTranslation = new PreTranslation();
				var id = node?.Attributes?["id"];
				if (id != null)
				{
					preTranslation.Id = id.Value;
					preTranslation.Translation = WebUtility.HtmlDecode(node.InnerText);
					preTranslations.Add(preTranslation);
				} 
			}  
			return preTranslations;
		}
	}
}
