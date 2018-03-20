using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.Plugins.AdvancedDisplayFilter.DisplayFilters;
using Sdl.Community.Toolkit.Integration.DisplayFilter;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Models
{
	public class SavedSettings
	{
		public CustomFilterSettings CustomFilterSettings { get; set; }
		public DisplayFilterSettings DisplayFilterSettings { get; set; }
	}
}
