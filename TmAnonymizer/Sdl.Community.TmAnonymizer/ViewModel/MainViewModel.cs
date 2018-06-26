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
		private TranslationMemoryViewModel _tmViewModel;

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
			SystemFieldsViewModel =new SystemFieldsViewModel(_tmViewModel);
			CustomFieldsViewModel=new CustomFieldsViewModel();
		}
	}
}
