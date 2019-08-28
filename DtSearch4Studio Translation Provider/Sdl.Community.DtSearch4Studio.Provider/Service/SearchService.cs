using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dtSearch.Engine;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;

namespace Sdl.Community.DtSearch4Studio.Provider.Service
{
	public class SearchService
	{
		public static readonly Log Log = Log.Instance;

		#region Constructors
		public SearchService()
		{
		}
		#endregion

		#region Public Methods
		public void GetResults(string indexPath, string segment)
		{
			using (var searchJob = new SearchJob())
			{
				using (var results = new SearchResults())
				{
					searchJob.Request = segment;
					searchJob.IndexesToSearch.Add(indexPath);
					searchJob.MaxFilesToRetrieve = 100;
					searchJob.SearchFlags = SearchFlags.dtsSearchTypeAnyWords;
					searchJob.Execute(results);

					if (searchJob.Errors.Count > 0)
					{
						for (int i = 0; i <= searchJob.Errors.Count; i++)
						{
							Log.Logger.Error($"{Constants.GetResults}{i}: {searchJob.Errors.Message(i)}");
						}
					}
					else
					{
						// ... display search results ...
					}
				}
			}
		}
		
		#endregion
	}
}