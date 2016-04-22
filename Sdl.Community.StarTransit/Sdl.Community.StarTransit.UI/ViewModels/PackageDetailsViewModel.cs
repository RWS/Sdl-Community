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
        private static string _textLocation;
        private static string _txtName;
        private static string _txtDescription;
        private static List<ProjectTemplateInfo> _studioTemplates;
        private static bool _hasDueDate;
        private static DateTime? _dueDate;
        private static string _sourceLanguage;
        private static string _targetLanguage;
        private static PackageModel _packageModel;
        private ICommand _browseCommand;
        private static bool _canExecute;
        private static ProjectTemplateInfo _template;
        private IProject _project;
        private static Customer _selectedCustomer; 
        public ObservableCollection<ProjectTemplateInfo> _templates;
        
        public PackageDetailsViewModel(PackageModel package)
        {
            _packageModel = package;
            _txtName = package.Name;
            _txtDescription = package.Description;
            _studioTemplates = package.StudioTemplates;
            _textLocation = package.Location;
            _sourceLanguage = package.SourceLanguage.DisplayName;
            _templates =new ObservableCollection<ProjectTemplateInfo>(package.StudioTemplates);
            _hasDueDate = false;
           var targetLanguage = string.Empty;
            foreach (var language in package.TargetLanguage)
            {
                targetLanguage = targetLanguage + language.DisplayName;
            }
            _targetLanguage = targetLanguage;

            _canExecute = true;

            AssemblyVersion();
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
            else if(assembly.Contains("11"))
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

        public void Browse()
        {
            //var folderDialog = new FolderBrowserDialog();
            //var result = folderDialog.ShowDialog();

            //if (result == DialogResult.OK)
            //{
            //   TextLocation = folderDialog.SelectedPath;

            //}
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {
                TextLocation = folderDialog.FileName;
            }

        }

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

        public DateTime? DueDate
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

        public static PackageModel GetPackageModel()
        {
            _packageModel.Name = _txtName;
            _packageModel.Description = _txtDescription;
            _packageModel.Location = _textLocation;
            if (_hasDueDate)
            {
                _packageModel.DueDate = _dueDate;
            }
           
            _packageModel.ProjectTemplate = _template;
            _packageModel.HasDueDate = _hasDueDate;
            _packageModel.Customer = _selectedCustomer;
           return _packageModel;
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

        //protected virtual void OnCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        //}

    }
}
