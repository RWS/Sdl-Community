using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		private ObservableCollection<TabItem> _tabItems;
		private TranslationMemoryViewModel _tmViewModel;
		private int _selectedIndex;

		public TranslationMemoryViewModel TmViewModel
		{
			get => _tmViewModel;
			set
			{
				_tmViewModel = value;
				OnPropertyChanged(nameof(TmViewModel));
			}
		}

	
		public TranslationViewModel TranslationViewModel { get; set; }
		public SystemFieldsViewModel SystemFieldsViewModel { get; set; }
		public CustomFieldsViewModel CustomFieldsViewModel { get; set; }
		public MainViewModel()
		{
			_tmViewModel = new TranslationMemoryViewModel();
			TranslationViewModel = new TranslationViewModel(_tmViewModel);
			SystemFieldsViewModel =new SystemFieldsViewModel();
			CustomFieldsViewModel=new CustomFieldsViewModel();
		}

		public int Selected
		{
			get => _selectedIndex;
			set
			{
				if (Equals(value, _selectedIndex))
				{
					return;
				}
				_selectedIndex = value;
				OnPropertyChanged(nameof(Selected));
			}
		}	


		public ObservableCollection<TabItem> TabItems
		{
			get => _tabItems;

			set
			{
				if (Equals(value, _tabItems))
				{
					return;
				}
				_tabItems = value;
				OnPropertyChanged(nameof(TabItems));
			}
		}
	}
}
