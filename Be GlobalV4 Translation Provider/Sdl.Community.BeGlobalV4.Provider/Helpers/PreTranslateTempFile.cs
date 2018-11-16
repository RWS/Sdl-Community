using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
			var ns = "http://sample/namespace";
			var nsAttribute = doc.CreateAttribute("xmlns", "xx","http://www.w3.org/2000/xmlns/");
			nsAttribute.Value = ns;
			root.Attributes.Append(nsAttribute);

			doc.AppendChild(root);
			doc.Save(filePath);
		}

		public void WriteSegments(string filePath,int id, string segmentText)
		{
			var doc = new XmlDocument();
			doc.Load(filePath);

			//var body = doc.GetElementsByTagName("body")[0];
			var root = doc.DocumentElement;
			var segment = doc.CreateElement("segment");
			segment.InnerText = segmentText;
			var idAttribute = doc.CreateAttribute("id");
			idAttribute.Value = id.ToString();
			segment.Attributes.Append(idAttribute);

			//segment.Attributes
			root?.AppendChild(segment);
			doc.Save(filePath);
		}
	}
}
