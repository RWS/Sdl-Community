using System.Collections.ObjectModel;
using Sdl.Community.ApplyTMTemplate.Models;

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