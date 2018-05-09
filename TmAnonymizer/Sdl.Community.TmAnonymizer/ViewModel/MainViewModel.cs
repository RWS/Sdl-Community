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
		private ViewModelBase _currentViewModel;
		private int _selectedIndex;
		public ViewModelBase CurrentViewModel
		{
			get => _currentViewModel;
			set
			{
				_currentViewModel = value;
				OnPropertyChanged(nameof(CurrentViewModel));
			}
		}
		public MainViewModel()
		{
			_tabItems = new ObservableCollection<TabItem>
			{
				new TabItem
				{
					Name = "Translations",
					Header = "Translations",
					ViewModel =  new TranslationViewModel()
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
			//LoadTranslationPage();
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
				if (value.Equals(0))
				{
					LoadTranslationPage();
				}
				if (value.Equals(1))
				{
					SystemFieldsPage();
				}
			}
		}	
		private void LoadTranslationPage()
		{
			CurrentViewModel = new TranslationViewModel();
		}
		private void SystemFieldsPage()
		{
			CurrentViewModel = new SystemFieldsViewModel();
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
