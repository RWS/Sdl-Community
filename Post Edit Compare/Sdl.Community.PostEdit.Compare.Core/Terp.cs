using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Compare.Core.SDLXLIFF;

namespace Sdl.Community.PostEdit.Compare.Core
{
	public class TERp
	{

		static TERp()
		{
			JavaPath = string.Empty;
			TerpPath = string.Empty;
		}


		public static List<DocumentResult> Process(string reportFilePath,
			Dictionary
				<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
				fileComparisonFileParagraphUnits, string javaPath)
		{
			JavaPath = javaPath;
			return Process(reportFilePath, fileComparisonFileParagraphUnits);
		}

		public static List<DocumentResult> Process(string reportFilePath,
			Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
				fileComparisonFileParagraphUnits)
		{
			return WriteXmlFiles(reportFilePath, fileComparisonFileParagraphUnits);
		}


		private static string WriteObjFile(string workingDir)
		{

			const string terpJar = "Sdl.Community.PostEdit.Compare.Core.Files.objout.jar";
			var terpJarOut = Path.Combine(workingDir, terpJar);

			var asb = Assembly.GetExecutingAssembly();

			if (File.Exists(terpJarOut))
				File.Delete(terpJarOut);

			using (var inputStream = asb.GetManifestResourceStream(terpJar))
			{
				if (inputStream == null)
					return terpJarOut;

				Stream outputStream = File.Open(terpJarOut, FileMode.Create);

				var bsInput = new BufferedStream(inputStream);
				var bsOutput = new BufferedStream(outputStream);

				var buffer = new byte[1024];
				int bytesRead;

				while ((bytesRead = bsInput.Read(buffer, 0, 1024)) > 0)
				{
					bsOutput.Write(buffer, 0, bytesRead);
				}

				bsInput.Flush();
				bsOutput.Flush();
				bsInput.Close();
				bsOutput.Close();
			}

			return terpJarOut;

		}

		private static List<TERpIdMapping> _idMappings { get; set; }

		private static List<DocumentResult> WriteXmlFiles(string reportFilePath
			, Dictionary<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
				fileComparisonFileParagraphUnits)
		{

			_idMappings = new List<TERpIdMapping>();

			var workingDir = Path.GetDirectoryName(reportFilePath);

			TerpPath = WriteObjFile(workingDir);

			var results = new List<DocumentResult>();

			foreach (var fileComparisonFileParagraphUnit in fileComparisonFileParagraphUnits)
			{
				if (fileComparisonFileParagraphUnit.Value == null)
					continue;

				var fileUnitProperties = fileComparisonFileParagraphUnit.Key;
				var language = fileUnitProperties.TargetLanguageIdUpdated;
				var filePath = Path.GetFileName(fileUnitProperties.FilePathUpdated);

				if (workingDir == null)
					continue;

				var fileRef = Path.Combine(workingDir, filePath + "." + language + ".terp.ref.xml");
				var filehyp = Path.Combine(workingDir, filePath + "." + language + ".terp.hyp.xml");
				var filePrefix = Path.Combine(workingDir, filePath + "." + language + ".out");

				WriteXmlDocument(fileRef, fileComparisonFileParagraphUnit, true);
				var segmentDatas = WriteXmlDocument(filehyp, fileComparisonFileParagraphUnit, false);

				if (segmentDatas.Count > 0)
				{
					var info = new ProcessStartInfo("\"" + JavaPath + "\"")
					{
						Arguments = " -jar \"" + TerpPath + "\" -r \"" + filehyp + "\" -h \"" + fileRef + "\" -n \"" + filePrefix + "\" -o sum,pra,nist,html,param",
						WindowStyle = ProcessWindowStyle.Hidden
					};
					var process = System.Diagnostics.Process.Start(info);
					if (process != null)
						process.WaitForExit();

					if (!File.Exists(filePrefix + ".sum"))
						continue;

					string sumContent;
					using (var reader = new StreamReader(filePrefix + ".sum"))
					{
						sumContent = reader.ReadToEnd();
						reader.Close();
					}

					var regexLines = new Regex("^(?<x1>.*|)$", RegexOptions.Multiline);
					var regexNameAndSegId = new Regex(@"\[(?<x1>.*|)\]\[(?<x2>.*|)\]", RegexOptions.IgnoreCase);
					var mcLines = regexLines.Matches(sumContent);
					if (mcLines.Count > 0)
					{
						foreach (Match mLine in mcLines)
						{
							var strLine = mLine.Value;

							var columns = strLine.Split('|');
							if (columns.Count() != 12)
								continue;

							var mcNameAndSegId = regexNameAndSegId.Match(columns[0]);
							if (!mcNameAndSegId.Success)
								continue;

							var name = mcNameAndSegId.Groups["x1"].Value;
							var id = mcNameAndSegId.Groups["x2"].Value.TrimStart('0');

							if (string.IsNullOrEmpty(id))
								continue;

							var segId = _idMappings.FirstOrDefault(s => s.Id.ToString() == id);
							if (segId == null)
								continue;

							var segmentData = segmentDatas.SingleOrDefault(a => a.SegmentId == segId.SegmentId && a.FileName == name);
							if (segmentData == null)
								continue;

							segmentData.Ins = ReportUtils.GetDecimal(columns[1]);
							segmentData.Del = ReportUtils.GetDecimal(columns[2]);
							segmentData.Sub = ReportUtils.GetDecimal(columns[3]);
							segmentData.Stem = ReportUtils.GetDecimal(columns[4]);
							segmentData.Syn = ReportUtils.GetDecimal(columns[5]);
							segmentData.Phrase = ReportUtils.GetDecimal(columns[6]);
							segmentData.Shft = ReportUtils.GetDecimal(columns[7]);
							segmentData.Wdsh = ReportUtils.GetDecimal(columns[8]);
							segmentData.NumEr = ReportUtils.GetDecimal(columns[9]);
							segmentData.NumWd = ReportUtils.GetDecimal(columns[10]);
							segmentData.Terp = ReportUtils.GetDecimal(columns[11]);
						}
					}

					var result = new DocumentResult
					{
						OriginalDocumentPath = fileUnitProperties.FilePathOriginal,
						UpdatedDocumentPath = fileUnitProperties.FilePathUpdated,
						HtmlPath = filePrefix + ".html",
						SegmentDatas = segmentDatas
					};
					results.Add(result);

					File.Delete(filePrefix + ".sum");
				}


				File.Delete(fileRef);
				File.Delete(filehyp);
			}

			if (File.Exists(TerpPath))
				File.Delete(TerpPath);

			return results;

		}


