using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.XliffReadWrite.SDLXLIFF;

namespace Sdl.Community.XliffToLegacyConverter.Core.RTF
{
    internal class Processor
    {

        internal delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        internal event ChangedEventHandler Progress;

        internal void OnProgress(int maximum, int current, string message)
        {
            if (Progress == null)
                return;
            try
            {
                var percent = Convert.ToInt32(current <= maximum && maximum > 0 ? Convert.ToDecimal(current) / Convert.ToDecimal(maximum) * Convert.ToDecimal(100) : maximum);

                Progress(maximum, current, percent, message);
            }
            catch
            {
                // ignored
            }
        }


        private class FileAttributes
        {
            public string FileSource { get; set; }
            public string FileTarget { get; set; }
            public string FileSegmentCount { get; set; }
            public string FilePath { get; set; }

            public FileAttributes()
            {
                FileSource = string.Empty;
                FileTarget = string.Empty;
                FileSegmentCount = string.Empty;
                FilePath = string.Empty;
            }
        }

        private class StringColorType
        {
            public string Black { get; set; }
            public string Gray { get; set; }
            public string Red { get; set; }
            public string Misc { get; set; }
            public string Purple { get; set; }

            public StringColorType()
            {
                Black = string.Empty;
                Gray = string.Empty;
                Red = string.Empty;
                Misc = string.Empty;
                Purple = string.Empty;
            }
        }

