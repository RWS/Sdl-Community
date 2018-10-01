using System;
using System.Collections.ObjectModel;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.SdlTmAnonymizer.Model
{
	public class CustomField : ModelBase
	{
		private bool _isSelected;
		public Guid Id { get; set; }
		public string Name { get; set; }
		public FieldValueType ValueType { get; set; }
		public bool IsPickList { get; set; }
		public ObservableCollection<CustomFieldValue> FieldValues { get; set; }
		public string TmPath { get; set; }
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