		private static List<SegmentData> WriteXmlDocument(string filePath, KeyValuePair<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
				fileComparisonFileParagraphUnit, bool isRef)
		{
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = false,
				Indent = false,
				Encoding = Encoding.UTF8
			};


			var xmlTxtWriter = XmlWriter.Create(new XmlTextWriter(filePath, new UTF8Encoding(true)), settings);
			xmlTxtWriter.WriteStartDocument();
			xmlTxtWriter.WriteStartElement("refset");
			xmlTxtWriter.WriteAttributeString("trglang", fileComparisonFileParagraphUnit.Key.TargetLanguageIdUpdated);
			if (fileComparisonFileParagraphUnit.Key.FilePathUpdated != null)
				xmlTxtWriter.WriteAttributeString("setid", Path.GetFileName(fileComparisonFileParagraphUnit.Key.FilePathUpdated));
			xmlTxtWriter.WriteAttributeString("srclang", "any");

			var segmentDatas = WriteXmlFiles(xmlTxtWriter, fileComparisonFileParagraphUnit, isRef);

			xmlTxtWriter.WriteEndElement(); //refset

			xmlTxtWriter.WriteEndDocument();

			xmlTxtWriter.Flush();
			xmlTxtWriter.Close();

			return segmentDatas;
		}

		private class TERpIdMapping
		{
			/// <summary>
			/// Id required by TERp; natural sequence
			/// </summary>
			public int Id { get; set; }

			/// <summary>
			/// Id used to support split segments
			/// </summary>
			public string SegmentId { get; set; }
		}

