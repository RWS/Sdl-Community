using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public class Reliabilities : ViewModelBase
	{
		private readonly List<string> _propertyNames;
		private bool _downgradePriorToDeletion;
		private bool _minimumReliability;
		private bool _notVerified;
		private bool _reliable;
		private bool _veryReliable;

		public Reliabilities()
		{
			_propertyNames = GetType().GetProperties().Select(prop => prop.Name).ToList();
			_propertyNames.Remove(nameof(All));

			PropertyChanged += Reliabilities_PropertyChanged;
		}

		public bool All
		{
			get => AreAllChecked();
			set
			{
				SwitchAll(value);
				OnPropertyChanged(nameof(All));
			}
		}

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

		private bool AreAllChecked()
		{
			return DowngradePriorToDeletion && NotVerified && MinimumReliability && Reliable && VeryReliable;
		}

		private void Reliabilities_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_propertyNames.Contains(e.PropertyName))
			{
				OnPropertyChanged(nameof(All));
			}
		}

		private void SwitchAll(bool onOff)
		{
			DowngradePriorToDeletion = onOff;
			NotVerified = onOff;
			MinimumReliability = onOff;
			Reliable = onOff;
			VeryReliable = onOff;
		}
	}
}