using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.TmAnonymizer.Model
{
    public class TargetSearchResult:ModelBase
    {
	    public string Id { get; set; }
	    public string TmSegmentId { get; set; }
	    public string SegmentNumber { get; set; }
	    public string TargetText { get; set; }
	    public string TmFilePath { get; set; }
	    public object Document { get; set; }
	    public MatchResult MatchResult { get; set; }
	}
}
