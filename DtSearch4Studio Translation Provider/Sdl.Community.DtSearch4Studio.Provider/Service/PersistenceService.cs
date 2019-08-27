using System;
using System.IO;
using Newtonsoft.Json;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;

namespace Sdl.Community.DtSearch4Studio.Provider.Service
{
	public class PersistenceService
	{
		#region Private Fields
		private readonly string _persistancePath;
		#endregion

		#region Public properties
		public static readonly Log Log = Log.Instance;
		#endregion

		#region Constructors
		public PersistenceService()
		{
			_persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.JsonPath);
		}
		#endregion

		#region Public Methods
		public void AddSettings(ProviderSettings providerSettings)
		{
			if (providerSettings == null)
			{
				throw new NullReferenceException(Constants.NoSettingsMessage);
			}
			WriteToFile(providerSettings);
		}

		public ProviderSettings GetProviderSettings()
		{
			var json = File.ReadAllText(_persistancePath);
			if (!string.IsNullOrEmpty(json))
			{
				return JsonConvert.DeserializeObject<ProviderSettings>(json);
			}
			return new ProviderSettings();
		}
		#endregion

		#region Internal methods
		internal void WriteToFile(ProviderSettings providerSettings)
		{
			try
			{
				var directoryPath = Path.GetDirectoryName(_persistancePath);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
				}					
				if(File.Exists(_persistancePath))
				{
					File.Delete(_persistancePath);
				}
				File.Create(_persistancePath).Dispose();

				var json = JsonConvert.SerializeObject(providerSettings);
				File.WriteAllText(_persistancePath, json);
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{Constants.WriteToFile}: {e.Message}\n{e.StackTrace}");
			}
		}
		#endregion
	}
}
