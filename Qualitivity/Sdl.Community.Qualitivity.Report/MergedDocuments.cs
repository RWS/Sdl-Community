using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Comparison;
using Sdl.Community.Structures.Documents;
using Sdl.Community.Structures.Documents.Records;
using Sdl.Community.Structures.Projects.Activities;
using Sdl.Community.Structures.Rates.Helpers;

namespace Sdl.Community.Report
{
    public class MergedDocuments
    {
        #region  |  variables status  |

        public int TotalNotTranslatedOriginal { get; set; }
        public int TotalDraftOriginal { get; set; }
        public int TotalTranslatedOriginal { get; set; }
        public int TotalTranslationRejectedOriginal { get; set; }
        public int TotalTranslationApprovedOriginal { get; set; }
        public int TotalSignOffRejectedOriginal { get; set; }
        public int TotalSignedOffOriginal { get; set; }


        public int TotalNotTranslatedUpdated { get; set; }
        public int TotalDraftUpdated { get; set; }
        public int TotalTranslatedUpdated { get; set; }
        public int TotalTranslationRejectedUpdated { get; set; }
        public int TotalTranslationApprovedUpdated { get; set; }
        public int TotalSignOffRejectedUpdated { get; set; }
        public int TotalSignedOffUpdated { get; set; }


        #endregion

        #region  |  variables main  |

        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string DocumentName { get; set; }
        public string DocumentSourceLanguage { get; set; }
        public string DocumentTargetLanguage { get; set; }
        public string DocumentActivityType { get; set; }

        public TimeSpan DocumentTotalElapsedTime { get; set; }

        public decimal DocumentTotalSegmentContentUpdated { get; set; }
        public decimal DocumentTotalSegmentStatusUpdated { get; set; }
        public decimal DocumentTotalSegmentCommentsUpdated { get; set; }

        public decimal DocumentTotalSegmentContentUpdatedWords { get; set; }
        public decimal DocumentTotalSegmentStatusUpdatedWords { get; set; }
        public decimal DocumentTotalSegmentCommentsUpdatedWords { get; set; }

        public decimal DocumentTotalSegmentContentUpdatedPercentage { get; set; }
        public decimal DocumentTotalSegmentStatusUpdatedPercentage { get; set; }
        public decimal DocumentTotalSegmentCommentsUpdatedPercentage { get; set; }


        public decimal TotalChangesPmSegments { get; set; }
        public decimal TotalChangesPmWords { get; set; }
        public decimal TotalChangesPmCharacters { get; set; }
        public decimal TotalChangesPmTags { get; set; }





        public decimal TotalChangesCmSegments { get; set; }
        public decimal TotalChangesCmWords { get; set; }
        public decimal TotalChangesCmCharacters { get; set; }
        public decimal TotalChangesCmTags { get; set; }





        public decimal TotalChangesExactSegments { get; set; }
        public decimal TotalChangesExactWords { get; set; }
        public decimal TotalChangesExactCharacters { get; set; }
        public decimal TotalChangesExactTags { get; set; }


        public decimal TotalChangesRepsSegments { get; set; }
        public decimal TotalChangesRepsWords { get; set; }
        public decimal TotalChangesRepsCharacters { get; set; }
        public decimal TotalChangesRepsTags { get; set; }


        public decimal TotalChangesAtSegments { get; set; }
        public decimal TotalChangesAtWords { get; set; }
        public decimal TotalChangesAtCharacters { get; set; }
        public decimal TotalChangesAtTags { get; set; }


        public decimal TotalChangesFuzzySegments { get; set; }
        public decimal TotalChangesFuzzyWords { get; set; }
        public decimal TotalChangesFuzzyCharacters { get; set; }
        public decimal TotalChangesFuzzyTags { get; set; }


        public decimal TotalChangesFuzzy99Segments { get; set; }
        public decimal TotalChangesFuzzy99Words { get; set; }
        public decimal TotalChangesFuzzy99Characters { get; set; }
        public decimal TotalChangesFuzzy99Tags { get; set; }


        public decimal TotalChangesFuzzy94Segments { get; set; }
        public decimal TotalChangesFuzzy94Words { get; set; }
        public decimal TotalChangesFuzzy94Characters { get; set; }
        public decimal TotalChangesFuzzy94Tags { get; set; }


        public decimal TotalChangesFuzzy84Segments { get; set; }
        public decimal TotalChangesFuzzy84Words { get; set; }
        public decimal TotalChangesFuzzy84Characters { get; set; }
        public decimal TotalChangesFuzzy84Tags { get; set; }

        public decimal TotalChangesFuzzy74Segments { get; set; }
        public decimal TotalChangesFuzzy74Words { get; set; }
        public decimal TotalChangesFuzzy74Characters { get; set; }
        public decimal TotalChangesFuzzy74Tags { get; set; }


        public decimal TotalChangesNewSegments { get; set; }
        public decimal TotalChangesNewWords { get; set; }
        public decimal TotalChangesNewCharacters { get; set; }
        public decimal TotalChangesNewTags { get; set; }


        public decimal TotalChangesTotalSegments { get; set; }
        public decimal TotalChangesTotalWords { get; set; }
        public decimal TotalChangesTotalCharacters { get; set; }
        public decimal TotalChangesTotalTags { get; set; }



        public decimal TotalSourcePmSegments { get; set; }
        public decimal TotalSourceCmSegments { get; set; }
        public decimal TotalSourceExactSegments { get; set; }
        public decimal TotalSourceRepsSegments { get; set; }
        public decimal TotalSourceAtSegments { get; set; }
        public decimal TotalSourceFuzzySegments { get; set; }
        public decimal TotalSourceFuzzy99Segments { get; set; }
        public decimal TotalSourceFuzzy94Segments { get; set; }
        public decimal TotalSourceFuzzy84Segments { get; set; }
        public decimal TotalSourceFuzzy74Segments { get; set; }
        public decimal TotalSourceNewSegments { get; set; }
        public decimal TotalSourceSegments { get; set; }

        public decimal TotalSourcePmWords { get; set; }
        public decimal TotalSourceCmWords { get; set; }
        public decimal TotalSourceExactWords { get; set; }
        public decimal TotalSourceRepsWords { get; set; }
        public decimal TotalSourceAtWords { get; set; }
        public decimal TotalSourceFuzzyWords { get; set; }
        public decimal TotalSourceFuzzy99Words { get; set; }
        public decimal TotalSourceFuzzy94Words { get; set; }
        public decimal TotalSourceFuzzy84Words { get; set; }
        public decimal TotalSourceFuzzy74Words { get; set; }
        public decimal TotalSourceNewWords { get; set; }       
        public decimal TotalSourceWords { get; set; }


