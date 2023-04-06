using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using NLog;
using Sdl.Community.Extended.MessageUI;
using Sdl.Community.NumberVerifier.Composers;
using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Reporter;
using Sdl.Community.NumberVerifier.Validator;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;

namespace Sdl.Community.NumberVerifier
{
	/// <summary>
	/// Required annotation for declaring the extension class.
	/// </summary>

	[GlobalVerifier("Number Verifier", "Plugin_Name", "Plugin_Description")]
	public class NumberVerifierMain : IGlobalVerifier, IBilingualVerifier, ISharedObjectsAware
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly TextFormatter _textFormatter;
		private bool? _enabled;
		private ISharedObjects _sharedObjects;
		private string _sourceText;
		private string _targetText;
		private INumberVerifierSettings _verificationSettings;

		public NumberVerifierMain() : this(null)
		{
			_textFormatter ??= new TextFormatter();
		}

		public NumberVerifierMain(INumberVerifierSettings numberVerifierSettings)
		{
			_verificationSettings = numberVerifierSettings;

			_textFormatter ??= new TextFormatter();

			_numberValidator = new NumberValidator();
		}

		public bool Enabled
		{
			get
			{
				if (_enabled.HasValue) return _enabled.Value;
				var settingBundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
				if (settingBundle == null)
				{
					_enabled = false;
				}
				else
				{
					var settingGroup = settingBundle.GetSettingsGroup(SettingsId);
					_enabled = settingGroup.GetSetting<bool>("Enabled");
				}

				return _enabled != null && _enabled.Value;
			}
		}

		/// <summary>
		/// Initializes the settings bundle object from which to retrieve the setting(s)
		/// to be used in the verification logic, e.g. the context display code to
		/// which the verification should be applied.
		/// </summary>

		public INumberVerifierSettings VerificationSettings
		{
			get
			{
				if (_verificationSettings != null || _sharedObjects == null) return _verificationSettings;
				var bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
				if (bundle == null) return _verificationSettings;
				_verificationSettings = bundle.GetSettingsGroup<NumberVerifierSettings>();
				return _verificationSettings;
			}
		}

		/// <summary>
		/// Creates the text generator, which traverses the elements of the current
		/// segment (e.g. text and tags). In our implementation the Text Generator
		/// is used to retrieve only plain text information from the current segment,
		/// if tags should not be considered for the verification.
		/// </summary>

		private TextGenerator _textGenerator;

		public TextGenerator TextGenerator
		{
			get { return _textGenerator ??= new TextGenerator(); }
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}

		/// <summary>
		/// The following members set some general properties of the verification plug-in,
		/// e.g. the plug-in name and the icon that are displayed in the user interface of Trados Studio.
		/// </summary>

		public string Description
		{
			get { return PluginResources.Verifier_Description; }
		}

		public Icon Icon
		{
			get { return PluginResources.icon; }
		}

		public string Name
		{
			get { return PluginResources.Plugin_Name; }
		}

		public string HelpTopic
		{
			get { return string.Empty; }
		}

		public string SettingsId
		{
			get { return "Number Verifier"; }
		}

