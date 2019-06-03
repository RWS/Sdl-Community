using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class Organization : BaseModel
	{
		private string _name;
		private string _path;

		public string Name
		{
			get => _name;

			set
			{
				if (_name == value)
				{
					return;
				}
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Path
		{
			get => _path;
			set
			{
				if (_path == value)
				{
					return;
				}
				_path = value;
				OnPropertyChanged(nameof(Path));
			}
		}

	}
}
