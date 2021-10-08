using System;
using System.Collections;
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
using Sdl.Community.NumberVerifier.Parsers.Number;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Verification.Api;

namespace Sdl.Community.NumberVerifier
{
	/// <summary>
	/// Required annotation for declaring the extension class.
	/// </summary>
	#region "Declaration"
	[GlobalVerifier("Number Verifier", "Plugin_Name", "Plugin_Description")]
	#endregion
	public class NumberVerifierMain : IGlobalVerifier, IBilingualVerifier, ISharedObjectsAware
	{
		#region "PrivateMembers"
		private ISharedObjects _sharedObjects;

		private bool? _enabled;
		private INumberVerifierSettings _verificationSettings;
		private readonly TextFormatter _textFormatter;
		private string _targetText;
		private string _sourceText;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		#endregion

		public NumberVerifierMain() : this(null)
		{
			if (_textFormatter == null)
			{
				_textFormatter = new TextFormatter();
			}

			
		}

		public NumberVerifierMain(INumberVerifierSettings numberVerifierSettings)
		{
			_verificationSettings = numberVerifierSettings;

			if (_textFormatter == null)
			{
				_textFormatter = new TextFormatter();
			}

			_numberNormalizer = new NumberNormalizer();
		}

		private static ProjectsController GetProjectController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
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
		#region Settings Bundle
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
		#endregion

		/// <summary>
		/// Creates the text generator, which traverses the elements of the current
		/// segment (e.g. text and tags). In our implementation the Text Generator
		/// is used to retrieve only plain text information from the current segment,
		/// if tags should not be considered for the verification.
		/// </summary>
		#region "text generator"
		private TextGenerator _textGenerator;

		public TextGenerator TextGenerator
		{
			get { return _textGenerator ?? (_textGenerator = new TextGenerator()); }
		}
		#endregion


		#region "ISharedObjectsAware Members"
		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}
		#endregion

		#region Members of IGlobalVerifier
		/// <summary>
		/// The following members set some general properties of the verification plug-in,
		/// e.g. the plug-in name and the icon that are displayed in the user interface of SDL Trados Studio. 
		/// </summary>
		#region "DescriptionNameIcon"
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
		#endregion

		public IList<string> GetSettingsPageExtensionIds()
		{
			IList<string> list = new List<string>();

			list.Add("Number Settings Definition ID");
			list.Add("Number Verifier Help Definition ID");

			return list;
		}

		public string SettingsId
		{
			get { return "Number Verifier"; }
		}

		public string HelpTopic
		{
			get { return string.Empty; }
		}

		public Type SettingsType
		{
			get { return typeof(NumberVerifierSettings); }
		}
		#endregion


		#region IBilingualFilterComponent Members
		#region "ItemFactory"
		public IDocumentItemFactory ItemFactory
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// This member is used to output any verification messages in the user interface of SDL Trados Studio.
		/// </summary>
		#region "MessageReporter"
		public IBilingualContentMessageReporter MessageReporter
		{
			get;
			set;
		}
		#endregion
		#endregion



		#region IBilingualContentHandler Members
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

		private string _sourceMatchingThousandSeparators = string.Empty;
		private string _targetMatchingThousandSeparators = string.Empty;
		private string _sourceMatchingDecimalSeparators = string.Empty;
		private string _targetMatchingDecimalSeparators = string.Empty;

		private bool _isSource;
		private readonly NumberNormalizer _numberNormalizer;

		public void Initialize(IDocumentProperties documentInfo)
		{
			_sourceMatchingThousandSeparators = string.Concat(VerificationSettings.GetSourceThousandSeparators());
			_targetMatchingThousandSeparators = string.Concat(VerificationSettings.GetTargetThousandSeparators());
			_sourceMatchingDecimalSeparators = string.Concat(VerificationSettings.GetSourceDecimalSeparators());
			_targetMatchingDecimalSeparators = string.Concat(VerificationSettings.GetTargetDecimalSeparators());

		}

		#endregion

