using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class SettingsModel
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;

		public SettingsModel()
		{
			_uriBuilder = new TranslationProviderUriBuilder(Constants.IATEGlossary);

			Domains = new List<DomainModel>();
			TermTypes = new List<TermTypeModel>();
			Collections = new List<CollectionModel>();
			Institutions = new List<InstitutionModel>();
		}

		public SettingsModel(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public List<CollectionModel> Collections
		{
			get
			{
				var collections = JsonConvert.DeserializeObject<List<CollectionModel>>(GetStringParameter("collections"));
				return collections;
			}
			set
			{
				var collections = JsonConvert.SerializeObject(value);
				SetStringParameter("collections", collections);
			}
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

		public List<InstitutionModel> Institutions
		{
			get
			{
				var institutions = JsonConvert.DeserializeObject<List<InstitutionModel>>(GetStringParameter("institutions"));
				return institutions;
			}
			set
			{
				var institutions = JsonConvert.SerializeObject(value);
				SetStringParameter("institutions", institutions);
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