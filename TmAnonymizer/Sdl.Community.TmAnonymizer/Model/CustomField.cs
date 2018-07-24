using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class CustomField:ModelBase
	{
		private bool _isSelected;
		public Guid Id { get; set; }
		public string Name { get; set; }
		public FieldValueType ValueType { get; set; }
		public bool IsPickList { get; set; }
		public ObservableCollection<Details> Details { get; set; }
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
