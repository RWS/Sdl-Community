using Sdl.Community.BetaAPIs.UI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.BetaAPIs.UI.Commands
{
    public class CommandsHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public bool SetCanExecute
        {
            get { return _canExecute; }
            set
            {
                _canExecute = value;
            }
        }

        public event EventHandler CanExecuteChanged;

        public CommandsHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public void RaiseCanExecuteChanged()
        {
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }
    }
}
