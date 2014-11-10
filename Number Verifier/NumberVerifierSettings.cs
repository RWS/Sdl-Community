using Sdl.Core.Settings;

namespace Sdl.Community.NumberVerifier
{
    /// <summary>
    /// This class is used for reading and writing the plug-in setting(s) value(s).
    /// The settings are physically stored in an (XML-compliant) *.sdlproj file, which
    /// is generated for each project that is created in SDL Trados Studio or for 
    /// each file that is opened and translated.
    /// </summary>
    class NumberVerifierSettings : SettingsGroup
    {
        #region "setting"
        // Define the setting constant.
        private const string ExcludeTagText_Setting = "ExcludeTagText";
        private const string ReportAddedNumbers_Setting = "ReportAddedNumbers";
        private const string ReportRemovedNumbers_Setting = "ReportRemovedNumbers";
        private const string ReportModifiedNumbers_Setting = "ReportModifiedNumbers";
        private const string ReportModifiedAlphanumerics_Setting = "ReportModifiedAlphanumerics";
        private const string AddedNumbersErrorType_Setting = "AddedNumbersErrorType";
        private const string RemovedNumbersErrorType_Setting = "RemovedNumbersErrorType";
        private const string ModifiedNumbersErrorType_Setting = "ModifiedNumbersErrorType";
        private const string ModifiedAlphanumericsErrorType_Setting = "ModifiedAlphanumericsErrorType";
        private const string ReportBriefMessages_Setting = "ReportBriefMessages";
        private const string ReportExtendedMessages_Setting = "ReportExtendedMessages";
        private const string AllowLocalizations_Setting = "AllowLocalizations";
        private const string RequireLocalizations_Setting = "RequireLocalizations";
        private const string PreventLocalizations_Setting = "PreventLocalizations";
        private const string SourceThousandsSpace_Setting = "SourceThousandsSpace";
        private const string SourceThousandsNobreakSpace_Setting = "SourceThousandsNobreakSpace";
        private const string SourceThousandsThinSpace_Setting = "SourceThousandsThinSpace";
        private const string SourceThousandsNobreakThinSpace_Setting = "SourceThousandsNobreakThinSpace";
        private const string SourceThousandsComma_Setting = "SourceThousandsComma";
        private const string SourceThousandsPeriod_Setting = "SourceThousandsPeriod";
        private const string TargetThousandsSpace_Setting = "TargetThousandsSpace";
        private const string TargetThousandsNobreakSpace_Setting = "TargetThousandsNobreakSpace";
        private const string TargetThousandsThinSpace_Setting = "TargetThousandsThinSpace";
        private const string TargetThousandsNobreakThinSpace_Setting = "TargetThousandsNobreakThinSpace";
        private const string TargetThousandsComma_Setting = "TargetThousandsComma";
        private const string TargetThousandsPeriod_Setting = "TargetThousandsPeriod";
        private const string SourceDecimalComma_Setting = "SourceDecimalComma";
        private const string SourceDecimalPeriod_Setting = "SourceDecimalPeriod";
        private const string TargetDecimalComma_Setting = "TargetDecimalComma";
        private const string TargetDecimalPeriod_Setting = "TargetDecimalPeriod";
        private const string ExcludeLockedSegments_Setting = "ExcludeLockedSegments";
        private const string Exclude100Percents_Setting = "Exclude100Percents";


        // Return the value of the setting.
        public Setting<bool> ExcludeTagText
        {
            get { return GetSetting<bool>(ExcludeTagText_Setting); }
        }

        public Setting<bool> ReportAddedNumbers
        {
            get { return GetSetting<bool>(ReportAddedNumbers_Setting); }
        }

        public Setting<bool> ReportRemovedNumbers
        {
            get { return GetSetting<bool>(ReportRemovedNumbers_Setting); }
        }

        public Setting<bool> ReportModifiedNumbers
        {
            get { return GetSetting<bool>(ReportModifiedNumbers_Setting); }
        }

        public Setting<bool> ReportModifiedAlphanumerics
        {
            get { return GetSetting<bool>(ReportModifiedAlphanumerics_Setting); }
        }

        public Setting<string> AddedNumbersErrorType
        {
            get { return GetSetting<string>(AddedNumbersErrorType_Setting); }
        }

        public Setting<string> RemovedNumbersErrorType
        {
            get { return GetSetting<string>(RemovedNumbersErrorType_Setting); }
        }

        public Setting<string> ModifiedNumbersErrorType
        {
            get { return GetSetting<string>(ModifiedNumbersErrorType_Setting); }
        }

        public Setting<string> ModifiedAlphanumericsErrorType
        {
            get { return GetSetting<string>(ModifiedAlphanumericsErrorType_Setting); }
        }

        public Setting<bool> ReportBriefMessages
        {
            get { return GetSetting<bool>(ReportBriefMessages_Setting); }
        }

        public Setting<bool> ReportExtendedMessages
        {
            get { return GetSetting<bool>(ReportExtendedMessages_Setting); }
        }

        public Setting<bool> AllowLocalizations
        {
            get { return GetSetting<bool>(AllowLocalizations_Setting); }
        }