		private static List<SegmentData> WriteXmlFiles(XmlWriter xmlTxtWriter, KeyValuePair<Comparison.Comparer.FileUnitProperties, Dictionary<string, Dictionary<string, Comparison.Comparer.ComparisonParagraphUnit>>>
				fileComparisonFileParagraphUnit, bool isRef)
		{

			int index = 1;

			var segmentDatas = new List<SegmentData>();

			foreach (var fileComparisonParagraphUnit in fileComparisonFileParagraphUnit.Value)
			{
				var innerFileName = Path.GetFileName(fileComparisonParagraphUnit.Key);

				xmlTxtWriter.WriteStartElement("doc");
				xmlTxtWriter.WriteAttributeString("sysid", isRef ? "ref" : "hyp");
				if (innerFileName != null)
				{
					xmlTxtWriter.WriteAttributeString("docid", innerFileName);
					xmlTxtWriter.WriteAttributeString("genre", "comparison");
					xmlTxtWriter.WriteAttributeString("origlang", fileComparisonFileParagraphUnit.Key.SourceLanguageIdUpdated);

					foreach (var comparisonParagraphUnit in fileComparisonParagraphUnit.Value.Values)
					{
						xmlTxtWriter.WriteStartElement("p");
						xmlTxtWriter.WriteAttributeString("id", comparisonParagraphUnit.ParagraphId);

						foreach (var comparisonSegmentUnit in comparisonParagraphUnit.ComparisonSegmentUnits)
						{
							var filtered = true;
							if (Processor.Settings.CalculateSummaryAnalysisBasedOnFilteredRows)
							{
								if ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
									&& (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
									&& (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
									&& (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments))
									filtered = false;

								if (filtered)
								{
									if (((!comparisonSegmentUnit.SegmentIsLocked || !Processor.Settings.ReportFilterLockedSegments) && comparisonSegmentUnit.SegmentIsLocked)
											|| !ReportUtils.IsFilterSegmentMatchPercentage(Processor.Settings.ReportFilterTranslationMatchValuesOriginal, Processor.Settings.ReportFilterTranslationMatchValuesUpdated, comparisonSegmentUnit.TranslationStatusOriginal, comparisonSegmentUnit.TranslationStatusUpdated)
											|| ((comparisonSegmentUnit.SegmentTextUpdated || !Processor.Settings.ReportFilterSegmentsWithNoChanges)
												&& (!Processor.Settings.ReportFilterChangedTargetContent || !comparisonSegmentUnit.SegmentTextUpdated)
												&& (!Processor.Settings.ReportFilterSegmentStatusChanged || !comparisonSegmentUnit.SegmentSegmentStatusUpdated)
												&& (!Processor.Settings.ReportFilterSegmentsContainingComments || !comparisonSegmentUnit.SegmentHasComments)))
										filtered = false;
								}
							}

							if (!filtered)
								continue;

							segmentDatas.Add(new SegmentData
							{
								FileName = innerFileName,
								ParagraphId = comparisonParagraphUnit.ParagraphId,
								SegmentId = comparisonSegmentUnit.SegmentId
							});

							xmlTxtWriter.WriteStartElement("seg");

							var mappingID = new TERpIdMapping { Id = index++, SegmentId = comparisonSegmentUnit.SegmentId };

							if (!_idMappings.Any(a => a.SegmentId == mappingID.SegmentId))
							{
								_idMappings.Add(mappingID);
							}

							xmlTxtWriter.WriteAttributeString("id", mappingID.Id.ToString());
							xmlTxtWriter.WriteAttributeString("segmentId", mappingID.SegmentId);

							WriteTargetContent(xmlTxtWriter,
								isRef ? comparisonSegmentUnit.TargetOriginal : comparisonSegmentUnit.TargetUpdated);
							xmlTxtWriter.WriteEndElement(); //seg
						}
						xmlTxtWriter.WriteEndElement(); //p
					}
				}

				xmlTxtWriter.WriteEndElement(); //doc
			}
			return segmentDatas;
		}

		private static void WriteTargetContent(XmlWriter xmlTxtWriter, IEnumerable<SegmentSection> segmentSections)
		{

			var strTemp = new StringBuilder();
			foreach (var section in segmentSections)
			{
				if (section.RevisionMarker != null && section.RevisionMarker.Type == RevisionMarker.RevisionType.Delete)
				{
					//ignore from the comparison process
				}
				else
				{
					strTemp.Append(section.Type == SegmentSection.ContentType.Text ? section.Content : " ");
				}
			}

			xmlTxtWriter.WriteString(ReportUtils.NormalizedWords(strTemp.ToString()));
		}



		public class DocumentResult
		{
			public string OriginalDocumentPath { get; set; }
			public string UpdatedDocumentPath { get; set; }

			public string HtmlPath { get; set; }
			public string ParamPath { get; set; }
			public string PraPath { get; set; }
			public string SumPath { get; set; }

			public List<SegmentData> SegmentDatas { get; set; }

		}

		public class SegmentData
		{
			public string FileName { get; set; }
			public string SegmentId { get; set; }
			public string ParagraphId { get; set; }
			public decimal Ins { get; set; }
			public decimal Del { get; set; }
			public decimal Sub { get; set; }
			public decimal Stem { get; set; }
			public decimal Syn { get; set; }
			public decimal Phrase { get; set; }
			public decimal Shft { get; set; }
			public decimal Wdsh { get; set; }
			public decimal NumEr { get; set; }
			public decimal NumWd { get; set; }
			public decimal Terp { get; set; }

			public SegmentData()
			{
				FileName = string.Empty;
				SegmentId = string.Empty;
				Ins = 0;
				Del = 0;
				Sub = 0;
				Stem = 0;
				Syn = 0;
				Phrase = 0;
				Shft = 0;
				Wdsh = 0;
				NumEr = 0;
				NumWd = 0;
				Terp = 0;
			}
		}

		public static string JavaPath { get; set; }
		private static string TerpPath { get; set; }


	}
}
