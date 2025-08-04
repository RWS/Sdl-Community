using Sdl.Community.NumberVerifier.Validator;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.Verification.Api;
using System;

namespace Sdl.Community.NumberVerifier.MessageUI
{
    public class AlignmentErrorExtendedData : ExtendedMessageEventData, IVerificationCustomMessageData
    {
        public AlignmentErrorExtendedData(string description)
        {
            DetailedDescription = description;
        }

        public TextRange SourceRange { get; set; } = new();
        public TextRange TargetRange { get; set; } = new();
        public string SourceIssues { get; set; }
        public string TargetIssues { get; set; }

        public string DetailedDescription { get; }

        public string SourceSegmentPlainText { get; set; }
        public string TargetSegmentPlainText { get; set; }
    }
}