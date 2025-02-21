using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
//using Sdl.FileTypeSupport.Framework.Integration;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    internal class Parser
    {

        internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        internal event ChangedEventHandler Progress;


        internal Dictionary<string, ParagraphUnit> ParagraphUnitsOriginal { get; set; }


        internal CultureInfo SourceLanguageCultureInfo { get; set; }
        internal CultureInfo TargetLanguageCultureInfo { get; set; }

        internal int TotalSegmentsOriginalFile;
        internal int TotalSegmentsBilingualFile;
        internal int TotalContentChanges;
        internal int TotalStatusChangesWithContentChanges;
        internal int TotalStatusChangesWithoutContentChanges;

        internal Dictionary<string, List<TagUnitWarning>> TagUnitWarnings { get; set; }


        //private readonly PocoFilterManager _pocoFilterManager;
       

        internal Parser()
        {
           // _pocoFilterManager = new PocoFilterManager(true);            
            ParagraphUnitsOriginal = new Dictionary<string, ParagraphUnit>();
           
            
        }

        internal int GetSegmentCount(Dictionary<string, ParagraphUnit> xParagraphUnits)
        {
            return xParagraphUnits.Values.Sum(paragraphUnit => paragraphUnit.SegmentPairs.Count);
        }


        internal Dictionary<string, Dictionary<string, ParagraphUnit>> GetParagraphUnits(string filePath)
        {    
            var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
            var converter = fileTypeManager.GetConverterToDefaultBilingual(filePath, null, null);

            var contentProcessor = new ContentProcessor
            {
                OutputPath = Path.GetDirectoryName(filePath),
                DummyOutputFiles = new List<string>(),
                CreatedDummyOutput = false,
                FileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>()
            };



            converter.AddBilingualProcessor(new SourceToTargetCopier(ExistingContentHandling.Preserve));

            converter.AddBilingualProcessor(contentProcessor);
            try
            {
                converter.Progress += converter_Progress;
                converter.Parse();

                SourceLanguageCultureInfo = contentProcessor.SourceLanguageCultureInfo;
                TargetLanguageCultureInfo = contentProcessor.TargetLanguageCultureInfo;              
            }
            finally
            {
                converter.Progress -= converter_Progress;

                if (contentProcessor.CreatedDummyOutput && contentProcessor.DummyOutputFiles.Count > 0)
                {
                    try
                    {
                        foreach (var dummyFilePath in contentProcessor.DummyOutputFiles)
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

            return contentProcessor.FileParagraphUnits;



        }
        internal void UpdateSegmentUnits(string filePathIn, string filePathOut, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits)
        {
            var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
            TotalSegmentsOriginalFile = 0;
            TotalSegmentsBilingualFile = 0;
            TotalContentChanges = 0;
            TotalStatusChangesWithContentChanges = 0;
            TotalStatusChangesWithoutContentChanges = 0;
            TagUnitWarnings = new Dictionary<string, List<TagUnitWarning>>();

            var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathIn, filePathOut, null);



            var contentWriter = new ContentWriter
            {
                OutputPath = Path.GetDirectoryName(filePathOut),
                DummyOutputFiles = new List<string>(),
                CreatedDummyOutput = false
            };


            var paragraphUnits = fileParagraphUnits.SelectMany(fileParagraphUnit => fileParagraphUnit.Value).ToDictionary(paragraphUnit => paragraphUnit.Key, paragraphUnit => paragraphUnit.Value);

            //make the fileParagraphUnit structure compatible with the writer unit structure, as it does not require the addition sub file info
            //to update the sdlxliff file

            contentWriter.XParagraphUnits = paragraphUnits;
          
      
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
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.TagClosing:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.Placeholder:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.LockedContent:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                default:
                    throw new ArgumentOutOfRangeException("contentType", contentType, null);
            }

            return rs;
        }
        internal static List<SegmentSection> GetSegmentSections(string text)
        {
            var segmentSections = new List<SegmentSection>();

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
                    var substring = text.Substring(icurrent, (indexStart - icurrent));
                    segmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", substring));
                }

                var type = (SegmentSection.ContentType)Enum.Parse(typeof(SegmentSection.ContentType), m.Groups["xType"].Value, true);
                var id = m.Groups["xId"].Value;
                var content = m.Groups["xContentText"].Value;
                segmentSections.Add(new SegmentSection(type, id, content));

                icurrent = indexEnd;
            }

            if (icurrent >= text.Length) return segmentSections;
            {
                var substring = text.Substring(icurrent);
                segmentSections.Add(new SegmentSection(SegmentSection.ContentType.Text, "", substring));
            }

            return segmentSections;
    
        }



        internal static string GetStartTagName(string text, ref string refId)
        {
            var sTagName = "";

            var regexTagName = new Regex(@"\<(?<xName>[^\s""\>]*)"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var regexTagId = new Regex(@"\<[^\s""]*\s+(?<xAttName>[^\s""]+)\=""(?<xID>[^""]*)"""
               , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var m = regexTagName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            m = regexTagId.Match(text);
            if (!m.Success) 
                return sTagName;

            var id = m.Groups["xID"].Value;
            var attName = m.Groups["xAttName"].Value;

            if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
            {
                refId = id;
            }


            return sTagName;
        }
        internal static string GetEndTagName(string text)
        {
            var sTagName = "";

            var regexTagName = new Regex(@"^\s*\<\s*\/\s*(?<xName>[^\s\>]*)"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var m = regexTagName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            return sTagName;
        }


        internal static string GetTextOnly(List<SegmentSection> sections)
        {
            return sections.Where(section => section.Type == SegmentSection.ContentType.Text).Aggregate(string.Empty, (current, section) => current + section.Content);
        }
        internal static string GetSectionsToText(List<SegmentSection> sections)
        {
            return sections.Aggregate(string.Empty, (current, section) => current + section.Content);
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
