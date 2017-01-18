using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sdl.Community.XliffCompare.Core.Comparer
{
    internal partial class Comparer
    {

        internal class FileUnitProperties
        {
            internal string FilePathOriginal { get; set; }
            internal string FilePathUpdated { get; set; }

            internal int TotalSegmentsOriginal { get; set; }
            internal int TotalSegmentsUpdated { get; set; }

            internal int TotalContentChanges { get; set; }
            internal decimal TotalContentChangesPercentage { get; private set; }

            internal int TotalStatusChanges { get; set; }
            internal decimal TotalStatusChangesPercentage { get; private set; }

            internal int TotalComments { get; set; }

            internal int TotalSegments { get; set; }

            internal int TotalNotTranslatedOriginal { get; set; }
            internal int TotalDraftOriginal { get; set; }
            internal int TotalTranslatedOriginal { get; set; }
            internal int TotalTranslationRejectedOriginal { get; set; }
            internal int TotalTranslationApprovedOriginal { get; set; }
            internal int TotalSignOffRejectedOriginal { get; set; }
            internal int TotalSignedOffOriginal { get; set; }

            internal int TotalNotTranslatedUpdated { get; set; }
            internal int TotalDraftUpdated { get; set; }
            internal int TotalTranslatedUpdated { get; set; }
            internal int TotalTranslationRejectedUpdated { get; set; }
            internal int TotalTranslationApprovedUpdated { get; set; }
            internal int TotalSignOffRejectedUpdated { get; set; }
            internal int TotalSignedOffUpdated { get; set; }


            internal int TotalTranslationChangesPm { get; set; }
            internal int TotalTranslationChangesCm { get; set; }
            internal int TotalTranslationChangesExact { get; set; }
            internal int TotalTranslationChangesAt { get; set; }
            internal int TotalTranslationChangesOther { get; set; }

            internal decimal TotalTranslationChangesPmWordsNew { get; set; }
            internal decimal TotalTranslationChangesCmWordsNew { get; set; }
            internal decimal TotalTranslationChangesExactWordsNew { get; set; }
            internal decimal TotalTranslationChangesAtWordsNew { get; set; }
            internal decimal TotalTranslationChangesOtherWordsNew { get; set; }

            internal decimal TotalTranslationChangesPmWordsRemoved { get; set; }
            internal decimal TotalTranslationChangesCmWordsRemoved { get; set; }
            internal decimal TotalTranslationChangesExactWordsRemoved { get; set; }
            internal decimal TotalTranslationChangesAtWordsRemoved { get; set; }
            internal decimal TotalTranslationChangesOtherWordsRemoved { get; set; }

            internal decimal TotalTranslationChangesPmWords { get; set; }
            internal decimal TotalTranslationChangesCmWords { get; set; }
            internal decimal TotalTranslationChangesExactWords { get; set; }
            internal decimal TotalTranslationChangesAtWords { get; set; }
            internal decimal TotalTranslationChangesOtherWords { get; set; }


            internal decimal TotalTranslationChangesPmCharactersNew { get; set; }
            internal decimal TotalTranslationChangesCmCharactersNew { get; set; }
            internal decimal TotalTranslationChangesExactCharactersNew { get; set; }
            internal decimal TotalTranslationChangesAtCharactersNew { get; set; }
            internal decimal TotalTranslationChangesOtherCharactersNew { get; set; }

            internal decimal TotalTranslationChangesPmCharactersRemoved { get; set; }
            internal decimal TotalTranslationChangesCmCharactersRemoved { get; set; }
            internal decimal TotalTranslationChangesExactCharactersRemoved { get; set; }
            internal decimal TotalTranslationChangesAtCharactersRemoved { get; set; }
            internal decimal TotalTranslationChangesOtherCharactersRemoved { get; set; }

            internal decimal TotalTranslationChangesPmCharacters { get; set; }
            internal decimal TotalTranslationChangesCmCharacters { get; set; }
            internal decimal TotalTranslationChangesExactCharacters { get; set; }
            internal decimal TotalTranslationChangesAtCharacters { get; set; }
            internal decimal TotalTranslationChangesOtherCharacters { get; set; }


            internal decimal TotalTranslationChangesPmTags { get; set; }
            internal decimal TotalTranslationChangesCmTags { get; set; }
            internal decimal TotalTranslationChangesExactTags { get; set; }
            internal decimal TotalTranslationChangesAtTags { get; set; }
            internal decimal TotalTranslationChangesOtherTags { get; set; }

            internal decimal TotalTranslationChangesPmTagsNew { get; set; }
            internal decimal TotalTranslationChangesCmTagsNew { get; set; }
            internal decimal TotalTranslationChangesExactTagsNew { get; set; }
            internal decimal TotalTranslationChangesAtTagsNew { get; set; }
            internal decimal TotalTranslationChangesOtherTagsNew { get; set; }

            internal decimal TotalTranslationChangesPmTagsRemoved { get; set; }
            internal decimal TotalTranslationChangesCmTagsRemoved { get; set; }
            internal decimal TotalTranslationChangesExactTagsRemoved { get; set; }
            internal decimal TotalTranslationChangesAtTagsRemoved { get; set; }
            internal decimal TotalTranslationChangesOtherTagsRemoved { get; set; }


            internal FileUnitProperties(string filePathOriginal, string filePathUpdated
                , int totalSegmentsOriginal, int totalSegmentsUpdated, Dictionary<string, Dictionary<string, ComparisonParagraphUnit>> comparisonFileParagraphUnits)
            {
             

                FilePathOriginal = filePathOriginal;
                FilePathUpdated = filePathUpdated;

                TotalSegmentsOriginal = totalSegmentsOriginal;
                TotalSegmentsUpdated = totalSegmentsUpdated;


                TotalContentChanges = 0;
                TotalContentChangesPercentage = 0;

                TotalStatusChanges = 0;
                TotalStatusChangesPercentage = 0;

                TotalComments = 0;


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



                TotalTranslationChangesPm = 0;
                TotalTranslationChangesCm = 0;
                TotalTranslationChangesExact = 0;
                TotalTranslationChangesAt = 0;
                TotalTranslationChangesOther = 0;


                TotalTranslationChangesPmWords = 0;
                TotalTranslationChangesCmWords = 0;
                TotalTranslationChangesExactWords = 0;
                TotalTranslationChangesAtWords = 0;
                TotalTranslationChangesOtherWords = 0;


                TotalTranslationChangesPmWordsNew = 0;
                TotalTranslationChangesCmWordsNew = 0;
                TotalTranslationChangesExactWordsNew = 0;
                TotalTranslationChangesAtWordsNew = 0;
                TotalTranslationChangesOtherWordsNew = 0;


                TotalTranslationChangesPmWordsRemoved = 0;
                TotalTranslationChangesCmWordsRemoved = 0;
                TotalTranslationChangesExactWordsRemoved = 0;
                TotalTranslationChangesAtWordsRemoved = 0;
                TotalTranslationChangesOtherWordsRemoved = 0;

                TotalTranslationChangesPmCharacters = 0;
                TotalTranslationChangesCmCharacters = 0;
                TotalTranslationChangesExactCharacters = 0;
                TotalTranslationChangesAtCharacters = 0;
                TotalTranslationChangesOtherCharacters = 0;


                TotalTranslationChangesPmCharactersNew = 0;
                TotalTranslationChangesCmCharactersNew = 0;
                TotalTranslationChangesExactCharactersNew = 0;
                TotalTranslationChangesAtCharactersNew = 0;
                TotalTranslationChangesOtherCharactersNew = 0;


                TotalTranslationChangesPmCharactersRemoved = 0;
                TotalTranslationChangesCmCharactersRemoved = 0;
                TotalTranslationChangesExactCharactersRemoved = 0;
                TotalTranslationChangesAtCharactersRemoved = 0;
                TotalTranslationChangesOtherCharactersRemoved = 0;

                TotalTranslationChangesPmTags = 0;
                TotalTranslationChangesCmTags = 0;
                TotalTranslationChangesExactTags = 0;
                TotalTranslationChangesAtTags = 0;
                TotalTranslationChangesOtherTags = 0;

                TotalTranslationChangesPmTagsNew = 0;
                TotalTranslationChangesCmTagsNew = 0;
                TotalTranslationChangesExactTagsNew = 0;
                TotalTranslationChangesAtTagsNew = 0;
                TotalTranslationChangesOtherTagsNew = 0;

                TotalTranslationChangesPmTagsRemoved = 0;
                TotalTranslationChangesCmTagsRemoved = 0;
                TotalTranslationChangesExactTagsRemoved = 0;
                TotalTranslationChangesAtTagsRemoved = 0;
                TotalTranslationChangesOtherTagsRemoved = 0;


                if (comparisonFileParagraphUnits != null)
                {
                    foreach (var comparisonFileParagraphUnitList in comparisonFileParagraphUnits)
                    {
                        foreach (var comparisonFileParagraphUnit in comparisonFileParagraphUnitList.Value)
                        {
                            var comparisonParagraphUnit = comparisonFileParagraphUnit.Value;

                            foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
                            {
                                TotalSegments++;

                                if (comparisonSegmentUnit.SegmentStatusOriginal == string.Empty)
                                    comparisonSegmentUnit.SegmentStatusOriginal = "Unspecified";

                                switch (comparisonSegmentUnit.SegmentStatusOriginal)
                                {
                                    case "Unspecified": TotalNotTranslatedOriginal++; break;
                                    case "Draft": TotalDraftOriginal++; break;
                                    case "Translated": TotalTranslatedOriginal++; break;
                                    case "RejectedTranslation": TotalTranslationRejectedOriginal++; break;
                                    case "ApprovedTranslation": TotalTranslationApprovedOriginal++; break;
                                    case "RejectedSignOff": TotalSignOffRejectedOriginal++; break;
                                    case "ApprovedSignOff": TotalSignedOffOriginal++; break;
                                }


                                if (comparisonSegmentUnit.SegmentStatusUpdated == string.Empty)
                                    comparisonSegmentUnit.SegmentStatusUpdated = "Unspecified";

                                switch (comparisonSegmentUnit.SegmentStatusUpdated)
                                {
                                    case "Unspecified": TotalNotTranslatedUpdated++; break;
                                    case "Draft": TotalDraftUpdated++; break;
                                    case "Translated": TotalTranslatedUpdated++; break;
                                    case "RejectedTranslation": TotalTranslationRejectedUpdated++; break;
                                    case "ApprovedTranslation": TotalTranslationApprovedUpdated++; break;
                                    case "RejectedSignOff": TotalSignOffRejectedUpdated++; break;
                                    case "ApprovedSignOff": TotalSignedOffUpdated++; break;
                                }


                                if (comparisonSegmentUnit.SegmentHasComments)
                                    TotalComments += comparisonSegmentUnit.Comments.Count;

                                if (comparisonSegmentUnit.SegmentTextUpdated)
                                    TotalContentChanges++;

                                if (comparisonSegmentUnit.SegmentSegmentStatusUpdated)
                                    TotalStatusChanges++;

                                if (!comparisonSegmentUnit.SegmentTextUpdated) 
                                    continue;

                                switch (comparisonSegmentUnit.TranslationStatusOriginal)
                                {
                                    case "PM":
                                    {
                                                
                                        TotalTranslationChangesPm++;
                                        TotalTranslationChangesPmWords += comparisonSegmentUnit.TranslationSectionsChangedWords;
                                        TotalTranslationChangesPmWordsNew += comparisonSegmentUnit.TranslationSectionsWordsNew;
                                        TotalTranslationChangesPmWordsRemoved += comparisonSegmentUnit.TranslationSectionsWordsRemoved;

                                        TotalTranslationChangesPmCharacters += comparisonSegmentUnit.TranslationSectionsChangedCharacters;
                                        TotalTranslationChangesPmCharactersNew += comparisonSegmentUnit.TranslationSectionsCharactersNew;
                                        TotalTranslationChangesPmCharactersRemoved += comparisonSegmentUnit.TranslationSectionsCharactersRemoved;

                                        TotalTranslationChangesPmTags += comparisonSegmentUnit.TranslationSectionsChangedTags;
                                        TotalTranslationChangesPmTagsNew += comparisonSegmentUnit.TranslationSectionsTagsNew;
                                        TotalTranslationChangesPmTagsRemoved += comparisonSegmentUnit.TranslationSectionsTagsRemoved;
                                    } break;
                                    case "CM":
                                    {
                                        TotalTranslationChangesCm++;
                                        TotalTranslationChangesCmWords += comparisonSegmentUnit.TranslationSectionsChangedWords;
                                        TotalTranslationChangesCmWordsNew += comparisonSegmentUnit.TranslationSectionsWordsNew;
                                        TotalTranslationChangesCmWordsRemoved += comparisonSegmentUnit.TranslationSectionsWordsRemoved;


                                        TotalTranslationChangesCmCharacters += comparisonSegmentUnit.TranslationSectionsChangedCharacters;
                                        TotalTranslationChangesCmCharactersNew += comparisonSegmentUnit.TranslationSectionsCharactersNew;
                                        TotalTranslationChangesCmCharactersRemoved += comparisonSegmentUnit.TranslationSectionsCharactersRemoved;

                                        TotalTranslationChangesCmTags += comparisonSegmentUnit.TranslationSectionsChangedTags;
                                        TotalTranslationChangesCmTagsNew += comparisonSegmentUnit.TranslationSectionsTagsNew;
                                        TotalTranslationChangesCmTagsRemoved += comparisonSegmentUnit.TranslationSectionsTagsRemoved;
                                    } break;
                                    case "100%":
                                    {
                                        TotalTranslationChangesExact++;
                                        TotalTranslationChangesExactWords += comparisonSegmentUnit.TranslationSectionsChangedWords;
                                        TotalTranslationChangesExactWordsNew += comparisonSegmentUnit.TranslationSectionsWordsNew;
                                        TotalTranslationChangesExactWordsRemoved += comparisonSegmentUnit.TranslationSectionsWordsRemoved;


                                        TotalTranslationChangesExactCharacters += comparisonSegmentUnit.TranslationSectionsChangedCharacters;
                                        TotalTranslationChangesExactCharactersNew += comparisonSegmentUnit.TranslationSectionsCharactersNew;
                                        TotalTranslationChangesExactCharactersRemoved += comparisonSegmentUnit.TranslationSectionsCharactersRemoved;


                                        TotalTranslationChangesExactTags += comparisonSegmentUnit.TranslationSectionsChangedTags;
                                        TotalTranslationChangesExactTagsNew += comparisonSegmentUnit.TranslationSectionsTagsNew;
                                        TotalTranslationChangesExactTagsRemoved += comparisonSegmentUnit.TranslationSectionsTagsRemoved;
                                    } break;
                                    case "AT":
                                    {                                                
                                        TotalTranslationChangesAt++;
                                        TotalTranslationChangesAtWords += comparisonSegmentUnit.TranslationSectionsChangedWords;
                                        TotalTranslationChangesAtWordsNew += comparisonSegmentUnit.TranslationSectionsWordsNew;
                                        TotalTranslationChangesAtWordsRemoved += comparisonSegmentUnit.TranslationSectionsWordsRemoved;

                                        TotalTranslationChangesAtCharacters += comparisonSegmentUnit.TranslationSectionsChangedCharacters;
                                        TotalTranslationChangesAtCharactersNew += comparisonSegmentUnit.TranslationSectionsCharactersNew;
                                        TotalTranslationChangesAtCharactersRemoved += comparisonSegmentUnit.TranslationSectionsCharactersRemoved;


                                        TotalTranslationChangesAtTags += comparisonSegmentUnit.TranslationSectionsChangedTags;
                                        TotalTranslationChangesAtTagsNew += comparisonSegmentUnit.TranslationSectionsTagsNew;
                                        TotalTranslationChangesAtTagsRemoved += comparisonSegmentUnit.TranslationSectionsTagsRemoved;
                                    } break;

                                    default:
                                    {
                                        TotalTranslationChangesOther++;
                                        TotalTranslationChangesOtherWords += comparisonSegmentUnit.TranslationSectionsChangedWords;
                                        TotalTranslationChangesOtherWordsNew += comparisonSegmentUnit.TranslationSectionsWordsNew;
                                        TotalTranslationChangesOtherWordsRemoved += comparisonSegmentUnit.TranslationSectionsWordsRemoved;

                                        TotalTranslationChangesOtherCharacters += comparisonSegmentUnit.TranslationSectionsChangedCharacters;
                                        TotalTranslationChangesOtherCharactersNew += comparisonSegmentUnit.TranslationSectionsCharactersNew;
                                        TotalTranslationChangesOtherCharactersRemoved += comparisonSegmentUnit.TranslationSectionsCharactersRemoved;

                                        TotalTranslationChangesOtherTags += comparisonSegmentUnit.TranslationSectionsChangedTags;
                                        TotalTranslationChangesOtherTagsNew += comparisonSegmentUnit.TranslationSectionsTagsNew;
                                        TotalTranslationChangesOtherTagsRemoved += comparisonSegmentUnit.TranslationSectionsTagsRemoved;
                                    } break;
                                }
                            }
                        }
                    }
                }

                if (TotalSegmentsUpdated > 0 && TotalContentChanges > 0)
                    TotalContentChangesPercentage = Math.Round(Convert.ToDecimal(TotalContentChanges) / Convert.ToDecimal(TotalSegments) * 100, 2);

                if (TotalSegmentsUpdated > 0 && TotalStatusChanges > 0)
                    TotalStatusChangesPercentage = Math.Round(Convert.ToDecimal(TotalStatusChanges) / Convert.ToDecimal(TotalSegments) * 100, 2);

            }



            internal int GetWordsCount(string text)
            {
                var iWords = 0;

                var rWords = new Regex(@"\s", RegexOptions.Singleline);

                foreach (var word in rWords.Split(text))
                {
                    var wortTmp = string.Empty;
                    foreach (var _char in word.ToCharArray())
                    {

                        if (Encoding.UTF8.GetByteCount(_char.ToString()) > 2) //aisian characters
                        {
                            if (wortTmp != string.Empty)
                                iWords++;

                            iWords++;
                            wortTmp = string.Empty;
                        }
                        else
                            wortTmp += _char.ToString();

                    }
                    if (wortTmp != string.Empty)
                        iWords++;
                }

                return iWords;
            }
        }
    }


}
