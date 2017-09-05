using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Sdl.Community.Extended.MessageUI;
using Sdl.Community.NumberVerifier.Composers;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Core.Globalization;
using Sdl.Core.PluginFramework;
using Sdl.Community.NumberVerifier.Processors;
using Sdl.Community.NumberVerifier.Specifications;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.NumberVerifier
{
	/// <summary>
	/// Required annotation for declaring the extension class.
	/// </summary>
	#region "Declaration"
	[GlobalVerifier("Number Verifier", "Plugin_Name", "Plugin_Description")]
	#endregion
	public class NumberVerifierMain : IGlobalVerifier, IBilingualVerifier, ISharedObjectsAware, INumberVerifierMethods
	{
		#region "PrivateMembers"
		private ISharedObjects _sharedObjects;

		private bool? _enabled;
		private bool _omitLeadingZero;
		private INumberVerifierSettings _verificationSettings;
		private string _language;

		#endregion

		public NumberVerifierMain() : this(null)
		{


		}

		public NumberVerifierMain(INumberVerifierSettings numberVerifierSettings)
		{
			_verificationSettings = numberVerifierSettings;
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
		internal INumberVerifierSettings VerificationSettings
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
		private TextGenerator _textGeneratorProcessor;

		public TextGenerator TextGeneratorProcessor
		{
			get { return _textGeneratorProcessor ?? (_textGeneratorProcessor = new TextGenerator()); }
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
			get { return String.Empty; }
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
		private string _sourceThousandSeparators = string.Empty;
		private string _sourceDecimalSeparators = string.Empty;
		private string _targetThousandSeparators = string.Empty;
		private string _targetDecimalSeparators = string.Empty;
		private string _alphanumericsCustomSeparators = string.Empty;

		private bool _isSource;

		public void Initialize(IDocumentProperties documentInfo)
		{
			_sourceMatchingThousandSeparators = string.Concat(VerificationSettings.GetSourceThousandSeparators());
			_targetMatchingThousandSeparators = string.Concat(VerificationSettings.GetTargetThousandSeparators());
			_sourceMatchingDecimalSeparators = string.Concat(VerificationSettings.GetSourceDecimalSeparators());
			_targetMatchingDecimalSeparators = string.Concat(VerificationSettings.GetTargetDecimalSeparators());

			_targetMatchingDecimalSeparators += VerificationSettings.TargetDecimalComma ? @"\u002C" : string.Empty;
			_targetMatchingDecimalSeparators += VerificationSettings.TargetDecimalPeriod ? @"\u002E" : string.Empty;
			_targetMatchingDecimalSeparators += VerificationSettings.TargetDecimalCustomSeparator
				? VerificationSettings.GetTargetDecimalCustomSeparator
				: string.Empty;


			//used in NoSeparator method, we need the character chosed not the code.
			_sourceThousandSeparators += VerificationSettings.SourceThousandsSpace ? " " : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakSpace ? " " : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsThinSpace ? " " : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakThinSpace ? " " : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsComma ? "," : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsPeriod ? "." : string.Empty;
			_sourceThousandSeparators += VerificationSettings.SourceThousandsCustomSeparator
				? VerificationSettings.GetSourceThousandsCustomSeparator
				: string.Empty;

			_sourceDecimalSeparators += VerificationSettings.SourceDecimalComma ? "," : string.Empty;
			_sourceDecimalSeparators += VerificationSettings.SourceDecimalPeriod ? "." : string.Empty;
			_sourceDecimalSeparators += VerificationSettings.SourceDecimalCustomSeparator
				? VerificationSettings.GetSourceDecimalCustomSeparator
				: string.Empty;

			_targetThousandSeparators += VerificationSettings.TargetThousandsSpace ? " " : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsNobreakSpace ? " " : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsThinSpace ? " " : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsNobreakThinSpace ? " " : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsComma ? "," : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsPeriod ? "." : string.Empty;
			_targetThousandSeparators += VerificationSettings.TargetThousandsCustomSeparator
				? VerificationSettings.GetTargetThousandsCustomSeparator
				: string.Empty;
			_targetDecimalSeparators += VerificationSettings.TargetDecimalComma ? "," : string.Empty;
			_targetDecimalSeparators += VerificationSettings.TargetDecimalPeriod ? "." : string.Empty;
			_targetDecimalSeparators += VerificationSettings.TargetDecimalCustomSeparator
				? VerificationSettings.GetSourceDecimalCustomSeparator
				: string.Empty;
			_alphanumericsCustomSeparators += VerificationSettings.CustomsSeparatorsAlphanumerics
			   ? VerificationSettings.GetAlphanumericsCustomSeparator
			   : string.Empty;
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

			// loop through the whole paragraph unit
			foreach (var segmentPair in paragraphUnit.SegmentPairs.Where(FilterSegmentPairs))
			{
				var sourceText = GetSegmentText(segmentPair.Source);
				var targetText = GetSegmentText(segmentPair.Target);

				var errorMessageList = CheckSourceAndTarget(sourceText, targetText);

				#region ReportingMessage

				foreach (var errorMessage in errorMessageList)
				{
					if (errorMessage.ExtendedErrorMessage != string.Empty && VerificationSettings.ReportExtendedMessages)
					{
						var extendedMessageReporter =
							MessageReporter as IBilingualContentMessageReporterWithExtendedData;
						if (extendedMessageReporter != null)
						{
							#region CreateExtendedData

							var extendedData = new NumberVerifierMessageData(errorMessage.SourceNumberIssues,
								errorMessage.TargetNumberIssues,
								segmentPair.Target);

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
						if(!string.IsNullOrEmpty(errorMessage.TargetNumberIssues))
						{
							List<string> targetNumbers = new List<string>();
							var numbers = Regex.Matches(errorMessage.TargetNumberIssues, @"-?[0-9]+\.?[0-9,]*");

							foreach (var value in numbers)
							{
								var targetNumber = string.Format(@"""{0}""", value.ToString());
								targetNumbers.Add(targetNumber);
							}
							var res = string.Join(", ", targetNumbers.ToArray());

							errorMessage.ErrorMessage = string.Concat(errorMessage.ErrorMessage, " (", res, ")");
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


		/// <summary>
		/// Returns a list of errors after checking the alphanumerics
		/// e.g: AB12
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <returns></returns>
		public IEnumerable<ErrorReporting> CheckAlphanumerics(string sourceText, string targetText)
		{
			var sourceAlphanumericsList = GetAlphanumericList(sourceText);

			// find all alphanumeric names in target and add to list
			var targetAlphanumericsList = GetAlphanumericList(targetText);

			// remove alphanumeric names found both in source and target from respective list
			RemoveMatchingAlphanumerics(sourceAlphanumericsList, targetAlphanumericsList);

			var numberResults = new NumberResults(VerificationSettings,
				sourceAlphanumericsList,
				targetAlphanumericsList);

			if (numberResults.SourceNumbers.Any())
			{
				numberResults.SourceNumbers[0] = sourceText;
			}
			if (numberResults.TargetNumbers.Any())
			{
				numberResults.TargetNumbers[0] = targetText;
			}

			var alphanumericErrorComposer = new AlphanumericErrorComposer();
			var verifyProcessor = alphanumericErrorComposer.Compose();

			return verifyProcessor.Verify(numberResults);
		}

		/// <summary>
		/// Calls the Normalize method and returns a list of numbers found in segment and anotherone with  normalized numbers.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="decimalSeparators"></param>
		/// <param name="thousandSeparators"></param>
		/// <param name="noSeparator"></param>
		/// <param name="omitZero"></param>
		/// <returns></returns>
		public Tuple<List<string>, List<string>> GetNumbersTuple(string text, string decimalSeparators,
			string thousandSeparators,
			bool noSeparator, bool omitZero)
		{
			var numberList = new List<string>();
			var normalizedNumberList = new List<string>();

			//call normalize method with source settings
			NormalizeAlphanumerics(text, numberList, normalizedNumberList,
				thousandSeparators, decimalSeparators, noSeparator, omitZero);

			var tulpleList = Tuple.Create(numberList, normalizedNumberList);

			return tulpleList;
		}

		/// <summary>
		/// Returns a errors list after numbers are normalized
		/// </summary>
		/// <param name="sourceText"></param>
		/// <param name="targetText"></param>
		/// <returns></returns>
		public IEnumerable<ErrorReporting> CheckNumbers(string sourceText, string targetText)
		{
			var sourceDecimalExtractComposer = new SourceDecimalSeparatorsExtractComposer().Compose();
			var sourceThousandsExtractComposer = new SourceThousandSeparatorsExtractComposer().Compose();

			var sourceDecimalSeparators = sourceDecimalExtractComposer.Extract(new ExtractData(VerificationSettings, new[] { sourceText }));
			var sourceThousandSeparators = sourceThousandsExtractComposer.Extract(new ExtractData(VerificationSettings, new[] { sourceText }));

			var sourceList = GetNumbersTuple(sourceText, string.Concat(sourceDecimalSeparators),
				string.Concat(sourceThousandSeparators), VerificationSettings.SourceNoSeparator,
				VerificationSettings.SourceOmitLeadingZero);

			var targetDecimalExtractComposer = new TargetDecimalSeparatorsExtractComposer().Compose();
			var targetThousandsExtractComposer = new TargetThousandSeparatorsExtractComposer().Compose();

			var targetDecimalSeparators = targetDecimalExtractComposer.Extract(new ExtractData(VerificationSettings, new[] { targetText }));
			var targetThousandSeparators = targetThousandsExtractComposer.Extract(new ExtractData(VerificationSettings, new[] { targetText }));

			var targetList = GetNumbersTuple(targetText, string.Concat(targetDecimalSeparators),
				string.Concat(targetThousandSeparators), VerificationSettings.TargetNoSeparator,
				VerificationSettings.TargetOmitLeadingZero);

			var sourceNumberList = sourceList.Item1;
			var sourceNormalizedNumberList = sourceList.Item2;

			var targetNumberList = targetList.Item1;
			var targetNormalizedNumberList = targetList.Item2;

			// remove identical numbers found both in source and target from respective list
			RemoveIdenticalNumbers(sourceNumberList, targetNumberList, targetNormalizedNumberList,
				sourceNormalizedNumberList);

			// remove numbers found both in source and target from respective list disregarding difference in thousands and decimal separators
			RemoveNumbersIgnoreThousandsAndDecimalSeparators(sourceNumberList, targetNormalizedNumberList,
				sourceNormalizedNumberList, targetNumberList);

			// remove numbers found both in source and target from respective list disregarding difference when thousands and decimal separators are undefined due to ambiguity 
			RemoveNumbersUndefinedThousandsAndDecimalSeparator(targetNumberList, sourceNumberList,
				sourceNormalizedNumberList, targetNormalizedNumberList);


			var numberResults = new NumberResults(VerificationSettings,
				sourceNumberList,
				targetNumberList, sourceText, targetText);
			
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
			var errorListAlphanumericsResult = new List<ErrorReporting>();
			var hindiNumbers = GetHindiNumbers();
			var hindiVerificationList = new List<string>();

			IEnumerable<ErrorReporting> errorsListFromNormalizedNumbers = Enumerable.Empty<ErrorReporting>();
			var errorsListFromAlphanumerics = CheckAlphanumerics(sourceText, targetText);

			if (_verificationSettings.HindiNumberVerification)
			{
				var _projectController = GetProjectController();
				if(_projectController.CurrentProject != null)
				{
					var projectInfo = _projectController.CurrentProject.GetProjectInfo();
					var sourceLanguage = projectInfo.SourceLanguage.DisplayName;
					if(sourceLanguage == "Hindi (India)" || projectInfo.TargetLanguages.Any(l=>l.DisplayName == "Hindi (India)"))
					{
						var result = GetTargetFromHindiNumbers(sourceText, targetText);

						foreach (var targetRes in result)
						{
							errorsListFromNormalizedNumbers = CheckNumbers(targetRes.SourceText, targetRes.TargetText);
							errorList.AddRange(errorsListFromNormalizedNumbers);
						}
					}
					else
					{
						errorsListFromNormalizedNumbers = CheckNumbers(sourceText, targetText);
					}
				}			
			}
			else
			{
				errorsListFromNormalizedNumbers = CheckNumbers(sourceText, targetText);
			}

			foreach (var error in errorsListFromAlphanumerics)
			{
				error.SourceNumberIssues = sourceText;
				error.TargetNumberIssues = targetText;	
				errorListAlphanumericsResult.Add(error);
			}			

			errorList.AddRange(errorListAlphanumericsResult);
			errorList.AddRange(errorsListFromNormalizedNumbers);

			return errorList;
		}

		private void RemoveNumbersUndefinedThousandsAndDecimalSeparator(IList targetNumberList, IList sourceNumberList,
			IList<string> sourceNormalizedNumberList, IList<string> targetNormalizedNumberList)
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

		private void RemoveNumbersIgnoreThousandsAndDecimalSeparators(IList sourceNumberList, IList<string> targetNormalizedNumberList,
			IList<string> sourceNormalizedNumberList, IList targetNumberList)
		{
			if (VerificationSettings.AllowLocalizations || VerificationSettings.RequireLocalizations)
			{
				int nJ;
				for (nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
				{
					if (!targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ])) continue;
					targetNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ]));
					targetNormalizedNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ]));
					sourceNormalizedNumberList.RemoveAt(nJ);
					sourceNumberList.RemoveAt(nJ);
				}
			}
		}

		private void RemoveIdenticalNumbers(IList<string> sourceNumberList, IList<string> targetNumberList, List<string> targetNormalizedNumberList,
			IList sourceNormalizedNumberList)
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

		//For more information see: https://www.cl.cam.ac.uk/~mgk25/ucs/quotes.html
		public string AddCustomSeparators(string selectedSeparators, bool isDecimalSeparator)
		{
			var expression = string.Empty;
			var separatorsList = new List<string>();
			var selectedSep = string.Empty;
			var separators = string.Empty;

			// you can use in target as separators source separators or selected target separators
			//1 specificicat-done
			if (_verificationSettings.AllowLocalizations)
			{

				if (isDecimalSeparator)
				{
					selectedSep = _sourceMatchingDecimalSeparators;//decimalAllowlocalizationProcessor
					selectedSep = selectedSep + _targetMatchingDecimalSeparators;
				}
				else
				{
					selectedSep = _sourceMatchingThousandSeparators;//thousandAlloProc
					selectedSep = selectedSep + _targetMatchingThousandSeparators;
				}
			}

			//1 specificicat
			//you can use only source separators selected
			if (_verificationSettings.PreventLocalizations)
			{
				if (isDecimalSeparator)
				{
					selectedSep = _sourceMatchingDecimalSeparators; //DecimalPrevProc
				}
				else
				{
					selectedSep = _sourceMatchingThousandSeparators;//ThouPrevProc
				}

			}

			//1 specificicat
			if (_verificationSettings.RequireLocalizations)
			{
				if (isDecimalSeparator)
				{
					selectedSep = selectedSeparators;//DecimalReqProc
													 //sourceDecimalProc
													 //tarDecimalProc
				}
				else
				{
					selectedSep = selectedSeparators;//sourceThProcess, targetThProc
				}
			}

			//Compozitie (ApostrophCompositionProcessor)
			if (selectedSeparators.Contains("'"))
			{
				selectedSep = string.Concat(selectedSeparators, @"\u2019\u0027");
			}

			//get a list of source separators if we are in case of allow localization, or prevent localization
			//Composition UniqueSeparatorComposition (are noduri)
			//primeste IEnumerable(aplic logica de unique)
			//trebuie com
			if (selectedSep != string.Empty)
			{
				var sepSource = selectedSep.Split('\\').ToList();

				//add the separator to list only if that separator does not exists
				foreach (var separator in sepSource)
				{
					if (!separatorsList.Contains(@"\" + separator.ToLower()) && !string.IsNullOrEmpty(separator))
					{
						separatorsList.Add(@"\" + separator.ToLower());
					}
				}
			}

			//returns final string of separators used
			foreach (var sep in separatorsList)
			{
				separators = separators + sep;
			}
			return separators;
		}

		public void NormalizeAlphanumerics(string text, ICollection<string> numeberCollection,
			ICollection<string> normalizedNumberCollection, string thousandSeparators, string decimalSeparators,
			bool noSeparator, bool omitLeadingZero)
		{

			var separators = string.Concat(thousandSeparators, decimalSeparators);
			//skip the "-" in case of: - 23 (dash, space, number)
			char[] dashSign = { '-', '\u2013', '\u2212' };
			char[] space = { ' ', '\u00a0', '\u2009', '\u202F' };
			var spacePosition = text.IndexOfAny(space);
			var dashPosition = text.IndexOfAny(dashSign);
			if (dashPosition == 0 && spacePosition == 1)
			{
				text = text.Substring(2);
			}

			#region Omit zero
			//if only "No separator" is selected "separators" variable will be a empty string
			string expresion = string.Empty;

			if (omitLeadingZero)
			{
				_omitLeadingZero = true;
				if (separators != string.Empty)
				{
					expresion = string.Format(@"-?\u2013?\u2212?\u002E?\u2013?\d+([{0}]\d+)*", separators);
				}
				else
				{
					expresion = string.Format(@"-?\u2013?\u2212?\u002E?\u2013?\d+(\d+)*");
				}
			}
			else
			{
				_omitLeadingZero = false;
				if (separators != string.Empty)
				{
					expresion = string.Format(@"-?\u2013?\u2212?\u2013?\d+([{0}]\d+)*", separators);

				}
				else
				{
					expresion = @"-?\u2013?\u2212?\u2013?\d+(\d+)*";
				}
			}
			#endregion
			foreach (Match match in Regex.Matches(text, expresion))
			{
				var normalizedNumber = NormalizedNumber(match.Value, thousandSeparators, decimalSeparators,
					noSeparator);

				numeberCollection.Add(match.Value);
				normalizedNumberCollection.Add(normalizedNumber);

			}

		}

		public string OmitZero(string number)
		{
			number = NormalizeNumberWithMinusSign(number);
			if (number.IndexOf('m') == 0 && number.IndexOf('.') == 1 || number.IndexOf(',') == 1)
			{
				var aux = number.Substring(1);
				number = string.Concat('m', "0", aux);
			}
			if (number.IndexOf('.') == 0)
			{
				number = string.Concat("0", number);
			}
			else if (number.StartsWith("0"))
			{
				number = string.Concat("0", number);
			}

			if (number.IndexOf("00", StringComparison.Ordinal) == 0)
			{
				number = number.Substring(1);
			}

			return number;
		}

		public string NormalizeNumberWithMinusSign(string number)
		{
			var positionOfNormalMinus = number.IndexOf('-');
			var positionOfSpecialMinus = number.IndexOf('\u2212');
			var positionOfDash = number.IndexOf('\u2013');
			char[] space = { ' ', '\u00a0', '\u2009', '\u202F' };
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
			return number.Normalize(NormalizationForm.FormKC);
		}

		public string NormalizedNumber(string number, string thousandSeparators, string decimalSeparators,
			bool noSeparator)
		{
			var normalizedNumber = string.Empty;
			try
			{
				// see http://www.fileformat.info/info/unicode/char/2212/index.htm
				//request to support special minus sign

				if (_omitLeadingZero)
				{
					number = OmitZero(number);
				}

				number = NormalizeNumberWithMinusSign(number);

				if (thousandSeparators != String.Empty &&
					Regex.IsMatch(number, @"^m?[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d(\1\d\d\d)+$"))
				// e.g 1,000,000
				{
					normalizedNumber = Regex.Replace(number, @"[" + thousandSeparators + @"]", "t");
				}
				else if (thousandSeparators != String.Empty && decimalSeparators != String.Empty &&
						 Regex.IsMatch(number,
							 @"^m?[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d(\1\d\d\d)*[" + decimalSeparators +
							 @"]\d+$")) // e.g. 1,000.5
				{
					var usedThousandSeparator =
						Regex.Match(number, @"[" + thousandSeparators + @"]").Value;

					//for ex if we have 1.45.67, we need to replace only first aparition of the thousand separator
					var reg = new Regex(Regex.Escape(usedThousandSeparator));
					normalizedNumber = reg.Replace(number, "t", 1);

					var usedDecimalSeparator =
						Regex.Match(normalizedNumber, @"[" + decimalSeparators + @"]").Value;
					normalizedNumber = usedDecimalSeparator != String.Empty
						? Regex.Replace(normalizedNumber, @"[" + usedDecimalSeparator + @"]", "d")
						: normalizedNumber;
				}
				else if (thousandSeparators != String.Empty &&
						 Regex.IsMatch(number, @"^m?[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d$"))
				// e.g. 1,000
				{
					if (_sourceMatchingDecimalSeparators != String.Empty &&
						Regex.IsMatch(number, @"^m?[1-9]\d{0,2}([" + decimalSeparators + @"])\d\d\d$"))
					{
						normalizedNumber = Regex.Replace(number, @"[" + thousandSeparators + @"]", "u");
					}
					else
					{
						normalizedNumber = Regex.Replace(number, @"[" + thousandSeparators + @"]", "t");
					}
				}
				else
				{
					if (_sourceMatchingDecimalSeparators != String.Empty &&
						Regex.IsMatch(number, @"^m?\d+[" + decimalSeparators + @"]\d+$")) // e.g. 0,100
					{
						normalizedNumber = Regex.Replace(number, @"[" + decimalSeparators + @"]", "d");
					}
					else
					{
						normalizedNumber = number;
					}

					if (noSeparator)
					{
						if (_isSource)
						{
							normalizedNumber = NormalizeNumberNoSeparator(_sourceDecimalSeparators,
								_sourceThousandSeparators, normalizedNumber);
						}
						else
						{
							normalizedNumber = NormalizeNumberNoSeparator(_targetDecimalSeparators,
								_targetThousandSeparators, normalizedNumber);
						}

					}
					return normalizedNumber.Normalize(NormalizationForm.FormKC);
				}
			}
			catch (Exception e) { }

			return normalizedNumber.Normalize(NormalizationForm.FormKC);
		}

		public string NormalizeNumberNoSeparator(string decimalSeparators, string thousandSeparators, string normalizedNumber)
		{
			var thousandSeparator = string.Empty;
			var decimalSeparator = string.Empty;
			var hasMinusSign = false;

			if (thousandSeparators != string.Empty)
			{
				thousandSeparator = thousandSeparators.Substring(0, 1);
			}

			if (decimalSeparators != string.Empty)
			{
				decimalSeparator = decimalSeparators.Substring(0, 1);
			}

			try
			{
				if (!(normalizedNumber.Contains("u") || normalizedNumber.Contains("t")))
				{
					var numberElements = Regex.Split(normalizedNumber, "d");
					decimal thousandNumber;

					if (numberElements[0].IndexOf('m') == 0)
					{
						var numberWithoutMinus = numberElements[0].Substring(1);
						thousandNumber = decimal.Parse(numberWithoutMinus.Normalize(NormalizationForm.FormKC));
						hasMinusSign = true;
					}
					else
					{
						decimal.TryParse(numberElements[0].Normalize(NormalizationForm.FormKC), out thousandNumber);	
					}

					//number must be >= 1000 to run no separator option
					if (thousandNumber >= 1000)
					{
						var thousands = thousandNumber.ToString(CultureInfo.InvariantCulture);
						var tempNormalized = new StringBuilder();
						var counter = 0;
						for (var i = thousands.Length - 1; i >= 0; i--)
						{
							if (tempNormalized.Length > 0 && counter % 3 == 0)
							{
								if (!string.IsNullOrEmpty(thousandSeparators))
								{
									if (!thousandSeparator.Contains(" "))
									{
										tempNormalized.Insert(0, string.Format(@"{0}{1}", thousands[i], thousandSeparator));
									}
									else
									{
										tempNormalized.Insert(0, string.Format(@"{0}{1}", thousands[i], string.Empty));
									}
								}
								else
								{
									tempNormalized.Insert(0, string.Format("{0}", thousands[i]));
								}

								counter = 1;
							}
							else
							{
								tempNormalized.Insert(0, thousands[i]);
								counter++;
							}
						}

						if (numberElements.Length > 1)
						{
							if (decimalSeparator != string.Empty)
							{

								tempNormalized.Append(string.Format(@"{0}{1}", decimalSeparator, numberElements[1]));
								if (hasMinusSign)
								{
									tempNormalized.Insert(0, "m");
								}

							}
							else
							{
								tempNormalized.Append(string.Format("{0}", numberElements[1]));
								if (hasMinusSign)
								{
									tempNormalized.Insert(0, "m");
								}
							}

						}
						var temNormalizedWithoutSpaces = tempNormalized.ToString().Normalize(NormalizationForm.FormKC);

						normalizedNumber = NormalizedNumber(temNormalizedWithoutSpaces, thousandSeparators,
							decimalSeparators,
							false);
					}
				}
			}
			catch (Exception e)
			{

			}
			return normalizedNumber.Normalize(NormalizationForm.FormKC);
		}

		private void RemoveMatchingAlphanumerics(IList<string> sourceAlphanumericsList, ICollection<string> targetAlphanumericsList)
		{

			for (var nJ = sourceAlphanumericsList.Count - 1; nJ >= 0; nJ--)
			{
				if (!targetAlphanumericsList.Contains(sourceAlphanumericsList[nJ])) continue;

				targetAlphanumericsList.Remove(sourceAlphanumericsList[nJ]);
				sourceAlphanumericsList.RemoveAt(nJ);
			}
		}

		public List<string> GetAlphanumericList(string text)
		{
			var alphaList = new List<string>();
			var words = Regex.Split(text, @"\s");

			if (_verificationSettings.CustomsSeparatorsAlphanumerics)
			{
				string[] customsSeparators = !string.IsNullOrEmpty(_verificationSettings.GetAlphanumericsCustomSeparator)
				? _verificationSettings.GetAlphanumericsCustomSeparator.Split(',')
				: new string[0];

				var res = string.Join(string.Empty, customsSeparators);

				// replace \ with \\ in order to recognize the regex expression
				if (res.Contains(@"\"))
				{
					res = res.Replace(@"\", @"\\");
				}
				var regex = string.Format(@"^-?\u2212?(^(?=.*[a-zA-Z{0}])(?=.*[0-9]).+$)", res);

				alphaList.AddRange(
					from word in words
					from Match match in Regex.Matches(word.Normalize(NormalizationForm.FormKC), regex)
					select Regex.Replace(match.Value, "\u2212|-", "m"));
			}
			else
			{
				alphaList.AddRange(
					from word in words
					from Match match in Regex.Matches(word.Normalize(NormalizationForm.FormKC), @"^-?\u2212?(^(?=.*[a-zA-Z-])(?=.*[0-9]).+$)")
					select Regex.Replace(match.Value, "\u2212|-", "m"));
			}
			return alphaList;
		}

		private string GetSegmentText(ISegment segment)
		{
			return VerificationSettings.ExcludeTagText == false ? segment.ToString() : TextGeneratorProcessor.GetPlainText(segment, false);
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

		private List<NumberModel> GetTargetFromHindiNumbers(string source, string target)
		{
			var _projectController = GetProjectController();

			List<NumberModel> result = new List<NumberModel>();
			StringBuilder sb = new StringBuilder();

			if (_projectController.CurrentProject != null)
			{
				var projectInfo = _projectController.CurrentProject.GetProjectInfo();
				_language = projectInfo.SourceLanguage.DisplayName;

				var hindiNumbers = GetHindiNumbers();

				if (_language == "Hindi (India)")
				{
					string sourceResult = string.Empty;
					string sourceGroupResult = string.Empty;

					var targetGroups = target.Split(' ').ToArray();
					var sourceGroups = source.Split(' ').ToArray();

					foreach (var sourceGroup in sourceGroups)
					{
						foreach (var s in sourceGroup)
						{
							if (hindiNumbers.ContainsValue(s.ToString()))
							{
								//add arabic values to result 
								sourceResult = sb.Append(hindiNumbers.FirstOrDefault(h => h.Value == s.ToString()).Key).ToString();
							}
							else
							{
								// add separator like , or . (or just the number)
								sourceResult = sb.Append(s.ToString()).ToString();
							}
						}
						sourceGroupResult = sourceGroupResult + " " + sourceResult;
						sourceResult = string.Empty;
						sb.Clear();
					}
					result = GetFormatedNumbers(sourceGroupResult, targetGroups);
				}
				else
				{
					string targetResult = string.Empty;
					string targetGroupResult = string.Empty;

					var targetGroups = target.Split(' ').ToArray();
					var sourceGroups = source.Split(' ').ToArray();

					foreach (var targetGroup in targetGroups)
					{
						foreach (var t in targetGroup)
						{
							if (hindiNumbers.ContainsValue(t.ToString()))
							{
								//add arabic values to result 
								targetResult = sb.Append(hindiNumbers.FirstOrDefault(h => h.Value == t.ToString()).Key).ToString();
							}
							else
							{
								// add separator like , or . (or just the number)
								targetResult = sb.Append(t.ToString()).ToString();
							}
						}
						targetGroupResult = targetGroupResult + " " + targetResult;
						targetResult = string.Empty;
						sb.Clear();
					}
					result = GetFormatedNumbers(targetGroupResult, sourceGroups);
				}
			}
			return result;
		}

		private Dictionary<string,string> GetHindiNumbers()
		{
			Dictionary<string, string> hindiDictionary = new Dictionary<string, string>();
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

		public List<NumberModel> GetFormatedNumbers(string textGroupResult, string[] textGroups)
		{
			List<NumberModel> result = new List<NumberModel>();

			var targetGroupRes = textGroupResult.Split(' ').ToArray();
			targetGroupRes = targetGroupRes.Skip(1).ToArray();

			var res = textGroups.Zip(targetGroupRes, (s, t) => new NumberModel { SourceText = s, TargetText = t }).ToList();

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
				result.Add(new NumberModel
				{
					SourceText = numberRes.SourceText,
					TargetText = numberRes.TargetText
				});
			}
			return result;
		}
		#endregion
	}
}
