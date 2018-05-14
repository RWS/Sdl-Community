using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
    public class SearchResult:ModelBase
    {
	    public ObservableCollection<SourceSearchResult> SourceSearchResults { get; set; }
	    public ObservableCollection<TargetSearchResult> TargetSearchResults { get; set; }
	}
}
