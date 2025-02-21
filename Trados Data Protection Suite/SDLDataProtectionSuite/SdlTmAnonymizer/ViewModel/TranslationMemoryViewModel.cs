﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Commands;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
    public class TranslationMemoryViewModel : ViewModelBase, IDisposable
    {
        private bool _selectAll;
        private ICommand _dragEnterCommand;
        private ICommand _clearTMCacheCommand;
        private ICommand _removeTMCommand;
        private IList _selectedItems;
        private ObservableCollection<TmFile> _tmsCollection;
        private bool _isEnabled;
        //private LoginViewModel _loginWindowViewModel;
        private readonly SDLTMAnonymizerView _controller;
        private readonly ContentParsingService _contentParsingService;
        private readonly SerializerService _serializerService;
        private Form _controlParent;

        public TranslationMemoryViewModel(SettingsService settingsService, ContentParsingService contentParsingService,
            SerializerService serializerService, SDLTMAnonymizerView controller, GroupshareCredentialManager groupShareCredentialManager)
        {
            SettingsService = settingsService;
            GroupShareCredentialManager = groupShareCredentialManager;

            _contentParsingService = contentParsingService;
            _serializerService = serializerService;
            _controller = controller;

            TmService = new TmService(settingsService, _contentParsingService, _serializerService, GroupShareCredentialManager);

            IsEnabled = true;
            TmsCollection = new ObservableCollection<TmFile>(SettingsService.GetTmFiles());
        }

        public Form ControlParent
        {
            get
            {
                if (_controlParent == null)
                {
                    try
                    {
                        var elementHost = _controller?.ContentControl?.Controls[0] as ElementHost;
                        _controlParent = elementHost?.FindForm();
                    }
                    catch { }
                }

                return _controlParent;
            }
        }

        public TmService TmService { get; set; }

        public SettingsService SettingsService { get; set; }
        private GroupshareCredentialManager GroupShareCredentialManager { get; }

        public ObservableCollection<TmFile> TmsCollection
        {
            get => _tmsCollection;
            set
            {
                if (Equals(value, _tmsCollection))
                {
                    return;
                }

                if (value != null)
                {
                    foreach (var tm in value)
                    {
                        tm.PropertyChanged -= Tm_PropertyChanged;
                    }
                }

                _tmsCollection = value;

                if (_tmsCollection != null)
                {
                    foreach (var tm in _tmsCollection)
                    {
                        tm.PropertyChanged += Tm_PropertyChanged;
                    }
                }

                OnPropertyChanged(nameof(TmsCollection));
            }
        }

        public IList SelectedItems
        {
            get => _selectedItems;
            set
            {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public bool SelectAll
        {
            get => _selectAll;
            set
            {
                if (Equals(value, _selectAll))
                {
                    return;
                }
                _selectAll = value;
                OnPropertyChanged(nameof(SelectAll));
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (Equals(value, _isEnabled))
                {
                    return;
                }
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public void Refresh()
        {
            OnPropertyChanged(nameof(TmsCollection));
        }

        public ICommand DragEnterCommand => _dragEnterCommand ??= new RelayCommand(TmDragEnter);

        public ICommand ClearTmCacheCommand => _clearTMCacheCommand ??= new RelayCommand(ClearTmCache);

        public ICommand RemoveTmCommand => _removeTMCommand ??= new RelayCommand(RemoveTm);

        public void AddServerTm()
        {
            var settings = SettingsService.GetSettings();
            var credentials = new Credentials
            {
                Url = settings.LastUsedServerUri,
                UserName = settings.LastUsedServerUserName
            };

            var translationProviderServer = GroupShareCredentialManager.GetProvider(credentials);

            settings.LastUsedServerUri = credentials.Url;
            settings.LastUsedServerUserName = credentials.UserName;
            SettingsService.SaveSettings(settings);

            if (translationProviderServer == null)
                return;

            var selectServers = new SelectServersView();
            var model = new SelectServersViewModel(selectServers, SettingsService, translationProviderServer);

            selectServers.DataContext = model;
            selectServers.ShowDialog();

            if (!selectServers.DialogResult.HasValue || !selectServers.DialogResult.Value) return;

            foreach (var tmFile in model.TranslationMemories.Where(tmFile => tmFile.IsSelected))
            {
                tmFile.Credentials = credentials;
                AddTm(tmFile);
            }
        }

        public void AddFileBasedTm()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var fileName in fileDialog.FileNames)
                {
                    if (!string.IsNullOrEmpty(fileName) && Path.GetExtension(fileName).Equals(".sdltm"))
                    {
                        AddTm(fileName);
                    }
                }
            }
        }

        public void SelectFolder()
        {
            var folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog())
            {
                var folderPath = folderDialog.FileName;
                var files = Directory.GetFiles(folderPath, "*.sdltm");
                foreach (var file in files)
                {
                    AddTm(file);
                }
            }
        }

        private void TmDragEnter(object parameter)
        {
            if (parameter != null && parameter is System.Windows.DragEventArgs eventArgs)
            {
                var fileDrop = eventArgs.Data.GetData(DataFormats.FileDrop, false);
                if (fileDrop is string[] filesOrDirectories && filesOrDirectories.Length > 0)
                {
                    foreach (var fullPath in filesOrDirectories)
                    {
                        if (!string.IsNullOrEmpty(fullPath) && Path.GetExtension(fullPath).Equals(".sdltm"))
                        {
                            AddTm(fullPath);
                        }
                    }
                }
            }
        }

        private void RemoveTm(object parameter)
        {
            var result = MessageBox.Show(StringResources.RemoveTm_Do_you_want_to_remove_selected_tms, StringResources.Confirmation, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK && SelectedItems != null)
            {
                var selectedTms = new List<TmFile>();

                foreach (TmFile selectedItem in SelectedItems)
                {
                    var rule = new TmFile
                    {
                        Path = selectedItem.Path
                    };

                    selectedTms.Add(rule);
                }

                SelectedItems.Clear();
                foreach (var selectedTm in selectedTms)
                {
                    var tm = TmsCollection.FirstOrDefault(r => r.Path.Equals(selectedTm.Path));
                    if (tm != null)
                    {
                        RemoveTm(tm);
                    }
                }
            }
        }

        private void ClearTmCache(object parameter)
        {
            foreach (TmFile tmFile in SelectedItems)
            {
                if (!string.IsNullOrEmpty(tmFile.CachePath) && File.Exists(tmFile.CachePath))
                {
                    File.Delete(tmFile.CachePath);
                    tmFile.CachePath = string.Empty;
                    tmFile.IsSelected = false;
                }
            }
        }

        private void SaveSetttings()
        {
            var settings = SettingsService.GetSettings();
            settings.TmFiles = TmsCollection.ToList();
            SettingsService.SaveSettings(settings);
        }

        private bool TmAlreadyExist(string tmPath)
        {
            return TmsCollection.Any(t => t.Path.Equals(tmPath));
        }

        private void AddTm(string tmPath)
        {
            if (!TmAlreadyExist(tmPath))
            {
                var tmFileInfo = new FileInfo(tmPath);

                var fileBasedTm = new FileBasedTranslationMemory(tmFileInfo.FullName);
                var unitsCount = fileBasedTm.LanguageDirection.GetTranslationUnitCount();

                var tm = new TmFile
                {
                    Name = tmFileInfo.Name,
                    Path = tmFileInfo.FullName,
                    TranslationUnits = unitsCount,
                    TmLanguageDirections =
                    [
                        new TmLanguageDirection()
                        {
                            Source = fileBasedTm.LanguageDirection.SourceLanguage.Name,
                            Target = fileBasedTm.LanguageDirection.TargetLanguage.Name,
                            TranslationUnitsCount = unitsCount
                        }
                    ]
                };


                AddTm(tm);
            }
        }

        private void AddTm(TmFile tm)
        {
            tm.IsSelected = false;
            tm.PropertyChanged += Tm_PropertyChanged;
            TmsCollection.Insert(0, tm);
            SaveSetttings();
        }

        private void RemoveTm(TmFile tm)
        {
            tm.PropertyChanged -= Tm_PropertyChanged;
            TmsCollection.Remove(tm);

            if (!string.IsNullOrEmpty(tm.CachePath) && File.Exists(tm.CachePath))
            {
                File.Delete(tm.CachePath);
            }

            SaveSetttings();
        }

        private void Tm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(TmFile.IsSelected)) || sender is not TmFile tmFile) return;

            if (tmFile.IsSelected && tmFile.IsServerTm)
            {
                var settings = SettingsService.GetSettings();

                tmFile.Credentials ??= new Credentials
                {
                    Url = settings.LastUsedServerUri,
                    UserName = settings.LastUsedServerUserName
                };

                var translationProviderServer = GroupShareCredentialManager.TryGetProviderWithoutUserInput(tmFile.Credentials);

                if (translationProviderServer != null)
                {
                    tmFile.Credentials = tmFile.Credentials;
                    SettingsService.SaveSettings(settings);
                }
                else
                {
                    tmFile.IsSelected = false;
                }
            }

            if (!tmFile.IsSelected)
            {
                foreach (var languageDirection in tmFile.TmLanguageDirections)
                {
                    languageDirection.TranslationUnits = null;
                }
            }

            SaveSetttings();

            OnPropertyChanged(nameof(TmsCollection));
        }

        public void Dispose()
        {
            if (TmsCollection == null) return;

            foreach (var tm in TmsCollection)
            {
                tm.PropertyChanged -= Tm_PropertyChanged;
            }
        }
    }
}
