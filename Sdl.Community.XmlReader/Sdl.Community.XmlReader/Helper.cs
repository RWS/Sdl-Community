using System.IO;
using System.Linq;

namespace Sdl.Community.XmlReader
{
    public static class Helper
    {
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

            return string.Empty;
        }
    }
}
