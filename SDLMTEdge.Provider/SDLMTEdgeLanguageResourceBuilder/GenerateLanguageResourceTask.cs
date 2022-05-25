using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;

namespace ETSLanguageResourceBuilder
{
    public class GenerateLanguageResourceTask : Task
    {
        [Required]
        public string XMTExternalsDir { get; set; }

        public override bool Execute()
        {
            string languagesPath = Path.Combine(XMTExternalsDir, @"..\Shared\resources\languages\languages.xml");
            // If the file doesn't exist, fail this task.
            if (!File.Exists(languagesPath))
            {
                Log.LogError("Languages xml file: \"" + languagesPath + "\"+ does not exist.");
                return false;
            }

            XmlDocument doc = new XmlDocument();
            using (StreamReader reader = new StreamReader(languagesPath))
                doc.LoadXml(reader.ReadToEnd());

            XmlNode languagesNode = doc.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name.Equals("languages"));
            if (languagesNode == null)
            {
                Log.LogError("Languages xml file: \"" + languagesPath + "\"+ is malformed.");
                return false;
            }

            // For each language in the xml, add a row to the resource file, relating the languageTag and the ets language code
            string resourcePath = @"..\ETSLanguages.resx";
            Log.LogMessage(MessageImportance.High, "Creating or overwriting resource file at " + Path.GetFullPath(resourcePath));
            using (FileStream fs = new FileStream(resourcePath, FileMode.Create))
            {
                using (ResXResourceWriter resx = new ResXResourceWriter(fs))
                {
                    foreach (XmlNode child in languagesNode.ChildNodes.Cast<XmlNode>().Where(n => n.Name.Equals("language")))
                    {
                        var attributes = child.Attributes.Cast<XmlAttribute>();

                        var languageTag = attributes.FirstOrDefault(a => a.Name == "languageTag");
                        if (languageTag == null)
                            continue;

                        var etsCode = attributes.FirstOrDefault(a => a.Name == "etsCode");
                        if (etsCode == null)
                            continue;

                        resx.AddResource(etsCode.Value, languageTag.Value);
                    }
                }
            }
            return true;
        }
    }
}