using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Annotations;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class MainWindowViewModel: ModelBase, IMainWindow
	{
		private ViewDetails _selectedView;

		public MainWindowViewModel(IProviderControlViewModel providerControlViewModel,ISettingsControlViewModel settingsControlViewModel)
		{
			SaveCommand = new CommandHandler(Save,true);
			ShowSettingsViewCommand =  new CommandHandler(ShowSettingsPage, true);
			ShowMainViewCommand = new CommandHandler(ShowProvidersPage,true);

			providerControlViewModel.ShowSettingsCommand = ShowSettingsViewCommand;
			settingsControlViewModel.ShowMainWindowCommand = ShowMainViewCommand;

			AvailableViews = new List<ViewDetails>
			{
				new ViewDetails
				{
					Name = PluginResources.PluginsView,
					ViewModel = providerControlViewModel.ViewModel,
				},
				new ViewDetails
				{
					Name = PluginResources.SettingsView,
					ViewModel = settingsControlViewModel.ViewModel
				}
			};

			ShowProvidersPage();
		}

		public ViewDetails SelectedView
		{
			get => _selectedView;
			set
			{
				_selectedView = value;
				OnPropertyChanged(nameof(SelectedView));
			}
		}

		public List<ViewDetails> AvailableViews { get; set; }
		public ICommand ShowSettingsViewCommand { get; set; }
		public ICommand ShowMainViewCommand { get; set; }
		public ICommand SaveCommand { get; set; }

		private void ShowSettingsPage()
		{
			SelectedView = AvailableViews[1];
		}

		private void ShowProvidersPage()
		{
			SelectedView = AvailableViews[0];
		}

		private void Save()
		{
			//TODO: Validate the form
			//TODO: Save all value on Provider option
		}
	}
}
