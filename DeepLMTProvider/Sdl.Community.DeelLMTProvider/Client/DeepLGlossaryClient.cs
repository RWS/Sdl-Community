using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Client
{
	public class DeepLGlossaryClient
	{
		private static readonly Logger Logger = Log.GetLogger(nameof(DeepLGlossaryClient));

		private static List<Glossary> TryDeserializeGlossaries(string serializedGlossaries)
		{
			try
			{
				return JObject.Parse(serializedGlossaries)["glossaries"]?.ToObject<List<Glossary>>();
			}
			catch (Exception e)
			{
				Logger.Warn(e);
				MessageBox.Show($"Glossaries could not be retrieved: {e.Message}");
			}

			return null;
		}

		public async Task<List<Glossary>> GetGlossaries(string apiKey)
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("https://api.deepl.com/v2/glossaries"),
				Headers =
				{
					{ "Authorization", $"DeepL-Auth-Key {apiKey}" },
				},
			};
			using var response = await AppInitializer.Client.SendAsync(request);

			if (!response.IsSuccessStatusCode)
			{
				var messageBoxText = $"Glossaries retrieval failed: {response.ReasonPhrase}";

				Logger.Warn(messageBoxText);
				MessageBox.Show(messageBoxText, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

				return null;
			}
			var body = await response.Content.ReadAsStringAsync();

			return TryDeserializeGlossaries(body);
		}
	}
}