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
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Utils;
using Sdl.Core.Globalization;
using Sdl.Core.PluginFramework;

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

        #endregion

        public NumberVerifierMain():this(null)
        {

          
        }

        public NumberVerifierMain(INumberVerifierSettings numberVerifierSettings)
        {
            _verificationSettings = numberVerifierSettings;
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

        private string _sourceThousandSeparators = "";
        private string _targetThousandSeparators = "";
        private string _sourceDecimalSeparators = "";
        private string _targetDecimalSeparators = "";


        public void Initialize(IDocumentProperties documentInfo)
        {

            _sourceThousandSeparators += VerificationSettings.SourceThousandsSpace ? " " : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakSpace ? "\u00A0" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsThinSpace ? "\u2009" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakThinSpace ? "\u202F" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsComma ? "," : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsPeriod ? "." : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsCustomSeparator
                ? VerificationSettings.GetSourceThousandsCustomSeparator
                : "";

            _targetThousandSeparators += VerificationSettings.TargetThousandsSpace ? " " : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsNobreakSpace ? "\u00A0" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsThinSpace ? "\u2009" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsNobreakThinSpace ? "\u202F" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsComma ? "," : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsPeriod ? "." : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsCustomSeparator
                ? VerificationSettings.GetTargetThousandsCustomSeparator
                : "";

            _sourceDecimalSeparators += VerificationSettings.SourceDecimalComma ? "," : "";
            _sourceDecimalSeparators += VerificationSettings.SourceDecimalPeriod ? "." : "";
            _sourceDecimalSeparators += VerificationSettings.SourceDecimalCustomSeparator
                ? VerificationSettings.GetSourceDecimalCustomSeparator
                : "";

            _targetDecimalSeparators += VerificationSettings.TargetDecimalComma ? "," : "";
            _targetDecimalSeparators += VerificationSettings.TargetDecimalPeriod ? "." : "";
            _targetDecimalSeparators += VerificationSettings.TargetDecimalCustomSeparator
                ? VerificationSettings.GetTargetDecimalCustomSeparator
                : "";

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
                    
                        var extendedMessageReporter = MessageReporter as IBilingualContentMessageReporterWithExtendedData;
                        if (extendedMessageReporter != null)
                        {
                            #region CreateExtendedData

                            var extendedData = new NumberVerifierMessageData(errorMessage.SourceNumberIssues, errorMessage.TargetNumberIssues,
                                segmentPair.Target);

                            #endregion

                            #region ReportingMessageWithExtendedData

                            extendedMessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
                                errorMessage.ErrorLevel, errorMessage.ErrorMessage,
                                new TextLocation(new Location(segmentPair.Target, true), 0),
                                new TextLocation(new Location(segmentPair.Target, false),
                                    segmentPair.Target.ToString().Length - 1),
                                extendedData);

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
        public List<ErrorReporting> CheckAlphanumerics(string sourceText,string targetText)
        {
            var errorReportingList = new List<ErrorReporting>();
            var sourceAlphanumericsList = GetAlphanumericList(sourceText, _sourceDecimalSeparators,
                _sourceThousandSeparators);

            // find all alphanumeric names in target and add to list
            var targetAlphanumericsList = GetAlphanumericList(targetText, _targetDecimalSeparators,
                _targetThousandSeparators);

            // remove alphanumeric names found both in source and target from respective list
            RemoveMatchingAlphanumerics(sourceAlphanumericsList, targetAlphanumericsList);

            var errorReporting = new ErrorReporting
            {
                ErrorLevel = ErrorReportingUtils.GetAlphanumericsErrorLevel(sourceAlphanumericsList,targetAlphanumericsList,VerificationSettings),
                ErrorMessage = PluginResources.Error_AlphanumericsModified,
                SourceNumberIssues = ErrorReportingUtils.GetAlphanumericsIssues(sourceAlphanumericsList,VerificationSettings),
                TargetNumberIssues = ErrorReportingUtils.GetAlphanumericsIssues(targetAlphanumericsList, VerificationSettings)
            };

            if (errorReporting.SourceNumberIssues != string.Empty || errorReporting.TargetNumberIssues != string.Empty)
            {
                errorReportingList.Add(errorReporting);
            }
            
            return errorReportingList;
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

            // find all numbers in source and add to list
            numberList.Clear();
            normalizedNumberList.Clear();
            var customDecimalSeparators = AddCustomSeparators(decimalSeparators, true);
            var customThousandSeparators = AddCustomSeparators(thousandSeparators, false);

            //call normalize method with source settings
            NormalizeAlphanumerics(text, numberList, normalizedNumberList,
                customThousandSeparators, customDecimalSeparators, noSeparator, omitZero);

            var tulpleList = Tuple.Create(numberList, normalizedNumberList);

            return tulpleList;
        }

        /// <summary>
        /// Returns a errors list after numbers are normalized
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="targetText"></param>
        /// <returns></returns>
        public List<ErrorReporting> CheckNumbers(string sourceText, string targetText)
        {
            var errorList = new List<ErrorReporting>();

            var sourceList = GetNumbersTuple(sourceText, _sourceDecimalSeparators,
             _sourceThousandSeparators, VerificationSettings.SourceNoSeparator, VerificationSettings.SourceOmitLeadingZero);

            var targetList = GetNumbersTuple(targetText, _targetDecimalSeparators,
                _targetThousandSeparators, VerificationSettings.TargetNoSeparator, VerificationSettings.TargetOmitLeadingZero);

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

            // check if numbers have been modified and should be reported
            if (sourceNumberList.Count > 0 && targetNumberList.Count > 0 &&
                VerificationSettings.ReportModifiedNumbers)
            {
                var errorReporting = new ErrorReporting
                {
                    ErrorLevel = ErrorLevel.Unspecified,
                    ErrorMessage = string.Empty,
                    SourceNumberIssues = string.Empty,
                    TargetNumberIssues = string.Empty
                };
                switch (VerificationSettings.ModifiedNumbersErrorType)
                {
                    case "Error":
                        errorReporting.ErrorLevel = ErrorLevel.Error;
                        errorReporting.ErrorMessage = PluginResources.Error_NumbersNotIdentical;
                        errorList.Add(errorReporting);
                        break;
                    case "Warning":
                        errorReporting.ErrorLevel = ErrorLevel.Warning;
                        errorReporting.ErrorMessage = PluginResources.Error_NumbersNotIdentical;
                        errorList.Add(errorReporting);
                        break;
                    default:
                        errorReporting.ErrorLevel = ErrorLevel.Note;
                        errorReporting.ErrorMessage = PluginResources.Error_NumbersNotIdentical;
                        errorList.Add(errorReporting);
                        break;
                }
                SetReportDetails(errorReporting, sourceNumberList, targetNumberList, sourceText, targetText);
            }

            // check if numbers have been added and should be reported
            if (sourceNumberList.Count < targetNumberList.Count && VerificationSettings.ReportAddedNumbers)
            {
                var errorReporting = new ErrorReporting
                {
                    ErrorLevel = ErrorLevel.Unspecified,
                    ErrorMessage = string.Empty,
                    SourceNumberIssues = string.Empty,
                    TargetNumberIssues = string.Empty

                };
                if (VerificationSettings.AddedNumbersErrorType == "Error")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Error;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersAdded;
                    errorList.Add(errorReporting);
                }
                else if (VerificationSettings.AddedNumbersErrorType == "Warning")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Warning;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersAdded;
                    errorList.Add(errorReporting);
                }
                else if (VerificationSettings.AddedNumbersErrorType != "Error" && VerificationSettings.AddedNumbersErrorType != "Warning")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Note;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersAdded;
                    errorList.Add(errorReporting);
                }

                SetReportDetails(errorReporting, sourceNumberList, targetNumberList, sourceText, targetText);
            }

            // check if numbers have been removed and should be reported
            if (sourceNumberList.Count > targetNumberList.Count &&
                VerificationSettings.ReportRemovedNumbers)
            {
                var errorReporting = new ErrorReporting
                {
                    ErrorLevel = ErrorLevel.Unspecified,
                    ErrorMessage = string.Empty,
                    SourceNumberIssues = string.Empty,
                    TargetNumberIssues = string.Empty
                };
                if (VerificationSettings.RemovedNumbersErrorType == "Error")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Error;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersRemoved;
                    errorList.Add(errorReporting);
                }
                else if (VerificationSettings.RemovedNumbersErrorType == "Warning")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Warning;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersRemoved;
                    errorList.Add(errorReporting);
                }
                else if (VerificationSettings.RemovedNumbersErrorType != "Error" && VerificationSettings.RemovedNumbersErrorType != "Warning")
                {
                    errorReporting.ErrorLevel = ErrorLevel.Note;
                    errorReporting.ErrorMessage = PluginResources.Error_NumbersRemoved;
                    errorList.Add(errorReporting);
                }
                SetReportDetails(errorReporting, sourceNumberList, targetNumberList, sourceText, targetText);
            }

            return errorList;
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

            var errorsListFromAlphanumerics = CheckAlphanumerics(sourceText, targetText);
            var errorsListFromNormalizedNumbers = CheckNumbers(sourceText, targetText);

            errorList.AddRange(errorsListFromAlphanumerics);
            errorList.AddRange(errorsListFromNormalizedNumbers);

            return errorList;
        }

        /// <summary>
        /// Adds source and target text to error
        /// </summary>
        /// <param name="errorReporting"></param>
        /// <param name="sourceNumberList"></param>
        /// <param name="targetNumberList"></param>
        /// <param name="sourceText"></param>
        /// <param name="targetText"></param>
        public void SetReportDetails(ErrorReporting errorReporting,List<string> sourceNumberList,List<string > targetNumberList,string sourceText,string targetText)
        {
            // collate remaining numbers and put into string variables for reporting of details       
            if (sourceNumberList.Count > 0 &&
                (VerificationSettings.ReportAddedNumbers ||
                 VerificationSettings.ReportRemovedNumbers ||
                 VerificationSettings.ReportModifiedNumbers))
            {
                errorReporting.SourceNumberIssues = sourceNumberList.Aggregate(errorReporting.SourceNumberIssues,
                    (current, t) => current + (t + " \r\n"));
            }



            if (targetNumberList.Count > 0 &&
               (VerificationSettings.ReportAddedNumbers ||
                VerificationSettings.ReportRemovedNumbers ||
                VerificationSettings.ReportModifiedNumbers))
            {
                errorReporting.TargetNumberIssues = targetNumberList.Aggregate(errorReporting.TargetNumberIssues,
                    (current, t) => current + (t + " \r\n"));
            }



            if (VerificationSettings.ReportExtendedMessages)
            {
                errorReporting.ErrorMessage = "\r\n SOURCE: " +
                                          sourceText + " \r\n TARGET: " +
                                          targetText;
            }
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
        public string AddCustomSeparators(string selectedSeparators,bool isDecimalSeparator)
        {
            var expression = string.Empty; 
            var separatorsList = new List<string>();
            var selectedSep = string.Empty;
            var separators = string.Empty;

            // you can use in target as separators source separators or selected target separators
            if (_verificationSettings.AllowLocalizations)
            {

                if (isDecimalSeparator)
                {
                    selectedSep = separators + AddSourceDecimalSeparators();
                    selectedSep = selectedSep + AddTargetDecimalSeparators();
                }
                else
                {
                    selectedSep = selectedSep+ AddSourceThousandSeparators();
                }
            }

            //you can use only source separators selected
            if (_verificationSettings.PreventLocalizations)
            {
                if (isDecimalSeparator)
                {
                    selectedSep = separators + AddSourceDecimalSeparators();
                }
                else
                {
                    selectedSep = selectedSep + AddSourceThousandSeparators();
                }

            }
            
            //put separators in a list, in order to eliminate the duplicates
            if (selectedSeparators != string.Empty)
            {
                foreach (char c in selectedSeparators)
                {
                    if (c.ToString().Contains("'"))
                    {
                        expression = @"\u2019\u0027";
                    }
                    else
                    {
                        expression = @"\" + string.Format("u{0:x4}", (int) c);
                    }

                    if (!separatorsList.Contains(expression.ToLower()) && !string.IsNullOrEmpty(expression))
                    {
                        separatorsList.Add(expression.ToLower());
                    }
                }
            }


            //get a list of source separators if we are in case of allow localization, or prevent localization
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

        /// <summary>
        /// Returns a string containing source decimal selectors selected
        /// Used in case allow localization or prevent localization are selected
        /// </summary>
        /// <returns></returns>
        private string AddSourceDecimalSeparators()
        {
            var sourceDecimalSeparators = string.Empty;

            if (_verificationSettings.SourceDecimalComma)
            {
                sourceDecimalSeparators = sourceDecimalSeparators + @"\u002C"; 
            }
            if (_verificationSettings.SourceDecimalPeriod)
            {
                sourceDecimalSeparators = sourceDecimalSeparators + @"\u002E";
            }
            if (_verificationSettings.SourceDecimalCustomSeparator)
            {
                sourceDecimalSeparators = sourceDecimalSeparators + _verificationSettings.GetSourceDecimalCustomSeparator;
            }
            return sourceDecimalSeparators;
        }

        private string AddTargetDecimalSeparators()
        {
            var targetDecimalSeparators = string.Empty;

            if (_verificationSettings.TargetDecimalComma)
            {
                targetDecimalSeparators = targetDecimalSeparators + @"\u002C";
            }
            if (_verificationSettings.TargetDecimalPeriod)
            {
                targetDecimalSeparators = targetDecimalSeparators + @"\u002E";
            }
            if (_verificationSettings.TargetDecimalCustomSeparator)
            {
                targetDecimalSeparators = targetDecimalSeparators + _verificationSettings.GetSourceDecimalCustomSeparator;
            }
            return targetDecimalSeparators;
        }

        /// <summary>
        /// Returns a string containing source thousand selectors selected
        /// Used in case allow localization or prevent localization are selected
        /// </summary>
        /// <returns></returns>
        private string AddSourceThousandSeparators()
        {
            var sourceThousandSeparators = string.Empty;

            if (_verificationSettings.SourceThousandsSpace)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u0020";
            }
            if (_verificationSettings.SourceThousandsNobreakSpace)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u00a0";
            }
            if (_verificationSettings.SourceThousandsThinSpace)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u2009";
            }
            if (_verificationSettings.SourceThousandsNobreakThinSpace)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u202F";
            }
            if (_verificationSettings.SourceThousandsComma)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u002C"; 
            }
            if (_verificationSettings.SourceThousandsPeriod)
            {
                sourceThousandSeparators = sourceThousandSeparators + @"\u002E";
            }
            if (_verificationSettings.SourceThousandsCustomSeparator)
            {
                sourceThousandSeparators = sourceThousandSeparators + _verificationSettings.GetSourceThousandsCustomSeparator;
            }

            return sourceThousandSeparators;
        }

        public void NormalizeAlphanumerics(string text, ICollection<string> numeberCollection,
            ICollection<string> normalizedNumberCollection, string thousandSeparators, string decimalSeparators,
            bool noSeparator,bool omitLeadingZero)
        {
            var separators = string.Concat(thousandSeparators, decimalSeparators);

            //skip the "-" in case of: - 23 (dash, space, number)
            char[] dashSign = {'-', '\u2013', '\u2212'};
            char[] space = { ' ', '\u00a0', '\u2009', '\u202F' };
            var spacePosition = text.IndexOfAny(space);
            var dashPosition = text.IndexOfAny(dashSign);
            if (dashPosition==0 && spacePosition == 1)
            {
                text = text.Substring(2);
            }

#region Omit zero
            //if only "No separator" is selected "separators" variable will be a empty string
            string expresion=string.Empty;

            if (omitLeadingZero)
            {
                _omitLeadingZero = true;
                if (separators != string.Empty)
                {
                   expresion = string.Format(@"-?\u2013?\u2212?\u002E?\u2013?\d+([{0}]\d+)*", separators);
                }
                else
                {
                    expresion =string.Format(@"-?\u2013?\u2212?\u002E?\u2013?\d+(\d+)*");
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
            char[] space = { ' ','\u00a0','\u2009','\u202F'};
            var spacePosition = number.IndexOfAny(space);
            

            //if it has space is not a negative number
            if (positionOfNormalMinus == 0 && spacePosition!=1)
            {
                number = number.Replace("-", "m");
            }
            if (positionOfSpecialMinus == 0 && spacePosition!=1)
            {
                number = number.Replace("\u2212", "m");
            }
            if (positionOfDash == 0 && spacePosition != 1)
            {
                number = number.Replace("\u2013", "m");
            }
            return number;
        }

        public string NormalizedNumber(string number, string thousandSeparators, string decimalSeparators,
            bool noSeparator)
        {
            string normalizedNumber;
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
                if (_sourceDecimalSeparators != String.Empty &&
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
                if (_sourceDecimalSeparators != String.Empty &&
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
                   
                    normalizedNumber = NormalizeNumberNoSeparator(_sourceDecimalSeparators,_targetDecimalSeparators, normalizedNumber);
                }
            }
            return normalizedNumber;
        }



        public string NormalizeNumberNoSeparator(string decimalSeparators, string thousandSeparators, string normalizedNumber)
        {
            var thousandSeparator=string.Empty;
            var decimalSeparator=string.Empty;
            var hasMinusSign = false;

            if (thousandSeparators != string.Empty)
            {
                 thousandSeparator = thousandSeparators.Substring(0,1);
            }

            if (decimalSeparators != string.Empty)
            {
                decimalSeparator = decimalSeparators.Substring(0,1);
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
                        thousandNumber = decimal.Parse(numberWithoutMinus);
                        hasMinusSign = true;
                    }
                    else
                    {
                        thousandNumber = decimal.Parse(numberElements[0]);
                    }

                    //number must be >= 1000 to run no separator option
                    if (thousandNumber >= 1000)
                    {

                        var thousands = thousandNumber.ToString(CultureInfo.InvariantCulture);
                        var tempNormalized = new StringBuilder();
                        var counter = 0;
                        for (var i = thousands.Length - 1; i >= 0; i--)
                        {
                            if (tempNormalized.Length > 0 && counter%3 == 0)
                            {
                                if (thousandSeparators != string.Empty)
                                {
                                    tempNormalized.Insert(0, string.Format(@"{0}{1}", thousands[i], thousandSeparator));
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
                        normalizedNumber = NormalizedNumber(tempNormalized.ToString(), thousandSeparators,
                            decimalSeparators,
                            false);
                    }
                }
            }
            catch (Exception e)
            {
                
            }
            return normalizedNumber;
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

        public List<string> GetAlphanumericList(string text,string decimalSeparators,string thousandSeparators)
        {
            var alphaList = new List<string>();
            //added no break space, thin space and no break thin space, as separators.
            var positionOfNormalMinus = text.IndexOf('-');
            var positionOfSpecialMinus = text.IndexOf('\u2212');
            if (positionOfNormalMinus == 0)
            {
                text = text.Replace("-", "m");
            }
            if (positionOfSpecialMinus == 0)
            {
                text = text.Replace("\u2212", "m");
            }
            char[] delimiterChars = {' ', '\u00a0', '\u2009', '\u202F'};
            var wordsList = text.Split(delimiterChars).ToList();
            var alpha = new List<string>();

            //get the words which contains numbers
            foreach (var word in wordsList)
            {
                if (word.Any(char.IsDigit))
                {
                    var upperCaseLetters = word.Where(d=>!char.IsDigit(d)).ToList();
                    if (upperCaseLetters.Count!=0 && upperCaseLetters.All(char.IsUpper))
                    {
                        alpha.Add(word);
                    }
                }
            }

            //check the alphanumerics 

            var customDecimalSeparators = AddSourceDecimalSeparators();
            var customThousandSeparators = AddSourceThousandSeparators();

            var separators = string.Concat(customDecimalSeparators, customThousandSeparators);
            try
            {
                string expresion;
                if (separators != string.Empty)
                {
                    expresion = string.Format(@"^-?\u2212?\d*([A-Z]|\d)*([{0}]\d+)*", separators);
                }
                else
                {
                    expresion= @"^-?\u2212?\d*([A-Z]|\d)*(\d+)*";
                }
                
                if (alpha.Count != 0)
                {
                    foreach (var alphaNumeric in alpha)
                    {
                        alphaList.AddRange(from Match match in Regex.Matches(alphaNumeric, expresion) select match.Value);
                    }
                }
              
            }
            catch (Exception e)
            {
                
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
                     &&!(VerificationSettings.ExcludeUntranslatedSegments == true && segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Unspecified)
                     &&!(VerificationSettings.ExcludeDraftSegments==true && segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Draft);
        }
                #endregion
    }
}
