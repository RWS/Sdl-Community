using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.PostEdit.Compare.Core.SDLXLIFF
{
    internal class ContentWriter : AbstractNativeFileParser, IBilingualContentProcessor, IBilingualParser, IDependencyFileProperties
    {
        internal int TotalSegmentsOriginalFile;
        internal int TotalSegmentsBilingualFile;
        internal int TotalContentChanges;
        internal int TotalStatusChangesWithContentChanges;
        internal int TotalStatusChangesWithoutContentChanges;

        internal CultureInfo SourceLanguageCultureInfo { get; set; }
        internal CultureInfo TargetLanguageCultureInfo { get; set; }

        private IFileProperties CurrentFileProperties { get; set; }

        internal bool IncludeTagText = true;
        internal Dictionary<string, ParagraphUnit> XParagraphUnits { get; set; }
        internal string OutputPath = string.Empty;
        internal bool CreatedDummyOutput = false;
        internal List<string> DummyOutputFiles = new List<string>();

        internal Dictionary<string, List<TagUnitWarning>> TagUnitWarnings { get; set; }

        #region  |  Content Generator  |
        private ContentGenerator _contentGeneratorProcessor;

        internal ContentGenerator ContentGeneratorProcessor
        {
            get
            {
                if (_contentGeneratorProcessor == null)
                {
                    _contentGeneratorProcessor = new ContentGenerator();
                }
                return _contentGeneratorProcessor;
            }
        }
        #endregion

        #region  |  IBilingualContentProcessor Members  |


        public ContentWriter()
        {
            XParagraphUnits = new Dictionary<string, ParagraphUnit>();
        }


        public IBilingualContentHandler Output
        {
            get;
            set;
        }

        #endregion

        #region  |  IBilingualContentHandler Members  |


        public void Complete()
        {
            Output.Complete();
        }

        public void FileComplete()
        {
          

            Output.FileComplete();
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
            TotalSegmentsOriginalFile = 0;
            TotalSegmentsBilingualFile = 0;
            TotalContentChanges = 0;
            TotalStatusChangesWithContentChanges = 0;
            TotalStatusChangesWithoutContentChanges = 0;

            TagUnitWarnings = new Dictionary<string, List<TagUnitWarning>>();

            SourceLanguageCultureInfo = documentInfo.SourceLanguage.CultureInfo;
            if (documentInfo.TargetLanguage != null && documentInfo.TargetLanguage.CultureInfo != null)
                TargetLanguageCultureInfo = documentInfo.TargetLanguage.CultureInfo;
            else
                TargetLanguageCultureInfo = SourceLanguageCultureInfo;

            Output.Initialize(documentInfo);


            
        }


      
        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {

            try
            {

                //string s1 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.AutomatedAlignment.ToString();
                //string s2 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.AutoPropagated.ToString();
                //string s3 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.DocumentMatch.ToString();
                //string s4 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.Interactive.ToString();
                //string s5 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.MachineTranslation.ToString();
                //string s6 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.NotTranslated.ToString();
                //string s7 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.Source.ToString();
                //string s8 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.TranslationMemory.ToString();
                //string s9 = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.Unknown.ToString();
                
                

                foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    TotalSegmentsOriginalFile++;

                if (XParagraphUnits.ContainsKey(paragraphUnit.Properties.ParagraphUnitId.Id))
                {

                    var paragraphUnitUpdated = XParagraphUnits[paragraphUnit.Properties.ParagraphUnitId.Id];

                    foreach (var segmentPair in paragraphUnit.SegmentPairs)
                    {
                        if (segmentPair.Target.Count == 0)
                        {
                            foreach (var t in segmentPair.Source)
                                segmentPair.Target.Add((IAbstractMarkupData)t.Clone());
                        }

                    


                        var segmentPairUpdated = paragraphUnitUpdated.SegmentPairs.FirstOrDefault(pairUpdated => segmentPair.Properties.Id.Id == pairUpdated.Id);

                        if (segmentPairUpdated == null)
                        {
                            #region  |  check update status  |

                            //check update status
                            if (Processor.Settings.NotImportedTranslationStatus != string.Empty)
                            {
                                if (
                                    string.Compare(segmentPair.Properties.ConfirmationLevel.ToString(),
                                        Processor.Settings.NotImportedTranslationStatus, StringComparison.OrdinalIgnoreCase) == 0) continue;
                                TotalStatusChangesWithoutContentChanges++;

                                segmentPair.Properties.ConfirmationLevel = getConfirmationLevel(Processor.Settings.NotImportedTranslationStatus);

                                if (segmentPair.Properties.TranslationOrigin != null)
                                {
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation = ItemFactory.CreateTranslationOrigin();
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginalTranslationHash = segmentPair.Properties.TranslationOrigin.OriginalTranslationHash;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem = segmentPair.Properties.TranslationOrigin.OriginSystem;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginType = segmentPair.Properties.TranslationOrigin.OriginType;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.RepetitionTableId = segmentPair.Properties.TranslationOrigin.RepetitionTableId;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.TextContextMatchLevel = segmentPair.Properties.TranslationOrigin.TextContextMatchLevel;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.MatchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
                                    segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.IsStructureContextMatch = segmentPair.Properties.TranslationOrigin.IsStructureContextMatch;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            TotalSegmentsBilingualFile++;

                            ContentGeneratorProcessor.ProcessSegment(segmentPair.Source, IncludeTagText);
                            var xTagPairsSource = ContentGeneratorProcessor.TagPairs;
                            var xPlaceHolderTagsSource = ContentGeneratorProcessor.PlaceholderTags;


                            ContentGeneratorProcessor.ProcessSegment(segmentPair.Target, IncludeTagText);
                            var xTagPairs = ContentGeneratorProcessor.TagPairs;
                            var xLockedContentTags = ContentGeneratorProcessor.LockedContentTags;
                            var xPlaceHolderTags = ContentGeneratorProcessor.PlaceholderTags;
                            //var xSegmentSections = ContentGeneratorProcessor.xSegmentSections;
                            var xComments = ContentGeneratorProcessor.Comments;
                            var xTagUnits = ContentGeneratorProcessor.TagUnits;

                            if (xTagPairsSource.Count > 0)
                                xTagPairs.AddRange(xTagPairsSource);
                            if (xPlaceHolderTagsSource.Count > 0)
                                xPlaceHolderTags.AddRange(xPlaceHolderTagsSource);

                            var targetOriginal = ContentGeneratorProcessor.PlainText.ToString();
                            var targetUpdated = segmentPairUpdated.Target;

                            var targetSectionsUpdated = segmentPairUpdated.TargetSections;
                            var commentsUpdated = new List<Comment>();
                            if (segmentPairUpdated.Comments != null && segmentPairUpdated.Comments.Count > 0)
                                commentsUpdated = segmentPairUpdated.Comments;


                            var segmentStatusUpdated = getConfirmationLevel(segmentPairUpdated.SegmentStatus);
                            var segmentPairProperties = segmentPair.Properties;


                            #region  |  ConfirmationLevel  |


                            if (Processor.Settings.NotChangedTranslationStatus != string.Empty)
                            {
                                if (string.Compare(segmentPairProperties.ConfirmationLevel.ToString(), Processor.Settings.NotChangedTranslationStatus, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    if (string.CompareOrdinal(targetOriginal, targetUpdated) != 0
                                        || (segmentPair.Properties.TranslationOrigin != null
                                            && (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                            )
                                        )
                                    {
                                        TotalStatusChangesWithContentChanges++;
                                    }
                                    else
                                    {
                                        TotalStatusChangesWithoutContentChanges++;
                                    }

                                    segmentPairProperties.ConfirmationLevel = getConfirmationLevel(Processor.Settings.NotChangedTranslationStatus);
                                }
                            }
                            else
                            {

                                if (string.Compare(segmentPairProperties.ConfirmationLevel.ToString(), segmentStatusUpdated.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    if (string.CompareOrdinal(targetOriginal, targetUpdated) != 0
                                        || (segmentPair.Properties.TranslationOrigin != null
                                            && (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                            )
                                        )
                                    {
                                        TotalStatusChangesWithContentChanges++;
                                    }
                                    else
                                    {
                                        TotalStatusChangesWithoutContentChanges++;
                                    }

                                    segmentPairProperties.ConfirmationLevel = segmentStatusUpdated;
                                }
                            }






                            if (string.Compare(segmentPairProperties.ConfirmationLevel.ToString(), segmentStatusUpdated.ToString(), StringComparison.OrdinalIgnoreCase) != 0
                                || string.CompareOrdinal(targetOriginal, targetUpdated) != 0
                                || (segmentPair.Properties.TranslationOrigin != null
                                    && (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                    )
                                )
                            {

                                if (segmentPair.Properties.TranslationOrigin != null)
                                {
                                    
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation = ItemFactory.CreateTranslationOrigin();
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginalTranslationHash = segmentPair.Properties.TranslationOrigin.OriginalTranslationHash;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem = segmentPair.Properties.TranslationOrigin.OriginSystem;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.OriginType = segmentPair.Properties.TranslationOrigin.OriginType;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.RepetitionTableId = segmentPair.Properties.TranslationOrigin.RepetitionTableId;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.TextContextMatchLevel = segmentPair.Properties.TranslationOrigin.TextContextMatchLevel;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.MatchPercent = segmentPair.Properties.TranslationOrigin.MatchPercent;
                                    segmentPairProperties.TranslationOrigin.OriginBeforeAdaptation.IsStructureContextMatch = segmentPair.Properties.TranslationOrigin.IsStructureContextMatch;


                                    
                                }

                              
                            }

                            #endregion


                            #region  |  incorporate the modifications to the target segment  |

                            if (segmentPairUpdated.TranslationOrigin.MatchPercentage > 100)
                                segmentPairUpdated.TranslationOrigin.MatchPercentage = 100;

                            if (string.CompareOrdinal(targetOriginal, targetUpdated) != 0
                                || (segmentPair.Properties.TranslationOrigin != null
                                    && (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                    )
                                )
                            {
                                TotalContentChanges++;


                                try
                                {                                
                                    if (Processor.Settings.ChangedTranslationStatus != string.Empty)
                                        segmentPairProperties.ConfirmationLevel = getConfirmationLevel(Processor.Settings.ChangedTranslationStatus);
                                    //else
                                    //    segmentPairProperties.ConfirmationLevel = ConfirmationLevel.Draft;

                                    var trgSegment = segmentPair.Target;

                                    //delete the items from the target container; keeping the target container
                                    var iLocationCount = segmentPair.Target.Count;
                                    for (var i = 0; i < iLocationCount; i++)
                                        segmentPair.Target.RemoveAt(0);



                                    if (trgSegment.Properties.TranslationOrigin == null)
                                        trgSegment.Properties.TranslationOrigin = ItemFactory.CreateTranslationOrigin();



                                    if (segmentPairUpdated.TranslationOrigin != null)
                                    {

                                        #region  |  originSystem  |

                                        segmentPairUpdated.TranslationOrigin.OriginSystem = "Legacy Converter";
                                        trgSegment.Properties.TranslationOrigin.OriginSystem = "Legacy Converter";
                         

                                        #endregion

                                      

                                        #region  |  matchPercentage  |
                                        if (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                        {
                                            if (segmentPairUpdated.TranslationOrigin.MatchPercentage > 0)
                                            {
                                                if (segmentPairUpdated.TranslationOrigin.MatchPercentage > 100)
                                                    segmentPairUpdated.TranslationOrigin.MatchPercentage = 100;

                                                if (segmentPairProperties.TranslationOrigin.MatchPercent != Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage))
                                                {
                                                    segmentPairUpdated.TranslationOrigin.OriginType = "tm";
                                                    trgSegment.Properties.TranslationOrigin.OriginType = Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi.DefaultTranslationOrigin.TranslationMemory;


                                                    segmentPairUpdated.TranslationOrigin.OriginSystem = "Legacy Converter";
                                                    trgSegment.Properties.TranslationOrigin.OriginSystem = "Legacy Converter";

                                                    segmentPairUpdated.TranslationOrigin.TextContextMatchLevel = string.Empty;
                                                    trgSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;
                                                }                                                
                                                
                                                segmentPairProperties.TranslationOrigin.MatchPercent = Convert.ToByte(segmentPairUpdated.TranslationOrigin.MatchPercentage);
                                            }
                                        }
                                        #endregion

                                        #region  |  originType  |

                                        if (segmentPairUpdated.TranslationOrigin.OriginType != null && segmentPairUpdated.TranslationOrigin.OriginType.Trim() != string.Empty)
                                        {

                                            if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "auto-propagated", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "AutoPropagated", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.AutoPropagated;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "interactive", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "Source", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Source;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "not-translated", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "NotTranslated", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.NotTranslated;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "tm", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "TranslationMemory", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.TranslationMemory;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "mt", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "amt", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "MachineTranslation", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.MachineTranslation;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "auto-aligned", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "AutomatedAlignment", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.AutomatedAlignment;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "document-match", StringComparison.OrdinalIgnoreCase) == 0
                                                || string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "DocumentMatch", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.DocumentMatch;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.OriginType, "unknown", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Unknown;
                                            else
                                                trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
                                        }
                                        else
                                        {
                                            trgSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
                                        }
                                        #endregion


                                        #region  |  textContextMatchLevel  |

                                        if (segmentPairUpdated.TranslationOrigin.TextContextMatchLevel.Trim() != string.Empty)
                                        {
                                            if (string.Compare(segmentPairUpdated.TranslationOrigin.TextContextMatchLevel, "None", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.TextContextMatchLevel, "Source", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.Source;
                                            else if (string.Compare(segmentPairUpdated.TranslationOrigin.TextContextMatchLevel, "SourceAndTarget", StringComparison.OrdinalIgnoreCase) == 0)
                                                trgSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.SourceAndTarget;
                                        }
                                        else
                                        {
                                            trgSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;
                                        }
                                        #endregion
                                    }


                                    _iStartIndex = 0;


                                    var xTagPairsCloned = new List<ITagPair>();
                                    var xPlaceHolderTagsCloned = new List<IPlaceholderTag>();
                                    var xLockedContentCloned = new List<ILockedContent>();
                      

                                    WriteTargetSegment(trgSegment, null, targetSectionsUpdated, xTagPairs, xPlaceHolderTags, xLockedContentTags);


                                    #region  |  get tag units list from the updated target segment  |
                                    var tagUnitsTargetUpdated = new List<TagUnit>();
                                    foreach (var section in targetSectionsUpdated)
                                    {
                                        if (section.Type == SegmentSection.ContentType.Text) continue;
                                        switch (section.Type)
                                        {
                                            case SegmentSection.ContentType.Placeholder:
                                                tagUnitsTargetUpdated.Add(new TagUnit(section.SectionId, string.Empty, section.Content, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsPlaceholder));
                                                break;
                                            case SegmentSection.ContentType.Tag:
                                                tagUnitsTargetUpdated.Add(new TagUnit(section.SectionId, string.Empty, section.Content, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
                                                break;
                                            case SegmentSection.ContentType.TagClosing:
                                                tagUnitsTargetUpdated.Add(new TagUnit(section.SectionId, string.Empty, section.Content, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));
                                                break;
                                            case SegmentSection.ContentType.LockedContent:
                                                tagUnitsTargetUpdated.Add(new TagUnit(section.SectionId, string.Empty, section.Content, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsLockedContent));
                                                break;
                                        }
                                    }
                                    #endregion

                                    var tagWarnings = GetTagWarnings(xTagUnits, tagUnitsTargetUpdated);

                                    if (tagWarnings.Count > 0)
                                    {
                                        if (TagUnitWarnings.ContainsKey(trgSegment.Properties.Id.Id))
                                        {
                                            TagUnitWarnings[trgSegment.Properties.Id.Id].AddRange(tagWarnings);
                                        }
                                        else
                                        {
                                            TagUnitWarnings.Add(trgSegment.Properties.Id.Id, tagWarnings);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error processing segment ID: " + segmentPair.Properties.Id.Id + "; Description: " + ex.Message);
                                }
                            }



                            #endregion


                            #region  |  add comment  |


                            if (commentsUpdated.Count > 0)
                            {
                                foreach (var commentNew in commentsUpdated)
                                {
                                    #region  |  date new  |
                                    //string date_new = _comment_new.date.Year.ToString() + "/"
                                    //       + _comment_new.date.Month.ToString().PadLeft(2, '0') + "/"
                                    //       + _comment_new.date.Day.ToString().PadLeft(2, '0') + "-"
                                    //       + _comment_new.date.Hour.ToString().PadLeft(2, '0') + ":"
                                    //       + _comment_new.date.Minute.ToString().PadLeft(2, '0') + ":"
                                    //       + _comment_new.date.Second.ToString().PadLeft(2, '0');
                                    #endregion
                                    bool foundComment = xComments.Any(commentExisting => string.Compare(commentNew.Text, commentExisting.Text, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare(commentNew.Author, commentExisting.Author, StringComparison.OrdinalIgnoreCase) == 0);

                                    if (!foundComment)
                                    {
                                        //CreateComment(segmentPair.Target, _comment_new.text, _comment_new.author, getSeverity(_comment_new.severity), _comment_new.date);
                                        //segmentPair.Target.ParentParagraphUnit.Properties.Comments.AddComments(CreateComment(segmentPair.Target, _comment_new.text, _comment_new.author, getSeverity(_comment_new.severity), _comment_new.date));                                 
                                        paragraphUnit.Properties.Comments = CreateComment(segmentPair.Target, commentNew.Text, commentNew.Author, getSeverity(commentNew.Severity), commentNew.Date);

                                    }
                                }
                            }

                            #endregion
                        }
                    }

                    Output.ProcessParagraphUnit(paragraphUnit);

                }
                else
                {

                    #region  |  check update status  |

                    //check update status
                    if (Processor.Settings.NotImportedTranslationStatus != string.Empty)
                    {
                        foreach (var inSegmentPair in paragraphUnit.SegmentPairs)
                        {
                            if (
                                string.Compare(inSegmentPair.Properties.ConfirmationLevel.ToString(),
                                    Processor.Settings.NotImportedTranslationStatus, StringComparison.OrdinalIgnoreCase) ==
                                0) continue;
                            TotalStatusChangesWithoutContentChanges++;

                            inSegmentPair.Properties.ConfirmationLevel = getConfirmationLevel(Processor.Settings.NotImportedTranslationStatus);

                            if (inSegmentPair.Properties.TranslationOrigin == null) continue;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation = ItemFactory.CreateTranslationOrigin();
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginalTranslationHash = inSegmentPair.Properties.TranslationOrigin.OriginalTranslationHash;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginSystem = inSegmentPair.Properties.TranslationOrigin.OriginSystem;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginType = inSegmentPair.Properties.TranslationOrigin.OriginType;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.RepetitionTableId = inSegmentPair.Properties.TranslationOrigin.RepetitionTableId;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.TextContextMatchLevel = inSegmentPair.Properties.TranslationOrigin.TextContextMatchLevel;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.MatchPercent = inSegmentPair.Properties.TranslationOrigin.MatchPercent;
                            inSegmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation.IsStructureContextMatch = inSegmentPair.Properties.TranslationOrigin.IsStructureContextMatch;
                        }
                    }

                    #endregion

                    Output.ProcessParagraphUnit(paragraphUnit);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating paragraph: " + paragraphUnit.Properties.ParagraphUnitId.Id + "\r\n\r\n" + ex.Message);
            }
        }


        public void SetFileProperties(IFileProperties fileInfo)
        {
            CurrentFileProperties = fileInfo;

            //this will output the individual file names that the sdlxliff file is comprized of.
            //In the case of a merged file, it will indicate the source file names as it is iterating
            //through the file.
            foreach (var t in fileInfo.FileConversionProperties.DependencyFiles)
            {
                var iDependencyFileProperties = t;

                if (iDependencyFileProperties.PreferredLinkage == DependencyFileLinkOption.Embed ||
                    iDependencyFileProperties.FileExists) continue;
                t.PreferredLinkage = DependencyFileLinkOption.ReferenceRelative;

                try
                {
                    if (!System.IO.File.Exists(iDependencyFileProperties.CurrentFilePath))
                    {
                        var dummyOutputFullPath = System.IO.Path.Combine(OutputPath, System.IO.Path.GetFileName(iDependencyFileProperties.CurrentFilePath));
                        CreatedDummyOutput = true;
                        DummyOutputFiles.Add(dummyOutputFullPath);
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(dummyOutputFullPath))
                        {
                            sw.WriteLine(string.Empty);
                            sw.Flush();
                            sw.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
            Output.SetFileProperties(fileInfo);
        }



        #endregion

        #region  |  private methods  |

        private Severity getSeverity(string severityStr)
        {           
            switch (severityStr)
            {
                case "High": return Severity.High;
                case "Medium": return Severity.Medium; 
                default: return Severity.Low;
            }           
        }
        private ConfirmationLevel getConfirmationLevel(string segmentStatus)
        {
            switch (segmentStatus)
            {
                case "Unspecified": return ConfirmationLevel.Unspecified; //not translated
                case "Draft": return ConfirmationLevel.Draft; 
                case "Translated": return ConfirmationLevel.Translated;
                case "RejectedTranslation": return ConfirmationLevel.RejectedTranslation; 
                case "ApprovedTranslation": return ConfirmationLevel.ApprovedTranslation;                                                
                case "RejectedSignOff": return ConfirmationLevel.RejectedSignOff;
                case "ApprovedSignOff": return ConfirmationLevel.ApprovedSignOff;
                default: return ConfirmationLevel.Unspecified;
            }
        }

        private int _iStartIndex;
        private void WriteTargetSegment(ISegment trgSegment, ITagPair tagPair, IReadOnlyList<SegmentSection> targetSections
            , IList<ITagPair> xTagPairsExisting, IList<IPlaceholderTag> xPlaceHolderTagsExisting, IList<ILockedContent> xLockedContentTagsExisting)
        {

            try
            {
                for (var i = _iStartIndex; i < targetSections.Count; i++)
                {
                    _iStartIndex = i;

                    var section = targetSections[i];

                    if (section.Type == SegmentSection.ContentType.Text)
                    {
                        if (tagPair != null)
                            tagPair.Add(CreateText(section.Content));
                        else
                            trgSegment.Add(CreateText(section.Content));
                    }
                    else if (section.Type == SegmentSection.ContentType.LockedContent)
                    {
                  

                        var lockedContentExisting = GetLockedContent(section, xLockedContentTagsExisting);
                        if (lockedContentExisting != null)
                        {
                            var lockedContent = lockedContentExisting;
                            lockedContent.RemoveFromParent();

                            if (tagPair != null)
                                tagPair.Add(lockedContent);
                            else
                                trgSegment.Add(lockedContent);
                        }
                        else
                        {
                            if (tagPair != null)
                                tagPair.Add(CreateText(section.Content));
                            else
                                trgSegment.Add(CreateText(section.Content));
                        }
                    }
                    else if (section.Type == SegmentSection.ContentType.Placeholder)
                    {
                        var placeholderUnitExisting = GetPlaceholder(section, xPlaceHolderTagsExisting);
                        IPlaceholderTag placeholderUnit = null;
                        if (placeholderUnitExisting != null)
                        {
                            placeholderUnit = placeholderUnitExisting;
                            placeholderUnit.RemoveFromParent();

                            if (tagPair != null)
                                tagPair.Add(placeholderUnit);
                            else
                                trgSegment.Add(placeholderUnit);
                        }
                        else
                        {
                            //new placeholder; report it!                            
                            var refId = string.Empty;
                            var sTagName = Parser.GetStartTagName(section.Content, ref refId);
                            var tagProperties = ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(section.Content);
                            //tagProperties.TagId = new TagId((uniqueTagId++).ToString());
                            tagProperties.TagId = new TagId(Guid.NewGuid().ToString());
                            tagProperties.DisplayText = sTagName;
                            placeholderUnit = ItemFactory.CreatePlaceholderTag(tagProperties);

                            if (tagPair != null)
                                tagPair.Add(placeholderUnit);
                            else
                                trgSegment.Add(placeholderUnit);
                        }


                    }
                    else
                    {
                        if (section.Type == SegmentSection.ContentType.TagClosing)
                        {
                            if (tagPair != null)
                            {
                                var sEndTagPushed = Parser.GetEndTagName(tagPair.EndTagProperties.TagContent);
                                var sEndTagPop = Parser.GetEndTagName(section.Content);

                                if (string.Compare(sEndTagPushed, sEndTagPop, StringComparison.OrdinalIgnoreCase) != 0)
                                {
                                    throw new Exception("Expecting the closing tag for '" + sEndTagPushed + "'; found '" + sEndTagPop + "'");
                                }
                                _iStartIndex = i;
                                tagPair = null;
                                break;
                            }
                            var placeholderUnitExisting = GetPlaceholder(section, xPlaceHolderTagsExisting);
                            IPlaceholderTag placeholderUnit = null;
                            if (placeholderUnitExisting != null)
                            {
                                placeholderUnit = placeholderUnitExisting;
                                placeholderUnit.RemoveFromParent();

                                trgSegment.Add(placeholderUnit);
                            }
                            else
                            {
                                //new placeholder; report it!                                

                                var refId = string.Empty;
                                var sTagName = Parser.GetStartTagName(section.Content, ref refId);
                                var tagProperties = ItemFactory.PropertiesFactory.CreatePlaceholderTagProperties(section.Content);
                                //tagProperties.TagId = new TagId((uniqueTagId++).ToString());
                                tagProperties.TagId = new TagId(Guid.NewGuid().ToString());
                                tagProperties.DisplayText = sTagName;
                                placeholderUnit = ItemFactory.CreatePlaceholderTag(tagProperties);

                                //IPlaceholderTagProperties tagProperties = placeholderTag.Properties;
                                //IPlaceholderTag _placeholderTag = ItemFactory.CreatePlaceholderTag(tagProperties);

                                trgSegment.Add(placeholderUnit);
                            }
                        }
                        else if (section.Type == SegmentSection.ContentType.Tag)
                        {
                            var tagunitExisting = GetTag(section, xTagPairsExisting);

                            ITagPair tagPairNew = null;
                            if (tagunitExisting != null)
                            {
                                tagunitExisting.Clear();
                                tagPairNew = tagunitExisting;
                                tagPairNew.RemoveFromParent();

                                if (tagPair != null)
                                    tagPair.Add(tagPairNew);
                                else
                                    trgSegment.Add(tagPairNew);
                            }
                            else
                            {
                                var placeholderUnitExisting = GetPlaceholder(section, xPlaceHolderTagsExisting);
                                if (placeholderUnitExisting != null)
                                {
                                    var placeholderUnit = placeholderUnitExisting;
                                    placeholderUnit.RemoveFromParent();

                                    if (tagPair != null)
                                        tagPair.Add(placeholderUnit);
                                    else
                                        trgSegment.Add(placeholderUnit);
                                }
                                else
                                {

                                    var lockedContentExisting = GetLockedContent(section, xLockedContentTagsExisting);
                                    if (lockedContentExisting != null)
                                    {
                                        var lockedContent = lockedContentExisting;
                                        lockedContent.RemoveFromParent();

                                        if (tagPair != null)
                                            tagPair.Add(lockedContent);
                                        else
                                            trgSegment.Add(lockedContent);
                                    }
                                    else
                                    {
                                        //Tag is new to the content, create new properties                                  

                                        var refId = string.Empty;
                                        var sTagName = Parser.GetStartTagName(section.Content, ref refId);
                                        var startTag = ItemFactory.PropertiesFactory.CreateStartTagProperties(sTagName);
                                        //startTag.TagId = new TagId((uniqueTagId++).ToString());
                                        startTag.TagId = new TagId(Guid.NewGuid().ToString());
                                        startTag.DisplayText = sTagName;
                                        startTag.TagContent = section.Content;

                                        var foundCfFormatting = false;
                                        if (section.Content.ToLower().StartsWith("<cf "))
                                            startTag = GetTagFormatting(startTag, section.Content, ref foundCfFormatting);

                                        startTag.CanHide = foundCfFormatting;

                                        var endTag = ItemFactory.PropertiesFactory.CreateEndTagProperties(sTagName);
                                        endTag.DisplayText = sTagName;

                                        if (section.Content.Trim().StartsWith("["))
                                            endTag.TagContent = "]";
                                        else
                                            endTag.TagContent = "</" + sTagName + ">";

                                        endTag.CanHide = foundCfFormatting;

                                        tagPairNew = ItemFactory.CreateTagPair(startTag, endTag);


                                        if (tagPair != null)
                                            tagPair.Add(tagPairNew);
                                        else
                                            trgSegment.Add(tagPairNew);
                                    }
                                }
                            }



                            _iStartIndex = (i + 1);
                            WriteTargetSegment(trgSegment, tagPairNew, targetSections, xTagPairsExisting, xPlaceHolderTagsExisting, xLockedContentTagsExisting);
                            i = _iStartIndex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating segment: " + trgSegment.Properties.Id.Id + "\r\n\r\n" + ex.Message);
            }
        }

        private IContextProperties CreateContext(string spec, string unitId)
        {
            // context info for type information, e.g. heading, paragraph, etc.
            IContextProperties contextProperties = ItemFactory.PropertiesFactory.CreateContextProperties();
            IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextInfo.Purpose = ContextPurpose.Information;

            // add unit id as metadata
            IContextInfo contextId = ItemFactory.PropertiesFactory.CreateContextInfo("UnitId");
            contextId.SetMetaData("UnitID", unitId);

            
            switch (spec)
            {
                case "Heading":
                    contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Topic);
                    contextInfo.DisplayColor = Color.Green;
                    break;
                case "Box":
                    contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.TextBox);
                    contextInfo.DisplayColor = Color.Gold;
                    break;
                case "Paragraph":
                    contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
                    contextInfo.DisplayColor = Color.Silver;
                    break;
                default:
                    break;
            }

            contextProperties.Contexts.Add(contextInfo);
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
        }


        private static IStartTagProperties GetTagFormatting(IStartTagProperties startTag, string sectionContent, ref bool canHide)
        {
            
            //"This is a <xProtected_ type=\"tag\" id=\"1\"><cf bold=\"True\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"1\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"2\"><cf underline=\"single\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"2\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"3\"><cf italic=\"True\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"3\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"5\"><cf size=\"24\" nfa=\"true\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"5\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"7\"><cf font=\"Arial Black\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"7\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"8\"><cf subscript=\"true\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"8\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"9\"><cf strikethrogh=\"True\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"9\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"10\"><cf superscript=\"true\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"10\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"11\"><cf highlight=\"yellow\"></xProtected_>single<xProtected_ type=\"tagClosing\" id=\"11\"></cf></xProtected_> line of text for testing"	string
            //"This is a <xProtected_ type=\"tag\" id=\"12\"><cf color=\"FF0000\"></xProtected_>single <xProtected_ type=\"tagClosing\" id=\"12\"></cf></xProtected_>line of text for testing"	string

            var rCf = new Regex(@"\s+(?<name>[^\s""]+)\=""(?<value>[^""]*)""", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var mcRCf = rCf.Matches(sectionContent);

            var unKnownFormatting = false;
            foreach (Match mRCf in mcRCf)
            {
              
                var name = mRCf.Groups["name"].Value;
                var value = mRCf.Groups["value"].Value;

                if (string.Compare(name, "bold", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var b = !(string.Compare(value, "off", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "0", StringComparison.OrdinalIgnoreCase) == 0);

                    var bold = new Bold(b);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(bold);
                    canHide = true;
                }
                else if (string.Compare(name, "underline", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var b = !(string.Compare(value, "off", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "0", StringComparison.OrdinalIgnoreCase) == 0);

                    var underline = new Underline(b);

                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(underline);
                    canHide = true;
                }
                else if (string.Compare(name, "italic", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var b = !(string.Compare(value, "off", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "0", StringComparison.OrdinalIgnoreCase) == 0);

                    var italic = new Italic(b);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(italic);
                    canHide = true;
                }
                else if (string.Compare(name, "size", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    try
                    {
                        var size = new FontSize(Convert.ToInt64(value));

                        if (startTag.Formatting == null)
                            startTag.Formatting = new FormattingGroup();

                        startTag.Formatting.Add(size);
                        canHide = true;
                    }
                    catch
                    {
                        unKnownFormatting = true;
                    }
                }
                else if (string.Compare(name, "font", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var fontName = new FontName(value);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(fontName);
                    canHide = true;
                }
                else if (string.Compare(name, "subscript", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var subScript = new TextPosition(TextPosition.SuperSub.Subscript);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(subScript);
                    canHide = true;
                }
                else if (string.Compare(name, "strikethrogh", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var b = !(string.Compare(value, "off", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(value, "0", StringComparison.OrdinalIgnoreCase) == 0);

                    var strikethrough = new Strikethrough(b);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(strikethrough);
                    canHide = true;
                }
                else if (string.Compare(name, "superscript", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var superScript = new TextPosition(TextPosition.SuperSub.Superscript);
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    startTag.Formatting.Add(superScript);
                    canHide = true;
                }
                else if (string.Compare(name, "highlight", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var backgroundColor = new BackgroundColor();
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    try
                    {
                        var col = Color.FromName(value);
                        if (!col.IsKnownColor && col.A == 0 && col.R == 0 && col.G == 0 && col.B == 0)
                        {
                            col = ColorTranslator.FromHtml("#" + value.Replace("#", ""));
                        }


                        backgroundColor.Value = col;
                        startTag.Formatting.Add(backgroundColor);
                        canHide = true;
                    }
                    catch
                    {
                        unKnownFormatting = true;
                    }
                }
                else if (string.Compare(name, "color", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var textColor = new TextColor();
                    if (startTag.Formatting == null)
                        startTag.Formatting = new FormattingGroup();

                    try
                    {
                        var col = Color.FromName(value);
                        if (!col.IsKnownColor && col.A == 0 && col.R == 0 && col.G == 0 && col.B == 0)
                        {

                            col = ColorTranslator.FromHtml("#" + value.Replace("#", ""));

                        }
                        textColor.Value = col;
                        startTag.Formatting.Add(textColor);
                        canHide = true;
                    }
                    catch
                    {
                        unKnownFormatting = true;
                    }
                }
                else
                {

                    unKnownFormatting = false;
                }

            }
            if (unKnownFormatting)
            {
                canHide = false;
            }

            return startTag;
        }
        private static ITagPair GetTag(SegmentSection section, IList<ITagPair> xTagPairs)
        {
            ITagPair tagunitUpdated = null;
            var i = 0;
            foreach (var tagunit in xTagPairs)
            {
                if (tagunit.TagProperties.TagId.Id == section.SectionId)
                {
                    tagunitUpdated = tagunit;
                    xTagPairs.RemoveAt(i);
                    break;
                }
                i++;
            }
            i = 0;
            if (tagunitUpdated != null) return tagunitUpdated;
            {
                foreach (var tagunit in xTagPairs)
                {
                    if (tagunit.TagProperties.TagContent != section.Content) continue;
                    tagunitUpdated = tagunit;
                    xTagPairs.RemoveAt(i);
                    break;
                }
                i++;
            }
            return tagunitUpdated;
        }
        private static IPlaceholderTag GetPlaceholder(SegmentSection section, IList<IPlaceholderTag> xPlaceholderTags)
        {

            IPlaceholderTag placeholderTagUpdated = null;
            var i = 0;
            foreach (var tagunit in xPlaceholderTags)
            {
                if (tagunit.TagProperties.TagId.Id == section.SectionId)
                {
                    placeholderTagUpdated = tagunit;
                    xPlaceholderTags.RemoveAt(i);
                    break;
                }
                i++;
            }
            i = 0;
            if (placeholderTagUpdated != null) return placeholderTagUpdated;
            {
                foreach (var tagunit in xPlaceholderTags)
                {
                    if (tagunit.TagProperties.TagContent != section.Content) continue;
                    placeholderTagUpdated = tagunit;
                    xPlaceholderTags.RemoveAt(i);
                    break;
                }
                i++;
            }


            return placeholderTagUpdated;
        }
        private static ILockedContent GetLockedContent(SegmentSection section, IList<ILockedContent> xLockedContentTags)
        {

            ILockedContent placeholderTagUpdated = null;

            var i = 0;
            foreach (var tagunit in xLockedContentTags)
            {
                if ( string.Compare(tagunit.Content.ToString().Trim(), section.Content.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
                {
                    i++;
                    continue;
                }
                placeholderTagUpdated = tagunit;
                xLockedContentTags.RemoveAt(i);
                break;
            }


            return placeholderTagUpdated;
        }
        private IText CreateText(string segText)
        {
            var textProperties = ItemFactory.PropertiesFactory.CreateTextProperties(segText);
            var textContent = ItemFactory.CreateText(textProperties);

            return textContent;
        }
        private IPlaceholderTag CreatePlaceholderTag(IPlaceholderTag placeholderTag)
        {
            var tagProperties = placeholderTag.Properties;
            var tag = ItemFactory.CreatePlaceholderTag(tagProperties);

            return tag;

        }
        private ICommentProperties CreateComment(ISegment trgSegment, string commentText, string author, Severity severity, DateTime date)
        {
            var commentProperties = ItemFactory.PropertiesFactory.CreateCommentProperties();
    
            var comment = ItemFactory.PropertiesFactory.CreateComment(commentText, author, severity);
            comment.Date = date;           
            commentProperties.Add(comment);
       

            return commentProperties;
        }



        public List<TagUnitWarning> GetTagWarnings(List<TagUnit> original, List<TagUnit> updated)
        {
            var tagWarnings = new List<TagUnitWarning>();

          

            var iSource = original.Count;
            var iTarget = updated.Count;

            var originalTmp = original.ToList();

            var updatedTmp = updated.ToList();


            foreach (var tagUnit in updatedTmp)
            {
                var index = 0;
                foreach (var tagUnitOriginalTmp in originalTmp)
                {
                    if (tagUnit.Content.Trim() == tagUnitOriginalTmp.Content.Trim())
                    {
                        originalTmp.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }


            foreach (var tagUnit in original)
            {
                var index = 0;
                foreach (var tagUnitUpdatedTmp in updatedTmp)
                {
                    if (tagUnit.Content.Trim() == tagUnitUpdatedTmp.Content.Trim())
                    {
                        updatedTmp.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }


            if (originalTmp.Count > 0)
            {
                const string warningMessage = "Tag Content miss-match; tags removed in the updated translation";
                tagWarnings.Add(new TagUnitWarning(TagUnitWarning.WarningType.Removed, warningMessage, originalTmp));
            }

            if (updatedTmp.Count > 0)
            {
                const string warningMessage = "Tag Content miss-match; tags added in the updated translation";
                tagWarnings.Add(new TagUnitWarning(TagUnitWarning.WarningType.Removed, warningMessage, updatedTmp));
            }

            if (originalTmp.Count != 0 || updatedTmp.Count != 0) return tagWarnings;
            {
                if (original.Count <= 0 || updated.Count <= 0) return tagWarnings;
                //check the placement of the tags...
                var strOriginal = string.Empty;
                var strUpdated = string.Empty;
                strOriginal = original.Aggregate(strOriginal, (current, tagUnit) => current + tagUnit.Content);

                strUpdated = updated.Aggregate(strUpdated, (current, tagUnit) => current + tagUnit.Content);


                if (string.CompareOrdinal(strOriginal.Trim(), strUpdated.Trim()) == 0) return tagWarnings;
                var warningMessage = "Found variation in the placement of the tags!\r\n";
                warningMessage += "Original Translation: " + strOriginal.Trim() + "\r\n" ;
                warningMessage += "Updated Translation: " + strUpdated.Trim() ;
                tagWarnings.Add(new TagUnitWarning(TagUnitWarning.WarningType.Placement,warningMessage, new List<TagUnit>()));
            }


            return tagWarnings;
        }




        #endregion

        #region IBilingualParser Members

        public IDocumentProperties DocumentProperties
        {
            get;
            set;
        }

        #endregion

        #region IParser Members

        public override bool ParseNext()
        {
            throw new NotImplementedException();
        }

        public new event EventHandler<ProgressEventArgs> Progress;

        #endregion

        #region IBilingualFileTypeComponent Members

        public IDocumentItemFactory ItemFactory
        {
            get;
            set;
        }

        public new IBilingualContentMessageReporter MessageReporter
        {
            get;
            set;
        }

        #endregion


        #region IDependencyFileProperties Members

        public string CurrentFilePath
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public IDisposable DisposableObject
        {
            get;
            set;
        }

        public DependencyFileUsage ExpectedUsage
        {
            get;
            set;
        }

        public bool FileExists
        {
            get;
            set;
        }

        public string Id
        {
            get;
            set;
        }

        public string OriginalFilePath
        {
            get;
            set;
        }

        public DateTime OriginalLastChangeDate
        {
            get;
            set;
        }

        public string PathRelativeToConverted
        {
            get;
            set;
        }

        public DependencyFileLinkOption PreferredLinkage
        {
            get
            {
                return DependencyFileLinkOption.Ignore;
            }
            set
            {
                value = DependencyFileLinkOption.Ignore;
            }
        }

        public Sdl.FileTypeSupport.Framework.FileJanitor ZippedCurrentFile
        {
            get;
            set;
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
