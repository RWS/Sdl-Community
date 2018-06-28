using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class CustomField
	{
		public string Name { get; set; }
		public string ValueType { get; set; }
		public ObservableCollection<Details> Details { get; set; }
	}
}
