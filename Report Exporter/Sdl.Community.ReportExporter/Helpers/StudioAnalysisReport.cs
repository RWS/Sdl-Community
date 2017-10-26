using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sdl.Community.ReportExporter.Model;

namespace Sdl.Community.ReportExporter.Helpers
{
	public class StudioAnalysisReport
	{
		public string ReportFile { get; }
		public IEnumerable<AnalyzedFile> AnalyzedFiles { get;  }

		public StudioAnalysisReport(string pathToXmlReport)
		{
			ReportFile = pathToXmlReport;

			if (!File.Exists(ReportFile))
			{
				throw new FileNotFoundException("Analysis report not found", ReportFile);
			}

			var xDoc = XDocument.Load(ReportFile);
			AnalyzedFiles = from f in xDoc.Root.Descendants("file")
				select new AnalyzedFile(f.Attribute("name").Value, f.Attribute("guid").Value)
				{
					Results = from r in f.Element("analyse").Elements()
						select new BandResult(r.Name.LocalName)
						{
							Segments = r.Attribute("segments") != null ? int.Parse(r.Attribute("segments").Value) : 0,
							Words = r.Attribute("words") != null ? int.Parse(r.Attribute("words").Value) : 0,
							Characters = r.Attribute("characters") != null ? int.Parse(r.Attribute("characters").Value) : 0,
							Placeables = r.Attribute("placeables") != null ? int.Parse(r.Attribute("placeables").Value) : 0,
							Tags = r.Attribute("tags") != null ? int.Parse(r.Attribute("tags").Value) : 0,
							Min = r.Attribute("min") != null ? int.Parse(r.Attribute("min").Value) : 0,
							Max = r.Attribute("max") != null ? int.Parse(r.Attribute("max").Value) : 0,
							PartialRecallWords = r.Attribute("partialRecallWords")!= null?int.Parse(r.Attribute("partialRecallWords").Value) : 0,
							FullRecallWords = r.Attribute("fullRecallWords") != null ? int.Parse(r.Attribute("fullRecallWords").Value) : 0,
						}
				};

			if (!AnalyzedFiles.Any())
			{
				throw new InvalidOperationException("No analyzed files in the report");
			}

		}

		public string ToCsv(bool includeHeader, OptionalInformation aditionalHeaders)
		{
			var csvSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
			var csvNewLine = Environment.NewLine;
			var csvHeader = GetCsvHeaderRow(csvSeparator, AnalyzedFiles.First().Fuzzies, aditionalHeaders);

			var sb = new StringBuilder();

			if (includeHeader)
			{
				sb.Append(csvHeader).Append(csvNewLine);
			}

			foreach (var file in AnalyzedFiles)
			{
				sb.Append(file.FileName).Append(csvSeparator)
					.Append(file.Repeated.Words).Append(csvSeparator)
					// the 100% match is actually a sum of Exact, Perfect and InContextExact matches
					.Append((file.Exact.Words + file.Perfect.Words + file.InContextExact.Words).ToString()).Append(csvSeparator);

				foreach (var fuzzy in file.Fuzzies.OrderByDescending(fz => fz.Max))
				{
					sb.Append(fuzzy.Words).Append(csvSeparator);

					var internalFuzzy = file.InternalFuzzy(fuzzy.Min, fuzzy.Max);
					sb.Append(internalFuzzy != null ? internalFuzzy.Words : 0).Append(csvSeparator);
				}

				if (aditionalHeaders.IncludeAdaptiveBaseline)
				{
					sb.Append(file.NewBaseline.FullRecallWords).Append(csvSeparator)
						.Append(file.NewBaseline.PartialRecallWords).Append(csvSeparator);
				}
				if (aditionalHeaders.IncludeAdaptiveLearnings)
				{
					sb.Append(file.NewLearnings.FullRecallWords).Append(csvSeparator)
						.Append(file.NewLearnings.PartialRecallWords).Append(csvSeparator);
				}
				if (aditionalHeaders.IncludeInternalFuzzies)
				{
					foreach (var fuzzy in file.Fuzzies.OrderByDescending(fz => fz.Max))
					{
						var internalFuzzy = file.InternalFuzzy(fuzzy.Min, fuzzy.Max);
						sb.Append(internalFuzzy != null ? internalFuzzy.FullRecallWords : 0).Append(csvSeparator);
					}
					foreach (var fuzzy in file.Fuzzies.OrderByDescending(fz => fz.Max))
					{
						var internalFuzzy = file.InternalFuzzy(fuzzy.Min, fuzzy.Max);
						sb.Append(internalFuzzy != null ? internalFuzzy.PartialRecallWords : 0).Append(csvSeparator);
					}
				}

				sb.Append(file.Untranslated.Words).Append(csvSeparator)
					.Append(file.Total.Words).Append(csvSeparator)
					.Append(csvNewLine);
			}

			return sb.ToString();
		}

		private string GetCsvHeaderRow(string separator, IEnumerable<BandResult> fuzzies, OptionalInformation aditionalHeaders)
		{
			var headerColumns = new List<string> { "\"Filename\"",
				"\"Reps\"",
				"\"100% (TM)\""
			};

			fuzzies.OrderByDescending(br => br.Max)
				.ToList()
				.ForEach(br =>
					{
						headerColumns.Add(string.Format("\"{0}% - {1}% (TM)\"", br.Max, br.Min));
						headerColumns.Add(string.Format("\"{0}% - {1}% (AP)\"", br.Max, br.Min));
					}
				);
			if (aditionalHeaders.IncludeAdaptiveBaseline)
			{
				headerColumns.Add("\"AdaptiveMT Baseline (whole TU)\"");
				headerColumns.Add("\"AdaptiveMT Baseline (TU fragment)\"");
			}
			if (aditionalHeaders.IncludeAdaptiveLearnings)
			{
				headerColumns.Add("\"AdaptiveMT with Learnings (whole TU)\"");
				headerColumns.Add("\"AdaptiveMT with Learnings (TU fragment)\"");
			}

			if (aditionalHeaders.IncludeInternalFuzzies)
			{
				fuzzies.OrderByDescending(br => br.Max)
					.ToList()
					.ForEach(br =>
						{
							headerColumns.Add(string.Format("\"{0}% - {1}% (I.F Whole TU)\"", br.Max, br.Min));
							
						}
					);
				fuzzies.OrderByDescending(br => br.Max)
					.ToList()
					.ForEach(br =>
						{
							headerColumns.Add(string.Format("\"{0}% - {1}% (I.F TU Fragment)\"", br.Max, br.Min));
						}
					);
			}

			headerColumns.Add("\"New\"");
			headerColumns.Add("\"Total Words\"");

			return headerColumns.Aggregate((res, next) => res + separator + next);
		}
	}
}
