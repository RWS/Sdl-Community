using System;
using System.Windows.Forms;

using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.Verification.Api;

namespace Sdl.Community.Extended.MessageUI
{
    /// <summary>
    /// QACheckerMessageControlPlugIn class represents a QA checker message control plug-in
    /// responsible for creating QA checker message controls
    /// </summary>
    [MessageControlPlugIn]
    public class NumberVerifierMessagePlugIn : IMessageControlPlugIn
    {
        #region SupportsMessage
        /// <summary>
        /// Determines whether the message control plug-in supports the given message.
        /// </summary>
        /// <param name="messageEventArgs">message</param>
        /// <returns>whether supports message</returns>
        public bool SupportsMessage(MessageEventArgs messageEventArgs)
        {
            return messageEventArgs.ExtendedData != null &&
                   messageEventArgs.ExtendedData.GetType().Equals(typeof(NumberVerifierMessageData));
        }
        #endregion

        #region CreateMessageControl
        /// <summary>
        /// Creates the message control for the given message.
        /// </summary>
        /// <param name="messageControlContainer">message control container</param>
        /// <param name="messageEventArgs">message</param>
        /// <param name="bilingualDocument">bilingual document</param>
        /// <param name="sourceSegment">source segment</param>
        /// <param name="targetSegment">target segment</param>
        /// <returns>message control</returns>
        public UserControl CreateMessageControl(IMessageControlContainer messageControlContainer, MessageEventArgs messageEventArgs, 
            IBilingualDocument bilingualDocument, ISegment sourceSegment, ISegment targetSegment)
        {
            if (!SupportsMessage(messageEventArgs))
            {
                throw new ArgumentException("messageEventArgs is not supported by this message control plug-in", "messageEventArgs");
            }

            return new NumberVerifierMessageUI(messageEventArgs, bilingualDocument, sourceSegment, targetSegment);
        }
        #endregion
    }
}