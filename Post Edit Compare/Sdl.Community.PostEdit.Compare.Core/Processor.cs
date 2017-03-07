using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.PostEdit.Compare.Core.Comparison;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;

namespace Sdl.Community.PostEdit.Compare.Core
{
    public class Processor
    {
        public delegate void ChangedEventHandlerComparer(int maximum, int current, int percent, string message);
        public delegate void ChangedEventHandlerParser(int maximum, int current, int percent, string message);
        public delegate void ChangedEventHandlerFiles(int maximum, int current, int percent, string message);
        public delegate void ChangedEventHandlerReport(int filesMax, int filesCurrent, string fileNameCurrent, int fileMaximum, int fileCurrent, int filePercent, string message);

        public event ChangedEventHandlerComparer ProgressComparer;
        public event ChangedEventHandlerParser ProgressParser;
        public event ChangedEventHandlerFiles ProgressFiles;
        public event ChangedEventHandlerReport ProgressReport;


        private static readonly string SettingsFilePath;
        public static Settings Settings;

        public int ComparisonTotalFiles { get; private set; }
        public int ComparisonTotalSegments { get; private set; }
        public int ComparisonTotalContentChanges { get; private set; }
        public int ComparisonTotalStatusChanges { get; private set; }
        public int ComparisonTotalComments { get; private set; }

        public Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>> FileComparisonParagraphUnits { get; set; }



        static Processor()
        {
            #region  |  initialize and serialize the settings for the calling client  |


            SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PostEdit.Compare");

            if (!Directory.Exists(SettingsFilePath))
                Directory.CreateDirectory(SettingsFilePath);

            SettingsFilePath = Path.Combine(SettingsFilePath, "PostEdit.Compare.Core.settings.xml");

            ReadSettings(SettingsFilePath);

            #endregion
        }


        #region  |  process files  |

