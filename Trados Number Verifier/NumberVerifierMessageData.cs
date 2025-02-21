using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Verification.Sdk.IdenticalCheck.Extended.MessageUI
{
    public class IdenticalVerifierMessageData : ExtendedMessageEventData
    {
        public IdenticalVerifierMessageData(string errorDetails, ISegment replacementSuggestion)
        {
            this.ErrorDetails = errorDetails;
            this.ReplacementSuggestion = replacementSuggestion;
            
            //Identifier for this custom message type
            this.MessageType = "Sdl.Verification.Sdk.IdenticalCheck.MessageUI, Error_NotIdentical";
        }

        /// <summary>
        /// Information which will be displayed in our custom UI.
        /// </summary>
        public string ErrorDetails
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