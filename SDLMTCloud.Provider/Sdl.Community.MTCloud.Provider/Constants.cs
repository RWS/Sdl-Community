namespace Sdl.Community.MTCloud.Provider
{
	public class Constants
	{
		public static string MTCloudTranslateAPIUriEU = "https://api.languageweaver.com";
		public static string MTCloudTranslateAPIUriUS = "https://us.api.languageweaver.com";

		public static string MTCloudTranslateAPIUrlEULogin = "https://portal.languageweaver.com/login";
		public static string MTCloudTranslateAPIUrlUSLogin = "https://us.portal.languageweaver.com/login";

		public static string MTCloudUriScheme = "sdlmtcloud";		
		public static string MTCloudUriResourceUserToken = "/token/user";
		public static string MTCloudUriResourceUserDetails = "/accounts/users/self";
		public static string MTCloudUriResourceClientToken = "/token";
		public static string MTCloudUriResourceClientDetails = "/accounts/api-credentials/self";

		public static string MTCloudUriDummyToken = "DUMMY-TOKEN";

		public static string ClientLoginAuthentication = "ClientLogin";
		public static string UserLoginAuthentication= "UserLogin";

		public static string StudioAuthentication = "Single Sign-On";
		public static string UserAuthentication = $"User credentials - {PluginResources.SDLMTCloud_Provider_Name}";
		public static string ClientAuthentication = $"Client credentials - {PluginResources.SDLMTCloud_Provider_Name}";

		// TODO confirm if some or all of these should be managed in a resource file		
		public static string SDLMachineTranslationCloud = "Language Weaver";
		public static string TradosAppStore = "Trados AppStore";
		public static string TraceId = "Trace-ID";
		public static string FAILED = "FAILED";
		public static string INIT = "INIT";
		public static string DONE = "DONE";
		public static string TRANSLATING = "TRANSLATING";
		public static string LanguageWeaver = "LanguageWeaverProvider";			
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
		public static string MTCloudServerIPMessage = "The Language Weaver IP Address is: ";
		public static string GetDictionaries = "GetDictionaries method: ";

		// Messages
		// TODO confirm if some or all of these should be managed in a resource file
		public static string ForbiddenLicense = "Forbidden: Please check your license";
		public static string TokenFailed = "Acquiring token failed";
		public static string CredentialsValidation = "Please fill the credentials fields!";
		public static string CredentialsNotValid = "Please verify your credentials!";
		public static string CredentialsAndInternetValidation = "The Language Weaver host could not be reached and setups cannot be saved. Please verify your credentials and internet connection, and ensure you are able to connect to the server from this computer.";
		public static string InternetConnection = "The Language Weaver host could not be reached. Please check the internet connection and ensure you are able to connect to the server from this computer.";
		public static string SuccessfullyUpdatedMessage = "The MT Code was successfully updated within the MTLanguageCodes.xlsx file.";
		public static string NoEnginesLoaded = "No LW engines were received from Language Weaver for the current project's Language Pairs";
		public static string EnginesSelectionMessage = "Settings not saved! Please select the corresponding engine from the Language Mappings tab.";
		public static string NoTranslationMessage = "Translation cannot be received because LW engine is not set for the current Language Pair";
	}
}