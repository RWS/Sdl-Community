using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Versions.ReportViewer.Model;
using System.Collections.Generic;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components
{
    public class ReportInteractionHandler
    {
        private StudioController StudioController { get; } = new();
        private ReportController ReportEditor { get; } = new();

        public void HandleHtmlInteraction(CoreWebView2WebMessageReceivedEventArgs message)
        {
            var jsonMessage = JObject.Parse(message.WebMessageAsJson);

            var action = jsonMessage["action"]?.ToString();

            switch (action)
            {
                case "navigate":
                    StudioController.NavigateToSegment(jsonMessage);
                    break;

                case "updateStatus":
                    StudioController.UpdateStatus(jsonMessage);
                    break;
            }
        }

        public void HandleEditorInteraction(string segmentId, List<CommentInfo> comments)
        {
            ReportController.ReplaceComments(segmentId, comments);
        }
    }
}