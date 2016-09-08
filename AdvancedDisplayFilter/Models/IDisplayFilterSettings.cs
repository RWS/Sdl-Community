using System;
using System.Collections.Generic;

namespace Sdl.Community.AdvancedDisplayFilter.Models
{
    public interface IDisplayFilterSettings
    {
        string Version { get; set; }
        DateTime Created { get; set; }
        bool IsRegularExpression { get; set; }
        bool IsCaseSensitive { get; set; }
        string SourceText { get; set; }
        string TargetText { get; set; }

        string CommentText { get; set; }
        string CommentAuthor { get; set; }
        int CommentSeverity { get; set; }

        List<string> ContextInfoTypes { get; set; }   

        bool ShowAllContent { get; set; }      

        List<string> RepetitionTypes { get; set; }

        List<string> SegmentReviewTypes { get; set; }

        List<string> SegmentLockingTypes { get; set; }

        List<string> SegmentContentTypes { get; set; } 

        List<string> ConfirmationLevels { get; set; }

        List<string> OriginTypes { get; set; }

        List<string> PreviousOriginTypes { get; set; }


    }
}
