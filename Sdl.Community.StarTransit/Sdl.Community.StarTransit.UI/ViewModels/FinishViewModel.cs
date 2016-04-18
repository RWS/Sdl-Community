using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class FinishViewModel: INotifyPropertyChanged
    {
        private ICommand _finishCommand;
        private bool _canExecute;
        private readonly ProjectService _projectService;

        public FinishViewModel(PackageModel package)
        {
            _projectService = new ProjectService();
             _canExecute = true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand FinishCommand
        {
            get { return _finishCommand ?? (_finishCommand = new CommandHandler(Finish, _canExecute)); }

        }

        public void Finish()
        {
            var package = PackageDetailsViewModel.GetPackageModel();
            _projectService.CreateProject(package);
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
