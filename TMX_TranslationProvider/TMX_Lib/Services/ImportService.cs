using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using TMX_Lib.Db;
using TMX_Lib.Search;

namespace TMX_UI.Services
{
	// for now, insanely simple - just track at most one import
	public class ImportService
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();
		private ImportService() { }
		public static ImportService Instance { get; } = new ImportService();

		private bool _isImporting;

		// the database we're importing to
		private TmxMongoDb _db;
		
		// contains the last import report
		private TmxImportReport _importReport  = new TmxImportReport();

		public bool IsImporting() { 
			lock(this)
				return _isImporting;
		}
		public double ImportProgress() {
			lock (this)
				return _db?.ImportProgress() ?? 0;
		}

		public void GetImportReport(TmxImportReport report) {
			lock (this)
				report.CopyFrom(_importReport);
		}

		public async Task ImportAsync(string fileName, string dbName)
		{
			lock (this)
				if (_isImporting)
					return;
				else
					_isImporting = true;

			try
			{
				var db = TmxSearchServiceProvider.GetDatabase(dbName);
				lock (this)
					_db = db;
				await db.InitAsync();
				await db.ImportToDbAsync(fileName, (r) => {
					lock (this)
						_importReport.CopyFrom(r);
				});
			}
			catch (Exception ex)
			{
				log.Fatal($"can't import file {fileName} into db {dbName} : {ex.Message}");
			}
			finally {
				lock (this) {
					_isImporting = false;
					_db = null;
				}
			}
		}
	}
}
