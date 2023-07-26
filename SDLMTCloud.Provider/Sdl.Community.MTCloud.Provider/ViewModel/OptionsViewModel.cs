using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using NLog;
using Sdl.Community.MTCloud.Languages.Provider;
using Sdl.Community.MTCloud.Provider.Commands;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.Community.MTCloud.Provider.Studio.TranslationProvider;
using Sdl.Community.MTCloud.Provider.View;
using Sdl.LanguagePlatform.Core;
using Application = System.Windows.Forms.Application;
using Cursors = System.Windows.Input.Cursors;
using LogManager = NLog.LogManager;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Sdl.Community.MTCloud.Provider.ViewModel
{
	public class OptionsViewModel : BaseViewModel, IDisposable
	{
		private readonly SdlMTCloudTranslationProvider _provider;
		private readonly List<LanguagePair> _languagePairs;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private ICommand _saveCommand;
		private ICommand _resetToDefaultsCommand;
		private ICommand _viewLanguageMappingsCommand;
		private ICommand _navigateToCommand;

		private bool _reSendChecked;
		private LanguageMappingModel _selectedLanguageMappingModel;
		private ObservableCollection<LanguageMappingModel> _languageMappingModels;
		private bool _isWaiting;
		private bool _sendFeedback;

		public OptionsViewModel(Window owner, SdlMTCloudTranslationProvider provider, List<LanguagePair> languagePairs)
		{
			Owner = owner;

			_provider = provider;
			_languagePairs = languagePairs;

			ReSendChecked = provider.Options?.ResendDraft ?? true;
			SendFeedback = provider.Options?.SendFeedback ?? true;

			LoadLanguageMappings();
		}

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

		public ICommand NavigateToCommand => _navigateToCommand ??= new RelayCommand(NavigateTo);

		public ICommand ResetToDefaultsCommand => _resetToDefaultsCommand ??= new RelayCommand(ResetToDefaults);

		public ICommand ViewLanguageMappingsCommand => _viewLanguageMappingsCommand ??= new RelayCommand(ViewLanguageMappings);

		public Window Owner { get; }

		public ObservableCollection<LanguageMappingModel> LanguageMappingModels
		{
			get => _languageMappingModels;
			set
			{
				if (_languageMappingModels != null)
				{
					foreach (var languageMappingModel in _languageMappingModels)
					{
						languageMappingModel.PropertyChanged -= LanguageMappingModel_PropertyChanged;
					}
				}

				_languageMappingModels = value;
				if (_languageMappingModels != null)
				{
					foreach (var languageMappingModel in _languageMappingModels)
					{
						languageMappingModel.PropertyChanged += LanguageMappingModel_PropertyChanged;
					}
				}

				OnPropertyChanged(nameof(LanguageMappingModels));
			}
		}

		public LanguageMappingModel SelectedLanguageMappingModel
		{
			get => _selectedLanguageMappingModel;
			set
			{
				_selectedLanguageMappingModel = value;
				OnPropertyChanged(nameof(SelectedLanguageMappingModel));
			}
		}

		public bool ReSendChecked
		{
			get => _reSendChecked;
			set
			{
				if (_reSendChecked == value)
				{
					return;
				}

				_reSendChecked = value;
				OnPropertyChanged(nameof(ReSendChecked));
			}
		}

		public bool SendFeedback
		{
			get => _sendFeedback;
			set
			{
				_sendFeedback = value;
				OnPropertyChanged(nameof(SendFeedback));
			}
		}

		public bool IsWaiting
		{
			get => _isWaiting;
			set
			{
				_isWaiting = value;
				OnPropertyChanged(nameof(IsWaiting));
			}
		}

		private void LoadLanguageMappings()
		{
			if (LanguageMappingModels != null && LanguageMappingModels.Any())
			{
				return;
			}

			var languages = _provider.LanguageProvider.GetMappedLanguages();
			var languageMappingModels = new List<LanguageMappingModel>();
			foreach (var languagePair in _languagePairs)
			{
				var languageMappingModel = _provider.GetLanguageMappingModel(languagePair, languages);
				if (languageMappingModel != null)
				{
					languageMappingModels.Add(languageMappingModel);
				}
			}

			LanguageMappingModels = new ObservableCollection<LanguageMappingModel>(languageMappingModels);
		}

		private void ResetToDefaults(object parameter)
		{
			try
			{
				_provider.Options = new Options();

				IsWaiting = true;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				if (LanguageMappingModels is null)
				{
					return;
				}

				var selectedIndex = LanguageMappingModels.IndexOf(SelectedLanguageMappingModel);
				if ((parameter as string) == "ResetSelected" && SelectedLanguageMappingModel is not null)
				{
					var selectedLanguageMappingModelName = SelectedLanguageMappingModel.Name;
					var originalLanguageMappingModels = LanguageMappingModels.Where(x => x.Name != SelectedLanguageMappingModel.Name).ToList();
					LanguageMappingModels.Clear();
					LoadLanguageMappings();

					originalLanguageMappingModels.Insert(selectedIndex, LanguageMappingModels.FirstOrDefault(x => x.Name.Equals(selectedLanguageMappingModelName)));
					LanguageMappingModels = new(originalLanguageMappingModels);
					SelectedLanguageMappingModel = LanguageMappingModels.ElementAt(selectedIndex) ?? LanguageMappingModels.FirstOrDefault();

					System.Windows.MessageBox.Show($"{PluginResources.Message_Successfully_reset_to_defaults} the selected model",
						Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else if ((parameter as string) == "ResetSelected" && SelectedLanguageMappingModel is null)
				{
					System.Windows.MessageBox.Show("Please select a model to reset",
						Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				else
				{
					LanguageMappingModels.Clear();
					LoadLanguageMappings();
					SelectedLanguageMappingModel = LanguageMappingModels.ElementAt(selectedIndex) ?? LanguageMappingModels.FirstOrDefault();
					System.Windows.MessageBox.Show(PluginResources.Message_Successfully_reset_to_defaults,
						Application.ProductName, MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				IsWaiting = false;
				_logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");

				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			finally
			{
				IsWaiting = false;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}

		private void Reload()
		{
			try
			{
				IsWaiting = true;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				if (LanguageMappingModels != null)
				{
					LanguageMappingModels.Clear();
					LoadLanguageMappings();
				}
			}
			catch (Exception ex)
			{
				IsWaiting = false;
				_logger.Error($"{Constants.IsWindowValid} {ex.Message}\n {ex.StackTrace}");

				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
					MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			finally
			{
				IsWaiting = false;
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}

		private void Save(object parameter)
		{
			try
			{
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Wait;
				}

				var canSave = true;
				var invalidModel = LanguageMappingModels.FirstOrDefault(a => a.SelectedModel.DisplayName == PluginResources.Message_No_model_available);
				if (invalidModel != null)
				{
					canSave = false;

					if (Owner != null)
					{
						var message = string.Format(PluginResources.Message_SelectLanguageDirectionForMTModel,
							invalidModel.SelectedSource.CodeName, invalidModel.SelectedTarget.CodeName);
						var question = PluginResources.Message_DoYouWantToProceed;

						var response = MessageBox.Show(message + Environment.NewLine + Environment.NewLine + question,
							Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (response == DialogResult.Yes)
						{
							canSave = true;
						}
					}
				}

				if (canSave)
				{
					_provider.Options.ResendDraft = ReSendChecked;
					_provider.Options.LanguageMappings = LanguageMappingModels.ToList();
					_provider.Options.SendFeedback = SendFeedback;

					Dispose();

					if (Owner != null)
					{
						WindowCloser.SetDialogResult(Owner, true);
						Owner.Close();
					}
				}
			}
			finally
			{
				if (Owner != null)
				{
					Mouse.OverrideCursor = Cursors.Arrow;
				}
			}
		}

		private void NavigateTo(object parameter)
		{
			const string WikiUrl = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/w/wiki/5561/rating-translations";
			const string AccountUrl_UE = "https://portal.languageweaver.com/settings/account";
			const string AccountUrl_US = "https://us.portal.languageweaver.com/settings/account";

			var region = _provider.TranslationService.ConnectionService.Credential.AccountRegion.ToString().ToLower();
			switch (parameter as string)
			{
				case "wiki":
					Process.Start(WikiUrl);
					break;

				case "account":
					Process.Start(region.Contains("us") ? AccountUrl_US : AccountUrl_UE);
					break;

				default:
					break;
			}
		}

		private void ViewLanguageMappings(object obj)
		{
			if (Owner == null)
			{
				return;
			}

			var window = new MTCodesWindow
			{
				Owner = Owner.Owner
			};

			var languages = new LanguageProvider();
			var viewModel = new MTCodesViewModel(window, languages);
			window.DataContext = viewModel;

			var result = window.ShowDialog();
			if (result.HasValue && result.Value)
			{
				Reload();
			}
		}

		private void LanguageMappingModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (sender is LanguageMappingModel languageModel)
			{
				if (e.PropertyName == nameof(LanguageMappingModel.SelectedSource) ||
					e.PropertyName == nameof(LanguageMappingModel.SelectedTarget))
				{
					_provider.UpdateLanguageMappingModel(languageModel);
				}
			}
		}

		public void Dispose()
		{
			if (_languageMappingModels != null)
			{
				foreach (var languageMappingModel in _languageMappingModels)
				{
					languageMappingModel.PropertyChanged -= LanguageMappingModel_PropertyChanged;
				}
			}
		}
	}
}