        public Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
            ProcessFiles(List<PairedFiles.PairedFile> pairedFiles)
        {
            ComparisonTotalFiles = 0;
            ComparisonTotalSegments = 0;
            ComparisonTotalContentChanges = 0;
            ComparisonTotalStatusChanges = 0;
            ComparisonTotalComments = 0;

            var fileComparisonParagraphUnits = new Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>();

            var comparer = new Comparison.Comparer();
            try
            {
                comparer.Progress += comparer_Progress;
                var sdlXliffParser = new Parser();
                try
                {
                    sdlXliffParser.Progress += parser_Progress;
                    var fileCurrent = 0;
                    var fileMaximum = pairedFiles.Count;
                    foreach (var pairedFile in pairedFiles)
                    {
                        fileCurrent++;
                        var filePercent = Convert.ToInt32(fileCurrent <= fileMaximum && fileMaximum > 0 ? (Convert.ToDecimal(fileCurrent) / Convert.ToDecimal(fileMaximum)) * Convert.ToDecimal(100) : fileMaximum);
                        var fileName = (pairedFile.OriginalFilePath != null ? pairedFile.OriginalFilePath.Name : pairedFile.UpdatedFilePath.Name);

                        files_Progress(fileMaximum, fileCurrent, filePercent, fileName);

                        if (!pairedFile.IsError)
                        {
                            try
                            {
                                #region  |  process paired file  |

                                if (pairedFile.OriginalFilePath != null)
                                {
                                    sdlXliffParser.GetParagraphUnits(pairedFile.OriginalFilePath.FullName, pairedFile.UpdatedFilePath.FullName);

                                    var comparisonParagraphUnits =
                                        comparer.GetComparisonParagraphUnits(sdlXliffParser.FileParagraphUnitsOriginal, sdlXliffParser.FileParagraphUnitsUpdated);


                                    fileComparisonParagraphUnits.Add(new Comparison.Comparer.FileUnitProperties(pairedFile.OriginalFilePath.FullName, pairedFile.UpdatedFilePath.FullName
                                            , sdlXliffParser.GetSegmentCount(sdlXliffParser.FileParagraphUnitsOriginal)
                                            , sdlXliffParser.GetSegmentCount(sdlXliffParser.FileParagraphUnitsUpdated)
                                            , comparisonParagraphUnits, sdlXliffParser.SourceLanguageIdOriginal, sdlXliffParser.TargetLanguageIdOriginal, sdlXliffParser.SourceLanguageIdUpdated, sdlXliffParser.TargetLanguageIdUpdated)
                                        , comparisonParagraphUnits);
                                }

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("File: " + pairedFile.UpdatedFilePath.FullName + "; \r\n" + ex.Message);
                            }
                        }
                        else
                        {
                            fileComparisonParagraphUnits.Add(
                                pairedFile.OriginalFilePath != null
                                    ? new Comparison.Comparer.FileUnitProperties(pairedFile.OriginalFilePath.FullName, string.Empty,
                                        0, 0, null, sdlXliffParser.SourceLanguageIdOriginal,
                                        sdlXliffParser.TargetLanguageIdOriginal, sdlXliffParser.SourceLanguageIdUpdated,
                                        sdlXliffParser.TargetLanguageIdUpdated)
                                    : new Comparison.Comparer.FileUnitProperties(string.Empty, pairedFile.UpdatedFilePath.FullName,
                                        0, 0, null, sdlXliffParser.SourceLanguageIdOriginal,
                                        sdlXliffParser.TargetLanguageIdOriginal, sdlXliffParser.SourceLanguageIdUpdated,
                                        sdlXliffParser.TargetLanguageIdUpdated), null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sdlXliffParser.Progress -= parser_Progress;
                }
            }
            finally
            {
                comparer.Progress -= comparer_Progress;
            }



            SetTotalCounters(fileComparisonParagraphUnits);



            return fileComparisonParagraphUnits;
        }


        private void SetTotalCounters(Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>> fileComparisonParagraphUnits)
        {
            foreach (var fileComparisonParagraphUnit in fileComparisonParagraphUnits)
            {
                var fileUnitProperties = fileComparisonParagraphUnit.Key;

                ComparisonTotalFiles++;
                ComparisonTotalSegments += fileUnitProperties.TotalSegments;
                ComparisonTotalContentChanges += fileUnitProperties.TotalContentChanges;
                ComparisonTotalStatusChanges += fileUnitProperties.TotalStatusChanges;
                ComparisonTotalComments += fileUnitProperties.TotalComments;
            }
        }


        #endregion

        #region |  create report  |


        public void CreateReport(string reportFilePath,
            Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>> fileComparisonParagraphUnits
            , Settings.PriceGroup priceGroup, out List<TERp.DocumentResult> terpResults)
        {
            try
            {
                var report = new Reports.Report();

                report.ProgressReport += Report_Progress_report;
                var transformXmlReport = (Settings.reportFormat == Settings.ReportFormat.Html ? true : false);


                report.CreateXmlReport(reportFilePath, fileComparisonParagraphUnits, transformXmlReport, priceGroup, out terpResults);


                if (Settings.ViewReportWhenFinishedProcessing)
                {
                    if (transformXmlReport)
                        reportFilePath += ".html";

                    System.Diagnostics.Process.Start(reportFilePath);
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion

        #region  |  progress callbacks  |

        private void comparer_Progress(int maximum, int current, int percent, string message)
        {
            if (ProgressComparer != null)
                ProgressComparer(maximum, current, percent, message);
        }
        private void parser_Progress(int maximum, int current, int percent, string message)
        {
            if (ProgressParser != null)
                ProgressParser(maximum, current, percent, message);
        }
        private void files_Progress(int maximum, int current, int percent, string message)
        {
            if (ProgressFiles != null)
                ProgressFiles(maximum, current, percent, message);
        }

        private void Report_Progress_report(int filesMax, int filesCurrent, string fileNameCurrent, int fileMaximum, int fileCurrent, int filePercent, string message)
        {
            if (ProgressReport != null)
                ProgressReport(filesMax, filesCurrent, fileNameCurrent, fileMaximum, fileCurrent, filePercent, message);
        }
        #endregion

        #region  |  settings serialization  |

        private static void BeforeSaveSettings()
        {
            if (Settings.FilePathCustomStyleSheet.Trim() != string.Empty && !File.Exists(Processor.Settings.FilePathCustomStyleSheet))
                Settings.UseCustomStyleSheet = false;

        }
        private static void AfterReadSettings()
        {
            //assign the reportfileName with todays date
            Settings.ReportFileName = "PostEdit.Compare.Report "
                + DateTime.Now.Year
                + "-" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + "-" + DateTime.Now.Day.ToString().PadLeft(2, '0');
        }

        public static void SaveSettings()
        {
            BeforeSaveSettings();

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, Settings);
            }           
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }
        private static void ReadSettings(string filename)
        {
            if (!File.Exists(filename))
            {
                Settings = new Settings();
                SaveSettings();
            }

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(filename, FileMode.Open);
                Settings = (Settings)serializer.Deserialize(stream) ?? new Settings();

                AfterReadSettings();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

        #endregion
    }
}
