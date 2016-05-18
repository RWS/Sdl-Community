using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class ReturnPackageMainWindowViewModel:INotifyPropertyChanged
    {
        private ICommand _createPackageCommand;
        private ReturnFilesViewModel _returnFilesViewModel;
        private ReturnPackage _returnPackage;

        public ReturnPackageMainWindowViewModel(ReturnFilesViewModel returnFilesViewModel)
        {
            _returnFilesViewModel = returnFilesViewModel;
        }
       

        public ICommand CreatePackageCommand
        {
            get { return _createPackageCommand ?? (_createPackageCommand = new CommandHandler(CreatePackage, true)); }
        }

        private void CreatePackage()
        {
            _returnPackage = _returnFilesViewModel.GetReturnPackage();
            foreach (var projectPath in _returnPackage.ProjectLocation)
            {
                var path = projectPath.Substring(0,projectPath.LastIndexOf(@"\", StringComparison.Ordinal));
                CreateReturnPackageFolderAndArchive(path);
            }
        }
            
        /// <summary>
        /// Create return  package folder in the studio project folder
        /// </summary>
        /// <param name="projectPath"></param>
        private void CreateReturnPackageFolderAndArchive(string projectPath)
        {
            var returnPackagePath = Path.Combine(projectPath, "Return package");
            _returnPackage.Location = new List<string>();
            if (!Directory.Exists(returnPackagePath))
            {
                Directory.CreateDirectory(returnPackagePath);
                _returnPackage.Location.Add(returnPackagePath);
               
            }
            CreateArchive(returnPackagePath, _returnPackage.TargetFiles);
        }

        /// <summary>
        /// Creates an archive in the Return Package folder and add project files to it
        /// For the moment we add the files without runing any task on them
        /// </summary>
        /// <param name="packagePath"></param>
        /// <param name="projectFiles"></param>
        private void CreateArchive(string packagePath, List<ProjectFile> projectFiles)
        {
            var archivePath = Path.Combine(packagePath, "returnPackage.tpf");
            using (var archive = ZipFile.Open(archivePath, ZipArchiveMode.Create))
            {
                foreach (var file in projectFiles)
                {
                    archive.CreateEntryFromFile(file.LocalFilePath, file.Name, CompressionLevel.Optimal);
                }
                
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
