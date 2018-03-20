using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;

namespace Sdl.LanguagePlatform.Lingua.Resources
{
	/// <summary>
	/// This class collects and caches language-specific resources, such as stop word lists, 
	/// prefix lists, and others and provides convenient access to the resources. All methods 
	/// are typically read-only. 
	/// </summary>
	public class LanguageResources
	{
		// TODO make thread-safe and allow multithreaded access w/ global cache?

		private System.Globalization.CultureInfo _Culture;
		private IResourceDataAccessor _Accessor;

		private Wordlist _Abbreviations;
		private ResourceStatus _AbbreviationsStatus;

		private Wordlist _Stopwords;
		private ResourceStatus _StopwordsStatus;

		private Lingua.Stemming.StemmingRuleSet _StemmingRules;
		private ResourceStatus _StemmingRulesStatus;

		// TODO consider IDisposable and keep refcount in RM?
		// TODO all methods must be thread-safe

		public LanguageResources(System.Globalization.CultureInfo culture)
			: this(culture, null)
		{
		}

		public LanguageResources(System.Globalization.CultureInfo culture, IResourceDataAccessor accessor)
		{
			if (culture == null)
				throw new ArgumentNullException("culture");
			if (accessor == null)
				accessor = Configuration.Load();

			_Culture = culture;
			_Accessor = accessor;

			// TODO if we have a DB-based resource accessor, we don't want individual roundtrips to the DB to 
			//  query the status for each resource. Rather the accessor should return all status in one call.
			// TODO this class intentionally implements a resource cache. We need however a protocol which
			//  detects changes to the resource storage and updates the cache.
			_AbbreviationsStatus = _Accessor.GetResourceStatus(culture, LanguageResourceType.Abbreviations, true);
			_StopwordsStatus = _Accessor.GetResourceStatus(culture, LanguageResourceType.Stopwords, true);
			_StemmingRulesStatus = _Accessor.GetResourceStatus(culture, LanguageResourceType.StemmingRules, true);
		}

		private Wordlist LoadWordlist(Core.Resources.LanguageResourceType t)
		{
			return ResourceStorage.LoadWordlist(_Accessor, Culture, t, true);
		}

		public bool IsAbbreviation(string s)
		{
			if (_Abbreviations == null)
			{
				if (_AbbreviationsStatus == ResourceStatus.NotAvailable)
					return false;

				_Abbreviations = LoadWordlist(LanguageResourceType.Abbreviations);
				if (_Abbreviations != null)
					_AbbreviationsStatus = ResourceStatus.Loaded;
			}

			// TODO case-insensitive lookup etc. 

			return _Abbreviations.Contains(s);
		}

		public bool IsStopword(string s)
		{
			if (_Stopwords == null)
			{
				if (_StopwordsStatus == ResourceStatus.NotAvailable)
					return false;

				_Stopwords = LoadWordlist(LanguageResourceType.Stopwords);
				if (_Stopwords != null)
					_StopwordsStatus = ResourceStatus.Loaded;
			}

			// TODO case-insensitive lookup etc. 

			return _Stopwords.Contains(s /*, SearchOption.CaseInsensitive */);
		}

		public Stemming.StemmingRuleSet StemmingRules
		{
			get
			{
				if (_StemmingRules != null)
					return _StemmingRules;

				if (_StemmingRulesStatus == ResourceStatus.NotAvailable)
					return null;

				// attempt loading
				using (System.IO.Stream s = _Accessor.ReadResourceData(_Culture, LanguageResourceType.StemmingRules, true))
				{
					Stemming.StemmingRuleSetReader rdr = new Sdl.LanguagePlatform.Lingua.Stemming.StemmingRuleSetReader(s);
					_StemmingRules = rdr.Read(_Culture);
					if (_StemmingRules != null)
						_StemmingRulesStatus = ResourceStatus.Loaded;
				}

				return _StemmingRules;
			}

			// for debugging only:
			set { _StemmingRules = value; }
		}

		public System.Globalization.CultureInfo Culture
		{
			get { return _Culture; }
		}

		public IResourceDataAccessor ResourceDataAccessor
		{
			get { return _Accessor; }
		}
	}
}
