using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AhkPlugin.Interface
{
	public interface IDialogService
	{
		List<string> ShowDialog(string filter);
	}
}
