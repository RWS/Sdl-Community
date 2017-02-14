using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Sdl.Community.Comparison;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Profile;
using Sdl.Community.Structures.Projects;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.Structures.Rates.Helpers;

namespace Sdl.Community.Report
{
    public class Processor
    {
        public void CreateTrackChangesReport(string applicationTrackChangesReportPath, string xmlFileFullPath
            , List<DocumentActivity> documentActivities
            , Activity tpa
            , CompanyProfile cpi)
        {
            _createTrackChangesReport(applicationTrackChangesReportPath, xmlFileFullPath, documentActivities, tpa, cpi);
        }
        private void _createTrackChangesReport(string applicationTrackChangesReportPath
            , string xmlFileFullPath, List<DocumentActivity> documentActivities
            , Activity tpa
            , CompanyProfile cpi)
        {
            try
            {
                #region  |  styles  |
                StyleNewText = new DifferencesFormatting();
                StyleRemovedText = new DifferencesFormatting();
                StyleNewTag = new DifferencesFormatting();
                StyleRemovedTag = new DifferencesFormatting();
                if (cpi != null)
                {
                    StyleNewText = cpi.ComparerOptions.StyleNewText;
                    StyleRemovedText = cpi.ComparerOptions.StyleRemovedText;
                    StyleNewTag = cpi.ComparerOptions.StyleNewTag;
                    StyleRemovedTag = cpi.ComparerOptions.StyleRemovedTag;
                }
                else
                {

                    #region  |  new text style  |

                    StyleNewText = new DifferencesFormatting
                    {
                        StyleBold = "Deactivate",
                        StyleItalic = "Deactivate",
                        StyleStrikethrough = "Deactivate",
                        StyleUnderline = "Activate",
                        TextPosition = "Normal",
                        FontSpecifyColor = true,
                        FontColor = "#0000FF",
                        FontSpecifyBackroundColor = true,
                        FontBackroundColor = "#FFFF66"
                    };

                    #endregion

                    #region  |  removed text style  |

                    StyleRemovedText = new DifferencesFormatting
                    {
                        StyleBold = "Deactivate",
                        StyleItalic = "Deactivate",
                        StyleStrikethrough = "Activate",
                        StyleUnderline = "Deactivate",
                        TextPosition = "Normal",
                        FontSpecifyColor = true,
                        FontColor = "#FF0000",
                        FontSpecifyBackroundColor = false,
                        FontBackroundColor = "#FFFFFF"
                    };

                    #endregion

                    #region  |  new tag style  |

                    StyleNewTag = new DifferencesFormatting
                    {
                        StyleBold = "Deactivate",
                        StyleItalic = "Deactivate",
                        StyleStrikethrough = "Deactivate",
                        StyleUnderline = "Deactivate",
                        TextPosition = "Normal",
                        FontSpecifyColor = false,
                        FontColor = "#000000",
                        FontSpecifyBackroundColor = true,
                        FontBackroundColor = "#DDEEFF"
                    };

                    #endregion

                    #region  |  removed tag style  |

                    StyleRemovedTag = new DifferencesFormatting
                    {
                        StyleBold = "Deactivate",
                        StyleItalic = "Deactivate",
                        StyleStrikethrough = "Deactivate",
                        StyleUnderline = "Deactivate",
                        TextPosition = "Normal",
                        FontSpecifyColor = false,
                        FontColor = "#000000",
                        FontSpecifyBackroundColor = true,
                        FontBackroundColor = "#FFE8E8"
                    };

                    #endregion
                }
                #endregion


                const string xsltName = "Report.StyleSheet.01.xslt";


                var tcc = new TextComparer { Type = TextComparer.ComparisonType.Words };

                var xmlTxtWriter = new XmlTextWriter(xmlFileFullPath, Encoding.UTF8)
                {
                    Formatting = Formatting.None,
                    Indentation = 3,
                    Namespaces = false
                };

                xmlTxtWriter.WriteStartDocument(true);


                xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + xsltName + "'");
                xmlTxtWriter.WriteComment("Qualitivity by Patrick Hartnett, 2015");

                xmlTxtWriter.WriteStartElement("documents");
                xmlTxtWriter.WriteAttributeString("xml:space", "preserve");


                var cultureInfoIds = new List<string>();

                foreach (var documentActivity in documentActivities)
                {
                    if (!cultureInfoIds.Contains(documentActivity.TranslatableDocument.SourceLanguage))
                        cultureInfoIds.Add(documentActivity.TranslatableDocument.SourceLanguage);
                    if (!cultureInfoIds.Contains(documentActivity.TranslatableDocument.TargetLanguage))
                        cultureInfoIds.Add(documentActivity.TranslatableDocument.TargetLanguage);
                }
                if (!cultureInfoIds.Contains("empty"))
                    cultureInfoIds.Add("empty");

                InitializeWriteFlagsFolder(applicationTrackChangesReportPath, cultureInfoIds);


                var documentActivitiesDictionary = new Dictionary<string, List<DocumentActivity>>();

                foreach (var documentActivity in documentActivities)
                    if (!documentActivitiesDictionary.ContainsKey(documentActivity.DocumentId))
                        documentActivitiesDictionary.Add(documentActivity.DocumentId, new List<DocumentActivity> { documentActivity });
                    else
                        documentActivitiesDictionary[documentActivity.DocumentId].Add(documentActivity);



                foreach (var kvp in documentActivitiesDictionary)
                {
                    xmlTxtWriter.WriteStartElement("document");
                    xmlTxtWriter.WriteAttributeString("activities", kvp.Value.Count.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("id", kvp.Key);


                    var mergedDocuments = new MergedDocuments(kvp.Value, tpa, tcc, null);




                    #region  |  document Total Attributes  |


                    xmlTxtWriter.WriteAttributeString("documentHasTrackChanges", mergedDocuments.DocumentHasTrackChanges.ToString().ToLower());


                    xmlTxtWriter.WriteAttributeString("documentTotalElapsedTime", string.Format("{0:00}:{1:00}:{2:00}", mergedDocuments.DocumentTotalElapsedTime.Hours, mergedDocuments.DocumentTotalElapsedTime.Minutes, mergedDocuments.DocumentTotalElapsedTime.Seconds));
                    xmlTxtWriter.WriteAttributeString("documentTotalElapsedHours", Math.Round(mergedDocuments.DocumentTotalElapsedTime.TotalHours, 3).ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentName", mergedDocuments.DocumentName);

                    xmlTxtWriter.WriteAttributeString("documentSourceLanguage", mergedDocuments.DocumentSourceLanguage);
                    xmlTxtWriter.WriteAttributeString("documentTargetlanguage", mergedDocuments.DocumentTargetLanguage);

                    var flagsPath = Path.Combine(applicationTrackChangesReportPath, "Flags");
                    var documentSourceLanguageFlagPath = Path.Combine(flagsPath, mergedDocuments.DocumentSourceLanguage + ".gif");
                    var documentTargetlanguageFlagPath = Path.Combine(flagsPath, mergedDocuments.DocumentTargetLanguage + ".gif");


                    try
                    {
                        var imageBase64Source = ImageToBase64(documentSourceLanguageFlagPath);
                        var imageBase64ElementSource = @"data:image/gif;base64, " + imageBase64Source;

                        var imageBase64Target = ImageToBase64(documentTargetlanguageFlagPath);
                        var imageBase64ElementTarget = @"data:image/gif;base64, " + imageBase64Target;

                        xmlTxtWriter.WriteAttributeString("documentSourceLanguageFlag", imageBase64ElementSource);
                        xmlTxtWriter.WriteAttributeString("documentTargetlanguageFlag", imageBase64ElementTarget);
                    }
                    catch
                    {
                        // ignored
                    }


                    xmlTxtWriter.WriteAttributeString("documentTotalSegments", mergedDocuments.TotalSourceSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentWords", mergedDocuments.TotalSourceWords.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentContentUpdatedWords", mergedDocuments.DocumentTotalSegmentContentUpdatedWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentStatusUpdatedWords", mergedDocuments.DocumentTotalSegmentStatusUpdatedWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentCommentsUpdatedWords", mergedDocuments.DocumentTotalSegmentCommentsUpdatedWords.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentContentUpdated", mergedDocuments.DocumentTotalSegmentContentUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentStatusUpdated", mergedDocuments.DocumentTotalSegmentStatusUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentCommentsUpdated", mergedDocuments.DocumentTotalSegmentCommentsUpdated.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentContentUpdatedPercentage", mergedDocuments.DocumentTotalSegmentContentUpdatedPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentStatusUpdatedPercentage", mergedDocuments.DocumentTotalSegmentStatusUpdatedPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalSegmentCommentsUpdatedPercentage", mergedDocuments.DocumentTotalSegmentCommentsUpdatedPercentage.ToString(CultureInfo.InvariantCulture));

                    var documentTotalTranslationChangesTotalSegments = mergedDocuments.TotalChangesPmSegments
                                                                + mergedDocuments.TotalChangesCmSegments
                                                                + mergedDocuments.TotalChangesExactSegments
                                                                + mergedDocuments.TotalChangesAtSegments
                                                                + mergedDocuments.TotalChangesFuzzySegments
                                                                + mergedDocuments.TotalChangesNewSegments;
                    xmlTxtWriter.WriteAttributeString("documentTotalTranslationChangesTotal_Segments", documentTotalTranslationChangesTotalSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalTranslationChangesTotal_Words", mergedDocuments.TotalChangesTotalWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalTranslationChangesTotal_Characters", mergedDocuments.TotalChangesTotalCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("documentTotalTranslationChangesTotal_Tags", mergedDocuments.TotalChangesTotalTags.ToString(CultureInfo.InvariantCulture));



                    #endregion


                    mergedDocuments.TotalNotTranslatedUpdated = mergedDocuments.TotalNotTranslatedOriginal;
                    mergedDocuments.TotalDraftUpdated = mergedDocuments.TotalDraftOriginal;
                    mergedDocuments.TotalTranslatedUpdated = mergedDocuments.TotalTranslatedOriginal;
                    mergedDocuments.TotalTranslationRejectedUpdated = mergedDocuments.TotalTranslationRejectedOriginal;
                    mergedDocuments.TotalTranslationApprovedUpdated = mergedDocuments.TotalTranslationApprovedOriginal;
                    mergedDocuments.TotalSignOffRejectedUpdated = mergedDocuments.TotalSignOffRejectedOriginal;
                    mergedDocuments.TotalSignedOffUpdated = mergedDocuments.TotalSignedOffOriginal;

                    foreach (var tcr in mergedDocuments.RecordsDictionary.Values)
                    {
                        if (string.Compare(tcr.TranslationOrigins.Original.ConfirmationLevel, tcr.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) == 0)
                            continue;

                        #region  |  confirmation_level original  |

                        switch (tcr.TranslationOrigins.Original.ConfirmationLevel)
                        {
                            case "Not Translated":
                            case "Unspecified": mergedDocuments.TotalNotTranslatedUpdated--; break;
                            case "Draft": mergedDocuments.TotalDraftUpdated--; break;
                            case "Translated": mergedDocuments.TotalTranslatedUpdated--; break;
                            case "Translation Rejected":
                            case "RejectedTranslation": mergedDocuments.TotalTranslationRejectedUpdated--; break;
                            case "Translation Approved":
                            case "ApprovedTranslation": mergedDocuments.TotalTranslationApprovedUpdated--; break;
                            case "Sign-off Rejected":
                            case "RejectedSignOff": mergedDocuments.TotalSignOffRejectedUpdated--; break;
                            case "Signed Off":
                            case "ApprovedSignOff": mergedDocuments.TotalSignedOffUpdated--; break;
                            default: mergedDocuments.TotalNotTranslatedUpdated--; break;
                        }

                        #endregion
                        #region  |  confirmation_level updated  |

                        switch (tcr.TranslationOrigins.Updated.ConfirmationLevel)
                        {
                            case "Not Translated":
                            case "Unspecified": mergedDocuments.TotalNotTranslatedUpdated++; break;
                            case "Draft": mergedDocuments.TotalDraftUpdated++; break;
                            case "Translated": mergedDocuments.TotalTranslatedUpdated++; break;
                            case "Translation Rejected":
                            case "RejectedTranslation": mergedDocuments.TotalTranslationRejectedUpdated++; break;
                            case "Translation Approved":
                            case "ApprovedTranslation": mergedDocuments.TotalTranslationApprovedUpdated++; break;
                            case "Sign-off Rejected":
                            case "RejectedSignOff": mergedDocuments.TotalSignOffRejectedUpdated++; break;
                            case "Signed Off":
                            case "ApprovedSignOff": mergedDocuments.TotalSignedOffUpdated++; break;
                            default: mergedDocuments.TotalNotTranslatedUpdated++; break;
                        }

                        #endregion
                    }

                    #region  |  status changes  |

                    decimal documentTotalStatusChangesUpdated =
                        mergedDocuments.TotalNotTranslatedUpdated
                        + mergedDocuments.TotalDraftUpdated
                        + mergedDocuments.TotalTranslatedUpdated
                        + mergedDocuments.TotalTranslationRejectedUpdated
                        + mergedDocuments.TotalTranslationApprovedUpdated
                        + mergedDocuments.TotalSignOffRejectedUpdated
                        + mergedDocuments.TotalSignedOffUpdated;


                    decimal documentTotalNotTranslatedChangesPercentage = 0;
                    decimal documentTotalDraftChangesPercentage = 0;
                    decimal documentTotalTranslatedChangesPercentage = 0;
                    decimal documentTotalTranslationRejectedChangesPercentage = 0;
                    decimal documentTotalTranslationApprovedChangesPercentage = 0;
                    decimal documentTotalSignOffRejectedChangesPercentage = 0;
                    decimal documentTotalSignedOffChangesPercentage = 0;



                    if (documentTotalStatusChangesUpdated > 0)
                    {
                        if (mergedDocuments.TotalNotTranslatedUpdated > mergedDocuments.TotalNotTranslatedOriginal)
                            documentTotalNotTranslatedChangesPercentage = Math.Round((mergedDocuments.TotalNotTranslatedUpdated - mergedDocuments.TotalNotTranslatedOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalNotTranslatedOriginal > 0)
                            documentTotalNotTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalNotTranslatedOriginal - mergedDocuments.TotalNotTranslatedUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalDraftUpdated > mergedDocuments.TotalDraftOriginal)
                            documentTotalDraftChangesPercentage = Math.Round((mergedDocuments.TotalDraftUpdated - mergedDocuments.TotalDraftOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalDraftOriginal > 0)
                            documentTotalDraftChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalDraftOriginal - mergedDocuments.TotalDraftUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalTranslatedUpdated > mergedDocuments.TotalTranslatedOriginal)
                            documentTotalTranslatedChangesPercentage = Math.Round((mergedDocuments.TotalTranslatedUpdated - mergedDocuments.TotalTranslatedOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalTranslatedOriginal > 0)
                            documentTotalTranslatedChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalTranslatedOriginal - mergedDocuments.TotalTranslatedUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalTranslationRejectedUpdated > mergedDocuments.TotalTranslationRejectedOriginal)
                            documentTotalTranslationRejectedChangesPercentage = Math.Round((mergedDocuments.TotalTranslationRejectedUpdated - mergedDocuments.TotalTranslationRejectedOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalTranslationRejectedOriginal > 0)
                            documentTotalTranslationRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalTranslationRejectedOriginal - mergedDocuments.TotalTranslationRejectedUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalTranslationApprovedUpdated > mergedDocuments.TotalTranslationApprovedOriginal)
                            documentTotalTranslationApprovedChangesPercentage = Math.Round((mergedDocuments.TotalTranslationApprovedUpdated - mergedDocuments.TotalTranslationApprovedOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalTranslationApprovedOriginal > 0)
                            documentTotalTranslationApprovedChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalTranslationApprovedOriginal - mergedDocuments.TotalTranslationApprovedUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalSignOffRejectedUpdated > mergedDocuments.TotalSignOffRejectedOriginal)
                            documentTotalSignOffRejectedChangesPercentage = Math.Round((mergedDocuments.TotalSignOffRejectedUpdated - mergedDocuments.TotalSignOffRejectedOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalSignOffRejectedOriginal > 0)
                            documentTotalSignOffRejectedChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalSignOffRejectedOriginal - mergedDocuments.TotalSignOffRejectedUpdated) / documentTotalStatusChangesUpdated * 100, 2));

                        if (mergedDocuments.TotalSignedOffUpdated > mergedDocuments.TotalSignedOffOriginal)
                            documentTotalSignedOffChangesPercentage = Math.Round((mergedDocuments.TotalSignedOffUpdated - mergedDocuments.TotalSignedOffOriginal) / documentTotalStatusChangesUpdated * 100, 2);
                        else if (mergedDocuments.TotalSignedOffOriginal > 0)
                            documentTotalSignedOffChangesPercentage = Convert.ToDecimal("-" + Math.Round((mergedDocuments.TotalSignedOffOriginal - mergedDocuments.TotalSignedOffUpdated) / documentTotalStatusChangesUpdated * 100, 2));
                    }

                    var documentTotalStatusChangesPercentage =
                        (documentTotalNotTranslatedChangesPercentage > 0 ? documentTotalNotTranslatedChangesPercentage : 0)
                        + (documentTotalDraftChangesPercentage > 0 ? documentTotalDraftChangesPercentage : 0)
                        + (documentTotalTranslatedChangesPercentage > 0 ? documentTotalTranslatedChangesPercentage : 0)
                        + (documentTotalTranslationRejectedChangesPercentage > 0 ? documentTotalTranslationRejectedChangesPercentage : 0)
                        + (documentTotalTranslationApprovedChangesPercentage > 0 ? documentTotalTranslationApprovedChangesPercentage : 0)
                        + (documentTotalSignOffRejectedChangesPercentage > 0 ? documentTotalSignOffRejectedChangesPercentage : 0)
                        + (documentTotalSignedOffChangesPercentage > 0 ? documentTotalSignedOffChangesPercentage : 0);


                    #endregion





                    xmlTxtWriter.WriteStartElement("translationModifications");


                    xmlTxtWriter.WriteAttributeString("sourcePMSegments", mergedDocuments.TotalSourcePmSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourcePMWords", mergedDocuments.TotalSourcePmWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourcePMCharacters", mergedDocuments.TotalSourcePmCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourcePMTags", mergedDocuments.TotalSourcePmTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesPMSegments", mergedDocuments.TotalChangesPmSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesPMWords", mergedDocuments.TotalChangesPmWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesPMCharacters", mergedDocuments.TotalChangesPmCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesPMTags", mergedDocuments.TotalChangesPmTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceCMSegments", mergedDocuments.TotalSourceCmSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceCMWords", mergedDocuments.TotalSourceCmWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceCMCharacters", mergedDocuments.TotalSourceCmCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceCMTags", mergedDocuments.TotalSourceCmTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesCMSegments", mergedDocuments.TotalChangesCmSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesCMWords", mergedDocuments.TotalChangesCmWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesCMCharacters", mergedDocuments.TotalChangesCmCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesCMTags", mergedDocuments.TotalChangesCmTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceExactSegments", mergedDocuments.TotalSourceExactSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceExactWords", mergedDocuments.TotalSourceExactWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceExactCharacters", mergedDocuments.TotalSourceExactCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceExactTags", mergedDocuments.TotalSourceExactTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesExactSegments", mergedDocuments.TotalChangesExactSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesExactWords", mergedDocuments.TotalChangesExactWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesExactCharacters", mergedDocuments.TotalChangesExactCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesExactTags", mergedDocuments.TotalChangesExactTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceRepsSegments", mergedDocuments.TotalSourceRepsSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceRepsWords", mergedDocuments.TotalSourceRepsWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceRepsCharacters", mergedDocuments.TotalSourceRepsCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceRepsTags", mergedDocuments.TotalSourceRepsTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesRepsSegments", mergedDocuments.TotalChangesRepsSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesRepsWords", mergedDocuments.TotalChangesRepsWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesRepsCharacters", mergedDocuments.TotalChangesRepsCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesRepsTags", mergedDocuments.TotalChangesRepsTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceFuzzySegments", mergedDocuments.TotalSourceFuzzySegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzyWords", mergedDocuments.TotalSourceFuzzyWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzyCharacters", mergedDocuments.TotalSourceFuzzyCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzyTags", mergedDocuments.TotalSourceFuzzyTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzySegments", mergedDocuments.TotalChangesFuzzySegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzyWords", mergedDocuments.TotalChangesFuzzyWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzyCharacters", mergedDocuments.TotalChangesFuzzyCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzyTags", mergedDocuments.TotalChangesFuzzyTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceFuzzy99Segments", mergedDocuments.TotalSourceFuzzy99Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy99Words", mergedDocuments.TotalSourceFuzzy99Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy99Characters", mergedDocuments.TotalSourceFuzzy99Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy99Tags", mergedDocuments.TotalSourceFuzzy99Tags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy99Segments", mergedDocuments.TotalChangesFuzzy99Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy99Words", mergedDocuments.TotalChangesFuzzy99Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy99Characters", mergedDocuments.TotalChangesFuzzy99Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy99Tags", mergedDocuments.TotalChangesFuzzy99Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceFuzzy94Segments", mergedDocuments.TotalSourceFuzzy94Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy94Words", mergedDocuments.TotalSourceFuzzy94Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy94Characters", mergedDocuments.TotalSourceFuzzy94Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy94Tags", mergedDocuments.TotalSourceFuzzy94Tags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy94Segments", mergedDocuments.TotalChangesFuzzy94Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy94Words", mergedDocuments.TotalChangesFuzzy94Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy94Characters", mergedDocuments.TotalChangesFuzzy94Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy94Tags", mergedDocuments.TotalChangesFuzzy94Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceFuzzy84Segments", mergedDocuments.TotalSourceFuzzy84Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy84Words", mergedDocuments.TotalSourceFuzzy84Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy84Characters", mergedDocuments.TotalSourceFuzzy84Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy84Tags", mergedDocuments.TotalSourceFuzzy84Tags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy84Segments", mergedDocuments.TotalChangesFuzzy84Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy84Words", mergedDocuments.TotalChangesFuzzy84Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy84Characters", mergedDocuments.TotalChangesFuzzy84Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy84Tags", mergedDocuments.TotalChangesFuzzy84Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceFuzzy74Segments", mergedDocuments.TotalSourceFuzzy74Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy74Words", mergedDocuments.TotalSourceFuzzy74Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy74Characters", mergedDocuments.TotalSourceFuzzy74Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceFuzzy74Tags", mergedDocuments.TotalSourceFuzzy74Tags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy74Segments", mergedDocuments.TotalChangesFuzzy74Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy74Words", mergedDocuments.TotalChangesFuzzy74Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy74Characters", mergedDocuments.TotalChangesFuzzy74Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesFuzzy74Tags", mergedDocuments.TotalChangesFuzzy74Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceNewSegments", mergedDocuments.TotalSourceNewSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceNewWords", mergedDocuments.TotalSourceNewWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceNewCharacters", mergedDocuments.TotalSourceNewCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceNewTags", mergedDocuments.TotalSourceNewTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesNewSegments", mergedDocuments.TotalChangesNewSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesNewWords", mergedDocuments.TotalChangesNewWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesNewCharacters", mergedDocuments.TotalChangesNewCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesNewTags", mergedDocuments.TotalChangesNewTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceATSegments", mergedDocuments.TotalSourceAtSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceATWords", mergedDocuments.TotalSourceAtWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceATCharacters", mergedDocuments.TotalSourceAtCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceATTags", mergedDocuments.TotalSourceAtTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesATSegments", mergedDocuments.TotalChangesAtSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesATWords", mergedDocuments.TotalChangesAtWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesATCharacters", mergedDocuments.TotalChangesAtCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesATTags", mergedDocuments.TotalChangesAtTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("sourceTotalSegments", mergedDocuments.TotalSourceSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceTotalWords", mergedDocuments.TotalSourceWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceTotalCharacters", mergedDocuments.TotalSourceCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("sourceTotalTags", mergedDocuments.TotalSourceTags.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesTotalSegments", mergedDocuments.TotalChangesTotalSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesTotalWords", mergedDocuments.TotalChangesTotalWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesTotalCharacters", mergedDocuments.TotalChangesTotalCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("changesTotalTags", mergedDocuments.TotalChangesTotalTags.ToString(CultureInfo.InvariantCulture));


                    xmlTxtWriter.WriteEndElement(); // translationModifications




                    xmlTxtWriter.WriteStartElement("pempAnalysis");

                    xmlTxtWriter.WriteStartElement("rates");

                    xmlTxtWriter.WriteAttributeString("exactWords", mergedDocuments.PostEditPriceExactWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99Words", mergedDocuments.PostEditPriceFuzzy9995Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Words", mergedDocuments.PostEditPriceFuzzy9485Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Words", mergedDocuments.PostEditPriceFuzzy8475Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Words", mergedDocuments.PostEditPriceFuzzy7450Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newWords", mergedDocuments.PostEditPriceNewWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("totalWords", ""); // not used


                    xmlTxtWriter.WriteAttributeString("exactTotal", mergedDocuments.PostEditPriceExactTotal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99Total", mergedDocuments.PostEditPriceFuzzy9995Total.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Total", mergedDocuments.PostEditPriceFuzzy9485Total.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Total", mergedDocuments.PostEditPriceFuzzy8475Total.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Total", mergedDocuments.PostEditPriceFuzzy7450Total.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newTotal", mergedDocuments.PostEditPriceNewTotal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("priceTotal", mergedDocuments.PostEditPriceTotal.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("currency", mergedDocuments.PostEditPriceCurrency);

                    xmlTxtWriter.WriteEndElement(); // rates

                    xmlTxtWriter.WriteStartElement("data");

                    xmlTxtWriter.WriteAttributeString("exactSegments", mergedDocuments.ExactSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("exactWords", mergedDocuments.ExactWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("exactCharacters", mergedDocuments.ExactCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("exactPercent", mergedDocuments.ExactPercent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("exactTags", mergedDocuments.ExactTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("fuzzy99Segments", mergedDocuments.Fuzzy99Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99Words", mergedDocuments.Fuzzy99Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99Characters", mergedDocuments.Fuzzy99Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99Percent", mergedDocuments.Fuzzy99Percent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy99tags", mergedDocuments.Fuzzy99Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("fuzzy94Segments", mergedDocuments.Fuzzy94Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Words", mergedDocuments.Fuzzy94Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Characters", mergedDocuments.Fuzzy94Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Percent", mergedDocuments.Fuzzy94Percent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy94Tags", mergedDocuments.Fuzzy94Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("fuzzy84Segments", mergedDocuments.Fuzzy84Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Words", mergedDocuments.Fuzzy84Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Characters", mergedDocuments.Fuzzy84Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Percent", mergedDocuments.Fuzzy84Percent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy84Tags", mergedDocuments.Fuzzy84Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("fuzzy74Segments", mergedDocuments.Fuzzy74Segments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Words", mergedDocuments.Fuzzy74Words.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Characters", mergedDocuments.Fuzzy74Characters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Percent", mergedDocuments.Fuzzy74Percent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("fuzzy74Tags", mergedDocuments.Fuzzy74Tags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("newSegments", mergedDocuments.NewSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newWords", mergedDocuments.NewWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newCharacters", mergedDocuments.NewCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newPercent", mergedDocuments.NewPercent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("newTags", mergedDocuments.NewTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("totalSegments", mergedDocuments.TotalSegments.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("totalWords", mergedDocuments.TotalWords.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("totalCharacters", mergedDocuments.TotalCharacters.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("totalPercent", mergedDocuments.TotalPercent.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("totalTags", mergedDocuments.TotalTags.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteEndElement(); // data

                    xmlTxtWriter.WriteEndElement(); // pempAnalysis



                    xmlTxtWriter.WriteStartElement("confirmationStatistics");

                    xmlTxtWriter.WriteAttributeString("notTranslatedUpdatedStatusChangesPercentage", documentTotalNotTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("draftUpdatedStatusChangesPercentage", documentTotalDraftChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translatedUpdatedStatusChangesPercentage", documentTotalTranslatedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationRejectedUpdatedStatusChangesPercentage", documentTotalTranslationRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationApprovedUpdatedStatusChangesPercentage", documentTotalTranslationApprovedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signOffRejectedUpdatedStatusChangesPercentage", documentTotalSignOffRejectedChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signedOffUpdatedStatusChangesPercentage", documentTotalSignedOffChangesPercentage.ToString(CultureInfo.InvariantCulture));


                    xmlTxtWriter.WriteAttributeString("notTranslatedOriginal", mergedDocuments.TotalNotTranslatedOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("draftOriginal", mergedDocuments.TotalDraftOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translatedOriginal", mergedDocuments.TotalTranslatedOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationRejectedOriginal", mergedDocuments.TotalTranslationRejectedOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationApprovedOriginal", mergedDocuments.TotalTranslationApprovedOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signOffRejectedOriginal", mergedDocuments.TotalSignOffRejectedOriginal.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signedOffOriginal", mergedDocuments.TotalSignedOffOriginal.ToString(CultureInfo.InvariantCulture));


                    xmlTxtWriter.WriteAttributeString("notTranslatedUpdated", mergedDocuments.TotalNotTranslatedUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("draftUpdated", mergedDocuments.TotalDraftUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translatedUpdated", mergedDocuments.TotalTranslatedUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationRejectedUpdated", mergedDocuments.TotalTranslationRejectedUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("translationApprovedUpdated", mergedDocuments.TotalTranslationApprovedUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signOffRejectedUpdated", mergedDocuments.TotalSignOffRejectedUpdated.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("signedOffUpdated", mergedDocuments.TotalSignedOffUpdated.ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteEndElement();



                    xmlTxtWriter.WriteStartElement("totals");
                    xmlTxtWriter.WriteAttributeString("statusChangesPercentage", documentTotalStatusChangesPercentage.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteEndElement();






                    #region  |  activities  |

                    xmlTxtWriter.WriteStartElement("activities");



                    foreach (var tca in kvp.Value)
                    {

                        var aDamdict = new Dictionary<string, DocumentAnalysisMerged>();


                        #region  |  variables main  |


                        decimal aDocumentTotalSegmentContentUpdated = 0;
                        decimal aDocumentTotalSegmentStatusUpdated = 0;
                        decimal aDocumentTotalSegmentCommentsUpdated = 0;

                        decimal aDocumentTotalSegmentCommentsUpdatedTotal = 0;

                        decimal aDocumentTotalSegments = 0;

                        #endregion

                        aDocumentTotalSegments = mergedDocuments.TotalSourceSegments; //total segments for that document; no need accumulate this


                        var qualityMetricsRecordCount = 0;
                        foreach (var record in tca.Records)
                        {
                            if (record.QualityMetrics != null && record.QualityMetrics.Count > 0)
                            {
                                qualityMetricsRecordCount += record.QualityMetrics.Count;
                            }

                            var trgo = Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, tpa.ComparisonOptions.IncludeTagsInComparison);
                            var trgu = Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, tpa.ComparisonOptions.IncludeTagsInComparison);

                            if (!aDamdict.ContainsKey(record.ParagraphId + "_" + record.SegmentId))
                            {
                                var aDam = new DocumentAnalysisMerged();

                                #region  |  common  |



                                if (trgo != trgu)
                                {
                                    aDocumentTotalSegmentContentUpdated++;
                                    aDam.TranslationChangesAdded = true;
                                }
                                if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    aDocumentTotalSegmentStatusUpdated++;
                                    aDam.StatusChangeAdded = true;
                                }
                                if (record.Comments.Count > 0)
                                {
                                    aDocumentTotalSegmentCommentsUpdated++;
                                    aDam.CommentChangeAdded = true;
                                    aDocumentTotalSegmentCommentsUpdatedTotal += record.Comments.Count;
                                }

                                aDamdict.Add(record.ParagraphId + "_" + record.SegmentId, aDam);

                                #endregion
                            }
                            else
                            {
                                #region  |  common  |


                                var aDam = mergedDocuments.DocumentAnalysisMergedDictionary[record.ParagraphId + "_" + record.SegmentId];


                                if ((trgo != trgu) && !aDam.TranslationChangesAdded)
                                {
                                    aDocumentTotalSegmentContentUpdated++;
                                    aDam.TranslationChangesAdded = true;
                                }

                                if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) != 0 && !aDam.StatusChangeAdded)
                                {
                                    aDocumentTotalSegmentStatusUpdated++;
                                    aDam.StatusChangeAdded = true;
                                }


                                if (record.Comments.Count > 0 && !aDam.CommentChangeAdded)
                                {
                                    aDocumentTotalSegmentCommentsUpdated++;
                                    aDam.CommentChangeAdded = true;
                                }
                                if (record.Comments.Count > 0)
                                    aDocumentTotalSegmentCommentsUpdatedTotal += record.Comments.Count;


                                #endregion
                            }
                        }

                        #region  |  activity  |

                        xmlTxtWriter.WriteStartElement("activity");

                        xmlTxtWriter.WriteAttributeString("id", tca.Id.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("activityId", tca.ProjectActivityId.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("activityName", tca.TranslatableDocument.DocumentName);
                        xmlTxtWriter.WriteAttributeString("activityType", tca.DocumentActivityType);
                        xmlTxtWriter.WriteAttributeString("documentId", tca.DocumentId);
                        xmlTxtWriter.WriteAttributeString("projectId", tca.TranslatableDocument.StudioProjectId);
                        xmlTxtWriter.WriteAttributeString("started", tca.Started.ToString());
                        xmlTxtWriter.WriteAttributeString("stopped", tca.Stopped.ToString());
                        xmlTxtWriter.WriteAttributeString("words", tca.WordCount.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("recordCount", tca.Records.Count.ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("a_documentTotalSegments", aDocumentTotalSegments.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("a_documentTotalSegmentContentUpdated", aDocumentTotalSegmentContentUpdated.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("a_documentTotalSegmentStatusUpdated", aDocumentTotalSegmentStatusUpdated.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("a_qualityMetrics_record_count", qualityMetricsRecordCount.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("a_documentTotalSegmentCommentsUpdated", aDocumentTotalSegmentCommentsUpdated.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("a_documentTotalSegmentCommentsUpdatedTotal", aDocumentTotalSegmentCommentsUpdatedTotal.ToString(CultureInfo.InvariantCulture));



                        long tcaTotalWordsFlattened = 0;
                        var segIdList = new List<string>();
                        foreach (var record in tca.Records)
                        {
                            if (segIdList.Contains(record.ParagraphId + "_" + record.SegmentId)) continue;
                            tcaTotalWordsFlattened += record.WordCount;
                            segIdList.Add(record.ParagraphId + "_" + record.SegmentId);
                        }
                        xmlTxtWriter.WriteAttributeString("cntf", tcaTotalWordsFlattened.ToString(CultureInfo.InvariantCulture));

                        var tcaTotalTime = new TimeSpan(tca.TicksActivity);


                        xmlTxtWriter.WriteAttributeString("totalElapsedTime", string.Format("{0:00}:{1:00}:{2:00}", tcaTotalTime.Hours, tcaTotalTime.Minutes, tcaTotalTime.Seconds));
                        xmlTxtWriter.WriteAttributeString("totalElapsedHours", Math.Round(tcaTotalTime.TotalHours, 3).ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteEndElement();//activity
                        #endregion
                    }


                    xmlTxtWriter.WriteEndElement();//activities
                    #endregion


                    #region  |  segments  |

                    xmlTxtWriter.WriteStartElement("segments");


                    foreach (var tcr in mergedDocuments.RecordsDictionary.Values)
                    {
                        var targetOriginal = Helper.GetCompiledSegmentText(tcr.ContentSections.TargetOriginalSections, tpa.ComparisonOptions.IncludeTagsInComparison);
                        var targetUpdated = Helper.GetCompiledSegmentText(tcr.ContentSections.TargetUpdatedSections, tpa.ComparisonOptions.IncludeTagsInComparison);


                        xmlTxtWriter.WriteStartElement("segment");

                        xmlTxtWriter.WriteAttributeString("id", tcr.Id.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("paragraphId", tcr.ParagraphId);
                        xmlTxtWriter.WriteAttributeString("segmentId", tcr.SegmentId);
                        xmlTxtWriter.WriteAttributeString("words", tcr.WordCount.ToString(CultureInfo.InvariantCulture));


                        xmlTxtWriter.WriteStartElement("date");
                        xmlTxtWriter.WriteStartElement("td");
                        xmlTxtWriter.WriteAttributeString("nowrap", "nowrap");
                        if (tcr.MergedDatesTemp.Count > 1)
                            xmlTxtWriter.WriteAttributeString("class", "multiplechangesapplied");

                        for (var i = 0; i < tcr.MergedDatesTemp.Count; i++)
                        {
                            if (i > 0)
                            {
                                xmlTxtWriter.WriteStartElement("br");
                                xmlTxtWriter.WriteEndElement();//br 
                            }
                            xmlTxtWriter.WriteString(tcr.MergedDatesTemp[i]);
                        }


                        xmlTxtWriter.WriteEndElement();//tde 
                        xmlTxtWriter.WriteEndElement();//created 



                        #region  |  confirmationLevelOriginal  |
                        xmlTxtWriter.WriteStartElement("confirmationLevelOriginal");
                        xmlTxtWriter.WriteString(tcr.TranslationOrigins.Original.ConfirmationLevel);
                        xmlTxtWriter.WriteEndElement();//confirmationLevelOriginal
                        #endregion

                        #region  |  confirmationLevelUpdated  |
                        xmlTxtWriter.WriteStartElement("confirmationLevelUpdated");
                        xmlTxtWriter.WriteString(tcr.TranslationOrigins.Updated.ConfirmationLevel);
                        xmlTxtWriter.WriteEndElement();//confirmationLevelUpdated
                        #endregion

                        #region  |  translationMatchType  |




                        xmlTxtWriter.WriteStartElement("translationMatchType");
                        xmlTxtWriter.WriteStartElement("td");

                        var matchColor = GetMatchColor(tcr.TranslationOrigins.Original.TranslationStatus, tcr.TranslationOrigins.Original.OriginType);
                        xmlTxtWriter.WriteAttributeString("style", "text-align: center" + (matchColor != string.Empty ? ";background-color: " + matchColor : string.Empty));


                        if (string.Compare(tcr.TranslationOrigins.Original.TranslationStatus, tcr.TranslationOrigins.Updated.TranslationStatus, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (tcr.TranslationOrigins.Original.TranslationStatus.Trim() != string.Empty)
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "text");
                                xmlTxtWriter.WriteString(tcr.TranslationOrigins.Original.TranslationStatus);
                                xmlTxtWriter.WriteEndElement();//span                                                                
                            }
                        }
                        else
                        {
                            if (tcr.TranslationOrigins.Original.TranslationStatus.Trim() != string.Empty)
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "textRemoved");
                                xmlTxtWriter.WriteString(tcr.TranslationOrigins.Original.TranslationStatus);
                                xmlTxtWriter.WriteEndElement();//span 

                                if (tcr.TranslationOrigins.Updated.TranslationStatus.Trim() != string.Empty)
                                {
                                    xmlTxtWriter.WriteStartElement("br");
                                    xmlTxtWriter.WriteEndElement();//br 
                                }
                            }

                            if (tcr.TranslationOrigins.Updated.TranslationStatus.Trim() != string.Empty)
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "textNew");
                                xmlTxtWriter.WriteString(tcr.TranslationOrigins.Updated.TranslationStatus);
                                xmlTxtWriter.WriteEndElement();//span 
                            }
                        }

                        xmlTxtWriter.WriteEndElement();//td 
                        xmlTxtWriter.WriteEndElement();//translationMatchType




                        #endregion

                        #region  |  segmentStatus  |

                        //Not Translated
                        //Draft
                        //Translated
                        //Translation Rejected
                        //Translation Approved
                        //Sign-off Rejected
                        //Signed Off

                        xmlTxtWriter.WriteStartElement("status");

                        if (string.Compare(tcr.TranslationOrigins.Original.ConfirmationLevel, tcr.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            xmlTxtWriter.WriteStartElement("span");
                            xmlTxtWriter.WriteAttributeString("class", "text");
                            xmlTxtWriter.WriteString(GetVisualSegmentStatus(tcr.TranslationOrigins.Updated.ConfirmationLevel));
                            xmlTxtWriter.WriteEndElement();//span 
                        }
                        else
                        {
                            xmlTxtWriter.WriteStartElement("span");
                            xmlTxtWriter.WriteAttributeString("class", "textRemoved");
                            xmlTxtWriter.WriteString(GetVisualSegmentStatus(tcr.TranslationOrigins.Original.ConfirmationLevel));
                            xmlTxtWriter.WriteEndElement();//span 

                            xmlTxtWriter.WriteStartElement("br");
                            xmlTxtWriter.WriteEndElement();//br 

                            xmlTxtWriter.WriteStartElement("span");
                            xmlTxtWriter.WriteAttributeString("class", "textNew");
                            xmlTxtWriter.WriteString(GetVisualSegmentStatus(tcr.TranslationOrigins.Updated.ConfirmationLevel));
                            xmlTxtWriter.WriteEndElement();//span 
                        }

                        xmlTxtWriter.WriteEndElement();//segmentStatus




                        #endregion

                        #region  |  source  |


                        xmlTxtWriter.WriteStartElement("source");
                        foreach (var scr in tcr.ContentSections.SourceSections)
                        {

                            if (scr.CntType == ContentSection.ContentType.Text)
                            {
                                var strList = Helper.GetTextSections(scr.Content);
                                foreach (var str in strList)
                                {
                                    xmlTxtWriter.WriteStartElement("span");
                                    xmlTxtWriter.WriteAttributeString("class", "text");
                                    xmlTxtWriter.WriteString(str);
                                    xmlTxtWriter.WriteEndElement();//span    
                                }
                            }
                            else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "tag");
                                xmlTxtWriter.WriteString(scr.Content);
                                xmlTxtWriter.WriteEndElement();//span                                    
                            }

                        }
                        xmlTxtWriter.WriteEndElement();//source


                        #endregion

                        #region  |  target  |


                        xmlTxtWriter.WriteStartElement("target");


                        foreach (var trgo in tcr.ContentSections.TargetOriginalSections)
                        {
                            if (trgo.RevisionMarker != null && trgo.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                            {
                                //ignore
                            }
                            else
                            {
                                if (trgo.CntType == ContentSection.ContentType.Text)
                                {
                                    var strList = Helper.GetTextSections(trgo.Content);
                                    foreach (var str in strList)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                        xmlTxtWriter.WriteString(str);
                                        xmlTxtWriter.WriteEndElement();//span    
                                    }
                                }
                                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                {
                                    xmlTxtWriter.WriteStartElement("span");
                                    xmlTxtWriter.WriteAttributeString("class", "tag");
                                    xmlTxtWriter.WriteString(trgo.Content);
                                    xmlTxtWriter.WriteEndElement();//span                                    
                                }
                            }

                        }
                        xmlTxtWriter.WriteEndElement();//source


                        #endregion

                        #region  |  target updated  |


                        xmlTxtWriter.WriteStartElement("target_updated");
                        foreach (var trgu in tcr.ContentSections.TargetUpdatedSections)
                        {

                            if (trgu.RevisionMarker != null && trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                            {
                                //ignore
                            }
                            else
                            {
                                if (trgu.CntType == ContentSection.ContentType.Text)
                                {
                                    var strList = Helper.GetTextSections(trgu.Content);
                                    foreach (var str in strList)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                        xmlTxtWriter.WriteString(str);
                                        xmlTxtWriter.WriteEndElement();//span    
                                    }
                                }
                                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                {
                                    xmlTxtWriter.WriteStartElement("span");
                                    xmlTxtWriter.WriteAttributeString("class", "tag");
                                    xmlTxtWriter.WriteString(trgu.Content);
                                    xmlTxtWriter.WriteEndElement();//span                                    
                                }
                            }

                        }
                        xmlTxtWriter.WriteEndElement();//source

                        #endregion

                        #region  |  target (Updated) - Revision Markers  |

                        var trackChangeSections = new List<ContentSection>();

                        if (mergedDocuments.SegmentPreviousTrackChanges.ContainsKey(tcr.ParagraphId + "_" + tcr.SegmentId))
                        {
                            trackChangeSections.AddRange(mergedDocuments.SegmentPreviousTrackChanges[tcr.ParagraphId + "_" + tcr.SegmentId]);
                        }
                        trackChangeSections.AddRange(tcr.ContentSections.TargetUpdatedSections.Where(trgu => trgu.RevisionMarker != null));

                        xmlTxtWriter.WriteStartElement("targetUpdatedRevisionMarkers");
                        xmlTxtWriter.WriteAttributeString("count", trackChangeSections.Count.ToString(CultureInfo.InvariantCulture));

                        if (trackChangeSections.Count > 0)
                        {
                            foreach (var trgu in trackChangeSections)
                            {
                                if (trgu.RevisionMarker == null) continue;
                                xmlTxtWriter.WriteStartElement("revisionMarker");

                                var dt = trgu.RevisionMarker.Created.Value.Year
                                         + "-" + trgu.RevisionMarker.Created.Value.Month.ToString().PadLeft(2, '0')
                                         + "-" + trgu.RevisionMarker.Created.Value.Day.ToString().PadLeft(2, '0')
                                         + " " + trgu.RevisionMarker.Created.Value.Hour.ToString().PadLeft(2, '0')
                                         + ":" + trgu.RevisionMarker.Created.Value.Minute.ToString().PadLeft(2, '0')
                                         + ":" + trgu.RevisionMarker.Created.Value.Second.ToString().PadLeft(2, '0');

                                xmlTxtWriter.WriteAttributeString("author", trgu.RevisionMarker.Author);
                                xmlTxtWriter.WriteAttributeString("date", dt);
                                xmlTxtWriter.WriteAttributeString("revisionType", trgu.RevisionMarker.RevType.ToString(CultureInfo.InvariantCulture));
                                xmlTxtWriter.WriteStartElement("content");

                                if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Unchanged)
                                {
                                    if (trgu.CntType == ContentSection.ContentType.Text)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "text");
                                        xmlTxtWriter.WriteString(trgu.Content);
                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                    else
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "tag");
                                        xmlTxtWriter.WriteString(trgu.Content);
                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                }
                                else if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Insert)
                                {
                                    if (trgu.CntType == ContentSection.ContentType.Text)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "textNew");

                                        xmlTxtWriter.WriteString(trgu.Content);

                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                    else
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "tagNew");

                                        xmlTxtWriter.WriteString(trgu.Content);
                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                }
                                else if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                                {
                                    if (trgu.CntType == ContentSection.ContentType.Text)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "textRemoved");

                                        xmlTxtWriter.WriteString(trgu.Content);

                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                    else
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "tagRemoved");

                                        xmlTxtWriter.WriteString(trgu.Content);
                                        xmlTxtWriter.WriteEndElement();//span
                                    }
                                }

                                xmlTxtWriter.WriteEndElement();//content
                                xmlTxtWriter.WriteEndElement();//revisionMarker
                            }
                        }

                        xmlTxtWriter.WriteEndElement();//targetUpdatedRevisionMarkers

                        #endregion

                        #region  |  comparison  |


                        xmlTxtWriter.WriteStartElement("comparison");

                        if (targetOriginal != targetUpdated)
                        {
                            var tccus = tcc.GetComparisonTextUnits(tcr.ContentSections.TargetOriginalSections, tcr.ContentSections.TargetUpdatedSections, tpa.ComparisonOptions.ConsolidateChanges);

                            foreach (var tccu in tccus)
                            {
                                if (tccu.Type == ComparisonUnit.ComparisonType.Identical)
                                {
                                    foreach (var trgu in tccu.Section)
                                    {
                                        if (trgu.CntType == ContentSection.ContentType.Text)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", "text");
                                            xmlTxtWriter.WriteString(trgu.Content);
                                            xmlTxtWriter.WriteEndElement();//span
                                        }
                                        else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", "tag");

                                            xmlTxtWriter.WriteString(trgu.Content);
                                            xmlTxtWriter.WriteEndElement();//span
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var trgu in tccu.Section)
                                    {
                                        if (trgu.CntType == ContentSection.ContentType.Text)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", tccu.Type == ComparisonUnit.ComparisonType.New ? "textNew" : "textRemoved");

                                            xmlTxtWriter.WriteString(trgu.Content);

                                            xmlTxtWriter.WriteEndElement();//span
                                        }
                                        else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                        {

                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", tccu.Type == ComparisonUnit.ComparisonType.New ? "tagNew" : "tagRemoved");

                                            xmlTxtWriter.WriteString(trgu.Content);
                                            xmlTxtWriter.WriteEndElement();//span
                                        }
                                    }
                                }
                            }
                        }
                        xmlTxtWriter.WriteEndElement();//comparison


                        #endregion

                        #region  |  Edit-Distance  |

                        var pempHolder = mergedDocuments.PempDictionary[tcr.ParagraphId + "/" + tcr.SegmentId];

                        xmlTxtWriter.WriteStartElement("EditDistance");
                        xmlTxtWriter.WriteString(pempHolder.EditDistance.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteEndElement();



                        xmlTxtWriter.WriteStartElement("MaxCharacters");
                        xmlTxtWriter.WriteString(pempHolder.MaxCharacters.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteEndElement();


                        xmlTxtWriter.WriteStartElement("PEMP");
                        if (string.Compare(tcr.TranslationOrigins.Updated.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            xmlTxtWriter.WriteStartElement("span");
                            xmlTxtWriter.WriteAttributeString("class", "text");
                            xmlTxtWriter.WriteString((targetOriginal != targetUpdated ? pempHolder.ModificationPercentage.ToString(CultureInfo.InvariantCulture) : "100") + "%");
                            xmlTxtWriter.WriteEndElement();//span
                        }
                        else
                        {
                            if ((targetOriginal != targetUpdated ? Convert.ToInt32(pempHolder.ModificationPercentage) : 100) != 100)
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "textRemoved");
                                xmlTxtWriter.WriteString((targetOriginal != targetUpdated ? pempHolder.ModificationPercentage.ToString(CultureInfo.InvariantCulture) : "100") + "%");
                                xmlTxtWriter.WriteEndElement();//span

                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "textNew");
                                xmlTxtWriter.WriteString("100%");
                                xmlTxtWriter.WriteEndElement();//span
                            }
                            else
                            {
                                xmlTxtWriter.WriteStartElement("span");
                                xmlTxtWriter.WriteAttributeString("class", "text");
                                xmlTxtWriter.WriteString("100%");
                                xmlTxtWriter.WriteEndElement();//span
                            }

                        }
                        xmlTxtWriter.WriteEndElement();

                        #endregion

                        #region  |  quality metrics  |

                        if (tcr.QualityMetrics != null && tcr.QualityMetrics.Count > 0)
                        {

                            //flatten the entries if there exists more than one entry for the same record
                            tcr.QualityMetrics = tcr.QualityMetrics.OrderByDescending(a => a.Modified).ToList();
                            //List<string> guids = new List<string>();

                            xmlTxtWriter.WriteStartElement("qualityMetrics");
                            xmlTxtWriter.WriteAttributeString("count", tcr.QualityMetrics.Count.ToString(CultureInfo.InvariantCulture));

                            foreach (var qm in tcr.QualityMetrics)
                            {
                                if (!mergedDocuments.QualityMetrics.Exists(a => a.Id == qm.Id)) continue;
                                //guids.Add(qm.guid);
                                xmlTxtWriter.WriteStartElement("qualityMetric");

                                xmlTxtWriter.WriteAttributeString("id", qm.Id);
                                xmlTxtWriter.WriteAttributeString("name", qm.Name);
                                xmlTxtWriter.WriteAttributeString("status", qm.Status.ToString());
                                xmlTxtWriter.WriteAttributeString("userName", qm.UserName);
                                xmlTxtWriter.WriteAttributeString("severity", qm.SeverityName);
                                xmlTxtWriter.WriteAttributeString("severityWeight", qm.SeverityValue.ToString(CultureInfo.InvariantCulture));
                                xmlTxtWriter.WriteAttributeString("created", qm.Created.ToString());
                                xmlTxtWriter.WriteAttributeString("modified", qm.Modified.ToString());

                                xmlTxtWriter.WriteStartElement("content");
                                xmlTxtWriter.WriteString(qm.Content);
                                xmlTxtWriter.WriteEndElement();//content

                                xmlTxtWriter.WriteStartElement("comment");
                                xmlTxtWriter.WriteString(qm.Comment);
                                xmlTxtWriter.WriteEndElement();//comment


                                xmlTxtWriter.WriteEndElement();//qualityMetric
                            }
                            xmlTxtWriter.WriteEndElement();//qualityMetrics
                        }

                        #endregion

                        #region  |  comments  |

                        if (tcr.Comments.Count > 0)
                        {
                            xmlTxtWriter.WriteStartElement("comments");

                            xmlTxtWriter.WriteAttributeString("count", tcr.Comments.Count.ToString(CultureInfo.InvariantCulture));

                            foreach (var comment in tcr.Comments)
                            {
                                xmlTxtWriter.WriteStartElement("comment");

                                xmlTxtWriter.WriteAttributeString("author", comment.Author);
                                xmlTxtWriter.WriteAttributeString("created", comment.Created.HasValue ? comment.Created.Value.ToString(CultureInfo.InvariantCulture) : string.Empty);
                                xmlTxtWriter.WriteAttributeString("severity", comment.Severity);
                                xmlTxtWriter.WriteAttributeString("version", comment.Version);


                                xmlTxtWriter.WriteString(comment.Content);

                                xmlTxtWriter.WriteEndElement();//comment
                            }

                            xmlTxtWriter.WriteEndElement();//comments
                        }
                        #endregion

                        xmlTxtWriter.WriteEndElement();//segment
                    }
                    xmlTxtWriter.WriteEndElement();//segments 
                    #endregion


                    xmlTxtWriter.WriteEndElement();//document

                }

                xmlTxtWriter.WriteEndElement();//documents

                xmlTxtWriter.WriteEndDocument();
                xmlTxtWriter.Flush();
                xmlTxtWriter.Close();


                WriteReportResourcesToDirectory(applicationTrackChangesReportPath, cpi != null, "01");

                TransformTrackChangesXmlReport(xmlFileFullPath, "01");

            }
            catch (Exception ex)
            {
                MessageBox.Show("CreateOverviewReport(): " + ex.Message);
            }

        }




        private static void TransformTrackChangesXmlReport(string xmlFileFullPath, string version)
        {
            var xsltName = "Report.StyleSheet." + version + ".xslt";



            var filePathXslt = Path.Combine(Path.GetDirectoryName(xmlFileFullPath), xsltName);



            var xsltSetting = new XsltSettings
            {
                EnableDocumentFunction = true,
                EnableScript = true
            };


            var myXPathDoc = new XPathDocument(xmlFileFullPath);


            var myXslTrans = new XslCompiledTransform();
            myXslTrans.Load(filePathXslt, xsltSetting, null);



            var myWriter = new XmlTextWriter(xmlFileFullPath + ".html", Encoding.UTF8);

            myXslTrans.Transform(myXPathDoc, null, myWriter);


            myWriter.Flush();
            myWriter.Close();

            try
            {
                File.Delete(filePathXslt);
                File.Delete(xmlFileFullPath);
            }
            catch (Exception ex)
            {
                //ignore
            }

        }
        private void WriteReportResourcesToDirectory(string reportDirectory, bool updateStyleSheet, string version)
        {

            var xsltName = "Report.StyleSheet." + version + ".xslt";


            var filePathXslt = Path.Combine(reportDirectory, xsltName);


            #region  |  default report  |

            var assembly = Assembly.GetExecutingAssembly();
                          
            var templateXsltName = "Sdl.Community.Report.Reports.Report.StyleSheet." + version + ".xslt";
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


            if (updateStyleSheet)
                UdpateStyleSheetInformation(filePathXslt);


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
                color = "#FFFFFF";
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
        public void InitializeWriteFlagsFolder(string applicationTrackChangesReportPath, List<string> cultureInfoIds)
        {
            //List<string> flags = new List<string> { "af-ZA", "ar-AE", "ar-BH", "ar-DZ", "ar-EG", "ar-IQ", "ar-JO", "ar-KW", "ar-LB", "ar-LY", "ar-MA", "ar-OM", "ar-QA", "ar-SA", "ar-SY", "ar-TN", "ar-YE", "ar"
            //, "be-BY", "be" , "bg-BG", "bg" , "bp", "bs-Cyrl-BA" , "bs-Latn-BA", "bs" , "ca-ES", "ca" , "cf", "ch" , "cs-CZ", "cs" , "da-DK", "da" , "de-AT", "de-CH" , "de-DE", "de-LI" , "de-LU", "de" , "el-GR"
            //, "el" , "empty", "en-AU" , "en-BZ", "en-CA" , "en-GB", "en-IE" , "en-JM", "en-NZ" , "en-PH", "en-TT" , "en-US", "en-ZW" , "en", "es-AR" , "es-BO", "es-CL" , "es-CO", "es-CR" , "es-DO", "es-EC" 
            //, "es-ES", "es-GT" , "es-HN", "es-MX" , "es-NI", "es-PA" , "es-PE", "es-PR" , "es-PY", "es-SV" , "es-UY", "es-VE" , "es", "et-EE" , "et", "eu-ES" , "eu", "fa-IR" , "fa", "fi-FI" , "fi", "fl" 
            //, "fr-BE", "fr-CA" , "fr-CH", "fr-FR" , "fr-LU", "fr" , "ga", "gb" , "he-IL", "he" , "hi-IN", "hi" , "hr-HR", "hr" , "hu-HU", "hu" , "id-ID", "in" , "is-IS", "is" 
            //, "it-CH", "it-IT" , "it", "ja-JP" , "ja", "kk-KZ" , "kk", "ko-KR" , "ko", "sourceLang" , "targetLang-LT", "targetLang" , "lv-LV", "lv" , "lx", "mk-MK" , "mk", "ms-MY" , "ms", "nb-NO" , "nl-BE", "nl-NL" , "nl"
            //, "nn-NO" , "no", "pl-PL" , "pl", "pt-BR", "pt-PT" , "pt", "ro-RO" , "ro", "ru-RU", "ru" , "sd", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", ""
            //, "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", "", "" , "", "" , "", ""};

            var flagsPath = Path.Combine(applicationTrackChangesReportPath, "Flags");
            if (!Directory.Exists(flagsPath))
                Directory.CreateDirectory(flagsPath);


            var asb = Assembly.GetExecutingAssembly();

            if (!cultureInfoIds.Contains("empty"))
                cultureInfoIds.Add("empty");

            foreach (var cultureInfoId in cultureInfoIds)
            {
                using (var inputStream = asb.GetManifestResourceStream("Report.Flags." + cultureInfoId + ".gif"))
                {
                    if (inputStream == null) continue;
                    var outputFilePath = Path.Combine(flagsPath, cultureInfoId + ".gif");
                    if (File.Exists(outputFilePath)) continue;
                    Stream outputStream = File.Open(outputFilePath, FileMode.Create);

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
        }


        #region  |  Udpate Style Sheet Information  |

        private readonly Regex _regexStyle = new Regex(@"(?<x1>\<style\s+[^\>]*\>)(?<x2>.*?)(?<x3>\<\/style\>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexNewText = new Regex(@"(?<x1>span\.textNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexRemovedText = new Regex(@"(?<x1>span\.textRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexNewTag = new Regex(@"(?<x1>span\.tagNew\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private readonly Regex _regexRemovedTag = new Regex(@"(?<x1>span\.tagRemoved\s*\{)(?<x2>.*?)(?<x3>\})", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private void UdpateStyleSheetInformation(string filePathXslt)
        {

            string str;
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

        private DifferencesFormatting StyleNewText { get; set; }
        private DifferencesFormatting StyleRemovedText { get; set; }
        private DifferencesFormatting StyleNewTag { get; set; }
        private DifferencesFormatting StyleRemovedTag { get; set; }

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
            str2 = GetHtmlStyleText(StyleNewText);
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_removed_text(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(StyleRemovedText);
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_new_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(StyleNewTag);
            return str1 + str2 + str3;
        }
        private string MatchEvaluator_removed_tag(Match m)
        {
            var str1 = m.Groups["x1"].Value;
            var str2 = m.Groups["x2"].Value;
            var str3 = m.Groups["x3"].Value;
            str2 = GetHtmlStyleText(StyleRemovedTag);
            return str1 + str2 + str3;
        }

        private static string GetHtmlStyleText(DifferencesFormatting style)
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
            else if (string.Compare(style.TextPosition, "Subscript", StringComparison.OrdinalIgnoreCase) == 0)
                str += "vertical-align: sub; ";


            return str;
        }

        #endregion



        public void CreateQualityMetricsReport(string applicationTrackChangesReportPath, string xmlFileFullPath
            , string projectName
            , List<DocumentActivity> documentActivities
            , Activity tpa
            , CompanyProfile cpi)
        {

            var xmlTxtWriter = new XmlTextWriter(xmlFileFullPath, Encoding.UTF8)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };

            xmlTxtWriter.WriteStartDocument(true);

            const string xsltName = "Report.StyleSheet.02.xslt";

            xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + xsltName + "'");
            xmlTxtWriter.WriteComment("Qualitivity by Patrick Hartnett, 2015");

            xmlTxtWriter.WriteStartElement("qualityMetrics");
            xmlTxtWriter.WriteAttributeString("xml:space", "preserve");

            var documentActivitiesDictionary = new Dictionary<string, List<DocumentActivity>>();

            foreach (var documentActivity in documentActivities)
                if (!documentActivitiesDictionary.ContainsKey(documentActivity.DocumentId))
                {

                    documentActivitiesDictionary.Add(documentActivity.DocumentId, new List<DocumentActivity> { documentActivity });
                }
                else
                {
                    documentActivitiesDictionary[documentActivity.DocumentId].Add(documentActivity);
                }

            xmlTxtWriter.WriteStartElement("settings");
            xmlTxtWriter.WriteAttributeString("metricGroupName", tpa.MetricReportSettings.MetricGroupName);
            xmlTxtWriter.WriteAttributeString("maxSeverityValue", tpa.MetricReportSettings.MaxSeverityValue.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("maxSeverityInValue", tpa.MetricReportSettings.MaxSeverityInValue.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("maxSeverityInType", tpa.MetricReportSettings.MaxSeverityInType);
            xmlTxtWriter.WriteEndElement(); //settings



            xmlTxtWriter.WriteStartElement("documents");
            xmlTxtWriter.WriteAttributeString("task", "Translation Quality Assessment");
            xmlTxtWriter.WriteAttributeString("projectName", projectName);
            xmlTxtWriter.WriteAttributeString("clientName", cpi != null ? cpi.Name : "[none]");
            xmlTxtWriter.WriteAttributeString("documentCount", documentActivitiesDictionary.Count.ToString(CultureInfo.InvariantCulture));
            xmlTxtWriter.WriteAttributeString("createdAt", tpa.Stopped.Value.ToString(CultureInfo.InvariantCulture));




            var passed = 0;
            var failed = 0;
            #region  |  get pass/fail totals  |
            foreach (var kvp in documentActivitiesDictionary)
            {
                var documentTotalWords = 0;

                #region  |  get total words  for current document  |
                foreach (var sci in kvp.Value[0].DocumentStateCounters.TranslationMatchTypes)
                {
                    if (string.Compare(sci.Name, "TotalWords", StringComparison.OrdinalIgnoreCase) == 0)
                        documentTotalWords = sci.Value;

                }
                #endregion

                #region  |  get the latest version of entries from the documents  |

                var qmDictTmp = new Dictionary<string, List<QualityMetric>>();
                var qmDictNew = new Dictionary<string, List<QualityMetric>>();



                foreach (var da in kvp.Value)
                {
                    foreach (var record in da.Records)
                    {
                        foreach (var qm in record.QualityMetrics)
                        {
                            if (!qmDictTmp.ContainsKey(qm.Name))
                                qmDictTmp.Add(qm.Name, new List<QualityMetric> { qm });
                            else
                                qmDictTmp[qm.Name].Add(qm);
                        }
                    }
                }


                var qmEntriesNewAll = new List<QualityMetric>();

                foreach (var kvpTmp in qmDictTmp)
                {
                    var qmEntriesNew = new List<QualityMetric>();
                    var qmEntriesTmp = kvpTmp.Value.OrderByDescending(a => a.Modified).ToList();

                    foreach (var qm in qmEntriesTmp)
                        if (!qmEntriesNew.Exists(a => a.Guid == qm.Guid))
                            qmEntriesNew.Add(qm);


                    qmDictNew.Add(kvpTmp.Key, qmEntriesNew);
                    qmEntriesNewAll.AddRange(qmEntriesNew);
                }

                #endregion

                var severityStatusOpenTotal = qmDictNew.Sum(_kvp => _kvp.Value.Where(a => a.Status == QualityMetric.ItemStatus.Open).Sum(a => a.SeverityValue));


                var maximumPenaltyPerWord = Math.Round(Convert.ToDouble(tpa.MetricReportSettings.MaxSeverityValue)
                                                       / Convert.ToDouble(tpa.MetricReportSettings.MaxSeverityInValue), 2);

                var penaltyAllowed = Math.Round(maximumPenaltyPerWord * Convert.ToDouble(documentTotalWords), 2);



                if (severityStatusOpenTotal >= penaltyAllowed)
                    failed++;
                else passed++;

            }
            #endregion


            xmlTxtWriter.WriteAttributeString("assessmentResultAll", failed > 0 ? "FAIL" : "PASS");
            xmlTxtWriter.WriteAttributeString("assessmentResultAllDetails", "(" + passed + " passed, " + failed + " failed)");


            foreach (var kvp in documentActivitiesDictionary)
            {

                var documentTotalWords = 0;
                var documentTotalChars = 0;
                var documentTotalSegments = 0;
                #region  |  get total words  for current document  |


                foreach (var da in kvp.Value)
                {
                    if (da.DocumentId != kvp.Key)
                        continue;

                    foreach (var sci in da.DocumentStateCounters.TranslationMatchTypes)
                    {
                        if (sci.DocumentActivityId == da.Id)
                        {
                            if (string.Compare(sci.Name, "TotalWords", StringComparison.OrdinalIgnoreCase) == 0)
                                documentTotalWords = sci.Value;
                            if (string.Compare(sci.Name, "TotalChars", StringComparison.OrdinalIgnoreCase) == 0)
                                documentTotalChars = sci.Value;
                            if (string.Compare(sci.Name, "Total", StringComparison.OrdinalIgnoreCase) == 0)
                                documentTotalSegments = sci.Value;
                        }
                    }
                }
                #endregion



                #region  |  dates/times  |
                DateTime? dateStart = null;
                DateTime? dateEnd = null;

                foreach (var tca in kvp.Value)
                {
                    if (dateStart.HasValue)
                    {
                        if (tca.Started.Value < dateStart.Value)
                            dateStart = tca.Started;
                    }
                    else
                        dateStart = tca.Started;


                    if (dateEnd.HasValue)
                    {
                        if (tca.Stopped.Value > dateEnd.Value)
                            dateEnd = tca.Stopped;
                    }
                    else
                        dateEnd = tca.Stopped;
                }
                #endregion


                xmlTxtWriter.WriteStartElement("document");

                #region  |  document properties  |
                xmlTxtWriter.WriteAttributeString("id", kvp.Key);
                xmlTxtWriter.WriteAttributeString("documentName", kvp.Value[0].TranslatableDocument.DocumentName);
                xmlTxtWriter.WriteAttributeString("documentSourceLanguage", kvp.Value[0].TranslatableDocument.SourceLanguage);
                xmlTxtWriter.WriteAttributeString("documentTargetlanguage", kvp.Value[0].TranslatableDocument.TargetLanguage);
                xmlTxtWriter.WriteAttributeString("documentActivityType", kvp.Value[0].DocumentActivityType);
                xmlTxtWriter.WriteAttributeString("documentTotalSegments", documentTotalSegments.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("documentTotalWords", documentTotalWords.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("documentTotalChars", documentTotalChars.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("documentActivities", kvp.Value.Count.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("started", dateStart.Value.Year
                                                            + "-" + dateStart.Value.Month.ToString().PadLeft(2, '0')
                                                            + "-" + dateStart.Value.Day.ToString().PadLeft(2, '0')
                                                            + " " + dateStart.Value.Hour.ToString().PadLeft(2, '0')
                                                            + ":" + dateStart.Value.Minute.ToString().PadLeft(2, '0')
                                                            + ":" + dateStart.Value.Second.ToString().PadLeft(2, '0')
                                                            );


                xmlTxtWriter.WriteAttributeString("stopped", dateEnd.Value.Year
                                                            + "-" + dateEnd.Value.Month.ToString().PadLeft(2, '0')
                                                            + "-" + dateEnd.Value.Day.ToString().PadLeft(2, '0')
                                                            + " " + dateEnd.Value.Hour.ToString().PadLeft(2, '0')
                                                            + ":" + dateEnd.Value.Minute.ToString().PadLeft(2, '0')
                                                            + ":" + dateEnd.Value.Second.ToString().PadLeft(2, '0')
                                                            );


                var documentHasTrackChanges = false;
                foreach (var da in kvp.Value)
                {

                    foreach (var record in da.Records)
                    {
                        if (record.QualityMetrics != null)
                        {
                            if (record.ContentSections.TargetUpdatedSections.Any(trgu => trgu.RevisionMarker != null))
                            {
                                documentHasTrackChanges = true;
                            }
                        }
                        if (documentHasTrackChanges)
                            break;
                    }
                    if (documentHasTrackChanges)
                        break;
                }
                xmlTxtWriter.WriteAttributeString("documentHasTrackChanges", documentHasTrackChanges.ToString().ToLower());



                var flagsPath = Path.Combine(applicationTrackChangesReportPath, "Flags");
                var documentSourceLanguageFlagPath = Path.Combine(flagsPath, kvp.Value[0].TranslatableDocument.SourceLanguage + ".gif");
                var documentTargetlanguageFlagPath = Path.Combine(flagsPath, kvp.Value[0].TranslatableDocument.TargetLanguage + ".gif");

                try
                {
                    var imageBase64Source = ImageToBase64(documentSourceLanguageFlagPath);
                    var imageBase64ElementSource = @"data:image/gif;base64, " + imageBase64Source;

                    var imageBase64Target = ImageToBase64(documentTargetlanguageFlagPath);
                    var imageBase64ElementTarget = @"data:image/gif;base64, " + imageBase64Target;

                    xmlTxtWriter.WriteAttributeString("documentSourceLanguageFlag", imageBase64ElementSource);
                    xmlTxtWriter.WriteAttributeString("documentTargetlanguageFlag", imageBase64ElementTarget);
                }
                catch
                {
                    // ignored
                }

                #endregion

                var entriesTotal = 0;
                #region  |  get the latest version of entries from the documents  |

                var qmDictTmp = new Dictionary<string, List<QualityMetric>>();
                var keyValuePairs = new Dictionary<string, List<QualityMetric>>();



                foreach (var da in kvp.Value)
                {
                    foreach (var record in da.Records)
                    {
                        foreach (var qm in record.QualityMetrics)
                        {
                            if (!qmDictTmp.ContainsKey(qm.Name))
                                qmDictTmp.Add(qm.Name, new List<QualityMetric> { qm });
                            else
                                qmDictTmp[qm.Name].Add(qm);
                        }
                    }
                }


                var qmEntriesNewAll = new List<QualityMetric>();

                foreach (var kvpTmp in qmDictTmp)
                {
                    var qmEntriesNew = new List<QualityMetric>();
                    var qmEntriesTmp = kvpTmp.Value.OrderByDescending(a => a.Modified).ToList();

                    foreach (var qm in qmEntriesTmp)
                        if (!qmEntriesNew.Exists(a => a.Guid == qm.Guid))
                            qmEntriesNew.Add(qm);


                    keyValuePairs.Add(kvpTmp.Key, qmEntriesNew);
                    qmEntriesNewAll.AddRange(qmEntriesNew);

                    entriesTotal += qmEntriesNew.Count();
                }

                #endregion



                #region  |  types  |



                var entriesStatusOpenTotal = 0;
                var entriesStatusResolvedTotal = 0;
                var entriesStatusIgnoreTotal = 0;

                var severityStatusOpenTotal = 0;
                var severityStatusResolvedTotal = 0;
                var severityStatusIgnoreTotal = 0;

                foreach (var keyValuePair in keyValuePairs)
                {
                    entriesStatusOpenTotal += keyValuePair.Value.Count(a => a.Status == QualityMetric.ItemStatus.Open);
                    entriesStatusResolvedTotal += keyValuePair.Value.Count(a => a.Status == QualityMetric.ItemStatus.Resolved);
                    entriesStatusIgnoreTotal += keyValuePair.Value.Count(a => a.Status == QualityMetric.ItemStatus.Ignore);

                    severityStatusOpenTotal += keyValuePair.Value.Where(a => a.Status == QualityMetric.ItemStatus.Open).Sum(a => a.SeverityValue);
                    severityStatusResolvedTotal += keyValuePair.Value.Where(a => a.Status == QualityMetric.ItemStatus.Resolved).Sum(a => a.SeverityValue);
                    severityStatusIgnoreTotal += keyValuePair.Value.Where(a => a.Status == QualityMetric.ItemStatus.Ignore).Sum(a => a.SeverityValue);
                }

                xmlTxtWriter.WriteAttributeString("severityTotal", severityStatusOpenTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("severityStatusOpenTotal", severityStatusOpenTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("severityStatusResolvedTotal", severityStatusResolvedTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("severityStatusIgnoreTotal", severityStatusIgnoreTotal.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteAttributeString("entriesTotal", entriesTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("entriesStatusOpenTotal", entriesStatusOpenTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("entriesStatusResolvedTotal", entriesStatusResolvedTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("entriesStatusIgnoreTotal", entriesStatusIgnoreTotal.ToString(CultureInfo.InvariantCulture));

                var maximumPenaltyPerWord = Math.Round(Convert.ToDouble(tpa.MetricReportSettings.MaxSeverityValue)
                                                       / Convert.ToDouble(tpa.MetricReportSettings.MaxSeverityInValue), 2);
                var penaltyAllowed = Math.Round(maximumPenaltyPerWord * Convert.ToDouble(documentTotalWords), 2);



                xmlTxtWriter.WriteAttributeString("penaltyAllowed", penaltyAllowed.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("penaltyApplied", severityStatusOpenTotal.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("assessmentResult", severityStatusOpenTotal >= penaltyAllowed ? "FAIL" : "PASS");

                xmlTxtWriter.WriteAttributeString("maxSeverityValue", tpa.MetricReportSettings.MaxSeverityValue.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("maxSeverityInValue", tpa.MetricReportSettings.MaxSeverityInValue.ToString(CultureInfo.InvariantCulture));

                xmlTxtWriter.WriteStartElement("types");

                var keys = keyValuePairs.Keys.ToList();
                keys.Sort();
                foreach (var key in keys)
                {
                    var qmList = keyValuePairs[key];

                    //check if all have the same type

                    var qmBySeverityType = new Dictionary<string, List<QualityMetric>>();
                    foreach (var qm in qmList)
                    {
                        if (qmBySeverityType.ContainsKey(qm.SeverityName))
                            qmBySeverityType[qm.SeverityName].Add(qm);
                        else
                            qmBySeverityType.Add(qm.SeverityName, new List<QualityMetric> { qm });
                    }

                    foreach (var kvpSeverityLevel in qmBySeverityType)
                    {
                        xmlTxtWriter.WriteStartElement("type");
                        xmlTxtWriter.WriteAttributeString("name", key);
                        xmlTxtWriter.WriteAttributeString("entries", kvpSeverityLevel.Value.Count.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("severityName", kvpSeverityLevel.Value[0].SeverityName);
                        xmlTxtWriter.WriteAttributeString("severity", kvpSeverityLevel.Value[0].SeverityValue.ToString(CultureInfo.InvariantCulture));


                        var entriesStatusOpen = kvpSeverityLevel.Value.Count(a => a.Status == QualityMetric.ItemStatus.Open);
                        var entriesStatusResolved = kvpSeverityLevel.Value.Count(a => a.Status == QualityMetric.ItemStatus.Resolved);
                        var entriesStatusIgnore = kvpSeverityLevel.Value.Count(a => a.Status == QualityMetric.ItemStatus.Ignore);

                        var severityStatusOpen = kvpSeverityLevel.Value.Where(a => a.Status == QualityMetric.ItemStatus.Open
                            && a.SeverityName == kvpSeverityLevel.Key).Sum(a => a.SeverityValue);
                        var severityStatusResolved = kvpSeverityLevel.Value.Where(a => a.Status == QualityMetric.ItemStatus.Resolved
                            && a.SeverityName == kvpSeverityLevel.Key).Sum(a => a.SeverityValue);
                        var severityStatusIgnore = kvpSeverityLevel.Value.Where(a => a.Status == QualityMetric.ItemStatus.Ignore
                            && a.SeverityName == kvpSeverityLevel.Key).Sum(a => a.SeverityValue);

                        xmlTxtWriter.WriteAttributeString("entriesStatusOpen", entriesStatusOpen.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("entriesStatusResolved", entriesStatusResolved.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("entriesStatusIgnore", entriesStatusIgnore.ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("severityStatusOpen", severityStatusOpen.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("severityStatusResolved", severityStatusResolved.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteAttributeString("severityStatusIgnore", severityStatusIgnore.ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteAttributeString("total", severityStatusOpen.ToString(CultureInfo.InvariantCulture));

                        xmlTxtWriter.WriteEndElement(); //type
                    }
                }
                xmlTxtWriter.WriteEndElement(); //types

                #endregion


                var tcc = new TextComparer();

                #region  |  segments  |
                xmlTxtWriter.WriteStartElement("segments");
                xmlTxtWriter.WriteAttributeString("count", entriesTotal.ToString(CultureInfo.InvariantCulture));
                var segmentIds = new List<string>();
                foreach (var da in kvp.Value)
                {
                    foreach (var record in da.Records)
                    {
                        if (!segmentIds.Contains(record.SegmentId.PadLeft(6, '0')))
                            segmentIds.Add(record.SegmentId.PadLeft(6, '0'));
                    }
                }
                segmentIds.Sort();

                foreach (var id in segmentIds)
                {
                    var segmentId = id.TrimStart('0');

                    foreach (var da in kvp.Value)
                    {
                        foreach (var record in da.Records)
                        {
                            if (record.SegmentId != segmentId || record.QualityMetrics == null ||
                                record.QualityMetrics.Count <= 0)
                                continue;
                            foreach (var qm in record.QualityMetrics)
                            {
                                if (!qmEntriesNewAll.Exists(a => a.Id == qm.Id)) continue;
                                var originalTargetText = Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, tpa.ComparisonOptions.IncludeTagsInComparison);
                                var updatedTargetText = Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, tpa.ComparisonOptions.IncludeTagsInComparison);

                                #region  |  segment  |
                                xmlTxtWriter.WriteStartElement("segment");

                                xmlTxtWriter.WriteAttributeString("paragraphId", record.ParagraphId);
                                xmlTxtWriter.WriteAttributeString("segmentId", record.SegmentId);
                                xmlTxtWriter.WriteAttributeString("words", record.WordCount.ToString(CultureInfo.InvariantCulture));

                                xmlTxtWriter.WriteAttributeString("qm_type", qm.Name);
                                xmlTxtWriter.WriteAttributeString("qm_severity_name", qm.SeverityName);
                                xmlTxtWriter.WriteAttributeString("qm_severity_value", qm.SeverityValue.ToString(CultureInfo.InvariantCulture));
                                xmlTxtWriter.WriteAttributeString("qm_status", qm.Status.ToString());
                                xmlTxtWriter.WriteAttributeString("qm_created", qm.Created.Value.Year
                                                                                + "-" + qm.Created.Value.Month.ToString().PadLeft(2, '0')
                                                                                + "-" + qm.Created.Value.Day.ToString().PadLeft(2, '0')
                                                                                + " " + qm.Created.Value.Hour.ToString().PadLeft(2, '0')
                                                                                + ":" + qm.Created.Value.Minute.ToString().PadLeft(2, '0')
                                                                                + ":" + qm.Created.Value.Second.ToString().PadLeft(2, '0')
                                );
                                xmlTxtWriter.WriteAttributeString("qm_modified", qm.Modified.Value.Year
                                                                                 + "-" + qm.Modified.Value.Month.ToString().PadLeft(2, '0')
                                                                                 + "-" + qm.Modified.Value.Day.ToString().PadLeft(2, '0')
                                                                                 + " " + qm.Modified.Value.Hour.ToString().PadLeft(2, '0')
                                                                                 + ":" + qm.Modified.Value.Minute.ToString().PadLeft(2, '0')
                                                                                 + ":" + qm.Modified.Value.Second.ToString().PadLeft(2, '0')
                                );
                                xmlTxtWriter.WriteAttributeString("qm_user_name", qm.UserName);


                                #region  |  source  |


                                xmlTxtWriter.WriteStartElement("source");
                                foreach (var scr in record.ContentSections.SourceSections)
                                {

                                    if (scr.CntType == ContentSection.ContentType.Text)
                                    {
                                        var strList = Helper.GetTextSections(scr.Content);
                                        foreach (var str in strList)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", "text");
                                            xmlTxtWriter.WriteString(str);
                                            xmlTxtWriter.WriteEndElement();//span    
                                        }
                                    }
                                    else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                    {
                                        xmlTxtWriter.WriteStartElement("span");
                                        xmlTxtWriter.WriteAttributeString("class", "tag");
                                        xmlTxtWriter.WriteString(scr.Content);
                                        xmlTxtWriter.WriteEndElement();//span                                    
                                    }

                                }
                                xmlTxtWriter.WriteEndElement();//source


                                #endregion

                                #region  |  target  |


                                xmlTxtWriter.WriteStartElement("target");
                                foreach (var trgo in record.ContentSections.TargetOriginalSections)
                                {
                                    if (trgo.RevisionMarker != null && trgo.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                                    {
                                        //ignore
                                    }
                                    else
                                    {
                                        if (trgo.CntType == ContentSection.ContentType.Text)
                                        {
                                            var strList = Helper.GetTextSections(trgo.Content);
                                            foreach (var str in strList)
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "text");
                                                xmlTxtWriter.WriteString(str);
                                                xmlTxtWriter.WriteEndElement();//span    
                                            }
                                        }
                                        else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", "tag");
                                            xmlTxtWriter.WriteString(trgo.Content);
                                            xmlTxtWriter.WriteEndElement();//span                                    
                                        }
                                    }

                                }
                                xmlTxtWriter.WriteEndElement();//source


                                #endregion

                                #region  |  target updated  |


                                xmlTxtWriter.WriteStartElement("target_updated");
                                foreach (var trgu in record.ContentSections.TargetUpdatedSections)
                                {

                                    if (trgu.RevisionMarker != null && trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                                    {
                                        //ignore
                                    }
                                    else
                                    {
                                        if (trgu.CntType == ContentSection.ContentType.Text)
                                        {
                                            var strList = Helper.GetTextSections(trgu.Content);
                                            foreach (var str in strList)
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "text");
                                                xmlTxtWriter.WriteString(str);
                                                xmlTxtWriter.WriteEndElement();//span    
                                            }
                                        }
                                        else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                        {
                                            xmlTxtWriter.WriteStartElement("span");
                                            xmlTxtWriter.WriteAttributeString("class", "tag");
                                            xmlTxtWriter.WriteString(trgu.Content);
                                            xmlTxtWriter.WriteEndElement();//span                                    
                                        }
                                    }

                                }
                                xmlTxtWriter.WriteEndElement();//source


                                #endregion

                                #region  |  target (Updated) - Revision Markers  |

                                var numberOfRevisionMarkers = record.ContentSections.TargetUpdatedSections.Count(trgu => trgu.RevisionMarker != null);
                                xmlTxtWriter.WriteStartElement("targetUpdatedRevisionMarkers");
                                xmlTxtWriter.WriteAttributeString("count", numberOfRevisionMarkers.ToString(CultureInfo.InvariantCulture));

                                if (numberOfRevisionMarkers > 0)
                                {

                                    foreach (var trgu in record.ContentSections.TargetUpdatedSections)
                                    {
                                        if (trgu.RevisionMarker == null) continue;
                                        xmlTxtWriter.WriteStartElement("revisionMarker");

                                        var dt = trgu.RevisionMarker.Created.Value.Year
                                                 + "-" + trgu.RevisionMarker.Created.Value.Month.ToString().PadLeft(2, '0')
                                                 + "-" + trgu.RevisionMarker.Created.Value.Day.ToString().PadLeft(2, '0')
                                                 + " " + trgu.RevisionMarker.Created.Value.Hour.ToString().PadLeft(2, '0')
                                                 + ":" + trgu.RevisionMarker.Created.Value.Minute.ToString().PadLeft(2, '0')
                                                 + ":" + trgu.RevisionMarker.Created.Value.Second.ToString().PadLeft(2, '0');

                                        xmlTxtWriter.WriteAttributeString("author", trgu.RevisionMarker.Author);
                                        xmlTxtWriter.WriteAttributeString("date", dt);
                                        xmlTxtWriter.WriteAttributeString("revisionType", trgu.RevisionMarker.RevType.ToString());
                                        xmlTxtWriter.WriteStartElement("content");



                                        if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Unchanged)
                                        {
                                            if (trgu.CntType == ContentSection.ContentType.Text)
                                            {

                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "text");
                                                xmlTxtWriter.WriteString(trgu.Content);
                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                            else
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "tag");

                                                xmlTxtWriter.WriteString(trgu.Content);
                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                        }
                                        else if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Insert)
                                        {
                                            if (trgu.CntType == ContentSection.ContentType.Text)
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "textNew");

                                                xmlTxtWriter.WriteString(trgu.Content);

                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                            else
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "tagNew");

                                                xmlTxtWriter.WriteString(trgu.Content);
                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                        }
                                        else if (trgu.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                                        {
                                            if (trgu.CntType == ContentSection.ContentType.Text)
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "textRemoved");

                                                xmlTxtWriter.WriteString(trgu.Content);

                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                            else
                                            {
                                                xmlTxtWriter.WriteStartElement("span");
                                                xmlTxtWriter.WriteAttributeString("class", "tagRemoved");

                                                xmlTxtWriter.WriteString(trgu.Content);
                                                xmlTxtWriter.WriteEndElement();//span
                                            }
                                        }


                                        xmlTxtWriter.WriteEndElement();//content

                                        xmlTxtWriter.WriteEndElement();//revisionMarker
                                    }
                                }


                                xmlTxtWriter.WriteEndElement();//targetUpdatedRevisionMarkers

                                #endregion

                                #region  |  comparison  |


                                xmlTxtWriter.WriteStartElement("comparison");

                                if (originalTargetText != updatedTargetText)
                                {

                                    var tccus = tcc.GetComparisonTextUnits(record.ContentSections.TargetOriginalSections, record.ContentSections.TargetUpdatedSections, tpa.ComparisonOptions.ConsolidateChanges);

                                    foreach (var tccu in tccus)
                                    {
                                        if (tccu.Type == ComparisonUnit.ComparisonType.Identical)
                                        {
                                            foreach (var trgu in tccu.Section)
                                            {

                                                if (trgu.CntType == ContentSection.ContentType.Text)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "text");
                                                    xmlTxtWriter.WriteString(trgu.Content);
                                                    xmlTxtWriter.WriteEndElement();//span
                                                }
                                                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", "tag");

                                                    xmlTxtWriter.WriteString(trgu.Content);
                                                    xmlTxtWriter.WriteEndElement();//span
                                                }

                                            }
                                        }
                                        else
                                        {
                                            foreach (var trgu in tccu.Section)
                                            {
                                                if (trgu.CntType == ContentSection.ContentType.Text)
                                                {
                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", tccu.Type == ComparisonUnit.ComparisonType.New ? "textNew" : "textRemoved");

                                                    xmlTxtWriter.WriteString(trgu.Content);

                                                    xmlTxtWriter.WriteEndElement();//span
                                                }
                                                else if (tpa.ComparisonOptions.IncludeTagsInComparison)
                                                {

                                                    xmlTxtWriter.WriteStartElement("span");
                                                    xmlTxtWriter.WriteAttributeString("class", tccu.Type == ComparisonUnit.ComparisonType.New ? "tagNew" : "tagRemoved");

                                                    xmlTxtWriter.WriteString(trgu.Content);
                                                    xmlTxtWriter.WriteEndElement();//span
                                                }

                                            }
                                        }
                                    }
                                }

                                xmlTxtWriter.WriteEndElement();//comparison


                                #endregion

                                xmlTxtWriter.WriteStartElement("qm_content");
                                xmlTxtWriter.WriteString(qm.Content);
                                xmlTxtWriter.WriteEndElement(); //qm_content
                                xmlTxtWriter.WriteStartElement("qm_comment");
                                xmlTxtWriter.WriteString(qm.Comment);
                                xmlTxtWriter.WriteEndElement(); //qm_comment

                                xmlTxtWriter.WriteEndElement(); //segment

                                #endregion
                            }
                        }
                    }
                }
                xmlTxtWriter.WriteEndElement(); //segments
                #endregion


                xmlTxtWriter.WriteEndElement(); //document
            }
            xmlTxtWriter.WriteEndElement(); //documents


            xmlTxtWriter.WriteEndDocument();//qualityMetrics
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();


            WriteReportResourcesToDirectory(applicationTrackChangesReportPath, false, "02");
            TransformTrackChangesXmlReport(xmlFileFullPath, "02");


        }


        //img src="data:image/jpeg;base64,[data]">
        private string MakeImageSrcData(string filename)
        {
            var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            var filebytes = new byte[fs.Length];
            fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
            return "data:image/png;base64," +
              Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
        }

        public static string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + ":" + dateTime.Minute.ToString().PadLeft(2, '0')
                + ":" + dateTime.Second.ToString().PadLeft(2, '0');
        }


        public string ImageToBase64(Image image, ImageFormat format)
        {

            //System.Drawing.Imaging.ImageFormat.Png
            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                var imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public string ImageToBase64(string filePath)
        {
            var image = Image.FromFile(filePath);

            using (var ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, image.RawFormat);
                var imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            var imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            var image = Image.FromStream(ms, true);
            return image;
        }




        private class CurrencyTotal
        {
            public string Currency { get; set; }
            public int Records { get; set; }
            public double Total { get; set; }
            public double LanguageRateTotal { get; set; }
            public double HourlyRateTotal { get; set; }
            public double CustomRateTotal { get; set; }

            public CurrencyTotal()
            {
                Currency = string.Empty;
                Records = 0;
                Total = 0;
                LanguageRateTotal = 0;
                HourlyRateTotal = 0;
                CustomRateTotal = 0;
            }
        }


        public void CreateActivityReport(string applicationTrackChangesReportPath, string xmlFileFullPath
           , Project project
           , List<Activity> activities
           , CompanyProfile cpi
           , UserProfile upi
           , Dictionary<int, List<DocumentActivity>> documentActivitiesDictionary)
        {
            var xmlTxtWriter = new XmlTextWriter(xmlFileFullPath, Encoding.UTF8)
            {
                Formatting = Formatting.None,
                Indentation = 3,
                Namespaces = false
            };

            xmlTxtWriter.WriteStartDocument(true);

            const string xsltName = "Report.StyleSheet.03.xslt";

            xmlTxtWriter.WriteProcessingInstruction("xml-stylesheet", "type='text/xsl' href='" + xsltName + "'");
            xmlTxtWriter.WriteComment("Qualitivity by Patrick Hartnett, 2015");

            xmlTxtWriter.WriteStartElement("activityRecords");
            xmlTxtWriter.WriteAttributeString("xml:space", "preserve");


            var currencyTotals = new Dictionary<string, CurrencyTotal>();

            #region  |  create currencyTotals dictionary object  |

            foreach (var activity in activities)
            {

                var languageRateTotal = activity.LanguageRateChecked ? activity.DocumentActivityRates.LanguageRateTotal : 0;
                var hourlyRateTotal = activity.HourlyRateChecked ? activity.DocumentActivityRates.HourlyRateTotal : 0;
                var customRateTotal = activity.CustomRateChecked ? activity.DocumentActivityRates.CustomRateTotal : 0;

                var total = (activity.LanguageRateChecked ? activity.DocumentActivityRates.LanguageRateTotal : 0)
                                      + (activity.HourlyRateChecked ? activity.DocumentActivityRates.HourlyRateTotal : 0)
                                      + (activity.CustomRateChecked ? activity.DocumentActivityRates.CustomRateTotal : 0);

                if (currencyTotals.ContainsKey(activity.DocumentActivityRates.HourlyRateCurrency))
                {
                    var ct = currencyTotals[activity.DocumentActivityRates.HourlyRateCurrency];
                    ct.Records++;
                    ct.LanguageRateTotal += languageRateTotal;
                    ct.HourlyRateTotal += hourlyRateTotal;
                    ct.CustomRateTotal += customRateTotal;
                    ct.Total += total;
                }
                else
                {
                    currencyTotals.Add(activity.DocumentActivityRates.HourlyRateCurrency, new CurrencyTotal
                    {
                        Currency = activity.DocumentActivityRates.HourlyRateCurrency,
                        Records = 1,
                        LanguageRateTotal = languageRateTotal,
                        HourlyRateTotal = hourlyRateTotal,
                        CustomRateTotal = customRateTotal,
                        Total = total
                    });
                }
            }
            #endregion

            #region  |  companyProfile  |

            xmlTxtWriter.WriteStartElement("companyProfile");
            if (cpi != null)
            {
                xmlTxtWriter.WriteAttributeString("name", cpi.Name.Trim());
                xmlTxtWriter.WriteAttributeString("street", cpi.Street.Trim());

                var cpiZipCityState = cpi.Zip.Trim() + (cpi.Zip.Trim() != string.Empty ? " " : string.Empty)
               + cpi.City.Trim() + (cpi.City.Trim() != string.Empty ? " " : string.Empty)
               + (cpi.State.Trim() != string.Empty ? " (" + cpi.State.Trim() + ")" : string.Empty);

                xmlTxtWriter.WriteAttributeString("zip_city_state", cpiZipCityState.Trim());

                xmlTxtWriter.WriteAttributeString("zip", cpi.Zip.Trim());
                xmlTxtWriter.WriteAttributeString("city", cpi.City.Trim());
                xmlTxtWriter.WriteAttributeString("state", cpi.State.Trim());
                xmlTxtWriter.WriteAttributeString("country", cpi.Country.Trim());
                xmlTxtWriter.WriteAttributeString("fax", cpi.Fax.Trim());
                xmlTxtWriter.WriteAttributeString("phone", cpi.Phone.Trim());
                xmlTxtWriter.WriteAttributeString("email", cpi.Email.Trim());
                xmlTxtWriter.WriteAttributeString("web", cpi.Web.Trim());
                xmlTxtWriter.WriteAttributeString("tax_code", cpi.TaxCode.Trim());
                xmlTxtWriter.WriteAttributeString("vat_code", cpi.VatCode.Trim());

                xmlTxtWriter.WriteAttributeString("contact_name", cpi.ContactName.Trim());

                xmlTxtWriter.WriteAttributeString("full_address", (cpi.Street.Trim() != string.Empty ? cpi.Street.Trim() + " - " : string.Empty)
                    + (cpiZipCityState.Trim() != string.Empty ? cpiZipCityState.Trim() : string.Empty)
                    + (cpi.Country.Trim() != string.Empty ? " - " + cpi.Country : string.Empty));
            }
            xmlTxtWriter.WriteEndElement(); //companyProfile
            #endregion

            #region  |  userProfile  |

            xmlTxtWriter.WriteStartElement("userProfile");
            xmlTxtWriter.WriteAttributeString("name", upi.Name.Trim());
            xmlTxtWriter.WriteAttributeString("user_name", upi.UserName.Trim());
            xmlTxtWriter.WriteAttributeString("street", upi.Street.Trim());

            var upiZipCityState = upi.Zip.Trim() + (upi.Zip.Trim() != string.Empty ? " " : string.Empty)
                + upi.City.Trim() + (upi.City.Trim() != string.Empty ? " " : string.Empty)
                + (upi.State.Trim() != string.Empty ? " (" + upi.State.Trim() + ")" : string.Empty);

            xmlTxtWriter.WriteAttributeString("zip_city_state", upiZipCityState.Trim());
            xmlTxtWriter.WriteAttributeString("zip", upi.Zip.Trim());
            xmlTxtWriter.WriteAttributeString("city", upi.City.Trim());
            xmlTxtWriter.WriteAttributeString("state", upi.State.Trim());


            xmlTxtWriter.WriteAttributeString("country", upi.Country.Trim());
            xmlTxtWriter.WriteAttributeString("fax", upi.Fax.Trim());
            xmlTxtWriter.WriteAttributeString("phone", upi.Phone.Trim());
            xmlTxtWriter.WriteAttributeString("email", upi.Email.Trim());
            xmlTxtWriter.WriteAttributeString("web", upi.Web.Trim());
            xmlTxtWriter.WriteAttributeString("tax_code", upi.TaxCode.Trim());
            xmlTxtWriter.WriteAttributeString("vat_code", upi.VatCode.Trim());
            xmlTxtWriter.WriteEndElement(); //userProfile
            #endregion

            #region  |  project  |
            xmlTxtWriter.WriteStartElement("project");
            xmlTxtWriter.WriteAttributeString("name", project.Name);
            xmlTxtWriter.WriteAttributeString("description", project.Description);
            xmlTxtWriter.WriteAttributeString("status", project.ProjectStatus);
            xmlTxtWriter.WriteAttributeString("started", project.Started.ToString());
            xmlTxtWriter.WriteAttributeString("due", project.Due.ToString());
            xmlTxtWriter.WriteEndElement(); //project
            #endregion

            #region  |  activity_totals  |

            xmlTxtWriter.WriteStartElement("activity_totals");
            xmlTxtWriter.WriteAttributeString("count", currencyTotals.Values.Count.ToString(CultureInfo.InvariantCulture));
            foreach (var ct in currencyTotals.Values)
            {
                xmlTxtWriter.WriteStartElement("currency_total");
                xmlTxtWriter.WriteAttributeString("currency", ct.Currency);
                xmlTxtWriter.WriteAttributeString("language_rate_total", Math.Round(ct.LanguageRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("hourly_rate_total", Math.Round(ct.HourlyRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("custom_rate_total", Math.Round(ct.CustomRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("total", Math.Round(ct.Total, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("records", ct.Records.ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteEndElement(); //currency_total
            }
            xmlTxtWriter.WriteEndElement(); //activity_totals
            #endregion

            xmlTxtWriter.WriteStartElement("activities");
            xmlTxtWriter.WriteAttributeString("count", activities.Count.ToString(CultureInfo.InvariantCulture));


            foreach (var currency in currencyTotals.Keys)
            {
                xmlTxtWriter.WriteStartElement("currency");
                xmlTxtWriter.WriteAttributeString("currency", currency);
                xmlTxtWriter.WriteAttributeString("language_rate_total", Math.Round(currencyTotals[currency].LanguageRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("hourly_rate_total", Math.Round(currencyTotals[currency].HourlyRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("custom_rate_total", Math.Round(currencyTotals[currency].CustomRateTotal, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("currency_total", Math.Round(currencyTotals[currency].Total, 2).ToString(CultureInfo.InvariantCulture));
                xmlTxtWriter.WriteAttributeString("records", currencyTotals[currency].Records.ToString(CultureInfo.InvariantCulture));
                #region  |  activities ordered by currency  |

                foreach (var activity in activities)
                {
                    if (activity.DocumentActivityRates.HourlyRateCurrency != currency)
                        continue;

                    #region  |  activity  |

                    var documentActivities = new List<DocumentActivity>();
                    if (documentActivitiesDictionary.ContainsKey(activity.Id))
                        documentActivities = documentActivitiesDictionary[activity.Id];

                    var sourceLanguage = string.Empty;
                    var targetLanguage = string.Empty;
                    if (activity.Activities.Count > 0)
                    {
                        sourceLanguage = activity.Activities[0].TranslatableDocument.SourceLanguage;
                        targetLanguage = activity.Activities[0].TranslatableDocument.TargetLanguage;
                    }


                    xmlTxtWriter.WriteStartElement("activity");

                    xmlTxtWriter.WriteAttributeString("id", activity.Id.ToString().PadLeft(4, '0'));
                    xmlTxtWriter.WriteAttributeString("name", activity.Name);
                    xmlTxtWriter.WriteAttributeString("description", activity.Description);
                    xmlTxtWriter.WriteAttributeString("activity_status", activity.ActivityStatus.ToString());
                    xmlTxtWriter.WriteAttributeString("billable", activity.Billable.ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("document_activities_count", activity.Activities.Count.ToString(CultureInfo.InvariantCulture));
                    if (activity.Started != null)
                        xmlTxtWriter.WriteAttributeString("started", GetStringFromDateTime(activity.Started.Value).Replace("T", " "));
                    if (activity.Stopped != null)
                        xmlTxtWriter.WriteAttributeString("stopped", GetStringFromDateTime(activity.Stopped.Value).Replace("T", " "));
                    xmlTxtWriter.WriteAttributeString("source_language", sourceLanguage);
                    xmlTxtWriter.WriteAttributeString("target_language", targetLanguage);


                    xmlTxtWriter.WriteAttributeString("activity_total", Math.Round((activity.LanguageRateChecked ? activity.DocumentActivityRates.LanguageRateTotal : 0)
                                                                                   + (activity.HourlyRateChecked ? activity.DocumentActivityRates.HourlyRateTotal : 0)
                                                                                   + (activity.CustomRateChecked ? activity.DocumentActivityRates.CustomRateTotal : 0), 2).ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("currency", activity.DocumentActivityRates.HourlyRateCurrency);

                    xmlTxtWriter.WriteAttributeString("language_rate_checked", activity.LanguageRateChecked.ToString().ToLower());
                    xmlTxtWriter.WriteAttributeString("language_rate_description", activity.DocumentActivityRates.LanguageRateDescription);
                    xmlTxtWriter.WriteAttributeString("language_rate_name", activity.DocumentActivityRates.LanguageRateName);
                    xmlTxtWriter.WriteAttributeString("language_rate_currency", activity.DocumentActivityRates.LanguageRateCurrency);
                    xmlTxtWriter.WriteAttributeString("language_rate_total", Math.Round(activity.LanguageRateChecked ? activity.DocumentActivityRates.LanguageRateTotal : 0, 2).ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("hourly_rate_checked", activity.HourlyRateChecked.ToString().ToLower());
                    xmlTxtWriter.WriteAttributeString("hourly_rate_description", activity.DocumentActivityRates.HourlyRateDescription);
                    xmlTxtWriter.WriteAttributeString("hourly_rate_quantity", Math.Round(activity.DocumentActivityRates.HourlyRateQuantity, 2).ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("hourly_rate_rate", Math.Round(activity.DocumentActivityRates.HourlyRateRate, 2).ToString(CultureInfo.InvariantCulture));
                    xmlTxtWriter.WriteAttributeString("hourly_rate_currency", activity.DocumentActivityRates.HourlyRateCurrency);
                    xmlTxtWriter.WriteAttributeString("hourly_rate_total", Math.Round(activity.HourlyRateChecked ? activity.DocumentActivityRates.HourlyRateTotal : 0, 2).ToString(CultureInfo.InvariantCulture));

                    xmlTxtWriter.WriteAttributeString("custom_rate_checked", activity.CustomRateChecked.ToString().ToLower());
                    xmlTxtWriter.WriteAttributeString("custom_rate_description", activity.DocumentActivityRates.CustomRateDescription);
                    xmlTxtWriter.WriteAttributeString("custom_rate_currency", activity.DocumentActivityRates.CustomRateCurrency);
                    xmlTxtWriter.WriteAttributeString("custom_rate_total", Math.Round(activity.CustomRateChecked ? activity.DocumentActivityRates.CustomRateTotal : 0, 2).ToString(CultureInfo.InvariantCulture));


                    xmlTxtWriter.WriteStartElement("document_activities");
                    xmlTxtWriter.WriteAttributeString("count", activity.Activities.Count.ToString(CultureInfo.InvariantCulture));
                    foreach (var documentActivity in documentActivities)
                    {

                        var tsActivity = new TimeSpan(documentActivity.TicksActivity);

                        xmlTxtWriter.WriteStartElement("document_activity");
                        xmlTxtWriter.WriteAttributeString("document_name", documentActivity.TranslatableDocument.DocumentName);
                        xmlTxtWriter.WriteAttributeString("source_language", documentActivity.TranslatableDocument.SourceLanguage);
                        xmlTxtWriter.WriteAttributeString("target_language", documentActivity.TranslatableDocument.TargetLanguage);
                        xmlTxtWriter.WriteAttributeString("document_activity_type", documentActivity.DocumentActivityType);
                        xmlTxtWriter.WriteAttributeString("duration", tsActivity.Hours.ToString().PadLeft(2, '0') + ":" + tsActivity.Minutes.ToString().PadLeft(2, '0') + ":" + tsActivity.Seconds.ToString().PadLeft(2, '0'));
                        if (documentActivity.Started != null)
                            xmlTxtWriter.WriteAttributeString("started", GetStringFromDateTime(documentActivity.Started.Value).Replace("T", " "));
                        if (documentActivity.Stopped != null)
                            xmlTxtWriter.WriteAttributeString("stopped", GetStringFromDateTime(documentActivity.Stopped.Value).Replace("T", " "));
                        xmlTxtWriter.WriteAttributeString("records", documentActivity.Records.Count.ToString(CultureInfo.InvariantCulture));
                        xmlTxtWriter.WriteEndElement(); //document_activity
                    }
                    xmlTxtWriter.WriteEndElement(); //document_activities

                    xmlTxtWriter.WriteEndElement(); //activity
                    #endregion
                }
                #endregion

                xmlTxtWriter.WriteEndElement(); //currency
            }


            xmlTxtWriter.WriteEndElement(); //activities


            xmlTxtWriter.WriteEndDocument();
            xmlTxtWriter.Flush();
            xmlTxtWriter.Close();

            InitializeWriteReportImages(applicationTrackChangesReportPath);
            WriteReportResourcesToDirectory(applicationTrackChangesReportPath, false, "03");
            TransformTrackChangesXmlReport(xmlFileFullPath, "03");

        }

        private static void InitializeWriteReportImages(string applicationTrackChangesReportPath)
        {
            var imageNames = new List<string> { "leaf.gif", "plus.gif", "minus.gif" };


            var assembly = Assembly.GetExecutingAssembly();
            foreach (var imageName in imageNames)
            {
                using (var inputStream = assembly.GetManifestResourceStream("Report.Images." + imageName))
                {
                    if (inputStream == null)
                        continue;

                    var outputFilePath = Path.Combine(applicationTrackChangesReportPath, imageName);
                    if (File.Exists(outputFilePath))
                        continue;

                    Stream outputStream = File.Open(outputFilePath, FileMode.Create);

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
        }


    }
}
