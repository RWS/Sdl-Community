using System;

namespace Sdl.Community.HunspellDictionaryManager.Helpers
{
	public static class Constants
	{
		public static readonly string BackupFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\Trados AppStore", Environment.UserName);
		public static readonly string Backup2022HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2022\HunspellDictionaries";
		public static readonly string Restore2022HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2022\RestoreHunspellDictionaries";

		public static readonly string HunspellDictionaries = "HunspellDictionaries";

		public static readonly string Visible = "Visible";
		public static readonly string Hidden = "Hidden";
		public static readonly string RedColor = "Red";
		public static readonly string GreenColor = "#30c23f";
		public static readonly string ConfigFileName = "spellcheckmanager_config.xml";

		public static readonly string HelpLink = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/3316/hunspell-dictionary-manager";
		public static readonly string Studio2022ErrorMessage = "Application error occured: Studio 2022 needs to be installed!";

		public static readonly string InformativeMessage = "Informative message";
		public static readonly string SuccessfullCreateMessage = "Dictionary language was successfully created!";
		public static readonly string SuccessfullDeleteMessage = "Dictionary language was successfully deleted!";
		public static readonly string DictionaryAlreadyExists = "Dictionary already exists! Are you sure you want to override?";
		public static readonly string RestoreSuccessMessage = "Hunspell dictionary was successfully restored!";

		public static readonly string LanguageAlreadyExists = "Hunspell language dictionary already exists with specified configuration!";
		public static readonly string NoLanguageDictionaryFound = "Please select a language dictionary for deletion!";

		// Logging messages
		public static readonly string GetInstalledStudioPath = "GetInstalledStudioPath method";
		public static readonly string CopyFiles = "CopyFiles method";
		public static readonly string CopyLanguageDictionary = "CopyLanguageDictionary method";
		public static readonly string UndoAction = "UndoAction method";
		public static readonly string UpdateConfigFile = "UpdateConfigFile method";
		public static readonly string RemoveConfigLanguageNode = "RemoveConfigLanguageNode method";
		public static readonly string SetDisplayLanguageName = "SetDisplayLanguageName method";
		public static readonly string BackupHunspellDictionaries = "BackupHunspellDictionaries method";
		public static readonly string AddUndoDictionaries = "AddUndoDictionaries method";
		public static readonly string RemoveDictFromDeleteFolder = "RemoveDictFromDeleteFolder method";		
	}
}