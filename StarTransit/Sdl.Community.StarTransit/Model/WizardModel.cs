using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Interface;
using Sdl.Community.StarTransit.Service;
using Sdl.Community.StarTransit.Shared.Models;

namespace Sdl.Community.StarTransit.Model
{
	public class WizardModel:BaseModel,IWizardModel
	{
		private string _studioProjectLocation;
		private Customer _selectedCustomer;
		private AsyncTaskWatcherService<List<Customer>> _customers;
		private AsyncTaskWatcherService<PackageModel> _packageModel;

		public string TransitFilePathLocation { get; set; }
		public string PathToTempFolder { get; set; }

		public string StudioProjectLocation
		{
			get => _studioProjectLocation;
			set
			{
				if (_studioProjectLocation == value) return;
				_studioProjectLocation = value;
				OnPropertyChanged(nameof(StudioProjectLocation));
			}
		}

		public AsyncTaskWatcherService<PackageModel> PackageModel
		{
			get => _packageModel;
			set
			{
				_packageModel = value;
				OnPropertyChanged(nameof(PackageModel));
			}
		}

		public AsyncTaskWatcherService<List<Customer>> Customers
		{
			get => _customers;
			set
			{
				_customers = value;
				OnPropertyChanged(nameof(Customers));
			}
		}

		public Customer SelectedCustomer
		{
			get => _selectedCustomer;
			set
			{
				_selectedCustomer = value;
				OnPropertyChanged(nameof(SelectedCustomer));
			}
		}
	}
}
