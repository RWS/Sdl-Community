using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class CustomFieldsViewModel : ViewModelBase
	{
		private static TranslationMemoryViewModel _translationMemoryViewModel;
		private ObservableCollection<CustomField> _customFields;

		public CustomFieldsViewModel(TranslationMemoryViewModel translationMemoryViewModel)
		{
			_translationMemoryViewModel = translationMemoryViewModel;
			_customFields = new ObservableCollection<CustomField>
			{
				new CustomField
				{
					Name = "Client",
					Type = "List",
					Details = new List<Details>
					{
						new Details
						{
							Value = "Pedigree",
							NewValue = ""
						},
						new Details
						{
							Value = "Another value",
							NewValue = ""
						}
					}
				},
				new CustomField
				{
					Name = "Client1",
					Type = "List",
					Details = new List<Details>
					{
						new Details
						{
							Value = "Pedigree",
							NewValue = ""
						},
						new Details
						{
							Value = "Another value",
							NewValue = ""
						},
						new Details
						{
							Value = "Another value",
							NewValue = ""
						}
					}
				}
			};
		}

		public ObservableCollection<CustomField> CustomFieldsCollection
		{
			get => _customFields;

			set
			{
				if (Equals(value, _customFields))
				{
					return;
				}
				_customFields = value;
				OnPropertyChanged(nameof(CustomFieldsCollection));

			}
		}
	}
}



