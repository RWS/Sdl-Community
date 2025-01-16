using Newtonsoft.Json.Linq;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace Sdl.Community.PostEdit.Compare.Core.ActionsFromReport
{
    public static class Listener
    {
        private static HttpListener _listener;

        public static void StartHttpListener()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:5000/");
            _listener.Start();
            _listener.BeginGetContext(OnRequestReceived, null);
        }

        private static void NavigateToSegment(string segmentId)
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            editorController.ActiveDocument.SetActiveSegmentPair(editorController.ActiveDocument.Files.First(), segmentId, true);
        }

        private static void OnRequestReceived(IAsyncResult result)
        {
            try
            {
                // Get the context from the listener
                var context = _listener.EndGetContext(result);

                // Begin accepting the next request early to avoid blocking
                _listener.BeginGetContext(OnRequestReceived, null);

                var request = context.Request;
                var response = context.Response;

                // Set common response headers for CORS
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");

                // Handle OPTIONS preflight request
                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 204; // No Content
                    response.Close();
                    return;
                }

                // Handle POST request
                if (request.Url.AbsolutePath == "/navigate" && request.HttpMethod == "POST")
                {
                    using var reader = new StreamReader(request.InputStream);
                    var requestBody = reader.ReadToEnd();

                    // Parse the request body
                    var data = JObject.Parse(requestBody);
                    var segmentId = data["segmentId"]?.ToString();

                    if (!string.IsNullOrEmpty(segmentId))
                    {
                        NavigateToSegment(segmentId);
                    }

                    // Respond with success
                    response.StatusCode = 200;
                    using var writer = new StreamWriter(response.OutputStream);
                    writer.Write("OK");
                }
                else
                {
                    // Handle invalid paths or methods
                    response.StatusCode = 404; // Not Found
                    using var writer = new StreamWriter(response.OutputStream);
                    writer.Write("Not Found");
                }

                // Close the response
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling request: {ex.Message}");
            }
        }

    }
}