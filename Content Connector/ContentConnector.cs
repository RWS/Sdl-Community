using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.ContentConnector
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

            List<ProjectRequest> dropFolderList = GetIncomingRequestsFolder();

            foreach (var folder in dropFolderList)
            {
                foreach (string directory in Directory.GetDirectories(folder.Path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    ProjectRequests.Add(new ProjectRequest
                    {
                        Name = dirInfo.Name,
                        Files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories),
                        Path = folder.Path,
                        ProjectTemplate = folder.ProjectTemplate
                    });
                }
            }
            
           
        }

        public static List<ProjectRequest>  GetIncomingRequestsFolder()
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
            try
            {
                Directory.Move(Path.Combine(request.Path, request.Name),
                    Path.Combine(GetAcceptedRequestsFolder(), request.Name));
               Persistence.Update(request);
            }
            catch (Exception e)
            {

            }
        }
    }
}
