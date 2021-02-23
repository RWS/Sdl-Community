using System;
using System.Collections.Generic;
using IATETerminologyProvider.Helpers;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace IATETerminologyProvider.Model
{
	public class SettingsModel
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public SettingsModel()
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.IATEGlossary);
		}

		public SettingsModel(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public List<DomainModel> Domains
		{
			get
			{
				var domains = JsonConvert.DeserializeObject<List<DomainModel>>(GetStringParameter("domains"));
				return domains;
			}
			set
			{
				var domains = JsonConvert.SerializeObject(value);
				SetStringParameter("domains", domains);
			}
		}
		public List<TermTypeModel> TermTypes
		{
			get
			{
				var termTypes = JsonConvert.DeserializeObject<List<TermTypeModel>>(GetStringParameter("termTypes"));
				return termTypes;
			}
			set
			{
				var termTypes = JsonConvert.SerializeObject(value);
				SetStringParameter("termTypes", termTypes);
			}
		}

		public bool SearchInSubdomains
		{
			get => SearchInSubdomainsParameter != null && Convert.ToBoolean(SearchInSubdomainsParameter);
			set => SearchInSubdomainsParameter = value.ToString();
		}

		public string SearchInSubdomainsParameter
		{
			get => GetStringParameter("searchInSubdomains");
			set => SetStringParameter("searchInSubdomains", value);
		}
		public Uri Uri => _uriBuilder.Uri;

		private string GetStringParameter(string p)
		{
			var paramString = _uriBuilder[p];
			return paramString;
		}

		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}
	}
}
