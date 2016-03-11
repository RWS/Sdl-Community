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

            List<Folder> dropFolderList = GetIncomingRequestsFolder();

            foreach (var floder in dropFolderList)
            {
                foreach (string directory in Directory.GetDirectories(floder.Path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    ProjectRequests.Add(new ProjectRequest
                    {
                        Name = dirInfo.Name,
                        Files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                    });
                }
            }
            
           
        }

        public static List<Folder>  GetIncomingRequestsFolder()
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
            var folderList = GetIncomingRequestsFolder();
            foreach (var folder in folderList)
            {
                Directory.Move(Path.Combine(folder.Path, request.Name), Path.Combine(GetAcceptedRequestsFolder(), request.Name));
            }
            //  Directory.Move(Path.Combine(GetIncomingRequestsFolder(), request.Name), Path.Combine(GetAcceptedRequestsFolder(), request.Name));
        }
    }
}
