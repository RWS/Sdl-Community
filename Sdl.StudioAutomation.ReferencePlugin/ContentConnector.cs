using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StudioIntegrationApiSample
{
    class ContentConnector
    {
        public static readonly ContentConnector Instance = new ContentConnector();
        public static Persistence Persistence = new Persistence();
        private ContentConnector()
        {
        }

        public  static List<ProjectRequest> ProjectRequests
        {
            get; private set;
        }

        public static void Refresh()
        {
            ProjectRequests = new List<ProjectRequest>();

            string dropFolder = GetIncomingRequestsFolder();

            foreach (string directory in Directory.GetDirectories(dropFolder))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                ProjectRequests.Add(new ProjectRequest
                {
                    Name = dirInfo.Name,
                    Files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                });
            }
           
        }

        public static string GetIncomingRequestsFolder()
        {
            var folderPath = Persistence.Load();
            return folderPath;
        }


        private static string GetAcceptedRequestsFolder()
        {
             var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Studio 2015\\AcceptedRequests");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        internal void RequestAccepted(ProjectRequest request)
        {
            Directory.CreateDirectory(GetAcceptedRequestsFolder());
            Directory.Move(Path.Combine(GetIncomingRequestsFolder(), request.Name), Path.Combine(GetAcceptedRequestsFolder(), request.Name));
        }
    }
}
