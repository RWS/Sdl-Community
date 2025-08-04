using Sdl.Community.NumberVerifier.Helpers;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.Verification.Api;
using System;

namespace Sdl.Community.Extended.MessageUI
{
	public class NumberVerifierMessageData : ExtendedMessageEventData, IVerificationCustomMessageData
    {
		public NumberVerifierMessageData(){}
		public NumberVerifierMessageData(
			MessageDataModel messageDataModel)
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

			DetailedDescription = messageDataModel.ErrorMessage;
        }

        /// <summary>
        /// Information which will be displayed in our custom UI.
        /// </summary>
        public string SourceIssues
        {
            get;
            set;
        }

        public string TargetIssues
        {
            get;
            set;
        }

		public string InitialSourceIssues
		{
			get;
			set;
		}

		public string InitialTargetIssues
		{
			get;
			set;
		}

		/// <summary>
		/// Suggestion which will be used in the custom UI for target segment replacement.
		/// </summary>
		public ISegment ReplacementSuggestion
        {
            get; 
            set;
        }

		public string DetailedDescription { get; }

		public string SourceSegmentPlainText { get; set; } = "Hello World";
		public string TargetSegmentPlainText { get; set; } = "Hallo Welt!";
    }
}