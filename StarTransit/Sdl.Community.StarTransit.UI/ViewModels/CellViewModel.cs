using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.UI.Annotations;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class CellViewModel:INotifyPropertyChanged
    {
        private string _name;
        private bool _isChecked;
        private Guid _id;
        private ICommand _selectCommand;
        public event PropertyChangedEventHandler PropertyChanged;
        private  static List<Guid> _selectedProjectIds = new List<Guid>();

        public CellViewModel()
        {
            _name = string.Empty;
            _isChecked = false;
            _id = new Guid();
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (Equals(value, _name))
                {
                    return;
                }
                _name = value;
                OnPropertyChanged();
            }
        }

        public bool Checked
        {
            get { return _isChecked; }
            set
            {
                if (Equals(_isChecked, value))
                {
                    return;
                }
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        public Guid Id
        {
            get { return _id; }
            set
            {
                if (Equals(_id, value)) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectCommand
        {
            get { return _selectCommand ?? (_selectCommand = new CommandHandler(Select, true)); }
        }

        private void Select()
        {
            if (Checked)
            {
                if (!_selectedProjectIds.Contains(Id))
                {
                    _selectedProjectIds.Add(Id);
                }
                }
            else
            {
                _selectedProjectIds.Remove(Id);
            }

        }

        public static List<Guid> ReturnSelectedProjectIds()
        {
            return _selectedProjectIds;
        }

        public void ClearSelectedProjectsList()
        {
            _selectedProjectIds.Clear();
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
