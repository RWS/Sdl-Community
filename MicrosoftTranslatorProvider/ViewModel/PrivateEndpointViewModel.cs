using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Commands;
using MicrosoftTranslatorProvider.Interface;
using MicrosoftTranslatorProvider.Model;
using RestSharp;

namespace MicrosoftTranslatorProvider.ViewModel
{
	public class PrivateEndpointViewModel : BaseModel, IPrivateEndpointViewModel
	{
		private string _endpoint;
		private ObservableCollection<UrlMetadata> _headers;
		private ObservableCollection<UrlMetadata> _parameters;

		private ICommand _clearCommand;
		private ICommand _addHeaderCommand;
		private ICommand _addParameterCommand;
		private ICommand _deletePairCommand;
		private ICommand _selectedItemChangedCommand;

		public PrivateEndpointViewModel()
		{
			Headers = new();
			Parameters = new();
			Endpoint = string.Empty;

			AddHeaderCommand.Execute(null);

			AddParameterCommand.Execute(new UrlMetadata()
			{
				Key = "from",
				Value = "sourceLanguage",
				IsReadOnly = true
			});

			AddParameterCommand.Execute(new UrlMetadata()
			{
				Key = "to",
				Value = "targetLanguage",
				IsReadOnly = true
			});
		}

		public BaseModel ViewModel => this;

		public string Endpoint
		{
			get => _endpoint;
			set
			{
				_endpoint = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<UrlMetadata> Headers
		{
			get => _headers;
			set
			{
				_headers = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<UrlMetadata> Parameters
		{
			get => _parameters;
			set
			{
				_parameters = value;
				OnPropertyChanged();
			}
		}

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public ICommand AddHeaderCommand => _addHeaderCommand ??= new RelayCommand(AddHeader);

		public ICommand AddParameterCommand => _addParameterCommand ??= new RelayCommand(AddParameter);

		public ICommand DeletePairCommand => _deletePairCommand ??= new RelayCommand(DeletePair);

		public ICommand SelectedItemChangedCommand => _selectedItemChangedCommand ??= new RelayCommand(SelectedItemChanged);

		private void Clear(object parameter)
		{
			if (parameter is not string parameterString)
			{
				return;
			}

			switch (parameterString)
			{
				case "url":
					Endpoint = string.Empty;
					break;
			}
		}

		private void DeletePair(object parameter)
		{
			if (parameter is not UrlMetadata pair)
			{
				return;
			}

			Headers.Remove(pair);
			Parameters.Remove(pair);
		}

		private void AddHeader(object parameter)
		{
			Headers.Add(parameter is UrlMetadata keyValuePair ? keyValuePair : new());
		}

		private void AddParameter(object parameter)
		{
			Parameters.Add(parameter is UrlMetadata keyValuePair ? keyValuePair : new());
		}

		private void SelectedItemChanged(object parameter)
		{
			if (parameter is not UrlMetadata selectedHeaderPair)
			{
				return;
			}

			if (Headers.FirstOrDefault(x => x.IsSelected) is UrlMetadata currentPair)
			{
				currentPair.IsSelected = false;
			}

			selectedHeaderPair.IsSelected = true;
		}
	}
}