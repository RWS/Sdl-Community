using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
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
            var result = string.Empty;

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
            var tempLocationPath = Path.GetTempPath();
            var termbaseDefinitionPath = Path.Combine(tempLocationPath + "Termbases", "termbaseDefinition.xdt");
            CreateDirectory(termbaseDefinitionPath);

            var doc = new XmlDocument();
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
            var termbaseDefinitionDirectory = Path.GetDirectoryName(termbaseDefinitionPath);

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
            var doc = new XmlDocument();
            doc.Load(termbaseDefinitionPath);

            var langTag = doc.GetElementsByTagName("Languages")[0];
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
