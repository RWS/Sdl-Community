using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.XliffReadWrite.SDLXLIFF;

namespace Sdl.Community.XliffToLegacyConverter.Core.TTX
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
                var percent = Convert.ToInt32(current <= maximum && maximum > 0 ? (Convert.ToDecimal(current) / Convert.ToDecimal(maximum)) * Convert.ToDecimal(100) : maximum);
                Progress(maximum, current, percent, message);
            }
            catch
            {
                // ignored
            }
        }

        private static string GetTtxCompatibleLanguageId(string cultureInfoName)
        {
            switch (cultureInfoName.ToUpper())
            {
                case "JA-JP":
                    return "JA";
                case "NB-NO":
                    return "NO-NO";
                case "NN-NO":
                    return "NO-NY";
                default:
                    return cultureInfoName.ToUpper();
            }
        }


        internal int SegmentsExported;
        internal int SegmentsNotExported;

        internal void WriteTtx(string fileName, string sdlxliffFilePath, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits, CultureInfo sourceCulture, CultureInfo targetCulture)
        {
            SegmentsExported = 0;
            SegmentsNotExported = 0;

            var xmlTxtWriter = new XmlTextWriter(fileName, Encoding.Unicode)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };
            xmlTxtWriter.WriteStartDocument(true);

            xmlTxtWriter.WriteComment(Application.ProductName + " by Sdl Community 2017");

            xmlTxtWriter.WriteStartElement("TRADOStag");
            xmlTxtWriter.WriteAttributeString("Version", "2.0");


            #region  |  FrontMatter  |
            xmlTxtWriter.WriteStartElement("FrontMatter");


            xmlTxtWriter.WriteStartElement("ToolSettings");
            xmlTxtWriter.WriteAttributeString("CreationDate", "" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + "T" + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + "Z" + "");
            xmlTxtWriter.WriteAttributeString("CreationTool", "SDL TRADOS TagEditor");
            xmlTxtWriter.WriteAttributeString("CreationToolVersion", "0.0.0.0");
            xmlTxtWriter.WriteEndElement();//ToolSettings


            xmlTxtWriter.WriteStartElement("UserSettings");
            xmlTxtWriter.WriteAttributeString("DataType", "RTF");
            xmlTxtWriter.WriteAttributeString("O-Encoding", "Unicode");
            xmlTxtWriter.WriteAttributeString("SettingsName", "");
            xmlTxtWriter.WriteAttributeString("SettingsPath", "");
            xmlTxtWriter.WriteAttributeString("SourceLanguage", GetTtxCompatibleLanguageId(sourceCulture.Name));
            xmlTxtWriter.WriteAttributeString("TargetLanguage", GetTtxCompatibleLanguageId(targetCulture.Name));
            xmlTxtWriter.WriteAttributeString("TargetDefaultFont", "");
            xmlTxtWriter.WriteAttributeString("SourceDocumentPath", sdlxliffFilePath);
            xmlTxtWriter.WriteAttributeString("SettingsRelativePath", "");
            xmlTxtWriter.WriteEndElement();//UserSettings


            xmlTxtWriter.WriteEndElement();//FrontMatter
            #endregion



            xmlTxtWriter.WriteStartElement("Body");
            xmlTxtWriter.WriteStartElement("Raw");

            #region  |  file  |

            var segmentCount = (
                from fileParagraphUnit in fileParagraphUnits
                from paragraphUnit in fileParagraphUnit.Value
                from segmentPair in paragraphUnit.Value.SegmentPairs
                select segmentPair).Count();

            xmlTxtWriter.WriteStartElement("ut");//file          
            xmlTxtWriter.WriteAttributeString("Type", "start");
            xmlTxtWriter.WriteAttributeString("Style", "external");
            xmlTxtWriter.WriteAttributeString("RightEdge", "angle");
            xmlTxtWriter.WriteAttributeString("DisplayText", "file");

            xmlTxtWriter.WriteString("<file source=\"" + sourceCulture.Name + "\" target=\"" + targetCulture.Name + "\" segmentCount=\"" + segmentCount + "\" path=\"" + sdlxliffFilePath + "\">");
            xmlTxtWriter.WriteEndElement();//ut, file



            var regIsEmptyElement = new Regex(@"\/\s*\>", RegexOptions.Singleline);

            foreach (var fileParagraphUnit in fileParagraphUnits)
            {
                #region  |  file  |

                xmlTxtWriter.WriteValue("\r\n");
                xmlTxtWriter.WriteStartElement("ut");//innerFile               
                xmlTxtWriter.WriteAttributeString("Type", "start");
                xmlTxtWriter.WriteAttributeString("Style", "external");
                xmlTxtWriter.WriteAttributeString("RightEdge", "angle");
                xmlTxtWriter.WriteAttributeString("DisplayText", "innerFile");

                xmlTxtWriter.WriteString("<innerFile name=\"" + fileParagraphUnit.Key + "\">");
                xmlTxtWriter.WriteEndElement();//ut, innerFile


                foreach (var paragraphUnit in fileParagraphUnit.Value)
                {
                    foreach (var segmentPair in paragraphUnit.Value.SegmentPairs)
                    {
                        #region  |  get match type  |


                        var doNotExportSegment = false;

                        var match = string.Empty;
                        if (segmentPair.TranslationOrigin != null)
                        {
                            if (segmentPair.TranslationOrigin.MatchPercentage >= 100)
                            {
                                if (string.Compare(segmentPair.TranslationOrigin.OriginType, "document-match", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    match = "Perfect Match";
                                }
                                else if (string.Compare(segmentPair.TranslationOrigin.TextContextMatchLevel, "SourceAndTarget", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    match = "Context Match";
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
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationApproved;
                                } break;
                            case "ApprovedTranslation":
                                {
                                    if (!doNotExportSegment)
                                        doNotExportSegment = Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationRejected;
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


                        if (!doNotExportSegment)
                        {
                            SegmentsExported++;

                            #region  |  seg  |

                            xmlTxtWriter.WriteValue("\r\n");
                            xmlTxtWriter.WriteStartElement("ut");//seg                            
                            xmlTxtWriter.WriteAttributeString("Type", "start");
                            xmlTxtWriter.WriteAttributeString("Style", "external");
                            xmlTxtWriter.WriteAttributeString("RightEdge", "angle");
                            xmlTxtWriter.WriteAttributeString("DisplayText", "seg");
                            xmlTxtWriter.WriteString("<seg id=\"" + segmentPair.Id + "\" pid=\"" + paragraphUnit.Key + "\" status=\"" + Core.Processor.GetVisualSegmentStatus(segmentPair.SegmentStatus) + "\" locked=\"" + segmentPair.IsLocked + "\" match=\"" + match + "\">");
                            xmlTxtWriter.WriteEndElement();//ut, seg


                            #region  |  tu  |

                            xmlTxtWriter.WriteStartElement("Tu");//tu                          
                            var origin = "undefined";
                            if (match == "Perfect Match")
                                origin = "xtranslate";
                            xmlTxtWriter.WriteAttributeString("Origin", origin);
                            if (segmentPair.TranslationOrigin != null)
                                xmlTxtWriter.WriteAttributeString("MatchPercent", segmentPair.TranslationOrigin.MatchPercentage.ToString());


                            #region  |  source  |
                            xmlTxtWriter.WriteStartElement("Tuv");//tuv                         
                            xmlTxtWriter.WriteAttributeString("lang", GetTtxCompatibleLanguageId(sourceCulture.Name));


                            foreach (var segmentSection in segmentPair.SourceSections)
                            {
                                switch (segmentSection.Type)
                                {
                                    case SegmentSection.ContentType.Text:
                                        xmlTxtWriter.WriteString(segmentSection.Content);
                                        break;
                                    case SegmentSection.ContentType.Tag:
                                        {
                                            xmlTxtWriter.WriteStartElement("df");//df                                        
                                            xmlTxtWriter.WriteAttributeString("Font", "Courier New");
                                            xmlTxtWriter.WriteAttributeString("Colour", "0xff0000");

                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                        
                                            xmlTxtWriter.WriteAttributeString("Type", "start");
                                            xmlTxtWriter.WriteAttributeString("RightEdge", "angle");
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);
                                            xmlTxtWriter.WriteEndElement();//ut
                                            xmlTxtWriter.WriteEndElement();//df
                                        }
                                        break;
                                    case SegmentSection.ContentType.LockedContent:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                      
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString("<ILockedContent>" + segmentSection.Content + "</ILockedContent>");
                                            xmlTxtWriter.WriteEndElement();//ut
                                        }
                                        break;
                                    case SegmentSection.ContentType.TagClosing:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                        
                                            xmlTxtWriter.WriteAttributeString("Type", "end");
                                            xmlTxtWriter.WriteAttributeString("LeftEdge", "angle");
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);
                                            xmlTxtWriter.WriteEndElement();//ut                                                                                                                  
                                        }
                                        break;
                                    default:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                           
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);
                                            xmlTxtWriter.WriteEndElement();//ut


                                        }
                                        break;
                                }
                            }

                            xmlTxtWriter.WriteEndElement();//tuv

                            #endregion

                            #region  |  target  |
                            xmlTxtWriter.WriteStartElement("Tuv");//tuv                           
                            xmlTxtWriter.WriteAttributeString("lang", GetTtxCompatibleLanguageId(targetCulture.Name));

                            foreach (var segmentSection in segmentPair.TargetSections)
                            {
                                switch (segmentSection.Type)
                                {
                                    case SegmentSection.ContentType.Text:
                                        xmlTxtWriter.WriteString(segmentSection.Content);
                                        break;
                                    case SegmentSection.ContentType.Tag:
                                        {
                                            xmlTxtWriter.WriteStartElement("df");//df                                      
                                            xmlTxtWriter.WriteAttributeString("Font", "Courier New");
                                            xmlTxtWriter.WriteAttributeString("Colour", "0xff0000");

                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                      
                                            xmlTxtWriter.WriteAttributeString("Type", "start");
                                            xmlTxtWriter.WriteAttributeString("RightEdge", "angle");
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);

                                            xmlTxtWriter.WriteEndElement();//ut
                                            xmlTxtWriter.WriteEndElement();//df


                                        }
                                        break;
                                    case SegmentSection.ContentType.LockedContent:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                       
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString("<ILockedContent>" + segmentSection.Content + "</ILockedContent>");

                                            xmlTxtWriter.WriteEndElement();//ut

                                        }
                                        break;
                                    case SegmentSection.ContentType.TagClosing:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                           
                                            xmlTxtWriter.WriteAttributeString("Type", "end");
                                            xmlTxtWriter.WriteAttributeString("LeftEdge", "angle");
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);
                                            xmlTxtWriter.WriteEndElement();//ut

                                        }
                                        break;
                                    default:
                                        {
                                            var refId = segmentSection.SectionId;
                                            var name = Core.Processor.GetStartTagName(segmentSection.Content, ref refId);

                                            xmlTxtWriter.WriteStartElement("ut");//start tag                                           
                                            xmlTxtWriter.WriteAttributeString("DisplayText", name);
                                            xmlTxtWriter.WriteString(segmentSection.Content);
                                            xmlTxtWriter.WriteEndElement();//ut

                                        }
                                        break;
                                }
                            }

                            xmlTxtWriter.WriteEndElement();//tuv
                            #endregion


                            xmlTxtWriter.WriteEndElement();//tu

                            #endregion


                            xmlTxtWriter.WriteStartElement("ut");//seg                           
                            xmlTxtWriter.WriteAttributeString("Type", "end");
                            xmlTxtWriter.WriteAttributeString("Style", "external");
                            xmlTxtWriter.WriteAttributeString("LeftEdge", "angle");
                            xmlTxtWriter.WriteAttributeString("DisplayText", "seg");
                            xmlTxtWriter.WriteString("</seg>");
                            xmlTxtWriter.WriteEndElement();//ut, seg

                            #endregion
                        }
                        else
                        {
                            SegmentsNotExported++;
                        }
                    }
                }

                xmlTxtWriter.WriteValue("\r\n");
                xmlTxtWriter.WriteStartElement("ut");//innerFile                
                xmlTxtWriter.WriteAttributeString("Type", "end");
                xmlTxtWriter.WriteAttributeString("Style", "external");
                xmlTxtWriter.WriteAttributeString("LeftEdge", "angle");
                xmlTxtWriter.WriteAttributeString("DisplayText", "innerFile");
                xmlTxtWriter.WriteString("</innerFile>");
                xmlTxtWriter.WriteEndElement();//ut, file

                #endregion
            }



            xmlTxtWriter.WriteValue("\r\n");
            xmlTxtWriter.WriteStartElement("ut");//file            
            xmlTxtWriter.WriteAttributeString("Type", "end");
            xmlTxtWriter.WriteAttributeString("Style", "external");
            xmlTxtWriter.WriteAttributeString("LeftEdge", "angle");
            xmlTxtWriter.WriteAttributeString("DisplayText", "file");
            xmlTxtWriter.WriteString("</file>");
            xmlTxtWriter.WriteEndElement();//ut, file

            #endregion

            xmlTxtWriter.WriteEndElement();//Raw
            xmlTxtWriter.WriteEndElement();//Body


            xmlTxtWriter.WriteEndElement();//TRADOStag


            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();
        }


        internal int SegmentsImported;
        internal int SegmentsNotImported;

        private class UTAttributes
        {
            public string Type { get; set; }
            public string Style { get; set; }
            public string DisplayText { get; set; }
            public string MatchPercent { get; set; }
            public string Origin { get; set; }
            public string Lang { get; set; }

            public UTAttributes()
            {
                Type = string.Empty;
                Style = string.Empty;
                DisplayText = string.Empty;
                MatchPercent = string.Empty;
                Origin = string.Empty;
                Lang = string.Empty;
            }
        }

        private class StateManager
        {
            public bool TUOpen { get; set; }
            public bool TUVSourceOpen { get; set; }
            public bool TUVTargetOpen { get; set; }
            public int OtherTagsOpen { get; set; }
            public bool SegOpen { get; set; }


            public StateManager()
            {
                SegOpen = false;
                TUOpen = false;
                OtherTagsOpen = 0;
                TUVSourceOpen = false;
                TUVTargetOpen = false;
            }
        }

        private const string TuElementName = "Tu";
        private const string UtElmentName = "ut";
        private const string TuvElementName = "Tuv";
        private const string SegElementName = "seg";

        private const string UtTypeAttributeValueStart = "start";
        private const string UtTypeAttributeValueEnd = "end";
        private const string XptTagIdConstant = "xpt";

        internal Dictionary<string, ParagraphUnit> ReadTtx(string filePath)
        {
            SegmentsImported = 0;
            SegmentsNotImported = 0;

            var paragraphUnits = new Dictionary<string, ParagraphUnit>();

            CheckTransformCharacters(filePath);

            var rdr = new XmlTextReader(filePath);


            try
            {
                var regexSeg = new Regex(@"\<seg id\=""(?<x1>[^""]*)""\s+pid\=""(?<x2>[^""]*)""\s+status\=""(?<x3>[^""]*)""\s+locked\=""(?<x4>[^""]*)""\s+match\=""(?<x5>[^""]*)""[^\>]*\>"
                    , RegexOptions.IgnoreCase | RegexOptions.Singleline);


                var iTagIdSource = 90000;
                var iTagIdTarget = 90000;

                var stateManager = new StateManager();
                var segmentPair = new SegmentPair();
	            var tagType = string.Empty;

                while (rdr.Read())
                {
	                //var tagType = "empty";

					switch (rdr.NodeType)
					{
                        case XmlNodeType.Element:
                            {
                                #region  |  XmlNodeType.Element  |

                                var elementName = rdr.Name;
	                            tagType = "empty";
								var elementAttributes = new Dictionary<string, string>();
                                while (rdr.MoveToNextAttribute())
                                    elementAttributes.Add(rdr.Name, rdr.Value);


                                var utAttributes = new UTAttributes();

                                foreach (var elementAttribute in elementAttributes)
                                {
	                                if (string.Compare(elementAttribute.Key, "Type", StringComparison.OrdinalIgnoreCase) ==
	                                    0)
	                                {
										utAttributes.Type = elementAttribute.Value;
		                                tagType = elementAttribute.Value;
									}
                                        
                                    else if (string.Compare(elementAttribute.Key, "Style", StringComparison.OrdinalIgnoreCase) == 0)
                                        utAttributes.Style = elementAttribute.Value;
                                    else if (string.Compare(elementAttribute.Key, "DisplayText", StringComparison.OrdinalIgnoreCase) == 0)
                                        utAttributes.DisplayText = elementAttribute.Value;
                                    else if (string.Compare(elementAttribute.Key, "MatchPercent", StringComparison.OrdinalIgnoreCase) == 0)
                                        utAttributes.MatchPercent = elementAttribute.Value;
                                    else if (string.Compare(elementAttribute.Key, "Origin", StringComparison.OrdinalIgnoreCase) == 0)
                                        utAttributes.Origin = elementAttribute.Value;

                                }

                                if (utAttributes.MatchPercent != string.Empty)
                                {
                                    try
                                    {
                                        segmentPair.TranslationOrigin.MatchPercentage = Convert.ToInt32(utAttributes.MatchPercent);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }


                                if (string.CompareOrdinal(elementName, UtElmentName) == 0)
                                {

                                    if (string.Compare(utAttributes.Type, UtTypeAttributeValueStart, StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        if (string.Compare(utAttributes.DisplayText, SegElementName, StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            segmentPair = new SegmentPair();
                                            stateManager = new StateManager
                                            {
                                                SegOpen = true
                                            };

                                        }
                                        else
                                        {
                                            stateManager.OtherTagsOpen++;
                                        }
                                    }
                                    else
                                    {

                                        if (string.Compare(utAttributes.Type, UtTypeAttributeValueEnd, StringComparison.OrdinalIgnoreCase) == 0)
                                        {
                                            if (string.Compare(utAttributes.DisplayText, SegElementName, StringComparison.OrdinalIgnoreCase) == 0)
                                            {
                                                stateManager.SegOpen = false;

                                                var importTranslation = ImportTranslation(segmentPair);
                                                if (importTranslation)
                                                {
                                                    var currentTagListSource = new List<TagUnit>();
                                                    for (var ix = 0; ix < segmentPair.SourceSections.Count; ix++)
                                                        segmentPair.SourceSections[ix] = MarkupTagContent(segmentPair.SourceSections[ix], currentTagListSource, ref iTagIdSource);

                                                    var currentTagListTarget = new List<TagUnit>();
                                                    for (var ix = 0; ix < segmentPair.TargetSections.Count; ix++)
                                                        segmentPair.TargetSections[ix] = MarkupTagContent(segmentPair.TargetSections[ix], currentTagListTarget, ref iTagIdTarget);

                                                    segmentPair.Source = Core.Processor.GetSectionsToText(segmentPair.SourceSections);
                                                    segmentPair.Target = Core.Processor.GetSectionsToText(segmentPair.TargetSections);


                                                    if (segmentPair.Target.Trim() == string.Empty && Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.IgnoreEmptyTranslations)
                                                    {
                                                        SegmentsNotImported++;
                                                    }
                                                    else
                                                    {
                                                        SegmentsImported++;

                                                        var paragraphUnit = new ParagraphUnit(segmentPair.ParagraphId, new List<SegmentPair>(), string.Empty);

                                                        paragraphUnit.SegmentPairs.Add(segmentPair);

                                                        if (segmentPair.ParagraphId != string.Empty)
                                                        {
                                                            if (!paragraphUnits.ContainsKey(segmentPair.ParagraphId))
                                                                paragraphUnits.Add(segmentPair.ParagraphId, paragraphUnit);
                                                            else
                                                                paragraphUnits[segmentPair.ParagraphId].SegmentPairs.AddRange(paragraphUnit.SegmentPairs);

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    SegmentsNotImported++;
                                                }

                                                segmentPair = new SegmentPair();
                                                stateManager = new StateManager();
                                            }
                                            else
                                            {
                                                stateManager.OtherTagsOpen++;
                                            }
                                        }
                                        else
                                        {
                                            stateManager.OtherTagsOpen++;
                                        }
                                    }
                                }
                                else if (string.CompareOrdinal(elementName, TuElementName) == 0)
                                {

                                    stateManager.TUOpen = true;
                                    stateManager.OtherTagsOpen = 0;
                                    stateManager.SegOpen = false;
                                }
                                else if (string.CompareOrdinal(elementName, TuvElementName) == 0)
                                {

                                    stateManager.OtherTagsOpen = 0;
                                    stateManager.SegOpen = false;

                                    if (stateManager.TUVSourceOpen)
                                    {
                                        stateManager.TUVSourceOpen = false;
                                        stateManager.TUVTargetOpen = true;
                                    }
                                    else
                                    {
                                        stateManager.TUVSourceOpen = true;
                                    }

                                }
                                #endregion
                            } break;
                        case XmlNodeType.EndElement:
                            {
                                #region  |  XmlNodeType.EndElement  |

                                var elementName = rdr.Name;

                                if (string.CompareOrdinal(elementName, TuElementName) == 0)
                                {
                                    stateManager.TUOpen = false;
                                }
                                else if (string.CompareOrdinal(elementName, TuvElementName) == 0)
                                {
                                    if (stateManager.TUVTargetOpen)
                                        stateManager.TUVTargetOpen = false;
                                }
                                else if (string.CompareOrdinal(elementName, UtElmentName) == 0)
                                {
                                    if (stateManager.OtherTagsOpen > 0)
                                        stateManager.OtherTagsOpen--;
                                }
                                #endregion
                            } break;
                        case XmlNodeType.Text:
                            {
                                #region  |  XmlNodeType.Text  |

                                if (stateManager.SegOpen)
                                {
                                    if (segmentPair.Id == string.Empty)
                                    {
                                        var matchSeg = regexSeg.Match(rdr.Value);
                                        if (matchSeg.Success)
                                        {
                                            segmentPair.Id = matchSeg.Groups["x1"].Value;
                                            segmentPair.ParagraphId = matchSeg.Groups["x2"].Value;
                                            segmentPair.SegmentStatus = Core.Processor.GetSegmentStatusFromVisual(matchSeg.Groups["x3"].Value);
                                            segmentPair.IsLocked = Convert.ToBoolean(matchSeg.Groups["x4"].Value);
                                            segmentPair.MatchTypeId = matchSeg.Groups["x5"].Value;

                                            stateManager.SegOpen = false;
                                        }
                                    }
                                }
                                else if (stateManager.OtherTagsOpen > 0)
                                {
                                    stateManager.OtherTagsOpen--;
									//aici ar trebui pus  atributul cu end tag
                                    var tags = Core.Processor.SeperateTags(rdr.Value,tagType);
                                    foreach (var tag in tags)
                                    {
                                        switch (tag.Type)
                                        {
                                            case TagUnit.TagUnitType.IsPlaceholder:
                                                {
                                                    AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.Placeholder, tag);
                                                    break;
                                                }
                                            case TagUnit.TagUnitType.IsTag:
                                                {
                                                    switch (tag.State)
                                                    {
                                                        case TagUnit.TagUnitState.IsOpening:
                                                            {
                                                                AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.Tag, tag);
                                                                break;
                                                            }
                                                        case TagUnit.TagUnitState.IsClosing:
                                                            {
                                                                AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.TagClosing, tag);
                                                                break;
                                                            }
                                                        case TagUnit.TagUnitState.IsEmpty:
                                                            {
                                                                AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.Placeholder, tag);
                                                                break;
                                                            }
                                                    } break;
                                                }
                                            case TagUnit.TagUnitType.IsLockedContent:
                                                {
                                                    AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.LockedContent, tag);
                                                    break;
                                                }
                                        }
                                    }
                                }
                                else if (stateManager.TUOpen)
                                {
                                    AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.Text, rdr.Value);
                                }

                                #endregion
                            } break;
                        case XmlNodeType.DocumentType:
                            break;
                        case XmlNodeType.CDATA:
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.Document:
                            break;
                        case XmlNodeType.Whitespace:
                            if (stateManager.TUOpen)                            
                                AddSegmentSections(stateManager, segmentPair, SegmentSection.ContentType.Text, rdr.Value);                                                            
                            break;
                        case XmlNodeType.SignificantWhitespace:
                            break;
                        case XmlNodeType.EntityReference:
                            break;
                        case XmlNodeType.XmlDeclaration:
                            break;
                    }
                }
            }
            finally
            {
                rdr.Close();
            }

            return paragraphUnits;
        }

        private static void AddSegmentSections(StateManager stateManager, SegmentPair segmentPair, SegmentSection.ContentType tagType, string content)
        {

            if (stateManager.TUVSourceOpen)
                segmentPair.SourceSections.Add(new SegmentSection(tagType, string.Empty,
                    GetTagWithNLineMarker(content)));
            else
                segmentPair.TargetSections.Add(new SegmentSection(tagType, string.Empty,
                    GetTagWithNLineMarker(content)));
        }

        private static void AddSegmentSections(StateManager stateManager, SegmentPair segmentPair, SegmentSection.ContentType tagType, TagUnit tag)
        {

            if (stateManager.TUVSourceOpen)
                segmentPair.SourceSections.Add(new SegmentSection(tagType, tag.Id,
                    GetTagWithNLineMarker(tag.Content)));
            else
                segmentPair.TargetSections.Add(new SegmentSection(tagType, tag.Id,
                    GetTagWithNLineMarker(tag.Content)));
        }

        private static SegmentSection MarkupTagContent(SegmentSection section, IList<TagUnit> currentTagList, ref int iTagId)
        {

            if (section.Type != SegmentSection.ContentType.Tag)
            {
                if (section.Type == SegmentSection.ContentType.TagClosing)
                {
                    var closingTagName = Core.Processor.GetEndTagName(section.Content);
                    if (currentTagList.Count > 0)
                    {
                        if (string.Compare(currentTagList[currentTagList.Count - 1].Name,
                                closingTagName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            section.Content =
                                Core.Processor.GetContentTypeToMarkup(section.Type, section.Content,
                                    currentTagList[currentTagList.Count - 1].Id);

                            currentTagList.RemoveAt(currentTagList.Count - 1);
                        }
                        else
                        {
                            section.Content =
                                Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, string.Empty);
                        }
                    }
                    else
                    {
                        section.Content =
                            Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, string.Empty);
                    }
                }
                else
                {
                    iTagId++;
                    var tagId = XptTagIdConstant + iTagId;
                    Core.Processor.GetStartTagName(section.Content, ref tagId);

                    section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content, tagId);
                }
            }
            else
            {
                iTagId++;
                var tagId = XptTagIdConstant + iTagId;
                var tagName = Core.Processor.GetStartTagName(section.Content, ref tagId);

                currentTagList.Add(new TagUnit(tagId, tagName, section.Content,
                    TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
                section.Content = Core.Processor.GetContentTypeToMarkup(section.Type, section.Content,
                    currentTagList[currentTagList.Count - 1].Id);
            }
            return section;
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


            if (string.Compare(segmentPair.SegmentStatus, "Not Translated", StringComparison.OrdinalIgnoreCase) == 0 &&
                Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportNotTranslated)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Draft", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportDraft)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Translated", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslated)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Translation Approved", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationApproved)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Translation Rejected", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportTranslationRejected)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Sign-off Rejected", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOffRejected)
                importTranslation = false;
            else if (string.Compare(segmentPair.SegmentStatus, "Sign-off", StringComparison.OrdinalIgnoreCase) == 0 &&
                     Sdl.Community.XliffReadWrite.Processor.ProcessorSettings.DoNotImportSignOff)
                importTranslation = false;
            return importTranslation;
        }



        private static string GetTagWithNLineMarker(string tagContent)
        {
            tagContent = tagContent.Replace("\r\n", "<-_(-_NEWLINE_KEEP_-)_->");
            tagContent = tagContent.Replace("\n", "\r\n");
            tagContent = tagContent.Replace("<-_(-_NEWLINE_KEEP_-)_->", "\r\n");

            return tagContent;
        }
        private static void CheckTransformCharacters(string filePath)
        {
            try
            {
                var sb = new StringBuilder();
                using (var r = new StreamReader(filePath, Encoding.Unicode))
                {
                    sb.Append(r.ReadToEnd());
                    r.Close();
                }

                if (sb.ToString().IndexOf((char)2) <= -1 && sb.ToString().IndexOf((char)3) <= -1 &&
                    sb.ToString().IndexOf((char)4) <= -1 && sb.ToString().IndexOf((char)5) <= -1 &&
                    sb.ToString().IndexOf((char)6) <= -1 && sb.ToString().IndexOf((char)7) <= -1 &&
                    sb.ToString().IndexOf((char)8) <= -1 && sb.ToString().IndexOf((char)15) <= -1 &&
                    sb.ToString().IndexOf((char)16) <= -1 && sb.ToString().IndexOf((char)17) <= -1 &&
                    sb.ToString().IndexOf((char)18) <= -1 && sb.ToString().IndexOf((char)19) <= -1 &&
                    sb.ToString().IndexOf((char)21) <= -1 && sb.ToString().IndexOf((char)22) <= -1 &&
                    sb.ToString().IndexOf((char)23) <= -1 && sb.ToString().IndexOf((char)24) <= -1 &&
                    sb.ToString().IndexOf((char)25) <= -1 && sb.ToString().IndexOf((char)26) <= -1 &&
                    sb.ToString().IndexOf((char)27) <= -1 && sb.ToString().IndexOf((char)28) <= -1 &&
                    sb.ToString().IndexOf((char)29) <= -1) return;
                var tmpFilePath = filePath + "___.tmp.bak";
                File.Copy(filePath, tmpFilePath, true);


                sb.Replace(((char)2).ToString(), "&#x2;");
                sb.Replace(((char)3).ToString(), "&#x3;");
                sb.Replace(((char)4).ToString(), "&#x4;");
                sb.Replace(((char)5).ToString(), "&#x5;");
                sb.Replace(((char)6).ToString(), "&#x6;");
                sb.Replace(((char)7).ToString(), "&#x7;");
                sb.Replace(((char)8).ToString(), "&#x8;");
                sb.Replace(((char)15).ToString(), "&#x000F;");
                sb.Replace(((char)16).ToString(), "&#x0010;");
                sb.Replace(((char)17).ToString(), "&#x0011;");
                sb.Replace(((char)18).ToString(), "&#x0012;");
                sb.Replace(((char)19).ToString(), "&#x0013;");

                sb.Replace(((char)21).ToString(), "&#x0015;");
                sb.Replace(((char)22).ToString(), "&#x0016;");
                sb.Replace(((char)23).ToString(), "&#x0017;");
                sb.Replace(((char)24).ToString(), "&#x0018;");
                sb.Replace(((char)25).ToString(), "&#x0019;");
                sb.Replace(((char)26).ToString(), "&#x001A;");
                sb.Replace(((char)27).ToString(), "&#x001B;");
                sb.Replace(((char)28).ToString(), "&#x001C;");
                sb.Replace(((char)29).ToString(), "&#x001D;");

                using (var w = new StreamWriter(tmpFilePath, false, Encoding.Unicode))
                {
                    w.Write(sb);
                    w.Flush();
                    w.Close();
                }

                File.Delete(filePath);
                File.Move(tmpFilePath, filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
