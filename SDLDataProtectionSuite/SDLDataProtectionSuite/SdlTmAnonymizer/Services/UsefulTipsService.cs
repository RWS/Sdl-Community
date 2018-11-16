using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Sdl.Community.UsefulTipsService;
using Sdl.Community.UsefulTipsService.Model;
using Sdl.Community.UsefulTipsService.Services;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class UsefulTipsService
	{
		private readonly SettingsService _settingsService;

		public UsefulTipsService(SettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		public void AddUsefulTips()
		{
			try
			{				
				var pathService = new PathService();
				var tipsProvider = new TipsProvider(pathService);

				var tipContexts = new List<TipContext>();
				foreach (var language in tipsProvider.SupportedLanguages)
				{					
					var tipsLanguageFullPath = GetTipsLanguagePath(language);

					CreateTipsImportContent(tipsLanguageFullPath, language);

					var tipsImportFile = Path.Combine(tipsLanguageFullPath, "TipsImport.xml");
					var tips = GetTipsForImport(tipsProvider, tipsImportFile, tipsLanguageFullPath);

					if (tips?.Count > 0)
					{
						tipContexts.Add(new TipContext
						{
							LanguageId = language,
							Tips = tips
						});
					}
				}

				tipsProvider.AddTips(tipContexts, StringResources.SDLTM_Anonymizer_Name);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.Message);
			}
		}

		private static List<Tip> GetTipsForImport(TipsProvider tipsService, string tipsImportFile, string tipsLanguageFullPath)
		{
			var tips = tipsService.ReadTipsImportFile(tipsImportFile);

			// update the relative path info for each of the Tips
			foreach (var tip in tips)
			{
				tip.Icon = GetFullPath(tip.Icon, tipsLanguageFullPath);
				tip.Content = GetFullPath(tip.Content, tipsLanguageFullPath);
				tip.DescriptionImage = GetFullPath(tip.DescriptionImage, tipsLanguageFullPath);
			}

			// save the updated tips with the relative path info
			tipsService.CreateTipsImportFile(tipsImportFile, tips);

			return tips;
		}

		private string GetTipsLanguagePath(string languageId)
		{
			var tipsLanguageFullPath = Path.Combine(_settingsService.PathInfo.TipsFullPath, languageId);
			if (!Directory.Exists(tipsLanguageFullPath))
			{
				Directory.CreateDirectory(tipsLanguageFullPath);
			}

			return tipsLanguageFullPath;
		}

		private static string GetFullPath(string fileName, string tipsLanguageFullPath)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				var name = Path.GetFileName(fileName);
				return Path.Combine(tipsLanguageFullPath, name);
			}

			return fileName;
		}

		private static void CreateTipsImportContent(string tipsLanguageFullPath, string languageId)
		{
			RemoveAllFilesInPath(tipsLanguageFullPath);

			var tipsContentFileInput = "Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.UsefulTips." + languageId + "." + languageId + ".zip";
			var tipsContentFileOutput = Path.Combine(tipsLanguageFullPath, languageId + ".zip");

			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tipsContentFileInput))
			{
				if (stream != null)
				{
					using (var reader = new BinaryReader(stream))
					{
						using (Stream writer = File.Create(tipsContentFileOutput))
						{
							var buffer = new byte[2048];
							while (true)
							{
								var current = reader.Read(buffer, 0, buffer.Length);
								if (current == 0)
								{
									break;
								}

								writer.Write(buffer, 0, current);
							}
						}
					}
				}
			}

			ZipFile.ExtractToDirectory(tipsContentFileOutput, tipsLanguageFullPath);
		}

		private static void RemoveAllFilesInPath(string tipsLanguageFullPath)
		{
			try
			{
				var files = Directory.GetFiles(tipsLanguageFullPath, "*.*");
				foreach (var file in files)
				{
					File.Delete(file);
				}
			}
			catch
			{
				//ignore
			}
		}
	}
}
