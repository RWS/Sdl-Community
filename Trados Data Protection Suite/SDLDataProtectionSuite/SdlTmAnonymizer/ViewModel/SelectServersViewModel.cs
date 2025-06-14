﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
    public class SelectServersViewModel : ViewModelBase, IDisposable
    {
        private List<TmFile> _translationMemories;
        private readonly SettingsService _settingsService;
        private readonly List<TranslationProviderServerWithCredentials> _translationProviderServers;
        private readonly BackgroundWorker _backgroundWorker;
        private readonly Window _controlWindow;

        public SelectServersViewModel(Window controlWindow, SettingsService settingsService, List<TranslationProviderServerWithCredentials> translationProviderServers)
        {
            _controlWindow = controlWindow;

            _settingsService = settingsService;
            _translationProviderServers = translationProviderServers;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerAsync();
        }

        public List<TmFile> TranslationMemories
        {
            get => _translationMemories ?? (_translationMemories = new List<TmFile>());
            set
            {
                if (_translationMemories == value)
                {
                    return;
                }

                _translationMemories = value;

                OnPropertyChanged(nameof(TranslationMemories));
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressDialogSettings settings = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                settings = new ProgressDialogSettings(_controlWindow, true, true, true);
                var result = ProgressDialog.Execute(StringResources.Loading_data, () =>
                {
                    GetServerTms(ProgressDialog.Current);

                }, settings);

                RefreshView();

                if (result.Cancelled)
                {
                    throw new Exception(StringResources.Process_cancelled_by_user);
                }

                if (result.OperationFailed)
                {
                    throw new Exception(StringResources.Process_failed + Environment.NewLine + Environment.NewLine + result.Error.Message);
                }
            });
        }

        private void GetServerTms(ProgressDialogContext context)
        {
            foreach (var translationProviderServer in _translationProviderServers)
            {
                List<ServerBasedTranslationMemory> tms = [];
                try
                {
                    tms = translationProviderServer.Server.GetTranslationMemories().ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                foreach (var serverBasedTranslationMemory in tms)
                {
                    if (context.CheckCancellationPending())
                    {
                        break;
                    }

                    var tmPath = serverBasedTranslationMemory.ParentResourceGroupPath == "/" ? "" : serverBasedTranslationMemory.ParentResourceGroupPath;
                    var path = tmPath + "/" + serverBasedTranslationMemory.Name;
                    var tmAlreadyExist = TranslationMemories.Any(t => t.Path.Equals(path));

                    context.Report(path);

                    if (!tmAlreadyExist)
                    {
                        var serverTm = new TmFile
                        {
                            Path = path,
                            Name = serverBasedTranslationMemory.Name,
                            Description = serverBasedTranslationMemory.Description,
                            TranslationUnits = serverBasedTranslationMemory.GetTranslationUnitCount(),
                            IsServerTm = true,
                            TmLanguageDirections = new List<TmLanguageDirection>(),
                            Credentials = translationProviderServer.Credentials
                        };

                        foreach (var languageDirection in serverBasedTranslationMemory.LanguageDirections)
                        {
                            serverTm.TmLanguageDirections.Add(

                                new TmLanguageDirection
                                {
                                    Source = languageDirection.SourceLanguage.Name,
                                    Target = languageDirection.TargetLanguage.Name,
                                    TranslationUnitsCount = languageDirection.GetTranslationUnitCount()
                                });
                        }

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            TranslationMemories.Add(serverTm);
                            RefreshView();
                        });
                    }

                }
            }


        }

        private void RefreshView()
        {
            OnPropertyChanged(nameof(TranslationMemories));
            ((SelectServersView)_controlWindow).Refresh();
        }

        public void Dispose()
        {
            if (_backgroundWorker != null)
            {
                _backgroundWorker.DoWork -= BackgroundWorker_DoWork;
                _backgroundWorker.Dispose();
            }
        }
    }
}
