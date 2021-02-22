using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using NLog;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace IATETerminologyProvider.Helpers
{
	public static class Utils
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public static string RemoveUriForbiddenCharacters(this string uriString)
		{
			var regex = new Regex(@"[$%+!*'(), ]");
			return regex.Replace(uriString, "");
		}

		public static string UppercaseFirstLetter(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}

			var textInfo = CultureInfo.CurrentCulture.TextInfo;
			return textInfo.ToTitleCase(s);
		}

		/// <summary>
		/// We need the project name to create the connection to db. User can access the project from multiple locations in Studio
		/// </summary>
		/// <returns>Active Studio project name</returns>
		public static string GetCurrentProjectName()
		{
			var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();

			var projectsControllerActiveProj = projectsController?.CurrentProject?.GetProjectInfo();
			if (projectsControllerActiveProj != null)
			{
				return projectsControllerActiveProj.Name;
			}

			Logger.Error("Current project name could not be obtained");
			return string.Empty;
		}

		public static void AddDefaultParameters(HttpClient httpClient)
		{
			//we accept only application/json because it is the only encoding we can handle at the moment
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Connection.Add("Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
			httpClient.DefaultRequestHeaders.Add("Origin", "https://iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
			httpClient.DefaultRequestHeaders.Add("Host", "iate.europa.eu");
			httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
			httpClient.Timeout = TimeSpan.FromMinutes(2);
		}
	}
}