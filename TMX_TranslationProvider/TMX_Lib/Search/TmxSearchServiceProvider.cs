using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Search
{
	public class TmxSearchServiceProvider
	{
		private TmxSearchServiceProvider()
		{
		}
		private static TmxSearchServiceProvider Instance { get; } = new TmxSearchServiceProvider();

		private Dictionary<string, TmxSearchService> _services = new Dictionary<string, TmxSearchService>();

		private static string Key(ISearchServiceParameters args) => $"{args.DbName}-{args.DbConnectionNoPassword}";

		private SearchServiceParameters empty = new SearchServiceParameters();
		// this service basically can't search anything
		public static TmxSearchService EmptySearchService => GetSearchService(Instance.empty);

		public static TmxSearchService GetSearchService(ISearchServiceParameters args)
		{
			var key = Key(args);
			lock (Instance) {
				if (Instance._services.TryGetValue(key, out var service))
					return service;

				var newService = new TmxSearchService(args);
				Instance._services.Add(key, newService);
				return newService;
			}
		}
	}
}
