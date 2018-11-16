using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class BeGlobalV4Translator
	{
		private readonly IRestClient _client;
		private readonly string _source;
		private readonly string _target;
		private readonly string _flavor;

		public BeGlobalV4Translator(
			string server,
			string user,
			string password,
			string source,
			string target,
			string flavor,
			bool useClientAuthentication)
		{
			_source = source;
			_target = target;
			_flavor = flavor;

			_client = new RestClient(string.Format($"{server}/v4"));

			// get a (time limited) token for authorisation
			// this can be a rather expensive call, and tokens remain valid for 24 hours, 
			// so buffering is recommended if you need to get repeated requests

			IRestRequest request;
			if (useClientAuthentication)
			{
				request = new RestRequest("/token", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				request.AddBody(new { clientId = user, clientSecret = password });
			}
			else
			{
				request = new RestRequest("/token/user", Method.POST)
				{
					RequestFormat = DataFormat.Json
				};
				request.AddBody(new { username = user, password = password });
			}
			request.RequestFormat = DataFormat.Json;
			var response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				throw new Exception("Acquiring token failed: " + response.Content);
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			_client.AddDefaultHeader("Authorization", $"Bearer {json.accessToken}");
		}

		public string TranslateText(string text)
		{
			var quick = text.Length < 5000; // quick (synchronous) translation only recommended for short plain text strings
			var json = UploadText(text, quick);
			if (quick)
			{
				return json.translation[0];
			}
			var rawData = WaitForTranslation(json.requestId.Value);
			json = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(rawData));
			return json.translation[0];
		}  

		public dynamic UploadText(string text, bool quick)
		{
			var request = new RestRequest("/mt/translations/" + (quick ? "sync" : "async"), Method.POST)
			{
				RequestFormat = DataFormat.Json
			};
			string[] texts = { text }; // could have multiple strings here, with a total max length of 5000 chars
			request.AddBody(new
			{
				input = texts,
				sourceLanguageId = _source,
				targetLanguageId = _target,
				model = _flavor,
				inputFormat = "HTML",
			});
			var response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
			{
				if (response.Content.Contains("does not exist"))
				{
					throw  new Exception("Language pair or engine model selected does not exist");
				}
			}  
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			return json;
		}

		private byte[] WaitForTranslation(string id)
		{
			IRestResponse response;
			string status;
			do
			{
				response = RestGet($"/mt/translations/async/{id}");
				if (response.StatusCode != System.Net.HttpStatusCode.OK)
					throw new Exception("Polling state failed: " + response.Content);

				dynamic json = JsonConvert.DeserializeObject(response.Content);
				status = json.translationStatus;

				if (!status.Equals("DONE", StringComparison.CurrentCultureIgnoreCase))
					System.Threading.Thread.Sleep(1000);
			}
			while (!status.Equals("DONE", StringComparison.CurrentCultureIgnoreCase)); // check for FAILED to catch errors

			response = RestGet($"/mt/translations/async/{id}/content");
			if (response.StatusCode != System.Net.HttpStatusCode.OK)
				throw new Exception("Downloading translation failed: " + response.Content);
			return response.RawBytes;
		}

		public void TranslateFile(string sourcepath, string targetpath)
		{
			var id = UploadFile(sourcepath);
			var rawTranslation = WaitForTranslation(id);
			using (var bw = new BinaryWriter(new FileStream(targetpath, FileMode.Create)))
			{
				bw.Write(rawTranslation);
			}
		}

		private string UploadFile(string path)
		{
			var request = new RestRequest("/mt/translations/async", Method.POST);
			// Because of the added file, RestSharp will create a Content-Type = multipart/form-data request for this
			request.AddFile("input", path, "application/octet-stream");
			request.AddParameter("sourceLanguageId", _source);
			request.AddParameter("targetLanguageId", _target);
			request.AddParameter("model", _flavor);
			request.AddParameter("inputFormat", "XML");

			var response = _client.Execute(request);
			if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
				throw new Exception("UploadFilefailed: " + response.Content);
			dynamic json = JsonConvert.DeserializeObject(response.Content);
			return json.requestId.Value;
		}

		private IRestResponse RestGet(string command)
		{
			var request = new RestRequest(command)
			{
				RequestFormat = DataFormat.Json
			};
			var response = _client.Execute(request);
			return response;
		}
	}
}
