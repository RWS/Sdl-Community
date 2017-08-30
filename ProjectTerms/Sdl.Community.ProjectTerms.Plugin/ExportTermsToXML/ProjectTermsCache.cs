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
                var cacheFile = GetXMLFilePath(projectPath);
                doc.Save(cacheFile);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_Save + e.Message);
            }
        }

        public static string GetXMLFilePath(string projectPath)
        {
            return Path.Combine(projectPath + "\\tmp", Path.GetFileNameWithoutExtension(projectPath) + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".xml");
        }

        public static string GetExistedFileName(string directoryPath)
        {
            var files = Directory.GetFiles(directoryPath);
            return files.Count() == 1 ? Path.GetFileName(files.FirstOrDefault()) : string.Empty;
        }
    }
}
