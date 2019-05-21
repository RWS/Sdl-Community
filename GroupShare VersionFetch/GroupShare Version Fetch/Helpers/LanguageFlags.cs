using System;
using System.Collections.ObjectModel;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class LanguageFlags
	{
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
								Image = new Language(targetLanguage).GetFlagImage()
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
