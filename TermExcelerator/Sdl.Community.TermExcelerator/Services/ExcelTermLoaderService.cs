﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services.Interfaces;

namespace Sdl.Community.TermExcelerator.Services
{
	public class ExcelTermLoaderService : IExcelTermLoaderService
	{
		public static readonly Log Log = Log.Instance;
		private readonly ProviderSettings _providerSettings;

		public ExcelTermLoaderService(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings ?? throw new ArgumentNullException(nameof(providerSettings));
		}

		public async Task AddOrUpdateTerm(int entryId, ExcelTerm excelTerm)
		{
			if (!IsExcelFileWritable()) return;

			using (var excelPackage = new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
			{
				var workSheet = await GetTerminologyWorksheet(excelPackage);
				if (workSheet == null) return;
				AddTermToWorksheet(entryId, excelTerm, workSheet);
				excelPackage.Save();
			}
		}

		public async Task AddOrUpdateTerms(Dictionary<int, ExcelTerm> excelTerms)
		{
			if (!IsExcelFileWritable()) return;

			using (var excelPackage =
			   new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
			{
				var workSheet = await GetTerminologyWorksheet(excelPackage);
				if (workSheet == null) return;
				foreach (var excelTerm in excelTerms)
				{
					AddTermToWorksheet(excelTerm.Key, excelTerm.Value, workSheet);
				}
				excelPackage.Save();
			}
		}

		public async Task DeleteTerm(int id)
		{
			if (!IsExcelFileWritable()) return;

			using (var excelPackage = new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
			{
				var workSheet = await GetTerminologyWorksheet(excelPackage);
				if (workSheet == null) return;

				workSheet.DeleteRow(id);
				excelPackage.Save();
			}
		}

		public Task<ExcelWorksheet> GetTerminologyWorksheet(ExcelPackage excelPackage)
		{
			if (excelPackage.Workbook.Worksheets == null) return null;
			if (excelPackage.Workbook.Worksheets.Count == 0) return null;

			if (string.IsNullOrEmpty(_providerSettings.WorksheetName))
			{
				return Task.FromResult(excelPackage.Workbook.Worksheets.FirstOrDefault());
			}

			return Task.FromResult(excelPackage.Workbook.Worksheets
				.FirstOrDefault(x => x.Name.Equals(_providerSettings.WorksheetName, StringComparison.InvariantCultureIgnoreCase)));
		}

		public async Task<Dictionary<int, ExcelTerm>> GetTermsFromExcel(ExcelWorksheet worksheet)
		{
			var result = new Dictionary<int, ExcelTerm>();
			try
			{
				if (worksheet == null) return result;
				await Task.Run(() =>
				{
					if (worksheet.Dimension?.Address == null) return;
					foreach (var cell in worksheet.Cells[worksheet.Dimension?.Address])
					{
						var excellCellAddress = new ExcelCellAddress(cell.Address);

						if (_providerSettings.HasHeader && excellCellAddress.Row == 1)
						{
							continue;
						}
						var id = excellCellAddress.Row;
						if (!result.ContainsKey(id))
						{
							result[id] = new ExcelTerm();
						}
						SetCellValue(result[id], cell, excellCellAddress.Column);
					}
				});
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"GetTermsFromExcel method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return result;
		}

		public async Task<Dictionary<int, ExcelTerm>> LoadTerms()
		{
			var result = new Dictionary<int, ExcelTerm>();
			try
			{
				using (var excelPackage = new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
				{
					var workSheet = await GetTerminologyWorksheet(excelPackage);
					result = await GetTermsFromExcel(workSheet);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"LoadTerms method: {ex.Message}\n {ex.StackTrace}");
				throw ex;
			}
			return result;
		}

		private void AddTermToWorksheet(int entryId, ExcelTerm excelTerm, ExcelWorksheet workSheet)
		{
			var sourceColumnAddress = $"{_providerSettings.SourceColumn}{entryId}";
			var targetColumnAddress = $"{_providerSettings.TargetColumn}{entryId}";

			workSheet.SetValue(sourceColumnAddress, excelTerm.Source);
			workSheet.SetValue(targetColumnAddress, excelTerm.Target);
			if (!string.IsNullOrEmpty(_providerSettings.ApprovedColumn))
			{
				var approvedColumnAddress = $"{_providerSettings.ApprovedColumn}{entryId}";
				workSheet.Cells[approvedColumnAddress].Value = excelTerm.Approved;
			}
		}

		private bool IsExcelFileWritable(bool onlyReadOnly = false)
		{
			if (_providerSettings.IsReadOnly)
			{
				MessageBox.Show(@"Terminology Provider is configured as read only!", @"Read Only", MessageBoxButtons.OK);
				return false;
			}
			if (!onlyReadOnly && !_providerSettings.IsFileReady())
			{
				MessageBox.Show(
					@"The excel file configured as a terminology provider appears to be also opened in the Excel application. Please close the file!",
					@"Excel file is used by another process",
					MessageBoxButtons.OK);
				return false;
			}

			return true;
		}

		private void SetCellValue(ExcelTerm excelTerm, ExcelRangeBase cell, int columnIndex)
		{
			var columnLetter = ExcelCellAddress.GetColumnLetter(columnIndex);
			if (columnLetter == _providerSettings.SourceColumn)
			{
				excelTerm.Source = cell.Text;
				excelTerm.SourceCulture = _providerSettings.SourceLanguage;
			}
			if (columnLetter == _providerSettings.TargetColumn.ToUpper())
			{
				excelTerm.Target = cell.Text;
				excelTerm.TargetCulture = _providerSettings.TargetLanguage;
			}
			if (columnLetter == _providerSettings.ApprovedColumn?.ToUpper())
			{
				excelTerm.Approved = cell.Text;
			}
		}
	}
}