using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Sdl.Community.PostEdit.Compare.Core
{


    [Serializable]
    public class Settings
    {

        public enum ComparisonType
        {
            Words = 0,
            Characters
        }
        public enum ReportFormat
        {
            Html = 0,
            Xml
        }

        public enum TagVisual
        {
            Empty = 0,
            Partial = 1,
            Full
        }
        public enum TagType
        {
            Opening,
            Closing,
            Standalone,
            Undefined
        }

        public Settings()
        {
            ApplicationSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");
            ApplicationSettingsPathTm = Path.Combine(ApplicationSettingsPath, "TM");
            ApplicationSettingsPathImages = Path.Combine(ApplicationSettingsPath, "Images");

            if (!Directory.Exists(ApplicationSettingsPath))
                Directory.CreateDirectory(ApplicationSettingsPath);

            if (!Directory.Exists(ApplicationSettingsPathTm))
                Directory.CreateDirectory(ApplicationSettingsPathTm);

            if (!Directory.Exists(ApplicationSettingsPathImages))
                Directory.CreateDirectory(ApplicationSettingsPathImages);

            ChangedTranslationStatus = "Draft";
            NotChangedTranslationStatus = string.Empty;
            NotImportedTranslationStatus = string.Empty;



            #region  |  new text style  |
            StyleNewText = new Settings.DifferencesFormatting();
            StyleNewText.StyleBold = "Deactivate";
            StyleNewText.StyleItalic = "Deactivate";
            StyleNewText.StyleStrikethrough = "Deactivate";
            StyleNewText.StyleUnderline = "Activate";
            StyleNewText.TextPosition = "Normal";
            StyleNewText.FontSpecifyColor = true;
            StyleNewText.FontColor = "#0000FF";
            StyleNewText.FontSpecifyBackroundColor = true;
            StyleNewText.FontBackroundColor = "#FFFF66";
            #endregion

            #region  |  removed text style  |
            StyleRemovedText = new Settings.DifferencesFormatting();
            StyleRemovedText.StyleBold = "Deactivate";
            StyleRemovedText.StyleItalic = "Deactivate";
            StyleRemovedText.StyleStrikethrough = "Activate";
            StyleRemovedText.StyleUnderline = "Deactivate";
            StyleRemovedText.TextPosition = "Normal";
            StyleRemovedText.FontSpecifyColor = true;
            StyleRemovedText.FontColor = "#FF0000";
            StyleRemovedText.FontSpecifyBackroundColor = false;
            StyleRemovedText.FontBackroundColor = "#FFFFFF";
            #endregion

            #region  |  new tag style  |
            StyleNewTag = new Settings.DifferencesFormatting();
            StyleNewTag.StyleBold = "Deactivate";
            StyleNewTag.StyleItalic = "Deactivate";
            StyleNewTag.StyleStrikethrough = "Deactivate";
            StyleNewTag.StyleUnderline = "Deactivate";
            StyleNewTag.TextPosition = "Normal";
            StyleNewTag.FontSpecifyColor = false;
            StyleNewTag.FontColor = "#000000";
            StyleNewTag.FontSpecifyBackroundColor = true;
            StyleNewTag.FontBackroundColor = "#DDEEFF";
            #endregion

            #region  |  removed tag style  |
            StyleRemovedTag = new Settings.DifferencesFormatting();
            StyleRemovedTag.StyleBold = "Deactivate";
            StyleRemovedTag.StyleItalic = "Deactivate";
            StyleRemovedTag.StyleStrikethrough = "Deactivate";
            StyleRemovedTag.StyleUnderline = "Deactivate";
            StyleRemovedTag.TextPosition = "Normal";
            StyleRemovedTag.FontSpecifyColor = false;
            StyleRemovedTag.FontColor = "#000000";
            StyleRemovedTag.FontSpecifyBackroundColor = true;
            StyleRemovedTag.FontBackroundColor = "#FFE8E8";

            #endregion

            ReportFilterFilesWithNoRecordsFiltered = true;
            ShowGoogleChartsInReport = true;
            CalculateSummaryAnalysisBasedOnFilteredRows = true;

            ReportFilterSegmentsWithNoChanges = false;
            ReportFilterChangedTargetContent = true;
            ReportFilterSegmentStatusChanged = false;
            ReportFilterSegmentsContainingComments = true;
            ReportFilterLockedSegments = true;

            ReportFilterTranslationMatchValuesOriginal = "{All}";
            ReportFilterTranslationMatchValuesUpdated = "{All}";

            comparisonType = ComparisonType.Words;
            ComparisonIncludeTags = true;
            IncludeIndividualFileInformation = true;


            FilePathOriginal = string.Empty;
            FilePathUpdated = string.Empty;

            DirectoryPathOriginal = string.Empty;
            DirectoryPathUpdated = string.Empty;
            SearchSubFolders = true;

            ReportDirectory = string.Empty;
            ReportFileName = string.Empty;
            reportFormat = ReportFormat.Html;
            ViewReportWhenFinishedProcessing = true;

            UseCustomStyleSheet = false;
            FilePathCustomStyleSheet = string.Empty;

            ShowSegmentLocked = false;
            ShowSegmentStatus = true;
            ShowSegmentMatch = true;
            ShowSegmentTerp = true;
            JavaExecutablePath = string.Empty;
            ShowSegmentPem = true;
            ShowOriginalSourceSegment = true;
            ShowOriginalTargetSegment = false;
            ShowOriginalRevisionMarkerTargetSegment = false;

            ShowUpdatedTargetSegment = true;
            ShowUpdatedRevisionMarkerTargetSegment = false;
            ShowTargetComparison = true;
            ShowSegmentComments = true;

            TagVisualStyle= TagVisual.Partial;
            

            TrackChangesTimeSpanIdleMinutes = 30;
            TrackChangesTimeSpanIdle = true;

        }

        public string ReportFilterTranslationMatchValuesOriginal { get; set; }
        public string ReportFilterTranslationMatchValuesUpdated { get; set; }

        public string ChangedTranslationStatus { get; set; }
        public string NotChangedTranslationStatus { get; set; }
        public string NotImportedTranslationStatus { get; set; }

        public string ApplicationSettingsPath { get; set; }
        public string ApplicationSettingsPathTm { get; set; }
        public string ApplicationSettingsPathImages { get; set; }

        public DifferencesFormatting StyleNewText { get; set; }
        public DifferencesFormatting StyleRemovedText { get; set; }
        public DifferencesFormatting StyleNewTag { get; set; }
        public DifferencesFormatting StyleRemovedTag { get; set; }


        public bool ReportFilterFilesWithNoRecordsFiltered { get; set; }
        public bool ShowGoogleChartsInReport { get; set; }
        public bool CalculateSummaryAnalysisBasedOnFilteredRows { get; set; }

        public bool ReportFilterSegmentsWithNoChanges { get; set; }
        public bool ReportFilterChangedTargetContent { get; set; }
        public bool ReportFilterSegmentStatusChanged { get; set; }
        public bool ReportFilterSegmentsContainingComments { get; set; }
        public bool ReportFilterLockedSegments { get; set; }


        public bool ShowSegmentLocked { get; set; }
        public bool ShowSegmentStatus { get; set; }
        public bool ShowSegmentMatch { get; set; }
        public bool ShowSegmentTerp { get; set; }
        public string JavaExecutablePath { get; set; }
        public bool ShowSegmentPem { get; set; }
        public bool ShowOriginalSourceSegment { get; set; }
        public bool ShowOriginalTargetSegment { get; set; }
        public bool ShowOriginalRevisionMarkerTargetSegment { get; set; }
        public bool ShowUpdatedTargetSegment { get; set; }
        public bool ShowUpdatedRevisionMarkerTargetSegment { get; set; }
        public bool ShowTargetComparison { get; set; }
        public bool ShowSegmentComments { get; set; }

        public TagVisual TagVisualStyle { get; set; }



        public int TrackChangesTimeSpanIdleMinutes { get; set; }
        public bool TrackChangesTimeSpanIdle { get; set; }

        public ComparisonType comparisonType { get; set; }
        public bool ComparisonIncludeTags { get; set; }
        public bool IncludeIndividualFileInformation { get; set; }

        public string FilePathOriginal { get; set; }
        public string FilePathUpdated { get; set; }

        public string DirectoryPathOriginal { get; set; }
        public string DirectoryPathUpdated { get; set; }
        public bool SearchSubFolders { get; set; }

        public string ReportDirectory { get; set; }
        public string ReportFileName { get; set; }
        public ReportFormat reportFormat { get; set; }
        public bool ViewReportWhenFinishedProcessing { get; set; }

        public bool UseCustomStyleSheet { get; set; }
        public string FilePathCustomStyleSheet { get; set; }





        [Serializable]
        public class DifferencesFormatting
        {
            public string StyleBold { get; set; }
            public string StyleItalic { get; set; }
            public string StyleStrikethrough { get; set; }
            public string StyleUnderline { get; set; }
            public string TextPosition { get; set; }

            public bool FontSpecifyColor { get; set; }
            public string FontColor { get; set; }
            public bool FontSpecifyBackroundColor { get; set; }
            public string FontBackroundColor { get; set; }

            public DifferencesFormatting()
            {
                StyleBold = "Deactivate";
                StyleItalic = "Deactivate";
                StyleStrikethrough = "Deactivate";
                StyleUnderline = "Deactivate";

                TextPosition = "Normal";

                FontSpecifyColor = false;
                FontColor = "000000";
                FontSpecifyBackroundColor = false;
                FontBackroundColor = "FFFFFF";
            }
        }

        [Serializable]
        public class PriceGroup : ICloneable
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Currency { get; set; }


            public AnalysisBand DefaultAnalysisBand { get; set; }

            public List<Price> GroupPrices { get; set; }

            public List<Language> SourceLanguages { get; set; }
            public List<Language> TargetLanguages { get; set; }

            public PriceGroup()
            {
                Id = Guid.NewGuid().ToString();
                Name = string.Empty;
                Description = string.Empty;
                Currency = string.Empty;


                DefaultAnalysisBand = new AnalysisBand();

                GroupPrices = new List<Price>();

                SourceLanguages = new List<Language>();
                TargetLanguages = new List<Language>();
            }
            public object Clone()
            {
                var priceGroup = new PriceGroup
                {
                    Id = Id,
                    Name = Name,
                    Description = Description,
                    Currency = Currency,
                    DefaultAnalysisBand = (AnalysisBand)this.DefaultAnalysisBand.Clone(),
                    GroupPrices = new List<Price>()
                };



                foreach (var price in this.GroupPrices)
                    priceGroup.GroupPrices.Add((Price)price.Clone());

                priceGroup.SourceLanguages = new List<Language>();
                foreach (var language in this.SourceLanguages)
                    priceGroup.SourceLanguages.Add((Language)language.Clone());

                priceGroup.TargetLanguages = new List<Language>();
                foreach (var language in this.TargetLanguages)
                    priceGroup.TargetLanguages.Add((Language)language.Clone());


                return priceGroup;
            }
        }

        public class AnalysisBand : ICloneable
        {

            public string Id { get; set; }
            public int PercentagePerfect { get; set; }
            public int PercentageContext { get; set; }
            public int PercentageRepetition { get; set; }
            public int PercentageExact { get; set; }
            public int Percentage9599 { get; set; }
            public int Percentage8594 { get; set; }
            public int Percentage7584 { get; set; }
            public int Percentage5074 { get; set; }
            public int PercentageNew { get; set; }

            public AnalysisBand()
            {
                Id = Guid.NewGuid().ToString();
                PercentagePerfect = 0;
                PercentageContext = 0;
                PercentageRepetition = 0;
                PercentageExact = 0;
                Percentage9599 = 20;
                Percentage8594 = 65;
                Percentage7584 = 75;
                Percentage5074 = 100;
                PercentageNew = 100;
            }
            public object Clone()
            {
                return (AnalysisBand)MemberwiseClone();
            }
        }

        [Serializable]
        public class Price : ICloneable
        {
            public enum RoundType
            {

                Roundup = 0,
                Rounddown = 1,
                Round = 2
            }

            public string Id { get; set; }
            public string Source { get; set; }
            public string Target { get; set; }
            public RoundType Round { get; set; }

            public decimal PriceBase { get; set; }
            public decimal PricePerfect { get; set; }
            public decimal PriceContext { get; set; }
            public decimal PriceRepetition { get; set; }
            public decimal PriceExact { get; set; }
            public decimal Price9599 { get; set; }
            public decimal Price8594 { get; set; }
            public decimal Price7584 { get; set; }
            public decimal Price5074 { get; set; }
            public decimal PriceNew { get; set; }

            public Price()
            {
                Id = Guid.NewGuid().ToString();
                Source = string.Empty;
                Target = string.Empty;
                Round = RoundType.Round;

                PriceBase = 0;
                PricePerfect = 0;
                PriceContext = 0;
                PriceRepetition = 0;
                PriceExact = 0;
                Price9599 = 0;
                Price8594 = 0;
                Price7584 = 0;
                Price5074 = 0;
                PriceNew = 0;
            }
            public object Clone()
            {
                return (Price)MemberwiseClone();
            }


        }

        [Serializable]
        public class Language : ICloneable
        {
            public string Id { get; set; } //culture Info name
            public string Id2 { get; set; } //ID assigned by the user if different than the culture info Name
            public string Iso2 { get; set; } //culture info ISO 2 ID
            public string Name { get; set; } //culture info language name


            public Language()
            {
                Id = string.Empty;
                Id2 = string.Empty;
                Iso2 = string.Empty;
                Name = string.Empty;
            }
            public Language(string id, string id2, string iso2, string name)
            {
                Id = id;
                Id2 = id2;
                Iso2 = iso2;
                Name = name;
            }
            public object Clone()
            {
                return (Language)MemberwiseClone();
            }
        }


        [Serializable]
        public class FilterSetting : ICloneable
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool IsDefault { get; set; }

            public List<string> FilterNamesInclude { get; set; }
            public List<string> FilterNamesExclude { get; set; }
            public bool UseRegularExpressionMatching { get; set; }

            public bool FilterDateUsed { get; set; }
            public FilterDate FilterDate { get; set; }

            public bool FilterAttributeArchiveUsed { get; set; }
            public string FilterAttributeAchiveType { get; set; }

            public bool FilterAttributeSystemUsed { get; set; }
            public string FilterAttributeSystemType { get; set; }

            public bool FilterAttributeHiddenUsed { get; set; }
            public string FilterAttributeHiddenType { get; set; }

            public bool FilterAttributeReadOnlyUsed { get; set; }
            public string FilterAttributeReadOnlyType { get; set; }


            public FilterSetting()
            {
                Id = Guid.NewGuid().ToString();
                Name = string.Empty;
                IsDefault = false;

                FilterNamesInclude = new List<string>();
                FilterNamesExclude = new List<string>();
                UseRegularExpressionMatching = false;

                FilterDateUsed = false;
                FilterDate = new FilterDate();

                FilterAttributeArchiveUsed = false;
                FilterAttributeAchiveType = "Included";

                FilterAttributeSystemUsed = false;
                FilterAttributeSystemType = "Included";

                FilterAttributeHiddenUsed = false;
                FilterAttributeHiddenType = "Included";

                FilterAttributeReadOnlyUsed = false;
                FilterAttributeReadOnlyType = "Included";
            }
            public object Clone()
            {
                var filterSetting = new FilterSetting
                {
                    Id = Id,
                    Name = Name,
                    IsDefault = IsDefault,
                    FilterDateUsed = FilterDateUsed,
                    FilterDate = (FilterDate)FilterDate.Clone(),
                    FilterNamesInclude = FilterNamesInclude,
                    FilterNamesExclude = FilterNamesExclude,
                    UseRegularExpressionMatching = UseRegularExpressionMatching,
                    FilterAttributeArchiveUsed = FilterAttributeArchiveUsed,
                    FilterAttributeAchiveType = FilterAttributeAchiveType,
                    FilterAttributeSystemUsed = FilterAttributeSystemUsed,
                    FilterAttributeSystemType = FilterAttributeSystemType,
                    FilterAttributeHiddenUsed = FilterAttributeHiddenUsed,
                    FilterAttributeHiddenType = FilterAttributeHiddenType,
                    FilterAttributeReadOnlyUsed = FilterAttributeReadOnlyUsed,
                    FilterAttributeReadOnlyType = FilterAttributeReadOnlyType
                };

                return filterSetting;
            }
        }
        [Serializable]
        public class FilterDate : ICloneable
        {
            [Serializable]
            public enum FilterType
            {
                GreaterThan = 0,
                LessThan
            }
            public DateTime Date { get; set; }
            public FilterType Type { get; set; }


            public FilterDate()
            {
                Date = DateTime.Now;
                Type = FilterType.GreaterThan;
            }
            public object Clone()
            {
                var filterDate = new FilterDate
                {
                    Date = Date,
                    Type = Type
                };

                return filterDate;
            }
        }



        [Serializable]
        public class ComparisonProject : ICloneable
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Created { get; set; }
            public bool FoundPathLeft { get; set; }
            public bool FoundPathRight { get; set; }
            public string PathLeft { get; set; }
            public string PathRight { get; set; }

            public List<FileAlignment> FileAlignment { get; set; }

            public ComparisonProject()
            {
                Id = Guid.NewGuid().ToString();
                Name = string.Empty;
                Created = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                FoundPathLeft = false;
                FoundPathRight = false;
                PathLeft = string.Empty;
                PathRight = string.Empty;
                FileAlignment = new List<FileAlignment>();
            }
            public object Clone()
            {
                var cp = new ComparisonProject
                {
                    Id = Id,
                    Name = Name,
                    Created = Created,
                    FoundPathLeft = FoundPathLeft,
                    FoundPathRight = FoundPathRight,
                    PathLeft = PathLeft,
                    PathRight = PathRight,
                    FileAlignment = new List<FileAlignment>()
                };

                foreach (var fa in FileAlignment)
                    cp.FileAlignment.Add((FileAlignment)fa.Clone());

                return cp;
            }
        }

        [Serializable]
        public class FileAlignment : ICloneable
        {
            public string PathLeft { get; set; }
            public string PathRight { get; set; }
            public decimal MatchPercentage { get; set; }
            public bool IsWorldServerFile { get; set; }
            public bool IsFullMatch { get; set; }
            public FileAlignment()
            {
                PathLeft = string.Empty;
                PathRight = string.Empty;
                MatchPercentage = 0;
                IsWorldServerFile = false;
                IsFullMatch = false;
            }
            public object Clone()
            {
                return (FileAlignment)MemberwiseClone();
            }
        }


        [Serializable]
        public class ComparisonLogEntry
        {
            [Serializable]
            public enum EntryType
            {
                ComparisonFoldersCompare = 0,
                ComparisonFoldersLeftChange = 1,
                ComparisonFoldersRightChange = 2,
                ReportCreate = 3,
                ReportSave = 4,
                ComparisonProjectNew = 5,
                ComparisonProjectEdit = 6,
                ComparisonProjectCompare = 7,
                ComparisonProjectSave = 8,
                ComparisonProjectRemove = 9,
                FilesMove = 10,
                FilesCopy = 11,
                FilesDelete = 12,
                FiltersAdd = 13,
                FilterEdit = 14,
                FiltersDelete = 15,
                None
            }
            public string Id { get; set; }
            public DateTime Date { get; set; }
            public EntryType Type { get; set; }


            public string Action { get; set; }


            public string ItemName { get; set; }
            public string Value01 { get; set; }
            public string Value02 { get; set; }

            public List<ComparisonLogEntryItem> Items { get; set; }

            public ComparisonLogEntry()
            {
                Id = Guid.NewGuid().ToString();
                Date = DateTime.Now;
                Type = EntryType.None;
                Action = GetComparisonLogEntryTypeName(Type);

                ItemName = string.Empty;
                Value01 = string.Empty;
                Value02 = string.Empty;

                Items = null;

            }
            public ComparisonLogEntry(EntryType entryType)
            {
                Id = Guid.NewGuid().ToString();
                Date = DateTime.Now;
                Type = entryType;
                Action = GetComparisonLogEntryTypeName(Type);


                ItemName = string.Empty;
                Value01 = string.Empty;
                Value02 = string.Empty;

                Items = null;
            }

        }

        [Serializable]
        public class ComparisonLogEntryItem
        {
            public string Option { get; set; }
            public string From { get; set; }
            public string To { get; set; }

            public ComparisonLogEntryItem()
            {
                Option = string.Empty;
                From = string.Empty;
                To = string.Empty;
            }
            public ComparisonLogEntryItem(string option, string @from, string to)
            {
                Option = option;
                From = @from;
                To = to;
            }
        }

        public static string GetComparisonLogEntryTypeName(ComparisonLogEntry.EntryType type)
        {
            switch (type)
            {
                case ComparisonLogEntry.EntryType.ComparisonFoldersCompare: return "Compared folders";
                case ComparisonLogEntry.EntryType.ComparisonFoldersLeftChange: return "Updated folder path (left side)";
                case ComparisonLogEntry.EntryType.ComparisonFoldersRightChange: return "Updated folder path (right side)";
                case ComparisonLogEntry.EntryType.ComparisonProjectCompare: return "Compared project";
                case ComparisonLogEntry.EntryType.ComparisonProjectEdit: return "Updated project settings";
                case ComparisonLogEntry.EntryType.ComparisonProjectNew: return "Added new project";
                case ComparisonLogEntry.EntryType.ComparisonProjectRemove: return "Removed project";
                case ComparisonLogEntry.EntryType.ComparisonProjectSave: return "Saved project settings";
                case ComparisonLogEntry.EntryType.FilesCopy: return "Copied files";
                case ComparisonLogEntry.EntryType.FilesDelete: return "Deleted files";
                case ComparisonLogEntry.EntryType.FilesMove: return "Moved files";
                case ComparisonLogEntry.EntryType.FilterEdit: return "Updated filter settings";
                case ComparisonLogEntry.EntryType.FiltersAdd: return "Added new filter";
                case ComparisonLogEntry.EntryType.ReportCreate: return "Created comparison report";
                default: return "Unspecified";
            }
        }

    }
}
