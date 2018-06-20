using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
    public class UniqueUsername:ModelBase
    {
		private bool _isSelected;
		private string _name;
		private string _description;

		public bool IsSelected
		{

			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public string Username
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Username));
			}
		}
		public string Alias
		{
			get => _description;
			set
			{
				_description = value;
				OnPropertyChanged(nameof(Alias));
			}
		}
	}
}
