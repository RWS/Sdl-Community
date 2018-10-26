using System;
using System.Collections.Generic;
using System.IO;
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
		public List<CustomFieldValue> FieldValues { get; set; }
		public string TmPath { get; set; }
		public string TmName => TmPath != null ? Path.GetFileName(TmPath) : string.Empty;
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
