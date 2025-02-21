using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Sdl.Community.XliffReadWrite.SDLXLIFF;

namespace Sdl.Community.XliffReadWrite
{
    public class Processor
    {
        public delegate void ChangedEventHandler(int maximum, int current, int percent, string message);
        public event ChangedEventHandler Progress;

        private static readonly string SettingsFilePath;
        public static Settings ProcessorSettings;

        public CultureInfo SourceLanguageCultureInfo { get; set; }
        public CultureInfo TargetLanguageCultureInfo { get; set; }

        public int TotalSegmentsOriginalFile;
        public int TotalSegmentsBilingualFile;
        public int TotalContentChanges;
        public int TotalStatusChangesWithContentChanges;
        public int TotalStatusChangesWithoutContentChanges;

        public Dictionary<string, List<TagUnitWarning>> TagUnitWarnings { get; set; }

        static Processor()
        {          
            SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SdlXliffReadWrite");

            if (!Directory.Exists(SettingsFilePath))
                Directory.CreateDirectory(SettingsFilePath);

            SettingsFilePath = Path.Combine(SettingsFilePath, "SdlXliffReadWrite.settings.xml");

            ReadSettings(SettingsFilePath);
        
        }

       

        public Dictionary<string, Dictionary<string, ParagraphUnit>> ReadFileParagraphUnits()
        {
            Dictionary<string, Dictionary<string, ParagraphUnit>> paragraphUnitsOriginal;
            var parser = new Parser();
            try
            {
                parser.Progress += parser_Progress;

                paragraphUnitsOriginal = parser.GetParagraphUnits(ProcessorSettings.FilePathOriginal);
                paragraphUnitsOriginal = PrepareParagraphUnits(paragraphUnitsOriginal);


                SourceLanguageCultureInfo = parser.SourceLanguageCultureInfo;
                TargetLanguageCultureInfo = parser.TargetLanguageCultureInfo;
            }
            finally
            {
                parser.Progress -= parser_Progress;
            }
            return paragraphUnitsOriginal;

        }
        private static Dictionary<string, Dictionary<string, ParagraphUnit>> PrepareParagraphUnits(Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnitsOriginal)
        {
            foreach (var fileParagraphUnitOriginal in fileParagraphUnitsOriginal)
            {
                foreach (var paragraphUnit in fileParagraphUnitOriginal.Value.Values)
                {
                    foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    {
                        var sourceText = string.Empty;
                        var targetText = string.Empty;

                        sourceText = segmentPair.SourceSections.Aggregate(sourceText, (current, segmentSection) => 
                            current + (segmentSection.Type == SegmentSection.ContentType.Text 
                                ? segmentSection.Content 
                                : Parser.GetContentTypeToMarkup(segmentSection.Type, segmentSection.Content, segmentSection.SectionId)));
                        segmentPair.Source = sourceText;

                        targetText = segmentPair.TargetSections.Aggregate(targetText, (current, segmentSection) => 
                            current + (segmentSection.Type == SegmentSection.ContentType.Text 
                                ? segmentSection.Content 
                                : Parser.GetContentTypeToMarkup(segmentSection.Type, segmentSection.Content, segmentSection.SectionId)));
                        segmentPair.Target = targetText;        
                    }
                }
            }
            return fileParagraphUnitsOriginal;
        }


      
     
        public void WriteFileParagraphUnits(Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnitsUpdated)
        {
            TotalSegmentsOriginalFile = 0;
            TotalSegmentsBilingualFile = 0;
            TotalContentChanges = 0;
            TotalStatusChangesWithContentChanges = 0;
            TotalStatusChangesWithoutContentChanges = 0;

            TagUnitWarnings = new Dictionary<string, List<TagUnitWarning>>();

            var sdlXliffParser = new Parser();
            try
            {
                sdlXliffParser.Progress += parser_Progress;

                fileParagraphUnitsUpdated = PrepareFromTableStructure(fileParagraphUnitsUpdated);
                sdlXliffParser.UpdateSegmentUnits(ProcessorSettings.FilePathOriginal, ProcessorSettings.FilePathUpdated, fileParagraphUnitsUpdated);

                SourceLanguageCultureInfo = sdlXliffParser.SourceLanguageCultureInfo;
                TargetLanguageCultureInfo = sdlXliffParser.TargetLanguageCultureInfo;

                TotalSegmentsOriginalFile = sdlXliffParser.TotalSegmentsOriginalFile;
                TotalSegmentsBilingualFile = sdlXliffParser.TotalSegmentsBilingualFile;
                TotalContentChanges = sdlXliffParser.TotalContentChanges;
                TotalStatusChangesWithContentChanges = sdlXliffParser.TotalStatusChangesWithContentChanges;
                TotalStatusChangesWithoutContentChanges = sdlXliffParser.TotalStatusChangesWithoutContentChanges;

                TagUnitWarnings = sdlXliffParser.TagUnitWarnings;
            }
            finally
            {
                sdlXliffParser.Progress -= parser_Progress;
            }
        }
        private Dictionary<string, Dictionary<string, ParagraphUnit>> PrepareFromTableStructure(Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits)
        {
            foreach (var fileParagraphUnit in fileParagraphUnits)
            {
                foreach (var paragraphUnit in fileParagraphUnit.Value.Values)
                {
                    foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    {    
                        segmentPair.SourceSections = Parser.GetSegmentSections(segmentPair.Source);
                        segmentPair.TargetSections = Parser.GetSegmentSections(segmentPair.Target);

                        segmentPair.Source = segmentPair.SourceSections.Aggregate("", (current, segmentSection) => current + segmentSection.Content);
                        segmentPair.Target = segmentPair.TargetSections.Aggregate("", (current, segmentSection) => current + segmentSection.Content);
                    }
                }
            }
            return fileParagraphUnits;
        }


  
        public static void SaveSettings()
        {
            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(SettingsFilePath, FileMode.Create, FileAccess.Write);
                serializer.Serialize(stream, ProcessorSettings);
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
                ProcessorSettings = new Settings();
                SaveSettings();
            }

            FileStream stream = null;
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                stream = new FileStream(filename, FileMode.Open);
                ProcessorSettings = (Settings)serializer.Deserialize(stream) ?? new Settings();
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
        }

       

        private void parser_Progress(int maximum, int current, int percent, string message)
        {
            if (Progress != null)
                Progress(maximum, current, percent, message);
        }      
    }
}
