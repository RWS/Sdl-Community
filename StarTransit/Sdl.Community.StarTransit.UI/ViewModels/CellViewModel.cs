using System;
using System.Collections.Generic;
using System.Windows.Input;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.UI.Commands;

namespace Sdl.Community.StarTransit.UI.ViewModels
{
	public class CellViewModel : BaseViewModel
	{
		private string _name;
		private bool _isChecked;
		private Guid _id;
		private ICommand _selectCommand;
		private static List<Guid> _selectedProjectIds;

		public CellViewModel()
		{
			_selectedProjectIds = new List<Guid>();
			_name = string.Empty;
			_isChecked = false;
			_id = new Guid();
		}

		public string Name
		{
			get => _name;
			set
			{
				if (Equals(value, _name))
				{
					return;
				}
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public bool Checked
		{
			get => _isChecked;
			set
			{
				if (Equals(_isChecked, value))
				{
					return;
				}
				_isChecked = value;
				OnPropertyChanged(nameof(Checked));
			}
		}

		public Guid Id
		{
			get => _id;
			set
			{
				if (Equals(_id, value)) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		public ICommand SelectCommand => _selectCommand ?? (_selectCommand = new CommandHandler(Select, true));

		private void Select()
		{
			if (Checked)
			{
				if (!_selectedProjectIds.Contains(Id))
				{
					_selectedProjectIds.Add(Id);
				}
			}
			else
			{
				_selectedProjectIds.Remove(Id);
			}
		}

		public static List<Guid> ReturnSelectedProjectIds()
		{
			return _selectedProjectIds;
		}

		public void ClearSelectedProjectsList()
		{
			_selectedProjectIds.Clear();
		}
	}
}