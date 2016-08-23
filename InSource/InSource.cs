using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.InSource.Insights;

namespace Sdl.Community.InSource
{
    class InSource
    {
        public static readonly InSource Instance = new InSource();
        public static Persistence Persistence = new Persistence();
        private string _requestPath = string.Empty;
        private InSource()
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

            var delete = Persistence.LoadRequest().DeleteFolders;

            if (!delete)
            {
                var acceptedRequestFolder = GetAcceptedRequestsFolder(request.Path);

                var directoryToMovePath = Path.Combine(acceptedRequestFolder, request.Name);
                //create the directory "Accepted request"
                if (!Directory.Exists(directoryToMovePath))
                {
                    Directory.CreateDirectory(directoryToMovePath);
                }

                try
                {
                    var files = Directory.GetFiles(_requestPath);
                    if (files.Length != 0)
                    {
                        MoveFilesToAcceptedFolder(files, directoryToMovePath);
                        Directory.Delete(_requestPath);
                    } //that means we have a subfolder in watch folder
                    else
                    {

                        var subdirectories = Directory.GetDirectories(_requestPath);
                        foreach (var subdirectory in subdirectories)
                        {
                            var currentDirInfo = new DirectoryInfo(subdirectory);
                            CheckForSubfolders(currentDirInfo, acceptedRequestFolder);

                        }

                        var currentDirectory = Directory.GetDirectories(_requestPath);

                        if (currentDirectory.Length == 0)
                        {
                            Directory.Delete(_requestPath);
                        }


                    }
                }
                catch (Exception exception)
                {
                    TelemetryService.Instance.AddException(exception);
                }
            }
            else
            {
                try
                {
                    Directory.Delete(_requestPath,true);
                }
                catch (Exception ex)
                {

                    TelemetryService.Instance.AddException(ex);
                }
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

                }
                catch (Exception e)
                {
                    TelemetryService.Instance.AddException(e);
                }
                
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
                    TelemetryService.Instance.AddException(e);
                }
            }
        }

    }
}
