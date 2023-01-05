using System.IO;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Models
{
	public class GCTPAdvancedSettings
	{
		public GCTPAdvancedSettings()
		{
			LoadState();
		}

		public bool PersistV3Project { get; set; }
		public string V3Path { get; set; } = Constants.AppDataFolder;

		public string DownloadPath { get; set; }

		public string DownloadFileName { get; set; }

		public bool AlwaysResendDrafts { get; set; }

		public bool AlwaysSendPlainText { get; set; }

		[JsonIgnore]
		public string ErrorMessage { get; private set; }

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
					ErrorMessage = "The file that contains the default settings have been corrupted.";
				}
			}
		}

		public void Clear()
		{
			PersistV3Project = false;
			V3Path = null;
			DownloadPath = Constants.AppDataFolder;
			DownloadFileName = "downloadedProject";
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