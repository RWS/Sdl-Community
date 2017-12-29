using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AdaptiveMT.Service.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.AdaptiveMT.Helpers
{
	public static class Credentials
	{
		/// <summary>
		/// Get user logged in Studio using Best Match Service dll
		/// Read the credentials saved in cache
		/// </summary>
		/// <returns></returns>
		public static UserRequest GetCredentials()
		{
			var bestMatchAssembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.BestMatchServiceStudioIntegration.Common.dll"));
			var languageCloudCredentialCacheType = bestMatchAssembly.GetType("Sdl.BestMatchServiceStudioIntegration.Common.Account.Model.LanguageCloudCredentialCache");

			//Read credentials from cache
			var getCredentialMethod = languageCloudCredentialCacheType.GetMethod("get_Credential");
			if (getCredentialMethod != null)
			{
				dynamic userDetails = getCredentialMethod.Invoke(null, null);
				if (userDetails != null)
				{
					var userRequest = new UserRequest
					{
						Email = userDetails.UserName,
						Password = userDetails.Password
					};
					return userRequest;
				}
			}
			return null;
		}
		/// <summary>
		/// Get Studio location
		/// </summary>
		/// <returns></returns>
		private static string ExecutingStudioLocation()
		{
			var entryAssembly = Assembly.GetEntryAssembly().Location;
			var location = entryAssembly.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

			return location;
		}
	}
}
