using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
    public class User:ModelBase
    {
		private bool _isSelected;
		private string _name;
		private string _alias;

		public bool IsSelected
		{

			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public string UserName
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(UserName));
			}
		}
		public string Alias
		{
			get => _alias;
			set
			{
				_alias = value;
				OnPropertyChanged(nameof(Alias));
			}
		}
	}
}
