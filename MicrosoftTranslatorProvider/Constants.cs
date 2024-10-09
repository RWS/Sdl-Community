using System;
using System.IO;

namespace MicrosoftTranslatorProvider
{
	public static class Constants
	{
		public const string MicrosoftProviderUriBase = "api.cognitive.microsofttranslator.com";
		public const string MicrosoftProviderServiceUriBase = "api.cognitive.microsoft.com";
		public const string MicrosoftProviderScheme = "microsofttranslatorprovider";
		public const string MicrosoftProviderFullScheme = "microsofttranslatorprovider:///";

		public const string MicrosoftProviderPrivateEndpointScheme = "microsofttranslatorproviderprivateendpoint:///";

		public const string MicrosoftNaming_FullName = "Microsoft Translator Provider";
		public const string MicrosoftTranslatorString = "Microsoft Translator";
		public const string OcpApimSubscriptionKeyHeader = "Ocp-Apim-Subscription-Key";
		public const string OcpApimSubscriptionRegionHeader = "Ocp-Apim-Subscription-Region";

		public const string TellMe_StoreUrl = "https://appstore.rws.com/plugin/179/";
		public const string TellMe_HelpUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/6546/microsoft-machine-translation-provider";
		public const string TellMe_CommunityForumUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/";

		public const string LearnMore_Regions = "https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/change-organization-location?view=azure-devops/";
		public const string Navigate_Categories = "https://portal.customtranslator.azure.ai/workspaces";
		public const string LearnMore_Category = "https://learn.microsoft.com/en-us/azure/cognitive-services/translator/custom-translator/overview/";


		public static string DatabaseLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Trados AppStore\\Language Mapping Provider";
		public static string DatabaseFilePath = Path.Combine(DatabaseLocation, $"{DatabaseName}data.sqlite3");
		public const string DatabaseName = "microsoft";
	}
}