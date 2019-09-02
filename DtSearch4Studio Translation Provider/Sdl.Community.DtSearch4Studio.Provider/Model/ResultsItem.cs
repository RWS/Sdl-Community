using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dtSearch.Engine;

namespace Sdl.Community.DtSearch4Studio.Provider.Model
{
	public class ResultsItem
	{		 
        public string Name { set; get; }
		public string Location { set; get; }
		public string FullName { set; get; }
		public string Hits { set; get; }
		public string Date { set; get; }
		public string Detail { set; get; }
		public int OrdinalInSearchResults { set; get; }

		public ResultsItem(SearchResultsItem item)
		{
			MakeFromItem(item);
		}

		private void MakeFromItem(SearchResultsItem item)
		{
			Name = item.ShortName;
			FullName = item.Filename;
			Location = item.Location;
			Date = item.Modified.ToString();
			Hits = item.HitCount.ToString();
			Detail = Hits + " hits " + Date;
		}
	}
}