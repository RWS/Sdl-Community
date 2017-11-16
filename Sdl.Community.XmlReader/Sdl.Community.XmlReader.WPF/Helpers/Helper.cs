using System.IO;

namespace Sdl.Community.XmlReader.WPF.Helpers
{
    public static class Helper
    {
        #region Analyze Files management
        public static string GetFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public static string GetXMLFileName(string filePath)
        {
            var fileName = GetFileName(filePath);

            if (!fileName.Contains(Properties.Resources.FileName))
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
        #endregion


        #region Studio language's flags
        public static string GetStudioInstalationPath()
        {
            var studioService = new Toolkit.Core.Services.StudioVersionService();
            return Path.GetDirectoryName(studioService.GetStudioVersion().InstallPath);
        }

        public static int GetImageStudioCodeByLanguageCode(string languageCode)
        {
            var studioPath = GetStudioInstalationPath();
            var studioLangsFlagsDictionary = Path.Combine(studioPath, "AutoCorrectDictionaries");
            if (Directory.Exists(studioLangsFlagsDictionary))
            {
                foreach (var file in Directory.GetFiles(studioLangsFlagsDictionary))
                {
                    var fileName = Path.GetFileName(file);

                    if (fileName.Contains(languageCode))
                    {
                        var codes = fileName.Split('-');
                        return int.Parse(codes[codes.Length - 1]);
                    }
                }
            }

            return 0;
        }

        public static string GetImagePathByStudioCode(int code)
        {
            var studioPath = GetStudioInstalationPath();
            var studioFlagsDirectory = studioPath + @"\ReportResources\images\Flags";

            if (Directory.Exists(studioFlagsDirectory))
            {
                foreach (var file in Directory.GetFiles(studioFlagsDirectory))
                {
                    var fileName = Path.GetFileName(file);

                    if (fileName.Equals(code.ToString() + ".bmp"))
                    {
                        return Path.Combine(studioFlagsDirectory, fileName);

                    }
                }
            }

            return Path.Combine(studioFlagsDirectory, "0.bmp"); //default image
        }
        #endregion
    }
}
