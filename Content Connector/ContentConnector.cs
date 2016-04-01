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
                    if (dirInfo.Name != "AcceptedRequests")
                    {
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
            
           
        }

        public static List<ProjectRequest>  GetIncomingRequestsFolder()
        {
            var folderPath = Persistence.Load();
            return folderPath;
        }


        private string GetAcceptedRequestsFolder(string path)
        {
            var acceptedPath = Path.Combine(path, "AcceptedRequests");
            if (!Directory.Exists(acceptedPath))
            {
                Directory.CreateDirectory(path);
            }
            return acceptedPath;
        }

        internal void RequestAccepted(ProjectRequest request)
        {
            var requestPath = Path.Combine(request.Path, request.Name);
          
            var acceptedRequestFolder = GetAcceptedRequestsFolder(request.Path);

            var directoryToMovePath = Path.Combine(acceptedRequestFolder, request.Name);
            //creez directorul "Accepted request"
            if (!Directory.Exists(directoryToMovePath))
            {
                Directory.CreateDirectory(directoryToMovePath);
            }

            
            var files = Directory.GetFiles(requestPath);
            if (files.Length != 0)
            {
                MoveFilesToAcceptedFolder(files, directoryToMovePath);
                Directory.Delete(requestPath);
            }//inseamna ca avem subfolder si nu fisiere in director
            else
            {
                
                var subdirectories = Directory.GetDirectories(requestPath);
                foreach (var subdirectory in subdirectories)
                {
                    var currentDirInfo = new DirectoryInfo(subdirectory);
                    var subdirectoryFiles = Directory.GetFiles(currentDirInfo.FullName);
                    MoveFilesToAcceptedFolder(subdirectoryFiles, directoryToMovePath);
                    Directory.Delete(requestPath);
                }
            }
            
            Persistence.Update(request);
        }

        private void MoveFilesToAcceptedFolder(string[] files,string acceptedFolderPath)
        {
            foreach (var subFile in files)
            {
                var fileName = subFile.Substring(subFile.LastIndexOf(@"\", StringComparison.Ordinal));
                try
                {
                    File.Copy(subFile, acceptedFolderPath + fileName, true);
                    File.Delete(subFile);
                }
                catch (Exception e)
                {

                }
            }
        }

    }
}
