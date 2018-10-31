using System;

namespace Sdl.Community.HunspellDictionaryManager.Helpers
{
	public static class Constants
	{
		public static readonly string BackupFolderPath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community", Environment.UserName);
		public static readonly string Backup2017HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2017\HunspellDictionaries";
		public static readonly string Backup2019HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2019\HunspellDictionaries";
		public static readonly string Restore2017HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2017\RestoreHunspellDictionaries";
		public static readonly string Restore2019HunspellDicFolderPath = @"HunspellDictionaryManager\Studio2019\RestoreHunspellDictionaries";
		public static readonly string HunspellDictionaries = "HunspellDictionaries";

		public static readonly string Visible = "Visible";
		public static readonly string Hidden = "Hidden";
		public static readonly string RedColor = "Red";
		public static readonly string GreenColor = "#A4D65E";
		public static readonly string ConfigFileName = "spellcheckmanager_config.xml";

		public static readonly string HelpLink = "https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3316.hunspell-dictionary-manager";
		public static readonly string Studio2017ErrorMessage = "Application error occured: Studio 2017 needs to be installed!";
		public static readonly string Studio2019ErrorMessage = "Application error occured: Studio 2019 needs to be installed!";
		public static readonly string InformativeMessage = "Informative message";
		public static readonly string SuccessfullCreateMessage = "Dictionary language was successfully created!";
		public static readonly string SuccessfullDeleteMessage = "Dictionary language was successfully deleted!";
		public static readonly string DictionaryAlreadyExists = "Dictionary already exists! Are you sure you want to override?";
		public static readonly string RestoreSuccessMessage = "Hunspell dictionary was successfully restored!";

		public static readonly string LanguageAlreadyExists = "Hunspell language dictionary already exists with specified configuration!";
		public static readonly string NoLanguageDictionaryFound = "Please select a language dictionary for deletion!";

		public static readonly string ControlsStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
		public static readonly string ColorsStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml";
		public static readonly string FontsStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml";
		public static readonly string GreenAccentStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/Green.xaml";
		public static readonly string BaseLightAccentStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml";
		public static readonly string FlatButtonStylePath = "pack://application:,,,/MahApps.Metro;component/Styles/FlatButton.xaml";
	}
}