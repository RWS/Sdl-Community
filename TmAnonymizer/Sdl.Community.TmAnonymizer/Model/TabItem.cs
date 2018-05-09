using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.TmAnonymizer.ViewModel;

namespace Sdl.Community.TmAnonymizer.Model
{
	public class TabItem
	{
		public string Name { set; get; }
		public string Header { set; get; }
		public ViewModelBase ViewModel { get; set; }
	}
}
