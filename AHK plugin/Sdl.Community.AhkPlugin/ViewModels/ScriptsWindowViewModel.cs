using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.ViewModels
{
	public class ScriptsWindowViewModel
	{
		public List<Script> Scripts;
		public ScriptsWindowViewModel()
		{
			var scripts = new List<Script>
			{
				new Script
				{
					Name = "Script1",
					Description = "dasdasdadsa"
				},
				new Script
				{
					Name = "Script2",
					Description = "oodoasdasda"
				}
			};
			Scripts = scripts;
		}
	}
}
