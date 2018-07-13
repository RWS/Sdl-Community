namespace Sdl.Community.SdlFreshstart.Helpers
{
	public static class FoldersDescriptionText
	{
		public static string ProjectsXml()
		{
			return
				"This option will only remove the projects.xml file, it will not physically remove your projects from your Documents folder.  It will remove them from Studio, so if you delete this file and need to put some or all of the projects back, you will have to add the projects back one at a time by using the Open Project command in Studio.  The projects.xml contains the following information:" +
				"\n - details of the Batch Tasks available to you. If you have custom Batch Tasks then these will be lost and you'll have to recreate them. " +
				"\n - details of all the projects in your Projects View.  Not the projects themselves, only metadata telling Studio what their names are and where to find them."+
				"\n - details of all the project templates available to you.  If you have custom templates then these will be lost from your options in Studio and you'll have to reselect them from wherever you saved them."+
				"\n - details of the users you have added.  These will all be lost and you'll have to recreate them." +
				"\n - details of any customers you have added. These will all be lost and you'll have to recreate them.";

		}

		public static string ProjectsTemplates()
		{
			return
				"This is the default location for your project templates.  If you used this location for custom templates then consider backing them up so you can easily replace them.  The default template will be replaced when you restart Studio but your custom templates will not."+
				"\n ";
		}

		public static string AppDataRoamingMajor()
		{
			return
				"This location contains the sdlplugins you have installed from the SDL AppStore that are intended to be used by one user and should be available to that user on any machine they log into under that same userprofile.  The plugins folder contains two folders:" +
				"\n - Packages: this is where the sdlplugin files are saved when they are installed" +
				"\n - Unpacked: sdlplugin files are essentially zip files and the unzipped versions and unpacked in this folder when Studio starts" +
				"\n If you want to replace all your plugins quickly then back up the content of your 'Packages' folder first so you only have to copy them back afterwards.  You don't need to, and in fact should not, back up the 'Unpacked' folder.";
		}

		public static string ProjectApi()
		{
			return
				"This folder contains one file, Sdl.ProjectApi.xml, which contains information relating to the location of local project folders. This is only the default since today Studio can be configured by the user to store projects wherever you like through the use of the project templates.  So unless you manually edited this file it seems unlikely that deleting it will cause any cause for concern.";
		}

		public static string AppDataRoamingMajorFull()
		{
			return
				"This folder is the most likely reason for a fresh start when working with Studio.  It contains several things." +
				"\n - AutoCorrect Dictionaries: if you make any changes to the default settings for AutoCorrect then separate files are created for each language you have edited and saved in the 'AutoCorrectDictionaries' folder.  This automatic backup file is not the same as the one you can export and if this is the source of your problem it is better to manually export these dictionaries from Studio and keep them somewhere safe so they are easy to restore." +
				"\n - UserProfiles: this folder holds the default profiles for your chosen user profile (Default, SDL Trados, SDLX).  These are used to recreate the default keyboard shortcuts in your profiles you use if you delete the UserSettings.xml.  None of your customisations are held in these files and they are replaced by the defaults in the Studio program folder if you delete them." +
				"\n - BaseSettings.xml: this file just points to where the default profile is coming from.  If you changed the location of this file then removing this file is going to reset to the location in the default UserProfiles." +
				"\n - UserSettings.xml : This file contains most of the personalisation you may have carried out in Studio.  So things like customised keyboard shortcuts, window locations, choice of colour scheme, whether or not you agreed to share data (error information for example), Translation Memory lists, your preferences in File -> Options, units used for your confirmation statistics, recently used folders for Projects, Translation memories, Files, Termbases etc.  This isn't a comprehensive list but hopefully illustrates how much of your personalised settings are held in this one file.  So deleting it is going to mean you spending time setting them all up again.  Most of the things in this file cannot be exported through the User Interface in Studio so starting again will be a manual process." +
				"\n - Settings.xml: if you have added any server connections, Translation Memories or Termbases for example, then the URL is held in here.  Credentials for logging in are not held here as they are encrypted elsewhere in the software. Deleting this file means you are going to lose all the connections you had and they need to be added again. "+
				"\n - plugincache.xml: can be used to deactivate plug-ins by default. If certain plug-ins you expect to be working are not, then one reason might be that the plugin cache has been configured to disable them by default.";

		}

		public static string AppDataLocalMajor()
		{
			return
				"This location contains the sdlplugins you have installed from the SDL AppStore that are intended to be used by one user on one machine. They will not be available to that user on another machine even if logged in with the same userprofile.  The plugins folder contains two folders:" +
				"\n - Packages: this is where the sdlplugin files are saved when they are installed" +
				"\n - Unpacked: sdlplugin files are essentially zip files and the unzipped versions and unpacked in this folder when Studio starts" +
				"\n If you want to replace all your plugins quickly then back up the content of your 'Packages' folder first so you only have to copy them back afterwards.  You don't need to, and in fact should not, back up the 'Unpacked' folder.";
		}

		public static string AppDataLocalMajorFull()
		{
			return
				"This folder mostly contains logfiles that have been generated while using the application.  They can be very useful for a developer in troubleshooting errors, but are unlikely to be useful to most users and can be deleted without cause for concern.  Might be worth backing up if you are trying to solve an error in the software however since you may be asked for them if you have a Support contract." +
				"\n The folder also contains the TranslationMemoryRepository.xml file which contains the list of Translation Memories you have open in your Translation Memories View. Deleting this file just means you will have to open them again if you want them listed in this view.  The Translation Memories themselves are not deleted.";
		}

		public static string ProgramData()
		{
			return
				"This location contains the sdlplugins you have installed from the SDL AppStore that are going to be shared across multiple users, usually in an Enterprise environment to avoid having to maintain multiple copies of the same files on every machine." +
				"\n The plugins folder contains two folders:" +
				"\n - Packages: this is where the sdlplugin files are saved when they are installed" +
				"\n - Unpacked: sdlplugin files are essentially zip files and the unzipped versions and unpacked in this folder when Studio starts" +
				"\n If you want to replace all your plugins quickly then back up the content of your 'Packages' folder first so you only have to copy them back afterwards.  You don't need to, and in fact should not, back up the 'Unpacked' folder.";
		}

		public static string ProgramDataFull()
		{
			return "Settings.xml: It stores information about your MultiTerm server connections.  Deleting it might help troubleshoot server connectivity, but you may need to recreate your logins to MultiTerm servers you may be using.";
		}

		public static string ProgramDataVersionNumber()
		{
			return
				"This folder contains two folders:" +
				"\n - Data: this contains your Studio.lic which is your encrypted Studio licence file.  This application will not delete the lic file because otherwise you would have to reset your licence.  So this file is left in place." +
				"\n - Updates: this folder contains the file necessary to use the Automatic Update.  This folder will be deleted because sometimes the problem we are trying to solve relates to a corrupted update mechanism.  If you choose to delete this you need to run a repair afterwards or the automatic update in Studio won't work.  You can run the repair with this tool.";
		}

		public static string MultiTermRoaming()
		{
			return
				"This folder contains several files and a folder supporting settings in Multiterm and MultiTermExtract if you use it." +
				"\n - UserProfiles.xml: this folder holds the MultiTerm profiles for your custom created profiles if you created any." +
				"\n - BaseSettings.xml: this file just points to where the default profile is coming from.  If you changed the location of this file then removing this file is going to reset to the location in the default UserProfiles." +
				"\n - MultiTermExtract_Connections.xml: Stores MultiTerm Server connections used in MT Extract." +
				"\n - MultiTermExtract_Settings.xml: Stores MultiTerm Extract settings." +
				"\n - plugincache.xml: can be used to deactivate plug-ins by default. If certain plug-ins you expect to be working are not, then one reason might be that the plugin cache has been configured to disable them by default." +
				"\n - Settings.xml: Last used MultiTerm Server connection." +
				"\n - UserSettings.xml: This file contains most of the personalisation you may have carried out in MultiTerm.  So things like customised keyboard shortcuts, window locations, choice of colour scheme, Termbase lists, your preferences in File -> Options, recently used locations for Termbases etc.  This isn't a comprehensive list but hopefully illustrates how much of your personalised settings are held in this one file.  So deleting it is going to mean you spending time setting them all up again.  Most of the things in this file cannot be exported through the User Interface in MultiTerm so starting again will be a manual process.";
		}

		public static string MultiTermLocal()
		{
			return
				"This folder contains nothing but log files. They can be very useful for a developer in troubleshooting errors, but are unlikely to be useful to most users and can be deleted without cause for concern.  Might be worth backing up if you are trying to solve an error in the software however since you may be asked for them if you have a Support contract.";
		}
	}
}
