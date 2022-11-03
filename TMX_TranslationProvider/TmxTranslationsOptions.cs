using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace TMX_TranslationProvider
{
	public class TmxTranslationsOptions
	{
		private readonly TranslationProviderUriBuilder _uriBuilder;
		//public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;
		public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.TranslationMemory;

		public TmxTranslationsOptions(Uri uri )
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}

		public TmxTranslationsOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder(new Uri($"{TmxTranslationProvider.ProviderScheme}://"));
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
