using Sdl.Community.ProjectTerms.Controls.Interfaces;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML
{
    public class ProjectTermsCache
    {
        public void Save(string projectPath, IEnumerable<ITerm> terms, bool wordCloudFile = false)
        {
            try
            {
                XDocument doc = new XDocument();
                doc.Add(
                    new XElement("projectTerms",
                        new XElement("terms",
                            from term in terms
                            select new XElement("term", new XAttribute("count", term.Occurrences), term.Text)))
                );

                if (!Directory.Exists(Path.GetDirectoryName(Utils.Utils.GetXMLFilePath(projectPath))))
                    Utils.Utils.CreateDirectory(Path.GetDirectoryName(Utils.Utils.GetXMLFilePath(projectPath)));

                if (!wordCloudFile)
                {
                    doc.Save(Utils.Utils.GetXMLFilePath(projectPath));
                }
                else
                {
                    doc.Save(Utils.Utils.GetXMLFilePath(projectPath, true));
                }
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_Save + e.Message);
            }
        }

        public IEnumerable<ITerm> ReadXmlFile(string filePath)
        {
            try
            {
                var xmlTerms = new List<ITerm>();
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);
                XmlNodeList terms = doc.GetElementsByTagName("term");
                foreach (XmlNode term in terms)
                {
                    xmlTerms.Add(new Term()
                    {
                        Text = term.InnerText,
                        Occurrences = int.Parse(term.Attributes["count"].Value)
                    });
                }
                return xmlTerms;
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_ReadXmlFile + e.Message);
            }
        }
    }
}
