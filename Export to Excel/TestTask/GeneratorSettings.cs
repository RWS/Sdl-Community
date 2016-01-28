using System.Collections.Generic;
using System.Drawing;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.ProjectAutomation.AutomaticTasks;

namespace ExportToExcel
{
    public class GeneratorSettings : SettingsGroup
    {
       
        public enum ExportType { Excel, Word, Xml }
        public enum ExclusionType { Category, Status }
        public enum UpdateSegmentMode { All, TrackedOnly }

        private readonly decimal _columnWidth = 75;
        private readonly string _fileNamePrefix = "Generated_";

        /// <summary>
        /// Resets the settings to default
        /// </summary>
        public void ResetToDefaults()
        {
            FileNamePrefix = "Generated_";
            TableLayoutType = LayoutType.TableType.TopDown;
            ColumnWidth = 75;
            ExtractComments = true;
            IsContextMatchLocked = false;
            IsExactMatchLocked = false;
            IsFuzzyMatchLocked = false;
            IsNoMatchLocked = false;
            DontExportContext = false;
            DontExportExact = false;
            DontExportFuzzy = false;
            DontExportNoMatch = false;
            UpdateSegmentStatusNoTracked = false;
            UpdateSegmentStatusTracked = false;
            NewSegmentStatusAll = ConfirmationLevel.Draft;
            NewSegmentStatusTrackedChanges = ConfirmationLevel.Draft;
            ExcludeExportType = ExclusionType.Category;
            ExcludedStatuses = new List<ConfirmationLevel>();
            ImportUpdateSegmentMode = UpdateSegmentMode.TrackedOnly;
            WarningWhenOverwrite = true;
            BackupImport = false;
            ApprovedSignOff = false;
            RejectedSignOff = false;
            Draft = false;
            RejectedTranslation = false;
            Translated = false;
            Unspecified = false;
        }

        /// <summary>
        /// Prefix which will be used for generated file names.
        /// </summary>
        public string FileNamePrefix
        {
            get { return GetSetting<string>(nameof(FileNamePrefix)); }
            set { GetSetting<string>(nameof(FileNamePrefix)).Value = value; }
        }

        /// <summary>
        /// Column width for generated excel document
        /// </summary>
        public decimal ColumnWidth
        {
            get { return GetSetting<decimal>(nameof(ColumnWidth)); }
            set { GetSetting<decimal>(nameof(ColumnWidth)).Value = value; }
        }

        /// <summary>
        /// Specify if comments from the bilingual file should be extracted
        /// </summary>
        public bool ExtractComments
        {
            get { return GetSetting<bool>(nameof(ExtractComments)); }
            set { GetSetting<bool>(nameof(ExtractComments)).Value = value; }
            
        }

        public bool ExcludeStatus
        {
            get { return GetSetting<bool>(nameof(ExcludeStatus)); }
        }
        public bool ExclusionTypeCategory
        {
            get { return GetSetting<bool>(nameof(ExclusionTypeCategory)); }
        }

        public bool ExclusionTypeCategoryStatus
        {
            get { return GetSetting<bool>(nameof(ExclusionTypeCategoryStatus)); }
        }

        public bool DontExportContext {
            get { return GetSetting<bool>(nameof(DontExportContext)); }
            set { GetSetting<bool>(nameof(DontExportContext)).Value = value; }
        }

        public bool DontExportExact
        {
            get { return GetSetting<bool>(nameof(DontExportExact)); }
            set { GetSetting<bool>(nameof(DontExportExact)).Value = value; }
        }

        public bool DontExportFuzzy
        {
            get { return GetSetting<bool>(nameof(DontExportFuzzy)); }
            set { GetSetting<bool>(nameof(DontExportFuzzy)).Value = value; }
        }

        public bool DontExportNoMatch
        {
            get { return GetSetting<bool>(nameof(DontExportNoMatch)); }
            set { GetSetting<bool>(nameof(DontExportNoMatch)).Value = value; }
        }

        public bool ApprovedSignOff
        {
            get { return GetSetting<bool>(nameof(ApprovedSignOff)); }
            set { GetSetting<bool>(nameof(ApprovedSignOff)).Value = value; }
        }

        public bool ApprovedTranslation
        {
            get { return GetSetting<bool>(nameof(ApprovedTranslation)); }
            set { GetSetting<bool>(nameof(ApprovedTranslation)).Value = value; }
        }

        public bool Draft
        {
            get { return GetSetting<bool>(nameof(Draft)); }
            set { GetSetting<bool>(nameof(Draft)).Value = value; }
        }

        public bool RejectedSignOff
        {
            get { return GetSetting<bool>(nameof(RejectedSignOff)); }
            set { GetSetting<bool>(nameof(RejectedSignOff)).Value = value; }
        }

        public bool RejectedTranslation
        {
            get { return GetSetting<bool>(nameof(RejectedSignOff)); }
            set { GetSetting<bool>(nameof(RejectedTranslation)).Value = value; }
        }

        public bool Translated
        {
            get { return GetSetting<bool>(nameof(Translated)); }
            set { GetSetting<bool>(nameof(Translated)).Value = value; }
        }

