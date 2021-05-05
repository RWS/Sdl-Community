using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;
using NLog;

namespace Sdl.Community.DeepLMTProvider.WPF
{
	public static class Helpers
	{
		public static readonly HttpClient Client = new HttpClient();

		private static readonly Logger _logger = Log.GetLogger(nameof(Helpers));

		static Helpers()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
			Client.Timeout = TimeSpan.FromMinutes(5);
			var pluginVersion = GetPluginVersion();
			Client.DefaultRequestHeaders.Add("Trace-ID", $"SDL Trados Studio 2021 /plugin {pluginVersion}");
		}

		private static List<string> FormalityIncompatibleTargetLanguages { get; set; }

		public static bool? AreLanguagesCompatibleWithFormalityParameter(List<CultureInfo> targetLanguages)
		{
			if (FormalityIncompatibleTargetLanguages is null) return null;
			return targetLanguages.All(IsLanguageCompatible);
		}

		public static List<string> GetSupportedSourceLanguages(string apiKey)
		{
			var supportedLanguages = new List<string>();
			try
			{
				var response = GetSupportedLanguages("source", apiKey);
				supportedLanguages = JArray.Parse(response).Select(item => item["language"].ToString().ToUpperInvariant()).ToList();
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public static Dictionary<string, bool> GetSupportedTargetLanguages(string apiKey)
		{
			var supportedLanguages = new Dictionary<string, bool>();
			try
			{
				var response = GetSupportedLanguages("target", apiKey);
				supportedLanguages =
					JArray.Parse(response).ToDictionary(
						item => item["language"].ToString().ToUpperInvariant(), item => bool.Parse(item["supports_formality"].ToString()));
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex}");
			}

			return supportedLanguages;
		}

		public static void ResetFormalityIncompatibleLanguages()
		{
			FormalityIncompatibleTargetLanguages.Clear();
		}

		public static void SetFormalityIncompatibleLanguages(string apiKey)
		{
			FormalityIncompatibleTargetLanguages =
				GetSupportedTargetLanguages(apiKey).Where(sl => !sl.Value).Select(sl => sl.Key.ToLower()).ToList();
		}

		private static string GetPluginVersion()
		{
			try
			{
				// fetch the version of the plugin from the manifest deployed
				var pexecutingAsseblyPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				pexecutingAsseblyPath = Path.Combine(pexecutingAsseblyPath, "pluginpackage.manifest.xml");
				var doc = new XmlDocument();
				doc.Load(pexecutingAsseblyPath);

				if (doc.DocumentElement?.ChildNodes != null)
				{
					foreach (XmlNode n in doc.DocumentElement.ChildNodes)
					{
						if (n.Name == "Version")
						{
							return n.InnerText;
						}
					}
				}
			}
			catch (Exception e)
			{
				// broad catch here, if anything goes wrong with determining the version we don't want the user to be disturbed in any way
			}
			return string.Empty;
		}

		private static string GetSupportedLanguages(string type, string apiKey)
		{
			var content = new StringContent($"type={type}" + $"&auth_key={apiKey}", Encoding.UTF8,
				"application/x-www-form-urlencoded");

			var response = Client.PostAsync("https://api.deepl.com/v1/languages", content).Result;
			response.EnsureSuccessStatusCode();

			return response.Content?.ReadAsStringAsync().Result;
		}

		private static bool IsLanguageCompatible(CultureInfo targetLanguage)
		{
			return !FormalityIncompatibleTargetLanguages.Contains(targetLanguage.ToString().ToLower()) &&
				   !FormalityIncompatibleTargetLanguages.Contains(targetLanguage.TwoLetterISOLanguageName.ToLower());
		}
	}
}