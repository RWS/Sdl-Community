using System;
using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Header: ICloneable
	{
		public Header()
		{
			Skl = new Skl();
			PhaseGroup = new PhaseGroup();
			Contexts = new List<Context>();
		}

		public Skl Skl { get; set; }

		public PhaseGroup PhaseGroup { get; set; }

		public List<Context> Contexts { get; set; }

		public object Clone()
		{
			var header = new Header();
			foreach (var context in Contexts)
			{
				header.Contexts.Add(context.Clone() as Context);
			}

			return header;
		}
	}
}
