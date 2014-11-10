using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Extended.MessageUI
{
    public class NumberVerifierMessageData : ExtendedMessageEventData
    {
        public NumberVerifierMessageData(string sourceIssues, string targetIssues, ISegment replacementSuggestion)
        {
            this.SourceIssues = sourceIssues;
            this.TargetIssues = targetIssues;
            this.ReplacementSuggestion = replacementSuggestion;
            
            //Identifier for this custom message type
            this.MessageType = "Sdl.Verification.Sdk.NumberVerifier.MessageUI, Number_Issue";
        }

        /// <summary>
        /// Information which will be displayed in our custom UI.
        /// </summary>
        public string SourceIssues
        {
            get;
            private set;
        }

        public string TargetIssues
        {
            get;
            private set;
        }

        /// <summary>
        /// Suggestion which will be used in the custom UI for target segment replacement.
        /// </summary>
        public ISegment ReplacementSuggestion
        {
            get; 
            private set;
        }
    }
}