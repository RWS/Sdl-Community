using Sdl.Community.ProjectTerms.Controls.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML
{
    public class ProjectTermsCache
    {
        public void Save(string projectPath, IEnumerable<ITerm> terms)
        {
            XDocument doc = new XDocument();
            doc.Add(
                new XElement("projectTerms",
                    new XElement("terms",
                        from term in terms
                        select new XElement("term", new XAttribute("count", term.Occurrences), term.Text)))
                );

            CreateCacheDirectory(projectPath);
            string cacheFile = GetXMLFilePath(projectPath);

            doc.Save(cacheFile);
        }

        public static string GetXMLFilePath(string projectPath)
        {
            return Path.Combine(projectPath + "\\tmp", "ProjectTerms_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".xml");
        }

        private void CreateCacheDirectory(string projectPath)
        {
            string tmpDirectoryPath = Path.GetDirectoryName(GetXMLFilePath(projectPath));

            if (!Directory.Exists(tmpDirectoryPath))
            {
                Directory.CreateDirectory(tmpDirectoryPath);
            } else
            {
                foreach (var file in Directory.GetFiles(tmpDirectoryPath))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
