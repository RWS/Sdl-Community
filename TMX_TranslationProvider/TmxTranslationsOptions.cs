using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using TMX_Lib.Search;

namespace TMX_TranslationProvider
{
	public class TmxTranslationsOptions : ISearchServiceParameters
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
			FullFileName = ""; // just make sure file name is not null
			OptionsGuid = Guid.NewGuid().ToString();
		}
		public TmxTranslationsOptions(ISearchServiceParameters args) : this()
		{
			CopyFrom(args);
		}

		[JsonIgnore]
		public bool IgnoreTranslatedSegments => false;

		public TmxTranslationsOptions Clone() => new TmxTranslationsOptions(_uriBuilder.Uri);
		public override string ToString() => _uriBuilder.ToString();

		public Uri Uri() => _uriBuilder.Uri;

		public void CopyFrom(ISearchServiceParameters other)
		{
			FullFileName = other.FullFileName;
			DbConnectionNoPassword = other.DbConnectionNoPassword;
			DbName = other.DbName;
			QuickImport = other.QuickImport;
		}
		
		// the idea - each URI should be unique, or we'd get exceptions from Trados.
		// the easiest way to do this is to generate a unique GUID on construction, and never change it
		public string OptionsGuid
		{
			get => GetStringParameter("Guid");
			private set => SetStringParameter("Guid", value);
		}

		public string FriendlyName => DbName != "" ? DbName : Path.GetFileName( FullFileName);

		public string FullFileName
		{
			get => GetStringParameter("FileName");
			set => SetStringParameter("FileName", value);
		}

		// example:
		// mongodb+srv://jtorjo_rws:<password>@cluster0.mbqowvc.mongodb.net/?retryWrites=true&w=majority
		//
		// note: this is exactly what mongodb atlas gives us, so <password> will be exactly the "<password>" string
		// (which we'll replace with the real password)
		//
		// the password will be kept using Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore
		public string DbConnectionNoPassword
		{
			get => GetStringParameter("DbConnection");
			set => SetStringParameter("DbConnection", value);
		}

		// the database name - if empty, we'll use the filename, by default
		public string DbName
		{
			get => GetStringParameter("DbName"); 
			set => SetStringParameter("DbName", value);
		}

		public bool QuickImport
		{
			get => GetStringParameter("QuickImport") == "1";
			set => SetStringParameter("QuickImport", value ? "1" : "0");
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
