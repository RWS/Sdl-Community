using System;
using System.Collections.Generic;
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

		public string Username
		{
			get => GetStringParameter("Username");
			set => SetStringParameter("Username", value);
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
