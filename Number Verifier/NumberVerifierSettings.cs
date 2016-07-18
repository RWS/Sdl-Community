using System;
using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Core.Settings;

namespace Sdl.Community.NumberVerifier
{
    /// <summary>
    /// This class is used for reading and writing the plug-in setting(s) value(s).
    /// The settings are physically stored in an (XML-compliant) *.sdlproj file, which
    /// is generated for each project that is created in SDL Trados Studio or for 
    /// each file that is opened and translated.
    /// </summary>
    public class NumberVerifierSettings : SettingsGroup, INumberVerifierSettings
    {
        #region "setting"

        // Return the value of the setting.
        public bool ExcludeTagText
        {
            set { GetSetting<bool>(nameof(ExcludeTagText)).Value = value; }
            get { return GetSetting<bool>(nameof(ExcludeTagText)).Value; }
        }

        public bool ReportAddedNumbers
        {
            set { GetSetting<bool>(nameof(ReportAddedNumbers)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportAddedNumbers)).Value; }
        }

        public bool ReportRemovedNumbers
        {
            set { GetSetting<bool>(nameof(ReportRemovedNumbers)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportRemovedNumbers)).Value; }
        }

        public bool ReportModifiedNumbers
        {
            set { GetSetting<bool>(nameof(ReportModifiedNumbers)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportModifiedNumbers)).Value; }
        }

        public bool ReportModifiedAlphanumerics
        {
            set { GetSetting<bool>(nameof(ReportModifiedAlphanumerics)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportModifiedAlphanumerics)).Value; }
        }

        public string AddedNumbersErrorType
        {
            set { GetSetting<string>(nameof(AddedNumbersErrorType)).Value = value; }
            get { return GetSetting<string>(nameof(AddedNumbersErrorType)).Value; }
        }

        public string RemovedNumbersErrorType
        {
            set { GetSetting<string>(nameof(RemovedNumbersErrorType)).Value = value; }
            get { return GetSetting<string>(nameof(RemovedNumbersErrorType)).Value; }
        }

        public string ModifiedNumbersErrorType
        {
            set { GetSetting<string>(nameof(ModifiedNumbersErrorType)).Value = value; }
            get { return GetSetting<string>(nameof(ModifiedNumbersErrorType)).Value; }
        }

        public string ModifiedAlphanumericsErrorType
        {
            set { GetSetting<string>(nameof(ModifiedAlphanumericsErrorType)).Value = value; }
            get { return GetSetting<string>(nameof(ModifiedAlphanumericsErrorType)).Value; }
        }

        public bool ReportBriefMessages
        {
            set { GetSetting<bool>(nameof(ReportBriefMessages)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportBriefMessages)).Value; }
        }

        public bool ReportExtendedMessages
        {
            set { GetSetting<bool>(nameof(ReportExtendedMessages)).Value = value; }
            get { return GetSetting<bool>(nameof(ReportExtendedMessages)).Value; }
        }

        public bool AllowLocalizations
        {
            set { GetSetting<bool>(nameof(AllowLocalizations)).Value = value; }
            get { return GetSetting<bool>(nameof(AllowLocalizations)).Value; }
        }

        public bool PreventLocalizations
        {
            set { GetSetting<bool>(nameof(PreventLocalizations)).Value = value; }
            get { return GetSetting<bool>(nameof(PreventLocalizations)).Value; }
        }

        public bool RequireLocalizations
        {
            set { GetSetting<bool>(nameof(RequireLocalizations)).Value = value; }
            get { return GetSetting<bool>(nameof(RequireLocalizations)).Value; }
        }

        public bool SourceThousandsSpace
        {
            set { GetSetting<bool>(nameof(SourceThousandsSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsSpace)).Value; }
        }

        public bool SourceThousandsNobreakSpace
        {
            set { GetSetting<bool>(nameof(SourceThousandsNobreakSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsNobreakSpace)).Value; }
        }

        public bool SourceThousandsThinSpace
        {
            set { GetSetting<bool>(nameof(SourceThousandsThinSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsThinSpace)).Value; }
        }

        public bool SourceThousandsNobreakThinSpace
        {
            set { GetSetting<bool>(nameof(SourceThousandsNobreakThinSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsNobreakThinSpace)).Value; }
        }

        public bool SourceThousandsComma
        {
            set { GetSetting<bool>(nameof(SourceThousandsComma)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsComma)).Value; }
        }

        public bool SourceThousandsPeriod
        {
            set { GetSetting<bool>(nameof(SourceThousandsPeriod)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsPeriod)).Value; }
        }

        public bool SourceNoSeparator
        {
            set { GetSetting<bool>(nameof(SourceNoSeparator)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceNoSeparator)).Value; }
        }

        public bool TargetThousandsSpace
        {
            set { GetSetting<bool>(nameof(TargetThousandsSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsSpace)).Value; }
        }

        public bool TargetThousandsNobreakSpace
        {
            set { GetSetting<bool>(nameof(TargetThousandsNobreakSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsNobreakSpace)).Value; }
        }

        public bool TargetThousandsThinSpace
        {
            set { GetSetting<bool>(nameof(TargetThousandsThinSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsThinSpace)).Value; }
        }

        public bool TargetThousandsNobreakThinSpace
        {
            set { GetSetting<bool>(nameof(TargetThousandsNobreakThinSpace)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsNobreakThinSpace)).Value; }
        }

        public bool TargetThousandsComma
        {
            set { GetSetting<bool>(nameof(TargetThousandsComma)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsComma)).Value; }
        }

        public bool TargetThousandsPeriod
        {
            set { GetSetting<bool>(nameof(TargetThousandsPeriod)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetThousandsPeriod)).Value; }
        }

        public bool TargetNoSeparator
        {
            set { GetSetting<bool>(nameof(TargetNoSeparator)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetNoSeparator)).Value; }
        }

        public bool SourceDecimalComma
        {
            set { GetSetting<bool>(nameof(SourceDecimalComma)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceDecimalComma)).Value; }
        }

        public bool SourceDecimalPeriod
        {
            set { GetSetting<bool>(nameof(SourceDecimalPeriod)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceDecimalPeriod)).Value; }
        }

        public bool TargetDecimalComma
        {
            set { GetSetting<bool>(nameof(TargetDecimalComma)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetDecimalComma)).Value; }
        }

        public bool TargetDecimalPeriod
        {
            set { GetSetting<bool>(nameof(TargetDecimalPeriod)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetDecimalPeriod)).Value; }
        }

        public bool ExcludeLockedSegments
        {
            set { GetSetting<bool>(nameof(ExcludeLockedSegments)).Value = value; }
            get { return GetSetting<bool>(nameof(ExcludeLockedSegments)).Value; }
        }

        public bool Exclude100Percents
        {
            set { GetSetting<bool>(nameof(Exclude100Percents)).Value = value; }
            get { return GetSetting<bool>(nameof(Exclude100Percents)).Value; }
        }

        public bool ExcludeUntranslatedSegments
        {
            set { GetSetting<bool>(nameof(ExcludeUntranslatedSegments)).Value = value; }
            get { return GetSetting<bool>(nameof(ExcludeUntranslatedSegments)).Value; }
        }


        public bool ExcludeDraftSegments
        {
            set { GetSetting<bool>(nameof(ExcludeDraftSegments)).Value = value; }
            get { return GetSetting<bool>(nameof(ExcludeDraftSegments)).Value; }
        }


        public bool SourceOmitLeadingZero
        {
            set { GetSetting<bool>(nameof(SourceOmitLeadingZero)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceOmitLeadingZero)).Value; }
        }

        public bool TargetOmitLeadingZero
        {
            set { GetSetting<bool>(nameof(TargetOmitLeadingZero)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetOmitLeadingZero)).Value; }
        }

        public bool SourceThousandsCustomSeparator
        {
            set { GetSetting<bool>(nameof(SourceThousandsCustomSeparator)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceThousandsCustomSeparator)).Value; }
        }

        public bool TargetThousandsCustomSeparator
        {
            set { GetSetting<bool>(nameof(TargetThousandsCustomSeparator)).Value=value; }
            get { return GetSetting<bool>(nameof(TargetThousandsCustomSeparator)).Value; }
        }

        public bool SourceDecimalCustomSeparator
        {
            set { GetSetting<bool>(nameof(SourceDecimalCustomSeparator)).Value = value; }
            get { return GetSetting<bool>(nameof(SourceDecimalCustomSeparator)).Value; }
        }

        public bool TargetDecimalCustomSeparator
        {
            set { GetSetting<bool>(nameof(TargetDecimalCustomSeparator)).Value = value; }
            get { return GetSetting<bool>(nameof(TargetDecimalCustomSeparator)).Value; }
        }

        public string GetSourceThousandsCustomSeparator
        {
            set { GetSetting<string>(nameof(GetSourceThousandsCustomSeparator)).Value = value; }
            get { return GetSetting<string>(nameof(GetSourceThousandsCustomSeparator)).Value; }
        }

        public string GetTargetThousandsCustomSeparator
        {
            set { GetSetting<string>(nameof(GetTargetThousandsCustomSeparator)).Value=value; }
            get { return GetSetting<string>(nameof(GetTargetThousandsCustomSeparator)).Value; }
        }

        public string GetSourceDecimalCustomSeparator
        {
            set { GetSetting<string>(nameof(GetSourceDecimalCustomSeparator)).Value=value; }
            get { return GetSetting<string>(nameof(GetSourceDecimalCustomSeparator)).Value; }
        }

        public string GetTargetDecimalCustomSeparator
        {
            set { GetSetting<string>(nameof(GetTargetDecimalCustomSeparator)).Value=value; }
            get { return GetSetting<string>(nameof(GetTargetDecimalCustomSeparator)).Value; }
        }

     
        #endregion

        public void Reset(string propertyName)
        {
            GetDefaultValue(propertyName);
        }

        /// <summary>
        /// Return the default value of the setting properties
        /// </summary>
        /// <param name="settingId"></param>
        /// <returns></returns>
        #region "default"
        protected override object GetDefaultValue(string settingId)
        {
            switch (settingId)
            {
                case nameof(ExcludeTagText):
                    return false;
                case nameof(ReportAddedNumbers):
                    return true;
                case nameof(ReportRemovedNumbers):
                    return true;
                case nameof(ReportModifiedNumbers) :
                    return true;
                case nameof(ReportModifiedAlphanumerics) :
                    return true;
                case nameof(AddedNumbersErrorType):
                    return "Warning";
                case nameof(RemovedNumbersErrorType):
                    return "Warning";
                case nameof(ModifiedNumbersErrorType):
                    return "Error";
                case nameof(ModifiedAlphanumericsErrorType):
                    return "Error";
                case nameof(ReportBriefMessages):
                    return true;
                case nameof(ReportExtendedMessages):
                    return false;
                case nameof(AllowLocalizations):
                    return true;
                case nameof(PreventLocalizations):
                    return false;
                case nameof(RequireLocalizations):
                    return false;
                case nameof(SourceThousandsSpace):
                    return true;
                case nameof(SourceThousandsNobreakSpace):
                    return true;
                case nameof(SourceThousandsThinSpace):
                    return true;
                case nameof(SourceThousandsNobreakThinSpace):
                    return true;
                case nameof(SourceThousandsComma):
                    return true;
                case nameof(SourceThousandsPeriod):
                    return true;
                case nameof(SourceNoSeparator):
                    return true;
                case nameof(TargetThousandsSpace):
                    return true;
                case nameof(TargetThousandsNobreakSpace):
                    return true;
                case nameof(TargetThousandsThinSpace):
                    return true;
                case nameof(TargetThousandsNobreakThinSpace):
                    return true;
                case nameof(TargetThousandsComma):
                    return true;
                case nameof(TargetThousandsPeriod):
                    return true;
                case nameof(TargetNoSeparator):
                    return true;
                case nameof(SourceDecimalComma):
                    return true;
                case nameof(SourceDecimalPeriod):
                    return true;
                case nameof(TargetDecimalComma):
                    return true;
                case nameof(TargetDecimalPeriod):
                    return true;
                case nameof(ExcludeLockedSegments):
                    return false;
                case nameof(Exclude100Percents):
                    return false;
                case nameof(ExcludeUntranslatedSegments):
                    return true;
                case nameof(ExcludeDraftSegments):
                    return true;
                case nameof(SourceOmitLeadingZero):
                    return false;
                case nameof(TargetOmitLeadingZero):
                    return false;
                default:
                    return base.GetDefaultValue(settingId);
            }
        }
        #endregion

        public IEnumerable<string> GetSourceDecimalSeparators()
        {
            yield return SourceDecimalComma ? @"\u002C" : string.Empty;
            yield return SourceDecimalPeriod ? @"\u002E" : string.Empty;
            yield return SourceDecimalCustomSeparator ? GetSourceDecimalCustomSeparator
                : string.Empty;
        }

        public IEnumerable<string> GetTargetDecimalSeparators()
        {
            yield return TargetDecimalComma ? @"\u002C" : string.Empty;
            yield return TargetDecimalPeriod ? @"\u002E" : string.Empty;
            yield return TargetDecimalCustomSeparator ? GetTargetDecimalCustomSeparator
                : string.Empty;
        }

        public IEnumerable<string> GetSourceThousandSeparators()
        {
            yield return SourceThousandsSpace ? @"\u0020" : string.Empty;
            yield return SourceThousandsNobreakSpace ? @"\u00A0" : string.Empty;
            yield return SourceThousandsThinSpace ? @"\u2009" : string.Empty;
            yield return SourceThousandsNobreakThinSpace ? @"\u202F" : string.Empty;
            yield return SourceThousandsComma ? @"\u002C" : string.Empty;
            yield return SourceThousandsPeriod ? @"\u002E" : string.Empty;
            yield return SourceThousandsCustomSeparator
                ? GetSourceThousandsCustomSeparator
                : string.Empty;

        }

      

        public IEnumerable<string> GetTargetThousandSeparators()
        {
            yield return TargetThousandsSpace ? @"\u0020" : string.Empty;
            yield return TargetThousandsNobreakSpace ? @"\u00A0" : string.Empty;
            yield return TargetThousandsThinSpace ? @"\u2009" : string.Empty;
            yield return TargetThousandsNobreakThinSpace ? @"\u202F" : string.Empty;
            yield return TargetThousandsComma ? @"\u002C" : string.Empty;
            yield return TargetThousandsPeriod ? @"\u002E" : string.Empty;
            yield return TargetThousandsCustomSeparator
                ? GetTargetThousandsCustomSeparator
                : string.Empty;
        }
    }
}