        internal Dictionary<string, ParagraphUnit> ReadRtf(string rtfFile)
        {
            var rtb = new RichTextBox
            {
                Enabled = false,
                LanguageOption = RichTextBoxLanguageOptions.AutoFont,
                ReadOnly = true,
                Visible = false,
                WordWrap = false,
                ShowSelectionMargin = false
            };
            rtb.SendToBack();
            rtb.SuspendLayout();
            rtb.LoadFile(rtfFile, RichTextBoxStreamType.RichText);
            rtb.Rtf = rtb.Rtf.Replace("\\v\\", "\\v0\\");
            rtb.AppendText("\n");



            var indexProcessingCurrent = 0;
            OnProgress(1000, indexProcessingCurrent, "Initialzing...");


            var paragraphUnits = new Dictionary<string, ParagraphUnit>();

            var file = rtfFile;
            var fileAttributes = new FileAttributes();

            var iTagId = 10000;
            var innerFileName = string.Empty;


            var regexFile = new Regex(@"\<file source\=""(?<x1>[^""]*)""\s+target\=""(?<x2>[^""]*)""\s+segmentCount\=""(?<x3>[^""]*)""\s+path\=""(?<x4>[^""]*)""[^\>]*\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regexSeg = new Regex(@"\<seg id\=""(?<x1>[^""]*)""\s+pid\=""(?<x2>[^""]*)""\s+status\=""(?<x3>[^""]*)""\s+locked\=""(?<x4>[^""]*)""\s+match\=""(?<x5>[^""]*)""[^\>]*\>" + "\n" + @"(?<x6>.*?)" + "\n" + @"\<\/seg\>" + "\n", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regexSource = new Regex(@"\{0\>(?<xSource>.*?)\<\}(?<xPercentage>[^\{]*)\{\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);


            var matchFile = regexFile.Match(rtb.Text);
            if (matchFile.Success)
            {
                fileAttributes.FileSource = matchFile.Groups["x1"].Value;
                fileAttributes.FileTarget = matchFile.Groups["x2"].Value;
                fileAttributes.FileSegmentCount = matchFile.Groups["x3"].Value;
                fileAttributes.FilePath = matchFile.Groups["x4"].Value;
            }

            var matchesSeg = regexSeg.Matches(rtb.Text);

            foreach (Match matchSeg in matchesSeg)
            {
                #region  |  init   |

                var isSource = false;

                var colorFormat = new StringColorType();

                var sourceSections = new List<SegmentSection>();
                var targetSections = new List<SegmentSection>();
                var segmentPairs = new List<SegmentPair>();
                var comments = new List<Comment>();

                #endregion

                var segmentPair = new SegmentPair
                {
                    Id = matchSeg.Groups["x1"].Value,
                    ParagraphId = matchSeg.Groups["x2"].Value,
                    SegmentStatus = Core.Processor.GetSegmentStatusFromVisual(matchSeg.Groups["x3"].Value),
                    IsLocked = Convert.ToBoolean(matchSeg.Groups["x4"].Value),
                    MatchTypeId = matchSeg.Groups["x5"].Value,
                    Comments = comments
                };

            
                var importTranslation = ImportTranslation(segmentPair);
                if (!importTranslation)
                    continue;

                var content = matchSeg.Groups["x6"].Value;

                var paragraphUnit = new ParagraphUnit(segmentPair.ParagraphId, segmentPairs, innerFileName);

                var startIndex = matchSeg.Groups["x6"].Index;
                var endIndex = startIndex + content.Length;


                segmentPair.TranslationOrigin = new TranslationOrigin(); ;


                var matchSource = regexSource.Match(content);
                if (matchSource.Success)
                {

                    var text = matchSource.Groups["xSource"].Value;
                    var percentage = matchSource.Groups["xPercentage"].Value;

                    if (colorFormat.Black != string.Empty)
                        sourceSections.Add(new SegmentSection(SegmentSection.ContentType.Text, string.Empty, text));


                    segmentPair.Source = Core.Processor.GetSectionsToText(sourceSections);
                    segmentPair.SourceSections = sourceSections;

                    startIndex = startIndex + matchSource.Groups["xPercentage"].Index + percentage.Length + 2;

                    var isTarget = true;


                    for (var i = startIndex; i <= endIndex; i++)
                    {

                        rtb.SelectionStart = i;
                        rtb.SelectionLength = 1;
                        text = rtb.Text[i].ToString();

                        var color = rtb.SelectionColor;

                        if (color == Color.Black)
                        {                           
                            if (colorFormat.Red != string.Empty)
                            {
                                UpdateTagFormatting(colorFormat, isSource ? sourceSections : targetSections);

                                colorFormat.Red = string.Empty;
                                colorFormat.Black = string.Empty;
                                colorFormat.Purple = string.Empty;
                                colorFormat.Gray = string.Empty;
                                colorFormat.Black += text;
                            }
                            else
                            {
                                colorFormat.Black += text;
                            }                       
                        }
                        else if (color == Color.Red)
                        {                           
                            colorFormat.Red += text;
                        }
                        else if (color == Color.Purple)
                        {                         
                            if (colorFormat.Red != string.Empty)
                            {                              
                                UpdateTagFormatting(colorFormat, isSource ? sourceSections : targetSections);

                                colorFormat.Red = string.Empty;
                                colorFormat.Black = string.Empty;                            
                                colorFormat.Red = string.Empty;
                            }                          
                         
                            colorFormat.Purple += text;

                            if (!isTarget)
                                continue;

                            if (!colorFormat.Purple.Trim().EndsWith("<0}"))
                                continue;

                            if (colorFormat.Black != string.Empty)
                                targetSections.Add(new SegmentSection(SegmentSection.ContentType.Text, string.Empty, colorFormat.Black));

                            isSource = false;
                            isTarget = false;
                            colorFormat.Black = string.Empty;
                            colorFormat.Purple = string.Empty;
                            colorFormat.Red = string.Empty;
                            colorFormat.Gray = string.Empty;
                        }
                        else
                        {
                            //outside the scope of the markup; error
                            colorFormat.Misc += text;
                        }
                    }
                
                    if (colorFormat.Red != string.Empty)
                    {                       
                        UpdateTagFormatting(colorFormat, isSource ? sourceSections : targetSections);

                        colorFormat.Red = string.Empty;
                        colorFormat.Black = string.Empty;
                        colorFormat.Red = string.Empty;                       
                    }                  

                    isSource = false;
                    isTarget = false;
                    colorFormat.Black = string.Empty;
                    colorFormat.Purple = string.Empty;
                    colorFormat.Red = string.Empty;



                    MarkupTagContent(targetSections, ref iTagId);


                    segmentPair.Target = Core.Processor.GetSectionsToText(targetSections);
                    segmentPair.TargetSections = targetSections;

                    paragraphUnit.SegmentPairs.Add(segmentPair);

                    if (segmentPair.ParagraphId != string.Empty)
                    {
                        if (!paragraphUnits.ContainsKey(segmentPair.ParagraphId))
                            paragraphUnits.Add(segmentPair.ParagraphId, paragraphUnit);
                        else
                        {
                            paragraphUnits[segmentPair.ParagraphId].SegmentPairs.AddRange(paragraphUnit.SegmentPairs);
                        }
                    }
                }

                indexProcessingCurrent++;
                OnProgress(matchesSeg.Count, indexProcessingCurrent, string.Format("Processing segment id: {0}", segmentPair.Id));
            }


            return paragraphUnits;
        }

        private static void MarkupTagContent(IList<SegmentSection> targetSections, ref int iTagId)
        {
            var currentTagList = new List<TagUnit>();

            for (var ix = 0; ix < targetSections.Count; ix++)
            {
                var section = targetSections[ix];
                switch (section.Type)
                {
                    case SegmentSection.ContentType.Tag:
                    {
                        iTagId++;
                        var tagId = "pt" + iTagId;
                        var tagName = Core.Processor.GetStartTagName(section.Content, ref tagId);

                        currentTagList.Add(new TagUnit(tagId, tagName, section.Content, TagUnit.TagUnitState.IsOpening,
                            TagUnit.TagUnitType.IsTag));
                        section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content,
                            currentTagList[currentTagList.Count - 1].Id);
                    }
                        break;
                    case SegmentSection.ContentType.TagClosing:
                        var closingTagName = Core.Processor.GetEndTagName(section.Content);
                        if (currentTagList.Count > 0)
                        {
                            if (
                                string.Compare(currentTagList[currentTagList.Count - 1].Name, closingTagName,
                                    StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content,
                                    currentTagList[currentTagList.Count - 1].Id);
                                currentTagList.RemoveAt(currentTagList.Count - 1);
                            }
                            else
                            {
                                section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content,
                                    string.Empty);
                            }
                        }
                        else
                        {
                            section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, string.Empty);
                        }
                        break;
                    default:
                    {
                        iTagId++;
                        var tagId = "pt" + iTagId;
                        Core.Processor.GetStartTagName(section.Content, ref tagId);

                        section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, tagId);
                    }
                        break;
                }

