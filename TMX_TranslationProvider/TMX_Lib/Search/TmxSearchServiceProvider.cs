using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using TMX_Lib.Db;

namespace TMX_Lib.Search
{
	public class TmxSearchServiceProvider
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		private TmxSearchServiceProvider()
		{
		}
		private static TmxSearchServiceProvider Instance { get; } = new TmxSearchServiceProvider();

		private Dictionary<string, TmxMongoDb> _databases = new Dictionary<string, TmxMongoDb>();

		private Dictionary<string, TmxSearchService> _services = new Dictionary<string, TmxSearchService>();

		private static string Key(IReadOnlyList<string> databaseNames) => string.Join(",",databaseNames);

		public static TmxMongoDb GetDatabase(string dbName) {
			lock (Instance) {
				if (Instance._databases.TryGetValue(dbName, out var db))
					return db;
			}
			var newDb = new TmxMongoDb(dbName);
			lock (Instance) {
				if (!Instance._databases.ContainsKey(dbName))
					Instance._databases.Add(dbName, newDb);
				else 
					newDb = Instance._databases[dbName];	
			}
			Task.Run(async () => await InitDatabasesAsync( new[] { newDb }));
			return newDb;
		}

		public static TmxSearchService GetSearchService(IReadOnlyList<string> databaseNames)
		{
			var key = Key(databaseNames);
			lock (Instance)
			{
				if (Instance._services.TryGetValue(key, out var service))
					return service;
			}

			List<TmxMongoDb> newDatabases = new List<TmxMongoDb>();
			lock (Instance) {
				foreach (var name in databaseNames)
					if (!Instance._databases.ContainsKey(name))
						try
						{
							var db = new TmxMongoDb(name);
							Instance._databases.Add(name, db);
							newDatabases.Add(db);
						}
						catch (Exception e) { 
							log.Fatal($"can't create database {name} : {e.Message}");
							// basically, mark this as a failed connection
							Instance._databases.Add(name, null);
						}
			}

			if (newDatabases.Count > 0)
				Task.Run(async () => await InitDatabasesAsync(newDatabases));

			lock (Instance){
				var databases = databaseNames.Select(n => Instance._databases[n]).Where(db => db != null).ToList();
				var newService = new TmxSearchService(databases);
				Instance._services.Add(key, newService);
				return newService;
			}
		}

		private static async Task InitDatabasesAsync(IReadOnlyList<TmxMongoDb> dbs) {
			foreach (var db in dbs)
			{
				try
				{
					await db.InitAsync();
				}
				catch (Exception e)
				{
					log.Fatal($"can't initialize database {db.Name} : {e.Message}");
				}
			}
		}
	}
}
