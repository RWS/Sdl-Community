using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.StudioCleanupTool.Model
{
	public class LocationDetails
	{
		public string OriginalFilePath { get; set; }
		public string BackupFilePath { get; set; }
		public string Version { get; set; }
		public string Alias { get; set; }
	}
}
