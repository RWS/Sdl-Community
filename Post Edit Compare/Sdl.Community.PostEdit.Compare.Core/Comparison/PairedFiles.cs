using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sdl.Community.PostEdit.Compare.Core.Comparison
{
    public class PairedFiles
    {
        public List<PairedFile> PairedProcessingFiles { get; set; }

        public PairedFiles(string originalFile, string updatedFile)
        {
            PairedProcessingFiles = new List<PairedFile>();
            var pairedFile = new PairedFile
            {
                OriginalFilePath = new FileInfo(originalFile),
                UpdatedFilePath = new FileInfo(updatedFile),
                IsError = false,
                Message = string.Empty
            };

            PairedProcessingFiles.Add(pairedFile);
        }

        public PairedFiles(string originalDirectory, string updatedDirectory, string[] filters, bool processSubFolders)
        {
            PairedProcessingFiles = new List<PairedFile>();


            

            var filePathsOriginal = GetProcessingFiles(originalDirectory, filters,  processSubFolders);
            var filePathsUpdated = GetProcessingFiles(updatedDirectory, filters, processSubFolders);


            foreach (var t1 in filePathsOriginal)
            {
                var pairedFile = new PairedFile
                {
                    OriginalFilePath = new FileInfo(t1),
                    UpdatedFilePath = null,
                    IsError = false,
                    Message = string.Empty
                };



                var originalPathCompare = t1.Replace(originalDirectory, string.Empty);


                var found = false;
                foreach (var t in filePathsUpdated)
                {
                    var updatedPathCompare = t.Replace(updatedDirectory, string.Empty);

                    if (string.Compare(originalPathCompare, updatedPathCompare, StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    pairedFile.UpdatedFilePath = new FileInfo(t);
                    found = true;
                    break;
                }

                if (!found)
                {
                    pairedFile.IsError = true;
                    pairedFile.Message = "Unable to locate the associated file in th updated folder " + originalPathCompare + "";
                }

                PairedProcessingFiles.Add(pairedFile);
            }

        }


        private List<string> ProcessingFiles { get; set; }
        private List<string> GetProcessingFiles(string directory, string[] filters, bool processSubFolders)
        {
            ProcessingFiles = new List<string>();
            if (processSubFolders)
                ProcessDirectories(directory, filters);
            else
                ProcessDirectory(directory, filters);

            return ProcessingFiles;
        }

        private void ProcessDirectory(string targetDirectory, IEnumerable<string> filters)
        {
            foreach (var filter in filters)
            {
                if (filter.Trim() == string.Empty) continue;
                var dirAndFiles = Directory.GetFiles(targetDirectory, filter);
                foreach (var filePath in dirAndFiles)
                {
                    if (!ProcessingFiles.Contains(filePath))
                        ProcessingFiles.Add(filePath);
                }
            }
        }
        private void ProcessDirectories(string targetDirectory, IEnumerable<string> filters)
        {
            var enumerable = filters as string[] ?? filters.ToArray();
            ProcessDirectory(targetDirectory, enumerable);

            var subdirectoryEntries = Directory.GetDirectories(targetDirectory);

            foreach (var subdirectory in subdirectoryEntries)
            {
                ProcessDirectories(subdirectory, enumerable);
            }
        }


        public class PairedFile
        {
            public FileInfo OriginalFilePath { get; set; }
            public FileInfo UpdatedFilePath { get; set; }


            public bool IsError { get; set; }
            public string Message { get; set; }

            public PairedFile()
            {
                OriginalFilePath = null;
                UpdatedFilePath = null;
                IsError = false;
                Message = string.Empty;
            }

        }
    }
}
