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
        private bool _active;
        private bool _completed;

        public FinishViewModel(PackageModel package)
        {
            _projectService = new ProjectService();
             _canExecute = true;
            _completed = false;
            // _isCreated = true;
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                if (Equals(value, _active))
                {
                    return;
                }
                _active = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand FinishCommand
        {
            get { return _finishCommand ?? (_finishCommand = new CommandHandler(Finish, _canExecute)); }
           
        }

        public async void Finish()
        {
            Active = true;
            var package = PackageDetailsViewModel.GetPackageModel();
       

            await Task.Run(()=>_projectService.CreateProject(package));
          
            
             Active = false;

        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
