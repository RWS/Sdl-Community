using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;
using System.Text.RegularExpressions;
using System.Threading;
using Sdl.Community.Extended.MessageUI;
using Sdl.Core.Globalization;

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
        private NumberVerifierSettings _verificationSettings;
        private bool? _enabled; 
        #endregion

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
        internal NumberVerifierSettings VerificationSettings
        {
            get
            {
                if (_verificationSettings != null || _sharedObjects == null) return _verificationSettings;
                var bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
                if (bundle == null) return _verificationSettings;
                _verificationSettings = bundle.GetSettingsGroup<NumberVerifierSettings>();
                var x = bundle.GetSettingsGroup(this.SettingsId);
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

            _sourceThousandSeparators += VerificationSettings.SourceThousandsSpace.Value ? " " : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakSpace.Value ? "\u00A0" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsThinSpace.Value ? "\u2009" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsNobreakThinSpace.Value ? "\u202F" : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsComma.Value ? "," : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsPeriod.Value ? "." : "";
            _sourceThousandSeparators += VerificationSettings.SourceThousandsCustomSeparator.Value
                ? VerificationSettings.GetSourceThousandsCustomSeparator
                : "";

            _targetThousandSeparators += VerificationSettings.TargetThousandsSpace.Value ? " " : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsNobreakSpace.Value ? "\u00A0" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsThinSpace.Value ? "\u2009" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsNobreakThinSpace.Value ? "\u202F" : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsComma.Value ? "," : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsPeriod.Value ? "." : "";
            _targetThousandSeparators += VerificationSettings.TargetThousandsCustomSeparator.Value
                ? VerificationSettings.GetTargetThousandsCustomSeparator
                : "";

            _sourceDecimalSeparators += VerificationSettings.SourceDecimalComma.Value ? "," : "";
            _sourceDecimalSeparators += VerificationSettings.SourceDecimalPeriod.Value ? "." : "";
            _sourceDecimalSeparators += VerificationSettings.SourceDecimalCustomSeparator.Value
                ? VerificationSettings.GetSourceDecimalCustomSeparator
                : "";

            _targetDecimalSeparators += VerificationSettings.TargetDecimalComma.Value ? "," : "";
            _targetDecimalSeparators += VerificationSettings.TargetDecimalPeriod.Value ? "." : "";
            _targetDecimalSeparators += VerificationSettings.TargetDecimalCustomSeparator.Value
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
        private void CheckParagraphUnit(IParagraphUnit paragraphUnit)
        {

            var sourceNumberList = new List<string>();
            var targetNumberList = new List<string>();
            var sourceNormalizedNumberList = new List<string>();
            var targetNormalizedNumberList = new List<string>();

            // loop through the whole paragraph unit
            foreach (var segmentPair in paragraphUnit.SegmentPairs.Where(FilterSegmentPairs))
            {
                var sourceText = GetSegmentText(segmentPair.Source);
                var targetText = GetSegmentText(segmentPair.Target);
               

                // find all alphanumeric names in source and add to list
                var sourceAlphanumericsList = GetAlphanumericList(sourceText);

                // find all alphanumeric names in target and add to list
                var targetAlphanumericsList = GetAlphanumericList(targetText);

                // remove alphanumeric names found both in source and target from respective list
                RemoveMatchingAlphanumerics(sourceAlphanumericsList, targetAlphanumericsList);

                // find all numbers in source and add to list
                sourceNumberList.Clear();
                sourceNormalizedNumberList.Clear();
                NormalizeAlphanumerics(sourceText, sourceNumberList, sourceNormalizedNumberList,_sourceThousandSeparators,_sourceDecimalSeparators, VerificationSettings.SourceNoSeparator);

                // find all numbers in target and add to list
                targetNumberList.Clear();
                targetNormalizedNumberList.Clear();
                NormalizeAlphanumerics(targetText, targetNumberList, targetNormalizedNumberList,_targetThousandSeparators,_targetDecimalSeparators, VerificationSettings.TargetNoSeparator);

                // remove identical numbers found both in source and target from respective list
                RemoveIdenticalNumbers(sourceNumberList, targetNumberList, targetNormalizedNumberList, sourceNormalizedNumberList);

                // remove numbers found both in source and target from respective list disregarding difference in thousands and decimal separators
                RemoveNumbersIgnoreThousandsAndDecimalSeparators(sourceNumberList, targetNormalizedNumberList, sourceNormalizedNumberList, targetNumberList);

                // remove numbers found both in source and target from respective list disregarding difference when thousands and decimal separators are undefined due to ambiguity 
                RemoveNumbersUndefinedThousandsAndDecimalSeparator(targetNumberList, sourceNumberList, sourceNormalizedNumberList, targetNormalizedNumberList);



                var errorLevel = ErrorLevel.Unspecified;
                var errorMessage = String.Empty;
                var extendedErrorMessage = String.Empty;

                // check if numbers have been modified and should be reported
                if (sourceNumberList.Count > 0 && targetNumberList.Count > 0 && VerificationSettings.ReportModifiedNumbers.Value)
                {
                    switch (VerificationSettings.ModifiedNumbersErrorType.Value)
                    {
                        case "Error":
                            errorLevel = ErrorLevel.Error;
                            break;
                        case "Warning":
                            errorLevel = ErrorLevel.Warning;
                            break;
                        default:
                            errorLevel = ErrorLevel.Note;
                            break;
                    }
                    errorMessage = errorMessage + PluginResources.Error_NumbersNotIdentical;
                }

                // check if numbers have been added and should be reported
                if (sourceNumberList.Count < targetNumberList.Count && VerificationSettings.ReportAddedNumbers.Value)
                {
                    if (VerificationSettings.AddedNumbersErrorType.Value == "Error")
                    {
                        errorLevel = ErrorLevel.Error;
                    }
                    else if (VerificationSettings.AddedNumbersErrorType.Value == "Warning" && errorLevel != ErrorLevel.Error)
                    {
                        errorLevel = ErrorLevel.Warning;
                    }
                    else if (errorLevel != ErrorLevel.Error && errorLevel != ErrorLevel.Warning)
                    {
                        errorLevel = ErrorLevel.Note;
                    }
                    errorMessage = errorMessage + PluginResources.Error_NumbersAdded;
                }

                // check if numbers have been removed and should be reported
                if (sourceNumberList.Count > targetNumberList.Count && VerificationSettings.ReportRemovedNumbers.Value)
                {
                    if (VerificationSettings.RemovedNumbersErrorType.Value == "Error")
                    {
                        errorLevel = ErrorLevel.Error;
                    }
                    else if (VerificationSettings.RemovedNumbersErrorType.Value == "Warning" && errorLevel != ErrorLevel.Error)
                    {
                        errorLevel = ErrorLevel.Warning;
                    }
                    else if (errorLevel != ErrorLevel.Error && errorLevel != ErrorLevel.Warning)
                    {
                        errorLevel = ErrorLevel.Note;
                    }
                    errorMessage = errorMessage + PluginResources.Error_NumbersRemoved;
                }

                // check if alphanumeric names are mismatched and should be reported
                if ((targetAlphanumericsList.Count > 0 || sourceAlphanumericsList.Count > 0) && VerificationSettings.ReportModifiedAlphanumerics.Value)
                {
                    if (VerificationSettings.ModifiedAlphanumericsErrorType.Value == "Error")
                    {
                        errorLevel = ErrorLevel.Error;
                    }
                    else if (VerificationSettings.ModifiedAlphanumericsErrorType.Value == "Warning" && errorLevel != ErrorLevel.Error)
                    {
                        errorLevel = ErrorLevel.Warning;
                    }
                    else if (errorLevel != ErrorLevel.Error && errorLevel != ErrorLevel.Warning)
                    {
                        errorLevel = ErrorLevel.Note;
                    }
                    errorMessage = errorMessage + PluginResources.Error_AlphanumericsModified;
                }


                // if there are any mismatched numbers or alphanumerics to report, output a warning message
                if (errorMessage == String.Empty) continue;

                // collate remaining numbers and put into string variables for reporting of details
                var sourceNumberIssues = String.Empty;
                if (sourceNumberList.Count > 0 && (VerificationSettings.ReportAddedNumbers.Value || VerificationSettings.ReportRemovedNumbers.Value || VerificationSettings.ReportModifiedNumbers.Value))
                {
                    sourceNumberIssues = sourceNumberList.Aggregate(sourceNumberIssues, (current, t) => current + (t + " \r\n"));
                }

                if (sourceAlphanumericsList.Count > 0 && VerificationSettings.ReportModifiedAlphanumerics.Value)
                {
                    sourceNumberIssues = sourceAlphanumericsList.Aggregate(sourceNumberIssues, (current, t) => current + (t + " \r\n"));
                }

                var targetNumberIssues = String.Empty;
                if (targetNumberList.Count > 0 && (VerificationSettings.ReportAddedNumbers.Value || VerificationSettings.ReportRemovedNumbers.Value || VerificationSettings.ReportModifiedNumbers.Value))
                {
                    targetNumberIssues = targetNumberList.Aggregate(targetNumberIssues, (current, t) => current + (t + " \r\n"));
                }

                if (targetAlphanumericsList.Count > 0 && VerificationSettings.ReportModifiedAlphanumerics.Value)
                {
                    targetNumberIssues = targetAlphanumericsList.Aggregate(targetNumberIssues, (current, t) => current + (t + " \r\n"));
                }


                if (VerificationSettings.ReportExtendedMessages == true)
                {
                    extendedErrorMessage = "\r\n SOURCE: " + TextGeneratorProcessor.GetPlainText(segmentPair.Source, !VerificationSettings.ExcludeTagText.Value) + " \r\n TARGET: " + TextGeneratorProcessor.GetPlainText(segmentPair.Target, !VerificationSettings.ExcludeTagText.Value);
                }


                #region ReportingMessage

                var extendedMessageReporter = MessageReporter as IBilingualContentMessageReporterWithExtendedData;
                if (extendedMessageReporter != null)
                {
                    #region CreateExtendedData
                    var extendedData = new NumberVerifierMessageData(sourceNumberIssues, targetNumberIssues, segmentPair.Target);
                    #endregion

                    #region ReportingMessageWithExtendedData
                    extendedMessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
                        errorLevel, errorMessage + extendedErrorMessage,
                        new TextLocation(new Location(segmentPair.Target, true), 0),
                        new TextLocation(new Location(segmentPair.Target, false), segmentPair.Target.ToString().Length - 1),
                        extendedData);
                    #endregion

                }
                else
                {
                    #region ReportingMessageWithoutExtendedData
                    MessageReporter.ReportMessage(this, PluginResources.Plugin_Name,
                        errorLevel, errorMessage + extendedErrorMessage,
                        new TextLocation(new Location(segmentPair.Target, true), 0),
                        new TextLocation(new Location(segmentPair.Target, false), segmentPair.Target.ToString().Length - 1));
                    #endregion
                }
            }
        }

        private void RemoveNumbersUndefinedThousandsAndDecimalSeparator(IList targetNumberList, IList sourceNumberList,
            IList<string> sourceNormalizedNumberList, IList<string> targetNormalizedNumberList)
        {
            if (VerificationSettings.AllowLocalizations.Value || VerificationSettings.RequireLocalizations.Value)
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
            if (VerificationSettings.AllowLocalizations.Value || VerificationSettings.RequireLocalizations.Value)
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
            if (VerificationSettings.PreventLocalizations.Value || VerificationSettings.AllowLocalizations.Value)
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
        public string AddCustomSeparators(string thousand, string decimalSeparator)
        {
            var expression = string.Empty; 
            var separatorsList = new List<string>();
            var sourceSeparators = string.Empty;
            var separators = string.Empty;

            // you can use in target as separators source separators or selected target separators
            if (_verificationSettings.AllowLocalizations)
            {
                sourceSeparators = sourceSeparators + AddSourceThousandSeparators();
                sourceSeparators = sourceSeparators + AddSourceDecimalSeparators();
            }

            //you can use only source separators selected
            if (_verificationSettings.PreventLocalizations)
            {
                sourceSeparators = sourceSeparators + AddSourceThousandSeparators();
                sourceSeparators = sourceSeparators + AddSourceDecimalSeparators();

            }
            
            //put separators in a list, in order to eliminate the duplicates
            if (decimalSeparator !=string.Empty && thousand != string.Empty)
            {
                var allSeparators = string.Concat(decimalSeparator, thousand); //string.Join(decimalSeparator, thousand);

                foreach (char c in allSeparators)
            {
                if (c.ToString().Contains("'"))
                {
                          expression = @"\u2019\u0027";
                }
                else
                {
                        expression = @"\" + string.Format("u{0:x4}", (int)c);
                }

                    if (!separatorsList.Contains(expression.ToLower()) && !string.IsNullOrEmpty(expression))
                    {
                        separatorsList.Add(expression.ToLower());
            }
                }
            }
      

            //get a list of source separators if we are in case of allow localisation, or prevent localisation
            if (sourceSeparators != string.Empty)
            {
                var sepSource = sourceSeparators.Split('\\').ToList();

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
                sourceThousandSeparators = sourceThousandSeparators + @"\u002C\"; 
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
            bool noSeparator)
        {
            var separators = AddCustomSeparators(thousandSeparators, decimalSeparators);

            //if only "No separator" is selected "separators" variable will be a empty string
            if (separators != string.Empty)
        {
                var expresion = string.Format(@"-?\d+([{0}]\d+)*", separators);
           
            foreach (Match match in Regex.Matches(text, expresion))
            {
                    var normalizedNumber = NormalizedNumber(match.Value, thousandSeparators, decimalSeparators,
                        noSeparator);

                numeberCollection.Add(match.Value);
                normalizedNumberCollection.Add(normalizedNumber);

                }
            }
            else
            {
                var normalizedNumber = NormalizedNumber(text, thousandSeparators, decimalSeparators,
                    noSeparator);
                numeberCollection.Add(text);
                normalizedNumberCollection.Add(normalizedNumber);
            }

        }

        private string NormalizedNumber(string number, string thousandSeparators, string decimalSeparators,
            bool noSeparator)
        {
            string normalizedNumber;

            decimalSeparators = AddCustomSeparators(null, decimalSeparators); 
            thousandSeparators = AddCustomSeparators(thousandSeparators, null);

            if (thousandSeparators != String.Empty &&
                Regex.IsMatch(number, @"^[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d(\1\d\d\d)+$"))
                // e.g 1,000,000
            {
                normalizedNumber = Regex.Replace(number, @"[" + thousandSeparators + @"]", "t");
            }
            else if (thousandSeparators != String.Empty && decimalSeparators != String.Empty &&
                     Regex.IsMatch(number,
                         @"^[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d(\1\d\d\d)*[" + decimalSeparators +
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
                     Regex.IsMatch(number, @"^[1-9]\d{0,2}([" + thousandSeparators + @"])\d\d\d$"))
                // e.g. 1,000
            {
                if (_sourceDecimalSeparators != String.Empty &&
                    Regex.IsMatch(number, @"^[1-9]\d{0,2}([" + decimalSeparators + @"])\d\d\d$"))
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
                    Regex.IsMatch(number, @"^\d+[" + decimalSeparators + @"]\d+$")) // e.g. 0,100
                {
                    normalizedNumber = Regex.Replace(number, @"[" + decimalSeparators + @"]", "d");
                }
                else
                {
                    normalizedNumber = number;
                }

                if (noSeparator)
                {
                    normalizedNumber = NormalizeNumberNoSeparator(decimalSeparators, normalizedNumber);
                }
            }
            return normalizedNumber;
        }

        private string NormalizeNumberNoSeparator(string decimalSeparators, string normalizedNumber)
        {
            //if there is no separator add comma as separator and run normalize process again
            if (normalizedNumber.Length > 3 && !(normalizedNumber.Contains("u") || normalizedNumber.Contains("t")))
            {
                var numberElements = Regex.Split(normalizedNumber, "d");
                var thousands = numberElements[0];
                var tempNormalized = new StringBuilder();
                for (var i = thousands.Length - 1; i >= 0; i--)
                {
                    if (tempNormalized.Length > 0 && tempNormalized.Length%3 == 0)
                    {
                        tempNormalized.Insert(0, string.Format("{0},", thousands[i]));
                    }
                    else
                    {
                        tempNormalized.Insert(0, thousands[i]);
                    }
                }
                if (numberElements.Length > 1)
                {
                    tempNormalized.Append(numberElements[1]);
                }
                normalizedNumber = NormalizedNumber(tempNormalized.ToString(), ",", decimalSeparators, false);
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

        private List<string> GetAlphanumericList(string text)
        {
            var alphaList = new List<string>();
            alphaList.AddRange(from Match match in Regex.Matches(text, @"\d*([A-Z]+\d+)|([A-Z])+[A-Z]*") select match.Value);
            return alphaList;
        }

        private string GetSegmentText(ISegment segment)
        {
            return VerificationSettings.ExcludeTagText.Value == false ? segment.ToString() : TextGeneratorProcessor.GetPlainText(segment, false);
        }

        private bool FilterSegmentPairs(ISegmentPair segmentPair)
        {
            return (VerificationSettings.ExcludeLockedSegments.Value == false ||
                    segmentPair.Properties.IsLocked == false) &&
                   (VerificationSettings.Exclude100Percents.Value == false ||
                    ((segmentPair.Properties.TranslationOrigin.OriginType != "auto-propagated" &&
                      segmentPair.Properties.TranslationOrigin.OriginType != "tm") ||
                     segmentPair.Properties.TranslationOrigin.MatchPercent != 100))
                     &&(VerificationSettings.ExcludeUntranslatedSegments == false && segmentPair.Properties.ConfirmationLevel == ConfirmationLevel.Draft);
        }
                #endregion
    }
}
