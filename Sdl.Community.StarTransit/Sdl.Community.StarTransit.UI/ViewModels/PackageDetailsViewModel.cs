using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.StarTransit.UI.Helpers;
using Sdl.ProjectAutomation.Core;



namespace Sdl.Community.StarTransit.UI.ViewModels
{
    public class PackageDetailsViewModel : IDataErrorInfo, INotifyPropertyChanged
    {
        private string _textLocation;
        private  string _txtName;
        private  string _txtDescription;
        private  List<ProjectTemplateInfo> _studioTemplates;
        private  bool _hasDueDate;
        private  DateTime? _dueDate;
        private  string _sourceLanguage;
        private  string _targetLanguage;
        private  PackageModel _packageModel;
        private ICommand _browseCommand;
        private  bool _canExecute;
        private  ProjectTemplateInfo _template;
        private IProject _project;
        private  Customer _selectedCustomer; 
        public ObservableCollection<ProjectTemplateInfo> _templates;
        private  int _selectedHour;
        private  int _selectedMinute;
        private  string _selectedMoment;

        public List<int> HourList { get; set; }
        public List<int> MinutesList { get; set; }
        public List<string> MomentsList { get; set; }

        public PackageDetailsViewModel(PackageModel package)
        {
            _packageModel = package;
            _txtName = package.Name;
            _txtDescription = package.Description;
            _studioTemplates = package.StudioTemplates;
            _textLocation = package.Location;
            _sourceLanguage = package.SourceLanguage.DisplayName;
            _templates = new ObservableCollection<ProjectTemplateInfo>(package.StudioTemplates);
            _hasDueDate = false;
            var targetLanguage = string.Empty;
            foreach (var language in package.TargetLanguage)
            {
                targetLanguage = targetLanguage + language.DisplayName;
            }
            _targetLanguage = targetLanguage;

            _canExecute = true;

            _selectedHour = -1;
            _selectedMinute = -1;
            _selectedMoment = string.Empty;
            AssemblyVersion();
            SetHours();
            SetMinutes();
            MomentsList = new List<string>
            {
                "AM","PM"
            };
        }

      
        public int SelectedHour
        {
            get { return _selectedHour; }
            set
            {
                if (Equals(value, _selectedHour))
                {
                    return;
                }
                _selectedHour = value;
                OnPropertyChanged();
            }
        }

        public int SelectedMinute
        {
            get { return _selectedMinute; }
            set
            {
                if (Equals(value, _selectedMinute))
                {
                    return;
                }
                _selectedMinute = value;
                OnPropertyChanged();
            }
        }

        public string SelectedMoment
        {
            get { return _selectedMoment; }
            set
            {
                if (Equals(value, _selectedMoment))
                {
                    return;
                }
                _selectedMoment = value;
                OnPropertyChanged();
            }
        }

       
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                if (Equals(_selectedCustomer, value)) return;
                _selectedCustomer = value;
                OnPropertyChanged();
            }
        }

        public ICommand BrowseCommand
        {
            get { return _browseCommand ?? (_browseCommand = new CommandHandler(Browse, _canExecute)); }
           
        }


        public List<Customer> Customers { get; set; }

       

        public ObservableCollection<ProjectTemplateInfo> Templates
        {
            get
            {
                return _templates;
            }
            set
            {
                if (Equals(value, _templates))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string TextLocation
        {
            get { return _textLocation; }
            set
            {
                if (Equals(value, _textLocation))
                {
                    return;
                }
                _textLocation = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _txtName; }
            set
            {
                if (Equals(value, _txtName))
                {
                    return;
                }
                _txtName = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _txtDescription; }
            set
            {
                if (Equals(value, _txtDescription))
                {
                    return;
                }
                _txtDescription = value;
                OnPropertyChanged();
            }
        }

        public List<ProjectTemplateInfo> StudioTemplates
        {
            get
            {
                return _studioTemplates;
            }
            set
            {
                if (Equals(value, _studioTemplates))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public ProjectTemplateInfo Template
        {
            get { return _template; }
            set
            {
                if (Equals(value, _template))
                {
                    return;
                }
                _template = value;
                OnPropertyChanged();
            }
        }

        public bool HasDueDate
        {
            get { return _hasDueDate; }
            set
            {
                if (Equals(value,_hasDueDate))
                {
                    return;
                }
                _hasDueDate = value;
                OnPropertyChanged();
            }
        }

        public  DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (Equals(value, _dueDate))
                {
                    return;
                }
                _dueDate = value;
                OnPropertyChanged();
            }
        }

        public string SourceLanguage
        {
            get { return _sourceLanguage; }
            set
            {
                if (Equals(value, _sourceLanguage))
                {
                    return;
                    
                }
                OnPropertyChanged();
            }
        }

        public string TargetLanguage
        {
            get { return _targetLanguage; }
            set
            {
                if (Equals(value, _targetLanguage))
                {
                    return;
                }
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TextLocation")
                {if(string.IsNullOrEmpty(TextLocation))
                        
                    return "Location is required";
                }
                if (columnName == "Template" && Template==null)
                {
                    
                        return "Template is rquired";
                   
                }
                return null;
            }
           
        }

        public string Error { get; }

        public  PackageModel GetPackageModel()
        {
            _packageModel.Name = _txtName;
            _packageModel.Description = _txtDescription;
            _packageModel.Location = _textLocation;
            if (_hasDueDate)
            {
                _packageModel.DueDate = TimeHelper.SetDateTime(DueDate, SelectedHour, SelectedMinute, SelectedMoment);
            }

            _packageModel.ProjectTemplate = _template;
            _packageModel.HasDueDate = _hasDueDate;
            _packageModel.Customer = _selectedCustomer;
           return _packageModel;
        }

        public void Browse()
        {
           
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {
                TextLocation = folderDialog.FileName;
            }

        }

       


        private void SetMinutes()
        {
            var minutesList = new List<int>();
           
            for (var i = 0; i <= 59; i++)
            {
                minutesList.Add(i);
            }
            MinutesList = minutesList;
        }

        private void AssemblyVersion()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName().CodeBase;
            var myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var projectsPath = string.Empty;
            if (assembly.Contains("12"))
            {
                projectsPath = Path.Combine(myDocumentsPath, @"Studio 2015\Projects\projects.xml");
            }
            else if (assembly.Contains("11"))
            {
                projectsPath = Path.Combine(myDocumentsPath, @"Studio 2014\Projects\projects.xml");
            }

            ReadCustomers(projectsPath);
        }

        private void ReadCustomers(string projectsPath)
        {
            var sourceProjectsXml = XElement.Load(projectsPath);
            if (!sourceProjectsXml.Element("Customers").HasElements) return;

            var customers = (from customer in sourceProjectsXml.Descendants("Customer")
                             select new Customer
                             {
                                 Guid = new Guid(customer.Attribute("Guid").Value),
                                 Name = customer.Attribute("Name").Value,
                                 Email = customer.Attribute("Email").Value
                             }).ToList();

            Customers = customers;

        }

        private void SetHours()
        {
            var hoursList = new List<int>();
            for (var i = 1; i <= 12; i++)
            {
                hoursList.Add(i);
            }

            HourList = hoursList;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

     
    }
    public class CommandHandler : ICommand,INotifyPropertyChanged
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

       public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
