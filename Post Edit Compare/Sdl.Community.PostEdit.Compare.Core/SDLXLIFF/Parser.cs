using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    public class Parser
    {

        public delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        public event ChangedEventHandler Progress;


        public Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnitsOriginal { get; set; }
        public Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnitsUpdated { get; set; }


        internal Dictionary<string, ParagraphUnit> ParagraphUnitsOriginal { get; set; }



        public string SourceLanguageIdOriginal { get; set; }
        public string TargetLanguageIdOriginal { get; set; }

        public string SourceLanguageIdUpdated { get; set; }
        public string TargetLanguageIdUpdated { get; set; }





        internal CultureInfo SourceLanguageCultureInfo { get; set; }
        internal CultureInfo TargetLanguageCultureInfo { get; set; }

        internal int TotalSegmentsOriginalFile;
        internal int TotalSegmentsBilingualFile;
        internal int TotalContentChanges;
        internal int TotalStatusChangesWithContentChanges;
        internal int TotalStatusChangesWithoutContentChanges;

        internal Dictionary<string, List<TagUnitWarning>> TagUnitWarnings { get; set; }

        public Parser()
        {
           
            //initialize the paragraph units
            FileParagraphUnitsOriginal = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
            FileParagraphUnitsUpdated = new Dictionary<string, Dictionary<string, ParagraphUnit>>();


            //initialize the paragraph units
            ParagraphUnitsOriginal = new Dictionary<string, ParagraphUnit>();
        }

        public int GetSegmentCount(Dictionary<string,Dictionary<string, ParagraphUnit>> xFileParagraphUnits)
        {
            return xFileParagraphUnits.SelectMany(xFileParagraphUnit => xFileParagraphUnit.Value).Sum(xParagraphUnit => xParagraphUnit.Value.SegmentPairs.Count);
        }


        public void GetParagraphUnits(string filePathOriginal, string filePathUpdated)
        {
            SourceLanguageIdOriginal = string.Empty;
            TargetLanguageIdOriginal = string.Empty;

            SourceLanguageIdUpdated = string.Empty;
            TargetLanguageIdUpdated = string.Empty;

            FileParagraphUnitsOriginal = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
            FileParagraphUnitsUpdated = new Dictionary<string, Dictionary<string, ParagraphUnit>>();


            FileParagraphUnitsOriginal = GetFileParagraphUnits(filePathOriginal,true);

            FileParagraphUnitsUpdated = GetFileParagraphUnits(filePathUpdated,false);
        }

        private Dictionary<string, Dictionary<string, ParagraphUnit>> GetFileParagraphUnits(string filePath, bool isOriginal)
        {
            var manager = DefaultFileTypeManager.CreateInstance(true);
            var converter = manager.GetConverterToDefaultBilingual(filePath, null, null);

            var contentProcessor = new ContentProcessor
            {
                FileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>(),
                OutputPath = Path.GetDirectoryName(filePath),
                DummyOutputFiles = new List<string>(),
                CreatedDummyOutput = false,
                IncludeTagText = Processor.Settings.ComparisonIncludeTags
            };


            contentProcessor.FileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
           
            converter.AddBilingualProcessor(contentProcessor);
            try
            {
                converter.Progress += converter_Progress;
                converter.Parse();

                if (isOriginal)
                {
                    if (contentProcessor.SourceLanguageId != string.Empty)
                        SourceLanguageIdOriginal = contentProcessor.SourceLanguageId;
                    else if (converter.DetectedSourceLanguage != null)
                        SourceLanguageIdOriginal = converter.DetectedSourceLanguage.First.CultureInfo.Name;

                    if (contentProcessor.TargetLanguageId != string.Empty)
                        TargetLanguageIdOriginal = contentProcessor.TargetLanguageId;
                    else if (converter.DetectedTargetLanguage != null && converter.DetectedTargetLanguage.First.CultureInfo != null)
                        TargetLanguageIdOriginal = converter.DetectedTargetLanguage.First.CultureInfo.Name;
                }
                else
                {
                    if (contentProcessor.SourceLanguageId != string.Empty)
                        SourceLanguageIdUpdated = contentProcessor.SourceLanguageId;
                    else if (converter.DetectedSourceLanguage != null)
                        SourceLanguageIdUpdated = converter.DetectedSourceLanguage.First.CultureInfo.Name;

                    if (contentProcessor.TargetLanguageId != string.Empty)
                        TargetLanguageIdUpdated = contentProcessor.TargetLanguageId;
                    else if (converter.DetectedTargetLanguage != null && converter.DetectedTargetLanguage.First.CultureInfo != null)
                        TargetLanguageIdUpdated = converter.DetectedTargetLanguage.First.CultureInfo.Name;
                }

                SourceLanguageCultureInfo = new CultureInfo(SourceLanguageIdOriginal);
                TargetLanguageCultureInfo = new CultureInfo(TargetLanguageIdUpdated);
            }
            finally
            {
                converter.Progress -= converter_Progress;
            }

            return contentProcessor.FileParagraphUnits;



        }
        internal void UpdateSegmentUnits(string filePathIn, string filePathOut, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits)
        {

            TotalSegmentsOriginalFile = 0;
            TotalSegmentsBilingualFile = 0;
            TotalContentChanges = 0;
            TotalStatusChangesWithContentChanges = 0;
            TotalStatusChangesWithoutContentChanges = 0;
            TagUnitWarnings = new Dictionary<string, List<TagUnitWarning>>();

            var manager = DefaultFileTypeManager.CreateInstance(true);
            var converter = manager.GetConverterToDefaultBilingual(filePathIn, filePathOut, null);



            var contentWriter = new ContentWriter
            {
                OutputPath = Path.GetDirectoryName(filePathOut),
                DummyOutputFiles = new List<string>(),
                CreatedDummyOutput = false
            };


            var xParagraphUnits = fileParagraphUnits.SelectMany(fileParagraphUnit => fileParagraphUnit.Value).ToDictionary(paragraphUnit => paragraphUnit.Key, paragraphUnit => paragraphUnit.Value);
         
            contentWriter.XParagraphUnits = xParagraphUnits;
            converter.AddBilingualProcessor(contentWriter);


            try
            {
                converter.Progress += converter_Progress;
                converter.Parse();

                SourceLanguageCultureInfo = contentWriter.SourceLanguageCultureInfo;
                TargetLanguageCultureInfo = contentWriter.TargetLanguageCultureInfo;


                TotalSegmentsOriginalFile = contentWriter.TotalSegmentsOriginalFile;
                TotalSegmentsBilingualFile = contentWriter.TotalSegmentsBilingualFile;
                TotalContentChanges = contentWriter.TotalContentChanges;
                TotalStatusChangesWithContentChanges = contentWriter.TotalStatusChangesWithContentChanges;
                TotalStatusChangesWithoutContentChanges = contentWriter.TotalStatusChangesWithoutContentChanges;

                TagUnitWarnings = contentWriter.TagUnitWarnings;
            }
            finally
            {
                converter.Progress -= converter_Progress;

                if (contentWriter.CreatedDummyOutput && contentWriter.DummyOutputFiles.Count > 0)
                {
                    try
                    {
                        foreach (var dummyFilePath in contentWriter.DummyOutputFiles)
                        {
                            File.Delete(dummyFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        internal static string GetContentTypeToMarkup(SegmentSection.ContentType contentType, string contentText, string contentId)
        {
            var rs = "";

            switch (contentType)
            {
                case SegmentSection.ContentType.Text:
                    {
                        //ignore
                    } break;
                case SegmentSection.ContentType.Tag:
                    {
                        rs = "<xProtected_ type=\"" + contentType.ToString() + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.TagClosing:
                    {
                        rs = "<xProtected_ type=\"" + contentType.ToString() + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.Placeholder:
                    {
                        rs = "<xProtected_ type=\"" + contentType.ToString() + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.LockedContent:
                    {
                        rs = "<xProtected_ type=\"" + contentType.ToString() + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
            }

            return rs;
        }
        internal static List<SegmentSection> GetSegmentSections(string text)
        {
            var xSegmentSections = new List<SegmentSection>();

            var regexProtected = new Regex(@"\<xProtected_\s+type=""(?<xType>[^""]*)""\s+id=""(?<xId>[^""]*)""\s*\>(?<xContentText>.*?)\<\/xProtected_\>"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);


            var mc = regexProtected.Matches(text);


            var icurrent = 0;

            foreach (Match m in mc)
            {
                var indexStart = m.Index;
                var indexEnd = m.Index + m.Length;

                if (icurrent < indexStart)
                {
                    var _text = text.Substring(icurrent, (indexStart - icurrent));
                    xSegmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", _text));
                }

                var type = (SegmentSection.ContentType)Enum.Parse(typeof(SegmentSection.ContentType), m.Groups["xType"].Value, true);
                var id = m.Groups["xId"].Value;
                var content = m.Groups["xContentText"].Value;
                xSegmentSections.Add(new SegmentSection(type, id, content));

                icurrent = indexEnd;
            }

            if (icurrent >= text.Length) return xSegmentSections;
            {
                var _text = text.Substring(icurrent);
                xSegmentSections.Add(new SegmentSection(SDLXLIFF.SegmentSection.ContentType.Text, "", _text));
            }

            return xSegmentSections;

        }

        internal static string GetStartTagName(string text, ref string refID)
        {
            var sTagName = "";

            Regex regex_tagName = new Regex(@"\<(?<xName>[^\s""\>]*)"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Regex regex_tagID = new Regex(@"\<[^\s""]*\s+(?<xAttName>[^\s""]+)\=""(?<xID>[^""]*)"""
               , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Match m = regex_tagName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            m = regex_tagID.Match(text);
            if (m.Success)
            {
                string id = m.Groups["xID"].Value;
                string attName = m.Groups["xAttName"].Value;

                if (string.Compare(attName, "id", true) == 0)
                {
                    refID = id;
                }
            }


            return sTagName;
        }
        internal static string GetEndTagName(string text)
        {
            string sTagName = "";

            Regex regex_tagName = new Regex(@"\<\s*\/\s*(?<xName>[^\s\>]*)"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            Match m = regex_tagName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            return sTagName;
        }


        internal static string GetTextOnly(List<SegmentSection> sections)
        {
            string textOnly = string.Empty;

            foreach (SegmentSection section in sections)
            {
                if (section.Type == SegmentSection.ContentType.Text)
                    textOnly += section.Content;
            }
            return textOnly;
        }
        internal static string GetSectionsToText(List<SegmentSection> sections)
        {
            string textOnly = string.Empty;

            foreach (SegmentSection section in sections)
            {
                textOnly += section.Content;
            }
            return textOnly;
        }










        private void converter_Progress(object sender, BatchProgressEventArgs e)
        {
            if (Progress != null)
            {
                Progress(100, e.FilePercentComplete, e.FilePercentComplete, Path.GetFileName(e.FilePath));
            }
        }
    }
}
