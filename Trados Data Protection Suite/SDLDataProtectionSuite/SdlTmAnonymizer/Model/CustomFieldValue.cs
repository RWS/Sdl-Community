namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model
{
	public class CustomFieldValue: ModelBase
	{
		private bool _isSelected;
		private string _newValue;

		public int? FieldId { get; set; }
		public int? ValueId { get; set; }

		public string Value { get; set; }
		public string NewValue
		{
			get => _newValue;
			set
			{
				_newValue = value;
				OnPropertyChanged(nameof(NewValue));
			}
		}
		
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
