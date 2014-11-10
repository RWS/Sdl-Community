using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections.Generic;
using System;

namespace Sdl.Verification.Sdk.EditAndApplyChanges.MessageUI
{
    /// <summary>
    /// CustomMessageData class represents message extended data that
    /// includes a detailed description.
    /// </summary>
    public class CustomMessageData : ExtendedMessageEventData
    {
        /// <summary>
        /// Constructor that takes the given message type and detailed description.
        /// </summary>
        /// <param name="messageType">message type</param>
        /// <param name="detailedDescription">detailed description</param>
        public CustomMessageData(string messageType, string detailedDescription)
        {
            MessageType = messageType;
            DetailedDescription = detailedDescription;
            SuggestedChanges = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// Constructor that takes the given message type, detailed description and suggested change.
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="detailedDescription"></param>
        /// <param name="qaSuggestion">KeyValuePair(stringformarkup,stringforlabel)</param>
        public CustomMessageData(string messageType, string detailedDescription, KeyValuePair<string, string> qaSuggestion)
        {
            MessageType = messageType;
            DetailedDescription = detailedDescription;
            SuggestedChanges = new List<KeyValuePair<string, string>>();
            SuggestedChanges.Add(qaSuggestion);
        }

        /// <summary>
        /// DetailedDescription property represents the detailed description.
        /// </summary>
        public string DetailedDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// List of suggestions which will be offered to user
        /// </summary>
        public List<KeyValuePair<string, string>> SuggestedChanges
        {
            get;
            set;
        }
    }
}
