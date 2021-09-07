using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using RwsAppStore.UsefulTipsService;
using RwsAppStore.UsefulTipsService.Model;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services
{
	public class UsefulTipsService
	{
		private readonly TipsProvider _tipsProvider;
		private readonly SettingsService _settingsService;

		public UsefulTipsService(TipsProvider tipsProvider, SettingsService settingsService)
		{
			_tipsProvider = tipsProvider;
			_settingsService = settingsService;
		}

		public int AddUsefulTips(List<TipContext> tipContexts, string applicationName)
		{
			return _tipsProvider.AddTips(tipContexts, applicationName);
		}

		public int RemoveUsefulTips(List<TipContext> tipContexts, string applicationName)
		{
			return _tipsProvider.RemoveTips(tipContexts, applicationName);
		}

		public List<TipContext> GetPluginUsefulTips()
		{
			var tipContexts = new List<TipContext>();
			foreach (var language in _tipsProvider.SupportedLanguages)
			{
				var tipsLanguageFullPath = GetTipsLanguagePath(language);
				var success = GetTipsImportResources(tipsLanguageFullPath, language);
				if (success)
				{
					var tipsImportFile = Path.Combine(tipsLanguageFullPath, "TipsImport.xml");
					var tips = NormalizePathInformation(_tipsProvider, tipsImportFile, tipsLanguageFullPath);

					if (tips?.Count > 0)
					{
						tipContexts.Add(new TipContext
						{
							LanguageId = language,
							Tips = tips
						});
					}
				}
			}

			return tipContexts;
		}

		public int TipsInstalled(string context)
		{
			var allTipContexts = _tipsProvider.GetAllTips();
			return allTipContexts.Sum(tipContext => tipContext.Tips.Count(a => a.Context == context));
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

		private bool GetTipsImportResources(string tipsLanguageFullPath, string languageId)
		{
			RemoveAllFilesInPath(tipsLanguageFullPath);

			var tipsOutputFile = Path.Combine(tipsLanguageFullPath, languageId + ".zip");

			var success = WriteEmbeddedResourceToFile(languageId, tipsOutputFile);
			if (success)
			{
				try
				{
					ZipFile.ExtractToDirectory(tipsOutputFile, tipsLanguageFullPath);
				}
				catch
				{
					//ignore; catch all
				}

				return true;
			}

			return false;
		}

		private List<Tip> NormalizePathInformation(TipsProvider ripsProvider, string tipsImportFile, string tipsLanguageFullPath)
		{
			// 1. read the tips from the import file
			var tips = ripsProvider.ReadTipsImportFile(tipsImportFile);

			// 2. update the relative path info for each of the tips
			foreach (var tip in tips)
			{
				tip.Icon = GetFullPath(tip.Icon, tipsLanguageFullPath);
				tip.Content = GetFullPath(tip.Content, tipsLanguageFullPath);
				tip.DescriptionImage = GetFullPath(tip.DescriptionImage, tipsLanguageFullPath);
			}

			// 3. write the import file with the updated tips
			ripsProvider.CreateTipsImportFile(tipsImportFile, tips);

			return tips;
		}

		private string GetFullPath(string fileName, string tipsLanguageFullPath)
		{
			return !string.IsNullOrEmpty(fileName)
				? Path.Combine(tipsLanguageFullPath, Path.GetFileName(fileName))
				: fileName;
		}

		private bool WriteEmbeddedResourceToFile(string languageId, string tipsContentFileOutput)
		{
			var tipsEmbeddedResource = typeof(PluginResources).Namespace + ".SdlTmAnonymizer.UsefulTips." + languageId + "." + languageId + ".zip";
			using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(tipsEmbeddedResource))
			{
				if (stream == null)
				{
					return false;
				}

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

			return true;
		}

		private void RemoveAllFilesInPath(string tipsLanguageFullPath)
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
				//ignore; catch all
			}
		}
	}
}
