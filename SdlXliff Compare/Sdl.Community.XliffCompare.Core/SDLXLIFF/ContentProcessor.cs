using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.XliffCompare.Core.SDLXLIFF
{
    public class ContentProcessor : IBilingualContentProcessor
    {

        internal bool IncludeTagText = true;
        internal Dictionary<string, Dictionary<string, ParagraphUnit>> FileParagraphUnits { get; set; }


        private IFileProperties CurrentFileProperties { get; set; }
        private Dictionary<string, ParagraphUnit> ParagraphUnits { get; set; }

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
			var containsKey = FileParagraphUnits.ContainsKey(CurrentFileProperties.FileConversionProperties.OriginalFilePath);
			if (!containsKey&&ParagraphUnits.Count>0)
			{
				FileParagraphUnits.Add(CurrentFileProperties.FileConversionProperties.OriginalFilePath, ParagraphUnits);
			}
           
        }

        public void Initialize(IDocumentProperties documentInfo)
        {
            //not needed for this implementation
        }

        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            #region  |  for comments that were added at the paragraph level  |
            
            var paragraphComments = new List<Comment>();

            if (paragraphUnit.Properties.Comments != null && paragraphUnit.Properties.Comments.Count > 0)
            {                
                foreach (var comment in paragraphUnit.Properties.Comments.Comments)
                {
                    var item = new Comment
                    {
                        Author = comment.Author,
                        Severity = comment.Severity.ToString(),
                        Text = comment.Text,
                        Version = comment.Version
                    };

                    if (comment.DateSpecified)
                        item.Date = comment.Date;

                    paragraphComments.Add(item);
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
                pair.Id = segmentPair.Properties.Id.Id;                
                pair.SegmentStatus =  segmentPair.Properties.ConfirmationLevel.ToString();
                pair.IsLocked = segmentPair.Properties.IsLocked;
                pair.Comments = ContentGeneratorProcessor.Comments;

                #region  |  for comments that were added at the paragraph level  |

                if (paragraphComments.Count > 0 && index == 0)
                {
                    if (pair.Comments.Count > 0)
                    {
                        foreach (var commentParagraph in paragraphComments)
                        {
                            var foundComment = pair.Comments.Any(xCommentSegmentPair 
                                => string.CompareOrdinal(commentParagraph.Text.Trim(), xCommentSegmentPair.Text.Trim()) == 0);


                            if (!foundComment)
                            {
                                pair.Comments.Add(commentParagraph);
                            }
                        }
                    }
                    else
                    {
                        foreach (var comment in paragraphComments)
                        {
                            pair.Comments.Add(comment);
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
                }

                #endregion
                
                #region  |  add the SegmentPair to the xParagraphs dictionary  |

                if (ParagraphUnits.ContainsKey(paragraphUnit.Properties.ParagraphUnitId.Id))
                {
                    var unit = ParagraphUnits[paragraphUnit.Properties.ParagraphUnitId.Id];                                        
                    unit.SegmentPairs.Add(pair);
                    ParagraphUnits[paragraphUnit.Properties.ParagraphUnitId.Id] = unit;
                }
                else
                {
                    var segmentPairs = new List<SegmentPair> {pair};
                    ParagraphUnits.Add(paragraphUnit.Properties.ParagraphUnitId.Id, new ParagraphUnit(paragraphUnit.Properties.ParagraphUnitId.Id, segmentPairs));
                }
                #endregion              
            }
        }


        public void SetFileProperties(IFileProperties fileInfo)
        {
            CurrentFileProperties = fileInfo;
            ParagraphUnits = new Dictionary<string, ParagraphUnit>();
        }

        #endregion

       
    }  
}
