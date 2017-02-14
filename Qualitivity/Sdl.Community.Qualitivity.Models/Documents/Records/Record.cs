using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{

    [Serializable]
    public class Record : ICloneable
    {
        public int Id { get; set; }
        public int DocumentActivityId { get; set; }

        public string ParagraphId { get; set; }
        public string SegmentId { get; set; }

        public long WordCount { get; set; }

        public int CharsCount { get; set; }

        public int TagsCount { get; set; }

        public int PlaceablesCount { get; set; }

      

        public DateTime? Started { get; set; }
        public DateTime? Stopped { get; set; }
        public long TicksElapsed { get; set; }

     
        [XmlIgnore]
        public string EditDistance { get; set; }
        [XmlIgnore]
        public string EditDistanceRelative { get; set; }
        [XmlIgnore]
        public string PemPercentage { get; set; }

        public TranslationOrigins TranslationOrigins { get; set; }
        public ContentSections ContentSections { get; set; }
        public List<KeyStroke> TargetKeyStrokes { get; set; }
        public List<Comment> Comments { get; set; }
        public List<QualityMetric> QualityMetrics { get; set; }



        //this is used only in the default report area to hold merged date information that is displayed in the report... 
        [XmlIgnore]
        public List<string> MergedDatesTemp { get; set; }

        //used in the document records area to display the data
        [XmlIgnore]
        public string SegmentIdIndex { get; set; }
        [XmlIgnore]
        public Span StatusLines { get; set; }
        [XmlIgnore]
        public Span MatchLines { get; set; }
        [XmlIgnore]
        public Span PemPercentageLines { get; set; }
        [XmlIgnore]
        public string MatchColor { get; set; }
        [XmlIgnore]
        public string ElapsedTime { get; set; }
        [XmlIgnore]
        public Span CommentLines { get; set; }
        [XmlIgnore]
        public Span QualityMetricsLines { get; set; }

        public Record()
        {

            Id = -1;
            DocumentActivityId = -1;

            ParagraphId = string.Empty;
            SegmentId = string.Empty;

            WordCount = 0;
            CharsCount = 0;
            TagsCount = 0;
            PlaceablesCount = 0;

            Started = null;
            Stopped = null;
            TicksElapsed = 0;

          
            EditDistance = string.Empty;
            EditDistanceRelative = string.Empty;
            PemPercentage = string.Empty;

            TranslationOrigins = new TranslationOrigins();
            ContentSections = new ContentSections();
            TargetKeyStrokes = null;
            Comments = new List<Comment>();
            QualityMetrics = new List<QualityMetric>();

            MergedDatesTemp = new List<string>();


            SegmentIdIndex = string.Empty;
            StatusLines = null;
            MatchLines = null;
            PemPercentageLines = null;
            MatchColor = string.Empty;
            ElapsedTime = string.Empty;
            CommentLines = null;
            QualityMetricsLines = null;


        }

        public object Clone()
        {
            var record = new Record
            {
                Id = Id,
                DocumentActivityId = DocumentActivityId,
                ParagraphId = ParagraphId,
                SegmentId = SegmentId,
                TranslationOrigins = (TranslationOrigins) TranslationOrigins.Clone(),
                WordCount = WordCount,
                CharsCount = CharsCount,
                TagsCount = TagsCount,
                PlaceablesCount = PlaceablesCount,
                Started = Started,
                Stopped = Stopped,
                TicksElapsed = TicksElapsed,             
                ContentSections = (ContentSections) ContentSections.Clone()
            };


            if (TargetKeyStrokes != null)
            {
                record.TargetKeyStrokes = new List<KeyStroke>();
                foreach (var keyStroke in TargetKeyStrokes)
                    record.TargetKeyStrokes.Add((KeyStroke)keyStroke.Clone());
            }
            else
                record.TargetKeyStrokes = TargetKeyStrokes;


            record.Comments = new List<Comment>();
            foreach (var comment in Comments)
                record.Comments.Add((Comment)comment.Clone());


            if (record.QualityMetrics != null)
            {
                record.QualityMetrics = new List<QualityMetric>();
                foreach (var qualityMetric in QualityMetrics)
                    record.QualityMetrics.Add((QualityMetric)qualityMetric.Clone());
            }
            else
                record.QualityMetrics = QualityMetrics;



            record.MergedDatesTemp = new List<string>();
            foreach (var mergedDateTemp in MergedDatesTemp)
                record.MergedDatesTemp.Add((string)mergedDateTemp.Clone());


            //don't want this cloning
            record.EditDistance = EditDistance;
            record.EditDistanceRelative = EditDistanceRelative;
            record.PemPercentage = PemPercentage;

            record.SegmentIdIndex = string.Empty;
            record.StatusLines = null;
            record.MatchLines = null;
            record.PemPercentageLines = null;
            record.MatchColor = string.Empty;
            record.ElapsedTime = string.Empty;
            record.CommentLines = null;
            QualityMetricsLines = null;

            return record;
        }
    }
}
