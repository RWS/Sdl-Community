using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
    public class TermbaseDefinitionFile
    {
        private static ResourceManager rm = new ResourceManager("Sdl.Community.ProjectTerms.Plugin.PluginResources", Assembly.GetExecutingAssembly());

        /// <summary>
        /// Extract the content from embedded resource file 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetResourceTextFile(string fileName)
        {
            try
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
            catch (Exception e)
            {
                throw new TermbaseDefinitionException("Extraction from embedded resource file failed!\n" + e.Message);
            }
        }

        /// <summary>
        /// Save the embedded resource content locally in the Temp folder
        /// </summary>
        /// <param name="content"></param>
        public static string SaveTermbaseDefinitionToTempLocation(string content)
        {
            try
            {
                var tempLocationPath = Path.GetTempPath();
                var termbaseDefinitionPath = Path.Combine(tempLocationPath + "Termbases", "termbaseDefinition.xdt");
                Utils.Utils.CreateDirectory(Path.GetDirectoryName(termbaseDefinitionPath));

                var doc = new XmlDocument();
                doc.LoadXml(content);
                doc.Save(termbaseDefinitionPath);

                return termbaseDefinitionPath;
            }
            catch (Exception e)
            {

                throw new TermbaseDefinitionException("Storing termbase definition file locally failed!\n" + e.Message);
            }
        }

        /// <summary>
        /// Add languages to termbase definition file.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <param name="langs"></param>
        public static void AddLanguages(string termbaseDefinitionPath, Dictionary<string, string> langs)
        {
            try
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
            catch (Exception e)
            {

                throw new TermbaseDefinitionException("Adding languages to termbase definition file failed!\n" + e.Message);
            }
        }
    }
}