        public decimal TotalSourcePmCharacters { get; set; }
        public decimal TotalSourceCmCharacters { get; set; }
        public decimal TotalSourceExactCharacters { get; set; }
        public decimal TotalSourceRepsCharacters { get; set; }
        public decimal TotalSourceAtCharacters { get; set; }
        public decimal TotalSourceFuzzyCharacters { get; set; }
        public decimal TotalSourceFuzzy99Characters { get; set; }
        public decimal TotalSourceFuzzy94Characters { get; set; }
        public decimal TotalSourceFuzzy84Characters { get; set; }
        public decimal TotalSourceFuzzy74Characters { get; set; }
        public decimal TotalSourceNewCharacters { get; set; }
        public decimal TotalSourceCharacters { get; set; }


        public decimal TotalSourcePmTags { get; set; }
        public decimal TotalSourceCmTags { get; set; }
        public decimal TotalSourceExactTags { get; set; }
        public decimal TotalSourceRepsTags { get; set; }
        public decimal TotalSourceAtTags { get; set; }
        public decimal TotalSourceFuzzyTags { get; set; }
        public decimal TotalSourceFuzzy99Tags { get; set; }
        public decimal TotalSourceFuzzy94Tags { get; set; }
        public decimal TotalSourceFuzzy84Tags { get; set; }
        public decimal TotalSourceFuzzy74Tags { get; set; }
        public decimal TotalSourceNewTags { get; set; }
        public decimal TotalSourceTags { get; set; }


        #endregion

        #region  |  variables demaru levenstein |

        public decimal ExactSegments { get; set; }
        public decimal ExactWords { get; set; }
        public decimal ExactCharacters { get; set; }
        public decimal ExactPercent { get; set; }
        public decimal ExactTags { get; set; }

        public decimal Fuzzy99Segments { get; set; }
        public decimal Fuzzy99Words { get; set; }
        public decimal Fuzzy99Characters { get; set; }
        public decimal Fuzzy99Percent { get; set; }
        public decimal Fuzzy99Tags { get; set; }

        public decimal Fuzzy94Segments { get; set; }
        public decimal Fuzzy94Words { get; set; }
        public decimal Fuzzy94Characters { get; set; }
        public decimal Fuzzy94Percent { get; set; }
        public decimal Fuzzy94Tags { get; set; }

        public decimal Fuzzy84Segments { get; set; }
        public decimal Fuzzy84Words { get; set; }
        public decimal Fuzzy84Characters { get; set; }
        public decimal Fuzzy84Percent { get; set; }
        public decimal Fuzzy84Tags { get; set; }

        public decimal Fuzzy74Segments { get; set; }
        public decimal Fuzzy74Words { get; set; }
        public decimal Fuzzy74Characters { get; set; }
        public decimal Fuzzy74Percent { get; set; }
        public decimal Fuzzy74Tags { get; set; }

        public decimal NewSegments { get; set; }
        public decimal NewWords { get; set; }
        public decimal NewCharacters { get; set; }
        public decimal NewPercent { get; set; }
        public decimal NewTags { get; set; }


        public decimal TotalSegments { get; set; }
        public decimal TotalWords { get; set; }
        public decimal TotalCharacters { get; set; }
        public decimal TotalPercent { get; set; }
        public decimal TotalTags { get; set; }


        public decimal PostEditPriceExactWords { get; set; }
        public decimal PostEditPriceFuzzy9995Words { get; set; }
        public decimal PostEditPriceFuzzy9485Words { get; set; }
        public decimal PostEditPriceFuzzy8475Words { get; set; }
        public decimal PostEditPriceFuzzy7450Words { get; set; }
        public decimal PostEditPriceNewWords { get; set; }

        public decimal PostEditPriceExactTotal { get; set; }
        public decimal PostEditPriceFuzzy9995Total { get; set; }
        public decimal PostEditPriceFuzzy9485Total { get; set; }
        public decimal PostEditPriceFuzzy8475Total { get; set; }
        public decimal PostEditPriceFuzzy7450Total { get; set; }
        public decimal PostEditPriceNewTotal { get; set; }
        public decimal PostEditPriceTotal { get; set; }


        public string PostEditPriceCurrency { get; set; }

        public LanguageRate LanguageRate { get; set; }

        #endregion

        public Dictionary<string, Record> RecordsDictionary { get; set; }
        public Dictionary<string, DocumentAnalysisMerged> DocumentAnalysisMergedDictionary { get; set; }
        public Dictionary<string, PEMp> PempDictionary { get; set; }

        public Dictionary<string, List<ContentSection>> SegmentPreviousTrackChanges { get; set; }
        public bool DocumentHasTrackChanges { get; set; }

        public List<QualityMetric> QualityMetrics { get; set; }

