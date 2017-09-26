using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using Sdl.Community.PostEdit.Compare.Core.Helper;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;

namespace Sdl.Community.PostEdit.Compare.Core.Reports
{
    public class Report
    {
        public delegate void ChangedEventHandlerProgress(int filesMax, int filesCurrent, string fileNameCurrent, int fileMaximum, int fileCurrent, int filePercent, string message);
        public event ChangedEventHandlerProgress ProgressReport;
        private void ReportProgress(int filesMax, int filesCurrent, string fileNameCurrent, int fileMaximum, int fileCurrent, int filePercent, string message)
        {
            if (ProgressReport != null)
                ProgressReport(filesMax, filesCurrent, fileNameCurrent, fileMaximum, fileCurrent, filePercent, message);
        }
        public enum ReportType
        {
            Xml = 0,
            Html
        }

        public void CreateXmlReport(string reportFilePath,
            Dictionary<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnits,
            bool transformReport, Settings.PriceGroup priceGroup, out List<TERp.DocumentResult> terpResults)
        {

            terpResults = new List<TERp.DocumentResult>();
            if (Processor.Settings.ShowSegmentTerp && File.Exists(Processor.Settings.JavaExecutablePath))
                terpResults = TERp.Process(reportFilePath
                    , fileComparisonFileParagraphUnits
                    , Processor.Settings.JavaExecutablePath);


            ReportUtils.WriteReportResourcesToDirectory(Path.GetDirectoryName(reportFilePath));
            var filePathXslt = Path.Combine(Path.GetDirectoryName(reportFilePath), ReportUtils.DefaultXsltName);
            var xsltContent = new StringBuilder();
            using (var r = new StreamReader(filePathXslt, Encoding.UTF8))
            {
                xsltContent.Append(r.ReadToEnd());
                r.Close();
            }
            var scriptIndex = 0;
            var rGoogle = new Regex(@"\<\!\-\-google\.setOnLoadCallback\(drawVisualization\)\;", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var mRGoogle = rGoogle.Match(xsltContent.ToString());
            if (mRGoogle.Success)
            {

                scriptIndex = mRGoogle.Index;
            }

            var xmlTxtWriter = new XmlTextWriter(reportFilePath, Encoding.UTF8)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };
            xmlTxtWriter.WriteStartDocument(true);


            xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + ReportUtils.DefaultXsltName + "'");
            xmlTxtWriter.WriteComment("Post-Edit Compare by Sdl Community, 2013");

            #region  |  variable totals  |


            decimal filesTotalTerp00NumEr = 0;
            decimal filesTotalTerp01NumEr = 0;
            decimal filesTotalTerp06NumEr = 0;
            decimal filesTotalTerp10NumEr = 0;
            decimal filesTotalTerp20NumEr = 0;
            decimal filesTotalTerp30NumEr = 0;
            decimal filesTotalTerp40NumEr = 0;
            decimal filesTotalTerp50NumEr = 0;

            decimal filesTotalTerp00NumWd = 0;
            decimal filesTotalTerp01NumWd = 0;
            decimal filesTotalTerp06NumWd = 0;
            decimal filesTotalTerp10NumWd = 0;
            decimal filesTotalTerp20NumWd = 0;
            decimal filesTotalTerp30NumWd = 0;
            decimal filesTotalTerp40NumWd = 0;
            decimal filesTotalTerp50NumWd = 0;

            decimal filesTotalTerp00SrcWd = 0;
            decimal filesTotalTerp01SrcWd = 0;
            decimal filesTotalTerp06SrcWd = 0;
            decimal filesTotalTerp10SrcWd = 0;
            decimal filesTotalTerp20SrcWd = 0;
            decimal filesTotalTerp30SrcWd = 0;
            decimal filesTotalTerp40SrcWd = 0;
            decimal filesTotalTerp50SrcWd = 0;

            decimal filesTotalTerp00Segments = 0;
            decimal filesTotalTerp01Segments = 0;
            decimal filesTotalTerp06Segments = 0;
            decimal filesTotalTerp10Segments = 0;
            decimal filesTotalTerp20Segments = 0;
            decimal filesTotalTerp30Segments = 0;
            decimal filesTotalTerp40Segments = 0;
            decimal filesTotalTerp50Segments = 0;

            decimal filesTotalTerp00Ins = 0;
            decimal filesTotalTerp01Ins = 0;
            decimal filesTotalTerp06Ins = 0;
            decimal filesTotalTerp10Ins = 0;
            decimal filesTotalTerp20Ins = 0;
            decimal filesTotalTerp30Ins = 0;
            decimal filesTotalTerp40Ins = 0;
            decimal filesTotalTerp50Ins = 0;


            decimal filesTotalTerp00Del = 0;
            decimal filesTotalTerp01Del = 0;
            decimal filesTotalTerp06Del = 0;
            decimal filesTotalTerp10Del = 0;
            decimal filesTotalTerp20Del = 0;
            decimal filesTotalTerp30Del = 0;
            decimal filesTotalTerp40Del = 0;
            decimal filesTotalTerp50Del = 0;

            decimal filesTotalTerp00Sub = 0;
            decimal filesTotalTerp01Sub = 0;
            decimal filesTotalTerp06Sub = 0;
            decimal filesTotalTerp10Sub = 0;
            decimal filesTotalTerp20Sub = 0;
            decimal filesTotalTerp30Sub = 0;
            decimal filesTotalTerp40Sub = 0;
            decimal filesTotalTerp50Sub = 0;

            decimal filesTotalTerp00Shft = 0;
            decimal filesTotalTerp01Shft = 0;
            decimal filesTotalTerp06Shft = 0;
            decimal filesTotalTerp10Shft = 0;
            decimal filesTotalTerp20Shft = 0;
            decimal filesTotalTerp30Shft = 0;
            decimal filesTotalTerp40Shft = 0;
            decimal filesTotalTerp50Shft = 0;


            decimal filesTotalPostEditExactSegments = 0;
            decimal filesTotalPostEditExactWords = 0;
            decimal filesTotalPostEditExactCharacters = 0;
            decimal filesTotalPostEditExactPercent = 0;
            decimal filesTotalPostEditExactTags = 0;
            decimal filesTotalPostEditExactPrice = 0;

            decimal filesTotalPostEditP99Segments = 0;
            decimal filesTotalPostEditP99Words = 0;
            decimal filesTotalPostEditP99Characters = 0;
            decimal filesTotalPostEditP99Percent = 0;
            decimal filesTotalPostEditP99Tags = 0;
            decimal filesTotalPostEditP99Price = 0;

            decimal filesTotalPostEditP94Segments = 0;
            decimal filesTotalPostEditP94Words = 0;
            decimal filesTotalPostEditP94Characters = 0;
            decimal filesTotalPostEditP94Percent = 0;
            decimal filesTotalPostEditP94Tags = 0;
            decimal filesTotalPostEditP94Price = 0;

            decimal filesTotalPostEditP84Segments = 0;
            decimal filesTotalPostEditP84Words = 0;
            decimal filesTotalPostEditP84Characters = 0;
            decimal filesTotalPostEditP84Percent = 0;
            decimal filesTotalPostEditP84Tags = 0;
            decimal filesTotalPostEditP84Price = 0;

            decimal filesTotalPostEditP74Segments = 0;
            decimal filesTotalPostEditP74Words = 0;
            decimal filesTotalPostEditP74Characters = 0;
            decimal filesTotalPostEditP74Percent = 0;
            decimal filesTotalPostEditP74Tags = 0;
            decimal filesTotalPostEditP74Price = 0;

            decimal filesTotalPostEditNewSegments = 0;
            decimal filesTotalPostEditNewWords = 0;
            decimal filesTotalPostEditNewCharacters = 0;
            decimal filesTotalPostEditNewPercent = 0;
            decimal filesTotalPostEditNewTags = 0;
            decimal filesTotalPostEditNewPrice = 0;

            decimal filesTotalPostEditTotalSegments = 0;
            decimal filesTotalPostEditTotalWords = 0;
            decimal filesTotalPostEditTotalCharacters = 0;
            decimal filesTotalPostEditTotalPercent = 0;
            decimal filesTotalPostEditTotalTags = 0;




            decimal filesSourcePmSegments = 0;
            decimal filesSourceCmSegments = 0;
            decimal filesSourceExactSegments = 0;
            decimal filesSourceRepsSegments = 0;
            decimal filesSoruceAtSegments = 0;
            decimal filesSourceFuzzy99Segments = 0;
            decimal filesSourceFuzzy94Segments = 0;
            decimal filesSourceFuzzy84Segments = 0;
            decimal filesSourceFuzzy74Segments = 0;
            decimal filesSourceNewSegments = 0;
            decimal filesSourceTotalSegments = 0;

            decimal filesSourcePmWords = 0;
            decimal filesSourceCmWords = 0;
            decimal filesSourceExactWords = 0;
            decimal filesSourceRepsWords = 0;
            decimal filesSoruceAtWords = 0;
            decimal filesSourceFuzzy99Words = 0;
            decimal filesSourceFuzzy94Words = 0;
            decimal filesSourceFuzzy84Words = 0;
            decimal filesSourceFuzzy74Words = 0;
            decimal filesSourceNewWords = 0;
            decimal filesSourceTotalWords = 0;


            decimal filesChangesPmSegments = 0;
            decimal filesChangesCmSegments = 0;
            decimal filesChangesExactSegments = 0;
            decimal filesChangesRepsSegments = 0;
            decimal filesChangesAtSegments = 0;
            decimal filesChangesFuzzy99Segments = 0;
            decimal filesChangesFuzzy94Segments = 0;
            decimal filesChangesFuzzy84Segments = 0;
            decimal filesChangesFuzzy74Segments = 0;
            decimal filesChangesNewSegments = 0;

            decimal filesChangesPmWords = 0;
            decimal filesChangesCmWords = 0;
            decimal filesChangesExactWords = 0;
            decimal filesChangesRepsWords = 0;
            decimal filesChangesAtWords = 0;
            decimal filesChangesFuzzy99Words = 0;
            decimal filesChangesFuzzy94Words = 0;
            decimal filesChangesFuzzy84Words = 0;
            decimal filesChangesFuzzy74Words = 0;
            decimal filesChangesNewWords = 0;

            decimal filesChangesPmCharacters = 0;
            decimal filesChangesCmCharacters = 0;
            decimal filesChangesExactCharacters = 0;
            decimal filesChangesRepsCharacters = 0;
            decimal filesChangesAtCharacters = 0;
            decimal filesChangesFuzzy99Characters = 0;
            decimal filesChangesFuzzy94Characters = 0;
            decimal filesChangesFuzzy84Characters = 0;
            decimal filesChangesFuzzy74Characters = 0;
            decimal filesChangesNewCharacters = 0;





            decimal filesTotalSegments = 0;
            decimal filesTotalSegmentsOriginal = 0;
            decimal filesTotalSegmentsUpdated = 0;
            decimal filesTotalContentChanges = 0;
            decimal filesTotalContentChangesPercentage = 0;
            decimal filesTotalStatusChanges = 0;
            decimal filesTotalStatusChangesPercentage = 0;
            decimal filesTotalComments = 0;

            decimal filesFilteredParagraphs = 0;
            decimal filesFilteredSegments = 0;
            decimal filesFilteredContentChanges = 0;
            decimal filesFilteredStatusChanges = 0;
            decimal filesFilteredComments = 0;

            decimal filesTotalNotTranslatedOriginal = 0;
            decimal filesTotalDraftOriginal = 0;
            decimal filesTotalTranslatedOriginal = 0;
            decimal filesTotalTranslationRejectedOriginal = 0;
            decimal filesTotalTranslationApprovedOriginal = 0;
            decimal filesTotalSignOffRejectedOriginal = 0;
            decimal filesTotalSignedOffOriginal = 0;

            decimal filesTotalNotTranslatedUpdated = 0;
            decimal filesTotalDraftUpdated = 0;
            decimal filesTotalTranslatedUpdated = 0;
            decimal filesTotalTranslationRejectedUpdated = 0;
            decimal filesTotalTranslationApprovedUpdated = 0;
            decimal filesTotalSignOffRejectedUpdated = 0;
            decimal filesTotalSignedOffUpdated = 0;

            #endregion

            #region  |  files  |

            var progressMaxFiles = 0;
            var progressCurrentFileIndex = 0;
            var progressFilesCompared = 0;


            foreach (var fileComparisonFileParagraphUnit in fileComparisonFileParagraphUnits)
            {
                progressFilesCompared += fileComparisonFileParagraphUnit.Value != null ? 1 : 0;
                progressMaxFiles++;
            }

            xmlTxtWriter.WriteStartElement("files");
            #region  |  files attributes  |
            xmlTxtWriter.WriteAttributeString("xml:space", "preserve");
            xmlTxtWriter.WriteAttributeString("count", fileComparisonFileParagraphUnits.Count.ToString());
            xmlTxtWriter.WriteAttributeString("totalfilesCompared", progressFilesCompared.ToString());
            xmlTxtWriter.WriteAttributeString("totalFilesWithErrors", (fileComparisonFileParagraphUnits.Count - progressFilesCompared).ToString());
            xmlTxtWriter.WriteAttributeString("dateCompared", DateTime.Now.ToLongDateString() + ", " + DateTime.Now.ToLongTimeString());


            xmlTxtWriter.WriteStartElement("filter");

            xmlTxtWriter.WriteAttributeString("viewFilewWithNoRecords", Processor.Settings.ReportFilterFilesWithNoRecordsFiltered.ToString());
            xmlTxtWriter.WriteAttributeString("showGoogleCharts", Processor.Settings.ShowGoogleChartsInReport.ToString());
            xmlTxtWriter.WriteAttributeString("calculateSummaryAnalysisBasedOnFilteredRows", Processor.Settings.CalculateSummaryAnalysisBasedOnFilteredRows.ToString());

            xmlTxtWriter.WriteAttributeString("showOriginalSourceSegment", Processor.Settings.ShowOriginalSourceSegment.ToString());
            xmlTxtWriter.WriteAttributeString("showOriginalTargetSegment", Processor.Settings.ShowOriginalTargetSegment.ToString());
            xmlTxtWriter.WriteAttributeString("showOriginalRevisionMarkersTargetSegment", Processor.Settings.ShowOriginalRevisionMarkerTargetSegment.ToString());
            xmlTxtWriter.WriteAttributeString("showUpdatedTargetSegment", Processor.Settings.ShowUpdatedTargetSegment.ToString());
            xmlTxtWriter.WriteAttributeString("showUpdatedRevisionMarkersTargetSegment", Processor.Settings.ShowUpdatedRevisionMarkerTargetSegment.ToString());
            xmlTxtWriter.WriteAttributeString("showTargetComparison", Processor.Settings.ShowTargetComparison.ToString());
            xmlTxtWriter.WriteAttributeString("showSegmentComments", Processor.Settings.ShowSegmentComments.ToString());
            xmlTxtWriter.WriteAttributeString("showSegmentLocked", Processor.Settings.ShowSegmentLocked.ToString());
            xmlTxtWriter.WriteAttributeString("showTranslationMatchValuesOriginal", Processor.Settings.ReportFilterTranslationMatchValuesOriginal);
            xmlTxtWriter.WriteAttributeString("showTranslationMatchValuesUpdated", Processor.Settings.ReportFilterTranslationMatchValuesUpdated);

            xmlTxtWriter.WriteAttributeString("showSegmentStatus", Processor.Settings.ShowSegmentStatus.ToString());
            xmlTxtWriter.WriteAttributeString("showSegmentMatch", Processor.Settings.ShowSegmentMatch.ToString());
            xmlTxtWriter.WriteAttributeString("showSegmentTerp", Processor.Settings.ShowSegmentTerp.ToString());
            xmlTxtWriter.WriteAttributeString("showSegmentPemp", Processor.Settings.ShowSegmentPem.ToString());

            xmlTxtWriter.WriteEndElement();

            #endregion


            var iFileIndex = -1;
            var iFilesWithContentFiltered = 0;


