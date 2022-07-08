using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32.SafeHandles;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;
using NLog;
using LogManager = NLog.LogManager;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ShortcutService : IShortcutService,IDisposable
	{
		private readonly List<StudioShortcut> _customShortcuts;
		private readonly string _settingsXmlPath;
		private readonly string _settingsFolderPath;
		private readonly string _settingsFileName;
		private readonly KeysConverter _keysConverter;
		private FileSystemWatcher _fileWatcher;
		private readonly IMessageBoxService _messageService;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public event ShortcutChangedEventRaiser StudioShortcutChanged;
		// Instantiate a SafeHandle instance.
		private readonly SafeHandle _safeHandle;
		// Public implementation of Dispose pattern callable by consumers.
		public void Dispose() => Dispose(true);

		public ShortcutService(VersionService versionService, IMessageBoxService messageBoxService)
		{
			_settingsFileName = "UserSettings.xml";
			_messageService = messageBoxService;
			_settingsFolderPath = versionService.GetAppDataStudioFolder();
			_settingsXmlPath =  Path.Combine(_settingsFolderPath,_settingsFileName);
			_safeHandle = new SafeFileHandle(IntPtr.Zero, true);
			_customShortcuts = new List<StudioShortcut>();
			_keysConverter = new KeysConverter();

			ReadCustomShortcutsFromUserSettingsXml();
			InitializeSettingsFileWatcher();
		}

		/// <summary>
		/// Helper method used to check if the file has been released from the os
		/// </summary>
		public bool IsFileInUse(FileInfo file)
		{
			FileStream stream = null;
			try
			{
				stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			}
			catch (IOException)
			{
				return true;
			}
			finally
			{
				stream?.Close();
			}
			return false;
		}

		private void InitializeSettingsFileWatcher()
		{
			if (!Directory.Exists(_settingsFolderPath))
			{
				var msg = String.Format(PluginResources.InvalidPathOrDirectoryDoesntExist, _settingsFolderPath);
				_messageService.ShowErrorMessage(msg, PluginResources.InvalidPath);
				_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name}, Error:" + msg);
				return;
			}
			_fileWatcher = new FileSystemWatcher(_settingsFolderPath)
			{
				Filter = _settingsFileName
			};

			_fileWatcher.Changed += UserSettingsFileChanged;
			_fileWatcher.EnableRaisingEvents = true;
		}

		private void UserSettingsFileChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType != WatcherChangeTypes.Changed) return;
			Dispose();

			ReadCustomShortcutsFromUserSettingsXml();
			InitializeSettingsFileWatcher();
			StudioShortcutChanged?.Invoke();
		}

		public string GetShortcutDetails(string actionId)
		{			
			return _customShortcuts.FirstOrDefault(a => a.ActionId.Equals(actionId))?.ShortcutText;
		}

		private void ReadCustomShortcutsFromUserSettingsXml()
		{
			try
			{
				if (!File.Exists(_settingsXmlPath))
				{
					var msg = string.Format(PluginResources.InvalidPathInFileName, _settingsXmlPath);
					_messageService.ShowErrorMessage(msg, PluginResources.InvalidPathOrFileDoesntExist);
					_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name}, Error:" + msg);
					return;
				}

				var userSettingsDocument = new XmlDocument();
				while (IsFileInUse(new FileInfo(_settingsXmlPath)))
				{
					//loop until filehandle is released
					Thread.Sleep(500);
				}
				userSettingsDocument.Load(_settingsXmlPath);
				_customShortcuts.Clear();

				//selects all "settings" nodes
				var shortcutSettingsGroup = userSettingsDocument.SelectNodes("//SettingsGroup[@Id='ShortcutsSettingsGroup']/*");

				if (shortcutSettingsGroup is null) return;
				foreach (XmlNode settingsGr in shortcutSettingsGroup)
				{
					var settingsChilds = settingsGr.ChildNodes;
					var settingsId = settingsGr.Attributes?["Id"].Value;

					if (string.IsNullOrEmpty(settingsId)) continue;
					var studioShortcut = new StudioShortcut
					{
						ActionId = settingsId,
						ShortcutCombination = settingsChilds[0]?.InnerText
					};
					if (!string.IsNullOrEmpty(studioShortcut.ShortcutCombination))
					{
						var keyCombination = (Keys)Enum.Parse(typeof(Keys), studioShortcut.ShortcutCombination, true);

						studioShortcut.ShortcutText = _keysConverter.ConvertToString(keyCombination);
					}
					_customShortcuts.Add(studioShortcut);
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{System.Reflection.MethodBase.GetCurrentMethod().Name} " + e.Message);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			// Dispose managed state (managed objects).
			_safeHandle?.Dispose();
			if (_fileWatcher != null)
			{
				_fileWatcher.Changed -= UserSettingsFileChanged;
				_fileWatcher.Dispose();
			}
			_fileWatcher = null;
		}
	}
}
