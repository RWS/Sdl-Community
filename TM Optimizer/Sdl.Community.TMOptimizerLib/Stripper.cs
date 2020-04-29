using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Sdl.Community.TMOptimizerLib
{
	/// <summary>
	/// Removes Workbench based TUs from Studio TM
	/// </summary>
	public class Stripper : ProcessorBase
	{
		private readonly TmxFile _inputFile;
		private readonly Settings _settings;
		private readonly TmxFile _outputFile;

		public Stripper(TmxFile inputFile, TmxFile outputFile, Settings settings)
		{
			_inputFile = inputFile;
			_outputFile = outputFile;
			_settings = settings;
		}

		public int TusRead
		{
			get;
			set;
		}

		public int TusStripped
		{
			get;
			set;
		}

		public void Execute()
		{
			var tusNode = from el in ReadFromTmxFile(_inputFile.FilePath) select el;

			var outputWriter = new OutputWriter(_outputFile.FilePath);
			outputWriter.InitializeStudioTmx(_inputFile.DetectInfo.SourceLanguage.IsoAbbreviation);

			int tuIndex = 0;
			int tuCount = 0;
			foreach (var tu in tusNode)
			{
				var format = from propElement in tu.Elements("prop")
							 where propElement.Attribute("type").Value == "x-OriginalFormat" && propElement.Value == "TradosTranslatorsWorkbench"
							 select propElement.Parent;

				if (!format.Any())
				{
					outputWriter.Write(tu.ToString());
					tuCount++;
				}
				else
				{
					TusStripped++;
				}

				TusRead++;
				ReportProgress((int)(100.0 * tuIndex / _inputFile.GetDetectInfo().TuCount));
			}

			outputWriter.Complete();

			// output file has same properties as input file
			_outputFile.DetectInfo = _inputFile.GetDetectInfo().Clone();
			_outputFile.DetectInfo.TuCount = tuCount;
			ReportProgress(100);
		}


		/// <summary>
		/// Main function to read the input file in streamed manner
		/// </summary>
		/// <param name="fileLocation"></param>
		/// <returns></returns>
		static IEnumerable<XElement> ReadFromTmxFile(string fileLocation)
		{
			var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };

			using (var reader = XmlReader.Create(fileLocation, settings))
			{
				reader.MoveToContent();
				while (reader.Read())
				{
					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							if (reader.Name == "tu")
							{
								var el = XNode.ReadFrom(reader) as XElement;
								if (el != null)
									yield return el;
							}
							break;
					}
				}
			}
		}
	}
}