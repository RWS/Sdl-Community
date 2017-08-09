using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sdl.Community.ProjectTerms.Plugin.Termbase
{
    public class TermbaseDefinitionFile
    {
        /// <summary>
        /// Extract the content from embedded resource file 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetResourceTextFile(string fileName)
        {
            string result = string.Empty;

            using (Stream stream = typeof(TermbaseDefinitionFile).Assembly.GetManifestResourceStream("Sdl.Community.ProjectTerms.Plugin.Resources." + fileName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// Save the embedded resource content locally in the Temp folder
        /// </summary>
        /// <param name="content"></param>
        public static string SaveTermbaseDefinitionToTempLocation(string content)
        {
            string tempLocationPath = Path.GetTempPath();
            string termbaseDefinitionPath = Path.Combine(tempLocationPath + "Termbases", "termbaseDefinition.xdt");
            CreateDirectory(termbaseDefinitionPath);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            doc.Save(termbaseDefinitionPath);

            return termbaseDefinitionPath;
        }

        /// <summary>
        /// Create a directory. If already exists will remove its content
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        public static void CreateDirectory(string termbaseDefinitionPath)
        {
            string termbaseDefinitionDirectory = Path.GetDirectoryName(termbaseDefinitionPath);

            if (!Directory.Exists(termbaseDefinitionDirectory))
            {
                Directory.CreateDirectory(termbaseDefinitionDirectory);
            }
            else
            {
                foreach (var file in Directory.GetFiles(termbaseDefinitionDirectory))
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Add languages to termbase definition file.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <param name="langs"></param>
        public static void AddLanguages(string termbaseDefinitionPath, Dictionary<string, string> langs)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(termbaseDefinitionPath);

            XmlNode langTag = doc.GetElementsByTagName("Languages")[0];
            foreach (var lang in langs.Keys)
            {
                var nodeItemLocale = doc.CreateNode(XmlNodeType.Element, "ItemLocale", null);
                nodeItemLocale.InnerText = langs[lang];
                langTag.AppendChild(nodeItemLocale);

                var nodeItemText = doc.CreateNode(XmlNodeType.Element, "ItemText", null);
                nodeItemText.InnerText = lang;
                langTag.AppendChild(nodeItemText);
            }

            doc.Save(termbaseDefinitionPath);
        }
    }
}
