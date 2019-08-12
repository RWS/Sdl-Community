using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Sdl.Community.XliffReadWrite.SDLXLIFF;

namespace Sdl.Community.XliffToLegacyConverter.Core.TMX
{

    internal class Processor
    {
       

        public delegate void WriterProgress(long maximum, long currentValue, string elementName);
        public static event WriterProgress Progress;

        private static void OnProgress(long maximum, long currentvalue, string elementname)
        {
            var handler = Progress;
            if (handler != null) handler(maximum, currentvalue, elementname);
        }
    

        public bool ExcludeTags { get; set; }
        public bool ReverseLanguageDirection { get; set; }

        internal int SegmentsExported;
        internal int SegmentsNotExported;

        public void WriteTmx(string fileName, string sdlxliffFilePath, 
            Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits, 
            CultureInfo sourceCulture, CultureInfo targetCulture)
        {

            SegmentsExported = 0;
            SegmentsNotExported = 0;

            #region  |  get total segments count  |
            var segmentCount = (from fileParagraphUnit in fileParagraphUnits 
                                from paragraphUnit in fileParagraphUnit.Value 
                                    from segmentPair in paragraphUnit.Value.SegmentPairs 
                                        select segmentPair).Count();

            #endregion



            var xmlTxtWriter = new XmlTextWriter(fileName, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 3,
                Namespaces = false
            };

            xmlTxtWriter.WriteStartDocument(true);


            xmlTxtWriter.WriteDocType("tmx", null, "tmx14.dtd", null);

            
            try
            {
                #region  |  tmx  |

                xmlTxtWriter.WriteStartElement("tmx");
                xmlTxtWriter.WriteAttributeString("version", "1.4");
                
                #region  |  tmx  |
                xmlTxtWriter.WriteStartElement("header");


                xmlTxtWriter.WriteAttributeString("creationtool", Application.ProductName);
                xmlTxtWriter.WriteAttributeString("creationtoolversion", Application.ProductVersion);
                xmlTxtWriter.WriteAttributeString("segtype", "sentence");
                xmlTxtWriter.WriteAttributeString("o-tmf", "TW4Win 2.0 Format");
                xmlTxtWriter.WriteAttributeString("adminlang", "en-US");
                xmlTxtWriter.WriteAttributeString("srclang", (ReverseLanguageDirection ? GetCompatibleLanguageId(targetCulture.Name) : GetCompatibleLanguageId(sourceCulture.Name)));
                xmlTxtWriter.WriteAttributeString("datatype", "xliff");
                xmlTxtWriter.WriteAttributeString("o-encoding", "UTF-8");
                xmlTxtWriter.WriteAttributeString("creationdate", getTMX_DateToString(DateTime.Now));
                xmlTxtWriter.WriteAttributeString("creationid", "SdlXliffToLegacyConverter");               
                xmlTxtWriter.WriteString("\r\n\r\n");


                xmlTxtWriter.WriteEndElement(); //header

                #endregion


                #region  |  body  |

                xmlTxtWriter.WriteStartElement("body");



                foreach (var fileParagraphUnit in fileParagraphUnits)
                {
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
                                        doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportPerfectMatch;
                                        break;
                                    }
                                case "Context Match":
                                    {
                                        doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportContextMatch;
                                        break;
                                    }
                                case "Exact Match":
                                    {
                                        doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportExactMatch;
                                        break;
                                    }
                                case "Fuzzy Match":
                                    {
                                        doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportFuzzyMatch;
                                        break;
                                    }
                                case "No Match":
                                    {
                                        doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportNoMatch;
                                        break;
                                    }
                            }



                            switch (segmentPair.SegmentStatus)
                            {
                                case "Unspecified":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportNotTranslated;
                                    } break;
                                case "Draft":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportDraft;
                                    } break;
                                case "Translated":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslated;
                                    } break;
                                case "RejectedTranslation":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationApproved;
                                    } break;
                                case "ApprovedTranslation":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportTranslationRejected;
                                    } break;
                                case "RejectedSignOff":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOffRejected;
                                    } break;
                                case "ApprovedSignOff":
                                    {
                                        if (!doNotExportSegment)
                                            doNotExportSegment = XliffReadWrite.Processor.ProcessorSettings.DoNotExportSignOff;
                                    } break;
                            }



                            if (!doNotExportSegment)
                            {
                                if (segmentPair.IsLocked && XliffReadWrite.Processor.ProcessorSettings.DoNotExportLocked)
                                    doNotExportSegment = true;
                                else if (!segmentPair.IsLocked && XliffReadWrite.Processor.ProcessorSettings.DoNotExportUnLocked)
                                    doNotExportSegment = true;
                            }

                            if (segmentPair.Target.Trim() == string.Empty)
                            {
                                doNotExportSegment = true;
                            }

                            #endregion

                            #region  |  create content text  |


                            if (!doNotExportSegment)
                            {
                                SegmentsExported++;

                                #region  |  tu  |

                                xmlTxtWriter.WriteStartElement("tu");
                                xmlTxtWriter.WriteAttributeString("creationdate", getTMX_DateToString(DateTime.Now));
                                xmlTxtWriter.WriteAttributeString("creationid", "SdlXliffToLegacyConverter");

                                #region  |  tu properties  |
                                if (segmentPair.TranslationOrigin != null)
                                {
                                    if (segmentPair.TranslationOrigin.OriginalTranslationHash != null && segmentPair.TranslationOrigin.OriginalTranslationHash.Trim() != string.Empty)
                                    {
                                        xmlTxtWriter.WriteStartElement("prop");
                                        xmlTxtWriter.WriteAttributeString("type", "Txt::OriginalTranslationHash");
                                        xmlTxtWriter.WriteString(segmentPair.TranslationOrigin.OriginalTranslationHash);
                                        xmlTxtWriter.WriteEndElement(); //prop
                                    }

                                 

                                    if (segmentPair.TranslationOrigin.OriginSystem != null && segmentPair.TranslationOrigin.OriginSystem.Trim() != string.Empty)
                                    {
                                        xmlTxtWriter.WriteStartElement("prop");
                                        xmlTxtWriter.WriteAttributeString("type", "Txt::OriginSystem");
                                        xmlTxtWriter.WriteString(segmentPair.TranslationOrigin.OriginSystem);
                                        xmlTxtWriter.WriteEndElement(); //prop
                                    }


                                }
                                #endregion

                                List<SegmentSection> sectionsSource;
                                List<SegmentSection> sectionsTarget;

                                string cultureInfoSource;
                                string cultureInfoTarget;

                                if (ReverseLanguageDirection)
                                {
                                    cultureInfoSource = GetCompatibleLanguageId(targetCulture.Name);
                                    cultureInfoTarget = GetCompatibleLanguageId(sourceCulture.Name);

                                    sectionsSource = segmentPair.TargetSections;
                                    sectionsTarget = segmentPair.SourceSections;
                                }
                                else
                                {
                                    cultureInfoSource = GetCompatibleLanguageId(sourceCulture.Name);
                                    cultureInfoTarget = GetCompatibleLanguageId(targetCulture.Name);

                                    sectionsSource = segmentPair.SourceSections;
                                    sectionsTarget = segmentPair.TargetSections;
                                }



                                xmlTxtWriter.WriteStartElement("tuv");
                                xmlTxtWriter.WriteAttributeString("xml:lang", cultureInfoSource);


                                #region  |  seg  |
                                xmlTxtWriter.WriteStartElement("seg");
                                foreach (var segmentSection in sectionsSource)
                                {
                                    if (segmentSection.Type == SegmentSection.ContentType.Text)
                                    {
                                        xmlTxtWriter.WriteString(segmentSection.Content);
                                    }
                                    else
                                    {
                                        if (ExcludeTags) 
                                            continue;

                                        switch (segmentSection.Type)
                                        {
                                            case SegmentSection.ContentType.Tag:
                                            {
                                                xmlTxtWriter.WriteStartElement("bpt");
                                                xmlTxtWriter.WriteAttributeString("i", segmentSection.SectionId);
                                            } break;
                                            case SegmentSection.ContentType.Placeholder:
                                            {
                                                xmlTxtWriter.WriteStartElement("ph");
                                                xmlTxtWriter.WriteAttributeString("x", segmentSection.SectionId);
                                            } break;
                                            case SegmentSection.ContentType.TagClosing:
                                            {
                                                xmlTxtWriter.WriteStartElement("ept");
                                                xmlTxtWriter.WriteAttributeString("i", segmentSection.SectionId);
                                            } break;
                                            default:
                                            {
                                                xmlTxtWriter.WriteStartElement("ph");
                                                xmlTxtWriter.WriteAttributeString("x", segmentSection.SectionId);
                                            } break;
                                        }

                                        xmlTxtWriter.WriteEndElement();
                                    }
                                }
                                xmlTxtWriter.WriteEndElement(); //seg
                                #endregion
                                xmlTxtWriter.WriteEndElement(); //tuv



                                xmlTxtWriter.WriteStartElement("tuv");
                                xmlTxtWriter.WriteAttributeString("xml:lang", cultureInfoTarget);


                                #region  |  seg  |
                                xmlTxtWriter.WriteStartElement("seg");

                                foreach (var segmentSection in sectionsTarget)
                                {
                                    if (segmentSection.Type == SegmentSection.ContentType.Text)
                                    {
                                        xmlTxtWriter.WriteString(segmentSection.Content);
                                    }
                                    else
                                    {
                                        if (ExcludeTags) 
                                            continue;

                                        switch (segmentSection.Type)
                                        {
                                            case SegmentSection.ContentType.Tag:
                                            {
                                                xmlTxtWriter.WriteStartElement("bpt");
                                                xmlTxtWriter.WriteAttributeString("i", segmentSection.SectionId);
                                            } break;
                                            case SegmentSection.ContentType.Placeholder:
                                            {
                                                xmlTxtWriter.WriteStartElement("ph");
                                                xmlTxtWriter.WriteAttributeString("x", segmentSection.SectionId);
                                            } break;
                                            case SegmentSection.ContentType.TagClosing:
                                            {
                                                xmlTxtWriter.WriteStartElement("ept");
                                                xmlTxtWriter.WriteAttributeString("i", segmentSection.SectionId);
                                            } break;
                                            default:
                                            {
                                                xmlTxtWriter.WriteStartElement("ph");
                                                xmlTxtWriter.WriteAttributeString("x", segmentSection.SectionId);
                                            } break;
                                        }

                                        xmlTxtWriter.WriteEndElement();
                                    }
                                }
                                xmlTxtWriter.WriteEndElement(); //seg
                                #endregion
                                xmlTxtWriter.WriteEndElement(); //tuv


                                xmlTxtWriter.WriteEndElement(); //tu

                                #endregion
                            }
                            else
                            {
                                SegmentsNotExported++;
                            }

                            #endregion
                        }
                    }
                }

                xmlTxtWriter.WriteEndElement(); //body

                #endregion


                xmlTxtWriter.WriteEndElement(); //tmx


                #endregion

            }
            finally
            {
                xmlTxtWriter.Flush();
                xmlTxtWriter.Close();
                xmlTxtWriter = null;
            }
          
        }

        private static string GetCompatibleLanguageId(string cultureInfoName)
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

        private static string getTMX_DateToString(DateTime dt)
        {
            return dt.Year + dt.Month.ToString().PadLeft(2, '0') + dt.Day.ToString().PadLeft(2, '0') + "T" + dt.Hour.ToString().PadLeft(2, '0') + dt.Minute.ToString().PadLeft(2, '0') + dt.Second.ToString().PadLeft(2, '0') + "Z";
        }


       
    }
}
