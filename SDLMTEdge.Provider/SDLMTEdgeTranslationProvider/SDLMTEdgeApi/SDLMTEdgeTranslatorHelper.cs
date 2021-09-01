using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.MTEdge.LPConverter;
using Sdl.Community.MTEdge.Provider.Helpers;
using Sdl.Community.MTEdge.Provider.Model;
using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.MTEdge.Provider.SDLMTEdgeApi
{
	public enum APIVersion { Unknown, v1, v2 };

	public static class SDLMTEdgeTranslatorHelper
	{
		private static Func<Uri, HttpClient, HttpResponseMessage> MtEdgePost = (uri, client) =>
			client.PostAsync(uri, content: null).Result;
		private static Func<Uri, HttpClient, HttpResponseMessage> MtEdgeGet = (uri, client) =>
			client.GetAsync(uri).Result;
		private static SDLMTEdgeLanguagePair[] _languagePairsOnServer;
		private static object languageLock = new object();
		private static object optionsLock = new object();

		private enum ErrorHResult
		{
			HandshakeFailure = -2146232800,
			ServerInaccessible = -2147467259,
			RequestTimeout = -2146233029,
		}

		public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Get the translation of an xliff file using the MTEdge API.
		/// </summary>
		public static string GetTranslation(TranslationOptions options,
			LanguagePair languageDirection,
			Xliff xliffFile)
		{
			var text = xliffFile.ToString();
			var queryString = new Dictionary<string, string>();
			var encodedInput = text.Base64Encode();

			lock (optionsLock)
			{
				if (options.ApiVersion == APIVersion.Unknown)
				{
					SetMtEdgeApiVersion(options);
				}
			}

			if (options.ApiVersion == APIVersion.v1)
			{
				queryString["sourceLanguageId"] = languageDirection.SourceCulture.ToMTEdgeCode();
				queryString["targetLanguageId"] = languageDirection.TargetCulture.ToMTEdgeCode();
				queryString["text"] = encodedInput;
			}
			else
			{
				// If LPPreferences doesn't contain the target language (source is always the same), figure out the
				// preferred LP. Previously set preferred LPs will stay, so this won't get run each time if you have
				// multiple LPs.
				if (!options.LPPreferences.ContainsKey(languageDirection.TargetCulture))
				{
					options.SetPreferredLanguages(new[] { languageDirection });
					if (!options.LPPreferences.ContainsKey(languageDirection.TargetCulture))
					{
						throw new Exception("There are no language pairs currently accessible via MtEdge.");
					}
				}
				queryString["languagePairId"] = options.LPPreferences[languageDirection.TargetCulture].LanguagePairId;
				if (options.LPPreferences[languageDirection.TargetCulture].DictionaryId != null)
				{
					if (!options.LPPreferences[languageDirection.TargetCulture].DictionaryId.Equals(Constants.NoDictionary))
					{
						queryString["dictionaryIds"] = options.LPPreferences[languageDirection.TargetCulture].DictionaryId;
					}
				}

				queryString["input"] = encodedInput;
			}
			queryString["inputFormat"] = "application/x-xliff";

			string jsonResult;
			try
			{
				jsonResult = Translate(queryString, options, "translations/quick");
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.Translation}: {e.Message}\n {e.StackTrace}\n Encoded Input: {encodedInput}");
				throw;
			}

			var encodedTranslation = JsonConvert.DeserializeObject<SDLMTEdgeTranslationOutput>(jsonResult).Translation;
			var decodedTranslation = encodedTranslation.Base64Decode();
			_logger.Debug("Resultant translation is: {0}", encodedTranslation);
			return decodedTranslation;
		}

		private static string Translate(Dictionary<string, string> parameters, TranslationOptions options, string path)
		{
			var content = GetStringContentFromParameters(parameters);

			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = options.UseBasicAuthentication
					? new AuthenticationHeaderValue("Bearer", options.ApiToken)
					: new AuthenticationHeaderValue("Basic", (options.ApiToken + ":").Base64Encode());

				var builder = new UriBuilder(options.Uri)
				{
					Path = $"/api/{options.ApiVersionString}/{path}",
					Scheme = options.RequiresSecureProtocol ? Uri.UriSchemeHttps : Uri.UriSchemeHttp
				};
				var response = httpClient.PostAsync(builder.Uri, content).Result;
				var result = response.Content.ReadAsStringAsync().Result;

				return result;
			}
		}

		private static StringContent GetStringContentFromParameters(IDictionary<string, string> parametersDictionary)
		{
			var limit = 32000;
			var content = new StringContent(parametersDictionary.Aggregate(new StringBuilder(), (sb, nxt) =>
			{
				var sbInternal = new StringBuilder();
				if (sb.Length > 0)
					sb.Append("&");

				var loops = nxt.Value.Length / limit;
				for (var i = 0; i <= loops; i++)
				{
					if (i < loops)
						sbInternal.Append(Uri.EscapeDataString(nxt.Value.Substring(limit * i, limit)));
					else
						sbInternal.Append(Uri.EscapeDataString(nxt.Value.Substring(limit * i)));
				}

				return sb.Append(nxt.Key + "=" + sbInternal.ToString());
			}).ToString(), Encoding.UTF8, "application/x-www-form-urlencoded");
			return content;
		}

		/// <summary>
		/// Get an array of Language Pairs accessible by the credentials provided.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static SDLMTEdgeLanguagePair[] GetLanguagePairs(TranslationOptions options)
		{
			lock (languageLock)
			{
				if (_languagePairsOnServer == null || !_languagePairsOnServer.Any())
				{
					try
					{
						// Ideally no exception should be thrown from ContactMtEdgeServer, but in rare cases
						// it could successfully authenticate using username/password, but then reject the token. If
						// that happens, open a message with the related error
						var jsonResult = ContactMtEdgeServer(MtEdgeGet, options, "language-pairs");
						var languagePairs = JsonConvert.DeserializeObject<LanguagePairResult>(jsonResult).LanguagePairs;
						_languagePairsOnServer = (languagePairs != null ? languagePairs : new SDLMTEdgeLanguagePair[0]);

						// In 60 seconds, wipe the LPs so we query again. That way, if someone makes a change, we'll
						// pick it up eventually.
						Task.Factory.StartNew(() =>
						{
							System.Threading.Thread.Sleep(60000);
							lock (languageLock)
							{
								ExpireLanguagePairs();
							}
						});
					}
					catch (Exception e)
					{
						_logger.Error($"{Constants.LanguagePairs}: {Constants.InaccessibleLangPairs}:  {e.Message}\n {e.StackTrace}");

						if (Environment.UserInteractive)
						{
							MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						}
						_languagePairsOnServer = new SDLMTEdgeLanguagePair[0];
					}
				}
			}
			return _languagePairsOnServer;
		}

		/// <summary>
		/// Get dictionaries from the MT Edge server 
		/// </summary>
		/// <param name="tradosToMtEdgeLP"></param>
		/// <param name="options"></param>
		public static void GetDictionaries(TradosToMTEdgeLP tradosToMtEdgeLP, TranslationOptions options)
		{
			var queryString = HttpUtility.ParseQueryString(string.Empty);
			foreach (var item in tradosToMtEdgeLP.MtEdgeLPs)
			{
				queryString["sourceLanguageId"] = item.SourceLanguageId;
				queryString["targetLanguageId"] = item.TargetLanguageId;
				queryString["perPage"] = "1000"; // set to 1000 to avoid the missing dictionaries
				var jsonResult = ContactMtEdgeServer(MtEdgeGet, options, "dictionaries", queryString);

				var result = JsonConvert.DeserializeObject<DictionaryInfo>(jsonResult);
				tradosToMtEdgeLP.Dictionaries = new List<DictionaryModel>(result.Dictionaries);
				tradosToMtEdgeLP.Dictionaries.Insert(0, new DictionaryModel
				{
					DictionaryId = Constants.NoDictionary,
					SourceLanguageId = string.Empty,
					TargetLanguageId = string.Empty
				});
			}
		}

		public static void ExpireLanguagePairs()
		{
			_languagePairsOnServer = new SDLMTEdgeLanguagePair[0];
		}

		/// <summary>
		/// Queries the MTEdge server specified in options.
		/// </summary>
		/// <param name="mtEdgeHttpMethod">MtEdgePost or MtEdgeSGet</param>
		/// <param name="options">The translation options</param>
		/// <param name="path">The path after /api/vX/ for MTEdge api queries</param>
		/// <param name="parameters">data to include in the query</param>
		/// <param name="useHTTP">Whether to use HTTP over HTTPS</param>
		/// <param name="timesToRetry">number of times to retry the query</param>
		/// <returns>The string result of the request.</returns>
		private static string ContactMtEdgeServer(Func<Uri, HttpClient, HttpResponseMessage> mtEdgeHttpMethod,
			TranslationOptions options,
			string path,
			NameValueCollection parameters = null,
			bool useHTTP = false)
		{
			lock (optionsLock)
			{
				if (options.ApiVersion == APIVersion.Unknown)
				{
					SetMtEdgeApiVersion(options);
				}
			}

			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;
			ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Authorization = options.UseBasicAuthentication
					? new AuthenticationHeaderValue("Bearer", options.ApiToken)
					: new AuthenticationHeaderValue("Basic", (options.ApiToken + ":").Base64Encode());

				var builder = new UriBuilder(options.Uri)
				{
					Path = $"/api/{options.ApiVersionString}/{path}",
					Scheme = options.RequiresSecureProtocol ? Uri.UriSchemeHttps : Uri.UriSchemeHttp
				};

				if (parameters != null)
				{
					builder.Query = parameters.ToString();
				}
				HttpResponseMessage httpResponse;

				try
				{
					httpResponse = mtEdgeHttpMethod(builder.Uri, httpClient);
				}
				catch (Exception e)
				{
					while (e.InnerException != null)
					{
						e = e.InnerException;
					}

					_logger.Error($"{Constants.MTEdgeServerContact}:\n {Constants.MtEdgeServerContactExResult} {e.HResult}\n {e.Message}\n {e.StackTrace}");
					throw TranslateAggregateException(e);
				}

				if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
				{
					return httpResponse.Content.ReadAsStringAsync().Result;
				}
				switch (httpResponse.StatusCode)
				{
					case HttpStatusCode.Unauthorized:
						_logger.Error($"{Constants.MTEdgeServerContact}: {Constants.InvalidCredentials}");
						throw new UnauthorizedAccessException("The credentials provided are not authorized.");
					case HttpStatusCode.BadRequest:
						_logger.Error($"{Constants.MTEdgeServerContact}: {Constants.BadRequest} {0}", httpResponse.Content.ReadAsStringAsync().Result);
						throw new Exception($"There was a problem with the request: { httpResponse.Content.ReadAsStringAsync().Result }");
					default:
						_logger.Error($"{Constants.MTEdgeServerContact}: {(int)httpResponse.StatusCode} {Constants.StatusCode}");

						return null;
				}
			}
		}

		public static void SetMtEdgeApiVersion(TranslationOptions options)
		{
			try
			{
				options.ApiVersion = APIVersion.v2;
				var systemInfo = ContactMtEdgeServer(MtEdgeGet, options, "system/info");
				if (systemInfo == null)
				{
					options.ApiVersion = APIVersion.v1;
				}
			}
			catch (Exception e)
			{
				_logger.Error($"{Constants.MTEdgeApiVersion}: {e.Message}\n {e.StackTrace}");
				options.ApiVersion = APIVersion.v1;
				throw;
			}
		}

		/// <summary>
		/// Verifies that the API Key passed by the user is a valid API key.
		/// </summary>
		public static void VerifyBasicAPIToken(TranslationOptions options, GenericCredentials credentials)
		{
			if (options == null)
			{
				throw new ArgumentNullException("Options is null");
			}
			var oldApiKey = options.ApiToken;
			options.ApiToken = credentials["API-Key"];
			options.UseBasicAuthentication = credentials["UseApiKey"] != "true";
			options.RequiresSecureProtocol = credentials["RequiresSecureProtocol"] == "true";

			try
			{
				// Make a request to the API using whatever path desired.
				ContactMtEdgeServer(MtEdgeGet, options, "language-pairs");
			}
			catch (AggregateException e)
			{
				_logger.Error($"{Constants.VerifyBasicAPIToken}: {e.Message}\n {e.StackTrace}");
				throw TranslateAggregateException(e);
			}
			catch (SocketException e)
			{
				_logger.Error($"{Constants.VerifyBasicAPIToken}: {e.Message}\n {e.StackTrace}");
				throw TranslateAggregateException(e);
			}
			finally
			{
				options.ApiToken = oldApiKey;
			}
		}

		/// <summary>
		/// Using the username and password passed in via credentials, obtain the authentication token that will be
		/// later used to validate API calls.
		/// </summary>
		public static string GetAuthToken(
			TranslationOptions options,
			GenericCredentials credentials,
			bool useHTTP = false)
		{
			lock (optionsLock)
			{
				if (options.ApiVersion == APIVersion.Unknown)
				{
					SetMtEdgeApiVersion(options);
				}
			}

			ServicePointManager.Expect100Continue = true;
			ServicePointManager.DefaultConnectionLimit = 9999;

			using (var httpClient = new HttpClient())
			{
				// Build the URI for querying the token
				var builder = new UriBuilder(options.Uri)
				{
					Path = $"/api/{options.ApiVersionString}/auth"
				};

				// Pass in the username and password as parameters to retrieve the auth token
				var queryString = HttpUtility.ParseQueryString(string.Empty);
				queryString["username"] = credentials.UserName;
				queryString["password"] = credentials.Password;
				builder.Query = queryString.ToString();
				builder.Scheme = useHTTP ? Uri.UriSchemeHttp : Uri.UriSchemeHttps;

				// Users may be hosting the service locally and therefore not sign their certificates. If so,
				// we'll want to accept all certificates. Otherwise, this would throw an exception.
				ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

				try
				{
					var httpResponse = httpClient.PostAsync(builder.Uri, null).Result;
					if (httpResponse.Content != null && httpResponse.StatusCode == HttpStatusCode.OK)
					{
						return httpResponse.Content.ReadAsStringAsync().Result;
					}
					else if (httpResponse.StatusCode == HttpStatusCode.Unauthorized || httpResponse.StatusCode == HttpStatusCode.BadRequest)
					{
						throw new UnauthorizedAccessException("The credentials provided are not authorized.");
					}
					throw new Exception("No response from the URI provided");
				}
				catch (Exception e)
				{
					while (e.InnerException != null)
					{
						e = e.InnerException;
					}

					if (!useHTTP && e.HResult == (int)ErrorHResult.HandshakeFailure)
					{
						return GetAuthToken(options, credentials, true);
					}
					_logger.Error($"{Constants.AuthToken}: {e.Message}\n {e.StackTrace}");
					throw TranslateAggregateException(e);
				}
			}
		}

		/// <summary>
		/// Translate exceptions thrown from the http requests into exceptions with client-friendly messages.
		/// </summary>
		/// <param name="culprit"></param>
		/// <returns></returns>
		private static Exception TranslateAggregateException(Exception culprit)
		{
			while (culprit.InnerException != null)
			{
				culprit = culprit.InnerException;
			}

			if (culprit.HResult == (int)ErrorHResult.ServerInaccessible)
			{
				return new WebException("Error with the server information. A connection cannot be formed. Please ensure the server information is correct.");
			}
			// TODO: Cannot replicate this. If possible, convert this to an HResult enum
			if (culprit.Message.Contains("the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond"))
			{
				return new WebException("The host was unable to be reached within an acceptable amount of time. Please ensure you are able to connect to the server from this computer.");
			}
			if (culprit.HResult == (int)ErrorHResult.RequestTimeout)
			{
				return new WebException("The request has been cancelled, either due to timeout or being interrupted externally.");
			}
			_logger.Error($"{Constants.TranslateAggregateException}: {culprit}");
			return culprit;
		}

		/// <summary>
		/// Encode a string using base64 encoding.
		/// </summary>
		private static string Base64Encode(this string text)
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
		}

		/// <summary>
		/// Decode a base64 encoded string.
		/// </summary>
		private static string Base64Decode(this string encodedText)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
		}
	}
}