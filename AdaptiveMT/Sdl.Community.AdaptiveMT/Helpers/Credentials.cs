using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AdaptiveMT.Helpers
{
	public static class Credentials
	{
		public static void GetCredentials()
		{
			var translationProviders = TranslationProviderManager.GetTranslationProviderWinFormsUIs();

			var provider =
				translationProviders.FirstOrDefault(d => d.TypeName.Equals("SDL Language &Cloud Machine Translation..."));

			if (provider != null)
			{
					var identityService = provider.GetType().GetField("LanguageCloudIdentityService",
						BindingFlags.NonPublic | BindingFlags.Instance);
					if (identityService != null)
					{
						var languageCloudCredential = identityService.FieldType.GetProperty("LanguageCloudCredential");
						if (languageCloudCredential != null)
						{
							var credentialsMethodInfo = languageCloudCredential.GetMethod;

							//var test = credentialsMethodInfo.Invoke(invokeConstrInstance, null);

						}
					}
				
				
			}
		}
	}
}
