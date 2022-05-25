using System;
using System.Collections.Generic;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class Body: ICloneable
	{
		public Body()
		{
			TransUnits = new List<TransUnit>();
		}

		public List<TransUnit> TransUnits { get; set; }

		public object Clone()
		{
			var body = new Body();
			foreach (var transUnit in TransUnits)
			{
				body.TransUnits.Add(transUnit.Clone() as TransUnit);
			}

			return body;
		}
	}
}
