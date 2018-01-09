using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StudioCleanupTool.Helpers
{
   public class DescriptionChangedEventArgs : EventArgs
   {
	   public DescriptionChangedEventArgs(string description)
	   {
		   Description = description;
	   }

	   public string Description { get; }
   }
}
