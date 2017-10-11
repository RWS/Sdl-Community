using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Sdl.Community.PostEdit.Compare.Core.Reports;
using Sdl.Community.PostEdit.Compare.DAL.ExcelTableModel;
using Sdl.Community.PostEdit.Compare.DAL.PostEditModificationsAnalysis;
using static Sdl.Community.PostEdit.Compare.Core.Reports.Report;

namespace Sdl.Community.PostEdit.Compare.Core.Helper
{
	public static class ExcelReportHelper
	{
		private static List<PEMModel> _pemTotal = new List<PEMModel>();
		private static List<TERpModel> _terpTotal=new List<TERpModel>();
		private  static PEMpAnalysisData _pemTotalValues=new PEMpAnalysisData();
		private static TERpAnalysisData _terpTotalValues = new TERpAnalysisData();
		public static void CreateReportHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			worksheet.Cells["A1"].Value = "Post-Edit Comparison Report" ;
			worksheet.Cells["A1:T1"].Merge = true;		
			worksheet.Cells["A1"].Style.Font.Size = 18;
			worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


			worksheet.Cells["A2"].Value = "Generated " + DateTime.Now;
			worksheet.Cells["A2:T2"].Merge = true;
			worksheet.Cells["A2"].Style.Font.Size = 10;
			worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

			xlPackage.Save();
		}

		public static void CreateExcelReport(string filePath,string sheetName)	
		{
			var newFile = new FileInfo(filePath);
			if (newFile.Exists)
			{
				newFile.Delete();  // ensures we create a new workbook
				newFile = new FileInfo(filePath);
			}
			using (var package = new ExcelPackage(newFile))
			{
				// Add a new worksheet to the empty workbook
				var worksheet = package.Workbook.Worksheets.Add(NormalizeWorksheetName(sheetName));
				package.Save();
			}

		}

		public static string NormalizeWorksheetName(string sheetName)
		{
			var count = sheetName.Count();
			var normalizedName = string.Empty;
			if (count > 30)
			{
				normalizedName = sheetName.Substring(0, 30);
				return normalizedName;
			}
			
			
			return sheetName;
		}

		public static ExcelPackage GetExcelPackage(string filePath)
		{
			var fileInfo = new FileInfo(filePath);
			var excelPackage = new ExcelPackage(fileInfo);
			return excelPackage;
		}


		public static ExcelWorksheet CreateWorksheetForReport(ExcelPackage xlPackage , string worksheetName)
		{
			var worksheet = xlPackage.Workbook.Worksheets.Add(worksheetName);

			return worksheet;
		}
		public static void TableTitle(ExcelPackage xlPackage, ExcelWorksheet worksheet, string title,int numberOfCellsToMerge)
		{
			var lastUsedRowIndex = worksheet.Dimension.End.Row;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Value = title;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Style.Font.Size = 14;
			worksheet.Cells[lastUsedRowIndex + 3, 1].Style.Font.Bold = true;
			worksheet.Cells[lastUsedRowIndex + 1, numberOfCellsToMerge].Merge = true;
		}
		public static void CreateTableHeader(ExcelPackage xlPackage, ExcelWorksheet worksheet,List<string> headerValues)
		{			
			int rowNr;
			var columnNr = 1;
			if (worksheet.Dimension == null)
			{
				rowNr = 1;
			}
			else
			{
				rowNr = worksheet.Dimension.End.Row + 2; // leave  1 empty row
			}

			foreach (var item in headerValues)
			{
				worksheet.Cells[rowNr, columnNr].Value = item;
				worksheet.Cells[rowNr, columnNr].AutoFitColumns();
				worksheet.Cells[rowNr, columnNr].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells[rowNr, columnNr].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
				worksheet.Cells[rowNr, columnNr].Style.Font.Color.SetColor(Color.White);
				worksheet.Cells[rowNr, columnNr].Style.Font.Bold = true;
				columnNr++;

			}
			xlPackage.Save();
		}

		public static void CreateFirstColumnValues(ExcelPackage xlPackage, ExcelWorksheet worksheet,List<string> firstColumnValues)
		{
			var rowIndex = worksheet.Dimension.End.Row + 1;

			foreach (var column in firstColumnValues)
			{
				worksheet.Cells[rowIndex, 1].Value = column;
				worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
				rowIndex++;
			}

			xlPackage.Save();
		}

		public static void SetTotalTablesValues(List<PEMModel>pemTotal,List<TERpModel> terpTotal)
		{
			if (pemTotal != null)
			{
				_pemTotal.AddRange(pemTotal);
			}
			if (terpTotal != null)
			{
				_terpTotal.AddRange(terpTotal);
			}
		}

