using System;

namespace Sdl.Community.XliffReadWrite
{
    [Serializable]
    public class Settings
    {

        public Settings()
        {       
            FilePathOriginal = string.Empty;
            FilePathUpdated = string.Empty;

            
            ChangedTranslationStatus = "Draft";

            IgnoreEmptyTranslations = false;

            NotChangedTranslationStatus = string.Empty;

            NotImportedTranslationStatus = string.Empty;

            CreateBackupOfSdlXliffFile = true;

            ReportDifferencesInTagsPlaceables = true;

            CopySourceToTargetEmptyTranslations = true;

            UndoAllChangesOnException = true;

            DoNotExportPerfectMatch = false;
            DoNotExportContextMatch = false;
            DoNotExportExactMatch = false;
            DoNotExportFuzzyMatch = false;
            DoNotExportNoMatch = false;
            DoNotExportLocked = false;
            DoNotExportUnLocked = false;

            DoNotExportNotTranslated = false;
            DoNotExportDraft = false;
            DoNotExportTranslated = false;
            DoNotExportTranslationRejected = false;
            DoNotExportTranslationApproved = false;
            DoNotExportSignOffRejected = false;
            DoNotExportSignOff = false;


            DoNotImportPerfectMatch = true;
            DoNotImportContextMatch = true;
            DoNotImportExactMatch = false;
            DoNotImportFuzzyMatch = false;
            DoNotImportNoMatch = false;
            DoNotImportLocked = true;
            DoNotImportUnLocked = false;

            DoNotImportNotTranslated = false;
            DoNotImportDraft = false;
            DoNotImportTranslated = false;
            DoNotImportTranslationRejected = false;
            DoNotImportTranslationApproved = false;
            DoNotImportSignOffRejected = false;
            DoNotImportSignOff = false;


            DefaultSettingsConverToFormat = 0;
            DefaultSettingsIncludeLegacyStructure = true;
            DefaultSettingsReverseLanguageDirection = false;
            DefaultSettingsViewReportWhenProcessingFinished = true;
            DefaultSettingsCreateBaKfile = true;
            DefaultSettingsExcludeTags = false;

          
        }


        public int DefaultSettingsConverToFormat { get; set; }
        public bool DefaultSettingsIncludeLegacyStructure { get; set; }
        public bool DefaultSettingsReverseLanguageDirection { get; set; }
        public bool DefaultSettingsViewReportWhenProcessingFinished { get; set; }
        public bool DefaultSettingsCreateBaKfile { get; set; }
        public bool DefaultSettingsExcludeTags { get; set; }

     
        public string ChangedTranslationStatus { get; set; }
        public string NotChangedTranslationStatus { get; set; }
        public string NotImportedTranslationStatus { get; set; }

        public bool CreateBackupOfSdlXliffFile { get; set; }

        public bool ReportDifferencesInTagsPlaceables { get; set; }

        public bool UndoAllChangesOnException { get; set; }  

        public string FilePathOriginal { get; set; }
        public string FilePathUpdated { get; set; }

        public bool CopySourceToTargetEmptyTranslations { get; set; }

        public bool IgnoreEmptyTranslations { get; set; }

        public bool DoNotExportPerfectMatch { get; set; }
        public bool DoNotExportContextMatch { get; set; }
        public bool DoNotExportExactMatch { get; set; }
        public bool DoNotExportFuzzyMatch { get; set; }
        public bool DoNotExportNoMatch { get; set; }
        public bool DoNotExportLocked { get; set; }
        public bool DoNotExportUnLocked { get; set; }

        public bool DoNotExportNotTranslated { get; set; }
        public bool DoNotExportDraft { get; set; }
        public bool DoNotExportTranslated { get; set; }
        public bool DoNotExportTranslationRejected { get; set; }
        public bool DoNotExportTranslationApproved { get; set; }
        public bool DoNotExportSignOffRejected { get; set; }
        public bool DoNotExportSignOff { get; set; }


        public bool DoNotImportPerfectMatch { get; set; }
        public bool DoNotImportContextMatch { get; set; }
        public bool DoNotImportExactMatch { get; set; }
        public bool DoNotImportFuzzyMatch { get; set; }
        public bool DoNotImportNoMatch { get; set; }
        public bool DoNotImportLocked { get; set; }
        public bool DoNotImportUnLocked { get; set; }

        public bool DoNotImportNotTranslated { get; set; }
        public bool DoNotImportDraft { get; set; }
        public bool DoNotImportTranslated { get; set; }
        public bool DoNotImportTranslationRejected { get; set; }
        public bool DoNotImportTranslationApproved { get; set; }
        public bool DoNotImportSignOffRejected { get; set; }
        public bool DoNotImportSignOff { get; set; }

    }
}
