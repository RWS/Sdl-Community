using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.Helpers
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
			var entyLocation = Assembly.GetEntryAssembly()?.Location;
			var locationSplited = entyLocation?.Split(new[] { "SDLTradosStudio.exe" }, StringSplitOptions.None);

			return locationSplited != null ? locationSplited[0] : string.Empty;
		}

		/// <summary>
		/// From GS if the project has multiple target languages we receive the value like this: "de-DE,fr-FR"
		/// </summary>
		public ObservableCollection<TargetLanguageFlag> GetTargetLanguageFlags(string projectTargetLanguage)
		{
			var targetLanguageFlags = new ObservableCollection<TargetLanguageFlag>();
			try
			{
				if (!string.IsNullOrEmpty(projectTargetLanguage))
				{
					var splitedTargetCode = projectTargetLanguage.Split(',');
					if (splitedTargetCode.Length > 0)
					{
						foreach (var targetLanguage in splitedTargetCode)
						{
							var targetLanguageFlag = new TargetLanguageFlag
							{
								Path = GetImageStudioCodeByLanguageCode(targetLanguage)
							};
							targetLanguageFlags.Add(targetLanguageFlag);
						}
					}
				}
			}
			catch (Exception e)
			{
				// here we'll add logging
			}
			return targetLanguageFlags;
		}
	}
}
