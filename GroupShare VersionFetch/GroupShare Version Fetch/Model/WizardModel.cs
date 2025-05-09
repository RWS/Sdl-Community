﻿using System.Collections.ObjectModel;

namespace Sdl.Community.GSVersionFetch.Model
{
    public class WizardModel : BaseModel
    {
        private ObservableCollection<GsFileVersion> _filesFileVersions;
        private ObservableCollection<GsFile> _gsFiles;
        private ObservableCollection<GsProject> _gsProjects;
        private ObservableCollection<OrganizationResponse> _organizations;
        private ObservableCollection<GsProject> _projectsForCurrentPage;
        private int _projectsNumber;
        private OrganizationResponse _selectedOrganization;
        private int _totalPages;
        private Credentials _userCredentials;
        private string _version;

        public ObservableCollection<GsFileVersion> FileVersions
        {
            get => _filesFileVersions;
            set
            {
                _filesFileVersions = value;
                OnPropertyChanged(nameof(FileVersions));
            }
        }

        public ObservableCollection<GsFile> GsFiles
        {
            get => _gsFiles;
            set
            {
                _gsFiles = value;
                OnPropertyChanged(nameof(GsFiles));
            }
        }

        public ObservableCollection<GsProject> GsProjects
        {
            get => _gsProjects;
            set
            {
                _gsProjects = value;
                OnPropertyChanged(nameof(GsProjects));
            }
        }

        public ObservableCollection<OrganizationResponse> Organizations
        {
            get => _organizations;
            set
            {
                if (_organizations == value)
                {
                    return;
                }
                _organizations = value;
                OnPropertyChanged(nameof(Organizations));
            }
        }

        public ObservableCollection<GsProject> ProjectsForCurrentPage
        {
            get => _projectsForCurrentPage;
            set
            {
                _projectsForCurrentPage = value;
                OnPropertyChanged(nameof(GsProjects));
            }
        }

        public int ProjectsNumber
        {
            get => _projectsNumber;
            set
            {
                if (_projectsNumber == value)
                {
                    return;
                }
                _projectsNumber = value;
                OnPropertyChanged(nameof(ProjectsNumber));
            }
        }

        public OrganizationResponse SelectedOrganization
        {
            get => _selectedOrganization;
            set
            {
                _selectedOrganization = value;
                OnPropertyChanged(nameof(SelectedOrganization));
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (_totalPages == value)
                {
                    return;
                }
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }

        public Credentials UserCredentials
        {
            get => _userCredentials;
            set
            {
                _userCredentials = value;
                OnPropertyChanged(nameof(UserCredentials));
            }
        }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }
    }
}