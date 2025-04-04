using Newtonsoft.Json.Linq;
using System;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Messaging
{
    public class SyncMessage : ISyncMessage
    {
        private const string AddCommentAction = "addComment";

        private const string NavigateAction = "navigate";

        private const string UpdateStatusAction = "updateStatus";

        public static ISyncMessage Create(JObject message)
        {
            var action = message["action"].ToString();
            return action switch
            {
                NavigateAction => new NavigateMessage(message),
                UpdateStatusAction => new UpdateStatusMessage(message),
                AddCommentAction => new UpdateCommentsMessage(message),
                _ => throw new ArgumentException($"Unknown action: {action}")
            };
        }

        public ISyncMessage Message { get; set; }
    }
}