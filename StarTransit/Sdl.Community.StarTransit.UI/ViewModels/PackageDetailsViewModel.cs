using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Linq;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Community.StarTransit.UI.Annotations;
using Sdl.Community.StarTransit.UI.Commands;
using Sdl.Community.StarTransit.UI.Helpers;
using Sdl.Community.StarTransit.UI.Interfaces;
using Sdl.Community.Toolkit.Core.Services;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class PackageDetailsViewModel : IDataErrorInfo, INotifyPropertyChanged, IWindowActions
    {
        private string _textLocation;
        private  string _txtName;
        private  string _txtDescription;
        private readonly List<ProjectTemplateInfo> _studioTemplates;
        private  bool _hasDueDate;
        private  DateTime? _dueDate;
        private  readonly string  _sourceLanguage;
        private  readonly string _targetLanguage;
        private readonly PackageModel _packageModel;
        private ICommand _browseCommand;
        private readonly bool _canExecute;
        private  ProjectTemplateInfo _template;
        private  Customer _selectedCustomer; 
        private readonly ObservableCollection<ProjectTemplateInfo> _templates;
        private  int _selectedHour;
        private  int _selectedMinute;
        private  string _selectedMoment;
		private StarTransitMainWindow _window;

		public static readonly Log Log = Log.Instance;

		public List<int> HourList { get; set; }
        public List<int> MinutesList { get; set; }
        public List<string> MomentsList { get; set; }

        public PackageDetailsViewModel(PackageModel package, StarTransitMainWindow window)
        {
            _window = window;
            _packageModel = package;
            _txtName = package.Name;
            _txtDescription = package.Description;
            _studioTemplates = package.StudioTemplates;
            _textLocation = package.Location;
            _sourceLanguage = package.LanguagePairs[0].SourceLanguage.DisplayName;
            _templates = new ObservableCollection<ProjectTemplateInfo>(package.StudioTemplates);
            _hasDueDate = false;
            _targetLanguage = string.Empty;

            foreach (var pair in package.LanguagePairs)
            {
                var targetLanguage = string.Concat(" ", pair.TargetLanguage.DisplayName);
                _targetLanguage =string.Concat(_targetLanguage,targetLanguage);
            }
            _canExecute = true;
            _selectedHour = -1;
            _selectedMinute = -1;
            _selectedMoment = string.Empty;
			GetCustomers();
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
                if (columnName == "TextLocation" && string.IsNullOrEmpty(TextLocation))
				{       
                    return "Location is required";
                }
                if (columnName == "Template" && Template==null)
                {
					return "Template is required";                   
                }
                if (columnName == "SelectedHour" && SelectedHour == -1)
                {
                    return "Please select an hour.";
                }
                if (columnName == "SelectedMinute" && SelectedMinute == -1)
                {
                    return "Please select minutes.";
                }
                return null;
            }           
        }

        public string Error { get; }

        public Action CloseAction { get; set; }

        public Action<string, string> ShowWindowsMessage { get; set; }


		public PackageModel GetPackageModel()
		{
			try
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
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"GetPackageModel method: {ex.Message}\n {ex.StackTrace}");
			}
			return _packageModel;
		}

        public void Browse()
        {
           
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {
                 if (!Utils.IsFolderEmpty(folderDialog.FileName))
                {
                    ShowWindowsMessage("Folder not empty!", "Please select an empty folder");
                }
                else
                {
                    TextLocation = folderDialog.FileName;
                }
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

        private void GetCustomers()
        {
			try
			{
				var shortStudioVersion = GetInstalledStudioShortVersion();
				if (!string.IsNullOrEmpty(shortStudioVersion))
				{
					var projectsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $@"Studio {shortStudioVersion}\Projects\projects.xml");
					ReadCustomers(projectsPath);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"GetCustomers method: {ex.Message}\n {ex.StackTrace}");
			}
        }

		private string GetInstalledStudioShortVersion()
		{
			var studioService = new StudioVersionService();
			return studioService?.GetStudioVersion()?.ShortVersion;
		}

		private void ReadCustomers(string projectsPath)
        {
			try
			{
				var sourceProjectsXml = XElement.Load(projectsPath);
				if (!sourceProjectsXml.Element("Customers").HasElements) return;

				var customers = (from customer in sourceProjectsXml.Descendants("Customer")
								 select new Customer
								 {
									 Guid = new Guid(customer.Attribute("Guid").Value),
									 Name = customer.Attribute("Name").Value,
									 Email = customer.Attribute("Email").Value
								 }).OrderBy(c => c.Name).ToList();

				Customers = customers;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ReadCustomers method: {ex.Message}\n {ex.StackTrace}");
			}
		}

        private void SetHours()
        {
			try
			{
				var hoursList = new List<int>();
				for (var i = 1; i <= 12; i++)
				{
					hoursList.Add(i);
				}
				HourList = hoursList;
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"SetHours method: {ex.Message}\n {ex.StackTrace}");
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