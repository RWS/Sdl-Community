using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class Primarities : ViewModelBase
	{
		private bool _notPrimary;
		private bool _primary;

		public bool All
		{
			get => Primary && NotPrimary;
			set
			{
				Primary = value;
				NotPrimary = value;
			} 
		}

		public bool NotPrimary
		{
			get => _notPrimary;
			set
			{
				_notPrimary = value;
				OnPropertyChanged(nameof(NotPrimary));
				OnPropertyChanged(nameof(All));
			}
		}

		public bool Primary
		{
			get => _primary;
			set
			{
				_primary = value;
				OnPropertyChanged(nameof(Primary));
				OnPropertyChanged(nameof(All));
			}
		}

		public List<int> GetPrimarities()
		{
			var codes = new List<int>();

			if (NotPrimary)
				codes.Add(0);
			if (Primary)
				codes.Add(1);

			return codes;
		}
	}
}