using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	public class DateTimePatternComputer
	{
		public static List<CalendarDateTimePatterns> GetPatterns(System.Globalization.CultureInfo culture,
			Core.Resources.IResourceDataAccessor accessor,
			Core.Tokenization.DateTimePatternType types)
		{
			DateTimePatternComputer computer = new DateTimePatternComputer(culture, accessor);

			List<CalendarDateTimePatterns> result = null;
			if (accessor != null)
			{
				// TODO support multiple calendars
				result = computer.LoadPatterns(types, false);
			}

			if (result == null)
			{
				// TODO support multiple calendars
				result = computer.ComputePatterns(types, false);
			}

			return result;
		}

		private System.Globalization.CultureInfo _Culture;
		private Core.Resources.IResourceDataAccessor _Accessor;

		private DateTimePatternComputer(System.Globalization.CultureInfo culture,
			Core.Resources.IResourceDataAccessor accessor)
		{
			_Culture = culture;
			_Accessor = accessor;
		}

		/// <summary>
		/// Attempts to load the FSTs from the resource accessor. 
		/// </summary>
		private List<CalendarDateTimePatterns> LoadPatterns(Core.Tokenization.DateTimePatternType types, bool allCalendars)
		{
			if (_Accessor == null)
				return null;

			DateTimePatternType[] iter = new DateTimePatternType[] 
			{
				DateTimePatternType.LongDate, 
				DateTimePatternType.ShortDate,
				DateTimePatternType.LongTime, 
				DateTimePatternType.ShortTime
			};

			List<CalendarDateTimePatterns> result = null;
			// we currently only support one culture pattern
			CalendarDateTimePatterns thePattern = null;

			foreach (DateTimePatternType t in iter)
			{
				if ((types & t) == 0)
					continue;

				Core.Resources.LanguageResourceType rt = Core.Resources.LanguageResourceType.Undefined;

				switch (t)
				{
					case DateTimePatternType.LongDate:
						rt = Core.Resources.LanguageResourceType.LongDateFST;
						break;
					case DateTimePatternType.ShortDate:
						rt = Core.Resources.LanguageResourceType.ShortDateFST;
						break;
					case DateTimePatternType.ShortTime:
						rt = Core.Resources.LanguageResourceType.ShortTimeFST;
						break;
					case DateTimePatternType.LongTime:
						rt = Core.Resources.LanguageResourceType.LongTimeFST;
						break;
					default:
						throw new Exception("Cannot map token type to corresponding resource type");
				}

				if (_Accessor.GetResourceStatus(_Culture, rt, false) != Core.Resources.ResourceStatus.NotAvailable)
				{
					byte[] data = _Accessor.GetResourceData(_Culture, rt, false);
					LanguagePlatform.Lingua.FST.FST fst = LanguagePlatform.Lingua.FST.FST.Create(data);

					if (thePattern == null)
					{
						// TODO support the case where some (not all) FSTs are loaded from the resources
						result = new List<CalendarDateTimePatterns>();
						// TODO support all calendars
						thePattern = new CalendarDateTimePatterns(_Culture, null);
						result.Add(thePattern);
					}

					// TODO compute FIRST() for the FST at load time (or persistently store it?)
					thePattern.Patterns.Add(new DateTimePattern(t, _Culture, "(unavailable)", fst));
				}
			}

			return result;
		}

		private List<CalendarDateTimePatterns> ComputePatterns(Core.Tokenization.DateTimePatternType types, bool allCalendars)
		{
			List<CalendarDateTimePatterns> result = new List<CalendarDateTimePatterns>();

			result.Add(ComputeSinglePattern(types, null));

			if (allCalendars)
			{
				foreach (System.Globalization.Calendar cal in _Culture.OptionalCalendars)
				{
					if (cal != _Culture.Calendar)
						result.Add(ComputeSinglePattern(types, cal));
				}
			}

			return result;
		}

		public static bool IgnorePattern(System.Globalization.CultureInfo culture,
			string pattern)
		{
			switch (culture.Name)
			{
				case "en-US":
					// drop yy/MM/dd due to ambiguity with MM/dd/yy
					return pattern.Equals("yy/MM/dd", StringComparison.Ordinal);
                case "de-DE":
                    // samsa #41554 - "20. Dez 2010" looks strange in de-DE and is ambiguous (for May) with 
                    //  the corresponding long date pattern "d. MMMM yyyy"
                    return pattern.Equals("dd. MMM yyyy", StringComparison.Ordinal)
                        || pattern.Equals("d. MMM yyyy", StringComparison.Ordinal);
				default:
					// nop;
					return false;
			}
		}

		public static List<string> GetCustomPatterns(System.Globalization.CultureInfo culture)
		{
			List<string> result = null;

			switch (culture.Name)
			{
				case "et-EE":
					result = new List<string>();
					result.Add("d. MMMM yyyy");
					result.Add("dd. MMMM yyyy");
					break;
				case "lv-LV":
					result = new List<string>();
					result.Add("yyyy'. gada 'd. MMMM");
					result.Add("yyyy'. gada 'dd. MMMM");
					result.Add("yyyy. MMMM dd");
					break;
				case "nl-NL":
				case "nl-BE":
					result = new List<string>();
					result.Insert(0, "d 'de' MMMM yyyy");
					result.Insert(0, "dd 'de' MMMM yyyy");
					break;
				case "da-DK":
					result = new List<string>();
					result.Add("d/M/yy");
					result.Add("d/M/yyyy");
					result.Add("dd/MM/yyyy");
					result.Add("d.M.yy");
					result.Add("d.M.yyyy");
					result.Add("dd.MM.yyyy");
					break;
				case "pl-PL":
					result = new List<string>();
					result.Add("dd.MM.yyyy");
					result.Add("dd.MM.yy");
					break;
				case "en-GB":
					result = new List<string>();
					result.Add("dddd, dd MMMM yyyy");
					result.Add("dddd, d MMMM yyyy");
					break;
				case "el-GR":
					result = new List<string>();
					result.Add("d.M.yy");
					result.Add("d.M.yyyy");
					result.Add("dd.MM.yyyy");
					break;
				case "hu-HU":
					result = new List<string>();
					result.Add("yyyy. MMMM dd.");
					break;
				case "cs-CZ":
					// #34755
					result = new List<string>();
					result.Add("h:mm tt");
					break;
				default:
					// nop
					break;
			}

			return result;
		}

		private CalendarDateTimePatterns ComputeSinglePattern(Core.Tokenization.DateTimePatternType types, System.Globalization.Calendar cal)
		{
			CalendarDateTimePatterns result;
			List<string> probePatterns;

			// TODO this doesn't yet work with alternate calendars

			if (cal == null)
			{
				result = new CalendarDateTimePatterns(_Culture, _Culture.Calendar);

				probePatterns = new List<string>(_Culture.DateTimeFormat.GetAllDateTimePatterns());
			}
			else
			{
				result = new CalendarDateTimePatterns(_Culture, cal);

				System.Globalization.DateTimeFormatInfo tmp = (System.Globalization.DateTimeFormatInfo)_Culture.DateTimeFormat.Clone();
				tmp.Calendar = cal;

				probePatterns = new List<string>(tmp.GetAllDateTimePatterns());
			}

			// manually augment list of date/time patterns for some languages
			List<string> customPatters = GetCustomPatterns(_Culture);
			if (customPatters != null)
				probePatterns.AddRange(customPatters);

			List<string> patterns = new List<string>();

			// TODO
			/*
			 * The current approach computes a transducer which will emit a canonicalized
			 * representation of the token value which will later be parsed during 
			 * tokenization. Alternatively, we could directly emit the parse pattern and
			 * skip the canonicalization.
			 * */

			foreach (string p in probePatterns)
			{
				if (patterns.Contains(p) || IgnorePattern(_Culture, p))
					continue;

				patterns.Add(p);

				string rx;
				DateTimePatternType patternType = ClassifyFormatString(p, out rx);
				if (patternType == DateTimePatternType.Unknown)
					continue;

				if ((types & patternType) == 0)
					continue;

				// TODO support addWordBoundary
#if DEBUG
				bool log = false;
				if (log)
				{
					using (System.IO.StreamWriter output = new System.IO.StreamWriter(System.IO.Path.GetTempPath() + @"\builder.log", true, System.Text.Encoding.UTF8))
					{
						output.WriteLine("{0}:\r\nPattern: {1}\r\nExpression: {2}\r\n",
							_Culture.Name, p, rx);
					}
				}
#endif

				LanguagePlatform.Lingua.FST.FST f = LanguagePlatform.Lingua.FST.FST.Create(rx);
				f.MakeDeterministic();
				result.Patterns.Add(new DateTimePattern(patternType, _Culture, p, f));
			}

			if (result.Patterns.Count > 0)
			{
				// combine FSTs of each pattern type into a single automaton

				result.Patterns.Sort((a, b) => (int)a.PatternType - (int)b.PatternType);

				List<DateTimePattern> combined = new List<DateTimePattern>();
				while (result.Patterns.Count > 0)
				{
					int first = 0;
					DateTimePatternType t = result.Patterns[0].PatternType;
					int last = first + 1; // intentionally starting at next pattern

					List<FST.FST> alternatives = new List<Sdl.LanguagePlatform.Lingua.FST.FST>();

					while (last < result.Patterns.Count && result.Patterns[last].PatternType == t)
					{
						alternatives.Add(result.Patterns[last].FST);
						++last;
					}

					result.Patterns[0].FST.Disjunct(alternatives);
					result.Patterns[0].FST.MakeDeterministic();

					combined.Add(result.Patterns[0]);
					result.Patterns.RemoveRange(0, last - first);
				}

				result.Patterns.AddRange(combined);
			}

			return result;
		}

		private void AppendLiteral(System.Text.StringBuilder builder, string s)
		{
			if (s.Length == 0)
				// TODO affects other operators such as optionality!
				return;

			for (int i = 0; i < s.Length; ++i)
				AppendLiteral(builder, s[i]);
		}

		private void EmitChar(System.Text.StringBuilder builder, char c)
		{
			bool isSpecial = FST.FST.ReservedCharacters.IndexOf(c) >= 0;

			if (isSpecial)
				builder.Append('\\');

			builder.Append(c);
		}

		private void AppendLiteral(System.Text.StringBuilder builder, char c)
		{
			builder.Append('<');

			if (c != '\0')
				EmitChar(builder, c);

			builder.Append(":>");
		}

		private void AppendLiterals(System.Text.StringBuilder builder,
			List<string> coll)
		{
			if (coll.Count == 0)
				return;

			builder.Append("(");
			for (int i = 0; i < coll.Count; ++i)
			{
				if (i > 0)
					builder.Append("|");
				AppendLiteral(builder, coll[i]);
			}

			builder.Append(")");
		}

		/// <summary>
		/// Determines the pattern type of a date/time format string.
		/// </summary>
		/// <param name="formatString">A format string, as returned from a culture's date/time pattern list.</param>
		/// <returns>The pattern type, which may be "Unknown" for unsupported patterns.</returns>
		public static Core.Tokenization.DateTimePatternType ClassifyPattern(string formatString)
		{
			DateTimePatternComputer computer = new DateTimePatternComputer(System.Globalization.CultureInfo.InvariantCulture, null);
			string rx;
			DateTimePatternType result = computer.ClassifyFormatString(formatString, out rx);
			return result;
		}

		private Core.Tokenization.DateTimePatternType GetPattern(char designator, int length, 
			ref bool hasDateComponents, ref bool hasTimeComponents,
			out string pattern)
		{
			pattern = String.Empty;

			DateTimePatternType type = DateTimePatternType.Unknown;

			switch (designator)
			{
				case 'd':
					hasDateComponents = true;
					switch (length)
					{
						case 1:
						case 2:
							// day, with or without leading zero (some date patterns only contain the 'dd' variant)
							pattern = "(<:d>((1|2)(0|1|2|3|4|5|6|7|8|9)|3(0|1)|0?(1|2|3|4|5|6|7|8|9)))";
							type = DateTimePatternType.ShortDate;
							break;
						case 3:
							// abbreviated day names
							pattern = AppendDayNames(true);
							type = DateTimePatternType.ShortDate;
							break;
						case 4:
							// full day names
							pattern = AppendDayNames(false);
							type = DateTimePatternType.LongDate;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;
				case 'f':
				case 'F':
					// we don't process these patterns during tokenization
					return DateTimePatternType.Unknown;

				case 'h':
					hasTimeComponents = true;
					switch (length)
					{
						case 1:
						case 2:
							// 12h clock with or without leading 0
							pattern = "(<:h>(1(0|1|2)|0?(0|1|2|3|4|5|6|7|8|9)))";
							type = DateTimePatternType.ShortTime;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;
				case 'm':
				case 's':
					{
						hasTimeComponents = true;
						switch (length)
						{
						case 1:
							// minute or second, with or without leading 0
							pattern = "(<:" + designator + ">(((0|1|2|3|4|5)(0|1|2|3|4|5|6|7|8|9))|(0|1|2|3|4|5|6|7|8|9)))";
							break;
						case 2:
							// minute or second, with leading 0
							pattern = "(<:" + designator + ">((0|1|2|3|4|5)(0|1|2|3|4|5|6|7|8|9)))";
							break;
						default:
							return DateTimePatternType.Unknown;
						}
						// the only difference between long and short time patterns is that the long pattern
						//  includes seconds
						if (designator == 's')
							type = DateTimePatternType.LongTime;
						else
							type = DateTimePatternType.ShortTime;
					}
					break;
				case 't':
					{
						hasTimeComponents = true;
						string s = AppendAMPM(length == 1);
						if (s != null)
							pattern = s;
						type = DateTimePatternType.ShortTime;
					}
					break;

				case 'y':
					hasDateComponents = true;
					switch (length)
					{
						case 1:
							// 2-digit year w/o century, w/o leading 0
							pattern = "(<:y>((1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)|(0|1|2|3|4|5|6|7|8|9)))";

							type = DateTimePatternType.ShortDate;
							break;
						case 2:
							// 2-digit year w/o century, w/ leading 0
							pattern = "(<:y>((0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)))";

							type = DateTimePatternType.ShortDate;
							break;
						case 4:
							// 4-digit year
							pattern = "(<:y>((0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)(0|1|2|3|4|5|6|7|8|9)))";
							type = DateTimePatternType.ShortDate;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;
				case 'g':
					hasDateComponents = true;
					switch (length)
					{
						case 2:
							{
								// TODO preceding whitespace in case eras are undefined
								string eras = AppendEras();
								if (eras != null)
									pattern = eras;
							}
							type = DateTimePatternType.ShortDate;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;

				case 'z':
					// time zone offset: only in special time patterns
					return DateTimePatternType.Unknown;

				case 'H':
					hasTimeComponents = true;
					switch (length)
					{
						case 1:
						case 2:
							// hours in 24h format, with or without leading 0
							pattern = "(<:H>(1(0|1|2|3|4|5|6|7|8|9)|2(0|1|2|3|4)|0?(0|1|2|3|4|5|6|7|8|9)))";
							type = DateTimePatternType.ShortTime;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;
				case 'M':
					hasDateComponents = true;
					switch (length)
					{
						case 1:
						case 2:
							// numeric month, with or without leading 0
							pattern = "(<:M>(1(0|1|2)|0?(1|2|3|4|5|6|7|8|9)))";
							type = DateTimePatternType.ShortDate;
							break;
						case 3:
							// abbreviated month names
							pattern = AppendMonthNames(true);
							type = DateTimePatternType.ShortDate;
							break;
						case 4:
							// full month names
							pattern = AppendMonthNames(false);
							type = DateTimePatternType.LongDate;
							break;
						default:
							return DateTimePatternType.Unknown;
					}
					break;

				default:
					System.Diagnostics.Debug.Assert(false, "Error in pattern char list");
					return DateTimePatternType.Unknown;
			}

			return type;
		}

		private Core.Tokenization.DateTimePatternType ClassifyFormatString(string formatString, out string rx)
		{
			const string formatChars = "dfFhmstyzgHM";

			int len = formatString.Length;
			int p = 0;

			DateTimePatternType resultPatternType = DateTimePatternType.Unknown;

			bool dateHasDay = false;
			bool dateHasMonth = false;
			bool dateHasYear = false;

			bool hasDateComponents = false;
			bool hasTimeComponents = false;
			
			rx = String.Empty;

			System.Text.StringBuilder result = new StringBuilder();

			bool escapeMode = false;

			while (p < len)
			{
				char c = formatString[p];

				if (escapeMode)
				{
					if (c == '\'')
					{
						if (p + 1 < len && formatString[p + 1] == '\'')
						{
							// two adjacent single quotes: append a single quote
							AppendLiteral(result, c);
							++p;
						}
						else
						{
							// single single quote ends escape mode
							escapeMode = false;
						}
					}
					else if (c == '\\')
					{
						// backslash followed by quote also represents a literal quote
						if (p + 1 < len && formatString[p + 1] == '\'')
						{
							AppendLiteral(result, '\'');
							++p;
						}
						else
						{
							AppendLiteral(result, c);
						}
					}
					else
					{
						if (c == '\u5e74' || c == '\u6708' || c == '\u65e5')
						{
							// ja, zh: indicates long date pattern
							resultPatternType = DateTimePatternType.LongDate;
						}

						AppendLiteral(result, c);
					}

					++p;
					continue;
				}

				if (formatChars.IndexOf(c) >= 0)
				{
					// format character -- process accordingly
					int formatLength = 0;

					while (p < len && formatString[p] == c)
					{
						++formatLength;
						++p;
					}

					switch (c)
					{
						case 'd':
							dateHasDay = true;
							break;
						case 'M':
							dateHasMonth = true;
							break;
						case 'y':
							dateHasYear = true;
							break;
						default:
							break;
					}

					string pattern;
					DateTimePatternType type = GetPattern(c, formatLength, 
						ref hasDateComponents, ref hasTimeComponents,
						out pattern);
					if (pattern == null || pattern.Length == 0)
					{
						// if a pattern string contains unknown or unprocessable elements, we don't 
						//  construct an RX for it
						return DateTimePatternType.Unknown;
					}
					else
					{
						result.Append(pattern);

						switch (resultPatternType)
						{
							case DateTimePatternType.Unknown:
								resultPatternType = type;
								break;
							case DateTimePatternType.LongDate:
								// long date patterns never "downgrade" to simpler ones
								break;
							case DateTimePatternType.ShortDate:
								// shot date patterns only upgrade to long date patterns
								if (type == DateTimePatternType.LongDate)
									resultPatternType = DateTimePatternType.LongDate;
								break;
							case DateTimePatternType.LongTime:
								// time patterns upgrade to date patterns only
								if (type == DateTimePatternType.LongDate || type == DateTimePatternType.ShortDate)
									resultPatternType = type;
								break;
							case DateTimePatternType.ShortTime:
								// short time patterns upgrade to all other patterns
								if (type != DateTimePatternType.Unknown)
									resultPatternType = type;
								break;
							default:
								throw new Exception("Invalid switch constant");
						}
					}
				}
				else if (c == '\'')
				{
					++p;
					escapeMode = true;
				}
				else if (c == '%')
				{
					// don't quite understand MSDN what this stands for..
					System.Diagnostics.Debug.Assert(false);
					++p;
				}
				else if (c == ':')
				{
					// default time separator
					// TODO the default date/time separator may be a blank in which case we maybe should
					//  append \s instead of ' '
					AppendLiteral(result, _Culture.DateTimeFormat.TimeSeparator);
					++p;
				}
				else if (c == '/')
				{
					// default date separator
					// TODO the default date/time separator may be a blank in which case we maybe should
					//  append \s instead of ' '
					AppendLiteral(result, _Culture.DateTimeFormat.DateSeparator);
					++p;
				}
				else if (c == '\\')
				{
					// literally append next char
					if (p + 1 < len)
					{
						AppendLiteral(result, formatString[p + 1]);
						++p;
					}
					else
					{
						// trailing backslash - may also yield an error
						AppendLiteral(result, c);
						++p;
					}
				}
				else if (c == ' ')
				{
					// append single whitepsace
					result.Append(@"<\s:>");
					// NOTE we assume that patterns don't include leading blanks and therefore don't add them to first
					++p;
				}
				else
				{
					// literally append char
					AppendLiteral(result, c);
					++p;
				}
			}

			if (resultPatternType == DateTimePatternType.LongDate
				|| resultPatternType == DateTimePatternType.ShortDate)
			{
				if (!(dateHasDay && dateHasMonth && dateHasYear))
					// #33803: don't recognize "July 08" without a year
					return DateTimePatternType.Unknown;
			}

			// #38942
			if ((hasDateComponents && hasTimeComponents)
				|| (!hasDateComponents && !hasTimeComponents))
			{
				// pattern either specifies neither date nor time, or both date and time
				// TODO allow "mixed" patterns (f, F, g, G pattern classes of DateTimeFormatInfo)
				return DateTimePatternType.Unknown;
			}

			rx = result.ToString();
			return resultPatternType;
		}

		private static bool UseNativeDigits(System.Globalization.CultureInfo culture)
		{
			string[] nativeDigits = culture.NumberFormat.NativeDigits;

			bool useNativeDigits = (nativeDigits.Length == 10);
			int standardDigits = 0;

			if (useNativeDigits)
			{
				for (int i = 0; i < 10 && useNativeDigits; ++i)
				{
					if (nativeDigits[i].Length != 1)
						useNativeDigits = false;
					else if (nativeDigits[i][0] == ('0' + i))
						++standardDigits;
				}
			}

			// TODO test whether the native digits are consecutive in their Unicode position 
			//  (important for RXs such as [0-9])

			// if all 10 digits are identical to the standard digits, we don't have 
			//  to use the native digits. 
			return useNativeDigits && (standardDigits != 10);
		}

		private string AppendAMPM(bool abbreviated)
		{
			string am = _Culture.DateTimeFormat.AMDesignator ?? String.Empty;
			string pm = _Culture.DateTimeFormat.PMDesignator ?? String.Empty;

			if (am.Length == 0 && pm.Length == 0)
			{
				return String.Empty;
			}

			string pattern;

			if (abbreviated)
			{
				pattern = "t";
				if (am.Length > 1)
					am = am.Substring(0, 1);
				if (pm.Length > 1)
					pm = pm.Substring(0, 1);
			}
			else
				pattern = "tt";

			System.Text.StringBuilder result = new StringBuilder("(");

			int emitted = 0;

			// need to emit the am/pm designators literally so that the parser can resolve them correctly
			if (am.Length > 0)
			{
				System.Diagnostics.Debug.Assert(am.IndexOf('\'') < 0);
				result.Append(FSTCombine(am, pattern + "'" + am + "'"));
				++emitted;
			}

			if (pm.Length > 0)
			{
				System.Diagnostics.Debug.Assert(pm.IndexOf('\'') < 0);
				if (emitted > 0)
					result.Append("|");
				result.Append(FSTCombine(pm, pattern + "'" + pm + "'"));
				++emitted;
			}

			result.Append(")");

			if (emitted == 1)
				// TODO any preceding whitespace needs to be optional as well
				result.Append("?");

			return result.ToString();
		}

		private string AppendMonthNames(bool getAbbreviatedNames)
		{
			System.Text.StringBuilder builder = new StringBuilder();

			bool atFirst = true;

			builder.Append("(");
			for (int iter = 0; iter < 2; ++iter)
			{
				string[] names;
				if (iter == 0)
					names = getAbbreviatedNames ? _Culture.DateTimeFormat.AbbreviatedMonthNames : _Culture.DateTimeFormat.MonthNames;
				else
					names = getAbbreviatedNames ? _Culture.DateTimeFormat.AbbreviatedMonthGenitiveNames : _Culture.DateTimeFormat.MonthGenitiveNames;

				for (int i = 0; i < names.Length; ++i)
				{
					if (String.IsNullOrEmpty(names[i]))
						continue;

					// TODO update FIRST set
					string expression = FSTCombine(names[i], String.Format("M{0}", i + 1));

					if (atFirst)
						atFirst = false;
					else
						builder.Append("|");

					builder.Append(expression);
				}
			}
			builder.Append(")");

			return builder.ToString();
		}

		private string AppendDayNames(bool getAbbreviatedNames)
		{
			List<string> coll = new List<string>();

			AddStrings(coll, getAbbreviatedNames ? _Culture.DateTimeFormat.AbbreviatedDayNames : _Culture.DateTimeFormat.DayNames);

			if (coll.Count == 0)
				return String.Empty;

			System.Text.StringBuilder builder = new StringBuilder();
			// no FST output for day names
			AppendLiterals(builder, coll);
			return builder.ToString();
		}

		private string AppendEras()
		{
			// TODO alternative calendars
			if (_Culture.DateTimeFormat.Calendar.Eras == null
				|| _Culture.DateTimeFormat.Calendar.Eras.Length == 0)
				return null;

			System.Text.StringBuilder builder = new StringBuilder();

			for (int e = 0; e < _Culture.DateTimeFormat.Calendar.Eras.Length; ++e)
			{
				// TODO this assumes that the eras are always sorted in the same way
				string eraName = _Culture.DateTimeFormat.GetEraName(_Culture.DateTimeFormat.Calendar.Eras[e]);
				if (e > 0)
					builder.Append('|');
				// need to emit the era name literally so that the parser can resolve it correctly
				System.Diagnostics.Debug.Assert(eraName.IndexOf('\'') < 0);
				builder.Append(FSTCombine(eraName, "g'" + eraName + "'"));
			}

			return builder.ToString();
		}

		private static void AddStrings(List<string> coll, string[] values)
		{
			foreach (string s in values)
				if (!(String.IsNullOrEmpty(s) || coll.Contains(s)))
					coll.Add(s);
		}

		private static string FSTCombine(string upper, string lower)
		{
			int upperLen = upper.Length;
			int lowerLen = lower.Length;

			System.Text.StringBuilder builder = new StringBuilder();

			int p = 0;
			while (p < upperLen || p < lowerLen)
			{
				builder.Append("<");

				if (p < upperLen)
				{
					if (FST.FST.ReservedCharacters.IndexOf(upper[p]) >= 0)
						builder.Append('\\');
					builder.Append(upper[p]);
				}

				builder.Append(":");

				if (p < lowerLen)
				{
					if (FST.FST.ReservedCharacters.IndexOf(lower[p]) >= 0)
						builder.Append('\\');
					builder.Append(lower[p]);
				}

				builder.Append(">");

				++p;
			}

			return builder.ToString();
		}
	}
}
