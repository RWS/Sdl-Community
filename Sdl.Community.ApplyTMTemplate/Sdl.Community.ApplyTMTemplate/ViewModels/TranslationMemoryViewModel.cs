using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ApplyTMTemplate.Models;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.ViewModels
{
	public class TranslationMemoryViewModel : ModelBase
	{
		private ObservableCollection<TranslationMemory> _tmCollection;

		public TranslationMemoryViewModel(ObservableCollection<TranslationMemory> tmCollection)
		{
			_tmCollection = tmCollection;
		}

		public ObservableCollection<TranslationMemory> TmCollection
		{
			get => _tmCollection;
			set
			{
				_tmCollection = value;
				OnPropertyChanged();
			}
		}
	}
}
