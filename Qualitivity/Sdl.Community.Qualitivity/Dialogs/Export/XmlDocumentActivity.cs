using System;
using System.Collections.Generic;
using Sdl.Community.Structures.Documents.Records;

namespace Sdl.Community.Qualitivity.Dialogs.Export
{
    public class XmlDocumentActivity
    {

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public DateTime? DocumentStartDate { get; set; }
        public DateTime? DocumentStopDate { get; set; }
        public double DocumentTotalSeconds { get; set; }
        public string RecordId { get; set; }
        public string ParagraphId { get; set; }
        public string SegmentId { get; set; }

        public string OriginalConfirmationLevel { get; set; }
        public string OriginalTranslationStatus { get; set; }
        public string OriginalOriginSystem { get; set; }
        public string OriginalOriginType { get; set; }

        public string UpdatedConfirmationLevel { get; set; }
        public string UpdatedTranslationStatus { get; set; }
        public string UpdatedOriginSystem { get; set; }
        public string UpdatedOriginType { get; set; }
        public string SourceLang { get; set; }
        public string TargetLang { get; set; }
        public string SourceText { get; set; }
        public string TargetText { get; set; }
        public string UpdatedText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public double TotalSeconds { get; set; }
        public double TotalMiliseconds { get; set; }
        public decimal WordsSource { get; set; }
        public decimal EditDistance { get; set; }
        public decimal EditDistanceRelative { get; set; }
        public decimal PemPercentage { get; set; }
        public string CommentsStr { get; set; }
        public List<QualityMetric> QualityMetrics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<KeyStroke> KeyStrokes { get; set; }

        public XmlDocumentActivity()
        {
            ProjectId = -1;
            ProjectName = string.Empty;
            ActivityId = -1;
            ActivityName = string.Empty;
            DocumentId = string.Empty;
            DocumentName = string.Empty;
            DocumentStartDate = null;
            DocumentStopDate = null;
            DocumentTotalSeconds = 0;
            RecordId = string.Empty;
            ParagraphId = string.Empty;
            SegmentId = string.Empty;
            OriginalConfirmationLevel = string.Empty;
            OriginalTranslationStatus = string.Empty;
            OriginalOriginSystem = string.Empty;
            OriginalOriginType = string.Empty;
            UpdatedConfirmationLevel = string.Empty;
            UpdatedTranslationStatus = string.Empty;
            UpdatedOriginSystem = string.Empty;
            UpdatedOriginType = string.Empty;
            SourceLang = string.Empty;
            TargetLang = string.Empty;
            SourceText = string.Empty;
            TargetText = string.Empty;
            UpdatedText = string.Empty;
            StartDate = null;
            StopDate = null;
            TotalSeconds = 0;
            TotalMiliseconds = 0;
            WordsSource = 0;
            EditDistance = 0;
            EditDistanceRelative = 0;
            PemPercentage = 0;
            CommentsStr = string.Empty;
            Comments = null;
            KeyStrokes = null;
            QualityMetrics = null;
        }
    }
}
