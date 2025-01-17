using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;

namespace Sdl.Community.PostEdit.Compare.Core.ActionsFromReport
{
    public class Controller
    {
        private StudioHelper Studio { get; } = new();

        public static void WriteResponse(HttpListenerResponse response, int statusCode, string message)
        {
            response.StatusCode = statusCode;
            using var writer = new StreamWriter(response.OutputStream);
            writer.Write(message);
        }

        public void HandleNavigateRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                using var reader = new StreamReader(request.InputStream);
                var requestBody = reader.ReadToEnd();
                var data = JObject.Parse(requestBody);
                var segmentId = data["segmentId"]?.ToString();
                var projectId = data["projectId"]?.ToString();
                var fileId = data["fileId"]?.ToString();

                if (!string.IsNullOrEmpty(segmentId)) Studio.NavigateToSegment(segmentId, fileId, projectId);

                WriteResponse(response, 200, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleNavigateRequest: {ex.Message}");
                WriteResponse(response, 500, "Internal Server Error");
            }
        }

        public void HandleNotFound(HttpListenerResponse response) => WriteResponse(response, 404, "Not Found");

        public void HandleOptionsRequest(HttpListenerResponse response) => response.StatusCode = 204;
    }
}