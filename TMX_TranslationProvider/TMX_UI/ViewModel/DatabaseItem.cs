using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMX_UI.Properties;

namespace TMX_UI.ViewModel
{
	public class DatabaseItem : INotifyPropertyChanged
	{
		private bool _isSelected;
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}
		public string Name { get; set; } = "";
		public IReadOnlyList<string> Languages { get; set; } = new List<string>();

		public string LanguagesText => string.Join(", ", Languages);

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