                targetSections[ix] = section;
            }
        }

        private static bool ImportTranslation(SegmentPair segmentPair)
        {
            var importTranslation = true;
            if (segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportLocked)
                importTranslation = false;
            else if (!segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportUnLocked)
                importTranslation = false;
            else if (string.Compare(segmentPair.MatchTypeId, "Perfect Match", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportPerfectMatch)
                importTranslation = false;
            else if (string.Compare(segmentPair.MatchTypeId, "Context Match", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportContextMatch)
                importTranslation = false;
            else if (string.Compare(segmentPair.MatchTypeId, "Exact Match", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportExactMatch)
                importTranslation = false;
            else if (string.Compare(segmentPair.MatchTypeId, "Fuzzy Match", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportFuzzyMatch)
                importTranslation = false;
            else if (string.Compare(segmentPair.MatchTypeId, "No Match", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNoMatch)
                importTranslation = false;
            return importTranslation;
        }

        private static void UpdateTagFormatting(StringColorType stringColorType, ICollection<SegmentSection> sections)
        {
            if (stringColorType.Black != string.Empty)
            {
                sections.Add(new SegmentSection(SegmentSection.ContentType.Text, string.Empty, stringColorType.Black));
            }

            var tags = Core.Processor.SeperateTags(stringColorType.Red);
            foreach (var tag in tags)
            {
                switch (tag.Type)
                {
                    case TagUnit.TagUnitType.IsPlaceholder:
                    {
                        sections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.Id, tag.Content));
                        break;
                    }
                    case TagUnit.TagUnitType.IsTag:
                    {
                        switch (tag.State)
                        {
                            case TagUnit.TagUnitState.IsOpening:
                            {
                                sections.Add(new SegmentSection(SegmentSection.ContentType.Tag, tag.Id, tag.Content));
                                break;
                            }
                            case TagUnit.TagUnitState.IsClosing:
                            {
                                sections.Add(new SegmentSection(SegmentSection.ContentType.TagClosing, tag.Id, tag.Content));
                                break;
                            }
                            case TagUnit.TagUnitState.IsEmpty:
                            {
                                sections.Add(new SegmentSection(SegmentSection.ContentType.Placeholder, tag.Id, tag.Content));
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        internal int SegmentsExported;
        internal int SegmentsNotExported;

        internal void WriteRtf(string fileName, string sdlxliffFilePath,
            Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits,
            CultureInfo sourceCulture, CultureInfo targetCulture
            , bool includeLegacyStructure)
        {
            SegmentsExported = 0;
            SegmentsNotExported = 0;

            var segmentCount = (from fileParagraphUnit in fileParagraphUnits
                                from paragraphUnit in fileParagraphUnit.Value
                                from segmentPair in paragraphUnit.Value.SegmentPairs
                                select segmentPair).Count();

            using (var sw = new StreamWriter(fileName, false, Encoding.GetEncoding(1252)))
            {
                sw.Write(GetRtfDefaultStyleHeader(targetCulture.LCID.ToString()));
                sw.WriteLine(_tw4WinExternalTagNormal + "<file source=\"" + sourceCulture.Name + "\" target=\"" + targetCulture.Name + "\" segmentCount=\""
                    + segmentCount + "\"  path=\"" + ConvertToWordUni(ConvertToRtFstr(sdlxliffFilePath))
                    + "\" includeLegacyStructure=\"" + includeLegacyStructure + "\">}" + _tw4WinExternalTagPar);



                foreach (var fileParagraphUnit in fileParagraphUnits)
                {
                    sw.WriteLine(_tw4WinExternalTagNormal + "<innerFile name=\"" + ConvertToWordUni(ConvertToRtFstr(fileParagraphUnit.Key)) + "\">}" + _tw4WinExternalTagPar);


                    foreach (var paragraphUnit in fileParagraphUnit.Value)
                    {
                        foreach (var segmentPair in paragraphUnit.Value.SegmentPairs)
                        {

                            #region  |  get match type  |

                            var protectSegment = false;
                            var doNotExportSegment = false;

                            var match = string.Empty;
                            if (segmentPair.TranslationOrigin != null)
                            {
                                if (segmentPair.TranslationOrigin.MatchPercentage >= 100)
                                {
                                    if (string.Compare(segmentPair.TranslationOrigin.OriginType, "document-match", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        match = "Perfect Match";
                                        segmentPair.TranslationOrigin.MatchPercentage = 101;
                                    }
                                    else if (string.Compare(segmentPair.TranslationOrigin.TextContextMatchLevel, "SourceAndTarget", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        match = "Context Match";
                                        segmentPair.TranslationOrigin.MatchPercentage = 101;
                                    }
                                    else
                                    {
                                        match = "Exact Match";
                                    }
                                }
                                else if (segmentPair.TranslationOrigin.MatchPercentage > 0)
                                {
                                    match = "Fuzzy Match";
                                }
                                else
                                {
                                    match = "No Match";
                                }
                            }
                            #endregion

                            #region  |  assign filter setttings  |


                            switch (match)
                            {
                                case "Perfect Match":
                                    {
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportPerfectMatch;
                                        break;
                                    }
                                case "Context Match":
                                    {
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportContextMatch;
                                        break;
                                    }
                                case "Exact Match":
                                    {
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportExactMatch;
                                        break;
                                    }
                                case "Fuzzy Match":
                                    {
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportFuzzyMatch;
                                        break;
                                    }
                                case "No Match":
                                    {
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNoMatch;
                                        break;
                                    }
                            }



                            switch (segmentPair.SegmentStatus)
                            {
                                case "Unspecified":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportNotTranslated;
                                    } break;
                                case "Draft":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportDraft;
                                    } break;
                                case "Translated":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslated;
                                    } break;
                                case "RejectedTranslation":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationRejected;
                                    } break;
                                case "ApprovedTranslation":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationApproved;
                                    } break;
                                case "RejectedSignOff":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOffRejected;
                                    } break;
                                case "ApprovedSignOff":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOff;
                                    } break;
                            }



                            if (!doNotExportSegment)
                            {
                                if (segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportLocked)
                                    doNotExportSegment = true;
                                else if (!segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportUnLocked)
                                    doNotExportSegment = true;
                            }

                            if (!doNotExportSegment)
                            {
                                if (segmentPair.Target.Trim() == string.Empty && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations)
                                    doNotExportSegment = true;
                            }


                            #endregion

                            #region  |  create content rtf text  |

                            var sourceContent = string.Empty;
                            var targetContent = string.Empty;

                            if (protectSegment)
                            {
                                foreach (var segmentSection in segmentPair.SourceSections)
                                {
                                    switch (segmentSection.Type)
                                    {
                                        case SegmentSection.ContentType.Text:
                                            sourceContent += ConvertToWordUni(ConvertToRtFstr(segmentSection.Content));
                                            break;
                                        case SegmentSection.ContentType.LockedContent:
                                            sourceContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr("<ILockedContent>" + segmentSection.Content + "</ILockedContent>")).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                        default:
                                            sourceContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr(segmentSection.Content)).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                    }
                                }

                                foreach (var segmentSection in segmentPair.TargetSections)
                                {
                                    switch (segmentSection.Type)
                                    {
                                        case SegmentSection.ContentType.Text:
                                            targetContent += ConvertToWordUni(ConvertToRtFstr(segmentSection.Content));
                                            break;
                                        case SegmentSection.ContentType.LockedContent:
                                            targetContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr("<ILockedContent>" + segmentSection.Content + "</ILockedContent>")).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                        default:
                                            targetContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr(segmentSection.Content)).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var segmentSection in segmentPair.SourceSections)
                                {
                                    switch (segmentSection.Type)
                                    {
                                        case SegmentSection.ContentType.Text:
                                            sourceContent += ConvertToWordUni(ConvertToRtFstr(segmentSection.Content));
                                            break;
                                        case SegmentSection.ContentType.LockedContent:
                                            sourceContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr("<ILockedContent>" + segmentSection.Content + "</ILockedContent>")).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                        default:
                                            sourceContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr(segmentSection.Content)).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                    }
                                }


                                foreach (var segmentSection in segmentPair.TargetSections)
                                {
                                    switch (segmentSection.Type)
                                    {
                                        case SegmentSection.ContentType.Text:
                                            targetContent += ConvertToWordUni(ConvertToRtFstr(segmentSection.Content));
                                            break;
                                        case SegmentSection.ContentType.LockedContent:
                                            targetContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr("<ILockedContent>" + segmentSection.Content + "</ILockedContent>")).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                        default:
                                            targetContent += _tw4WinInternalTagNormal + ConvertToWordUni(ConvertToRtFstr(segmentSection.Content)).Replace("\r\n", @"{\line }") + "}";
                                            break;
                                    }
                                }
                            }
                            #endregion

                            #region  |  write segment  |

                            if (segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportLocked)
                                doNotExportSegment = true;
                            else if (!segmentPair.IsLocked && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportUnLocked)
                                doNotExportSegment = true;


                            if (includeLegacyStructure)
                            {
                                #region  |  includeLegacyStructure  |
                                if (!doNotExportSegment)
                                {
                                    SegmentsExported++;

                                    if (segmentPair.TranslationOrigin != null)
                                        sw.WriteLine(string.Empty

                                                     + _tw4WinExternalTagNormal + "<seg id=\"" + segmentPair.Id + "\" pid=\"" + paragraphUnit.Key + "\" status=\""
                                                     + Core.Processor.GetVisualSegmentStatus(segmentPair.SegmentStatus) + "\" locked=\"" + segmentPair.IsLocked
                                                     + "\" match=\"" + match + "\">" + "}\r\n" + _tw4WinExternalTagPar
                                                     + _tw4WinMarkTagHidden + "\\{0>" + "}"
                                                     + _tw4WinPlainTagHidden
                                                     + sourceContent
                                                     + "}"
                                                     + _tw4WinMarkTagHidden + "<\\}" + segmentPair.TranslationOrigin.MatchPercentage + "\\{>\r\n" + "}"
                                                     + _tw4WinPlainTagNormal
                                                     + targetContent
                                                     + "}"
                                                     + _tw4WinMarkTagHidden + "<0\\}" + "\r\n" + "}" + _tw4WinExternalTagPar
                                                     + _tw4WinExternalTagNormal + "\r\n</seg>" + "\r\n" + "}"
                                                     + _tw4WinExternalTagPar);
                                }
                                else
                                {
                                    SegmentsNotExported++;
                                }
                                #endregion
                            }
                            else
                            {
                                #region  |  excludeLegacyStructure  |
                                SegmentsExported++;

                                if (segmentPair.TranslationOrigin != null)
                                    sw.WriteLine(string.Empty

                                                 + _tw4WinMarkTagHidden + "\\{0>" + "}"
                                                 + _tw4WinPlainTagHidden
                                                 + sourceContent
                                                 + "}"
                                                 + _tw4WinMarkTagHidden + "<\\}" + segmentPair.TranslationOrigin.MatchPercentage + "\\{>\r\n" + "}"
                                                 + _tw4WinPlainTagNormal
                                                 + targetContent
                                                 + "}"
                                                 + _tw4WinMarkTagHidden + "<0\\}" + "\r\n" + "}"
                                                 + _tw4WinExternalTagNormal + "  }"
                                    );

                                #endregion
                            }

                            #endregion
                        }

                        if (!includeLegacyStructure)
                        {
                            sw.WriteLine(_tw4WinExternalTagPar
                                + _tw4WinExternalTagNormal + "" + "\r\n" + "}");
                        }
                    }

                    sw.WriteLine(_tw4WinExternalTagNormal + "</innerFile>}" + _tw4WinExternalTagPar);
                }

                sw.WriteLine(_tw4WinExternalTagNormal + "</file>}" + _tw4WinExternalTagPar);

                sw.WriteLine("}");
                sw.Flush();
                sw.Close();
            }



        }


        private string _tw4WinPlainTagNormal;
        private string _tw4WinPlainTagHidden;
        private string _tw4WinInternalTagNormal;
        private string _tw4WinExternalTagNormal;
        private string _tw4WinExternalTagPar;

        private string _tw4WinMarkTagHidden;



        private void SetTradosStyles(string targetLcid)
        {
            _tw4WinPlainTagNormal = @"{\fs20\cgrid0\f1\lang" + targetLcid + @" " + "\r\n";
            _tw4WinPlainTagHidden = @"{\v \fs20\cgrid0\f1\lang1024 " + "\r\n";
            _tw4WinInternalTagNormal = "\r\n" + @"{\cs6\f1\cf6\lang1024 ";
            _tw4WinMarkTagHidden = @"{\plain \cs1\v\f1\fs24\sub\cf12 ";
            _tw4WinExternalTagNormal = @"{\cs5\f1\cf15\lang1024 ";
            _tw4WinExternalTagPar = @"{\f3 \par }" + "\r\n" + @"\s0 \plain \f3\fs20\cgrid0 " + "\r\n\r\n";

        }
        private string GetRtfDefaultStyleHeader(string targetLcid)
        {
            SetTradosStyles(targetLcid);

            var rtf = @"{\rtf1 \ansi\ansicpg1033\deff3\deflang1024 " + "\r\n";
            rtf += @"{\fonttbl " + "\r\n";
            rtf += @"{\f1 \fmodern\fprq1 \fcharset0 Courier New;}" + "\r\n";
            rtf += @"{\f2 \fswiss\fprq2 \fcharset0 Arial;}" + "\r\n";
            rtf += @"{\f3 \froman\fprq2 \fcharset0 Times New Roman;}}" + "\r\n";
            rtf += @"{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}" + "\r\n";
            rtf += @"{\stylesheet " + "\r\n";
            rtf += @"{\s0 \snext0 \sb0\slmult1\widctlpar \fs20\f3 Normal;}" + "\r\n";
            rtf += @"{\cs1 \additive \v\f1\fs24\sub\cf12 tw4winMark;}" + "\r\n";
            rtf += @"{\cs2 \cf4\fs40\f1 tw4winError;}" + "\r\n";
            rtf += @"{\cs3 \f1\cf11\lang1024 tw4winPopup;}" + "\r\n";
            rtf += @"{\cs4 \f1\cf10\lang1024 tw4winJump;}" + "\r\n";
            rtf += @"{\cs5 \additive \f1\cf15\lang1024 tw4winExternal;}" + "\r\n";
            rtf += @"{\cs6 \additive \f1\cf6\lang1024 tw4winInternal;}" + "\r\n";
            rtf += @"{\cs7 \cf2 tw4winTerm;}" + "\r\n";
            rtf += @"{\cs9 \additive Default Paragraph Font;}" + "\r\n";
            rtf += @"{\cs8 \f1\cf13\lang1024 DO_NOT_TRANSLATE;}}" + "\r\n";
            rtf += @"\paperw11907\paperh16840 " + "\r\n\r\n";
            rtf += @"\pard\s0\sb0\slmult1\widctlpar\pard\s0\sb0\slmult1\widctlpar \plain \fs20\cs5\cf15\cgrid0\f3 " + "\r\n\r\n";

            return rtf;
        }
        private static string ConvertToRtFstr(string str)
        {
            str = str.Replace(@"\", @"\\");
            str = str.Replace(@"{", @"\{");
            str = str.Replace(@"}", @"\}");
            return str;
        }
        private static string ConvertToWordUni(string inputStr)
        {

            inputStr = inputStr.Replace("\n", @"{\line }");

            var strOut = string.Empty;
            foreach (var c in inputStr.ToCharArray())
            {
                if (c <= 0x7f)
                {
                    strOut += c;
                }
                else
                {
                    strOut += "\\u" + Convert.ToUInt32(c) + "?";// "\\'83";// "\\ ";

                }
            }
            return strOut;
        }
    }
}
