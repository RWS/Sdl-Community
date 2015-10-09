using Sdl.Community.BetaAPIs.UI.DataProvider;
using Sdl.Community.BetaAPIs.UI.Model;
using Sdl.Community.BetaAPIs.UI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.BetaAPIs.UI.Commands;

namespace Sdl.Community.BetaAPIs.UI.ViewModel
{
    public class MainViewModel: Observable
    {
        private readonly IAPIDataProvider _dataProvider;
        private readonly PluginService _pluginService;
        private PluginConfig _pluginConfig;
        private API _selectedApi;

        public ObservableCollection<API> APIs { get; set; }

        private CommandsHandler _enableCommand;
        public CommandsHandler EnableCommand
        {
            get
            {
                return _enableCommand ?? (_enableCommand = new CommandsHandler(() => Enable(), !SelectedAPI.Enabled));
            }
        }

        private CommandsHandler _disableCommand;
        public CommandsHandler DisableCommand
        {
            get
            {
                return _disableCommand ?? (_disableCommand = new CommandsHandler(() => Disable(), SelectedAPI.Enabled));
            }
        }

        public API SelectedAPI
        {
            get { return _selectedApi; }
            set
            {
                _selectedApi = value;
                OnPropertyChanged();
                DisableCommand.SetCanExecute = _selectedApi.Enabled;
                EnableCommand.SetCanExecute = !_selectedApi.Enabled;
                EnableCommand.RaiseCanExecuteChanged();
                DisableCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel(IAPIDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            APIs = new ObservableCollection<API>();
            _pluginService = new PluginService();
            LoadData();
        }

        private void LoadData()
        {
            var apis = _dataProvider.LoadAPIs();
            _pluginConfig = _pluginService.LoadPluginConfig();
            DetermineEnabledAPIs(apis, _pluginConfig);

            foreach (var api in apis)
            {
                APIs.Add(api);
            }

            SelectedAPI = APIs.Count > 0 ? APIs.First() : null;

        }

        private void DetermineEnabledAPIs(IEnumerable<API> apis, PluginConfig pluginConfig)
        {
            foreach (var api in apis)
            {
                api.Enabled = CheckIfEnabled(api.Assemblies,pluginConfig);                 
            }
        }

        private bool CheckIfEnabled(List<string> assemblies, PluginConfig pluginConfig)
        {
            var enabled = false;
            var publicAssemblies = pluginConfig.Items.OfType<PluginConfigPublicAssemblies>().FirstOrDefault();

            if (publicAssemblies == null) return enabled;

            foreach (var publicAssembly in publicAssemblies.PublicAssembly)
            {
                enabled = assemblies.Contains(publicAssembly.name);
            }

            return enabled;

        }

        private void Enable()
        {
            var termsDialog = new TermsDialog();
            
            var result = termsDialog.ShowDialog();

            if(result.HasValue && result.Value)
            {
                _pluginService.AddPublicAssemblies(SelectedAPI.Assemblies);
                _pluginService.SavePluginConfig();
                SelectedAPI.Enabled = true;
                EnableCommand.SetCanExecute = false;
                DisableCommand.SetCanExecute = true;
                EnableCommand.RaiseCanExecuteChanged();
                DisableCommand.RaiseCanExecuteChanged();

            }

        }

        private void Disable()
        {
            _pluginService.RemovePublicAssemblies(SelectedAPI.Assemblies);
            _pluginService.SavePluginConfig();
            SelectedAPI.Enabled = false;
            DisableCommand.SetCanExecute = false;
            EnableCommand.SetCanExecute = true;
            EnableCommand.RaiseCanExecuteChanged();
            DisableCommand.RaiseCanExecuteChanged();
        }
    }
}
