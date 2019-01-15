namespace IATETerminologyProvider.Model
{
	public class DomainModel : ViewModelBase
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
				OnPropertyChanged();
			}
		}	
	}
}