		public static void ClearTotalValues()
		{
			_pemTotalValues = new PEMpAnalysisData();
			_terpTotalValues = new TERpAnalysisData();
		}

		public static void CreateTotalsTables(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			GeneratePemTotalTables(xlPackage, worksheet);
			GenerateTerpTotalTables(xlPackage, worksheet);

		}

		private static void GeneratePemTotalTables(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			//Insert row for table header
			var rowNr = 4;
			var columnNr = 1;
			worksheet.InsertRow(rowNr, columnNr);
			var headerValues = PemExcelReport.GetTableHeaderValues();
			foreach (var item in headerValues)
			{
				worksheet.Cells[rowNr, columnNr].Value = item;
				worksheet.Cells[rowNr, columnNr].AutoFitColumns();
				worksheet.Cells[rowNr, columnNr].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells[rowNr, columnNr].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
				worksheet.Cells[rowNr, columnNr].Style.Font.Color.SetColor(Color.White);
				worksheet.Cells[rowNr, columnNr].Style.Font.Bold = true;
				columnNr++;

			}

			var analysisBandCell =
								(from cell in worksheet.Cells
								 where cell.Value?.ToString() == Constants.AnalysisBand
								 select cell).First();
			var rowIndex = analysisBandCell.Start.Row + 1;
			var columnIndex = analysisBandCell.Start.Column + 1;
			worksheet.InsertRow(rowIndex, PemExcelReport.GetFirstColumnValues().Count);
			//Fill first column
			foreach (var column in PemExcelReport.GetFirstColumnValues())
			{				
				worksheet.Cells[rowIndex, 1].Value = column;
				worksheet.Cells[rowIndex, 1].Style.Font.Bold = true;
				rowIndex++;
			}


			var analyseResults = PemExcelReportHelper.CreatePemExcelDataModels(_pemTotalValues);

			//fill pem total table
			var matchValuesTest = new List<string>();
			var matchValue = string.Empty;
			for (var i = analysisBandCell.Start.Row + 1; i <= PemExcelReport.GetFirstColumnValues().Count + analysisBandCell.Start.Row; i++)
			{
				matchValue = worksheet.Cells[i, 1].Text;

				for (var j = columnIndex; j <= PemExcelReport.GetTableHeaderValues().Count + 1; j++)
				{
					var typeValue = worksheet.Cells[analysisBandCell.Start.Row, j].Text;
					var value = PemExcelReport.GetMatchingResultValue(matchValue, typeValue, analyseResults);

					worksheet.Cells[i, j].Value = value;
				}
			}

			xlPackage.Save();
			
		}
		private static void GenerateTerpTotalTables(ExcelPackage xlPackage, ExcelWorksheet worksheet)
		{
			var totalBandCell =
								(from cell in worksheet.Cells["a:a"]
								 where cell.Value?.ToString() == Constants.Total
								 select cell).First();
			//generate table header
		
			worksheet.InsertRow(totalBandCell.Start.Row+1, 2);

			var rowNr = totalBandCell.Start.Row + 2;
			var columnNr = totalBandCell.Start.Column;
			var headerValues = TerpExcelReport.GetTableHeaderValues();
			foreach (var item in headerValues)
			{
				worksheet.Cells[rowNr, columnNr].Value = item;
				worksheet.Cells[rowNr, columnNr].AutoFitColumns();
				worksheet.Cells[rowNr, columnNr].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells[rowNr, columnNr].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
				worksheet.Cells[rowNr, columnNr].Style.Font.Color.SetColor(Color.White);
				worksheet.Cells[rowNr, columnNr].Style.Font.Bold = true;
				columnNr++;

			}

			//fill table with total results

			var rangeCellQueryResult =
								(from cell in worksheet.Cells
								 where cell.Value?.ToString() == Constants.Range
								 select cell).First();

			worksheet.InsertRow(rangeCellQueryResult.Start.Row + 1, TerpExcelReport.GetFirstColumnValues().Count);
			var index = rangeCellQueryResult.Start.Row + 1;
			foreach (var column in TerpExcelReport.GetFirstColumnValues())
			{
				worksheet.Cells[index, 1].Value = column;
				worksheet.Cells[index, 1].Style.Font.Bold = true;
				index++;
			}
			var analyseResults = TerpExcelReportHelper.CreateTerpExcelDataModels(_terpTotalValues);

			if (rangeCellQueryResult != null)
			{
				var rangeCell = rangeCellQueryResult.ToList()[0];
				var rowIndex = rangeCell.Start.Row + 1;
				var columnIndex = rangeCell.Start.Column + 1;
				var matchValuesTest = new List<string>();
				var matchValue = string.Empty;
				for (var i = rowIndex; i <= TerpExcelReport.GetFirstColumnValues().Count + rangeCell.Start.Row; i++)
				{
					matchValue = worksheet.Cells[i, 1].Text;

					for (var j = columnIndex; j <= TerpExcelReport.GetTableHeaderValues().Count + 1; j++)
					{
						var typeValue = worksheet.Cells[rangeCell.Start.Row, j].Text;
						var value = TerpExcelReport.GetMatchingResultValue(matchValue, typeValue, analyseResults);

						worksheet.Cells[i, j].Value = value;
					}
				}

				xlPackage.Save();
			}


			xlPackage.Save();
		}


