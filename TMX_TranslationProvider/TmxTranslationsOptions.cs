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

		public TmxTranslationsOptions(Uri uri )
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public TmxTranslationsOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(new Uri($"{TmxTranslationProvider.ProviderScheme}://"));
			FileName = ""; // just make sure file name is not null
		}

		[JsonIgnore]
		public bool IgnoreTranslatedSegments => false;

		public TmxTranslationsOptions Clone() => new TmxTranslationsOptions(_uriBuilder.Uri);
		public override string ToString() => _uriBuilder.ToString();

		public Uri Uri() => _uriBuilder.Uri;
		
		// the idea - each unique Tmx Provider is identified by a GUID -- you can edit its settings, and we'll reuse that provider
		// this way, if I edit a Tmx Provider, change the settings, and end up with a large file that gets imported into a database (which could take a long time),
		// I will preserve the same Tmx Provider between edits
		//
		// (by default, each new edit, if you change any setting, would create a new provider)
		public string Guid
		{
			get => GetStringParameter("Guid");
			set => SetStringParameter("Guid", value);
		}

		public string FriendlyName => DbName != "" ? DbName : Path.GetFileName( FileName);

		public string FileName
		{
			get => GetStringParameter("FileName");
			set => SetStringParameter("FileName", value);
		}

		// example:
		// mongodb+srv://jtorjo_rws:<password>@cluster0.mbqowvc.mongodb.net/?retryWrites=true&w=majority
		//
		// note: this is exactly what mongodb atlas gives us, so <password> will be exactly the "<password>" string
		// (which we'll replace with the real password)
		public string DbConnectionNoPassword
		{
			get => GetStringParameter("DbConnection");
			set => SetStringParameter("DbConnection", value);
		}

		// FIXME I should use Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProviderCredentialStore
		public string Password
		{
			get => GetStringParameter("Password");
			set => SetStringParameter("Password", value);
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