        public Setting<bool> PreventLocalizations
        {
            get { return GetSetting<bool>(PreventLocalizations_Setting); }
        }

        public Setting<bool> RequireLocalizations
        {
            get { return GetSetting<bool>(RequireLocalizations_Setting); }
        }

        public Setting<bool> SourceThousandsSpace
        {
            get { return GetSetting<bool>(SourceThousandsSpace_Setting); }
        }

        public Setting<bool> SourceThousandsNobreakSpace
        {
            get { return GetSetting<bool>(SourceThousandsNobreakSpace_Setting); }
        }

        public Setting<bool> SourceThousandsThinSpace
        {
            get { return GetSetting<bool>(SourceThousandsThinSpace_Setting); }
        }

        public Setting<bool> SourceThousandsNobreakThinSpace
        {
            get { return GetSetting<bool>(SourceThousandsNobreakThinSpace_Setting); }
        }

        public Setting<bool> SourceThousandsComma
        {
            get { return GetSetting<bool>(SourceThousandsComma_Setting); }
        }

        public Setting<bool> SourceThousandsPeriod
        {
            get { return GetSetting<bool>(SourceThousandsPeriod_Setting); }
        }

        public Setting<bool> TargetThousandsSpace
        {
            get { return GetSetting<bool>(TargetThousandsSpace_Setting); }
        }

        public Setting<bool> TargetThousandsNobreakSpace
        {
            get { return GetSetting<bool>(TargetThousandsNobreakSpace_Setting); }
        }

        public Setting<bool> TargetThousandsThinSpace
        {
            get { return GetSetting<bool>(TargetThousandsThinSpace_Setting); }
        }

        public Setting<bool> TargetThousandsNobreakThinSpace
        {
            get { return GetSetting<bool>(TargetThousandsNobreakThinSpace_Setting); }
        }

        public Setting<bool> TargetThousandsComma
        {
            get { return GetSetting<bool>(TargetThousandsComma_Setting); }
        }

        public Setting<bool> TargetThousandsPeriod
        {
            get { return GetSetting<bool>(TargetThousandsPeriod_Setting); }
        }
        
        public Setting<bool> SourceDecimalComma
        {
            get { return GetSetting<bool>(SourceDecimalComma_Setting); }
        }

        public Setting<bool> SourceDecimalPeriod
        {
            get { return GetSetting<bool>(SourceDecimalPeriod_Setting); }
        }

        public Setting<bool> TargetDecimalComma
        {
            get { return GetSetting<bool>(TargetDecimalComma_Setting); }
        }

        public Setting<bool> TargetDecimalPeriod
        {
            get { return GetSetting<bool>(TargetDecimalPeriod_Setting); }
        }

        public Setting<bool> ExcludeLockedSegments
        {
            get { return GetSetting<bool>(ExcludeLockedSegments_Setting); }
        }

        public Setting<bool> Exclude100Percents
        {
            get { return GetSetting<bool>(Exclude100Percents_Setting); }
        }

        #endregion

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
                case "ExcludeTagText":
                    return (bool)false;
                case "ReportAddedNumbers":
                    return (bool)true;
                case "ReportRemovedNumbers":
                    return (bool)true;
                case "ReportModifiedNumbers":
                    return (bool)true;
                case "ReportModifiedAlphanumerics":
                    return (bool)true;
                case "AddedNumbersErrorType":
                    return (string)"Warning";
                case "RemovedNumbersErrorType":
                    return (string)"Warning";
                case "ModifiedNumbersErrorType":
                    return (string)"Error";
                case "ModifiedAlphanumericsErrorType":
                    return (string)"Error";
                case "ReportBriefMessages":
                    return (bool)true;
                case "ReportExtendedMessages":
                    return (bool)false;
                case "AllowLocalizations":
                    return (bool)true;
                case "PreventLocalizations":
                    return (bool)false;
                case "RequireLocalizations":
                    return (bool)false;
                case "SourceThousandsSpace":
                    return (bool)true;
                case "SourceThousandsNobreakSpace":
                    return (bool)true;
                case "SourceThousandsThinSpace":
                    return (bool)true;
                case "SourceThousandsNobreakThinSpace":
                    return (bool)true;
                case "SourceThousandsComma":
                    return (bool)true;
                case "SourceThousandsPeriod":
                    return (bool)true;
                case "TargetThousandsSpace":
                    return (bool)true;
                case "TargetThousandsNobreakSpace":
                    return (bool)true;
                case "TargetThousandsThinSpace":
                    return (bool)true;
                case "TargetThousandsNobreakThinSpace":
                    return (bool)true;
                case "TargetThousandsComma":
                    return (bool)true;
                case "TargetThousandsPeriod":
                    return (bool)true;
                case "SourceDecimalComma":
                    return (bool)true;
                case "SourceDecimalPeriod":
                    return (bool)true;
                case "TargetDecimalComma":
                    return (bool)true;
                case "TargetDecimalPeriod":
                    return (bool)true;
                case "ExcludeLockedSegments":
                    return (bool)false;
                case "Exclude100Percents":
                    return (bool)false;
                default:
                    return base.GetDefaultValue(settingId);
            }
        }
        #endregion
    }
}
