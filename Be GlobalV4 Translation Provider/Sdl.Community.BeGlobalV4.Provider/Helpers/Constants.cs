namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public static class Constants
	{
		public static readonly string Authentication = @"Authentication";
		public static readonly string PluginName = "Machine Translation Cloud Provider";
		public static readonly string MachineTranslationCloud = "MachineTranslationCloud";
		public static readonly string TraceId = "Trace-ID";
		public static readonly string FAILED = "FAILED";
		public static readonly string INIT = "INIT";
		public static readonly string DONE = "DONE";
		public static readonly string TRANSLATING = "TRANSLATING";
		public static readonly string Authorization = "Authorization";
		

		// Logging
		public static readonly string TranslateTextMethod = "Translate text method:";
		public static readonly string SubscriptionInfoMethod = "Subscription info method:";
		public static readonly string WaitTranslationMethod = "Wait for translation method:";
		public static readonly string ErrorCode = "Error code:";

		// Messages
		public static readonly string UnauthorizedCredentials = "Unauthorized: Please check your credentials.";
		public static readonly string UnauthorizedToken = "Unauthorized: Translate text using refresh token \nTrace-Id:";
		public static readonly string UnauthorizedUserInfo = "Unauthorized: Get UserInfo using refresh token\nTrace-Id:";
		public static readonly string UnauthorizedLanguagePairs = "Unauthorized: Get Language Pairs using refresh token \nTrace-Id:";
		public static readonly string ForbiddenLicense = "Forbidden: Please check your license";
		public static readonly string TokenFailed = "Acquiring token failed: ";

	}
}