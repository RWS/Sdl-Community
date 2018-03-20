using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Resources;

namespace Sdl.LanguagePlatform.Lingua.Resources
{
	/// <summary>
	/// An abstract base class which provides functionality to retrieve resources stored in 
	/// some key-value container. That container may be the file system, an assembly, a 
	/// database, or a resources file. The class standardizes the resource key and naming 
	/// conventions for the resources.
	/// </summary>
	public abstract class ResourceStorage : Core.Resources.IResourceDataAccessor
	{
		private string _KeyPrefix;
		private List<string> _ResourceNames;

		/// <summary>
		/// Initializes a new instance with default values.
		/// </summary>
		public ResourceStorage()
			: this(null)
		{
			// TODO could rather be "protected"
		}

		/// <summary>
		/// Initializes a new instance with the specified values.
		/// </summary>
		/// <param name="keyPrefix">An optional prefix which is prepended to each resource
		/// key prior to lookup</param>
		public ResourceStorage(string keyPrefix)
		{
			_KeyPrefix = keyPrefix;
		}

		/// <summary>
		/// Returns the keys of the stored resources as a list of strings.
		/// </summary>
		/// <returns>The list of resource keys known to the storage.</returns>
		public abstract List<string> GetAllResourceKeys();

		/// <summary>
		/// See <see cref="IResourceDataAccessor.ReadResourceData"/>
		/// </summary>
		public abstract System.IO.Stream ReadResourceData(System.Globalization.CultureInfo culture, 
			LanguageResourceType t, bool fallback);

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetResourceData"/>
		/// </summary>
		public abstract byte[] GetResourceData(System.Globalization.CultureInfo culture, LanguageResourceType t, bool fallback);

		/// <summary>
		/// Gets the standardized resource key name for the specified language resource type, 
		/// without an added prefix.
		/// </summary>
		/// <param name="t">The language resource type</param>
		/// <returns>The standardized resource key name for the specified language resource type</returns>
		public static string GetResourceTypeName(LanguageResourceType t)
		{
			switch (t)
			{
			case LanguageResourceType.Variables:
				return "Variables.txt";
			case LanguageResourceType.Abbreviations:
				return "Abbreviations.txt";
			case LanguageResourceType.OrdinalFollowers:
				return "OrdinalFollowers.txt";
			case LanguageResourceType.SegmentationRules:
				return "SegmentationRules.xml";
			case LanguageResourceType.TokenizerSettings:
				return "TokenizerSettings.xml";
			case LanguageResourceType.StemmingRules:
				return "StemmingRules.txt";
			case LanguageResourceType.Stopwords:
				return "Stopwords.txt";
			case LanguageResourceType.DatePatterns:
				return "DatePatterns.xml";
			case LanguageResourceType.TimePatterns:
				return "TimePatterns.xml";
			case LanguageResourceType.NumberPatterns:
				return "NumberPatterns.xml";
			case LanguageResourceType.MeasurementPatterns:
				return "MeasurementPatterns.xml";
			case LanguageResourceType.CharTrigramVector:
				return "CharTrigrams.dat";
			case LanguageResourceType.ShortDateFST:
				return "ShortDate.fst";
			case LanguageResourceType.LongDateFST:
				return "LongDate.fst";
			case LanguageResourceType.ShortTimeFST:
				return "ShortTime.fst";
			case LanguageResourceType.LongTimeFST:
				return "LongTime.fst";
			case LanguageResourceType.CurrencySymbols:
				return "CurrencySymbols.txt";
			case LanguageResourceType.PhysicalUnits:
				return "PhysicalUnits.txt";
			case LanguageResourceType.NumberFST:
				return "Number.fst";
			case LanguageResourceType.MeasurementFST:
				return "Measurement.fst";
			case LanguageResourceType.GenericRecognizers:
				return "GenericRecognizers.xml";
			case LanguageResourceType.Undefined:
			default:
				throw new ArgumentException("Illegal enum");
			}
		}

		/// <summary>
		/// Returns the resource data for the required resource as a word list. If the stored 
		/// resource is not a word list, an exception will be thrown.
		/// </summary>
		/// <param name="accessor">The resource data accessor to use to retrieve the data</param>
		/// <param name="culture">The culture to obtain the resource for</param>
		/// <param name="t">The type of the language resource to retrieve</param>
		/// <param name="fallback">Whether or not to use a fallback stragegy if specific
		/// resources cannot be found (see <see cref="IResourceDataAccessor.ReadResourceData"/>)</param>
		/// <returns>The requested data as a word list</returns>
		public static Core.Wordlist LoadWordlist(IResourceDataAccessor accessor, 
			System.Globalization.CultureInfo culture, 
			LanguageResourceType t, 
			bool fallback)
		{
			// TODO check resource type and load only compatible data

			using (System.IO.Stream s = accessor.ReadResourceData(culture, t, fallback))
			{
				if (s == null)
					return null;

				Core.Wordlist l = new Core.Wordlist();
				l.Load(s);

				return l;
			}
		}

		/// <summary>
		/// Gets the configured key prefix, which may be null.
		/// </summary>
		public string KeyPrefix
		{
			get { return _KeyPrefix; }
		}

		/// <summary>
		/// Computes the full resource key name, which is a combination of the optional <see cref="KeyPrefix"/>,
		/// the <paramref name="culturePrefix"/>, and the standardized resource key name (see <see cref="GetResourceTypeName"/>).
		/// </summary>
		/// <param name="culturePrefix">An optional prefix which usually is the culture code (e.g. "en-US")</param>
		/// <param name="t">The language resource type for which to get the resource key</param>
		/// <returns>The full language resource key</returns>
		public string GetName(string culturePrefix, LanguageResourceType t)
		{
			System.Text.StringBuilder sb = new StringBuilder();
			if (!String.IsNullOrEmpty(_KeyPrefix))
			{
				sb.Append(_KeyPrefix);
				sb.Append(".");
			}

			if (!String.IsNullOrEmpty(culturePrefix))
			{
				sb.Append(culturePrefix);
				sb.Append("_");
			}

			sb.Append(GetResourceTypeName(t));

			return sb.ToString();
		}

