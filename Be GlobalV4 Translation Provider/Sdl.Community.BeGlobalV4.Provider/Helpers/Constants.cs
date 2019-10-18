namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class Constants
	{
		public static readonly string Authentication = @"Authentication";
		public static readonly string PluginName = "SDL Machine Translation Cloud provider";
		public static readonly string SDLMachineTranslationCloud = "SDLMachineTranslationCloud";
		public static readonly string TraceId = "Trace-ID";
		public static readonly string FAILED = "FAILED";
		public static readonly string INIT = "INIT";
		public static readonly string DONE = "DONE";
		public static readonly string TRANSLATING = "TRANSLATING";
		public static readonly string Authorization = "Authorization";
		public static readonly string StudioAuthentication = "Studio Authentication (via SDL ID)";
		public static readonly string StudioAuthenticationType = "StudioAuthentication";
		public static readonly string APICredentials = "API Credentials";
		public static readonly string APICredentialsType = "APICredentials";
		public static readonly string SDLMachineTranslationCloudProvider = "SDLMachineTranslationCloudProvider";		

		// Logging
		public static readonly string TranslateTextMethod = "Translate text method: ";
		public static readonly string SubscriptionInfoMethod = "Subscription info method: ";
		public static readonly string WaitTranslationMethod = "Wait for translation method: ";
		public static readonly string ErrorCode = "Error code:";
		public static readonly string EditWindow = "Edit window:";
		public static readonly string Browse = "Browse:";
		public static readonly string SupportsLanguageDirection = "SupportsLanguageDirection: ";
		public static readonly string BeGlobalV4Translator = "BeGlobalV4Translator constructor: ";
		public static readonly string GetUserInformation = "GetUserInformation method: ";
		public static readonly string IsWindowValid = "Is window valid method: ";

		// Messages
		public static readonly string UnauthorizedCredentials = "Unauthorized: Please check your credentials.";
		public static readonly string UnauthorizedToken = "Unauthorized: Translate text using refresh token \nTrace-Id:";
		public static readonly string UnauthorizedUserInfo = "Unauthorized: Get UserInfo using refresh token\nTrace-Id:";
		public static readonly string UnauthorizedLanguagePairs = "Unauthorized: Get Language Pairs using refresh token \nTrace-Id:";
		public static readonly string ForbiddenLicense = "Forbidden: Please check your license";
		public static readonly string TokenFailed = "Acquiring token failed: ";
		public static readonly string CredentialsValidation = "Please fill the Client Id and Client Secret!";
	}
}