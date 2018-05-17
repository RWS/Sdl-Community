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
		public MainViewModel()
		{
			_tmViewModel = new TranslationMemoryViewModel();
			
			_tabItems = new ObservableCollection<TabItem>
			{
				new TabItem
				{
					Name = "Translations",
					Header = "Translations",
					ViewModel =  new TranslationViewModel(_tmViewModel.TmsCollection)
				},
				new TabItem
				{
					Name = "System",
					Header = "System fields",
					ViewModel = new SystemFieldsViewModel()
				},
				new TabItem
				{
					Name = "Custom",
					Header = "Custom fields",
					ViewModel = new CustomFieldsViewModel()
				}
				
			};
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
