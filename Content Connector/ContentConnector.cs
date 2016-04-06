using System;
using System.Collections.Generic;
using System.IO;

namespace Sdl.Community.ContentConnector
{
    class ContentConnector
    {
        public static readonly ContentConnector Instance = new ContentConnector();
        public static Persistence Persistence = new Persistence();
        private string _requestPath = string.Empty;
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
            _requestPath = Path.Combine(request.Path, request.Name);
          
            var acceptedRequestFolder = GetAcceptedRequestsFolder(request.Path);

            var directoryToMovePath = Path.Combine(acceptedRequestFolder, request.Name);
            //create the directory "Accepted request"
            if (!Directory.Exists(directoryToMovePath))
            {
                Directory.CreateDirectory(directoryToMovePath);
            }

            
            var files = Directory.GetFiles(_requestPath);
            if (files.Length != 0)
            {
                MoveFilesToAcceptedFolder(files, directoryToMovePath);
                Directory.Delete(_requestPath);
            }//that means we habe a subfolder in watch folder
            else
            {
                
                var subdirectories = Directory.GetDirectories(_requestPath);
                foreach (var subdirectory in subdirectories)
                {
                    var currentDirInfo = new DirectoryInfo(subdirectory);
                    // HasSubfolders(currentDirInfo.FullName);
                    CheckForSubfolders(currentDirInfo, acceptedRequestFolder);
                
                }

                var currentDirectory = Directory.GetDirectories(_requestPath);
                try
                {
                    if (currentDirectory.Length == 0)
                    {
                        Directory.Delete(_requestPath);
                    }
                }catch(Exception exception) { }
               
                
            }
            
            Persistence.Update(request);
        }

  
        private void CheckForSubfolders(DirectoryInfo directory,string root)
        {
            var subdirectories = directory.GetDirectories();
            var path = root + @"\" + directory.Parent;
            var subdirectoryFiles = Directory.GetFiles(directory.FullName);
            if (subdirectoryFiles.Length != 0)
            {
                var subdirectoryToMovePath = Path.Combine(path, directory.Name);
                if (!Directory.Exists(subdirectoryToMovePath))
                {
                    Directory.CreateDirectory(subdirectoryToMovePath);
                }

                MoveFilesToAcceptedFolder(subdirectoryFiles, subdirectoryToMovePath);
                try
                {
                    Directory.Delete(directory.FullName);
                    var pathToDelete = Path.Combine(_requestPath, directory.Parent.Name);
                    var directoryToDelete = Directory.GetDirectories(pathToDelete);
                    if (directoryToDelete.Length == 0)
                    {
                        Directory.Delete(pathToDelete);
                    }
                   
                }catch(Exception e) { }
                
            }
            if (subdirectories.Length != 0)
            {
                foreach (var subdirectory in subdirectories)
                {
                    CheckForSubfolders(subdirectory, path);
                }
            }
            
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
