using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SdlTmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class CustomField
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ValueType { get; set; }
		public bool IsPickList { get; set; }
		public ObservableCollection<Details> Details { get; set; }
	}
}
