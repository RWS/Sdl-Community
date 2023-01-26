using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_UI.Services
{
	// for now, insanely simple - just track at most one import
	internal class ImportService
	{
		private ImportService() { }
		public static ImportService Instance { get; } = new ImportService();

		public bool IsImporting() => false;
		public double ImportProgress() => 0.1;
	}
}
