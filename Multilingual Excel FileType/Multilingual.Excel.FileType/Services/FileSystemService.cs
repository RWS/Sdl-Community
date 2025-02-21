using System;
using System.IO;

namespace Multilingual.Excel.FileType.Services
{
    public class FileSystemService
    {
        public Stream OpenRead(string filePath)
        {
            return File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream OpenWrite(string filePath)
        {
            return File.Open(filePath, FileMode.Create);
        }

        public DateTime RetrieveLastWriteTimeUtc(string filePath)
        {
            return CreateFileInfo(filePath).LastWriteTimeUtc;
        }

        public bool FileExists(string filePath)
        {
            try
            {
                return CreateFileInfo(filePath).Exists;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public string GetParentFolderPath(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public bool IsAbsolutePath(string filePath)
        {
            try
            {
                var pathRoot = Path.GetPathRoot(filePath);
                return Path.IsPathRooted(filePath)
                       && pathRoot != null &&
                       pathRoot.Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // not needed
            }
        }

        public void Move(string sourceFilePath, string destFilePath)
        {
            try
            {
                File.Move(sourceFilePath, destFilePath);
            }
            catch
            {
                // empty
            }
        }

        private static FileInfo CreateFileInfo(string filePath)
        {
            return new FileInfo(filePath);
        }
    }
}
