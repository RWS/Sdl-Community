using System.IO;
using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Models
{
	public class GCTPAdvancedSettings
	{
		public GCTPAdvancedSettings()
		{
			LoadState();
		}

		public string DownloadPath { get; set; }

		public string DownloadFileName { get; set; }

		public bool AlwaysResendDrafts { get; set; }

		public bool AlwaysSendPlainText { get; set; }

		public void SaveState()
		{
			EnsureFileExists();
			var jsonString = JsonConvert.SerializeObject(this);
			File.WriteAllText(Constants.AdvancedSettingsOnPath, jsonString);
		}

		public void LoadState()
		{
			EnsureFileExists();
			var jsonContent = new StreamReader(Constants.AdvancedSettingsOnPath).ReadToEnd();
			dynamic savedSettings = JsonConvert.DeserializeObject(jsonContent);
			var errorMessage = string.Empty;
			foreach (var savedSetting in savedSettings)
			{
				try
				{
					switch (savedSetting.Name)
					{
						case nameof(DownloadPath):
							DownloadPath = savedSettings.DownloadPath;
							break;
						case nameof(DownloadFileName):
							DownloadFileName = savedSettings.DownloadFileName;
							break;
						case nameof(AlwaysResendDrafts):
							AlwaysResendDrafts = savedSettings.AlwaysResendDrafts;
							break;
						case nameof(AlwaysSendPlainText):
							AlwaysSendPlainText = savedSettings.AlwaysSendPlainText;
							break;
					}
				}
				catch
				{
					errorMessage = PluginResources.AdvancedSettings_CorruptedFile;
				}
			}

			/*if (errorMessage == string.Empty)
			{

			}*/
		}

		public void Clear()
		{
			DownloadPath = Constants.AppDataFolder;
			DownloadFileName = Constants.DefaultDownloadedJsonFileName.Substring(0, Constants.DefaultDownloadedJsonFileName.IndexOf(".json"));
			AlwaysResendDrafts = false;
			AlwaysSendPlainText = false;
		}

		private void EnsureFileExists()
		{
			if (!Directory.Exists(Constants.AppDataFolder))
			{
				Directory.CreateDirectory(Constants.AppDataFolder);
			}

			if (!File.Exists(Constants.AdvancedSettingsOnPath))
			{
				File.Create(Constants.AdvancedSettingsOnPath).Close();
				Clear();
				var jsonString = JsonConvert.SerializeObject(this);
				File.WriteAllText(Constants.AdvancedSettingsOnPath, jsonString);
			}
		}
	}
}