            foreach (var fileComparisonFileParagraphUnit in fileComparisonFileParagraphUnits)
            {
                progressCurrentFileIndex++;
                var progressMaxSegments = 0;
                var progressCurrentSegmentIndex = 0;
                var progressPercentage = 0;
                var progressCurrentFileName = Path.GetFileName(fileComparisonFileParagraphUnit.Key.FilePathOriginal);
                var progressCurrentMessage = "Initializing file comparison...";

                ReportProgress(progressMaxFiles, progressCurrentFileIndex
                    , progressCurrentFileName, progressMaxSegments, progressCurrentSegmentIndex, progressPercentage
                    , progressCurrentMessage);


                iFileIndex++;
                var iFileInnderIndex = 0;
                var fileId = "fileId_" + iFileIndex + "_" + iFileInnderIndex++;



                var filteredContentChanges = 0;
                var filteredStatusChanges = 0;
                var filteredComments = 0;
                var filteredSegments = 0;
                var filteredParagraphs = 0;


                FilteredContentChanges(fileComparisonFileParagraphUnit, ref filteredContentChanges, ref filteredStatusChanges, ref filteredComments, ref filteredSegments, ref filteredParagraphs);


                progressMaxSegments = filteredSegments;
                progressCurrentSegmentIndex = 0;
                progressPercentage = 0;
                progressCurrentMessage = "Initializing file comparison...";

                ReportProgress(progressMaxFiles, progressCurrentFileIndex
                    , progressCurrentFileName, progressMaxSegments, progressCurrentSegmentIndex, progressPercentage
                    , progressCurrentMessage);


                var fileUnitProperties = fileComparisonFileParagraphUnit.Key;


                var excludeFileNoChanges = false;
                if (!Processor.Settings.ReportFilterFilesWithNoRecordsFiltered)
                    if (fileUnitProperties.TotalContentChanges == 0)
                        excludeFileNoChanges = true;


                if (excludeFileNoChanges)
                    continue;


                var languageRate = LanguageRate(priceGroup, fileUnitProperties);


                #region  |  status changes  |

                decimal filesTotalStatusChangesUpdated =

                    fileUnitProperties.TotalNotTranslatedUpdated
                    + fileUnitProperties.TotalDraftUpdated
                    + fileUnitProperties.TotalTranslatedUpdated
                    + fileUnitProperties.TotalTranslationRejectedUpdated
                    + fileUnitProperties.TotalTranslationApprovedUpdated
                    + fileUnitProperties.TotalSignOffRejectedUpdated
                    + fileUnitProperties.TotalSignedOffUpdated;


                decimal filesTotalNotTranslatedChangesPercentage = 0;
                decimal filesTotalDraftChangesPercentage = 0;
                decimal filesTotalTranslatedChangesPercentage = 0;
                decimal filesTotalTranslationRejectedChangesPercentage = 0;
                decimal filesTotalTranslationApprovedChangesPercentage = 0;
                decimal filesTotalSignOffRejectedChangesPercentage = 0;
                decimal filesTotalSignedOffChangesPercentage = 0;



                if (filesTotalStatusChangesUpdated > 0)
                {
                    if (fileUnitProperties.TotalNotTranslatedUpdated > fileUnitProperties.TotalNotTranslatedOriginal)
                        filesTotalNotTranslatedChangesPercentage = Math.Round((fileUnitProperties.TotalNotTranslatedUpdated - fileUnitProperties.TotalNotTranslatedOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalNotTranslatedOriginal > 0)
                        filesTotalNotTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalNotTranslatedOriginal - fileUnitProperties.TotalNotTranslatedUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                    if (fileUnitProperties.TotalDraftUpdated > fileUnitProperties.TotalDraftOriginal)
                        filesTotalDraftChangesPercentage = Math.Round((fileUnitProperties.TotalDraftUpdated - fileUnitProperties.TotalDraftOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalDraftOriginal > 0)
                        filesTotalDraftChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalDraftOriginal - fileUnitProperties.TotalDraftUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                    if (fileUnitProperties.TotalTranslatedUpdated > fileUnitProperties.TotalTranslatedOriginal)
                        filesTotalTranslatedChangesPercentage = Math.Round((fileUnitProperties.TotalTranslatedUpdated - fileUnitProperties.TotalTranslatedOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalTranslatedOriginal > 0)
                        filesTotalTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalTranslatedOriginal - fileUnitProperties.TotalTranslatedUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                    if (fileUnitProperties.TotalTranslationRejectedUpdated > fileUnitProperties.TotalTranslationRejectedOriginal)
                        filesTotalTranslationRejectedChangesPercentage = Math.Round((fileUnitProperties.TotalTranslationRejectedUpdated - fileUnitProperties.TotalTranslationRejectedOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalTranslationRejectedOriginal > 0)
                        filesTotalTranslationRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalTranslationRejectedOriginal - fileUnitProperties.TotalTranslationRejectedUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));



                    if (fileUnitProperties.TotalTranslationApprovedUpdated > fileUnitProperties.TotalTranslationApprovedOriginal)
                        filesTotalTranslationApprovedChangesPercentage = Math.Round((fileUnitProperties.TotalTranslationApprovedUpdated - fileUnitProperties.TotalTranslationApprovedOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalTranslationApprovedOriginal > 0)
                        filesTotalTranslationApprovedChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalTranslationApprovedOriginal - fileUnitProperties.TotalTranslationApprovedUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                    if (fileUnitProperties.TotalSignOffRejectedUpdated > fileUnitProperties.TotalSignOffRejectedOriginal)
                        filesTotalSignOffRejectedChangesPercentage = Math.Round((fileUnitProperties.TotalSignOffRejectedUpdated - fileUnitProperties.TotalSignOffRejectedOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalSignOffRejectedOriginal > 0)
                        filesTotalSignOffRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalSignOffRejectedOriginal - fileUnitProperties.TotalSignOffRejectedUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                    if (fileUnitProperties.TotalSignedOffUpdated > fileUnitProperties.TotalSignedOffOriginal)
                        filesTotalSignedOffChangesPercentage = Math.Round((fileUnitProperties.TotalSignedOffUpdated - fileUnitProperties.TotalSignedOffOriginal) / filesTotalStatusChangesUpdated * 100, 2);
                    else if (fileUnitProperties.TotalSignedOffOriginal > 0)
                        filesTotalSignedOffChangesPercentage = Convert.ToDecimal("-" + Math.Round((fileUnitProperties.TotalSignedOffOriginal - fileUnitProperties.TotalSignedOffUpdated) / filesTotalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));

                }

                filesTotalStatusChangesPercentage =
                (filesTotalNotTranslatedChangesPercentage > 0 ? filesTotalNotTranslatedChangesPercentage : 0)
                + (filesTotalDraftChangesPercentage > 0 ? filesTotalDraftChangesPercentage : 0)
                + (filesTotalTranslatedChangesPercentage > 0 ? filesTotalTranslatedChangesPercentage : 0)
                + (filesTotalTranslationRejectedChangesPercentage > 0 ? filesTotalTranslationRejectedChangesPercentage : 0)
                + (filesTotalTranslationApprovedChangesPercentage > 0 ? filesTotalTranslationApprovedChangesPercentage : 0)
                + (filesTotalSignOffRejectedChangesPercentage > 0 ? filesTotalSignOffRejectedChangesPercentage : 0)
                + (filesTotalSignedOffChangesPercentage > 0 ? filesTotalSignedOffChangesPercentage : 0);


                #endregion

                var pempDict = new Dictionary<string, PEMp>();
                var terpResult = terpResults.SingleOrDefault(a => a.OriginalDocumentPath == fileUnitProperties.FilePathOriginal && a.UpdatedDocumentPath == fileUnitProperties.FilePathUpdated);

                xmlTxtWriter.WriteStartElement("file");

                #region  |  file attributes  |


                #region  |  top properties  |

                xmlTxtWriter.WriteAttributeString("id", fileId);

                xmlTxtWriter.WriteAttributeString("filePathOriginal", fileUnitProperties.FilePathOriginal);
                xmlTxtWriter.WriteAttributeString("filePathUpdated", fileUnitProperties.FilePathUpdated);

                xmlTxtWriter.WriteAttributeString("fileOriginalLanguageId", fileUnitProperties.SourceLanguageIdOriginal + " / " + fileUnitProperties.TargetLanguageIdOriginal);
                xmlTxtWriter.WriteAttributeString("fileTargetLanguageId", fileUnitProperties.SourceLanguageIdUpdated + " / " + fileUnitProperties.TargetLanguageIdUpdated);

                xmlTxtWriter.WriteStartElement("totals");

                xmlTxtWriter.WriteAttributeString("segments", fileUnitProperties.TotalSegments.ToString());
                xmlTxtWriter.WriteAttributeString("segmentsOriginal", fileUnitProperties.TotalSegmentsOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("segmentsUpdated", fileUnitProperties.TotalSegmentsUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("contentChanges", fileUnitProperties.TotalContentChanges.ToString());
                xmlTxtWriter.WriteAttributeString("contentChangesPercentage", fileUnitProperties.TotalContentChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("statusChanges", fileUnitProperties.TotalStatusChanges.ToString());
                xmlTxtWriter.WriteAttributeString("statusChangesPercentage", filesTotalStatusChangesPercentage.ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteAttributeString("comments", fileUnitProperties.TotalComments.ToString());

                xmlTxtWriter.WriteEndElement();

                xmlTxtWriter.WriteStartElement("filtered");


                xmlTxtWriter.WriteAttributeString("paragraphs", filteredParagraphs.ToString());
                xmlTxtWriter.WriteAttributeString("segments", filteredSegments.ToString());
                xmlTxtWriter.WriteAttributeString("contentChanges", filteredContentChanges.ToString());
                xmlTxtWriter.WriteAttributeString("statusChanges", filteredStatusChanges.ToString());
                xmlTxtWriter.WriteAttributeString("comments", filteredComments.ToString());

                xmlTxtWriter.WriteEndElement();

                xmlTxtWriter.WriteStartElement("filters");
                xmlTxtWriter.WriteAttributeString("excludeFileNoChanges", excludeFileNoChanges.ToString());
                xmlTxtWriter.WriteEndElement();




                #endregion

                #region  |  terp file analysis  |
                var TERpErrors = new List<TERp.SegmentData>();
                var terpAnalysisData = GetTERpAnalysisData(terpResult, fileComparisonFileParagraphUnit, ref TERpErrors);

                filesTotalTerp00SrcWd += terpAnalysisData.terp00SrcWd;
                filesTotalTerp01SrcWd += terpAnalysisData.terp01SrcWd;
                filesTotalTerp06SrcWd += terpAnalysisData.terp06SrcWd;
                filesTotalTerp10SrcWd += terpAnalysisData.terp10SrcWd;
                filesTotalTerp20SrcWd += terpAnalysisData.terp20SrcWd;
                filesTotalTerp30SrcWd += terpAnalysisData.terp30SrcWd;
                filesTotalTerp40SrcWd += terpAnalysisData.terp40SrcWd;
                filesTotalTerp50SrcWd += terpAnalysisData.terp50SrcWd;

                filesTotalTerp00Ins += terpAnalysisData.terp00Ins;
                filesTotalTerp01Ins += terpAnalysisData.terp01Ins;
                filesTotalTerp06Ins += terpAnalysisData.terp06Ins;
                filesTotalTerp10Ins += terpAnalysisData.terp10Ins;
                filesTotalTerp20Ins += terpAnalysisData.terp20Ins;
                filesTotalTerp30Ins += terpAnalysisData.terp30Ins;
                filesTotalTerp40Ins += terpAnalysisData.terp40Ins;
                filesTotalTerp50Ins += terpAnalysisData.terp50Ins;


                filesTotalTerp00Del += terpAnalysisData.terp00Del;
                filesTotalTerp01Del += terpAnalysisData.terp01Del;
                filesTotalTerp06Del += terpAnalysisData.terp06Del;
                filesTotalTerp10Del += terpAnalysisData.terp10Del;
                filesTotalTerp20Del += terpAnalysisData.terp20Del;
                filesTotalTerp30Del += terpAnalysisData.terp30Del;
                filesTotalTerp40Del += terpAnalysisData.terp40Del;
                filesTotalTerp50Del += terpAnalysisData.terp50Del;

                filesTotalTerp00Sub += terpAnalysisData.terp00Sub;
                filesTotalTerp01Sub += terpAnalysisData.terp01Sub;
                filesTotalTerp06Sub += terpAnalysisData.terp06Sub;
                filesTotalTerp10Sub += terpAnalysisData.terp10Sub;
                filesTotalTerp20Sub += terpAnalysisData.terp20Sub;
                filesTotalTerp30Sub += terpAnalysisData.terp30Sub;
                filesTotalTerp40Sub += terpAnalysisData.terp40Sub;
                filesTotalTerp50Sub += terpAnalysisData.terp50Sub;

                filesTotalTerp00Shft += terpAnalysisData.terp00Shft;
                filesTotalTerp01Shft += terpAnalysisData.terp01Shft;
                filesTotalTerp06Shft += terpAnalysisData.terp06Shft;
                filesTotalTerp10Shft += terpAnalysisData.terp10Shft;
                filesTotalTerp20Shft += terpAnalysisData.terp20Shft;
                filesTotalTerp30Shft += terpAnalysisData.terp30Shft;
                filesTotalTerp40Shft += terpAnalysisData.terp40Shft;
                filesTotalTerp50Shft += terpAnalysisData.terp50Shft;

                filesTotalTerp00NumEr += terpAnalysisData.terp00NumEr;
                filesTotalTerp01NumEr += terpAnalysisData.terp01NumEr;
                filesTotalTerp06NumEr += terpAnalysisData.terp06NumEr;
                filesTotalTerp10NumEr += terpAnalysisData.terp10NumEr;
                filesTotalTerp20NumEr += terpAnalysisData.terp20NumEr;
                filesTotalTerp30NumEr += terpAnalysisData.terp30NumEr;
                filesTotalTerp40NumEr += terpAnalysisData.terp40NumEr;
                filesTotalTerp50NumEr += terpAnalysisData.terp50NumEr;

                filesTotalTerp00NumWd += terpAnalysisData.terp00NumWd;
                filesTotalTerp01NumWd += terpAnalysisData.terp01NumWd;
                filesTotalTerp06NumWd += terpAnalysisData.terp06NumWd;
                filesTotalTerp10NumWd += terpAnalysisData.terp10NumWd;
                filesTotalTerp20NumWd += terpAnalysisData.terp20NumWd;
                filesTotalTerp30NumWd += terpAnalysisData.terp30NumWd;
                filesTotalTerp40NumWd += terpAnalysisData.terp40NumWd;
                filesTotalTerp50NumWd += terpAnalysisData.terp50NumWd;

                filesTotalTerp00Segments += terpAnalysisData.terp00Segments;
                filesTotalTerp01Segments += terpAnalysisData.terp01Segments;
                filesTotalTerp06Segments += terpAnalysisData.terp06Segments;
                filesTotalTerp10Segments += terpAnalysisData.terp10Segments;
                filesTotalTerp20Segments += terpAnalysisData.terp20Segments;
                filesTotalTerp30Segments += terpAnalysisData.terp30Segments;
                filesTotalTerp40Segments += terpAnalysisData.terp40Segments;
                filesTotalTerp50Segments += terpAnalysisData.terp50Segments;


                #region  |  TERp  |

                xmlTxtWriter.WriteStartElement("terpAnalysis");

                xmlTxtWriter.WriteAttributeString("terp00SrcWd", terpAnalysisData.terp00SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01SrcWd", terpAnalysisData.terp01SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06SrcWd", terpAnalysisData.terp06SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10SrcWd", terpAnalysisData.terp10SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20SrcWd", terpAnalysisData.terp20SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30SrcWd", terpAnalysisData.terp30SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40SrcWd", terpAnalysisData.terp40SrcWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50SrcWd", terpAnalysisData.terp50SrcWd.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00NumEr", terpAnalysisData.terp00NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01NumEr", terpAnalysisData.terp01NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06NumEr", terpAnalysisData.terp06NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10NumEr", terpAnalysisData.terp10NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20NumEr", terpAnalysisData.terp20NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30NumEr", terpAnalysisData.terp30NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40NumEr", terpAnalysisData.terp40NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50NumEr", terpAnalysisData.terp50NumEr.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00NumWd", terpAnalysisData.terp00NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01NumWd", terpAnalysisData.terp01NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06NumWd", terpAnalysisData.terp06NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10NumWd", terpAnalysisData.terp10NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20NumWd", terpAnalysisData.terp20NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30NumWd", terpAnalysisData.terp30NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40NumWd", terpAnalysisData.terp40NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50NumWd", terpAnalysisData.terp50NumWd.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00Segments", terpAnalysisData.terp00Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01Segments", terpAnalysisData.terp01Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06Segments", terpAnalysisData.terp06Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10Segments", terpAnalysisData.terp10Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20Segments", terpAnalysisData.terp20Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30Segments", terpAnalysisData.terp30Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40Segments", terpAnalysisData.terp40Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50Segments", terpAnalysisData.terp50Segments.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00Ins", terpAnalysisData.terp00Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01Ins", terpAnalysisData.terp01Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06Ins", terpAnalysisData.terp06Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10Ins", terpAnalysisData.terp10Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20Ins", terpAnalysisData.terp20Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30Ins", terpAnalysisData.terp30Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40Ins", terpAnalysisData.terp40Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50Ins", terpAnalysisData.terp50Ins.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00Del", terpAnalysisData.terp00Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01Del", terpAnalysisData.terp01Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06Del", terpAnalysisData.terp06Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10Del", terpAnalysisData.terp10Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20Del", terpAnalysisData.terp20Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30Del", terpAnalysisData.terp30Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40Del", terpAnalysisData.terp40Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50Del", terpAnalysisData.terp50Del.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00Sub", terpAnalysisData.terp00Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01Sub", terpAnalysisData.terp01Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06Sub", terpAnalysisData.terp06Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10Sub", terpAnalysisData.terp10Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20Sub", terpAnalysisData.terp20Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30Sub", terpAnalysisData.terp30Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40Sub", terpAnalysisData.terp40Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50Sub", terpAnalysisData.terp50Sub.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("terp00Shft", terpAnalysisData.terp00Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp01Shft", terpAnalysisData.terp01Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp06Shft", terpAnalysisData.terp06Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp10Shft", terpAnalysisData.terp10Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp20Shft", terpAnalysisData.terp20Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp30Shft", terpAnalysisData.terp30Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp40Shft", terpAnalysisData.terp40Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp50Shft", terpAnalysisData.terp50Shft.ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteEndElement();

                #endregion

                #endregion

                #region  |  pemp file analysis  |

                var pempAnalysisData = GetPEMpAnalysisData(fileComparisonFileParagraphUnit
                    , pempDict
                    , filteredSegments
                    , progressCurrentSegmentIndex
                    , progressMaxFiles
                    , progressCurrentFileIndex
                    , progressCurrentFileName
                    , TERpErrors);

				


				if (pempAnalysisData.totalWords > 0)
                {
                    pempAnalysisData.exactPercent = Math.Round(pempAnalysisData.exactWords, 2);
                    if (pempAnalysisData.exactWords > 0)
                        pempAnalysisData.exactPercent = Math.Round(pempAnalysisData.exactWords / pempAnalysisData.totalWords * 100, 2);

                    pempAnalysisData.fuzzy99Percent = Math.Round(pempAnalysisData.fuzzy99Words, 2);
                    if (pempAnalysisData.fuzzy99Words > 0)
                        pempAnalysisData.fuzzy99Percent = Math.Round(pempAnalysisData.fuzzy99Words / pempAnalysisData.totalWords * 100, 2);

                    pempAnalysisData.fuzzy94Percent = Math.Round(pempAnalysisData.fuzzy94Words, 2);
                    if (pempAnalysisData.fuzzy94Words > 0)
                        pempAnalysisData.fuzzy94Percent = Math.Round(pempAnalysisData.fuzzy94Words / pempAnalysisData.totalWords * 100, 2);

                    pempAnalysisData.fuzzy84Percent = Math.Round(pempAnalysisData.fuzzy84Words, 2);
                    if (pempAnalysisData.fuzzy84Words > 0)
                        pempAnalysisData.fuzzy84Percent = Math.Round(pempAnalysisData.fuzzy84Words / pempAnalysisData.totalWords * 100, 2);

                    pempAnalysisData.fuzzy74Percent = Math.Round(pempAnalysisData.fuzzy74Words, 2);
                    if (pempAnalysisData.fuzzy74Words > 0)
                        pempAnalysisData.fuzzy74Percent = Math.Round(pempAnalysisData.fuzzy74Words / pempAnalysisData.totalWords * 100, 2);

                    pempAnalysisData.newPercent = Math.Round(pempAnalysisData.newWords, 2);
                    if (pempAnalysisData.newWords > 0)
                        pempAnalysisData.newPercent = Math.Round(pempAnalysisData.newWords / pempAnalysisData.totalWords * 100, 2);

					var pemExcelModel = ExcelReportHelper.CreateExcelDataModels(pempAnalysisData);
					ExcelReport.CreateExcelReport(pemExcelModel);

					filesTotalPostEditExactSegments += pempAnalysisData.exactSegments;
                    filesTotalPostEditP99Segments += pempAnalysisData.fuzzy99Segments;
                    filesTotalPostEditP94Segments += pempAnalysisData.fuzzy94Segments;
                    filesTotalPostEditP84Segments += pempAnalysisData.fuzzy84Segments;
                    filesTotalPostEditP74Segments += pempAnalysisData.fuzzy74Segments;
                    filesTotalPostEditNewSegments += pempAnalysisData.newSegments;

                    filesTotalPostEditExactCharacters += pempAnalysisData.exactCharacters;
                    filesTotalPostEditP99Characters += pempAnalysisData.fuzzy99Characters;
                    filesTotalPostEditP94Characters += pempAnalysisData.fuzzy94Characters;
                    filesTotalPostEditP84Characters += pempAnalysisData.fuzzy84Characters;
                    filesTotalPostEditP74Characters += pempAnalysisData.fuzzy74Characters;
                    filesTotalPostEditNewCharacters += pempAnalysisData.newCharacters;

                    filesTotalPostEditExactWords += pempAnalysisData.exactWords;
                    filesTotalPostEditP99Words += pempAnalysisData.fuzzy99Words;
                    filesTotalPostEditP94Words += pempAnalysisData.fuzzy94Words;
                    filesTotalPostEditP84Words += pempAnalysisData.fuzzy84Words;
                    filesTotalPostEditP74Words += pempAnalysisData.fuzzy74Words;
                    filesTotalPostEditNewWords += pempAnalysisData.newWords;

                    filesTotalPostEditExactTags += pempAnalysisData.exactTags;
                    filesTotalPostEditP99Tags += pempAnalysisData.fuzzy99Tags;
                    filesTotalPostEditP94Tags += pempAnalysisData.fuzzy94Tags;
                    filesTotalPostEditP84Tags += pempAnalysisData.fuzzy84Tags;
                    filesTotalPostEditP74Tags += pempAnalysisData.fuzzy74Tags;
                    filesTotalPostEditNewTags += pempAnalysisData.newTags;


                    filesTotalPostEditTotalSegments += pempAnalysisData.exactSegments + pempAnalysisData.fuzzy99Segments + pempAnalysisData.fuzzy94Segments + pempAnalysisData.fuzzy84Segments + pempAnalysisData.fuzzy74Segments + pempAnalysisData.newSegments;
                    filesTotalPostEditTotalWords += pempAnalysisData.exactWords + pempAnalysisData.fuzzy99Words + pempAnalysisData.fuzzy94Words + pempAnalysisData.fuzzy84Words + pempAnalysisData.fuzzy74Words + pempAnalysisData.newWords;
                    filesTotalPostEditTotalCharacters += pempAnalysisData.exactCharacters + pempAnalysisData.fuzzy99Characters + pempAnalysisData.fuzzy94Characters + pempAnalysisData.fuzzy84Characters + pempAnalysisData.fuzzy74Characters + pempAnalysisData.newCharacters;
                    filesTotalPostEditTotalTags += pempAnalysisData.exactTags + pempAnalysisData.fuzzy99Tags + pempAnalysisData.fuzzy94Tags + pempAnalysisData.fuzzy84Tags + pempAnalysisData.fuzzy74Tags + pempAnalysisData.newTags;


                }

                #region  |  pemp Analysis   |


                xmlTxtWriter.WriteStartElement("pempAnalysis");

                xmlTxtWriter.WriteStartElement("rates");

                #region  |  language rates  |

                xmlTxtWriter.WriteAttributeString("pmWords", languageRate.PricePerfect.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("cmWords", languageRate.PriceContext.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("repetitionsWords", languageRate.PriceRepetition.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactWords", languageRate.PriceExact.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Words", languageRate.Price9599.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Words", languageRate.Price8594.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Words", languageRate.Price7584.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Words", languageRate.Price5074.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newWords", languageRate.PriceNew.ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteAttributeString("pmTotal", Math.Round(languageRate.PricePerfect * 0, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("cmTotal", Math.Round(languageRate.PriceContext * 0, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("repetitionsTotal", Math.Round(languageRate.PriceRepetition * 0, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactTotal", Math.Round(languageRate.PriceExact * pempAnalysisData.exactWords, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Total", Math.Round(languageRate.Price9599 * pempAnalysisData.fuzzy99Words, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Total", Math.Round(languageRate.Price8594 * pempAnalysisData.fuzzy94Words, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Total", Math.Round(languageRate.Price7584 * pempAnalysisData.fuzzy84Words, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Total", Math.Round(languageRate.Price5074 * pempAnalysisData.fuzzy74Words, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newTotal", Math.Round(languageRate.PriceNew * pempAnalysisData.newWords, 2).ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("priceTotal",
                (
                    Math.Round(languageRate.PricePerfect * 0, 2)
                    + Math.Round(languageRate.PriceContext * 0, 2)
                    + Math.Round(languageRate.PriceRepetition * 0, 2)
                    + Math.Round(languageRate.PriceExact * pempAnalysisData.exactWords, 2)
                    + Math.Round(languageRate.Price9599 * pempAnalysisData.fuzzy99Words, 2)
                    + Math.Round(languageRate.Price8594 * pempAnalysisData.fuzzy94Words, 2)
                    + Math.Round(languageRate.Price7584 * pempAnalysisData.fuzzy84Words, 2)
                    + Math.Round(languageRate.Price5074 * pempAnalysisData.fuzzy74Words, 2)
                    + Math.Round(languageRate.PriceNew * pempAnalysisData.newWords, 2)
                ).ToString(CultureInfo.InvariantCulture));

				if (priceGroup != null)
				{
					xmlTxtWriter.WriteAttributeString("currency", priceGroup.Currency);
				}
                




                filesTotalPostEditExactPrice += Math.Round(languageRate.PriceExact * pempAnalysisData.exactWords, 2);
                filesTotalPostEditP99Price += Math.Round(languageRate.Price9599 * pempAnalysisData.fuzzy99Words, 2);
                filesTotalPostEditP94Price += Math.Round(languageRate.Price8594 * pempAnalysisData.fuzzy94Words, 2);
                filesTotalPostEditP84Price += Math.Round(languageRate.Price7584 * pempAnalysisData.fuzzy84Words, 2);
                filesTotalPostEditP74Price += Math.Round(languageRate.Price5074 * pempAnalysisData.fuzzy74Words, 2);
                filesTotalPostEditNewPrice += Math.Round(languageRate.PriceNew * pempAnalysisData.newWords, 2);


                #endregion

                xmlTxtWriter.WriteEndElement();

                xmlTxtWriter.WriteStartElement("data");

                #region  |  data  |

                xmlTxtWriter.WriteAttributeString("exactSegments", pempAnalysisData.exactSegments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactWords", pempAnalysisData.exactWords.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactCharacters", pempAnalysisData.exactCharacters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactPercent", pempAnalysisData.exactPercent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("exactTags", pempAnalysisData.exactTags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("fuzzy99Segments", pempAnalysisData.fuzzy99Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Words", pempAnalysisData.fuzzy99Words.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Characters", pempAnalysisData.fuzzy99Characters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Percent", pempAnalysisData.fuzzy99Percent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy99Tags", pempAnalysisData.fuzzy99Tags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("fuzzy94Segments", pempAnalysisData.fuzzy94Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Words", pempAnalysisData.fuzzy94Words.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Characters", pempAnalysisData.fuzzy94Characters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Percent", pempAnalysisData.fuzzy94Percent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy94Tags", pempAnalysisData.fuzzy94Tags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("fuzzy84Segments", pempAnalysisData.fuzzy84Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Words", pempAnalysisData.fuzzy84Words.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Characters", pempAnalysisData.fuzzy84Characters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Percent", pempAnalysisData.fuzzy84Percent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy84Tags", pempAnalysisData.fuzzy84Tags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("fuzzy74Segments", pempAnalysisData.fuzzy74Segments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Words", pempAnalysisData.fuzzy74Words.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Characters", pempAnalysisData.fuzzy74Characters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Percent", pempAnalysisData.fuzzy74Percent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("fuzzy74Tags", pempAnalysisData.fuzzy74Tags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("newSegments", pempAnalysisData.newSegments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newWords", pempAnalysisData.newWords.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newCharacters", pempAnalysisData.newCharacters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newPercent", pempAnalysisData.newPercent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("newTags", pempAnalysisData.newTags.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("totalSegments", pempAnalysisData.totalSegments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("totalWords", pempAnalysisData.totalWords.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("totalCharacters", pempAnalysisData.totalCharacters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("totalPercent", pempAnalysisData.totalPercent.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("tags", pempAnalysisData.totalTags.ToString(CultureInfo.InvariantCulture));

                #endregion

                xmlTxtWriter.WriteEndElement();



                xmlTxtWriter.WriteEndElement();

                #endregion


                #endregion



                #region  |  Translation Modifications  |

                xmlTxtWriter.WriteStartElement("translationModifications");

                xmlTxtWriter.WriteAttributeString("changesPMWords", decimal.Truncate(fileUnitProperties.TotalChangesPmWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesCMWords", decimal.Truncate(fileUnitProperties.TotalChangesCmWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesExactWords", decimal.Truncate(fileUnitProperties.TotalChangesExactWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesRepsWords", decimal.Truncate(fileUnitProperties.TotalChangesRepsWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesATWords", decimal.Truncate(fileUnitProperties.TotalChangesAtWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy99Words", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy99Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy94Words", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy94Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy84Words", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy84Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy74Words", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy74Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesNewWords", decimal.Truncate(fileUnitProperties.TotalChangesNewWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesTotalWords", decimal.Truncate(
                fileUnitProperties.TotalChangesPmWords
                + fileUnitProperties.TotalChangesCmWords
                + fileUnitProperties.TotalChangesExactWords
                + fileUnitProperties.TotalChangesRepsWords
                + fileUnitProperties.TotalChangesAtWords
                + fileUnitProperties.TotalChangesFuzzy99Words
                + fileUnitProperties.TotalChangesFuzzy94Words
                + fileUnitProperties.TotalChangesFuzzy84Words
                + fileUnitProperties.TotalChangesFuzzy74Words
                + fileUnitProperties.TotalChangesNewWords).ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteAttributeString("changesPMCharacters", decimal.Truncate(fileUnitProperties.TotalChangesPmCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesCMCharacters", decimal.Truncate(fileUnitProperties.TotalChangesCmCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesExactCharacters", decimal.Truncate(fileUnitProperties.TotalChangesExactCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesRepsCharacters", decimal.Truncate(fileUnitProperties.TotalChangesRepsCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesATCharacters", decimal.Truncate(fileUnitProperties.TotalChangesAtCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy99Characters", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy99Characters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy94Characters", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy94Characters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy84Characters", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy84Characters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesFuzzy74Characters", decimal.Truncate(fileUnitProperties.TotalChangesFuzzy74Characters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesNewCharacters", decimal.Truncate(fileUnitProperties.TotalChangesNewCharacters).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("changesTotalCharacters", decimal.Truncate(
                fileUnitProperties.TotalChangesPmCharacters
                + fileUnitProperties.TotalChangesCmCharacters
                + fileUnitProperties.TotalChangesExactCharacters
                + fileUnitProperties.TotalChangesRepsCharacters
                + fileUnitProperties.TotalChangesAtCharacters
                + fileUnitProperties.TotalChangesFuzzy99Characters
                + fileUnitProperties.TotalChangesFuzzy94Characters
                + fileUnitProperties.TotalChangesFuzzy84Characters
                + fileUnitProperties.TotalChangesFuzzy74Characters
                + fileUnitProperties.TotalChangesNewCharacters).ToString(CultureInfo.InvariantCulture));



                xmlTxtWriter.WriteAttributeString("changesPMSegments", fileUnitProperties.TotalChangesPmSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesCMSegments", fileUnitProperties.TotalChangesCmSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesExactSegments", fileUnitProperties.TotalChangesExactSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesRepsSegments", fileUnitProperties.TotalChangesRepsSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesATSegments", fileUnitProperties.TotalChangesAtSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesFuzzy99Segments", fileUnitProperties.TotalChangesFuzzy99Segments.ToString());
                xmlTxtWriter.WriteAttributeString("changesFuzzy94Segments", fileUnitProperties.TotalChangesFuzzy94Segments.ToString());
                xmlTxtWriter.WriteAttributeString("changesFuzzy84Segments", fileUnitProperties.TotalChangesFuzzy84Segments.ToString());
                xmlTxtWriter.WriteAttributeString("changesFuzzy74Segments", fileUnitProperties.TotalChangesFuzzy74Segments.ToString());
                xmlTxtWriter.WriteAttributeString("changesNewSegments", fileUnitProperties.TotalChangesNewSegments.ToString());
                xmlTxtWriter.WriteAttributeString("changesTotalSegments",
                (fileUnitProperties.TotalChangesPmSegments
                 + fileUnitProperties.TotalChangesCmSegments
                 + fileUnitProperties.TotalChangesExactSegments
                 + fileUnitProperties.TotalChangesRepsSegments
                 + fileUnitProperties.TotalChangesAtSegments
                 + fileUnitProperties.TotalChangesFuzzy99Segments
                 + fileUnitProperties.TotalChangesFuzzy94Segments
                 + fileUnitProperties.TotalChangesFuzzy84Segments
                 + fileUnitProperties.TotalChangesFuzzy74Segments
                 + fileUnitProperties.TotalChangesNewSegments).ToString());



                xmlTxtWriter.WriteAttributeString("sourcePMSegments", decimal.Truncate(fileUnitProperties.TotalSourcePmSegments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceCMSegments", decimal.Truncate(fileUnitProperties.TotalSourceCmSegment).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceExactSegments", decimal.Truncate(fileUnitProperties.TotalSourceExactSegments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceRepsSegments", decimal.Truncate(fileUnitProperties.TotalSourceRepsSegments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceATSegments", decimal.Truncate(fileUnitProperties.TotalSourceAtSegments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy99Segments", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy99Segments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy94Segments", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy94Segments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy84Segments", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy84Segments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy74Segments", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy74Segments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceNewSegments", decimal.Truncate(fileUnitProperties.TotalSourceNewSegments).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceTotalSegments", decimal.Truncate(
                fileUnitProperties.TotalSourcePmSegments
                + fileUnitProperties.TotalSourceCmSegment
                + fileUnitProperties.TotalSourceExactSegments
                + fileUnitProperties.TotalSourceRepsSegments
                + fileUnitProperties.TotalSourceAtSegments
                + fileUnitProperties.TotalSourceFuzzy99Segments
                + fileUnitProperties.TotalSourceFuzzy94Segments
                + fileUnitProperties.TotalSourceFuzzy84Segments
                + fileUnitProperties.TotalSourceFuzzy74Segments
                + fileUnitProperties.TotalSourceNewSegments).ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteAttributeString("sourcePMWords", decimal.Truncate(fileUnitProperties.TotalSourcePmWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceCMWords", decimal.Truncate(fileUnitProperties.TotalSourceCmWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceExactWords", decimal.Truncate(fileUnitProperties.TotalSourceExactWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceRepsWords", decimal.Truncate(fileUnitProperties.TotalSourceRepsWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceATWords", decimal.Truncate(fileUnitProperties.TotalSourceAtWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy99Words", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy99Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy94Words", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy94Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy84Words", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy84Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceFuzzy74Words", decimal.Truncate(fileUnitProperties.TotalSourceFuzzy74Words).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceNewWords", decimal.Truncate(fileUnitProperties.TotalSourceNewWords).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sourceTotalWords", decimal.Truncate(
                fileUnitProperties.TotalSourcePmWords
                + fileUnitProperties.TotalSourceCmWords
                + fileUnitProperties.TotalSourceExactWords
                + fileUnitProperties.TotalSourceRepsWords
                + fileUnitProperties.TotalSourceAtWords
                + fileUnitProperties.TotalSourceFuzzy99Words
                + fileUnitProperties.TotalSourceFuzzy94Words
                + fileUnitProperties.TotalSourceFuzzy84Words
                + fileUnitProperties.TotalSourceFuzzy74Words
                + fileUnitProperties.TotalSourceNewWords).ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteEndElement();

                #region |  Totals  |


                filesTotalSegments += fileUnitProperties.TotalSegments;
                filesTotalSegmentsOriginal += fileUnitProperties.TotalSegmentsOriginal;
                filesTotalSegmentsUpdated += fileUnitProperties.TotalSegmentsUpdated;
                filesTotalContentChanges += fileUnitProperties.TotalContentChanges;
                filesTotalContentChangesPercentage = 0;
                filesTotalStatusChanges += fileUnitProperties.TotalStatusChanges;
                filesTotalStatusChangesPercentage = 0;
                filesTotalComments += fileUnitProperties.TotalComments;

                if (fileUnitProperties.TotalContentChanges > 0)
                    filesTotalContentChangesPercentage = Math.Round(Convert.ToDecimal(fileUnitProperties.TotalContentChanges) / Convert.ToDecimal(fileUnitProperties.TotalSegments) * 100, 2);


                filesFilteredParagraphs += filteredParagraphs;
                filesFilteredSegments += filteredSegments;
                filesFilteredContentChanges += filteredContentChanges;
                filesFilteredStatusChanges += filteredStatusChanges;
                filesFilteredComments += filteredComments;

                filesTotalNotTranslatedOriginal += fileUnitProperties.TotalNotTranslatedOriginal;
                filesTotalDraftOriginal += fileUnitProperties.TotalDraftOriginal;
                filesTotalTranslatedOriginal += fileUnitProperties.TotalTranslatedOriginal;
                filesTotalTranslationRejectedOriginal += fileUnitProperties.TotalTranslationRejectedOriginal;
                filesTotalTranslationApprovedOriginal += fileUnitProperties.TotalTranslationApprovedOriginal;
                filesTotalSignOffRejectedOriginal += fileUnitProperties.TotalSignOffRejectedOriginal;
                filesTotalSignedOffOriginal += fileUnitProperties.TotalSignedOffOriginal;

                filesTotalNotTranslatedUpdated += fileUnitProperties.TotalNotTranslatedUpdated;
                filesTotalDraftUpdated += fileUnitProperties.TotalDraftUpdated;
                filesTotalTranslatedUpdated += fileUnitProperties.TotalTranslatedUpdated;
                filesTotalTranslationRejectedUpdated += fileUnitProperties.TotalTranslationRejectedUpdated;
                filesTotalTranslationApprovedUpdated += fileUnitProperties.TotalTranslationApprovedUpdated;
                filesTotalSignOffRejectedUpdated += fileUnitProperties.TotalSignOffRejectedUpdated;
                filesTotalSignedOffUpdated += fileUnitProperties.TotalSignedOffUpdated;

                filesChangesPmSegments += fileUnitProperties.TotalChangesPmSegments;
                filesChangesCmSegments += fileUnitProperties.TotalChangesCmSegments;
                filesChangesExactSegments += fileUnitProperties.TotalChangesExactSegments;
                filesChangesRepsSegments += fileUnitProperties.TotalChangesRepsSegments;
                filesChangesAtSegments += fileUnitProperties.TotalChangesAtSegments;
                filesChangesFuzzy99Segments += fileUnitProperties.TotalChangesFuzzy99Segments;
                filesChangesFuzzy94Segments += fileUnitProperties.TotalChangesFuzzy94Segments;
                filesChangesFuzzy84Segments += fileUnitProperties.TotalChangesFuzzy84Segments;
                filesChangesFuzzy74Segments += fileUnitProperties.TotalChangesFuzzy74Segments;
                filesChangesNewSegments += fileUnitProperties.TotalChangesNewSegments;


                filesChangesPmWords += fileUnitProperties.TotalChangesPmWords;
                filesChangesCmWords += fileUnitProperties.TotalChangesCmWords;
                filesChangesExactWords += fileUnitProperties.TotalChangesExactWords;
                filesChangesRepsWords += fileUnitProperties.TotalChangesRepsWords;
                filesChangesAtWords += fileUnitProperties.TotalChangesAtWords;
                filesChangesFuzzy99Words += fileUnitProperties.TotalChangesFuzzy99Words;
                filesChangesFuzzy94Words += fileUnitProperties.TotalChangesFuzzy94Words;
                filesChangesFuzzy84Words += fileUnitProperties.TotalChangesFuzzy84Words;
                filesChangesFuzzy74Words += fileUnitProperties.TotalChangesFuzzy74Words;
                filesChangesNewWords += fileUnitProperties.TotalChangesNewWords;

                filesChangesPmCharacters += fileUnitProperties.TotalChangesPmCharacters;
                filesChangesCmCharacters += fileUnitProperties.TotalChangesCmCharacters;
                filesChangesExactCharacters += fileUnitProperties.TotalChangesExactCharacters;
                filesChangesRepsCharacters += fileUnitProperties.TotalChangesRepsCharacters;
                filesChangesAtCharacters += fileUnitProperties.TotalChangesAtCharacters;
                filesChangesFuzzy99Characters += fileUnitProperties.TotalChangesFuzzy99Characters;
                filesChangesFuzzy94Characters += fileUnitProperties.TotalChangesFuzzy94Characters;
                filesChangesFuzzy84Characters += fileUnitProperties.TotalChangesFuzzy84Characters;
                filesChangesFuzzy74Characters += fileUnitProperties.TotalChangesFuzzy74Characters;
                filesChangesNewCharacters += fileUnitProperties.TotalChangesNewCharacters;



                filesSourcePmSegments += fileUnitProperties.TotalSourcePmSegments;
                filesSourceCmSegments += fileUnitProperties.TotalSourceCmSegment;
                filesSourceExactSegments += fileUnitProperties.TotalSourceExactSegments;
                filesSourceRepsSegments += fileUnitProperties.TotalSourceRepsSegments;
                filesSoruceAtSegments += fileUnitProperties.TotalSourceAtSegments;
                filesSourceFuzzy99Segments += fileUnitProperties.TotalSourceFuzzy99Segments;
                filesSourceFuzzy94Segments += fileUnitProperties.TotalSourceFuzzy94Segments;
                filesSourceFuzzy84Segments += fileUnitProperties.TotalSourceFuzzy84Segments;
                filesSourceFuzzy74Segments += fileUnitProperties.TotalSourceFuzzy74Segments;
                filesSourceNewSegments += fileUnitProperties.TotalSourceNewSegments;
                filesSourceTotalSegments +=
                fileUnitProperties.TotalSourcePmSegments
                + fileUnitProperties.TotalSourceCmSegment
                + fileUnitProperties.TotalSourceExactSegments
                + fileUnitProperties.TotalSourceRepsSegments
                + fileUnitProperties.TotalSourceAtSegments
                + fileUnitProperties.TotalSourceFuzzy99Segments
                + fileUnitProperties.TotalSourceFuzzy94Segments
                + fileUnitProperties.TotalSourceFuzzy84Segments
                + fileUnitProperties.TotalSourceFuzzy74Segments
                + fileUnitProperties.TotalSourceNewSegments;


                filesSourcePmWords += fileUnitProperties.TotalSourcePmWords;
                filesSourceCmWords += fileUnitProperties.TotalSourceCmWords;
                filesSourceExactWords += fileUnitProperties.TotalSourceExactWords;
                filesSourceRepsWords += fileUnitProperties.TotalSourceRepsWords;
                filesSoruceAtWords += fileUnitProperties.TotalSourceAtWords;
                filesSourceFuzzy99Words += fileUnitProperties.TotalSourceFuzzy99Words;
                filesSourceFuzzy94Words += fileUnitProperties.TotalSourceFuzzy94Words;
                filesSourceFuzzy84Words += fileUnitProperties.TotalSourceFuzzy84Words;
                filesSourceFuzzy74Words += fileUnitProperties.TotalSourceFuzzy74Words;
                filesSourceNewWords += fileUnitProperties.TotalSourceNewWords;
                filesSourceTotalWords +=
                fileUnitProperties.TotalSourcePmWords
                + fileUnitProperties.TotalSourceCmWords
                + fileUnitProperties.TotalSourceExactWords
                + fileUnitProperties.TotalSourceRepsWords
                + fileUnitProperties.TotalSourceAtWords
                + fileUnitProperties.TotalSourceFuzzy99Words
                + fileUnitProperties.TotalSourceFuzzy94Words
                + fileUnitProperties.TotalSourceFuzzy84Words
                + fileUnitProperties.TotalSourceFuzzy74Words
                + fileUnitProperties.TotalSourceNewWords;



                #endregion

                #endregion

                #region  |  confirmationStatistics  |

                xmlTxtWriter.WriteStartElement("confirmationStatistics");

                xmlTxtWriter.WriteAttributeString("notTranslatedOriginal", fileUnitProperties.TotalNotTranslatedOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("draftOriginal", fileUnitProperties.TotalDraftOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("translatedOriginal", fileUnitProperties.TotalTranslatedOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("translationRejectedOriginal", fileUnitProperties.TotalTranslationRejectedOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("translationApprovedOriginal", fileUnitProperties.TotalTranslationApprovedOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("signOffRejectedOriginal", fileUnitProperties.TotalSignOffRejectedOriginal.ToString());
                xmlTxtWriter.WriteAttributeString("signedOffOriginal", fileUnitProperties.TotalSignedOffOriginal.ToString());


                xmlTxtWriter.WriteAttributeString("notTranslatedUpdated", fileUnitProperties.TotalNotTranslatedUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("draftUpdated", fileUnitProperties.TotalDraftUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("translatedUpdated", fileUnitProperties.TotalTranslatedUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("translationRejectedUpdated", fileUnitProperties.TotalTranslationRejectedUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("translationApprovedUpdated", fileUnitProperties.TotalTranslationApprovedUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("signOffRejectedUpdated", fileUnitProperties.TotalSignOffRejectedUpdated.ToString());
                xmlTxtWriter.WriteAttributeString("signedOffUpdated", fileUnitProperties.TotalSignedOffUpdated.ToString());


                xmlTxtWriter.WriteAttributeString("notTranslatedUpdatedStatusChangesPercentage", filesTotalNotTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("draftUpdatedStatusChangesPercentage", filesTotalDraftChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("translatedUpdatedStatusChangesPercentage", filesTotalTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("translationRejectedUpdatedStatusChangesPercentage", filesTotalTranslationRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("translationApprovedUpdatedStatusChangesPercentage", filesTotalTranslationApprovedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("signOffRejectedUpdatedStatusChangesPercentage", filesTotalSignOffRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("signedOffUpdatedStatusChangesPercentage", filesTotalSignedOffChangesPercentage.ToString(CultureInfo.InvariantCulture));


                xmlTxtWriter.WriteEndElement();

                #endregion

                #endregion


                progressMaxSegments = filteredSegments;
                progressCurrentSegmentIndex = 0;
                progressPercentage = 0;
                progressCurrentMessage = "Saving report details...";

                ReportProgress(progressMaxFiles, progressCurrentFileIndex
                    , progressCurrentFileName, progressMaxSegments, progressCurrentSegmentIndex, progressPercentage
                    , progressCurrentMessage);


                if (fileComparisonFileParagraphUnit.Value == null)
                {
                    xmlTxtWriter.WriteStartElement("innerFiles");
                    xmlTxtWriter.WriteAttributeString("count", "0");

                    xmlTxtWriter.WriteEndElement();//innerFiles
                }
                else
                {
                    iFilesWithContentFiltered++;

                    if (Processor.Settings.ShowGoogleChartsInReport)
                    {
                        xsltContent.Insert(scriptIndex, ReportUtils.GetTranslationModificationsColumnChartScriptFilesLevel(fileId, fileUnitProperties, filesTotalContentChangesPercentage));
                        xsltContent.Insert(scriptIndex, ReportUtils.GetConfirmationStatisticsBarChartFilesLevel(fileId, fileUnitProperties));
                        xsltContent.Insert(scriptIndex, ReportUtils.GetTERpLineChartFilesLevel(fileId, terpAnalysisData.ChartData));
                        xsltContent.Insert(scriptIndex, ReportUtils.GetPEMLineChartFilesLevel(fileId, pempAnalysisData.ChartData, pempAnalysisData.ChartDataMerged));
                    }


                    var fileFilteredInnerFiles = FileFilteredInnerFiles(fileComparisonFileParagraphUnit);


                    xmlTxtWriter.WriteStartElement("innerFiles");
                    xmlTxtWriter.WriteAttributeString("count", fileFilteredInnerFiles.ToString());


                    foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                    {
                        var fileInnerId = "fileId_" + iFileIndex + "_" + iFileInnderIndex++;
                        var comparisonParagraphUnits = fileComparisonParagraphUnit.Value;


                        int innerFileFilteredParagraphCount;
                        int innerFileFilteredSegmentCount;
                        InnerFileFilteredParagraphCount(comparisonParagraphUnits, out innerFileFilteredParagraphCount, out innerFileFilteredSegmentCount);


                        if (innerFileFilteredParagraphCount <= 0)
                            continue;


                        xmlTxtWriter.WriteStartElement("innerFile");
                        xmlTxtWriter.WriteAttributeString("id", fileInnerId);
                        xmlTxtWriter.WriteAttributeString("showInnerFileName", Processor.Settings.IncludeIndividualFileInformation.ToString().ToLower());
                        xmlTxtWriter.WriteAttributeString("name", Path.GetFileName(fileComparisonParagraphUnit.Key));
                        xmlTxtWriter.WriteAttributeString("filteredParagraphCount", innerFileFilteredParagraphCount.ToString());
                        xmlTxtWriter.WriteAttributeString("filteredSegmentCount", innerFileFilteredSegmentCount.ToString());

                        xmlTxtWriter.WriteStartElement("paragraphs");
                        xmlTxtWriter.WriteAttributeString("count", innerFileFilteredParagraphCount.ToString());

                        foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                        {
                            if ((comparisonParagraphUnit.ParagraphIsUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonParagraphUnit.ParagraphIsUpdated)
                                && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonParagraphUnit.ParagraphStatusChanged)
                                && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonParagraphUnit.ParagraphHasComments))
                                continue;

                            xmlTxtWriter.WriteStartElement("paragraph");
                            xmlTxtWriter.WriteAttributeString("paragraphId", comparisonParagraphUnit.ParagraphId);


                            var paragraphFilteredSegmentCount = comparisonParagraphUnit.ComparisonSegmentUnits.Count(comparisonSegmentUnit => ((comparisonSegmentUnit.SegmentIsLocked && Processor.Settings.ReportFilterLockedSegments) || !comparisonSegmentUnit.SegmentIsLocked) && ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated) && ((!comparisonSegmentUnit.SegmentTextUpdated && Processor.Settings.ReportFilterSegmentsWithNoChanges) || (Processor.Settings.ReportFilterChangedTargetContent && comparisonSegmentUnit.SegmentTextUpdated) || (Processor.Settings.ReportFilterSegmentStatusChanged && comparisonSegmentUnit.SegmentSegmentStatusUpdated) || (Processor.Settings.ReportFilterSegmentsContainingComments && comparisonSegmentUnit.SegmentHasComments)));

                            xmlTxtWriter.WriteStartElement("segments");
                            xmlTxtWriter.WriteAttributeString("count", paragraphFilteredSegmentCount.ToString());

                            foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                            {
                                if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked)
                                    || !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
                                    || ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                        && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                        && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                        && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
                                    continue;

                                progressMaxSegments = filteredSegments;
                                progressCurrentSegmentIndex++;
                                progressPercentage = Convert.ToInt32(progressCurrentSegmentIndex <= progressMaxSegments && progressMaxSegments > 0 ? Convert.ToDecimal(progressCurrentSegmentIndex) / Convert.ToDecimal(progressMaxSegments) * Convert.ToDecimal(100) : progressMaxSegments);
                                progressCurrentMessage = "Writing report details...";

                                ReportProgress(progressMaxFiles, progressCurrentFileIndex
                                    , progressCurrentFileName, progressMaxSegments, progressCurrentSegmentIndex, progressPercentage
                                    , progressCurrentMessage);


                                WriteSegment(xmlTxtWriter
                                    , fileComparisonFileParagraphUnit.Key
                                    , comparisonParagraphUnit
                                    , comparisonSegmentUnit, pempDict
                                    , terpResults);
                            }
                            xmlTxtWriter.WriteEndElement();//segments


                            xmlTxtWriter.WriteEndElement();//paragraph
                        }
                        xmlTxtWriter.WriteEndElement();//paragraphs

                        xmlTxtWriter.WriteEndElement();//innerFile
                    }

                    xmlTxtWriter.WriteEndElement();//innerFiles

                }
                xmlTxtWriter.WriteEndElement();//file


            }


            #region  |  filesTotal  |

            xmlTxtWriter.WriteStartElement("filesTotal");

            const string filesIdTotal = "filesIdTotal";

            xmlTxtWriter.WriteAttributeString("id", filesIdTotal);


            #region  |  status changes  |

            var totalStatusChangesUpdated =
            filesTotalNotTranslatedUpdated
            + filesTotalDraftUpdated
            + filesTotalTranslatedUpdated
            + filesTotalTranslationRejectedUpdated
            + filesTotalTranslationApprovedUpdated
            + filesTotalSignOffRejectedUpdated
            + filesTotalSignedOffUpdated;


            decimal totalNotTranslatedChangesPercentage = 0;
            decimal totalDraftChangesPercentage = 0;
            decimal totalTranslatedChangesPercentage = 0;
            decimal totalTranslationRejectedChangesPercentage = 0;
            decimal totalTranslationApprovedChangesPercentage = 0;
            decimal totalSignOffRejectedChangesPercentage = 0;
            decimal totalSignedOffChangesPercentage = 0;



            if (totalStatusChangesUpdated > 0)
            {
                if (filesTotalNotTranslatedUpdated > filesTotalNotTranslatedOriginal)
                    totalNotTranslatedChangesPercentage = Math.Round((filesTotalNotTranslatedUpdated - filesTotalNotTranslatedOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalNotTranslatedOriginal > 0)
                    totalNotTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalNotTranslatedOriginal - filesTotalNotTranslatedUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalDraftUpdated > filesTotalDraftOriginal)
                    totalDraftChangesPercentage = Math.Round((filesTotalDraftUpdated - filesTotalDraftOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalDraftOriginal > 0)
                    totalDraftChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalDraftOriginal - filesTotalDraftUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalTranslatedUpdated > filesTotalTranslatedOriginal)
                    totalTranslatedChangesPercentage = Math.Round((filesTotalTranslatedUpdated - filesTotalTranslatedOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalTranslatedOriginal > 0)
                    totalTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalTranslatedOriginal - filesTotalTranslatedUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalTranslationRejectedUpdated > filesTotalTranslationRejectedOriginal)
                    totalTranslationRejectedChangesPercentage = Math.Round((filesTotalTranslationRejectedUpdated - filesTotalTranslationRejectedOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalTranslationRejectedOriginal > 0)
                    totalTranslationRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalTranslationRejectedOriginal - filesTotalTranslationRejectedUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalTranslationApprovedUpdated > filesTotalTranslationApprovedOriginal)
                    totalTranslationApprovedChangesPercentage = Math.Round((filesTotalTranslationApprovedUpdated - filesTotalTranslationApprovedOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalTranslationApprovedOriginal > 0)
                    totalTranslationApprovedChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalTranslationApprovedOriginal - filesTotalTranslationApprovedUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalSignOffRejectedUpdated > filesTotalSignOffRejectedOriginal)
                    totalSignOffRejectedChangesPercentage = Math.Round((filesTotalSignOffRejectedUpdated - filesTotalSignOffRejectedOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalSignOffRejectedOriginal > 0)
                    totalSignOffRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalSignOffRejectedOriginal - filesTotalSignOffRejectedUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));


                if (filesTotalSignedOffUpdated > filesTotalSignedOffOriginal)
                    totalSignedOffChangesPercentage = Math.Round((filesTotalSignedOffUpdated - filesTotalSignedOffOriginal) / totalStatusChangesUpdated * 100, 2);
                else if (filesTotalSignedOffOriginal > 0)
                    totalSignedOffChangesPercentage = Convert.ToDecimal("-" + Math.Round((filesTotalSignedOffOriginal - filesTotalSignedOffUpdated) / totalStatusChangesUpdated * 100, 2).ToString(CultureInfo.InvariantCulture));

            }

            filesTotalStatusChangesPercentage =
            (totalNotTranslatedChangesPercentage > 0 ? totalNotTranslatedChangesPercentage : 0)
            + (totalDraftChangesPercentage > 0 ? totalDraftChangesPercentage : 0)
            + (totalTranslatedChangesPercentage > 0 ? totalTranslatedChangesPercentage : 0)
            + (totalTranslationRejectedChangesPercentage > 0 ? totalTranslationRejectedChangesPercentage : 0)
            + (totalTranslationApprovedChangesPercentage > 0 ? totalTranslationApprovedChangesPercentage : 0)
            + (totalSignOffRejectedChangesPercentage > 0 ? totalSignOffRejectedChangesPercentage : 0)
            + (totalSignedOffChangesPercentage > 0 ? totalSignedOffChangesPercentage : 0);


            #endregion

            var totalTranslationChangesTotal =
            filesChangesPmSegments
            + filesChangesCmSegments
            + filesChangesExactSegments
            + filesChangesRepsSegments
            + filesChangesAtSegments
            + filesChangesFuzzy99Segments
            + filesChangesFuzzy94Segments
            + filesChangesFuzzy84Segments
            + filesChangesFuzzy74Segments
            + filesChangesNewSegments;


            if (totalTranslationChangesTotal > 0)
                filesTotalContentChangesPercentage = Math.Round(totalTranslationChangesTotal / filesSourceTotalSegments * 100, 2);


            xmlTxtWriter.WriteStartElement("totals");

            xmlTxtWriter.WriteAttributeString("segments", filesTotalSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("segmentsOriginal", filesTotalSegmentsOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("segmentsUpdated", filesTotalSegmentsUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("contentChanges", filesTotalContentChanges.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("contentChangesPercentage", filesTotalContentChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("statusChanges", filesTotalStatusChanges.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("statusChangesPercentage", filesTotalStatusChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("comments", filesTotalComments.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteEndElement();


            xmlTxtWriter.WriteStartElement("filtered");

            xmlTxtWriter.WriteAttributeString("paragraphs", filesFilteredParagraphs.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("segments", filesFilteredSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("contentChanges", filesFilteredContentChanges.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("statusChanges", filesFilteredStatusChanges.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("comments", filesFilteredComments.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteEndElement();



            #region  |  PEMp   |

            if (filesTotalPostEditTotalWords > 0)
            {
                filesTotalPostEditExactPercent = Math.Round(filesTotalPostEditExactWords, 2);
                if (filesTotalPostEditExactWords > 0)
                    filesTotalPostEditExactPercent = Math.Round(filesTotalPostEditExactWords / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditP99Percent = Math.Round(filesTotalPostEditP99Words, 2);
                if (filesTotalPostEditP99Words > 0)
                    filesTotalPostEditP99Percent = Math.Round(filesTotalPostEditP99Words / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditP94Percent = Math.Round(filesTotalPostEditP94Words, 2);
                if (filesTotalPostEditP94Words > 0)
                    filesTotalPostEditP94Percent = Math.Round(filesTotalPostEditP94Words / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditP84Percent = Math.Round(filesTotalPostEditP84Words, 2);
                if (filesTotalPostEditP84Words > 0)
                    filesTotalPostEditP84Percent = Math.Round(filesTotalPostEditP84Words / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditP74Percent = Math.Round(filesTotalPostEditP74Words, 2);
                if (filesTotalPostEditP74Words > 0)
                    filesTotalPostEditP74Percent = Math.Round(filesTotalPostEditP74Words / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditNewPercent = Math.Round(filesTotalPostEditNewWords, 2);
                if (filesTotalPostEditNewWords > 0)
                    filesTotalPostEditNewPercent = Math.Round(filesTotalPostEditNewWords / filesTotalPostEditTotalWords * 100, 2);

                filesTotalPostEditTotalPercent = 100;
            }

            xmlTxtWriter.WriteStartElement("pempAnalysis");

            #region  |  rates  |



            xmlTxtWriter.WriteStartElement("rates");


            xmlTxtWriter.WriteAttributeString("pmWords", "");
            xmlTxtWriter.WriteAttributeString("cmWords", "");
            xmlTxtWriter.WriteAttributeString("repetitionsWords", "");
            xmlTxtWriter.WriteAttributeString("exactWords", "");
            xmlTxtWriter.WriteAttributeString("fuzzy99Words", "");
            xmlTxtWriter.WriteAttributeString("fuzzy94Words", "");
            xmlTxtWriter.WriteAttributeString("fuzzy84Words", "");
            xmlTxtWriter.WriteAttributeString("fuzzy74Words", "");
            xmlTxtWriter.WriteAttributeString("newWords", "");

            xmlTxtWriter.WriteAttributeString("pmTotal", "0");
            xmlTxtWriter.WriteAttributeString("cmTotal", "0");
            xmlTxtWriter.WriteAttributeString("repetitionsTotal", "0");
            xmlTxtWriter.WriteAttributeString("exactTotal", Math.Round(filesTotalPostEditExactPrice, 2).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy99Total", Math.Round(filesTotalPostEditP99Price, 2).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy94Total", Math.Round(filesTotalPostEditP94Price, 2).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy84Total", Math.Round(filesTotalPostEditP84Price, 2).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy74Total", Math.Round(filesTotalPostEditP74Price, 2).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("newTotal", Math.Round(filesTotalPostEditNewPrice, 2).ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("priceTotal",
            (
                Math.Round(filesTotalPostEditExactPrice, 2)
                + Math.Round(filesTotalPostEditP99Price, 2)
                + Math.Round(filesTotalPostEditP94Price, 2)
                + Math.Round(filesTotalPostEditP84Price, 2)
                + Math.Round(filesTotalPostEditP74Price, 2)
                + Math.Round(filesTotalPostEditNewPrice, 2)
            ).ToString(CultureInfo.InvariantCulture));

			if (priceGroup != null)
			{
				xmlTxtWriter.WriteAttributeString("currency", priceGroup.Currency);
			}
           

            xmlTxtWriter.WriteEndElement();

            #endregion



            xmlTxtWriter.WriteStartElement("data");

            #region  |  data  |

            xmlTxtWriter.WriteAttributeString("exactSegments", filesTotalPostEditExactSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("exactWords", filesTotalPostEditExactWords.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("exactCharacters", filesTotalPostEditExactCharacters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("exactPercent", filesTotalPostEditExactPercent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("exactTags", filesTotalPostEditExactTags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("fuzzy99Segments", filesTotalPostEditP99Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy99Words", filesTotalPostEditP99Words.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy99Characters", filesTotalPostEditP99Characters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy99Percent", filesTotalPostEditP99Percent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy99Tags", filesTotalPostEditP99Tags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("fuzzy94Segments", filesTotalPostEditP94Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy94Words", filesTotalPostEditP94Words.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy94Characters", filesTotalPostEditP94Characters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy94Percent", filesTotalPostEditP94Percent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy94Tags", filesTotalPostEditP94Tags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("fuzzy84Segments", filesTotalPostEditP84Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy84Words", filesTotalPostEditP84Words.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy84Characters", filesTotalPostEditP84Characters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy84Percent", filesTotalPostEditP84Percent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy84Tags", filesTotalPostEditP84Tags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("fuzzy74Segments", filesTotalPostEditP74Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy74Words", filesTotalPostEditP74Words.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy74Characters", filesTotalPostEditP74Characters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy74Percent", filesTotalPostEditP74Percent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("fuzzy74Tags", filesTotalPostEditP74Tags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("newSegments", filesTotalPostEditNewSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("newWords", filesTotalPostEditNewWords.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("newCharacters", filesTotalPostEditNewCharacters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("newPercent", filesTotalPostEditNewPercent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("newTags", filesTotalPostEditNewTags.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("totalSegments", filesTotalPostEditTotalSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("totalWords", filesTotalPostEditTotalWords.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("totalCharacters", filesTotalPostEditTotalCharacters.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("totalPercent", filesTotalPostEditTotalPercent.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("tags", filesTotalPostEditTotalTags.ToString(CultureInfo.InvariantCulture));

            #endregion


            xmlTxtWriter.WriteEndElement(); //data

            xmlTxtWriter.WriteEndElement();

            #endregion

            #region  |  TERp  |

            xmlTxtWriter.WriteStartElement("terpAnalysis");

            xmlTxtWriter.WriteAttributeString("terp00SrcWd", filesTotalTerp00SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01SrcWd", filesTotalTerp01SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06SrcWd", filesTotalTerp06SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10SrcWd", filesTotalTerp10SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20SrcWd", filesTotalTerp20SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30SrcWd", filesTotalTerp30SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40SrcWd", filesTotalTerp40SrcWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50SrcWd", filesTotalTerp50SrcWd.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00NumEr", filesTotalTerp00NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01NumEr", filesTotalTerp01NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06NumEr", filesTotalTerp06NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10NumEr", filesTotalTerp10NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20NumEr", filesTotalTerp20NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30NumEr", filesTotalTerp30NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40NumEr", filesTotalTerp40NumEr.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50NumEr", filesTotalTerp50NumEr.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00NumWd", filesTotalTerp00NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01NumWd", filesTotalTerp01NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06NumWd", filesTotalTerp06NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10NumWd", filesTotalTerp10NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20NumWd", filesTotalTerp20NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30NumWd", filesTotalTerp30NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40NumWd", filesTotalTerp40NumWd.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50NumWd", filesTotalTerp50NumWd.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00Segments", filesTotalTerp00Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01Segments", filesTotalTerp01Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06Segments", filesTotalTerp06Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10Segments", filesTotalTerp10Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20Segments", filesTotalTerp20Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30Segments", filesTotalTerp30Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40Segments", filesTotalTerp40Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50Segments", filesTotalTerp50Segments.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00Ins", filesTotalTerp00Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01Ins", filesTotalTerp01Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06Ins", filesTotalTerp06Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10Ins", filesTotalTerp10Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20Ins", filesTotalTerp20Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30Ins", filesTotalTerp30Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40Ins", filesTotalTerp40Ins.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50Ins", filesTotalTerp50Ins.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00Del", filesTotalTerp00Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01Del", filesTotalTerp01Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06Del", filesTotalTerp06Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10Del", filesTotalTerp10Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20Del", filesTotalTerp20Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30Del", filesTotalTerp30Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40Del", filesTotalTerp40Del.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50Del", filesTotalTerp50Del.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00Sub", filesTotalTerp00Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01Sub", filesTotalTerp01Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06Sub", filesTotalTerp06Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10Sub", filesTotalTerp10Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20Sub", filesTotalTerp20Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30Sub", filesTotalTerp30Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40Sub", filesTotalTerp40Sub.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50Sub", filesTotalTerp50Sub.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteAttributeString("terp00Shft", filesTotalTerp00Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp01Shft", filesTotalTerp01Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp06Shft", filesTotalTerp06Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp10Shft", filesTotalTerp10Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp20Shft", filesTotalTerp20Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp30Shft", filesTotalTerp30Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp40Shft", filesTotalTerp40Shft.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("terp50Shft", filesTotalTerp50Shft.ToString(CultureInfo.InvariantCulture));


            xmlTxtWriter.WriteEndElement();

            #endregion

            #region  |  translationModifications  |

            xmlTxtWriter.WriteStartElement("translationModifications");


            xmlTxtWriter.WriteAttributeString("changesPMWords", decimal.Truncate(filesChangesPmWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesCMWords", decimal.Truncate(filesChangesCmWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesExactWords", decimal.Truncate(filesChangesExactWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesRepsWords", decimal.Truncate(filesChangesRepsWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesATWords", decimal.Truncate(filesChangesAtWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy99Words", decimal.Truncate(filesChangesFuzzy99Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy94Words", decimal.Truncate(filesChangesFuzzy94Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy84Words", decimal.Truncate(filesChangesFuzzy84Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy74Words", decimal.Truncate(filesChangesFuzzy74Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesNewWords", decimal.Truncate(filesChangesNewWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesTotalWords", decimal.Truncate(
            filesChangesPmWords
            + filesChangesCmWords
            + filesChangesExactWords
            + filesChangesRepsWords
            + filesChangesAtWords
            + filesChangesFuzzy99Words
            + filesChangesFuzzy94Words
            + filesChangesFuzzy84Words
            + filesChangesFuzzy74Words
            + filesChangesNewWords).ToString(CultureInfo.InvariantCulture));


            xmlTxtWriter.WriteAttributeString("changesPMCharacters", decimal.Truncate(filesChangesPmCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesCMCharacters", decimal.Truncate(filesChangesCmCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesExactCharacters", decimal.Truncate(filesChangesExactCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesRepsCharacters", decimal.Truncate(filesChangesRepsCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesATCharacters", decimal.Truncate(filesChangesAtCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy99Characters", decimal.Truncate(filesChangesFuzzy99Characters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy94Characters", decimal.Truncate(filesChangesFuzzy94Characters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy84Characters", decimal.Truncate(filesChangesFuzzy84Characters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy74Characters", decimal.Truncate(filesChangesFuzzy74Characters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesNewCharacters", decimal.Truncate(filesChangesNewCharacters).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesTotalCharacters", decimal.Truncate(
            filesChangesPmCharacters
            + filesChangesCmCharacters
            + filesChangesExactCharacters
            + filesChangesRepsCharacters
            + filesChangesAtCharacters
            + filesChangesFuzzy99Characters
            + filesChangesFuzzy94Characters
            + filesChangesFuzzy84Characters
            + filesChangesFuzzy74Characters
            + filesChangesNewCharacters).ToString(CultureInfo.InvariantCulture));



            xmlTxtWriter.WriteAttributeString("changesPMSegments", filesChangesPmSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesCMSegments", filesChangesCmSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesExactSegments", filesChangesExactSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesRepsSegments", filesChangesRepsSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesATSegments", filesChangesAtSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy99Segments", filesChangesFuzzy99Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy94Segments", filesChangesFuzzy94Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy84Segments", filesChangesFuzzy84Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesFuzzy74Segments", filesChangesFuzzy74Segments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesNewSegments", filesChangesNewSegments.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("changesTotalSegments",
            (filesChangesPmSegments
             + filesChangesCmSegments
             + filesChangesExactSegments
             + filesChangesRepsSegments
             + filesChangesAtSegments
             + filesChangesFuzzy99Segments
             + filesChangesFuzzy94Segments
             + filesChangesFuzzy84Segments
             + filesChangesFuzzy74Segments
             + filesChangesNewSegments).ToString(CultureInfo.InvariantCulture));



            xmlTxtWriter.WriteAttributeString("sourcePMSegments", decimal.Truncate(filesSourcePmSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceCMSegments", decimal.Truncate(filesSourceCmSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceExactSegments", decimal.Truncate(filesSourceExactSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceRepsSegments", decimal.Truncate(filesSourceRepsSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceATSegments", decimal.Truncate(filesSoruceAtSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy99Segments", decimal.Truncate(filesSourceFuzzy99Segments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy94Segments", decimal.Truncate(filesSourceFuzzy94Segments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy84Segments", decimal.Truncate(filesSourceFuzzy84Segments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy74Segments", decimal.Truncate(filesSourceFuzzy74Segments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceNewSegments", decimal.Truncate(filesSourceNewSegments).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceTotalSegments", decimal.Truncate(filesSourceTotalSegments).ToString(CultureInfo.InvariantCulture));


            xmlTxtWriter.WriteAttributeString("sourcePMWords", decimal.Truncate(filesSourcePmWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceCMWords", decimal.Truncate(filesSourceCmWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceExactWords", decimal.Truncate(filesSourceExactWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceRepsWords", decimal.Truncate(filesSourceRepsWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceATWords", decimal.Truncate(filesSoruceAtWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy99Words", decimal.Truncate(filesSourceFuzzy99Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy94Words", decimal.Truncate(filesSourceFuzzy94Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy84Words", decimal.Truncate(filesSourceFuzzy84Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceFuzzy74Words", decimal.Truncate(filesSourceFuzzy74Words).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceNewWords", decimal.Truncate(filesSourceNewWords).ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("sourceTotalWords", decimal.Truncate(filesSourceTotalWords).ToString(CultureInfo.InvariantCulture));


            xmlTxtWriter.WriteEndElement();

            #endregion

            #region  |  confirmationStatistics  |
            xmlTxtWriter.WriteStartElement("confirmationStatistics");

            xmlTxtWriter.WriteAttributeString("notTranslatedOriginal", filesTotalNotTranslatedOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("draftOriginal", filesTotalDraftOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translatedOriginal", filesTotalTranslatedOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationRejectedOriginal", filesTotalTranslationRejectedOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationApprovedOriginal", filesTotalTranslationApprovedOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signOffRejectedOriginal", filesTotalSignOffRejectedOriginal.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signedOffOriginal", filesTotalSignedOffOriginal.ToString(CultureInfo.InvariantCulture));


            xmlTxtWriter.WriteAttributeString("notTranslatedUpdated", filesTotalNotTranslatedUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("draftUpdated", filesTotalDraftUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translatedUpdated", filesTotalTranslatedUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationRejectedUpdated", filesTotalTranslationRejectedUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationApprovedUpdated", filesTotalTranslationApprovedUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signOffRejectedUpdated", filesTotalSignOffRejectedUpdated.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signedOffUpdated", filesTotalSignedOffUpdated.ToString(CultureInfo.InvariantCulture));



            xmlTxtWriter.WriteAttributeString("notTranslatedUpdatedStatusChangesPercentage", totalNotTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("draftUpdatedStatusChangesPercentage", totalDraftChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translatedUpdatedStatusChangesPercentage", totalTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationRejectedUpdatedStatusChangesPercentage", totalTranslationRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("translationApprovedUpdatedStatusChangesPercentage", totalTranslationApprovedChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signOffRejectedUpdatedStatusChangesPercentage", totalSignOffRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("signedOffUpdatedStatusChangesPercentage", totalSignedOffChangesPercentage.ToString(CultureInfo.InvariantCulture));

            xmlTxtWriter.WriteEndElement();

            #endregion

            if (Processor.Settings.ShowGoogleChartsInReport)
            {
                xsltContent.Insert(scriptIndex, ReportUtils.GetTranslationModificationsColumnChartScriptTotalsLevel(filesChangesPmSegments, filesSourcePmSegments, filesChangesCmSegments, filesSourceCmSegments, filesChangesRepsSegments, filesSourceRepsSegments, filesChangesExactSegments, filesSourceExactSegments, filesChangesFuzzy99Segments, filesChangesFuzzy94Segments, filesChangesFuzzy84Segments, filesChangesFuzzy74Segments, filesSourceFuzzy99Segments, filesSourceFuzzy94Segments, filesSourceFuzzy84Segments, filesSourceFuzzy74Segments, filesChangesNewSegments, filesSourceNewSegments, filesChangesAtSegments, filesSoruceAtSegments, filesTotalContentChangesPercentage));
                xsltContent.Insert(scriptIndex, ReportUtils.GetPEMColumnChartScriptTotalsLevel(filesTotalPostEditExactSegments, filesTotalPostEditP99Segments, filesTotalPostEditP94Segments, filesTotalPostEditP84Segments, filesTotalPostEditP74Segments, filesTotalPostEditNewSegments));
                xsltContent.Insert(scriptIndex, ReportUtils.GetPEMPieChartScriptTotalsLevel(filesTotalPostEditExactWords, filesTotalPostEditP99Words, filesTotalPostEditP94Words, filesTotalPostEditP84Words, filesTotalPostEditP74Words, filesTotalPostEditNewWords));
                xsltContent.Insert(scriptIndex, ReportUtils.GetConfirmationStatisticsBarChartTotalsLevel(filesTotalNotTranslatedOriginal, filesTotalNotTranslatedUpdated, filesTotalDraftOriginal, filesTotalDraftUpdated, filesTotalTranslatedOriginal, filesTotalTranslatedUpdated, filesTotalTranslationRejectedOriginal, filesTotalTranslationRejectedUpdated, filesTotalTranslationApprovedOriginal, filesTotalTranslationApprovedUpdated, filesTotalSignOffRejectedOriginal, filesTotalSignOffRejectedUpdated, filesTotalSignedOffOriginal, filesTotalSignedOffUpdated, filesTotalStatusChangesPercentage));
                xsltContent.Insert(scriptIndex, ReportUtils.GetTERpBarChartTotalsLevel(filesTotalTerp00Segments, filesTotalTerp01Segments, filesTotalTerp06Segments, filesTotalTerp10Segments, filesTotalTerp20Segments, filesTotalTerp30Segments, filesTotalTerp40Segments, filesTotalTerp50Segments));
                xsltContent.Insert(scriptIndex, ReportUtils.GetTERpPieChartTotalsLevel(filesTotalTerp00NumEr, filesTotalTerp01NumEr, filesTotalTerp06NumEr, filesTotalTerp10NumEr, filesTotalTerp20NumEr, filesTotalTerp30NumEr, filesTotalTerp40NumEr, filesTotalTerp50NumEr));
            }

            xmlTxtWriter.WriteEndElement();//filesTotal

            #endregion

            xmlTxtWriter.WriteEndElement();//files

            #endregion


            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();

            if (iFilesWithContentFiltered == 0 || !Processor.Settings.ShowGoogleChartsInReport)
            {
                var rScript = new Regex(@"\<script\s+type\=""text\/javascript""[^\>]*\>.*?\<\/script\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                xsltContent = new StringBuilder(rScript.Replace(xsltContent.ToString(), string.Empty));
            }

            using (var w = new StreamWriter(filePathXslt, false, Encoding.UTF8))
            {
                w.Write(xsltContent);
                w.Flush();
                w.Close();
            }

            if (transformReport)
                ReportUtils.TransformXmlReport(reportFilePath);
        }

        private static void InnerFileFilteredParagraphCount(Dictionary<string, Comparer.ComparisonParagraphUnit> comparisonParagraphUnits, out int innerFileFilteredParagraphCount, out int innerFileFilteredSegmentCount)
        {
            innerFileFilteredParagraphCount = 0;
            innerFileFilteredSegmentCount = 0;

            foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
            {
                var filteredAsegment = false;
                foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                {
                    if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked)
                        || !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
                        || ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                            && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                            && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                            && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
                        continue;
                    filteredAsegment = true;
                    innerFileFilteredSegmentCount++;
                }

                if (filteredAsegment)
                    innerFileFilteredParagraphCount++;
            }
        }

        private static int FileFilteredInnerFiles(KeyValuePair<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnit)
        {
            var fileFilteredInnerFiles = 0;
            foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
            {
                var comparisonParagraphUnits = fileComparisonParagraphUnit.Value;
                var testInnerFileFilteredParagraphCount = 0;

                foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                {
                    var filteredAsegment = false;
                    foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                    {
                        if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) &&
                             comparisonSegmentUnit.SegmentIsLocked) || !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
                            || ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                             && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                             && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                             && (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                              !comparisonSegmentUnit.SegmentHasComments)))
                            continue;
                        filteredAsegment = true;
                    }

                    if (filteredAsegment)
                        testInnerFileFilteredParagraphCount++;
                }

                if (testInnerFileFilteredParagraphCount > 0)
                    fileFilteredInnerFiles++;
            }
            return fileFilteredInnerFiles;
        }

        private static TERpAnalysisData GetTERpAnalysisData(TERp.DocumentResult terpResult
            , KeyValuePair<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnit
            , ref List<TERp.SegmentData> TERpErrors)
        {
            var terpAnalysisData = new TERpAnalysisData();

            if (terpResult == null)
                return terpAnalysisData;

            foreach (var segmentData in terpResult.SegmentDatas)
            {
                var wordCount = 0;
                var filtered = false;
                if (fileComparisonFileParagraphUnit.Value != null)
                {
                    foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                    {
                        if (Path.GetFileName(fileComparisonParagraphUnit.Key) != segmentData.FileName)
                            continue;
                        foreach (var comparisonParagraphUnit in fileComparisonParagraphUnit.Value.Values)
                        {
                            if (comparisonParagraphUnit.ParagraphId != segmentData.ParagraphId)
                                continue;

                            foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                            {
                                if (comparisonSegmentUnit.SegmentId != segmentData.SegmentId)
                                    continue;

                                filtered = true;
                                wordCount = comparisonSegmentUnit.SourceWordsOriginal;
                                break;
                            }
                            if (filtered)
                                break;
                        }
                        if (filtered)
                            break;
                    }
                }

                if (!filtered)
                    continue;


                terpAnalysisData.ChartData.Add("[" + terpAnalysisData.ChartData.Count + ", " + segmentData.Ins + ", " +
                                               segmentData.Del + ", " + segmentData.Sub + ", " + segmentData.Shft + "]");

                TERpErrors.Add(segmentData);

                if (segmentData.Terp <= 0)
                {
                    terpAnalysisData.terp00SrcWd += wordCount;
                    terpAnalysisData.terp00Ins += segmentData.Ins;
                    terpAnalysisData.terp00Del += segmentData.Del;
                    terpAnalysisData.terp00Sub += segmentData.Sub;
                    terpAnalysisData.terp00Shft += segmentData.Shft;
                    terpAnalysisData.terp00NumEr += segmentData.NumEr;
                    terpAnalysisData.terp00NumWd += segmentData.NumWd;
                    terpAnalysisData.terp00Segments++;
                }
                else if (segmentData.Terp <= 5)
                {
                    terpAnalysisData.terp01SrcWd += wordCount;
                    terpAnalysisData.terp01Ins += segmentData.Ins;
                    terpAnalysisData.terp01Del += segmentData.Del;
                    terpAnalysisData.terp01Sub += segmentData.Sub;
                    terpAnalysisData.terp01Shft += segmentData.Shft;
                    terpAnalysisData.terp01NumEr += segmentData.NumEr;
                    terpAnalysisData.terp01NumWd += segmentData.NumWd;
                    terpAnalysisData.terp01Segments++;
                }
                else if (segmentData.Terp <= 9)
                {
                    terpAnalysisData.terp06SrcWd += wordCount;
                    terpAnalysisData.terp06Ins += segmentData.Ins;
                    terpAnalysisData.terp06Del += segmentData.Del;
                    terpAnalysisData.terp06Sub += segmentData.Sub;
                    terpAnalysisData.terp06Shft += segmentData.Shft;
                    terpAnalysisData.terp06NumEr += segmentData.NumEr;
                    terpAnalysisData.terp06NumWd += segmentData.NumWd;
                    terpAnalysisData.terp06Segments++;
                }
                else if (segmentData.Terp <= 19)
                {
                    terpAnalysisData.terp10SrcWd += wordCount;
                    terpAnalysisData.terp10Ins += segmentData.Ins;
                    terpAnalysisData.terp10Del += segmentData.Del;
                    terpAnalysisData.terp10Sub += segmentData.Sub;
                    terpAnalysisData.terp10Shft += segmentData.Shft;
                    terpAnalysisData.terp10NumEr += segmentData.NumEr;
                    terpAnalysisData.terp10NumWd += segmentData.NumWd;
                    terpAnalysisData.terp10Segments++;
                }
                else if (segmentData.Terp <= 29)
                {
                    terpAnalysisData.terp20SrcWd += wordCount;
                    terpAnalysisData.terp20Ins += segmentData.Ins;
                    terpAnalysisData.terp20Del += segmentData.Del;
                    terpAnalysisData.terp20Sub += segmentData.Sub;
                    terpAnalysisData.terp20Shft += segmentData.Shft;
                    terpAnalysisData.terp20NumEr += segmentData.NumEr;
                    terpAnalysisData.terp20NumWd += segmentData.NumWd;
                    terpAnalysisData.terp20Segments++;
                }
                else if (segmentData.Terp <= 39)
                {
                    terpAnalysisData.terp30SrcWd += wordCount;
                    terpAnalysisData.terp30Ins += segmentData.Ins;
                    terpAnalysisData.terp30Del += segmentData.Del;
                    terpAnalysisData.terp30Sub += segmentData.Sub;
                    terpAnalysisData.terp30Shft += segmentData.Shft;
                    terpAnalysisData.terp30NumEr += segmentData.NumEr;
                    terpAnalysisData.terp30NumWd += segmentData.NumWd;
                    terpAnalysisData.terp30Segments++;
                }
                else if (segmentData.Terp <= 49)
                {
                    terpAnalysisData.terp40SrcWd += wordCount;
                    terpAnalysisData.terp40Ins += segmentData.Ins;
                    terpAnalysisData.terp40Del += segmentData.Del;
                    terpAnalysisData.terp40Sub += segmentData.Sub;
                    terpAnalysisData.terp40Shft += segmentData.Shft;
                    terpAnalysisData.terp40NumEr += segmentData.NumEr;
                    terpAnalysisData.terp40NumWd += segmentData.NumWd;
                    terpAnalysisData.terp40Segments++;
                }
                else
                {
                    terpAnalysisData.terp50SrcWd += wordCount;
                    terpAnalysisData.terp50Ins += segmentData.Ins;
                    terpAnalysisData.terp50Del += segmentData.Del;
                    terpAnalysisData.terp50Sub += segmentData.Sub;
                    terpAnalysisData.terp50Shft += segmentData.Shft;
                    terpAnalysisData.terp50NumEr += segmentData.NumEr;
                    terpAnalysisData.terp50NumWd += segmentData.NumWd;
                    terpAnalysisData.terp50Segments++;
                }
            }
            return terpAnalysisData;
        }

        private PEMpAnalysisData GetPEMpAnalysisData(KeyValuePair<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnit, Dictionary<string, PEMp> pempDict,
            int filteredSegments, int progressCurrentSegmentIndex, int progressMaxFiles, int progressCurrentFileIndex, string progressCurrentFileName, List<TERp.SegmentData> TERpErrors)
        {
            var pempAnalysisData = new PEMpAnalysisData();


            if (fileComparisonFileParagraphUnit.Value == null)
                return pempAnalysisData;


            foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
            {
                foreach (var comparisonParagraphUnit in fileComparisonParagraphUnit.Value.Values)
                {
                    foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                    {
                        var filtered = true;
                        if (Processor.Settings.CalculateSummaryAnalysisBasedOnFilteredRows)
                        {
                            if ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments))
                                filtered = false;

                            if (filtered)
                            {
                                if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked)
                                    || !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
                                    || ((comparisonSegmentUnit.SegmentTextUpdated ||
                                         !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                        && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                        && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                        && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
                                    filtered = false;
                            }
                        }

                        if (filtered)
                        {
                            decimal percentage = 100;


                            #region  |  Edit-distance Calculation |


                            var targetOriginalForCompareLen = GetCharLength(comparisonSegmentUnit.TargetOriginal);
                            var targetUpdatedForCompareLen = GetCharLength(comparisonSegmentUnit.TargetUpdated);


                            decimal editDistanceChars = Comparer.DamerauLevenshteinDistanceFromObject(
                                    comparisonSegmentUnit.TargetOriginal,
                                    comparisonSegmentUnit.TargetUpdated);
                            decimal modificationPercentage = 100;
                            decimal maxCharacters = targetOriginalForCompareLen > targetUpdatedForCompareLen
                                ? targetOriginalForCompareLen
                                : targetUpdatedForCompareLen;

                            if (editDistanceChars > 0 && targetOriginalForCompareLen > 0)
                            {
                                var charsDistPerTmp = (decimal)(editDistanceChars / maxCharacters);
                                modificationPercentage = 1.0m - charsDistPerTmp;

                                editDistanceChars = Math.Round(editDistanceChars, 2);
                                modificationPercentage = Math.Round(modificationPercentage * 100, 2);
                            }

                            if (targetOriginalForCompareLen == 0)
                            {
                                modificationPercentage = 0;
                                editDistanceChars = 0;
                            }

                            percentage = modificationPercentage;


                            var pempHolder = new PEMp(comparisonParagraphUnit.ParagraphId, comparisonSegmentUnit.SegmentId,
                                editDistanceChars, modificationPercentage, maxCharacters);
                            pempDict.Add(comparisonParagraphUnit.ParagraphId + "/" + comparisonSegmentUnit.SegmentId,
                                pempHolder);

                            #endregion

                            #region  |  percentage  apply  |

                            pempAnalysisData.totalSegments++;
                            pempAnalysisData.totalWords += comparisonSegmentUnit.SourceWordsOriginal;
                            pempAnalysisData.totalCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                            pempAnalysisData.totalPercent = 100;
                            pempAnalysisData.totalTags += comparisonSegmentUnit.SourceTagsOriginal;

                            var normalizedEditDistance = percentage == 0 ? 0 : Math.Round(100 - percentage, 2);

                            if (
                                string.Compare(comparisonSegmentUnit.TranslationOriginTypeUpdated, "interactive",
                                    StringComparison.OrdinalIgnoreCase) != 0)
                            {
                                if (editDistanceChars == 0)
                                {
                                    var trgo = ReportUtils.GetCompiledSegmentText(comparisonSegmentUnit.TargetOriginal, true);
                                    var trgu = ReportUtils.GetCompiledSegmentText(comparisonSegmentUnit.TargetUpdated, true);

                                    if (trgo != trgu)
                                        normalizedEditDistance = 100;
                                }
                            }


                            pempAnalysisData.ChartData.Add("[" + comparisonSegmentUnit.SourceWordsOriginal + ", " +
                                                           normalizedEditDistance + "]");

                            if (TERpErrors.Count > 0)
                            {
                                var TERpItem =
                                    TERpErrors.First(
                                        a =>
                                            a.ParagraphId == comparisonParagraphUnit.ParagraphId &&
                                            a.SegmentId == comparisonSegmentUnit.SegmentId);

                                pempAnalysisData.ChartDataMerged.Add(
                                    "[" + comparisonSegmentUnit.SourceWordsOriginal 
                                    + ", " + normalizedEditDistance
                                    + ", " + (TERpItem == null ? "0" : Math.Round(TERpItem.Terp > 100 ? 100 : TERpItem.Terp, 2).ToString(CultureInfo.InvariantCulture)) 
                                    + "]");


                            }

                            if (string.Compare(comparisonSegmentUnit.TranslationOriginTypeUpdated, "interactive",
                                    StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                if (percentage >= 100)
                                {
                                    pempAnalysisData.exactSegments++;
                                    pempAnalysisData.exactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.exactCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.exactPercent = 0;
                                    pempAnalysisData.exactTags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else if (percentage >= 95 && percentage < 100)
                                {
                                    pempAnalysisData.fuzzy99Segments++;
                                    pempAnalysisData.fuzzy99Words += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.fuzzy99Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.fuzzy99Percent = 0;
                                    pempAnalysisData.fuzzy99Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else if (percentage >= 85 && percentage < 95)
                                {
                                    pempAnalysisData.fuzzy94Segments++;
                                    pempAnalysisData.fuzzy94Words += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.fuzzy94Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.fuzzy94Percent = 0;
                                    pempAnalysisData.fuzzy94Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else if (percentage >= 75 && percentage < 85)
                                {
                                    pempAnalysisData.fuzzy84Segments++;
                                    pempAnalysisData.fuzzy84Words += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.fuzzy84Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.fuzzy84Percent = 0;
                                    pempAnalysisData.fuzzy84Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else if (percentage >= 50 && percentage < 75)
                                {
                                    pempAnalysisData.fuzzy74Segments++;
                                    pempAnalysisData.fuzzy74Words += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.fuzzy74Characters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.fuzzy74Percent = 0;
                                    pempAnalysisData.fuzzy74Tags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                                else
                                {
                                    pempAnalysisData.newSegments++;
                                    pempAnalysisData.newWords += comparisonSegmentUnit.SourceWordsOriginal;
                                    pempAnalysisData.newCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                    pempAnalysisData.newPercent = 0;
                                    pempAnalysisData.newTags += comparisonSegmentUnit.SourceTagsOriginal;
                                }
                            }
                            else
                            {
                                pempAnalysisData.exactSegments++;
                                pempAnalysisData.exactWords += comparisonSegmentUnit.SourceWordsOriginal;
                                pempAnalysisData.exactCharacters += comparisonSegmentUnit.SourceCharsOriginal;
                                pempAnalysisData.exactPercent = 0;
                                pempAnalysisData.exactTags += comparisonSegmentUnit.SourceTagsOriginal;
                            }

                            #endregion
                        }


                        var progressMaxSegments = filteredSegments;
                        progressCurrentSegmentIndex++;
                        var progressPercentage = Convert.ToInt32(progressCurrentSegmentIndex <= progressMaxSegments && progressMaxSegments > 0
                            ? Convert.ToDecimal(progressCurrentSegmentIndex) / Convert.ToDecimal(progressMaxSegments) *
                              Convert.ToDecimal(100)
                            : progressMaxSegments);
                        const string progressCurrentMessage = "Calculating report totals...";

                        ReportProgress(progressMaxFiles, progressCurrentFileIndex
                            , progressCurrentFileName, progressMaxSegments, progressCurrentSegmentIndex, progressPercentage
                            , progressCurrentMessage);
                    }
                }
            }



            return pempAnalysisData;
        }

        private static int GetCharLength(IEnumerable<SegmentSection> segmentSections)
        {
            var length = 0;
            foreach (var section in segmentSections)
            {
                if (section.RevisionMarker != null &&
                    section.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (section.Type == SegmentSection.ContentType.Text)
                    {
                        length += ReportUtils.GetTextSections(section.Content).SelectMany(part => part).Count();
                    }
                    else
                    {
                        length++;
                    }
                }
            }
            return length;
        }

        private static Settings.Price LanguageRate(Settings.PriceGroup priceGroup, Comparer.FileUnitProperties fileUnitProperties)
        {
            var languageRate = new Settings.Price();

			if (priceGroup != null)
			{
				foreach (var p in priceGroup.GroupPrices)
				{
					if (string.Compare(p.Source, fileUnitProperties.SourceLanguageIdUpdated, StringComparison.OrdinalIgnoreCase) != 0
						|| string.Compare(p.Target, fileUnitProperties.TargetLanguageIdUpdated, StringComparison.OrdinalIgnoreCase) != 0)
						continue;
					languageRate = p;
					break;
				}
			}
       
            if (languageRate.PriceBase <= 0)
            {
				if (priceGroup != null)
				{
					foreach (var p in priceGroup.GroupPrices)
					{
						if (string.Compare(p.Source, "*", StringComparison.OrdinalIgnoreCase) != 0
							|| string.Compare(p.Target, fileUnitProperties.TargetLanguageIdUpdated, StringComparison.OrdinalIgnoreCase) != 0)
							continue;
						languageRate = p;
						break;
					}
				}
                
            }
            if (languageRate.PriceBase <= 0)
            {
				if (priceGroup != null)
				{
					foreach (var p in priceGroup.GroupPrices)
					{
						if (
							string.Compare(p.Source, fileUnitProperties.SourceLanguageIdUpdated, StringComparison.OrdinalIgnoreCase) != 0
							|| string.Compare(p.Target, "*", StringComparison.OrdinalIgnoreCase) != 0)
							continue;
						languageRate = p;
						break;
					}
				}
             
            }
            if (languageRate.PriceBase <= 0)
            {
				if (priceGroup != null)
				{
					foreach (var p in priceGroup.GroupPrices)
					{
						if (string.Compare(p.Source, "*", StringComparison.OrdinalIgnoreCase) != 0
							|| string.Compare(p.Target, "*", StringComparison.OrdinalIgnoreCase) != 0)
							continue;
						languageRate = p;
						break;
					}
				}
              
            }


            return languageRate;
        }

        private static void FilteredContentChanges(KeyValuePair<Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnit, ref int filteredContentChanges,
            ref int filteredStatusChanges, ref int filteredComments, ref int filteredSegments, ref int filteredParagraphs)
        {
            if (fileComparisonFileParagraphUnit.Value == null)
                return;
            foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
            {
                foreach (var comparisonParagraphUnit in fileComparisonParagraphUnit.Value.Values)
                {
                    if ((comparisonParagraphUnit.ParagraphIsUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                        && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonParagraphUnit.ParagraphIsUpdated)
                        && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonParagraphUnit.ParagraphStatusChanged)
                        && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonParagraphUnit.ParagraphHasComments))
                        continue;

                    var filteredAsegment = false;
                    foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                    {
                        if (((!comparisonSegmentUnit.SegmentIsLocked 
                            || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked) 
                            || !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated) 
                            || ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
                                && (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
                                && (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                && (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
                            continue;

                        filteredContentChanges += comparisonSegmentUnit.SegmentTextUpdated ? 1 : 0;
                        filteredStatusChanges += comparisonSegmentUnit.SegmentSegmentStatusUpdated ? 1 : 0;
                        filteredComments += comparisonSegmentUnit.Comments != null ? comparisonSegmentUnit.Comments.Count : 0;

                        filteredAsegment = true;
                        filteredSegments++;
                    }

                    if (filteredAsegment)
                        filteredParagraphs++;
                }
            }
        }


        private static void WriteSegment(XmlWriter xmlTxtWriter
            , Comparer.FileUnitProperties fileComparisonFileUnitProperties
            , Comparer.ComparisonParagraphUnit comparisonParagraphUnit
            , Comparer.ComparisonSegmentUnit comparisonSegmentUnit
            , IReadOnlyDictionary<string, PEMp> pempDict
            , IEnumerable<TERp.DocumentResult> terpResults)
        {
            if (comparisonSegmentUnit.SegmentTextUpdated && string.Compare(comparisonSegmentUnit.TranslationOriginTypeUpdated, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                comparisonSegmentUnit.TranslationStatusUpdated = string.Empty;


            xmlTxtWriter.WriteStartElement("segment");
            xmlTxtWriter.WriteAttributeString("segmentId", comparisonSegmentUnit.SegmentId);


            #region  |  segmentTextUpdated  |
            xmlTxtWriter.WriteStartElement("segmentTextUpdated");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.SegmentTextUpdated.ToString());
            xmlTxtWriter.WriteEndElement();//segmentTextUpdated


            #endregion

            #region  |  segmentIsLockedOriginal  |
            xmlTxtWriter.WriteStartElement("segmentIsLockedOriginal");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.SegmentIsLocked.ToString());
            xmlTxtWriter.WriteEndElement();//segmentIsLockedOriginal
            #endregion

            #region  |  segmentStatusOriginal  |
            xmlTxtWriter.WriteStartElement("segmentStatusOriginal");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.SegmentStatusOriginal);
            xmlTxtWriter.WriteEndElement();//segmentStatusOriginal
            #endregion

            #region  |  segmentStatusUpdated  |
            xmlTxtWriter.WriteStartElement("segmentStatusUpdated");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.SegmentStatusUpdated);
            xmlTxtWriter.WriteEndElement();//segmentStatusUpdated


            #endregion

            #region  |  translationStatusOriginal  |
            xmlTxtWriter.WriteStartElement("translationStatusOriginal");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusOriginal);
            xmlTxtWriter.WriteEndElement();//translationStatusOriginal
            #endregion

            #region  |  translationStatusUpdated  |
            xmlTxtWriter.WriteStartElement("translationStatusUpdated");
            xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusUpdated);
            xmlTxtWriter.WriteEndElement();//translationStatusUpdated



            #endregion

            #region  |  source  |



            xmlTxtWriter.WriteStartElement("source");
            foreach (var xSegmentSection in comparisonSegmentUnit.Source)
            {
                if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                    {
                        var strList = ReportUtils.GetTextSections(xSegmentSection.Content);
                        foreach (var str in strList)
                        {
                            xmlTxtWriter.WriteStartElement("token");
                            xmlTxtWriter.WriteAttributeString("type", "text");
                            xmlTxtWriter.WriteString(str);
                            xmlTxtWriter.WriteEndElement();//span    
                        }
                    }
                    else
                    {
                        xmlTxtWriter.WriteStartElement("token");
                        xmlTxtWriter.WriteAttributeString("type", "tag");
                        if (Processor.Settings.TagVisualStyle != Settings.TagVisual.Full)
                            xmlTxtWriter.WriteAttributeString("tooltip", xSegmentSection.Content);
                        xmlTxtWriter.WriteString(ReportUtils.GetTagNameVisual(xSegmentSection.Content));
                        xmlTxtWriter.WriteEndElement();//span                                    
                    }
                }
            }
            xmlTxtWriter.WriteEndElement();//source


            #endregion

            #region  |  segmentStatus  |

            //Not Translated
            //Draft
            //Translated
            //Translation Rejected
            //Translation Approved
            //Sign-off Rejected
            //Signed Off

            xmlTxtWriter.WriteStartElement("segmentStatus");

            if (string.Compare(comparisonSegmentUnit.SegmentStatusOriginal, comparisonSegmentUnit.SegmentStatusUpdated, StringComparison.OrdinalIgnoreCase) == 0)
            {

                xmlTxtWriter.WriteStartElement("token");
                xmlTxtWriter.WriteAttributeString("type", "text");
                xmlTxtWriter.WriteString(ReportUtils.GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusUpdated));
                xmlTxtWriter.WriteEndElement();
            }
            else
            {
                xmlTxtWriter.WriteStartElement("token");
                xmlTxtWriter.WriteAttributeString("type", "textRemoved");
                xmlTxtWriter.WriteString(ReportUtils.GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusOriginal));
                xmlTxtWriter.WriteEndElement();

                xmlTxtWriter.WriteStartElement("token");
                xmlTxtWriter.WriteAttributeString("type", "br");
                xmlTxtWriter.WriteEndElement();

                xmlTxtWriter.WriteStartElement("token");
                xmlTxtWriter.WriteAttributeString("type", "textNew");
                xmlTxtWriter.WriteString(ReportUtils.GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusUpdated));
                xmlTxtWriter.WriteEndElement();
            }

            xmlTxtWriter.WriteEndElement();//segmentStatus




            #endregion

            #region  |  translationMatchType  |

            var matchColor = ReportUtils.GetMatchColor(comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationOriginTypeOriginal);

            xmlTxtWriter.WriteStartElement("translationMatchType");
            if (matchColor.Trim() != string.Empty)
                xmlTxtWriter.WriteAttributeString("backgroundColor", matchColor);

            if (string.Compare(comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated, StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (comparisonSegmentUnit.TranslationStatusOriginal.Trim() != string.Empty)
                {
                    xmlTxtWriter.WriteStartElement("token");
                    xmlTxtWriter.WriteAttributeString("type", "text");
                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusOriginal);
                    xmlTxtWriter.WriteEndElement();//span                                                                
                }
            }
            else
            {
                if (comparisonSegmentUnit.TranslationStatusOriginal.Trim() != string.Empty)
                {
                    xmlTxtWriter.WriteStartElement("token");
                    xmlTxtWriter.WriteAttributeString("type", "textRemoved");
                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusOriginal);
                    xmlTxtWriter.WriteEndElement();

                    if (comparisonSegmentUnit.TranslationStatusUpdated.Trim() != string.Empty)
                    {
                        xmlTxtWriter.WriteStartElement("token");
                        xmlTxtWriter.WriteAttributeString("type", "br");
                        xmlTxtWriter.WriteEndElement();
                    }
                }

                if (comparisonSegmentUnit.TranslationStatusUpdated.Trim() != string.Empty)
                {
                    xmlTxtWriter.WriteStartElement("token");
                    xmlTxtWriter.WriteAttributeString("type", "textNew");
                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusUpdated);
                    xmlTxtWriter.WriteEndElement();
                }
            }


            xmlTxtWriter.WriteEndElement();//translationMatchType




            #endregion

            #region  |  target (original)  |

            xmlTxtWriter.WriteStartElement("targetOriginal");
            foreach (var xSegmentSection in comparisonSegmentUnit.TargetOriginal)
            {
                if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                    {
                        var strList = ReportUtils.GetTextSections(xSegmentSection.Content);
                        foreach (var str in strList)
                        {
                            xmlTxtWriter.WriteStartElement("token");
                            xmlTxtWriter.WriteAttributeString("type", "text");
                            xmlTxtWriter.WriteString(str);
                            xmlTxtWriter.WriteEndElement();//span    
                        }
                    }
                    else
                    {
                        xmlTxtWriter.WriteStartElement("token");
                        xmlTxtWriter.WriteAttributeString("type", "tag");
                        if (Processor.Settings.TagVisualStyle != Settings.TagVisual.Full)
                            xmlTxtWriter.WriteAttributeString("tooltip", xSegmentSection.Content);
                        xmlTxtWriter.WriteString(ReportUtils.GetTagNameVisual(xSegmentSection.Content));
                        xmlTxtWriter.WriteEndElement();//span                                    
                    }
                }
            }
            xmlTxtWriter.WriteEndElement();//targetOriginal


            #endregion

            #region  |  target (Original) - Revision Markers  |
            xmlTxtWriter.WriteStartElement("targetOriginalRevisionMarkers");
            xmlTxtWriter.WriteAttributeString("count", comparisonSegmentUnit.TargetOriginalRevisionMarkers.Count.ToString());

            if (comparisonSegmentUnit.TargetUpdatedRevisionMarkers.Count > 0)
            {

                var xSegmentSectionRmOriginal = new Dictionary<string, List<SegmentSection>>();
                foreach (var xSegmentSection in comparisonSegmentUnit.TargetOriginalRevisionMarkers)
                {
                    var dateStr = xSegmentSection.RevisionMarker.Date.Year
                                  + "-" + xSegmentSection.RevisionMarker.Date.Month.ToString().PadLeft(2, '0')
                                  + "-" + xSegmentSection.RevisionMarker.Date.Day.ToString().PadLeft(2, '0')
                                  + "T" + xSegmentSection.RevisionMarker.Date.Hour.ToString().PadLeft(2, '0')
                                  + ":" + xSegmentSection.RevisionMarker.Date.Minute.ToString().PadLeft(2, '0')
                                  + ":" + xSegmentSection.RevisionMarker.Date.Second.ToString().PadLeft(2, '0');

                    if (!xSegmentSectionRmOriginal.ContainsKey(dateStr + "-" + xSegmentSection.RevisionMarker.Type))
                    {
                        var s1 = new List<SegmentSection> { xSegmentSection };
                        xSegmentSectionRmOriginal.Add(dateStr + "-" + xSegmentSection.RevisionMarker.Type, s1);
                    }
                    else
                    {
                        var s1 = xSegmentSectionRmOriginal[dateStr + "-" + xSegmentSection.RevisionMarker.Type];
                        s1.Add(xSegmentSection);
                        xSegmentSectionRmOriginal[dateStr + "-" + xSegmentSection.RevisionMarker.Type] = s1;
                    }
                }

                foreach (var kvp in xSegmentSectionRmOriginal)
                {
                    foreach (var xSegmentSection in kvp.Value)
                    {
                        xmlTxtWriter.WriteStartElement("revisionMarker");

                        xmlTxtWriter.WriteAttributeString("author", xSegmentSection.RevisionMarker.Author);
                        xmlTxtWriter.WriteAttributeString("date", xSegmentSection.RevisionMarker.Date.ToShortDateString()
                                                                  + " " + xSegmentSection.RevisionMarker.Date.ToShortTimeString());
                        xmlTxtWriter.WriteAttributeString("Type", xSegmentSection.RevisionMarker.Type.ToString());
                        xmlTxtWriter.WriteStartElement("content");
                        if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Unchanged)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {

                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "text");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tag");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                        }
                        else if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Insert)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "textNew");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tagNew");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                        }
                        else if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "textRemoved");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tagRemoved");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();
                            }
                        }
                        xmlTxtWriter.WriteEndElement();//content

                        xmlTxtWriter.WriteEndElement();//revisionMarker
                    }
                }
            }


            xmlTxtWriter.WriteEndElement();//targetOriginalRevisionMarkers

            #endregion

            #region  |  target (updated)  |


            xmlTxtWriter.WriteStartElement("targetUpdated");
            foreach (var xSegmentSection in comparisonSegmentUnit.TargetUpdated)
            {
                if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                {
                    //ignore from the comparison process
                }
                else
                {
                    if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                    {
                        var strList = ReportUtils.GetTextSections(xSegmentSection.Content);
                        foreach (var str in strList)
                        {
                            xmlTxtWriter.WriteStartElement("token");
                            xmlTxtWriter.WriteAttributeString("type", "text");
                            xmlTxtWriter.WriteString(str);
                            xmlTxtWriter.WriteEndElement();//token    
                        }
                    }
                    else
                    {
                        xmlTxtWriter.WriteStartElement("token");
                        xmlTxtWriter.WriteAttributeString("type", "tag");
                        if (Processor.Settings.TagVisualStyle != Settings.TagVisual.Full)
                            xmlTxtWriter.WriteAttributeString("tooltip", xSegmentSection.Content);
                        xmlTxtWriter.WriteString(ReportUtils.GetTagNameVisual(xSegmentSection.Content));
                        xmlTxtWriter.WriteEndElement();//token                                    
                    }
                }
            }
            xmlTxtWriter.WriteEndElement();//targetUpdated




            #endregion

            #region  |  target (Updated) - Revision Markers  |

            xmlTxtWriter.WriteStartElement("targetUpdatedRevisionMarkers");
            xmlTxtWriter.WriteAttributeString("count", comparisonSegmentUnit.TargetUpdatedRevisionMarkers.Count.ToString());

            if (comparisonSegmentUnit.TargetUpdatedRevisionMarkers.Count > 0)
            {
                var xSegmentSectionRmUpdated = new Dictionary<string, List<SegmentSection>>();
                foreach (var xSegmentSection in comparisonSegmentUnit.TargetUpdatedRevisionMarkers)
                {
                    var dateStr = xSegmentSection.RevisionMarker.Date.Year
                                  + "-" + xSegmentSection.RevisionMarker.Date.Month.ToString().PadLeft(2, '0')
                                  + "-" + xSegmentSection.RevisionMarker.Date.Day.ToString().PadLeft(2, '0')
                                  + "T" + xSegmentSection.RevisionMarker.Date.Hour.ToString().PadLeft(2, '0')
                                  + ":" + xSegmentSection.RevisionMarker.Date.Minute.ToString().PadLeft(2, '0')
                                  + ":" + xSegmentSection.RevisionMarker.Date.Second.ToString().PadLeft(2, '0');


                    if (!xSegmentSectionRmUpdated.ContainsKey(dateStr + "-" + xSegmentSection.RevisionMarker.Type))
                    {
                        var s1 = new List<SegmentSection> { xSegmentSection };
                        xSegmentSectionRmUpdated.Add(dateStr + "-" + xSegmentSection.RevisionMarker.Type, s1);
                    }
                    else
                    {
                        var s1 = xSegmentSectionRmUpdated[dateStr + "-" + xSegmentSection.RevisionMarker.Type];

                        s1.Add(xSegmentSection);
                        xSegmentSectionRmUpdated[dateStr + "-" + xSegmentSection.RevisionMarker.Type] = s1;
                    }
                }


                foreach (var kvp in xSegmentSectionRmUpdated)
                {
                    xmlTxtWriter.WriteStartElement("revisionMarker");

                    xmlTxtWriter.WriteAttributeString("author", kvp.Value[0].RevisionMarker.Author);
                    xmlTxtWriter.WriteAttributeString("date", kvp.Value[0].RevisionMarker.Date.ToShortDateString() + " "
                                                              + kvp.Value[0].RevisionMarker.Date.ToShortTimeString());
                    xmlTxtWriter.WriteAttributeString("Type", kvp.Value[0].RevisionMarker.Type.ToString());
                    xmlTxtWriter.WriteStartElement("content");

                    foreach (var xSegmentSection in kvp.Value)
                    {

                        if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Unchanged)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "text");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tag");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                        }
                        else if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Insert)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "textNew");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tagNew");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                        }
                        else if (xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "textRemoved");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tagRemoved");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                        }

                    }
                    xmlTxtWriter.WriteEndElement();//content

                    xmlTxtWriter.WriteEndElement();//revisionMarker
                }
            }


            xmlTxtWriter.WriteEndElement();//targetUpdatedRevisionMarkers

            #endregion

            #region  |  target comparison  |
            xmlTxtWriter.WriteStartElement("targetComparison");


            foreach (var comparisonTextUnit in comparisonSegmentUnit.ComparisonTextUnits)
            {
                if (comparisonTextUnit.ComparisonTextUnitType == Comparison.Text.TextComparer.ComparisonTextUnitType.Identical)
                {
                    foreach (var xSegmentSection in comparisonTextUnit.TextSections)
                    {
                        if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                        {
                            //ignore from the comparison process
                        }
                        else
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "text");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", "tag");
                                if (Processor.Settings.TagVisualStyle != Settings.TagVisual.Full)
                                    xmlTxtWriter.WriteAttributeString("tooltip", xSegmentSection.Content);
                                xmlTxtWriter.WriteString(ReportUtils.GetTagNameVisual(xSegmentSection.Content));
                                xmlTxtWriter.WriteEndElement();//token
                            }
                        }
                    }
                }
                else
                {
                    foreach (var xSegmentSection in comparisonTextUnit.TextSections)
                    {
                        if (xSegmentSection.RevisionMarker != null && xSegmentSection.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
                        {
                            //ignore from the comparison process
                        }
                        else
                        {
                            if (xSegmentSection.Type == SegmentSection.ContentType.Text)
                            {
                                xmlTxtWriter.WriteStartElement("token");
                                xmlTxtWriter.WriteAttributeString("type", comparisonTextUnit.ComparisonTextUnitType == Comparison.Text.TextComparer.ComparisonTextUnitType.New ? "textNew" : "textRemoved");
                                xmlTxtWriter.WriteString(xSegmentSection.Content);
                                xmlTxtWriter.WriteEndElement();//token
                            }
                            else
                            {

                                xmlTxtWriter.WriteStartElement("token");
                                if (Processor.Settings.TagVisualStyle != Settings.TagVisual.Full)
                                    xmlTxtWriter.WriteAttributeString("tooltip", xSegmentSection.Content);
                                xmlTxtWriter.WriteAttributeString("type", comparisonTextUnit.ComparisonTextUnitType == Comparison.Text.TextComparer.ComparisonTextUnitType.New ? "tagNew" : "tagRemoved");
                                xmlTxtWriter.WriteString(ReportUtils.GetTagNameVisual(xSegmentSection.Content));
                                xmlTxtWriter.WriteEndElement();//token
                            }
                        }
                    }
                }
            }
            xmlTxtWriter.WriteEndElement();//targetComparison

            #endregion


            var pemp = pempDict.ContainsKey(comparisonParagraphUnit.ParagraphId + "/" + comparisonSegmentUnit.SegmentId)
                    ? pempDict[comparisonParagraphUnit.ParagraphId + "/" + comparisonSegmentUnit.SegmentId]
                    : null;
            var trgo = ReportUtils.GetCompiledSegmentText(comparisonSegmentUnit.TargetOriginal, true);
            var trgu = ReportUtils.GetCompiledSegmentText(comparisonSegmentUnit.TargetUpdated, true);

            TERp.SegmentData terpSegmentData = null;
            var terpResult = terpResults.SingleOrDefault(a => a.OriginalDocumentPath == fileComparisonFileUnitProperties.FilePathOriginal && a.UpdatedDocumentPath == fileComparisonFileUnitProperties.FilePathUpdated);

            if (terpResult != null)
                terpSegmentData = terpResult.SegmentDatas.SingleOrDefault(a => a.ParagraphId == comparisonParagraphUnit.ParagraphId && a.SegmentId == comparisonSegmentUnit.SegmentId);



            #region  |  comments  |

            if (comparisonSegmentUnit.SegmentHasComments)
            {
                xmlTxtWriter.WriteStartElement("comments");
                xmlTxtWriter.WriteAttributeString("count", comparisonSegmentUnit.Comments.Count.ToString());

                foreach (var comment in comparisonSegmentUnit.Comments)
                {
                    xmlTxtWriter.WriteStartElement("comment");

                    xmlTxtWriter.WriteAttributeString("author", comment.Author);
                    xmlTxtWriter.WriteAttributeString("date", comment.Date.ToShortDateString());
                    xmlTxtWriter.WriteAttributeString("dateSpecified", comment.DateSpecified.ToString());
                    xmlTxtWriter.WriteAttributeString("severity", comment.Severity);
                    xmlTxtWriter.WriteAttributeString("version", comment.Version);


                    xmlTxtWriter.WriteString(comment.Text);

                    xmlTxtWriter.WriteEndElement();//comment
                }

                xmlTxtWriter.WriteEndElement();//comments
            }
            #endregion

            #region  |  statistics  |

            xmlTxtWriter.WriteStartElement("statistics");

            xmlTxtWriter.WriteStartElement("source");
            xmlTxtWriter.WriteAttributeString("words", comparisonSegmentUnit.SourceWordsOriginal.ToString());
            xmlTxtWriter.WriteAttributeString("characters", comparisonSegmentUnit.SourceCharsOriginal.ToString());
            xmlTxtWriter.WriteAttributeString("tags", comparisonSegmentUnit.SourceTagsOriginal.ToString());
            xmlTxtWriter.WriteAttributeString("placeables", comparisonSegmentUnit.SourcePlaceablesOriginal.ToString());
            xmlTxtWriter.WriteEndElement();//source  


            xmlTxtWriter.WriteStartElement("target");

            if (pemp != null)
            {
                xmlTxtWriter.WriteStartElement("pemp");
                if (string.Compare(comparisonSegmentUnit.TranslationOriginTypeUpdated, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    xmlTxtWriter.WriteAttributeString("type", "normal");
                }
                else
                {
                    xmlTxtWriter.WriteAttributeString("type", (trgo != trgu
                        ? Convert.ToInt32(pemp.ModificationPercentage)
                        : 100) != 100
                            ? "updated"
                            : "added");
                }
                xmlTxtWriter.WriteAttributeString("pemp", trgo != trgu ? pemp.ModificationPercentage.ToString(CultureInfo.InvariantCulture) : "100");
                xmlTxtWriter.WriteAttributeString("editDist", pemp.EditDistance.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("maxChars", pemp.MaxCharacters.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteEndElement(); //pemp
            }

            if (terpSegmentData != null)
            {
                xmlTxtWriter.WriteStartElement("terp");
                xmlTxtWriter.WriteAttributeString("ins", terpSegmentData.Ins.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("del", terpSegmentData.Del.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("sub", terpSegmentData.Sub.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("stem", terpSegmentData.Stem.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("syn", terpSegmentData.Syn.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("phrase", terpSegmentData.Phrase.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("shft", terpSegmentData.Shft.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("wdsh", terpSegmentData.Wdsh.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("numEr", terpSegmentData.NumEr.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("numWd", terpSegmentData.NumWd.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("terp", terpSegmentData.Terp.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteEndElement();//terp  
            }

            xmlTxtWriter.WriteEndElement();//target  

            xmlTxtWriter.WriteEndElement();//statistics

            #endregion


            xmlTxtWriter.WriteEndElement();//segment


        }


        private class PEMp
        {
            public string ParagraphId { get; set; }
            public string SegmentId { get; set; }
            public decimal EditDistance { get; private set; }
            public decimal ModificationPercentage { get; private set; }
            public decimal MaxCharacters { get; private set; }

            public PEMp()
            {
                SegmentId = string.Empty;
                EditDistance = 0;
                ModificationPercentage = 0;
                MaxCharacters = 0;
            }
            public PEMp(string paragraphid, string segmentId, decimal editDistance, decimal modificationPercentage, decimal maxCharacters)
            {
                ParagraphId = paragraphid;
                SegmentId = segmentId;
                EditDistance = editDistance;
                ModificationPercentage = modificationPercentage;
                MaxCharacters = maxCharacters;
            }

        }


        public class PEMpAnalysisData
        {
            public decimal exactSegments { get; set; }
            public decimal exactWords { get; set; }
            public decimal exactCharacters { get; set; }
            public decimal exactPercent { get; set; }
            public decimal exactTags { get; set; }

            public decimal fuzzy99Segments { get; set; }
            public decimal fuzzy99Words { get; set; }
            public decimal fuzzy99Characters { get; set; }
            public decimal fuzzy99Percent { get; set; }
            public decimal fuzzy99Tags { get; set; }

            public decimal fuzzy94Segments { get; set; }
            public decimal fuzzy94Words { get; set; }
            public decimal fuzzy94Characters { get; set; }
            public decimal fuzzy94Percent { get; set; }
            public decimal fuzzy94Tags { get; set; }

            public decimal fuzzy84Segments { get; set; }
            public decimal fuzzy84Words { get; set; }
            public decimal fuzzy84Characters { get; set; }
            public decimal fuzzy84Percent { get; set; }
            public decimal fuzzy84Tags { get; set; }

            public decimal fuzzy74Segments { get; set; }
            public decimal fuzzy74Words { get; set; }
            public decimal fuzzy74Characters { get; set; }
            public decimal fuzzy74Percent { get; set; }
            public decimal fuzzy74Tags { get; set; }

            public decimal newSegments { get; set; }
            public decimal newWords { get; set; }
            public decimal newCharacters { get; set; }
            public decimal newPercent { get; set; }
            public decimal newTags { get; set; }


            public decimal totalSegments { get; set; }
            public decimal totalWords { get; set; }
            public decimal totalCharacters { get; set; }
            public decimal totalPercent { get; set; }
            public decimal totalTags { get; set; }

            public List<string> ChartData { get; set; }
            public List<string> ChartDataMerged { get; set; }

            public PEMpAnalysisData()
            {
                exactSegments = 0;
                exactWords = 0;
                exactCharacters = 0;
                exactPercent = 0;
                exactTags = 0;

                fuzzy99Segments = 0;
                fuzzy99Words = 0;
                fuzzy99Characters = 0;
                fuzzy99Percent = 0;
                fuzzy99Tags = 0;

                fuzzy94Segments = 0;
                fuzzy94Words = 0;
                fuzzy94Characters = 0;
                fuzzy94Percent = 0;
                fuzzy94Tags = 0;

                fuzzy84Segments = 0;
                fuzzy84Words = 0;
                fuzzy84Characters = 0;
                fuzzy84Percent = 0;
                fuzzy84Tags = 0;

                fuzzy74Segments = 0;
                fuzzy74Words = 0;
                fuzzy74Characters = 0;
                fuzzy74Percent = 0;
                fuzzy74Tags = 0;

                newSegments = 0;
                newWords = 0;
                newCharacters = 0;
                newPercent = 0;
                newTags = 0;


                totalSegments = 0;
                totalWords = 0;
                totalCharacters = 0;
                totalPercent = 0;
                totalTags = 0;

                ChartData = new List<string>();
                ChartDataMerged = new List<string>();
            }
        }

        private class TERpAnalysisData
        {

            public decimal terp00NumEr { get; set; }
            public decimal terp01NumEr { get; set; }
            public decimal terp06NumEr { get; set; }
            public decimal terp10NumEr { get; set; }
            public decimal terp20NumEr { get; set; }
            public decimal terp30NumEr { get; set; }
            public decimal terp40NumEr { get; set; }
            public decimal terp50NumEr { get; set; }

            public decimal terp00NumWd { get; set; }
            public decimal terp01NumWd { get; set; }
            public decimal terp06NumWd { get; set; }
            public decimal terp10NumWd { get; set; }
            public decimal terp20NumWd { get; set; }
            public decimal terp30NumWd { get; set; }
            public decimal terp40NumWd { get; set; }
            public decimal terp50NumWd { get; set; }

            public decimal terp00SrcWd { get; set; }
            public decimal terp01SrcWd { get; set; }
            public decimal terp06SrcWd { get; set; }
            public decimal terp10SrcWd { get; set; }
            public decimal terp20SrcWd { get; set; }
            public decimal terp30SrcWd { get; set; }
            public decimal terp40SrcWd { get; set; }
            public decimal terp50SrcWd { get; set; }

            public decimal terp00Segments { get; set; }
            public decimal terp01Segments { get; set; }
            public decimal terp06Segments { get; set; }
            public decimal terp10Segments { get; set; }
            public decimal terp20Segments { get; set; }
            public decimal terp30Segments { get; set; }
            public decimal terp40Segments { get; set; }
            public decimal terp50Segments { get; set; }

            public decimal terp00Ins { get; set; }
            public decimal terp01Ins { get; set; }
            public decimal terp06Ins { get; set; }
            public decimal terp10Ins { get; set; }
            public decimal terp20Ins { get; set; }
            public decimal terp30Ins { get; set; }
            public decimal terp40Ins { get; set; }
            public decimal terp50Ins { get; set; }

            public decimal terp00Del { get; set; }
            public decimal terp01Del { get; set; }
            public decimal terp06Del { get; set; }
            public decimal terp10Del { get; set; }
            public decimal terp20Del { get; set; }
            public decimal terp30Del { get; set; }
            public decimal terp40Del { get; set; }
            public decimal terp50Del { get; set; }

            public decimal terp00Sub { get; set; }
            public decimal terp01Sub { get; set; }
            public decimal terp06Sub { get; set; }
            public decimal terp10Sub { get; set; }
            public decimal terp20Sub { get; set; }
            public decimal terp30Sub { get; set; }
            public decimal terp40Sub { get; set; }
            public decimal terp50Sub { get; set; }


            public decimal terp00Shft { get; set; }
            public decimal terp01Shft { get; set; }
            public decimal terp06Shft { get; set; }
            public decimal terp10Shft { get; set; }
            public decimal terp20Shft { get; set; }
            public decimal terp30Shft { get; set; }
            public decimal terp40Shft { get; set; }
            public decimal terp50Shft { get; set; }


            public List<string> ChartData { get; set; }

            public TERpAnalysisData()
            {

                terp00NumEr = 0;
                terp01NumEr = 0;
                terp06NumEr = 0;
                terp10NumEr = 0;
                terp20NumEr = 0;
                terp30NumEr = 0;
                terp40NumEr = 0;
                terp50NumEr = 0;

                terp00NumWd = 0;
                terp01NumWd = 0;
                terp06NumWd = 0;
                terp10NumWd = 0;
                terp20NumWd = 0;
                terp30NumWd = 0;
                terp40NumWd = 0;
                terp50NumWd = 0;

                terp00SrcWd = 0;
                terp01SrcWd = 0;
                terp06SrcWd = 0;
                terp10SrcWd = 0;
                terp20SrcWd = 0;
                terp30SrcWd = 0;
                terp40SrcWd = 0;
                terp50SrcWd = 0;

                terp00Segments = 0;
                terp01Segments = 0;
                terp06Segments = 0;
                terp10Segments = 0;
                terp20Segments = 0;
                terp30Segments = 0;
                terp40Segments = 0;
                terp50Segments = 0;

                terp00Ins = 0;
                terp01Ins = 0;
                terp06Ins = 0;
                terp10Ins = 0;
                terp20Ins = 0;
                terp30Ins = 0;
                terp40Ins = 0;
                terp50Ins = 0;

                terp00Del = 0;
                terp01Del = 0;
                terp06Del = 0;
                terp10Del = 0;
                terp20Del = 0;
                terp30Del = 0;
                terp40Del = 0;
                terp50Del = 0;

                terp00Sub = 0;
                terp01Sub = 0;
                terp06Sub = 0;
                terp10Sub = 0;
                terp20Sub = 0;
                terp30Sub = 0;
                terp40Sub = 0;
                terp50Sub = 0;


                terp00Shft = 0;
                terp01Shft = 0;
                terp06Shft = 0;
                terp10Shft = 0;
                terp20Shft = 0;
                terp30Shft = 0;
                terp40Shft = 0;
                terp50Shft = 0;

                ChartData = new List<string>();
            }
        }
    }
}

