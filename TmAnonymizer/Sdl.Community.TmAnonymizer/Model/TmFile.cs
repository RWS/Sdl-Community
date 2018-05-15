using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class TmFile :ModelBase
	{
		private bool _isSelected;
		private string _name;
		private string _path;
		private bool _shouldRemove;

		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}
		public string Path
		{
			get => _path;
			set
			{
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

		public bool IsSelected
		{

			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}
		public bool ShouldRemove
		{

			get => _shouldRemove;
			set
			{
				_shouldRemove = value;
				OnPropertyChanged(nameof(ShouldRemove));
			}
		}
	}
}
