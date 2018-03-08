using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Lingua.Resources;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	/// <summary>
	/// Encapsulates the settings which control the behavior and the output of a <see cref="Tokenizer"/>.
	/// </summary>
	public class TokenizerParameters
	{
		private List<Recognizer> _Recognizers;
		/// <summary>
		/// If true, acronyms will be reclassified to words if the segment is "all caps", i.e. 
		/// the number of uppercase chars is at least twice the number of lowercase or nocase chars.
		/// </summary>
		private bool _ReclassifyAcronyms;

		private bool _BreakOnWhitespace;
		private bool _CreateWhitespaceTokens;
		private System.Globalization.CultureInfo _Culture;

		internal static readonly bool UseRXNumberRecognizer = false;
		internal static readonly bool UseRXMeasurementRecognizer = false;

		/// <summary>
		/// Initialize the tokenizer parameters from the tokenizer setup information. 
		/// The resource data accessor is only used for retrieving variable values.
		/// </summary>
		/// <param name="setup">The tokenizer setup to use</param>
		/// <param name="accessor">A resource data accessor, to initialize the variables list</param>
		public TokenizerParameters(TokenizerSetup setup, Core.Resources.IResourceDataAccessor accessor)
		{
			if (setup == null)
				throw new ArgumentNullException("setup");

			if (accessor == null)
				accessor = new ResourceFileResourceAccessor();

			_BreakOnWhitespace = setup.BreakOnWhitespace;
			_CreateWhitespaceTokens = setup.CreateWhitespaceTokens;
			_Culture = Core.CultureInfoExtensions.GetCultureInfo(setup.CultureName);

			_Recognizers = new List<Recognizer>();
			_ReclassifyAcronyms = false;

			// we need to determine a region-qualified culture since neutral cultures 
			//  don't have date/time/number pattern info
			System.Globalization.CultureInfo actualCulture = _Culture;
			if (_Culture.IsNeutralCulture)
				actualCulture = Core.CultureInfoExtensions.GetRegionQualifiedCulture(_Culture);

			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeDates) != 0)
			{
				AddRecognizer(DateTimeRecognizer.Create(accessor, actualCulture,
					DateTimePatternType.ShortDate | DateTimePatternType.LongDate, 100));
			}

			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeTimes) != 0)
			{
				AddRecognizer(DateTimeRecognizer.Create(accessor, actualCulture,
					DateTimePatternType.ShortTime | DateTimePatternType.LongTime, 100));
			}

			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeNumbers) != 0)
			{
				if (UseRXNumberRecognizer)
					AddRecognizer(NumberRegexRecognizer.Create(actualCulture, 100));
				else
					AddRecognizer(NumberFSTRecognizer.Create(accessor, actualCulture, 100));

				// it does not make sense to recognize ordinal numbers if they don't become placeables and 
				//  don't participate in auto-localization. They'd also need to be auto-localized (3. -> 3rd)
				bool createOrdinalNumberRecognizer = false;
				if (createOrdinalNumberRecognizer)
				{
					// add special recognizer for ordinal numbers if ordinal followers are available
					// [0-9]+\. (?=[OrdinalFollowers])
					if (accessor.GetResourceStatus(_Culture, Core.Resources.LanguageResourceType.OrdinalFollowers, true) != Core.Resources.ResourceStatus.NotAvailable)
					{
						Wordlist ordinalFollowers = new Wordlist();
						CharacterSet dummy;
						ordinalFollowers.Load(accessor.ReadResourceData(_Culture, Core.Resources.LanguageResourceType.OrdinalFollowers, true));
						if (ordinalFollowers.Count > 0)
						{
							string ordinalNumbersRX = "[0-9]+\\.(?=[ \u00A0]" + ordinalFollowers.GetRegularExpression(out dummy) + "\\s)";
							RegexRecognizer ordinalNumbersRecognizer = new RegexRecognizer(TokenType.Word, 100, "ORDINALNUMBER", "Ordinal Number Recognizer");
							CharacterSet ordinalFirst = new CharacterSet();
							ordinalFirst.Add('0', '9');
							ordinalNumbersRecognizer.Add(ordinalNumbersRX, ordinalFirst);
							AddRecognizer(ordinalNumbersRecognizer);
						}
					}
				}
			}
			else
			{
				// TODO should we still add a rudimentary recognizer for alpha-numerals?
			}

			// TODO other recognizer types (for builtin token classes)
			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeAcronyms) != 0)
			{
				RegexRecognizer recog = CreateAcronymRecognizer(actualCulture, 100);
				if (recog != null)
				{
					_ReclassifyAcronyms = true;
					AddRecognizer(recog);
				}

				// this shouldn't be in the "acronym" setting but it's too late for a UI change...
				recog = CreateUriRecognizer(actualCulture, 100);
				AddRecognizer(recog);

				// TODO make IP address recognizer optional?
				AddRecognizer(CreateIPAddressRecognizer(actualCulture, 101));
				// AddRecognizer(CreateHeadingNumberRecognizer(actualCulture, 50));
			}

			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeVariables) != 0)
			{
				if (accessor != null)
				{
					try
					{
						RegexRecognizer recog = CreateVariableRecognizer(accessor, actualCulture);
						if (recog != null)
							AddRecognizer(recog);
					}
					catch // (System.Exception e)
					{
						// nop - ignore errors
					}
				}
			}

			if ((setup.BuiltinRecognizers & Core.Tokenization.BuiltinRecognizers.RecognizeMeasurements) != 0)
			{
				Recognizer recog;

				if (UseRXMeasurementRecognizer)
					recog = MeasureRegexRecognizer.Create(actualCulture, 100);
				else
					recog = MeasureFSTRecognizer.Create(accessor, actualCulture, 100);

				AddRecognizer(recog);

				// disable for the time being due to performance issues
                //if (accessor.GetResourceStatus(actualCulture, Core.Resources.LanguageResourceType.CurrencySymbols, true) != Core.Resources.ResourceStatus.NotAvailable)
                //{
                //    recog = CreateCurrencyRecognizer(accessor, actualCulture);
                //    AddRecognizer(recog);
                //}
			}

