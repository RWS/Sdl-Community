using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class ReturnFilesViewModel :INotifyPropertyChanged
    {
        private ReturnPackage _returnPackage;
        private string _title;
        private List<ProjectFile> _projectFiles;
        public ReturnFilesViewModel(ReturnPackage returnPackage)
        {
            _returnPackage = returnPackage;
            _title = "Please select files for the return package";
            Title = _title;
            ProjectFiles = returnPackage.TargetFiles;
        }

        public string Title { get; set; }

        public List<ProjectFile> ProjectFiles
        {
            get { return _projectFiles; }
            set
            {
                if (Equals(value, _projectFiles))
                {
                    return;
                }
                _projectFiles = value;
                OnPropertyChanged();
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
