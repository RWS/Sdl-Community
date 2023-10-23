using System;
using LanguageWeaverProvider.ViewModel;

namespace LanguageWeaverProvider.Model
{
	public class PairDictionary : BaseViewModel
	{
		bool _isSelected;

		public string Name { get; set; }
		
		public string DictionaryId { get; set; }

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public string Description { get; set; }
		
		public string Source { get; set; }
		
		public string Target { get; set; }
		
		public int TermCount { get; set; }
		
		public string CreatedBy { get; set; }
		
		public DateTime CreationDate { get; set; }
		
		public string LastModifiedBy { get; set; }
		
		public DateTime LastModifyDate { get; set; }
	}
}