#if false
			// TODO NOTE this slows down the performance too much - need to find a better way

			if (accessor.GetResourceStatus(actualCulture, Core.Resources.LanguageResourceType.Abbreviations, true) != Core.Resources.ResourceStatus.NotAvailable)
			{
				// add an abbreviation recognizer
				Wordlist abbreviations = new Wordlist();
				CharacterSet first;
				abbreviations.Load(accessor.ReadResourceData(actualCulture, Core.Resources.LanguageResourceType.Abbreviations, true));
				string abbreviationsRX = abbreviations.GetRegularExpression(out first) + @"(?=\W)";
				RegexRecognizer abbreviationsRecognizer = new RegexRecognizer(TokenType.Abbreviation, 101, "ABBREVIATION", "Abbreviation Recognizer");
				abbreviationsRecognizer.Add(abbreviationsRX, first);
				AddRecognizer(abbreviationsRecognizer);
			}
#endif

			{
				Recognizer recog;

				bool split = setup.SeparateClitics && Core.CultureInfoExtensions.UsesClitics(_Culture);
				recog = CreateDefaultFallbackRecognizer(split, accessor);
				AddRecognizer(recog);
			}

			SortRecognizers();
		}

		private Recognizer CreateIPAddressRecognizer(System.Globalization.CultureInfo culture, int priority)
		{
			try
			{
				// TODO use culture's digits but don't use Regex \d placeholder as it's not culture sensitive
				// TODO set context (word boundaries)
				// TODO make IP address an alphanumeric token with placeable features?
				// TODO treat all alphanumeric tokens as placeables?
				string pattern = "[0-9]{1,3}(\\.([0-9]{1,3})){3}";
				CharacterSet first = new CharacterSet();
				first.Add('0', '9');

				RegexRecognizer recog = new RegexRecognizer(TokenType.OtherTextPlaceable,
					priority, "IPADDRESS", "DEFAULT_IPADDRESS_RECOGNIZER", true);
				recog.Add(pattern, first);
				// TODO is this culture-dependent?
				recog.OnlyIfFollowedByNonwordCharacter = true;

				return recog;
			}
			catch // (System.Exception e)
			{
				return null;
			}
		}

		private Recognizer CreateHeadingNumberRecognizer(System.Globalization.CultureInfo culture, int priority)
		{
			try
			{
				// TODO use culture's digits but don't use Regex \d placeholder as it's not culture sensitive
				// TODO set context (word boundaries)
				// TODO make IP address an alphanumeric token with placeable features?
				// TODO treat all alphanumeric tokens as placeables?
				string pattern = "[0-9]+(\\.([0-9]{1,3}))+";
				CharacterSet first = new CharacterSet();
				first.Add('0', '9');

				RegexRecognizer recog = new RegexRecognizer(TokenType.OtherTextPlaceable,
					priority, "GENHNUMPLC", "GENERIC_HEADINGNUMBER_RECOGNIZER", true);
				recog.Add(pattern, first);
				// TODO is this culture-dependent?
				recog.OnlyIfFollowedByNonwordCharacter = true;

				return recog;
			}
			catch // (System.Exception e)
			{
				return null;
			}
		}

		private RegexRecognizer CreateVariableRecognizer(Core.Resources.IResourceDataAccessor accessor,
			System.Globalization.CultureInfo actualCulture)
		{
			Wordlist wl = new Wordlist();

			// TODO also create the recognizer if no variables are defined/available? 

			using (System.IO.Stream data = accessor.ReadResourceData(_Culture, Core.Resources.LanguageResourceType.Variables, true))
			{
				if (data != null)
				{
					wl.Load(data);
				}
			}

			if (wl.Count == 0)
				return null;

			// TODO set context restrictions of the recognizer
			RegexRecognizer recog = new RegexRecognizer(TokenType.Variable, 100, "VAR", "DEFAULT_VAR_REGOCNIZER");
			CharacterSet first;
			string rx = wl.GetRegularExpression(out first);
			recog.Add(rx, first);

			recog.OnlyIfFollowedByNonwordCharacter =
				Core.CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);

			return recog;
		}

		private Recognizer CreateCurrencyRecognizer(Core.Resources.IResourceDataAccessor accessor,
			System.Globalization.CultureInfo actualCulture)
		{
			Wordlist wl = new Wordlist();

			using (System.IO.Stream data = accessor.ReadResourceData(actualCulture, Core.Resources.LanguageResourceType.CurrencySymbols, true))
			{
				if (data != null)
				{
					wl.Load(data);
				}
			}

			if (wl.Count == 0)
				return null;

			return CurrencyRegexRecognizer.Create(actualCulture, wl, 100);
		}

		private RegexRecognizer CreateAcronymRecognizer(System.Globalization.CultureInfo actualCulture, int priority)
		{
			// TODO this shouldn't be a recognizer but rather a simple classifier after the 
			//  default fallback recognizer (for performance reasons)

			// TODO set context restrictions of the recognizer
			// TODO acronyms shouldn't be recognized in an all-caps context.
			// TODO we may also include some additional special symbols such as "_" "."
			RegexRecognizer recog = new RegexRecognizer(TokenType.Acronym, priority, "ACR", "DEFAULT_ACR_REGOCNIZER");

			CharacterSet first = new CharacterSet();

			first.Add(System.Globalization.UnicodeCategory.UppercaseLetter);
			// TODO doesn't catch e.g. OePNV, BfA, APIs
			// recog.Add("[A-Z][A-Z&]{0,2}[A-Z]{1,3}", first);
			recog.Add(@"\p{Lu}[\p{Lu}&]{0,4}\p{Lu}", first);

			recog.OnlyIfFollowedByNonwordCharacter
				= Core.CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);

			return recog;
		}

		private RegexRecognizer CreateUriRecognizer(System.Globalization.CultureInfo actualCulture, int priority)
		{
			// TODO this shouldn't be a recognizer but rather a simple classifier after the 
			//  default fallback recognizer (for performance reasons)
			// TODO set context restrictions of the recognizer

			RegexRecognizer recog = new RegexRecognizer(TokenType.Uri, priority, "URI", "DEFAULT_URI_REGOCNIZER");

			CharacterSet first = new CharacterSet();

			// http, https, mailto, ftp, file
			first.Add('h');
			first.Add('H');
			first.Add('m');
			first.Add('M');
			first.Add('f');
			first.Add('F');

			recog.Add("(mailto:|((https|http|ftp|file)://))[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{Po}\\p{S}-['\"<>]]*[\\p{L}\\p{N}\\p{Pc}\\p{Pd}\\p{S}/]", first, true);
			
			// not sure about this one:
			recog.OnlyIfFollowedByNonwordCharacter
				= Core.CultureInfoExtensions.UseBlankAsWordSeparator(actualCulture);

			return recog;
		}

		/// <summary>
		/// Add a recognizer to the collection. 
		/// </summary>
		/// <param name="r">The recognizer</param>
		/// <param name="priority">The priority. Must be >= 0.</param>
		private void AddRecognizer(Recognizer r)
		{
			if (r == null)
				// may happen sometimes if we get exceptions during recognizer construction
				return;

			if (r.Priority < 0)
				throw new ArgumentOutOfRangeException("Priority must be >= 0", "priority");

			_Recognizers.Add(r);
		}

		private void SortRecognizers()
		{
			// TODO stable sort?
			// TODO secondary sort on equal prios?
			_Recognizers.Sort((a, b) => b.Priority - a.Priority);
		}

		public System.Globalization.CultureInfo Culture
		{
			get { return _Culture; }
		}

		// TODO this should rather be sth along "language uses whitespace to separate words"
		public bool BreakOnWhitespace
		{
			get { return _BreakOnWhitespace; }
			set { _BreakOnWhitespace = value; }
		}

		public bool CreateWhitespaceTokens
		{
			get { return _CreateWhitespaceTokens; }
			set { _CreateWhitespaceTokens = value; }
		}

		internal bool ReclassifyAcronyms
		{
			get { return _ReclassifyAcronyms; }
		}

		private DefaultFallbackRecognizer CreateDefaultFallbackRecognizer(bool separateClitics, 
			Core.Resources.IResourceDataAccessor accessor)
		{
			// A fallback recognizer is always lowest prio
			// TODO should this check whether there's already one? 

			DefaultFallbackRecognizer r = null;

			switch (_Culture.TwoLetterISOLanguageName)
			{
			case "ja":
			case "zh":
				r = new DefaultJAZHFallbackRecognizer(TokenType.Unknown, 0, _Culture, accessor);
				break;
			case "th":
			case "km": // Khmer
				r = new DefaultThaiFallbackRecognizer(TokenType.Unknown, 0, _Culture, accessor);
				break;
			default:
				r = new DefaultFallbackRecognizer(TokenType.Unknown, 0, _Culture, accessor, separateClitics);
				break;
			}

			return r;
		}

		// TODO enumerator/IEnumerator impl?
		// TODO do we need to access the priority from outside?
		internal Recognizer this[int index]
		{
			get { return _Recognizers[index]; }
		}

		public int Count
		{
			get { return _Recognizers.Count; }
		}

	}
}
