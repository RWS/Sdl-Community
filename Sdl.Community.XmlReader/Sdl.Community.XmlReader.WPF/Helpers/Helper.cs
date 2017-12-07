using System.Globalization;
using System.IO;
using System.Xml;

namespace Sdl.Community.XmlReader.WPF.Helpers
{
    public static class Helper
    {
        #region Analyze Files management
        public static string GetFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }
		
	    public static string GetTargetLanguageCode(string filePath)
	    {
		    var doc = new XmlDocument();
		    doc.Load(filePath);

		    var languageNodeAttributes = doc.GetElementsByTagName("language")[0].Attributes;
		    var languageLcid = string.Empty;
		    if (languageNodeAttributes == null) return string.Empty;

		    foreach (XmlNode attribute in languageNodeAttributes)
		    {
			    if (attribute.Name.Equals("lcid"))
			    {
				    languageLcid = attribute.Value;
			    }

		    }
		    int.TryParse(languageLcid, out int lcid);
		    var languageCultureInfoName = new CultureInfo(lcid).Name;

		    return languageCultureInfoName;
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

                    if (fileName.Equals(code + ".bmp"))
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
