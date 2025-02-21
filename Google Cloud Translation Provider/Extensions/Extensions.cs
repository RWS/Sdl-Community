using System.Linq;

namespace GoogleCloudTranslationProvider.Extensions
{
	public static class Extensions
	{
		public static string SetProviderName(this string customName, bool useCustomName, ApiVersion currentVersion)
		{
			var providerNamePrefix = Constants.GoogleNaming_ShortName;
			string providerNameSufix;
			if (string.IsNullOrEmpty(customName) || !useCustomName)
			{
				providerNameSufix = currentVersion == ApiVersion.V2
								  ? Constants.GoogleVersion_V2_FullName
								  : Constants.GoogleVersion_V3_FullName;
			}
			else
			{
				var providerVersion = currentVersion == ApiVersion.V2
									? Constants.GoogleVersion_V2_ShortName
									: Constants.GoogleVersion_V3_ShortName;
				providerNameSufix = $"{providerVersion} {customName}";
			}

			return $"{providerNamePrefix} - {providerNameSufix}";
		}

		public static string EncodeSpecialChars(this string text)
		{
			text = text.Replace("#", "%23");
			text = text.Replace("&", "%26");
			text = text.Replace(";", "%3b");
			return text;
		}

		public static string RemoveZeroWidthSpaces(this string text)
		{
			var charArr = text.ToCharArray()
							  .Where(val => val != (char)8203)
							  .ToArray();
			return new string(charArr);
		}
	}
}