using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TMX_Lib.Search;

namespace TMX_UI.Services
{
	// for now, insanely simple - just track at most one export
	public class ExportService
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		private ExportService() { }
		public static ExportService Instance { get; } = new ExportService();

		private bool _isExporting = false;
		private double _exportProgress = 0;

		public bool IsExporting() {
			lock (this)
				return _isExporting;
		}
		public double ExportProgress() { 
			lock(this)
				return _exportProgress;
		}

		public async Task ExportAsync(string dbName, string fileName) {
			lock (this)
				if (_isExporting)
					return;
				else
					_isExporting = true;

			try
			{
				var db = TmxSearchServiceProvider.GetDatabase(dbName);
				await db.InitAsync();
				await db.ExportToFileAsync(fileName, (d) => {
					lock (this)
						_exportProgress = d;
					return true;
				});
			}
			catch (Exception ex)
			{
				log.Fatal($"can't export db {dbName} into file {fileName} : {ex.Message}");
			}
			finally
			{
				lock (this)
				{
					_isExporting = false;
				}
			}

		}
	}
}