			internal static void PemAnalysisTotal(PEMpAnalysisData pempAnalysisData)
		{
			_pemTotalValues.exactCharacters = _pemTotalValues.exactCharacters + pempAnalysisData.exactCharacters;
			_pemTotalValues.exactPercent = _pemTotalValues.exactPercent + pempAnalysisData.exactPercent;
			_pemTotalValues.exactSegments = _pemTotalValues.exactSegments + pempAnalysisData.exactSegments;
			_pemTotalValues.exactTags = _pemTotalValues.exactTags + pempAnalysisData.exactTags;
			_pemTotalValues.exactWords = _pemTotalValues.exactWords + pempAnalysisData.exactWords;

			_pemTotalValues.fuzzy74Percent = _pemTotalValues.fuzzy74Percent + pempAnalysisData.fuzzy74Percent;
			_pemTotalValues.fuzzy74Characters = _pemTotalValues.fuzzy74Characters + pempAnalysisData.fuzzy74Characters;
			_pemTotalValues.fuzzy74Segments = _pemTotalValues.fuzzy74Segments + pempAnalysisData.fuzzy74Segments;
			_pemTotalValues.fuzzy74Tags = _pemTotalValues.fuzzy74Tags + pempAnalysisData.fuzzy74Tags;
			_pemTotalValues.fuzzy74Words = _pemTotalValues.fuzzy74Words + pempAnalysisData.fuzzy74Words;

			_pemTotalValues.fuzzy84Characters = _pemTotalValues.fuzzy84Characters + pempAnalysisData.fuzzy84Characters;
			_pemTotalValues.fuzzy84Percent = _pemTotalValues.fuzzy84Percent + pempAnalysisData.fuzzy84Percent;
			_pemTotalValues.fuzzy84Segments = _pemTotalValues.fuzzy84Segments + pempAnalysisData.fuzzy84Segments;
			_pemTotalValues.fuzzy84Words = _pemTotalValues.fuzzy84Words + pempAnalysisData.fuzzy84Words;
			_pemTotalValues.fuzzy84Tags = _pemTotalValues.fuzzy84Tags + pempAnalysisData.fuzzy84Tags;

			_pemTotalValues.fuzzy94Characters = _pemTotalValues.fuzzy94Characters + pempAnalysisData.fuzzy94Characters;
			_pemTotalValues.fuzzy94Percent = _pemTotalValues.fuzzy94Percent + pempAnalysisData.fuzzy94Percent;
			_pemTotalValues.fuzzy94Segments = _pemTotalValues.fuzzy94Segments + pempAnalysisData.fuzzy94Segments;
			_pemTotalValues.fuzzy94Tags = _pemTotalValues.fuzzy94Tags + pempAnalysisData.fuzzy94Tags;
			_pemTotalValues.fuzzy94Words = _pemTotalValues.fuzzy94Words = pempAnalysisData.fuzzy94Words;

			_pemTotalValues.fuzzy99Characters = _pemTotalValues.fuzzy99Characters + pempAnalysisData.fuzzy99Characters;
			_pemTotalValues.fuzzy99Percent = _pemTotalValues.fuzzy99Percent + pempAnalysisData.fuzzy99Percent;
			_pemTotalValues.fuzzy99Segments = _pemTotalValues.fuzzy99Segments + pempAnalysisData.fuzzy99Segments;
			_pemTotalValues.fuzzy99Tags = _pemTotalValues.fuzzy99Tags + pempAnalysisData.fuzzy99Tags;
			_pemTotalValues.fuzzy99Words = _pemTotalValues.fuzzy99Words + pempAnalysisData.fuzzy99Words;

			_pemTotalValues.newCharacters = _pemTotalValues.newCharacters + pempAnalysisData.newCharacters;
			_pemTotalValues.newPercent = _pemTotalValues.newPercent + pempAnalysisData.newPercent;
			_pemTotalValues.newSegments = _pemTotalValues.newSegments + pempAnalysisData.newSegments;
			_pemTotalValues.newTags = _pemTotalValues.newTags + pempAnalysisData.newTags;
			_pemTotalValues.newWords = _pemTotalValues.newWords + pempAnalysisData.newWords;

			_pemTotalValues.totalCharacters = _pemTotalValues.totalCharacters + pempAnalysisData.totalCharacters;
			_pemTotalValues.totalSegments = _pemTotalValues.totalSegments + pempAnalysisData.totalSegments;
			_pemTotalValues.totalPercent = _pemTotalValues.totalPercent + pempAnalysisData.totalPercent;
			_pemTotalValues.totalTags = _pemTotalValues.totalTags + pempAnalysisData.totalTags;
			_pemTotalValues.totalWords = _pemTotalValues.totalWords + pempAnalysisData.totalWords;


		}

