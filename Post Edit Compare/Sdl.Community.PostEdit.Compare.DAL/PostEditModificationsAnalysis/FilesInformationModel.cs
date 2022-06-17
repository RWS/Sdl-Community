using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis
{
	public class FilesInformationModel
	{
		//public string Version { get; set; }
		//public string Language { get; set; }
		//public string FilePath { get; set; }
		public Tuple<string, string, string> FilesInfo { get; set; }
	}
}
