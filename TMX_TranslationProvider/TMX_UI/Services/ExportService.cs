using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_UI.Services
{
	// for now, insanely simple - just track at most one export
	internal class ExportService
	{
		private ExportService() { }
		public static ExportService Instance { get; } = new ExportService();

		public bool IsExporting() => false;
		public double ExportProgress() => 0.2;
	}
}
