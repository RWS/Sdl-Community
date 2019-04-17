using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using ETSLPConverter;
using ETSTranslationProvider.ETSApi;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace ETSTranslationProvider
{
	public class TranslationOptions
	{
		private string ResolvedHost { get; set; }

		private string ResolveHost()
		{
			if (ResolvedHost != null)
			{
				return ResolvedHost;
			}
			// If the host is an IP address, preserve that, otherwise get the DNS host and cache it.
			ResolvedHost = IPAddress.TryParse(Host, out var address) ? Host : Dns.GetHostEntry(Host).HostName;
			return ResolvedHost;
		}

		readonly TranslationProviderUriBuilder _uriBuilder;

		public TranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(TranslationProvider.TranslationProviderScheme);
			UseBasicAuthentication = true;
			Port = 8001;
			LPPreferences = new Dictionary<CultureInfo, ETSLanguagePair>();
		}

		public TranslationOptions(Uri uri) : this()
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

		public Dictionary<CultureInfo, ETSLanguagePair> LPPreferences { get; }

		public bool PersistCredentials { get; set; }

		[JsonIgnore]
		public string ApiToken { get; set; }

		public bool UseBasicAuthentication { get; set; }

		#region URI Properties
		public string Host
		{
			get => _uriBuilder.HostName;
			set
			{
				_uriBuilder.HostName = value;
				ResolvedHost = null;
			}
		}

		public int Port
		{
			get => _uriBuilder.Port;
			set => _uriBuilder.Port = value;
		}

		public APIVersion ApiVersion { get; set; }

		public string ApiVersionString => ApiVersion == APIVersion.v1 ? "v1" : "v2";

		public Uri Uri
		{
			get
			{
				var resolvedUri = new UriBuilder(_uriBuilder.Uri)
				{
					Host = ResolveHost()
				};
				return resolvedUri.Uri;
			}
		}

		public TradosToETSLP[] SetPreferredLanguages(LanguagePair[] languagePairs)
		{
			var etsLanguagePairs = ETSTranslatorHelper.GetLanguagePairs(this);
			if (!etsLanguagePairs.Any())
			{
				return null;
			}
			var languagePairChoices = languagePairs.GroupJoin(
				etsLanguagePairs,
				requestedLP =>
					new
					{
						SourceLanguageId = requestedLP.SourceCulture.ToETSCode(),
						TargetLanguageId = requestedLP.TargetCulture.ToETSCode()
					},
				installedLP =>
					new
					{
						SourceLanguageId = installedLP.SourceLanguageId,
						TargetLanguageId = installedLP.TargetLanguageId
					},
				(requestedLP, installedLP) =>
					new ETSApi.TradosToETSLP(
						tradosCulture: requestedLP.TargetCulture,
						etsLPs: installedLP.OrderBy(lp => lp.LanguagePairId).ToList())
			).ToList();

			// Fix for French Canada engine	 which has language code on server frc
			var frenchCanadianLp = languagePairs.FirstOrDefault(lp => lp.SourceCulture.ThreeLetterWindowsLanguageName.Equals("FRC") ||
																	  lp.TargetCulture.ThreeLetterWindowsLanguageName.Equals("FRC"));
			if (frenchCanadianLp != null)
			{
				var etsLangPairEngines = etsLanguagePairs.Where(lp => lp.SourceLanguageId.Equals("frc") ||
																		lp.TargetLanguageId.Equals("frc")).ToList();
				if (etsLangPairEngines.Any())
				{
					var projectSourceLanguage = languagePairChoices.FirstOrDefault(s => s.TradosCulture.ThreeLetterISOLanguageName.Equals("fra"));
					foreach (var etsEngine in etsLangPairEngines)
					{
						projectSourceLanguage?.ETSLPs.Add(etsEngine);
					}
				}
			}
			RemoveLPChoices(languagePairChoices);

			return languagePairChoices.ToArray();
		}

		/// <summary>
		/// Empty out the previous LP choices.
		/// </summary>
		/// <param name="languagePairChoices">languagePairChoices</param>
		private void RemoveLPChoices(List<TradosToETSLP> languagePairChoices)
		{
			foreach (var lpChoice in languagePairChoices)
			{
				// By default, select the preferences to be the first LP of each set.
				var defaultOption = lpChoice.ETSLPs.FirstOrDefault();

				// Verify that the preferred LP still exists on ets server and if not, remove it from preferences
				if (LPPreferences.ContainsKey(lpChoice.TradosCulture) && !lpChoice.ETSLPs.Contains(LPPreferences[lpChoice.TradosCulture]))
				{
					LPPreferences.Remove(lpChoice.TradosCulture);
				}
				if (defaultOption != null && !LPPreferences.ContainsKey(lpChoice.TradosCulture))
				{
					LPPreferences[lpChoice.TradosCulture] = defaultOption;
				}
			}
		}
		#endregion
	}
}