		internal static void TerpAnalysisTotal(TERpAnalysisData terpAnalysisData)
		{
			_terpTotalValues.terp00Del = _terpTotalValues.terp00Del + terpAnalysisData.terp00Del;
			_terpTotalValues.terp00Ins = _terpTotalValues.terp00Ins + terpAnalysisData.terp00Ins;
			_terpTotalValues.terp00NumEr = _terpTotalValues.terp00NumEr + terpAnalysisData.terp00NumEr;
			_terpTotalValues.terp00NumWd = _terpTotalValues.terp00NumWd + terpAnalysisData.terp00NumWd;
			_terpTotalValues.terp00Segments = _terpTotalValues.terp00Segments + terpAnalysisData.terp00Segments;
			_terpTotalValues.terp00Shft = _terpTotalValues.terp00Shft + terpAnalysisData.terp00Shft;
			_terpTotalValues.terp00SrcWd = _terpTotalValues.terp00SrcWd + terpAnalysisData.terp00SrcWd;
			_terpTotalValues.terp00Sub = _terpTotalValues.terp00Sub + terpAnalysisData.terp00Sub;

			_terpTotalValues.terp01Del = _terpTotalValues.terp01Del + terpAnalysisData.terp01Del;
			_terpTotalValues.terp01Ins = _terpTotalValues.terp01Ins + terpAnalysisData.terp01Ins;
			_terpTotalValues.terp01NumEr = _terpTotalValues.terp01NumEr + terpAnalysisData.terp01NumEr;
			_terpTotalValues.terp01NumWd = _terpTotalValues.terp01NumWd + terpAnalysisData.terp01NumWd;
			_terpTotalValues.terp01Segments = _terpTotalValues.terp01Segments + terpAnalysisData.terp01Segments;
			_terpTotalValues.terp01Shft = _terpTotalValues.terp01Shft + terpAnalysisData.terp01Shft;
			_terpTotalValues.terp01SrcWd = _terpTotalValues.terp01SrcWd + terpAnalysisData.terp01SrcWd;
			_terpTotalValues.terp01Sub = _terpTotalValues.terp01Sub + terpAnalysisData.terp01Sub;

			_terpTotalValues.terp06Del = _terpTotalValues.terp06Del + terpAnalysisData.terp06Del;
			_terpTotalValues.terp06Ins = _terpTotalValues.terp06Ins + terpAnalysisData.terp06Ins;
			_terpTotalValues.terp06NumEr = _terpTotalValues.terp06NumEr + terpAnalysisData.terp06NumEr;
			_terpTotalValues.terp06NumWd = _terpTotalValues.terp06NumWd + terpAnalysisData.terp06NumWd;
			_terpTotalValues.terp06Segments = _terpTotalValues.terp06Segments + terpAnalysisData.terp06Segments;
			_terpTotalValues.terp06Shft = _terpTotalValues.terp06Shft + terpAnalysisData.terp06Shft;
			_terpTotalValues.terp06SrcWd = _terpTotalValues.terp06SrcWd + terpAnalysisData.terp06SrcWd;
			_terpTotalValues.terp06Sub = _terpTotalValues.terp06Sub + terpAnalysisData.terp06Sub;

			_terpTotalValues.terp10Del = _terpTotalValues.terp10Del + terpAnalysisData.terp10Del;
			_terpTotalValues.terp10Ins = _terpTotalValues.terp10Ins + terpAnalysisData.terp10Ins;
			_terpTotalValues.terp10NumEr = _terpTotalValues.terp10NumEr + terpAnalysisData.terp10NumEr;
			_terpTotalValues.terp10NumWd = _terpTotalValues.terp10NumWd + terpAnalysisData.terp10NumWd;
			_terpTotalValues.terp10Segments = _terpTotalValues.terp10Segments + terpAnalysisData.terp10Segments;
			_terpTotalValues.terp10Shft = _terpTotalValues.terp10Shft + terpAnalysisData.terp10Shft;
			_terpTotalValues.terp10SrcWd = _terpTotalValues.terp10SrcWd + terpAnalysisData.terp10SrcWd;
			_terpTotalValues.terp10Sub = _terpTotalValues.terp10Sub + terpAnalysisData.terp10Sub;

			_terpTotalValues.terp20Del = _terpTotalValues.terp20Del + terpAnalysisData.terp20Del;
			_terpTotalValues.terp20Ins = _terpTotalValues.terp20Ins + terpAnalysisData.terp20Ins;
			_terpTotalValues.terp20NumEr = _terpTotalValues.terp20NumEr + terpAnalysisData.terp20NumEr;
			_terpTotalValues.terp20NumWd = _terpTotalValues.terp20NumWd + terpAnalysisData.terp20NumWd;
			_terpTotalValues.terp20Segments = _terpTotalValues.terp20Segments + terpAnalysisData.terp20Segments;
			_terpTotalValues.terp20Shft = _terpTotalValues.terp20Shft + terpAnalysisData.terp20Shft;
			_terpTotalValues.terp20SrcWd = _terpTotalValues.terp20SrcWd + terpAnalysisData.terp20SrcWd;
			_terpTotalValues.terp20Sub = _terpTotalValues.terp20Sub + terpAnalysisData.terp20Sub;

			_terpTotalValues.terp30Del = _terpTotalValues.terp30Del + terpAnalysisData.terp30Del;
			_terpTotalValues.terp30Ins = _terpTotalValues.terp30Ins + terpAnalysisData.terp30Ins;
			_terpTotalValues.terp30NumEr = _terpTotalValues.terp30NumEr + terpAnalysisData.terp30NumEr;
			_terpTotalValues.terp30NumWd = _terpTotalValues.terp30NumWd + terpAnalysisData.terp30NumWd;
			_terpTotalValues.terp30Segments = _terpTotalValues.terp30Segments + terpAnalysisData.terp30Segments;
			_terpTotalValues.terp30Shft = _terpTotalValues.terp30Shft + terpAnalysisData.terp30Shft;
			_terpTotalValues.terp30SrcWd = _terpTotalValues.terp30SrcWd + terpAnalysisData.terp30SrcWd;
			_terpTotalValues.terp30Sub = _terpTotalValues.terp30Sub + terpAnalysisData.terp30Sub;

			_terpTotalValues.terp40Del = _terpTotalValues.terp40Del + terpAnalysisData.terp40Del;
			_terpTotalValues.terp40Ins = _terpTotalValues.terp40Ins + terpAnalysisData.terp40Ins;
			_terpTotalValues.terp40NumEr = _terpTotalValues.terp40NumEr + terpAnalysisData.terp40NumEr;
			_terpTotalValues.terp40NumWd = _terpTotalValues.terp40NumWd + terpAnalysisData.terp40NumWd;
			_terpTotalValues.terp40Segments = _terpTotalValues.terp40Segments + terpAnalysisData.terp40Segments;
			_terpTotalValues.terp40Shft = _terpTotalValues.terp40Shft + terpAnalysisData.terp40Shft;
			_terpTotalValues.terp40SrcWd = _terpTotalValues.terp40SrcWd + terpAnalysisData.terp40SrcWd;
			_terpTotalValues.terp40Sub = _terpTotalValues.terp40Sub + terpAnalysisData.terp40Sub;


			_terpTotalValues.terp50Del = _terpTotalValues.terp50Del + terpAnalysisData.terp50Del;
			_terpTotalValues.terp50Ins = _terpTotalValues.terp50Ins + terpAnalysisData.terp50Ins;
			_terpTotalValues.terp50NumEr = _terpTotalValues.terp50NumEr + terpAnalysisData.terp50NumEr;
			_terpTotalValues.terp50NumWd = _terpTotalValues.terp50NumWd + terpAnalysisData.terp50NumWd;
			_terpTotalValues.terp50Segments = _terpTotalValues.terp50Segments + terpAnalysisData.terp50Segments;
			_terpTotalValues.terp50Shft = _terpTotalValues.terp50Shft + terpAnalysisData.terp50Shft;
			_terpTotalValues.terp50SrcWd = _terpTotalValues.terp50SrcWd + terpAnalysisData.terp50SrcWd;
			_terpTotalValues.terp50Sub = _terpTotalValues.terp50Sub + terpAnalysisData.terp50Sub;

		}
	}
}