        public bool Unspecified
        {
            get { return GetSetting<bool>(nameof(Unspecified)); }
            set { GetSetting<bool>(nameof(Unspecified)).Value = value; }
        }
 

        /// <summary>
        /// Specify if the no matches will be locked
        /// </summary>
        public bool IsNoMatchLocked
        {
            get;
            set;
        }

        /// <summary>
        /// Specify if the fuzzy matches will be locked
        /// </summary>
        public bool IsFuzzyMatchLocked
        {
            get;
            set;
        }

        /// <summary>
        /// Specify if the exact matches will be locked
        /// </summary>
        public bool IsExactMatchLocked
        {
            get;
            set;
        }

        /// <summary>
        /// Specify if the context/perfect matches will be locked
        /// </summary>
        public bool IsContextMatchLocked
        {
            get;
            set;
        }

        /// <summary>
        /// Color used to represent no match
        /// </summary>
        public Color NoMatchColor{get; set; } = Color.White;



        /// <summary>
        /// Color used to represent fuzzy match
        /// </summary>
        public Color FuzzyMatchColor { get; set; } = Color.Wheat;


        /// <summary>
        /// Color used to represent exact match
        /// </summary>
        public Color ExactMatchColor { get; set; } = Color.PaleGreen;


        /// <summary>
        /// Color used to represent context/perfect match
        /// </summary>
        public Color ContextMatchColor { get; set; } = Color.LightGray;


        public ConfirmationLevel NewSegmentStatusAll
        {
            get;
            set;
        }

        public ConfirmationLevel NewSegmentStatusTrackedChanges
        {
            get;
            set;
        }

        public LayoutType.TableType TableLayoutType
        {
            get;
            set;
        }

        public ExclusionType ExcludeExportType
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a list used later for data binding
        /// </summary>
        public List<ConfirmationLevel> ExcludedStatuses
        {
            get
            {
                var confirmationLevels = new List<ConfirmationLevel>();
                if (ApprovedSignOff)
                    confirmationLevels.Add(ConfirmationLevel.ApprovedSignOff);
                if(ApprovedTranslation)
                    confirmationLevels.Add(ConfirmationLevel.ApprovedTranslation);
                if(Draft)
                    confirmationLevels.Add(ConfirmationLevel.Draft);
                if (RejectedSignOff)
                    confirmationLevels.Add(ConfirmationLevel.RejectedSignOff);
                if(RejectedTranslation)
                    confirmationLevels.Add(ConfirmationLevel.RejectedTranslation);
                if(Translated)
                    confirmationLevels.Add(ConfirmationLevel.Translated);
                if(Unspecified)
                    confirmationLevels.Add(ConfirmationLevel.Unspecified);
                return confirmationLevels;
            }
            set
            {
                GetSetting<List<ConfirmationLevel>>(nameof(ExcludedStatuses)).Value = value;
            }
            
        }

        public UpdateSegmentMode ImportUpdateSegmentMode
        {
            get;
            set;
        }

        public bool WarningWhenOverwrite
        {
            get;
            set;
        }

        public bool BackupImport
        {
            get;
            set;
        }

        public bool UpdateSegmentStatusTracked
        {
            get;
            set;
        }

        public bool UpdateSegmentStatusNoTracked
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the value for checkboxes
        /// </summary>
        /// <param name="status">Name of the checkbox</param>
        /// <param name="value">true/false</param>
        public void SetSegmentStatus(ConfirmationLevel status, bool value)
        {
            switch (status)
            {
                case ConfirmationLevel.ApprovedSignOff:
                    ApprovedSignOff = value;
                    break;
                case ConfirmationLevel.RejectedSignOff:
                    RejectedSignOff = value;
                    break;
                    case ConfirmationLevel.ApprovedTranslation:
                    ApprovedTranslation = value;
                    break;
                    case ConfirmationLevel.Draft:
                    Draft = value;
                    break;
                    case ConfirmationLevel.Translated:
                    Translated = value;
                    break;
                    case ConfirmationLevel.RejectedTranslation:
                    RejectedTranslation = value;
                    break;
                    case ConfirmationLevel.Unspecified:
                    Unspecified = value;
                    break;

            }
        }

        /// <summary>
        /// Sets the default values
        /// If you change the values on Ui, labels will be displayed with bold.
        /// </summary>
        /// <param name="settingId">The name of the setting</param>
        /// <returns></returns>
        protected override object GetDefaultValue(string settingId)
        {
            switch (settingId)
            {
                case nameof(ColumnWidth):
                    return _columnWidth;
                case nameof(ExcludeStatus):
                    return false;
                case nameof(ExtractComments):
                    return true;
                case nameof(ExclusionTypeCategoryStatus):
                    return false;
                case nameof(ExclusionTypeCategory):
                    return false;
                case nameof(DontExportFuzzy):
                    return false;
                case nameof(DontExportContext):
                    return false;
                case nameof(DontExportExact):
                    return false;
                case nameof(DontExportNoMatch):
                    return false;
                case nameof(FileNamePrefix):
                    return _fileNamePrefix;
            }
            return base.GetDefaultValue(settingId);
        }
    }
}

