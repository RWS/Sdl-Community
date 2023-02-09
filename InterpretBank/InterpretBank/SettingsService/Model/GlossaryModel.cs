using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using InterpretBank.Model;

namespace InterpretBank.SettingsService.Model
{
	public class GlossaryModel : INotifyPropertyChanged
	{
		private ObservableCollection<TagModel> _tags;
		public string GlossaryName { get; set; }
		public int Id { get; set; }
		public bool IsDirty { get; set; } = false;
		public List<Language> Languages { get; set; } = new();

		public ObservableCollection<TagModel> Tags
		{
			get => _tags;
			set => SetField(ref _tags, value);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}