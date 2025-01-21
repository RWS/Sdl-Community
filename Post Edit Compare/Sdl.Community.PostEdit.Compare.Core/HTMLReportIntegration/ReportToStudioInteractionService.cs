using Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration.Components;
using System;
using System.Net;

namespace Sdl.Community.PostEdit.Compare.Core.HTMLReportIntegration
{
    public class ReportToStudioInteractionService
    {
        public const string FileId = "fileId";
        public const string ProjectId = "projectId";
        public const string SegmentId = "segmentId";
        public const string Status = "status";

        private DataHandler DataHandler { get; } = new();
        private StudioCommunicator StudioCommunicator { get; } = new();

        public void NavigateToSegment(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var data = DataHandler.GetData(request.InputStream);
                var segmentId = data[SegmentId]?.ToString();
                var projectId = data[ProjectId]?.ToString();
                var fileId = data[FileId]?.ToString();

                if (!string.IsNullOrEmpty(segmentId)) StudioCommunicator.NavigateToSegment(segmentId, fileId, projectId);

                DataHandler.WriteResponse(response, 200, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleNavigateRequest: {ex.Message}");
                DataHandler.WriteResponse(response, 500, "Internal Server Error");
            }
        }

        public void NotFound(HttpListenerResponse response)
        {
            DataHandler.WriteResponse(response, 404, "Not Found");
        }

        public void Options(HttpListenerResponse response)
        {
            response.StatusCode = 204;
        }

        public void UpdateStatus(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var data = DataHandler.GetData(request.InputStream);
                var status = data[Status]?.ToString();
                var segmentId = data[SegmentId]?.ToString();
                var fileId = data[FileId]?.ToString();
                var projectId = data[ProjectId]?.ToString();

                if (!string.IsNullOrEmpty(status))
                    StudioCommunicator.ChangeStatusOfSegment(status,
                        segmentId,
                        fileId,
                        projectId);

                DataHandler.WriteResponse(response, 200, "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleNavigateRequest: {ex.Message}");
                DataHandler.WriteResponse(response, 500, "Internal Server Error");
            }
        }
    }
}