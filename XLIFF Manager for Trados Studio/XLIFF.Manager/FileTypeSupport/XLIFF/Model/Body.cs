using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model
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
