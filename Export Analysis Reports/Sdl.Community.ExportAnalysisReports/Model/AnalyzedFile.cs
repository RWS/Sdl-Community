using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.ExportAnalysisReports.Model
{
	public class AnalyzedFile
	{
		public string FileName { get; set; }
		public Guid Guid { get; set; }
		public IEnumerable<BandResult> Results { get; set; }

		public AnalyzedFile(string fileName, string guid)
		{
			FileName = fileName;
			Guid = new Guid(guid);
		}

		public BandResult Total
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "total"); }
		}

		public BandResult Untranslated
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "new"); }
		}

		public BandResult Repeated
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "repeated"); }
		}

		public BandResult Perfect
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "perfect"); }
		}

		public BandResult InContextExact
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "incontextexact"); }
		}

		public BandResult Exact
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "exact"); }
		}

		public IEnumerable<BandResult> Fuzzies
		{
			get { return Results.Where(r => r.BandName.ToLower() == "fuzzy"); }
		}
		public BandResult NewBaseline
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "newbaseline"); }
		}
		public BandResult Locked
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "locked"); }
		}
		public BandResult CrossRep
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "crossfilerepeated"); }
		}

		public BandResult NewLearnings
		{
			get { return Results.SingleOrDefault(r => r.BandName.ToLower() == "newlearnings"); }
		}
		public IEnumerable<BandResult> InternalFuzzies
		{
			get { return Results.Where(r => r.BandName.ToLower() == "internalfuzzy"); }
		}

		public BandResult Fuzzy(int min, int max)
		{
			return Results.SingleOrDefault(r => r.BandName.ToLower() == "fuzzy" && r.Min == min && r.Max == max);
		}

		public BandResult InternalFuzzy(int min, int max)
		{
			return Results.SingleOrDefault(r => r.BandName.ToLower() == "internalfuzzy" && r.Min == min && r.Max == max);
		}	
	}
}