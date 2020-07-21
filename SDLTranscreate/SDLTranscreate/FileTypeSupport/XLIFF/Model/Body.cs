using System.Collections.Generic;

namespace Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Body
	{
		public Body()
		{
			TransUnits = new List<TransUnit>();
		}

		public List<TransUnit> TransUnits { get; set; }

	}
}
