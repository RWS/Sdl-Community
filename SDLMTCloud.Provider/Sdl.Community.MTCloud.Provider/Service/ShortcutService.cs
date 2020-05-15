using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class ShortcutService : IShortcutService
	{
		private readonly List<StudioShortcut> _customShortcuts;
		private readonly string _settingsXmlPath;
		private readonly KeysConverter _keysConverter;

		public ShortcutService()
		{
			_settingsXmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL",
				"SDL Trados Studio", "15.0.0.0", "UserSettings.xml");
			_customShortcuts = new List<StudioShortcut>();
			_keysConverter = new KeysConverter();

			ReadCustomShortcutsFromUserSettingsXml();
		}

		public string GetShotcutDetails(string actionId)
		{			
			return _customShortcuts.FirstOrDefault(a => a.ActionId.Equals(actionId))?.ShortcutText;
		}

		private void ReadCustomShortcutsFromUserSettingsXml()
		{
			if (!File.Exists(_settingsXmlPath))
			{
				return;
			}

			var userSettingsDocument = new XmlDocument();
			userSettingsDocument.Load(_settingsXmlPath);
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
					var keyCombination = (Keys) Enum.Parse(typeof(Keys), studioShortcut.ShortcutCombination, true);
					
					studioShortcut.ShortcutText = _keysConverter.ConvertToString(keyCombination);
				}
				_customShortcuts.Add(studioShortcut);
			}
		}
	}
}
