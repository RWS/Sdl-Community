namespace IATETerminologyProvider.Model
{
	public class TermTypeModel : ViewModelBase
    {
		#region Private Fields
		private bool _isSelected;
		#endregion

		#region Public Properties
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
		#endregion
	}
}