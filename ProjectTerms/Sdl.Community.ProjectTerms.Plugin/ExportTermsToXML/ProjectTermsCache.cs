using Sdl.Community.ProjectTerms.Controls.Interfaces;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
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
            try
            {
                XDocument doc = new XDocument();
                doc.Add(
                    new XElement("projectTerms",
                        new XElement("terms",
                            from term in terms
                            select new XElement("term", new XAttribute("count", term.Occurrences), term.Text)))
                    );

                Utils.Utils.CreateDirectory(Path.GetDirectoryName(GetXMLFilePath(projectPath)));
                string cacheFile = GetXMLFilePath(projectPath);

                doc.Save(cacheFile);
            }
            catch (Exception e)
            {

                throw new ProjectTermsException("The xml file failed to be saved!\n" + e.Message);
            }
        }

        public static string GetXMLFilePath(string projectPath)
        {
            return Path.Combine(projectPath + "\\tmp", "ProjectTerms_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".xml");
        }
    }
}
