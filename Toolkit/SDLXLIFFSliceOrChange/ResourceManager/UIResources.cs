using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace SDLXLIFFSliceOrChange.ResourceManager
{
    public class UIResources
    {
        #region public props
        private Dictionary<String, String> _defaultValues; 

        public string Browse { get { return GetString("Browse"); } }
        public string Slice { get { return GetString("Slice"); } }
        public string Sliceit { get { return GetString("Sliceit"); } }
        public string Change { get { return GetString("Change"); } }
        public string ChangeToStatusOr { get { return GetString("ChangeToStatusOr"); } }
        public string ChangeLocked { get { return GetString("ChangeLocked"); } }
        public string ChangeToUnlocked { get { return GetString("ChangeToUnlocked"); } }
        public string ChangeToLocked { get { return GetString("ChangeToLocked"); } }
        public string ChangeTranslationStatus { get { return GetString("ChangeTranslationStatus"); } }
        public string ChangeToSignedOff { get { return GetString("ChangeToSignedOff"); } }
        public string ChangeToSignOffRejected { get { return GetString("ChangeToSignOffRejected"); } }
        public string ChangeToTranslationApproved { get { return GetString("ChangeToTranslationApproved"); } }
        public string ChangeToTranslationRejected { get { return GetString("ChangeToTranslationRejected"); } }
        public string ChangeToTranslated { get { return GetString("ChangeToTranslated"); } }
        public string ChangeToDraft { get { return GetString("ChangeToDraft"); } }
        public string ChangeToNotTranslated { get { return GetString("ChangeToNotTranslated"); } }
        public string Changeit { get { return GetString("Changeit"); } }
        public string Statuses { get { return GetString("Statuses"); } }
        public string ReverseSelection { get { return GetString("ReverseSelection"); } }
        public string DocumentStructure { get { return GetString("DocumentStructure"); } }
        public string GenerateDSI { get { return GetString("GenerateDSI"); } }
        public string System { get { return GetString("System"); } }
        public string Propagated { get { return GetString("Propagated"); } }
        public string SystemTranslationMemory { get { return GetString("SystemTranslationMemory"); } }
        public string SystemMachineTranslation { get { return GetString("SystemMachineTranslation"); } }
        public string TranslationOrigin { get { return GetString("TranslationOrigin"); } }
        public string AutoPropagated { get { return GetString("AutoPropagated"); } }
        public string AutomatedTranslation { get { return GetString("AutomatedTranslation"); } }
        public string Interactive { get { return GetString("Interactive"); } }
        public string TranslationMemory { get { return GetString("TranslationMemory"); } }
        public string Score { get { return GetString("Score"); } }
        public string MatchValues { get { return GetString("MatchValues"); } }
        public string ContextMatch { get { return GetString("ContextMatch"); } }
        public string PerfectMatch { get { return GetString("PerfectMatch"); } }
        public string StatusesLocked { get { return GetString("StatusesLocked"); } }
        public string Unlocked { get { return GetString("Unlocked"); } }
        public string Locked { get { return GetString("Locked"); } }
        public string StatusesTranslationStatus { get { return GetString("StatusesTranslationStatus"); } }

        public string ApprovedSignOff { get { return GetString("ApprovedSignOff"); } }
        public string RejectedSignOff { get { return GetString("RejectedSignOff"); } }
        public string ApprovedTranslation { get { return GetString("ApprovedTranslation"); } }
        public string RejectedTranslation { get { return GetString("RejectedTranslation"); } }
        public string Translated { get { return GetString("Translated"); } }
        public string Draft { get { return GetString("Draft"); } }
        public string NotTranslated { get { return GetString("NotTranslated"); } }

        public string FindAll { get { return GetString("FindAll"); } }
        public string Options { get { return GetString("Options"); } }
        public string SearchInTags { get { return GetString("SearchInTags"); } }
        public string SearchRegEx { get { return GetString("SearchRegEx"); } }
        public string SearchMatchWholeWord { get { return GetString("SearchMatchWholeWord"); } }
        public string SearchMatchCase { get { return GetString("SearchMatchCase"); } }
        public string Search { get { return GetString("Search"); } }
        public string FileLocation { get { return GetString("FileLocation"); } }
        public string Name { get { return GetString("Name"); } }
        public string Size { get { return GetString("Size"); } }
        public string Date { get { return GetString("Date"); } }
        public string SliceComments { get { return GetString("SliceComments"); } }
        public string ChangeComments { get { return GetString("ChangeComments"); } }
        public string GenerateDSIComments { get { return GetString("GenerateDSIComments"); } }
        public string Seg { get { return GetString("Seg"); } }
        public string Status { get { return GetString("Status"); } }
        public string SearchResultsSource { get { return GetString("SearchResultsSource"); } }
        public string SearchResultsTarget { get { return GetString("SearchResultsTarget"); } }
        public string SearchTarget { get { return GetString("SearchTarget"); } }
        public string SearchSource { get { return GetString("SearchSource"); } }
        public string SelectFolder { get { return GetString("SelectFolder"); } }
        public string ProjectFile { get { return GetString("ProjectFile"); } }
        public string PleaseWait { get { return GetString("PleaseWait"); } }
        public string OR { get { return GetString("OR"); } }
        public string AND { get { return GetString("AND"); } }
        public string Clear { get { return GetString("Clear"); } }
        public string Clearit { get { return GetString("Clearit"); } }
        public string ClearitDescription { get { return GetString("ClearitDescription"); } }
        public string ErrorNoFilesSelected { get { return GetString("Error_NoFilesSelected"); } }
        public string ErrorTitleNoFilesSelected { get { return GetString("Error_TitleNoFilesSelected"); } }
        
        public string ReplaceWith { get { return GetString("ReplaceWith"); } }
        public string Preview { get { return GetString("Preview"); } }
        public string Replace { get { return GetString("Replace"); } }
        public string ReplaceGroup { get { return GetString("ReplaceGroup"); } }
        public string CopySourceToTarget { get { return GetString("CopySourceToTarget"); } }
        public string Merge { get { return GetString("Merge"); } }
        public string ClearFiles { get { return GetString("ClearFiles"); } }
        public string SelectXLIFF { get { return GetString("SelectXLIFF"); } }
        #endregion

        #region prive things and .ctor
        private readonly string _culture;

        private string File
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SDLXLIFF.{0}.resx"); }
        }

        private DataSet _resources;

        public UIResources(String culture)
        {
            try
            {
                _culture = culture;
                InitDefaultValues();
                LoadResources();

            } catch
            {
            }
        }

        private void InitDefaultValues()
        {
            _defaultValues = new Dictionary<string, string>();
            _defaultValues.Add("Browse", "Browse for SDLXLIFFs ...");
            _defaultValues.Add("Slice", "Slice");
            _defaultValues.Add("Sliceit", "Sliceit!");
            _defaultValues.Add("Change", "Change");
            _defaultValues.Add("ChangeToStatusOr", "Change to Status and/or Lock value");
            _defaultValues.Add("ChangeLocked", "Locked / Unlocked");
            _defaultValues.Add("ChangeToUnlocked", "Unlock content");
            _defaultValues.Add("ChangeToLocked", "Lock segment");
            _defaultValues.Add("ChangeTranslationStatus", "Translation Status");
            _defaultValues.Add("ChangeToSignedOff", "Signed Off");
            _defaultValues.Add("ChangeToSignOffRejected", "Sign-off Rejected");
            _defaultValues.Add("ChangeToTranslationApproved", "Translation Approved");
            _defaultValues.Add("ChangeToTranslationRejected", "Translation Rejected");
            _defaultValues.Add("ChangeToTranslated", "Translated");
            _defaultValues.Add("ChangeToDraft", "Draft");
            _defaultValues.Add("ChangeToNotTranslated", "Not translated");
            _defaultValues.Add("Changeit", "Changeit!");
            _defaultValues.Add("Statuses", "Statuses");
            _defaultValues.Add("ReverseSelection", "Reverse selection");
            _defaultValues.Add("Error_NoFilesSelected", "No files selected");
            _defaultValues.Add("DocumentStructure", "Document Structure");
            _defaultValues.Add("GenerateDSI", "Generate DSI");
            _defaultValues.Add("System", "System");
            _defaultValues.Add("Propagated", "Propagated");
            _defaultValues.Add("SystemTranslationMemory", "Translation Memory");
            _defaultValues.Add("SystemMachineTranslation", "Machine Translation");
            _defaultValues.Add("TranslationOrigin", "Translation Origin");
            _defaultValues.Add("AutoPropagated", "Auto-Propagated");
            _defaultValues.Add("AutomatedTranslation", "Automated Translation");
            _defaultValues.Add("Interactive", "Interactive");
            _defaultValues.Add("TranslationMemory", "Translation Memory");
            _defaultValues.Add("Score", "Score");
            _defaultValues.Add("MatchValues", "Match Value(s)");
            _defaultValues.Add("ContextMatch", "Context Match");
            _defaultValues.Add("PerfectMatch", "Perfect Match");
            _defaultValues.Add("StatusesLocked", "Locked / Unlocked");
            _defaultValues.Add("Unlocked", "Unlocked");
            _defaultValues.Add("Locked", "Locked");
            _defaultValues.Add("StatusesTranslationStatus", "Translation Status");
            _defaultValues.Add("ApprovedSignOff", "Signed Off");
            _defaultValues.Add("RejectedSignOff", "Sign-off Rejected");
            _defaultValues.Add("ApprovedTranslation", "Translation Approved");
            _defaultValues.Add("RejectedTranslation", "Translation Rejected");
            _defaultValues.Add("Error_TitleNoFilesSelected", "Error");
            _defaultValues.Add("Translated", "Translated");
            _defaultValues.Add("Draft", "Draft");
            _defaultValues.Add("NotTranslated", "Not translated");
            _defaultValues.Add("Search", "Search");
            _defaultValues.Add("FindAll", "Find All");
            _defaultValues.Add("Options", "Options");
            _defaultValues.Add("SearchInTags", "Search in tags");
            _defaultValues.Add("SearchRegEx", "Use regular expressions");
            _defaultValues.Add("SearchMatchWholeWord", "Match whole word");
            _defaultValues.Add("SearchMatchCase", "Match case");
            _defaultValues.Add("FileLocation", "Column1");
            _defaultValues.Add("Name", "Name");
            _defaultValues.Add("Size", "Size");
            _defaultValues.Add("Date", "Date");
            _defaultValues.Add("SliceComments", "Click Sliceit! to create a new SDLXLIFF file based on your selection criteria. You can add this file to your Project or share it with others to handle specific segments first.");
            _defaultValues.Add("ChangeComments", "Click Changeit! to change the selected segments, to a specific Translation Status or to lock or unlock them.");
            _defaultValues.Add("GenerateDSIComments", "Click \"Generate DSI\" to generate a list of all different types of structure information used in the selected files. Hold Ctrl key down and select the ones you want with the mouse.");
            _defaultValues.Add("Seg", "Seg#");
            _defaultValues.Add("Status", "Status");
            _defaultValues.Add("SearchResultsSource", "Search Results");
            _defaultValues.Add("SearchResultsTarget", "");
            _defaultValues.Add("SearchTarget", "Target search");
            _defaultValues.Add("SearchSource", "Source search");
            _defaultValues.Add("SelectFolder", "Select folder ...");
            _defaultValues.Add("ProjectFile", "Select project file ...");
            _defaultValues.Add("PleaseWait", "Please Wait ...");
            _defaultValues.Add("OR", "OR");
            _defaultValues.Add("AND", "AND");
            _defaultValues.Add("Clear", "Clear");
            _defaultValues.Add("Clearit", "Clearit!");
            _defaultValues.Add("ClearitDescription", "Click Clearit! to clear all the translated segments based on your selection criteria.");

        }

        private void LoadResources()
        {
            if (_resources == null)
            {
                _resources = new DataSet();
                _resources.ReadXml(String.Format(File, _culture));
            }
        }

        public String GetString(String token)
        {
            if (_resources == null) return Getefault(token);
            try
            {
                DataTable values = _resources.Tables["data"];
                DataRow[] rows = values.Select(String.Format("name = '{0}'", token));
                if (rows.Length == 0)
                {
                    return Getefault(token);
                }
                return rows[0]["Value"].ToString();

            } catch
            {
                return Getefault(token);
            }
        }

        private string Getefault(string token)
        {
            if (_defaultValues != null && _defaultValues.ContainsKey(token)) return _defaultValues[token];
            return token;
        }

        #endregion
    }
}

