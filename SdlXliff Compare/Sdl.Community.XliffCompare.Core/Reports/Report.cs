using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.Community.XliffCompare.Core.Comparer.TextComparer;

namespace Sdl.Community.XliffCompare.Core.Reports
{
    internal class Report
    {

        internal enum ReportType
        {
            Xml = 0,
            Html
        }



        internal void CreateXmlReport(string reportFilePath,
            Dictionary<Comparer.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparer.Comparer.ComparisonParagraphUnit>>> fileComparisonFileParagraphUnits,
            bool transformReport)
        {

            try
            {
                var xmlTxtWriter = new XmlTextWriter(reportFilePath, Encoding.UTF8)
                {
                    Formatting = Formatting.None,
                    Indentation = 3,
                    Namespaces = false
                };
                xmlTxtWriter.WriteStartDocument(true);



                xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + DefaultXsltName + "'");
                xmlTxtWriter.WriteComment("SDLXLIFF Compare by Patrick Hartnett, 2011");

                #region  |  files  |


                var filesCompared = fileComparisonFileParagraphUnits.Sum(fileComparisonFileParagraphUnit => fileComparisonFileParagraphUnit.Value != null ? 1 : 0);


                xmlTxtWriter.WriteStartElement("files");               
                xmlTxtWriter.WriteAttributeString("xml:space", "preserve");
                xmlTxtWriter.WriteAttributeString("count", fileComparisonFileParagraphUnits.Count.ToString());
                xmlTxtWriter.WriteAttributeString("filesCompared", filesCompared.ToString());
                xmlTxtWriter.WriteAttributeString("filesWithErrors", (fileComparisonFileParagraphUnits.Count - filesCompared).ToString());
                xmlTxtWriter.WriteAttributeString("date", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            

                foreach (var fileComparisonFileParagraphUnit in fileComparisonFileParagraphUnits)
                {
                    #region  |  file  |


                    var filteredContentChanges = 0;
                    var filteredStatusChanges = 0;
                    var filteredComments = 0;
                    var filteredSegments = 0;
                    var filteredParagraphs = 0;


                    if (fileComparisonFileParagraphUnit.Value != null)
                    {
                        foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                        {
                            foreach (var comparisonParagraphUnit in fileComparisonParagraphUnit.Value.Values)
                            {

                                #region  |  filtered segments count  |

                                if ((Processor.Settings.ReportFilterChangedTargetContent ||
                                     Processor.Settings.ReportFilterSegmentStatusChanged ||
                                     Processor.Settings.ReportFilterSegmentsContainingComments) &&
                                    (!Processor.Settings.ReportFilterChangedTargetContent ||
                                     !comparisonParagraphUnit.ParagraphIsUpdated) &&
                                    (!Processor.Settings.ReportFilterSegmentStatusChanged ||
                                     !comparisonParagraphUnit.ParagraphStatusChanged) &&
                                    (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                                     !comparisonParagraphUnit.ParagraphHasComments)) continue;
                                var filteredAsegment = false;
                                foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                                {
                                    if ((Processor.Settings.ReportFilterChangedTargetContent ||
                                         Processor.Settings.ReportFilterSegmentStatusChanged ||
                                         Processor.Settings.ReportFilterSegmentsContainingComments) &&
                                        (!Processor.Settings.ReportFilterChangedTargetContent ||
                                         !comparisonSegmentUnit.SegmentTextUpdated) &&
                                        (!Processor.Settings.ReportFilterSegmentStatusChanged ||
                                         !comparisonSegmentUnit.SegmentSegmentStatusUpdated) &&
                                        (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                                         !comparisonSegmentUnit.SegmentHasComments)) continue;
                                    filteredContentChanges += comparisonSegmentUnit.SegmentTextUpdated ? 1 : 0;
                                    filteredStatusChanges += comparisonSegmentUnit.SegmentSegmentStatusUpdated ? 1 : 0;
                                    filteredComments += comparisonSegmentUnit.Comments != null ? comparisonSegmentUnit.Comments.Count : 0;

                                    filteredAsegment = true;
                                    filteredSegments++;
                                }

                                if (filteredAsegment)
                                    filteredParagraphs++;

                                #endregion
                            }
                        }
                    }




                    var fileUnitProperties = fileComparisonFileParagraphUnit.Key;

                    var reportFilterFilesWithNoRecordsFilteredCheck = true;
                    if (!Processor.Settings.ReportFilterFilesWithNoRecordsFiltered)
                    {
                        if ((filteredSegments == 0)
                            || (fileUnitProperties.TotalContentChanges == 0
                            && fileUnitProperties.TotalStatusChanges == 0
                            && fileUnitProperties.TotalComments == 0))
                            reportFilterFilesWithNoRecordsFilteredCheck = false;
                    }

                    if (!reportFilterFilesWithNoRecordsFilteredCheck) continue;
                    {
                        xmlTxtWriter.WriteStartElement("file");
                        #region  |  file attributes  |
                        xmlTxtWriter.WriteAttributeString("filePathOriginal", fileUnitProperties.FilePathOriginal);
                        xmlTxtWriter.WriteAttributeString("filePathUpdated", fileUnitProperties.FilePathUpdated);
                        xmlTxtWriter.WriteAttributeString("totalSegments", fileUnitProperties.TotalSegments.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSegmentsOriginal", fileUnitProperties.TotalSegmentsOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSegmentsUpdated", fileUnitProperties.TotalSegmentsUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalContentChanges", fileUnitProperties.TotalContentChanges.ToString());
                        xmlTxtWriter.WriteAttributeString("totalContentChangesPercentage", fileUnitProperties.TotalContentChangesPercentage.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalStatusChanges", fileUnitProperties.TotalStatusChanges.ToString());
                        xmlTxtWriter.WriteAttributeString("totalStatusChangesPercentage", fileUnitProperties.TotalStatusChangesPercentage.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalComments", fileUnitProperties.TotalComments.ToString());

                        xmlTxtWriter.WriteAttributeString("filteredParagraphs", filteredParagraphs.ToString());
                        xmlTxtWriter.WriteAttributeString("filteredSegments", filteredSegments.ToString());
                        xmlTxtWriter.WriteAttributeString("filteredContentChanges", filteredContentChanges.ToString());
                        xmlTxtWriter.WriteAttributeString("filteredStatusChanges", filteredStatusChanges.ToString());
                        xmlTxtWriter.WriteAttributeString("filteredComments", filteredComments.ToString());


                        xmlTxtWriter.WriteAttributeString("totalNotTranslatedOriginal", fileUnitProperties.TotalNotTranslatedOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalDraftOriginal", fileUnitProperties.TotalDraftOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslatedOriginal", fileUnitProperties.TotalTranslatedOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationRejectedOriginal", fileUnitProperties.TotalTranslationRejectedOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationApprovedOriginal", fileUnitProperties.TotalTranslationApprovedOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSignOffRejectedOriginal", fileUnitProperties.TotalSignOffRejectedOriginal.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSignedOffOriginal", fileUnitProperties.TotalSignedOffOriginal.ToString());

                        xmlTxtWriter.WriteAttributeString("totalNotTranslatedUpdated", fileUnitProperties.TotalNotTranslatedUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalDraftUpdated", fileUnitProperties.TotalDraftUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslatedUpdated", fileUnitProperties.TotalTranslatedUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationRejectedUpdated", fileUnitProperties.TotalTranslationRejectedUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationApprovedUpdated", fileUnitProperties.TotalTranslationApprovedUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSignOffRejectedUpdated", fileUnitProperties.TotalSignOffRejectedUpdated.ToString());
                        xmlTxtWriter.WriteAttributeString("totalSignedOffUpdated", fileUnitProperties.TotalSignedOffUpdated.ToString());


                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM", fileUnitProperties.TotalTranslationChangesPm.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM", fileUnitProperties.TotalTranslationChangesCm.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact", fileUnitProperties.TotalTranslationChangesExact.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT", fileUnitProperties.TotalTranslationChangesAt.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther", fileUnitProperties.TotalTranslationChangesOther.ToString());
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal",
                        (fileUnitProperties.TotalTranslationChangesPm
                         + fileUnitProperties.TotalTranslationChangesCm
                         + fileUnitProperties.TotalTranslationChangesExact
                         + fileUnitProperties.TotalTranslationChangesAt
                         + fileUnitProperties.TotalTranslationChangesOther).ToString());



                        #region  |  words  |

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Words", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmWords).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Words", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmWords).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Words", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactWords).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Words", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtWords).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Words", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherWords).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Words", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmWords
                            + fileUnitProperties.TotalTranslationChangesCmWords
                            + fileUnitProperties.TotalTranslationChangesExactWords
                            + fileUnitProperties.TotalTranslationChangesAtWords
                            + fileUnitProperties.TotalTranslationChangesOtherWords).ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Words_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmWordsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Words_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmWordsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Words_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactWordsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Words_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtWordsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Words_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherWordsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Words_new", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmWordsNew
                            + fileUnitProperties.TotalTranslationChangesCmWordsNew
                            + fileUnitProperties.TotalTranslationChangesExactWordsNew
                            + fileUnitProperties.TotalTranslationChangesAtWordsNew
                            + fileUnitProperties.TotalTranslationChangesOtherWordsNew).ToString(CultureInfo.InvariantCulture));


                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Words_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmWordsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Words_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmWordsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Words_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactWordsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Words_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtWordsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Words_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherWordsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Words_removed", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmWordsRemoved
                            + fileUnitProperties.TotalTranslationChangesCmWordsRemoved
                            + fileUnitProperties.TotalTranslationChangesExactWordsRemoved
                            + fileUnitProperties.TotalTranslationChangesAtWordsRemoved
                            + fileUnitProperties.TotalTranslationChangesOtherWordsRemoved).ToString(CultureInfo.InvariantCulture));

