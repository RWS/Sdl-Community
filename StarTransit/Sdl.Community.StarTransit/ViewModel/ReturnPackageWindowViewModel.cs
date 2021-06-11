using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.StarTransit.Command;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Model;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;

namespace Sdl.Community.StarTransit.ViewModel
{
	public class ReturnPackageWindowViewModel: BaseModel
	{
		private readonly IDialogService _dialogService;
		private ICommand _browseCommand;
		private ICommand _clearCommand;
		private string _returnPackageLocation;

		public ReturnPackageWindowViewModel(IReturnPackage returnPackage, IDialogService dialogService)
		{
			_dialogService = dialogService;
			ReturnPackage = returnPackage;
		}

		public string ReturnPackageLocation
		{
			get => _returnPackageLocation;
			set
			{
				if (_returnPackageLocation == value) return;
				_returnPackageLocation = value;
				OnPropertyChanged(nameof(ReturnPackageLocation));
			}
		}

		public IReturnPackage ReturnPackage { get; set; }

		public ICommand BrowseCommand => _browseCommand ?? (_browseCommand = new RelayCommand(BrowseLocation));
		public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(ClearLocation));

		private void ClearLocation()
		{
			ReturnPackageLocation = string.Empty;
		}

		private void BrowseLocation()
		{
			//TODO: Add validation
		}
	}
}
