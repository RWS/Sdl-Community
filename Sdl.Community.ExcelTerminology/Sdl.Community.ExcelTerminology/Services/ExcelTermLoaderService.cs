using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Sdl.Community.ExcelTerminology.Model;
using Sdl.Community.ExcelTerminology.Services.Interfaces;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.ExcelTerminology.Services
{
    public  class ExcelTermLoaderService : IExcelTermLoaderService
    {
        private readonly ProviderSettings _providerSettings;

        public ExcelTermLoaderService(ProviderSettings providerSettings)
        {
            _providerSettings = providerSettings;
        }

        public async Task<Dictionary<int, ExcelTerm>> LoadTerms()
        {
            using (var excelPackage =
                new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
            {
                var workSheet = await GetTerminologyWorksheet(excelPackage);
                return await GetTermsFromExcel(workSheet);
            }

        }

        public async Task AddOrUpdateTerm(int entryId,ExcelTerm excelTerm)
        {
            using (var excelPackage =
               new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
            {
                var workSheet = await GetTerminologyWorksheet(excelPackage);
                var sourceColumnAddress = $"{_providerSettings.SourceColumn}{entryId}";
                var targetColumnAddress = $"{_providerSettings.TargetColumn}{entryId}";

                workSheet.SetValue(sourceColumnAddress,excelTerm.Source);
                workSheet.SetValue(targetColumnAddress, excelTerm.Target);
                if (!string.IsNullOrEmpty(_providerSettings.ApprovedColumn))
                {
                    var approvedColumnAddress = $"{_providerSettings.ApprovedColumn}{entryId}";
                    workSheet.Cells[approvedColumnAddress].Value = excelTerm.Approved;
                }
                excelPackage.Save();
            }
        }

        public async Task DeleteTerm(int id)
        {
            using (var excelPackage =
               new ExcelPackage(new FileInfo(_providerSettings.TermFilePath)))
            {
                var workSheet = await GetTerminologyWorksheet(excelPackage);

                workSheet.DeleteRow(id);
                excelPackage.Save();
            }
        }

        public async Task<ExcelWorksheet> GetTerminologyWorksheet(ExcelPackage excelPackage)
        {
            var worksheet = string.IsNullOrEmpty(_providerSettings.WorksheetName)
                ? excelPackage.Workbook.Worksheets[1]
                : excelPackage.Workbook.Worksheets[_providerSettings.WorksheetName];

            return worksheet;
        }

        public async Task<Dictionary<int, ExcelTerm>> GetTermsFromExcel(ExcelWorksheet worksheet)
        {
            var result = new Dictionary<int, ExcelTerm>();

            var excelRangeAddress = _providerSettings.GetExcelRangeAddress();

            foreach (var cell in worksheet.Cells[excelRangeAddress])
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
            return result;
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

            if (columnLetter == _providerSettings.ApprovedColumn.ToUpper())
            {
                excelTerm.Approved = cell.Text;
            }
        }

       
    }
}
