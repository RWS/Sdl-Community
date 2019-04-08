using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class WordlistResource : IResource
	{
		private readonly XmlNode _resource;
		private readonly string _propertyName;

		public WordlistResource(XmlNode resource, string propertyName)
		{
			_resource = resource;
			_propertyName = propertyName;
		}

		public void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle)
		{
			var vars = Encoding.UTF8.GetString(Convert.FromBase64String(_resource.InnerText));

			var langResBundleSetter = typeof(LanguageResourceBundle).GetProperty(_propertyName)?.SetMethod;
			langResBundleSetter?.Invoke(langResBundle, new[] {new Wordlist()});

			var langResBundleGetter = (typeof(LanguageResourceBundle).GetProperty(_propertyName)?.GetMethod.Invoke(langResBundle, null) as Wordlist);

			foreach (Match s in Regex.Matches(vars, @"([^\s]+)"))
			{
				langResBundleGetter?.Add(s.ToString());
			}
		}
	}
}
