using System.IO;
using System.Windows;
using System.Xml;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
    public static class FileIdentifier
    {
        public static string GetFileInfo(string filepath)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
            {
                MessageBox.Show($"{nameof(GetFileInfo)}: Original project filepath missing. ");
                return null;
            }
            var filename = Path.GetFileName(filepath);
            var currentFileParentDirName = Directory.GetParent(filepath)?.Name;
            var fileInfo = $"{currentFileParentDirName}//{filename}";
            return fileInfo;
        }

        public static string GetProjectId(string projectFilePath)
        {
            var filepath = GetSdlProjFilePathFromProjectFile(projectFilePath);
            return ReadProjectId(filepath);
        }

        public static string GetSdlProjFilePathFromProjectFile(string projectFilePath)
        {
            if (string.IsNullOrEmpty(projectFilePath) || !File.Exists(projectFilePath))
            {
                MessageBox.Show($"{nameof(ReadProjectId)}: Original project filepath missing. ");
                return null;
            }

            var projectFolderPath = Directory.GetParent(Path.GetDirectoryName(projectFilePath));
            while (projectFolderPath is not null)
            {
                var sdlProjFiles = Directory.GetFiles(projectFolderPath.FullName, "*.sdlproj",
                    SearchOption.TopDirectoryOnly);

                if (sdlProjFiles.Length == 1) return sdlProjFiles[0];

                projectFolderPath = projectFolderPath.Parent;
            }

            return null;
        }

        private static string ReadProjectId(string projectFilePath)
        {
            if (string.IsNullOrWhiteSpace(projectFilePath)) return null;

            var doc = new XmlDocument();
            doc.Load(projectFilePath);

            var root = doc.DocumentElement;

            return root.Attributes["Guid"].Value;
        }
    }
}