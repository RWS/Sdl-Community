﻿namespace Sdl.Community.MTCloud.Provider
{
	public class Constants
	{
		public static string MTCloudTranslateAPIUri = "https://translate-api.sdlbeglobal.com";
		public static string MTCloudTranslateUri = "https://translate.sdlbeglobal.com";
		public static string MTCloudUriScheme = "sdlmtcloud";		
		public static string MTCloudUriResourceUserToken = "/token/user";
		public static string MTCloudUriResourceUserDetails = "/accounts/users/self";
		public static string MTCloudUriResourceClientToken = "/token";
		public static string MTCloudUriResourceClientDetails = "/accounts/api-credentials/self";

		public static string MTCloudUriDummyToken = "DUMMY-TOKEN";

		public static string ClientLoginAuthentication = "ClientLogin";
		public static string UserLoginAuthentication= "UserLogin";

		public static string StudioAuthentication = "Studio credentials - SDL Language Cloud";
		public static string UserAuthentication = $"User credentials - {PluginResources.Plugin_NiceName}";
		public static string ClientAuthentication = $"Client credentials - {PluginResources.Plugin_NiceName}";


		// TODO confirm if some or all of these should be managed in a resource file		
		public static string SDLMachineTranslationCloud = "SDLMachineTranslationCloud";
		public static string SDLCommunity = "SDL Community";
		public static string TraceId = "Trace-ID";
		public static string FAILED = "FAILED";
		public static string INIT = "INIT";
		public static string DONE = "DONE";
		public static string TRANSLATING = "TRANSLATING";
		public static string SDLMachineTranslationCloudProvider = "SDLMachineTranslationCloudProvider";			
		public static string SettingsGroupId = "SDLMTCloudLanguageMappings";			

		// Logging
		// TODO confirm if some or all of these should be managed in a resource file
		public static string TranslateTextMethod = "Translate text method: ";
		public static string SubscriptionInfoMethod = "Subscription info method: ";
		public static string WaitTranslationMethod = "Wait for translation method: ";
		public static string ErrorCode = "Error code:";
		public static string EditWindow = "Edit window:";
		public static string Browse = "Browse:";
		public static string SupportsLanguageDirection = "SupportsLanguageDirection: ";
		public static string BeGlobalV4Translator = "BeGlobalV4Translator constructor: ";
		public static string GetClientInformation = "GetClientInformation method: ";
		public static string GetUserInformation = "GetUserInformation method: ";
		public static string IsWindowValid = "Is window valid method: ";
		public static string IsEmailValid = "IsEmailValid method: ";
		public static string ExcelExecuteAction = "BeGlobalExcelAction Execute method: ";
		public static string WriteExcelLocally = "WriteExcelLocally method: ";
		public static string AddMTCode = "AddMTCode method: ";
		public static string RemoveMTCode = "RemoveMTCode method: ";
		public static string FormatLanguageName = "FormatLanguageName method: ";
		public static string SplitLanguagePair = "SplitLanguagePair method: ";
		public static string MTCloudServerIPMessage = "The MTCloud IP Address is: ";
		public static string GetDictionaries = "GetDictionaries method: ";

		// Messages
		// TODO confirm if some or all of these should be managed in a resource file
		public static string ForbiddenLicense = "Forbidden: Please check your license";
		public static string TokenFailed = "Acquiring token failed";
		public static string CredentialsValidation = "Please fill the credentials fields!";
		public static string CredentialsNotValid = "Please verify your credentials!";
		public static string CredentialsAndInternetValidation = "The MTCloud host could not be reached and setups cannot be saved. Please verify your credentials and internet connection, and ensure you are able to connect to the server from this computer.";
		public static string InternetConnection = "The The MTCloud host could not be reached. Please check the internet connection and ensure you are able to connect to the server from this computer.";
		public static string SuccessfullyUpdatedMessage = "The MT Code was successfully updated within the MTLanguageCodes.xlsx file.";
		public static string NoEnginesLoaded = "No MT engines were received from MTCloud for the current project's Language Pairs";
		public static string EnginesSelectionMessage = "Settings not saved! Please select the corresponding engine from the Language Mappings tab.";
		public static string NoTranslationMessage = "Translation cannot be received because MT Cloud engine is not set for the current Language Pair";				
	}
}