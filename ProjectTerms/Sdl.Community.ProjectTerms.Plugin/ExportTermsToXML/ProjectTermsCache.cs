using Sdl.Community.ProjectTerms.Controls.Interfaces;
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

            string cacheFile = CreateCacheFilePath(projectPath);

            doc.Save(cacheFile);
        }

        private string CreateCacheFilePath(string projectPath)
        {
            string tmpDirectoryPath = projectPath + "\\tmp";

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

            return Path.Combine(tmpDirectoryPath, Path.GetFileNameWithoutExtension(projectPath.ToString()) + "_ProjectTerms" + ".xml");
        }
    }
}
