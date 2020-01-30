namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class Constants
	{
		public readonly string PluginName = "SDL Machine Translation Cloud provider";
		public readonly string SDLMachineTranslationCloud = "SDLMachineTranslationCloud";
		public readonly string SDLMTCloud = "SDL Machine Translation Cloud";
		public readonly string SDLCommunity = "SDL Community";
		public readonly string TraceId = "Trace-ID";
		public readonly string FAILED = "FAILED";
		public readonly string INIT = "INIT";
		public readonly string DONE = "DONE";
		public readonly string TRANSLATING = "TRANSLATING";
		public readonly string Authorization = "Authorization";
		public readonly string SDLMachineTranslationCloudProvider = "SDLMachineTranslationCloudProvider";
		public readonly string ClientAuthentication = "Client Authentication";
		public readonly string UserAuthentication = "User Authentication";
		public readonly string Client = "Client";
		public readonly string User = "User";
		public readonly string PasswordBox = "PasswordBox";
		public readonly string NullValue = "Value cannot be null.";
		public readonly string Red = "Red";
		public readonly string Green = "Green";
		public readonly string PrintMTCodes = "Printing MT Codes";
		public readonly string SettingsGrId = "SDLMTCloudLanguageMapping";
		public readonly string HostAddress = "translate-api.sdlbeglobal.com";

		// Excel MTCodes values
		public readonly string ExcelSheet = "Sheet1";

		// Logging
		public readonly string TranslateTextMethod = "Translate text method: ";
		public readonly string SubscriptionInfoMethod = "Subscription info method: ";
		public readonly string WaitTranslationMethod = "Wait for translation method: ";
		public readonly string ErrorCode = "Error code:";
		public readonly string EditWindow = "Edit window:";
		public readonly string Browse = "Browse:";
		public readonly string SupportsLanguageDirection = "SupportsLanguageDirection: ";
		public readonly string BeGlobalV4Translator = "BeGlobalV4Translator constructor: ";
		public readonly string GetClientInformation = "GetClientInformation method: ";
		public readonly string GetUserInformation = "GetUserInformation method: ";
		public readonly string IsWindowValid = "Is window valid method: ";
		public readonly string IsEmailValid = "IsEmailValid method: ";
		public readonly string ExcelExecuteAction = "BeGlobalExcelAction Execute method: ";
		public readonly string WriteExcelLocally = "WriteExcelLocally method: ";
		public readonly string AddMTCode = "AddMTCode method: ";
		public readonly string RemoveMTCode = "RemoveMTCode method: ";
		public readonly string FormatLanguageName = "FormatLanguageName method: ";
		public readonly string SplitLanguagePair = "SplitLanguagePair method: ";
		public readonly string MTCloudServerIPMessage = "The MTCloud IP Address is: ";
		public readonly string GetDictionaries = "GetDictionaries method: ";


		// Messages
		public readonly string ForbiddenLicense = "Forbidden: Please check your license";
		public readonly string TokenFailed = "Acquiring token failed";
		public readonly string CredentialsValidation = "Please fill the credentials fields!";
		public readonly string CredentialsNotValid = "Please verify your credentials!";
		public readonly string CredentialsAndInternetValidation = "The MTCloud host could not be reached and setups cannot be saved. Please verify your credentials and internet connection, and ensure you are able to connect to the server from this computer.";
		public readonly string InternetConnection = "The The MTCloud host could not be reached. Please check the internet connection and ensure you are able to connect to the server from this computer.";
		public readonly string SuccessfullyUpdatedMessage = "The MT Code was successfully updated within the MTLanguageCodes.xlsx file.";
		public readonly string NoEnginesLoaded = "No MT engines were received from MTCloud for the current project's Language Pairs";
		public readonly string EnginesSelectionMessage = "Settings not saved! Please select the corresponding engine from the Language Mappings tab.";
		public readonly string NoTranslationMessage = "Translation cannot be received because MT Cloud engine is not set for the current Language Pair";
		public readonly string NoAvailableDictionary = "No dictionary available";
	}
}