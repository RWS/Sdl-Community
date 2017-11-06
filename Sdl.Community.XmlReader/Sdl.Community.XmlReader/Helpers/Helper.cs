using Sdl.Community.XmlReader.Properties;
using System.IO;

namespace Sdl.Community.XmlReader.Helpers
{
    public static class Helper
    {

        public static string GetFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public static string GetXMLFileName(string filePath)
        {
            var fileName = GetFileName(filePath);

            if (!fileName.Contains(Resources.FileName))
            {
                return null;
            }

            // The xml file name always should follow this pattern "Analyze Files 'souce_code''_''target_code'\.*.xml"
            // After the target code can be '- Copy' if the user has many analyze file with exact source/target language code 
            return fileName;
        }

        public static string GetTargetLanguageCode(string fileName)
        {
            // az-Cyrl-AZ - Copy
            return (fileName.Split('_')[1]).Split(' ')[0];
        }
    }
}
