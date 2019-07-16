using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Extended.MessageUI
{
	public class NumberVerifierMessageData : ExtendedMessageEventData
    {
		public NumberVerifierMessageData(MessageDataModel messageDataModel)
		{
			SourceIssues = messageDataModel.SourceIssues;
			InitialSourceIssues = messageDataModel.InitialSourceIssues;
			InitialTargetIssues = messageDataModel.InitialTargetIssues;
			TargetIssues = messageDataModel.TargetIssues;
			ReplacementSuggestion = messageDataModel.ReplacementSuggestion;

			//Identifier for this custom message type
			if (messageDataModel.ErrorMessage.Equals(Constants.AlphanumericMessage))
			{
				MessageType = Constants.AlphanumericIssue;
			}
			else if (messageDataModel.IsHindiVerification)
			{
				MessageType = Constants.HindiIssue;
			}
			else
			{
				MessageType = Constants.NumberIssue;
			}
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

		public string InitialSourceIssues
		{
			get;
			private set;
		}

		public string InitialTargetIssues
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