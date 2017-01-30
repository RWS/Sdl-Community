using System;

namespace Sdl.Community.XliffCompare.Core
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

        public Settings()
        {
            #region  |  new text style  |

            StyleNewText = new DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Activate",
                TextPosition = "Normal",
                FontSpecifyColor = true,
                FontColor = "#0000FF",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#FFFF66"
            };

            #endregion

            #region  |  removed text style  |

            StyleRemovedText = new DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Activate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = true,
                FontColor = "#FF0000",
                FontSpecifyBackroundColor = false,
                FontBackroundColor = "#FFFFFF"
            };

            #endregion

            #region  |  new tag style  |

            StyleNewTag = new DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = false,
                FontColor = "#000000",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#DDEEFF"
            };

            #endregion

            #region  |  removed tag style  |

            StyleRemovedTag = new DifferencesFormatting
            {
                StyleBold = "Deactivate",
                StyleItalic = "Deactivate",
                StyleStrikethrough = "Deactivate",
                StyleUnderline = "Deactivate",
                TextPosition = "Normal",
                FontSpecifyColor = false,
                FontColor = "#000000",
                FontSpecifyBackroundColor = true,
                FontBackroundColor = "#FFE8E8"
            };

            #endregion

            ReportFilterChangedTargetContent = true;
            ReportFilterSegmentStatusChanged = false;
            ReportFilterSegmentsContainingComments = true;
            ReportFilterFilesWithNoRecordsFiltered = true;

            CompareType = ComparisonType.Words;
            ComparisonIncludeTags = true;
            IncludeIndividualFileInformation = true;

            FilePathOriginal = string.Empty;
            FilePathUpdated = string.Empty;

            DirectoryPathOriginal = string.Empty;
            DirectoryPathUpdated = string.Empty;
            SearchSubFolders = true;

            ReportDirectory = string.Empty;
            ReportFileName = string.Empty;
            ReportingFormat = ReportFormat.Html;
            ViewReportWhenFinishedProcessing = true;

            UseCustomStyleSheet = false;
            FilePathCustomStyleSheet = string.Empty;
        }


        public DifferencesFormatting StyleNewText { get; set; }
        public DifferencesFormatting StyleRemovedText { get; set; }
        public DifferencesFormatting StyleNewTag { get; set; }
        public DifferencesFormatting StyleRemovedTag { get; set; }

        public bool ReportFilterChangedTargetContent { get; set; }
        public bool ReportFilterSegmentStatusChanged { get; set; }
        public bool ReportFilterSegmentsContainingComments { get; set; }
        public bool ReportFilterFilesWithNoRecordsFiltered { get; set; }



        public ComparisonType CompareType { get; set; }
        public bool ComparisonIncludeTags { get; set; }
        public bool IncludeIndividualFileInformation { get; set; }

        public string FilePathOriginal { get; set; }
        public string FilePathUpdated { get; set; }

        public string DirectoryPathOriginal { get; set; }
        public string DirectoryPathUpdated { get; set; }
        public bool SearchSubFolders { get; set; }

        public string ReportDirectory { get; set; }
        public string ReportFileName { get; set; }
        public ReportFormat ReportingFormat { get; set; }
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


    }
}
