using Sdl.Community.IATETerminologyProvider.Interface;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class DomainModel : ViewModelBase, IModel
	{		
		private bool _isSelected;
	
		public string Code { get; set; }
		public string Name { get; set; }
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
	}
}