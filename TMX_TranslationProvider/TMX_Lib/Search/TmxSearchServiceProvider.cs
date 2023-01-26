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

		private static string Key(string name) => name;

		public static TmxSearchService GetSearchService(string name)
		{
			var key = Key(name);
			lock (Instance) {
				if (Instance._services.TryGetValue(key, out var service))
					return service;

				var newService = new TmxSearchService(name);
				Instance._services.Add(key, newService);
				return newService;
			}
		}
	}
}