		/// <summary>
		/// Computes the full resource key name, which is a combination of the optional <see cref="KeyPrefix"/>,
		/// the name of the provided <paramref name="culture"/>, and the standardized resource key name (see <see cref="GetResourceTypeName"/>).
		/// </summary>
		/// <param name="culture">An optional culture. If supplied, the culture's name will be used to compute the resource key.</param>
		/// <param name="t">The language resource type for which to get the resource key</param>
		/// <returns>The full language resource key</returns>
		public string GetName(System.Globalization.CultureInfo culture, LanguageResourceType t)
		{
			if (culture == null || System.Globalization.CultureInfo.InvariantCulture.LCID == culture.LCID)
				return GetName("", t);
			else
				return GetName(culture.Name, t);
		}

		/// <summary>
		/// Gets the resource key name, given a culture and the language resource type. Optionally 
		/// the following search strategy is applied if resources for the exact culture cannot be found:
		/// <list type="ordered">
		/// <item>First, it is checked whether data for the exact culture is available.</item>
		/// <item>If not, data is continually requested for the culture's parent culture, 
		/// until the parent culture is the invariant culture, or undefined.</item>
		/// <item>Data is then requested for the culture's language group.</item>
		/// <item>Finally, data is requested for the invariant culture (<see cref="System.Globalization.CultureInfo.InvariantCulture"/>)</item>
		/// </list>
		/// The first resource key for which data is available is then returned, or <c>null</c> if no data can be found.
		/// </summary>
		/// <param name="culture">The culture for which to retrieve resource data</param>
		/// <param name="t">The type of the language resource to retrieve</param>
		/// <param name="fallback">If <c>true</c>, a fallback search strategy is applied to find
		/// resources for compatible, but less specific resource data.</param>
		/// <returns>The full resource key for the language resource, or <c>null</c> if 
		/// no data is available.</returns>
		public string GetResourceName(System.Globalization.CultureInfo culture, 
			LanguageResourceType t, bool fallback)
		{
			EnsureResourceNames();

			string fullName = GetName(culture, t);
			string found = Find(fullName);

			if (!fallback)
				return found; // may be null

			// language fallback
			// NOTE that the invariant's culture's parent is the invariant culture (not a null culture). We therefore
			//  test on LCID equality
			bool triedLanguageFamily = false;

			while (found == null && culture.Parent != null && culture.Parent.LCID != culture.LCID)
			{
				if (culture.Parent.LCID == System.Globalization.CultureInfo.InvariantCulture.LCID
					&& !triedLanguageFamily)
				{
					string languageGroupName = CultureInfoExtensions.GetLanguageGroupName(culture);
					if (languageGroupName != null)
					{
						fullName = GetName(languageGroupName, t);
						found = Find(fullName);
						// culture remains the same (will fall back on next iter)
					}
					triedLanguageFamily = true;
				}
				else
				{
					culture = culture.Parent;
					fullName = GetName(culture, t);
					found = Find(fullName);
				}
			}

			return found;
		}

		private void EnsureResourceNames()
		{
			if (_ResourceNames == null)
			{
				_ResourceNames = GetAllResourceKeys();
				if (_ResourceNames == null)
					// make dummy so that callers don't have to check on null
					_ResourceNames = new List<string>();
			}
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetSupportedCultures"/>
		/// </summary>
		public List<System.Globalization.CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			EnsureResourceNames();

			List<System.Globalization.CultureInfo> result = new List<System.Globalization.CultureInfo>();

			string typeSuffix = GetResourceTypeName(t);

			foreach (string fullrn in _ResourceNames)
			{
				if (_KeyPrefix == null || fullrn.StartsWith(_KeyPrefix, StringComparison.OrdinalIgnoreCase))
				{
					string rn = _KeyPrefix == null ? fullrn : fullrn.Substring(_KeyPrefix.Length + 1);

					if (rn.EndsWith(typeSuffix, StringComparison.OrdinalIgnoreCase))
					{
						try
						{
							string cn = rn.Substring(0, rn.Length - typeSuffix.Length - 1);
							result.Add(Core.CultureInfoExtensions.GetCultureInfo(cn));
						}
						catch
						{
						}
					}
				}
			}

			return result.Count > 0 ? result : null;
		}

		private string Find(string fullName)
		{
			// NOTE lookup/comparison is done case-invariant. However, reading an assembly's resource stream
			//  is case-sensitive. We therefore return the original assembly's resource name here, as collected
			//  in the constructor.

			return _ResourceNames.Find(s => s.Equals(fullName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetResourceStatus"/>
		/// </summary>
		public ResourceStatus GetResourceStatus(System.Globalization.CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			string fullName = GetResourceName(culture, t, fallback);
			return (fullName == null) ? ResourceStatus.NotAvailable : ResourceStatus.Loadable;
		}

		internal static byte[] StreamToByteArray(System.IO.Stream stream)
		{
			if (stream == null)
				return null;

			using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
			{
				const int bufSize = 4096;
				byte[] buffer = new byte[bufSize];
				int read;
				do
				{
					read = stream.Read(buffer, 0, bufSize);
					if (read > 0)
						mem.Write(buffer, 0, read);
				} while (read == bufSize);

				return mem.ToArray();
			}
		}

	}
}