		#region "process"

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (Enabled)
			{
				// Apply the verification logic.
				CheckParagraphUnit(paragraphUnit);
			}
		}

		#endregion

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
		/// and in the Messages window of SDL Trados Studio.
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

					var errorMessages = CheckSourceAndTarget(_sourceText, _targetText);

					// generic number verifier to identify errors related to the numeric convention taking
					// into consideration the settings applied.
					var genericErrorMeassages = GenericNumberVerifier.Verify(segmentPair);
					if (genericErrorMeassages.Any())
					{
						errorMessages.AddRange(genericErrorMeassages);
					}
					
					#region ReportingMessage

					foreach (var errorMessage in errorMessages)
					{
						if (errorMessage.ExtendedErrorMessage != string.Empty && VerificationSettings.ReportExtendedMessages)
						{
							if (MessageReporter is IBilingualContentMessageReporterWithExtendedData extendedMessageReporter)
							{
								#region CreateExtendedData
								
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

								#endregion

								#region ReportingMessageWithExtendedData

								extendedMessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
									errorMessage.ErrorLevel, errorMessage.ExtendedErrorMessage,
									new TextLocation(new Location(segmentPair.Target, true), 0),
									new TextLocation(new Location(segmentPair.Target, false),
										segmentPair.Target.ToString().Length - 1),
									extendedData);

								#endregion

							}
						}
						else if (errorMessage.ErrorMessage != string.Empty)
						{
							#region ReportingMessageWithoutExtendedData
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
											var alphanumericTarget = string.Format(@"""{0}""", alphaElement);
											alphaTargetList.Add(alphanumericTarget);
										}
										var alphanumericRes = string.Join(", ", alphaTargetList.ToArray());
										errorMessage.ErrorMessage = string.Concat(errorMessage.ErrorMessage, " (", alphanumericRes, ")");
									}
								}

								else
								{
									var targetNumbers = new List<string>();
									var numbers = Regex.Matches(errorMessage.TargetNumberIssues, @"[\+\-]?\s*[0-9\.\,]*[Ee]?[\+\-]?\d+", RegexOptions.Singleline);

									foreach (var value in numbers)
									{
										var targetNumber = string.Format(@"""{0}""", value);
										targetNumbers.Add(targetNumber);
									}
									var res = string.Join(", ", targetNumbers.ToArray());

									errorMessage.ErrorMessage = string.Concat(errorMessage.ErrorMessage, " (", res, ")");
								}
							}

							MessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
								errorMessage.ErrorLevel, errorMessage.ErrorMessage,
								new TextLocation(new Location(segmentPair.Target, true), 0),
								new TextLocation(new Location(segmentPair.Target, false),
									segmentPair.Target.ToString().Length - 1));

							#endregion
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		/// <summary>
		/// Returns a list of errors after checking the alphanumerics
		/// e.g: AB12
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <returns></returns>
		public IEnumerable<ErrorReporting> CheckAlphanumerics(string sourceText, string targetText)
		{
			try
			{
				var sourceAlphanumericsList = GetAlphanumericList(sourceText);

				// find all alphanumeric names in target and add to list
				var targetAlphanumericsList = GetAlphanumericList(targetText);

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

		public Tuple<List<string>, List<string>> GetAlphnumericsTuple(List<string> alphaNumericsList, List<string> normalizedAlphaNumericsList)
		{
			return Tuple.Create(alphaNumericsList, normalizedAlphaNumericsList);
		}

		/// <summary>
		/// Returns a errors list after numbers are normalized
		/// </summary>
		/// <param name="sourceLanguage"></param>
		/// <param name="targetLanguage"></param>
		/// <param name="numberModel"></param>
		/// <returns></returns>
		public IEnumerable<ErrorReporting> CheckNumbers(string sourceLanguage, string targetLanguage, NumberModel numberModel)
		{
			var initialSourceHindiText = numberModel.SourceText;
			var initialTargetHindiText = numberModel.TargetText;
			var isHindiVerification = false;
			if (targetLanguage.Equals(Constants.HindiLanguage))
			{
				numberModel.SourceText = numberModel.SourceArabicText;
				numberModel.TargetText = numberModel.TargetArabicText;
				isHindiVerification = true;
			}
			if (sourceLanguage.Equals(Constants.HindiLanguage))
			{
				numberModel.SourceText = numberModel.SourceArabicText;
				isHindiVerification = true;
			}

			_numberNormalizer.GetNormalizedNumbers(numberModel.SourceText, numberModel.TargetText, VerificationSettings, out var sourceNumbersNormalized, out var targetNumbersNormalized);
			var sourceNumberList = sourceNumbersNormalized?.InitialPartsList;
			var sourceNormalizedNumberList = sourceNumbersNormalized?.NormalizedPartsList;

			var targetNumberList = targetNumbersNormalized?.InitialPartsList;
			var targetNormalizedNumberList = targetNumbersNormalized?.NormalizedPartsList;

			// remove identical numbers found both in source and target from respective list
			RemoveIdenticalNumbers(sourceNumberList, targetNumberList, targetNormalizedNumberList, sourceNormalizedNumberList);

			// remove numbers found both in source and target from respective list disregarding difference in thousands and decimal separators
			RemoveNumbersIgnoreThousandsAndDecimalSeparators(sourceNumberList, targetNormalizedNumberList, sourceNormalizedNumberList, targetNumberList);

			// remove numbers found both in source and target from respective list disregarding difference when thousands and decimal separators are undefined due to ambiguity 
			RemoveNumbersUndefinedThousandsAndDecimalSeparator(targetNumberList, sourceNumberList, sourceNormalizedNumberList, targetNormalizedNumberList);

			var sourceHindiList = initialSourceHindiText.Equals(numberModel.SourceText) ? new List<string> { numberModel.SourceText } : new List<string> { initialSourceHindiText };
			var targetHindiList = initialTargetHindiText.Equals(numberModel.TargetText) ? new List<string> { numberModel.TargetText } : new List<string> { initialTargetHindiText };

			var numberModelRes = new NumberModel
			{
				Settings = VerificationSettings,
				SourceNumbers = sourceNumberList,
				TargetNumbers = targetNumberList,
				InitialSourceNumbers = sourceHindiList,
				InitialTargetNumbers = targetHindiList,
				SourceText = !string.IsNullOrEmpty(numberModel.SourceArabicText) ? numberModel.SourceArabicText : numberModel.SourceText,
				TargetText = !string.IsNullOrEmpty(numberModel.TargetArabicText) ? numberModel.TargetArabicText : numberModel.TargetText,
				IsHindiVerification = isHindiVerification
			};

			var numberResults = new NumberResults(numberModelRes);
			var numberErrorComposer = new NumberErrorComposer();
			var verifyProcessor = numberErrorComposer.Compose();

			return verifyProcessor.Verify(numberResults);
		}

		/// <summary>
		/// Returns a list of errors which contains combined errors from alphanumerics check and numbers check
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <returns></returns>
		public List<ErrorReporting> CheckSourceAndTarget(string sourceText, string targetText)
		{
			var errorList = new List<ErrorReporting>();
			var errorsListFromNormalizedNumbers = Enumerable.Empty<ErrorReporting>();
			var numberModel = new NumberModel
			{
				SourceText = sourceText,
				TargetText = targetText
			};


			if (_verificationSettings.CustomsSeparatorsAlphanumerics || _verificationSettings.ReportModifiedAlphanumerics)
			{
				var errorsListFromAlphanumerics = CheckAlphanumerics(sourceText, targetText);
				errorList.AddRange(errorsListFromAlphanumerics);
			}

			if (_verificationSettings.HindiNumberVerification)
			{
				var projectController = GetProjectController();
				if (projectController.CurrentProject != null)
				{
					var projectInfo = projectController.CurrentProject.GetProjectInfo();
					var sourceLanguage = projectInfo.SourceLanguage.DisplayName;
					if (sourceLanguage.Equals(Constants.HindiLanguage) || projectInfo.TargetLanguages.Any(l => l.DisplayName.Equals(Constants.HindiLanguage)))
					{
						var result = GetTargetFromHindiNumbers(sourceText, targetText, sourceLanguage);
						var targetLanguage = projectInfo.TargetLanguages.FirstOrDefault(l => l.DisplayName.Equals(Constants.HindiLanguage));
						var targetLanguageName = targetLanguage != null ? targetLanguage.DisplayName : string.Empty;
						foreach (var targetRes in result)
						{
							errorsListFromNormalizedNumbers = CheckNumbers(sourceLanguage, targetLanguageName, targetRes);
							errorList.AddRange(errorsListFromNormalizedNumbers);
						}
						return errorList;
					}
					return ReturnErrorList(errorsListFromNormalizedNumbers, errorList, numberModel);
				}
			}
			else
			{
				return ReturnErrorList(errorsListFromNormalizedNumbers, errorList, numberModel);
			}
			return errorList;
		}

		private List<ErrorReporting> ReturnErrorList(
			IEnumerable<ErrorReporting> errorsListFromNormalizedNumbers,
			List<ErrorReporting> errorList,
			NumberModel numberModel)
		{
			errorsListFromNormalizedNumbers = CheckNumbers(string.Empty, string.Empty, numberModel);
			errorList.AddRange(errorsListFromNormalizedNumbers);
			return errorList;
		}

		private void RemoveNumbersUndefinedThousandsAndDecimalSeparator(IList targetNumberList, IList sourceNumberList,
			IList<string> sourceNormalizedNumberList, IList<string> targetNormalizedNumberList)
		{
			try
			{
				if (VerificationSettings.AllowLocalizations || VerificationSettings.RequireLocalizations)
				{
					if (targetNumberList.Count > 0 && sourceNumberList.Count > 0)
					{
						int nJ;
						for (nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
						{
							if (sourceNormalizedNumberList[nJ].IndexOf("u", StringComparison.InvariantCultureIgnoreCase) > 0 &&
								targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ].Replace("u", "d")))
							{
								targetNumberList.RemoveAt(
									targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "d")));
								targetNormalizedNumberList.RemoveAt(
									targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "d")));
								sourceNormalizedNumberList.RemoveAt(nJ);
								sourceNumberList.RemoveAt(nJ);
							}
							else if (sourceNormalizedNumberList[nJ].IndexOf("u", StringComparison.InvariantCultureIgnoreCase) > 0 &&
									 targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ].Replace("u", "t")))
							{
								targetNumberList.RemoveAt(
									targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "t")));
								targetNormalizedNumberList.RemoveAt(
									targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "t")));
								sourceNormalizedNumberList.RemoveAt(nJ);
								sourceNumberList.RemoveAt(nJ);
							}
						}
					}

					if (targetNumberList.Count > 0 && sourceNumberList.Count > 0)
					{
						int nJ;
						for (nJ = targetNumberList.Count - 1; nJ >= 0; nJ--)
						{
							if (targetNormalizedNumberList[nJ].IndexOf("u", StringComparison.InvariantCultureIgnoreCase) > 0 &&
								sourceNormalizedNumberList.Contains(targetNormalizedNumberList[nJ].Replace("u", "d")))
							{
								sourceNumberList.RemoveAt(
									sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "d")));
								sourceNormalizedNumberList.RemoveAt(
									sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "d")));
								targetNormalizedNumberList.RemoveAt(nJ);
								targetNumberList.RemoveAt(nJ);
							}
							else if (targetNormalizedNumberList[nJ].IndexOf("u", StringComparison.InvariantCultureIgnoreCase) > 0 &&
									 sourceNormalizedNumberList.Contains(targetNormalizedNumberList[nJ].Replace("u", "t")))
							{
								sourceNumberList.RemoveAt(
									sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "t")));
								sourceNormalizedNumberList.RemoveAt(
									sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "t")));
								targetNormalizedNumberList.RemoveAt(nJ);
								targetNumberList.RemoveAt(nJ);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		private void RemoveNumbersIgnoreThousandsAndDecimalSeparators(IList sourceNumberList, IList<string> targetNormalizedNumberList,
			IList<string> sourceNormalizedNumberList, IList targetNumberList)
		{
			try
			{
				if (VerificationSettings.AllowLocalizations || VerificationSettings.RequireLocalizations)
				{
					int sourceNumberIndex;
					for (sourceNumberIndex = sourceNumberList.Count - 1; sourceNumberIndex >= 0; sourceNumberIndex--)
					{
						var currentNormalizedSourceNumber = sourceNormalizedNumberList[sourceNumberIndex];

						if (!targetNormalizedNumberList.Contains(currentNormalizedSourceNumber)) continue;
						var indexOfSourceNumberInTarget = targetNormalizedNumberList.IndexOf(currentNormalizedSourceNumber);

						targetNumberList.RemoveAt(indexOfSourceNumberInTarget);
						targetNormalizedNumberList.RemoveAt(indexOfSourceNumberInTarget);

						sourceNormalizedNumberList.RemoveAt(sourceNumberIndex);
						sourceNumberList.RemoveAt(sourceNumberIndex);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		private void RemoveIdenticalNumbers(IList<string> sourceNumberList, IList<string> targetNumberList, List<string> targetNormalizedNumberList,
			IList sourceNormalizedNumberList)
		{
			try
			{
				if (targetNormalizedNumberList == null) throw new ArgumentNullException("targetNormalizedNumberList");
				if (VerificationSettings.PreventLocalizations || VerificationSettings.AllowLocalizations)
				{
					for (var nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
					{
						if (!targetNumberList.Contains(sourceNumberList[nJ])) continue;
						targetNormalizedNumberList.RemoveAt(targetNumberList.IndexOf(sourceNumberList[nJ]));
						targetNumberList.RemoveAt(targetNumberList.IndexOf(sourceNumberList[nJ]));
						sourceNormalizedNumberList.RemoveAt(nJ);
						sourceNumberList.RemoveAt(nJ);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
		}

		//For more information see: https://www.cl.cam.ac.uk/~mgk25/ucs/quotes.html
		public string AddCustomSeparators(string selectedSeparators, bool isDecimalSeparator)
		{
			var separatorsList = new List<string>();
			var selectedSep = string.Empty;
			var separators = new StringBuilder();
			try
			{
				// you can use in target as separators source separators or selected target separators
				if (_verificationSettings.AllowLocalizations)
				{
					selectedSep = isDecimalSeparator ? _sourceMatchingDecimalSeparators : _sourceMatchingThousandSeparators;
					selectedSep = isDecimalSeparator ? $"{selectedSep}{_targetMatchingDecimalSeparators}" : $"{selectedSep}{_targetMatchingThousandSeparators}";
				}

				// you can use only source separators selected
				if (_verificationSettings.PreventLocalizations)
				{
					selectedSep = isDecimalSeparator ? _sourceMatchingDecimalSeparators : _sourceMatchingThousandSeparators;
				}

				if (_verificationSettings.RequireLocalizations)
				{
					selectedSep = selectedSeparators;
				}

				// Composition (ApostrophCompositionProcessor)
				if (selectedSeparators.Contains("'"))
				{
					selectedSep = string.Concat(selectedSeparators, @"\u2019\u0027");
				}

				// get a list of source separators if we are in case of allow localization, or prevent localization
				// Composition UniqueSeparatorComposition
				if (!string.IsNullOrEmpty(selectedSep))
				{
					var sepSource = selectedSep.Split('\\').ToList();

					// add the separator to list only if that separator does not exists
					foreach (var separator in sepSource)
					{
						if (!separatorsList.Contains(@"\" + separator.ToLower()) && !string.IsNullOrEmpty(separator))
						{
							separatorsList.Add(@"\" + separator.ToLower());
						}
					}
				}

				// returns final string of separators used
				foreach (var sep in separatorsList)
				{
					separators.Append(sep);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{MethodBase.GetCurrentMethod().Name} \n {ex}");
			}
			return separators.ToString();
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

		// Remove the matching items which are the same in both sourceAlphanumeric and targetAlphanumeric lists
		// or in case sourceAlphanumericsList contains items which are not alphanumerics (ex: ABCD) 
		private void RemoveMatchingAlphanumerics(IList<string> sourceAlphanumericsList, ICollection<string> targetAlphanumericsList)
		{
			for (var nJ = sourceAlphanumericsList.Count - 1; nJ >= 0; nJ--)
			{
				if (targetAlphanumericsList.Contains(sourceAlphanumericsList[nJ]) || !sourceAlphanumericsList[nJ].Any(c => char.IsDigit(c)))
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

		public Tuple<List<string>, List<string>> GetAlphanumericList(string text)
		{
			try
			{
				var normalizedAlphaList = new List<string>();
				var words = Regex.Split(text, @"\s");
				var customsSeparators = !string.IsNullOrEmpty(_verificationSettings.AlphanumericsCustomSeparator)
					? _verificationSettings.AlphanumericsCustomSeparator.Split(',')
					: new string[0];

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
					from word in wordsRes
					from Match match in Regex.Matches(word.Normalize(NormalizationForm.FormKC), regex)
					select Regex.Replace(match.Value, "\u2212|-", "m"));

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

		public List<NumberModel> GetTargetFromHindiNumbers(string source, string target, string sourceLanguage)
		{
			var result = new List<NumberModel>();
			try
			{
				var sb = new StringBuilder();
				var hindiNumbers = GetHindiNumbers();
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
							sourceResult = hindiNumbers.ContainsValue(s.ToString())
								? sb.Append(hindiNumbers.FirstOrDefault(h => h.Value == s.ToString()).Key).ToString()
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
							targetResult = hindiNumbers.ContainsValue(t.ToString())
								? sb.Append(hindiNumbers.FirstOrDefault(h => h.Value == t.ToString()).Key).ToString()
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

		public Dictionary<string, string> GetHindiNumbers()
		{
			var hindiDictionary = new Dictionary<string, string>();
			hindiDictionary.Add("0", "٠");
			hindiDictionary.Add("1", "١");
			hindiDictionary.Add("2", "٢");
			hindiDictionary.Add("3", "٣");
			hindiDictionary.Add("4", "٤");
			hindiDictionary.Add("5", "٥");
			hindiDictionary.Add("6", "٦");
			hindiDictionary.Add("7", "٧");
			hindiDictionary.Add("8", "٨");
			hindiDictionary.Add("9", "٩");

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
						var sourceText = hindiNumberModel.HindiDictionary.Where(s => s.Key.Equals(numberRes.SourceText)).FirstOrDefault();
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
						var sourceText = hindiNumberModel.HindiDictionary.Where(s => s.Key.Contains(numberRes.SourceText)).FirstOrDefault();
						var targetText = hindiNumberModel.TargetDictionary.Where(t => t.Key.Contains(sourceText.Value)).FirstOrDefault();
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
		#endregion
	}
}