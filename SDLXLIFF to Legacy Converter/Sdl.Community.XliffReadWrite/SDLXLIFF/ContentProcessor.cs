using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.XliffReadWrite.SDLXLIFF
{
    internal class ContentProcessor : IBilingualContentProcessor
    {

        internal bool IncludeTagText = true;
        internal Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnits { get; set; }


        public CultureInfo SourceLanguageCultureInfo { get; set; }
        public CultureInfo TargetLanguageCultureInfo { get; set; }

        private IFileProperties CurrentFileProperties { get; set; }
        private Dictionary<string, ParagraphUnit> ParagraphUnits { get; set; }

        internal string OutputPath = string.Empty;
        internal bool CreatedDummyOutput;
        internal List<string> DummyOutputFiles = new List<string>();

        #region  |  Content Generator  |
        private ContentGenerator _contentGeneratorProcessor;

        internal ContentGenerator ContentGeneratorProcessor
        {
            get { return _contentGeneratorProcessor ?? (_contentGeneratorProcessor = new ContentGenerator()); }
        }
        #endregion

        #region  |  IBilingualContentProcessor Members  |


        public ContentProcessor()
        {
            FileParagraphUnits = new Dictionary<string, Dictionary<string, ParagraphUnit>>();
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
            //not needed for this implementation
        }

        public void FileComplete()
        {
            FileParagraphUnits.Add(CurrentFileProperties.FileConversionProperties.OriginalFilePath, ParagraphUnits);
        }

        public void Initialize(IDocumentProperties documentInfo)
        {


            SourceLanguageCultureInfo = documentInfo.SourceLanguage.CultureInfo;
            if (documentInfo.TargetLanguage != null && documentInfo.TargetLanguage.CultureInfo != null)
                TargetLanguageCultureInfo = documentInfo.TargetLanguage.CultureInfo;
            else
                TargetLanguageCultureInfo = SourceLanguageCultureInfo;
        }




        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            #region  |  for comments that were added at the paragraph level  |

            var paragraphComments = new List<Comment>();

            if (paragraphUnit.Properties.Comments != null && paragraphUnit.Properties.Comments.Count > 0)
            {
                foreach (var comment in paragraphUnit.Properties.Comments.Comments)
                {
                   
                    var xComment = new Comment { Author = comment.Author };
                    if (comment.DateSpecified)
                        xComment.Date = comment.Date;
                    xComment.Severity = comment.Severity.ToString();
                    xComment.Text = comment.Text;
                    xComment.Version = comment.Version;

                    paragraphComments.Add(xComment);
                }
            }
            #endregion



            var index = -1;
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                index++;

                #region  |  initialize and assign values to xSegmentPair  |

                var pair = new SegmentPair();

                ContentGeneratorProcessor.ProcessSegment(segmentPair.Source, IncludeTagText);
                pair.Source = ContentGeneratorProcessor.PlainText.ToString();
                pair.SourceSections = ContentGeneratorProcessor.SegmentSections;


                ContentGeneratorProcessor.ProcessSegment(segmentPair.Target, IncludeTagText);
                pair.Target = ContentGeneratorProcessor.PlainText.ToString();
                pair.TargetSections = ContentGeneratorProcessor.SegmentSections;

                if (pair.SourceSections.Count > 0 && pair.TargetSections.Count == 0)
                {
                    if (Processor.ProcessorSettings.CopySourceToTargetEmptyTranslations)
                    {
                        pair.Target = pair.Source;
                        pair.TargetSections = pair.SourceSections;
                    }
                }

                pair.Id = segmentPair.Properties.Id.Id;
                pair.SegmentStatus = segmentPair.Properties.ConfirmationLevel.ToString();
                pair.IsLocked = segmentPair.Properties.IsLocked;
                pair.Comments = ContentGeneratorProcessor.Comments;

                #region  |  for comments that were added at the paragraph level  |

                if (paragraphComments.Count > 0 && index == 0)
                {
                    if (pair.Comments.Count > 0)
                    {
                        foreach (var xCommentParagraph in paragraphComments)
                        {
                            var foundComment = pair.Comments.Any(xCommentSegmentPair => string.CompareOrdinal(xCommentParagraph.Text.Trim(), xCommentSegmentPair.Text.Trim()) == 0);
                            if (!foundComment)
                            {
                                pair.Comments.Add(xCommentParagraph);
                            }
                        }
                    }
                    else
                    {
                        foreach (var xComment in paragraphComments)
                        {
                            pair.Comments.Add(xComment);
                        }
                    }
                }
                #endregion


                if (segmentPair.Properties.TranslationOrigin != null)
                {
                    pair.TranslationOrigin.IsRepeated = segmentPair.Properties.TranslationOrigin.IsRepeated;
                    pair.TranslationOrigin.IsStructureContextMatch = segmentPair.Properties.TranslationOrigin.IsStructureContextMatch;
                    pair.TranslationOrigin.MatchPercentage = segmentPair.Properties.TranslationOrigin.MatchPercent;
                    pair.TranslationOrigin.OriginSystem = segmentPair.Properties.TranslationOrigin.OriginSystem;
                    pair.TranslationOrigin.OriginType = segmentPair.Properties.TranslationOrigin.OriginType;
                    pair.TranslationOrigin.RepetitionTableId = segmentPair.Properties.TranslationOrigin.RepetitionTableId.Id;
                    pair.TranslationOrigin.TextContextMatchLevel = segmentPair.Properties.TranslationOrigin.TextContextMatchLevel.ToString();

                    pair.TranslationOrigin.OriginalTranslationHash = segmentPair.Properties.TranslationOrigin.OriginalTranslationHash;
                    pair.TranslationOrigin.OriginalTranslationHashSource = segmentPair.Source.Properties.TranslationOrigin.OriginalTranslationHash;
                    pair.TranslationOrigin.OriginalTranslationHashTarget = segmentPair.Target.Properties.TranslationOrigin.OriginalTranslationHash;



                }

                #endregion


                #region  |  add the SegmentPair to the xParagraphs dictionary  |

                if (ParagraphUnits.ContainsKey(paragraphUnit.Properties.ParagraphUnitId.Id))
                {
                    var xParagraphUnit = ParagraphUnits[paragraphUnit.Properties.ParagraphUnitId.Id];
                    xParagraphUnit.SegmentPairs.Add(pair);
                    ParagraphUnits[paragraphUnit.Properties.ParagraphUnitId.Id] = xParagraphUnit;
                }
                else
                {
                    var pairItem = new List<SegmentPair> { pair };
                    ParagraphUnits.Add(paragraphUnit.Properties.ParagraphUnitId.Id, new ParagraphUnit(paragraphUnit.Properties.ParagraphUnitId.Id, pairItem, CurrentFileProperties.FileConversionProperties.OriginalFilePath));
                }
                #endregion

                if (segmentPair.Target.Count != 0 || !Processor.ProcessorSettings.CopySourceToTargetEmptyTranslations)
                    continue;
                foreach (var item in segmentPair.Source)
                    segmentPair.Target.Add((IAbstractMarkupData)item.Clone());
            }
        }

        public void SetFileProperties(IFileProperties fileInfo)
        {
            CurrentFileProperties = fileInfo;
            ParagraphUnits = new Dictionary<string, ParagraphUnit>();

            // this should output the individual file names that the sdlxliff file is comprized of.
            // in the case of a merged file, it will indicate the source file names as it is iterating
            // through the file.
            foreach (var fileProperties in fileInfo.FileConversionProperties.DependencyFiles)
            {
                var iDependencyFileProperties = fileProperties;

                if (iDependencyFileProperties.PreferredLinkage == DependencyFileLinkOption.Embed ||
                    iDependencyFileProperties.FileExists) continue;
                fileProperties.PreferredLinkage = DependencyFileLinkOption.ReferenceRelative;

                try
                {
                    if (!System.IO.File.Exists(iDependencyFileProperties.CurrentFilePath))
                    {
                        if (iDependencyFileProperties.CurrentFilePath != null)
                        {
                            var dummyOutputFullPath = System.IO.Path.Combine(OutputPath, System.IO.Path.GetFileName(iDependencyFileProperties.CurrentFilePath));
                            CreatedDummyOutput = true;
                            DummyOutputFiles.Add(dummyOutputFullPath);
                            using (var sw = new System.IO.StreamWriter(dummyOutputFullPath))
                            {
                                sw.WriteLine(string.Empty);
                                sw.Flush();
                                sw.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }



        #endregion

    }
}
