namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class TermTypeModel : ViewModelBase
    {
		private bool _isSelected;
		public int Code { get; set; }
		public string Name { get; set; }
		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}
	}
}