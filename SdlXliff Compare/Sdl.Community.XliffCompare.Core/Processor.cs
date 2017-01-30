using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.XliffCompare.Core.Comparer;
using Sdl.Community.XliffCompare.Core.SDLXLIFF;

namespace Sdl.Community.XliffCompare.Core
{
    public class Processor
    {
        public delegate void ChangedEventHandler(int maximum, int current, int percent, string message);
        public event ChangedEventHandler Progress;

        private static readonly string SettingsFilePath;
        public static Settings Settings;



        static Processor()
        {
            #region  |  initialize and serialize the settings for the calling client  |


            SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SdlXliffCompare");

            if (!Directory.Exists(SettingsFilePath))
                Directory.CreateDirectory(SettingsFilePath);

            SettingsFilePath = Path.Combine(SettingsFilePath, "SdlxliffCompare2.settings.xml");

            ReadSettings(SettingsFilePath);

            #endregion
        }


        #region  |  process files  |

        public void ProcessFiles(bool compareDirectoryOfFiles)
        {
            ComparisonTotalFiles = 0;
            ComparisonTotalSegments = 0;
            ComparisonTotalContentChanges = 0;
            ComparisonTotalStatusChanges = 0;
            ComparisonTotalComments = 0;


            #region  |  get list of paired processing files  |

            var pairedFiles = new List<PairedFiles.PairedFile>();
            switch (compareDirectoryOfFiles)
            {
                case false: pairedFiles = new PairedFiles(Settings.FilePathOriginal, Settings.FilePathUpdated).PairedProcessingFiles; break;
                case true: pairedFiles = new PairedFiles(Settings.DirectoryPathOriginal, Settings.DirectoryPathUpdated, new[] { "*.sdlxliff" }, Settings.SearchSubFolders).PairedProcessingFiles; break;
            }

            #endregion

               

            var fileComparisonParagraphUnits = new Dictionary<Comparer.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.Comparer.ComparisonParagraphUnit>>>();

            var comparer = new Comparer.Comparer();
            try
            {
                comparer.Progress += comparer_Progress;
                var sdlXliffParser = new Parser();
                try
                {
                    sdlXliffParser.Progress += parser_Progress;
                    foreach(var pairedFile in pairedFiles)                        
                    {
                        if (!pairedFile.IsError)
                        {
                            try
                            {
                                #region  |  process paired file  |

                                sdlXliffParser.GetParagraphUnits(pairedFile.OriginalFilePath.FullName, pairedFile.UpdatedFilePath.FullName);

                                var comparisonParagraphUnits =
                                    comparer.GetComparisonParagraphUnits(sdlXliffParser.FileParagraphUnitsOriginal, sdlXliffParser.FileParagraphUnitsUpdated);

                                fileComparisonParagraphUnits.Add(new Comparer.Comparer.FileUnitProperties(pairedFile.OriginalFilePath.FullName, pairedFile.UpdatedFilePath.FullName
                                    , sdlXliffParser.GetSegmentCount(sdlXliffParser.FileParagraphUnitsOriginal), sdlXliffParser.GetSegmentCount(sdlXliffParser.FileParagraphUnitsUpdated)
                                    , comparisonParagraphUnits), comparisonParagraphUnits);

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("File: " + pairedFile.UpdatedFilePath.FullName + "; \r\n" + ex.Message);
                            }
                        }
                        else
                        {
                            fileComparisonParagraphUnits.Add(new Comparer.Comparer.FileUnitProperties(pairedFile.OriginalFilePath.FullName, "", 0, 0, null), null);
                        }
                    }
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

         
            #region  |  create report  |

            var reportFileName = Settings.ReportFileName;

            if (!reportFileName.ToLower().EndsWith(".xml"))
                reportFileName += ".xml";

            var reportFilePath = Path.Combine(Settings.ReportDirectory, reportFileName);

            SetTotalCounters(fileComparisonParagraphUnits);
            CreateReport(reportFilePath, fileComparisonParagraphUnits);

            #endregion
        }




        public int ComparisonTotalFiles { get; private set; }
        public int ComparisonTotalSegments { get; private set; }
        public int ComparisonTotalContentChanges { get; private set; }
        public int ComparisonTotalStatusChanges { get; private set; }
        public int ComparisonTotalComments { get; private set; }

        private void SetTotalCounters(Dictionary<Comparer.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnits)
        { 
            foreach (var fileComparisonFileParagraphUnit in fileComparisonFileParagraphUnits)
            {
                var fileUnitProperties = fileComparisonFileParagraphUnit.Key;

                ComparisonTotalFiles++;
                ComparisonTotalSegments += fileUnitProperties.TotalSegments;
                ComparisonTotalContentChanges += fileUnitProperties.TotalContentChanges;
                ComparisonTotalStatusChanges += fileUnitProperties.TotalStatusChanges;
                ComparisonTotalComments += fileUnitProperties.TotalComments;                
            }
        }


        #endregion

        #region |  create report  |


        private static void CreateReport(string reportFilePath,
             Dictionary<Comparer.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.Comparer.ComparisonParagraphUnit>>> fileComparisonParagraphUnits)
        {
            var report = new Reports.Report();

            var transformXmlReport = Settings.ReportingFormat == Settings.ReportFormat.Html;

            report.CreateXmlReport(reportFilePath, fileComparisonParagraphUnits, transformXmlReport);


            if (!Settings.ViewReportWhenFinishedProcessing) 
                return;
            if (transformXmlReport)
                reportFilePath += ".html";

            System.Diagnostics.Process.Start(reportFilePath);
        }

        #endregion

        #region  |  progress callbacks  |

        private void comparer_Progress(int maximum, int current, int percent, string message)
        {
            if (Progress!=null)
                Progress(maximum, current, percent, message);
        }
        private void parser_Progress(int maximum, int current, int percent, string message)
        {
            if (Progress != null)
                Progress(maximum, current, percent, message);
        }

        #endregion

        #region  |  settings serialization  |

        private static void BeforeSaveSettings()
        {        
            if (Settings.FilePathCustomStyleSheet.Trim() != string.Empty && !File.Exists(Settings.FilePathCustomStyleSheet))            
                Settings.UseCustomStyleSheet = false;               
            
        }
        private static void AfterReadSettings()
        {
            //assign the reportfileName with todays date
            Settings.ReportFileName = "SdlXliff Compare Report "
                + DateTime.Now.Year.ToString()
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
        private static void ReadSettings(String filename)
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