                        #endregion

                        #region  |  characters  |

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Characters", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmCharacters).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Characters", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmCharacters).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Characters", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactCharacters).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Characters", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtCharacters).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Characters", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherCharacters).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Characters", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmCharacters
                            + fileUnitProperties.TotalTranslationChangesCmCharacters
                            + fileUnitProperties.TotalTranslationChangesExactCharacters
                            + fileUnitProperties.TotalTranslationChangesAtCharacters
                            + fileUnitProperties.TotalTranslationChangesOtherCharacters).ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Characters_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmCharactersNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Characters_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmCharactersNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Characters_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactCharactersNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Characters_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtCharactersNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Characters_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherCharactersNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Characters_new", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmCharactersNew
                            + fileUnitProperties.TotalTranslationChangesCmCharactersNew
                            + fileUnitProperties.TotalTranslationChangesExactCharactersNew
                            + fileUnitProperties.TotalTranslationChangesAtCharactersNew
                            + fileUnitProperties.TotalTranslationChangesOtherCharactersNew).ToString(CultureInfo.InvariantCulture));


                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Characters_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Characters_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Characters_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Characters_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Characters_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Characters_removed", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmCharactersRemoved
                            + fileUnitProperties.TotalTranslationChangesCmCharactersRemoved
                            + fileUnitProperties.TotalTranslationChangesExactCharactersRemoved
                            + fileUnitProperties.TotalTranslationChangesAtCharactersRemoved
                            + fileUnitProperties.TotalTranslationChangesOtherCharactersRemoved).ToString(CultureInfo.InvariantCulture));

                        #endregion

                        #region  |  Tags  |

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Tags", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmTags).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Tags", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmTags).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Tags", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactTags).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Tags", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtTags).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Tags", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherTags).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Tags", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmTags
                            + fileUnitProperties.TotalTranslationChangesCmTags
                            + fileUnitProperties.TotalTranslationChangesExactTags
                            + fileUnitProperties.TotalTranslationChangesAtTags
                            + fileUnitProperties.TotalTranslationChangesOtherTags).ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Tags_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmTagsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Tags_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmTagsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Tags_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactTagsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Tags_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtTagsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Tags_new", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherTagsNew).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Tags_new", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmTagsNew
                            + fileUnitProperties.TotalTranslationChangesCmTagsNew
                            + fileUnitProperties.TotalTranslationChangesExactTagsNew
                            + fileUnitProperties.TotalTranslationChangesAtTagsNew
                            + fileUnitProperties.TotalTranslationChangesOtherTagsNew).ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesPM_Tags_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesPmTagsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesCM_Tags_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesCmTagsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesExact_Tags_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesExactTagsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesAT_Tags_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesAtTagsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesOther_Tags_removed", decimal.Truncate(fileUnitProperties.TotalTranslationChangesOtherTagsRemoved).ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("totalTranslationChangesTotal_Tags_removed", decimal.Truncate(
                            fileUnitProperties.TotalTranslationChangesPmTagsRemoved
                            + fileUnitProperties.TotalTranslationChangesCmTagsRemoved
                            + fileUnitProperties.TotalTranslationChangesExactTagsRemoved
                            + fileUnitProperties.TotalTranslationChangesAtTagsRemoved
                            + fileUnitProperties.TotalTranslationChangesOtherTagsRemoved).ToString(CultureInfo.InvariantCulture));


                        #endregion

                        #endregion



            

                        if (fileComparisonFileParagraphUnit.Value == null)
                        {
                            xmlTxtWriter.WriteStartElement("innerFiles");
                            xmlTxtWriter.WriteAttributeString("count", "0");

                            xmlTxtWriter.WriteEndElement();//innerFiles
                        }
                        else
                        {
                            #region  |  innerFiles |


                            #region  |  counters  |

                            var fileFilteredInnerFiles = 0;
                            foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                            {
                                var comparisonParagraphUnits = fileComparisonParagraphUnit.Value;

                                var testInnerFileFilteredParagraphCount = 0;

                                #region  |  counters  |

                                foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                                {

                                    var filteredAsegment = false;
                                    foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                                    {
                                        if (
                                            (!Processor.Settings.ReportFilterChangedTargetContent && !Processor.Settings.ReportFilterSegmentStatusChanged
                                             && !Processor.Settings.ReportFilterSegmentsContainingComments)
                                            ||
                                            (Processor.Settings.ReportFilterChangedTargetContent && comparisonSegmentUnit.SegmentTextUpdated)
                                            || (Processor.Settings.ReportFilterSegmentStatusChanged && comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                            || (Processor.Settings.ReportFilterSegmentsContainingComments && comparisonSegmentUnit.SegmentHasComments)

                                        )
                                        {
                                            filteredAsegment = true;                                            
                                        }
                                    }

                                    if (filteredAsegment)
                                        testInnerFileFilteredParagraphCount++;
                                }
                                #endregion

                                if (testInnerFileFilteredParagraphCount > 0)
                                    fileFilteredInnerFiles++;
                            }
                            #endregion

                            xmlTxtWriter.WriteStartElement("innerFiles");
                            xmlTxtWriter.WriteAttributeString("count", fileFilteredInnerFiles.ToString());


                            foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
                            {

                                var comparisonParagraphUnits = fileComparisonParagraphUnit.Value;

                                #region  |  innerFile  |

                                var innerFileFilteredParagraphCount = 0;
                                var innerFileFilteredSegmentCount = 0;


                                #region  |  counters  |

                                foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                                {

                                    var filteredAsegment = false;
                                    foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                                    {
                                        if ((Processor.Settings.ReportFilterChangedTargetContent ||
                                             Processor.Settings.ReportFilterSegmentStatusChanged ||
                                             Processor.Settings.ReportFilterSegmentsContainingComments) &&
                                            (!Processor.Settings.ReportFilterChangedTargetContent ||
                                             !comparisonSegmentUnit.SegmentTextUpdated) &&
                                            (!Processor.Settings.ReportFilterSegmentStatusChanged ||
                                             !comparisonSegmentUnit.SegmentSegmentStatusUpdated) &&
                                            (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                                             !comparisonSegmentUnit.SegmentHasComments)) continue;
                                        filteredAsegment = true;
                                        innerFileFilteredSegmentCount++;
                                    }

                                    if (filteredAsegment)
                                        innerFileFilteredParagraphCount++;
                                }
                                #endregion

                                if (innerFileFilteredParagraphCount > 0)
                                {
                                    xmlTxtWriter.WriteStartElement("innerFile");
                                    xmlTxtWriter.WriteAttributeString("showInnerFileName", Processor.Settings.IncludeIndividualFileInformation.ToString().ToLower());
                                    xmlTxtWriter.WriteAttributeString("name", Path.GetFileName(fileComparisonParagraphUnit.Key));
                                    xmlTxtWriter.WriteAttributeString("filteredParagraphCount", innerFileFilteredParagraphCount.ToString());
                                    xmlTxtWriter.WriteAttributeString("filteredSegmentCount", innerFileFilteredSegmentCount.ToString());



                                    xmlTxtWriter.WriteStartElement("paragraphs");
                                    xmlTxtWriter.WriteAttributeString("count", innerFileFilteredParagraphCount.ToString());

                                    foreach (var comparisonParagraphUnit in comparisonParagraphUnits.Values)
                                    {
                                        if ((Processor.Settings.ReportFilterChangedTargetContent ||
                                             Processor.Settings.ReportFilterSegmentStatusChanged ||
                                             Processor.Settings.ReportFilterSegmentsContainingComments) &&
                                            (!Processor.Settings.ReportFilterChangedTargetContent ||
                                             !comparisonParagraphUnit.ParagraphIsUpdated) &&
                                            (!Processor.Settings.ReportFilterSegmentStatusChanged ||
                                             !comparisonParagraphUnit.ParagraphStatusChanged) &&
                                            (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                                             !comparisonParagraphUnit.ParagraphHasComments)) continue;

                                        #region  |  paragraph  |
                                        xmlTxtWriter.WriteStartElement("paragraph");
                                        xmlTxtWriter.WriteAttributeString("paragraphId", comparisonParagraphUnit.ParagraphId);

                                        #region  |  segments  |


                                        var paragraphFilteredSegmentCount = comparisonParagraphUnit.ComparisonSegmentUnits.Count(comparisonSegmentUnit => 
                                            (!Processor.Settings.ReportFilterChangedTargetContent && !Processor.Settings.ReportFilterSegmentStatusChanged && !Processor.Settings.ReportFilterSegmentsContainingComments) 
                                            || (Processor.Settings.ReportFilterChangedTargetContent && comparisonSegmentUnit.SegmentTextUpdated) 
                                            || (Processor.Settings.ReportFilterSegmentStatusChanged && comparisonSegmentUnit.SegmentSegmentStatusUpdated) 
                                            || (Processor.Settings.ReportFilterSegmentsContainingComments && comparisonSegmentUnit.SegmentHasComments));
                                        #region  |  counters  |

                                        #endregion


                                        xmlTxtWriter.WriteStartElement("segments");
                                        xmlTxtWriter.WriteAttributeString("count", paragraphFilteredSegmentCount.ToString());

                                        foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                                        {
                                            #region  |  segment  |

                                            if ((Processor.Settings.ReportFilterChangedTargetContent ||
                                                 Processor.Settings.ReportFilterSegmentStatusChanged ||
                                                 Processor.Settings.ReportFilterSegmentsContainingComments) &&
                                                (!Processor.Settings.ReportFilterChangedTargetContent ||
                                                 !comparisonSegmentUnit.SegmentTextUpdated) &&
                                                (!Processor.Settings.ReportFilterSegmentStatusChanged ||
                                                 !comparisonSegmentUnit.SegmentSegmentStatusUpdated) &&
                                                (!Processor.Settings.ReportFilterSegmentsContainingComments ||
                                                 !comparisonSegmentUnit.SegmentHasComments)) continue;
                                            xmlTxtWriter.WriteStartElement("segment");
                                            xmlTxtWriter.WriteAttributeString("segmentId", comparisonSegmentUnit.SegmentId);

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
                                                if (xSegmentSection.IsText)
                                                {
                                                    var strList = GetTextSections(xSegmentSection.Content);
                                                    foreach (var str in strList)
                                                    {
                                                        xmlTxtWriter.WriteStartElement("span");
                                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                                        xmlTxtWriter.WriteString(str);
                                                        xmlTxtWriter.WriteEndElement();//span    
                                                    }
                                                }
                                                else
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "tag");
                                                    xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                    xmlTxtWriter.WriteEndElement();//span                                    
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

                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "text");
                                                xmlTxtWriter.WriteString(GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusUpdated));
                                                xmlTxtWriter.WriteEndElement();//span 
                                            }
                                            else
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "textRemoved");
                                                xmlTxtWriter.WriteString(GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusOriginal));
                                                xmlTxtWriter.WriteEndElement();//span 

                                                xmlTxtWriter.WriteStartElement("br");
                                                xmlTxtWriter.WriteEndElement();//br 

                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "textNew");
                                                xmlTxtWriter.WriteString(GetVisualSegmentStatus(comparisonSegmentUnit.SegmentStatusUpdated));
                                                xmlTxtWriter.WriteEndElement();//span 
                                            }

                                            xmlTxtWriter.WriteEndElement();//segmentStatus




                                            #endregion

                                            #region  |  translationMatchType  |



                                            xmlTxtWriter.WriteStartElement("translationMatchType");
                                            xmlTxtWriter.WriteStartElement("td");

                                            var matchColor = GetMatchColor(comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationOriginTypeOriginal);
                                            xmlTxtWriter.WriteAttributeString("style", "text-align: center" + (matchColor != string.Empty ? ";background-color: " + matchColor : string.Empty));


                                            if (string.Compare(comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated, StringComparison.OrdinalIgnoreCase) == 0)
                                            {


                                                if (comparisonSegmentUnit.TranslationStatusOriginal.Trim() != string.Empty)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "text");
                                                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusOriginal);
                                                    xmlTxtWriter.WriteEndElement();//span                                                                
                                                }
                                            }
                                            else
                                            {
                                                if (comparisonSegmentUnit.TranslationStatusOriginal.Trim() != string.Empty)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "textRemoved");
                                                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusOriginal);
                                                    xmlTxtWriter.WriteEndElement();//span 

                                                    if (comparisonSegmentUnit.TranslationStatusUpdated.Trim() != string.Empty)
                                                    {
                                                        xmlTxtWriter.WriteStartElement("br");
                                                        xmlTxtWriter.WriteEndElement();//br 
                                                    }
                                                }



                                                if (comparisonSegmentUnit.TranslationStatusUpdated.Trim() != string.Empty)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "textNew");
                                                    xmlTxtWriter.WriteString(comparisonSegmentUnit.TranslationStatusUpdated);
                                                    xmlTxtWriter.WriteEndElement();//span 
                                                }


                                            }

                                            xmlTxtWriter.WriteEndElement();//td 
                                            xmlTxtWriter.WriteEndElement();//translationMatchType




                                            #endregion

                                            #region  |  target (original)  |

                                            xmlTxtWriter.WriteStartElement("targetOriginal");
                                            foreach (var xSegmentSection in comparisonSegmentUnit.TargetOriginal)
                                            {
                                                if (xSegmentSection.IsText)
                                                {
                                                    var strList = GetTextSections(xSegmentSection.Content);
                                                    foreach (var str in strList)
                                                    {
                                                        xmlTxtWriter.WriteStartElement("span");
                                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                                        xmlTxtWriter.WriteString(str);
                                                        xmlTxtWriter.WriteEndElement();//span    
                                                    }
                                                }
                                                else
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "tag");
                                                    xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                    xmlTxtWriter.WriteEndElement();//span                                    
                                                }
                                            }
                                            xmlTxtWriter.WriteEndElement();//targetOriginal


                                            #endregion

                                            #region  |  target (updated)  |


                                            xmlTxtWriter.WriteStartElement("targetUpdated");
                                            foreach (var xSegmentSection in comparisonSegmentUnit.TargetUpdated)
                                            {
                                                if (xSegmentSection.IsText)
                                                {
                                                    var strList = GetTextSections(xSegmentSection.Content);
                                                    foreach (var str in strList)
                                                    {
                                                        xmlTxtWriter.WriteStartElement("span");
                                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                                        xmlTxtWriter.WriteString(str);
                                                        xmlTxtWriter.WriteEndElement();//span    
                                                    }
                                                }
                                                else
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "tag");
                                                    xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                    xmlTxtWriter.WriteEndElement();//span                                    
                                                }
                                            }
                                            xmlTxtWriter.WriteEndElement();//targetUpdated




                                            #endregion

                                            #region  |  target comparison  |
                                            xmlTxtWriter.WriteStartElement("targetComparison");


                                            foreach (var comparisonTextUnit in comparisonSegmentUnit.ComparisonTextUnits)
                                            {
                                                if (comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.Identical)
                                                {
                                                    foreach (var xSegmentSection in comparisonTextUnit.TextSections)
                                                    {
                                                        if (xSegmentSection.IsText)
                                                        {
                                                            xmlTxtWriter.WriteStartElement("span");
                                                            xmlTxtWriter.WriteAttributeString("class", "text");
                                                            xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                            xmlTxtWriter.WriteEndElement();//span
                                                        }
                                                        else
                                                        {
                                                            xmlTxtWriter.WriteStartElement("span");
                                                            xmlTxtWriter.WriteAttributeString("class", "tag");

                                                            xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                            xmlTxtWriter.WriteEndElement();//span
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (var xSegmentSection in comparisonTextUnit.TextSections)
                                                    {
                                                        if (xSegmentSection.IsText)
                                                        {
                                                            xmlTxtWriter.WriteStartElement("span");
                                                            xmlTxtWriter.WriteAttributeString("class", comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New ? "textNew" : "textRemoved");

                                                            xmlTxtWriter.WriteString(xSegmentSection.Content);

                                                            xmlTxtWriter.WriteEndElement();//span
                                                        }
                                                        else
                                                        {

                                                            xmlTxtWriter.WriteStartElement("span");
                                                            xmlTxtWriter.WriteAttributeString("class", comparisonTextUnit.ComparisonTextUnitType == TextComparer.ComparisonTextUnitType.New ? "tagNew" : "tagRemoved");

                                                            xmlTxtWriter.WriteString(xSegmentSection.Content);
                                                            xmlTxtWriter.WriteEndElement();//span
                                                        }
                                                    }
                                                }
                                            }
                                            xmlTxtWriter.WriteEndElement();//targetComparison

                                            #endregion

                                            #region  |  translationChangedWords  |

                                            xmlTxtWriter.WriteStartElement("translationChangedWords_added");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsWordsNew).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedWords


                                            xmlTxtWriter.WriteStartElement("translationChangedWords_removed");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsWordsRemoved).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedWords

                                            #endregion

                                            #region  |  translationChangedCharacters  |

                                            xmlTxtWriter.WriteStartElement("translationChangedCharacters_added");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsCharactersNew).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedCharacters



                                            xmlTxtWriter.WriteStartElement("translationChangedCharacters_removed");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsCharactersRemoved).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedCharacters


                                            #endregion

                                            #region  |  translationChangedTags  |

                                            xmlTxtWriter.WriteStartElement("translationChangedTags_added");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsTagsNew).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedTags



                                            xmlTxtWriter.WriteStartElement("translationChangedTags_removed");
                                            xmlTxtWriter.WriteString(decimal.Truncate(comparisonSegmentUnit.TranslationSectionsTagsRemoved).ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//translationChangedTags


                                            #endregion

                                            #region  |  translationChangedPercentage  |

                                            decimal p1 = 100;
                                            decimal p2 = 0;
                                            var totalCharacters = comparisonSegmentUnit.TranslationSectionsCharacters + comparisonSegmentUnit.TranslationSectionsTags;
                                            var weightedChanges = totalCharacters + comparisonSegmentUnit.TranslationSectionsChangedCharacters + comparisonSegmentUnit.TranslationSectionsChangedTags;
                                            if (totalCharacters > 0 && weightedChanges > 0)
                                            {
                                                p1 = Math.Round(totalCharacters / weightedChanges * 100, 2);
                                                p2 = Math.Round(100 - p1, 2);
                                            }

                                            xmlTxtWriter.WriteStartElement("translationChangedPercentage_01");
                                            xmlTxtWriter.WriteString(p1.ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//ModificationPercentage

                                            xmlTxtWriter.WriteStartElement("translationChangedPercentage_02");
                                            xmlTxtWriter.WriteString(p2.ToString(CultureInfo.InvariantCulture));
                                            xmlTxtWriter.WriteEndElement();//ModificationPercentage


                                            #endregion

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

                                            xmlTxtWriter.WriteEndElement();//segment

                                            #endregion
                                        }
                                        xmlTxtWriter.WriteEndElement();//segments
                                        #endregion


                                        xmlTxtWriter.WriteEndElement();//paragraph


                                        #endregion
                                    }
                                    xmlTxtWriter.WriteEndElement();//paragraphs

                                    xmlTxtWriter.WriteEndElement();//innerFile
                                }

                                #endregion


                            }

                            xmlTxtWriter.WriteEndElement();//innerFiles

                            #endregion

                        }

                        xmlTxtWriter.WriteEndElement();//file
                    }

                    #endregion
                }


                xmlTxtWriter.WriteEndElement();//files

                #endregion


                xmlTxtWriter.WriteEndDocument();
                xmlTxtWriter.Flush();
                xmlTxtWriter.Close();

                WriteReportResourcesToDirectory(Path.GetDirectoryName(reportFilePath));

                if (transformReport)
                    TransformXmlReport(reportFilePath);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static IEnumerable<string> GetTextSections(string str)
        {
            var strList = new List<string>();

            var regexDoubleSpaces = new Regex(@"\s{2,}", RegexOptions.Singleline);
            var mcRegexDoubleSpaces = regexDoubleSpaces.Matches(str);


            var previousStart = 0;
            foreach (Match mRegexDoubleSpaces in mcRegexDoubleSpaces)
            {
                if (mRegexDoubleSpaces.Index > previousStart)
                {
                    var startText = str.Substring(previousStart, mRegexDoubleSpaces.Index - previousStart);
                    if (startText.Length > 0)
                        strList.Add(startText);
                }


                var tagText = mRegexDoubleSpaces.Value.Replace(" ", ((char)160).ToString());
                if (tagText.Length > 0)
                    strList.Add(tagText);


                previousStart = mRegexDoubleSpaces.Index + mRegexDoubleSpaces.Length;

            }

            var endText = str.Substring(previousStart);
            if (endText.Length > 0)
                strList.Add(endText);


            return strList;
        }
        private static string GetVisualSegmentStatus(string segmentStatusId)
        {
            switch (segmentStatusId)
            {
                case "Unspecified": return "Not Translated";
                case "Draft": return "Draft";
                case "Translated": return "Translated";
                case "RejectedTranslation": return "Translation Rejected";
                case "ApprovedTranslation": return "Translation Approved";
                case "RejectedSignOff": return "Sign-off Rejected";
                case "ApprovedSignOff": return "Signed Off";
                default: return "Unknown";
            }
        }

        private static string GetMatchColor(string match, string originType)
        {
            
            var color = string.Empty;

            if (string.Compare(originType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                color = "#D3FF4F";
            else if (string.Compare(originType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                color = string.Empty;
            else
            {
                switch (match)
                {
                    case "PM": color = "#DFBFFF;"; break;
                    case "CM": color = "#B3ECB3"; break;
                    case "AT": color = "#00B8F4"; break;               
                    case "100%": color = "#B3ECB3"; break;                  
                    default:
                        {
                            if (match.Trim() != string.Empty)
                            {
                                color = string.Empty;
                            }
                        } break;
                }
            }

         

            return color;
        }


        private static void TransformXmlReport(string reportFilePath)
        {
            
                var filePathXslt = Path.Combine(Path.GetDirectoryName(reportFilePath), DefaultXsltName);

                var myXPathDoc = new XPathDocument(reportFilePath);
                var myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(filePathXslt);
                var myWriter = new XmlTextWriter(reportFilePath + ".html", Encoding.UTF8);
                myXslTrans.Transform(myXPathDoc, null, myWriter);

            
                myWriter.Flush();
                myWriter.Close();

            


                File.Delete(filePathXslt);
            
            File.Delete(reportFilePath);

        }
        
        private void WriteReportResourcesToDirectory(string reportDirectory)
        {

            var filePathXslt = Path.Combine(reportDirectory, DefaultXsltName);

            if (Processor.Settings.UseCustomStyleSheet
                && File.Exists(Processor.Settings.FilePathCustomStyleSheet))
            {
                #region  |  custom report  |
                if (string.Compare(filePathXslt, Processor.Settings.FilePathCustomStyleSheet, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    try
                    {
                        File.Copy(Processor.Settings.FilePathCustomStyleSheet, filePathXslt, true);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to write the custom xlst file to the report directory!\r\n" + ex.Message);
                    }
                }
                #endregion
            }
            else
            {
                #region  |  default report  |

                var assembly = Assembly.GetExecutingAssembly();

                
                 var   templateXsltName = "SdlXliffCompare.Core.Reports.SdlXliffCompare.StyleSheet.01.xslt";
             
                using (var inputStream = assembly.GetManifestResourceStream(templateXsltName))
                {

                    Stream outputStream = File.Open(filePathXslt, FileMode.Create);

                    if (inputStream != null)
                    {
                        var bsInput = new BufferedStream(inputStream);
                        var bsOutput = new BufferedStream(outputStream);

                        var buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
                        {
                            bsOutput.Write(buffer, 0, bytesRead);
                        }

                        bsInput.Flush();
                        bsOutput.Flush();
                        bsInput.Close();
                        bsOutput.Close();
                    }
                }
                #endregion
            }

            UdpateStyleSheetInformation(filePathXslt);

        }

        private const string DefaultXsltName = "SdlXliffCompare.StyleSheet.xslt";


        #region  |  Udpate Style Sheet Information  |

        private readonly Regex _regexStyle = new Regex(@"(?<x1>\<style\s+[^\>]*\>)(?<x2>.*?)(?<x3>\<\/style\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexNewText = new Regex(@"(?<x1>span\.textNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexRemovedText = new Regex(@"(?<x1>span\.textRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexNewTag = new Regex(@"(?<x1>span\.tagNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexRemovedTag = new Regex(@"(?<x1>span\.tagRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private void UdpateStyleSheetInformation(string filePathXslt)
        {

            var str = string.Empty;
            using (var sr = new StreamReader(filePathXslt, Encoding.UTF8))
            {
                str = sr.ReadToEnd();
                sr.Close();
            }

            str = _regexStyle.Replace(str, MatchEvaluator_style);

            using (var sw = new StreamWriter(filePathXslt, false, Encoding.UTF8))
            {
                sw.Write(str);
                sw.Flush();
                sw.Close();
            }
        }
       
        private string MatchEvaluator_style(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;

            str2 = _regexNewText.Replace(str2, MatchEvaluator_new_text);
            str2 = _regexRemovedText.Replace(str2, MatchEvaluator_removed_text);
            str2 = _regexNewTag.Replace(str2, MatchEvaluator_new_tag);
            str2 = _regexRemovedTag.Replace(str2, MatchEvaluator_removed_tag);

            return str1 + str2 + str3;
        }

        private string MatchEvaluator_new_text(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleNewText);   
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_removed_text(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleRemovedText);
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_new_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleNewTag);
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_removed_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(Processor.Settings.StyleRemovedTag);
            return str1 + str2 + str3;
        }

        private static string GetHtmlStyleText(Settings.DifferencesFormatting style)
        {
            var str = string.Empty;

            if (style.FontSpecifyColor)
                str += "color: " + style.FontColor + "; ";
            if (style.FontSpecifyBackroundColor)
                str += "background-color: " + style.FontBackroundColor + "; ";

            if (style.StyleBold == "Activate")
                str += "font-weight: bold; ";
            if (style.StyleItalic == "Activate")
                str += "font-style: italic; ";
            if (style.StyleStrikethrough == "Activate")
                str += "text-decoration: line-through; ";
            if (style.StyleUnderline == "Activate")
                str += "text-decoration: underline; ";

            if (string.Compare(style.TextPosition, "Superscript", StringComparison.OrdinalIgnoreCase) == 0)          
                str += "vertical-align: super; ";
            else if  (string.Compare(style.TextPosition, "Subscript", StringComparison.OrdinalIgnoreCase) == 0)          
                str += "vertical-align: sub; ";
            

            return str;
        }

        #endregion
    }
}
