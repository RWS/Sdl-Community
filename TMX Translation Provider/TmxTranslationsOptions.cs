using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TMX_Lib.Search;
using TMX_UI.ViewModel;

namespace TMX_TranslationProvider
{
	public class TmxTranslationsOptions 
	{
		protected bool Equals(TmxTranslationsOptions other)
		{
			return Equals(_uriBuilder, other._uriBuilder);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TmxTranslationsOptions)obj);
		}

		public override int GetHashCode()
		{
			return (_uriBuilder != null ? _uriBuilder.GetHashCode() : 0);
		}

		private readonly TranslationProviderUriBuilder _uriBuilder;
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.TranslationMemory;

		public TmxTranslationsOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
			if (OptionsGuid == "")
				OptionsGuid = Guid.NewGuid().ToString();
		}

		public TmxTranslationsOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(new Uri($"{TmxTranslationProvider.ProviderScheme}://"));
			OptionsGuid = Guid.NewGuid().ToString();
		}
		public TmxTranslationsOptions(TmxTranslationsOptions args) : this()
		{
			OptionsGuid = args.OptionsGuid;
			Databases = args.Databases.ToList();
		}

		[JsonIgnore]
		public bool IgnoreTranslatedSegments => false;

		public TmxTranslationsOptions Clone() => new TmxTranslationsOptions(_uriBuilder.Uri);
		public override string ToString() => _uriBuilder.ToString();

		public Uri Uri() => _uriBuilder.Uri;

		public string FriendlyName => string.Join(", ", Databases);

		public void CopyFrom(OptionsViewModel other)
		{
			Databases = other.Databases.Where(db => db.IsSelected).Select(db => db.Name).ToList();
			CareForLocale = other.CareForLocale;
		}
		
		// the idea - each URI should be unique, or we'd get exceptions from Trados.
		// the easiest way to do this is to generate a unique GUID on construction, and never change it
		public string OptionsGuid
		{
			get => GetStringParameter("Guid");
			private set => SetStringParameter("Guid", value);
		}

		public IReadOnlyList<string> Databases
		{
			get => GetStringParameter("Databases").Split(',');
			set => SetStringParameter("Databases", string.Join(",",value));
		}

		public bool CareForLocale {
			get => GetStringParameter("CareForLocale") == "1";
			set => SetStringParameter("CareForLocale", value ? "1" : "0");
		}

		private string GetStringParameter(string p)
		{
			var paramString = _uriBuilder[p] ?? "";
			return paramString;
		}
		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}
	}
}
