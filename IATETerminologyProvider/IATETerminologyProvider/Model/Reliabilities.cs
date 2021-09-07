using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class Reliabilities : ViewModelBase
	{
		private bool _downgradePriorToDeletion;
		private bool _minimumReliability;
		private bool _notVerified;
		private bool _reliable;
		private bool _veryReliable;

		public bool DowngradePriorToDeletion
		{
			get => _downgradePriorToDeletion;
			set
			{
				_downgradePriorToDeletion = value;
				OnPropertyChanged(nameof(DowngradePriorToDeletion));
			}
		}

		public bool MinimumReliability
		{
			get => _minimumReliability;
			set
			{
				_minimumReliability = value;
				OnPropertyChanged(nameof(MinimumReliability));
			}
		}

		public bool NotVerified
		{
			get => _notVerified;
			set
			{
				_notVerified = value;
				OnPropertyChanged(nameof(NotVerified));
			}
		}

		public bool Reliable
		{
			get => _reliable;
			set
			{
				_reliable = value;
				OnPropertyChanged(nameof(Reliable));
			}
		}

		public bool VeryReliable
		{
			get => _veryReliable;
			set
			{
				_veryReliable = value;
				OnPropertyChanged(nameof(VeryReliable));
			}
		}

		public List<int> GetReliabilityCodes()
		{
			var codes = new List<int>();

			if (DowngradePriorToDeletion)
				codes.Add(0);
			if (NotVerified)
				codes.Add(1);
			if (MinimumReliability)
				codes.Add(2);
			if (Reliable)
				codes.Add(3);
			if (VeryReliable)
				codes.Add(4);

			return codes;
		}
	}
}