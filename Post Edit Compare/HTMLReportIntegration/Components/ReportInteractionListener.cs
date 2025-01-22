using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.Components
{
    public static class ReportInteractionListener
    {
        private static ReportToStudioInteractionService Controller { get; } = new();

        public static void HandleRequest(CoreWebView2WebMessageReceivedEventArgs message)
        {
            var jsonMessage = JObject.Parse(message.WebMessageAsJson);

            var action = jsonMessage["action"]?.ToString();

            switch (action)
            {
                case "navigate":
                    Controller.NavigateToSegment(jsonMessage);
                    break;

                case "updateStatus":
                    Controller.UpdateStatus(jsonMessage);
                    break;
            }
        }
    }
}