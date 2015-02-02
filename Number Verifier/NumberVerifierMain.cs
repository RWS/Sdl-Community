using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;

using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.Extended.MessageUI;

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
        #endregion

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
                if (_verificationSettings == null && _sharedObjects != null)
                {
                    ISettingsBundle bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
                    if (bundle != null)
                    {
                        _verificationSettings = bundle.GetSettingsGroup<NumberVerifierSettings>();
                    }
                }
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
            get
            {
                if (_textGeneratorProcessor == null)
                {
                    _textGeneratorProcessor = new TextGenerator();
                }
                return _textGeneratorProcessor;
            }
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
        public System.Drawing.Icon Icon
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

            _sourceThousandSeparators += (VerificationSettings.SourceThousandsSpace.Value == true) ? " " : "";
            _sourceThousandSeparators += (VerificationSettings.SourceThousandsNobreakSpace.Value == true) ? "\u00A0" : "";
            _sourceThousandSeparators += (VerificationSettings.SourceThousandsThinSpace.Value == true) ? "\u2009" : "";
            _sourceThousandSeparators += (VerificationSettings.SourceThousandsNobreakThinSpace.Value == true) ? "\u202F" : "";
            _sourceThousandSeparators += (VerificationSettings.SourceThousandsComma.Value == true) ? "," : "";
            _sourceThousandSeparators += (VerificationSettings.SourceThousandsPeriod.Value == true) ? "." : "";

            _targetThousandSeparators += (VerificationSettings.TargetThousandsSpace.Value == true) ? " ": "";
            _targetThousandSeparators += (VerificationSettings.TargetThousandsNobreakSpace.Value == true) ? "\u00A0" : "";
            _targetThousandSeparators += (VerificationSettings.TargetThousandsThinSpace.Value == true) ? "\u2009" : "";
            _targetThousandSeparators += (VerificationSettings.TargetThousandsNobreakThinSpace.Value == true) ? "\u202F" : "";
            _targetThousandSeparators += (VerificationSettings.TargetThousandsComma.Value == true) ? "," : "";
            _targetThousandSeparators += (VerificationSettings.TargetThousandsPeriod.Value == true) ? "." : "";

            _sourceDecimalSeparators += (VerificationSettings.SourceDecimalComma.Value == true) ? "," : "";
            _sourceDecimalSeparators += (VerificationSettings.SourceDecimalPeriod.Value == true) ? "." : "";

            _targetDecimalSeparators += (VerificationSettings.TargetDecimalComma.Value == true) ? "," : "";
            _targetDecimalSeparators += (VerificationSettings.TargetDecimalPeriod.Value == true) ? "." : "";

        }
        #endregion

        #region "process"
        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            // Apply the verification logic.
            CheckParagraphUnit(paragraphUnit);
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
        #region "verify"
        private void CheckParagraphUnit(IParagraphUnit paragraphUnit)
        {
            
            List<string> sourceNumberList = new List<string>();
            List<string> targetNumberList = new List<string>();
            List<string> sourceNormalizedNumberList = new List<string>();
            List<string> targetNormalizedNumberList = new List<string>();
            List<string> sourceAlphanumericsList = new List<string>();
            List<string> targetAlphanumericsList = new List<string>();

            string normalizedNumber;
            string targetUsedThousandSeparator, targetUsedDecimalSeparator, sourceUsedThousandSeparator, sourceUsedDecimalSeparator;
            string sourceText = string.Empty, targetText = string.Empty;
            int nJ;
            ErrorLevel errorLevel = ErrorLevel.Unspecified;
            string errorMessage = string.Empty;
            string extendedErrorMessage = string.Empty;

            // loop through the whole paragraph unit
            foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
            {
                if ((VerificationSettings.ExcludeLockedSegments.Value == false || segmentPair.Properties.IsLocked == false) && (VerificationSettings.Exclude100Percents.Value == false || !((segmentPair.Properties.TranslationOrigin.OriginType == "auto-propagated" || segmentPair.Properties.TranslationOrigin.OriginType == "tm") && segmentPair.Properties.TranslationOrigin.MatchPercent == 100)))
                {

                    if (VerificationSettings.ExcludeTagText.Value == false)
                    {
                        sourceText = segmentPair.Source.ToString();
                        targetText = segmentPair.Target.ToString();
                    }
                    else
                    {
                        sourceText = TextGeneratorProcessor.GetPlainText(segmentPair.Source, false);
                        targetText = TextGeneratorProcessor.GetPlainText(segmentPair.Target, false);
                    }


                    // find all alphanumeric names in source and add to list
                    sourceAlphanumericsList.Clear();
                    foreach (Match match in Regex.Matches(sourceText, @"\d*([A-Z]+\d+)+[A-Z]*"))
                    {
                        sourceAlphanumericsList.Add(match.Value);
                    }
                    
                    // find all alphanumeric names in target and add to list
                    targetAlphanumericsList.Clear();
                    foreach (Match match in Regex.Matches(targetText, @"\d*([A-Z]+\d+)+[A-Z]*"))
                    {
                        targetAlphanumericsList.Add(match.Value);
                    }
                    
                    // remove alphanumeric names found both in source and target from respective list
                    for (nJ = sourceAlphanumericsList.Count - 1; nJ >= 0; nJ--)
                    {
                        if (targetAlphanumericsList.Contains(sourceAlphanumericsList[nJ]))
                        {
                            targetAlphanumericsList.Remove(sourceAlphanumericsList[nJ]);
                            sourceAlphanumericsList.RemoveAt(nJ);
                        }
                    }

                    // find all numbers in source and add to list
                    sourceNumberList.Clear();
                    sourceNormalizedNumberList.Clear();
                    foreach (Match match in Regex.Matches(sourceText, @"^-?\d+([ \u00A0\u2009\u202F.,]\d+)*"))
                    {
                        if (_sourceThousandSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _sourceThousandSeparators + @"])\d\d\d(\1\d\d\d)+$"))  // e.g 1,000,000
                        {
                            normalizedNumber = Regex.Replace(match.Value, @"[" + _sourceThousandSeparators + @"]", "t");
                        }
                        else if (_sourceThousandSeparators != string.Empty && _sourceDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _sourceThousandSeparators + @"])\d\d\d(\1\d\d\d)*[" + _sourceDecimalSeparators + @"]\d+$"))  // e.g. 1,000.5
                        {
                            sourceUsedThousandSeparator = Regex.Match(match.Value, @"[" + _sourceThousandSeparators + @"]").Value;
                            normalizedNumber = Regex.Replace(match.Value, @"[" + sourceUsedThousandSeparator + @"]", "t");
                            sourceUsedDecimalSeparator = Regex.Match(normalizedNumber, @"[" + _sourceDecimalSeparators + @"]").Value;
                            if (sourceUsedDecimalSeparator != string.Empty)
                            {
                                normalizedNumber = Regex.Replace(normalizedNumber, @"[" + sourceUsedDecimalSeparator + @"]", "d");
                            }
                            else
                            {
                                normalizedNumber = match.Value;
                            }
                        }
                        else if (_sourceThousandSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _sourceThousandSeparators + @"])\d\d\d$"))  // e.g. 1,000
                        {
                            if (_sourceDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _sourceDecimalSeparators + @"])\d\d\d$"))
                            {
                                normalizedNumber = Regex.Replace(match.Value, @"[" + _sourceThousandSeparators + @"]", "u");
                            }
                            else
                            {
                                normalizedNumber = Regex.Replace(match.Value, @"[" + _sourceThousandSeparators + @"]", "t");
                            }
                        }
                        else if (_sourceDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^\d+[" + _sourceDecimalSeparators + @"]\d+$"))   // e.g. 0,100
                        {
                            normalizedNumber = Regex.Replace(match.Value, @"[" + _sourceDecimalSeparators + @"]", "d");
                        }
                        else
                        {
                            normalizedNumber = match.Value;
                        }
                        sourceNumberList.Add(match.Value);
                        sourceNormalizedNumberList.Add(normalizedNumber);
                    }

                    // find all numbers in target and add to list
                    targetNumberList.Clear();
                    targetNormalizedNumberList.Clear();
                    foreach (Match match in Regex.Matches(targetText, @"^-?\d+([ \u00A0\u2009\u202F.,]\d+)*"))
                    {
                        if (_targetThousandSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _targetThousandSeparators + @"])\d\d\d(\1\d\d\d)+$"))
                        {
                            normalizedNumber = Regex.Replace(match.Value, @"[" + _targetThousandSeparators + @"]", "t");
                        }
                        else if (_targetThousandSeparators != string.Empty && _targetDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _targetThousandSeparators + @"])\d\d\d(\1\d\d\d)*[" + _targetDecimalSeparators + @"]\d+$"))
                        {
                            targetUsedThousandSeparator = Regex.Match(match.Value, @"[" + _targetThousandSeparators + @"]").Value;
                            normalizedNumber = Regex.Replace(match.Value, @"[" + targetUsedThousandSeparator + @"]", "t");
                            targetUsedDecimalSeparator = Regex.Match(normalizedNumber, @"[" + _targetDecimalSeparators + @"]").Value;
                            if (targetUsedDecimalSeparator != string.Empty)
                            {
                                normalizedNumber = Regex.Replace(normalizedNumber, @"[" + targetUsedDecimalSeparator + @"]", "d");
                            }
                            else
                            {
                                normalizedNumber = match.Value;
                            }
                        }
                        else if (_targetThousandSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _targetThousandSeparators + @"])\d\d\d$"))
                        {
                            if (_targetDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^[1-9]\d{0,2}([" + _targetDecimalSeparators + @"])\d\d\d$"))
                            {
                                normalizedNumber = Regex.Replace(match.Value, @"[" + _targetThousandSeparators + @"]", "u");
                            }
                            else
                            {
                                normalizedNumber = Regex.Replace(match.Value, @"[" + _targetThousandSeparators + @"]", "t");
                            }
                        }
                        else if (_targetDecimalSeparators != string.Empty && Regex.IsMatch(match.Value, @"^\d+[" + _targetDecimalSeparators + @"]\d+$"))
                        {
                            normalizedNumber = Regex.Replace(match.Value, @"[" + _targetDecimalSeparators + @"]", "d");
                        }
                        else
                        {
                            normalizedNumber = match.Value;
                        }
                        targetNumberList.Add(match.Value);
                        targetNormalizedNumberList.Add(normalizedNumber);
                    }

                    // remove identical numbers found both in source and target from respective list
                    if (VerificationSettings.PreventLocalizations.Value == true || VerificationSettings.AllowLocalizations.Value == true)
                    {
                        for (nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
                        {
                            if (targetNumberList.Contains(sourceNumberList[nJ]))
                            {
                                targetNormalizedNumberList.RemoveAt(targetNumberList.IndexOf(sourceNumberList[nJ]));
                                targetNumberList.RemoveAt(targetNumberList.IndexOf(sourceNumberList[nJ]));
                                sourceNormalizedNumberList.RemoveAt(nJ);
                                sourceNumberList.RemoveAt(nJ);
                            }
                        }
                    }

                    // remove numbers found both in source and target from respective list disregarding difference in thousands and decimal separators
                    if (VerificationSettings.AllowLocalizations.Value == true || VerificationSettings.RequireLocalizations.Value == true)
                    {
                        for (nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
                        {
                            if (targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ]))
                            {
                                targetNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ]));
                                targetNormalizedNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ]));
                                sourceNormalizedNumberList.RemoveAt(nJ);
                                sourceNumberList.RemoveAt(nJ);
                            }
                        }
                    }

                    // remove numbers found both in source and target from respective list disregarding difference when thousands and decimal separators are undefined due to ambiguity 
                    if (VerificationSettings.AllowLocalizations.Value == true || VerificationSettings.RequireLocalizations.Value == true)
                    {
                        if (targetNumberList.Count > 0 && sourceNumberList.Count > 0)
                        {
                            for (nJ = sourceNumberList.Count - 1; nJ >= 0; nJ--)
                            {
                                if (sourceNormalizedNumberList[nJ].IndexOf("u") > 0 && targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ].Replace("u", "d")))
                                {
                                    targetNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "d")));
                                    targetNormalizedNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "d")));
                                    sourceNormalizedNumberList.RemoveAt(nJ);
                                    sourceNumberList.RemoveAt(nJ);
                                }
                                else if (sourceNormalizedNumberList[nJ].IndexOf("u") > 0 && targetNormalizedNumberList.Contains(sourceNormalizedNumberList[nJ].Replace("u", "t")))
                                {
                                    targetNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "t")));
                                    targetNormalizedNumberList.RemoveAt(targetNormalizedNumberList.IndexOf(sourceNormalizedNumberList[nJ].Replace("u", "t")));
                                    sourceNormalizedNumberList.RemoveAt(nJ);
                                    sourceNumberList.RemoveAt(nJ);
                                }
                            }
                        }

                        if (targetNumberList.Count > 0 && sourceNumberList.Count > 0)
                        {
                            for (nJ = targetNumberList.Count - 1; nJ >= 0; nJ--)
                            {
                                if (targetNormalizedNumberList[nJ].IndexOf("u") > 0 && sourceNormalizedNumberList.Contains(targetNormalizedNumberList[nJ].Replace("u", "d")))
                                {
                                    sourceNumberList.RemoveAt(sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "d")));
                                    sourceNormalizedNumberList.RemoveAt(sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "d")));
                                    targetNormalizedNumberList.RemoveAt(nJ);
                                    targetNumberList.RemoveAt(nJ);
                                }
                                else if (targetNormalizedNumberList[nJ].IndexOf("u") > 0 && sourceNormalizedNumberList.Contains(targetNormalizedNumberList[nJ].Replace("u", "t")))
                                {
                                    sourceNumberList.RemoveAt(sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "t")));
                                    sourceNormalizedNumberList.RemoveAt(sourceNormalizedNumberList.IndexOf(targetNormalizedNumberList[nJ].Replace("u", "t")));
                                    targetNormalizedNumberList.RemoveAt(nJ);
                                    targetNumberList.RemoveAt(nJ);
                                }
                            }
                        }
                    }



                    errorLevel = ErrorLevel.Unspecified;
                    errorMessage = string.Empty;
                    extendedErrorMessage = string.Empty;

                    // check if numbers have been modified and should be reported
                    if (sourceNumberList.Count > 0 && targetNumberList.Count > 0 && VerificationSettings.ReportModifiedNumbers.Value == true)
                    {
                        if (VerificationSettings.ModifiedNumbersErrorType.Value == "Error")
                        {
                            errorLevel = ErrorLevel.Error;
                        }
                        else if (VerificationSettings.ModifiedNumbersErrorType.Value == "Warning")
                        {
                            errorLevel = ErrorLevel.Warning;
                        }
                        else
                        {
                            errorLevel = ErrorLevel.Note;
                        }                            
                        errorMessage = errorMessage + PluginResources.Error_NumbersNotIdentical;
                    }

                    // check if numbers have been added and should be reported
                    if (sourceNumberList.Count < targetNumberList.Count && VerificationSettings.ReportAddedNumbers.Value == true)
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
                    if (sourceNumberList.Count > targetNumberList.Count && VerificationSettings.ReportRemovedNumbers.Value == true)
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
                    if ((targetAlphanumericsList.Count > 0 || sourceAlphanumericsList.Count > 0) && VerificationSettings.ReportModifiedAlphanumerics.Value == true)
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
                    if (errorMessage != string.Empty)
                    {

                        // collate remaining numbers and put into string variables for reporting of details
                        string sourceNumberIssues = string.Empty;
                        if (sourceNumberList.Count > 0 && (VerificationSettings.ReportAddedNumbers.Value == true || VerificationSettings.ReportRemovedNumbers.Value == true || VerificationSettings.ReportModifiedNumbers.Value == true))
                        {
                            for (nJ = 0; nJ < sourceNumberList.Count; nJ++)
                                sourceNumberIssues += sourceNumberList[nJ].ToString() + " \r\n";  
                        }

                        if (sourceAlphanumericsList.Count > 0 && VerificationSettings.ReportModifiedAlphanumerics.Value == true)
                        {
                            for (nJ = 0; nJ < sourceAlphanumericsList.Count; nJ++)
                                sourceNumberIssues += sourceAlphanumericsList[nJ].ToString() + " \r\n";
                        }

                        string targetNumberIssues = string.Empty;
                        if (targetNumberList.Count > 0 && (VerificationSettings.ReportAddedNumbers.Value == true || VerificationSettings.ReportRemovedNumbers.Value == true || VerificationSettings.ReportModifiedNumbers.Value == true))
                        {
                            for (nJ = 0; nJ < targetNumberList.Count; nJ++)
                                targetNumberIssues += targetNumberList[nJ].ToString() + " \r\n";
                        }

                        if (targetAlphanumericsList.Count > 0 && VerificationSettings.ReportModifiedAlphanumerics.Value == true)
                        {
                            for (nJ = 0; nJ < targetAlphanumericsList.Count; nJ++)
                                targetNumberIssues += targetAlphanumericsList[nJ].ToString() + " \r\n";
                        }


                        if (VerificationSettings.ReportExtendedMessages == true)
                        {
                            extendedErrorMessage = "\r\n SOURCE: " + TextGeneratorProcessor.GetPlainText(segmentPair.Source, !VerificationSettings.ExcludeTagText.Value) + " \r\n TARGET: " + TextGeneratorProcessor.GetPlainText(segmentPair.Target, !VerificationSettings.ExcludeTagText.Value);
                        }


                        #region ReportingMessage
                        if (MessageReporter is IBilingualContentMessageReporterWithExtendedData)
                        {
                            #region CreateExtendedData
                            NumberVerifierMessageData extendedData = new NumberVerifierMessageData(sourceNumberIssues, targetNumberIssues, segmentPair.Target);
                            #endregion

                            #region ReportingMessageWithExtendedData
                            IBilingualContentMessageReporterWithExtendedData extendedMessageReporter = (IBilingualContentMessageReporterWithExtendedData)MessageReporter;
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
                        #endregion


                    }

                }
    
            }
        }
        #endregion


    }
}
