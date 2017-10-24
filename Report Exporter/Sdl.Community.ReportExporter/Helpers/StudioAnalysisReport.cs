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

			var _xDoc = XDocument.Load(ReportFile);
			AnalyzedFiles = from f in _xDoc.Root.Descendants("file")
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
						}
				};

			if (!AnalyzedFiles.Any())
			{
				throw new InvalidOperationException("No analyzed files in the report");
			}

		}

		public string ToCsv(bool includeHeader)
		{
			var csvSeparator = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
			var csvNewLine = Environment.NewLine;
			var csvHeader = GetCsvHeaderRow(csvSeparator, AnalyzedFiles.First().Fuzzies);

			StringBuilder _sb = new StringBuilder();

			if (includeHeader)
			{
				_sb.Append(csvHeader).Append(csvNewLine);
			}

			foreach (var file in AnalyzedFiles)
			{
				_sb.Append(file.FileName).Append(csvSeparator)
					.Append(file.Repeated.Words).Append(csvSeparator)
					// the 100% match is actually a sum of Exact, Perfect and InContextExact matches
					.Append((file.Exact.Words + file.Perfect.Words + file.InContextExact.Words).ToString()).Append(csvSeparator);

				foreach (var fuzzy in file.Fuzzies.OrderByDescending(fz => fz.Max))
				{
					_sb.Append(fuzzy.Words).Append(csvSeparator);

					var _internalFuzzy = file.InternalFuzzy(fuzzy.Min, fuzzy.Max);
					_sb.Append(_internalFuzzy != null ? _internalFuzzy.Words : 0).Append(csvSeparator);
				}

				_sb.Append(file.Untranslated.Words).Append(csvSeparator)
					.Append(file.Total.Words).Append(csvSeparator)
					.Append(csvNewLine);
			}

			return _sb.ToString();
		}

		private string GetCsvHeaderRow(string separator, IEnumerable<BandResult> fuzzies)
		{
			List<string> _headerColumns = new List<string>() { "\"Filename\"",
				"\"Reps\"",
				"\"100% (TM)\""
			};

			fuzzies.OrderByDescending(br => br.Max)
				.ToList()
				.ForEach(br =>
					{
						_headerColumns.Add(string.Format("\"{0}% - {1}% (TM)\"", br.Max, br.Min));
						_headerColumns.Add(string.Format("\"{0}% - {1}% (AP)\"", br.Max, br.Min));
					}
				);

			_headerColumns.Add("\"New\"");
			_headerColumns.Add("\"Total Words\"");

			return _headerColumns.Aggregate((res, next) => res + separator + next);
		}
	}
}