        public MergedDocuments(List<DocumentActivity> documentActivityList
            , Activity projectActivity
            , TextComparer comparer
            , ActivityRates activityRates)
        {
           
            TotalNotTranslatedOriginal = 0;
            TotalDraftOriginal = 0;
            TotalTranslatedOriginal = 0;
            TotalTranslationRejectedOriginal = 0;
            TotalTranslationApprovedOriginal = 0;
            TotalSignOffRejectedOriginal = 0;
            TotalSignedOffOriginal = 0;


            TotalNotTranslatedUpdated = 0;
            TotalDraftUpdated = 0;
            TotalTranslatedUpdated = 0;
            TotalTranslationRejectedUpdated = 0;
            TotalTranslationApprovedUpdated = 0;
            TotalSignOffRejectedUpdated = 0;
            TotalSignedOffUpdated = 0;


            DateStart = null;
            DateEnd = null;
            DocumentName = string.Empty;
            DocumentSourceLanguage = string.Empty;
            DocumentTargetLanguage = string.Empty;
            DocumentActivityType = string.Empty;

            DocumentTotalElapsedTime = new TimeSpan();

            DocumentTotalSegmentContentUpdated = 0;
            DocumentTotalSegmentStatusUpdated = 0;
            DocumentTotalSegmentCommentsUpdated = 0;

            DocumentTotalSegmentContentUpdatedWords = 0;
            DocumentTotalSegmentStatusUpdatedWords = 0;
            DocumentTotalSegmentCommentsUpdatedWords = 0;

            DocumentTotalSegmentContentUpdatedPercentage = 0;
            DocumentTotalSegmentStatusUpdatedPercentage = 0;
            DocumentTotalSegmentCommentsUpdatedPercentage = 0;


            #region  |  variables main  |

        

            TotalChangesPmSegments = 0;
            TotalChangesPmWords = 0;
            TotalChangesPmCharacters = 0;
            TotalChangesPmTags = 0;

            TotalChangesCmSegments = 0;
            TotalChangesCmWords = 0;
            TotalChangesCmCharacters = 0;
            TotalChangesCmTags = 0;

            TotalChangesExactSegments = 0;
            TotalChangesExactWords = 0;
            TotalChangesExactCharacters = 0;
            TotalChangesExactTags = 0;

            TotalChangesRepsSegments = 0;
            TotalChangesRepsWords = 0;
            TotalChangesRepsCharacters = 0;
            TotalChangesRepsTags = 0;

            TotalChangesAtSegments = 0;
            TotalChangesAtWords = 0;
            TotalChangesAtCharacters = 0;
            TotalChangesAtTags = 0;

            TotalChangesFuzzySegments = 0;
            TotalChangesFuzzyWords = 0;
            TotalChangesFuzzyCharacters = 0;
            TotalChangesFuzzyTags = 0;

            TotalChangesFuzzy99Segments = 0;
            TotalChangesFuzzy99Words = 0;
            TotalChangesFuzzy99Characters = 0;
            TotalChangesFuzzy99Tags = 0;

            TotalChangesFuzzy94Segments = 0;
            TotalChangesFuzzy94Words = 0;
            TotalChangesFuzzy94Characters = 0;
            TotalChangesFuzzy94Tags = 0;

            TotalChangesFuzzy84Segments = 0;
            TotalChangesFuzzy84Words = 0;
            TotalChangesFuzzy84Characters = 0;
            TotalChangesFuzzy84Tags = 0;

            TotalChangesFuzzy74Segments = 0;
            TotalChangesFuzzy74Words = 0;
            TotalChangesFuzzy74Characters = 0;
            TotalChangesFuzzy74Tags = 0;

            TotalChangesNewSegments = 0;
            TotalChangesNewWords = 0;
            TotalChangesNewCharacters = 0;
            TotalChangesNewTags = 0;

            TotalChangesTotalSegments = 0;
            TotalChangesTotalWords = 0;
            TotalChangesTotalCharacters = 0;
            TotalChangesTotalTags = 0;

       

            TotalSourceSegments = 0;
            TotalSourcePmSegments = 0;
            TotalSourceCmSegments = 0;
            TotalSourceExactSegments = 0;
            TotalSourceFuzzySegments = 0;
            TotalSourceFuzzy99Segments = 0;
            TotalSourceFuzzy94Segments = 0;
            TotalSourceFuzzy84Segments = 0;
            TotalSourceFuzzy74Segments = 0;
            TotalSourceNewSegments = 0;
            TotalSourceAtSegments = 0;


            TotalSourceWords = 0;
            TotalSourcePmWords = 0;
            TotalSourceCmWords = 0;
            TotalSourceExactWords = 0;
            TotalSourceFuzzyWords = 0;
            TotalSourceFuzzy99Words = 0;
            TotalSourceFuzzy94Words = 0;
            TotalSourceFuzzy84Words = 0;
            TotalSourceFuzzy74Words = 0;
            TotalSourceNewWords = 0;
            TotalSourceAtWords = 0;


            TotalSourceCharacters = 0;
            TotalSourcePmCharacters = 0;
            TotalSourceCmCharacters = 0;
            TotalSourceExactCharacters = 0;
            TotalSourceFuzzyCharacters = 0;
            TotalSourceFuzzy99Characters = 0;
            TotalSourceFuzzy94Characters = 0;
            TotalSourceFuzzy84Characters = 0;
            TotalSourceFuzzy74Characters = 0;
            TotalSourceNewCharacters = 0;
            TotalSourceAtCharacters = 0;


            TotalSourceTags = 0;
            TotalSourcePmTags = 0;
            TotalSourceCmTags = 0;
            TotalSourceExactTags = 0;
            TotalSourceFuzzyTags = 0;
            TotalSourceFuzzy99Tags = 0;
            TotalSourceFuzzy94Tags = 0;
            TotalSourceFuzzy84Tags = 0;
            TotalSourceFuzzy74Tags = 0;
            TotalSourceNewTags = 0;
            TotalSourceAtTags = 0;


            #endregion

            #region  |  get totals  |

            RecordsDictionary = new Dictionary<string, Record>();
            DocumentAnalysisMergedDictionary = new Dictionary<string, DocumentAnalysisMerged>();
            PempDictionary = new Dictionary<string, PEMp>();
            SegmentPreviousTrackChanges = new Dictionary<string, List<ContentSection>>();

            var recoveredTctmto = false;
            var recoveredTccsos = false;     

            foreach (var documentActivity in documentActivityList.OrderBy(a => a.Started.Value))
            {

                #region  |  translation_match_types |
                if (!recoveredTctmto && documentActivity.DocumentStateCounters.TranslationMatchTypes != null && documentActivity.DocumentStateCounters.TranslationMatchTypes.Count > 0)
                {
                    recoveredTctmto = true;
                    foreach (var stateCountItem in documentActivity.DocumentStateCounters.TranslationMatchTypes)
                    {
                        if (string.Compare(stateCountItem.Name, "PM", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourcePmSegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "CM", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceCmSegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "REPS", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceRepsSegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Exact", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceExactSegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzySegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy99", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy99Segments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy94", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy94Segments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy84", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy84Segments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy74", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy74Segments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "New", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceNewSegments = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "AT", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceAtSegments = stateCountItem.Value; }

                        if (string.Compare(stateCountItem.Name, "PMWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourcePmWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "CMWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceCmWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "REPSWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceRepsWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ExactWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceExactWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "FuzzyWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzyWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy99Words", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy99Words = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy94Words", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy94Words = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy84Words", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy84Words = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy74Words", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy74Words = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "NewWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceNewWords = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ATWords", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceAtWords = stateCountItem.Value; }


                        if (string.Compare(stateCountItem.Name, "PMCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourcePmCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "CMCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceCmCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "REPSCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceRepsCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ExactCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceExactCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "FuzzyCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzyCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy99Characters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy99Characters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy94Characters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy94Characters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy84Characters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy84Characters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy74Characters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy74Characters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "NewCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceNewCharacters = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ATCharacters", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceAtCharacters = stateCountItem.Value; }


                        if (string.Compare(stateCountItem.Name, "PMTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourcePmTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "CMTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceCmTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "REPSTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceRepsTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ExactTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceExactTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "FuzzyTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzyTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy99Tags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy99Tags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy94Tags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy94Tags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy84Tags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy84Tags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "Fuzzy74Tags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceFuzzy74Tags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "NewTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceNewTags = stateCountItem.Value; }
                        if (string.Compare(stateCountItem.Name, "ATTags", StringComparison.OrdinalIgnoreCase) == 0)
                        { TotalSourceAtTags = stateCountItem.Value; }
                    }
                }

                TotalSourceSegments = TotalSourcePmSegments
                                      + TotalSourceCmSegments
                                      + TotalSourceRepsSegments
                                      + TotalSourceExactSegments
                                      + TotalSourceFuzzySegments
                                      + TotalSourceNewSegments
                                      + TotalSourceAtSegments;

                TotalSourceWords = TotalSourcePmWords
                                      + TotalSourceCmWords
                                      + TotalSourceRepsWords
                                      + TotalSourceExactWords
                                      + TotalSourceFuzzyWords
                                      + TotalSourceNewWords
                                      + TotalSourceAtWords;


                TotalSourceCharacters = TotalSourcePmCharacters
                                     + TotalSourceCmCharacters
                                     + TotalSourceRepsCharacters
                                     + TotalSourceExactCharacters
                                     + TotalSourceFuzzyCharacters
                                     + TotalSourceNewCharacters
                                     + TotalSourceAtCharacters;


                TotalSourceTags = TotalSourcePmTags
                                     + TotalSourceCmTags
                                     + TotalSourceRepsTags
                                     + TotalSourceExactTags
                                     + TotalSourceFuzzyTags
                                     + TotalSourceNewTags
                                     + TotalSourceAtTags;
                #endregion

                #region  |  confirmation_statuses  |

                if (!recoveredTccsos && documentActivity.DocumentStateCounters.ConfirmationStatuses != null && documentActivity.DocumentStateCounters.ConfirmationStatuses.Count > 0)
                {
                    recoveredTccsos = true;
                    foreach (var tccso in documentActivity.DocumentStateCounters.ConfirmationStatuses)
                    {
                        if (string.Compare(tccso.Name, "NotTranslated", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalNotTranslatedOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "Draft", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalDraftOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "Translated", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalTranslatedOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "TranslationRejected", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalTranslationRejectedOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "TranslationApproved", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalTranslationApprovedOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "SignOffRejected", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalSignOffRejectedOriginal = tccso.Value;
                        if (string.Compare(tccso.Name, "SignedOff", StringComparison.OrdinalIgnoreCase) == 0)
                            TotalSignedOffOriginal = tccso.Value;
                    }
                }
                #endregion

       


                DocumentName = documentActivity.TranslatableDocument.DocumentName;
                DocumentSourceLanguage = documentActivity.TranslatableDocument.SourceLanguage;
                DocumentTargetLanguage = documentActivity.TranslatableDocument.TargetLanguage;

              

                foreach (var record in documentActivity.Records.OrderBy(a => a.Started.Value))
                {
                    var compiledTargetOriginal = Helper.GetCompiledSegmentText(record.ContentSections.TargetOriginalSections, projectActivity.ComparisonOptions.IncludeTagsInComparison);
                    var compiledTargetUpdated = Helper.GetCompiledSegmentText(record.ContentSections.TargetUpdatedSections, projectActivity.ComparisonOptions.IncludeTagsInComparison);


                    if (!RecordsDictionary.ContainsKey(record.ParagraphId + "_" + record.SegmentId))
                    {
                        var tcrClone = (Record)record.Clone();
                        tcrClone.MergedDatesTemp.Add(record.Stopped.ToString());

                        RecordsDictionary.Add(record.ParagraphId + "_" + record.SegmentId, tcrClone);
                        var documentAnalysisMerged = new DocumentAnalysisMerged();

                        #region  |  common  |

                     
                        TotalChangesTotalSegments++;
                        TotalChangesTotalWords += record.WordCount;
                        TotalChangesTotalCharacters += record.CharsCount;
                        TotalChangesTotalTags += record.TagsCount + record.PlaceablesCount;                        

                     
                        #region  |  tcr.translationOrigins.original.translationStatus  |

                        if ( string.Compare(record.TranslationOrigins.Original.OriginType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            TotalChangesRepsSegments++;
                            TotalChangesRepsWords += record.WordCount;
                            TotalChangesRepsCharacters += record.CharsCount;
                            TotalChangesRepsTags += record.TagsCount + record.PlaceablesCount;
                        }
                        else
                        {
                            switch (record.TranslationOrigins.Original.TranslationStatus.ToUpper())
                            {
                                case "PM":
                                {
                                    TotalChangesPmSegments++;
                                    TotalChangesPmWords += record.WordCount;
                                    TotalChangesPmCharacters += record.CharsCount;
                                    TotalChangesPmTags += record.TagsCount + record.PlaceablesCount;
                                    break;
                                }
                                case "CM":
                                {
                                    TotalChangesCmSegments++;
                                    TotalChangesCmWords += record.WordCount;
                                    TotalChangesCmCharacters += record.CharsCount;
                                    TotalChangesCmTags += record.TagsCount + record.PlaceablesCount;
                                    break;
                                }
                                case "AT":
                                {
                                    TotalChangesAtSegments++;
                                    TotalChangesAtWords += record.WordCount;
                                    TotalChangesAtCharacters += record.CharsCount;
                                    TotalChangesAtTags += record.TagsCount + record.PlaceablesCount;
                                    break;
                                }
                                case "REPS":
                                {
                                    TotalChangesRepsSegments++;
                                    TotalChangesRepsWords += record.WordCount;
                                    TotalChangesRepsCharacters += record.CharsCount;
                                    TotalChangesRepsTags += record.TagsCount + record.PlaceablesCount;
                                    break;
                                }
                                case "100%":
                                {
                                    TotalChangesExactSegments++;
                                    TotalChangesExactWords += record.WordCount;
                                    TotalChangesExactCharacters += record.CharsCount;
                                    TotalChangesExactTags += record.TagsCount + record.PlaceablesCount;
                                    break;
                                }
                                default:
                                {
                                    var matchPercentage =
                                        GetMatchValue(record.TranslationOrigins.Original.TranslationStatus);

                                    if (matchPercentage >= 100)
                                    {
                                        TotalChangesExactSegments++;
                                        TotalChangesExactWords += record.WordCount;
                                        TotalChangesExactCharacters += record.CharsCount;
                                        TotalChangesExactTags += record.TagsCount + record.PlaceablesCount;
                                    }
                                    else if (matchPercentage >= 95 && matchPercentage < 100)
                                    {
                                        TotalChangesFuzzySegments++;
                                        TotalChangesFuzzyWords += record.WordCount;
                                        TotalChangesFuzzyCharacters += record.CharsCount;
                                        TotalChangesFuzzyTags += record.TagsCount + record.PlaceablesCount;

                                        TotalChangesFuzzy99Segments++;
                                        TotalChangesFuzzy99Words += record.WordCount;
                                        TotalChangesFuzzy99Characters += record.CharsCount;
                                        TotalChangesFuzzy99Tags += record.TagsCount + record.PlaceablesCount;
                                    }
                                    else if (matchPercentage >= 85 && matchPercentage < 95)
                                    {
                                        TotalChangesFuzzySegments++;
                                        TotalChangesFuzzyWords += record.WordCount;
                                        TotalChangesFuzzyCharacters += record.CharsCount;
                                        TotalChangesFuzzyTags += record.TagsCount + record.PlaceablesCount;

                                        TotalChangesFuzzy94Segments++;
                                        TotalChangesFuzzy94Words += record.WordCount;
                                        TotalChangesFuzzy94Characters += record.CharsCount;
                                        TotalChangesFuzzy94Tags += record.TagsCount + record.PlaceablesCount;
                                    }
                                    else if (matchPercentage >= 75 && matchPercentage < 85)
                                    {
                                        TotalChangesFuzzySegments++;
                                        TotalChangesFuzzyWords += record.WordCount;
                                        TotalChangesFuzzyCharacters += record.CharsCount;
                                        TotalChangesFuzzyTags += record.TagsCount + record.PlaceablesCount;

                                        TotalChangesFuzzy84Segments++;
                                        TotalChangesFuzzy84Words += record.WordCount;
                                        TotalChangesFuzzy84Characters += record.CharsCount;
                                        TotalChangesFuzzy84Tags += record.TagsCount + record.PlaceablesCount;
                                    }
                                    else if (matchPercentage >= 50 && matchPercentage < 75)
                                    {
                                        TotalChangesFuzzySegments++;
                                        TotalChangesFuzzyWords += record.WordCount;
                                        TotalChangesFuzzyCharacters += record.CharsCount;
                                        TotalChangesFuzzyTags += record.TagsCount + record.PlaceablesCount;

                                        TotalChangesFuzzy74Segments++;
                                        TotalChangesFuzzy74Words += record.WordCount;
                                        TotalChangesFuzzy74Characters += record.CharsCount;
                                        TotalChangesFuzzy74Tags += record.TagsCount + record.PlaceablesCount;
                                    }
                                    else
                                    {
                                        TotalChangesNewSegments++;
                                        TotalChangesNewWords += record.WordCount;
                                        TotalChangesNewCharacters += record.CharsCount;
                                        TotalChangesNewTags += record.TagsCount + record.PlaceablesCount;
                                    }

                                    break;
                                }
                            }
                        }

                        #endregion


                        if (compiledTargetOriginal != compiledTargetUpdated)
                        {
                            DocumentTotalSegmentContentUpdated++;
                            DocumentTotalSegmentContentUpdatedWords += record.WordCount;

                            documentAnalysisMerged.TranslationChangesAdded = true;
                        }
                        if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            DocumentTotalSegmentStatusUpdated++;
                            DocumentTotalSegmentStatusUpdatedWords += record.WordCount;

                            documentAnalysisMerged.StatusChangeAdded = true;
                        }
                        if (record.Comments.Count > 0)
                        {
                            DocumentTotalSegmentCommentsUpdated++;
                            DocumentTotalSegmentCommentsUpdatedWords += record.WordCount;

                            documentAnalysisMerged.CommentChangeAdded = true;
                        }

                        DocumentAnalysisMergedDictionary.Add(record.ParagraphId + "_" + record.SegmentId, documentAnalysisMerged);

                        #endregion
                    }
                    else
                    {
                        #region  |  common  |

                        var tcr = RecordsDictionary[record.ParagraphId + "_" + record.SegmentId];

                        if (record.QualityMetrics.Count > 0)
                            tcr.QualityMetrics.AddRange(record.QualityMetrics);

                        tcr.MergedDatesTemp.Add(record.Stopped.ToString());

                        var documentAnalysisMerged = DocumentAnalysisMergedDictionary[record.ParagraphId + "_" + record.SegmentId];

                        //_tcr.started = tcr.started;
                        tcr.Stopped = record.Stopped;

                        tcr.TranslationOrigins.Original.TranslationStatus = record.TranslationOrigins.Original.TranslationStatus;
                        tcr.TranslationOrigins.Updated.TranslationStatus = record.TranslationOrigins.Updated.TranslationStatus;

                        if ((compiledTargetOriginal != compiledTargetUpdated) && !documentAnalysisMerged.TranslationChangesAdded)
                        {
                            DocumentTotalSegmentContentUpdated++;
                            DocumentTotalSegmentContentUpdatedWords += record.WordCount;

                            documentAnalysisMerged.TranslationChangesAdded = true;                            
                        }
                        if (compiledTargetOriginal != compiledTargetUpdated)
                        {
                            //add the previous changes if exist
                            #region  |  add the previous changes  |
                            var css = (from cs in tcr.ContentSections.TargetUpdatedSections
                                       where cs.RevisionMarker != null
                                       select cs.Clone() as ContentSection).ToList();
                            if (css.Count > 0)
                            {
                                if (SegmentPreviousTrackChanges.ContainsKey(record.ParagraphId + "_" + record.SegmentId))
                                    SegmentPreviousTrackChanges[record.ParagraphId + "_" + record.SegmentId].AddRange(css);
                                else
                                    SegmentPreviousTrackChanges.Add(record.ParagraphId + "_" + record.SegmentId, css);
                            }
                            #endregion
                            //always get the last translation change                                                    
                            tcr.ContentSections.TargetUpdatedSections = new List<ContentSection>();
                            foreach (var tcrs in record.ContentSections.TargetUpdatedSections)
                                tcr.ContentSections.TargetUpdatedSections.Add((ContentSection)tcrs.Clone());
                        }
                        if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) != 0 && !documentAnalysisMerged.StatusChangeAdded)
                        {
                            DocumentTotalSegmentStatusUpdated++;
                            DocumentTotalSegmentStatusUpdatedWords += record.WordCount;

                            documentAnalysisMerged.StatusChangeAdded = true;
                        }
                        if (string.Compare(record.TranslationOrigins.Original.ConfirmationLevel, record.TranslationOrigins.Updated.ConfirmationLevel, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            //always get the last status change
                            tcr.TranslationOrigins.Updated.ConfirmationLevel = (string)record.TranslationOrigins.Updated.ConfirmationLevel.Clone();
                        }

                        if (record.Comments.Count > 0 && !documentAnalysisMerged.CommentChangeAdded)
                        {
                            DocumentTotalSegmentCommentsUpdated++;
                            DocumentTotalSegmentCommentsUpdatedWords += record.WordCount;

                            documentAnalysisMerged.CommentChangeAdded = true;
                        }

                        if (record.Comments.Count <= 0) continue;
                        //always add the new comments to the list
                        foreach (var tcrc in record.Comments)
                            tcr.Comments.Add((Comment)tcrc.Clone());

                        #endregion
                    }

                }

                #region  |  dates/times  |
                long ticks = 0;

                if (documentActivity.Stopped.HasValue && documentActivity.Started.HasValue && documentActivity.Stopped.Value >= documentActivity.Started.Value)
                    ticks += documentActivity.TicksActivity;

                var ts = new TimeSpan(ticks);
                DocumentTotalElapsedTime = DocumentTotalElapsedTime.Add(ts);



                if (DateStart.HasValue)
                {
                    if (documentActivity.Started.Value < DateStart.Value)
                        DateStart = documentActivity.Started;
                }
                else
                    DateStart = documentActivity.Started;



                if (DateEnd.HasValue)
                {
                    if (documentActivity.Stopped.Value > DateEnd.Value)
                        DateEnd = documentActivity.Stopped;
                }
                else
                    DateEnd = documentActivity.Stopped;
                #endregion
            }



            #endregion

            #region  |  Initialize PEM variables  |

            ExactSegments = 0;
            ExactWords = 0;
            ExactCharacters = 0;
            ExactPercent = 0;
            ExactTags = 0;

            Fuzzy99Segments = 0;
            Fuzzy99Words = 0;
            Fuzzy99Characters = 0;
            Fuzzy99Percent = 0;
            Fuzzy99Tags = 0;

            Fuzzy94Segments = 0;
            Fuzzy94Words = 0;
            Fuzzy94Characters = 0;
            Fuzzy94Percent = 0;
            Fuzzy94Tags = 0;

            Fuzzy84Segments = 0;
            Fuzzy84Words = 0;
            Fuzzy84Characters = 0;
            Fuzzy84Percent = 0;
            Fuzzy84Tags = 0;

            Fuzzy74Segments = 0;
            Fuzzy74Words = 0;
            Fuzzy74Characters = 0;
            Fuzzy74Percent = 0;
            Fuzzy74Tags = 0;

            NewSegments = 0;
            NewWords = 0;
            NewCharacters = 0;
            NewPercent = 0;
            NewTags = 0;


            TotalSegments = 0;
            TotalWords = 0;
            TotalCharacters = 0;
            TotalPercent = 0;
            TotalTags = 0;


            PostEditPriceExactWords = 0;
            PostEditPriceFuzzy9995Words = 0;
            PostEditPriceFuzzy9485Words = 0;
            PostEditPriceFuzzy8475Words = 0;
            PostEditPriceFuzzy7450Words = 0;
            PostEditPriceNewWords = 0;

            PostEditPriceExactTotal = 0;
            PostEditPriceFuzzy9995Total = 0;
            PostEditPriceFuzzy9485Total = 0;
            PostEditPriceFuzzy8475Total = 0;
            PostEditPriceFuzzy7450Total = 0;
            PostEditPriceNewTotal = 0;
            PostEditPriceTotal = 0;


            PostEditPriceCurrency = string.Empty;

            #endregion

            #region  |  Edit-Distance Calculation  |
            foreach (var kvpTcr in RecordsDictionary)
            {
                

                #region  |  Calculation  |

               
                var targetOriginalForCompareLen = GetCharLength(projectActivity, kvpTcr.Value.ContentSections.TargetOriginalSections);
                var targetUpdatedForCompareLen = GetCharLength(projectActivity, kvpTcr.Value.ContentSections.TargetUpdatedSections);
                              
                int charsTargetO;
                int charsTargetU;
                decimal editDistanceChars = comparer.DamerauLevenshteinDistance_fromObject(kvpTcr.Value.ContentSections.TargetOriginalSections, kvpTcr.Value.ContentSections.TargetUpdatedSections, out charsTargetO, out charsTargetU, projectActivity.ComparisonOptions.IncludeTagsInComparison);
                decimal modificationPercentage = 100;

                decimal maxChars = targetOriginalForCompareLen > targetUpdatedForCompareLen ? targetOriginalForCompareLen : targetUpdatedForCompareLen;
               
                if (targetOriginalForCompareLen > 0)
                {
                    var charsDistPerTmp = editDistanceChars / maxChars;
                    modificationPercentage = 1.0m - charsDistPerTmp;

                    editDistanceChars = Math.Round(editDistanceChars, 2);
                    modificationPercentage = Math.Round(modificationPercentage * 100, 2);
                }
                if (targetOriginalForCompareLen == 0)
                {
                    modificationPercentage = 0;
                    editDistanceChars = 0;
                }

               
                var PEMpCache = new PEMp(kvpTcr.Value.ParagraphId + "/" + kvpTcr.Value.SegmentId, editDistanceChars, modificationPercentage, maxChars);
                PempDictionary.Add(kvpTcr.Value.ParagraphId + "/" + kvpTcr.Value.SegmentId, PEMpCache);

                

                #region  |  PEMp  apply  |

                TotalSegments++;
                TotalWords += kvpTcr.Value.WordCount;
                TotalCharacters += kvpTcr.Value.CharsCount;
                TotalPercent = 100;
                TotalTags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;


                if (string.Compare(kvpTcr.Value.TranslationOrigins.Updated.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (modificationPercentage >= 100)
                    {
                        ExactSegments++;
                        ExactWords += kvpTcr.Value.WordCount;
                        ExactCharacters += kvpTcr.Value.CharsCount;
                        ExactPercent = 0;
                        ExactTags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                    else if (modificationPercentage >= 95 && modificationPercentage < 100)
                    {
                        Fuzzy99Segments++;
                        Fuzzy99Words += kvpTcr.Value.WordCount;
                        Fuzzy99Characters += kvpTcr.Value.CharsCount;
                        Fuzzy99Percent = 0;
                        Fuzzy99Tags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                    else if (modificationPercentage >= 85 && modificationPercentage < 95)
                    {
                        Fuzzy94Segments++;
                        Fuzzy94Words += kvpTcr.Value.WordCount;
                        Fuzzy94Characters += kvpTcr.Value.CharsCount;
                        Fuzzy94Percent = 0;
                        Fuzzy94Tags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                    else if (modificationPercentage >= 75 && modificationPercentage < 85)
                    {
                        Fuzzy84Segments++;
                        Fuzzy84Words += kvpTcr.Value.WordCount;
                        Fuzzy84Characters += kvpTcr.Value.CharsCount;
                        Fuzzy84Percent = 0;
                        Fuzzy84Tags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                    else if (modificationPercentage >= 50 && modificationPercentage < 75)
                    {
                        Fuzzy74Segments++;
                        Fuzzy74Words += kvpTcr.Value.WordCount;
                        Fuzzy74Characters += kvpTcr.Value.CharsCount;
                        Fuzzy74Percent = 0;
                        Fuzzy74Tags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                    else
                    {
                        NewSegments++;
                        NewWords += kvpTcr.Value.WordCount;
                        NewCharacters += kvpTcr.Value.CharsCount;
                        NewPercent = 0;
                        NewTags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                    }
                }
                else
                {
                    ExactSegments++;
                    ExactWords += kvpTcr.Value.WordCount;
                    ExactCharacters += kvpTcr.Value.CharsCount;
                    ExactPercent = 0;
                    ExactTags += kvpTcr.Value.TagsCount + kvpTcr.Value.PlaceablesCount;
                }
                #endregion

                #endregion
            }
            #endregion

            if (projectActivity != null
                && (activityRates != null
                    ? activityRates.LanguageRates.Count > 0
                    : projectActivity.DocumentActivityRates.LanguageRates.Count > 0)
                && projectActivity.LanguageRateChecked)
            {
                LanguageRate = null;

                #region  |  get language rate  |

                foreach (var lr in activityRates != null ? activityRates.LanguageRates : projectActivity.DocumentActivityRates.LanguageRates)
                {
                    if (string.Compare(DocumentSourceLanguage, lr.SourceLanguage, StringComparison.OrdinalIgnoreCase) != 0 ||
                        string.Compare(DocumentTargetLanguage, lr.TargetLanguage, StringComparison.OrdinalIgnoreCase) != 0) continue;
                    LanguageRate = lr;
                    break;
                }

                if (LanguageRate == null)
                {
                    foreach (var lr in activityRates != null ? activityRates.LanguageRates : projectActivity.DocumentActivityRates.LanguageRates)
                    {
                        if (string.Compare("*", lr.SourceLanguage, StringComparison.OrdinalIgnoreCase) != 0
                            || string.Compare(DocumentTargetLanguage, lr.TargetLanguage, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;
                        LanguageRate = lr;
                        break;
                    }
                }

                if (LanguageRate == null)
                {
                    foreach (var lr in activityRates != null ? activityRates.LanguageRates : projectActivity.DocumentActivityRates.LanguageRates)
                    {
                        if (string.Compare(DocumentSourceLanguage, lr.SourceLanguage, StringComparison.OrdinalIgnoreCase) != 0
                            || string.Compare("*", lr.TargetLanguage, StringComparison.OrdinalIgnoreCase) != 0)
                            continue;
                        LanguageRate = lr;
                        break;
                    }
                }

                if (LanguageRate == null)
                {
                    foreach (var lr in activityRates != null ? activityRates.LanguageRates : projectActivity.DocumentActivityRates.LanguageRates)
                    {
                        if (string.Compare("*", lr.SourceLanguage, StringComparison.OrdinalIgnoreCase) != 0 ||
                            string.Compare("*", lr.TargetLanguage, StringComparison.OrdinalIgnoreCase) != 0) continue;
                        LanguageRate = lr;
                        break;
                    }
                }



                if (LanguageRate != null)
                {
                    PostEditPriceExactWords = LanguageRate.Rate100;
                    PostEditPriceFuzzy9995Words = LanguageRate.Rate95;
                    PostEditPriceFuzzy9485Words = LanguageRate.Rate85;
                    PostEditPriceFuzzy8475Words = LanguageRate.Rate75;
                    PostEditPriceFuzzy7450Words = LanguageRate.Rate50;
                    PostEditPriceNewWords = LanguageRate.RateNew;

                    PostEditPriceExactTotal = Math.Round(PostEditPriceExactWords * ExactWords, 2);
                    PostEditPriceFuzzy9995Total = Math.Round(PostEditPriceFuzzy9995Words * Fuzzy99Words, 2);
                    PostEditPriceFuzzy9485Total = Math.Round(PostEditPriceFuzzy9485Words * Fuzzy94Words, 2);
                    PostEditPriceFuzzy8475Total = Math.Round(PostEditPriceFuzzy8475Words * Fuzzy84Words, 2);
                    PostEditPriceFuzzy7450Total = Math.Round(PostEditPriceFuzzy7450Words * Fuzzy74Words, 2);
                    PostEditPriceNewTotal = Math.Round(PostEditPriceNewWords * NewWords, 2);

                    PostEditPriceTotal = Math.Round(PostEditPriceExactTotal + PostEditPriceFuzzy9995Total + PostEditPriceFuzzy9485Total
                        + PostEditPriceFuzzy8475Total + PostEditPriceFuzzy7450Total + PostEditPriceNewTotal, 2);

                    PostEditPriceCurrency = projectActivity.DocumentActivityRates.LanguageRateCurrency;
                }
                #endregion
            }

            #region  |  percentages  |

            if (TotalWords > 0)
            {
                ExactPercent = Math.Round(ExactWords, 2);
                if (ExactWords > 0)
                    ExactPercent = Math.Round(ExactWords / TotalWords * 100, 2);

                Fuzzy99Percent = Math.Round(Fuzzy99Words, 2);
                if (Fuzzy99Words > 0)
                    Fuzzy99Percent = Math.Round(Fuzzy99Words / TotalWords * 100, 2);

                Fuzzy94Percent = Math.Round(Fuzzy94Words, 2);
                if (Fuzzy94Words > 0)
                    Fuzzy94Percent = Math.Round(Fuzzy94Words / TotalWords * 100, 2);

                Fuzzy84Percent = Math.Round(Fuzzy84Words, 2);
                if (Fuzzy84Words > 0)
                    Fuzzy84Percent = Math.Round(Fuzzy84Words / TotalWords * 100, 2);

                Fuzzy74Percent = Math.Round(Fuzzy74Words, 2);
                if (Fuzzy74Words > 0)
                    Fuzzy74Percent = Math.Round(Fuzzy74Words / TotalWords * 100, 2);

                NewPercent = Math.Round(NewWords, 2);
                if (NewWords > 0)
                    NewPercent = Math.Round(NewWords / TotalWords * 100, 2);
            }

            if (DocumentTotalSegmentContentUpdated > 0 && TotalSourceSegments > 0)
            {
                DocumentTotalSegmentContentUpdatedPercentage = TotalSourceSegments >= DocumentTotalSegmentContentUpdated ? Math.Round(DocumentTotalSegmentContentUpdated / TotalSourceSegments * 100, 2) : 100;
            }

            if (DocumentTotalSegmentStatusUpdated > 0 && TotalSourceSegments > 0)
            {
                DocumentTotalSegmentStatusUpdatedPercentage = TotalSourceSegments >= DocumentTotalSegmentStatusUpdated ? Math.Round(DocumentTotalSegmentStatusUpdated / TotalSourceSegments * 100, 2) : 100;
            }

            if (DocumentTotalSegmentCommentsUpdated > 0 && TotalSourceSegments > 0)
            {
                DocumentTotalSegmentCommentsUpdatedPercentage = TotalSourceSegments >= DocumentTotalSegmentCommentsUpdated ? Math.Round(DocumentTotalSegmentCommentsUpdated / TotalSourceSegments * 100, 2) : 100;
            }

            #endregion


            #region  |  documents have track changes?  |

            DocumentHasTrackChanges = false;
            foreach (var da in documentActivityList)
            {
                foreach (var record in da.Records)
                {
                    foreach (var trgu in record.ContentSections.TargetUpdatedSections)
                    {
                        if (trgu.RevisionMarker == null) continue;
                        DocumentHasTrackChanges = true;
                        break;
                    }
                }
                if (DocumentHasTrackChanges)
                    break;
            }
            #endregion


            #region  | set up the merged quality metrics  |

            QualityMetrics = new List<QualityMetric>();
            var qualityMetricsTmp = new List<QualityMetric>();

            foreach (var aTcr in RecordsDictionary.Values)
                qualityMetricsTmp.AddRange(aTcr.QualityMetrics);

            qualityMetricsTmp = qualityMetricsTmp.OrderByDescending(a => a.Modified).ToList();

            foreach (var qm in qualityMetricsTmp)
                if (!QualityMetrics.Exists(a => a.Guid == qm.Guid))
                    QualityMetrics.Add(qm);

            #endregion
        }

        private static int GetCharLength(Activity projectActivity, IEnumerable<ContentSection> contentSections )
        {
            var length = 0;
            foreach (var contentSection in contentSections)
            {
                if (contentSection.RevisionMarker != null && contentSection.RevisionMarker.RevType == RevisionMarker.RevisionType.Delete)
                {
                    //ignore
                }
                else
                {
                    if (contentSection.CntType == ContentSection.ContentType.Text)
                    {
                        var strLists = Helper.GetTextSections(contentSection.Content);
                        length += strLists.SelectMany(part => part).Count();
                    }
                    else
                    {
                        if (projectActivity.ComparisonOptions.IncludeTagsInComparison)
                            length++;
                    }
                }
            }
            return length;
        }

        public static decimal GetMatchValue(string matchPercentage)
        {
            decimal value = 0;
            if (string.IsNullOrEmpty(matchPercentage))
                return value;
            var mRegexPercentage = RegexPercentage.Match(matchPercentage);
            if (mRegexPercentage.Success)
                value = NormalizeTerpStringValueToDecimal(mRegexPercentage.Groups["x1"].Value);

            return value;
        }
        private static decimal NormalizeTerpStringValueToDecimal(string str)
        {
            var strOut = str.Trim();
            decimal decOut;

            var hasDot = str.Contains(".");
            var hasComma = str.Contains(",");

            if (hasDot && hasComma)
            {
                var dotIndex = str.IndexOf(".", StringComparison.Ordinal);
                var commaIndex = str.IndexOf(",", StringComparison.Ordinal);

                strOut = strOut.Replace(dotIndex < commaIndex ? "." : ",", string.Empty);
                strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            }
            else if (hasDot)
            {
                NormalizeValue(ref strOut, '.');
            }
            else if (hasComma)
            {
                NormalizeValue(ref strOut, ',');
                strOut = strOut.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
            }

            decimal.TryParse(strOut, NumberStyles.Any, CultureInfo.InvariantCulture, out decOut);

            return decOut;
        }
        private static void NormalizeValue(ref string strOut, char seperator)
        {
            var split = strOut.Split(seperator);
            if (split.Length != 2)
                return;

            strOut = split[0].Trim().TrimStart('0');
            var end = split[1].Trim().TrimEnd('0');
            strOut += end != string.Empty
                ? seperator + end
                : string.Empty;

            if (strOut.Trim() == string.Empty)
                strOut = "0";
        }

        private static readonly Regex RegexPercentage = new Regex(@"(?<x1>[\d]+)\%");
    }
}