		public IList<string> GetSettingsPageExtensionIds()
		{
			IList<string> list = new List<string>();

			list.Add("Number Settings Definition ID");
			list.Add("Number Verifier Help Definition ID");

			return list;
		}

		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}

		/// <summary>
		/// This member is used to output any verification messages in the user interface of Trados Studio.
		/// </summary>

		public IBilingualContentMessageReporter MessageReporter
		{
			get;
			set;
		}

		private readonly NumberValidator _numberValidator;

		public void Initialize(IDocumentProperties documentInfo)
		{
			
		}

		public void Complete()
		{
			// Not required for this implementation.
		}

		public void FileComplete()
		{
			// Not required for this implementation.
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			// Not required for this implementation.
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (Enabled)
			{
				// Apply the verification logic.
				CheckParagraphUnit(paragraphUnit);
			}
		}

		public INumberVerifier GenericNumberVerifier
		{
			get
			{
				return new NumberFormatVerifier(VerificationSettings, TextGenerator);
			}
		}

		/// <summary>
		/// The following member performs the actual verification. It traverses the segment pairs of the current document,
		/// and checks whether a particular segment has any numbers. It then determines whether
		/// the target and the source contains the same numbers.
		/// If not, a warning message will be generated, which is then displayed between the source and target segments,
		/// and in the Messages window of Trados Studio.
		/// </summary>
		/// <param name="paragraphUnit"></param>
		public void CheckParagraphUnit(IParagraphUnit paragraphUnit)
		{
			try
			{
				// loop through the whole paragraph unit
				foreach (var segmentPair in paragraphUnit.SegmentPairs.Where(FilterSegmentPairs))
				{
					_sourceText = GetSegmentText(segmentPair.Source);
					_targetText = GetSegmentText(segmentPair.Target);

					var errorMessages = CheckSegmentPair(_sourceText, _targetText, segmentPair);
					ReportErrors(segmentPair, errorMessages);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		private void ReportErrors(ISegmentPair segmentPair, List<ErrorReporting> errorMessages)
		{
			foreach (var errorMessage in errorMessages)
			{
				if (errorMessage.ExtendedErrorMessage != string.Empty && VerificationSettings.ReportExtendedMessages)
				{
					if (MessageReporter is IBilingualContentMessageReporterWithExtendedData extendedMessageReporter)
					{
						var messageDataModel = new MessageDataModel
						{
							SourceIssues = errorMessage.SourceNumberIssues?.Replace(Environment.NewLine, string.Empty),
							TargetIssues = errorMessage.TargetNumberIssues?.Replace(Environment.NewLine, string.Empty),
							ReplacementSuggestion = segmentPair.Target,
							InitialSourceIssues = errorMessage.InitialSourceNumber,
							InitialTargetIssues = errorMessage.InitialTargetNumber,
							ErrorMessage = errorMessage.ErrorMessage,
							IsHindiVerification = errorMessage.IsHindiVerification
						};
						var extendedData = new NumberVerifierMessageData(messageDataModel);

						extendedMessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
							errorMessage.ErrorLevel, errorMessage.ExtendedErrorMessage,
							new TextLocation(new Location(segmentPair.Target, true), 0),
							new TextLocation(new Location(segmentPair.Target, false),
								segmentPair.Target.ToString().Length - 1),
							extendedData);
					}
				}
				else if (errorMessage.ErrorMessage != string.Empty)
				{
					if (!string.IsNullOrEmpty(errorMessage.TargetNumberIssues))
					{
						if (errorMessage.ErrorMessage == PluginResources.Error_AlphanumericsModified)
						{
							var alphaList = new List<string>();
							List<string> alphaTargetList = new List<string>();

							var alphanumericsText = Regex.Matches(errorMessage.TargetNumberIssues, @"^-?\u2212?(^(?=.*[a-zA-Z{0}])(?=.*[0-9]).+$)");

							foreach (Match alphanumericText in alphanumericsText)
							{
								var words = Regex.Split(alphanumericText.Value, @"\s");

								alphaList.AddRange(
											from word in words
											from Match match in Regex.Matches(word.Normalize(NormalizationForm.FormKC), @"^-?\u2212?(^(?=.*[a-zA-Z{0}])(?=.*[0-9]).+$)")
											select match.Value);

								foreach (var alphaElement in alphaList)
								{
									var alphanumericTarget = $@"""{alphaElement}""";
									alphaTargetList.Add(alphanumericTarget);
								}
								var alphanumericRes = string.Join(", ", alphaTargetList.ToArray());
								errorMessage.ErrorMessage = string.Concat(errorMessage.ErrorMessage, " (", alphanumericRes, ")");
							}
						}
						else
						{
							var targetNumbers = new List<string>();
							var numbers = Regex.Matches(errorMessage.TargetNumberIssues, @"[\+\-]?\s*[0-9\.\,]*[Ee]?[\+\-]?\d+",
								RegexOptions.Singleline);

							foreach (var value in numbers)
							{
								var targetNumber = $@"""{value}""";
								targetNumbers.Add(targetNumber);
							}
							var res = string.Join(", ", targetNumbers.ToArray());

							errorMessage.ErrorMessage = string.Concat(errorMessage.ErrorMessage, " (", res, ")");
						}
					}

					MessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
						errorMessage.ErrorLevel, errorMessage.ErrorMessage,
						new TextLocation(new Location(segmentPair.Target, true), 0),
						new TextLocation(new Location(segmentPair.Target, false), segmentPair.Target.ToString().Length - 1));
				}
			}
		}

		/// <summary>
		/// Returns a list of errors after checking the alphanumerics
		/// e.g: AB12
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <param name="sourceExcludedRanges"></param>
		/// <param name="targetExcludedRanges"></param>
		/// <returns></returns>
		public IEnumerable<ErrorReporting> CheckAlphanumerics(string sourceText, string targetText, List<ExcludedRange> sourceExcludedRanges = null, List<ExcludedRange> targetExcludedRanges = null)
		{
			if (!_verificationSettings.CustomsSeparatorsAlphanumerics && !_verificationSettings.ReportModifiedAlphanumerics)
			{
				return Enumerable.Empty<ErrorReporting>();
			}
			try
			{
				var sourceAlphanumericsList = GetAlphanumericList(sourceText, true, sourceExcludedRanges);

				// find all alphanumeric names in target and add to list
				var targetAlphanumericsList = GetAlphanumericList(targetText, false, targetExcludedRanges);

				// remove alphanumeric names found both in source and target from respective list
				RemoveMatchingAlphanumerics(sourceAlphanumericsList.Item2, targetAlphanumericsList.Item2);
				var numberModel = new NumberModel
				{
					Settings = VerificationSettings,
					SourceNumbers = sourceAlphanumericsList.Item2,
					TargetNumbers = targetAlphanumericsList.Item2,
					InitialSourceNumbers = sourceAlphanumericsList.Item1,
					InitialTargetNumbers = targetAlphanumericsList.Item1,
					SourceText = sourceText,
					TargetText = targetText
				};
				var numberResults = new NumberResults(numberModel);

				var alphanumericErrorComposer = new AlphanumericErrorComposer();
				var verifyProcessor = alphanumericErrorComposer.Compose();

				return verifyProcessor.Verify(numberResults);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
				return new List<ErrorReporting>();
			}
		}

		/// <summary>
		/// Returns a errors list after numbers are normalized
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <param name="sourceExcludedRanges"></param>
		/// <param name="targetExcludedRanges"></param>
		/// <param name="segmentPair"></param>
		/// <returns></returns>
		public void CheckSequences(string sourceText, string targetText, List<ExcludedRange> sourceExcludedRanges, List<ExcludedRange> targetExcludedRanges,  ISegmentPair segmentPair = null)
		{
			_numberValidator.Verify(sourceText, targetText, VerificationSettings, out var sourceNumbers, out var targetNumbers, sourceExcludedRanges, targetExcludedRanges);

			if (segmentPair is null) return;

			var reportAllowanceTable = new Dictionary<string, bool>
			{
				[PluginResources.Error_NumberAdded] = VerificationSettings.ReportAddedNumbers,
				[PluginResources.Error_NumbersRemoved] = VerificationSettings.ReportRemovedNumbers,
				[PluginResources.Error_DifferentValues] = VerificationSettings.ReportModifiedNumbers,
				[PluginResources.Error_AlphanumericsModified] = VerificationSettings.ReportModifiedAlphanumerics
			};
			var errorReporter = new ErrorReporter(MessageReporter, VerificationSettings, new MessageFilter(reportAllowanceTable));
			errorReporter.ReportErrors(sourceNumbers, targetNumbers, segmentPair);
		}

		/// <summary>
		/// Returns a list of errors which contains combined errors from alphanumerics check and numbers check
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <param name="segmentPair"></param>
		/// <returns></returns>
		public List<ErrorReporting> CheckSegmentPair(string sourceText, string targetText, ISegmentPair segmentPair = null)
		{
			var sourceExcludedRanges = GetExcludedRanges(sourceText);
			var targetExcludedRanges = GetExcludedRanges(targetText);

			var errorList = new List<ErrorReporting>();

			var errorsListFromAlphanumerics = CheckAlphanumerics(sourceText, targetText, sourceExcludedRanges, targetExcludedRanges);
			errorList.AddRange(errorsListFromAlphanumerics);

			CheckSequences(sourceText, targetText, sourceExcludedRanges, targetExcludedRanges, segmentPair);

			// generic number verifier to identify errors related to the numeric convention taking
			// into consideration the settings applied.
			var genericErrorMeassages = GenericNumberVerifier.Verify(segmentPair, sourceExcludedRanges, targetExcludedRanges);
			errorList.AddRange(genericErrorMeassages);

			return errorList;
		}

		private List<ExcludedRange> GetExcludedRanges(string text)
		{
			if (VerificationSettings.RegexExclusionList is null) return new List<ExcludedRange>();
			var excludedRanges = new List<ExcludedRange>();
			foreach (var pattern in VerificationSettings.RegexExclusionList)
			{
				var matches =
					Regex.Matches(text, pattern.Pattern)
						.Cast<Match>()
						.Where(m => !string.IsNullOrWhiteSpace(m.Value))
						.ToList();

				matches.ForEach(m => excludedRanges.Add(new ExcludedRange
				{
					LeftLimit = m.Index,
					RightLimit = m.Index + m.Length - 1
				}));
			}

			return excludedRanges.MergeAdjacentRanges();
		}

		public Tuple<List<string>, List<string>> GetAlphanumericList(string text, bool isSource = false, List<ExcludedRange> excludedRanges = null)
		{
			try
			{
				var normalizedAlphaList = new List<string>();
				var words = Regex.Split(text, @"\s");
				var customsSeparators = !string.IsNullOrEmpty(_verificationSettings.AlphanumericsCustomSeparator)
					? _verificationSettings.AlphanumericsCustomSeparator.Split(',')
					: Array.Empty<string>();

				// The below foreach is used when checking those tags like Source: "<color=70236>Word" and Target:<color=70236>OtherWord
				// and no empty space is between the '>' and 'Word' or between the '>' and 'OtherWord'.
				// Because of the missing of empty space, the functionality recognize as beeing alphanumeric and when source and target were not that same('Word' different than 'OtherWord'
				// error message regarding Alphanumeric modification appeard.
				var wordsRes = new List<string>();
				foreach (var w in words)
				{
					if (w.Contains('<') || w.Contains('>'))
					{
						var wRes = new string[] { };
						if (w.Contains('<'))
						{
							var charIndex = w.IndexOf('<');
							var wordReplace = w.Insert(charIndex, " ");
							wRes = Regex.Split(wordReplace, @"\s");
						}
						if (w.Contains('>'))
						{
							var charIndex = w.IndexOf('>');
							var wordReplace = w.Insert(charIndex + 1, " ");
							wRes = Regex.Split(wordReplace, @"\s");
						}
						foreach (var r in wRes)
						{
							wordsRes.Add(r);
						}
					}
					else
					{
						wordsRes.Add(w);
					}
				}

				var separators = _verificationSettings.CustomsSeparatorsAlphanumerics
					? _textFormatter.GetAlphanumericsCustomSeparators(customsSeparators)
					: string.Empty;
				var regex = $"(?i)(?=.*[0-9])(?=.*[a-z])([a-z0-9{separators}]+)";

				normalizedAlphaList.AddRange(
					wordsRes.SelectMany(word =>
					{
						var matches = Regex.Matches(word.Normalize(NormalizationForm.FormKC), regex).Cast<Match>().ToList();

						matches.ExcludeRanges(excludedRanges);
						return matches;
					},
						(word, match) => Regex.Replace(match.Value, "\u2212|-", "m")));

				RemoveNonAlphanumericals(isSource, normalizedAlphaList);

				var unNormalizedAlphanumerics = new List<string>();
				unNormalizedAlphanumerics.AddRange(from word in wordsRes
												   from Match match in Regex.Matches(word.Normalize(NormalizationForm.FormKC), regex)
												   select word);

				return GetAlphnumericsTuple(unNormalizedAlphanumerics, normalizedAlphaList);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
				return new Tuple<List<string>, List<string>>(new List<string>(), new List<string>());
			}
		}

		public Tuple<List<string>, List<string>> GetAlphnumericsTuple(List<string> alphaNumericsList, List<string> normalizedAlphaNumericsList)
		{
			return Tuple.Create(alphaNumericsList, normalizedAlphaNumericsList);
		}

		public Dictionary<string, string> GetEasternArabicNumbers()
		{
			var hindiDictionary = new Dictionary<string, string>
			{
				{ "0", "٠" },
				{ "1", "١" },
				{ "2", "٢" },
				{ "3", "٣" },
				{ "4", "٤" },
				{ "5", "٥" },
				{ "6", "٦" },
				{ "7", "٧" },
				{ "8", "٨" },
				{ "9", "٩" }
			};

			return hindiDictionary;
		}

		public List<NumberModel> GetFormatedNumbers(HindiNumberModel hindiNumberModel)
		{
			var result = new List<NumberModel>();

			try
			{
				var res = hindiNumberModel.TextGroups.Zip(hindiNumberModel.TargetGroups, (s, t) => new NumberModel { SourceText = s, TargetText = t }).ToList();

				// add thousand separator or decimal separtor in the target text as it is in the source text where needed
				foreach (var numberRes in res)
				{
					if (!string.IsNullOrEmpty(numberRes.TargetText))
					{
						// add . separator in the translated number as it is in the source number(this change will work only for valid verification)
						// source: the converted hindi to arabic/just arabic(depending on source langauge) and target; (arabic/converted hindi to arabic)
						// valid ex: source: 1234,56 => target: 1.234,56/1,234.56 or source: 1.234,56 => target: 1.234,56
						// invalid ex: soruce: 1234,56  => target: 12.34,56
						if (numberRes.SourceText.Contains("."))
						{
							var sourceTextIndex = numberRes.SourceText.IndexOf(".");
							if (!numberRes.TargetText.Contains("."))
							{
								numberRes.TargetText = numberRes.TargetText.Insert(sourceTextIndex, ".");
							}
						}
						if (numberRes.SourceText.Contains(","))
						{
							var sourceTextIndex = numberRes.SourceText.IndexOf(",");
							if (!numberRes.TargetText.Contains(","))
							{
								numberRes.TargetText = numberRes.TargetText.Insert(sourceTextIndex, ",");
							}
							else
							{
								// Scenario of translation from Hindi to Arabic: ١٢٣٤,٨٩ => 1.234,56 or 1,234.56 should be valid.
								// in scenario: ١٢٣٤,٨٩ => 1,234.56, the Hindi number is converted to 1234,56
								// in the above code the . separator is added where it should be
								// in the the bellow code, the , separator is moved at the right place
								// so the target result it will be 1,234.56 for verification.
								if (numberRes.TargetText.IndexOf(",.") != -1)
								{
									numberRes.TargetText = Regex.Replace(numberRes.TargetText, ",+\\.+", ".");
									numberRes.TargetText = numberRes.TargetText.Insert(sourceTextIndex, ",");
								}
							}
						}
						if (numberRes.TargetText.IndexOf(".,") != -1)
						{
							numberRes.TargetText = Regex.Replace(numberRes.TargetText, "\\.+\\,+", ".");
						}
					}

					if (hindiNumberModel.SourceLanguage.Equals(Constants.HindiLanguage))
					{
						var sourceText = hindiNumberModel.HindiDictionary.FirstOrDefault(s => s.Key.Equals(numberRes.SourceText));
						result.Add(new NumberModel
						{
							SourceText = !string.IsNullOrEmpty(sourceText.Value) ? sourceText.Value : numberRes.SourceText,
							SourceArabicText = numberRes.SourceText,
							TargetText = numberRes.TargetText,
							TargetArabicText = sourceText.Key
						});
					}
					// map to the corresponding source text for the Hindi target numbers found with issues
					if (hindiNumberModel.TargetDictionary.Count > 0)
					{
						var sourceText = hindiNumberModel.HindiDictionary.FirstOrDefault(s => s.Key.Contains(numberRes.SourceText));

						var targetText = hindiNumberModel.TargetDictionary.FirstOrDefault(t => t.Key.Contains(sourceText.Value));
						result.Add(new NumberModel
						{
							SourceText = !string.IsNullOrEmpty(sourceText.Key) ? sourceText.Key : numberRes.SourceText,
							SourceArabicText = numberRes.SourceText,
							TargetText = targetText.Value,
							TargetArabicText = targetText.Key
						});
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
			return result;
		}

		public List<NumberModel> GetTargetFromHindiNumbers(string source, string target, string sourceLanguage)
		{
			var result = new List<NumberModel>();
			try
			{
				var sb = new StringBuilder();
				var easternArabicNumbers = GetEasternArabicNumbers();
				var hindiNumberModel = new HindiNumberModel
				{
					SourceGroups = source.Split(' ').ToArray(),
					TargetGroups = target.Split(' ').ToArray(),
					SourceLanguage = sourceLanguage,
					TargetDictionary = new Dictionary<string, string>(),
					HindiDictionary = new Dictionary<string, string>(),
					TextGroups = new string[] { }
				};
				if (sourceLanguage.Equals(Constants.HindiLanguage))
				{
					var sourceResult = string.Empty;
					var sourceGroupResult = new List<string>();
					foreach (var sourceGroup in hindiNumberModel.SourceGroups)
					{
						foreach (var s in sourceGroup)
						{
							sourceResult = easternArabicNumbers.ContainsValue(s.ToString())
								? sb.Append(easternArabicNumbers.FirstOrDefault(h => h.Value == s.ToString()).Key).ToString()
								: sb.Append(s.ToString()).ToString();
						}

						if (!string.IsNullOrEmpty(sourceResult) && !string.IsNullOrEmpty(sourceGroup))
						{
							hindiNumberModel.HindiDictionary.Add(sourceResult, sourceGroup);
							sourceGroupResult.Add(sourceResult);
						}
						sourceResult = string.Empty;
						hindiNumberModel.TextGroups = sourceGroupResult.ToArray();
						sb.Clear();
					}
					result = GetFormatedNumbers(hindiNumberModel);
				}
				else
				{
					var targetResult = string.Empty;
					foreach (var targetGroup in hindiNumberModel.TargetGroups)
					{
						foreach (var t in targetGroup)
						{
							targetResult = easternArabicNumbers.ContainsValue(t.ToString())
								? sb.Append(easternArabicNumbers.FirstOrDefault(h => h.Value == t.ToString()).Key).ToString()
								: sb.Append(t.ToString()).ToString();
						}

						if (!string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(targetResult))
						{
							hindiNumberModel.HindiDictionary.Add(source, targetResult);
						}

						if (!string.IsNullOrEmpty(targetResult) && !string.IsNullOrEmpty(targetGroup))
						{
							hindiNumberModel.TargetDictionary.Add(targetResult, targetGroup);
						}
						targetResult = string.Empty;
						hindiNumberModel.TextGroups = hindiNumberModel.SourceGroups;
						sb.Clear();
					}
					result = GetFormatedNumbers(hindiNumberModel);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
			return result;
		}

		public string NormalizeNumberWithMinusSign(string number)
		{
			try
			{
				var positionOfNormalMinus = number.IndexOf('-');
				var positionOfSpecialMinus = number.IndexOf('\u2212');
				var positionOfDash = number.IndexOf('\u2013');
				char[] space = { ' ', '\u00a0', '\u2009', '\u202F', '\u0020' };
				var spacePosition = number.IndexOfAny(space);

				//if it has space is not a negative number
				if (positionOfNormalMinus == 0 && spacePosition != 1)
				{
					number = number.Replace("-", "m");
				}
				if (positionOfSpecialMinus == 0 && spacePosition != 1)
				{
					number = number.Replace("\u2212", "m");
				}
				if (positionOfDash == 0 && spacePosition != 1)
				{
					number = number.Replace("\u2013", "m");
				}
				if (positionOfSpecialMinus == 1 && spacePosition == 0)
				{
					number = number.Replace("\u2212", "m");
				}
				return number.Normalize(NormalizationForm.FormKC);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
				return string.Empty;
			}
		}

		private bool FilterSegmentPairs(ISegmentPair segmentPair)
		{
			return (VerificationSettings.ExcludeLockedSegments == false ||
					segmentPair.Properties.IsLocked == false) &&
				   (VerificationSettings.Exclude100Percents == false ||
					((segmentPair.Properties.TranslationOrigin.OriginType != "auto-propagated" &&
					  segmentPair.Properties.TranslationOrigin.OriginType != "tm") ||
					 segmentPair.Properties.TranslationOrigin.MatchPercent != 100))
					 && !(VerificationSettings.ExcludeUntranslatedSegments == true && segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Unspecified)
					 && !(VerificationSettings.ExcludeDraftSegments == true && segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Draft);
		}

		private string GetSegmentText(ISegment segment)
		{
			try
			{
				return VerificationSettings.ExcludeTagText == false ? segment.ToString() : TextGenerator.GetPlainText(segment, false);
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
				return string.Empty;
			}
		}

		// Remove the matching items which are the same in both sourceAlphanumeric and targetAlphanumeric lists
		// or in case sourceAlphanumericsList contains items which are not alphanumerics (ex: ABCD)
		private void RemoveMatchingAlphanumerics(IList<string> sourceAlphanumericsList, ICollection<string> targetAlphanumericsList)
		{
			for (var nJ = sourceAlphanumericsList.Count - 1; nJ >= 0; nJ--)
			{
				if (targetAlphanumericsList.Contains(sourceAlphanumericsList[nJ]) || !sourceAlphanumericsList[nJ].Any(char.IsDigit))
				{
					targetAlphanumericsList.Remove(sourceAlphanumericsList[nJ]);
					sourceAlphanumericsList.RemoveAt(nJ);
				}
			}

			// Remove the items which are not alphanumerics and where not identified within the sourceAlphanumericsList
			RemoveNoAlphanumerics(targetAlphanumericsList);
		}

		// Remove the items which are not alphanumerics
		private void RemoveNoAlphanumerics(ICollection<string> targetAlphanumericsList)
		{
			var itemsToRemove = targetAlphanumericsList.Where(item => !item.Any(char.IsDigit) || item.All(char.IsDigit)).ToList();
			if (itemsToRemove.Any())
			{
				foreach (var item in itemsToRemove.Where(targetAlphanumericsList.Contains))
				{
					targetAlphanumericsList.Remove(item);
				}
			}
		}

		private void RemoveNonAlphanumericals(bool isSource, List<string> normalizedAlphaList)
		{
			var thoAndDecSeparators = new List<string>();
			if (isSource)
			{
				thoAndDecSeparators.AddRange(VerificationSettings.GetSourceThousandSeparators());
				thoAndDecSeparators.AddRange(VerificationSettings.GetSourceDecimalSeparators());
			}
			else
			{
				thoAndDecSeparators.AddRange(VerificationSettings.GetTargetThousandSeparators());
				thoAndDecSeparators.AddRange(VerificationSettings.GetTargetDecimalSeparators());
			}
			thoAndDecSeparators.RemoveAll(string.IsNullOrEmpty);

			var forRemoval = new List<string>();
			foreach (var item in normalizedAlphaList)
			{
				var itemWoSeparators = item;

				thoAndDecSeparators.ForEach(sep => itemWoSeparators = itemWoSeparators.Replace(sep, ""));

				var unitsOfMeasurement = "(^(?![A-Za-z]))\\d+[a-z]+$";
				if (Regex.Match(itemWoSeparators, unitsOfMeasurement).Success || int.TryParse(itemWoSeparators, out _)) forRemoval.Add(item);
			}
			forRemoval.ForEach(item => normalizedAlphaList.Remove(item));
		}
	}
}