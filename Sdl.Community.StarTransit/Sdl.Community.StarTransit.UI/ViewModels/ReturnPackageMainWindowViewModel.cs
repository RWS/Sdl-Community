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
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class ReturnPackageMainWindowViewModel:INotifyPropertyChanged
    {
        private ICommand _createPackageCommand;
        private readonly ReturnFilesViewModel _returnFilesViewModel;
        private List<ReturnPackage> _returnPackageList;
        private readonly ReturnPackageService _returnService;

        public ReturnPackageMainWindowViewModel(ReturnFilesViewModel returnFilesViewModel)
        {
            _returnFilesViewModel = returnFilesViewModel;
            _returnService = new ReturnPackageService();
        }
       

        public ICommand CreatePackageCommand
        {
            get { return _createPackageCommand ?? (_createPackageCommand = new CommandHandler(CreatePackage, true)); }
        }

        private void CreatePackage()
        {
            _returnPackageList = _returnFilesViewModel.GetReturnPackage();
            foreach (var project in _returnPackageList)
            {
                var path = project.ProjectLocation.Substring(0,
                    project.ProjectLocation.LastIndexOf(@"\", StringComparison.Ordinal));
                var returnPackageFolderPath = CreateReturnPackageFolder(path);
                project.FolderLocation = returnPackageFolderPath;

                _returnService.ExportFiles(project);
            }
        }
            
        /// <summary>
        /// Create return  package folder in the studio project folder
        /// </summary>
        /// <param name="projectPath"></param>
        private string CreateReturnPackageFolder(string projectPath)
        {
            var returnPackageFolderPath = Path.Combine(projectPath, "Return package");

            if (!Directory.Exists(returnPackageFolderPath))
            {
                Directory.CreateDirectory(returnPackageFolderPath);

            }
            return returnPackageFolderPath;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
