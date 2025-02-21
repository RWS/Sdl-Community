using System.Collections.Generic;
using System.Linq;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model
{
	public class LinguisticOption : BaseViewModel
	{
		string _selectedValue;

		public string Id { get; set; }

		public string Name { get; set; }

		public string SystemDefault { get; set; }

		public List<string> Values { get; set; }

		public string SelectedValue
		{
			get => _selectedValue ??= Values.First(x => x.Equals(SystemDefault)) ?? Values.First(x => x.Equals(SystemDefault));
			set
			{
				_selectedValue = value;
				OnPropertyChanged();
			}
		}

		[JsonIgnore]
		public string DispayName => Name.Length switch
		{
			>= 15 => $"{Name.Substring(0, 12)}..",
			_ => Name
		};


		public LinguisticOption Clone()
		{
			return MemberwiseClone() as LinguisticOption;
		}
	}
}