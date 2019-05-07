using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Sdl.Core.Globalization;

namespace Sdl.Community.ApplyTMTemplate.Utilities
{
	public class LanguageFlags
	{
		public string GetImageStudioCodeByLanguageCode(string languageCode)
        {
            var studioLangsFlagsDictionary = Path.Combine(GetStudioInstallPath(), "AutoCorrectDictionaries");
            if (!Directory.Exists(studioLangsFlagsDictionary))
            {
                return GetImagePathByStudioCode(0);
            }
            foreach (var file in Directory.GetFiles(studioLangsFlagsDictionary))
            {
                var fileName = Path.GetFileName(file);

                if (fileName.ToLower().Contains(languageCode.ToLower()))
                {
                    var codes = fileName.Split('-');
                    var lCode = int.Parse(codes[codes.Length - 1]);
                    return GetImagePathByStudioCode(lCode);
                }
            }
            var iconPath = CreateNewIconFlag(languageCode);
            return iconPath;
        }

        private string CreateNewIconFlag(string languageCode)
        {
            var lang = new Language(new CultureInfo(languageCode));
            var langFlag = lang.GetFlagImage();
            var dirInfo = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "Flags"));

            if (!dirInfo.Exists)
            {
                Directory.CreateDirectory(dirInfo.FullName);
            }
            var iconPath = Path.Combine(dirInfo.FullName, languageCode);

            if (dirInfo.GetFiles().Count(f => f.FullName.Equals(iconPath.ToString())) == 0)
            {
                langFlag.Save(iconPath);
            }

            return iconPath;
        }

        private string GetImagePathByStudioCode(int code)
		{
			var studioPath = GetStudioInstallPath();
			var studioFlagsDirectory = studioPath + @"ReportResources\images\Flags";

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

		private string GetStudioInstallPath()
		{
			var entyLocation = Assembly.GetEntryAssembly().Location;
			var locationSplited = entyLocation.Split(new[] { "SDLTradosStudio.exe" }, StringSplitOptions.None);

			return locationSplited[0];
		}
	}
}
