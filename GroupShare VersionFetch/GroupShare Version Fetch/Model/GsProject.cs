using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class GsProject:BaseModel
	{
		private string _name;
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			} }
		public bool IsSelected { get; set; }

	}
}
